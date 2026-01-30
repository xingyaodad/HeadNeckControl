# HeadNeckControl

<div align="center">

**基于树莓派的仿生颈部舵机控制系统**

通过 8 个舵机实现仿生头部的实时控制和动画播放

</div>

---

## 效果展示

### 仿生头部运行演示

<div align="center">

 <!-- 替换为你的实际 GIF 文件 -->
<img src="images/media/demo.gif" width="600" alt="仿生头部运行演示" />

*仿生头部实时追踪演示*

</div>

### Windows 控制界面

<div align="center">

 <!-- 替换为你的实际截图 -->
<img src="images/media/windows-ui.png" width="800" alt="Windows 控制界面" />

*Windows 端手动控制界面*

</div>

### 硬件组装图

<div align="center">

 <!-- 替换为你的实际照片 -->
<img src="images/media/hardware.jpg" width="600" alt="硬件组装图" />

*硬件组装效果图*

</div>

---

## 项目简介

HeadNeckControl 是一个完整的仿生颈部控制系统，通过树莓派和 PCA9685 驱动 8 个舵机来控制仿生头部的运动。系统支持多种控制方式：手机 ARKit 面部追踪、动画文件播放、Windows 端手动控制和测试模式，可以精确控制头部的俯仰、偏航和滚动三个自由度。

## 功能特点

- **ARKit 实时追踪**：通过 iOS 设备使用 ARKit 捕捉面部姿态，通过 UDP 直接发送到树莓派进行实时控制
- **Unity iOS 应用**：提供完整的 Unity 项目，用于生成 ARKit 面部追踪 iOS 应用，支持自定义和扩展
- **动画播放**：支持录制和播放预设的头部动作序列，动画文件存储在树莓派本地
- **Windows 手动控制**：提供图形界面，可通过滑块直接控制舵机角度
- **测试模式**：用于调试和校准舵机的测试模式
- **平滑过渡**：使用 RBF（径向基函数）插值算法，实现流畅的动作过渡
- **调试模式**：无需硬件即可在电脑上模拟运行，便于开发调试
- **多输入源**：支持多种输入源的灵活切换

## 系统架构

```
┌─────────────────┐     UDP      ┌──────────────────┐
│  Windows App    │ ────────────> │  Raspberry Pi    │
│  (C# .NET 8)    │              │   (Python 3.11)  │
│                 │              │                  │
│  - TrackBar     │              │  - BaseInput     │
│  - 手动控制     │              │  - HeadMapper    │
└─────────────────┘              │  - ServoController│
                                 └────────┬─────────┘
                                          │
                            UDP             │ I2C
┌─────────────────┐                       │
│   iOS Device    │ ────────────────────────┤
│   (ARKit)       │                       ▼
└─────────────────┘              ┌──────────────┐
                                 │   PCA9685    │
                                 │ (PWM Driver) │
                                 └──────┬───────┘
                                        │
                                  8个舵机
                                        ▼
                                 ┌──────────────┐
                                 │  仿生头部    │
                                 └──────────────┘
```

**数据流向说明**：
- **Windows 端**：通过滑块手动控制舵机角度，通过 UDP 发送舵机角度数据到树莓派
- **iOS ARKit 端**：直接通过 UDP 发送 ARKit 捕捉的面部姿态数据（blendshapes + rotation）到树莓派
- **动画播放**：动画文件（`.anim`）存储在树莓派本地，由树莓派读取并播放
- **树莓派**：作为控制中心，接收所有输入源数据，经过 HeadMapper 映射后控制舵机

## 硬件清单

### 树莓派端
- **树莓派**：3B+ 或更高版本（推荐 4B）
- **PCA9685 PWM 驱动板**：16 通道 I2C PWM 驱动器
- **舵机**：MG996R 或类似的金属齿轮舵机 × 8
- **电源**：5V 3A 或更高功率的开关电源
- **杜邦线**：连接树莓派、PCA9685 和舵机
- **面包板或 PCB**：用于电路连接

