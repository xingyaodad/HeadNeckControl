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
<img src="images/media/test.gif" width="600" alt="仿生脖子运行演示" />

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
- **树莓派**：3B+ 或更高版本
- **windows电脑**：随意，与树莓派需连统一局域网
- **PCA9685 PWM 驱动板**：一块
- **舵机**：MG996R 或类似的金属齿轮舵机 8个，每个需要十字舵臂
- **电源**：12V33A电源 + 2-4个6V5A降压模块（可根据实际情况调整）
- **杜邦线、导线**：若干，连接电源、树莓派、PCA9685 和舵机
- **dyneema绳**：直径1.5mm 3-5米，使用前需要做预拉伸（找个重物吊个一两天）
- **特氟龙硬管**：内径2mm * 外径3mm 3-5米
- **陶瓷眼**：小外径3.9 * 孔2.3 * 总高6  25个
- **小轴承**：9 * 14 * 2.5 8个
- **TPU胶圈**：直径8mm * 宽2mm 8个，可以用差不多的橡皮筋
- **胶水**：502或其他快干胶
- **螺丝**：平头自攻M3 * 8 50个，平头自攻M2 * 10 32个，平头自攻M2 * 8 12个，M2 * 10 + 螺母 8组（螺丝建议多备些，很容易丢😓）

- **面包板（可选）**：用于电路连接
- **锁边液（可选）**：用于把毛躁的线头弄顺，可以更好的穿孔，也可以用普通的办公胶水代替
- **润滑脂（可选）**：关节交接处的润滑，最好有

### 希望你可以有的一些工具
- **螺丝刀、钳子**：如果没有的话就放弃这个项目吧~
- **FDM 3D打印机**：最大打印尺寸最好超过200 * 200 * 200，没有的话找个代打吧~
- **电磨**：用于清理打磨模型粗糙的细节，实在没有用砂纸也不是不行

### 最后希望你可以有
- **很强的动手能力**：这个项目所有东西都需要你自己亲自组装😅
- **很强的耐心**：这套脖子比例大小是按照真实6-7岁孩子尺寸设计的，很多部件非常小且难安装，所以如果你已经开始了，那么一定要耐心一点❤

## 快速开始

### 树莓派环境配置

#### 1. 系统要求
- **操作系统**：Raspberry Pi OS
- **Python 版本**：Python 3.10 或更高（本系统使用 Python 3.11）
- **内存**：至少 2GB RAM（推荐 4GB）

#### 2. 克隆项目（可以在pc上克隆好，把RaspberryPi文件夹拷贝到树莓派上）

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
