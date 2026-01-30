import threading
import os
import time
from pathlib import Path
from input.base_input import BaseInput
from utils.filetools import *
from controller.servo_controller import ServoController
from controller.head_mapper import HeadMapper

class AnimInput(BaseInput):
    def __init__(self, filepath, fps=30, controller: ServoController | None=None, mapper: HeadMapper | None = None):
        super().__init__(controller, mapper)
        self.controller = controller
        self.mapper = mapper
        self.filepath = Path(filepath)
        self.fps = fps
        self.frames = self._load_faceblend_file(self.filepath)
        self.index = 0
        self.running = False
        self.lock = threading.Lock()
        self.thread = None

    def _load_faceblend_file(self, path: Path) -> list[list[float]]:
        """
        加载动画文件，只提取旋转数据
        返回格式: [[rot_x, rot_y, rot_z], ...]
        """
        if not os.path.isfile(path):
            return None
        datas = []
        try:
            with open(path, 'r', encoding='utf-8') as f:
                lines = f.read().splitlines()
                if len(lines) < 2:
                    return None

                for line in lines[1:]:  # 跳过第一行（blendshape 数量）
                    if not line.strip():
                        continue  # 跳过空行

                    parts = line.strip().split()
                    if len(parts) < 3:
                        continue  # 数据不完整
                    try:
                        # 只提取旋转数据 (前3个值)
                        rot = [float(parts[0]), float(parts[1]), float(parts[2])]
                        datas.append(rot)
                    except ValueError:
                        continue  # 某行包含非法数字，跳过

        except Exception as e:
            print(f"读取文件失败: {e}")
            return None
        return datas if datas else None

    def start(self):
        super()._setInitialValue()
        self.running = True
        self.thread = threading.Thread(target=self._playback_loop, daemon=True)
        self.thread.start()

    def _playback_loop(self):
        if self.frames is None:
            return
        interval = 1.0 / self.fps
        while self.running:
            with self.lock:
                current_frame = self.frames[self.index]
                self.index = (self.index + 1) % len(self.frames)
                # 直接传递旋转数据到mapper
                angles = self.mapper.map(current_frame)
                self.controller.put(angles)
            time.sleep(interval)

    def stop(self):
        self.running = False
        if self.thread and self.thread.is_alive():
            self.thread.join()

    def get_next_frame(self):
        with self.lock:
            if self.frames is None or len(self.frames) == 0:
                return [0.0, 0.0, 0.0]
            return self.frames[self.index].copy()

    def is_active(self) -> bool:
        return self.running