### Windows 端（可选）
- **Windows 10/11 电脑**：用于手动控制
- **网络连接**：与树莓派在同一局域网

### iOS 端（ARKit，可选）
- **iPhone/iPad**：支持 ARKit 4.0 或更高版本
- **Unity 2021.3 或更高版本**：用于构建 iOS 应用
- **Xcode**：用于编译和部署 iOS 应用
- **Apple Developer 账号**：用于真机部署（开发测试可用免费账号）
- **Unity ARKit 项目**：项目包含 `UnityArkit/` 目录，包含完整的 ARKit 面部追踪应用源代码

### 3D 打印设备（可选）
- **3D 打印机**：FDM 打印机，打印尺寸至少 200×200×200mm
- **打印材料**：PLA、PETG 或 ABS（推荐 PETG）
- **切片软件**：Cura、PrusaSlicer 等
- **模型文件**：`3D-Models/` 目录包含 OBJ、3MF 和 STL 格式的模型

## 快速开始

### 树莓派环境配置

#### 1. 系统要求
- **操作系统**：Raspberry Pi OS Bullseye 或 Bookworm（推荐 64 位）
- **Python 版本**：Python 3.10 或更高（本系统使用 Python 3.11）
- **内存**：至少 2GB RAM（推荐 4GB）

#### 2. 克隆项目

```bash
cd ~
git clone https://github.com/yourusername/HeadNeckControl.git
cd HeadNeckControl/RaspberryPi
```

#### 3. 安装 Python 依赖

```bash
# 更新系统
sudo apt update
sudo apt upgrade -y

# 安装系统依赖
sudo apt install -y python3-pip python3-dev

# 安装 Python 包
pip3 install -r requirements.txt
```

**requirements.txt 包含的包**：
```
numpy>=1.24.0                    # 数值计算
adafruit-circuitpython-pca9685    # PCA9685 驱动
adafruit-circuitpython-servokit   # 舵机控制工具包
adafruit-circuitpython-motor      # 电机控制基础库
```

#### 4. 启用 I2C 接口

```bash
# 方法 1：使用 raspi-config
sudo raspi-config
# 选择 Interface Options -> I2C -> Enable

# 方法 2：命令行
sudo apt install -y i2c-tools
sudo raspi-config nonint do_i2c 0

# 重启树莓派
sudo reboot
```

#### 5. 验证 I2C 连接

重启后，检查 PCA9685 是否被识别：

```bash
# 列出所有 I2C 设备
i2cdetect -y 1

# 应该看到类似输出（0x40 是 PCA9685 默认地址）：
#      0  1  2  3  4  5  6  7  8  9  a  b  c  d  e  f
# 00:          -- -- -- -- -- -- -- -- -- -- -- -- --
# 10: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
# 20: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
# 30: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
# 40: 40 -- -- -- -- -- -- -- -- -- -- -- -- -- --
# ...
```

如果看到 `40`，说明 PCA9685 已正确连接。

#### 6. 配置网络

编辑 `config.py`：

```python
class Config():
    DEBUG = False        # 生产环境设为 False
    ip = ''            # 树莓派的 IP 地址（自动获取）
    port = 9003        # UDP 接收端口
    bs_count = 52       # ARKit blendshapes 数量
    servo_count = 8     # 舵机数量
    test_servo_count = 8
    pca9685_address = [0x40]  # PCA9685 I2C 地址
    pca_frequency = 50          # PWM 频率 (50Hz)
    is_anim = False      # 是否在播放动画
    anim_file = 'anim_data/neck.anim'  # 动画文件路径
    neck_config_file = 'controller/neck_matrix_config.json'
```

获取树莓派 IP 地址：
```bash
hostname -I
```

#### 7. 配置文件

项目包含两个重要的配置文件：

**树莓派配置文件**：
- 文件位置：`RaspberryPi/controller/neck_matrix_config.json`
- 用途：RBF 插值矩阵和舵机角度映射
- 说明：首次下载时已包含默认配置，可直接使用
- 示例文件：`neck_matrix_config.example.json`（参考用）

