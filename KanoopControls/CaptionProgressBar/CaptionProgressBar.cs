using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KanoopControls.CaptionProgressBar
{
	public partial class CaptionProgressBar : ProgressBar
	{
		public enum ProgressBarDisplayText
		{
			Percentage,
			CustomText
		}

		public CaptionProgressBar()
		{
			InitializeComponent();
		}


		//Property to set to decide whether to print a % or Text
		public ProgressBarDisplayText DisplayStyle { get; set; }

	    //Property to hold the custom text
		private string m_CustomText;
	    public string CustomText 
		{
			get { return m_CustomText; }
			set 
			{
				m_CustomText = value;
				Invalidate();
			}
		}

		//protected override void OnPaint(PaintEventArgs e)
		//{
		//	base.OnPaint(e);
		//	int m_Percent = Convert.ToInt32((Convert.ToDouble(Value) / Convert.ToDouble(Maximum)) * 100);
		//	dynamic flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis;

		//	using(Graphics g = e.Graphics)
		//	{
		//		using (Brush textBrush = new SolidBrush(ForeColor)) 
		//		{
		//			switch (DisplayStyle) 
		//			{
		//				case ProgressBarDisplayText.CustomText:
		//					TextRenderer.DrawText(g, CustomText, new Font("Arial", Convert.ToSingle(8.25), FontStyle.Regular), new Rectangle(0, 0, this.Width, this.Height), Color.Black, flags);
		//					break;

		//				case ProgressBarDisplayText.Percentage:
		//					TextRenderer.DrawText(g, string.Format("{0}%", m_Percent), new Font("Arial", Convert.ToSingle(8.25), FontStyle.Regular), new Rectangle(0, 0, this.Width, this.Height), Color.Black, flags);
		//					break;
		//			}

		//		}
		//	}

		//}


		private const int WM_PAINT = 0x000F;
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			switch (m.Msg) 
			{
				case WM_PAINT:
					int m_Percent = Convert.ToInt32((Convert.ToDouble(Value) / Convert.ToDouble(Maximum)) * 100);
					dynamic flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis;

					using (Graphics g = Graphics.FromHwnd(Handle)) 
					{
						using (Brush textBrush = new SolidBrush(ForeColor)) 
						{

							switch (DisplayStyle) 
							{
								case ProgressBarDisplayText.CustomText:
									TextRenderer.DrawText(g, CustomText, new Font("Arial", Convert.ToSingle(8.25), FontStyle.Regular), new Rectangle(0, 0, this.Width, this.Height), Color.Black, flags);
									break;
								case ProgressBarDisplayText.Percentage:
									TextRenderer.DrawText(g, string.Format("{0}%", m_Percent), new Font("Arial", Convert.ToSingle(8.25), FontStyle.Regular), new Rectangle(0, 0, this.Width, this.Height), Color.Black, flags);
									break;
							}

						}
					}

					break;
			}
		}
	}
}
