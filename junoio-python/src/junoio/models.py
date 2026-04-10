from __future__ import annotations

from dataclasses import dataclass, field


@dataclass(frozen=True, slots=True)
class Snapshot:
    seq: int = -1
    time: float = 0.0
    altitude_asl: float = 0.0
    altitude_agl: float = 0.0
    altitude_terrain: float = 0.0
    mass: float = 0.0
    fuel_mass_scaled: float = 0.0
    remaining_fuel_in_stage_fraction: float = 0.0
    remaining_monopropellant_fraction: float = 0.0
    remaining_battery_fraction: float = 0.0
    surface_speed: float = 0.0
    horizontal_speed: float = 0.0
    vertical_speed: float = 0.0
    orbital_speed: float = 0.0
    pitch: float = 0.0
    bank_angle: float = 0.0
    heading: float = 0.0
    angle_of_attack: float = 0.0
    side_slip: float = 0.0
    latitude_deg: float = 0.0
    longitude_deg: float = 0.0
    distance_from_center: float = 0.0
    acceleration: float = 0.0
    drag_acceleration: float = 0.0
    g_force: float = 0.0
    angular_speed: float = 0.0
    current_engine_thrust: float = 0.0
    current_rcs_thrust_scaled: float = 0.0
    active_engine_count: int = 0
    active_rcs_count: int = 0
    grounded: bool = False
    in_water: bool = False
    supports_warp_burn: bool = False
    mach_number: float = 0.0
    current_isp: float = 0.0
    delta_v_stage: float = 0.0
    thrust_to_weight_ratio: float = 0.0
    remaining_burn_time: float = 0.0
    fuel_all_stages_fraction: float = 0.0
    weighted_throttle_response: float = 0.0
    weighted_throttle_response_time: float = 0.0
    planet_name: str = ""
    planet_radius: float = 0.0
    planet_surface_gravity: float = 0.0
    position_x: float = 0.0
    position_y: float = 0.0
    position_z: float = 0.0
    velocity_x: float = 0.0
    velocity_y: float = 0.0
    velocity_z: float = 0.0
    surface_velocity_x: float = 0.0
    surface_velocity_y: float = 0.0
    surface_velocity_z: float = 0.0
    target_velocity_x: float = 0.0
    target_velocity_y: float = 0.0
    target_velocity_z: float = 0.0
    acceleration_x: float = 0.0
    acceleration_y: float = 0.0
    acceleration_z: float = 0.0
    angular_velocity_x: float = 0.0
    angular_velocity_y: float = 0.0
    angular_velocity_z: float = 0.0
    craft_forward_x: float = 0.0
    craft_forward_y: float = 0.0
    craft_forward_z: float = 0.0
    craft_right_x: float = 0.0
    craft_right_y: float = 0.0
    craft_right_z: float = 0.0
    craft_up_x: float = 0.0
    craft_up_y: float = 0.0
    craft_up_z: float = 0.0
    nav_north_x: float = 0.0
    nav_north_y: float = 0.0
    nav_north_z: float = 0.0
    nav_east_x: float = 0.0
    nav_east_y: float = 0.0
    nav_east_z: float = 0.0
    nav_up_x: float = 0.0
    nav_up_y: float = 0.0
    nav_up_z: float = 0.0
    nav_craft_roll_axis_x: float = 0.0
    nav_craft_roll_axis_y: float = 0.0
    nav_craft_roll_axis_z: float = 0.0
    nav_craft_pitch_axis_x: float = 0.0
    nav_craft_pitch_axis_y: float = 0.0
    nav_craft_pitch_axis_z: float = 0.0
    nav_craft_yaw_axis_x: float = 0.0
    nav_craft_yaw_axis_y: float = 0.0
    nav_craft_yaw_axis_z: float = 0.0
    body_velocity_forward: float = 0.0
    body_velocity_right: float = 0.0
    body_velocity_up: float = 0.0
    surface_body_velocity_forward: float = 0.0
    surface_body_velocity_right: float = 0.0
    surface_body_velocity_up: float = 0.0
    control_throttle: float = 0.0
    control_pitch: float = 0.0
    control_roll: float = 0.0
    control_yaw: float = 0.0
    control_brake: float = 0.0
    control_translate_forward: float = 0.0
    control_translate_right: float = 0.0
    control_translate_up: float = 0.0
    control_slider_1: float = 0.0
    control_slider_2: float = 0.0
    control_slider_3: float = 0.0
    control_slider_4: float = 0.0
    control_translation_mode_enabled: bool = False
    control_pitch_input_received: bool = False
    control_roll_input_received: bool = False
    control_yaw_input_received: bool = False
    environment_air_density: float = 0.0
    environment_air_pressure: float = 0.0
    environment_temperature: float = 0.0
    environment_speed_of_sound: float = 0.0
    environment_atmosphere_height: float = 0.0
    environment_sample_altitude: float = 0.0
    environment_surface_air_density: float = 0.0
    max_engine_thrust: float = 0.0
    gravity_acceleration: float = 0.0
    gravity_acceleration_x: float = 0.0
    gravity_acceleration_y: float = 0.0
    gravity_acceleration_z: float = 0.0
    engine_force_x: float = 0.0
    engine_force_y: float = 0.0
    engine_force_z: float = 0.0
    lift_force_x: float = 0.0
    lift_force_y: float = 0.0
    lift_force_z: float = 0.0
    drag_force_x: float = 0.0
    drag_force_y: float = 0.0
    drag_force_z: float = 0.0
    solar_radiation_intensity: float = 0.0
    orbit_apoapsis_altitude: float = 0.0
    orbit_apoapsis_time: float = 0.0
    orbit_periapsis_altitude: float = 0.0
    orbit_periapsis_time: float = 0.0
    orbit_eccentricity: float = 0.0
    orbit_inclination_rad: float = 0.0
    orbit_period: float = 0.0
    frame_delta_time: float = 0.0
    time_since_launch: float = 0.0
    total_time: float = 0.0
    warp_amount: float = 0.0
    real_time: float = 0.0


