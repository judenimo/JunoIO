use glam::Vec3;

pub struct RayHit {
    pub hit_type: String,
    pub distance: f32,
    pub point: Vec3,
    pub normal: Vec3,
    pub craft_id: Option<String>,
    pub part_id: Option<i64>,
}
