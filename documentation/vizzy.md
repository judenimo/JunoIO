# Vizzy To JunoIO Reference

This document is a block-by-block Vizzy parity guide for JunoIO.

Its purpose is twofold:

- help a Vizzy user understand how to write the same idea in Python/JunoIO
- track which Vizzy-style capabilities are already present, partially present, or still missing

Important framing:

- many Vizzy blocks map directly to normal Python syntax rather than a special JunoIO API
- when JunoIO already has a dedicated helper, this document shows that exact helper
- when something is not available yet, the entry explains what would likely need implementing

Status meanings:

- `Implemented`
- `Partial`
- `Missing`

## Program Flow

Sequential mission logic that needs cross-tick waiting should usually live in `script()`, not plain `loop()`.

### Wait

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:**

```python
yield from self.wait_seconds(3.0)
```

or:

```python
yield from self.juno.wait_seconds(3.0)
```

- **Notes:** this is non-blocking and only works inside `script()`. In v1, `script()` is generator-based, so `yield from` is required.

### Wait Until

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:**

```python
yield from self.wait_until(lambda state: state.altitude_asl > 1000.0)
```

or:

```python
yield from self.juno.wait_until(lambda state: state.altitude_asl > 1000.0)
```

- **Notes:** this checks the latest live state each telemetry tick. It also supports zero-argument callables.

### Repeat

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use a normal Python loop.

```python
for _ in range(10):
    print("hello")
```

- **Notes:** if you want the loop body to spread across ticks, add a wait or next-tick yield inside the loop.

### While

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use a normal Python `while`.

```python
while self.juno.state.altitude_agl < 100.0:
    yield from self.juno.next_tick()
```

- **Notes:** this is just Python control flow. JunoIO does not need a special `while` helper.

### For

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use a normal Python `for`.

```python
for i in range(1, 11):
    print(i)
```

- **Notes:** Vizzy's `for i from ... to ... by ...` usually maps to `range(...)`, but you may need to adjust the end value because Python ranges are end-exclusive.

### If Then

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use normal Python `if`.

```python
if self.juno.state.altitude_agl < 50.0:
    self.juno.control.throttle = 1.0
```

- **Notes:** this is Python syntax, not a dedicated JunoIO feature.

### Else If Then

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use Python `elif`.

```python
if value < 0:
    print("negative")
elif value == 0:
    print("zero")
```

- **Notes:** direct Python replacement.

### Else

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use Python `else`.

```python
if ready:
    print("go")
else:
    print("wait")
```

- **Notes:** direct Python replacement.

### Display

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:**

```python
self.juno.display("Hello world", duration=2.0)
```

- **Notes:** this shows a flight-scene message in Juno. It is the closest direct equivalent to the Vizzy display block.

### Local Log

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:**

```python
print("debug message")
```

- **Notes:** the natural Python equivalent is normal stdout logging with `print(...)`. There is not currently a separate dedicated `self.juno.local_log(...)` helper.

### Flight Log

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.flight_log(message, replace=False)`.

```python
self.juno.flight_log("Landing burn armed")
self.juno.flight_log("Altitude hold active", replace=True)
```

- **Notes:** this writes to Juno's flight log rather than the transient display message area. When `replace=True`, JunoIO updates the current dynamic Vizzy-style log entry instead of appending a new one each time. When `replace=False`, it adds a fresh entry.

### Break

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use normal Python `break`.

```python
for i in range(10):
    if i == 5:
        break
```

- **Notes:** no special JunoIO break is needed.

### Comment

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use normal Python comments.

```python
# This is a comment.
```

- **Notes:** comments are handled by Python itself and do not need a JunoIO feature.

## Operators

Most Vizzy operator blocks map directly to normal Python operators, built-in functions, or `numpy`. In JunoIO, these are usually not special bridge features. They are just ordinary Python. When both `math` and `numpy` provide a clean equivalent, prefer the `numpy` form for consistency.

### Add

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `+`.

```python
total = a + b
```

- **Notes:** this is normal Python arithmetic.

### Subtract

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `-`.

```python
delta = a - b
```

- **Notes:** this is normal Python arithmetic.

### Divide

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `/`.

```python
ratio = a / b
```

- **Notes:** Python `/` always produces floating-point division.

### Multiply

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `*`.

```python
product = a * b
```

- **Notes:** this is normal Python arithmetic.

### Exponential

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `**`.

```python
value = base ** exponent
```

- **Notes:** `numpy.exp(x)` is also available if you specifically want `e**x`.

### Modulus

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `%`.

```python
remainder = a % b
```

- **Notes:** this is the normal Python remainder operator.

### Random

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use Python's `random` module.

```python
import random

