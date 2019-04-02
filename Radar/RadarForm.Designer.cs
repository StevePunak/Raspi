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
			this.picBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.picBox)).BeginInit();
			this.SuspendLayout();
			// 
			// picBox
			// 
			this.picBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.picBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picBox.Location = new System.Drawing.Point(0, 0);
			this.picBox.Name = "picBox";
			this.picBox.Size = new System.Drawing.Size(748, 669);
			this.picBox.TabIndex = 0;
			this.picBox.TabStop = false;
			// 
			// RadarForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(748, 669);
			this.Controls.Add(this.picBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "RadarForm";
			this.Text = "TrackBot Radar";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this.Load += new System.EventHandler(this.OnFormLoad);
			((System.ComponentModel.ISupportInitialize)(this.picBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picBox;
	}
}

