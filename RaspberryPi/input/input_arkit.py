import threading
import time
import socket
import struct
from input.base_input import BaseInput
from utils.filetools import *
from controller.servo_controller import ServoController
from controller.head_mapper import HeadMapper

class ArkitInput(BaseInput):
    def __init__(self, ip='', port=9003, controller: ServoController | None=None, mapper: HeadMapper | None = None):
        super().__init__(controller, mapper)
        self.controller = controller
        self.mapper = mapper
        self.ip = ip
        self.port = port
        self.socket = socket
        self.running = False
        self.lock = threading.Lock()
        self.current_frame = [0.0, 0.0, 0.0]  # 只保存旋转数据
        self.thread = None

    def _receive_data(self):
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.socket.bind((self.ip, self.port))
        self.socket.setblocking(False)
        while self.running:
            with self.lock:
                client_data = []
                try:
                    time.sleep(0.005)
                    client_data = self.socket.recv(274)
                except Exception as e:
                    continue

                # ARKit数据格式验证
                if client_data[0] == 42 and len(client_data) == 274:
                    rot = []
                    offset = 221  # 旋转数据从偏移221开始

                    # 提取3个旋转值
                    for _ in range(3):
                        value, = struct.unpack('f', client_data[offset:offset+4])
                        rot.append(value)
                        offset += 4

                    # 更新当前帧数据
                    self.current_frame = rot

                    # 映射到舵机角度
                    angles = self.mapper.map(rot)
                    self.controller.put(angles)

    def start(self):
        self.running = True
        self.thread = threading.Thread(target=self._receive_data, daemon=True)
        self.thread.start()

    def stop(self):
        self.running = False
        if self.thread and self.thread.is_alive():
            self.thread.join()
        try:
            self.socket.shutdown(socket.SHUT_RDWR)
        except:
            pass
        self.socket.close()

    def get_next_frame(self):
        with self.lock:
            return self.current_frame.copy()

    def is_active(self):
        return self.running
