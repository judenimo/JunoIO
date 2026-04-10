from __future__ import annotations

import importlib.util
import inspect
import types
from pathlib import Path
from time import monotonic

from .api import JunoFacade, JunoProgram, juno
from .errors import JunoRpcError
from .models import TelemetryEnvelope
from .transport import UdpBridgeTransport


class JunoRuntime:
    def __init__(
        self,
        facade: JunoFacade | None = None,
        transport: UdpBridgeTransport | None = None,
        state_timeout: float = 0.5,
        send_interval: float = 0.05,
    ) -> None:
        self._facade = facade or juno
        self._transport = transport or UdpBridgeTransport(poll_timeout=min(send_interval / 2.0, 0.02))
        self._facade._set_rpc_caller(self.call_rpc)
        self._state_timeout = state_timeout
        self._send_interval = send_interval
        self._last_snapshot_time = 0.0
        self._last_control_send = 0.0
        self._last_sequence = -1
        self._user_module: types.ModuleType | None = None

    def load_user_script(self, script_path: str | Path) -> JunoProgram:
        self._ensure_rpc_open()

        path = Path(script_path).expanduser().resolve()
        spec = importlib.util.spec_from_file_location("junoio_user_script", path)
        if spec is None or spec.loader is None:
            raise RuntimeError(f"Unable to load user script: {path}")

        module = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(module)
        self._user_module = module

        program_classes = [
            member
            for _, member in inspect.getmembers(module, inspect.isclass)
            if issubclass(member, JunoProgram)
            and member is not JunoProgram
            and member.__module__ == module.__name__
        ]

        if len(program_classes) != 1:
            raise RuntimeError(
                f"User script must define exactly one JunoProgram subclass: {path}"
            )

        program = program_classes[0](self._facade)
        program.on_start()
        return program

    def call_rpc(self, method: str, args: dict[str, object] | None = None, timeout: float = 1.0) -> dict[str, object]:
        self._ensure_rpc_open()

        rpc_caller = getattr(self._transport, "call_rpc", None)
        if not callable(rpc_caller):
            raise JunoRpcError("RPC transport is not available.")

        return rpc_caller(method, args, timeout)

    def process_snapshot(self, telemetry: TelemetryEnvelope, program: JunoProgram, now: float | None = None) -> bool:
        current_time = monotonic() if now is None else now
        if telemetry.seq <= self._last_sequence:
            self.refresh_connection(current_time)
            return False

        self._last_sequence = telemetry.seq
        self._last_snapshot_time = current_time
        new_events = self._facade._apply_telemetry(telemetry)
        self._facade._set_connected(True)
        for event in new_events:
            if getattr(event, "event_type", None) == "message_received":
                program.on_message(event)
            program.on_event(event)

        program.loop()
        self.send_control(current_time)
        return True

    def refresh_connection(self, now: float | None = None) -> bool:
        current_time = monotonic() if now is None else now
        connected = self._last_snapshot_time > 0.0 and current_time - self._last_snapshot_time <= self._state_timeout
        self._facade._set_connected(connected)
        return connected

    def send_control(self, now: float | None = None) -> None:
        current_time = monotonic() if now is None else now
        self._transport.send_control(
            self._facade.state.seq,
            self._facade.control,
            self._facade._consume_display(),
            self._facade._consume_flight_log(),
            self._facade._consume_stage_activation(),
            self._facade._consume_time_warp_mode(),
            self._facade._consume_heading_lock_mode(),
            self._facade._consume_heading_lock_vector(),
            self._facade._consume_attitude_target(),
            self._facade._consume_activation_group(),
            self._facade._consume_target_burn_node(),
            self._facade._consume_pid_gains_pitch(),
            self._facade._consume_pid_gains_roll(),
            self._facade._consume_messages(),
        )
        self._last_control_send = current_time

    def run(self, program: JunoProgram) -> None:
        self._transport.open()

        try:
            while True:
                snapshot = self._transport.receive_latest_snapshot()
                current_time = monotonic()

                if snapshot is not None:
                    self.process_snapshot(snapshot, program, current_time)
                    continue

                connected = self.refresh_connection(current_time)
                if connected and current_time - self._last_control_send >= self._send_interval:
                    self.send_control(current_time)
        finally:
            self._transport.close()

    def _ensure_rpc_open(self) -> None:
        rpc_opener = getattr(self._transport, "open_rpc", None)
        if callable(rpc_opener):
            rpc_opener()
