using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KanoopCommon.Database;
using KanoopCommon.Linux;
using Ookii.Dialogs;

namespace KanoopControls.DatabaseCredentials
{
	public partial class DatabaseCredentialsDialogEx : Form
	{
		public SqlDBCredentials		Credentials { get; set; }

		public DatabaseCredentialsDialogEx()
			: this(new SqlDBCredentials()) {}

		bool _initialized;

		public DatabaseCredentialsDialogEx(SqlDBCredentials credentials)
		{
			_initialized = false;
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

			textSSHDBHost.Text = Credentials.Host;
			textSSHDBUser.Text = Credentials.UserName;
			textSSHDBPassword.Text = Credentials.Password;
			textSSHDBSchema.Text = Credentials.Schema;
			textSSHDBDriver.Text = Credentials.Driver;

			textSSHHost.Text = Credentials.SSHCredentials.Host;
			textSSHUser.Text = Credentials.SSHCredentials.UserName;
			textSSHKeyFile.Text = Credentials.SSHCredentials.KeyFile;

			checkTunnel.Checked = Credentials.SSHCredentials.IsTunnel;
			textSSHTunnelHost.Text = Credentials.SSHCredentials.TunnelHost;
			textSSHLocalPort.Text = Credentials.SSHCredentials.LocalTunnelPort.ToString();

			tabControlMain.SelectedTab = Credentials.IsSSH ? tabPageSSH : tabPageStandard;

			textHost.Select();

			textDriver.Items.AddRange(SqlDataSource.AvailableDrivers.ToArray());

			ValidateButtons();

			_initialized = true;
		}

		void ValidateButtons()
		{
			textSSHLocalPort.Enabled = textSSHTunnelHost.Enabled = checkTunnel.Checked;
			if(tabControlMain.SelectedTab == tabPageSSH)
			{
				btnOK.Enabled =	
                    String.IsNullOrEmpty(textSSHDBHost.Text) == false &&
                    String.IsNullOrEmpty(textSSHDBUser.Text) == false &&
                    String.IsNullOrEmpty(textSSHDBPassword.Text) == false &&
                    String.IsNullOrEmpty(textSSHDBSchema.Text) == false &&
                    String.IsNullOrEmpty(textSSHDBDriver.Text) == false &&
                    String.IsNullOrEmpty(textSSHHost.Text) == false &&
                    String.IsNullOrEmpty(textSSHUser.Text) == false &&
                    String.IsNullOrEmpty(textSSHKeyFile.Text) == false;

				if(btnOK.Enabled && checkTunnel.Checked)
				{
					UInt16 port;
					btnOK.Enabled =
					String.IsNullOrEmpty(textSSHTunnelHost.Text) == false &&
                    UInt16.TryParse(textSSHLocalPort.Text, out port) == true;
				}

			}
			else
			{
				btnOK.Enabled =	textHost.Text.Length > 0 &&
								textUser.Text.Length > 0 &&
								textPassword.Text.Length > 0 &&
								textSchema.Text.Length > 0 &&
								textDriver.Text.Length > 0;
			}

		}

		private void OnOKClicked(object sender, EventArgs e)
		{
			if(tabControlMain.SelectedTab == tabPageStandard)
			{
                Credentials.Host = textHost.Text;
                Credentials.UserName = textUser.Text;
                Credentials.Password = textPassword.Text;
                Credentials.Schema = textSchema.Text;
                Credentials.Driver = textDriver.Text;
				Credentials.IsSSH = false;
            }
            else
            {
                Credentials.Host = textSSHDBHost.Text;
                Credentials.UserName = textSSHDBUser.Text;
                Credentials.Password = textSSHDBPassword.Text;
                Credentials.Schema = textSSHDBSchema.Text;
                Credentials.Driver = textSSHDBDriver.Text;
				Credentials.SSHCredentials = new SSHCredentials(textSSHHost.Text, textSSHUser.Text, textSSHKeyFile.Text,
					checkTunnel.Checked, textSSHTunnelHost.Text, UInt16.Parse(textSSHLocalPort.Text), 3306);
				Credentials.IsSSH = true;
            }

            
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

		private void OnBrowseKeyClicked(object sender, EventArgs e)
		{
			VistaOpenFileDialog dlg = new VistaOpenFileDialog();
			dlg.Filter = "All Files (*.*)|*.*";
			dlg.Title = "Find Private Key";
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				textSSHKeyFile.Text = dlg.FileName;
			}

		}

		private void OnTunnelCheckedChanged(object sender, EventArgs e)
		{
			if(_initialized)
			{
				Credentials.SSHCredentials.IsTunnel = checkTunnel.Checked;
				Credentials.SSHCredentials.TunnelHost = textSSHTunnelHost.Text;
				Credentials.SSHCredentials.LocalTunnelPort = Convert.ToUInt16(textSSHLocalPort.Text);
				ValidateButtons();
			}
		}
	}
}
