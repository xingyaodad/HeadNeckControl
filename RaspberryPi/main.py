from config import Config
from input.input_anim import AnimInput
from input.input_arkit import ArkitInput
from input.input_test import Test
from controller.servo_controller import ServoController
from controller.head_mapper import HeadMapper
import time
if not Config.DEBUG:
    import board
    import busio
import sys

# 初始化 I2C 连接
i2c = None
if not Config.DEBUG:
    i2c = busio.I2C(board.SCL, board.SDA)

servo_ctrl = ServoController(i2c=i2c, address=Config.pca9685_address, servo_count=Config.servo_count)
servo_ctrl.start()
head_mapper = HeadMapper(Config.neck_config_file)
run_thread = None

def run_anim():
    global run_thread
    Config.is_anim = True
    run_thread = AnimInput(Config.anim_file, controller=servo_ctrl, mapper=head_mapper)
    run_thread.start()

def run_arkit():
    global run_thread
    Config.is_anim = False
    run_thread = ArkitInput(Config.ip, Config.port, controller=servo_ctrl, mapper=head_mapper)
    run_thread.start()

def test():
    global run_thread
    Config.is_anim = True
    run_thread = Test(Config.ip, Config.port, servo_ctrl, servo_count=Config.test_servo_count)
    run_thread.start()

def exit_out():
    if run_thread != None:
        run_thread.stop()
    servo_ctrl.stop()
    exit()

modes = {
    1: ("ARKit 实时捕捉", run_arkit),
    2: ("播放动画文件", run_anim),
    3: ("测试", test),
    4: ("退出", exit_out)
}

if __name__ == "__main__":
    print("请选择启动模式：")
    for num, (desc, _) in modes.items():
        print(f"{num}. {desc}")
    try:
        while True:
            choice = int(input("输入数字").strip())
            if choice in modes:
                print(f"\n 启动：{modes[choice][0]}\n")
                modes[choice][1]()  # 调用对应的函数

    except ValueError:
        print("请输入合法数字")
