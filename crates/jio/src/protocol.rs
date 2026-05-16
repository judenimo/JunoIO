//! Raw wire types (serde) and conversion into the public `state` types.

use glam::Vec3;
use serde::Deserialize;
use std::collections::HashMap;

use crate::state::{
    BodyRef, Craft, Event, EventCommon, State, Target, TargetCommon, Telemetry,
};

#[derive(Debug, Deserialize)]
pub(crate) struct RawTelemetry {
    #[serde(rename = "type")]
    pub kind: String,
    pub seq: i64,
    #[serde(default, rename = "t")]
    pub t: f64,
    #[serde(default)]
    pub active_craft_id: String,
    #[serde(default)]
    pub crafts: HashMap<String, RawCraft>,
    #[serde(default)]
    pub target: Option<RawTarget>,
    #[serde(default)]
    pub events: Vec<RawEvent>,
}

#[derive(Debug, Deserialize)]
pub(crate) struct RawCraft {
    #[serde(default)]
    pub craft_id: String,
    #[serde(default)]
    pub name: String,
    #[serde(default)]
    pub state: Option<RawState>,
}

#[derive(Debug, Deserialize)]
pub(crate) struct RawVec3 {
    #[serde(default)]
    pub x: f32,
    #[serde(default)]
    pub y: f32,
    #[serde(default)]
    pub z: f32,
}

impl RawVec3 {
    fn to_vec3(&self) -> Vec3 {
        Vec3::new(self.x, self.y, self.z)
    }
}

#[derive(Debug, Deserialize)]
pub(crate) struct RawBodyRef {
    #[serde(default)]
    pub body_id: String,
    #[serde(default)]
    pub name: String,
}

impl RawBodyRef {
    fn into_body_ref(self) -> BodyRef {
        BodyRef {
            body_id: self.body_id,
            name: self.name,
        }
    }
}

#[derive(Debug, Deserialize)]
pub(crate) struct RawTarget {
    #[serde(default)]
    pub target_type: String,
    #[serde(default)]
    pub target_id: String,
    #[serde(default)]
    pub name: String,
    #[serde(default)]
    pub parent_body: Option<RawBodyRef>,
    #[serde(default)]
    pub parent_body_id: Option<String>,
    #[serde(default)]
    pub parent_body_name: Option<String>,
    #[serde(default)]
    pub position: Option<RawVec3>,
    #[serde(default)]
    pub velocity: Option<RawVec3>,
    #[serde(default)]
    pub craft_id: Option<String>,
    #[serde(default)]
    pub part_id: Option<i64>,
    #[serde(default)]
    pub part_name: Option<String>,
    #[serde(default)]
    pub parent_craft_id: Option<String>,
    #[serde(default)]
    pub parent_craft_name: Option<String>,
    #[serde(default)]
    pub body_id: Option<String>,
    #[serde(default)]
    pub radius: Option<f32>,
    #[serde(default)]
    pub mass: Option<f32>,
    #[serde(default)]
    pub mu: Option<f32>,
    #[serde(default)]
    pub angular_velocity: Option<f32>,
    #[serde(default)]
    pub landmark_id: Option<String>,
    #[serde(default)]
    pub latitude_deg: Option<f32>,
    #[serde(default)]
    pub longitude_deg: Option<f32>,
}

#[derive(Debug, Deserialize)]
pub(crate) struct RawEvent {
    #[serde(default)]
    pub event_type: String,
    #[serde(default)]
    pub event_id: String,
    #[serde(default)]
    pub time: f32,
    #[serde(default)]
    pub craft_id: Option<String>,
    #[serde(default)]
    pub craft_name: Option<String>,
    #[serde(default)]
    pub sender_craft_id: Option<String>,
    #[serde(default)]
    pub sender_craft_name: Option<String>,
    #[serde(default)]
    pub other_craft_id: Option<String>,
    #[serde(default)]
    pub other_craft_name: Option<String>,
    #[serde(default)]
    pub part_id: Option<i64>,
    #[serde(default)]
    pub part_name: Option<String>,
    #[serde(default)]
    pub other_part_id: Option<i64>,
    #[serde(default)]
    pub other_part_name: Option<String>,
    #[serde(default)]
    pub message_name: Option<String>,
    #[serde(default)]
    pub message_data: serde_json::Value,
    #[serde(default)]
    pub delivery_scope: Option<String>,
    #[serde(default)]
    pub parent_body: Option<RawBodyRef>,
    #[serde(default)]
    pub parent_body_id: Option<String>,
    #[serde(default)]
    pub parent_body_name: Option<String>,
    #[serde(default)]
    pub new_parent_body: Option<RawBodyRef>,
    #[serde(default)]
    pub relative_velocity: Option<f32>,
    #[serde(default)]
    pub impulse: Option<f32>,
    #[serde(default)]
    pub is_ground_collision: Option<bool>,
    #[serde(default)]
    pub point: Option<RawVec3>,
    #[serde(default)]
    pub normal: Option<RawVec3>,
}

