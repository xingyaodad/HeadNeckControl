# 贡献指南

感谢你考虑为 HeadNeckControl 项目做出贡献！我们欢迎各种形式的贡献，包括但不限于：错误报告、功能建议、代码改进和文档更新。

## 行为准则

- 保持尊重和专业的交流
- 欢迎新手提问和寻求帮助
- 建设性地讨论不同观点
- 尊重所有贡献者

## 如何贡献

### 报告问题

在提交 Issue 之前，请先搜索现有的 Issues，确保问题未被报告。

创建新 Issue 时，请包含：

- **标题**：简明扼要地描述问题
- **问题描述**：详细说明发生了什么
- **重现步骤**：如何复现问题
- **预期行为**：期望发生什么
- **实际行为**：实际发生了什么
- **环境信息**：
  - 操作系统和版本
  - Python 版本（树莓派端）
  - .NET 版本（Windows 端）
  - 硬件型号（树莓派版本、舵机型号等）
- **日志/截图**：相关的错误日志或截图
- **附加信息**：任何其他有助于解决问题的信息

### 提交代码

#### 1. Fork 项目

点击 GitHub 页面右上角的 "Fork" 按钮

#### 2. 创建分支

```bash
# 克隆你的 fork
git clone https://github.com/yourusername/HeadNeckControl.git
cd HeadNeckControl

# 添加上游仓库
git remote add upstream https://github.com/original-owner/HeadNeckControl.git

# 创建特性分支
git checkout -b feature/your-feature-name
# 或修复分支
git checkout -b fix/bug-description
```

#### 3. 进行开发

在开发之前，请阅读 [AGENTS.md](AGENTS.md) 中的代码风格指南：

**Python 代码风格**：
- 遵循 PEP 8 规范
- 使用 Python 3.10+ 类型提示：`Type | None` 而不是 `Optional[Type]`
- 导入顺序：标准库 → 第三方库 → 本地模块
- 使用 `Config.DEBUG` 进行硬件抽象

**C# 代码风格**：
- 类和方法使用 PascalCase
- 变量和字段使用 camelCase
- 事件处理器命名：`controlName_EventName`
- 跨线程 UI 更新使用 `InvokeRequired` 模式
- 使用 `System.Text.Json` 进行序列化

#### 4. 测试你的修改

**测试 Python 代码**：
```bash
cd RaspberryPi
# 启用调试模式（无需硬件）
# 编辑 config.py: DEBUG = True
python main.py

# 如果有测试
pytest -v
```

**测试 C# 代码**：
```bash
cd Windows
dotnet build
dotnet run
```

#### 5. 提交更改

```bash
# 查看更改
git status
git diff

# 添加文件
git add .

# 提交（使用清晰的提交信息）
git commit -m "Add feature to support new servo configuration"
```

提交信息格式：
- 添加功能：`Add: description`
- 修复 bug：`Fix: description`
- 重构：`Refactor: description`
- 文档更新：`Docs: description`

#### 6. 同步上游仓库

```bash
# 获取上游更新
git fetch upstream

# 合并上游主分支
git checkout main
git merge upstream/main

# 将更新合并到你的分支
git checkout feature/your-feature-name
git merge main
```

#### 7. 推送到你的 fork

```bash
git push origin feature/your-feature-name
```

#### 8. 创建 Pull Request

在 GitHub 上创建 Pull Request：
- 提供清晰的标题和描述
- 说明你的修改解决了什么问题或添加了什么功能
- 引用相关的 Issue（如果有）
- 确保所有检查通过（CI/CD）

### Pull Request 审查流程

1. **自动检查**：CI 会自动运行测试和代码检查
2. **人工审查**：维护者会审查你的代码
3. **反馈**：可能需要根据反馈进行修改
4. **合并**：审查通过后，代码将被合并到主分支

## 开发建议

1. **保持更改最小化**：每个 PR 应该专注于一个功能或修复
2. **编写测试**：为新功能或修复添加相应的测试
3. **更新文档**：如果你的修改影响用户使用，请更新 README 或相关文档
4. **兼容性**：确保修改不会破坏现有功能
5. **代码审查**：提交前自己审查一遍代码

## 项目结构

```
HeadNeckControl/
├── RaspberryPi/          # 树莓派端代码（Python）
│   ├── main.py          # 主程序入口
│   ├── config.py        # 配置文件
│   ├── controller/      # 控制器模块
│   │   ├── servo_controller.py
│   │   └── head_mapper.py
│   ├── input/           # 输入源模块
│   │   ├── base_input.py
│   │   ├── input_arkit.py
│   │   ├── input_anim.py
│   │   ├── input_test.py
│   │   └── input_ai.py
│   ├── utils/           # 工具模块
│   ├── anim_data/       # 动画文件目录
│   └── requirements.txt # Python 依赖
│
├── Windows/             # Windows 端代码（C#）
│   └── NeckControlOutput/
│       ├── Form1.cs     # 主窗体
│       ├── Models.cs    # 数据模型
│       ├── RBFHelper.cs # RBF 插值助手
│       ├── WeightMatrixHelper.cs
│       └── NeckControlOutput.csproj
│
├── AGENTS.md           # 开发者指南
├── CONTRIBUTING.md     # 本文件
├── LICENSE            # MIT 许可证
└── README.md          # 项目说明
```

## 常见任务

### 添加新的输入源

1. 在 `RaspberryPi/input/` 创建新类，继承自 `BaseInput`
2. 实现所有抽象方法：`start()`, `stop()`, `get_next_frame()`, `is_active()`
3. 在 `main.py` 中注册新的输入源
4. 更新文档

### 修改舵机配置

1. 编辑 `neck_matrix_config.json`（树莓派端）或 `neck_config.json`（Windows 端）
2. 调整 `ServoMinMax` 中的舵机最小/最大角度
3. 更新 `NeckAngles` 添加新的姿态点
4. 重新计算 RBF 矩阵（使用 Windows 端的"创建权重矩阵"功能）

### 添加新功能

1. 先创建 Issue 讨论功能设计
2. 创建分支进行开发
3. 遵循现有代码风格和模式
4. 添加测试和文档
5. 提交 Pull Request

## 获取帮助

- **查看文档**：[README.md](README.md) 和 [AGENTS.md](AGENTS.md)
- **搜索 Issues**：查看是否有类似问题已被解决
- **创建 Issue**：如果找不到答案，创建新的 Issue
- **讨论**：在 Issue 中提问或讨论想法

## 许可证

通过贡献代码，你同意你的贡献将在 MIT 许可证下发布。

---

再次感谢你的贡献！🎉
