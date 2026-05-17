//! Sparse outbound control: only fields the user has touched go on the wire.

use glam::Vec3;
use serde_json::{json, Map, Value};

#[derive(Debug, Clone, Copy)]
pub enum HeadingLockMode {
    None,
    Prograde,
    Retrograde,
    Target,
    BurnNode,
    Current,
}

impl HeadingLockMode {
    fn as_str(self) -> &'static str {
        match self {
            HeadingLockMode::None => "none",
            HeadingLockMode::Prograde => "prograde",
            HeadingLockMode::Retrograde => "retrograde",
            HeadingLockMode::Target => "target",
            HeadingLockMode::BurnNode => "burnnode",
            HeadingLockMode::Current => "current",
        }
    }
}

#[derive(Debug, Clone)]
pub enum MessageScope {
    Broadcast,
    Craft { craft_id: String },
    Nearby { radius: Option<f32> },
}

#[derive(Debug, Clone)]
pub struct OutboundMessage {
    pub scope: MessageScope,
    pub name: String,
    pub data: Value,
}

#[derive(Debug, Default, Clone)]
pub struct Control {
    // Axes
    throttle: Option<f32>,
    pitch: Option<f32>,
    roll: Option<f32>,
    yaw: Option<f32>,
    brake: Option<f32>,
    translate_forward: Option<f32>,
    translate_right: Option<f32>,
    translate_up: Option<f32>,
    slider_1: Option<f32>,
    slider_2: Option<f32>,
    slider_3: Option<f32>,
    slider_4: Option<f32>,
    translation_mode_enabled: Option<bool>,

    // Side channels
    display: Option<(String, f32)>,
    flight_log: Option<(String, bool)>,
    stage_activate_next: bool,
    time_warp: Option<u32>,
    heading_lock: Option<HeadingLockMode>,
    heading_lock_vector: Option<Vec3>,
    attitude_target: Option<(Option<f32>, Option<f32>)>,
    activation_group: Option<(u32, bool)>,
    target_burn_node: bool,
    pid_gains_pitch: Option<(f32, f32, f32)>,
    pid_gains_roll: Option<(f32, f32, f32)>,
    messages: Vec<OutboundMessage>,
}

fn clamp(v: f32, lo: f32, hi: f32) -> f32 {
    if v.is_nan() { lo } else { v.clamp(lo, hi) }
}

impl Control {
    pub fn throttle(&mut self, v: f32) -> &mut Self {
        self.throttle = Some(clamp(v, 0.0, 1.0));
        self
    }
    pub fn pitch(&mut self, v: f32) -> &mut Self {
        self.pitch = Some(clamp(v, -1.0, 1.0));
        self
    }
    pub fn roll(&mut self, v: f32) -> &mut Self {
        self.roll = Some(clamp(v, -1.0, 1.0));
        self
    }
    pub fn yaw(&mut self, v: f32) -> &mut Self {
        self.yaw = Some(clamp(v, -1.0, 1.0));
        self
    }
    pub fn brake(&mut self, v: f32) -> &mut Self {
        self.brake = Some(clamp(v, 0.0, 1.0));
        self
    }
    pub fn translate_forward(&mut self, v: f32) -> &mut Self {
        self.translate_forward = Some(clamp(v, -1.0, 1.0));
        self
    }
    pub fn translate_right(&mut self, v: f32) -> &mut Self {
        self.translate_right = Some(clamp(v, -1.0, 1.0));
        self
    }
    pub fn translate_up(&mut self, v: f32) -> &mut Self {
        self.translate_up = Some(clamp(v, -1.0, 1.0));
        self
    }
    pub fn slider_1(&mut self, v: f32) -> &mut Self {
        self.slider_1 = Some(clamp(v, -1.0, 1.0));
        self
    }
    pub fn slider_2(&mut self, v: f32) -> &mut Self {
        self.slider_2 = Some(clamp(v, -1.0, 1.0));
        self
    }
    pub fn slider_3(&mut self, v: f32) -> &mut Self {
        self.slider_3 = Some(clamp(v, -1.0, 1.0));
        self
    }
    pub fn slider_4(&mut self, v: f32) -> &mut Self {
        self.slider_4 = Some(clamp(v, -1.0, 1.0));
        self
    }
    pub fn translation_mode(&mut self, enabled: bool) -> &mut Self {
        self.translation_mode_enabled = Some(enabled);
        self
    }

