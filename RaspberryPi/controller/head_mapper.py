from config import Config
from utils.filetools import *
import json
import numpy as np


class HeadMapper:
    def __init__(self, neck_config_file):
        self.neck_config_file = neck_config_file

        # 颈部控制相关属性
        self.neck_minmax_values = []
        self.neck_angles = []
        self.neck_data = {}
        self.neck_rbf_sigma = 1.0
        self.neck_matrix_size = []
        self.neck_matrix = None
        self.neck_prev_rot = [0] * 3
        self.neck_angle_minmax = None  # [x_min, x_max, y_min, y_max, z_min, z_max]，由 _loadNeckConfigJson 加载
        self._loadNeckConfigJson()

    def _loadNeckConfigJson(self):
        with open(self.neck_config_file, "r", encoding="utf-8") as f:
            data = json.load(f)
        ServoMinMax = data["ServoMinMax"]
        for k in ServoMinMax.keys():
            self.neck_minmax_values.append(sorted(ServoMinMax[k]))
        self.neck_angles = data["NeckAngles"]
        self.neck_rbf_sigma = data["sigma"]
        self.neck_matrix_size = data["MatrixSize"]
        self.neck_matrix = np.array(data["Matrix"])

        # 验证并加载 neck_angle_minmax（必须存在且格式正确）
        if "NeckAngleMinMax" not in data:
            raise ValueError(
                f"配置文件 {self.neck_config_file} 中缺少必需的 NeckAngleMinMax 字段。"
                "请使用 Windows 端重新生成配置文件。"
            )

        neck_angle_minmax = data["NeckAngleMinMax"]
        if not isinstance(neck_angle_minmax, list) or len(neck_angle_minmax) != 6:
            raise ValueError(
                f"配置文件 {self.neck_config_file} 中的 NeckAngleMinMax 格式错误。"
                "应为包含6个元素的列表 [x_min, x_max, y_min, y_max, z_min, z_max]。"
                f"实际内容: {neck_angle_minmax}"
            )

        self.neck_angle_minmax = neck_angle_minmax

    def _lerpServoAngle(self, servos1, servos2, value):
        if len(servos1) != len(servos2):
            return servos1
        new_servos = [90] * len(servos1)
        for i in range(len(new_servos)):
            new_servos[i] = (servos2[i] - servos1[i]) * value + servos1[i]
        return new_servos

    def _rbf(self, x, y):
        return np.linalg.norm(x - y)

    def _gaussian_rbf(self, x, y):
        diff = x - y
        dist_sq = np.dot(diff, diff)  # 快速计算 ||x - y||^2
        return np.exp(-dist_sq / (2 * self.neck_rbf_sigma**2))

    def _getNeckServoAngle(self, rot_data):
        rotx = rot_data[0]
        roty = rot_data[1]
        rotz = rot_data[2]

        # 应用 neck_angle_minmax 限制（配置文件验证通过后一定存在）
        x_min, x_max, y_min, y_max, z_min, z_max = self.neck_angle_minmax
        rotx = max(x_min, min(x_max, rotx))
        roty = max(y_min, min(y_max, roty))
        rotz = max(z_min, min(z_max, rotz))

        if not Config.is_anim:
            # 如果是加载动画，或是测试，不需要搞这些
            self.neck_prev_rot[0] = self.neck_prev_rot[0] * 0.7 + rotx * 0.3
            self.neck_prev_rot[1] = self.neck_prev_rot[1] * 0.7 + roty * 0.3
            self.neck_prev_rot[2] = self.neck_prev_rot[2] * 0.7 + rotz * 0.3
            rotx = self.neck_prev_rot[0]
            roty = self.neck_prev_rot[1]
            rotz = self.neck_prev_rot[2]

        rot = np.array([rotx, roty, rotz])
        mat = []
        for i in range(len(self.neck_angles)):
            mat.append(self._gaussian_rbf(rot, np.array(self.neck_angles[i])))
        mat = np.array(mat)
        angles = np.matmul(mat, self.neck_matrix)
        angles = angles.tolist()
        for i in range(self.neck_matrix_size[1]):
            if angles[i] < self.neck_minmax_values[i][0]:
                angles[i] = self.neck_minmax_values[i][0]
            elif angles[i] > self.neck_minmax_values[i][1]:
                angles[i] = self.neck_minmax_values[i][1]
        return angles

    def map(self, data):
        """
        映射旋转数据到颈部舵机角度
        data: [rot_x, rot_y, rot_z] 或者 [[rot_x, rot_y, rot_z], blendshapes]
        """
        # 兼容旧格式数据
        if isinstance(data, list) and len(data) == 2:
            rot = data[0]
        else:
            rot = data

        all_neck_angle = self._getNeckServoAngle(rot)
        return all_neck_angle
