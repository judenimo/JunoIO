from __future__ import annotations

from junoio import JunoProgram


class TimedTransitionProgram(JunoProgram):
    def on_start(self) -> None:
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
            print("[fsm] entered ascent")
            self.juno.display("Entered ascent", duration=2.0)

        self.juno.control.throttle = 0.0
