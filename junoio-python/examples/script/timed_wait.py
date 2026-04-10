from __future__ import annotations

from junoio import JunoProgram


class TimedWaitProgram(JunoProgram):
    def script(self):
        self.juno.display("Starting timed script", duration=1.0)
        yield from self.wait_seconds(3.0)
        self.juno.control.throttle = 0.5
        self.juno.display("Throttle set after 3 seconds", duration=2.0)
