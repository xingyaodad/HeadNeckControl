# 媒体文件指南

本目录用于存放项目的图片、GIF 动画和视频文件，这些文件将在 README 中展示项目效果。

## 目录结构

```
images/media/
├── README.md           # 本文件
├── demo.gif           # 仿生头部运行演示 GIF
├── windows-ui.png      # Windows 控制界面截图
├── hardware.jpg       # 硬件组装照片
├── schematic.png      # 电路连接示意图
├── assembly.jpg       # 3D 打印件组装图
└── ...
```

## 如何添加 GIF 到 README

### 1. 准备 GIF 文件

**录制 GIF 的方法**：

**Windows 系统**：
- 使用 [LICEcap](https://www.cockos.com/licecap/) - 轻量级 GIF 录制工具
- 使用 [OBS Studio](https://obsproject.com/) - 专业录制软件，可导出为 GIF
- 使用 [ScreenToGif](https://www.screentogif.com/) - 专为 GIF 设计

**macOS 系统**：
- 使用 [GIPHY Capture](https://giphy.com/apps/giphycapture)
- 使用 [Loom](https://www.loom.com/) - 在线录制工具
- 使用 QuickTime + [Gifski](https://github.com/sindresorhus/Gifski)

**Linux 系统**：
- 使用 [Kazam](https://launchpad.net/kazam)
- 使用 [SimpleScreenRecorder](http://www.maartenbaert.be/sr/)
- 使用 [byzanz](https://github.com/GNOME/byzanz) - 命令行录制工具

**从视频转换为 GIF**：
```bash
# 使用 FFmpeg
ffmpeg -i input.mp4 -vf "fps=10,scale=600:-1:flags=lanczos" -c:v gif output.gif

# 使用 gifsicle 优化（减小文件大小）
gifsicle -O3 --lossy=80 -o optimized.gif input.gif
```

### 2. GIF 文件建议

**最佳实践**：
- **尺寸**：建议宽度 400-800 像素
- **帧率**：10-15 fps（降低帧率减小文件大小）
- **时长**：3-10 秒（展示核心功能即可）
- **文件大小**：尽量控制在 5MB 以内（GitHub 推荐）
- **循环**：确保 GIF 无缝循环

**命名规范**：
- `demo.gif` - 核心功能演示
- `arkit-tracking.gif` - ARKit 追踪演示
- `animation-playback.gif` - 动画播放演示
- `manual-control.gif` - 手动控制演示

### 3. 在 README 中引用

**基本语法**：
```markdown
![GIF 描述](images/media/demo.gif)
```

**带宽度控制**：
```markdown
<div align="center">

<img src="images/media/demo.gif" width="600" alt="演示 GIF" />

*演示说明文字*

</div>
```

**GitHub 推荐方式**（相对路径）：
```markdown
<img src="images/media/demo.gif" width="600" alt="演示 GIF" />
```

**绝对路径**（如果使用其他图床）：
```markdown
<img src="https://raw.githubusercontent.com/yourusername/HeadNeckControl/main/images/media/demo.gif" width="600" alt="演示 GIF" />
```

### 4. 添加多个 GIF

**并排显示**：
```markdown
<div align="center">

<img src="images/media/demo1.gif" width="400" alt="演示 1" />
<img src="images/media/demo2.gif" width="400" alt="演示 2" />

*左：ARKit 追踪 | 右：动画播放*

</div>
```

**垂直排列**：
```markdown
### ARKit 追踪演示
<img src="images/media/arkit-demo.gif" width="600" alt="ARKit 演示" />

### 动画播放演示
<img src="images/media/animation-demo.gif" width="600" alt="动画演示" />
```

## 添加静态图片

### 1. 图片文件建议

**格式选择**：
- **PNG**：适合截图（支持透明背景）
- **JPG**：适合照片（文件小）
- **SVG**：适合图标和示意图（矢量，无限缩放）

**尺寸建议**：
- 截图：宽度 800-1200 像素
- 照片：宽度 600-1000 像素
- 图标：64×64 或 128×128 像素

### 2. 在 README 中引用

```markdown
### Windows 控制界面

<img src="images/media/windows-ui.png" width="800" alt="Windows 控制界面" />
```

## 优化文件大小

### GIF 优化

**使用在线工具**：
- [Ezgif](https://ezgif.com/optimize) - 在线 GIF 优化
- [GIF Optimizer](https://gifoptimizer.com/) - 批量优化
- [GifSicle](https://www.lcdf.org/gifsicle/) - 命令行工具

**手动优化**：
```bash
# 压缩 GIF
gifsicle -O3 --lossy=80 --colors 256 input.gif -o output.gif

# 调整帧率（从 30fps 降到 15fps）
gifsicle --delay 10 input.gif -o output.gif
```

### 图片优化

**使用在线工具**：
- [TinyPNG](https://tinypng.com/) - PNG 压缩
- [Squoosh](https://squoosh.app/) - Google 图片优化工具

**使用命令行**：
```bash
# 使用 optipng 优化 PNG
optipng -o7 input.png

# 使用 jpegoptim 优化 JPG
jpegoptim --max=80 input.jpg
```

## 媒体文件检查清单

### 录制前
- [ ] 确定要展示的核心功能
- [ ] 准备演示场景和材料
- [ ] 测试录制工具

### 录制后
- [ ] 检查 GIF/图片质量
- [ ] 检查文件大小（建议 <5MB）
- [ ] 测试在本地预览是否正常
- [ ] 验证 GIF 是否无缝循环

### 提交前
- [ ] 文件命名符合规范
- [ ] 文件放置在 `images/media/` 目录
- [ ] README 中的引用路径正确
- [ ] 图片添加了 `alt` 属性（可访问性）
- [ ] 测试 GitHub 预览效果

## 常见问题

### Q: GIF 文件太大怎么办？

**A**: 使用以下方法优化：
1. 降低帧率（15-20 fps）
2. 减小尺寸（宽度 400-600px）
3. 缩短时长（3-8秒）
4. 使用 GIF 优化工具

### Q: GIF 在 GitHub 上不显示？

**A**: 检查以下几点：
1. 文件路径是否正确
2. 文件是否已提交到 Git
3. 文件大小是否超过 10MB（GitHub 限制）
4. 尝试使用绝对路径或 GitHub raw 链接

### Q: 如何录制高质量 GIF？

**A**: 高质量录制建议：
1. 使用专业录制软件（OBS）
2. 录制高分辨率视频（1080p）
3. 后期转换为 GIF 时：
   - 先降低帧率
   - 再降低分辨率
   - 最后优化文件大小

### Q: 可以上传视频吗？

**A**: GitHub README 不直接支持视频播放，但可以：
1. 使用 GIF（推荐）
2. 外链到 YouTube 或 Bilibili
3. 使用 GitHub Pages 嵌入视频

## 推荐的录制场景

### 核心功能演示
1. **ARKit 追踪**：展示头部跟随手机移动
2. **动画播放**：展示预设动作的流畅播放
3. **手动控制**：展示 Windows 端滑块控制
4. **测试模式**：展示单个舵机测试

### 使用场景演示
1. **初始设置**：展示硬件组装和连接
2. **配置界面**：展示参数设置过程
3. **运行效果**：展示实际使用效果

## 外部资源

- **GIF 录制工具**：
  - [LICEcap](https://www.cockos.com/licecap/) - Windows/macOS
  - [ScreenToGif](https://www.screentogif.com/) - Windows
  - [OBS Studio](https://obsproject.com/) - 跨平台

- **GIF 优化工具**：
  - [Ezgif](https://ezgif.com/) - 在线工具
  - [GifSicle](https://www.lcdf.org/gifsicle/) - 命令行

- **图片优化工具**：
  - [TinyPNG](https://tinypng.com/) - 在线压缩
  - [Squoosh](https://squoosh.app/) - Google 工具

## 注意事项

1. **文件大小**：GitHub 推荐 GIF <5MB，图片 <2MB
2. **加载速度**：避免在单个页面使用过多大文件
3. **可访问性**：为所有图片添加 `alt` 属性
4. **版权**：确保使用的是原创内容或有授权
5. **隐私**：避免在截图中泄露敏感信息（IP 地址、密钥等）

## 示例代码

### 完整的 README 演示部分

```markdown
## 效果展示

### 仿生头部运行演示

<div align="center">

<img src="images/media/demo.gif" width="600" alt="仿生头部运行演示" />

*仿生头部实时追踪演示*

</div>

### 多模式控制演示

<div align="center">

| ARKit 追踪 | 动画播放 | 手动控制 |
|:----------:|:----------:|:----------:|
| <img src="images/media/arkit.gif" width="300" alt="ARKit" /> | <img src="images/media/animation.gif" width="300" alt="动画" /> | <img src="images/media/manual.gif" width="300" alt="手动" /> |

</div>

```

---

**提示**：录制 GIF 前，请先在干净的环境下测试功能，确保演示效果流畅且准确。

**Happy Recording! 🎥**
