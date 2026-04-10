from __future__ import annotations

import json
import select
import socket
import time
from typing import Final

from .api import ControlView
from .errors import JunoRpcError
from .models import (
    Snapshot,
    TelemetryEvent,
    TelemetryBodyRef,
    TelemetryCraft,
    TelemetryEnvelope,
    TelemetryTarget,
    TelemetryVector3,
)

DEFAULT_HOST: Final[str] = "127.0.0.1"
DEFAULT_TELEMETRY_PORT: Final[int] = 5005
DEFAULT_COMMAND_PORT: Final[int] = 5006
DEFAULT_RPC_REQUEST_PORT: Final[int] = 5007
DEFAULT_RPC_RESPONSE_PORT: Final[int] = 5008


def _as_float(value: object, default: float = 0.0) -> float:
    if isinstance(value, (int, float)):
        return float(value)

    return default


def _as_int(value: object, default: int = 0) -> int:
    if isinstance(value, bool):
        return int(value)
    if isinstance(value, int):
        return value
    if isinstance(value, float):
        return int(value)
    return default


def _as_bool(value: object, default: bool = False) -> bool:
    if isinstance(value, bool):
        return value
    return default


def _as_str(value: object, default: str = "") -> str:
    if isinstance(value, str):
        return value
    return default


def _as_optional_float(value: object) -> float | None:
    if isinstance(value, (int, float)):
        return float(value)
    return None


def _as_optional_int(value: object) -> int | None:
    if isinstance(value, bool):
        return int(value)
    if isinstance(value, int):
        return value
    if isinstance(value, float):
        return int(value)
    return None


def _parse_vector3(value: object) -> TelemetryVector3 | None:
    if not isinstance(value, dict):
        return None

    return TelemetryVector3(
        x=_as_float(value.get("x")),
        y=_as_float(value.get("y")),
        z=_as_float(value.get("z")),
    )


def _parse_body_ref(value: object) -> TelemetryBodyRef | None:
    if not isinstance(value, dict):
        return None

    body_id = _as_str(value.get("body_id"))
    name = _as_str(value.get("name"))
    if not body_id and not name:
        return None

    return TelemetryBodyRef(body_id=body_id, name=name)


def _parse_body_ref_from_target(value: dict[str, object]) -> TelemetryBodyRef | None:
    nested = _parse_body_ref(value.get("parent_body"))
    if nested is not None:
        return nested

    body_id = _as_str(value.get("parent_body_id"))
    name = _as_str(value.get("parent_body_name"))
    if not body_id and not name:
        return None

    return TelemetryBodyRef(body_id=body_id, name=name)


def _parse_target(value: object) -> TelemetryTarget | None:
    if value is None:
        return None

    if not isinstance(value, dict):
        return None

    target_type = _as_str(value.get("target_type"))
    target_id = _as_str(value.get("target_id"))
    name = _as_str(value.get("name"))
    if not target_type and not target_id and not name:
        return None

    return TelemetryTarget(
        target_type=target_type,
        target_id=target_id,
        name=name,
        parent_body=_parse_body_ref_from_target(value),
        position=_parse_vector3(value.get("position")),
        velocity=_parse_vector3(value.get("velocity")),
        craft_id=_as_str(value.get("craft_id")) or None,
        part_id=_as_optional_int(value.get("part_id")),
        part_name=_as_str(value.get("part_name")) or None,
        parent_craft_id=_as_str(value.get("parent_craft_id")) or None,
        parent_craft_name=_as_str(value.get("parent_craft_name")) or None,
        body_id=_as_str(value.get("body_id")) or None,
        radius=_as_optional_float(value.get("radius")),
        mass=_as_optional_float(value.get("mass")),
        mu=_as_optional_float(value.get("mu")),
        angular_velocity=_as_optional_float(value.get("angular_velocity")),
        landmark_id=_as_str(value.get("landmark_id")) or None,
        latitude_deg=_as_optional_float(value.get("latitude_deg")),
        longitude_deg=_as_optional_float(value.get("longitude_deg")),
    )


