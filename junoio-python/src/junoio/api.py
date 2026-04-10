from __future__ import annotations

import inspect
import json
import math
from collections import deque
from dataclasses import fields
from collections.abc import Callable, Iterable, Iterator, Mapping

import numpy as np

from .errors import JunoCraftAmbiguityError, JunoRpcError
from .models import (
    Snapshot,
    TelemetryBodyRef,
    TelemetryEnvelope,
    TelemetryEvent,
    TelemetryTarget,
    TelemetryVector3,
)


def _clamp(value: float, minimum: float, maximum: float) -> float:
    if not math.isfinite(value):
        return minimum

    return max(minimum, min(maximum, value))


def _normalize_craft_id(craft_id: str | int) -> str:
    if isinstance(craft_id, int):
        return f"craft_{craft_id}"

    text = str(craft_id).strip()
    if text.startswith("craft_"):
        return text

    if text.isdigit():
        return f"craft_{text}"

    return text


_SNAPSHOT_FIELD_NAMES = {field.name for field in fields(Snapshot)}
_BASE_VECTOR_ALIASES = {
    field.name[:-2]: (field.name, f"{field.name[:-2]}_y", f"{field.name[:-2]}_z")
    for field in fields(Snapshot)
    if field.name.endswith("_x")
    and any(candidate.name == f"{field.name[:-2]}_y" for candidate in fields(Snapshot))
    and any(candidate.name == f"{field.name[:-2]}_z" for candidate in fields(Snapshot))
}
_COLLIDING_VECTOR_ALIASES = {
    alias: components
    for alias, components in _BASE_VECTOR_ALIASES.items()
    if alias in _SNAPSHOT_FIELD_NAMES
}
_STATE_VECTOR_ALIASES = {
    **{
        alias: components
        for alias, components in _BASE_VECTOR_ALIASES.items()
        if alias not in {"velocity", "surface_velocity", "target_velocity"}
        and alias not in _SNAPSHOT_FIELD_NAMES
    },
    "velocity_orbit": _BASE_VECTOR_ALIASES["velocity"],
    "velocity_surface": _BASE_VECTOR_ALIASES["surface_velocity"],
    "velocity_target": _BASE_VECTOR_ALIASES["target_velocity"],
    **{
        f"{alias}_vector": components
        for alias, components in _COLLIDING_VECTOR_ALIASES.items()
    },
}


class _WaitForSeconds:
    __slots__ = ("seconds", "_start_time", "_start_seq")

    def __init__(self, seconds: float) -> None:
        self.seconds = max(0.0, float(seconds))
        self._start_time: float | None = None
        self._start_seq: int | None = None

    def ready(self, program: "JunoProgram") -> bool:
        current_time = float(program.juno.state.time)
        current_seq = int(program.juno.state.seq)
        if self._start_time is None:
            self._start_time = current_time
            self._start_seq = current_seq
            return False

        if self.seconds == 0.0:
            return self._start_seq is not None and current_seq > self._start_seq

        return current_time - self._start_time >= self.seconds


class _WaitUntil:
    __slots__ = ("condition",)

    def __init__(self, condition: Callable[..., object]) -> None:
        self.condition = condition

    def ready(self, program: "JunoProgram") -> bool:
        return program._evaluate_condition(self.condition)


class StateView:
    __slots__ = ("_snapshot",)

    def __init__(self) -> None:
        self._snapshot = Snapshot()

    def _replace(self, snapshot: Snapshot) -> None:
        self._snapshot = snapshot

    def __getattr__(self, name: str) -> object:
        if name in _SNAPSHOT_FIELD_NAMES:
            return getattr(self._snapshot, name)

        components = _STATE_VECTOR_ALIASES.get(name)
        if components is not None:
            return np.array(
                [float(getattr(self._snapshot, component)) for component in components],
                dtype=float,
            )

        return getattr(self._snapshot, name)


def _vector3_to_array(vector: TelemetryVector3 | None) -> np.ndarray | None:
    if vector is None:
        return None

    return np.array([float(vector.x), float(vector.y), float(vector.z)], dtype=float)


class BodyRefView:
    __slots__ = ("body_id", "name")

    def __init__(self, body_ref: TelemetryBodyRef) -> None:
        self.body_id = str(body_ref.body_id)
        self.name = str(body_ref.name)

    def __repr__(self) -> str:
        return f"BodyRefView(body_id={self.body_id!r}, name={self.name!r})"


class BodyView:
    __slots__ = ("_body",)

    def __init__(self, body: dict[str, object]) -> None:
        self._body = body

    @property
    def body_id(self) -> str:
        return str(self._body.get("body_id", ""))

    @property
    def name(self) -> str:
        return str(self._body.get("name", ""))

    @property
    def parent_body(self) -> BodyRefView | None:
        body_id = self._body.get("parent_body_id")
        name = self._body.get("parent_body_name")
        if not isinstance(body_id, str) and not isinstance(name, str):
            return None
        return BodyRefView(
            TelemetryBodyRef(
                body_id=str(body_id or ""),
                name=str(name or ""),
            )
        )

    @property
    def child_body_ids(self) -> list[str]:
        values = self._body.get("child_body_ids", [])
        if not isinstance(values, list):
            return []
        return [str(value) for value in values if isinstance(value, str)]

    @property
    def craft_ids(self) -> list[str]:
        values = self._body.get("craft_ids", [])
        if not isinstance(values, list):
            return []
        return [str(value) for value in values if isinstance(value, str)]

    @property
    def mass(self) -> float | None:
        value = self._body.get("mass")
        return float(value) if isinstance(value, (int, float)) else None

    @property
    def radius(self) -> float | None:
        value = self._body.get("radius")
        return float(value) if isinstance(value, (int, float)) else None

    @property
    def surface_gravity(self) -> float | None:
        value = self._body.get("surface_gravity")
        return float(value) if isinstance(value, (int, float)) else None

    @property
    def sphere_of_influence(self) -> float | None:
        value = self._body.get("sphere_of_influence")
        return float(value) if isinstance(value, (int, float)) else None

    @property
    def angular_velocity(self) -> float | None:
        value = self._body.get("angular_velocity")
        return float(value) if isinstance(value, (int, float)) else None

    @property
    def has_water(self) -> bool:
        return bool(self._body.get("has_water", False))

    @property
    def has_terrain_physics(self) -> bool:
        return bool(self._body.get("has_terrain_physics", False))

    @property
    def orbit(self) -> dict[str, float | None]:
        keys = (
            "apoapsis",
            "periapsis",
            "period",
            "inclination_rad",
            "eccentricity",
            "semi_major_axis",
            "semi_minor_axis",
            "true_anomaly",
            "mean_anomaly",
            "periapsis_argument",
            "right_ascension",
        )
        orbit: dict[str, float | None] = {}
        for key in keys:
            value = self._body.get(f"orbit_{key}")
            orbit[key] = float(value) if isinstance(value, (int, float)) else None
        return orbit

    def __repr__(self) -> str:
        return f"BodyView(body_id={self.body_id!r}, name={self.name!r})"


