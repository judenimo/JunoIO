mod channel;
mod group;
mod window;

use std::collections::HashMap;
use std::sync::OnceLock;

pub use channel::Channel;
pub use group::{PlotBuilder, PlotGroup};
pub use window::run_plot_window;

static GLOBAL: OnceLock<HashMap<String, Channel>> = OnceLock::new();

pub(crate) fn set_global(map: HashMap<String, Channel>) {
    GLOBAL.set(map).ok();
}

/// Push a (time, value) sample to a named channel.
/// No-op if the channel name is unknown or `PlotGroup::install` hasn't been called.
pub fn push(name: &str, t: f64, value: f64) {
    if let Some(map) = GLOBAL.get() {
        if let Some(ch) = map.get(name) {
            ch.push(t, value);
        }
    }
}
