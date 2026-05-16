use jio::glam::Vec3;

/// Planet-Centered Inertial position vector.
#[derive(Debug, Clone, Copy)]
pub struct Pci(pub Vec3);

/// Geodetic position: latitude, longitude (degrees), altitude (meters).
#[derive(Debug, Clone, Copy)]
pub struct Lla {
    pub lat_deg: f32,
    pub lon_deg: f32,
    pub alt_m: f32,
}
