from __future__ import annotations

import math
from dataclasses import dataclass

@dataclass(slots=True)
class TickTimingStats:
    tick_count: int = 0
    previous_t: float | None = None
    last_t: float | None = None
    last_dt: float | None = None
    min_dt: float | None = None
    max_dt: float | None = None
    total_dt: float = 0.0
    average_dt: float = 0.0

    def record(self, current_t: float) -> float | None:
        self.tick_count += 1
        self.last_t = float(current_t)

        if self.previous_t is None:
            self.previous_t = self.last_t
            return None

        dt = self.last_t - self.previous_t
        self.previous_t = self.last_t
        self.last_dt = dt
        self.total_dt += dt

        if self.min_dt is None or dt < self.min_dt:
            self.min_dt = dt

        if self.max_dt is None or dt > self.max_dt:
            self.max_dt = dt

        sample_count = self.tick_count - 1
        if sample_count > 0:
            self.average_dt = self.total_dt / sample_count

        return dt


@dataclass(frozen=True, slots=True)
class Vector3Sample:
    x: float
    y: float
    z: float

    @property
    def magnitude(self) -> float:
        return math.sqrt(self.x * self.x + self.y * self.y + self.z * self.z)

    def add(self, other: "Vector3Sample") -> "Vector3Sample":
        return Vector3Sample(self.x + other.x, self.y + other.y, self.z + other.z)

    def subtract(self, other: "Vector3Sample") -> "Vector3Sample":
        return Vector3Sample(self.x - other.x, self.y - other.y, self.z - other.z)

    def scale(self, factor: float) -> "Vector3Sample":
        return Vector3Sample(self.x * factor, self.y * factor, self.z * factor)

    def dot(self, other: "Vector3Sample") -> float:
        return self.x * other.x + self.y * other.y + self.z * other.z

    def cosine_with(self, other: "Vector3Sample") -> float | None:
        left = self.magnitude
        right = other.magnitude
        if left <= 1e-9 or right <= 1e-9:
            return None

        cosine = self.dot(other) / (left * right)
        return max(-1.0, min(1.0, cosine))

    def as_tuple(self) -> tuple[float, float, float]:
        return (self.x, self.y, self.z)


@dataclass(frozen=True, slots=True)
class AeroResidualResult:
    actual_acceleration: Vector3Sample
    gravity_acceleration: Vector3Sample
    engine_acceleration: Vector3Sample
    expected_non_aero_acceleration: Vector3Sample
    residual_aero_acceleration: Vector3Sample
    lift_acceleration_candidate: Vector3Sample
    drag_acceleration_candidate: Vector3Sample
    total_aero_acceleration_candidate: Vector3Sample
    drag_delta: Vector3Sample
    total_aero_delta: Vector3Sample
    drag_alignment_cosine: float | None
    total_aero_alignment_cosine: float | None


def snapshot_vector(snapshot: object, prefix: str) -> Vector3Sample:
    return Vector3Sample(
        float(getattr(snapshot, f"{prefix}_x")),
        float(getattr(snapshot, f"{prefix}_y")),
        float(getattr(snapshot, f"{prefix}_z")),
    )


def compute_aero_residual(snapshot: object) -> AeroResidualResult:
    actual_acceleration = snapshot_vector(snapshot, "acceleration")
    gravity_acceleration = snapshot_vector(snapshot, "gravity_acceleration")

    mass = max(float(snapshot.mass), 1e-9)
    engine_acceleration = snapshot_vector(snapshot, "engine_force").scale(1.0 / mass)
    expected_non_aero_acceleration = engine_acceleration.add(gravity_acceleration)
    residual_aero_acceleration = actual_acceleration.subtract(expected_non_aero_acceleration)

    lift_acceleration_candidate = snapshot_vector(snapshot, "lift_force").scale(1.0 / mass)
    drag_acceleration_candidate = snapshot_vector(snapshot, "drag_force").scale(1.0 / mass)
    total_aero_acceleration_candidate = lift_acceleration_candidate.add(drag_acceleration_candidate)
    drag_delta = residual_aero_acceleration.subtract(drag_acceleration_candidate)
    total_aero_delta = residual_aero_acceleration.subtract(total_aero_acceleration_candidate)

    return AeroResidualResult(
        actual_acceleration=actual_acceleration,
        gravity_acceleration=gravity_acceleration,
        engine_acceleration=engine_acceleration,
        expected_non_aero_acceleration=expected_non_aero_acceleration,
        residual_aero_acceleration=residual_aero_acceleration,
        lift_acceleration_candidate=lift_acceleration_candidate,
        drag_acceleration_candidate=drag_acceleration_candidate,
        total_aero_acceleration_candidate=total_aero_acceleration_candidate,
        drag_delta=drag_delta,
        total_aero_delta=total_aero_delta,
        drag_alignment_cosine=residual_aero_acceleration.cosine_with(drag_acceleration_candidate),
        total_aero_alignment_cosine=residual_aero_acceleration.cosine_with(total_aero_acceleration_candidate),
    )
