# AGENTS.md - Agent Guidelines for NeckControlOutput

This file contains build commands and code style guidelines for agents working on this repository.

## Build Commands

```bash
# Build the solution
dotnet build

# Run the application
dotnet run

# Format code
dotnet format

# Clean build artifacts
dotnet clean
```

### Testing
No test framework configured. When adding tests, use xUnit/NUnit in a separate `NeckControlOutput.Tests` project.

---

## Code Style Guidelines

### Naming Conventions

| Type | Convention | Examples |
|------|------------|----------|
| Classes | PascalCase | `Form1`, `DataConfig`, `OneData` |
| Methods | PascalCase | `LoadComboBox`, `SendData`, `TrackBar_Scroll` |
| Properties | PascalCase | `ServoAngle`, `NeckAngle`, `ServoMinMax` |
| Variables/Fields | camelCase (no underscore prefix) | `udpClient`, `currentTrackBar`, `running` |
| Event Handlers | `controlName_EventName` | `button_w_Click`, `Form1_Load` |
| Constants | PascalCase | `serverPort`, `raspberryPiIp` |

### File Organization
- **Form1.cs**: Main application logic and event handlers
- **Form1.Designer.cs**: Auto-generated (DO NOT manually edit)
- **Program.cs**: Application entry point
- **Models.cs**: Data models (`DataConfig`, `OneData`, `NackMatrixConfig`)
- **RBFHelper.cs**: Radial basis function utilities
- **WeightMatrixHelper.cs**: Weight matrix calculation logic

### Code Patterns

```csharp
// Event handler naming
private void button_w_Click(object sender, EventArgs e) { }

// Cross-thread UI updates (MANDATORY pattern)
if (tb.InvokeRequired)
{
    tb.Invoke(new Action(() => tb.Value = value));
}
else
{
    tb.Value = value;
}

// Thread synchronization with lock objects
private readonly object timerLock = new object();
lock (timerLock) { transitionTimer?.Stop(); }

// JSON serialization
string newJson = JsonSerializer.Serialize(config,
    new JsonSerializerOptions { WriteIndented = true });
```

### Error Handling
- Use `MessageBox.Show(ex.Message)` for user-facing errors
- Wrap parsing with `try-catch` (e.g., `float.Parse()`)
- Never suppress exceptions silently

### Threading & Concurrency
- Use `Thread` with `IsBackground = true` for background operations
- Cross-thread UI updates MUST use `InvokeRequired` pattern
- Use `lock` objects for thread synchronization (e.g., `timerLock`, `udpLock`, `lerpDataLock`)
- `Thread.Sleep(33)` for ~30Hz update rate
- Use `System.Windows.Forms.Timer` for UI updates (not `System.Timers.Timer`)

### JSON Configuration
- Library: `System.Text.Json`
- Default files: `neck_config.json`, `neck_matrix_config.json`
- Always use `WriteIndented = true` for readability

### UDP Networking
- Class: `UdpClient`
- Default remote: `192.168.3.11:9003`
- Default local port: `5001`
- Packet format: 4-byte header + float values

### External Dependencies
- **NumSharp** (v0.30.0): Numerical operations (numpy-like)
- **Numpy** (v3.11.1.35): Additional numerical support

### UI Patterns
- TrackBar for servo control (8 servos: `servo_0` through `servo_7`)
- TextBox for angle input (x, y, z)
- ComboBox for configuration selection
- Smooth transitions using Timer with step-based interpolation

### Project Configuration
```xml
<TargetFramework>net8.0-windows</TargetFramework>
<UseWindowsForms>true</UseWindowsForms>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
```

### Guidelines
1. Maintain single-form architecture
2. Preserve Chinese UI strings and labels
3. Keep event handlers focused—delegate to helper methods
4. Use meaningful variable names (no `var1`, `temp`)
5. Dispose resources properly (timers, UDP clients)

### Anti-Patterns
- ❌ Edit `Form1.Designer.cs` manually
- ❌ Suppress type warnings/errors
- ❌ Use `var` when type is not obvious
- ❌ Empty catch blocks
- ❌ Create threads without cleanup