class PartView:
    __slots__ = ("_facade", "_part")

    def __init__(self, facade: "JunoFacade", part: dict[str, object]) -> None:
        self._facade = facade
        self._part = part

    @property
    def part_id(self) -> int:
        return int(self._part.get("part_id", 0))

    @property
    def name(self) -> str:
        return str(self._part.get("name", ""))

    @property
    def craft_id(self) -> str:
        return str(self._part.get("craft_id", ""))

    @property
    def craft_name(self) -> str:
        return str(self._part.get("craft_name", ""))

    @property
    def part_type(self) -> str:
        return str(self._part.get("part_type", ""))

    @property
    def mass(self) -> float | None:
        value = self._part.get("mass")
        return float(value) if isinstance(value, (int, float)) else None

    @property
    def drag_scale(self) -> float | None:
        value = self._part.get("drag_scale")
        return float(value) if isinstance(value, (int, float)) else None

    @property
    def temperature(self) -> float | None:
        value = self._part.get("temperature")
        return float(value) if isinstance(value, (int, float)) else None

    @property
    def activated(self) -> bool:
        return bool(self._part.get("activated", False))

    @property
    def enabled(self) -> bool:
        return bool(self._part.get("enabled", False))

    @property
    def destroyed(self) -> bool:
        return bool(self._part.get("destroyed", False))

    @property
    def position(self) -> np.ndarray | None:
        value = self._part.get("position")
        if not isinstance(value, dict):
            return None
        if not all(isinstance(value.get(axis), (int, float)) for axis in ("x", "y", "z")):
            return None
        return np.array([float(value["x"]), float(value["y"]), float(value["z"])], dtype=float)

    @property
    def local_position(self) -> np.ndarray | None:
        value = self._part.get("local_position")
        if not isinstance(value, dict):
            return None
        if not all(isinstance(value.get(axis), (int, float)) for axis in ("x", "y", "z")):
            return None
        return np.array([float(value["x"]), float(value["y"]), float(value["z"])], dtype=float)

    def local_to_pci(self, vector: Iterable[float]) -> np.ndarray:
        return self._facade._rpc_part_vector_result(
            "part_local_to_pci",
            self.part_id,
            vector,
            self.craft_id,
        )

    def pci_to_local(self, vector: Iterable[float]) -> np.ndarray:
        return self._facade._rpc_part_vector_result(
            "part_pci_to_local",
            self.part_id,
            vector,
            self.craft_id,
        )

    def __repr__(self) -> str:
        return (
            f"PartView(part_id={self.part_id!r}, name={self.name!r}, "
            f"craft_id={self.craft_id!r})"
        )


class PartsCollectionView(Mapping[int, PartView]):
    __slots__ = ("_facade", "_craft_id")

    def __init__(self, facade: "JunoFacade", craft_id: str | None = None) -> None:
        self._facade = facade
        self._craft_id = craft_id

    def __getitem__(self, part_id: int) -> PartView:
        part = self._facade.get_part(part_id, craft_id=self._craft_id)
        if part is None:
            raise KeyError(part_id)
        return part

    def __iter__(self) -> Iterator[int]:
        return iter(part.part_id for part in self._facade.list_parts(craft_id=self._craft_id))

    def __len__(self) -> int:
        return len(self._facade.list_parts(craft_id=self._craft_id))


class RaycastHitView:
    __slots__ = ("_facade", "_hit")

    def __init__(self, facade: "JunoFacade", hit: dict[str, object]) -> None:
        self._facade = facade
        self._hit = hit

    @property
    def hit_type(self) -> str:
        return str(self._hit.get("hit_type", "unknown"))

    @property
    def distance(self) -> float:
        value = self._hit.get("distance", 0.0)
        return float(value) if isinstance(value, (int, float)) else 0.0

    @property
    def point(self) -> np.ndarray:
        value = self._hit.get("point", {})
        return np.array(
            [float(value.get("x", 0.0)), float(value.get("y", 0.0)), float(value.get("z", 0.0))],
            dtype=float,
        )

    @property
    def normal(self) -> np.ndarray:
        value = self._hit.get("normal", {})
        return np.array(
            [float(value.get("x", 0.0)), float(value.get("y", 0.0)), float(value.get("z", 0.0))],
            dtype=float,
        )

    @property
    def part(self) -> PartView | None:
        part = self._hit.get("part")
        return PartView(self._facade, part) if isinstance(part, dict) else None

    def __repr__(self) -> str:
        return f"RaycastHitView(hit_type={self.hit_type!r}, distance={self.distance!r})"


class BodiesCollectionView(Mapping[str, BodyView]):
    __slots__ = ("_facade",)

    def __init__(self, facade: "JunoFacade") -> None:
        self._facade = facade

    def __getitem__(self, body_id: str) -> BodyView:
        body = self._facade.get_body(body_id)
        if body is None:
            raise KeyError(body_id)
        return body

    def __iter__(self) -> Iterator[str]:
        return iter(self._facade._body_cache())

    def __len__(self) -> int:
        return len(self._facade._body_cache())


class WorldFacade:
    __slots__ = ("_facade",)

    def __init__(self, facade: "JunoFacade") -> None:
        self._facade = facade

    def to_lat_lon_agl(
        self, position: Iterable[float], body: str | None = None
    ) -> np.ndarray:
        return self._facade._rpc_world_vector_result(
            "world_to_lat_lon_agl",
            position=position,
            body=body,
        )

    def to_lat_lon_asl(
        self, position: Iterable[float], body: str | None = None
    ) -> np.ndarray:
        return self._facade._rpc_world_vector_result(
            "world_to_lat_lon_asl",
            position=position,
            body=body,
        )

    def to_position(
        self,
        lat_deg: float,
        lon_deg: float,
        altitude: float,
        body: str | None = None,
        reference: str = "agl",
    ) -> np.ndarray:
        normalized_reference = str(reference).strip().lower()
        if normalized_reference not in {"agl", "asl"}:
            raise ValueError("reference must be 'agl' or 'asl'.")
        return self._facade._rpc_world_vector_result(
            "world_to_position",
            lat_deg=float(lat_deg),
            lon_deg=float(lon_deg),
            altitude=float(altitude),
            body=body,
            reference=normalized_reference,
        )

    def get_terrain_height(
        self,
        lat_deg: float,
        lon_deg: float,
        body: str | None = None,
    ) -> float | None:
        result = self._facade._rpc_call(
            "world_get_terrain_height",
            {
                "lat_deg": float(lat_deg),
                "lon_deg": float(lon_deg),
                "body": None if body is None else str(body),
            },
        )
        value = result.get("height") if isinstance(result, dict) else None
        return float(value) if isinstance(value, (int, float)) else None

    def cast_ray(
        self,
        origin: Iterable[float],
        direction: Iterable[float],
        max_distance: float | None = None,
    ) -> "RaycastHitView | None":
        result = self._facade._rpc_call(
            "world_cast_ray",
            {
                "origin": self._facade._vector_payload(origin, argument_name="origin"),
                "direction": self._facade._vector_payload(direction, argument_name="direction"),
                "max_distance": None if max_distance is None else float(max_distance),
            },
        )
        hit = result.get("hit") if isinstance(result, dict) else None
        return RaycastHitView(self._facade, hit) if isinstance(hit, dict) else None


