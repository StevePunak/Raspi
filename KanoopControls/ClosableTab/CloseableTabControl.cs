using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using KanoopCommon.Extensions;
using KanoopCommon.CommonObjects;

namespace KanoopControls.CloseableTab
{
	public delegate bool PreRemoveTab(int indx);

	public partial class CloseableTabControl : TabControl
	{
		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
		private const int TCM_SETMINTABWIDTH = 0x1300 + 49;

		public PreRemoveTab PreRemoveTabPage;

		Bitmap _addNewTabImage;
		Bitmap _closeTabImage;

		bool _startupComplete;

		public CloseableTabControl()
		{
			InitializeComponent();

			SuspendLayout();
			Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			SelectedIndexChanged += new System.EventHandler(OnSelectedIndexChanged);

			MouseDown += OnMouseDown;
			PreRemoveTabPage = null;
			DrawMode = TabDrawMode.OwnerDrawFixed;
			DrawItem += OnDrawItem;
			HandleCreated += OnHandleCreated;
			ControlAdded += OnControlAdded;

			ResumeLayout();

			Assembly myAssembly = Assembly.GetExecutingAssembly();
			List<String> names = new List<string>(myAssembly.GetManifestResourceNames());
			Stream s1 = myAssembly.GetManifestResourceStream("KanoopControls.ClosableTab.addtotab.png");
			_addNewTabImage = new Bitmap(s1);

			Stream s2 = myAssembly.GetManifestResourceStream("KanoopControls.ClosableTab.closetab.png");
			_closeTabImage = new Bitmap(s2);

			_addNewTabImage = _addNewTabImage.Resize(16, 16);
			_closeTabImage = _closeTabImage.Resize(16, 16);

			_startupComplete = false;
		}

		private void OnControlAdded(object sender, ControlEventArgs e)
		{
			if(e.Control.Text.Length == 0)
			{
				Controls.Remove(e.Control);
			}
			else
			{
				e.Control.Text += "          ";
				Size textSize = TextRenderer.MeasureText(e.Control.Text, Font);
				SendMessage(e.Control.Handle, 140 /*textSize.Width - 18*/, IntPtr.Zero, (IntPtr)16);
			}


		}

		protected void OnDrawItem(Object sender, DrawItemEventArgs args)
		{
			try
			{
				TabPage tabPage = TabPages[args.Index];
				Rectangle tabRect = GetTabRect(args.Index);
				tabRect.Inflate(-2, -2);
				if(args.Index == TabCount - /*1*/999) // Add button to the last TabPage only
				{
					args.Graphics.DrawImage(_addNewTabImage,
						tabRect.Left + (tabRect.Width - _closeTabImage.Width) / 2,
						tabRect.Top + (tabRect.Height - _closeTabImage.Height) / 2);
				}
				else // draw Close button to all other TabPages
				{
					args.Graphics.DrawImage(_closeTabImage,
						(tabRect.Right - _closeTabImage.Width),
						tabRect.Top + (tabRect.Height - _closeTabImage.Height) / 2);
					TextRenderer.DrawText(args.Graphics, tabPage.Text, tabPage.Font,
						tabRect, tabPage.ForeColor, TextFormatFlags.Left);
				}
			}
			catch(Exception e)
			{
				throw new CommonException("Error loading images - {0}", e.Message);
			}

		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			for(int i = 0;i < TabPages.Count;i++)
			{
				Rectangle tabRect = this.GetTabRect(i);
				tabRect.Inflate(-2, -2);
				Rectangle imageRect = new Rectangle((tabRect.Right - _closeTabImage.Width),
													tabRect.Top + (tabRect.Height - _closeTabImage.Height) / 2,
													_closeTabImage.Width,
													_closeTabImage.Height);
				if(imageRect.Contains(e.Location))
				{
					TabPages.RemoveAt(i);
					break;
				}
			}
		}

		private void OnHandleCreated(object sender, EventArgs e)
		{
			if(_startupComplete == false)
			{
				DoStartupCleanup();
			}

			SendMessage(Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)16);
		}

		private void OnSelectedIndexChanged(object sender, EventArgs e)
		{
			// If the last TabPage is selected then Create a new TabPage
			if(SelectedIndex == TabPages.Count - 1)
				CreateTabPage();
		}

		private void CreateTabPage()
		{
		}

		private void DoStartupCleanup()
		{
			foreach(TabPage page in TabPages)
			{
				TabPages.Remove(page);
			}
			_startupComplete = true;
		}
	}
}
