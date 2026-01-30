using System.Text.Json;

namespace NeckControlOutput
{
    public class DataConfig
    {
        public Dictionary<string, int[]> ServoMinMax { get; set; } = new();
        public Dictionary<string, OneData> NeckData { get; set; } = new();
        public Dictionary<string, string[]> ServoLabels { get; set; } = new();
        public string ServerIp { get; set; } = "192.168.3.11";
        public int ServerPort { get; set; } = 9003;
    }

    public class NackMatrixConfig
    {
        public Dictionary<string, int[]> ServoMinMax { get; set; } = new();
        public float[][] NeckAngles { get; set; } = Array.Empty<float[]>();
        public int[] MatrixSize { get; set; } = Array.Empty<int>();
        public float sigma { get; set; }
        public float[][] Matrix { get; set; } = Array.Empty<float[]>();
    }

    public class OneData
    {
        public float[] NeckAngle { get; set; } = Array.Empty<float>();
        public int[] ServoAngle { get; set; } = Array.Empty<int>();
    }
}