def _parse_event(value: object) -> TelemetryEvent | None:
    if not isinstance(value, dict):
        return None

    event_type = _as_str(value.get("event_type"))
    event_id = _as_str(value.get("event_id"))
    if not event_type and not event_id:
        return None

    return TelemetryEvent(
        event_type=event_type,
        event_id=event_id,
        time=_as_float(value.get("time")),
        craft_id=_as_str(value.get("craft_id")) or None,
        craft_name=_as_str(value.get("craft_name")) or None,
        sender_craft_id=_as_str(value.get("sender_craft_id")) or None,
        sender_craft_name=_as_str(value.get("sender_craft_name")) or None,
        other_craft_id=_as_str(value.get("other_craft_id")) or None,
        other_craft_name=_as_str(value.get("other_craft_name")) or None,
        part_id=_as_optional_int(value.get("part_id")),
        part_name=_as_str(value.get("part_name")) or None,
        other_part_id=_as_optional_int(value.get("other_part_id")),
        other_part_name=_as_str(value.get("other_part_name")) or None,
        message_name=_as_str(value.get("message_name")) or None,
        message_data=value.get("message_data"),
        delivery_scope=_as_str(value.get("delivery_scope")) or None,
        parent_body=_parse_body_ref_from_target(value),
        new_parent_body=_parse_body_ref(value.get("new_parent_body")),
        relative_velocity=_as_optional_float(value.get("relative_velocity")),
        impulse=_as_optional_float(value.get("impulse")),
        is_ground_collision=(
            bool(value.get("is_ground_collision"))
            if isinstance(value.get("is_ground_collision"), bool)
            else None
        ),
        point=_parse_vector3(value.get("point")),
        normal=_parse_vector3(value.get("normal")),
    )