value = random.uniform(low, high)
```

- **Notes:** use `random.randint(a, b)` for inclusive integer ranges, or `random.random()` for a float in `[0.0, 1.0)`.

### Min Of

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `min(...)`.

```python
value = min(a, b)
```

- **Notes:** for vectors/arrays, `numpy.min(...)` may be more appropriate.

### Max Of

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `max(...)`.

```python
value = max(a, b)
```

- **Notes:** for vectors/arrays, `numpy.max(...)` may be more appropriate.

### And

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `and`.

```python
ready = armed and stable
```

- **Notes:** this is normal Python boolean logic.

### Or

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `or`.

```python
ready = landed or splashed
```

- **Notes:** this is normal Python boolean logic.

### Not

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `not`.

```python
ready = not failed
```

- **Notes:** this is normal Python boolean logic.

### Equal To

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `==`.

```python
same = a == b
```

- **Notes:** use `is None` when testing for `None`.

### Less Than

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `<`.

```python
below_limit = value < limit
```

- **Notes:** this is normal Python comparison.

### Less Than Or Equal To

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `<=`.

```python
within_limit = value <= limit
```

- **Notes:** this is normal Python comparison.

### Greater Than

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `>`.

```python
above_limit = value > limit
```

- **Notes:** this is normal Python comparison.

### Greater Than Or Equal To

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `>=`.

```python
within_limit = value >= limit
```

- **Notes:** this is normal Python comparison.

### Abs

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.abs(...)`.

```python
import numpy as np

distance = np.abs(error)
```

- **Notes:** this keeps scalar and array usage consistent.

### Floor

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.floor(...)`.

```python
import numpy as np

value = np.floor(x)
```

- **Notes:** NumPy keeps this consistent with the other numeric operators.

### Ceiling

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.ceil(...)`.

```python
import numpy as np

value = np.ceil(x)
```

- **Notes:** NumPy keeps this consistent with the other numeric operators.

### Round

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.round(...)`.

```python
import numpy as np

value = np.round(x)
```

- **Notes:** Python and NumPy have slightly different rounding details in some edge cases, so use one style consistently within a project.

### Sqrt

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.sqrt(...)`.

```python
import numpy as np

value = np.sqrt(x)
```

- **Notes:** this works cleanly for both scalars and arrays.

### Sin

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.sin(...)`.

```python
import numpy as np

value = np.sin(angle_rad)
```

- **Notes:** NumPy trig functions use radians.

### Cos

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.cos(...)`.

```python
import numpy as np

value = np.cos(angle_rad)
```

- **Notes:** NumPy trig functions use radians.

### Tan

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.tan(...)`.

```python
import numpy as np

value = np.tan(angle_rad)
```

- **Notes:** NumPy trig functions use radians.

### Asin

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.arcsin(...)`.

```python
import numpy as np

angle_rad = np.arcsin(x)
```

- **Notes:** the result is in radians.

### Acos

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.arccos(...)`.

```python
import numpy as np

angle_rad = np.arccos(x)
```

- **Notes:** the result is in radians.

### Atan

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.arctan(...)`.

```python
import numpy as np

angle_rad = np.arctan(x)
```

- **Notes:** use `numpy.arctan2(y, x)` when you need quadrant-aware two-argument behaviour.

### Ln

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.log(...)`.

```python
import numpy as np

value = np.log(x)
```

- **Notes:** this is the natural logarithm.

### Log

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.log10(...)`.

```python
import numpy as np

value = np.log10(x)
```

- **Notes:** use `numpy.log(x) / numpy.log(base)` if you need a different base.

### Deg2Rad

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.deg2rad(...)`.

```python
import numpy as np

angle_rad = np.deg2rad(angle_deg)
```

- **Notes:** this is the clearest equivalent.

### Rad2Deg

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.rad2deg(...)`.

```python
import numpy as np

angle_deg = np.rad2deg(angle_rad)
```

- **Notes:** this is the clearest equivalent.

### If Then Else

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use Python's conditional expression.

```python
value = a if condition else b
```

- **Notes:** for larger logic, use a normal `if` statement instead.

### Length Of

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `len(...)`.

```python
count = len(text)
```

- **Notes:** this works for strings, lists, tuples, dictionaries, and more.

### Letter Of

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use string indexing.

```python
letter = text[index]
```

- **Notes:** Python is zero-indexed, so the first character is `text[0]`.

### Letters To Of

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use string slicing.

```python
chunk = text[start:end]
```

- **Notes:** Python slices are zero-indexed and the end index is exclusive.

### Contains

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `in`.

```python
found = "apo" in text
```

- **Notes:** this works for strings, lists, sets, dictionaries, and more.

### Format

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use f-strings or `str.format(...)`.

```python
text = f"altitude={altitude_agl:.1f} m"
```

- **Notes:** f-strings are usually the cleanest Python equivalent.

### Friendly Acceleration / Angular Velocity / Coordinate / Density / Distance / Energy / Force / Specific Impulse / Mass / Power / Pressure / Temperature / Time / Time (Date) / Velocity

- **Status:** `Partial`
- **How to do it in JunoIO/Python:** format the numeric value yourself using Python strings.

```python
text = f"{self.juno.state.altitude_asl:.1f} m"
```

- **Notes:** Python can format the numbers easily, but JunoIO does not currently ship dedicated "friendly unit string" helpers for every Vizzy unit-format block.
- **If missing:** add helpers like `junoio.formatters.friendly_distance(...)`, `friendly_velocity(...)`, `friendly_time(...)`, and `friendly_coordinate(...)` if you want exact Vizzy-style unit presentation.

### Vec

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use a NumPy array.

```python
import numpy as np

vector = np.array([x, y, z], dtype=float)
```

- **Notes:** JunoIO already exposes many vectors as `numpy.ndarray` values.

Vector coordinate note:
Juno/JunoIO uses vectors in `(x, y, z)` order with `y` as the up direction. In a lot of physics, controls, and aerospace/engineering writing, you will also see `(x, y, z)`, but with `z` treated as up. That means the math operations themselves are not wrong in JunoIO, but your interpretation can be wrong if you mentally assume a `z-up` world.

