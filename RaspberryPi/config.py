class Config():
    DEBUG = True
    ip = ''
    port = 9003
    bs_count = 52
    servo_count = 8  # 只保留颈部8个舵机
    test_servo_count = 8
    pca9685_address = [0x40]
    pca_frequency = 50
    is_anim = False #是否是加载动画文件
    anim_file = 'anim_data/neck.anim'
    neck_config_file = 'controller/neck_matrix_config.json'