def _build_snapshot(seq: int, time_value: float, state: dict[str, object]) -> Snapshot:
    return Snapshot(
        seq=seq,
        time=time_value,
        altitude_asl=_as_float(state.get("altitude_asl")),
        altitude_agl=_as_float(state.get("altitude_agl")),
        altitude_terrain=_as_float(state.get("altitude_terrain")),
        mass=_as_float(state.get("mass")),
        fuel_mass_scaled=_as_float(state.get("fuel_mass_scaled")),
        remaining_fuel_in_stage_fraction=_as_float(state.get("remaining_fuel_in_stage_fraction")),
        remaining_monopropellant_fraction=_as_float(state.get("remaining_monopropellant_fraction")),
        remaining_battery_fraction=_as_float(state.get("remaining_battery_fraction")),
        surface_speed=_as_float(state.get("surface_speed")),
        horizontal_speed=_as_float(state.get("horizontal_speed")),
        vertical_speed=_as_float(state.get("vertical_speed")),
        orbital_speed=_as_float(state.get("orbital_speed")),
        pitch=_as_float(state.get("pitch")),
        bank_angle=_as_float(state.get("bank_angle")),
        heading=_as_float(state.get("heading")),
        angle_of_attack=_as_float(state.get("angle_of_attack")),
        side_slip=_as_float(state.get("side_slip")),
        latitude_deg=_as_float(state.get("latitude_deg")),
        longitude_deg=_as_float(state.get("longitude_deg")),
        distance_from_center=_as_float(state.get("distance_from_center")),
        acceleration=_as_float(state.get("acceleration")),
        drag_acceleration=_as_float(state.get("drag_acceleration")),
        g_force=_as_float(state.get("g_force")),
        angular_speed=_as_float(state.get("angular_speed")),
        current_engine_thrust=_as_float(state.get("current_engine_thrust")),
        current_rcs_thrust_scaled=_as_float(state.get("current_rcs_thrust_scaled")),
        active_engine_count=_as_int(state.get("active_engine_count")),
        active_rcs_count=_as_int(state.get("active_rcs_count")),
        grounded=_as_bool(state.get("grounded")),
        in_water=_as_bool(state.get("in_water")),
        supports_warp_burn=_as_bool(state.get("supports_warp_burn")),
        mach_number=_as_float(state.get("mach_number")),
        current_isp=_as_float(state.get("current_isp")),
        delta_v_stage=_as_float(state.get("delta_v_stage")),
        thrust_to_weight_ratio=_as_float(state.get("thrust_to_weight_ratio")),
        remaining_burn_time=_as_float(state.get("remaining_burn_time")),
        fuel_all_stages_fraction=_as_float(state.get("fuel_all_stages_fraction")),
        weighted_throttle_response=_as_float(state.get("weighted_throttle_response")),
        weighted_throttle_response_time=_as_float(state.get("weighted_throttle_response_time")),
        planet_name=_as_str(state.get("planet_name")),
        planet_radius=_as_float(state.get("planet_radius")),
        planet_surface_gravity=_as_float(state.get("planet_surface_gravity")),
        position_x=_as_float(state.get("position_x")),
        position_y=_as_float(state.get("position_y")),
        position_z=_as_float(state.get("position_z")),
        velocity_x=_as_float(state.get("velocity_x")),
        velocity_y=_as_float(state.get("velocity_y")),
        velocity_z=_as_float(state.get("velocity_z")),
        surface_velocity_x=_as_float(state.get("surface_velocity_x")),
        surface_velocity_y=_as_float(state.get("surface_velocity_y")),
        surface_velocity_z=_as_float(state.get("surface_velocity_z")),
        target_velocity_x=_as_float(state.get("target_velocity_x")),
        target_velocity_y=_as_float(state.get("target_velocity_y")),
        target_velocity_z=_as_float(state.get("target_velocity_z")),
        acceleration_x=_as_float(state.get("acceleration_x")),
        acceleration_y=_as_float(state.get("acceleration_y")),
        acceleration_z=_as_float(state.get("acceleration_z")),
        angular_velocity_x=_as_float(state.get("angular_velocity_x")),
        angular_velocity_y=_as_float(state.get("angular_velocity_y")),
        angular_velocity_z=_as_float(state.get("angular_velocity_z")),
        craft_forward_x=_as_float(state.get("craft_forward_x")),
        craft_forward_y=_as_float(state.get("craft_forward_y")),
        craft_forward_z=_as_float(state.get("craft_forward_z")),
        craft_right_x=_as_float(state.get("craft_right_x")),
        craft_right_y=_as_float(state.get("craft_right_y")),
        craft_right_z=_as_float(state.get("craft_right_z")),
        craft_up_x=_as_float(state.get("craft_up_x")),
        craft_up_y=_as_float(state.get("craft_up_y")),
        craft_up_z=_as_float(state.get("craft_up_z")),
        nav_north_x=_as_float(state.get("nav_north_x")),
        nav_north_y=_as_float(state.get("nav_north_y")),
        nav_north_z=_as_float(state.get("nav_north_z")),
        nav_east_x=_as_float(state.get("nav_east_x")),
        nav_east_y=_as_float(state.get("nav_east_y")),
        nav_east_z=_as_float(state.get("nav_east_z")),
        nav_up_x=_as_float(state.get("nav_up_x")),
        nav_up_y=_as_float(state.get("nav_up_y")),
        nav_up_z=_as_float(state.get("nav_up_z")),
        nav_craft_roll_axis_x=_as_float(state.get("nav_craft_roll_axis_x")),
        nav_craft_roll_axis_y=_as_float(state.get("nav_craft_roll_axis_y")),
        nav_craft_roll_axis_z=_as_float(state.get("nav_craft_roll_axis_z")),
        nav_craft_pitch_axis_x=_as_float(state.get("nav_craft_pitch_axis_x")),
        nav_craft_pitch_axis_y=_as_float(state.get("nav_craft_pitch_axis_y")),
        nav_craft_pitch_axis_z=_as_float(state.get("nav_craft_pitch_axis_z")),
        nav_craft_yaw_axis_x=_as_float(state.get("nav_craft_yaw_axis_x")),
        nav_craft_yaw_axis_y=_as_float(state.get("nav_craft_yaw_axis_y")),
        nav_craft_yaw_axis_z=_as_float(state.get("nav_craft_yaw_axis_z")),
        body_velocity_forward=_as_float(state.get("body_velocity_forward")),
        body_velocity_right=_as_float(state.get("body_velocity_right")),
        body_velocity_up=_as_float(state.get("body_velocity_up")),
        surface_body_velocity_forward=_as_float(state.get("surface_body_velocity_forward")),
        surface_body_velocity_right=_as_float(state.get("surface_body_velocity_right")),
        surface_body_velocity_up=_as_float(state.get("surface_body_velocity_up")),
        control_throttle=_as_float(state.get("control_throttle")),
        control_pitch=_as_float(state.get("control_pitch")),
        control_roll=_as_float(state.get("control_roll")),
        control_yaw=_as_float(state.get("control_yaw")),
        control_brake=_as_float(state.get("control_brake")),
        control_translate_forward=_as_float(state.get("control_translate_forward")),
        control_translate_right=_as_float(state.get("control_translate_right")),
        control_translate_up=_as_float(state.get("control_translate_up")),
        control_slider_1=_as_float(state.get("control_slider_1")),
        control_slider_2=_as_float(state.get("control_slider_2")),
        control_slider_3=_as_float(state.get("control_slider_3")),
        control_slider_4=_as_float(state.get("control_slider_4")),
        control_translation_mode_enabled=_as_bool(state.get("control_translation_mode_enabled")),
        control_pitch_input_received=_as_bool(state.get("control_pitch_input_received")),
        control_roll_input_received=_as_bool(state.get("control_roll_input_received")),
        control_yaw_input_received=_as_bool(state.get("control_yaw_input_received")),
        environment_air_density=_as_float(state.get("environment_air_density")),
        environment_air_pressure=_as_float(state.get("environment_air_pressure")),
        environment_temperature=_as_float(state.get("environment_temperature")),
        environment_speed_of_sound=_as_float(state.get("environment_speed_of_sound")),
        environment_atmosphere_height=_as_float(state.get("environment_atmosphere_height")),
        environment_sample_altitude=_as_float(state.get("environment_sample_altitude")),
        environment_surface_air_density=_as_float(state.get("environment_surface_air_density")),
        max_engine_thrust=_as_float(state.get("max_engine_thrust")),
        gravity_acceleration=_as_float(state.get("gravity_acceleration")),
        gravity_acceleration_x=_as_float(state.get("gravity_acceleration_x")),
        gravity_acceleration_y=_as_float(state.get("gravity_acceleration_y")),
        gravity_acceleration_z=_as_float(state.get("gravity_acceleration_z")),
        engine_force_x=_as_float(state.get("engine_force_x")),
        engine_force_y=_as_float(state.get("engine_force_y")),
        engine_force_z=_as_float(state.get("engine_force_z")),
        lift_force_x=_as_float(state.get("lift_force_x")),
        lift_force_y=_as_float(state.get("lift_force_y")),
        lift_force_z=_as_float(state.get("lift_force_z")),
        drag_force_x=_as_float(state.get("drag_force_x")),
        drag_force_y=_as_float(state.get("drag_force_y")),
        drag_force_z=_as_float(state.get("drag_force_z")),
        solar_radiation_intensity=_as_float(state.get("solar_radiation_intensity")),
        orbit_apoapsis_altitude=_as_float(state.get("orbit_apoapsis_altitude")),
        orbit_apoapsis_time=_as_float(state.get("orbit_apoapsis_time")),
        orbit_periapsis_altitude=_as_float(state.get("orbit_periapsis_altitude")),
        orbit_periapsis_time=_as_float(state.get("orbit_periapsis_time")),
        orbit_eccentricity=_as_float(state.get("orbit_eccentricity")),
        orbit_inclination_rad=_as_float(state.get("orbit_inclination_rad")),
        orbit_period=_as_float(state.get("orbit_period")),
        frame_delta_time=_as_float(state.get("frame_delta_time")),
        time_since_launch=_as_float(state.get("time_since_launch")),
        total_time=_as_float(state.get("total_time")),
        warp_amount=_as_float(state.get("warp_amount")),
        real_time=_as_float(state.get("real_time")),
    )


