namespace TrackBotCommon.Controls
{
	partial class BotDash
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.picTank = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.textRearSecondary = new System.Windows.Forms.Label();
			this.textRearPrimary = new System.Windows.Forms.Label();
			this.textFrontPrimary = new System.Windows.Forms.Label();
			this.textFrontSecondary = new System.Windows.Forms.Label();
			this.picCompass = new System.Windows.Forms.PictureBox();
			this.panelCompass = new System.Windows.Forms.Panel();
			this.textDestinationBearing = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.picTank)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picCompass)).BeginInit();
			this.panelCompass.SuspendLayout();
			this.SuspendLayout();
			// 
			// picTank
			// 
			this.picTank.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.picTank.Location = new System.Drawing.Point(3, 24);
			this.picTank.Name = "picTank";
			this.picTank.Size = new System.Drawing.Size(100, 100);
			this.picTank.TabIndex = 0;
			this.picTank.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.textRearSecondary);
			this.panel1.Controls.Add(this.textRearPrimary);
			this.panel1.Controls.Add(this.textFrontPrimary);
			this.panel1.Controls.Add(this.textFrontSecondary);
			this.panel1.Controls.Add(this.picTank);
			this.panel1.Location = new System.Drawing.Point(120, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(105, 148);
			this.panel1.TabIndex = 1;
			// 
			// textRearSecondary
			// 
			this.textRearSecondary.BackColor = System.Drawing.Color.Maroon;
			this.textRearSecondary.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.textRearSecondary.Font = new System.Drawing.Font("Noto Sans Lisu", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textRearSecondary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.textRearSecondary.Location = new System.Drawing.Point(52, 125);
			this.textRearSecondary.Name = "textRearSecondary";
			this.textRearSecondary.Size = new System.Drawing.Size(49, 25);
			this.textRearSecondary.TabIndex = 1;
			this.textRearSecondary.Text = "1m";
			this.textRearSecondary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textRearPrimary
			// 
			this.textRearPrimary.BackColor = System.Drawing.Color.Maroon;
			this.textRearPrimary.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.textRearPrimary.Font = new System.Drawing.Font("Noto Sans Lisu", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textRearPrimary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.textRearPrimary.Location = new System.Drawing.Point(3, 125);
			this.textRearPrimary.Name = "textRearPrimary";
			this.textRearPrimary.Size = new System.Drawing.Size(49, 25);
			this.textRearPrimary.TabIndex = 1;
			this.textRearPrimary.Text = "1m";
			this.textRearPrimary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textFrontPrimary
			// 
			this.textFrontPrimary.BackColor = System.Drawing.Color.Maroon;
			this.textFrontPrimary.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.textFrontPrimary.Font = new System.Drawing.Font("Noto Sans Lisu", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textFrontPrimary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.textFrontPrimary.Location = new System.Drawing.Point(3, -1);
			this.textFrontPrimary.Name = "textFrontPrimary";
			this.textFrontPrimary.Size = new System.Drawing.Size(49, 25);
			this.textFrontPrimary.TabIndex = 1;
			this.textFrontPrimary.Text = "1m";
			this.textFrontPrimary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textFrontSecondary
			// 
			this.textFrontSecondary.BackColor = System.Drawing.Color.Maroon;
			this.textFrontSecondary.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.textFrontSecondary.Font = new System.Drawing.Font("Noto Sans Lisu", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textFrontSecondary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.textFrontSecondary.Location = new System.Drawing.Point(54, -1);
			this.textFrontSecondary.Name = "textFrontSecondary";
			this.textFrontSecondary.Size = new System.Drawing.Size(49, 25);
			this.textFrontSecondary.TabIndex = 1;
			this.textFrontSecondary.Text = "1m";
			this.textFrontSecondary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// picCompass
			// 
			this.picCompass.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picCompass.Location = new System.Drawing.Point(0, 0);
			this.picCompass.Name = "picCompass";
			this.picCompass.Size = new System.Drawing.Size(112, 100);
			this.picCompass.TabIndex = 2;
			this.picCompass.TabStop = false;
			// 
			// panelCompass
			// 
			this.panelCompass.Controls.Add(this.picCompass);
			this.panelCompass.Location = new System.Drawing.Point(4, 0);
			this.panelCompass.Name = "panelCompass";
			this.panelCompass.Size = new System.Drawing.Size(112, 100);
			this.panelCompass.TabIndex = 3;
			// 
			// textDestinationBearing
			// 
			this.textDestinationBearing.BackColor = System.Drawing.SystemColors.Menu;
			this.textDestinationBearing.Font = new System.Drawing.Font("Noto Sans Lisu", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textDestinationBearing.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.textDestinationBearing.Location = new System.Drawing.Point(3, 102);
			this.textDestinationBearing.Name = "textDestinationBearing";
			this.textDestinationBearing.Size = new System.Drawing.Size(113, 25);
			this.textDestinationBearing.TabIndex = 4;
			this.textDestinationBearing.Text = "360°";
			this.textDestinationBearing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// BotDash
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textDestinationBearing);
			this.Controls.Add(this.panelCompass);
			this.Controls.Add(this.panel1);
			this.Name = "BotDash";
			this.Size = new System.Drawing.Size(233, 154);
			((System.ComponentModel.ISupportInitialize)(this.picTank)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picCompass)).EndInit();
			this.panelCompass.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picTank;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label textRearSecondary;
		private System.Windows.Forms.Label textRearPrimary;
		private System.Windows.Forms.Label textFrontPrimary;
		private System.Windows.Forms.Label textFrontSecondary;
		private System.Windows.Forms.PictureBox picCompass;
		private System.Windows.Forms.Panel panelCompass;
		private System.Windows.Forms.Label textDestinationBearing;
	}
}
