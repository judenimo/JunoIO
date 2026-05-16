//! Public async entry point. The user drives this from their own `loop {}`.

use std::sync::atomic::{AtomicI64, Ordering};
use std::time::Duration;

use serde_json::{json, Value};

use glam::Vec3;

use crate::control::Control;
use crate::error::{Error, Result};
use crate::protocol::{RawRpcResponse, RawTelemetry};
use crate::state::Telemetry;
use crate::transport::{Addresses, UdpBridge};
use crate::world::RayHit;

pub struct Client {
    bridge: UdpBridge,
    last_seq: i64,
    next_rpc_id: AtomicI64,
}

impl Client {
    pub async fn connect() -> Result<Self> {
        Self::connect_with(Addresses::default()).await
    }

    pub async fn connect_with(addrs: Addresses) -> Result<Self> {
        Ok(Self {
            bridge: UdpBridge::bind(addrs).await?,
            last_seq: -1,
            next_rpc_id: AtomicI64::new(1),
        })
    }

    /// Await the next telemetry packet with a higher `seq` than the last
    /// one we returned. Drops stale/replay packets internally.
    pub async fn next_telemetry(&mut self) -> Result<Telemetry> {
        loop {
            let bytes = self.bridge.recv_latest_telemetry().await?;
            let raw: RawTelemetry = match serde_json::from_slice(&bytes) {
                Ok(t) => t,
                Err(_) => continue,
            };
            let Some(tel) = raw.vectorize() else { continue };
            if tel.seq <= self.last_seq {
                continue;
            }
            self.last_seq = tel.seq;
            return Ok(tel);
        }
    }

    /// Fire-and-forget control send. `seq` should typically be the seq of
    /// the telemetry packet you're responding to.
    pub async fn send_control(&self, seq: i64, control: &Control) -> Result<()> {
        self.bridge.send_command(&control.encode(seq)).await
    }

    /// Synchronous RPC: send a request, await the matching response.
    pub async fn rpc(
        &self,
        method: &str,
        args: Value,
        timeout: Duration,
    ) -> Result<Value> {
        let id = self.next_rpc_id.fetch_add(1, Ordering::Relaxed);
        let request = json!({
            "type": "rpc_request",
            "id": id,
            "method": method,
            "args": args,
        });
        let payload = serde_json::to_vec(&request)?;
        self.bridge.send_rpc_request(&payload).await?;

        let method_owned = method.to_string();
        let result = tokio::time::timeout(timeout, async {
            loop {
                let bytes = self.bridge.recv_rpc_response().await?;
                let resp: RawRpcResponse = match serde_json::from_slice(&bytes) {
                    Ok(r) => r,
                    Err(_) => continue,
                };
                if resp.kind != "rpc_response" || resp.id != id {
                    continue;
                }
                if !resp.ok {
                    let msg = resp
                        .error
                        .map(|e| {
                            if e.message.is_empty() {
                                format!("{} ({})", e.code, method_owned)
                            } else {
                                e.message
                            }
                        })
                        .unwrap_or_else(|| format!("rpc failed: {method_owned}"));
                    return Err(Error::Rpc(msg));
                }
                return Ok(resp.result);
            }
        })
        .await;

        match result {
            Ok(inner) => inner,
            Err(_) => Err(Error::RpcTimeout(method.to_string())),
        }
    }

    /// Seq of the last telemetry packet handed to the caller. Useful when
    /// you want to send control without re-reading telemetry.
    pub fn last_seq(&self) -> i64 {
        self.last_seq
    }

    pub async fn terrain_height(
        &self,
        lat_deg: f32,
        lon_deg: f32,
        body: Option<&str>,
    ) -> Result<f32> {
        let result = self
            .rpc(
                "world_get_terrain_height",
                json!({ "lat_deg": lat_deg, "lon_deg": lon_deg, "body": body }),
                Duration::from_secs(5),
            )
            .await?;
        result["height"]
            .as_f64()
            .map(|h| h as f32)
            .ok_or_else(|| Error::Rpc("missing height in terrain response".into()))
    }

    pub async fn cast_ray(
        &self,
        origin: Vec3,
        direction: Vec3,
        max_distance: Option<f32>,
    ) -> Result<Option<RayHit>> {
        let result = self
            .rpc(
                "world_cast_ray",
                json!({
                    "origin":    { "x": origin.x,    "y": origin.y,    "z": origin.z },
                    "direction": { "x": direction.x, "y": direction.y, "z": direction.z },
                    "max_distance": max_distance,
                }),
                Duration::from_secs(5),
            )
            .await?;
        let hit = &result["hit"];
        if hit.is_null() {
            return Ok(None);
        }
        Ok(Some(RayHit {
            hit_type: hit["hit_type"].as_str().unwrap_or("world").to_string(),
            distance: hit["distance"].as_f64().unwrap_or(0.0) as f32,
            point: vec3_from_json(&hit["point"]),
            normal: vec3_from_json(&hit["normal"]),
            craft_id: hit["part"]["craft_id"].as_str().map(|s| s.to_string()),
            part_id: hit["part"]["part_id"]["value"]
                .as_i64()
                .or_else(|| hit["part"]["part_id"].as_i64()),
        }))
    }
}

fn vec3_from_json(v: &Value) -> Vec3 {
    Vec3::new(
        v["x"].as_f64().unwrap_or(0.0) as f32,
        v["y"].as_f64().unwrap_or(0.0) as f32,
        v["z"].as_f64().unwrap_or(0.0) as f32,
    )
}
