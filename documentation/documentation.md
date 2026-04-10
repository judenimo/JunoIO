# JunoIO Documentation

## Purpose Of This Document

This is the context-rich reference for JunoIO.

Use it when you want:

- the supported v0.1 core as described in `README.md`
- the broader implemented surface that still exists for development
- runtime and protocol details
- enough context to resume development from a fresh clone

`README.md` is the normal public entrypoint for the project. This file exists to preserve the deeper context that would be annoying to rediscover later.

## What JunoIO Is

JunoIO is a real-time bridge between:

- a Juno: New Origins mod written in C# / Unity
- a Python runtime and package used for scripting flight logic

The bridge is local-only and loop-driven.

At a high level:

1. Juno samples telemetry for the active craft and any explicitly tracked non-active crafts.
2. Juno sends a fixed JSON telemetry envelope to Python over UDP.
3. Python parses that envelope and updates:
   - `self.juno.state`
   - `self.juno.active_craft`
   - `self.juno.crafts`
   - `self.juno.target`
4. Python runs your program's `loop()` method.
5. Your program writes outputs to `self.juno.control` or other supported command helpers.
6. Python sends a fixed JSON control packet back to Juno over UDP.
7. Juno applies the latest control values to the active craft only.

This is not REST and not a scheduler above your program. `loop()` remains the true per-tick entry point. FSM and generator-based `script()` are layered on top of that same model.

## Source Of Truth

Important live source directories:

- Unity mod source:
  - `..\Assets\Scripts`
- Python package source:
  - `src\junoio`

Important runtime location:

- the mod Juno actually loads:
  - `C:\Users\[user]\AppData\LocalLow\Jundroo\SimpleRockets 2\Mods\JunoIO.sr2-mod`

Mirroring rule:

- if Unity-side source changes under `Assets\Scripts`, mirror the same source changes into `juno-io-python\Scripts`

## Doc Split

The package now uses two main docs:

- [README.md](README.md)
  - public-facing overview, quick start, and supported v0.1 surface
- [documentation.md](documentation.md)
  - richer development reference, including implemented but lighter-tested APIs

`juno-dev.md` is the shorter development handoff.

## Core Architecture

### Unity / Juno side

The mod entry point is `Mod.cs`.

When Juno loads the mod:

- `OnModInitialized()` creates a persistent `GameObject`
- that object gets a `JunoIoBridge` component
- `JunoIoBridge.cs` becomes the live bridge runtime inside Juno

`JunoIoBridge.cs` currently handles:

- opening UDP sockets
- telemetry sampling
- fixed-schema JSON writing
- control receive and apply for the active craft
- RPC request / response handling
- extra helper command handling

Maintainability note:

- `JunoIoBridge.cs` is large
- that is acceptable for now
- future cleanup should be gradual and conservative, not a risky rewrite

### Python side

Main pieces:

- `api.py`
  - public script-facing API
- `runtime.py`
  - loads the user program and drives the loop
- `transport.py`
  - UDP transport and JSON parsing/building
- `models.py`
  - fixed snapshot schema and telemetry envelope model
- `cli.py`
  - command-line entry point

## Networking Model

JunoIO uses UDP over localhost.

Current addresses:

- telemetry: `127.0.0.1:5005`
- commands: `127.0.0.1:5006`
- RPC requests: `127.0.0.1:5007`
- RPC responses: `127.0.0.1:5008`

Current behavior:

- Juno sends telemetry to Python at roughly `20 Hz`
- Python sends controls back to Juno continuously while connected
- Python sends occasional RPC queries for discovery and structured queries
- there is no HTTP, REST, or subscription system

## Packet Shapes

### Telemetry envelope

Telemetry is a fixed top-level envelope:

```json
{
  "type": "telemetry",
  "seq": 123,
  "t": 42.0,
  "active_craft_id": "craft_5",
  "crafts": {
    "craft_5": {
      "craft_id": "craft_5",
      "name": "Booster",
      "state": {
        "...many fixed fields..."
      }
    }
  },
  "target": {
    "target_type": "craft",
    "target_id": "craft_2",
    "name": "Payload",
    "position": { "x": 1.0, "y": 2.0, "z": 3.0 },
    "velocity": { "x": 4.0, "y": 5.0, "z": 6.0 }
  }
}
```