class BaseTargetView:
    __slots__ = ("_facade", "_target")

    def __init__(self, facade: "JunoFacade", target: TelemetryTarget) -> None:
        self._facade = facade
        self._target = target

    @property
    def target_type(self) -> str:
        return self._target.target_type

    @property
    def target_id(self) -> str:
        return self._target.target_id

    @property
    def name(self) -> str:
        return self._target.name

    @property
    def parent_body(self) -> BodyRefView | None:
        parent = self._target.parent_body
        return None if parent is None else BodyRefView(parent)

    @property
    def position(self) -> np.ndarray | None:
        return _vector3_to_array(self._target.position)

    @property
    def velocity(self) -> np.ndarray | None:
        return _vector3_to_array(self._target.velocity)

    def __repr__(self) -> str:
        return (
            f"{self.__class__.__name__}(target_type={self.target_type!r}, "
            f"target_id={self.target_id!r}, name={self.name!r})"
        )


class CraftTargetView(BaseTargetView):
    __slots__ = ()

    @property
    def craft_id(self) -> str | None:
        return self._target.craft_id or self._target.target_id or None

    @property
    def craft(self) -> "CraftView | None":
        craft_id = self.craft_id
        if not craft_id:
            return None
        return self._facade.get_craft(craft_id)

    @property
    def state(self) -> StateView | None:
        craft = self.craft
        return None if craft is None else craft.state


class BodyTargetView(BaseTargetView):
    __slots__ = ()

    @property
    def body_id(self) -> str | None:
        return self._target.body_id or self._target.target_id or None

    @property
    def radius(self) -> float | None:
        return self._target.radius

    @property
    def mass(self) -> float | None:
        return self._target.mass

    @property
    def mu(self) -> float | None:
        return self._target.mu

    @property
    def angular_velocity(self) -> float | None:
        return self._target.angular_velocity


class LandmarkTargetView(BaseTargetView):
    __slots__ = ()

    @property
    def landmark_id(self) -> str | None:
        return self._target.landmark_id or self._target.target_id or None

    @property
    def latitude_deg(self) -> float | None:
        return self._target.latitude_deg

    @property
    def longitude_deg(self) -> float | None:
        return self._target.longitude_deg


class PositionTargetView(BaseTargetView):
    __slots__ = ()


class PartTargetView(BaseTargetView):
    __slots__ = ()

    @property
    def part_id(self) -> int | None:
        return self._target.part_id

    @property
    def part_name(self) -> str | None:
        return self._target.part_name

    @property
    def parent_craft_id(self) -> str | None:
        return self._target.parent_craft_id

    @property
    def parent_craft_name(self) -> str | None:
        return self._target.parent_craft_name


class BaseEventView:
    __slots__ = ("_event",)

    def __init__(self, event: TelemetryEvent) -> None:
        self._event = event

    @property
    def event_type(self) -> str:
        return self._event.event_type

    @property
    def event_id(self) -> str:
        return self._event.event_id

    @property
    def time(self) -> float:
        return self._event.time

    @property
    def craft_id(self) -> str | None:
        return self._event.craft_id

    @property
    def craft_name(self) -> str | None:
        return self._event.craft_name

    @property
    def parent_body(self) -> BodyRefView | None:
        parent = self._event.parent_body
        return None if parent is None else BodyRefView(parent)

    def __repr__(self) -> str:
        return (
            f"{self.__class__.__name__}(event_type={self.event_type!r}, "
            f"event_id={self.event_id!r})"
        )


class CollisionEventView(BaseEventView):
    @property
    def part_id(self) -> int | None:
        return self._event.part_id

    @property
    def part_name(self) -> str | None:
        return self._event.part_name

    @property
    def other_part_id(self) -> int | None:
        return self._event.other_part_id

    @property
    def other_part_name(self) -> str | None:
        return self._event.other_part_name

    @property
    def other_craft_id(self) -> str | None:
        return self._event.other_craft_id

    @property
    def other_craft_name(self) -> str | None:
        return self._event.other_craft_name

    @property
    def relative_velocity(self) -> float | None:
        return self._event.relative_velocity

    @property
    def impulse(self) -> float | None:
        return self._event.impulse

    @property
    def is_ground_collision(self) -> bool | None:
        return self._event.is_ground_collision

    @property
    def point(self) -> np.ndarray | None:
        return _vector3_to_array(self._event.point)

    @property
    def normal(self) -> np.ndarray | None:
        return _vector3_to_array(self._event.normal)


class PartExplodedEventView(BaseEventView):
    @property
    def part_id(self) -> int | None:
        return self._event.part_id

    @property
    def part_name(self) -> str | None:
        return self._event.part_name


class CraftDockedEventView(BaseEventView):
    @property
    def other_craft_id(self) -> str | None:
        return self._event.other_craft_id

    @property
    def other_craft_name(self) -> str | None:
        return self._event.other_craft_name


class EnteredSoiEventView(BaseEventView):
    @property
    def new_parent_body(self) -> BodyRefView | None:
        parent = self._event.new_parent_body
        return None if parent is None else BodyRefView(parent)


class MessageReceivedEventView(BaseEventView):
    @property
    def message(self) -> str | None:
        return self._event.message_name

    @property
    def data(self) -> object:
        return self._event.message_data

    @property
    def sender_craft_id(self) -> str | None:
        return self._event.sender_craft_id

    @property
    def sender_craft_name(self) -> str | None:
        return self._event.sender_craft_name

    @property
    def delivery_scope(self) -> str | None:
        return self._event.delivery_scope


def _build_event_view(event: TelemetryEvent) -> BaseEventView:
    if event.event_type == "collision":
        return CollisionEventView(event)
    if event.event_type == "part_exploded":
        return PartExplodedEventView(event)
    if event.event_type == "craft_docked":
        return CraftDockedEventView(event)
    if event.event_type == "entered_soi":
        return EnteredSoiEventView(event)
    if event.event_type == "message_received":
        return MessageReceivedEventView(event)
    return BaseEventView(event)


