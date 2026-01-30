using Numpy;

namespace NeckControlOutput
{
    public static class RBFHelper
    {
        /// <summary>
        /// 径向基函数（RBF）- 计算两点之间的欧几里得距离
        /// </summary>
        public static float RBF(NDarray x, NDarray y)
        {
            NDarray z = np.linalg.norm(x - y);
            return (float)z;
        }

        /// <summary>
        /// 高斯径向基函数
        /// </summary>
        public static float GaussianRBF(NDarray x, NDarray y, float sigma = 1.0f)
        {
            var diff = x - y;
            var dist_sq = np.dot(diff, diff);
            return (float)np.exp(-dist_sq / (2f * sigma * sigma));
        }
    }
}
