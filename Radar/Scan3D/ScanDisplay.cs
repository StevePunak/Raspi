using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenGL;
using OpenTK;

namespace Radar.Scan3D
{
	public partial class ScanDisplay : Form
	{
		public ScanDisplay()
		{
			InitializeComponent();
		}

		private void OnFormLoad(object sender, EventArgs args)
		{
			try
			{
				new TestWindow().Run(30);
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
				Close();
			}

		}
	}
}
