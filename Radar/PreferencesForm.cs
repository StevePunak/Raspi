using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RaspiCommon.Devices.GamePads;

namespace Radar
{
	public partial class PreferencesForm : Form
	{
		public PreferencesForm()
		{
			InitializeComponent();
		}

		private void PreferencesForm_Load(object sender, EventArgs args)
		{
			
			try
			{
				foreach(GamePadType t in Enum.GetValues(typeof(GamePadType)))
				{
					listGamePads.Items.Add(t);
				}
				listGamePads.SelectedItem = Program.Config.GamePadType;
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
				Close();
			}
		}

		private void OnSaveClicked(object sender, EventArgs e)
		{
			if(listGamePads.SelectedItem != null)
				Program.Config.GamePadType = (GamePadType)listGamePads.SelectedItem;
			Program.Config.Save();
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
