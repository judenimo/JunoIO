//! `jio` — a tiny Rust client for the JunoIO Unity mod UDP protocol.
//!
//! Four JSON-over-UDP channels on `127.0.0.1`:
//!
//! | Port | Direction      | Purpose                       |
//! | ---- | -------------- | ----------------------------- |
//! | 5005 | Unity → client | Telemetry snapshots + events  |
//! | 5006 | client → Unity | Control + side-channel cmds   |
//! | 5007 | client → Unity | RPC request                   |
//! | 5008 | Unity → client | RPC response                  |
//!
//! Usage shape:
//!
//! ```no_run
//! use jio::{Client, Control, Result};
//!
//! #[tokio::main]
//! async fn main() -> Result<()> {
//!     let mut client = Client::connect().await?;
//!     let mut ctrl = Control::default();
//!     ctrl.display("jio hover", 5.0);
//!
//!     loop {
//!         let tel = client.next_telemetry().await?;
//!         let Some(craft) = tel.active() else { continue };
//!         ctrl.throttle(if craft.state.altitude_agl < 100.0 { 0.6 } else { 0.2 });
//!         client.send_control(tel.seq, &ctrl).await?;
//!         ctrl.clear_transient();
//!     }
//! }
//! ```

mod client;
mod control;
mod error;
mod protocol;
mod state;
mod transport;
mod world;

pub use client::Client;
pub use control::{Control, HeadingLockMode, MessageScope, OutboundMessage};
pub use error::{Error, Result};
pub use state::{BodyRef, Craft, Event, EventCommon, State, Target, TargetCommon, Telemetry};
pub use world::RayHit;
pub use transport::{
    Addresses, DEFAULT_COMMAND_PORT, DEFAULT_HOST, DEFAULT_RPC_REQUEST_PORT,
    DEFAULT_RPC_RESPONSE_PORT, DEFAULT_TELEMETRY_PORT,
};

pub use glam;
