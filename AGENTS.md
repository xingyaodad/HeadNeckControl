# AGENTS.md - HeadNeckControl 项目开发指南

**项目概述**：基于树莓派的仿生颈部舵机控制系统，支持 ARKit 面部追踪、动画播放和手动控制。
**架构**：Windows 控制端（C# .NET 8） → UDP → 树莓派（Python 3.11） → PCA9685 → 舵机
**ARKit 数据流**：iOS 设备 → UDP → 树莓派 → 头部映射 → 舵机控制
**动画播放**：树莓派本地 `.anim` 文件 → 头部映射 → 舵机控制

---

## 构建和测试命令

### Windows (C# .NET 8)
```bash
# 构建解决方案
cd Windows && dotnet build

# 运行应用程序
cd Windows && dotnet run

# 格式化代码
cd Windows && dotnet format

# 清理构建产物
cd Windows && dotnet clean
```

### RaspberryPi (Python 3.11)
```bash
# 运行主程序
cd RaspberryPi && python main.py

# 安装依赖
cd RaspberryPi && pip install -r requirements.txt

# 运行测试（如果存在）
cd RaspberryPi && pytest -v
cd RaspberryPi && pytest tests/test_file.py::test_function_name

# 运行单个测试文件
cd RaspberryPi && pytest tests/test_specific.py

# 运行匹配模式的测试
cd RaspberryPi && pytest -k "test_pattern"
```

---

## 代码风格指南

### Python (RaspberryPi/)

**导入顺序**：标准库 → 第三方库 → 本地模块
```python
import threading
import time
from abc import ABC, abstractmethod
from adafruit_motor import servo
from controller.servo_controller import ServoController
```

**命名约定**：
- 类名：PascalCase (`ServoController`, `HeadMapper`)
- 函数/方法：snake_case (`get_next_frame()`, `_receive_data()`)
- 常量：UPPER_SNAKE_CASE (`DEBUG`, `servo_count`)
- 私有方法：_leading_underscore
- 枚举成员：PascalCase (`Servo.browDown`)

**类型提示**：使用 Python 3.10+ 联合语法 `Type | None`，**不使用** `Optional[Type]`
```python
# 正确
def __init__(self, controller: ServoController | None = None, mapper: HeadMapper | None = None):
    self.controller = controller

# 错误
from typing import Optional
def __init__(self, controller: Optional[ServoController] = None):
    pass
```

**类和继承**：
- 使用抽象基类（ABC）定义接口
- 在子类中调用 `super().__init__()`
- 后台线程使用：`threading.Thread(target=self._method, daemon=True)`

```python
from abc import ABC, abstractmethod

class BaseInput(ABC):
    def __init__(self, controller: ServoController | None = None):
        self.controller = controller

    @abstractmethod
    def start(self):
        pass
```

**错误处理**：
- I/O 操作使用最少的 try-except（文件、socket）
- 调试模式下打印错误
- 失败时返回 `None`

```python
try:
    with open(path, 'r', encoding='utf-8') as f:
        data = json.load(f)
except Exception as e:
    print(f"读取文件失败: {e}")
    return None
```

**线程模式**：
- 共享状态使用 `threading.Lock()`
- 守护线程：`threading.Thread(target=self._loop, daemon=True)`
- 线程安全的队列操作：`queue.Queue()` 带超时

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

**常量和配置**：
- 所有常量在 `config.py` 中作为类属性
- `Config.DEBUG` 标志用于无硬件测试
- 文件路径相对于项目根目录

```python
class Config():
    DEBUG = True
    servo_count = 24 + 8
    pca9685_address = [0x40, 0x41]
```

**硬件抽象**：
- 硬件操作前始终检查 `Config.DEBUG`
- 调试模式下跳过板卡/舵机初始化
- 调试时打印值而不是硬件命令

```python
def set_angle(self, channel, angle):
    if Config.DEBUG:
        print([channel, angle])
    else:
        self.all_servo[channel].angle = angle
```

**注释和文档字符串**：
- 中文行内注释（保持最小化）
- 公共方法使用英文文档字符串（需要时）
- 注释掉未使用的代码块而不是删除（作为参考）

**文件编码**：
- 文件操作始终指定 `encoding='utf-8'`
- 支持中文文件名和数据

---

### C# (Windows/NeckControlOutput/)

**命名约定**：
- 类名：PascalCase (`Form1`, `DataConfig`, `OneData`)
- 方法：PascalCase (`LoadComboBox`, `SendData`, `TrackBar_Scroll`)
- 属性：PascalCase (`ServoAngle`, `NeckAngle`)
- 变量/字段：camelCase（无下划线前缀）(`udpClient`, `currentTrackBar`, `running`)
- 事件处理器：`controlName_EventName` (`button_w_Click`, `Form1_Load`)
- 常量：PascalCase (`serverPort`, `raspberryPiIp`)

**文件组织**：
- **Form1.cs**：主应用逻辑和事件处理器
- **Form1.Designer.cs**：自动生成（**不要手动编辑**）
- **Program.cs**：应用程序入口点
- **Models.cs**：数据模型（`DataConfig`, `OneData`, `NackMatrixConfig`）
- **RBFHelper.cs**：径向基函数工具
- **WeightMatrixHelper.cs**：权重矩阵计算逻辑

**代码模式**：

```csharp
// 事件处理器命名
private void button_w_Click(object sender, EventArgs e) { }

// 跨线程 UI 更新（强制模式）
if (tb.InvokeRequired)
{
    tb.Invoke(new Action(() => tb.Value = value));
}
else
{
    tb.Value = value;
}

// 线程同步使用 lock 对象
private readonly object timerLock = new object();
lock (timerLock) { transitionTimer?.Stop(); }

// JSON 序列化
string newJson = JsonSerializer.Serialize(config,
    new JsonSerializerOptions { WriteIndented = true });
```

