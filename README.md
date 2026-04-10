# JunoIO

JunoIO is a real-time bridge between Juno: New Origins and Python.

This release bundle contains two parts:

- `junoio-python/`
  - the lightweight Python package for writing and running scripts
- `junoio-unity-mod/`
  - the Unity C# mod project for rebuilding or editing the mod

## Start Here

If you want to write Python scripts:

1. go to `junoio-python/`
2. run `uv sync`
3. create your own script file
4. run `uv run junoio run my_script.py`

If you want to rebuild the Unity mod:

1. open `junoio-unity-mod/` as a Unity project
2. use Unity `2022.3.62f3`
3. build the mod from there

## Layout

- `README.md`
  - top-level overview
- `LICENSE`
  - MIT license for this release bundle
- `CHANGELOG.md`
  - release notes
- `AGENTS.md`
  - tool-agnostic AI / contributor onboarding
- `junoio-python/`
  - Python runtime, CLI, and examples
- `junoio-unity-mod/`
  - Unity mod project snapshot

## v0.1 Core

The intended v0.1 supported core includes:

- active-craft state access
- active-craft control
- current selected target
- craft discovery and tracked-craft state access
- bodies / parts / world query APIs
- required RPC support
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

## Quick Commands

Python side:

```bash
cd junoio-python
uv sync
uv run junoio --help
uv run junoio run examples/hello_display.py
```

Telemetry check:

```bash
cd junoio-python
uv run junoio listen --count 1
```

## Notes

- The Python side assumes the Unity mod is already loaded in Juno: New Origins.
- The Unity project and Python package are intentionally separated so users can take only what they need.
