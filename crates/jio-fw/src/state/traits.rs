use super::StateCtx;

pub enum Transition {
    Stay,
    Next(Box<dyn JunoState>),
    Done,
}

impl Transition {
    pub fn next(next: impl JunoState) -> Self {
        Transition::Next(Box::new(next))
    }
}

pub trait JunoState: Send + 'static {
    /// Called once when the executor first enters this state.
    fn init(&mut self, _ctx: &mut StateCtx<'_>) {}
    fn step(&mut self, ctx: &mut StateCtx<'_>) -> Transition;
}