Practical consequences:
- vertical/up components in JunoIO are usually the `y` component, not `z`
- horizontal-plane reasoning is often the `x-z` plane, not the `x-y` plane
- if you copy formulas, diagrams, or intuition from a `z-up` source, you may need to remap axes before comparing results
- the right-hand rule still works, but you need to apply it to Juno's actual axis convention rather than a `z-up` mental model

This matters most for:
- `cross`
- `angle`
- `project`
- `dot`
- `x / y / z` component access
- any custom frame conversions or heading/pitch calculations

The vector algebra itself is still standard. The main thing to be careful about is which physical direction each component represents.

### Length

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.linalg.norm(...)`.

```python
import numpy as np

length = np.linalg.norm(vector)
```

- **Notes:** this is the standard vector magnitude operation.

### X

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `vector[0]`.

```python
x = vector[0]
```

- **Notes:** vectors are zero-indexed in Python.

### Y

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `vector[1]`.

```python
y = vector[1]
```

- **Notes:** vectors are zero-indexed in Python.

### Z

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `vector[2]`.

```python
z = vector[2]
```

- **Notes:** vectors are zero-indexed in Python.

### Norm

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** divide by the vector magnitude.

```python
import numpy as np

unit = vector / np.linalg.norm(vector)
```

- **Notes:** guard against zero-length vectors if that is possible in your use case.

### Angle

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use the dot product and `numpy.arccos(...)`.

```python
import numpy as np

cos_theta = np.dot(a, b) / (np.linalg.norm(a) * np.linalg.norm(b))
angle_rad = np.arccos(np.clip(cos_theta, -1.0, 1.0))
```

- **Notes:** clamp the cosine value into `[-1, 1]` before calling `acos`.

### Clamp

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.clip(...)`.

```python
import numpy as np

value = np.clip(value, low, high)
```

- **Notes:** this is the preferred NumPy-first clamp form.

### Cross

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.cross(...)`.

```python
import numpy as np

value = np.cross(a, b)
```

- **Notes:** this returns a vector. Be especially careful with Juno's `y-up` axis convention here. The cross product follows the normal right-hand rule, but if you are expecting a `z-up` result from external aerospace/math references, the resulting component directions may look "wrong" until you reinterpret them in Juno's frame. In JunoIO, an "upward" component usually means a positive `y`, not a positive `z`.

### Dot

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.dot(...)` or `a @ b`.

```python
value = a @ b
```

- **Notes:** for 3-vectors, `a @ b` is usually the nicest syntax.

### Dist

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** subtract the vectors and take the norm.

```python
import numpy as np

distance = np.linalg.norm(a - b)
```

- **Notes:** this is the standard Euclidean distance.

### Min

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.minimum(a, b)` for elementwise vector min, or `numpy.min(...)` for a single array minimum.

```python
import numpy as np

value = np.minimum(a, b)
```

- **Notes:** this is different from Python's scalar `min(...)`.

### Max

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `numpy.maximum(a, b)` for elementwise vector max, or `numpy.max(...)` for a single array maximum.

```python
import numpy as np

value = np.maximum(a, b)
```

- **Notes:** this is different from Python's scalar `max(...)`.

### Project

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** project one vector onto another with the dot product.

```python
import numpy as np

projection = (np.dot(a, b) / np.dot(b, b)) * b
```

- **Notes:** guard against `b` being a zero vector.

### Scale

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** multiply the vector by a scalar.

```python
scaled = 3.0 * vector
```

- **Notes:** this is normal NumPy/Python vector scaling.

### Funk

- **Status:** `Partial`
- **How to do it in JunoIO/Python:** if this refers to calling a function-like helper, just call a normal Python function.

```python
value = helper(x, y)
```

- **Notes:** "Funk" does not appear to be a standard current JunoIO feature name. If this is referring to a specific Vizzy block with a different exact name, it should be clarified and documented more precisely.
- **If missing:** only add something special if there is a real Vizzy operator here that cannot already be represented as a normal Python function call.

### True

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `True`.

```python
armed = True
```

- **Notes:** this is the normal Python boolean literal.

### False

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `False`.

```python
armed = False
```

- **Notes:** this is the normal Python boolean literal.

## Craft Instructions

These blocks are more mixed than Program Flow or Operators. Some already map cleanly to the current JunoIO control bridge, while many of Vizzy's higher-level craft, camera, part, and audio actions still need dedicated APIs.

### Activate Stage

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.activate_stage()`.

```python
self.juno.activate_stage()
```

- **Notes:** this is a one-shot command sent on the next control packet.

### Set [Roll / Pitch / Yaw / Throttle] To (number)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** write to `self.juno.control`.

```python
self.juno.control.throttle = 0.8
self.juno.control.pitch = 0.1
self.juno.control.roll = -0.2
self.juno.control.yaw = 0.0
```

- **Notes:** these are direct active-craft control inputs, not higher-level autopilot setpoints.

### Set [Brake] To (number)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.control.brake`.

```python
self.juno.control.brake = 0.5
```

- **Notes:** `brake` is a direct active-craft control input and clamps to `[0, 1]`.

### Set [Slider 1 / Slider 2 / Slider 3 / Slider 4] To (number)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** write to the matching slider field on `self.juno.control`.

