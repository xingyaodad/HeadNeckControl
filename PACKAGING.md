# 打包发布指南

本文档说明如何打包和发布 HeadNeckControl 项目。

## Windows 端打包

### 1. 编译 Release 版本

```bash
cd Windows/NeckControlOutput
dotnet build --configuration Release
```

编译后的文件位于：`bin/Release/net8.0-windows/`

### 2. 复制配置文件

Release 编译不会自动包含配置文件，需要手动复制：

```bash
cp neck_config.example.json bin/Release/net8.0-windows/neck_config.json
```

### 3. 打包为 ZIP

将以下文件打包为 ZIP：
- `NeckControlOutput.exe`
- `NeckControlOutput.dll`
- `NeckControlOutput.deps.json`
- `NeckControlOutput.runtimeconfig.json`
- `NumSharp.dll`
- `Numpy.dll`
- `Python.Runtime.dll`
- `Python.Included.dll`
- `Python.Deployment.dll`
- `neck_config.json`

```bash
cd bin/Release/net8.0-windows
zip -r HeadNeckControl-Windows.zip *.dll *.json *.exe
```

### 4. 用户使用

用户下载 ZIP 后：
1. 解压到任意目录
2. 双击 `NeckControlOutput.exe` 运行
3. 首次运行会自动加载或创建配置文件

## 树莓派端打包

### 1. 确保配置文件完整

```bash
cd RaspberryPi
ls -la controller/neck_matrix_config.json
```

### 2. 打包为 ZIP

```bash
cd RaspberryPi
zip -r HeadNeckControl-RaspberryPi.zip \
  *.py \
  config.py \
  controller/ \
  input/ \
  utils/ \
  requirements.txt
```

### 3. 用户部署

用户下载 ZIP 后：
```bash
# 解压
unzip HeadNeckControl-RaspberryPi.zip -d ~/HeadNeckControl
cd ~/HeadNeckControl

# 安装依赖
pip3 install -r requirements.txt

# 运行
python3 main.py
```

## GitHub Release

### 1. 创建 Tag

```bash
git tag -a v1.0.0 -m "Release v1.0.0"
git push origin v1.0.0
```

### 2. 创建 Release

在 GitHub 上：
1. 进入 "Releases" 页面
2. 点击 "Draft a new release"
3. 选择 Tag: v1.0.0
4. 填写 Release Notes
5. 上传打包的 ZIP 文件：
   - `HeadNeckControl-Windows.zip`
   - `HeadNeckControl-RaspberryPi.zip`
6. 点击 "Publish release"

### 3. Release Notes 模板

```markdown
## HeadNeckControl v1.0.0

### 下载

- [Windows 版本](HeadNeckControl-Windows.zip) - 适用于 Windows 10/11
- [树莓派版本](HeadNeckControl-RaspberryPi.zip) - 适用于 Raspberry Pi OS

### 新功能
- 支持 ARKit 实时面部追踪
- 支持动画文件播放
- Windows 端图形界面手动控制
- RBF 插值平滑过渡

### 系统要求

**Windows**：
- Windows 10/11
- .NET 8.0 Runtime（包含在 ZIP 中）

**树莓派**：
- Raspberry Pi 3B+ 或更高
- Raspberry Pi OS Bullseye/Bookworm
- Python 3.10+

### 快速开始

详细说明请参考 [README.md](../../README.md)

### 已知问题
- 无

### 更新日志
- 初始版本发布
```

## 注意事项

1. **配置文件**：确保示例配置文件包含在发行版中
2. **依赖库**：Windows 端需要将 NuGet 包的 DLL 包含在 ZIP 中
3. **版本号**：更新 NeckControlOutput.csproj 中的版本号
4. **文档**：确保 README 和 AGENTS.md 已更新
5. **测试**：在发布前在干净环境中测试 ZIP 文件
