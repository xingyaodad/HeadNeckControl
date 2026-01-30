import threading
import os
import time
import socket
import struct
from utils.filetools import *
from controller.servo_controller import ServoController
from controller.head_mapper import HeadMapper

class Test():
    def __init__(self, ip='', port=9003, controller: ServoController | None=None, servo_count=8):
        self.ip = ip
        self.port = port
        self.controller = controller
        self.socket = socket
        self.running = False
        self.lock = threading.Lock()
        self.thread = None
        self.servo_count = servo_count
        self.last_value = [0] * self.servo_count

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

    def _receive_data(self):
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.socket.bind((self.ip, self.port))
        self.socket.setblocking(False)
        while self.running:
            with self.lock:
                client_data = []
                try:
                    time.sleep(0.005)
                    client_data = self.socket.recv(1024)
                except Exception as e:
                    continue

                if len(client_data) < 8:
                    continue

                head = struct.unpack('i', client_data[:4])[0]

                # 模式0: 控制前8个舵机（颈部舵机）
                if head == 1:
                    for i in range(8):
                        offset = i * 4 + 4
                        value = struct.unpack('f', client_data[offset:offset + 4])[0]
                        if self.last_value[i] != value:
                            self.controller.put([[i, value]])
                            self.last_value[i] = value