```python
self.juno.control.slider_1 = -0.25
self.juno.control.slider_2 = 0.25
self.juno.control.slider_3 = 0.75
self.juno.control.slider_4 = -0.75
```

- **Notes:** slider values clamp to `[-1, 1]`.

### Set [Translate Forward / Translate Right / Translate Up] To (number)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use the translation-axis fields on `self.juno.control`.

```python
self.juno.control.translate_forward = 0.4
self.juno.control.translate_right = -0.2
self.juno.control.translate_up = 0.8
```

- **Notes:** translation axes are direct active-craft inputs and clamp to `[-1, 1]`.

### Set [Translation Mode] To (bool)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** set `self.juno.control.translation_mode_enabled`.

```python
self.juno.control.translation_mode_enabled = True
```

- **Notes:** this toggles the active craft's translation-mode flag in the outgoing control packet.

### Set [Craft Heading] To (number)

- **Status:** `Partial`
- **How to do it in JunoIO/Python:** use `self.juno.set_attitude_target(heading_deg=...)`.

```python
self.juno.set_attitude_target(heading_deg=90.0)
```

- **Notes:** this is a higher-level attitude target helper, not the same as raw yaw input.

### Set [Craft Pitch] To (number)

- **Status:** `Partial`
- **How to do it in JunoIO/Python:** use `self.juno.set_attitude_target(pitch_deg=...)`.

```python
self.juno.set_attitude_target(pitch_deg=10.0)
```

- **Notes:** this is a higher-level pitch target helper, not the same as raw `self.juno.control.pitch`.

### Set [Craft Pitch PIDs] To (p, i, d)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.set_pid_gains_pitch(p, i, d)`.

```python
self.juno.set_pid_gains_pitch(1.0, 0.1, 0.01)
```

- **Notes:** this follows the currently exposed helper surface; treat it as more specialized than the basic v0.1 control inputs.

### Set [Craft Roll PIDs] To (p, i, d)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.set_pid_gains_roll(p, i, d)`.

```python
self.juno.set_pid_gains_roll(1.0, 0.1, 0.01)
```

- **Notes:** this is available, but it is a more specialized helper than the basic control surface.

### Target [Node]

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.target_burn_node()`.

```python
self.juno.target_burn_node()
```

- **Notes:** this is a command helper, not a read of the current target model.

### Set [Activation Group] To (group, enabled)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.set_activation_group(group, enabled=True)`.

```python
self.juno.set_activation_group(3, enabled=True)
```

- **Notes:** this is distinct from stage activation.

### Lock Heading On [None / Prograde / Retrograde / Target / Burnnode / Current]

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.set_heading_lock(...)`.

```python
self.juno.set_heading_lock("prograde")
```

- **Notes:** this is a higher-level guidance helper, not raw control input.

### Lock Heading On [Vector]

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.lock_heading_vector(vector)`.

```python
self.juno.lock_heading_vector((0.0, 1.0, 0.0))
```

- **Notes:** this is separate from raw pitch/roll/yaw inputs.

### Set [Time Mode] To (level)

- **Status:** `Partial`
- **How to do it in JunoIO/Python:** use `self.juno.set_time_warp(level)`.

```python
self.juno.set_time_warp(3)
```

- **Notes:** the current API exposes integer warp modes, not a named Vizzy-style enum like `paused`, `normal`, or `timewarp5`.
- **If missing:** add friendly named constants or a higher-level `set_time_mode(...)` wrapper if you want Vizzy-like ergonomics.

### Set [Camera X Rotation / Y Rotation / Tilt / Zoom / Camera Mode / Camera Index / Target Offset] To (...)

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no current camera-control API.
- **Notes:** camera manipulation is not currently part of the bridge.
- **If missing:** add a dedicated camera command surface if the ModAPI exposes these controls cleanly enough.

### Set Part (...) Activated / Focused / Name / Explode / Fuel Transfer To (...)

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no current part-action command API.
- **Notes:** JunoIO currently exposes craft/state information, not a rich part-command layer.
- **If missing:** add explicit part lookup and part command APIs rather than overloading the craft control surface.

### Switch To Craft

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no current command to change the active craft.
- **Notes:** JunoIO can discover crafts and track their state, but it does not currently switch player control between them.
- **If missing:** add a dedicated craft-switch command if the ModAPI supports it safely.

### Beep (...) Hz At (...) Volume For (...) Seconds

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no current in-game audio/beep API.
- **Notes:** Python can make sounds locally on your machine, but that is not the same as a Juno flight/beep instruction.
- **If missing:** add a Unity-side audio command path if in-game beeps are a real goal.

### Frequency Of (...) Octave (...)

- **Status:** `Partial`
- **How to do it in JunoIO/Python:** this can be represented as a normal helper function if you just need the frequency calculation.

```python
def frequency_of(note_hz: float, octave_shift: int) -> float:
    return note_hz * (2.0 ** octave_shift)
```

- **Notes:** the pure calculation is easy in Python, but there is no dedicated built-in JunoIO music/audio helper and no paired in-game `beep(...)` API yet.
- **If missing:** add a small utility helper only if you want musical/audio ergonomics in the public API.

## Craft Information

This section is a mix of already-available telemetry, newer structured target data, and several still-missing world/part/planet query features. Where possible, use the current `self.juno.state`, `self.juno.target`, and craft-handle APIs directly.

