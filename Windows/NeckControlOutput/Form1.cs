using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Text.Json;
using System;
using Numpy;

namespace NeckControlOutput
{
    public partial class Form1 : Form
    {
        UdpClient? udpClient;
        volatile bool running = true;
        IPEndPoint? remoteEndPoint;

        List<TrackBar> trackBars = new List<TrackBar>();

        List<int> lastValues = new List<int>();

        TrackBar? currentTrackBar = null;
        OneData? lerp_onedata1 = null;
        OneData? lerp_onedata2 = null;

        // 平滑过渡相关
        System.Windows.Forms.Timer? transitionTimer;
        int[] initialServoAngles = new int[8];  // 过渡开始时的初始角度
        int[] targetServoAngles = new int[8];
        const int transitionDurationMs = 1000; // 过渡时间：1秒
        const int transitionIntervalMs = 30; // 更新间隔：30ms (从10ms优化)
        int currentTransitionStep = 0;
        int totalTransitionSteps = 0;
        bool isAutoTransitioning = false;  // 标记是否正在进行自动过渡

        string raspberryPiIp = "192.168.3.11";
        int serverPort = 9003;

        // 线程同步对象
        private readonly object udpLock = new object();
        private readonly object lastValuesLock = new object();
        private readonly object currentTrackBarLock = new object();
        private readonly object lerpDataLock = new object();
        private readonly object timerLock = new object();
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;

            // 初始化平滑过渡定时器
            transitionTimer = new System.Windows.Forms.Timer();
            transitionTimer.Interval = transitionIntervalMs;
            transitionTimer.Tick += TransitionTimer_Tick;
        }

        private void TransitionTimer_Tick(object? sender, EventArgs e)
        {
            if (currentTransitionStep >= totalTransitionSteps)
            {
                // 过渡完成，确保所有舵机到达目标角度
                for (int i = 0; i < trackBars.Count && i < 8; i++)
                {
                    changeTrackBarValue(i, targetServoAngles[i]);
                }
                // 停止定时器并清除标志
                lock (timerLock)
                {
                    transitionTimer?.Stop();
                    isAutoTransitioning = false;
                }
                return;
            }

            // 更新所有舵机位置 - 使用固定起点线性插值
            for (int i = 0; i < trackBars.Count && i < 8; i++)
            {
                if (initialServoAngles[i] != targetServoAngles[i])
                {
                    // 计算当前进度 (0.0 到 1.0)
                    float progress = (float)currentTransitionStep / totalTransitionSteps;

                    // 线性插值：起点 + (终点-起点) * 进度
                    int interpolatedAngle = (int)(initialServoAngles[i] + (targetServoAngles[i] - initialServoAngles[i]) * progress);

                    changeTrackBarValue(i, interpolatedAngle);
                }
            }

            currentTransitionStep++;
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            running = false;
            transitionTimer?.Stop();
            transitionTimer?.Dispose();
            udpClient?.Close();
            udpClient?.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            trackBars.Add(servo_0);
            trackBars.Add(servo_1);
            trackBars.Add(servo_2);
            trackBars.Add(servo_3);
            trackBars.Add(servo_4);
            trackBars.Add(servo_5);
            trackBars.Add(servo_6);
            trackBars.Add(servo_7);

            foreach (TrackBar tb in trackBars)
            {
                tb.Scroll += TrackBar_Scroll;
                tb.MouseDown += TrackBar_Scroll;
                tb.ValueChanged += TrackBar_Scroll;
            }
            trackBar_lerp.Scroll += LerpTrackBar_Scroll;

            // 为标签TextBox添加TextChanged事件
            label061.TextChanged += ServoLabel_TextChanged;
            label060.TextChanged += ServoLabel_TextChanged;
            label051.TextChanged += ServoLabel_TextChanged;
            label050.TextChanged += ServoLabel_TextChanged;
            label041.TextChanged += ServoLabel_TextChanged;
            label040.TextChanged += ServoLabel_TextChanged;
            label070.TextChanged += ServoLabel_TextChanged;
            label071.TextChanged += ServoLabel_TextChanged;
            label031.TextChanged += ServoLabel_TextChanged;
            label030.TextChanged += ServoLabel_TextChanged;
            label011.TextChanged += ServoLabel_TextChanged;
            label010.TextChanged += ServoLabel_TextChanged;
            label021.TextChanged += ServoLabel_TextChanged;
            label020.TextChanged += ServoLabel_TextChanged;
            label001.TextChanged += ServoLabel_TextChanged;
            label000.TextChanged += ServoLabel_TextChanged;

            // 为IP和端口TextBox添加TextChanged事件
            textBox_ip.TextChanged += IpPort_TextChanged;
            textBox_port.TextChanged += IpPort_TextChanged;

            lastValues = new List<int>(new int[trackBars.Count]);

            button_w_min.Click += write_minmax;
            button_w_max.Click += write_minmax;

            comboBox1.SelectedValueChanged += ComboBoxChange;
            comboBox2.SelectedValueChanged += ComboBoxChange;



            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] jsonFiles = Directory.GetFiles(currentDir, "*.json");

