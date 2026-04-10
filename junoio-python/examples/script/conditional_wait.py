from __future__ import annotations

from junoio import JunoProgram


class ConditionalWaitProgram(JunoProgram):
    def script(self):
        self.juno.control.throttle = 1.0
        yield from self.wait_until(lambda state: state.altitude_asl >= 1000.0)
        self.juno.control.throttle = 0.0
        self.juno.display("Altitude threshold reached", duration=2.0)