### Altitude AGL

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.state.altitude_agl`.

```python
altitude_agl = self.juno.state.altitude_agl
```

- **Notes:** AGL means Above Ground Level, measured as the vertical distance between the craft's centre of mass and the terrain or water surface below. It does not account for structures, bases, or other crafts beneath you. Because it is measured from the centre of mass, it can change even when the craft appears stationary and its external dimensions do not change.

AGL centre-of-mass note:
As fuel burns or mass shifts around the craft, the centre of mass can move. That means `altitude_agl` can drift even if the lowest physical point of the craft has not moved relative to the ground. This matters especially for:
- hover and landing controllers
- touchdown detection
- fixed-height holds
- any conversion or logic that assumes `AGL == distance from lowest point to ground`

Practical consequences:
- a rocket can report a different AGL at liftoff and near-empty propellant even if its gear/nozzle/shape is unchanged
- landing logic based on a hardcoded `0 m AGL` can be misleading if you really care about geometry-to-ground clearance
- it is often useful to capture an `agl_offset` near the start of a flight phase when you want a more consistent effective touchdown reference

### Altitude ASL

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.state.altitude_asl`.

```python
altitude_asl = self.juno.state.altitude_asl
```

- **Notes:** this is altitude above sea level.

### Altitude ASF

- **Status:** `Partial`
- **How to do it in JunoIO/Python:** there is no dedicated `altitude_asf` field, but `self.juno.state.altitude_terrain` is the nearest current equivalent to terrain-relative altitude.

```python
altitude_terrain = self.juno.state.altitude_terrain
```

- **Notes:** if you need an exact Vizzy ASF semantic, confirm how Juno defines it relative to water and terrain before adding a dedicated alias.
- **If missing:** add an explicit alias only if its meaning is distinct from the current terrain-relative telemetry.

### Orbit Apoapsis / Periapsis / Time To Apoapsis / Time To Periapsis / Eccentricity / Inclination / Period

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use the current orbit telemetry fields on `self.juno.state`.

```python
apoapsis = self.juno.state.orbit_apoapsis_altitude
periapsis = self.juno.state.orbit_periapsis_altitude
time_to_apoapsis = self.juno.state.orbit_apoapsis_time
time_to_periapsis = self.juno.state.orbit_periapsis_time
eccentricity = self.juno.state.orbit_eccentricity
inclination_rad = self.juno.state.orbit_inclination_rad
period = self.juno.state.orbit_period
```

- **Notes:** inclination is currently exposed in radians.

### Performance Engine Thrust / Mass / Dry Mass / Fuel Mass / Max Engine Thrust / TWR / Current ISP / Stage Delta-V / Stage Burn Time

- **Status:** `Partial`
- **How to do it in JunoIO/Python:** use the implemented fields directly where available.

```python
engine_thrust = self.juno.state.current_engine_thrust
mass = self.juno.state.mass
fuel_mass = self.juno.state.fuel_mass_scaled
max_engine_thrust = self.juno.state.max_engine_thrust
twr = self.juno.state.thrust_to_weight_ratio
current_isp = self.juno.state.current_isp
stage_delta_v = self.juno.state.delta_v_stage
stage_burn_time = self.juno.state.remaining_burn_time
```

- **Notes:** `dry_mass` is not currently exposed as its own field.
- **If missing:** add `dry_mass` telemetry explicitly if you want exact Vizzy parity.

### Fuel Battery / Stage / Mono / AllStages

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use the fraction fields on `self.juno.state`.

```python
battery = self.juno.state.remaining_battery_fraction
stage_fuel = self.juno.state.remaining_fuel_in_stage_fraction
mono = self.juno.state.remaining_monopropellant_fraction
all_stages = self.juno.state.fuel_all_stages_fraction
```

- **Notes:** these are currently fractional values rather than named unit objects.

### Nav Position

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.state.position`.

```python
position = self.juno.state.position
```

- **Notes:** this is a vector alias backed by the PCI/world-frame position components.

### Nav Target Position

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** prefer `self.juno.target.position`.

```python
target = self.juno.target
target_position = None if target is None else target.position
```

- **Notes:** this is the preferred structured target API. Avoid relying on old raw target-position-style assumptions.

### Nav Heading / Pitch / Bank Angle / Angle Of Attack / Side Slip

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use the telemetry fields on `self.juno.state`.

```python
heading = self.juno.state.heading
pitch = self.juno.state.pitch
bank_angle = self.juno.state.bank_angle
angle_of_attack = self.juno.state.angle_of_attack
side_slip = self.juno.state.side_slip
```

- **Notes:** these are the current active-craft navigation/orientation scalars.

### Nav Autopilot Heading / Autopilot Pitch

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no current autopilot-heading/pitch telemetry field.
- **Notes:** JunoIO currently exposes craft attitude state, not in-game autopilot target setpoints.
- **If missing:** add explicit autopilot target telemetry only if ModAPI exposes it cleanly.

### Nav North / East / Craft Roll Axis / Craft Pitch Axis / Craft Yaw Axis

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use the vector aliases on `self.juno.state`.

```python
north = self.juno.state.nav_north
east = self.juno.state.nav_east
roll_axis = self.juno.state.nav_craft_roll_axis
pitch_axis = self.juno.state.nav_craft_pitch_axis
yaw_axis = self.juno.state.nav_craft_yaw_axis
```

- **Notes:** these are PCI/world-frame vectors and follow Juno's `y-up` convention.

### Velocity Surface / Orbit / Target / Gravity / Drag / Acceleration / Angular

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use the current scalar or vector fields as appropriate.

```python
surface_velocity = self.juno.state.velocity_surface
orbit_velocity = self.juno.state.velocity_orbit
target_velocity = self.juno.state.velocity_target
gravity = self.juno.state.gravity_acceleration
gravity_vector = self.juno.state.gravity_acceleration_vector
drag_accel = self.juno.state.drag_acceleration
acceleration = self.juno.state.acceleration
acceleration_vector = self.juno.state.acceleration_vector
angular_velocity = self.juno.state.angular_velocity
```

- **Notes:** some names are scalar magnitudes and some are vectors. In particular, `gravity_acceleration` and `acceleration` are scalar fields, while `gravity_acceleration_vector` and `acceleration_vector` are the explicit vector forms.

### Velocity Lateral / Vertical

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `horizontal_speed` and `vertical_speed`.

```python
lateral_speed = self.juno.state.horizontal_speed
vertical_speed = self.juno.state.vertical_speed
```

- **Notes:** these are scalar speeds, not vectors.

### Velocity Mach Number

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.state.mach_number`.

