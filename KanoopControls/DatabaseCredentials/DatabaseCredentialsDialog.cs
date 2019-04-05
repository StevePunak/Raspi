using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KanoopCommon.Database;

namespace KanoopControls.DatabaseCredentials
{
	public partial class DatabaseCredentialsDialog : Form
	{
		public SqlDBCredentials		Credentials { get; set; }

		public DatabaseCredentialsDialog()
			: this(new SqlDBCredentials()) {}

		public DatabaseCredentialsDialog(SqlDBCredentials credentials)
		{
			Credentials = credentials.Clone();

			InitializeComponent();
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			textHost.Text = Credentials.Host;
			textUser.Text = Credentials.UserName;
			textPassword.Text = Credentials.Password;
			textSchema.Text = Credentials.Schema;
			textDriver.Text = Credentials.Driver;

			textHost.Select();

			textDriver.Items.AddRange(SqlDataSource.AvailableDrivers.ToArray());

			ValidateButtons();
		}

		void ValidateButtons()
		{
			btnOK.Enabled =	textHost.Text.Length > 0 &&
							textUser.Text.Length > 0 &&
							textPassword.Text.Length > 0 &&
							textSchema.Text.Length > 0 &&
							textDriver.Text.Length > 0;

		}

		private void OnOKClicked(object sender, EventArgs e)
		{
			Credentials.Host = textHost.Text;
			Credentials.UserName = textUser.Text;
			Credentials.Password = textPassword.Text;
			Credentials.Schema = textSchema.Text;
			Credentials.Driver = textDriver.Text;

			DialogResult = DialogResult.OK;
			Close();
		}

		private void OnCancelClicked(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void OnTextFieldChanged(object sender, EventArgs e)
		{
			ValidateButtons();
		}
	}
}
