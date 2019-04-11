namespace KanoopControls.DatabaseCredentials
{
	partial class DatabaseCredentialsDialogEx
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
			if (disposing && (components != null))
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
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabPageStandard = new System.Windows.Forms.TabPage();
			this.textDriver = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textSchema = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textUser = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textHost = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPageSSH = new System.Windows.Forms.TabPage();
			this.checkTunnel = new System.Windows.Forms.CheckBox();
			this.btnBrowseKey = new System.Windows.Forms.Button();
			this.textSSHKeyFile = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.textSSHUser = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.textSSHHost = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textSSHDBDriver = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textSSHDBSchema = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textSSHDBPassword = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textSSHDBUser = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textSSHLocalPort = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.textSSHTunnelHost = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textSSHDBHost = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.tabControlMain.SuspendLayout();
			this.tabPageStandard.SuspendLayout();
			this.tabPageSSH.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPageStandard);
			this.tabControlMain.Controls.Add(this.tabPageSSH);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.Location = new System.Drawing.Point(0, 0);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(432, 375);
			this.tabControlMain.TabIndex = 0;
			// 
			// tabPageStandard
			// 
			this.tabPageStandard.Controls.Add(this.textDriver);
			this.tabPageStandard.Controls.Add(this.label5);
			this.tabPageStandard.Controls.Add(this.textSchema);
			this.tabPageStandard.Controls.Add(this.label4);
			this.tabPageStandard.Controls.Add(this.textPassword);
			this.tabPageStandard.Controls.Add(this.label3);
			this.tabPageStandard.Controls.Add(this.textUser);
			this.tabPageStandard.Controls.Add(this.label2);
			this.tabPageStandard.Controls.Add(this.textHost);
			this.tabPageStandard.Controls.Add(this.label1);
			this.tabPageStandard.Location = new System.Drawing.Point(4, 22);
			this.tabPageStandard.Name = "tabPageStandard";
			this.tabPageStandard.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageStandard.Size = new System.Drawing.Size(424, 349);
			this.tabPageStandard.TabIndex = 0;
			this.tabPageStandard.Text = "Standard MySQL";
			this.tabPageStandard.UseVisualStyleBackColor = true;
			// 
			// textDriver
			// 
			this.textDriver.FormattingEnabled = true;
			this.textDriver.Location = new System.Drawing.Point(87, 131);
			this.textDriver.Name = "textDriver";
			this.textDriver.Size = new System.Drawing.Size(201, 21);
			this.textDriver.TabIndex = 16;
			this.textDriver.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(17, 134);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Driver";
			// 
			// textSchema
			// 
			this.textSchema.Location = new System.Drawing.Point(87, 103);
			this.textSchema.Name = "textSchema";
			this.textSchema.Size = new System.Drawing.Size(201, 22);
			this.textSchema.TabIndex = 15;
			this.textSchema.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(17, 106);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(46, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Schema";
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(87, 75);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(201, 22);
			this.textPassword.TabIndex = 14;
			this.textPassword.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 78);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Password";
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(87, 47);
			this.textUser.Name = "textUser";
			this.textUser.Size = new System.Drawing.Size(201, 22);
			this.textUser.TabIndex = 11;
			this.textUser.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(17, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "User Name";
			// 
			// textHost
			// 
			this.textHost.Location = new System.Drawing.Point(87, 19);
			this.textHost.Name = "textHost";
			this.textHost.Size = new System.Drawing.Size(201, 22);
			this.textHost.TabIndex = 7;
			this.textHost.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 13);
			this.label1.TabIndex = 13;
			this.label1.Text = "Host";
			// 
			// tabPageSSH
			// 
			this.tabPageSSH.Controls.Add(this.checkTunnel);
			this.tabPageSSH.Controls.Add(this.btnBrowseKey);
			this.tabPageSSH.Controls.Add(this.textSSHKeyFile);
			this.tabPageSSH.Controls.Add(this.label11);
			this.tabPageSSH.Controls.Add(this.textSSHUser);
			this.tabPageSSH.Controls.Add(this.label12);
			this.tabPageSSH.Controls.Add(this.textSSHHost);
			this.tabPageSSH.Controls.Add(this.label13);
			this.tabPageSSH.Controls.Add(this.textSSHDBDriver);
			this.tabPageSSH.Controls.Add(this.label6);
			this.tabPageSSH.Controls.Add(this.textSSHDBSchema);
			this.tabPageSSH.Controls.Add(this.label7);
			this.tabPageSSH.Controls.Add(this.textSSHDBPassword);
			this.tabPageSSH.Controls.Add(this.label8);
			this.tabPageSSH.Controls.Add(this.textSSHDBUser);
			this.tabPageSSH.Controls.Add(this.label9);
			this.tabPageSSH.Controls.Add(this.textSSHLocalPort);
			this.tabPageSSH.Controls.Add(this.label15);
			this.tabPageSSH.Controls.Add(this.textSSHTunnelHost);
			this.tabPageSSH.Controls.Add(this.label14);
			this.tabPageSSH.Controls.Add(this.textSSHDBHost);
			this.tabPageSSH.Controls.Add(this.label10);
			this.tabPageSSH.Location = new System.Drawing.Point(4, 22);
			this.tabPageSSH.Name = "tabPageSSH";
			this.tabPageSSH.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageSSH.Size = new System.Drawing.Size(424, 349);
			this.tabPageSSH.TabIndex = 1;
			this.tabPageSSH.Text = "MySQL via SSH";
			this.tabPageSSH.UseVisualStyleBackColor = true;
			this.tabPageSSH.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// checkTunnel
			// 
			this.checkTunnel.AutoSize = true;
			this.checkTunnel.Location = new System.Drawing.Point(316, 93);
			this.checkTunnel.Name = "checkTunnel";
			this.checkTunnel.Size = new System.Drawing.Size(61, 17);
			this.checkTunnel.TabIndex = 34;
			this.checkTunnel.Text = "Tunnel";
			this.checkTunnel.UseVisualStyleBackColor = true;
			this.checkTunnel.CheckedChanged += new System.EventHandler(this.OnTunnelCheckedChanged);
			// 
			// btnBrowseKey
			// 
			this.btnBrowseKey.Location = new System.Drawing.Point(316, 60);
			this.btnBrowseKey.Name = "btnBrowseKey";
			this.btnBrowseKey.Size = new System.Drawing.Size(28, 23);
			this.btnBrowseKey.TabIndex = 33;
			this.btnBrowseKey.Text = "...";
			this.btnBrowseKey.UseVisualStyleBackColor = true;
			this.btnBrowseKey.Click += new System.EventHandler(this.OnBrowseKeyClicked);
			// 
			// textSSHKeyFile
			// 
			this.textSSHKeyFile.Location = new System.Drawing.Point(95, 62);
			this.textSSHKeyFile.Name = "textSSHKeyFile";
			this.textSSHKeyFile.Size = new System.Drawing.Size(201, 22);
			this.textSSHKeyFile.TabIndex = 2;
			this.textSSHKeyFile.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(9, 65);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(45, 13);
			this.label11.TabIndex = 28;
			this.label11.Text = "Key File";
			// 
			// textSSHUser
			// 
			this.textSSHUser.Location = new System.Drawing.Point(95, 34);
			this.textSSHUser.Name = "textSSHUser";
			this.textSSHUser.Size = new System.Drawing.Size(201, 22);
			this.textSSHUser.TabIndex = 1;
			this.textSSHUser.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(9, 37);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(53, 13);
			this.label12.TabIndex = 30;
			this.label12.Text = "SSH User";
			// 
			// textSSHHost
			// 
			this.textSSHHost.Location = new System.Drawing.Point(95, 6);
			this.textSSHHost.Name = "textSSHHost";
			this.textSSHHost.Size = new System.Drawing.Size(201, 22);
			this.textSSHHost.TabIndex = 0;
			this.textSSHHost.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(9, 9);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(54, 13);
			this.label13.TabIndex = 31;
			this.label13.Text = "SSH Host";
			// 
			// textSSHDBDriver
			// 
			this.textSSHDBDriver.FormattingEnabled = true;
			this.textSSHDBDriver.Location = new System.Drawing.Point(95, 275);
			this.textSSHDBDriver.Name = "textSSHDBDriver";
			this.textSSHDBDriver.Size = new System.Drawing.Size(201, 21);
			this.textSSHDBDriver.TabIndex = 9;
			this.textSSHDBDriver.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(9, 278);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(37, 13);
			this.label6.TabIndex = 18;
			this.label6.Text = "Driver";
			// 
			// textSSHDBSchema
			// 
			this.textSSHDBSchema.Location = new System.Drawing.Point(95, 247);
			this.textSSHDBSchema.Name = "textSSHDBSchema";
			this.textSSHDBSchema.Size = new System.Drawing.Size(201, 22);
			this.textSSHDBSchema.TabIndex = 8;
			this.textSSHDBSchema.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(9, 250);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(46, 13);
			this.label7.TabIndex = 19;
			this.label7.Text = "Schema";
			// 
			// textSSHDBPassword
			// 
			this.textSSHDBPassword.Location = new System.Drawing.Point(95, 219);
			this.textSSHDBPassword.Name = "textSSHDBPassword";
			this.textSSHDBPassword.PasswordChar = '*';
			this.textSSHDBPassword.Size = new System.Drawing.Size(201, 22);
			this.textSSHDBPassword.TabIndex = 7;
			this.textSSHDBPassword.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(9, 222);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(56, 13);
			this.label8.TabIndex = 20;
			this.label8.Text = "Password";
			// 
			// textSSHDBUser
			// 
			this.textSSHDBUser.Location = new System.Drawing.Point(95, 191);
			this.textSSHDBUser.Name = "textSSHDBUser";
			this.textSSHDBUser.Size = new System.Drawing.Size(201, 22);
			this.textSSHDBUser.TabIndex = 6;
			this.textSSHDBUser.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(9, 194);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(62, 13);
			this.label9.TabIndex = 22;
			this.label9.Text = "User Name";
			// 
			// textSSHLocalPort
			// 
			this.textSSHLocalPort.Location = new System.Drawing.Point(95, 118);
			this.textSSHLocalPort.Name = "textSSHLocalPort";
			this.textSSHLocalPort.Size = new System.Drawing.Size(201, 22);
			this.textSSHLocalPort.TabIndex = 4;
			this.textSSHLocalPort.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(9, 121);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(57, 13);
			this.label15.TabIndex = 23;
			this.label15.Text = "Local Port";
			// 
			// textSSHTunnelHost
			// 
			this.textSSHTunnelHost.Location = new System.Drawing.Point(95, 90);
			this.textSSHTunnelHost.Name = "textSSHTunnelHost";
			this.textSSHTunnelHost.Size = new System.Drawing.Size(201, 22);
			this.textSSHTunnelHost.TabIndex = 3;
			this.textSSHTunnelHost.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(9, 93);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(83, 13);
			this.label14.TabIndex = 23;
			this.label14.Text = "Tunnel to Host";
			// 
			// textSSHDBHost
			// 
			this.textSSHDBHost.Location = new System.Drawing.Point(95, 163);
			this.textSSHDBHost.Name = "textSSHDBHost";
			this.textSSHDBHost.Size = new System.Drawing.Size(201, 22);
			this.textSSHDBHost.TabIndex = 5;
			this.textSSHDBHost.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(9, 166);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(31, 13);
			this.label10.TabIndex = 23;
			this.label10.Text = "Host";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tabControlMain);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.btnCancel);
			this.splitContainer1.Panel2.Controls.Add(this.btnOK);
			this.splitContainer1.Size = new System.Drawing.Size(523, 375);
			this.splitContainer1.SplitterDistance = 432;
			this.splitContainer1.TabIndex = 1;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(5, 41);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 20;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.OnCancelClicked);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(5, 12);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 19;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.OnOKClicked);
			// 
			// DatabaseCredentialsDialogEx
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(523, 375);
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "DatabaseCredentialsDialogEx";
			this.Text = "Database Credentials";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.tabControlMain.ResumeLayout(false);
			this.tabPageStandard.ResumeLayout(false);
			this.tabPageStandard.PerformLayout();
			this.tabPageSSH.ResumeLayout(false);
			this.tabPageSSH.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.TabPage tabPageStandard;
		private System.Windows.Forms.TabPage tabPageSSH;
		private System.Windows.Forms.ComboBox textDriver;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textSchema;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textHost;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textSSHKeyFile;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textSSHUser;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textSSHHost;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ComboBox textSSHDBDriver;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textSSHDBSchema;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textSSHDBPassword;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textSSHDBUser;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textSSHDBHost;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnBrowseKey;
		private System.Windows.Forms.CheckBox checkTunnel;
		private System.Windows.Forms.TextBox textSSHLocalPort;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textSSHTunnelHost;
		private System.Windows.Forms.Label label14;
	}
}