Important rules:

- `craft_id` is canonical
- names are metadata only and may duplicate
- the active craft is always present
- tracked non-active crafts are only present when explicitly configured and currently available
- unavailable tracked crafts are omitted
- the current selected nav target is emitted as a top-level `target` block when available

### Control packet

Current core control packet shape:

```json
{
  "seq": 123,
  "control": {
    "throttle": 0.5,
    "pitch": 0.0,
    "roll": 0.0,
    "yaw": 0.0,
    "brake": 0.0,
    "translate_forward": 0.0,
    "translate_right": 0.0,
    "translate_up": 0.0,
    "slider_1": 0.0,
    "slider_2": 0.0,
    "slider_3": 0.0,
    "slider_4": 0.0,
    "translation_mode_enabled": false
  }
}
```

Other top-level payload blocks may also appear for features such as display, stage activation, heading lock, messages, or other helpers.

### RPC packets

Example request:

```json
{
  "type": "rpc_request",
  "id": 1,
  "method": "list_crafts",
  "args": {}
}
```

Example tracking request:

```json
{
  "type": "rpc_request",
  "id": 2,
  "method": "set_tracked_crafts",
  "args": {
    "craft_ids": ["craft_5", "craft_9"]
  }
}
```

## Supported v0.1 Core

### Runtime contract

Supported public runtime model:

- `JunoProgram.on_start()`
- `JunoProgram.loop()`
- built-in FSM helpers
- generator-based `script()`
- `script()` waits:
  - `yield from self.wait_seconds(...)`
  - `yield from self.wait_until(...)`
  - facade equivalents on `self.juno`

Important wait semantics:

- `script()` may either return `None` for a one-shot script or be a generator for cross-tick sequencing
- `wait_seconds(...)` uses simulation time from `self.juno.state.time`
- `wait_seconds(0)` is a guaranteed next-tick yield
- `wait_until(...)` accepts either a state-taking callable or a zero-arg callable
- waits suspend the script task, not the runtime
- `loop()` remains the top-level per-tick hook even when `script()` is used

### Active craft and targets

Primary objects:

- `self.juno.state`
- `self.juno.active_craft`
- `self.juno.target`

Supported target types:

- `craft`
- `body`
- `landmark`
- `position`
- `part`

### Discovery and tracking

Supported helpers:

- `self.juno.list_crafts()`
- `self.juno.list_craft_names()`
- `self.juno.list_craft_ids()`
- `self.juno.get_craft(...)`
- `self.juno.get_craft_name(...)`
- `self.juno.get_crafts_by_name(...)`
- `self.juno.get_craft_by_name(...)`
- `self.juno.set_tracked_crafts([...])`

Important rules:

- `self.juno.state` is active-craft only
- tracked craft state is accessed via `self.juno.crafts["craft_..."].state`
- `self.juno.crafts_by_name` returns lists because names are not unique
- `get_craft_by_name(...)` raises `JunoCraftAmbiguityError` on ambiguity

### Bodies, parts, and world queries

Supported body helpers:

- `self.juno.list_bodies()`
- `self.juno.get_body(...)`
- `self.juno.bodies[...]`

Supported part helpers:

- `self.juno.list_parts(...)`
- `self.juno.get_part(...)`
- `craft.parts[...]`

Supported world helpers:

- `self.juno.world.to_lat_lon_agl(...)`
- `self.juno.world.to_lat_lon_asl(...)`
- `self.juno.world.to_position(...)`
- `self.juno.world.get_terrain_height(...)`
- `self.juno.world.cast_ray(...)`

### Active-craft control surface

Supported control fields:

- `throttle`
- `pitch`
- `roll`
- `yaw`
- `brake`
- `translate_forward`
- `translate_right`
- `translate_up`
- `slider_1`
- `slider_2`
- `slider_3`
- `slider_4`
- `translation_mode_enabled`