```python
mach = self.juno.state.mach_number
```

- **Notes:** this is a scalar.

### Input Roll / Pitch / Yaw / Throttle / Brake / Translate Forward / Translate Right / Translate Up / Translation Mode

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** read the control telemetry fields on `self.juno.state`.

```python
throttle = self.juno.state.control_throttle
pitch = self.juno.state.control_pitch
roll = self.juno.state.control_roll
yaw = self.juno.state.control_yaw
brake = self.juno.state.control_brake
translate_forward = self.juno.state.control_translate_forward
translate_right = self.juno.state.control_translate_right
translate_up = self.juno.state.control_translate_up
translation_mode = self.juno.state.control_translation_mode_enabled
```

- **Notes:** these are telemetry values showing current inputs/state, not all of them are currently writable through the control API.

### Input Slider 1 / Slider 2 / Slider 3 / Slider 4

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there are no current slider telemetry fields.
- **Notes:** sliders are not yet exposed in the Python-facing state model.
- **If missing:** add slider telemetry only if the underlying ModAPI/state path is available.

### Misc Stage / Num Stages

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there are no current stage-count/stage-index telemetry fields.
- **Notes:** stage activation exists as a command, but stage-number telemetry is not currently exposed.
- **If missing:** add explicit stage telemetry if you want parity.

### Misc Grounded / Solar Radiation

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use the current fields on `self.juno.state`.

```python
grounded = self.juno.state.grounded
solar_radiation = self.juno.state.solar_radiation_intensity
```

- **Notes:** `in_water` is also available separately.

### Misc User Camera Position / User Camera Pointing / User Camera Up Direction

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no current camera telemetry surface.
- **Notes:** camera state is not currently bridged.
- **If missing:** add a dedicated camera telemetry block if this becomes important.

### Misc Pitch PIDs / Roll PIDs

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no current PID telemetry surface for in-game autopilot tuning.
- **Notes:** this is distinct from raw control inputs.
- **If missing:** only add it if the underlying ModAPI exposes those values clearly.

### Time Frame Delta Time / Time Since Launch / Total Time / Warp Amount / Real Time

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use the timing fields on `self.juno.state`.

```python
frame_dt = self.juno.state.frame_delta_time
time_since_launch = self.juno.state.time_since_launch
total_time = self.juno.state.total_time
warp_amount = self.juno.state.warp_amount
real_time = self.juno.state.real_time
```

- **Notes:** `self.juno.state.time` is also the current simulation timestamp.

### Name Of Craft / Planet / Target Name / Target Planet

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use the current craft and target APIs.

```python
craft_name = self.juno.active_craft.name
planet_name = self.juno.state.planet_name
target_name = None if self.juno.target is None else self.juno.target.name
target_planet = None if self.juno.target is None or self.juno.target.parent_body is None else self.juno.target.parent_body.name
```

- **Notes:** use `self.juno.get_craft_name(craft_id)` when you need a non-active craft name by id.

### Activation Group ()

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no current activation-group query API.
- **Notes:** activation groups are not yet exposed as state or RPC.
- **If missing:** add explicit activation-group access if you need parity here.

### Convert (...) To Lat/Long/AGL

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no built-in conversion helper yet.
- **Notes:** this needs a world/body conversion API rather than a simple field read. Also remember that AGL is centre-of-mass-relative and can shift with mass redistribution.
- **If missing:** add a dedicated coordinate-conversion helper backed by body/frame geometry.

### Convert (...) To Lat/Long/ASL

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no built-in conversion helper yet.
- **Notes:** this needs a world/body conversion API rather than a simple field read.
- **If missing:** add a dedicated coordinate-conversion helper backed by body/frame geometry.

### Convert (...) To Position

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no built-in lat/long/alt-to-position helper yet.
- **Notes:** this should eventually become a dedicated body-coordinate conversion helper.
- **If missing:** add an explicit conversion API rather than pushing this into ad hoc user math.