**错误处理**：
- 用户面向的错误使用 `MessageBox.Show(ex.Message)`
- 用 try-catch 包装解析（例如：`float.Parse()`）
- 不要静默抑制异常

**线程和并发**：
- 后台操作使用 `Thread`，设置 `IsBackground = true`
- 跨线程 UI 更新**必须**使用 `InvokeRequired` 模式
- 使用 `lock` 对象进行线程同步（例如：`timerLock`, `udpLock`, `lerpDataLock`）
- `Thread.Sleep(33)` 用于 ~30Hz 更新速率
- UI 更新使用 `System.Windows.Forms.Timer`（不是 `System.Timers.Timer`）

**JSON 配置**：
- 库：`System.Text.Json`
- 默认文件：`neck_config.json`, `neck_matrix_config.json`
- 始终使用 `WriteIndented = true` 以提高可读性

**UDP 网络**：
- 类：`UdpClient`
- 默认远程：`192.168.3.11:9003`
- 默认本地端口：`5001`
- 数据包格式：4 字节头部 + float 值

**外部依赖**：
- **NumSharp** (v0.30.0)：数值操作（类似 numpy）
- **Numpy** (v3.11.1.35)：附加数值支持

**UI 模式**：
- TrackBar 用于舵机控制（8 个舵机：`servo_0` 到 `servo_7`）
- TextBox 用于角度输入（x, y, z）
- ComboBox 用于配置选择
- 使用 Timer 和基于步骤的插值实现平滑过渡

**项目配置**：
```xml
<TargetFramework>net8.0-windows</TargetFramework>
<UseWindowsForms>true</UseWindowsForms>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
```

**指南**：
1. 保持单窗体架构
2. 保留中文 UI 字符串和标签
3. 事件处理器要简洁—委托给辅助方法
4. 使用有意义的变量名（不要 `var1`, `temp`）
5. 正确释放资源（定时器、UDP 客户端）

**反模式**：
- ❌ 手动编辑 `Form1.Designer.cs`
- ❌ 抑制类型警告/错误
- ❌ 类型不明显时使用 `var`
- ❌ 空的 catch 块
- ❌ 创建线程而不清理

---

## 架构模式

### 数据流
```
Windows App (TrackBar) → UDP 舵机角度 → RaspberryPi → HeadMapper → ServoController → PCA9685 → 舵机
iOS Device (ARKit) → UDP 姿态数据 → RaspberryPi → HeadMapper → ServoController → PCA9685 → 舵机
Animation File (.anim) → RaspberryPi → HeadMapper → ServoController → PCA9685 → 舵机
```

### 输入源（抽象工厂模式）
- `BaseInput` (ABC) - 所有输入源的接口
- `ArkitInput` - ARKit 面部追踪数据（来自 iOS 设备的 UDP）
- `AnimInput` - 文件动画播放（树莓派本地 `.anim` 文件）
- `Test` - 测试输入模式
- `AiInput` - AI 驱动输入（未来扩展）

### 控制器模式
- `ServoController` - 通过队列的线程安全舵机控制
- `HeadMapper` - 使用 RBF 插值将 blendshapes/旋转映射到舵机角度
- 分离关注点：输入 → 映射器 → 控制器

---

## 常见问题

**Python**：
- 导入顺序：硬件导入（`board`, `busio`）包裹在 `if not Config.DEBUG:` 中
- Socket 关闭：在 `close()` 之前始终调用 `socket.shutdown()` 以避免错误
- 线程清理：在 `stop()` 方法中使用 `thread.join()`
- 类型联合语法：使用 `Type | None` (Python 3.10+)，不是 `Optional[Type]`

**C#**：
- 永远不要手动编辑 `Form1.Designer.cs`（自动生成）
- 跨线程 UI 更新**必须**使用 `InvokeRequired` 模式
- 正确释放资源（定时器、UDP 客户端）
- UI 更新使用 `System.Windows.Forms.Timer`（不是 `System.Timers.Timer`）

---

## 配置文件

- `RaspberryPi/controller/neck_matrix_config.json` - 舵机角度映射和权重矩阵
- `Windows/NeckControlOutput/bin/Debug/net8.0-windows/neck_config.json` - Windows 端舵机配置
- `RaspberryPi/requirements.txt` - Python 依赖
- `Windows/NeckControlOutput/NeckControlOutput.csproj` - C# 项目配置
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

**Python**：目前不存在正式的测试基础设施。使用 `test.py` 进行手动测试模式。添加测试时，在 `tests/` 目录中创建 `test_*.py` 文件。为 CI/测试模拟硬件（`adafruit_pca9685`, `board`, `busio`）。

**C#**：没有配置测试框架。添加测试时，在单独的 `NeckControlOutput.Tests` 项目中使用 xUnit/NUnit。

---

## 网络通信协议

### UDP 数据包格式

**Windows 端发送（舵机角度）**：
```
[4 字节头部][32 个 float（舵机角度）]
- 头部：消息类型 ID（通常为 1）
- 数据：8 个颈部舵机角度 + 24 个面部舵机角度
```

**ARKit 端发送（姿态数据）**：
```
[4 字节头部][52 个 blendshapes][3 个 rotation floats]
- 头部：消息类型 ID（通常为 2）
- 数据：ARKit 52 个 blendshape 系数 + 3 个旋转角度 (x, y, z)
```

**端口配置**：
- 树莓派监听端口：`9003`（在 `Config.port` 中配置）
- Windows 本地端口：`5001`（用于发送数据）
