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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitTopToBottom = new System.Windows.Forms.SplitContainer();
			this.splitRadar = new System.Windows.Forms.SplitContainer();
			this.picLidar = new System.Windows.Forms.PictureBox();
			this.statusRadar = new System.Windows.Forms.StatusStrip();
			this.textRadarCateresian = new System.Windows.Forms.ToolStripStatusLabel();
			this.textLandscapeCoords = new System.Windows.Forms.ToolStripStatusLabel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageAnalysis = new System.Windows.Forms.TabPage();
			this.picWorkBitmap = new System.Windows.Forms.PictureBox();
			this.tabPageDeadReckoning = new System.Windows.Forms.TabPage();
			this.picDeadReckoning = new System.Windows.Forms.PictureBox();
			this.flowBitmap = new StackPanel();
			this.picFullEnvironment = new System.Windows.Forms.PictureBox();
			this.flowBigButtons = new StackPanel();
			this.btnGrabInitial = new System.Windows.Forms.Button();
			this.btnLoadLandmarks = new System.Windows.Forms.Button();
			this.btnSavePointMarkers = new System.Windows.Forms.Button();
			this.listLandmarks = new System.Windows.Forms.ListView();
			this.btnPullBitmap = new System.Windows.Forms.Button();
			this.btnParms = new System.Windows.Forms.Button();
			this.btnAnalyze = new System.Windows.Forms.Button();
			this.botDash = new TrackBotCommon.Controls.BotDash();
			this.panel2 = new System.Windows.Forms.Panel();
			this.textCommand = new System.Windows.Forms.TextBox();
			this.btnCommand = new System.Windows.Forms.Button();
			this.cmenuLandscape = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.assignLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cmenuRadar.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
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
			this.tabControl1.SuspendLayout();
			this.tabPageAnalysis.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picWorkBitmap)).BeginInit();
			this.tabPageDeadReckoning.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picDeadReckoning)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picFullEnvironment)).BeginInit();
			this.flowBigButtons.SuspendLayout();
			this.panel2.SuspendLayout();
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
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitTopToBottom);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.flowBigButtons);
			this.splitContainer1.Size = new System.Drawing.Size(1053, 702);
			this.splitContainer1.SplitterDistance = 775;
			this.splitContainer1.SplitterWidth = 5;
			this.splitContainer1.TabIndex = 1;
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
			this.splitTopToBottom.Panel2.Controls.Add(this.picFullEnvironment);
			this.splitTopToBottom.Size = new System.Drawing.Size(775, 702);
			this.splitTopToBottom.SplitterDistance = 399;
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
			this.splitRadar.Panel2.Controls.Add(this.tabControl1);
			this.splitRadar.Panel2.Controls.Add(this.flowBitmap);
			this.splitRadar.Size = new System.Drawing.Size(775, 399);
			this.splitRadar.SplitterDistance = 385;
			this.splitRadar.TabIndex = 0;
			this.splitRadar.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.OnSplitterMoved);
			// 
			// picLidar
			// 
			this.picLidar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.picLidar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.picLidar.ContextMenuStrip = this.cmenuRadar;
			this.picLidar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picLidar.Location = new System.Drawing.Point(0, 0);
			this.picLidar.Name = "picLidar";
			this.picLidar.Size = new System.Drawing.Size(385, 375);
			this.picLidar.TabIndex = 0;
			this.picLidar.TabStop = false;
			this.picLidar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnRadarMouseMove);
			// 
			// statusRadar
			// 
			this.statusRadar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textRadarCateresian,
            this.textLandscapeCoords});
			this.statusRadar.Location = new System.Drawing.Point(0, 375);
			this.statusRadar.Name = "statusRadar";
			this.statusRadar.Size = new System.Drawing.Size(385, 24);
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
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageAnalysis);
			this.tabControl1.Controls.Add(this.tabPageDeadReckoning);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(386, 399);
			this.tabControl1.TabIndex = 1;
			// 
			// tabPageAnalysis
			// 
			this.tabPageAnalysis.Controls.Add(this.picWorkBitmap);
			this.tabPageAnalysis.Location = new System.Drawing.Point(4, 24);
			this.tabPageAnalysis.Name = "tabPageAnalysis";
			this.tabPageAnalysis.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAnalysis.Size = new System.Drawing.Size(378, 371);
			this.tabPageAnalysis.TabIndex = 0;
			this.tabPageAnalysis.Text = "Analysis";
			this.tabPageAnalysis.UseVisualStyleBackColor = true;
			// 
			// picWorkBitmap
			// 
			this.picWorkBitmap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.picWorkBitmap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.picWorkBitmap.Location = new System.Drawing.Point(4, 16);
			this.picWorkBitmap.Name = "picWorkBitmap";
			this.picWorkBitmap.Size = new System.Drawing.Size(371, 339);
			this.picWorkBitmap.TabIndex = 2;
			this.picWorkBitmap.TabStop = false;
			// 
			// tabPageDeadReckoning
			// 
			this.tabPageDeadReckoning.Controls.Add(this.picDeadReckoning);
			this.tabPageDeadReckoning.Location = new System.Drawing.Point(4, 22);
			this.tabPageDeadReckoning.Name = "tabPageDeadReckoning";
			this.tabPageDeadReckoning.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDeadReckoning.Size = new System.Drawing.Size(378, 373);
			this.tabPageDeadReckoning.TabIndex = 1;
			this.tabPageDeadReckoning.Text = "Dead Reckoning";
			this.tabPageDeadReckoning.UseVisualStyleBackColor = true;
			// 
			// picDeadReckoning
			// 
			this.picDeadReckoning.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.picDeadReckoning.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picDeadReckoning.Location = new System.Drawing.Point(3, 3);
			this.picDeadReckoning.Name = "picDeadReckoning";
			this.picDeadReckoning.Size = new System.Drawing.Size(372, 367);
			this.picDeadReckoning.TabIndex = 0;
			this.picDeadReckoning.TabStop = false;
			// 
			// flowBitmap
			// 
			this.flowBitmap.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.flowBitmap.Location = new System.Drawing.Point(0, 399);
			this.flowBitmap.Name = "flowBitmap";
			this.flowBitmap.Size = new System.Drawing.Size(386, 0);
			this.flowBitmap.TabIndex = 0;
			// 
			// picFullEnvironment
			// 
			this.picFullEnvironment.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.picFullEnvironment.Location = new System.Drawing.Point(0, 42);
			this.picFullEnvironment.Name = "picFullEnvironment";
			this.picFullEnvironment.Size = new System.Drawing.Size(706, 198);
			this.picFullEnvironment.TabIndex = 1;
			this.picFullEnvironment.TabStop = false;
			this.picFullEnvironment.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnLandscapeMouseClick);
			this.picFullEnvironment.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnLandscapeMouseMove);
			// 
			// flowBigButtons
			// 
			this.flowBigButtons.Controls.Add(this.btnGrabInitial);
			this.flowBigButtons.Controls.Add(this.btnLoadLandmarks);
			this.flowBigButtons.Controls.Add(this.btnSavePointMarkers);
			this.flowBigButtons.Controls.Add(this.listLandmarks);
			this.flowBigButtons.Controls.Add(this.btnPullBitmap);
			this.flowBigButtons.Controls.Add(this.btnParms);
			this.flowBigButtons.Controls.Add(this.btnAnalyze);
			this.flowBigButtons.Controls.Add(this.botDash);
			this.flowBigButtons.Controls.Add(this.panel2);
			this.flowBigButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowBigButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowBigButtons.Location = new System.Drawing.Point(0, 0);
			this.flowBigButtons.Name = "flowBigButtons";
			this.flowBigButtons.Size = new System.Drawing.Size(273, 702);
			this.flowBigButtons.TabIndex = 0;
			// 
			// btnGrabInitial
			// 
			this.btnGrabInitial.Location = new System.Drawing.Point(3, 3);
			this.btnGrabInitial.Name = "btnGrabInitial";
			this.btnGrabInitial.Size = new System.Drawing.Size(267, 46);
			this.btnGrabInitial.TabIndex = 0;
			this.btnGrabInitial.Text = "&Grab Initial Landmarks";
			this.btnGrabInitial.UseVisualStyleBackColor = true;
			this.btnGrabInitial.Click += new System.EventHandler(this.OnGrabInitialLandmarksClicked);
			// 
			// btnLoadLandmarks
			// 
			this.btnLoadLandmarks.Location = new System.Drawing.Point(3, 55);
			this.btnLoadLandmarks.Name = "btnLoadLandmarks";
			this.btnLoadLandmarks.Size = new System.Drawing.Size(267, 46);
			this.btnLoadLandmarks.TabIndex = 0;
			this.btnLoadLandmarks.Text = "&Load Landmarks";
			this.btnLoadLandmarks.UseVisualStyleBackColor = true;
			this.btnLoadLandmarks.Click += new System.EventHandler(this.OnLoadLandmarksClicked);
			// 
			// btnSavePointMarkers
			// 
			this.btnSavePointMarkers.Location = new System.Drawing.Point(3, 107);
			this.btnSavePointMarkers.Name = "btnSavePointMarkers";
			this.btnSavePointMarkers.Size = new System.Drawing.Size(267, 46);
			this.btnSavePointMarkers.TabIndex = 0;
			this.btnSavePointMarkers.Text = "&Save Point Markers";
			this.btnSavePointMarkers.UseVisualStyleBackColor = true;
			this.btnSavePointMarkers.Click += new System.EventHandler(this.OnSavePointMarkersClicked);
			// 
			// listLandmarks
			// 
			this.listLandmarks.FullRowSelect = true;
			this.listLandmarks.HideSelection = false;
			this.listLandmarks.Location = new System.Drawing.Point(3, 159);
			this.listLandmarks.Name = "listLandmarks";
			this.listLandmarks.Size = new System.Drawing.Size(267, 97);
			this.listLandmarks.TabIndex = 1;
			this.listLandmarks.UseCompatibleStateImageBehavior = false;
			this.listLandmarks.View = System.Windows.Forms.View.Details;
			// 
			// btnPullBitmap
			// 
			this.btnPullBitmap.Location = new System.Drawing.Point(3, 262);
			this.btnPullBitmap.Name = "btnPullBitmap";
			this.btnPullBitmap.Size = new System.Drawing.Size(267, 42);
			this.btnPullBitmap.TabIndex = 0;
			this.btnPullBitmap.Text = "Pull";
			this.btnPullBitmap.UseVisualStyleBackColor = true;
			this.btnPullBitmap.Click += new System.EventHandler(this.OnPullClicked);
			// 
			// btnParms
			// 
			this.btnParms.Location = new System.Drawing.Point(3, 310);
			this.btnParms.Name = "btnParms";
			this.btnParms.Size = new System.Drawing.Size(267, 42);
			this.btnParms.TabIndex = 0;
			this.btnParms.Text = "Parms";
			this.btnParms.UseVisualStyleBackColor = true;
			this.btnParms.Click += new System.EventHandler(this.OnParmsClicked);
			// 
			// btnAnalyze
			// 
			this.btnAnalyze.Location = new System.Drawing.Point(3, 358);
			this.btnAnalyze.Name = "btnAnalyze";
			this.btnAnalyze.Size = new System.Drawing.Size(267, 42);
			this.btnAnalyze.TabIndex = 0;
			this.btnAnalyze.Text = "Analyze";
			this.btnAnalyze.UseVisualStyleBackColor = true;
			this.btnAnalyze.Click += new System.EventHandler(this.OnAnalyzeClicked);
			// 
			// botDash
			// 
			this.botDash.Bearing = 0D;
			this.botDash.DestinationBearing = 0D;
			this.botDash.FrontPrimaryRange = 0D;
			this.botDash.FrontSecondaryRange = 0D;
			this.botDash.Location = new System.Drawing.Point(3, 406);
			this.botDash.Name = "botDash";
			this.botDash.RearPrimaryRange = 0D;
			this.botDash.RearSecondaryRange = 0D;
			this.botDash.Size = new System.Drawing.Size(267, 182);
			this.botDash.TabIndex = 7;
			this.botDash.TankBitmap = null;
			// 
			// panel2
			// 
			this.panel2.AutoScroll = true;
			this.panel2.Controls.Add(this.textCommand);
			this.panel2.Controls.Add(this.btnCommand);
			this.panel2.Location = new System.Drawing.Point(3, 594);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(267, 23);
			this.panel2.TabIndex = 6;
			// 
			// textCommand
			// 
			this.textCommand.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textCommand.Location = new System.Drawing.Point(0, 0);
			this.textCommand.Name = "textCommand";
			this.textCommand.Size = new System.Drawing.Size(239, 23);
			this.textCommand.TabIndex = 4;
			// 
			// btnCommand
			// 
			this.btnCommand.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnCommand.Location = new System.Drawing.Point(239, 0);
			this.btnCommand.Name = "btnCommand";
			this.btnCommand.Size = new System.Drawing.Size(28, 23);
			this.btnCommand.TabIndex = 5;
			this.btnCommand.Text = "+";
			this.btnCommand.UseVisualStyleBackColor = true;
			this.btnCommand.Click += new System.EventHandler(this.OnCommandButtonClicked);
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
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "RadarForm";
			this.Text = "TrackBot Radar";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.ResizeEnd += new System.EventHandler(this.OnFormResizeEnd);
			this.Move += new System.EventHandler(this.OnWindowMoved);
			this.cmenuRadar.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitTopToBottom.Panel1.ResumeLayout(false);
			this.splitTopToBottom.Panel2.ResumeLayout(false);
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
			this.tabControl1.ResumeLayout(false);
			this.tabPageAnalysis.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picWorkBitmap)).EndInit();
			this.tabPageDeadReckoning.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picDeadReckoning)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picFullEnvironment)).EndInit();
			this.flowBigButtons.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.cmenuLandscape.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.SplitContainer splitContainer1;
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
		private System.Windows.Forms.ListView listLandmarks;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TextBox textCommand;
		private System.Windows.Forms.Button btnCommand;
		private System.Windows.Forms.ContextMenuStrip cmenuRadar;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveBitmapToolStripMenuItem;
		private TrackBotCommon.Controls.BotDash botDash;
		private System.Windows.Forms.SplitContainer splitRadar;
		private System.Windows.Forms.PictureBox picLidar;
		private System.Windows.Forms.StatusStrip statusRadar;
		private System.Windows.Forms.ToolStripStatusLabel textRadarCateresian;
		private System.Windows.Forms.ToolStripStatusLabel textLandscapeCoords;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageAnalysis;
		private System.Windows.Forms.PictureBox picWorkBitmap;
		private System.Windows.Forms.TabPage tabPageDeadReckoning;
		private System.Windows.Forms.PictureBox picDeadReckoning;
		private StackPanel flowBitmap;
	}
}