### Convert (...) To Position Over Sea

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no built-in helper yet.
- **Notes:** this is another world/body coordinate conversion case.
- **If missing:** add it alongside the other conversion helpers if you build a proper body/frame conversion module.

### Get Terrain Color / Height At Lat/Long (...)

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no terrain query API yet.
- **Notes:** this likely needs direct ModAPI terrain/body sampling support.
- **If missing:** add a dedicated terrain-query API if the underlying game surfaces allow it.

### Cast A Ray From (...) Towards (...)

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no raycast API yet.
- **Notes:** this is a world-query feature, not current telemetry.
- **If missing:** add a dedicated raycast helper on the Unity side if needed.

### Planet () ...

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no current structured planet/body query API.
- **Notes:** JunoIO currently exposes only a small amount of active-craft planet context like `planet_name`, `planet_radius`, `planet_surface_gravity`, plus target body information when a body is selected as the target.
- **If missing:** add a dedicated body/planet RPC or telemetry-backed API instead of scattering body fields across `self.juno.state`.

### Part () Name / Mass / Dry Mass / Wet Mass / Activated / Part Type / Position / Temperature / Drag / This Part ID

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no current structured part query API.
- **Notes:** JunoIO does not yet expose per-part inspection on the Python side.
- **If missing:** add an explicit part object/query layer rather than flattening part data into the craft state.

### Part ID Of ()

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no part lookup helper yet.
- **Notes:** part identifiers are not currently surfaced to Python as a public query model.
- **If missing:** add a part discovery/query API first.

### Part () Local To PCI / PCI To Local (...)

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no current part-frame conversion API.
- **Notes:** frame conversion is possible in principle, but it needs explicit part transform access.
- **If missing:** add part transform queries plus helper conversion methods.

### Craft () Altitude / Destroyed / Grounded / Mass / Name / Part Count / Planet / Position / Velocity / Is Player ...

- **Status:** `Partial`
- **How to do it in JunoIO/Python:** use `CraftView` and its `.state` where supported.

```python
craft = self.juno.crafts["craft_2"]
craft_name = craft.name
craft_state = craft.state
```

- **Notes:** for tracked or active crafts, you can already read many of these through `craft.state`, including altitude, mass, grounded, planet name, position, and velocity. But there is no full Vizzy-style `craft(...)` query surface yet, and fields like part count, destroyed status, bounding boxes, and the full orbital element set for arbitrary crafts are not currently wrapped as first-class craft query helpers.
- **If missing:** add a richer craft query API if you want full parity for arbitrary craft objects.

### Craft ID Of ()

- **Status:** `Partial`
- **How to do it in JunoIO/Python:** craft ids are already the canonical keys in JunoIO.

```python
craft_id = self.juno.active_craft.craft_id
craft_ids = self.juno.list_craft_ids()
```

- **Notes:** if you mean "get the id of a known craft handle", just use `.craft_id`. There is not a separate generic `craft_id_of(...)` helper because ids are already first-class.

### True

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `True`.

```python
value = True
```

- **Notes:** this is just the normal Python boolean literal.

### False

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `False`.

```python
value = False
```

- **Notes:** this is just the normal Python boolean literal.

## Events

These are different from most Vizzy categories because they need real event hooks or a message bus, not just telemetry fields. JunoIO now has a small but growing amount of explicit event-style behaviour, including a basic messaging layer.

### On Start

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** define `on_start(self)`.

```python
from junoio import JunoProgram


class SimpleProgram(JunoProgram):
    def on_start(self) -> None:
        self.juno.display("Started")
```

- **Notes:** this runs once when the program is loaded.

### On (Part) Collide With (Other) At (Velocity) And (Impulse)

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no collision event API yet.
- **Notes:** this needs real collision callbacks or collision event telemetry from Unity. Polling state is not the same thing.
- **If missing:** add a structured collision event stream with part ids, other-object identity, impact velocity, and impulse if the ModAPI exposes them.

### On (Part) Exploded

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no part-explosion event API yet.
- **Notes:** this needs part lifecycle/damage event hooks, not just normal telemetry.
- **If missing:** add a dedicated explosion/destruction event path if the ModAPI exposes it.

### On (CraftA) And (CraftB) Docked

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no docking event API yet.
- **Notes:** this needs a real craft relationship event or docking status event from Unity.
- **If missing:** add a structured docking event with both craft ids and timestamps if the underlying game API supports it.

### On Enter (Planet) SOI

- **Status:** `Missing`
- **How to do it in JunoIO/Python:** there is no direct SOI-entry event API yet.
- **Notes:** you can sometimes infer a body change by polling `self.juno.state.planet_name`, but that is not the same as a first-class event and may miss details or be awkward for exact sequencing.
- **If missing:** add an explicit SOI/body-transition event if precise event semantics are important.

### Receive (Message) With (Data)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** define `on_message(self, message)`, or poll `self.juno.messages.poll()` / `self.juno.receive_messages()`.

```python
def on_message(self, message) -> None:
    print(message.message, message.data)
```

- **Notes:** message receipt also appears in `on_event(...)` and `self.juno.events.poll()` as `message_received`.

### Broadcast (Message) With Data ()

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.broadcast("status", {"ok": True})`.
- **Notes:** this excludes the sender craft. Use `broadcast_to_craft(...)` if you want explicit self-delivery.

### Broadcast (Message) With Data () To Craft

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.broadcast_to_craft("craft_2", "ping", {"mode": "hold"})`.
- **Notes:** this is the current explicit path for self-delivery too, using the active craft's own `craft_id`.