#[derive(Debug, Deserialize)]
pub(crate) struct RawRpcResponse {
    #[serde(rename = "type")]
    pub kind: String,
    pub id: i64,
    pub ok: bool,
    #[serde(default)]
    pub result: serde_json::Value,
    #[serde(default)]
    pub error: Option<RawRpcError>,
}

#[derive(Debug, Deserialize)]
pub(crate) struct RawRpcError {
    #[serde(default)]
    pub code: String,
    #[serde(default)]
    pub message: String,
}

/// Raw flat state block. All fields default to 0/false so a missing key
/// is tolerated, mirroring `_as_float(default=0.0)` in the Python parser.
#[derive(Debug, Deserialize, Default)]
#[serde(default)]
pub(crate) struct RawState {
    pub altitude_asl: f32,
    pub altitude_agl: f32,
    pub altitude_terrain: f32,
    pub mass: f32,
    pub fuel_mass_scaled: f32,
    pub remaining_fuel_in_stage_fraction: f32,
    pub remaining_monopropellant_fraction: f32,
    pub remaining_battery_fraction: f32,
    pub surface_speed: f32,
    pub horizontal_speed: f32,
    pub vertical_speed: f32,
    pub orbital_speed: f32,
    pub pitch: f32,
    pub bank_angle: f32,
    pub heading: f32,
    pub angle_of_attack: f32,
    pub side_slip: f32,
    pub latitude_deg: f32,
    pub longitude_deg: f32,
    pub distance_from_center: f32,
    pub acceleration: f32,
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

    pub position_x: f32,
    pub position_y: f32,
    pub position_z: f32,
    pub velocity_x: f32,
    pub velocity_y: f32,
    pub velocity_z: f32,
    pub surface_velocity_x: f32,
    pub surface_velocity_y: f32,
    pub surface_velocity_z: f32,
    pub target_velocity_x: f32,
    pub target_velocity_y: f32,
    pub target_velocity_z: f32,
    pub acceleration_x: f32,
    pub acceleration_y: f32,
    pub acceleration_z: f32,
    pub angular_velocity_x: f32,
    pub angular_velocity_y: f32,
    pub angular_velocity_z: f32,
    pub craft_forward_x: f32,
    pub craft_forward_y: f32,
    pub craft_forward_z: f32,
    pub craft_right_x: f32,
    pub craft_right_y: f32,
    pub craft_right_z: f32,
    pub craft_up_x: f32,
    pub craft_up_y: f32,
    pub craft_up_z: f32,
    pub nav_north_x: f32,
    pub nav_north_y: f32,
    pub nav_north_z: f32,
    pub nav_east_x: f32,
    pub nav_east_y: f32,
    pub nav_east_z: f32,
    pub nav_up_x: f32,
    pub nav_up_y: f32,
    pub nav_up_z: f32,
    pub nav_craft_roll_axis_x: f32,
    pub nav_craft_roll_axis_y: f32,
    pub nav_craft_roll_axis_z: f32,
    pub nav_craft_pitch_axis_x: f32,
    pub nav_craft_pitch_axis_y: f32,
    pub nav_craft_pitch_axis_z: f32,
    pub nav_craft_yaw_axis_x: f32,
    pub nav_craft_yaw_axis_y: f32,
    pub nav_craft_yaw_axis_z: f32,
    pub body_velocity_forward: f32,
    pub body_velocity_right: f32,
    pub body_velocity_up: f32,
    pub surface_body_velocity_forward: f32,
    pub surface_body_velocity_right: f32,
    pub surface_body_velocity_up: f32,

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

    pub environment_air_density: f32,
    pub environment_air_pressure: f32,
    pub environment_temperature: f32,
    pub environment_speed_of_sound: f32,
    pub environment_atmosphere_height: f32,
    pub environment_sample_altitude: f32,
    pub environment_surface_air_density: f32,
    pub max_engine_thrust: f32,
    pub gravity_acceleration: f32,
    pub gravity_acceleration_x: f32,
    pub gravity_acceleration_y: f32,
    pub gravity_acceleration_z: f32,
    pub engine_force_x: f32,
    pub engine_force_y: f32,
    pub engine_force_z: f32,
    pub lift_force_x: f32,
    pub lift_force_y: f32,
    pub lift_force_z: f32,
    pub drag_force_x: f32,
    pub drag_force_y: f32,
    pub drag_force_z: f32,
    pub solar_radiation_intensity: f32,