class EventsView:
    __slots__ = ("_facade",)

    def __init__(self, facade: "JunoFacade") -> None:
        self._facade = facade

    def poll(self) -> list[BaseEventView]:
        return self._facade._drain_events()


class MessagesView:
    __slots__ = ("_facade",)

    def __init__(self, facade: "JunoFacade") -> None:
        self._facade = facade

    def poll(self) -> list[MessageReceivedEventView]:
        return self._facade._drain_messages()


def _build_target_view(
    facade: "JunoFacade", target: TelemetryTarget | None
) -> BaseTargetView | None:
    if target is None:
        return None

    target_type = str(target.target_type)
    if target_type == "craft":
        return CraftTargetView(facade, target)
    if target_type == "body":
        return BodyTargetView(facade, target)
    if target_type == "landmark":
        return LandmarkTargetView(facade, target)
    if target_type == "part":
        return PartTargetView(facade, target)
    if target_type == "position":
        return PositionTargetView(facade, target)

    return BaseTargetView(facade, target)


class CraftView:
    __slots__ = ("_facade", "craft_id", "name", "_is_active", "_state_view", "parts")

    def __init__(
        self,
        facade: "JunoFacade",
        craft_id: str,
        name: str,
        is_active: bool = False,
        state_view: StateView | None = None,
    ) -> None:
        self._facade = facade
        self.craft_id = _normalize_craft_id(craft_id)
        self.name = str(name)
        self._is_active = bool(is_active)
        self._state_view = state_view
        self.parts = PartsCollectionView(facade, self.craft_id)

    @property
    def is_active(self) -> bool:
        return self._is_active

    @property
    def state(self) -> StateView | None:
        if self._state_view is None and not self._is_active:
            self._facade._ensure_tracked(self.craft_id)
        return self._state_view

    def __repr__(self) -> str:
        return (
            f"CraftView(craft_id={self.craft_id!r}, name={self.name!r}, "
            f"is_active={self._is_active!r})"
        )


class CraftCollectionView(Mapping[str, CraftView]):
    __slots__ = ("_facade",)

    def __init__(self, facade: "JunoFacade") -> None:
        self._facade = facade

    def __getitem__(self, craft_id: str) -> CraftView:
        self._facade._discover_crafts()
        normalized = _normalize_craft_id(craft_id)
        return self._facade._craft_views[normalized]

    def __iter__(self) -> Iterator[str]:
        self._facade._discover_crafts()
        return iter(self._facade._craft_views)

    def __len__(self) -> int:
        self._facade._discover_crafts()
        return len(self._facade._craft_views)


class CraftsByNameCollectionView(Mapping[str, list[CraftView]]):
    __slots__ = ("_facade",)

    def __init__(self, facade: "JunoFacade") -> None:
        self._facade = facade

    def __getitem__(self, name: str) -> list[CraftView]:
        self._facade._discover_crafts()
        return list(self._facade._crafts_by_name().get(str(name), []))

    def __iter__(self) -> Iterator[str]:
        self._facade._discover_crafts()
        return iter(self._facade._crafts_by_name())

    def __len__(self) -> int:
        self._facade._discover_crafts()
        return len(self._facade._crafts_by_name())


class ControlView:
    __slots__ = (
        "_throttle",
        "_pitch",
        "_roll",
        "_yaw",
        "_brake",
        "_translate_forward",
        "_translate_right",
        "_translate_up",
        "_slider_1",
        "_slider_2",
        "_slider_3",
        "_slider_4",
        "_translation_mode_enabled",
        "_throttle_set",
        "_pitch_set",
        "_roll_set",
        "_yaw_set",
        "_brake_set",
        "_translate_forward_set",
        "_translate_right_set",
        "_translate_up_set",
        "_slider_1_set",
        "_slider_2_set",
        "_slider_3_set",
        "_slider_4_set",
        "_translation_mode_enabled_set",
    )

    def __init__(self) -> None:
        self.reset()

    def reset(self) -> None:
        self._throttle = 0.0
        self._pitch = 0.0
        self._roll = 0.0
        self._yaw = 0.0
        self._brake = 0.0
        self._translate_forward = 0.0
        self._translate_right = 0.0
        self._translate_up = 0.0
        self._slider_1 = 0.0
        self._slider_2 = 0.0
        self._slider_3 = 0.0
        self._slider_4 = 0.0
        self._translation_mode_enabled = False
        self._throttle_set = False
        self._pitch_set = False
        self._roll_set = False
        self._yaw_set = False
        self._brake_set = False
        self._translate_forward_set = False
        self._translate_right_set = False
        self._translate_up_set = False
        self._slider_1_set = False
        self._slider_2_set = False
        self._slider_3_set = False
        self._slider_4_set = False
        self._translation_mode_enabled_set = False

    @property
    def throttle(self) -> float:
        return self._throttle

    @throttle.setter
    def throttle(self, value: float) -> None:
        self._throttle = _clamp(float(value), 0.0, 1.0)
        self._throttle_set = True

    @property
    def pitch(self) -> float:
        return self._pitch

    @pitch.setter
    def pitch(self, value: float) -> None:
        self._pitch = _clamp(float(value), -1.0, 1.0)
        self._pitch_set = True

    @property
    def roll(self) -> float:
        return self._roll

    @roll.setter
    def roll(self, value: float) -> None:
        self._roll = _clamp(float(value), -1.0, 1.0)
        self._roll_set = True

    @property
    def yaw(self) -> float:
        return self._yaw

    @yaw.setter
    def yaw(self, value: float) -> None:
        self._yaw = _clamp(float(value), -1.0, 1.0)
        self._yaw_set = True

    @property
    def brake(self) -> float:
        return self._brake

    @brake.setter
    def brake(self, value: float) -> None:
        self._brake = _clamp(float(value), 0.0, 1.0)
        self._brake_set = True

    @property
    def translate_forward(self) -> float:
        return self._translate_forward

    @translate_forward.setter
    def translate_forward(self, value: float) -> None:
        self._translate_forward = _clamp(float(value), -1.0, 1.0)
        self._translate_forward_set = True

    @property
    def translate_right(self) -> float:
        return self._translate_right

    @translate_right.setter
    def translate_right(self, value: float) -> None:
        self._translate_right = _clamp(float(value), -1.0, 1.0)
        self._translate_right_set = True

    @property
    def translate_up(self) -> float:
        return self._translate_up

    @translate_up.setter
    def translate_up(self, value: float) -> None:
        self._translate_up = _clamp(float(value), -1.0, 1.0)
        self._translate_up_set = True

    @property
    def slider_1(self) -> float:
        return self._slider_1

    @slider_1.setter
    def slider_1(self, value: float) -> None:
        self._slider_1 = _clamp(float(value), -1.0, 1.0)
        self._slider_1_set = True

    @property
    def slider_2(self) -> float:
        return self._slider_2

    @slider_2.setter
    def slider_2(self, value: float) -> None:
        self._slider_2 = _clamp(float(value), -1.0, 1.0)
        self._slider_2_set = True

    @property
    def slider_3(self) -> float:
        return self._slider_3

    @slider_3.setter
    def slider_3(self, value: float) -> None:
        self._slider_3 = _clamp(float(value), -1.0, 1.0)
        self._slider_3_set = True

    @property
    def slider_4(self) -> float:
        return self._slider_4

    @slider_4.setter
    def slider_4(self, value: float) -> None:
        self._slider_4 = _clamp(float(value), -1.0, 1.0)
        self._slider_4_set = True

    @property
    def translation_mode_enabled(self) -> bool:
        return self._translation_mode_enabled

    @translation_mode_enabled.setter
    def translation_mode_enabled(self, value: bool) -> None:
        self._translation_mode_enabled = bool(value)
        self._translation_mode_enabled_set = True

    def as_dict(self) -> dict[str, float]:
        controls: dict[str, float | bool] = {}
        if self._throttle_set:
            controls["throttle"] = self._throttle
        if self._pitch_set:
            controls["pitch"] = self._pitch
        if self._roll_set:
            controls["roll"] = self._roll
        if self._yaw_set:
            controls["yaw"] = self._yaw
        if self._brake_set:
            controls["brake"] = self._brake
        if self._translate_forward_set:
            controls["translate_forward"] = self._translate_forward
        if self._translate_right_set:
            controls["translate_right"] = self._translate_right
        if self._translate_up_set:
            controls["translate_up"] = self._translate_up
        if self._slider_1_set:
            controls["slider_1"] = self._slider_1
        if self._slider_2_set:
            controls["slider_2"] = self._slider_2
        if self._slider_3_set:
            controls["slider_3"] = self._slider_3
        if self._slider_4_set:
            controls["slider_4"] = self._slider_4
        if self._translation_mode_enabled_set:
            controls["translation_mode_enabled"] = self._translation_mode_enabled
        return controls