    pub fn display(&mut self, message: impl Into<String>, duration: f32) -> &mut Self {
        self.display = Some((message.into(), duration.max(0.0)));
        self
    }
    pub fn flight_log(&mut self, message: impl Into<String>, replace: bool) -> &mut Self {
        self.flight_log = Some((message.into(), replace));
        self
    }
    pub fn stage(&mut self) -> &mut Self {
        self.stage_activate_next = true;
        self
    }
    pub fn time_warp(&mut self, mode: u32) -> &mut Self {
        self.time_warp = Some(mode);
        self
    }
    pub fn heading_lock(&mut self, mode: HeadingLockMode) -> &mut Self {
        self.heading_lock = Some(mode);
        self
    }
    pub fn heading_lock_vector(&mut self, vector: Vec3) -> &mut Self {
        self.heading_lock_vector = Some(vector);
        self
    }
    pub fn attitude_target(
        &mut self,
        heading_deg: Option<f32>,
        pitch_deg: Option<f32>,
    ) -> &mut Self {
        self.attitude_target = Some((heading_deg, pitch_deg));
        self
    }
    pub fn activation_group(&mut self, group: u32, enabled: bool) -> &mut Self {
        self.activation_group = Some((group.max(1), enabled));
        self
    }
    pub fn target_burn_node(&mut self) -> &mut Self {
        self.target_burn_node = true;
        self
    }
    pub fn pid_gains_pitch(&mut self, p: f32, i: f32, d: f32) -> &mut Self {
        self.pid_gains_pitch = Some((p, i, d));
        self
    }
    pub fn pid_gains_roll(&mut self, p: f32, i: f32, d: f32) -> &mut Self {
        self.pid_gains_roll = Some((p, i, d));
        self
    }

    pub fn broadcast(&mut self, message: impl Into<String>, data: Value) -> &mut Self {
        self.messages.push(OutboundMessage {
            scope: MessageScope::Broadcast,
            name: message.into(),
            data,
        });
        self
    }
    pub fn broadcast_to_craft(
        &mut self,
        craft_id: impl Into<String>,
        message: impl Into<String>,
        data: Value,
    ) -> &mut Self {
        self.messages.push(OutboundMessage {
            scope: MessageScope::Craft {
                craft_id: craft_id.into(),
            },
            name: message.into(),
            data,
        });
        self
    }
    pub fn broadcast_to_nearby(
        &mut self,
        message: impl Into<String>,
        data: Value,
        radius: Option<f32>,
    ) -> &mut Self {
        self.messages.push(OutboundMessage {
            scope: MessageScope::Nearby { radius },
            name: message.into(),
            data,
        });
        self
    }

    /// Drop side-channel one-shots; keep continuous axis settings.
    pub fn clear_transient(&mut self) {
        self.display = None;
        self.flight_log = None;
        self.stage_activate_next = false;
        self.time_warp = None;
        self.heading_lock = None;
        self.heading_lock_vector = None;
        self.attitude_target = None;
        self.activation_group = None;
        self.target_burn_node = false;
        self.pid_gains_pitch = None;
        self.pid_gains_roll = None;
        self.messages.clear();
    }

    /// Drop every field.
    pub fn clear(&mut self) {
        *self = Self::default();
    }

