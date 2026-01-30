# 如何在 README 中添加 GIF 演示

快速指南：将你的 GIF 动画和截图添加到项目中。

## 快速开始

### 1. 录制或准备媒体文件

**GIF 录制工具推荐**：
- Windows: [LICEcap](https://www.cockos.com/licecap/)
- macOS: [GIPHY Capture](https://giphy.com/apps/giphycapture)
- 跨平台: [OBS Studio](https://obsproject.com/)

**截图工具推荐**：
- Windows: Snipping Tool, ShareX
- macOS: Cmd+Shift+4
- 跨平台: Snipaste

### 2. 放置文件

将媒体文件放入 `images/media/` 目录：

```
images/media/
├── demo.gif           # 主要演示 GIF
├── arkit-tracking.gif # ARKit 追踪演示
├── animation.gif      # 动画播放演示
├── windows-ui.png     # Windows 界面截图
└── hardware.jpg       # 硬件照片
```

### 3. 在 README 中引用

**在 README.md 的"效果展示"部分替换占位符**：

```markdown
### 仿生头部运行演示

<div align="center">

<img src="images/media/demo.gif" width="600" alt="仿生头部运行演示" />

*仿生头部实时追踪演示*

</div>
```

### 4. 提交到 Git

```bash
git add images/media/
git commit -m "Add demo GIF and screenshots"
git push
```

## GIF 优化技巧

如果 GIF 文件太大（>5MB），使用以下方法优化：

### 在线工具
1. 访问 [Ezgif](https://ezgif.com/optimize)
2. 上传你的 GIF
3. 调整设置：
   - 降低帧率到 10-15 fps
   - 缩小尺寸到宽度 400-600px
   - 选择"优化"选项
4. 下载优化后的 GIF

### FFmpeg 命令行
```bash
# 转换视频为 GIF，降低帧率
ffmpeg -i input.mp4 -vf "fps=10,scale=600:-1" output.gif

# 优化 GIF
gifsicle -O3 --lossy=80 output.gif -o optimized.gif
```

## 多个 GIF 并排显示

```markdown
<div align="center">

<table>
  <tr>
    <td align="center">ARKit 追踪</td>
    <td align="center">动画播放</td>
    <td align="center">手动控制</td>
  </tr>
  <tr>
    <td align="center"><img src="images/media/arkit.gif" width="300" /></td>
    <td align="center"><img src="images/media/animation.gif" width="300" /></td>
    <td align="center"><img src="images/media/manual.gif" width="300" /></td>
  </tr>
</table>

</div>
```

## 注意事项

1. **文件大小**：GitHub 推荐 GIF <5MB，图片 <2MB
2. **命名规范**：使用有意义的英文名（小写，连字符分隔）
3. **格式选择**：
   - 演示动画 → GIF
   - 界面截图 → PNG（支持透明）
   - 实物照片 → JPG（文件更小）
4. **版权信息**：确保你有权使用这些媒体文件

## 测试预览

提交前，在本地预览 README：

```bash
# 使用 GitHub CLI（如果已安装）
gh repo view --web

# 或直接推送到 GitHub 测试
git push origin main
```

## 更多帮助

详细说明请参考 `images/media/README.md`

---

**Happy GIF Making! 🎬**
