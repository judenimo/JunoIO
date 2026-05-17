use std::f32::consts::PI;

use glam::{Vec2, Vec3};

/// Returns `b − a` normalized to `(−π, π]` radians.
pub fn angle_difference(a: f32, b: f32) -> f32 {
    (b - a + PI).rem_euclid(2.0 * PI) - PI
}

/// Returns `b − a` normalized to `(−180, 180]` degrees.
pub fn angle_difference_deg(a: f32, b: f32) -> f32 {
    (b - a + 180.0).rem_euclid(360.0) - 180.0
}

/// Converts world-space (heading_error, pitch_error) into body-frame (yaw, pitch)
/// control inputs by rotating by the craft's current roll angle.
/// All angles in degrees.
pub fn heading_pitch_to_yaw_pitch(heading_error: f32, pitch_error: f32, roll: f32) -> (f32, f32) {
    let (sin_r, cos_r) = roll.to_radians().sin_cos();
    let yaw = heading_error * cos_r - pitch_error * sin_r;
    let pitch = pitch_error * cos_r + heading_error * sin_r;
    (yaw, pitch)
}

pub fn horizontal_dir_heading(surface_velocity: Vec3, nav_east: Vec3, nav_north: Vec3) -> f32 {
    let h_vel = horizontal_velocity(surface_velocity, nav_east, nav_north);
    h_vel.y.atan2(h_vel.x).to_degrees() + 90.0
}

pub fn horizontal_velocity(surface_velocity: Vec3, nav_east: Vec3, nav_north: Vec3) -> Vec2 {
    let h_vel_east = surface_velocity.dot(nav_east);
    let h_vel_north = surface_velocity.dot(nav_north);
    Vec2::new(h_vel_east, h_vel_north)
}

/// Returns the effective horizontal pitch, which is over 90 degrees when the craft is flying
/// backwards.
pub fn effective_h_pitch(heading: f32, pitch: f32) -> f32 {
    let h_pitch = if heading.abs() <= 90.0 {
        pitch
    } else {
        180.0 - pitch
    };

    h_pitch
}