Semantics:

- `throttle` and `brake` clamp to `[0, 1]`
- pitch / roll / yaw clamp to `[-1, 1]`
- translation axes clamp to `[-1, 1]`
- sliders clamp to `[-1, 1]`
- `translation_mode_enabled` is boolean

## Additional Implemented APIs Kept For Development

These are implemented and intentionally still documented here because they matter for future development continuity. They are not all equally tested or emphasized in the public release doc.

### Messaging and events

Available messaging helpers:

- `self.juno.broadcast(...)`
- `self.juno.broadcast_to_craft(...)`
- `self.juno.broadcast_to_nearby(...)`
- `self.juno.messages.poll()`
- `self.juno.receive_messages()`
- `on_message(self, message)`

Typed event convenience remains available through:

- `self.juno.events.poll()`
- `on_event(self, event)`

Current limitation:

- message receipt is surfaced when the recipient craft becomes the active telemetry craft

### Convenience command helpers

These helpers are still implemented and useful:

- `self.juno.display(...)`
- `self.juno.flight_log(...)`
- `self.juno.activate_stage()`
- `self.juno.set_time_warp(...)`
- `self.juno.set_heading_lock(...)`
- `self.juno.lock_heading_vector(...)`
- `self.juno.set_attitude_target(...)`
- `self.juno.set_activation_group(...)`
- `self.juno.target_burn_node()`
- `self.juno.set_pid_gains_pitch(...)`
- `self.juno.set_pid_gains_roll(...)`

Treat these as available but not the main v0.1 release story unless they are explicitly included in the release-gate narrative.

## Verification Programs

Core verification examples:

- `examples/verification/lifecycle_persistence.py`
- `examples/verification/state_probe.py`
- `examples/verification/active_control_probe.py`
- `examples/verification/control_step.py`
- `examples/verification/major_controls_probe.py`
- `examples/verification/target_probe.py`
- `examples/verification/tracking_probe.py`
- `examples/verification/rpc_probe.py`
- `examples/verification/fsm_demo.py`
- `examples/verification/fsm_release_check.py`
- `examples/verification/script_wait_release_check.py`
- `examples/script/timed_wait.py`
- `examples/script/conditional_wait.py`
- `examples/script/tick_loop_demo.py`

Additional examples kept for development:

- `examples/verification/timing_probe.py`
- `examples/verification/message_sender.py`
- `examples/verification/message_receiver.py`
- `examples/verification/aero_force_probe.py`

Run examples one at a time with:

```bash
uv run python -m junoio run <path-to-example>
```

## Release Gate

Core Python release gate:

```bash
uv run python -m unittest tests.test_release_gate -v
```

Unity build:

```bash
dotnet build ..\JunoIO.csproj -nologo
```

Recommended live release pass:

```bash
uv run junoio listen --count 1
uv run junoio run examples/verification/state_probe.py
uv run junoio run examples/verification/active_control_probe.py
uv run junoio run examples/verification/major_controls_probe.py
uv run junoio run examples/verification/target_probe.py
uv run junoio run examples/verification/tracking_probe.py
uv run junoio run examples/verification/rpc_probe.py
uv run junoio run examples/verification/fsm_release_check.py
uv run junoio run examples/verification/script_wait_release_check.py
```

## Publishable Repo Cleanup

The publish-facing package root should stay clean.

Keep:

- `src/`
- `tests/`
- `examples/`
- docs
- useful development handoff materials

Move or remove:

- generated probe artifacts
- checked-in `__pycache__` directories
- one-off diagnostics or personal experiments from the package root

Historical one-offs now live under:

- `experimental/`

## Important Gotchas

### `self.juno.state` is active-craft only

For other streamed crafts, use `self.juno.crafts["craft_..."].state`.

### The bridge is loop-driven, not blocking

Do not write your own blocking `while True` in a user program.

### One script process at a time

Only one `junoio run ...` process should be active at a time unless you intentionally want conflicting bridge clients.

### Names are not unique

Use canonical `craft_id` values whenever identity matters.
