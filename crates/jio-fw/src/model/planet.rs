use jio::glam::Vec3;

use super::coords::{Lla, Pci};

pub struct Planet {
    pub radius: f32,
    pub rotation_angle: f32,
}

impl From<&jio::State> for Planet {
    fn from(s: &jio::State) -> Self {
        Planet { radius: s.planet_radius, rotation_angle: 0.0 }
    }
}

impl Planet {
    pub fn pci_to_lla(&self, pci: &Pci) -> Lla {
        let v = pci.0;
        let r = v.length();
        let alt_m = r - self.radius;
        let lat_deg = (v.y / r).asin().to_degrees();
        let inertial_lon = f32::atan2(v.z, v.x);
        let mut lon_deg = (inertial_lon - self.rotation_angle).to_degrees();
        if lon_deg > 180.0 { lon_deg -= 360.0; }
        if lon_deg < -180.0 { lon_deg += 360.0; }
        Lla { lat_deg, lon_deg, alt_m }
    }

    pub fn lla_to_pci(&self, lla: &Lla) -> Pci {
        let lat = lla.lat_deg.to_radians();
        let lon = lla.lon_deg.to_radians();
        let r = self.radius + lla.alt_m;
        let inertial_lon = lon + self.rotation_angle;
        Pci(Vec3::new(
            r * lat.cos() * inertial_lon.cos(),
            r * lat.sin(),
            r * lat.cos() * inertial_lon.sin(),
        ))
    }
}

impl Pci {
    pub fn to_lla(&self, planet: &Planet) -> Lla {
        planet.pci_to_lla(self)
    }
}

impl Lla {
    pub fn to_pci(&self, planet: &Planet) -> Pci {
        planet.lla_to_pci(self)
    }
}
