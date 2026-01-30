# AGENTS.md - HeadNeckControl 项目开发指南

**项目概述**：基于树莓派的仿生颈部舵机控制系统，支持 ARKit 面部追踪、动画播放和手动控制。
**架构**：Windows (C# .NET 8) → UDP → 树莓派 (Python 3.11) → PCA9685 → 舵机

---

## 构建和测试命令

### Windows (C# .NET 8)
```bash
cd Windows && dotnet build    # 构建
cd Windows && dotnet run      # 运行
cd Windows && dotnet format   # 格式化
```

### RaspberryPi (Python 3.11)
```bash
cd RaspberryPi && python main.py                        # 运行主程序
cd RaspberryPi && pip install -r requirements.txt      # 安装依赖
cd RaspberryPi && pytest -v                            # 运行所有测试
cd RaspberryPi && pytest tests/test_file.py::test_func  # 运行单个测试
cd RaspberryPi && pytest -k "pattern"                  # 运行匹配测试
```

---

## 代码风格指南

### Python (RaspberryPi/)

**导入顺序**：标准库 → 第三方库 → 本地模块

**命名约定**：类名 PascalCase，函数 snake_case，常量 UPPER_SNAKE_CASE，私有方法 _leading_underscore

**类型提示**：使用 Python 3.10+ 联合语法 `Type | None`，**不使用** `Optional[Type]`

**线程模式**：
```python
def _executor_loop(self):
    while self.running:
        try:
            item = self.queue.get(timeout=0.1)
            with self.lock:
                # 处理 item
        except queue.Empty:
            continue
```

**常量和配置**：所有常量在 `config.py` 中作为类属性，`Config.DEBUG` 标志用于无硬件测试

**硬件抽象**：硬件操作前始终检查 `Config.DEBUG`，调试模式下打印值而不是硬件命令
```python
if Config.DEBUG:
    print([channel, angle])
else:
    self.all_servo[channel].angle = angle
```

**错误处理**：I/O 操作使用最少的 try-except，失败时返回 `None`
**文件编码**：始终指定 `encoding='utf-8'`

---

### C# (Windows/NeckControlOutput/)

**命名约定**：类名/方法/属性 PascalCase，变量/字段 camelCase，事件处理器 `controlName_EventName`

**代码模式**：
```csharp
// 跨线程 UI 更新（强制模式）
if (tb.InvokeRequired) {
    tb.Invoke(new Action(() => tb.Value = value));
} else {
    tb.Value = value;
}

// 线程同步
private readonly object timerLock = new object();
lock (timerLock) { transitionTimer?.Stop(); }
```

**错误处理**：用户面向的错误使用 `MessageBox.Show(ex.Message)`，用 try-catch 包装解析

**线程和并发**：
- 后台操作使用 `Thread`，设置 `IsBackground = true`
- 跨线程 UI 更新**必须**使用 `InvokeRequired` 模式
- UI 更新使用 `System.Windows.Forms.Timer`（不是 `System.Timers.Timer`）

**项目配置**：`net8.0-windows`, `UseWindowsForms`, `Nullable enable`, `ImplicitUsings enable`

**反模式**：❌ 手动编辑 `Form1.Designer.cs`，❌ 抑制类型警告，❌ 类型不明显时使用 `var`

---

## 架构模式

### 数据流
```
Windows (TrackBar) → UDP 舵机角度 → RaspberryPi → HeadMapper → ServoController → PCA9685 → 舵机
iOS (ARKit) → UDP 姿态数据 → RaspberryPi → HeadMapper → ServoController → PCA9685 → 舵机
Animation (.anim) → RaspberryPi → HeadMapper → ServoController → PCA9685 → 舵机
```

### 输入源（抽象工厂模式）
- `BaseInput` (ABC) - 所有输入源的接口
- `ArkitInput` - ARKit 面部追踪数据（UDP）
- `AnimInput` - 文件动画播放
- `Test` - 测试输入模式

### 控制器模式
- `ServoController` - 通过队列的线程安全舵机控制
- `HeadMapper` - 使用 RBF 插值将 blendshapes/旋转映射到舵机角度

---

## 常见问题

**Python**：硬件导入（`board`, `busio`）包裹在 `if not Config.DEBUG:` 中；Socket 关闭：在 `close()` 之前始终调用 `socket.shutdown()`；线程清理：在 `stop()` 方法中使用 `thread.join()`

**C#**：永远不要手动编辑 `Form1.Designer.cs`（自动生成）；跨线程 UI 更新**必须**使用 `InvokeRequired` 模式

---

## 配置文件

- `RaspberryPi/controller/neck_matrix_config.json` - 舵机角度映射和权重矩阵
- `Windows/NeckControlOutput/bin/Debug/net8.0-windows/neck_config.json` - Windows 端舵机配置
- `RaspberryPi/requirements.txt` - Python 依赖
- `RaspberryPi/config.py` - 运行时配置（IP、端口、调试模式等）

---

## 重要提示

1. **调试模式**：硬件部署前始终使用 `Config.DEBUG = True` 测试
2. **线程**：所有输入源在守护线程中运行 - 确保通过 `stop()` 干净关闭
3. **舵机限制**：始终将角度限制在 JSON 配置文件中的 min/max 范围内
4. **基于队列的控制**：使用 `controller.put()` 进行线程安全的舵机更新
5. **平台特定细节**：详细指南参见 `RaspberryPi/AGENTS.md` 和 `Windows/AGENTS.md`

---

## 测试指南

**Python**：目前不存在正式的测试基础设施。使用 `test.py` 进行手动测试模式。添加测试时，在 `tests/` 目录中创建 `test_*.py` 文件。

**C#**：没有配置测试框架。添加测试时，在单独的 `NeckControlOutput.Tests` 项目中使用 xUnit/NUnit。

---

## 网络通信协议

### UDP 数据包格式

**Windows 端发送（舵机角度）**：`[4 字节头部][32 个 float（舵机角度）]`
**ARKit 端发送（姿态数据）**：`[4 字节头部][52 个 blendshapes][3 个 rotation floats]`

**端口配置**：树莓派监听端口 `9003`，Windows 本地端口 `5001`