    pub orbit_apoapsis_altitude: f32,
    pub orbit_apoapsis_time: f32,
    pub orbit_periapsis_altitude: f32,
    pub orbit_periapsis_time: f32,
    pub orbit_eccentricity: f32,
    pub orbit_inclination_rad: f32,
    pub orbit_period: f32,

    pub frame_delta_time: f32,
    pub time_since_launch: f32,
    pub total_time: f32,
    pub warp_amount: f32,
    pub real_time: f32,
}

impl RawState {
    fn into_state(self, seq: i64, time: f64) -> State {
        State {
            seq,
            time,
            altitude_asl: self.altitude_asl,
            altitude_agl: self.altitude_agl,
            altitude_terrain: self.altitude_terrain,
            mass: self.mass,
            fuel_mass_scaled: self.fuel_mass_scaled,
            remaining_fuel_in_stage_fraction: self.remaining_fuel_in_stage_fraction,
            remaining_monopropellant_fraction: self.remaining_monopropellant_fraction,
            remaining_battery_fraction: self.remaining_battery_fraction,
            surface_speed: self.surface_speed,
            horizontal_speed: self.horizontal_speed,
            vertical_speed: self.vertical_speed,
            orbital_speed: self.orbital_speed,
            pitch: self.pitch,
            bank_angle: self.bank_angle,
            heading: self.heading,
            angle_of_attack: self.angle_of_attack,
            side_slip: self.side_slip,
            latitude_deg: self.latitude_deg,
            longitude_deg: self.longitude_deg,
            distance_from_center: self.distance_from_center,
            acceleration_mag: self.acceleration,
            drag_acceleration: self.drag_acceleration,
            g_force: self.g_force,
            angular_speed: self.angular_speed,
            current_engine_thrust: self.current_engine_thrust,
            current_rcs_thrust_scaled: self.current_rcs_thrust_scaled,
            active_engine_count: self.active_engine_count,
            active_rcs_count: self.active_rcs_count,
            grounded: self.grounded,
            in_water: self.in_water,
            supports_warp_burn: self.supports_warp_burn,
            mach_number: self.mach_number,
            current_isp: self.current_isp,
            delta_v_stage: self.delta_v_stage,
            thrust_to_weight_ratio: self.thrust_to_weight_ratio,
            remaining_burn_time: self.remaining_burn_time,
            fuel_all_stages_fraction: self.fuel_all_stages_fraction,
            weighted_throttle_response: self.weighted_throttle_response,
            weighted_throttle_response_time: self.weighted_throttle_response_time,
            planet_name: self.planet_name,
            planet_radius: self.planet_radius,
            planet_surface_gravity: self.planet_surface_gravity,

            position: Vec3::new(self.position_x, self.position_y, self.position_z),
            velocity: Vec3::new(self.velocity_x, self.velocity_y, self.velocity_z),
            surface_velocity: Vec3::new(
                self.surface_velocity_x,
                self.surface_velocity_y,
                self.surface_velocity_z,
            ),
            target_velocity: Vec3::new(
                self.target_velocity_x,
                self.target_velocity_y,
                self.target_velocity_z,
            ),
            acceleration: Vec3::new(
                self.acceleration_x,
                self.acceleration_y,
                self.acceleration_z,
            ),
            angular_velocity: Vec3::new(
                self.angular_velocity_x,
                self.angular_velocity_y,
                self.angular_velocity_z,
            ),
            craft_forward: Vec3::new(
                self.craft_forward_x,
                self.craft_forward_y,
                self.craft_forward_z,
            ),
            craft_right: Vec3::new(self.craft_right_x, self.craft_right_y, self.craft_right_z),
            craft_up: Vec3::new(self.craft_up_x, self.craft_up_y, self.craft_up_z),
            nav_north: Vec3::new(self.nav_north_x, self.nav_north_y, self.nav_north_z),
            nav_east: Vec3::new(self.nav_east_x, self.nav_east_y, self.nav_east_z),
            nav_up: Vec3::new(self.nav_up_x, self.nav_up_y, self.nav_up_z),
            nav_craft_roll_axis: Vec3::new(
                self.nav_craft_roll_axis_x,
                self.nav_craft_roll_axis_y,
                self.nav_craft_roll_axis_z,
            ),
            nav_craft_pitch_axis: Vec3::new(
                self.nav_craft_pitch_axis_x,
                self.nav_craft_pitch_axis_y,
                self.nav_craft_pitch_axis_z,
            ),
            nav_craft_yaw_axis: Vec3::new(
                self.nav_craft_yaw_axis_x,
                self.nav_craft_yaw_axis_y,
                self.nav_craft_yaw_axis_z,
            ),
            body_velocity: Vec3::new(
                self.body_velocity_forward,
                self.body_velocity_right,
                self.body_velocity_up,
            ),
            surface_body_velocity: Vec3::new(
                self.surface_body_velocity_forward,
                self.surface_body_velocity_right,
                self.surface_body_velocity_up,
            ),

            control_throttle: self.control_throttle,
            control_pitch: self.control_pitch,
            control_roll: self.control_roll,
            control_yaw: self.control_yaw,
            control_brake: self.control_brake,
            control_translate_forward: self.control_translate_forward,
            control_translate_right: self.control_translate_right,
            control_translate_up: self.control_translate_up,
            control_slider_1: self.control_slider_1,
            control_slider_2: self.control_slider_2,
            control_slider_3: self.control_slider_3,
            control_slider_4: self.control_slider_4,
            control_translation_mode_enabled: self.control_translation_mode_enabled,
            control_pitch_input_received: self.control_pitch_input_received,
            control_roll_input_received: self.control_roll_input_received,
            control_yaw_input_received: self.control_yaw_input_received,

            environment_air_density: self.environment_air_density,
            environment_air_pressure: self.environment_air_pressure,
            environment_temperature: self.environment_temperature,
            environment_speed_of_sound: self.environment_speed_of_sound,
            environment_atmosphere_height: self.environment_atmosphere_height,
            environment_sample_altitude: self.environment_sample_altitude,
            environment_surface_air_density: self.environment_surface_air_density,

            max_engine_thrust: self.max_engine_thrust,
            gravity_acceleration_mag: self.gravity_acceleration,
            gravity_acceleration: Vec3::new(
                self.gravity_acceleration_x,
                self.gravity_acceleration_y,
                self.gravity_acceleration_z,
            ),
            engine_force: Vec3::new(
                self.engine_force_x,
                self.engine_force_y,
                self.engine_force_z,
            ),
            lift_force: Vec3::new(self.lift_force_x, self.lift_force_y, self.lift_force_z),
            drag_force: Vec3::new(self.drag_force_x, self.drag_force_y, self.drag_force_z),
            solar_radiation_intensity: self.solar_radiation_intensity,

            orbit_apoapsis_altitude: self.orbit_apoapsis_altitude,
            orbit_apoapsis_time: self.orbit_apoapsis_time,
            orbit_periapsis_altitude: self.orbit_periapsis_altitude,
            orbit_periapsis_time: self.orbit_periapsis_time,
            orbit_eccentricity: self.orbit_eccentricity,
            orbit_inclination_rad: self.orbit_inclination_rad,
            orbit_period: self.orbit_period,

            frame_delta_time: self.frame_delta_time,
            time_since_launch: self.time_since_launch,
            total_time: self.total_time,
            warp_amount: self.warp_amount,
            real_time: self.real_time,
        }
    }
}

