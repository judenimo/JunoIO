from __future__ import annotations

from junoio import JunoProgram


class TimedConditionalTransitionProgram(JunoProgram):
    def on_start(self) -> None:
        self.altitude_threshold = 30.0
        self.counter = 0
        self.set_mode("prelaunch")

    def mode_prelaunch(self) -> None:
        if self.mode_just_entered:
            print("[fsm] entered prelaunch")
            self.juno.display("Entered prelaunch", duration=2.0)

        self.juno.control.throttle = 0.0

        if self.wait_seconds_done(3.0):
            print("[fsm] transition: prelaunch -> ascent")
            self.juno.display("Transition: prelaunch -> ascent", duration=2.0)
            self.set_mode("ascent")

    def mode_ascent(self) -> None:
        if self.mode_just_entered:
            self.counter = 0
            print("[fsm] entered ascent")
            self.juno.display("Entered ascent", duration=2.0)

        self.counter += 1
        self.juno.control.throttle = 1.0

        if self.counter % 20 == 0:
            print(
                f"[fsm] ascent ticks={self.counter} altitude_agl={self.juno.state.altitude_agl:.2f}"
            )

        if self.wait_until_done(lambda state: state.altitude_agl > self.altitude_threshold):
            print("[fsm] transition: ascent -> done")
            self.juno.display("Transition: ascent -> done", duration=2.0)
            self.set_mode("done")

    def mode_done(self) -> None:
        if self.mode_just_entered:
            print("[fsm] entered done")
            self.juno.display("Entered done", duration=2.0)

        self.juno.control.throttle = 0.0
