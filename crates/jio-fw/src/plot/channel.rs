use std::collections::VecDeque;
use std::sync::{Arc, RwLock};

struct ChannelInner {
    label: String,
    buf: RwLock<VecDeque<[f64; 2]>>,
    capacity: usize,
}

#[derive(Clone)]
pub struct Channel(Arc<ChannelInner>);

impl Channel {
    pub(super) fn new(label: impl Into<String>, capacity: usize) -> Self {
        Channel(Arc::new(ChannelInner {
            label: label.into(),
            buf: RwLock::new(VecDeque::with_capacity(capacity)),
            capacity,
        }))
    }

    /// Push a (time, value) sample from the sim thread.
    /// Drops the oldest point when the ring buffer is full.
    pub fn push(&self, t: f64, value: f64) {
        let mut b = self.0.buf.write().unwrap();
        if b.len() == self.0.capacity {
            b.pop_front();
        }
        b.push_back([t, value]);
    }

    pub(super) fn label(&self) -> &str {
        &self.0.label
    }

    pub(super) fn snapshot(&self) -> Vec<[f64; 2]> {
        self.0.buf.read().unwrap().iter().copied().collect()
    }
}
