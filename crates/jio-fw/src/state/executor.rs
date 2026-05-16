use super::{StateCtx, Transition};
use super::traits::JunoState;

pub struct Executor {
    current: Box<dyn JunoState>,
    done: bool,
}

impl Executor {
    pub fn new(initial: impl JunoState) -> Self {
        Self { current: Box::new(initial), done: false }
    }

    pub fn is_done(&self) -> bool {
        self.done
    }

    pub fn step(&mut self, ctx: &mut StateCtx<'_>) {
        if self.done {
            return;
        }
        match self.current.step(ctx) {
            Transition::Stay => {}
            Transition::Next(mut next) => {
                next.init(ctx);
                self.current = next;
            }
            Transition::Done => self.done = true,
        }
    }
}