### Broadcast (Message) With Data () To Nearby Crafts

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `self.juno.broadcast_to_nearby("nearby_test", {"step": 1}, radius=500.0)`.
- **Notes:** when `radius=None`, the current Unity-side default is `1000 m`.

## Variables

Vizzy variables mostly map directly to normal Python names and assignment. In JunoIO, these are usually just ordinary Python variables or attributes stored on `self`.

### Set Variable () To ()

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use normal assignment.

```python
target_height = 200.0
```

- **Notes:** for persistent program state across ticks, store values on `self`.

```python
self.target_height = 200.0
```

### Change Variable () By ()

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `+=`.

```python
counter += 1
```

- **Notes:** for persistent program state across ticks, use attributes on `self`.

```python
self.counter += 1
```

### Set Variable () To User Input ()

- **Status:** `Partial`
- **How to do it in JunoIO/Python:** use Python's `input(...)` only for local terminal input.

```python
name = input("Enter a name: ")
```

- **Notes:** this reads from the local Python terminal, not from an in-game Juno UI prompt. That means it is usable for developer/operator interaction at script startup, but it is not a full Vizzy-style in-game user-input block.
- **If missing:** add a dedicated in-game prompt/input API if you want true flight-scene UI input rather than terminal input.

## Lists

Vizzy list operations mostly map directly to normal Python lists. The main thing to be careful about is indexing: Python is zero-based.

### Add (Item) To (List)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `.append(...)`.

```python
items.append(value)
```

- **Notes:** this adds the item to the end of the list.

### Insert (Item) At (Pos) In (List)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `.insert(index, value)`.

```python
items.insert(index, value)
```

- **Notes:** Python uses zero-based indexes.

### Remove (Item) From (List)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `.remove(...)`.

```python
items.remove(value)
```

- **Notes:** this removes the first matching item. It raises an error if the item is not present.

### Set (Pos) In (List) To (Item)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** assign by index.

```python
items[index] = value
```

- **Notes:** Python uses zero-based indexes.

### Remove All From (List)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `.clear()`.

```python
items.clear()
```

- **Notes:** this empties the list in place.

### Sort (List)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `.sort()`.

```python
items.sort()
```

- **Notes:** use `sorted(items)` if you want a new sorted list instead of modifying the existing one.

### Reverse (List)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `.reverse()`.

```python
items.reverse()
```

- **Notes:** use `list(reversed(items))` if you want a reversed copy instead of modifying the existing list.

### Set List (List) To (Create List From ())

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** create a new list directly.

```python
items = [a, b, c]
```

- **Notes:** you can also build lists from iterables with `list(...)`.

### Item (Pos) In (List)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** index into the list.

```python
value = items[index]
```

- **Notes:** Python uses zero-based indexes.

### Length Of (List)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `len(...)`.

```python
count = len(items)
```

- **Notes:** this is the normal Python list length operation.

### Index Of (Item) In List (List)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** use `.index(...)`.

```python
index = items.index(value)
```

- **Notes:** this returns the first matching zero-based index and raises an error if the item is not present.

## Custom Expressions

Custom expressions map very naturally to normal Python functions that take parameters and return a value.

### Expression (param1, param2, ...)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** define a normal Python function with parameters and `return` a value.

```python
def hover_error(target_height: float, altitude_agl: float) -> float:
    return target_height - altitude_agl
```

- **Notes:** this is the closest Python equivalent to a Vizzy custom expression. The key idea is the same: it computes and returns a value, and should not be used for instruction-style side effects if you want to keep the same mental model.

You can define these:
- as normal helper functions at module scope
- as instance methods on your `JunoProgram`

Example as a method:

```python
from junoio import JunoProgram


class SimpleProgram(JunoProgram):
    def height_error(self, target_height: float) -> float:
        return target_height - self.juno.state.altitude_agl
```

- **Notes:** Python does not need a separate “custom expressions” system because ordinary functions already fill that role.

## Custom Instructions

Custom instructions map naturally to normal Python functions or methods that perform actions and do not return anything important.

### Instruction (param1, param2, ...)

- **Status:** `Implemented`
- **How to do it in JunoIO/Python:** define a normal Python function or method that takes parameters and performs work.

```python
def log_hover_status(height: float) -> None:
    print(f"hover height = {height:.1f} m")
```

- **Notes:** this is the closest Python equivalent to a Vizzy custom instruction. The mental model is the same: this is for doing something, not mainly for returning a value.

Example as a method on your program:

```python
from junoio import JunoProgram


class SimpleProgram(JunoProgram):
    def set_hover_throttle(self, throttle: float) -> None:
        self.juno.control.throttle = throttle
        self.juno.flight_log(f"Throttle set to {throttle:.2f}", replace=True)
```

- **Notes:** if the custom instruction needs to span multiple ticks, it can itself be written as a generator helper and used from `script()` with `yield from ...`.

Example cross-tick helper:

```python
from collections.abc import Iterator


def wait_and_log(self, seconds: float) -> Iterator[object]:
    self.juno.flight_log("Waiting...", replace=True)
    yield from self.juno.wait_seconds(seconds)
```

- **Notes:** Python does not need a separate custom-instruction system because ordinary functions/methods already provide this cleanly.
