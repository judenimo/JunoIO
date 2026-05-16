//! Minimal flight controller: hold ~100 m AGL with bang-bang throttle.

use jio::{Client, Control, Result};

#[tokio::main]
async fn main() -> Result<()> {
    let mut client = Client::connect().await?;
    let mut ctrl = Control::default();
    ctrl.display("jio hover", 5.0);

    loop {
        let tel = client.next_telemetry().await?;
        let Some(craft) = tel.active() else { continue };

        ctrl.throttle(if craft.state.altitude_agl < 100.0 { 0.6 } else { 0.2 });
        client.send_control(tel.seq, &ctrl).await?;
        ctrl.clear_transient();
    }
}