def parse_snapshot_payload(payload: bytes) -> TelemetryEnvelope | None:
    try:
        envelope = json.loads(payload.decode("utf-8"))
    except (UnicodeDecodeError, json.JSONDecodeError):
        return None

    if not isinstance(envelope, dict):
        return None

    if envelope.get("type") != "telemetry":
        return None

    seq = envelope.get("seq")
    crafts = envelope.get("crafts")
    if not isinstance(seq, int) or not isinstance(crafts, dict):
        return None

    time_value = _as_float(envelope.get("t"))
    active_craft_id = _as_str(envelope.get("active_craft_id"))
    parsed_crafts: dict[str, TelemetryCraft] = {}

    for craft_id, craft_payload in crafts.items():
        if not isinstance(craft_id, str) or not isinstance(craft_payload, dict):
            continue

        name = _as_str(craft_payload.get("name"))
        state = craft_payload.get("state")
        if not isinstance(state, dict):
            continue

        snapshot = _build_snapshot(seq, time_value, state)
        parsed_crafts[craft_id] = TelemetryCraft(
            craft_id=_as_str(craft_payload.get("craft_id"), craft_id),
            name=name,
            state=snapshot,
        )

    event_payloads = envelope.get("events", [])
    parsed_events: list[TelemetryEvent] = []
    if isinstance(event_payloads, list):
        for event_payload in event_payloads:
            parsed_event = _parse_event(event_payload)
            if parsed_event is not None:
                parsed_events.append(parsed_event)

    return TelemetryEnvelope(
        seq=seq,
        time=time_value,
        active_craft_id=active_craft_id,
        crafts=parsed_crafts,
        target=_parse_target(envelope.get("target")),
        events=tuple(parsed_events),
    )