@dataclass(frozen=True, slots=True)
class TelemetryCraft:
    craft_id: str = ""
    name: str = ""
    state: Snapshot = field(default_factory=Snapshot)


@dataclass(frozen=True, slots=True)
class TelemetryVector3:
    x: float = 0.0
    y: float = 0.0
    z: float = 0.0


@dataclass(frozen=True, slots=True)
class TelemetryBodyRef:
    body_id: str = ""
    name: str = ""


@dataclass(frozen=True, slots=True)
class TelemetryTarget:
    target_type: str = ""
    target_id: str = ""
    name: str = ""
    parent_body: TelemetryBodyRef | None = None
    position: TelemetryVector3 | None = None
    velocity: TelemetryVector3 | None = None
    craft_id: str | None = None
    part_id: int | None = None
    part_name: str | None = None
    parent_craft_id: str | None = None
    parent_craft_name: str | None = None
    body_id: str | None = None
    radius: float | None = None
    mass: float | None = None
    mu: float | None = None
    angular_velocity: float | None = None
    landmark_id: str | None = None
    latitude_deg: float | None = None
    longitude_deg: float | None = None


@dataclass(frozen=True, slots=True)
class TelemetryEvent:
    event_type: str = ""
    event_id: str = ""
    time: float = 0.0
    craft_id: str | None = None
    craft_name: str | None = None
    sender_craft_id: str | None = None
    sender_craft_name: str | None = None
    other_craft_id: str | None = None
    other_craft_name: str | None = None
    part_id: int | None = None
    part_name: str | None = None
    other_part_id: int | None = None
    other_part_name: str | None = None
    message_name: str | None = None
    message_data: object = None
    delivery_scope: str | None = None
    parent_body: TelemetryBodyRef | None = None
    new_parent_body: TelemetryBodyRef | None = None
    relative_velocity: float | None = None
    impulse: float | None = None
    is_ground_collision: bool | None = None
    point: TelemetryVector3 | None = None
    normal: TelemetryVector3 | None = None


@dataclass(frozen=True, slots=True)
class TelemetryEnvelope:
    seq: int = -1
    time: float = 0.0
    active_craft_id: str = ""
    crafts: dict[str, TelemetryCraft] = field(default_factory=dict)
    target: TelemetryTarget | None = None
    events: tuple[TelemetryEvent, ...] = ()
