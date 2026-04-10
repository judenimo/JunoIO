from __future__ import annotations

from junoio import JunoProgram


class TickLoopDemoProgram(JunoProgram):
    def script(self):
        same_tick_values = []
        for index in range(3):
            same_tick_values.append(index)
        print("same tick:", same_tick_values)

        for index in range(3):
            print("cross tick:", index, "time:", self.juno.state.time)
            yield from self.juno.next_tick()

        counter = 0
        while counter < 3:
            print("while tick:", counter, "time:", self.juno.state.time)
            counter += 1
            yield from self.next_tick()
