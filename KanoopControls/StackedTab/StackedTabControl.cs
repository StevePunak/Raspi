using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KanoopControls.StackedTab
{
	public partial class StackedTabControl : TabControl
	{
		public StackedTabControl()
		{
			InitializeComponent();
		}

		protected override void WndProc(ref Message m) 
		{
			// Hide tabs by trapping the TCM_ADJUSTRECT message
			if (m.Msg == 0x1328 && !DesignMode)
			{
				m.Result = (IntPtr)1;
			}
			else
			{
				base.WndProc(ref m);
			}
		}

	}
}
