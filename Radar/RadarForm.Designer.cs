namespace Radar
{
	partial class RadarForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RadarForm));
			this.cmenuRadar = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveBitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitButtons = new System.Windows.Forms.SplitContainer();
			this.splitTopToBottom = new System.Windows.Forms.SplitContainer();
			this.splitRadar = new System.Windows.Forms.SplitContainer();
			this.picLidar = new System.Windows.Forms.PictureBox();
			this.statusRadar = new System.Windows.Forms.StatusStrip();
			this.textRadarCateresian = new System.Windows.Forms.ToolStripStatusLabel();
			this.textLandscapeCoords = new System.Windows.Forms.ToolStripStatusLabel();
			this.tabControImaging = new System.Windows.Forms.TabControl();
			this.tabPageFullEnvironment = new System.Windows.Forms.TabPage();
			this.joystickControl1 = new Radar.JoystickControl();
			this.picFullEnvironment = new System.Windows.Forms.PictureBox();
			this.tabPageOptions = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.checkFlipVertical = new System.Windows.Forms.CheckBox();
			this.checkFlipHorizontal = new System.Windows.Forms.CheckBox();
			this.listImageMetering = new System.Windows.Forms.ComboBox();
			this.listImageAutoWhite = new System.Windows.Forms.ComboBox();
			this.listImageExposure = new System.Windows.Forms.ComboBox();
			this.label14 = new System.Windows.Forms.Label();
			this.listImageEffect = new System.Windows.Forms.ComboBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.textImageColorEffect = new System.Windows.Forms.TextBox();
			this.textImageSaturation = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textImageContrast = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textImageBrightness = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textImageHeight = new System.Windows.Forms.TextBox();
			this.textImageWidth = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textImageDelay = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.checkFullScaleImages = new System.Windows.Forms.CheckBox();
			this.tabPageRobotArm = new System.Windows.Forms.TabPage();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.trackBarClaw = new System.Windows.Forms.TrackBar();
			this.trackBarThrust = new System.Windows.Forms.TrackBar();
			this.trackBarElevation = new System.Windows.Forms.TrackBar();
			this.trackBarRotation = new System.Windows.Forms.TrackBar();
			this.picRobotArm = new System.Windows.Forms.PictureBox();
			this.tabPageMotion = new System.Windows.Forms.TabPage();
			this.panelJoystick = new System.Windows.Forms.Panel();
			this.flowBitmap = new StackPanel();
			this.tabAnalysis = new System.Windows.Forms.TabControl();
			this.tabPageAnalysis = new System.Windows.Forms.TabPage();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.picWorkBitmap = new System.Windows.Forms.ProgressBar();
			this.tabPageDeadReckoning = new System.Windows.Forms.TabPage();
			this.picDeadReckoning = new System.Windows.Forms.PictureBox();
			this.tabPageImage = new System.Windows.Forms.TabPage();
			this.tabCameraImages = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPageVideo = new System.Windows.Forms.TabPage();
			this.pictureTempVideo = new System.Windows.Forms.PictureBox();
			this.liveView = new Radar.LiveView();
			this.tabPageObjects = new System.Windows.Forms.TabPage();
			this.labelCircles = new System.Windows.Forms.Label();
			this.labelRectangles = new System.Windows.Forms.Label();
			this.labelTriangles = new System.Windows.Forms.Label();
			this.imageCircle = new Emgu.CV.UI.ImageBox();
			this.imageRectangles = new Emgu.CV.UI.ImageBox();
			this.imageTriangles = new Emgu.CV.UI.ImageBox();
			this.imageAllDetected = new Emgu.CV.UI.ImageBox();
			this.imageCanny = new Emgu.CV.UI.ImageBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.textAnalysisCoordinates = new System.Windows.Forms.ToolStripStatusLabel();
			this.flowBigButtons = new StackPanel();
			this.btnGrabInitial = new System.Windows.Forms.Button();
			this.btnLoadLandmarks = new System.Windows.Forms.Button();
			this.btnPullBitmap = new System.Windows.Forms.Button();
			this.btnSavePointMarkers = new System.Windows.Forms.Button();
			this.btnParms = new System.Windows.Forms.Button();
			this.btnAnalyze = new System.Windows.Forms.Button();
			this.btnTestArm = new System.Windows.Forms.Button();
			this.botDash = new TrackBotCommon.Controls.BotDash();
			this.panel2 = new System.Windows.Forms.Panel();
			this.textCommand = new System.Windows.Forms.TextBox();
			this.btnCommand = new System.Windows.Forms.Button();
			this.cmenuVideo = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.snapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cmenuLandscape = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.assignLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cmenuRadar.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitButtons)).BeginInit();
			this.splitButtons.Panel1.SuspendLayout();
			this.splitButtons.Panel2.SuspendLayout();
			this.splitButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitTopToBottom)).BeginInit();
			this.splitTopToBottom.Panel1.SuspendLayout();
			this.splitTopToBottom.Panel2.SuspendLayout();
			this.splitTopToBottom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitRadar)).BeginInit();
			this.splitRadar.Panel1.SuspendLayout();
			this.splitRadar.Panel2.SuspendLayout();
			this.splitRadar.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picLidar)).BeginInit();
			this.statusRadar.SuspendLayout();
			this.tabControImaging.SuspendLayout();
			this.tabPageFullEnvironment.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.joystickControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picFullEnvironment)).BeginInit();
			this.tabPageOptions.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tabPageRobotArm.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarClaw)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarThrust)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarElevation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarRotation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picRobotArm)).BeginInit();
			this.tabPageMotion.SuspendLayout();
			this.tabAnalysis.SuspendLayout();
			this.tabPageAnalysis.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tabPageDeadReckoning.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picDeadReckoning)).BeginInit();
			this.tabPageImage.SuspendLayout();
			this.tabCameraImages.SuspendLayout();
			this.tabPageVideo.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureTempVideo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.liveView)).BeginInit();
			this.tabPageObjects.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageCircle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageRectangles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageTriangles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageAllDetected)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCanny)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.flowBigButtons.SuspendLayout();
			this.panel2.SuspendLayout();
			this.cmenuVideo.SuspendLayout();
			this.cmenuLandscape.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmenuRadar
			// 
			this.cmenuRadar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveBitmapToolStripMenuItem});
			this.cmenuRadar.Name = "cmenuRadar";
			this.cmenuRadar.Size = new System.Drawing.Size(165, 48);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.saveToolStripMenuItem.Text = "&Save Point Cloud";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.OnSavePointCloudClicked);
			// 
			// saveBitmapToolStripMenuItem
			// 
			this.saveBitmapToolStripMenuItem.Name = "saveBitmapToolStripMenuItem";
			this.saveBitmapToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.saveBitmapToolStripMenuItem.Text = "S&ave Bitmap";
			this.saveBitmapToolStripMenuItem.Click += new System.EventHandler(this.OnSaveBitmapClicked);
			// 
			// splitButtons
			// 
			this.splitButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitButtons.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitButtons.Location = new System.Drawing.Point(0, 0);
			this.splitButtons.Name = "splitButtons";
			// 
			// splitButtons.Panel1
			// 
			this.splitButtons.Panel1.Controls.Add(this.splitTopToBottom);
			// 
			// splitButtons.Panel2
			// 
			this.splitButtons.Panel2.Controls.Add(this.flowBigButtons);
			this.splitButtons.Size = new System.Drawing.Size(1053, 702);
			this.splitButtons.SplitterDistance = 745;
			this.splitButtons.SplitterWidth = 5;
			this.splitButtons.TabIndex = 1;
			this.splitButtons.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.OnSplitterMoved);
			// 
			// splitTopToBottom
			// 
			this.splitTopToBottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitTopToBottom.Location = new System.Drawing.Point(0, 0);
			this.splitTopToBottom.Name = "splitTopToBottom";
			this.splitTopToBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitTopToBottom.Panel1
			// 
			this.splitTopToBottom.Panel1.Controls.Add(this.splitRadar);
			// 
			// splitTopToBottom.Panel2
			// 
			this.splitTopToBottom.Panel2.AutoScroll = true;
			this.splitTopToBottom.Panel2.Controls.Add(this.tabAnalysis);
			this.splitTopToBottom.Panel2.Controls.Add(this.statusStrip1);
			this.splitTopToBottom.Size = new System.Drawing.Size(745, 702);
			this.splitTopToBottom.SplitterDistance = 237;
			this.splitTopToBottom.SplitterWidth = 5;
			this.splitTopToBottom.TabIndex = 0;
			this.splitTopToBottom.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.OnSplitterMoved);
			// 
			// splitRadar
			// 
			this.splitRadar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitRadar.Location = new System.Drawing.Point(0, 0);
			this.splitRadar.Name = "splitRadar";
			// 
			// splitRadar.Panel1
			// 
			this.splitRadar.Panel1.Controls.Add(this.picLidar);
			this.splitRadar.Panel1.Controls.Add(this.statusRadar);
			// 
			// splitRadar.Panel2
			// 
			this.splitRadar.Panel2.AutoScroll = true;
			this.splitRadar.Panel2.Controls.Add(this.tabControImaging);
			this.splitRadar.Panel2.Controls.Add(this.flowBitmap);
			this.splitRadar.Size = new System.Drawing.Size(745, 237);
			this.splitRadar.SplitterDistance = 355;
			this.splitRadar.TabIndex = 0;
			this.splitRadar.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.OnSplitterMoved);
			// 
			// picLidar
			// 
			this.picLidar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.picLidar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.picLidar.ContextMenuStrip = this.cmenuRadar;
			this.picLidar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picLidar.Location = new System.Drawing.Point(0, 0);
			this.picLidar.Name = "picLidar";
			this.picLidar.Size = new System.Drawing.Size(355, 213);
			this.picLidar.TabIndex = 0;
			this.picLidar.TabStop = false;
			this.picLidar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnRadarMouseMove);
			// 
			// statusRadar
			// 
			this.statusRadar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textRadarCateresian,
            this.textLandscapeCoords});
			this.statusRadar.Location = new System.Drawing.Point(0, 213);
			this.statusRadar.Name = "statusRadar";
			this.statusRadar.Size = new System.Drawing.Size(355, 24);
			this.statusRadar.TabIndex = 1;
			this.statusRadar.Text = "statusStrip1";
			// 
			// textRadarCateresian
			// 
			this.textRadarCateresian.Name = "textRadarCateresian";
			this.textRadarCateresian.Size = new System.Drawing.Size(22, 19);
			this.textRadarCateresian.Text = "0,0";
			// 
			// textLandscapeCoords
			// 
			this.textLandscapeCoords.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.textLandscapeCoords.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
			this.textLandscapeCoords.Name = "textLandscapeCoords";
			this.textLandscapeCoords.Size = new System.Drawing.Size(26, 19);
			this.textLandscapeCoords.Text = "0,0";
			// 
			// tabControImaging
			// 
			this.tabControImaging.Controls.Add(this.tabPageFullEnvironment);
			this.tabControImaging.Controls.Add(this.tabPageOptions);
			this.tabControImaging.Controls.Add(this.tabPageRobotArm);
			this.tabControImaging.Controls.Add(this.tabPageMotion);
			this.tabControImaging.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControImaging.Location = new System.Drawing.Point(0, 0);
			this.tabControImaging.Name = "tabControImaging";
			this.tabControImaging.SelectedIndex = 0;
			this.tabControImaging.Size = new System.Drawing.Size(386, 237);
			this.tabControImaging.TabIndex = 2;
			// 
			// tabPageFullEnvironment
			// 
			this.tabPageFullEnvironment.Controls.Add(this.joystickControl1);
			this.tabPageFullEnvironment.Controls.Add(this.picFullEnvironment);
			this.tabPageFullEnvironment.Location = new System.Drawing.Point(4, 24);
			this.tabPageFullEnvironment.Name = "tabPageFullEnvironment";
			this.tabPageFullEnvironment.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageFullEnvironment.Size = new System.Drawing.Size(378, 209);
			this.tabPageFullEnvironment.TabIndex = 0;
			this.tabPageFullEnvironment.Text = "Full Environment";
			this.tabPageFullEnvironment.UseVisualStyleBackColor = true;
			// 
			// joystickControl1
			// 
			this.joystickControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.joystickControl1.Location = new System.Drawing.Point(3, 3);
			this.joystickControl1.Name = "joystickControl1";
			this.joystickControl1.Size = new System.Drawing.Size(372, 203);
			this.joystickControl1.TabIndex = 2;
			this.joystickControl1.TabStop = false;
			this.joystickControl1.SpeedChanged += new Radar.JoystickControl.SpeedChangedHandler(this.OnJoystickSpeedChanged);
			// 
			// picFullEnvironment
			// 
			this.picFullEnvironment.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.picFullEnvironment.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picFullEnvironment.Location = new System.Drawing.Point(3, 3);
			this.picFullEnvironment.Name = "picFullEnvironment";
			this.picFullEnvironment.Size = new System.Drawing.Size(372, 203);
			this.picFullEnvironment.TabIndex = 1;
			this.picFullEnvironment.TabStop = false;
			this.picFullEnvironment.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnLandscapeMouseClick);
			this.picFullEnvironment.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnLandscapeMouseMove);
			// 
			// tabPageOptions
			// 
			this.tabPageOptions.Controls.Add(this.panel1);
			this.tabPageOptions.Location = new System.Drawing.Point(4, 22);
			this.tabPageOptions.Name = "tabPageOptions";
			this.tabPageOptions.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageOptions.Size = new System.Drawing.Size(378, 211);
			this.tabPageOptions.TabIndex = 1;
			this.tabPageOptions.Text = "Image Options";
			this.tabPageOptions.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.checkFlipVertical);
			this.panel1.Controls.Add(this.checkFlipHorizontal);
			this.panel1.Controls.Add(this.listImageMetering);
			this.panel1.Controls.Add(this.listImageAutoWhite);
			this.panel1.Controls.Add(this.listImageExposure);
			this.panel1.Controls.Add(this.label14);
			this.panel1.Controls.Add(this.listImageEffect);
			this.panel1.Controls.Add(this.label15);
			this.panel1.Controls.Add(this.label12);
			this.panel1.Controls.Add(this.label13);
			this.panel1.Controls.Add(this.textImageColorEffect);
			this.panel1.Controls.Add(this.textImageSaturation);
			this.panel1.Controls.Add(this.label7);
			this.panel1.Controls.Add(this.label10);
			this.panel1.Controls.Add(this.textImageContrast);
			this.panel1.Controls.Add(this.label9);
			this.panel1.Controls.Add(this.textImageBrightness);
			this.panel1.Controls.Add(this.label8);
			this.panel1.Controls.Add(this.textImageHeight);
			this.panel1.Controls.Add(this.textImageWidth);
			this.panel1.Controls.Add(this.label11);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.textImageDelay);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.checkFullScaleImages);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(372, 205);
			this.panel1.TabIndex = 0;
			// 
			// checkFlipVertical
			// 
			this.checkFlipVertical.AutoSize = true;
			this.checkFlipVertical.Location = new System.Drawing.Point(14, 301);
			this.checkFlipVertical.Name = "checkFlipVertical";
			this.checkFlipVertical.Size = new System.Drawing.Size(86, 19);
			this.checkFlipVertical.TabIndex = 13;
			this.checkFlipVertical.Text = "Flip Vertical";
			this.checkFlipVertical.UseVisualStyleBackColor = true;
			this.checkFlipVertical.CheckStateChanged += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// checkFlipHorizontal
			// 
			this.checkFlipHorizontal.AutoSize = true;
			this.checkFlipHorizontal.Location = new System.Drawing.Point(14, 276);
			this.checkFlipHorizontal.Name = "checkFlipHorizontal";
			this.checkFlipHorizontal.Size = new System.Drawing.Size(103, 19);
			this.checkFlipHorizontal.TabIndex = 12;
			this.checkFlipHorizontal.Text = "Flip Horizontal";
			this.checkFlipHorizontal.UseVisualStyleBackColor = true;
			this.checkFlipHorizontal.CheckStateChanged += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// listImageMetering
			// 
			this.listImageMetering.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listImageMetering.FormattingEnabled = true;
			this.listImageMetering.Location = new System.Drawing.Point(256, 215);
			this.listImageMetering.Name = "listImageMetering";
			this.listImageMetering.Size = new System.Drawing.Size(61, 23);
			this.listImageMetering.TabIndex = 9;
			this.listImageMetering.SelectedIndexChanged += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// listImageAutoWhite
			// 
			this.listImageAutoWhite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listImageAutoWhite.FormattingEnabled = true;
			this.listImageAutoWhite.Location = new System.Drawing.Point(256, 244);
			this.listImageAutoWhite.Name = "listImageAutoWhite";
			this.listImageAutoWhite.Size = new System.Drawing.Size(61, 23);
			this.listImageAutoWhite.TabIndex = 11;
			this.listImageAutoWhite.SelectedIndexChanged += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// listImageExposure
			// 
			this.listImageExposure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listImageExposure.FormattingEnabled = true;
			this.listImageExposure.Location = new System.Drawing.Point(89, 244);
			this.listImageExposure.Name = "listImageExposure";
			this.listImageExposure.Size = new System.Drawing.Size(61, 23);
			this.listImageExposure.TabIndex = 10;
			this.listImageExposure.SelectedIndexChanged += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(178, 218);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(55, 15);
			this.label14.TabIndex = 1;
			this.label14.Text = "Metering";
			// 
			// listImageEffect
			// 
			this.listImageEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listImageEffect.FormattingEnabled = true;
			this.listImageEffect.Location = new System.Drawing.Point(89, 215);
			this.listImageEffect.Name = "listImageEffect";
			this.listImageEffect.Size = new System.Drawing.Size(61, 23);
			this.listImageEffect.TabIndex = 8;
			this.listImageEffect.SelectedIndexChanged += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(178, 247);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(67, 15);
			this.label15.TabIndex = 1;
			this.label15.Text = "Auto White";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(11, 189);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(69, 15);
			this.label12.TabIndex = 1;
			this.label12.Text = "Color Effect";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(11, 247);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(54, 15);
			this.label13.TabIndex = 1;
			this.label13.Text = "Exposure";
			// 
			// textImageColorEffect
			// 
			this.textImageColorEffect.Location = new System.Drawing.Point(145, 186);
			this.textImageColorEffect.Name = "textImageColorEffect";
			this.textImageColorEffect.Size = new System.Drawing.Size(49, 23);
			this.textImageColorEffect.TabIndex = 7;
			this.textImageColorEffect.Leave += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// textImageSaturation
			// 
			this.textImageSaturation.Location = new System.Drawing.Point(145, 157);
			this.textImageSaturation.Name = "textImageSaturation";
			this.textImageSaturation.Size = new System.Drawing.Size(49, 23);
			this.textImageSaturation.TabIndex = 6;
			this.textImageSaturation.Leave += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(11, 218);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(73, 15);
			this.label7.TabIndex = 1;
			this.label7.Text = "Image Effect";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(14, 160);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(61, 15);
			this.label10.TabIndex = 1;
			this.label10.Text = "Saturation";
			// 
			// textImageContrast
			// 
			this.textImageContrast.Location = new System.Drawing.Point(145, 128);
			this.textImageContrast.Name = "textImageContrast";
			this.textImageContrast.Size = new System.Drawing.Size(49, 23);
			this.textImageContrast.TabIndex = 5;
			this.textImageContrast.Leave += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(14, 131);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(52, 15);
			this.label9.TabIndex = 1;
			this.label9.Text = "Contrast";
			// 
			// textImageBrightness
			// 
			this.textImageBrightness.Location = new System.Drawing.Point(145, 99);
			this.textImageBrightness.Name = "textImageBrightness";
			this.textImageBrightness.Size = new System.Drawing.Size(49, 23);
			this.textImageBrightness.TabIndex = 4;
			this.textImageBrightness.Leave += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(14, 102);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(62, 15);
			this.label8.TabIndex = 1;
			this.label8.Text = "Brightness";
			// 
			// textImageHeight
			// 
			this.textImageHeight.Location = new System.Drawing.Point(220, 67);
			this.textImageHeight.Name = "textImageHeight";
			this.textImageHeight.Size = new System.Drawing.Size(49, 23);
			this.textImageHeight.TabIndex = 3;
			this.textImageHeight.Leave += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// textImageWidth
			// 
			this.textImageWidth.Location = new System.Drawing.Point(145, 67);
			this.textImageWidth.Name = "textImageWidth";
			this.textImageWidth.Size = new System.Drawing.Size(49, 23);
			this.textImageWidth.TabIndex = 2;
			this.textImageWidth.Leave += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(200, 72);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(14, 15);
			this.label11.TabIndex = 1;
			this.label11.Text = "X";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(14, 70);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(69, 15);
			this.label6.TabIndex = 1;
			this.label6.Text = "Dimensions";
			// 
			// textImageDelay
			// 
			this.textImageDelay.Location = new System.Drawing.Point(145, 38);
			this.textImageDelay.Name = "textImageDelay";
			this.textImageDelay.Size = new System.Drawing.Size(49, 23);
			this.textImageDelay.TabIndex = 1;
			this.textImageDelay.Leave += new System.EventHandler(this.OnImageParametersCheckChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(14, 41);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(99, 15);
			this.label5.TabIndex = 1;
			this.label5.Text = "Image Delay (ms)";
			// 
			// checkFullScaleImages
			// 
			this.checkFullScaleImages.AutoSize = true;
			this.checkFullScaleImages.Location = new System.Drawing.Point(14, 15);
			this.checkFullScaleImages.Name = "checkFullScaleImages";
			this.checkFullScaleImages.Size = new System.Drawing.Size(116, 19);
			this.checkFullScaleImages.TabIndex = 0;
			this.checkFullScaleImages.Text = "Full Scale Images";
			this.checkFullScaleImages.UseVisualStyleBackColor = true;
			this.checkFullScaleImages.CheckedChanged += new System.EventHandler(this.OnFullScaleImagesChecked);
			// 
			// tabPageRobotArm
			// 
			this.tabPageRobotArm.Controls.Add(this.label4);
			this.tabPageRobotArm.Controls.Add(this.label3);
			this.tabPageRobotArm.Controls.Add(this.label2);
			this.tabPageRobotArm.Controls.Add(this.label1);
			this.tabPageRobotArm.Controls.Add(this.trackBarClaw);
			this.tabPageRobotArm.Controls.Add(this.trackBarThrust);
			this.tabPageRobotArm.Controls.Add(this.trackBarElevation);
			this.tabPageRobotArm.Controls.Add(this.trackBarRotation);
			this.tabPageRobotArm.Controls.Add(this.picRobotArm);
			this.tabPageRobotArm.Location = new System.Drawing.Point(4, 22);
			this.tabPageRobotArm.Name = "tabPageRobotArm";
			this.tabPageRobotArm.Size = new System.Drawing.Size(378, 211);
			this.tabPageRobotArm.TabIndex = 2;
			this.tabPageRobotArm.Text = "Robot Arm";
			this.tabPageRobotArm.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(35, 280);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(33, 15);
			this.label4.TabIndex = 16;
			this.label4.Text = "Claw";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(35, 193);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(41, 15);
			this.label3.TabIndex = 17;
			this.label3.Text = "Thrust";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(35, 109);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 15);
			this.label2.TabIndex = 18;
			this.label2.Text = "Elevation";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(35, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 15);
			this.label1.TabIndex = 19;
			this.label1.Text = "Rotation";
			// 
			// trackBarClaw
			// 
			this.trackBarClaw.Location = new System.Drawing.Point(38, 298);
			this.trackBarClaw.Maximum = 100;
			this.trackBarClaw.Name = "trackBarClaw";
			this.trackBarClaw.Size = new System.Drawing.Size(306, 45);
			this.trackBarClaw.TabIndex = 12;
			this.trackBarClaw.Scroll += new System.EventHandler(this.OnClawChange);
			// 
			// trackBarThrust
			// 
			this.trackBarThrust.Location = new System.Drawing.Point(38, 211);
			this.trackBarThrust.Maximum = 100;
			this.trackBarThrust.Name = "trackBarThrust";
			this.trackBarThrust.Size = new System.Drawing.Size(306, 45);
			this.trackBarThrust.TabIndex = 13;
			this.trackBarThrust.Scroll += new System.EventHandler(this.OnThrustChange);
			// 
			// trackBarElevation
			// 
			this.trackBarElevation.Location = new System.Drawing.Point(38, 127);
			this.trackBarElevation.Maximum = 100;
			this.trackBarElevation.Name = "trackBarElevation";
			this.trackBarElevation.Size = new System.Drawing.Size(306, 45);
			this.trackBarElevation.TabIndex = 14;
			this.trackBarElevation.Scroll += new System.EventHandler(this.OnElevationChange);
			// 
			// trackBarRotation
			// 
			this.trackBarRotation.Location = new System.Drawing.Point(38, 45);
			this.trackBarRotation.Maximum = 100;
			this.trackBarRotation.Name = "trackBarRotation";
			this.trackBarRotation.Size = new System.Drawing.Size(306, 45);
			this.trackBarRotation.TabIndex = 15;
			this.trackBarRotation.Scroll += new System.EventHandler(this.OnRotationChange);
			// 
			// picRobotArm
			// 
			this.picRobotArm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.picRobotArm.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picRobotArm.Location = new System.Drawing.Point(0, 0);
			this.picRobotArm.Name = "picRobotArm";
			this.picRobotArm.Size = new System.Drawing.Size(378, 211);
			this.picRobotArm.TabIndex = 0;
			this.picRobotArm.TabStop = false;
			// 
			// tabPageMotion
			// 
			this.tabPageMotion.Controls.Add(this.panelJoystick);
			this.tabPageMotion.Location = new System.Drawing.Point(4, 22);
			this.tabPageMotion.Name = "tabPageMotion";
			this.tabPageMotion.Size = new System.Drawing.Size(378, 211);
			this.tabPageMotion.TabIndex = 3;
			this.tabPageMotion.Text = "Motion";
			this.tabPageMotion.UseVisualStyleBackColor = true;
			// 
			// panelJoystick
			// 
			this.panelJoystick.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelJoystick.Location = new System.Drawing.Point(0, 0);
			this.panelJoystick.Name = "panelJoystick";
			this.panelJoystick.Size = new System.Drawing.Size(378, 211);
			this.panelJoystick.TabIndex = 0;
			// 
			// flowBitmap
			// 
			this.flowBitmap.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.flowBitmap.Location = new System.Drawing.Point(0, 237);
			this.flowBitmap.Name = "flowBitmap";
			this.flowBitmap.Size = new System.Drawing.Size(386, 0);
			this.flowBitmap.TabIndex = 0;
			// 
			// tabAnalysis
			// 
			this.tabAnalysis.Controls.Add(this.tabPageAnalysis);
			this.tabAnalysis.Controls.Add(this.tabPageDeadReckoning);
			this.tabAnalysis.Controls.Add(this.tabPageImage);
			this.tabAnalysis.Controls.Add(this.tabPageVideo);
			this.tabAnalysis.Controls.Add(this.tabPageObjects);
			this.tabAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabAnalysis.Location = new System.Drawing.Point(0, 0);
			this.tabAnalysis.Name = "tabAnalysis";
			this.tabAnalysis.SelectedIndex = 0;
			this.tabAnalysis.Size = new System.Drawing.Size(745, 438);
			this.tabAnalysis.TabIndex = 1;
			this.tabAnalysis.Selected += new System.Windows.Forms.TabControlEventHandler(this.OnTabAnalysisSelected);
			// 
			// tabPageAnalysis
			// 
			this.tabPageAnalysis.AutoScroll = true;
			this.tabPageAnalysis.Controls.Add(this.pictureBox1);
			this.tabPageAnalysis.Controls.Add(this.picWorkBitmap);
			this.tabPageAnalysis.Location = new System.Drawing.Point(4, 24);
			this.tabPageAnalysis.Name = "tabPageAnalysis";
			this.tabPageAnalysis.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAnalysis.Size = new System.Drawing.Size(737, 410);
			this.tabPageAnalysis.TabIndex = 0;
			this.tabPageAnalysis.Text = "Analysis";
			this.tabPageAnalysis.UseVisualStyleBackColor = true;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(8, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(100, 50);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// picWorkBitmap
			// 
			this.picWorkBitmap.Location = new System.Drawing.Point(181, 93);
			this.picWorkBitmap.Name = "picWorkBitmap";
			this.picWorkBitmap.Size = new System.Drawing.Size(100, 23);
			this.picWorkBitmap.TabIndex = 0;
			// 
			// tabPageDeadReckoning
			// 
			this.tabPageDeadReckoning.AutoScroll = true;
			this.tabPageDeadReckoning.Controls.Add(this.picDeadReckoning);
			this.tabPageDeadReckoning.Location = new System.Drawing.Point(4, 22);
			this.tabPageDeadReckoning.Name = "tabPageDeadReckoning";
			this.tabPageDeadReckoning.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDeadReckoning.Size = new System.Drawing.Size(737, 412);
			this.tabPageDeadReckoning.TabIndex = 1;
			this.tabPageDeadReckoning.Text = "Dead Reckoning";
			this.tabPageDeadReckoning.UseVisualStyleBackColor = true;
			// 
			// picDeadReckoning
			// 
			this.picDeadReckoning.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.picDeadReckoning.Location = new System.Drawing.Point(3, 0);
			this.picDeadReckoning.Name = "picDeadReckoning";
			this.picDeadReckoning.Size = new System.Drawing.Size(375, 370);
			this.picDeadReckoning.TabIndex = 0;
			this.picDeadReckoning.TabStop = false;
			// 
			// tabPageImage
			// 
			this.tabPageImage.AutoScroll = true;
			this.tabPageImage.Controls.Add(this.tabCameraImages);
			this.tabPageImage.Location = new System.Drawing.Point(4, 22);
			this.tabPageImage.Name = "tabPageImage";
			this.tabPageImage.Size = new System.Drawing.Size(737, 412);
			this.tabPageImage.TabIndex = 2;
			this.tabPageImage.Text = "Image";
			this.tabPageImage.UseVisualStyleBackColor = true;
			// 
			// tabCameraImages
			// 
			this.tabCameraImages.Controls.Add(this.tabPage1);
			this.tabCameraImages.Controls.Add(this.tabPage2);
			this.tabCameraImages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabCameraImages.Location = new System.Drawing.Point(0, 0);
			this.tabCameraImages.Name = "tabCameraImages";
			this.tabCameraImages.SelectedIndex = 0;
			this.tabCameraImages.Size = new System.Drawing.Size(737, 412);
			this.tabCameraImages.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(4, 24);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(729, 384);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(730, 386);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tabPageVideo
			// 
			this.tabPageVideo.Controls.Add(this.pictureTempVideo);
			this.tabPageVideo.Controls.Add(this.liveView);
			this.tabPageVideo.Location = new System.Drawing.Point(4, 24);
			this.tabPageVideo.Name = "tabPageVideo";
			this.tabPageVideo.Size = new System.Drawing.Size(737, 410);
			this.tabPageVideo.TabIndex = 3;
			this.tabPageVideo.Text = "Video";
			this.tabPageVideo.UseVisualStyleBackColor = true;
			// 
			// pictureTempVideo
			// 
			this.pictureTempVideo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pictureTempVideo.ContextMenuStrip = this.cmenuVideo;
			this.pictureTempVideo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureTempVideo.Location = new System.Drawing.Point(0, 0);
			this.pictureTempVideo.Name = "pictureTempVideo";
			this.pictureTempVideo.Size = new System.Drawing.Size(737, 410);
			this.pictureTempVideo.TabIndex = 2;
			this.pictureTempVideo.TabStop = false;
			// 
			// liveView
			// 
			this.liveView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.liveView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.liveView.Location = new System.Drawing.Point(0, 0);
			this.liveView.Name = "liveView";
			this.liveView.Size = new System.Drawing.Size(737, 410);
			this.liveView.TabIndex = 1;
			this.liveView.TabStop = false;
			// 
			// tabPageObjects
			// 
			this.tabPageObjects.Controls.Add(this.labelCircles);
			this.tabPageObjects.Controls.Add(this.labelRectangles);
			this.tabPageObjects.Controls.Add(this.labelTriangles);
			this.tabPageObjects.Controls.Add(this.imageCircle);
			this.tabPageObjects.Controls.Add(this.imageRectangles);
			this.tabPageObjects.Controls.Add(this.imageTriangles);
			this.tabPageObjects.Controls.Add(this.imageAllDetected);
			this.tabPageObjects.Controls.Add(this.imageCanny);
			this.tabPageObjects.Location = new System.Drawing.Point(4, 22);
			this.tabPageObjects.Name = "tabPageObjects";
			this.tabPageObjects.Size = new System.Drawing.Size(737, 412);
			this.tabPageObjects.TabIndex = 4;
			this.tabPageObjects.Text = "Objects";
			this.tabPageObjects.UseVisualStyleBackColor = true;
			// 
			// labelCircles
			// 
			this.labelCircles.AutoSize = true;
			this.labelCircles.Location = new System.Drawing.Point(613, 189);
			this.labelCircles.Name = "labelCircles";
			this.labelCircles.Size = new System.Drawing.Size(44, 15);
			this.labelCircles.TabIndex = 10;
			this.labelCircles.Text = "label17";
			// 
			// labelRectangles
			// 
			this.labelRectangles.AutoSize = true;
			this.labelRectangles.Location = new System.Drawing.Point(434, 189);
			this.labelRectangles.Name = "labelRectangles";
			this.labelRectangles.Size = new System.Drawing.Size(44, 15);
			this.labelRectangles.TabIndex = 11;
			this.labelRectangles.Text = "label17";
			// 
			// labelTriangles
			// 
			this.labelTriangles.AutoSize = true;
			this.labelTriangles.Location = new System.Drawing.Point(260, 189);
			this.labelTriangles.Name = "labelTriangles";
			this.labelTriangles.Size = new System.Drawing.Size(44, 15);
			this.labelTriangles.TabIndex = 12;
			this.labelTriangles.Text = "label17";
			// 
			// imageCircle
			// 
			this.imageCircle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imageCircle.Location = new System.Drawing.Point(551, 24);
			this.imageCircle.Name = "imageCircle";
			this.imageCircle.Size = new System.Drawing.Size(171, 151);
			this.imageCircle.TabIndex = 5;
			this.imageCircle.TabStop = false;
			// 
			// imageRectangles
			// 
			this.imageRectangles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imageRectangles.Location = new System.Drawing.Point(374, 24);
			this.imageRectangles.Name = "imageRectangles";
			this.imageRectangles.Size = new System.Drawing.Size(171, 151);
			this.imageRectangles.TabIndex = 6;
			this.imageRectangles.TabStop = false;
			// 
			// imageTriangles
			// 
			this.imageTriangles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imageTriangles.Location = new System.Drawing.Point(197, 24);
			this.imageTriangles.Name = "imageTriangles";
			this.imageTriangles.Size = new System.Drawing.Size(171, 151);
			this.imageTriangles.TabIndex = 7;
			this.imageTriangles.TabStop = false;
			// 
			// imageAllDetected
			// 
			this.imageAllDetected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imageAllDetected.Location = new System.Drawing.Point(20, 236);
			this.imageAllDetected.Name = "imageAllDetected";
			this.imageAllDetected.Size = new System.Drawing.Size(171, 151);
			this.imageAllDetected.TabIndex = 8;
			this.imageAllDetected.TabStop = false;
			// 
			// imageCanny
			// 
			this.imageCanny.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imageCanny.Location = new System.Drawing.Point(20, 24);
			this.imageCanny.Name = "imageCanny";
			this.imageCanny.Size = new System.Drawing.Size(171, 151);
			this.imageCanny.TabIndex = 9;
			this.imageCanny.TabStop = false;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textAnalysisCoordinates});
			this.statusStrip1.Location = new System.Drawing.Point(0, 438);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(745, 22);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// textAnalysisCoordinates
			// 
			this.textAnalysisCoordinates.Name = "textAnalysisCoordinates";
			this.textAnalysisCoordinates.Size = new System.Drawing.Size(49, 17);
			this.textAnalysisCoordinates.Text = "x=0,y=0";
			// 
			// flowBigButtons
			// 
			this.flowBigButtons.Controls.Add(this.btnGrabInitial);
			this.flowBigButtons.Controls.Add(this.btnLoadLandmarks);
			this.flowBigButtons.Controls.Add(this.btnPullBitmap);
			this.flowBigButtons.Controls.Add(this.btnSavePointMarkers);
			this.flowBigButtons.Controls.Add(this.btnParms);
			this.flowBigButtons.Controls.Add(this.btnAnalyze);
			this.flowBigButtons.Controls.Add(this.btnTestArm);
			this.flowBigButtons.Controls.Add(this.botDash);
			this.flowBigButtons.Controls.Add(this.panel2);
			this.flowBigButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowBigButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowBigButtons.Location = new System.Drawing.Point(0, 0);
			this.flowBigButtons.Name = "flowBigButtons";
			this.flowBigButtons.Size = new System.Drawing.Size(303, 702);
			this.flowBigButtons.TabIndex = 0;
			// 
			// btnGrabInitial
			// 
			this.btnGrabInitial.Location = new System.Drawing.Point(3, 3);
			this.btnGrabInitial.Name = "btnGrabInitial";
			this.btnGrabInitial.Size = new System.Drawing.Size(297, 46);
			this.btnGrabInitial.TabIndex = 0;
			this.btnGrabInitial.Text = "&Grab Initial Landmarks";
			this.btnGrabInitial.UseVisualStyleBackColor = true;
			this.btnGrabInitial.Click += new System.EventHandler(this.OnGrabInitialLandmarksClicked);
			// 
			// btnLoadLandmarks
			// 
			this.btnLoadLandmarks.Location = new System.Drawing.Point(3, 55);
			this.btnLoadLandmarks.Name = "btnLoadLandmarks";
			this.btnLoadLandmarks.Size = new System.Drawing.Size(297, 46);
			this.btnLoadLandmarks.TabIndex = 0;
			this.btnLoadLandmarks.Text = "&Load Landmarks";
			this.btnLoadLandmarks.UseVisualStyleBackColor = true;
			this.btnLoadLandmarks.Click += new System.EventHandler(this.OnLoadLandmarksClicked);
			// 
			// btnPullBitmap
			// 
			this.btnPullBitmap.Location = new System.Drawing.Point(3, 107);
			this.btnPullBitmap.Name = "btnPullBitmap";
			this.btnPullBitmap.Size = new System.Drawing.Size(297, 42);
			this.btnPullBitmap.TabIndex = 0;
			this.btnPullBitmap.Text = "Pull";
			this.btnPullBitmap.UseVisualStyleBackColor = true;
			this.btnPullBitmap.Click += new System.EventHandler(this.OnPullClicked);
			// 
			// btnSavePointMarkers
			// 
			this.btnSavePointMarkers.Location = new System.Drawing.Point(3, 155);
			this.btnSavePointMarkers.Name = "btnSavePointMarkers";
			this.btnSavePointMarkers.Size = new System.Drawing.Size(297, 46);
			this.btnSavePointMarkers.TabIndex = 0;
			this.btnSavePointMarkers.Text = "&Fetch Image";
			this.btnSavePointMarkers.UseVisualStyleBackColor = true;
			this.btnSavePointMarkers.Click += new System.EventHandler(this.OnFetchImageClicked);
			// 
			// btnParms
			// 
			this.btnParms.Location = new System.Drawing.Point(3, 207);
			this.btnParms.Name = "btnParms";
			this.btnParms.Size = new System.Drawing.Size(297, 42);
			this.btnParms.TabIndex = 0;
			this.btnParms.Text = "Change Corner Parms";
			this.btnParms.UseVisualStyleBackColor = true;
			this.btnParms.Click += new System.EventHandler(this.OnParmsClicked);
			// 
			// btnAnalyze
			// 
			this.btnAnalyze.Location = new System.Drawing.Point(3, 255);
			this.btnAnalyze.Name = "btnAnalyze";
			this.btnAnalyze.Size = new System.Drawing.Size(297, 42);
			this.btnAnalyze.TabIndex = 0;
			this.btnAnalyze.Text = "Analyze";
			this.btnAnalyze.UseVisualStyleBackColor = true;
			this.btnAnalyze.Click += new System.EventHandler(this.OnAnalyzeClicked);
			// 
			// btnTestArm
			// 
			this.btnTestArm.Location = new System.Drawing.Point(3, 303);
			this.btnTestArm.Name = "btnTestArm";
			this.btnTestArm.Size = new System.Drawing.Size(297, 42);
			this.btnTestArm.TabIndex = 0;
			this.btnTestArm.Text = "Analyze";
			this.btnTestArm.UseVisualStyleBackColor = true;
			this.btnTestArm.Click += new System.EventHandler(this.OnTestArmClicked);
			// 
			// botDash
			// 
			this.botDash.Bearing = 0D;
			this.botDash.DestinationBearing = 0D;
			this.botDash.FrontPrimaryRange = 0D;
			this.botDash.FrontSecondaryRange = 0D;
			this.botDash.Location = new System.Drawing.Point(3, 351);
			this.botDash.Name = "botDash";
			this.botDash.RearPrimaryRange = 0D;
			this.botDash.RearSecondaryRange = 0D;
			this.botDash.Size = new System.Drawing.Size(297, 182);
			this.botDash.TabIndex = 7;
			this.botDash.TankBitmap = null;
			// 
			// panel2
			// 
			this.panel2.AutoScroll = true;
			this.panel2.Controls.Add(this.textCommand);
			this.panel2.Controls.Add(this.btnCommand);
			this.panel2.Location = new System.Drawing.Point(3, 539);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(297, 23);
			this.panel2.TabIndex = 6;
			// 
			// textCommand
			// 
			this.textCommand.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textCommand.Location = new System.Drawing.Point(0, 0);
			this.textCommand.Name = "textCommand";
			this.textCommand.Size = new System.Drawing.Size(269, 23);
			this.textCommand.TabIndex = 4;
			// 
			// btnCommand
			// 
			this.btnCommand.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnCommand.Location = new System.Drawing.Point(269, 0);
			this.btnCommand.Name = "btnCommand";
			this.btnCommand.Size = new System.Drawing.Size(28, 23);
			this.btnCommand.TabIndex = 5;
			this.btnCommand.Text = "+";
			this.btnCommand.UseVisualStyleBackColor = true;
			this.btnCommand.Click += new System.EventHandler(this.OnCommandButtonClicked);
			// 
			// cmenuVideo
			// 
			this.cmenuVideo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.snapToolStripMenuItem,
            this.preferencesToolStripMenuItem});
			this.cmenuVideo.Name = "cmenuVideo";
			this.cmenuVideo.Size = new System.Drawing.Size(136, 48);
			// 
			// snapToolStripMenuItem
			// 
			this.snapToolStripMenuItem.Name = "snapToolStripMenuItem";
			this.snapToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.snapToolStripMenuItem.Text = "Snap";
			this.snapToolStripMenuItem.Click += new System.EventHandler(this.OnSnapClicked);
			// 
			// preferencesToolStripMenuItem
			// 
			this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
			this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.preferencesToolStripMenuItem.Text = "Preferences";
			this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.OnPreferencesClicked);
			// 
			// cmenuLandscape
			// 
			this.cmenuLandscape.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.assignLabelToolStripMenuItem});
			this.cmenuLandscape.Name = "contextMenuStrip1";
			this.cmenuLandscape.Size = new System.Drawing.Size(141, 26);
			// 
			// assignLabelToolStripMenuItem
			// 
			this.assignLabelToolStripMenuItem.Name = "assignLabelToolStripMenuItem";
			this.assignLabelToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.assignLabelToolStripMenuItem.Text = "&Assign Label";
			this.assignLabelToolStripMenuItem.Click += new System.EventHandler(this.OnAssignLandmarkLabelClicked);
			// 
			// RadarForm
			// 
			this.AcceptButton = this.btnCommand;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1053, 702);
			this.Controls.Add(this.splitButtons);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "RadarForm";
			this.Text = "TrackBot Radar";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.ResizeEnd += new System.EventHandler(this.OnFormResizeEnd);
			this.Move += new System.EventHandler(this.OnWindowMoved);
			this.cmenuRadar.ResumeLayout(false);
			this.splitButtons.Panel1.ResumeLayout(false);
			this.splitButtons.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitButtons)).EndInit();
			this.splitButtons.ResumeLayout(false);
			this.splitTopToBottom.Panel1.ResumeLayout(false);
			this.splitTopToBottom.Panel2.ResumeLayout(false);
			this.splitTopToBottom.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitTopToBottom)).EndInit();
			this.splitTopToBottom.ResumeLayout(false);
			this.splitRadar.Panel1.ResumeLayout(false);
			this.splitRadar.Panel1.PerformLayout();
			this.splitRadar.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitRadar)).EndInit();
			this.splitRadar.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picLidar)).EndInit();
			this.statusRadar.ResumeLayout(false);
			this.statusRadar.PerformLayout();
			this.tabControImaging.ResumeLayout(false);
			this.tabPageFullEnvironment.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.joystickControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picFullEnvironment)).EndInit();
			this.tabPageOptions.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.tabPageRobotArm.ResumeLayout(false);
			this.tabPageRobotArm.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarClaw)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarThrust)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarElevation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarRotation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picRobotArm)).EndInit();
			this.tabPageMotion.ResumeLayout(false);
			this.tabAnalysis.ResumeLayout(false);
			this.tabPageAnalysis.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tabPageDeadReckoning.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picDeadReckoning)).EndInit();
			this.tabPageImage.ResumeLayout(false);
			this.tabCameraImages.ResumeLayout(false);
			this.tabPageVideo.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureTempVideo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.liveView)).EndInit();
			this.tabPageObjects.ResumeLayout(false);
			this.tabPageObjects.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageCircle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageRectangles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageTriangles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageAllDetected)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCanny)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.flowBigButtons.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.cmenuVideo.ResumeLayout(false);
			this.cmenuLandscape.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.SplitContainer splitButtons;
		private System.Windows.Forms.SplitContainer splitTopToBottom;
		private StackPanel flowBigButtons;
		private System.Windows.Forms.PictureBox picFullEnvironment;
		private System.Windows.Forms.Button btnGrabInitial;
		private System.Windows.Forms.Button btnLoadLandmarks;
		private System.Windows.Forms.Button btnSavePointMarkers;
		private System.Windows.Forms.ContextMenuStrip cmenuLandscape;
		private System.Windows.Forms.ToolStripMenuItem assignLabelToolStripMenuItem;
		private System.Windows.Forms.Button btnPullBitmap;
		private System.Windows.Forms.Button btnParms;
		private System.Windows.Forms.Button btnAnalyze;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TextBox textCommand;
		private System.Windows.Forms.Button btnCommand;
		private System.Windows.Forms.ContextMenuStrip cmenuRadar;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveBitmapToolStripMenuItem;
		private TrackBotCommon.Controls.BotDash botDash;
		private System.Windows.Forms.SplitContainer splitRadar;
		private System.Windows.Forms.StatusStrip statusRadar;
		private System.Windows.Forms.ToolStripStatusLabel textRadarCateresian;
		private System.Windows.Forms.ToolStripStatusLabel textLandscapeCoords;
		private System.Windows.Forms.TabControl tabAnalysis;
		private System.Windows.Forms.TabPage tabPageAnalysis;
		private System.Windows.Forms.TabPage tabPageDeadReckoning;
		private System.Windows.Forms.PictureBox picDeadReckoning;
		private StackPanel flowBitmap;
		private System.Windows.Forms.TabPage tabPageImage;
		private System.Windows.Forms.TabControl tabCameraImages;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPageVideo;
		private System.Windows.Forms.ContextMenuStrip cmenuVideo;
		private System.Windows.Forms.TabControl tabControImaging;
		private System.Windows.Forms.TabPage tabPageFullEnvironment;
		private System.Windows.Forms.TabPage tabPageOptions;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox checkFullScaleImages;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel textAnalysisCoordinates;
		private System.Windows.Forms.TabPage tabPageRobotArm;
		private System.Windows.Forms.PictureBox picRobotArm;
		private System.Windows.Forms.Button btnTestArm;
		private System.Windows.Forms.TextBox textImageDelay;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textImageHeight;
		private System.Windows.Forms.TextBox textImageWidth;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textImageSaturation;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textImageContrast;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textImageBrightness;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ComboBox listImageEffect;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.ComboBox listImageMetering;
		private System.Windows.Forms.ComboBox listImageAutoWhite;
		private System.Windows.Forms.ComboBox listImageExposure;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckBox checkFlipVertical;
		private System.Windows.Forms.CheckBox checkFlipHorizontal;
		private System.Windows.Forms.TextBox textImageColorEffect;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar trackBarClaw;
		private System.Windows.Forms.TrackBar trackBarThrust;
		private System.Windows.Forms.TrackBar trackBarElevation;
		private System.Windows.Forms.TrackBar trackBarRotation;
		private System.Windows.Forms.TabPage tabPageMotion;
		private System.Windows.Forms.Panel panelJoystick;
		private JoystickControl joystickControl1;
		private LiveView liveView;
		private System.Windows.Forms.ProgressBar picWorkBitmap;
		private System.Windows.Forms.TabPage tabPageObjects;
		private System.Windows.Forms.Label labelCircles;
		private System.Windows.Forms.Label labelRectangles;
		private System.Windows.Forms.Label labelTriangles;
		private Emgu.CV.UI.ImageBox imageCircle;
		private Emgu.CV.UI.ImageBox imageRectangles;
		private Emgu.CV.UI.ImageBox imageTriangles;
		private Emgu.CV.UI.ImageBox imageAllDetected;
		private Emgu.CV.UI.ImageBox imageCanny;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureTempVideo;
		private System.Windows.Forms.PictureBox picLidar;
		private System.Windows.Forms.ToolStripMenuItem snapToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
	}
}

