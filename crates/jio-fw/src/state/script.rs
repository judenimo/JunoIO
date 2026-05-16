use super::{JunoState, StateCtx, Transition};

enum ScriptStep {
    Act(Box<dyn FnMut(&mut StateCtx<'_>) + Send>),
    Wait(f64),
    WaitUntil(Box<dyn FnMut(&mut StateCtx<'_>) -> bool + Send>),
}

pub struct Script {
    steps: Vec<ScriptStep>,
    idx: usize,
    step_entered_at: Option<f64>,
    on_done: Option<Box<dyn JunoState>>,
}

impl Script {
    pub fn new() -> Self {
        Self { steps: vec![], idx: 0, step_entered_at: None, on_done: None }
    }

    /// Run a closure once, then immediately advance to the next step.
    pub fn act(mut self, f: impl FnMut(&mut StateCtx<'_>) + Send + 'static) -> Self {
        self.steps.push(ScriptStep::Act(Box::new(f)));
        self
    }

    /// Block for `secs` seconds of mission time before advancing.
    pub fn wait(mut self, secs: f64) -> Self {
        self.steps.push(ScriptStep::Wait(secs));
        self
    }

    /// Block each tick, calling `f(ctx)` until it returns `true`. The closure
    /// receives `&mut StateCtx` so it can issue control commands while waiting.
    pub fn wait_until(mut self, f: impl FnMut(&mut StateCtx<'_>) -> bool + Send + 'static) -> Self {
        self.steps.push(ScriptStep::WaitUntil(Box::new(f)));
        self
    }

    /// Configure the terminal transition emitted after all steps complete.
    /// Without this, the script emits `Transition::Done`.
    pub fn then(mut self, next: impl JunoState) -> Self {
        self.on_done = Some(Box::new(next));
        self
    }
}

impl Default for Script {
    fn default() -> Self { Self::new() }
}

impl JunoState for Script {
    fn step(&mut self, ctx: &mut StateCtx<'_>) -> Transition {
        loop {
            let Some(step) = self.steps.get_mut(self.idx) else {
                return match self.on_done.take() {
                    Some(next) => Transition::Next(next),
                    None => Transition::Done,
                };
            };

            let advance = match step {
                ScriptStep::Act(f) => { f(ctx); true }
                ScriptStep::Wait(secs) => {
                    let start = *self.step_entered_at.get_or_insert(ctx.now);
                    ctx.now >= start + *secs
                }
                ScriptStep::WaitUntil(f) => f(ctx),
            };

            if advance {
                self.idx += 1;
                self.step_entered_at = None;
            } else {
                return Transition::Stay;
            }
        }
    }
}
