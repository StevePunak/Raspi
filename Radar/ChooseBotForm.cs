using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Radar
{
	public partial class ChooseBotForm : Form
	{
		public String Host { get; set; }

		public ChooseBotForm()
		{
			InitializeComponent();
		}

		private void OnFormLoad(object sender, EventArgs args)
		{
			if(String.IsNullOrEmpty(Host) && Program.Config.MqttPublicHost == null)
			{
				Program.Config.MqttPublicHost = "raspi";
				Program.Config.Save();
			}

			try
			{
				textHost.Text = Host;
				textHost.Select();
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
				Close();
			}
		}

		private void OnOKClicked(object sender, EventArgs e)
		{
			Host = textHost.Text;
			Program.Config.MqttPublicHost = textHost.Text;
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