def build_control_payload(
    seq: int,
    control: ControlView,
    display: tuple[str, float] | None = None,
    flight_log: tuple[str, bool] | None = None,
    activate_stage: bool = False,
    time_warp_mode: int | None = None,
    heading_lock_mode: str | None = None,
    heading_lock_vector: object | None = None,
    attitude_target: dict[str, float] | None = None,
    activation_group: tuple[int, bool] | None = None,
    target_burn_node: bool = False,
    pid_gains_pitch: tuple[float, float, float] | None = None,
    pid_gains_roll: tuple[float, float, float] | None = None,
    messages: list[dict[str, object]] | None = None,
) -> bytes:
    message = {
        "seq": max(seq, 0),
        "control": control.as_dict(),
    }

    if display is not None:
        text, duration = display
        message["display"] = {
            "message": text,
            "duration": max(0.0, float(duration)),
        }

    if flight_log is not None:
        text, replace = flight_log
        message["flight_log"] = {
            "message": text,
            "replace": bool(replace),
        }

    if activate_stage:
        message["stage"] = {
            "activate_next": True,
        }

    if time_warp_mode is not None:
        message["time_warp"] = {
            "mode": max(0, int(time_warp_mode)),
        }

    if heading_lock_mode is not None:
        message["heading_lock"] = {
            "mode": str(heading_lock_mode),
        }

    if heading_lock_vector is not None:
        vector = list(heading_lock_vector)
        message["heading_lock_vector"] = {
            "x": float(vector[0]),
            "y": float(vector[1]),
            "z": float(vector[2]),
        }

    if attitude_target is not None:
        payload: dict[str, float] = {}
        if "heading_deg" in attitude_target:
            payload["heading_deg"] = float(attitude_target["heading_deg"])
        if "pitch_deg" in attitude_target:
            payload["pitch_deg"] = float(attitude_target["pitch_deg"])
        if payload:
            message["attitude_target"] = payload

    if activation_group is not None:
        group, enabled = activation_group
        message["activation_group"] = {
            "group": max(1, int(group)),
            "enabled": bool(enabled),
        }

    if target_burn_node:
        message["target_node"] = {
            "name": "burnnode",
        }

    if pid_gains_pitch is not None:
        p, i, d = pid_gains_pitch
        message["pid_gains_pitch"] = {"p": float(p), "i": float(i), "d": float(d)}

    if pid_gains_roll is not None:
        p, i, d = pid_gains_roll
        message["pid_gains_roll"] = {"p": float(p), "i": float(i), "d": float(d)}

    if messages:
        message["messages"] = list(messages)

    return json.dumps(message, separators=(",", ":")).encode("utf-8")


