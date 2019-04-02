namespace RaspiForm
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.sliderMotor = new System.Windows.Forms.TrackBar();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.radioStepperReverse = new System.Windows.Forms.RadioButton();
			this.radioStepperForward = new System.Windows.Forms.RadioButton();
			this.label12 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.listGpioStepperB1 = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.listGpioStepperB2 = new System.Windows.Forms.ComboBox();
			this.listGpioStepperA1 = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.listGpioStepperA2 = new System.Windows.Forms.ComboBox();
			this.sliderServo = new System.Windows.Forms.TrackBar();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textServoSpeed = new System.Windows.Forms.Label();
			this.listGpioServo = new System.Windows.Forms.ComboBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.textRange = new System.Windows.Forms.Label();
			this.listGpioRangeFinderOuput = new System.Windows.Forms.ComboBox();
			this.listGpioRangeFinderInput = new System.Windows.Forms.ComboBox();
			this.label16 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.listGpioTracksRightA1 = new System.Windows.Forms.ComboBox();
			this.listGpioTracksLeftA1 = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.listGpioTracksLeftA2 = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.listGpioTracksRightA2 = new System.Windows.Forms.ComboBox();
			this.listGpioTracksRightEna = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.listGpioTracksLeftEna = new System.Windows.Forms.ComboBox();
			this.trackTrackLeftSpeed = new System.Windows.Forms.TrackBar();
			this.trackTrackRightSpeed = new System.Windows.Forms.TrackBar();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.btnCalibrateEsc = new System.Windows.Forms.Button();
			this.label19 = new System.Windows.Forms.Label();
			this.listGpioEscControlPin = new System.Windows.Forms.ComboBox();
			this.label18 = new System.Windows.Forms.Label();
			this.trackEscSpeed = new System.Windows.Forms.TrackBar();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageLidar = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.groupLidarCommands = new System.Windows.Forms.GroupBox();
			this.btnReset = new System.Windows.Forms.Button();
			this.btnTestLidar = new System.Windows.Forms.Button();
			this.btnStopScan = new System.Windows.Forms.Button();
			this.btnStartScan = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.comboComPorts = new System.Windows.Forms.ComboBox();
			this.btnStopLidar = new System.Windows.Forms.Button();
			this.btnStartLidar = new System.Windows.Forms.Button();
			this.panelBitmap = new System.Windows.Forms.Panel();
			this.picLidar = new System.Windows.Forms.PictureBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.textLidarStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.tabPageControl = new System.Windows.Forms.TabPage();
			this.btnBitmap = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.sliderMotor)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sliderServo)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackTrackLeftSpeed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackTrackRightSpeed)).BeginInit();
			this.groupBox5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackEscSpeed)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPageLidar.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupLidarCommands.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picLidar)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.tabPageControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// sliderMotor
			// 
			this.sliderMotor.Location = new System.Drawing.Point(328, 30);
			this.sliderMotor.Maximum = 50;
			this.sliderMotor.Minimum = 1;
			this.sliderMotor.Name = "sliderMotor";
			this.sliderMotor.Size = new System.Drawing.Size(251, 45);
			this.sliderMotor.TabIndex = 1;
			this.sliderMotor.Value = 1;
			this.sliderMotor.Scroll += new System.EventHandler(this.OnMotorSliderScroll);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnStop);
			this.groupBox1.Controls.Add(this.btnStart);
			this.groupBox1.Controls.Add(this.radioStepperReverse);
			this.groupBox1.Controls.Add(this.radioStepperForward);
			this.groupBox1.Controls.Add(this.label12);
			this.groupBox1.Controls.Add(this.sliderMotor);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.listGpioStepperB1);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.listGpioStepperB2);
			this.groupBox1.Controls.Add(this.listGpioStepperA1);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.listGpioStepperA2);
			this.groupBox1.Location = new System.Drawing.Point(18, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(680, 91);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Stepper Motor";
			// 
			// btnStop
			// 
			this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStop.Location = new System.Drawing.Point(599, 53);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 32);
			this.btnStop.TabIndex = 0;
			this.btnStop.Text = "S&top";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.OnStopClicked);
			// 
			// btnStart
			// 
			this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStart.Location = new System.Drawing.Point(599, 15);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 32);
			this.btnStart.TabIndex = 0;
			this.btnStart.Text = "&Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.OnStartClicked);
			// 
			// radioStepperReverse
			// 
			this.radioStepperReverse.AutoSize = true;
			this.radioStepperReverse.Location = new System.Drawing.Point(248, 55);
			this.radioStepperReverse.Name = "radioStepperReverse";
			this.radioStepperReverse.Size = new System.Drawing.Size(72, 21);
			this.radioStepperReverse.TabIndex = 0;
			this.radioStepperReverse.TabStop = true;
			this.radioStepperReverse.Text = "Reverse";
			this.radioStepperReverse.UseVisualStyleBackColor = true;
			this.radioStepperReverse.CheckedChanged += new System.EventHandler(this.OnRadioClicked);
			// 
			// radioStepperForward
			// 
			this.radioStepperForward.AutoSize = true;
			this.radioStepperForward.Location = new System.Drawing.Point(248, 28);
			this.radioStepperForward.Name = "radioStepperForward";
			this.radioStepperForward.Size = new System.Drawing.Size(74, 21);
			this.radioStepperForward.TabIndex = 0;
			this.radioStepperForward.TabStop = true;
			this.radioStepperForward.Text = "Forward";
			this.radioStepperForward.UseVisualStyleBackColor = true;
			this.radioStepperForward.CheckedChanged += new System.EventHandler(this.OnRadioClicked);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(126, 61);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(22, 17);
			this.label12.TabIndex = 3;
			this.label12.Text = "B2";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(15, 61);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(23, 17);
			this.label6.TabIndex = 3;
			this.label6.Text = "A2";
			// 
			// listGpioStepperB1
			// 
			this.listGpioStepperB1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioStepperB1.FormattingEnabled = true;
			this.listGpioStepperB1.Location = new System.Drawing.Point(168, 27);
			this.listGpioStepperB1.Name = "listGpioStepperB1";
			this.listGpioStepperB1.Size = new System.Drawing.Size(55, 25);
			this.listGpioStepperB1.TabIndex = 2;
			this.listGpioStepperB1.SelectedIndexChanged += new System.EventHandler(this.OnGpioStepperB1_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(15, 30);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(23, 17);
			this.label4.TabIndex = 3;
			this.label4.Text = "A1";
			// 
			// listGpioStepperB2
			// 
			this.listGpioStepperB2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioStepperB2.FormattingEnabled = true;
			this.listGpioStepperB2.Location = new System.Drawing.Point(168, 58);
			this.listGpioStepperB2.Name = "listGpioStepperB2";
			this.listGpioStepperB2.Size = new System.Drawing.Size(55, 25);
			this.listGpioStepperB2.TabIndex = 2;
			this.listGpioStepperB2.SelectedIndexChanged += new System.EventHandler(this.OnGpioStepperB2_SelectedIndexChanged);
			// 
			// listGpioStepperA1
			// 
			this.listGpioStepperA1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioStepperA1.FormattingEnabled = true;
			this.listGpioStepperA1.Location = new System.Drawing.Point(57, 27);
			this.listGpioStepperA1.Name = "listGpioStepperA1";
			this.listGpioStepperA1.Size = new System.Drawing.Size(55, 25);
			this.listGpioStepperA1.TabIndex = 2;
			this.listGpioStepperA1.SelectedIndexChanged += new System.EventHandler(this.OnGpioStepperA1_SelectedIndexChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(126, 30);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(22, 17);
			this.label5.TabIndex = 3;
			this.label5.Text = "B1";
			// 
			// listGpioStepperA2
			// 
			this.listGpioStepperA2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioStepperA2.FormattingEnabled = true;
			this.listGpioStepperA2.Location = new System.Drawing.Point(57, 58);
			this.listGpioStepperA2.Name = "listGpioStepperA2";
			this.listGpioStepperA2.Size = new System.Drawing.Size(55, 25);
			this.listGpioStepperA2.TabIndex = 2;
			this.listGpioStepperA2.SelectedIndexChanged += new System.EventHandler(this.OnGpioStepperA2_SelectedIndexChanged);
			// 
			// sliderServo
			// 
			this.sliderServo.Location = new System.Drawing.Point(191, 24);
			this.sliderServo.Maximum = 50;
			this.sliderServo.Minimum = 1;
			this.sliderServo.Name = "sliderServo";
			this.sliderServo.Size = new System.Drawing.Size(258, 45);
			this.sliderServo.TabIndex = 1;
			this.sliderServo.Value = 1;
			this.sliderServo.Scroll += new System.EventHandler(this.OnServoSliderScroll);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textServoSpeed);
			this.groupBox2.Controls.Add(this.sliderServo);
			this.groupBox2.Controls.Add(this.listGpioServo);
			this.groupBox2.Location = new System.Drawing.Point(18, 103);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(473, 81);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Servo";
			// 
			// textServoSpeed
			// 
			this.textServoSpeed.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textServoSpeed.Location = new System.Drawing.Point(113, 21);
			this.textServoSpeed.Name = "textServoSpeed";
			this.textServoSpeed.Size = new System.Drawing.Size(72, 44);
			this.textServoSpeed.TabIndex = 2;
			this.textServoSpeed.Text = "0";
			this.textServoSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listGpioServo
			// 
			this.listGpioServo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioServo.FormattingEnabled = true;
			this.listGpioServo.Location = new System.Drawing.Point(18, 32);
			this.listGpioServo.Name = "listGpioServo";
			this.listGpioServo.Size = new System.Drawing.Size(55, 25);
			this.listGpioServo.TabIndex = 2;
			this.listGpioServo.SelectedIndexChanged += new System.EventHandler(this.OnGpioServo_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.textRange);
			this.groupBox3.Controls.Add(this.listGpioRangeFinderOuput);
			this.groupBox3.Controls.Add(this.listGpioRangeFinderInput);
			this.groupBox3.Controls.Add(this.label16);
			this.groupBox3.Controls.Add(this.label15);
			this.groupBox3.Location = new System.Drawing.Point(18, 190);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(304, 81);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Range Finder";
			// 
			// textRange
			// 
			this.textRange.Dock = System.Windows.Forms.DockStyle.Right;
			this.textRange.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textRange.Location = new System.Drawing.Point(241, 21);
			this.textRange.Name = "textRange";
			this.textRange.Size = new System.Drawing.Size(60, 57);
			this.textRange.TabIndex = 2;
			this.textRange.Text = "0";
			this.textRange.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listGpioRangeFinderOuput
			// 
			this.listGpioRangeFinderOuput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioRangeFinderOuput.FormattingEnabled = true;
			this.listGpioRangeFinderOuput.Location = new System.Drawing.Point(180, 38);
			this.listGpioRangeFinderOuput.Name = "listGpioRangeFinderOuput";
			this.listGpioRangeFinderOuput.Size = new System.Drawing.Size(55, 25);
			this.listGpioRangeFinderOuput.TabIndex = 2;
			this.listGpioRangeFinderOuput.SelectedIndexChanged += new System.EventHandler(this.OnGpioRangeFinderOutput_SelectedIndexChanged);
			// 
			// listGpioRangeFinderInput
			// 
			this.listGpioRangeFinderInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioRangeFinderInput.FormattingEnabled = true;
			this.listGpioRangeFinderInput.Location = new System.Drawing.Point(57, 38);
			this.listGpioRangeFinderInput.Name = "listGpioRangeFinderInput";
			this.listGpioRangeFinderInput.Size = new System.Drawing.Size(55, 25);
			this.listGpioRangeFinderInput.TabIndex = 2;
			this.listGpioRangeFinderInput.SelectedIndexChanged += new System.EventHandler(this.OnGpioRangeFinderInput_SelectedIndexChanged);
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(13, 41);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(38, 17);
			this.label16.TabIndex = 3;
			this.label16.Text = "Pulse";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(142, 41);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(30, 17);
			this.label15.TabIndex = 3;
			this.label15.Text = "Trig";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.label14);
			this.groupBox4.Controls.Add(this.label13);
			this.groupBox4.Controls.Add(this.label11);
			this.groupBox4.Controls.Add(this.label3);
			this.groupBox4.Controls.Add(this.label10);
			this.groupBox4.Controls.Add(this.label2);
			this.groupBox4.Controls.Add(this.label9);
			this.groupBox4.Controls.Add(this.listGpioTracksRightA1);
			this.groupBox4.Controls.Add(this.listGpioTracksLeftA1);
			this.groupBox4.Controls.Add(this.label7);
			this.groupBox4.Controls.Add(this.listGpioTracksLeftA2);
			this.groupBox4.Controls.Add(this.label8);
			this.groupBox4.Controls.Add(this.listGpioTracksRightA2);
			this.groupBox4.Controls.Add(this.listGpioTracksRightEna);
			this.groupBox4.Controls.Add(this.label1);
			this.groupBox4.Controls.Add(this.listGpioTracksLeftEna);
			this.groupBox4.Controls.Add(this.trackTrackLeftSpeed);
			this.groupBox4.Controls.Add(this.trackTrackRightSpeed);
			this.groupBox4.Location = new System.Drawing.Point(18, 277);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(680, 159);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Robot Tracks";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(278, 96);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(79, 17);
			this.label14.TabIndex = 4;
			this.label14.Text = "Right Speed";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(278, 42);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(70, 17);
			this.label13.TabIndex = 4;
			this.label13.Text = "Left Speed";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(149, 122);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(33, 17);
			this.label11.TabIndex = 3;
			this.label11.Text = "ENA";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(15, 122);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(33, 17);
			this.label3.TabIndex = 3;
			this.label3.Text = "ENA";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(149, 91);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(23, 17);
			this.label10.TabIndex = 3;
			this.label10.Text = "A2";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 91);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(23, 17);
			this.label2.TabIndex = 3;
			this.label2.Text = "A2";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(149, 25);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(38, 17);
			this.label9.TabIndex = 3;
			this.label9.Text = "Right";
			// 
			// listGpioTracksRightA1
			// 
			this.listGpioTracksRightA1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioTracksRightA1.FormattingEnabled = true;
			this.listGpioTracksRightA1.Location = new System.Drawing.Point(191, 57);
			this.listGpioTracksRightA1.Name = "listGpioTracksRightA1";
			this.listGpioTracksRightA1.Size = new System.Drawing.Size(55, 25);
			this.listGpioTracksRightA1.TabIndex = 2;
			this.listGpioTracksRightA1.SelectedIndexChanged += new System.EventHandler(this.OnGpioTracksRightA1_SelectedIndexChanged);
			// 
			// listGpioTracksLeftA1
			// 
			this.listGpioTracksLeftA1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioTracksLeftA1.FormattingEnabled = true;
			this.listGpioTracksLeftA1.Location = new System.Drawing.Point(57, 57);
			this.listGpioTracksLeftA1.Name = "listGpioTracksLeftA1";
			this.listGpioTracksLeftA1.Size = new System.Drawing.Size(55, 25);
			this.listGpioTracksLeftA1.TabIndex = 2;
			this.listGpioTracksLeftA1.SelectedIndexChanged += new System.EventHandler(this.OnGpioTracksLeftA1_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(15, 25);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(29, 17);
			this.label7.TabIndex = 3;
			this.label7.Text = "Left";
			// 
			// listGpioTracksLeftA2
			// 
			this.listGpioTracksLeftA2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioTracksLeftA2.FormattingEnabled = true;
			this.listGpioTracksLeftA2.Location = new System.Drawing.Point(57, 88);
			this.listGpioTracksLeftA2.Name = "listGpioTracksLeftA2";
			this.listGpioTracksLeftA2.Size = new System.Drawing.Size(55, 25);
			this.listGpioTracksLeftA2.TabIndex = 2;
			this.listGpioTracksLeftA2.SelectedIndexChanged += new System.EventHandler(this.OnGpioTracksLeftA2_SelectedIndexChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(149, 60);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(23, 17);
			this.label8.TabIndex = 3;
			this.label8.Text = "A1";
			// 
			// listGpioTracksRightA2
			// 
			this.listGpioTracksRightA2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioTracksRightA2.FormattingEnabled = true;
			this.listGpioTracksRightA2.Location = new System.Drawing.Point(191, 88);
			this.listGpioTracksRightA2.Name = "listGpioTracksRightA2";
			this.listGpioTracksRightA2.Size = new System.Drawing.Size(55, 25);
			this.listGpioTracksRightA2.TabIndex = 2;
			this.listGpioTracksRightA2.SelectedIndexChanged += new System.EventHandler(this.OnGpioTracksRightA2_SelectedIndexChanged);
			// 
			// listGpioTracksRightEna
			// 
			this.listGpioTracksRightEna.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioTracksRightEna.FormattingEnabled = true;
			this.listGpioTracksRightEna.Location = new System.Drawing.Point(191, 119);
			this.listGpioTracksRightEna.Name = "listGpioTracksRightEna";
			this.listGpioTracksRightEna.Size = new System.Drawing.Size(55, 25);
			this.listGpioTracksRightEna.TabIndex = 2;
			this.listGpioTracksRightEna.SelectedIndexChanged += new System.EventHandler(this.OnGpioTracksRightEna_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 60);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(23, 17);
			this.label1.TabIndex = 3;
			this.label1.Text = "A1";
			// 
			// listGpioTracksLeftEna
			// 
			this.listGpioTracksLeftEna.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioTracksLeftEna.FormattingEnabled = true;
			this.listGpioTracksLeftEna.Location = new System.Drawing.Point(57, 119);
			this.listGpioTracksLeftEna.Name = "listGpioTracksLeftEna";
			this.listGpioTracksLeftEna.Size = new System.Drawing.Size(55, 25);
			this.listGpioTracksLeftEna.TabIndex = 2;
			this.listGpioTracksLeftEna.SelectedIndexChanged += new System.EventHandler(this.OnGpioTracksLeftEna_SelectedIndexChanged);
			// 
			// trackTrackLeftSpeed
			// 
			this.trackTrackLeftSpeed.Location = new System.Drawing.Point(368, 32);
			this.trackTrackLeftSpeed.Maximum = 100;
			this.trackTrackLeftSpeed.Minimum = -100;
			this.trackTrackLeftSpeed.Name = "trackTrackLeftSpeed";
			this.trackTrackLeftSpeed.Size = new System.Drawing.Size(284, 45);
			this.trackTrackLeftSpeed.TabIndex = 1;
			this.trackTrackLeftSpeed.Value = 1;
			this.trackTrackLeftSpeed.Scroll += new System.EventHandler(this.OnLeftTrackSpeedSliderChanged);
			// 
			// trackTrackRightSpeed
			// 
			this.trackTrackRightSpeed.Location = new System.Drawing.Point(368, 88);
			this.trackTrackRightSpeed.Maximum = 100;
			this.trackTrackRightSpeed.Minimum = -100;
			this.trackTrackRightSpeed.Name = "trackTrackRightSpeed";
			this.trackTrackRightSpeed.Size = new System.Drawing.Size(284, 45);
			this.trackTrackRightSpeed.TabIndex = 1;
			this.trackTrackRightSpeed.Value = 1;
			this.trackTrackRightSpeed.Scroll += new System.EventHandler(this.OnRightTrackSpeedSliderChanged);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.checkBox1);
			this.groupBox5.Controls.Add(this.btnCalibrateEsc);
			this.groupBox5.Controls.Add(this.label19);
			this.groupBox5.Controls.Add(this.listGpioEscControlPin);
			this.groupBox5.Controls.Add(this.label18);
			this.groupBox5.Controls.Add(this.trackEscSpeed);
			this.groupBox5.Location = new System.Drawing.Point(18, 442);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(680, 81);
			this.groupBox5.TabIndex = 4;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "ESC";
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(245, 38);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(44, 21);
			this.checkBox1.TabIndex = 5;
			this.checkBox1.Text = "On";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// btnCalibrateEsc
			// 
			this.btnCalibrateEsc.Location = new System.Drawing.Point(145, 34);
			this.btnCalibrateEsc.Name = "btnCalibrateEsc";
			this.btnCalibrateEsc.Size = new System.Drawing.Size(75, 31);
			this.btnCalibrateEsc.TabIndex = 4;
			this.btnCalibrateEsc.Text = "Calibrate";
			this.btnCalibrateEsc.UseVisualStyleBackColor = true;
			this.btnCalibrateEsc.Click += new System.EventHandler(this.OnCalibrateEscClicked);
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(334, 38);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(45, 17);
			this.label19.TabIndex = 4;
			this.label19.Text = "Speed";
			// 
			// listGpioEscControlPin
			// 
			this.listGpioEscControlPin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listGpioEscControlPin.FormattingEnabled = true;
			this.listGpioEscControlPin.Location = new System.Drawing.Point(57, 38);
			this.listGpioEscControlPin.Name = "listGpioEscControlPin";
			this.listGpioEscControlPin.Size = new System.Drawing.Size(55, 25);
			this.listGpioEscControlPin.TabIndex = 2;
			this.listGpioEscControlPin.SelectedIndexChanged += new System.EventHandler(this.OnGpioEscControl_SelectedIndexChanged);
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(13, 41);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(39, 17);
			this.label18.TabIndex = 3;
			this.label18.Text = "PWM";
			// 
			// trackEscSpeed
			// 
			this.trackEscSpeed.Location = new System.Drawing.Point(404, 24);
			this.trackEscSpeed.Maximum = 2000;
			this.trackEscSpeed.Minimum = 500;
			this.trackEscSpeed.Name = "trackEscSpeed";
			this.trackEscSpeed.Size = new System.Drawing.Size(221, 45);
			this.trackEscSpeed.TabIndex = 1;
			this.trackEscSpeed.Value = 500;
			this.trackEscSpeed.Scroll += new System.EventHandler(this.OnEscSpeedSliderChanged);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageLidar);
			this.tabControl1.Controls.Add(this.tabPageControl);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(720, 566);
			this.tabControl1.TabIndex = 5;
			// 
			// tabPageLidar
			// 
			this.tabPageLidar.Controls.Add(this.splitContainer1);
			this.tabPageLidar.Controls.Add(this.statusStrip1);
			this.tabPageLidar.Location = new System.Drawing.Point(4, 26);
			this.tabPageLidar.Name = "tabPageLidar";
			this.tabPageLidar.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageLidar.Size = new System.Drawing.Size(712, 536);
			this.tabPageLidar.TabIndex = 1;
			this.tabPageLidar.Text = "Lidar";
			this.tabPageLidar.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.groupLidarCommands);
			this.splitContainer1.Panel1.Controls.Add(this.button1);
			this.splitContainer1.Panel1.Controls.Add(this.comboComPorts);
			this.splitContainer1.Panel1.Controls.Add(this.btnStopLidar);
			this.splitContainer1.Panel1.Controls.Add(this.btnStartLidar);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.panelBitmap);
			this.splitContainer1.Panel2.Controls.Add(this.picLidar);
			this.splitContainer1.Size = new System.Drawing.Size(706, 508);
			this.splitContainer1.SplitterDistance = 93;
			this.splitContainer1.TabIndex = 0;
			// 
			// groupLidarCommands
			// 
			this.groupLidarCommands.Controls.Add(this.btnBitmap);
			this.groupLidarCommands.Controls.Add(this.btnReset);
			this.groupLidarCommands.Controls.Add(this.btnTestLidar);
			this.groupLidarCommands.Controls.Add(this.btnStopScan);
			this.groupLidarCommands.Controls.Add(this.btnStartScan);
			this.groupLidarCommands.Location = new System.Drawing.Point(204, 14);
			this.groupLidarCommands.Name = "groupLidarCommands";
			this.groupLidarCommands.Size = new System.Drawing.Size(270, 60);
			this.groupLidarCommands.TabIndex = 3;
			this.groupLidarCommands.TabStop = false;
			this.groupLidarCommands.Text = "Commands";
			// 
			// btnReset
			// 
			this.btnReset.Location = new System.Drawing.Point(169, 25);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(43, 23);
			this.btnReset.TabIndex = 0;
			this.btnReset.Text = "Reset";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.OnResetLidarClicked);
			// 
			// btnTestLidar
			// 
			this.btnTestLidar.Location = new System.Drawing.Point(120, 25);
			this.btnTestLidar.Name = "btnTestLidar";
			this.btnTestLidar.Size = new System.Drawing.Size(43, 23);
			this.btnTestLidar.TabIndex = 0;
			this.btnTestLidar.Text = "Test";
			this.btnTestLidar.UseVisualStyleBackColor = true;
			this.btnTestLidar.Click += new System.EventHandler(this.OnTestLidarClicked);
			// 
			// btnStopScan
			// 
			this.btnStopScan.Location = new System.Drawing.Point(71, 25);
			this.btnStopScan.Name = "btnStopScan";
			this.btnStopScan.Size = new System.Drawing.Size(43, 23);
			this.btnStopScan.TabIndex = 0;
			this.btnStopScan.Text = "Stop";
			this.btnStopScan.UseVisualStyleBackColor = true;
			this.btnStopScan.Click += new System.EventHandler(this.OnStopScanClicked);
			// 
			// btnStartScan
			// 
			this.btnStartScan.Location = new System.Drawing.Point(22, 25);
			this.btnStartScan.Name = "btnStartScan";
			this.btnStartScan.Size = new System.Drawing.Size(43, 23);
			this.btnStartScan.TabIndex = 0;
			this.btnStartScan.Text = "Scan";
			this.btnStartScan.UseVisualStyleBackColor = true;
			this.btnStartScan.Click += new System.EventHandler(this.OnStartScanLicked);
			// 
			// button1
			// 
			this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
			this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.button1.Location = new System.Drawing.Point(145, 14);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(37, 34);
			this.button1.TabIndex = 2;
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.OnRefreshComportsClicked);
			// 
			// comboComPorts
			// 
			this.comboComPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboComPorts.FormattingEnabled = true;
			this.comboComPorts.Location = new System.Drawing.Point(18, 20);
			this.comboComPorts.Name = "comboComPorts";
			this.comboComPorts.Size = new System.Drawing.Size(121, 25);
			this.comboComPorts.TabIndex = 1;
			this.comboComPorts.SelectedIndexChanged += new System.EventHandler(this.OnSelectedLidarComportChanged);
			// 
			// btnStopLidar
			// 
			this.btnStopLidar.Location = new System.Drawing.Point(594, 11);
			this.btnStopLidar.Name = "btnStopLidar";
			this.btnStopLidar.Size = new System.Drawing.Size(95, 40);
			this.btnStopLidar.TabIndex = 0;
			this.btnStopLidar.Text = "Stop &Lidar";
			this.btnStopLidar.UseVisualStyleBackColor = true;
			this.btnStopLidar.Click += new System.EventHandler(this.OnStopLidarClicked);
			// 
			// btnStartLidar
			// 
			this.btnStartLidar.Location = new System.Drawing.Point(480, 11);
			this.btnStartLidar.Name = "btnStartLidar";
			this.btnStartLidar.Size = new System.Drawing.Size(93, 40);
			this.btnStartLidar.TabIndex = 0;
			this.btnStartLidar.Text = "&Start Lidar";
			this.btnStartLidar.UseVisualStyleBackColor = true;
			this.btnStartLidar.Click += new System.EventHandler(this.OnStartLidarClicked);
			// 
			// panelBitmap
			// 
			this.panelBitmap.Location = new System.Drawing.Point(93, 70);
			this.panelBitmap.Name = "panelBitmap";
			this.panelBitmap.Size = new System.Drawing.Size(405, 325);
			this.panelBitmap.TabIndex = 1;
			this.panelBitmap.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPanelPaint);
			// 
			// picLidar
			// 
			this.picLidar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picLidar.Location = new System.Drawing.Point(0, 0);
			this.picLidar.Name = "picLidar";
			this.picLidar.Size = new System.Drawing.Size(706, 411);
			this.picLidar.TabIndex = 0;
			this.picLidar.TabStop = false;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textLidarStatus});
			this.statusStrip1.Location = new System.Drawing.Point(3, 511);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(706, 22);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// textLidarStatus
			// 
			this.textLidarStatus.Name = "textLidarStatus";
			this.textLidarStatus.Size = new System.Drawing.Size(68, 17);
			this.textLidarStatus.Text = "Lidar Status";
			// 
			// tabPageControl
			// 
			this.tabPageControl.Controls.Add(this.groupBox1);
			this.tabPageControl.Controls.Add(this.groupBox4);
			this.tabPageControl.Controls.Add(this.groupBox2);
			this.tabPageControl.Controls.Add(this.groupBox5);
			this.tabPageControl.Controls.Add(this.groupBox3);
			this.tabPageControl.Location = new System.Drawing.Point(4, 26);
			this.tabPageControl.Name = "tabPageControl";
			this.tabPageControl.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageControl.Size = new System.Drawing.Size(712, 536);
			this.tabPageControl.TabIndex = 0;
			this.tabPageControl.Text = "Control";
			this.tabPageControl.UseVisualStyleBackColor = true;
			// 
			// btnBitmap
			// 
			this.btnBitmap.Location = new System.Drawing.Point(218, 24);
			this.btnBitmap.Name = "btnBitmap";
			this.btnBitmap.Size = new System.Drawing.Size(43, 23);
			this.btnBitmap.TabIndex = 0;
			this.btnBitmap.Text = "BM";
			this.btnBitmap.UseVisualStyleBackColor = true;
			this.btnBitmap.Click += new System.EventHandler(this.OnLidarBitmapPressed);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(720, 566);
			this.Controls.Add(this.tabControl1);
			this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MainForm";
			this.Text = "Control";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this.Load += new System.EventHandler(this.OnFormLoad);
			((System.ComponentModel.ISupportInitialize)(this.sliderMotor)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sliderServo)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackTrackLeftSpeed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackTrackRightSpeed)).EndInit();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackEscSpeed)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPageLidar.ResumeLayout(false);
			this.tabPageLidar.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupLidarCommands.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picLidar)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.tabPageControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TrackBar sliderMotor;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioStepperReverse;
		private System.Windows.Forms.RadioButton radioStepperForward;
		private System.Windows.Forms.TrackBar sliderServo;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label textServoSpeed;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label textRange;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox listGpioStepperB1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox listGpioStepperB2;
		private System.Windows.Forms.ComboBox listGpioStepperA1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox listGpioStepperA2;
		private System.Windows.Forms.ComboBox listGpioServo;
		private System.Windows.Forms.ComboBox listGpioRangeFinderInput;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox listGpioTracksRightA1;
		private System.Windows.Forms.ComboBox listGpioTracksLeftA1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox listGpioTracksLeftA2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox listGpioTracksRightA2;
		private System.Windows.Forms.ComboBox listGpioTracksRightEna;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox listGpioTracksLeftEna;
		private System.Windows.Forms.TrackBar trackTrackRightSpeed;
		private System.Windows.Forms.TrackBar trackTrackLeftSpeed;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.ComboBox listGpioRangeFinderOuput;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button btnCalibrateEsc;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.ComboBox listGpioEscControlPin;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TrackBar trackEscSpeed;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageLidar;
		private System.Windows.Forms.TabPage tabPageControl;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button btnStopLidar;
		private System.Windows.Forms.Button btnStartLidar;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ComboBox comboComPorts;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel textLidarStatus;
		private System.Windows.Forms.GroupBox groupLidarCommands;
		private System.Windows.Forms.Button btnStopScan;
		private System.Windows.Forms.Button btnStartScan;
		private System.Windows.Forms.Button btnTestLidar;
		private System.Windows.Forms.PictureBox picLidar;
		private System.Windows.Forms.Button btnReset;
		private System.Windows.Forms.Panel panelBitmap;
		private System.Windows.Forms.Button btnBitmap;
	}
}

