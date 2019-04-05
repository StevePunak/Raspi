namespace KanoopControls.DatabaseCredentials
{
	partial class DatabaseCredentialsDialog
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
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textHost = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textUser = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textSchema = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textDriver = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(301, 12);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.OnOKClicked);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(301, 41);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.OnCancelClicked);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Host";
			// 
			// textHost
			// 
			this.textHost.Location = new System.Drawing.Point(83, 10);
			this.textHost.Name = "textHost";
			this.textHost.Size = new System.Drawing.Size(201, 22);
			this.textHost.TabIndex = 0;
			this.textHost.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "User Name";
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(83, 38);
			this.textUser.Name = "textUser";
			this.textUser.Size = new System.Drawing.Size(201, 22);
			this.textUser.TabIndex = 1;
			this.textUser.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 69);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Password";
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(83, 66);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(201, 22);
			this.textPassword.TabIndex = 2;
			this.textPassword.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 97);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(46, 13);
			this.label4.TabIndex = 1;
			this.label4.Text = "Schema";
			// 
			// textSchema
			// 
			this.textSchema.Location = new System.Drawing.Point(83, 94);
			this.textSchema.Name = "textSchema";
			this.textSchema.Size = new System.Drawing.Size(201, 22);
			this.textSchema.TabIndex = 3;
			this.textSchema.TextChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(13, 125);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "Driver";
			// 
			// textDriver
			// 
			this.textDriver.FormattingEnabled = true;
			this.textDriver.Location = new System.Drawing.Point(83, 122);
			this.textDriver.Name = "textDriver";
			this.textDriver.Size = new System.Drawing.Size(201, 21);
			this.textDriver.TabIndex = 4;
			this.textDriver.SelectedIndexChanged += new System.EventHandler(this.OnTextFieldChanged);
			// 
			// DatabaseCredentialsDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(388, 161);
			this.ControlBox = false;
			this.Controls.Add(this.textDriver);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textSchema);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textHost);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DatabaseCredentialsDialog";
			this.Text = "Enter Database Credentials";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textHost;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textSchema;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox textDriver;
	}
}