**Windows 配置文件**：
- 文件位置：`Windows/NeckControlOutput/neck_config.json`（运行时生成）
- 用途：舵机角度、标签、网络设置
- 说明：首次运行程序时会自动创建
- 示例文件：`neck_config.example.json`（参考用）

**注意**：如果配置文件被意外删除或损坏：
1. 树莓派端：复制 `.example.json` 文件并重命名为 `neck_matrix_config.json`
2. Windows 端：程序会自动创建默认配置，或复制 `neck_config.example.json`

#### 8. 测试运行

**首次运行建议启用调试模式**（无需硬件）：

```python
# 编辑 config.py
DEBUG = True
```

运行程序：
```bash
python main.py
```

选择模式：
```
请选择启动模式：
1. ARKit 实时捕捉
2. 播放动画文件
3. 测试
4. 退出
```

### Unity iOS 应用配置

#### 1. 系统要求
- **操作系统**：macOS 10.15 或更高版本
- **Unity 版本**：2021.3 LTS 或更高版本
- **Xcode**：12.0 或更高版本
- **iOS 设备**：iPhone/iPad（支持 ARKit 4.0 或更高版本）
- **Apple Developer 账号**：用于真机部署

#### 2. 安装 Unity 和依赖

1. **安装 Unity Hub 和 Unity 编辑器**
   - 从 [Unity 官网](https://unity.com/) 下载并安装 Unity Hub
   - 安装 Unity 2021.3 LTS 或更高版本
   - 在安装时选择 **iOS Build Support** 和 **ARKit** 模块

2. **安装 Xcode**
   ```bash
   # 从 Mac App Store 安装 Xcode
   # 或使用命令行工具
   xcode-select --install
   ```

#### 3. 打开 Unity 项目

```bash
cd UnityArkit
# 使用 Unity Hub 打开项目
# 或者直接双击 UnityArkit.sln（如果配置了关联）
```

#### 4. 配置 ARKit 插件

1. 在 Unity 编辑器中，打开 `Window > Package Manager`
2. 确保 `AR Foundation` 和 `ARKit` 包已安装
3. 如果未安装，点击 `+` 按钮选择 `Unity Registry`，搜索并安装

#### 5. 配置网络连接

修改网络相关脚本，确保指向正确的树莓派 IP 地址和端口：

```csharp
// 在网络脚本中配置
private string raspberryPiIp = "192.168.3.11";
private int raspberryPiPort = 9003;
```

#### 6. 构建并部署 iOS 应用

1. **切换平台到 iOS**
   - 打开 `File > Build Settings`
   - 选择 `iOS` 平台
   - 点击 `Switch Platform`

2. **配置构建设置**
   - 在 `Build Settings` 中配置：
     - **Bundle Identifier**：如 `com.yourcompany.HeadNeckControl`
     - **Minimum iOS Version**：12.0（ARKit 要求）
     - **Architecture**：ARM64

3. **构建 Xcode 项目**
   - 点击 `Build` 按钮
   - 选择输出文件夹，如 `Build/iOS`
   - 等待构建完成

4. **在 Xcode 中编译并部署**
   ```bash
   cd Build/iOS
   open Unity-iPhone.xcodeproj
   ```
   - 在 Xcode 中，选择你的开发团队
   - 选择目标设备（真机或模拟器）
   - 点击 `Run` 按钮或按 `Cmd+R`

5. **真机部署要求**
   - 确保设备已通过 USB 连接到 Mac
   - 在设备设置中启用"开发者模式"
   - 信任开发者的证书

#### 7. 使用 ARKit 应用

1. **启动应用**
   - 在 iOS 设备上打开应用
   - 授权相机和 ARKit 权限

2. **连接到树莓派**
   - 确保 iOS 设备和树莓派在同一局域网
   - 应用会自动发送面部姿态数据到树莓派

3. **测试追踪**
   - 将设备对准脸部
   - 应用会实时捕捉面部表情和头部姿态
   - 树莓派端的舵机会根据头部运动实时响应

### Windows 端配置

#### 1. 安装 .NET 8 SDK

下载并安装 [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

#### 2. 打开解决方案

```bash
cd Windows
dotnet build
dotnet run
```

#### 3. 配置连接

- 在界面中设置树莓派的 IP 地址和端口
- 加载或创建 `neck_config.json` 配置文件
- 使用滑块控制舵机角度

### 动画文件使用

#### 1. 录制动画

使用程序录制模式将动作保存到 `.anim` 文件。

#### 2. 放置动画文件

将动画文件放到树莓派的 `anim_data/` 目录：

```bash
mkdir -p anim_data
cp your_animation.anim anim_data/
```

#### 3. 播放动画

运行树莓派程序，选择"播放动画文件"模式，程序会自动播放 `anim_data/neck.anim`。

## 配置文件说明

### `neck_matrix_config.json`（树莓派）

用于 RBF 插值的核心配置文件：

```json
{
  "ServoMinMax": {
    "servo1": [0, 180],   // 舵机最小/最大角度
    "servo2": [180, 15],
    ...
  },
  "NeckAngles": [
    [0, 0, 0],           // 中立位置（x, y, z）
    [30, 0, 0],          // 低头 30 度
    [-20, 0, 0],         // 抬头 20 度
    [0, 30, 0],          // 左转 30 度
    [0, -30, 0],         // 右转 30 度
    ...
  ],
  "MatrixSize": [8, 8],   // 矩阵尺寸
  "sigma": 30.670156,     // RBF 核函数宽度
  "Matrix": [...]         // RBF 插值矩阵（8x8）
}
```

### `neck_config.json`（Windows）

存储舵机角度映射和标签的配置文件：

```json
{
  "ServoMinMax": {
    "servo_0": [0, 180],
    ...
  },
  "NeckData": {
    "0_0_0": {
      "NeckAngle": [0.0, 0.0, 0.0],    // 颈部姿态角度
      "ServoAngle": [90, 90, ...]      // 对应的舵机角度
    }
  },
  "ServoLabels": {
    "servo_0": ["紧", "松"],
    ...
  },
  "ServerIp": "192.168.3.11",    // 树莓派 IP
  "ServerPort": 9003              // UDP 端口
}
```

## 调试技巧

### 1. 启用调试模式

设置 `Config.DEBUG = True`，无需硬件即可运行：

```python
class Config():
    DEBUG = True  # 调试模式（打印舵机命令，不执行硬件操作）
```

在调试模式下，舵机控制命令会打印到控制台：
```
[0, 90]
[1, 95]
...
```

### 2. 网络调试

**检查 UDP 端口是否正常监听**：
```bash
sudo netstat -ulnp | grep 9003
```

**测试 UDP 连接**（从 Windows 发送测试数据）：
```bash
# 使用 netcat 或 PowerShell 发送测试数据
echo "test data" | nc -u 192.168.3.11 9003
```

**使用 Wireshark 抓包**：
- 安装 Wireshark
- 设置过滤器：`udp.port == 9003`
- 查看 UDP 数据包内容

### 3. 舵机调试

**测试单个舵机**：
```python
# 在测试模式下测试单个舵机
# 选择"测试"模式，输入舵机编号和角度
```

**检查 PCA9685 PWM 输出**：
```bash
# 使用示波器或逻辑分析仪测量 PCA9685 的 PWM 输出
# 正常的 PWM 频率应为 50Hz，脉宽范围 500-2500μs
```

### 4. 性能优化

- **调整 PWM 频率**：默认 50Hz，某些舵机可能需要 60Hz
- **调整 RBF sigma 参数**：影响插值平滑度
- **优化线程优先级**：确保控制线程有足够 CPU 时间

## 常见问题

### 树莓派相关问题

**Q: PCA9685 检测不到？**
```bash
# 检查 I2C 是否启用
raspi-config nonint get_i2c

# 检查接线（SDA -> GPIO 2, SCL -> GPIO 3）
# 检查电源电压（PCA9685 需要 3.3V-5V）
```

**Q: 舵机抖动？**
- 检查电源是否充足（5V 3A+）
- 调整 PCA9685 PWM 频率到 60Hz
- 增加 PCA9685 旁路电容（100μF）

**Q: UDP 数据接收不到？**
```bash
# 检查防火墙
sudo ufw status
sudo ufw allow 9003/udp

# 检查端口监听
sudo lsof -i:9003
```

### Windows 端相关问题

**Q: 无法连接到树莓派？**
- 检查 IP 地址是否正确
- 确认两台设备在同一局域网
- 测试网络连通性：`ping 192.168.3.11`

**Q: 配置文件加载失败？**
- 检查 JSON 文件格式是否正确
- 确认文件编码为 UTF-8
- 检查文件路径是否正确

## 开发指南

详细的开发指南请参考 [AGENTS.md](AGENTS.md)，其中包含：
- 构建和测试命令
- 代码风格规范（Python 和 C#）
- 架构设计模式
- 常见问题和解决方案
- 如何添加新的输入源

## 项目结构

```
HeadNeckControl/
 ├── RaspberryPi/              # 树莓派端代码（Python）
 │   ├── main.py             # 主程序入口
 │   ├── config.py           # 配置文件
 │   ├── controller/         # 控制器模块
 │   │   ├── servo_controller.py   # 舵机控制器
 │   │   └── head_mapper.py       # 头部姿态映射器
 │   ├── input/              # 输入源模块
 │   │   ├── base_input.py        # 输入源基类
 │   │   ├── input_arkit.py       # ARKit 输入源
 │   │   ├── input_anim.py        # 动画文件输入源
 │   │   ├── input_ai.py          # AI 输入源
 │   │   └── input_test.py       # 测试输入源
 │   ├── utils/              # 工具模块
 │   │   └── filetools.py        # 文件处理工具
 │   ├── anim_data/          # 动画文件目录
 │   └── requirements.txt    # Python 依赖
 │
 ├── Windows/                # Windows 端代码（C#）
 │   └── NeckControlOutput/
 │       ├── Form1.cs            # 主窗体
 │       ├── Form1.Designer.cs   # 窗体设计器（自动生成）
 │       ├── Models.cs           # 数据模型
 │       ├── RBFHelper.cs        # RBF 插值助手
 │       ├── WeightMatrixHelper.cs # 权重矩阵计算
 │       ├── Program.cs          # 程序入口
 │       └── NeckControlOutput.csproj # 项目文件
 │
 ├── UnityArkit/              # Unity iOS ARKit 应用
 │   ├── Assets/             # Unity 资源和脚本
 │   │   ├── Scenes/          # 场景文件
 │   │   ├── Scripts/         # C# 脚本
 │   │   └── Samples/         # AR Foundation 示例
 │   ├── Packages/           # Unity 包配置
 │   ├── ProjectSettings/     # 项目设置
 │   └── UnityArkit.sln     # 解决方案文件
 │
 ├── 3D-Models/              # 3D 打印模型文件
 │   ├── OBJ/                 # Wavefront OBJ 格式模型
 │   ├── 3MF/                 # 3D Manufacturing Format 格式模型
 │   ├── STL/                 # STL 格式模型
 │   └── README.md            # 3D 打印说明
 │
 ├── AGENTS.md               # 开发者指南
 ├── CHECKLIST.md            # 项目检查清单
 ├── CONTRIBUTING.md         # 贡献指南
 ├── LICENSE                # MIT 许可证
 ├── PACKAGING.md            # 打包发布指南
 ├── README.md              # 本文件
 └── .gitignore            # Git 忽略规则
 ```

## 贡献

欢迎贡献代码、报告问题或提出改进建议！请阅读 [CONTRIBUTING.md](CONTRIBUTING.md) 了解详情。

## 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件。

## 致谢

- Adafruit 的 [CircuitPython](https://circuitpython.org/) 库

## 欢迎围观我的B站

- B站 [星瑶爸爸呀](https://space.bilibili.com/26313918)

---

<div align="center">

**Made with ❤️ for robotics enthusiasts**

</div>