class JunoFacade:
    __slots__ = (
        "state",
        "control",
        "world",
        "events",
        "messages",
        "bodies",
        "parts",
        "crafts",
        "crafts_by_name",
        "_target_view",
        "_active_craft_id",
        "_connected",
        "_pending_display",
        "_pending_flight_log",
        "_pending_stage_activation",
        "_pending_time_warp_mode",
        "_pending_heading_lock_mode",
        "_pending_heading_lock_vector",
        "_pending_attitude_target",
        "_pending_activation_group",
        "_pending_target_burn_node",
        "_pending_pid_gains_pitch",
        "_pending_pid_gains_roll",
        "_pending_messages",
        "_rpc_caller",
        "_craft_views",
        "_body_views",
        "_tracked_craft_ids",
        "_event_queue",
        "_message_queue",
        "_program",
    )

    def __init__(self) -> None:
        self.state = StateView()
        self.control = ControlView()
        self.world = WorldFacade(self)
        self.events = EventsView(self)
        self.messages = MessagesView(self)
        self.bodies = BodiesCollectionView(self)
        self.parts = PartsCollectionView(self)
        self.crafts = CraftCollectionView(self)
        self.crafts_by_name = CraftsByNameCollectionView(self)
        self._target_view: BaseTargetView | None = None
        self._active_craft_id: str | None = None
        self._connected = False
        self._pending_display: tuple[str, float, int] | None = None
        self._pending_flight_log: tuple[str, bool] | None = None
        self._pending_stage_activation = False
        self._pending_time_warp_mode: int | None = None
        self._pending_heading_lock_mode: str | None = None
        self._pending_heading_lock_vector: np.ndarray | None = None
        self._pending_attitude_target: dict[str, float] | None = None
        self._pending_activation_group: tuple[int, bool] | None = None
        self._pending_target_burn_node = False
        self._pending_pid_gains_pitch: tuple[float, float, float] | None = None
        self._pending_pid_gains_roll: tuple[float, float, float] | None = None
        self._pending_messages: deque[dict[str, object]] = deque()
        self._rpc_caller = None
        self._craft_views: dict[str, CraftView] = {}
        self._body_views: dict[str, BodyView] = {}
        self._tracked_craft_ids: set[str] = set()
        self._event_queue: deque[BaseEventView] = deque()
        self._message_queue: deque[MessageReceivedEventView] = deque()
        self._program: JunoProgram | None = None

    @property
    def connected(self) -> bool:
        return self._connected

    @property
    def active_craft(self) -> CraftView | None:
        if self._active_craft_id is None and self._rpc_caller is not None:
            self._discover_crafts()

        if self._active_craft_id is None:
            return None

        return self._craft_views.get(self._active_craft_id)

    @property
    def target(self) -> BaseTargetView | None:
        return self._target_view

    def display(self, message: str, duration: float = 2.0) -> None:
        text = str(message)
        seconds = max(0.0, float(duration))
        # Repeat a short-lived UI message a few times because the control path is UDP and
        # one-shot display commands are otherwise easy to miss during startup or reconnects.
        self._pending_display = (text, seconds, 5)

    def flight_log(self, message: str, replace: bool = False) -> None:
        text = str(message)
        self._pending_flight_log = (text, bool(replace))

    def activate_stage(self) -> None:
        self._pending_stage_activation = True

    def set_time_warp(self, level: int) -> None:
        self._pending_time_warp_mode = max(0, int(level))

    def set_heading_lock(self, mode: str) -> None:
        normalized = str(mode).strip().lower()
        valid_modes = {"none", "prograde", "retrograde", "target", "burnnode", "current"}
        if normalized not in valid_modes:
            raise ValueError(
                "set_heading_lock(mode) must be one of: none, prograde, retrograde, "
                "target, burnnode, current."
            )
        self._pending_heading_lock_mode = normalized

    def lock_heading_vector(self, vector: Iterable[float]) -> None:
        values = np.asarray(list(vector), dtype=float)
        if values.shape != (3,):
            raise ValueError("lock_heading_vector(vector) requires exactly 3 components.")
        self._pending_heading_lock_vector = values

    def set_attitude_target(
        self, heading_deg: float | None = None, pitch_deg: float | None = None
    ) -> None:
        payload: dict[str, float] = {}
        if heading_deg is not None:
            payload["heading_deg"] = float(heading_deg)
        if pitch_deg is not None:
            payload["pitch_deg"] = float(pitch_deg)
        if not payload:
            raise ValueError("set_attitude_target(...) requires heading_deg and/or pitch_deg.")
        self._pending_attitude_target = payload

    def set_activation_group(self, group: int, enabled: bool = True) -> None:
        group_number = int(group)
        if group_number < 1:
            raise ValueError("Activation groups start at 1.")
        self._pending_activation_group = (group_number, bool(enabled))

    def target_burn_node(self) -> None:
        self._pending_target_burn_node = True

    def set_pid_gains_pitch(self, p: float, i: float, d: float) -> None:
        self._pending_pid_gains_pitch = (float(p), float(i), float(d))

    def set_pid_gains_roll(self, p: float, i: float, d: float) -> None:
        self._pending_pid_gains_roll = (float(p), float(i), float(d))

    def broadcast(self, message: str, data: object = None) -> None:
        self._enqueue_message("broadcast", message, data=data)

    def broadcast_to_craft(self, craft_id: str | int, message: str, data: object = None) -> None:
        self._enqueue_message("craft", message, craft_id=_normalize_craft_id(craft_id), data=data)

    def broadcast_to_nearby(
        self, message: str, data: object = None, radius: float | None = None
    ) -> None:
        payload_radius = None if radius is None else float(radius)
        if payload_radius is not None and payload_radius <= 0.0:
            raise ValueError("broadcast_to_nearby(..., radius=...) must be positive.")
        self._enqueue_message("nearby", message, data=data, radius=payload_radius)

    def receive_messages(self) -> list[MessageReceivedEventView]:
        return self._drain_messages()

    def wait_seconds(self, seconds: float) -> Iterator[object]:
        if self._program is None:
            raise RuntimeError("wait_seconds(...) is only available from an active JunoProgram.")
        return self._program.wait_seconds(seconds)

    def next_tick(self) -> Iterator[object]:
        if self._program is None:
            raise RuntimeError("next_tick() is only available from an active JunoProgram.")
        return self._program.next_tick()

    def wait_until(self, condition: Callable[..., object]) -> Iterator[object]:
        if self._program is None:
            raise RuntimeError("wait_until(...) is only available from an active JunoProgram.")
        return self._program.wait_until(condition)

    def list_crafts(self) -> list[CraftView]:
        self._discover_crafts()
        return list(self._craft_views.values())

    def list_craft_names(self) -> list[str]:
        return [craft.name for craft in self.list_crafts()]

    def list_craft_ids(self) -> list[str]:
        return [craft.craft_id for craft in self.list_crafts()]

    def list_bodies(self) -> list[BodyView]:
        return list(self._body_cache().values())

    def get_body(self, body: str) -> BodyView | None:
        normalized = str(body).strip()
        if not normalized:
            return None

        cached = self._body_views.get(normalized)
        if cached is not None:
            return cached

        self._discover_bodies()
        cached = self._body_views.get(normalized)
        if cached is not None:
            return cached

        result = self._rpc_call("get_body", {"body": normalized})
        body_payload = result.get("body") if isinstance(result, dict) else None
        if not isinstance(body_payload, dict):
            return None

        body_view = BodyView(body_payload)
        self._body_views[body_view.body_id] = body_view
        self._body_views[body_view.name] = body_view
        return body_view

    def list_parts(self, craft_id: str | int | None = None) -> list[PartView]:
        args: dict[str, object] = {}
        if craft_id is not None:
            args["craft_id"] = _normalize_craft_id(craft_id)

        result = self._rpc_call("list_parts", args)
        parts = result.get("parts", []) if isinstance(result, dict) else []
        if not isinstance(parts, list):
            return []

        return [PartView(self, part) for part in parts if isinstance(part, dict)]

    def get_part(self, part_id: int, craft_id: str | int | None = None) -> PartView | None:
        args: dict[str, object] = {"part_id": int(part_id)}
        if craft_id is not None:
            args["craft_id"] = _normalize_craft_id(craft_id)

        result = self._rpc_call("get_part", args)
        part = result.get("part") if isinstance(result, dict) else None
        return PartView(self, part) if isinstance(part, dict) else None

    def get_craft(self, craft_id: str | int) -> CraftView | None:
        normalized = _normalize_craft_id(craft_id)
        craft = self._craft_views.get(normalized)
        if craft is not None:
            return craft

        self._discover_crafts()
        return self._craft_views.get(normalized)

    def get_craft_name(self, craft_id: str | int) -> str | None:
        craft = self.get_craft(craft_id)
        return None if craft is None else craft.name

    def get_crafts_by_name(self, name: str) -> list[CraftView]:
        self._discover_crafts()
        return list(self._crafts_by_name().get(str(name), []))

    def get_craft_by_name(self, name: str) -> CraftView | None:
        matches = self.get_crafts_by_name(name)
        if not matches:
            return None

        if len(matches) > 1:
            raise JunoCraftAmbiguityError(
                f"Craft name '{name}' is ambiguous. Use craft_id instead."
            )

        return matches[0]

    def set_tracked_crafts(self, craft_ids: Iterable[str | int]) -> list[str]:
        if self._rpc_caller is None:
            raise JunoRpcError("RPC transport is not available.")

        normalized_ids = [_normalize_craft_id(craft_id) for craft_id in craft_ids]
        result = self._rpc_caller("set_tracked_crafts", {"craft_ids": normalized_ids})
        accepted_ids = result.get("craft_ids", []) if isinstance(result, dict) else []
        if not isinstance(accepted_ids, list):
            self._tracked_craft_ids = set()
            return []

        self._tracked_craft_ids = {
            craft_id for craft_id in accepted_ids if isinstance(craft_id, str)
        }
        self._discover_crafts()
        return sorted(self._tracked_craft_ids)

    def _discover_crafts(self) -> None:
        craft_records = self._list_crafts()
        self._update_craft_metadata(craft_records)

    def _discover_bodies(self) -> None:
        result = self._rpc_call("list_bodies", {})
        bodies = result.get("bodies", []) if isinstance(result, dict) else []
        if not isinstance(bodies, list):
            return

        for body in bodies:
            if not isinstance(body, dict):
                continue
            body_view = BodyView(body)
            self._body_views[body_view.body_id] = body_view
            self._body_views[body_view.name] = body_view

    def _list_crafts(self) -> list[dict[str, object]]:
        if self._rpc_caller is None:
            raise JunoRpcError("RPC transport is not available.")

        result = self._rpc_caller("list_crafts", {})
        crafts = result.get("crafts", []) if isinstance(result, dict) else []
        return crafts if isinstance(crafts, list) else []

    def _body_cache(self) -> dict[str, BodyView]:
        self._discover_bodies()
        return {
            body_id: body
            for body_id, body in self._body_views.items()
            if body_id == body.body_id
        }

    def _update_craft_metadata(self, craft_records: Iterable[dict[str, object]]) -> None:
        active_craft_id: str | None = self._active_craft_id

        for craft_record in craft_records:
            craft_id = craft_record.get("craft_id")
            craft_name = craft_record.get("name", craft_record.get("craft_name"))
            if not isinstance(craft_id, str) or not isinstance(craft_name, str) or not craft_name:
                continue

            normalized_id = _normalize_craft_id(craft_id)
            is_active = bool(craft_record.get("is_active", False))
            if is_active and active_craft_id is None:
                active_craft_id = normalized_id

            craft = self._craft_views.get(normalized_id)
            if craft is None:
                should_bind_active_state = self._connected and normalized_id == active_craft_id
                should_bind_rpc_active = not self._connected and is_active
                state_view = self.state if (should_bind_active_state or should_bind_rpc_active) else None
                craft = CraftView(
                    self,
                    craft_id=normalized_id,
                    name=craft_name,
                    is_active=should_bind_active_state or should_bind_rpc_active,
                    state_view=state_view,
                )
                self._craft_views[normalized_id] = craft
                continue

            craft.name = craft_name
            if not self._connected:
                craft._is_active = is_active
                if is_active:
                    craft._state_view = self.state
            if self._connected and normalized_id == active_craft_id:
                craft._is_active = True
                craft._state_view = self.state

        if self._active_craft_id is None:
            self._active_craft_id = active_craft_id

    def _apply_telemetry(self, telemetry: TelemetryEnvelope) -> list[BaseEventView]:
        active_id = telemetry.active_craft_id or None

        for craft in self._craft_views.values():
            craft._is_active = False
            craft._state_view = None

        for craft_id, telemetry_craft in telemetry.crafts.items():
            craft = self._craft_views.get(craft_id)
            if craft is None:
                craft = CraftView(self, craft_id=craft_id, name=telemetry_craft.name)
                self._craft_views[craft_id] = craft

            craft.name = telemetry_craft.name
            craft._is_active = craft_id == active_id

            if craft._is_active:
                self.state._replace(telemetry_craft.state)
                craft._state_view = self.state
            else:
                if craft._state_view is None or craft._state_view is self.state:
                    craft._state_view = StateView()
                craft._state_view._replace(telemetry_craft.state)

        self._active_craft_id = active_id
        self._target_view = _build_target_view(self, telemetry.target)
        new_events = [_build_event_view(event) for event in telemetry.events]
        self._event_queue.extend(new_events)
        for event in new_events:
            if isinstance(event, MessageReceivedEventView):
                self._message_queue.append(event)
        return new_events

    def _crafts_by_name(self) -> dict[str, list[CraftView]]:
        crafts_by_name: dict[str, list[CraftView]] = {}
        for craft in self._craft_views.values():
            crafts_by_name.setdefault(craft.name, []).append(craft)
        return crafts_by_name

    def _set_connected(self, connected: bool) -> None:
        self._connected = connected

    def _set_rpc_caller(self, rpc_caller) -> None:
        self._rpc_caller = rpc_caller

    def _rpc_call(self, method: str, args: dict[str, object]) -> dict[str, object]:
        if self._rpc_caller is None:
            raise JunoRpcError("RPC transport is not available.")
        result = self._rpc_caller(method, args)
        return result if isinstance(result, dict) else {}

    def _rpc_world_vector_result(self, method: str, **args: object) -> np.ndarray:
        payload = dict(args)
        if "position" in payload:
            payload["position"] = self._vector_payload(payload["position"], argument_name="position")

        if payload.get("body") is not None:
            payload["body"] = str(payload["body"])

        result = self._rpc_call(method, payload)
        vector = result.get("vector") if isinstance(result, dict) else None
        if not isinstance(vector, dict):
            raise JunoRpcError(f"{method} returned an invalid vector result.")

        x = vector.get("x")
        y = vector.get("y")
        z = vector.get("z")
        if not isinstance(x, (int, float)) or not isinstance(y, (int, float)) or not isinstance(z, (int, float)):
            raise JunoRpcError(f"{method} returned an invalid vector result.")

        return np.array([float(x), float(y), float(z)], dtype=float)

    def _rpc_part_vector_result(
        self,
        method: str,
        part_id: int,
        vector: Iterable[float],
        craft_id: str | None,
    ) -> np.ndarray:
        result = self._rpc_call(
            method,
            {
                "part_id": int(part_id),
                "craft_id": craft_id,
                "vector": self._vector_payload(vector, argument_name="vector"),
            },
        )
        payload = result.get("vector") if isinstance(result, dict) else None
        if not isinstance(payload, dict):
            raise JunoRpcError(f"{method} returned an invalid vector result.")
        return np.array(
            [
                float(payload.get("x", 0.0)),
                float(payload.get("y", 0.0)),
                float(payload.get("z", 0.0)),
            ],
            dtype=float,
        )

    @staticmethod
    def _vector_payload(values: Iterable[float], argument_name: str) -> dict[str, float]:
        vector = np.asarray(list(values), dtype=float)
        if vector.shape != (3,):
            raise ValueError(f"{argument_name} must have exactly 3 components.")
        return {
            "x": float(vector[0]),
            "y": float(vector[1]),
            "z": float(vector[2]),
        }

    def _ensure_tracked(self, craft_id: str) -> None:
        normalized_id = _normalize_craft_id(craft_id)
        if normalized_id == self._active_craft_id or normalized_id in self._tracked_craft_ids:
            return

        if self._rpc_caller is None:
            return

        desired_ids = sorted(self._tracked_craft_ids | {normalized_id})
        try:
            self.set_tracked_crafts(desired_ids)
        except JunoRpcError:
            return

    def _consume_display(self) -> tuple[str, float] | None:
        pending = self._pending_display
        if pending is None:
            return None

        text, seconds, remaining_sends = pending
        if remaining_sends <= 1:
            self._pending_display = None
        else:
            self._pending_display = (text, seconds, remaining_sends - 1)

        return (text, seconds)

    def _consume_flight_log(self) -> tuple[str, bool] | None:
        pending = self._pending_flight_log
        self._pending_flight_log = None
        return pending

    def _consume_stage_activation(self) -> bool:
        pending = self._pending_stage_activation
        self._pending_stage_activation = False
        return pending

    def _consume_time_warp_mode(self) -> int | None:
        pending = self._pending_time_warp_mode
        self._pending_time_warp_mode = None
        return pending

    def _consume_heading_lock_mode(self) -> str | None:
        pending = self._pending_heading_lock_mode
        self._pending_heading_lock_mode = None
        return pending

    def _consume_heading_lock_vector(self) -> np.ndarray | None:
        pending = self._pending_heading_lock_vector
        self._pending_heading_lock_vector = None
        return pending

    def _consume_attitude_target(self) -> dict[str, float] | None:
        pending = self._pending_attitude_target
        self._pending_attitude_target = None
        return pending

    def _consume_activation_group(self) -> tuple[int, bool] | None:
        pending = self._pending_activation_group
        self._pending_activation_group = None
        return pending

    def _consume_target_burn_node(self) -> bool:
        pending = self._pending_target_burn_node
        self._pending_target_burn_node = False
        return pending

    def _consume_pid_gains_pitch(self) -> tuple[float, float, float] | None:
        pending = self._pending_pid_gains_pitch
        self._pending_pid_gains_pitch = None
        return pending

    def _consume_pid_gains_roll(self) -> tuple[float, float, float] | None:
        pending = self._pending_pid_gains_roll
        self._pending_pid_gains_roll = None
        return pending

    def _consume_messages(self) -> list[dict[str, object]]:
        pending = list(self._pending_messages)
        self._pending_messages.clear()
        return pending

    def _drain_events(self) -> list[BaseEventView]:
        events = list(self._event_queue)
        self._event_queue.clear()
        return events

    def _drain_messages(self) -> list[MessageReceivedEventView]:
        messages = list(self._message_queue)
        self._message_queue.clear()
        return messages

    def _enqueue_message(
        self,
        scope: str,
        message: str,
        *,
        data: object = None,
        craft_id: str | None = None,
        radius: float | None = None,
    ) -> None:
        text = str(message)
        if not text:
            raise ValueError("message must not be empty.")

        try:
            normalized_data = json.loads(json.dumps(data))
        except (TypeError, ValueError) as exc:
            raise TypeError("message data must be JSON-serializable.") from exc

        payload: dict[str, object] = {
            "scope": scope,
            "message": text,
            "data": normalized_data,
        }
        if craft_id is not None:
            payload["craft_id"] = str(craft_id)
        if radius is not None:
            payload["radius"] = float(radius)
        self._pending_messages.append(payload)


