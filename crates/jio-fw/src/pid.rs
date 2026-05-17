#[derive(Debug, Clone)]
pub struct Pid {
    pub kp: f32,
    pub ki: f32,
    pub kd: f32,
    integral: f32,
    prev_error: f32,
    prev_time: Option<f64>,
}

#[derive(Debug, Clone, Copy, Default, PartialEq)]
pub struct PidSample {
    pub error: f32,
    pub p_term: f32,
    pub i_term: f32,
    pub d_term: f32,
    pub output: f32,
}

impl Pid {
    pub fn new(kp: f32, ki: f32, kd: f32) -> Self {
        Self {
            kp,
            ki,
            kd,
            integral: 0.0,
            prev_error: 0.0,
            prev_time: None,
        }
    }

    pub fn reset(&mut self) {
        self.integral = 0.0;
        self.prev_error = 0.0;
        self.prev_time = None;
    }

    pub fn update_by_error(&mut self, error: f32, now: f64) -> PidSample {
        let Some(prev) = self.prev_time else {
            self.prev_time = Some(now);
            self.prev_error = error;
            return PidSample::default();
        };

        let dt = (now - prev) as f32;
        self.prev_time = Some(now);

        if dt <= 0.0 {
            return PidSample::default();
        }

        self.integral += error * dt;
        let derivative = (error - self.prev_error) / dt;
        self.prev_error = error;

        let p_term = self.kp * error;
        let i_term = self.ki * self.integral;
        let d_term = self.kd * derivative;
        PidSample {
            error,
            p_term,
            i_term,
            d_term,
            output: p_term + i_term + d_term,
        }
    }

    pub fn update(&mut self, current: f32, target: f32, now: f64) -> PidSample {
        let error = target - current;
        self.update_by_error(error, now)
    }
}