impl RawTelemetry {
    pub(crate) fn vectorize(self) -> Option<Telemetry> {
        if self.kind != "telemetry" {
            return None;
        }

        let seq = self.seq;
        let time = self.t;

        let crafts = self
            .crafts
            .into_iter()
            .filter_map(|(id, raw)| {
                let state = raw.state?.into_state(seq, time);
                let craft_id = if raw.craft_id.is_empty() {
                    id.clone()
                } else {
                    raw.craft_id
                };
                Some((
                    id,
                    Craft {
                        craft_id,
                        name: raw.name,
                        state,
                    },
                ))
            })
            .collect();

        Some(Telemetry {
            seq,
            time,
            active_craft_id: self.active_craft_id,
            crafts,
            target: self.target.map(RawTarget::into_target),
            events: self.events.into_iter().map(RawEvent::into_event).collect(),
        })
    }
}

impl RawTarget {
    fn into_target(self) -> Target {
        let parent_body = match (self.parent_body, self.parent_body_id, self.parent_body_name) {
            (Some(nested), _, _) => Some(nested.into_body_ref()),
            (None, Some(id), name) => Some(BodyRef {
                body_id: id,
                name: name.unwrap_or_default(),
            }),
            (None, None, Some(name)) => Some(BodyRef {
                body_id: String::new(),
                name,
            }),
            _ => None,
        };

        let base = TargetCommon {
            target_id: self.target_id,
            name: self.name,
            parent_body,
            position: self.position.map(|v| v.to_vec3()),
            velocity: self.velocity.map(|v| v.to_vec3()),
        };

        match self.target_type.as_str() {
            "craft" => Target::Craft {
                base,
                craft_id: self.craft_id,
            },
            "body" => Target::Body {
                base,
                body_id: self.body_id,
                radius: self.radius,
                mass: self.mass,
                mu: self.mu,
                angular_velocity: self.angular_velocity,
            },
            "landmark" => Target::Landmark {
                base,
                landmark_id: self.landmark_id,
                latitude_deg: self.latitude_deg,
                longitude_deg: self.longitude_deg,
            },
            "part" => Target::Part {
                base,
                part_id: self.part_id,
                part_name: self.part_name,
                parent_craft_id: self.parent_craft_id,
                parent_craft_name: self.parent_craft_name,
            },
            "position" => Target::Position { base },
            other => Target::Other {
                target_type: other.to_string(),
                base,
            },
        }
    }
}

