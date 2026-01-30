using System.Text.Json;
using Numpy;

namespace NeckControlOutput
{
    public static class WeightMatrixHelper
    {
        /// <summary>
        /// 创建权重矩阵配置
        /// </summary>
        /// <param name="config">原始配置</param>
        /// <param name="servoCount">舵机数量</param>
        /// <param name="rbf">径向基函数</param>
        /// <param name="gaussianRbf">高斯径向基函数</param>
        public static NackMatrixConfig CreateWeightMatrixConfig(DataConfig config, int servoCount,
            Func<NDarray, NDarray, float> rbf,
            Func<NDarray, NDarray, float, float> gaussianRbf)
        {
            var servo_minmax = config.ServoMinMax;
            var neck_data = config.NeckData;
            int neck_data_num = neck_data.Keys.Count;

            if (neck_data_num < servoCount)
            {
                throw new Exception("数据不足");
            }

            string[] keys = neck_data.Keys.ToArray<string>();

            // 计算欧几里得距离矩阵
            float[,] mat0 = new float[neck_data_num, neck_data_num];
            for (int i = 0; i < neck_data_num; i++)
            {
                for (int j = 0; j < neck_data_num; j++)
                {
                    var oneData1 = neck_data[keys[i]];
                    var oneData2 = neck_data[keys[j]];
                    if (oneData1?.NeckAngle == null || oneData2?.NeckAngle == null) continue;

                    float[] neck_angle = oneData1.NeckAngle;
                    float[] neck_angle2 = oneData2.NeckAngle;
                    float dis = rbf(np.array(neck_angle), np.array(neck_angle2));
                    mat0[i, j] = dis;
                }
            }

            // 计算平均距离作为 sigma
            int all_count = neck_data_num * neck_data_num;
            float sigma = 0f;
            for (int i = 0; i < neck_data_num; i++)
            {
                for (int j = 0; j < neck_data_num; j++)
                {
                    sigma += mat0[i, j];
                }
            }
            sigma /= all_count;

            // 计算高斯核矩阵
            float[,] mat = new float[neck_data_num, neck_data_num];
            for (int i = 0; i < neck_data_num; i++)
            {
                for (int j = 0; j < neck_data_num; j++)
                {
                    var oneData1 = neck_data[keys[i]];
                    var oneData2 = neck_data[keys[j]];
                    if (oneData1?.NeckAngle == null || oneData2?.NeckAngle == null) continue;

                    float[] neck_angle = oneData1.NeckAngle;
                    float[] neck_angle2 = oneData2.NeckAngle;
                    float dis = gaussianRbf(np.array(neck_angle), np.array(neck_angle2), sigma);
                    mat[i, j] = dis;
                }
            }

            // 构建舵机数据矩阵
            float[,] servo_mat = new float[neck_data_num, servoCount];
            for (int i = 0; i < neck_data_num; i++)
            {
                var oneData = neck_data[keys[i]];
                if (oneData?.ServoAngle == null) continue;

                int[] servo_angle = oneData.ServoAngle;
                for (int j = 0; j < servo_angle.Length; j++)
                {
                    servo_mat[i, j] = servo_angle[j];
                }
            }

            // 计算权重矩阵
            NDarray nack_array = np.array(mat);
            NDarray servo_array = np.array(servo_mat);
            var weight_mat = np.matmul(np.linalg.inv(nack_array), servo_array);

            // 提取颈部角度
            float[][] neck_angles = new float[neck_data_num][];
            for (int i = 0; i < neck_data_num; i++)
            {
                var oneData = neck_data[keys[i]];
                neck_angles[i] = oneData?.NeckAngle ?? Array.Empty<float>();
            }

            // 构建 NACK 矩阵配置
            int rows = weight_mat.shape[0];
            int cols = weight_mat.shape[1];
            float[][] result = new float[rows][];
            for (int i = 0; i < rows; i++)
            {
                float[] col_data = new float[cols];
                for (int j = 0; j < cols; j++)
                {
                    col_data[j] = (float)weight_mat[i, j];
                }
                result[i] = col_data;
            }

            NackMatrixConfig nack_config = new NackMatrixConfig
            {
                ServoMinMax = servo_minmax,
                NeckAngles = neck_angles,
                sigma = sigma,
                Matrix = result,
                MatrixSize = new int[] { rows, cols }
            };

            return nack_config;
        }
    }
}
