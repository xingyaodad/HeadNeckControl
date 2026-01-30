from config import Config
import time
import threading
import queue
from adafruit_motor import servo
from adafruit_pca9685 import PCA9685

class ServoController():
    def __init__(self,i2c,address=[0x40],pca_frequency = 50,servo_count = 16):
        self.pca = []
        if not Config.DEBUG:
            for addres in address:
                pca = PCA9685(i2c,address=addres)
                pca.frequency = pca_frequency
                self.pca.append(pca)
        self.servo_count = servo_count
        self.all_servo = []
        tem_count = 0
        if not Config.DEBUG:
            for i in range(len(address)):
                for j in range(16):
                    self.all_servo.append(servo.Servo(self.pca[i].channels[j], min_pulse = 500, max_pulse = 2500))
                    tem_count += 1
                    if tem_count >= servo_count:
                        break
        self.last_angles = [0] * servo_count
        self.queue = queue.Queue()
        self.running = False
        self.thread = None
        
    def _executor_loop(self):
        while self.running:
            try:
                item = self.queue.get(timeout=0.1)
                
                if isinstance(item, dict):
                    for i, angle in item.items():
                        self.set_angle(i, angle)
                elif isinstance(item, list) and all(isinstance(a, (int, float)) for a in item):
                    self.set_angles(item)
                elif isinstance(item, list) and all(isinstance(a, (tuple, list)) for a in item):
                    for i, angle in item:
                        self.set_angle(i, angle)
                else:
                    print(f" 无效舵机任务格式：{item}")
            except queue.Empty:
                continue
    def put(self, task):
        """
        task 可以是：
        - dict: {通道:角度}
        - list of float: 所有通道角度列表
        - list of (通道, 角度) 对
        """
        self.queue.put(task)

    def start(self):
        self.running = True
        self.thread = threading.Thread(target=self._executor_loop, daemon=True)
        self.thread.start()

    def stop(self):
        self.running = False
        if self.thread and self.thread.is_alive():
            self.thread.join()

    def set_angle(self, channel_index, angle):
        """
        直接设置单个舵机角度（立即生效）
        """
        if channel_index >= self.servo_count:
            print(f" 通道 {channel_index} 不对")
            return
        if Config.DEBUG:
            print([channel_index,angle])
        else:
            self.all_servo[channel_index].angle = angle  
    
    def set_angles(self,angles):
        if Config.DEBUG:
            print(angles)
            return
        for i in range(len(angles)):
            if self.last_angles[i] != angles[i]:
                self.all_servo[i].angle = angles[i]
                self.last_angles[i] = angles[i]

    def get_servo_count(self):
        return len(self.all_servo)