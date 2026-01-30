from config import Config
import time
from abc import ABC, abstractmethod
from controller.servo_controller import ServoController
from controller.head_mapper import HeadMapper
from input.servo_enum import Servo

class BaseInput(ABC):
    """
    所有输入源（如ARKit、动画文件、AI驱动）的抽象基类。
    """
    def __init__(self, controller: ServoController | None=None, mapper: HeadMapper | None = None):
        self.controller = controller
        self.mapper = mapper

    @abstractmethod
    def start(self):
        """
        启动输入源。如果需要线程、连接等初始化，可在此实现。
        """
        pass

    @abstractmethod
    def stop(self):
        """
        停止输入源，用于关闭线程、socket 等。
        """
        pass

    @abstractmethod
    def get_next_frame()-> list[list[float]]:
        """
        获取下一帧的 blendshape 表情数据（float 列表）。
        必须固定长度，保持和舵机映射一致。
        """
        pass

    @abstractmethod
    def is_active(self) -> bool:
        """
        返回当前输入源是否还在运行。
        例如播放动画时可以判断是否播放完。
        """
        pass

    #设置颈部初始值（默认状态）
    def _setInitialValue(self):
        """
        设置颈部舵机的初始位置为中间位置
        """
        if Config.DEBUG:
            return
        # 设置所有颈部舵机到90度（中间位置）
        initial_angles = [90] * 8  # 8个颈部舵机
        self.controller.put(initial_angles)
        time.sleep(1)
