use super::{Lla, Pci, Planet};

pub trait CraftExt {
    fn planet(&self) -> Planet;
    fn position_pci(&self) -> Pci;
    fn position_lla(&self) -> Lla;
}

impl CraftExt for jio::Craft {
    fn planet(&self) -> Planet {
        Planet::from(&self.state)
    }

    fn position_pci(&self) -> Pci {
        Pci(self.state.position)
    }

    fn position_lla(&self) -> Lla {
        self.position_pci().to_lla(&self.planet())
    }
}
