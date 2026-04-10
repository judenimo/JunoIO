from junoio import JunoProgram


class HelloDisplayProgram(JunoProgram):
    def on_start(self) -> None:
        self.juno.display("Hello from JunoIO", duration=5)

    def loop(self) -> None:
        pass
