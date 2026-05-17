use glam::Vec3;
use std::collections::HashMap;

#[derive(Debug, Clone, Default)]
pub struct State {
    pub seq: i64,
    pub time: f64,

    // Altitudes / mass / fuel
    pub altitude_asl: f32,
    pub altitude_agl: f32,
    pub altitude_terrain: f32,
    pub mass: f32,
    pub fuel_mass_scaled: f32,
    pub remaining_fuel_in_stage_fraction: f32,
    pub remaining_monopropellant_fraction: f32,
    pub remaining_battery_fraction: f32,

    // Speeds
    pub surface_speed: f32,
    pub horizontal_speed: f32,
    pub vertical_speed: f32,
    pub orbital_speed: f32,

    // Attitude scalars
    pub pitch: f32,
    pub bank_angle: f32,
    pub heading: f32,
    pub angle_of_attack: f32,
    pub side_slip: f32,

    pub latitude_deg: f32,
    pub longitude_deg: f32,
    pub distance_from_center: f32,

    pub acceleration_mag: f32,
    pub drag_acceleration: f32,
    pub g_force: f32,
    pub angular_speed: f32,

    pub current_engine_thrust: f32,
    pub current_rcs_thrust_scaled: f32,
    pub active_engine_count: i32,
    pub active_rcs_count: i32,

    pub grounded: bool,
    pub in_water: bool,
    pub supports_warp_burn: bool,

    pub mach_number: f32,
    pub current_isp: f32,
    pub delta_v_stage: f32,
    pub thrust_to_weight_ratio: f32,
    pub remaining_burn_time: f32,
    pub fuel_all_stages_fraction: f32,
    pub weighted_throttle_response: f32,
    pub weighted_throttle_response_time: f32,

    pub planet_name: String,
    pub planet_radius: f32,
    pub planet_surface_gravity: f32,

    // Vectorized
    pub position: Vec3,
    pub velocity: Vec3,
    pub surface_velocity: Vec3,
    pub target_velocity: Vec3,
    pub acceleration: Vec3,
    pub angular_velocity: Vec3,
    pub craft_forward: Vec3,
    pub craft_right: Vec3,
    pub craft_up: Vec3,
    pub nav_north: Vec3,
    pub nav_east: Vec3,
    pub nav_up: Vec3,
    pub nav_craft_roll_axis: Vec3,
    pub nav_craft_pitch_axis: Vec3,
    pub nav_craft_yaw_axis: Vec3,
    pub body_velocity: Vec3,         // forward / right / up
    pub surface_body_velocity: Vec3, // forward / right / up

    // Control read-back (scalar)
    pub control_throttle: f32,
    pub control_pitch: f32,
    pub control_roll: f32,
    pub control_yaw: f32,
    pub control_brake: f32,
    pub control_translate_forward: f32,
    pub control_translate_right: f32,
    pub control_translate_up: f32,
    pub control_slider_1: f32,
    pub control_slider_2: f32,
    pub control_slider_3: f32,
    pub control_slider_4: f32,
    pub control_translation_mode_enabled: bool,
    pub control_pitch_input_received: bool,
    pub control_roll_input_received: bool,
    pub control_yaw_input_received: bool,

    // Environment
    pub environment_air_density: f32,
    pub environment_air_pressure: f32,
    pub environment_temperature: f32,
    pub environment_speed_of_sound: f32,
    pub environment_atmosphere_height: f32,
    pub environment_sample_altitude: f32,
    pub environment_surface_air_density: f32,

    pub max_engine_thrust: f32,
    pub gravity_acceleration_mag: f32,
    pub gravity_acceleration: Vec3,
    pub engine_force: Vec3,
    pub lift_force: Vec3,
    pub drag_force: Vec3,
    pub solar_radiation_intensity: f32,

    // Orbit
    pub orbit_apoapsis_altitude: f32,
    pub orbit_apoapsis_time: f32,
    pub orbit_periapsis_altitude: f32,
    pub orbit_periapsis_time: f32,
    pub orbit_eccentricity: f32,
    pub orbit_inclination_rad: f32,
    pub orbit_period: f32,

    // Time
    pub frame_delta_time: f32,
    pub time_since_launch: f32,
    pub total_time: f32,
    pub warp_amount: f32,
    pub real_time: f32,
}

#[derive(Debug, Clone, Default)]
pub struct Craft {
    pub craft_id: String,
    pub name: String,
    pub state: State,
}

#[derive(Debug, Clone, Default)]
pub struct Telemetry {
    pub seq: i64,
    pub time: f64,
    pub active_craft_id: String,
    pub crafts: HashMap<String, Craft>,
    pub target: Option<Target>,
    pub events: Vec<Event>,
}

impl Telemetry {
    pub fn active(&self) -> Option<&Craft> {
        self.crafts.get(&self.active_craft_id)
    }
}

#[derive(Debug, Clone, Default)]
pub struct BodyRef {
    pub body_id: String,
    pub name: String,
}

#[derive(Debug, Clone, Default)]
pub struct TargetCommon {
    pub target_id: String,
    pub name: String,
    pub parent_body: Option<BodyRef>,
    pub position: Option<Vec3>,
    pub velocity: Option<Vec3>,
}

#[derive(Debug, Clone)]
pub enum Target {
    Craft {
        base: TargetCommon,
        craft_id: Option<String>,
    },
    Body {
        base: TargetCommon,
        body_id: Option<String>,
        radius: Option<f32>,
        mass: Option<f32>,
        mu: Option<f32>,
        angular_velocity: Option<f32>,
    },
    Landmark {
        base: TargetCommon,
        landmark_id: Option<String>,
        latitude_deg: Option<f32>,
        longitude_deg: Option<f32>,
    },
    Part {
        base: TargetCommon,
        part_id: Option<i64>,
        part_name: Option<String>,
        parent_craft_id: Option<String>,
        parent_craft_name: Option<String>,
    },
    Position {
        base: TargetCommon,
    },
    Other {
        target_type: String,
        base: TargetCommon,
    },
}

#[derive(Debug, Clone, Default)]
pub struct EventCommon {
    pub event_id: String,
    pub time: f32,
    pub craft_id: Option<String>,
    pub craft_name: Option<String>,
    pub parent_body: Option<BodyRef>,
}

#[derive(Debug, Clone)]
pub enum Event {
    Collision {
        base: EventCommon,
        part_id: Option<i64>,
        part_name: Option<String>,
        other_part_id: Option<i64>,
        other_part_name: Option<String>,
        other_craft_id: Option<String>,
        other_craft_name: Option<String>,
        relative_velocity: Option<f32>,
        impulse: Option<f32>,
        is_ground_collision: Option<bool>,
        point: Option<Vec3>,
        normal: Option<Vec3>,
    },
    PartExploded {
        base: EventCommon,
        part_id: Option<i64>,
        part_name: Option<String>,
    },
    CraftDocked {
        base: EventCommon,
        other_craft_id: Option<String>,
        other_craft_name: Option<String>,
    },
    EnteredSoi {
        base: EventCommon,
        new_parent_body: Option<BodyRef>,
    },
    MessageReceived {
        base: EventCommon,
        message: Option<String>,
        data: serde_json::Value,
        sender_craft_id: Option<String>,
        sender_craft_name: Option<String>,
        delivery_scope: Option<String>,
    },
    Other {
        event_type: String,
        base: EventCommon,
    },
}
