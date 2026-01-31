# HeadNeckControl

<div align="center">

**基于RBF的仿生颈部舵机控制系统**

通过 8 个舵机实现仿生头部的实时控制和动画播放

</div>

---

## 效果展示

### 仿生脖子运行演示

<div align="center">

 <!-- 替换为你的实际 GIF 文件 -->
<img src="images/media/demo.gif" width="600" alt="仿生脖子运行演示" />

*仿生脖子实时追踪演示*

</div>

---

## 项目简介

HeadNeckControl 是一个完整的仿生颈部控制系统，通过树莓派和 PCA9685 驱动 8 个舵机来控制仿生头部的运动。系统支持多种控制方式：手机 ARKit 面部追踪、动画文件播放、Windows 端手动控制和测试模式，可以精确控制头部的俯仰、偏航和滚动三个自由度。

## 功能特点

- **ARKit 实时追踪**：通过 iOS 设备使用 ARKit 捕捉面部姿态，通过 UDP 直接发送到树莓派进行实时控制
- **Unity iOS 应用**：提供完整的 Unity 项目，用于生成 ARKit 面部追踪 iOS 应用，支持自定义和扩展
- **动画播放**：支持播放预设的头部动作序列，动画文件存储在树莓派本地
- **Windows 手动控制**：提供图形界面，可通过滑块直接控制舵机角度
- **测试模式**：用于调试和校准舵机的测试模式
- **平滑过渡**：使用 RBF（径向基函数）插值算法，实现流畅的动作过渡
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
- **iOS ARKit 端**：直接通过 UDP 发送 ARKit 捕捉的面部姿态数据到树莓派
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
- **3D 打印机**：FDM 打印机，打印尺寸至少 256×256×256mm
- **打印材料**：PLA、PETG 或 ABS（推荐 PETG）
- **切片软件**：Bambu Studio
- **模型文件**：`3D-Models/` 目录包含 OBJ模型文件、3MF切片文件

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

#### 6. 配置

编辑 `config.py`：

```python
class Config():
    DEBUG = False        # 生产环境设为 False
    ip = ''            # 树莓派的 IP 地址（自动获取）
    port = 9003        # UDP 接收端口
    servo_count = 8     # 舵机数量
    test_servo_count = 8 #测试用的与上面保持一致就行
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


**树莓派配置文件**：
- 文件位置：`RaspberryPi/controller/neck_matrix_config.json`
- 用途：RBF 插值矩阵和舵机角度映射
- 说明：需要在PC端手动生成

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

#### 7. 使用 ARKit 应用

1. **启动应用**
   - 在 iOS 设备上打开应用
   - 授权相机和 ARKit 权限

2. **连接到树莓派**
   - 确保 iOS 设备和树莓派在同一局域网
   - 点击按钮发送面部姿态数据到树莓派

3. **测试追踪**
   - 将设备对准脸部
   - 应用会实时捕捉面部表情和头部姿态
   - 树莓派端的舵机会根据头部运动实时响应


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
