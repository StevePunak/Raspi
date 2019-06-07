namespace BotClaw
{
	partial class BotClawForm
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
			this.picVideo = new System.Windows.Forms.PictureBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.trackRotation = new System.Windows.Forms.TrackBar();
			this.trackThrust = new System.Windows.Forms.TrackBar();
			this.trackClaw = new System.Windows.Forms.TrackBar();
			this.trackElevation = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.l2 = new System.Windows.Forms.Label();
			this.l1 = new System.Windows.Forms.Label();
			this.l3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.picVideo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackRotation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackThrust)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackClaw)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackElevation)).BeginInit();
			this.SuspendLayout();
			// 
			// picVideo
			// 
			this.picVideo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picVideo.Location = new System.Drawing.Point(0, 0);
			this.picVideo.Name = "picVideo";
			this.picVideo.Size = new System.Drawing.Size(585, 450);
			this.picVideo.TabIndex = 1;
			this.picVideo.TabStop = false;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.picVideo);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.l1);
			this.splitContainer1.Panel2.Controls.Add(this.l3);
			this.splitContainer1.Panel2.Controls.Add(this.l2);
			this.splitContainer1.Panel2.Controls.Add(this.label1);
			this.splitContainer1.Panel2.Controls.Add(this.trackElevation);
			this.splitContainer1.Panel2.Controls.Add(this.trackClaw);
			this.splitContainer1.Panel2.Controls.Add(this.trackThrust);
			this.splitContainer1.Panel2.Controls.Add(this.trackRotation);
			this.splitContainer1.Size = new System.Drawing.Size(800, 450);
			this.splitContainer1.SplitterDistance = 585;
			this.splitContainer1.TabIndex = 2;
			// 
			// trackRotation
			// 
			this.trackRotation.Location = new System.Drawing.Point(30, 30);
			this.trackRotation.Maximum = 100;
			this.trackRotation.Name = "trackRotation";
			this.trackRotation.Size = new System.Drawing.Size(153, 45);
			this.trackRotation.TabIndex = 0;
			this.trackRotation.Scroll += new System.EventHandler(this.OnRotationScroll);
			// 
			// trackThrust
			// 
			this.trackThrust.Location = new System.Drawing.Point(30, 93);
			this.trackThrust.Maximum = 100;
			this.trackThrust.Name = "trackThrust";
			this.trackThrust.Size = new System.Drawing.Size(153, 45);
			this.trackThrust.TabIndex = 0;
			this.trackThrust.Scroll += new System.EventHandler(this.OnThrustScroll);
			// 
			// trackClaw
			// 
			this.trackClaw.Location = new System.Drawing.Point(30, 160);
			this.trackClaw.Maximum = 100;
			this.trackClaw.Name = "trackClaw";
			this.trackClaw.Size = new System.Drawing.Size(153, 45);
			this.trackClaw.TabIndex = 0;
			this.trackClaw.Scroll += new System.EventHandler(this.OnClawScroll);
			// 
			// trackElevation
			// 
			this.trackElevation.Location = new System.Drawing.Point(30, 223);
			this.trackElevation.Maximum = 100;
			this.trackElevation.Name = "trackElevation";
			this.trackElevation.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackElevation.Size = new System.Drawing.Size(45, 153);
			this.trackElevation.TabIndex = 0;
			this.trackElevation.Scroll += new System.EventHandler(this.OnElevationScroll);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(94, 74);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Rotation";
			// 
			// l2
			// 
			this.l2.AutoSize = true;
			this.l2.Location = new System.Drawing.Point(94, 192);
			this.l2.Name = "l2";
			this.l2.Size = new System.Drawing.Size(30, 13);
			this.l2.TabIndex = 1;
			this.l2.Text = "Claw";
			// 
			// l1
			// 
			this.l1.AutoSize = true;
			this.l1.Location = new System.Drawing.Point(94, 125);
			this.l1.Name = "l1";
			this.l1.Size = new System.Drawing.Size(37, 13);
			this.l1.TabIndex = 1;
			this.l1.Text = "Thrust";
			// 
			// l3
			// 
			this.l3.AutoSize = true;
			this.l3.Location = new System.Drawing.Point(66, 296);
			this.l3.Name = "l3";
			this.l3.Size = new System.Drawing.Size(51, 13);
			this.l3.TabIndex = 1;
			this.l3.Text = "Elevation";
			// 
			// BotClawForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.splitContainer1);
			this.Name = "BotClawForm";
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this.Load += new System.EventHandler(this.OnFormLoad);
			((System.ComponentModel.ISupportInitialize)(this.picVideo)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.trackRotation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackThrust)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackClaw)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackElevation)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picVideo;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Label l1;
		private System.Windows.Forms.Label l3;
		private System.Windows.Forms.Label l2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar trackElevation;
		private System.Windows.Forms.TrackBar trackClaw;
		private System.Windows.Forms.TrackBar trackThrust;
		private System.Windows.Forms.TrackBar trackRotation;
	}
}

