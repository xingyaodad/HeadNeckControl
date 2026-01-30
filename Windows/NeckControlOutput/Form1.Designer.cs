namespace NeckControlOutput
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            servo_7 = new TrackBar();
            servo_3 = new TrackBar();
            servo_5 = new TrackBar();
            servo_1 = new TrackBar();
            servo_4 = new TrackBar();
            servo_6 = new TrackBar();
            label_nack_angle = new Label();
            textBox_x = new TextBox();
            textBox_y = new TextBox();
            textBox_z = new TextBox();
            button_w = new Button();
            button_send = new Button();
            button_stop = new Button();
            button_createjson = new Button();
            textBox_jsonpath = new TextBox();
            button_loadjson = new Button();
            servo_2 = new TrackBar();
            servo_0 = new TrackBar();
            button_w_min = new Button();
            button_w_max = new Button();
            trackBar_lerp = new TrackBar();
            button_create_weight_matrix = new Button();
            comboBox1 = new ComboBox();
            comboBox2 = new ComboBox();
            label_tb_name = new Label();
            label061 = new TextBox();
            label060 = new TextBox();
            label051 = new TextBox();
            label050 = new TextBox();
            label041 = new TextBox();
            label040 = new TextBox();
            label070 = new TextBox();
            label071 = new TextBox();
            label031 = new TextBox();
            label030 = new TextBox();
            label011 = new TextBox();
            label010 = new TextBox();
            label021 = new TextBox();
            label020 = new TextBox();
            label001 = new TextBox();
            label000 = new TextBox();
            label17 = new Label();
            label18 = new Label();
            label19 = new Label();
            label20 = new Label();
            label21 = new Label();
            label22 = new Label();
            label23 = new Label();
            label24 = new Label();
            label25 = new Label();
            btn_add = new Button();
            btn_sub = new Button();
            label26 = new Label();
            label27 = new Label();
            label28 = new Label();
            label29 = new Label();
            label1 = new Label();
            textBox_ip = new TextBox();
            label2 = new Label();
            textBox_port = new TextBox();
            ((System.ComponentModel.ISupportInitialize)servo_7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)servo_3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)servo_5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)servo_1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)servo_4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)servo_6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)servo_2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)servo_0).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar_lerp).BeginInit();
            SuspendLayout();
            // 
            // servo_7
            // 
            servo_7.BackColor = SystemColors.Control;
            servo_7.LargeChange = 1;
            servo_7.Location = new Point(67, 293);
            servo_7.Maximum = 180;
            servo_7.Name = "servo_7";
            servo_7.Size = new Size(230, 45);
            servo_7.TabIndex = 0;
            servo_7.TickStyle = TickStyle.Both;
            servo_7.Value = 90;
            // 
            // servo_3
            // 
            servo_3.BackColor = SystemColors.Control;
            servo_3.LargeChange = 1;
            servo_3.Location = new Point(354, 293);
            servo_3.Maximum = 180;
            servo_3.Name = "servo_3";
            servo_3.Size = new Size(230, 45);
            servo_3.TabIndex = 2;
            servo_3.TickStyle = TickStyle.Both;
            servo_3.Value = 90;
            // 
            // servo_5
            // 
            servo_5.BackColor = SystemColors.Control;
            servo_5.LargeChange = 1;
            servo_5.Location = new Point(303, 25);
            servo_5.Maximum = 180;
            servo_5.Name = "servo_5";
            servo_5.Orientation = Orientation.Vertical;
            servo_5.RightToLeft = RightToLeft.No;
            servo_5.Size = new Size(45, 230);
            servo_5.TabIndex = 3;
            servo_5.TickStyle = TickStyle.Both;
            servo_5.Value = 90;
            // 
            // servo_1
            // 
            servo_1.BackColor = SystemColors.Control;
            servo_1.LargeChange = 1;
            servo_1.Location = new Point(303, 362);
            servo_1.Maximum = 180;
            servo_1.Name = "servo_1";
            servo_1.Orientation = Orientation.Vertical;
            servo_1.Size = new Size(45, 230);
            servo_1.TabIndex = 4;
            servo_1.TickStyle = TickStyle.Both;
            servo_1.Value = 90;
            // 
            // servo_4
            // 
            servo_4.BackColor = SystemColors.Control;
            servo_4.LargeChange = 1;
            servo_4.Location = new Point(471, 25);
            servo_4.Maximum = 180;
            servo_4.Name = "servo_4";
            servo_4.Orientation = Orientation.Vertical;
            servo_4.Size = new Size(45, 236);
            servo_4.TabIndex = 5;
            servo_4.TickStyle = TickStyle.Both;
            servo_4.Value = 90;
            // 
            // servo_6
            // 
            servo_6.BackColor = SystemColors.Control;
            servo_6.LargeChange = 1;
            servo_6.Location = new Point(147, 25);
            servo_6.Maximum = 180;
            servo_6.Name = "servo_6";
            servo_6.Orientation = Orientation.Vertical;
            servo_6.Size = new Size(45, 236);
            servo_6.TabIndex = 6;
            servo_6.TickStyle = TickStyle.Both;
            servo_6.Value = 90;
            // 
            // label_nack_angle
            // 
            label_nack_angle.AutoSize = true;
            label_nack_angle.Location = new Point(645, 199);
            label_nack_angle.Name = "label_nack_angle";
            label_nack_angle.Size = new Size(56, 17);
            label_nack_angle.TabIndex = 12;
            label_nack_angle.Text = "脖子角度";
            // 
            // textBox_x
            // 
            textBox_x.Location = new Point(727, 196);
            textBox_x.Name = "textBox_x";
            textBox_x.Size = new Size(67, 23);
            textBox_x.TabIndex = 13;
            textBox_x.Text = "0";
            // 
            // textBox_y
            // 
            textBox_y.Location = new Point(818, 196);
            textBox_y.Name = "textBox_y";
            textBox_y.Size = new Size(61, 23);
            textBox_y.TabIndex = 14;
            textBox_y.Text = "0";
            // 
            // textBox_z
            // 
            textBox_z.Location = new Point(905, 196);
            textBox_z.Name = "textBox_z";
            textBox_z.Size = new Size(61, 23);
            textBox_z.TabIndex = 15;
            textBox_z.Text = "0";
            // 
            // button_w
            // 
            button_w.Location = new Point(974, 163);
            button_w.Name = "button_w";
            button_w.Size = new Size(151, 56);
            button_w.TabIndex = 16;
            button_w.Text = "写入一组角度";
            button_w.UseVisualStyleBackColor = true;
            button_w.Click += button_w_Click;
            // 
            // button_send
            // 
            button_send.Location = new Point(640, 528);
            button_send.Name = "button_send";
            button_send.Size = new Size(145, 47);
            button_send.TabIndex = 17;
            button_send.Text = "开始发送到树莓派";
            button_send.UseVisualStyleBackColor = true;
            button_send.Click += button_send_Click;
            // 
            // button_stop
            // 
            button_stop.Location = new Point(795, 528);
            button_stop.Name = "button_stop";
            button_stop.Size = new Size(160, 47);
            button_stop.TabIndex = 18;
            button_stop.Text = "结束发送";
            button_stop.UseVisualStyleBackColor = true;
            button_stop.Click += button_stop_send_Click;
            // 
            // button_createjson
            // 
            button_createjson.Location = new Point(974, 67);
            button_createjson.Name = "button_createjson";
            button_createjson.Size = new Size(151, 24);
            button_createjson.TabIndex = 19;
            button_createjson.Text = "创建默认json文件";
            button_createjson.UseVisualStyleBackColor = true;
            button_createjson.Click += button_createjson_Click;
            // 
            // textBox_jsonpath
            // 
            textBox_jsonpath.Location = new Point(656, 37);
            textBox_jsonpath.Name = "textBox_jsonpath";
            textBox_jsonpath.Size = new Size(310, 23);
            textBox_jsonpath.TabIndex = 20;
            // 
            // button_loadjson
            // 
            button_loadjson.Location = new Point(974, 37);
            button_loadjson.Name = "button_loadjson";
            button_loadjson.Size = new Size(151, 24);
            button_loadjson.TabIndex = 21;
            button_loadjson.Text = "加载配置json文件";
            button_loadjson.UseVisualStyleBackColor = true;
            button_loadjson.Click += button_load_json_Click;
            // 
            // servo_2
            // 
            servo_2.BackColor = SystemColors.Control;
            servo_2.LargeChange = 1;
            servo_2.Location = new Point(471, 356);
            servo_2.Maximum = 180;
            servo_2.Name = "servo_2";
            servo_2.Orientation = Orientation.Vertical;
            servo_2.Size = new Size(45, 236);
            servo_2.TabIndex = 28;
            servo_2.TickStyle = TickStyle.Both;
            servo_2.Value = 90;
            // 
            // servo_0
            // 
            servo_0.BackColor = SystemColors.Control;
            servo_0.LargeChange = 1;
            servo_0.Location = new Point(147, 356);
            servo_0.Maximum = 180;
            servo_0.Name = "servo_0";
            servo_0.Orientation = Orientation.Vertical;
            servo_0.Size = new Size(45, 236);
            servo_0.TabIndex = 29;
            servo_0.TickStyle = TickStyle.Both;
            servo_0.Value = 90;
            // 
            // button_w_min
            // 
            button_w_min.Location = new Point(645, 255);
            button_w_min.Name = "button_w_min";
            button_w_min.Size = new Size(154, 47);
            button_w_min.TabIndex = 34;
            button_w_min.Text = "写入当前舵机最小角度";
            button_w_min.UseVisualStyleBackColor = true;
            button_w_min.Visible = false;
            // 
            // button_w_max
            // 
            button_w_max.Location = new Point(867, 255);
            button_w_max.Name = "button_w_max";
            button_w_max.Size = new Size(155, 47);
            button_w_max.TabIndex = 35;
            button_w_max.Text = "写入当前舵机最大角度";
            button_w_max.UseVisualStyleBackColor = true;
            button_w_max.Visible = false;
            // 
            // trackBar_lerp
            // 
            trackBar_lerp.LargeChange = 1;
            trackBar_lerp.Location = new Point(648, 433);
            trackBar_lerp.Maximum = 100;
            trackBar_lerp.Name = "trackBar_lerp";
            trackBar_lerp.Size = new Size(374, 45);
            trackBar_lerp.TabIndex = 36;
            // 
            // button_create_weight_matrix
            // 
            button_create_weight_matrix.Location = new Point(974, 528);
            button_create_weight_matrix.Name = "button_create_weight_matrix";
            button_create_weight_matrix.Size = new Size(171, 47);
            button_create_weight_matrix.TabIndex = 37;
            button_create_weight_matrix.Text = "创建矩阵json";
            button_create_weight_matrix.UseVisualStyleBackColor = true;
            button_create_weight_matrix.Click += button_create_weight_matrix_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "0" });
            comboBox1.Location = new Point(648, 402);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 25);
            comboBox1.TabIndex = 38;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "0" });
            comboBox2.Location = new Point(901, 402);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(121, 25);
            comboBox2.TabIndex = 39;
            // 
            // label_tb_name
            // 
            label_tb_name.AutoSize = true;
            label_tb_name.Location = new Point(648, 163);
            label_tb_name.Name = "label_tb_name";
            label_tb_name.Size = new Size(73, 17);
            label_tb_name.TabIndex = 40;
            label_tb_name.Text = "当前舵机：-";
            // 
            // label061
            // 
            label061.Location = new Point(155, 5);
            label061.Name = "label061";
            label061.Size = new Size(22, 23);
            label061.TabIndex = 70;
            label061.Text = "紧";
            // 
            // label060
            // 
            label060.Location = new Point(155, 255);
            label060.Name = "label060";
            label060.Size = new Size(22, 23);
            label060.TabIndex = 71;
            label060.Text = "松";
            // 
            // label051
            // 
            label051.Location = new Point(314, 3);
            label051.Name = "label051";
            label051.Size = new Size(20, 23);
            label051.TabIndex = 72;
            label051.Text = "松";
            // 
            // label050
            // 
            label050.Location = new Point(314, 249);
            label050.Name = "label050";
            label050.Size = new Size(20, 23);
            label050.TabIndex = 73;
            label050.Text = "紧";
            // 
            // label041
            // 
            label041.Location = new Point(482, 5);
            label041.Name = "label041";
            label041.Size = new Size(21, 23);
            label041.TabIndex = 74;
            label041.Text = "松";
            // 
            // label040
            // 
            label040.Location = new Point(482, 255);
            label040.Name = "label040";
            label040.Size = new Size(21, 23);
            label040.TabIndex = 75;
            label040.Text = "紧";
            // 
            // label070
            // 
            label070.Location = new Point(294, 305);
            label070.Name = "label070";
            label070.Size = new Size(21, 23);
            label070.TabIndex = 76;
            label070.Text = "紧";
            // 
            // label071
            // 
            label071.Location = new Point(38, 305);
            label071.Name = "label071";
            label071.Size = new Size(23, 23);
            label071.TabIndex = 77;
            label071.Text = "松";
            // 
            // label031
            // 
            label031.Location = new Point(339, 305);
            label031.Name = "label031";
            label031.Size = new Size(21, 23);
            label031.TabIndex = 78;
            label031.Text = "松";
            // 
            // label030
            // 
            label030.Location = new Point(579, 305);
            label030.Name = "label030";
            label030.Size = new Size(25, 23);
            label030.TabIndex = 79;
            label030.Text = "紧";
            // 
            // label011
            // 
            label011.Location = new Point(314, 341);
            label011.Name = "label011";
            label011.Size = new Size(20, 23);
            label011.TabIndex = 80;
            label011.Text = "松";
            // 
            // label010
            // 
            label010.Location = new Point(314, 595);
            label010.Name = "label010";
            label010.Size = new Size(20, 23);
            label010.TabIndex = 81;
            label010.Text = "紧";
            // 
            // label021
            // 
            label021.Location = new Point(482, 338);
            label021.Name = "label021";
            label021.Size = new Size(21, 23);
            label021.TabIndex = 82;
            label021.Text = "松";
            // 
            // label020
            // 
            label020.Location = new Point(482, 595);
            label020.Name = "label020";
            label020.Size = new Size(21, 23);
            label020.TabIndex = 83;
            label020.Text = "紧";
            // 
            // label001
            // 
            label001.Location = new Point(155, 335);
            label001.Name = "label001";
            label001.Size = new Size(22, 23);
            label001.TabIndex = 84;
            label001.Text = "紧";
            // 
            // label000
            // 
            label000.Location = new Point(155, 595);
            label000.Name = "label000";
            label000.Size = new Size(22, 23);
            label000.TabIndex = 85;
            label000.Text = "松";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(94, 462);
            label17.Name = "label17";
            label17.RightToLeft = RightToLeft.No;
            label17.Size = new Size(47, 17);
            label17.TabIndex = 86;
            label17.Text = "servo0";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(522, 462);
            label18.Name = "label18";
            label18.RightToLeft = RightToLeft.No;
            label18.Size = new Size(47, 17);
            label18.TabIndex = 87;
            label18.Text = "servo2";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(253, 482);
            label19.Name = "label19";
            label19.RightToLeft = RightToLeft.No;
            label19.Size = new Size(47, 17);
            label19.TabIndex = 88;
            label19.Text = "servo1";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(29, 341);
            label20.Name = "label20";
            label20.RightToLeft = RightToLeft.No;
            label20.Size = new Size(47, 17);
            label20.TabIndex = 89;
            label20.Text = "servo7";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(557, 341);
            label21.Name = "label21";
            label21.RightToLeft = RightToLeft.No;
            label21.Size = new Size(47, 17);
            label21.TabIndex = 90;
            label21.Text = "servo3";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(94, 135);
            label22.Name = "label22";
            label22.RightToLeft = RightToLeft.No;
            label22.Size = new Size(47, 17);
            label22.TabIndex = 91;
            label22.Text = "servo6";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(418, 135);
            label23.Name = "label23";
            label23.RightToLeft = RightToLeft.No;
            label23.Size = new Size(47, 17);
            label23.TabIndex = 92;
            label23.Text = "servo4";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new Point(253, 131);
            label24.Name = "label24";
            label24.RightToLeft = RightToLeft.No;
            label24.Size = new Size(47, 17);
            label24.TabIndex = 93;
            label24.Text = "servo5";
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new Point(645, 222);
            label25.Name = "label25";
            label25.Size = new Size(355, 17);
            label25.TabIndex = 94;
            label25.Text = "x(低头)    -x(仰头)    y(左看)    -y(右看)    z(右歪头)    -z(左歪头)";
            // 
            // btn_add
            // 
            btn_add.Location = new Point(818, 163);
            btn_add.Name = "btn_add";
            btn_add.Size = new Size(62, 23);
            btn_add.TabIndex = 95;
            btn_add.Text = "角度+1";
            btn_add.UseVisualStyleBackColor = true;
            btn_add.Click += btn_add_Click;
            // 
            // btn_sub
            // 
            btn_sub.Location = new Point(904, 163);
            btn_sub.Name = "btn_sub";
            btn_sub.Size = new Size(62, 23);
            btn_sub.TabIndex = 96;
            btn_sub.Text = "角度-1";
            btn_sub.UseVisualStyleBackColor = true;
            btn_sub.Click += btn_sub_Click;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new Point(648, 372);
            label26.Name = "label26";
            label26.Size = new Size(212, 17);
            label26.TabIndex = 97;
            label26.Text = "可以选择两个角度，然后拉动滑条过渡";
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Location = new Point(707, 199);
            label27.Name = "label27";
            label27.Size = new Size(14, 17);
            label27.TabIndex = 98;
            label27.Text = "x";
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Location = new Point(798, 199);
            label28.Name = "label28";
            label28.Size = new Size(14, 17);
            label28.TabIndex = 99;
            label28.Text = "y";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(885, 199);
            label29.Name = "label29";
            label29.Size = new Size(14, 17);
            label29.TabIndex = 100;
            label29.Text = "z";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(640, 496);
            label1.Name = "label1";
            label1.Size = new Size(19, 17);
            label1.TabIndex = 101;
            label1.Text = "IP";
            // 
            // textBox_ip
            // 
            textBox_ip.Location = new Point(669, 496);
            textBox_ip.Name = "textBox_ip";
            textBox_ip.Size = new Size(286, 23);
            textBox_ip.TabIndex = 102;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(974, 499);
            label2.Name = "label2";
            label2.Size = new Size(32, 17);
            label2.TabIndex = 103;
            label2.Text = "端口";
            // 
            // textBox_port
            // 
            textBox_port.Location = new Point(1012, 496);
            textBox_port.Name = "textBox_port";
            textBox_port.Size = new Size(133, 23);
            textBox_port.TabIndex = 104;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1157, 658);
            Controls.Add(textBox_port);
            Controls.Add(label2);
            Controls.Add(textBox_ip);
            Controls.Add(label1);
            Controls.Add(label29);
            Controls.Add(label28);
            Controls.Add(label27);
            Controls.Add(label26);
            Controls.Add(btn_sub);
            Controls.Add(btn_add);
            Controls.Add(label25);
            Controls.Add(label24);
            Controls.Add(label23);
            Controls.Add(label22);
            Controls.Add(label21);
            Controls.Add(label20);
            Controls.Add(label19);
            Controls.Add(label18);
            Controls.Add(label17);
            Controls.Add(label000);
            Controls.Add(label001);
            Controls.Add(label020);
            Controls.Add(label021);
            Controls.Add(label010);
            Controls.Add(label011);
            Controls.Add(label030);
            Controls.Add(label031);
            Controls.Add(label071);
            Controls.Add(label070);
            Controls.Add(label040);
            Controls.Add(label041);
            Controls.Add(label050);
            Controls.Add(label051);
            Controls.Add(label060);
            Controls.Add(label061);
            Controls.Add(label_tb_name);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(button_create_weight_matrix);
            Controls.Add(trackBar_lerp);
            Controls.Add(button_w_max);
            Controls.Add(button_w_min);
            Controls.Add(servo_0);
            Controls.Add(servo_2);
            Controls.Add(button_loadjson);
            Controls.Add(textBox_jsonpath);
            Controls.Add(button_createjson);
            Controls.Add(button_stop);
            Controls.Add(button_send);
            Controls.Add(button_w);
            Controls.Add(textBox_z);
            Controls.Add(textBox_y);
            Controls.Add(textBox_x);
            Controls.Add(label_nack_angle);
            Controls.Add(servo_6);
            Controls.Add(servo_4);
            Controls.Add(servo_1);
            Controls.Add(servo_5);
            Controls.Add(servo_3);
            Controls.Add(servo_7);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)servo_7).EndInit();
            ((System.ComponentModel.ISupportInitialize)servo_3).EndInit();
            ((System.ComponentModel.ISupportInitialize)servo_5).EndInit();
            ((System.ComponentModel.ISupportInitialize)servo_1).EndInit();
            ((System.ComponentModel.ISupportInitialize)servo_4).EndInit();
            ((System.ComponentModel.ISupportInitialize)servo_6).EndInit();
            ((System.ComponentModel.ISupportInitialize)servo_2).EndInit();
            ((System.ComponentModel.ISupportInitialize)servo_0).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar_lerp).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TrackBar servo_7;
        private TrackBar servo_3;
        private TrackBar servo_5;
        private TrackBar servo_1;
        private TrackBar servo_4;
        private TrackBar servo_6;
        private Label label_nack_angle;
        private TextBox textBox_x;
        private TextBox textBox_y;
        private TextBox textBox_z;
        private Button button_w;
        private Button button_send;
        private Button button_stop;
        private Button button_createjson;
        private TextBox textBox_jsonpath;
        private Button button_loadjson;
        private TrackBar servo_2;
        private TrackBar servo_0;
        private Button button_w_min;
        private Button button_w_max;
        private TrackBar trackBar_lerp;
        private Button button_create_weight_matrix;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private Label label_tb_name;
        private TextBox label061;
        private TextBox label060;
        private TextBox label051;
        private TextBox label050;
        private TextBox label041;
        private TextBox label040;
        private TextBox label070;
        private TextBox label071;
        private TextBox label031;
        private TextBox label030;
        private TextBox label011;
        private TextBox label010;
        private TextBox label021;
        private TextBox label020;
        private TextBox label001;
        private TextBox label000;
        private Label label17;
        private Label label18;
        private Label label19;
        private Label label20;
        private Label label21;
        private Label label22;
        private Label label23;
        private Label label24;
        private Label label25;
        private Button btn_add;
        private Button btn_sub;
        private Label label26;
        private Label label27;
        private Label label28;
        private Label label29;
        private Label label1;
        private TextBox textBox_ip;
        private Label label2;
        private TextBox textBox_port;
    }
}
