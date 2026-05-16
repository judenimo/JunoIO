mod executor;
mod script;
mod traits;

pub use executor::Executor;
pub use script::Script;
pub use traits::{JunoState, Transition};

pub struct StateCtx<'a> {
    pub telemetry: &'a jio::Telemetry,
    pub control: &'a mut jio::Control,
    pub client: &'a jio::Client,
    pub dt: f32,
    pub now: f64,
}

impl StateCtx<'_> {
    pub fn craft(&self) -> Option<&jio::Craft> {
        self.telemetry.active()
    }
}