def build_rpc_request_payload(request_id: int, method: str, args: dict[str, object] | None = None) -> bytes:
    message = {
        "type": "rpc_request",
        "id": max(request_id, 0),
        "method": str(method),
        "args": args or {},
    }
    return json.dumps(message, separators=(",", ":")).encode("utf-8")


def build_rpc_response_payload(
    request_id: int,
    ok: bool,
    result: dict[str, object] | None = None,
    error_code: str | None = None,
    error_message: str | None = None,
) -> bytes:
    message: dict[str, object] = {
        "type": "rpc_response",
        "id": max(request_id, 0),
        "ok": bool(ok),
    }

    if ok:
        message["result"] = result or {}
    else:
        message["error"] = {
            "code": error_code or "rpc_error",
            "message": error_message or "RPC request failed.",
        }

    return json.dumps(message, separators=(",", ":")).encode("utf-8")


def parse_rpc_response_payload(payload: bytes) -> dict[str, object] | None:
    try:
        envelope = json.loads(payload.decode("utf-8"))
    except (UnicodeDecodeError, json.JSONDecodeError):
        return None

    if not isinstance(envelope, dict):
        return None

    if envelope.get("type") != "rpc_response":
        return None

    if not isinstance(envelope.get("id"), int):
        return None

    if not isinstance(envelope.get("ok"), bool):
        return None

    return envelope


