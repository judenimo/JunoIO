# JunoIO Python Agent Guide

This file is the tool-agnostic onboarding guide for the standalone `junoio-python` package.

## Purpose

This folder contains the minimum Python-side package needed to write and run JunoIO scripts.

A user should be able to:

1. `cd` into this folder
2. run `uv sync`
3. create their own script file
4. run `uv run junoio run my_script.py`

## Included

- `src/junoio/`
  - runtime, transport, CLI, and public API
- `examples/script/`
  - simple `script()` examples
- `examples/fsm/`
  - simple FSM examples
- `examples/hello_display.py`
  - tiny starter example
- `README.md`
- `pyproject.toml`
- `LICENSE`

## Not Included

This lightweight package intentionally does not include:

- `.venv`
- `tests`
- `experimental`
- Unity-side source mirrors
- the full Unity project

## Runtime Contract

Treat these as the supported Python-side v0.1 surface:

- `self.juno.state`
- `self.juno.active_craft`
- `self.juno.target`
- `self.juno.control`
- craft discovery / tracking helpers
- bodies / parts / world query helpers
- `loop()`
- FSM helpers
- `script()`
- `wait_seconds(...)`
- `wait_until(...)`

Major controls include:

- `throttle`
- `pitch`
- `roll`
- `yaw`
- `brake`
- `translate_forward`
- `translate_right`
- `translate_up`
- `slider_1` through `slider_4`
- `translation_mode_enabled`

## Important Rules

- `loop()` is the top-level per-tick hook on the Python side
- do not assume a Python `update()` user hook exists
- `script()` may be one-shot or generator-based
- `wait_seconds(...)` and `wait_until(...)` are supported
- only one `junoio run ...` process should normally be active at a time
- Juno should already be running in an active flight scene with the Unity mod loaded

## Quick Start

Install dependencies:

```bash
uv sync
```

Check the CLI:

```bash
uv run junoio --help
```

Inspect one telemetry packet:

```bash
uv run junoio listen --count 1
```

Run the starter example:

```bash
uv run junoio run examples/hello_display.py
```

## Minimal Example

```python
from junoio import JunoProgram


class MyProgram(JunoProgram):
    def on_start(self) -> None:
        self.juno.display("Connected", duration=5)

    def loop(self) -> None:
        state = self.juno.state
        if state.altitude_agl < 100.0:
            self.juno.control.throttle = 0.6
        else:
            self.juno.control.throttle = 0.2
```

Run it with:

```bash
uv run junoio run my_script.py
```
