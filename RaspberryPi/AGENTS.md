# AGENTS.md - NeckControl Project Guide

**Project Overview**: Robotic head/neck servo control system using ARKit face tracking and animation playback.
**Python Version**: 3.11.9
**Key Dependencies**: numpy, adafruit-pca9685, adafruit-motor

---

## Build/Test/Lint Commands

```bash
# Run main application
python main.py

# Run all tests (if tests exist)
pytest

# Run a specific test file
pytest tests/test_specific.py

# Run a specific test function
pytest tests/test_file.py::test_function_name

# Run with verbose output
pytest -v

# Run tests matching pattern
pytest -k "test_pattern"
```

---

## Code Style Guidelines

### Imports (Standard Python Order)
1. Standard library imports
2. Third-party imports
3. Local/application imports

```python
# Correct
import threading
import time
from abc import ABC, abstractmethod
from adafruit_motor import servo
from controller.servo_controller import ServoController
```

### Naming Conventions

| Type | Convention | Example |
|------|------------|---------|
| Classes | PascalCase | `ServoController`, `BaseInput`, `HeadMapper` |
| Functions/Methods | snake_case | `get_next_frame()`, `_receive_data()` |
| Constants | UPPER_SNAKE_CASE | `DEBUG`, `servo_count`, `pca_frequency` |
| Private methods | _leading_underscore | `_load_facial_expression_json()` |
| Enum members | PascalCase | `Servo.browDown`, `Servo.eye_vertical_left` |

### Type Hints

**Use Python 3.10+ union syntax** `Type | None`, **NOT** `Optional[Type]`

```python
# Correct
def __init__(self, controller: ServoController | None = None, mapper: HeadMapper | None = None):
    self.controller = controller

# Incorrect
from typing import Optional
def __init__(self, controller: Optional[ServoController] = None):
    pass
```

### Classes & Inheritance

- Use abstract base classes (ABC) for interface definitions
- Call `super().__init__()` in subclasses
- Use `threading.Thread(target=self._method, daemon=True)` for background threads

```python
from abc import ABC, abstractmethod

class BaseInput(ABC):
    def __init__(self, controller: ServoController | None = None):
        self.controller = controller

    @abstractmethod
    def start(self):
        pass
```

### Error Handling

- Minimal try-except for I/O operations (files, sockets)
- Print errors for debug mode
- Return `None` on failure where appropriate

```python
try:
    with open(path, 'r', encoding='utf-8') as f:
        data = json.load(f)
except Exception as e:
    print(f"读取文件失败: {e}")
    return None
```

### Threading Patterns

- Use `threading.Lock()` for shared state
- Daemon threads: `threading.Thread(target=self._loop, daemon=True)`
- Thread-safe queue operations: `queue.Queue()` with timeout

```python
def _executor_loop(self):
    while self.running:
        try:
            item = self.queue.get(timeout=0.1)
            with self.lock:
                # Process item
        except queue.Empty:
            continue
```

### Constants & Configuration

- All constants in `config.py` as class attributes
- `Config.DEBUG` flag for hardware-less testing
- File paths relative to project root

```python
class Config():
    DEBUG = True
    servo_count = 24 + 8
    pca9685_address = [0x40, 0x41]
```

### Hardware Abstraction

- Always check `Config.DEBUG` before hardware operations
- Skip board/servo initialization in debug mode
- Print values instead of hardware commands when debugging

```python
def set_angle(self, channel, angle):
    if Config.DEBUG:
        print([channel, angle])
    else:
        self.all_servo[channel].angle = angle
```

### Comments & Docstrings

- Minimal inline comments in Chinese
- English docstrings for public methods where clarity is needed
- Comment out unused code blocks instead of deletion (for reference)

### File Encoding

- Always specify `encoding='utf-8'` for file operations
- Support Chinese filenames and data

---

## Architecture Patterns

### Input Sources (Abstract Factory Pattern)
- `BaseInput` (ABC) - Interface for all input sources
- `ArkitInput` - UDP socket for ARKit face tracking data
- `AnimInput` - File-based animation playback
- `Test` - Testing input mode

### Controller Pattern
- `ServoController` - Thread-safe servo control via queue
- `HeadMapper` - Maps blendshapes/rotation to servo angles
- Separation of concerns: Input → Mapper → Controller

### Data Flow
```
Input Source → blendshape/rotation data → HeadMapper.map() → servo angles → ServoController
```

---

## Important Notes

1. **Debug Mode**: Always test with `Config.DEBUG = True` before hardware deployment
2. **Threading**: All input sources run in daemon threads - ensure clean shutdown with `stop()`
3. **Servo Limits**: Always clamp angles to min/max ranges in JSON config files
4. **Queue-Based Control**: Use `controller.put()` for thread-safe servo updates
5. **Hardware Dependencies**: PCA9685 drivers only imported when `Config.DEBUG = False`

---

## Testing Guidelines

- No formal test infrastructure currently exists
- Use `test.py` for manual testing modes
- When adding tests: Create `tests/` directory with `test_*.py` files
- Mock hardware (`adafruit_pca9685`, `board`, `busio`) for CI/testing

---

## Common Gotchas

1. **Import Order**: Hardware imports (`board`, `busio`) wrapped in `if not Config.DEBUG:`
2. **Socket Shutdown**: Always call `socket.shutdown()` before `close()` to avoid errors
3. **Thread Cleanup**: Use `thread.join()` in `stop()` methods
4. **Type Union Syntax**: Use `Type | None` (Python 3.10+), not `Optional[Type]`