            // 查找目标文件在当前目录下的路径
            string targetFileName = "neck_config.json";

            string? targetFile = jsonFiles.FirstOrDefault(file => Path.GetFileName(file).Equals(targetFileName, StringComparison.OrdinalIgnoreCase));

            if (targetFile != null)
            {
                textBox_jsonpath.Text = targetFile;
                LoadTrackBarsFromJson(targetFile);
                LoadComboBox();
            }

            // 如果配置文件中没有IP和端口，设置默认值
            if (string.IsNullOrEmpty(textBox_ip.Text))
            {
                textBox_ip.Text = raspberryPiIp;
            }
            if (string.IsNullOrEmpty(textBox_port.Text))
            {
                textBox_port.Text = serverPort.ToString();
            }

        }

        void LoadComboBox()
        {
            try
            {
                string json = File.ReadAllText(textBox_jsonpath.Text);
                var config = JsonSerializer.Deserialize<DataConfig>(json);
                if (config?.NeckData == null)
                {
                    MessageBox.Show("配置文件格式错误");
                    return;
                }

                var data = config.NeckData;
                int num = data.Keys.Count;
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox1.Items.Add("空");
                comboBox2.Items.Add("空");
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;

                if (num > 0)
                {
                    foreach (var kv in data)
                    {
                        OneData oneData = data[kv.Key];
                        comboBox1.Items.Add(kv.Key);
                        comboBox2.Items.Add(kv.Key);
                    }
                }
                lerp_onedata1 = null;
                lerp_onedata2 = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载配置失败: {ex.Message}");
            }
        }
        void ComboBoxChange(object sender, EventArgs e)
        {
            try
            {
                string json = File.ReadAllText(textBox_jsonpath.Text);
                var config = JsonSerializer.Deserialize<DataConfig>(json);
                if (config?.NeckData == null)
                {
                    return;
                }

                var data = config.NeckData;
                OneData? oneData = null;

                ComboBox? cb = sender as ComboBox;
                if (cb == null) return;

                string cb_name = cb.Name;
                string? sel = cb.SelectedItem as string;

                if (sel == null) return;

                if (sel == "空")
                {
                    // 选择"空"时不启动过渡
                    return;
                }

                foreach (var kv in data)
                {
                    if (kv.Key == sel)
                    {
                        oneData = data[kv.Key];
                        break;
                    }
                }
                if (cb_name == "comboBox1")
                {
                    lock (lerpDataLock)
                    {
                        lerp_onedata1 = oneData;
                    }
                }
                else if (cb_name == "comboBox2")
                {
                    lock (lerpDataLock)
                    {
                        lerp_onedata2 = oneData;
                    }
                }

                if (lerp_onedata1 != null && lerp_onedata1.ServoAngle != null && lerp_onedata1.NeckAngle != null)
                {
                    // 验证数组长度
                    if (lerp_onedata1.ServoAngle.Length < 8 || lerp_onedata1.NeckAngle.Length < 3)
                    {
                        return;
                    }

                    // 启动平滑过渡
                    StartSmoothTransition(lerp_onedata1.ServoAngle);
                    trackBar_lerp.Value = 0;
                    textBox_x.Text = lerp_onedata1.NeckAngle[0].ToString();
                    textBox_y.Text = lerp_onedata1.NeckAngle[1].ToString();
                    textBox_z.Text = lerp_onedata1.NeckAngle[2].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载选择失败: {ex.Message}");
            }
        }

        private void StartSmoothTransition(int[] targetAngles)
        {
            lock (timerLock)
            {
                // 停止当前过渡
                transitionTimer?.Stop();
            }

            // 记录当前舵机角度作为起点
            for (int i = 0; i < trackBars.Count && i < 8; i++)
            {
                initialServoAngles[i] = GetValueSafe(trackBars[i]!);
            }

            // 设置目标角度
            for (int i = 0; i < 8; i++)
            {
                targetServoAngles[i] = i < targetAngles.Length ? targetAngles[i] : 90;
            }

            // 计算过渡步数
            totalTransitionSteps = transitionDurationMs / transitionIntervalMs;
            currentTransitionStep = 0;
            isAutoTransitioning = true;  // 标记开始自动过渡

            // 启动定时器
            lock (timerLock)
            {
                transitionTimer?.Start();
            }
        }

        private void UpdateServoAngles(int[] targetAngles, bool instant = false)
        {
            for (int i = 0; i < trackBars.Count && i < targetAngles.Length; i++)
            {
                changeTrackBarValue(i, targetAngles[i]);
            }
        }


        private void LerpTrackBar_Scroll(object sender, EventArgs e)
        {
            lock (timerLock)
            {
                // 手动拖动插值滑条时，停止自动过渡
                transitionTimer?.Stop();
                isAutoTransitioning = false;
            }

            TrackBar? tb = sender as TrackBar;
            if (tb == null) return;

            lock (lerpDataLock)
            {
                if (lerp_onedata1 == null || lerp_onedata2 == null)
                    return;

                if (lerp_onedata1.ServoAngle == null || lerp_onedata2.ServoAngle == null ||
                    lerp_onedata1.NeckAngle == null || lerp_onedata2.NeckAngle == null)
                    return;
            }

            int[] servo_angle1 = lerp_onedata1.ServoAngle;
            int[] servo_angle2 = lerp_onedata2.ServoAngle;

            float[] neck_angle1 = lerp_onedata1.NeckAngle;
            float[] neck_angle2 = lerp_onedata2.NeckAngle;

            TrackBar? slider = sender as TrackBar;
            if (slider == null) return;
            float tb_value = GetValueSafe(slider) / 100f;

            int[] new_angle = new int[servo_angle1.Length];
            for (int i = 0; i < servo_angle1.Length; i++)
            {
                new_angle[i] = (int)(servo_angle1[i] + (servo_angle2[i] - servo_angle1[i]) * tb_value);
            }
            for (int i = 0; i < trackBars.Count && i < new_angle.Length; i++)
            {
                //trackBars[i].Value = new_angle[i];
                changeTrackBarValue(i, new_angle[i]);
            }
            float[] lerp_angle = new float[3];
            for (int i = 0; i < 3 && i < neck_angle1.Length && i < neck_angle2.Length; i++)
            {
                lerp_angle[i] = neck_angle1[i] + (neck_angle2[i] - neck_angle1[i]) * tb_value;
            }
            textBox_x.Text = lerp_angle[0].ToString("F3");
            textBox_y.Text = lerp_angle[1].ToString("F3");
            textBox_z.Text = lerp_angle[2].ToString("F3");

        }

        void changeTrackBarValue(int index, int value)
        {
            if (index < 0 || index >= trackBars.Count)
                return;

            var tb = trackBars[index];
            if (tb.InvokeRequired)
            {
                tb.Invoke(new Action(() =>
                {
                    tb.Value = value;
                    label_tb_name.Text = "当前舵机：" + tb.Name + " " + tb.Value.ToString();
                }));
                return;
            }

            tb.Value = value;
            label_tb_name.Text = "当前舵机：" + tb.Name + " " + tb.Value.ToString();
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            TrackBar? tb = sender as TrackBar;
            if (tb != null)
            {
                // 只有在非自动过渡时才停止平滑过渡定时器
                if (!isAutoTransitioning)
                {
                    transitionTimer?.Stop();
                }

                string name = tb.Name;

                lock (currentTrackBarLock)
                {
                    currentTrackBar = tb;
                    label_tb_name.Text = "当前舵机：" + tb.Name + " " + tb.Value.ToString();
                }
            }
        }

        void SendData()
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

                int headerSize = 4; // 预留前4个字节
                int floatCount = trackBars.Count + 24;

                float[] values = new float[trackBars.Count];
                byte[] packet = new byte[headerSize + 4 * floatCount];

                int typeId = 1; // 自定义消息类型标识
                Buffer.BlockCopy(BitConverter.GetBytes(typeId), 0, packet, 0, 4); // 写入前4个字节

                while (running)
                {
                    bool changed = false;
                    lock (lastValuesLock)
                    {
                        for (int i = 0; i < trackBars.Count; i++)
                        {
                            int val = GetValueSafe(trackBars[i]!);
                            values[i] = val;

                            // 确保 lastValues 有足够的元素
                            while (lastValues.Count <= i)
                            {
                                lastValues.Add(val);
                            }

                            if (val != lastValues[i])
                            {
                                changed = true;
                                lastValues[i] = val;
                            }
                        }
                    }

                    if (!changed)
                    {
                        Thread.Sleep(33);
                        continue;
                    }

                    // 追加到数据值
                    for (int i = 0; i < values.Length; i++)
                    {
                        Buffer.BlockCopy(BitConverter.GetBytes(values[i]), 0, packet, headerSize + i * 4, 4);
                    }

                    lock (udpLock)
                    {
                        if (udpClient != null && remoteEndPoint != null && running)
                        {
                            try
                            {
                                udpClient.Send(packet, packet.Length, remoteEndPoint);
                            }
                            catch (SocketException)
                            {
                                // UDP 连接已断开，正常退出
                                return;
                            }
                            catch (ObjectDisposedException)
                            {
                                // UDP 客户端已关闭，正常退出
                                return;
                            }
                        }
                    }

                    Thread.Sleep(33);
                }
            }
            catch (Exception ex)
            {
                if (running)
                {
                    // 使用 Invoke 确保在 UI 线程显示错误
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => MessageBox.Show($"发送数据错误: {ex.Message}")));
                    }
                    else
                    {
                        MessageBox.Show($"发送数据错误: {ex.Message}");
                    }
                }
            }
        }
        void LoadTrackBarsFromJson(string filePath)
        {
            if (!File.Exists(filePath)) return;

            try
            {
                string json = File.ReadAllText(filePath);
                var config = JsonSerializer.Deserialize<DataConfig>(json);
                if (config?.NeckData == null)
                {
                    MessageBox.Show("配置文件格式错误");
                    return;
                }

                // 加载IP和端口配置
                if (!string.IsNullOrEmpty(config.ServerIp))
                {
                    textBox_ip.Text = config.ServerIp;
                }
                if (config.ServerPort > 0)
                {
                    textBox_port.Text = config.ServerPort.ToString();
                }

                var data = config.NeckData;
                int num = data.Keys.Count;
                if (num == 0) return;

                // 加载舵机标签文本
                if (config.ServoLabels != null)
                {
                    foreach (var kv in config.ServoLabels)
                    {
                        if (kv.Value != null && kv.Value.Length >= 2)
                        {
                            // 根据舵机名称找到对应的label TextBox
                            string servoName = kv.Key;
                            string[] labels = kv.Value;


                            if (servoName == "servo_0")
                            {
                                label001.Text = labels[0] ?? "紧";
                                label000.Text = labels[1] ?? "松";
                            }
                            else if (servoName == "servo_6")
                            {
                                label061.Text = labels[0] ?? "紧";
                                label060.Text = labels[1] ?? "松";
                            }
                            else if (servoName == "servo_5")
                            {
                                label051.Text = labels[0] ?? "松";
                                label050.Text = labels[1] ?? "紧";
                            }
                            else if (servoName == "servo_4")
                            {
                                label041.Text = labels[0] ?? "松";
                                label040.Text = labels[1] ?? "紧";
                            }
                            else if (servoName == "servo_7")
                            {
                                label070.Text = labels[0] ?? "紧";
                                label071.Text = labels[1] ?? "松";
                            }
                            else if (servoName == "servo_1")
                            {
                                label011.Text = labels[0] ?? "松";
                                label010.Text = labels[1] ?? "紧";
                            }
                            else if (servoName == "servo_3")
                            {
                                label031.Text = labels[0] ?? "松";
                                label030.Text = labels[1] ?? "紧";
                            }
                            else if (servoName == "servo_2")
                            {
                                label021.Text = labels[0] ?? "松";
                                label020.Text = labels[1] ?? "紧";
                            }
                        }
                    }
                }

                foreach (var kv in data)
                {
                    OneData oneData = data[kv.Key];
                    if (oneData?.NeckAngle == null || oneData?.ServoAngle == null) continue;

                    float[] neck_angle = oneData.NeckAngle;
                    int[] servo_angle = oneData.ServoAngle;

                    // 验证数组长度
                    if (neck_angle.Length < 3)
                    {
                        MessageBox.Show($"配置文件错误: NeckAngle 数组长度必须至少为 3，实际为 {neck_angle.Length}");
                        return;
                    }
                    if (servo_angle.Length != trackBars.Count)
                    {
                        MessageBox.Show($"配置文件错误: ServoAngle 数组长度必须为 {trackBars.Count}，实际为 {servo_angle.Length}");
                        return;
                    }

                    for (int i = 0; i < servo_angle.Length && i < trackBars.Count; i++)
                    {
                        var tb = trackBars[i];
                        if (tb.InvokeRequired)
                        {
                            tb.Invoke(new Action(() =>
                            {
                                tb.Value = servo_angle[i];
                            }));
                        }
                        else
                        {
                            tb.Value = servo_angle[i];
                        }
                    }
                    textBox_x.Text = neck_angle[0].ToString();
                    textBox_y.Text = neck_angle[1].ToString();
                    textBox_z.Text = neck_angle[2].ToString();


                    Console.WriteLine($"Key: {kv.Key}, Value: {kv.Value}");
                    break; // 只加载第一个
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载TrackBars配置失败: {ex.Message}");
            }
        }

        private void ServoLabel_TextChanged(object? sender, EventArgs e)
        {
            SaveServoLabels();
        }

        private void IpPort_TextChanged(object? sender, EventArgs e)
        {
            SaveIpPort();
        }

        private void SaveIpPort()
        {
            try
            {
                if (string.IsNullOrEmpty(textBox_jsonpath.Text) || !File.Exists(textBox_jsonpath.Text))
                    return;

                string json = File.ReadAllText(textBox_jsonpath.Text);
                var config = JsonSerializer.Deserialize<DataConfig>(json);
                if (config == null)
                {
                    config = new DataConfig();
                }

                // 更新IP和端口
                config.ServerIp = textBox_ip.Text;
                if (int.TryParse(textBox_port.Text, out int port))
                {
                    config.ServerPort = port;
                }

                string newJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(textBox_jsonpath.Text, newJson);

                // 更新类级别的IP和端口变量
                raspberryPiIp = config.ServerIp;
                serverPort = config.ServerPort;
            }
            catch (Exception ex)
            {
                // 静默失败，不弹窗干扰用户
                Console.WriteLine($"保存IP端口配置失败: {ex.Message}");
            }
        }

        private void SaveServoLabels()
        {
            try
            {
                if (string.IsNullOrEmpty(textBox_jsonpath.Text) || !File.Exists(textBox_jsonpath.Text))
                    return;

                string json = File.ReadAllText(textBox_jsonpath.Text);
                var config = JsonSerializer.Deserialize<DataConfig>(json);
                if (config == null)
                {
                    config = new DataConfig();
                }

                if (config.ServoLabels == null)
                {
                    config.ServoLabels = new Dictionary<string, string[]>();
                }

                // 保存每个舵机的标签
                config.ServoLabels["servo_0"] = new string[] { label001.Text, label000.Text };
                config.ServoLabels["servo_6"] = new string[] { label061.Text, label060.Text };
                config.ServoLabels["servo_5"] = new string[] { label051.Text, label050.Text };
                config.ServoLabels["servo_4"] = new string[] { label041.Text, label040.Text };
                config.ServoLabels["servo_7"] = new string[] { label070.Text, label071.Text };
                config.ServoLabels["servo_1"] = new string[] { label011.Text, label010.Text };
                config.ServoLabels["servo_3"] = new string[] { label031.Text, label030.Text };  // servo_3共用servo_7的标签
                config.ServoLabels["servo_2"] = new string[] { label021.Text, label020.Text };

                string newJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(textBox_jsonpath.Text, newJson);
            }
            catch (Exception ex)
            {
                // 静默失败，不弹窗干扰用户
                Console.WriteLine($"保存标签配置失败: {ex.Message}");
            }
        }

        private void button_stop_send_Click(object sender, EventArgs e)
        {
            lock (udpLock)
            {
                running = false;
                udpClient?.Close();
                udpClient?.Dispose();
                udpClient = null;
            }
            button_send.Enabled = true;
            button_stop.Enabled = false;
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            try
            {
                remoteEndPoint = new IPEndPoint(IPAddress.Parse(raspberryPiIp), serverPort);

                // 为接收回复，绑定一个本地端口
                int localPort = 5001;

                lock (udpLock)
                {
                    // 先释放旧的 UDP 客户端
                    udpClient?.Close();
                    udpClient?.Dispose();
                    udpClient = new UdpClient(localPort);
                    running = true;
                }

                // 创建发送线程
                Thread sendThread = new Thread(new ThreadStart(SendData));
                sendThread.IsBackground = true;
                sendThread.Start();
                button_send.Enabled = false;
                button_stop.Enabled = true;
            }
            catch (FormatException)
            {
                MessageBox.Show("IP 地址格式错误，请检查配置");
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"网络端口错误: {ex.Message}");
            }
        }
        private void button_createjson_Click(object sender, EventArgs e)
        {
            button_createjson.Enabled = false;
            try
            {
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "请选择要保存 JSON 文件的文件夹";
                    dialog.ShowNewFolderButton = true;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedPath = dialog.SelectedPath;
                        string jsonPath = Path.Combine(selectedPath, "neck_config.json");

                        DataConfig config = new DataConfig();
                        var servo_minmax = config.ServoMinMax;
                        foreach (var tb in trackBars)
                        {
                            servo_minmax[tb.Name] = new int[] { 0, 180 };
                        }

                        string newJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(jsonPath, newJson);
                        MessageBox.Show("创建成功");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建配置失败: {ex.Message}");
            }
            finally
            {
                button_createjson.Enabled = true;
            }
        }
        void write_minmax(object sender, EventArgs e)
        {
            if (currentTrackBar == null) return;

            Button? b = sender as Button;
            if (b == null) return;

            string b_name = b.Name;

            try
            {
                string json = File.ReadAllText(textBox_jsonpath.Text);
                var config = JsonSerializer.Deserialize<DataConfig>(json);
                if (config?.ServoMinMax == null)
                {
                    MessageBox.Show("配置文件格式错误");
                    return;
                }

                var servo_minmax = config.ServoMinMax;
                string tb_name = currentTrackBar.Name;
                if (b_name == "button_w_min")
                {
                    if (!servo_minmax.ContainsKey(tb_name))
                    {
                        servo_minmax[tb_name] = new int[2];
                    }
                    servo_minmax[tb_name][0] = currentTrackBar.Value;
                }
                else if (b_name == "button_w_max")
                {
                    if (!servo_minmax.ContainsKey(tb_name))
                    {
                        servo_minmax[tb_name] = new int[2];
                    }
                    servo_minmax[tb_name][1] = currentTrackBar.Value;
                }
                string newJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(textBox_jsonpath.Text, newJson);
                SaveServoLabels();  // 同时保存标签配置
                MessageBox.Show("ok");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"写入配置失败: {ex.Message}");
            }
        }
        private void button_load_json_Click(object sender, EventArgs e)
        {
            button_loadjson.Enabled = false;
            try
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Title = "打开文件";
                    dialog.Filter = "JSON 文件 (*.json)|*.json";

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string jsonPath = dialog.FileName;

                        textBox_jsonpath.Text = jsonPath;
                        LoadTrackBarsFromJson(jsonPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载文件失败: {ex.Message}");
            }
            finally
            {
                button_loadjson.Enabled = true;
            }
        }

        private int GetValueSafe(TrackBar tb)
        {
            if (tb.InvokeRequired)
            {
                int val = 0;
                tb.Invoke(new Action(() => val = tb.Value));
                return val;
            }
            else return tb.Value;
        }
        private void button_w_Click(object sender, EventArgs e)
        {
            button_w.Enabled = false;
            try
            {
                string json = File.ReadAllText(textBox_jsonpath.Text);
                var config = JsonSerializer.Deserialize<DataConfig>(json);
                if (config?.NeckData == null)
                {
                    MessageBox.Show("配置文件格式错误");
                    return;
                }

                var data = config.NeckData;
                int num = data.Keys.Count;

                float x = 0;
                float y = 0;
                float z = 0;
                try
                {
                    x = float.Parse(textBox_x.Text);
                    y = float.Parse(textBox_y.Text);
                    z = float.Parse(textBox_z.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"角度解析错误: {ex.Message}");
                    return;
                }
                string data_name = x.ToString() + "_" + y + "_" + z;
                float[] neck_angle = new float[] {
                x, y, z
                };
                int[] servo_angle = new int[trackBars.Count];
                for (int i = 0; i < trackBars.Count; i++)
                {
                    servo_angle[i] = GetValueSafe(trackBars[i]);
                }
                OneData oneData = new OneData();
                oneData.ServoAngle = servo_angle;
                oneData.NeckAngle = neck_angle;
                data[data_name] = oneData;

                string newJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(textBox_jsonpath.Text, newJson);

                // 自动刷新 ComboBox
                LoadComboBox();
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"写入配置失败: {ex.Message}");
            }
            finally
            {
                button_w.Enabled = true;
            }
        }

        private void button_create_weight_matrix_Click(object sender, EventArgs e)
        {
            button_create_weight_matrix.Enabled = false;
            try
            {
                string json = File.ReadAllText(textBox_jsonpath.Text);
                var config = JsonSerializer.Deserialize<DataConfig>(json);
                if (config?.NeckData == null || config?.ServoMinMax == null)
                {
                    MessageBox.Show("配置文件格式错误");
                    return;
                }

                int servoCount = trackBars.Count;
                var nackConfig = WeightMatrixHelper.CreateWeightMatrixConfig(
                    config, servoCount, RBFHelper.RBF, RBFHelper.GaussianRBF);

                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "请选择要保存 JSON 文件的文件夹";
                    dialog.ShowNewFolderButton = true;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedPath = dialog.SelectedPath;
                        string jsonPath = Path.Combine(selectedPath, "neck_matrix_config.json");

                        string newJson = JsonSerializer.Serialize(nackConfig, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(jsonPath, newJson);
                        MessageBox.Show("保存成功");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建权重矩阵失败: {ex.Message}");
            }
            finally
            {
                button_create_weight_matrix.Enabled = true;
            }
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (currentTrackBar == null) return;
            int v = currentTrackBar.Value;
            if (v < currentTrackBar.Maximum)
                currentTrackBar.Value = v + 1;
        }

        private void btn_sub_Click(object sender, EventArgs e)
        {
            if (currentTrackBar == null) return;
            int v = currentTrackBar.Value;
            if (v > currentTrackBar.Minimum)
                currentTrackBar.Value = v - 1;
        }
    }
}