impl RawEvent {
    fn into_event(self) -> Event {
        let parent_body = match (self.parent_body, self.parent_body_id, self.parent_body_name) {
            (Some(nested), _, _) => Some(nested.into_body_ref()),
            (None, Some(id), name) => Some(BodyRef {
                body_id: id,
                name: name.unwrap_or_default(),
            }),
            (None, None, Some(name)) => Some(BodyRef {
                body_id: String::new(),
                name,
            }),
            _ => None,
        };

        let base = EventCommon {
            event_id: self.event_id,
            time: self.time,
            craft_id: self.craft_id,
            craft_name: self.craft_name,
            parent_body,
        };

        match self.event_type.as_str() {
            "collision" => Event::Collision {
                base,
                part_id: self.part_id,
                part_name: self.part_name,
                other_part_id: self.other_part_id,
                other_part_name: self.other_part_name,
                other_craft_id: self.other_craft_id,
                other_craft_name: self.other_craft_name,
                relative_velocity: self.relative_velocity,
                impulse: self.impulse,
                is_ground_collision: self.is_ground_collision,
                point: self.point.map(|v| v.to_vec3()),
                normal: self.normal.map(|v| v.to_vec3()),
            },
            "part_exploded" => Event::PartExploded {
                base,
                part_id: self.part_id,
                part_name: self.part_name,
            },
            "craft_docked" => Event::CraftDocked {
                base,
                other_craft_id: self.other_craft_id,
                other_craft_name: self.other_craft_name,
            },
            "entered_soi" => Event::EnteredSoi {
                base,
                new_parent_body: self.new_parent_body.map(|r| r.into_body_ref()),
            },
            "message_received" => Event::MessageReceived {
                base,
                message: self.message_name,
                data: self.message_data,
                sender_craft_id: self.sender_craft_id,
                sender_craft_name: self.sender_craft_name,
                delivery_scope: self.delivery_scope,
            },
            other => Event::Other {
                event_type: other.to_string(),
                base,
            },
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn vectorizes_basic_telemetry() {
        let payload = serde_json::json!({
            "type": "telemetry",
            "seq": 7,
            "t": 12.5,
            "active_craft_id": "craft_0",
            "crafts": {
                "craft_0": {
                    "craft_id": "craft_0",
                    "name": "Demo",
                    "state": {
                        "altitude_agl": 42.0,
                        "position_x": 1.0, "position_y": 2.0, "position_z": 3.0,
                        "velocity_x": -1.5, "velocity_y": 0.5, "velocity_z": 9.0,
                        "craft_forward_x": 0.0, "craft_forward_y": 0.0, "craft_forward_z": 1.0
                    }
                }
            }
        });

        let raw: RawTelemetry = serde_json::from_value(payload).unwrap();
        let tel = raw.vectorize().expect("telemetry");
        assert_eq!(tel.seq, 7);
        let craft = tel.active().unwrap();
        assert_eq!(craft.name, "Demo");
        assert_eq!(craft.state.altitude_agl, 42.0);
        assert_eq!(craft.state.position, Vec3::new(1.0, 2.0, 3.0));
        assert_eq!(craft.state.velocity, Vec3::new(-1.5, 0.5, 9.0));
        assert_eq!(craft.state.craft_forward, Vec3::Z);
    }
}