    pub(crate) fn encode(&self, seq: i64) -> Vec<u8> {
        let mut root = Map::new();
        root.insert("seq".into(), json!(seq.max(0)));

        let mut control = Map::new();
        if let Some(v) = self.throttle {
            control.insert("throttle".into(), json!(v));
        }
        if let Some(v) = self.pitch {
            control.insert("pitch".into(), json!(v));
        }
        if let Some(v) = self.roll {
            control.insert("roll".into(), json!(v));
        }
        if let Some(v) = self.yaw {
            control.insert("yaw".into(), json!(v));
        }
        if let Some(v) = self.brake {
            control.insert("brake".into(), json!(v));
        }
        if let Some(v) = self.translate_forward {
            control.insert("translate_forward".into(), json!(v));
        }
        if let Some(v) = self.translate_right {
            control.insert("translate_right".into(), json!(v));
        }
        if let Some(v) = self.translate_up {
            control.insert("translate_up".into(), json!(v));
        }
        if let Some(v) = self.slider_1 {
            control.insert("slider_1".into(), json!(v));
        }
        if let Some(v) = self.slider_2 {
            control.insert("slider_2".into(), json!(v));
        }
        if let Some(v) = self.slider_3 {
            control.insert("slider_3".into(), json!(v));
        }
        if let Some(v) = self.slider_4 {
            control.insert("slider_4".into(), json!(v));
        }
        if let Some(v) = self.translation_mode_enabled {
            control.insert("translation_mode_enabled".into(), json!(v));
        }
        root.insert("control".into(), Value::Object(control));

        if let Some((text, duration)) = &self.display {
            root.insert(
                "display".into(),
                json!({ "message": text, "duration": duration }),
            );
        }
        if let Some((text, replace)) = &self.flight_log {
            root.insert(
                "flight_log".into(),
                json!({ "message": text, "replace": replace }),
            );
        }
        if self.stage_activate_next {
            root.insert("stage".into(), json!({ "activate_next": true }));
        }
        if let Some(mode) = self.time_warp {
            root.insert("time_warp".into(), json!({ "mode": mode }));
        }
        if let Some(mode) = self.heading_lock {
            root.insert("heading_lock".into(), json!({ "mode": mode.as_str() }));
        }
        if let Some(vector) = self.heading_lock_vector {
            root.insert(
                "heading_lock_vector".into(),
                json!({ "x": vector.x, "y": vector.y, "z": vector.z }),
            );
        }
        if let Some((heading_deg, pitch_deg)) = self.attitude_target {
            let mut payload = Map::new();
            if let Some(h) = heading_deg {
                payload.insert("heading_deg".into(), json!(h));
            }
            if let Some(p) = pitch_deg {
                payload.insert("pitch_deg".into(), json!(p));
            }
            if !payload.is_empty() {
                root.insert("attitude_target".into(), Value::Object(payload));
            }
        }
        if let Some((group, enabled)) = self.activation_group {
            root.insert(
                "activation_group".into(),
                json!({ "group": group, "enabled": enabled }),
            );
        }
        if self.target_burn_node {
            root.insert("target_node".into(), json!({ "name": "burnnode" }));
        }
        if let Some((p, i, d)) = self.pid_gains_pitch {
            root.insert("pid_gains_pitch".into(), json!({ "p": p, "i": i, "d": d }));
        }
        if let Some((p, i, d)) = self.pid_gains_roll {
            root.insert("pid_gains_roll".into(), json!({ "p": p, "i": i, "d": d }));
        }
        if !self.messages.is_empty() {
            let messages: Vec<Value> = self
                .messages
                .iter()
                .map(|m| {
                    let mut payload = Map::new();
                    let scope_name = match &m.scope {
                        MessageScope::Broadcast => "broadcast",
                        MessageScope::Craft { .. } => "craft",
                        MessageScope::Nearby { .. } => "nearby",
                    };
                    payload.insert("scope".into(), json!(scope_name));
                    payload.insert("message".into(), json!(m.name));
                    payload.insert("data".into(), m.data.clone());
                    if let MessageScope::Craft { craft_id } = &m.scope {
                        payload.insert("craft_id".into(), json!(craft_id));
                    }
                    if let MessageScope::Nearby { radius: Some(r) } = &m.scope {
                        payload.insert("radius".into(), json!(r));
                    }
                    Value::Object(payload)
                })
                .collect();
            root.insert("messages".into(), Value::Array(messages));
        }

        serde_json::to_vec(&Value::Object(root)).expect("control encode")
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn sparse_encoding_only_includes_set_fields() {
        let mut c = Control::default();
        c.throttle(0.5).pitch(-0.25);
        let bytes = c.encode(3);
        let v: Value = serde_json::from_slice(&bytes).unwrap();
        assert_eq!(v["seq"], 3);
        let control = v["control"].as_object().unwrap();
        assert_eq!(control.len(), 2);
        assert_eq!(control["throttle"], 0.5);
        assert_eq!(control["pitch"], -0.25);
        assert!(v.get("display").is_none());
        assert!(v.get("stage").is_none());
    }

    #[test]
    fn clamps_axes() {
        let mut c = Control::default();
        c.throttle(2.0).pitch(-5.0);
        let bytes = c.encode(1);
        let v: Value = serde_json::from_slice(&bytes).unwrap();
        assert_eq!(v["control"]["throttle"], 1.0);
        assert_eq!(v["control"]["pitch"], -1.0);
    }

    #[test]
    fn side_channels_emit_when_set() {
        let mut c = Control::default();
        c.display("hello", 5.0).stage().heading_lock(HeadingLockMode::Prograde);
        let bytes = c.encode(0);
        let v: Value = serde_json::from_slice(&bytes).unwrap();
        assert_eq!(v["display"]["message"], "hello");
        assert_eq!(v["display"]["duration"], 5.0);
        assert_eq!(v["stage"]["activate_next"], true);
        assert_eq!(v["heading_lock"]["mode"], "prograde");
    }
}