juno = JunoFacade()


class JunoProgram:
    __slots__ = (
        "juno",
        "mode",
        "_mode_entry_time",
        "mode_just_entered",
        "_script_iterator",
        "_script_waiter",
        "_script_completed",
        "_script_started",
    )

    def __init__(self, facade: JunoFacade | None = None) -> None:
        self.juno = facade or juno
        self.juno._program = self
        self.mode: str | None = None
        self._mode_entry_time: float | None = None
        self.mode_just_entered = False
        self._script_iterator: Iterator[object] | None = None
        self._script_waiter: _WaitForSeconds | _WaitUntil | None = None
        self._script_completed = False
        self._script_started = False

    def on_start(self) -> None:
        pass

    def on_event(self, event: BaseEventView) -> None:
        pass

    def on_message(self, message: MessageReceivedEventView) -> None:
        pass

    def set_mode(self, name: str) -> None:
        self.mode = str(name)
        if self.juno.connected:
            self._mode_entry_time = self.juno.state.time
        else:
            self._mode_entry_time = None
        self.mode_just_entered = True

    def mode_elapsed_time(self) -> float:
        if self._mode_entry_time is None:
            return 0.0

        return max(0.0, float(self.juno.state.time - self._mode_entry_time))

    def wait_seconds_done(self, seconds: float) -> bool:
        return self.mode_elapsed_time() >= max(0.0, float(seconds))

    def wait_until_done(self, condition) -> bool:
        return self._evaluate_condition(condition)

    def wait_seconds(self, seconds: float) -> Iterator[object]:
        yield _WaitForSeconds(seconds)

    def next_tick(self) -> Iterator[object]:
        yield from self.wait_seconds(0.0)

    def wait_until(self, condition: Callable[..., object]) -> Iterator[object]:
        if not callable(condition):
            raise TypeError("wait_until(...) requires a callable condition.")

        yield _WaitUntil(condition)

    def dispatch_mode(self) -> None:
        if not self.mode:
            raise RuntimeError(
                "No active mode is set. Call set_mode(...) in on_start() or before dispatch_mode()."
            )

        if self._mode_entry_time is None:
            self._mode_entry_time = self.juno.state.time

        handler_name = f"mode_{self.mode}"
        handler = getattr(self, handler_name, None)
        if handler is None or not callable(handler):
            raise RuntimeError(
                f"Active mode '{self.mode}' has no handler. Expected method '{handler_name}()'."
            )

        just_entered = self.mode_just_entered
        try:
            handler()
        finally:
            if just_entered and self.mode_just_entered and getattr(self, "mode", None) is not None:
                self.mode_just_entered = False

    def loop(self) -> None:
        if self.mode:
            self.dispatch_mode()
            return

        if self._script_started or self._has_script():
            self._step_script()
            return

        self.dispatch_mode()

    def _has_script(self) -> bool:
        return callable(getattr(self, "script", None))

    def _ensure_script_iterator(self) -> Iterator[object] | None:
        if self._script_completed:
            return None

        if self._script_iterator is not None:
            return self._script_iterator

        script_method = getattr(self, "script", None)
        if not callable(script_method):
            return None

        script_result = script_method()
        self._script_started = True
        if script_result is None:
            self._script_completed = True
            return None

        if not isinstance(script_result, Iterator):
            raise RuntimeError(
                "script() must either return None for a one-shot script or be a generator. "
                "Use 'yield from self.wait_seconds(...)' or 'yield from self.wait_until(...)' "
                "for cross-tick sequencing."
            )

        self._script_iterator = script_result
        return script_result

    def _step_script(self) -> None:
        if self._script_completed:
            return

        script_iterator = self._ensure_script_iterator()
        if script_iterator is None:
            return

        while True:
            if self._script_waiter is not None:
                if not self._script_waiter.ready(self):
                    return
                self._script_waiter = None

            try:
                yielded = next(script_iterator)
            except StopIteration:
                self._script_completed = True
                self._script_waiter = None
                return

            if isinstance(yielded, (_WaitForSeconds, _WaitUntil)):
                self._script_waiter = yielded
                continue

            raise RuntimeError(
                "script() yielded an unsupported value. Use "
                "'yield from self.wait_seconds(...)' or 'yield from self.wait_until(...)'."
            )

    def _evaluate_condition(self, condition: Callable[..., object]) -> bool:
        if not callable(condition):
            raise TypeError("Condition must be callable.")

        try:
            signature = inspect.signature(condition)
        except (TypeError, ValueError):
            return bool(condition(self.juno.state))

        positional_params = [
            parameter
            for parameter in signature.parameters.values()
            if parameter.kind
            in (
                inspect.Parameter.POSITIONAL_ONLY,
                inspect.Parameter.POSITIONAL_OR_KEYWORD,
            )
        ]
        has_varargs = any(
            parameter.kind == inspect.Parameter.VAR_POSITIONAL
            for parameter in signature.parameters.values()
        )

        if not positional_params and not has_varargs:
            return bool(condition())

        return bool(condition(self.juno.state))
