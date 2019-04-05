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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RadarForm));
			this.picLidar = new System.Windows.Forms.PictureBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitTopToBottom = new System.Windows.Forms.SplitContainer();
			this.splitRadar = new System.Windows.Forms.SplitContainer();
			this.picFullEnvironment = new System.Windows.Forms.PictureBox();
			this.stackPanel = new StackPanel();
			this.btnGrabInitial = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.listLandmarks = new System.Windows.Forms.ListView();
			((System.ComponentModel.ISupportInitialize)(this.picLidar)).BeginInit();
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
			this.splitRadar.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picFullEnvironment)).BeginInit();
			this.stackPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// picLidar
			// 
			this.picLidar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.picLidar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.picLidar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picLidar.Location = new System.Drawing.Point(0, 0);
			this.picLidar.Name = "picLidar";
			this.picLidar.Size = new System.Drawing.Size(425, 399);
			this.picLidar.TabIndex = 0;
			this.picLidar.TabStop = false;
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
			this.splitContainer1.Panel2.Controls.Add(this.stackPanel);
			this.splitContainer1.Size = new System.Drawing.Size(1053, 702);
			this.splitContainer1.SplitterDistance = 834;
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
			this.splitTopToBottom.Size = new System.Drawing.Size(834, 702);
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
			this.splitRadar.Size = new System.Drawing.Size(834, 399);
			this.splitRadar.SplitterDistance = 425;
			this.splitRadar.TabIndex = 0;
			this.splitRadar.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.OnSplitterMoved);
			// 
			// picFullEnvironment
			// 
			this.picFullEnvironment.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.picFullEnvironment.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picFullEnvironment.Location = new System.Drawing.Point(0, 0);
			this.picFullEnvironment.Name = "picFullEnvironment";
			this.picFullEnvironment.Size = new System.Drawing.Size(834, 298);
			this.picFullEnvironment.TabIndex = 1;
			this.picFullEnvironment.TabStop = false;
			// 
			// stackPanel
			// 
			this.stackPanel.Controls.Add(this.btnGrabInitial);
			this.stackPanel.Controls.Add(this.button2);
			this.stackPanel.Controls.Add(this.listLandmarks);
			this.stackPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.stackPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.stackPanel.Location = new System.Drawing.Point(0, 0);
			this.stackPanel.Name = "stackPanel";
			this.stackPanel.Size = new System.Drawing.Size(214, 702);
			this.stackPanel.TabIndex = 0;
			// 
			// btnGrabInitial
			// 
			this.btnGrabInitial.Location = new System.Drawing.Point(3, 3);
			this.btnGrabInitial.Name = "btnGrabInitial";
			this.btnGrabInitial.Size = new System.Drawing.Size(208, 46);
			this.btnGrabInitial.TabIndex = 0;
			this.btnGrabInitial.Text = "&Grab Initial Landmarks";
			this.btnGrabInitial.UseVisualStyleBackColor = true;
			this.btnGrabInitial.Click += new System.EventHandler(this.OnGrabInitialLandmarksClicked);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(3, 55);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(208, 46);
			this.button2.TabIndex = 0;
			this.button2.Text = "Set &Initial Position";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// listLandmarks
			// 
			this.listLandmarks.FullRowSelect = true;
			this.listLandmarks.HideSelection = false;
			this.listLandmarks.Location = new System.Drawing.Point(3, 107);
			this.listLandmarks.Name = "listLandmarks";
			this.listLandmarks.Size = new System.Drawing.Size(208, 97);
			this.listLandmarks.TabIndex = 1;
			this.listLandmarks.UseCompatibleStateImageBehavior = false;
			this.listLandmarks.View = System.Windows.Forms.View.Details;
			// 
			// RadarForm
			// 
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
			((System.ComponentModel.ISupportInitialize)(this.picLidar)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitTopToBottom.Panel1.ResumeLayout(false);
			this.splitTopToBottom.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitTopToBottom)).EndInit();
			this.splitTopToBottom.ResumeLayout(false);
			this.splitRadar.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitRadar)).EndInit();
			this.splitRadar.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picFullEnvironment)).EndInit();
			this.stackPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picLidar;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitTopToBottom;
		private StackPanel stackPanel;
		private System.Windows.Forms.SplitContainer splitRadar;
		private System.Windows.Forms.PictureBox picFullEnvironment;
		private System.Windows.Forms.Button btnGrabInitial;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ListView listLandmarks;
	}
}

