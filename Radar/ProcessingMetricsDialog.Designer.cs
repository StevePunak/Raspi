namespace Radar
{
	partial class ProcessingMetricsDialog
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textThresholdLinking = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textCannyThreshold = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textHoughThreshold = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textThetaRes = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textRhoRes = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.textMinLandmarkSegment = new System.Windows.Forms.TextBox();
			this.textMinConnectLength = new System.Windows.Forms.TextBox();
			this.textMaxConnectLength = new System.Windows.Forms.TextBox();
			this.textBearingSlack = new System.Windows.Forms.TextBox();
			this.textMinLandmarkSeparation = new System.Windows.Forms.TextBox();
			this.textRightAngleSlack = new System.Windows.Forms.TextBox();
			this.textLinePathWidth = new System.Windows.Forms.TextBox();
			this.btnDefaults = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textThresholdLinking);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textCannyThreshold);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(23, 23);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(291, 80);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Canny";
			// 
			// textThresholdLinking
			// 
			this.textThresholdLinking.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textThresholdLinking.Location = new System.Drawing.Point(176, 47);
			this.textThresholdLinking.Name = "textThresholdLinking";
			this.textThresholdLinking.Size = new System.Drawing.Size(100, 22);
			this.textThresholdLinking.TabIndex = 1;
			this.textThresholdLinking.TextChanged += new System.EventHandler(this.OnInputFieldChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(99, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Threshold Linking";
			// 
			// textCannyThreshold
			// 
			this.textCannyThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textCannyThreshold.Location = new System.Drawing.Point(176, 19);
			this.textCannyThreshold.Name = "textCannyThreshold";
			this.textCannyThreshold.Size = new System.Drawing.Size(100, 22);
			this.textCannyThreshold.TabIndex = 0;
			this.textCannyThreshold.TextChanged += new System.EventHandler(this.OnInputFieldChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(93, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Canny Threshold";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textHoughThreshold);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.textThetaRes);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.textRhoRes);
			this.groupBox2.Location = new System.Drawing.Point(23, 129);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(291, 100);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Hough";
			// 
			// textHoughThreshold
			// 
			this.textHoughThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textHoughThreshold.Location = new System.Drawing.Point(176, 71);
			this.textHoughThreshold.Name = "textHoughThreshold";
			this.textHoughThreshold.Size = new System.Drawing.Size(100, 22);
			this.textHoughThreshold.TabIndex = 2;
			this.textHoughThreshold.TextChanged += new System.EventHandler(this.OnInputFieldChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(7, 74);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(97, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "Hough Threshold";
			// 
			// textThetaRes
			// 
			this.textThetaRes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textThetaRes.Location = new System.Drawing.Point(176, 43);
			this.textThetaRes.Name = "textThetaRes";
			this.textThetaRes.Size = new System.Drawing.Size(100, 22);
			this.textThetaRes.TabIndex = 1;
			this.textThetaRes.TextChanged += new System.EventHandler(this.OnInputFieldChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 46);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Theta Res";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 18);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(49, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Rho Res";
			// 
			// textRhoRes
			// 
			this.textRhoRes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textRhoRes.Location = new System.Drawing.Point(176, 15);
			this.textRhoRes.Name = "textRhoRes";
			this.textRhoRes.Size = new System.Drawing.Size(100, 22);
			this.textRhoRes.TabIndex = 0;
			this.textRhoRes.TextChanged += new System.EventHandler(this.OnInputFieldChanged);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(333, 13);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 45);
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.OnOKClicked);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.textLinePathWidth);
			this.groupBox3.Controls.Add(this.textBearingSlack);
			this.groupBox3.Controls.Add(this.textMinLandmarkSeparation);
			this.groupBox3.Controls.Add(this.textRightAngleSlack);
			this.groupBox3.Controls.Add(this.textMaxConnectLength);
			this.groupBox3.Controls.Add(this.label12);
			this.groupBox3.Controls.Add(this.textMinConnectLength);
			this.groupBox3.Controls.Add(this.label11);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Controls.Add(this.textMinLandmarkSegment);
			this.groupBox3.Controls.Add(this.label9);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Location = new System.Drawing.Point(23, 236);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(291, 222);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Corners";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(8, 77);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(113, 13);
			this.label6.TabIndex = 1;
			this.label6.Text = "Max Connect Length";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(8, 49);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(112, 13);
			this.label7.TabIndex = 2;
			this.label7.Text = "Min Connect Length";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(6, 21);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(128, 13);
			this.label8.TabIndex = 3;
			this.label8.Text = "Min Landmark Segment";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(8, 105);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(101, 13);
			this.label9.TabIndex = 3;
			this.label9.Text = "Right Angle Slack°";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(8, 133);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(139, 13);
			this.label10.TabIndex = 2;
			this.label10.Text = "Min Landmark Separation";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(8, 161);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(76, 13);
			this.label11.TabIndex = 1;
			this.label11.Text = "Bearing Slack";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(8, 189);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(89, 13);
			this.label12.TabIndex = 1;
			this.label12.Text = "Line Path Width";
			// 
			// textMinLandmarkSegment
			// 
			this.textMinLandmarkSegment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textMinLandmarkSegment.Location = new System.Drawing.Point(176, 18);
			this.textMinLandmarkSegment.Name = "textMinLandmarkSegment";
			this.textMinLandmarkSegment.Size = new System.Drawing.Size(100, 22);
			this.textMinLandmarkSegment.TabIndex = 0;
			this.textMinLandmarkSegment.TextChanged += new System.EventHandler(this.OnInputFieldChanged);
			// 
			// textMinConnectLength
			// 
			this.textMinConnectLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textMinConnectLength.Location = new System.Drawing.Point(176, 46);
			this.textMinConnectLength.Name = "textMinConnectLength";
			this.textMinConnectLength.Size = new System.Drawing.Size(100, 22);
			this.textMinConnectLength.TabIndex = 1;
			this.textMinConnectLength.TextChanged += new System.EventHandler(this.OnInputFieldChanged);
			// 
			// textMaxConnectLength
			// 
			this.textMaxConnectLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textMaxConnectLength.Location = new System.Drawing.Point(176, 74);
			this.textMaxConnectLength.Name = "textMaxConnectLength";
			this.textMaxConnectLength.Size = new System.Drawing.Size(100, 22);
			this.textMaxConnectLength.TabIndex = 2;
			this.textMaxConnectLength.TextChanged += new System.EventHandler(this.OnInputFieldChanged);
			// 
			// textBearingSlack
			// 
			this.textBearingSlack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBearingSlack.Location = new System.Drawing.Point(176, 158);
			this.textBearingSlack.Name = "textBearingSlack";
			this.textBearingSlack.Size = new System.Drawing.Size(100, 22);
			this.textBearingSlack.TabIndex = 6;
			// 
			// textMinLandmarkSeparation
			// 
			this.textMinLandmarkSeparation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textMinLandmarkSeparation.Location = new System.Drawing.Point(176, 130);
			this.textMinLandmarkSeparation.Name = "textMinLandmarkSeparation";
			this.textMinLandmarkSeparation.Size = new System.Drawing.Size(100, 22);
			this.textMinLandmarkSeparation.TabIndex = 5;
			// 
			// textRightAngleSlack
			// 
			this.textRightAngleSlack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textRightAngleSlack.Location = new System.Drawing.Point(176, 102);
			this.textRightAngleSlack.Name = "textRightAngleSlack";
			this.textRightAngleSlack.Size = new System.Drawing.Size(100, 22);
			this.textRightAngleSlack.TabIndex = 4;
			// 
			// textLinePathWidth
			// 
			this.textLinePathWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textLinePathWidth.Location = new System.Drawing.Point(176, 186);
			this.textLinePathWidth.Name = "textLinePathWidth";
			this.textLinePathWidth.Size = new System.Drawing.Size(100, 22);
			this.textLinePathWidth.TabIndex = 6;
			// 
			// btnDefaults
			// 
			this.btnDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDefaults.Location = new System.Drawing.Point(333, 64);
			this.btnDefaults.Name = "btnDefaults";
			this.btnDefaults.Size = new System.Drawing.Size(75, 45);
			this.btnDefaults.TabIndex = 0;
			this.btnDefaults.Text = "&Defaults";
			this.btnDefaults.UseVisualStyleBackColor = true;
			this.btnDefaults.Click += new System.EventHandler(this.OnDefaultsClicked);
			// 
			// ProcessingMetricsDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(420, 472);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.btnDefaults);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ProcessingMetricsDialog";
			this.Text = "Processing Metrics";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textCannyThreshold;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textThresholdLinking;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textHoughThreshold;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textThetaRes;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textRhoRes;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textLinePathWidth;
		private System.Windows.Forms.TextBox textBearingSlack;
		private System.Windows.Forms.TextBox textMinLandmarkSeparation;
		private System.Windows.Forms.TextBox textRightAngleSlack;
		private System.Windows.Forms.TextBox textMaxConnectLength;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textMinConnectLength;
		private System.Windows.Forms.TextBox textMinLandmarkSegment;
		private System.Windows.Forms.Button btnDefaults;
	}
}