class UdpBridgeTransport:
    def __init__(
        self,
        telemetry_address: tuple[str, int] = (DEFAULT_HOST, DEFAULT_TELEMETRY_PORT),
        command_address: tuple[str, int] = (DEFAULT_HOST, DEFAULT_COMMAND_PORT),
        rpc_request_address: tuple[str, int] = (DEFAULT_HOST, DEFAULT_RPC_REQUEST_PORT),
        rpc_response_address: tuple[str, int] = (DEFAULT_HOST, DEFAULT_RPC_RESPONSE_PORT),
        poll_timeout: float = 0.02,
    ) -> None:
        self._telemetry_address = telemetry_address
        self._command_address = command_address
        self._rpc_request_address = rpc_request_address
        self._rpc_response_address = rpc_response_address
        self._poll_timeout = poll_timeout
        self._telemetry_socket: socket.socket | None = None
        self._command_socket: socket.socket | None = None
        self._rpc_request_socket: socket.socket | None = None
        self._rpc_response_socket: socket.socket | None = None
        self._next_rpc_id = 1

    def open(self) -> None:
        self.open_streams()
        self.open_rpc()

    def open_streams(self) -> None:
        if self._telemetry_socket is None:
            self._telemetry_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            self._telemetry_socket.bind(self._telemetry_address)
            self._telemetry_socket.setblocking(False)

        if self._command_socket is None:
            self._command_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            self._command_socket.connect(self._command_address)

    def open_rpc(self) -> None:
        if self._rpc_response_socket is None:
            self._rpc_response_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            self._rpc_response_socket.bind(self._rpc_response_address)
            self._rpc_response_socket.setblocking(False)

        if self._rpc_request_socket is None:
            self._rpc_request_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            self._rpc_request_socket.connect(self._rpc_request_address)

    def close(self) -> None:
        if self._telemetry_socket is not None:
            self._telemetry_socket.close()
            self._telemetry_socket = None

        if self._command_socket is not None:
            self._command_socket.close()
            self._command_socket = None

        if self._rpc_request_socket is not None:
            self._rpc_request_socket.close()
            self._rpc_request_socket = None

        if self._rpc_response_socket is not None:
            self._rpc_response_socket.close()
            self._rpc_response_socket = None

    def receive_latest_snapshot(self) -> TelemetryEnvelope | None:
        if self._telemetry_socket is None:
            raise RuntimeError("Telemetry socket is not open.")

        ready, _, _ = select.select([self._telemetry_socket], [], [], self._poll_timeout)
        if not ready:
            return None

        latest_telemetry: TelemetryEnvelope | None = None

        while True:
            try:
                payload, _ = self._telemetry_socket.recvfrom(65535)
            except BlockingIOError:
                break

            telemetry = parse_snapshot_payload(payload)
            if telemetry is not None:
                latest_telemetry = telemetry

        return latest_telemetry

    def send_control(
        self,
        seq: int,
        control: ControlView,
        display: tuple[str, float] | None = None,
        flight_log: tuple[str, bool] | None = None,
        activate_stage: bool = False,
        time_warp_mode: int | None = None,
        heading_lock_mode: str | None = None,
        heading_lock_vector: object | None = None,
        attitude_target: dict[str, float] | None = None,
        activation_group: tuple[int, bool] | None = None,
        target_burn_node: bool = False,
        pid_gains_pitch: tuple[float, float, float] | None = None,
        pid_gains_roll: tuple[float, float, float] | None = None,
        messages: list[dict[str, object]] | None = None,
    ) -> None:
        if self._command_socket is None:
            raise RuntimeError("Command socket is not open.")

        self._command_socket.send(
            build_control_payload(
                seq,
                control,
                display,
                flight_log,
                activate_stage,
                time_warp_mode,
                heading_lock_mode,
                heading_lock_vector,
                attitude_target,
                activation_group,
                target_burn_node,
                pid_gains_pitch,
                pid_gains_roll,
                messages,
            )
        )

    def call_rpc(
        self,
        method: str,
        args: dict[str, object] | None = None,
        timeout: float = 1.0,
    ) -> dict[str, object]:
        self.open_rpc()

        if self._rpc_request_socket is None or self._rpc_response_socket is None:
            raise JunoRpcError("RPC sockets are not open.")

        request_id = self._next_rpc_id
        self._next_rpc_id += 1

        self._rpc_request_socket.send(build_rpc_request_payload(request_id, method, args))

        deadline = time.monotonic() + max(0.0, float(timeout))
        while True:
            remaining = deadline - time.monotonic()
            if remaining <= 0.0:
                raise JunoRpcError(f"RPC request timed out: {method}")

            ready, _, _ = select.select([self._rpc_response_socket], [], [], remaining)
            if not ready:
                raise JunoRpcError(f"RPC request timed out: {method}")

            try:
                payload, _ = self._rpc_response_socket.recvfrom(65535)
            except BlockingIOError:
                continue

            response = parse_rpc_response_payload(payload)
            if response is None:
                continue

            if response.get("id") != request_id:
                continue

            if not response.get("ok", False):
                error = response.get("error")
                if isinstance(error, dict):
                    message = error.get("message")
                    if isinstance(message, str) and message:
                        raise JunoRpcError(message)

                raise JunoRpcError(f"RPC request failed: {method}")

            result = response.get("result")
            return result if isinstance(result, dict) else {}
