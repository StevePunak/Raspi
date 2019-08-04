using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using RaspiCommon.Extensions;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using Emgu.CV.Util;
using KanoopCommon.Logging;

namespace TrackBotCommon.Controls
{
	public partial class BotDash : UserControl
	{
		static readonly Color StandardRangeColor = Color.Maroon;
		static readonly Color CautionRangeColor = Color.Red;
		const Double CautionRange = .3;
		const Double CloseRange = .8;

		public Double Bearing { get; set; }
		public Double MagBearing { get; set; }
		public Double DestinationBearing { get; set; }
		public Double FrontPrimaryRange { get; set; }
		public Double FrontSecondaryRange { get; set; }
		public Double RearPrimaryRange { get; set; }
		public Double RearSecondaryRange { get; set; }

		Double MinimumFrontRange { get { return Math.Min(FrontPrimaryRange, FrontSecondaryRange); } }
		Double MinimumRearRange { get { return Math.Min(RearPrimaryRange, RearSecondaryRange); } }

		public Mat TankBitmap { get; set; }

		public BotDash()
		{
			InitializeComponent();
		}

		public void Redraw()
		{
			Mat bitmap = MakeCompassBitmap();
			picCompass.BackgroundImage = bitmap.Bitmap;

			DrawRanges();
			DrawTank();
		}

		void DrawTank()
		{
			Mat bitmap = new Mat(picTank.Size, DepthType.Cv8U, 3);

			PointD center = bitmap.Center();

			bitmap.SetTo(Color.Black.ToMCvScalar());

			Mat tank = TankBitmap.Scale(2);
			bitmap.DrawCenteredImage(tank, center);

			// draw ranging lines
			{
				List<Double> radii = GetRangeRadii(tank.Height / 2, center.Y - 2, MinimumFrontRange);
				foreach(Double radius in radii)
				{
					bitmap.DrawEllipse(center, new Size((int)radius, (int)radius), 340, 20, Color.Red);
				}
			}
			{
				List<Double> radii = GetRangeRadii(tank.Height / 2, center.Y - 2, MinimumRearRange);
				foreach(Double radius in radii)
				{
					bitmap.DrawEllipse(center, new Size((int)radius, (int)radius), 160, 200, Color.Red);
				}
			}
			picTank.BackgroundImage = bitmap.Bitmap;
		}

		List<Double> GetRangeRadii(Double min, Double max, Double range)
		{
			Double centerRadius = min + ((max - min) / 2);

			int rangingLines = range <= CautionRange ? 3 : range <= CloseRange ? 2 : 1;
			List<Double> radii = null;
			switch(rangingLines)
			{
				case 1:
					radii = new List<double>() { min, centerRadius, max };
					break;
				case 2:
					radii = new List<double>() { min + 10, centerRadius, max - 10 };
					break;
				case 3:
					radii = new List<double>() { centerRadius - 3, centerRadius - 6, centerRadius, centerRadius + 3, centerRadius + 6 };
					break;
			}
			return radii;
		}

		void DrawRanges()
		{
			WriteRange(textFrontPrimary, FrontPrimaryRange);
			WriteRange(textRearPrimary, RearPrimaryRange);
			WriteRange(textFrontSecondary, FrontSecondaryRange);
			WriteRange(textRearSecondary, RearSecondaryRange);
		}

		void WriteRange(Control textBox, Double range)
		{
			String text = String.Format("{0:0.00}m", range);
			textBox.Text = text;
			textBox.BackColor = range <= CautionRange ? CautionRangeColor : StandardRangeColor; 
		}

		Mat MakeCompassBitmap()
		{
			Mat bitmap = new Mat(picCompass.Size, DepthType.Cv8U, 3);

			PointD center = bitmap.Center();

			bitmap.SetTo(Color.DarkGreen.ToMCvScalar());

			String bearingString = String.Format("{0:0.0}", Bearing);
			bitmap.DrawCenteredText(bearingString, FontFace.HersheyPlain, Color.White, center, 1, 2);
			bitmap.DrawCross(center, 4, Color.White);

			PointD magTextPoint = new PointD(center); magTextPoint.Y += 20;
			String magString = String.Format("mag {0:0.0}", MagBearing);
			bitmap.DrawCenteredText(magString, FontFace.HersheyPlain, Color.Gray, magTextPoint, 1, .75);

			Double radius = bitmap.Size.MinimumSize().Height / 2 - 1;
			bitmap.DrawCircle(new Circle(center, radius), Color.White, 1);

			// tick marks
			for(int angle = 0;angle < 360;angle += 5)
			{
				int width = (angle % 45 == 0) ? 2 : 1;
				int length = (angle % 90 == 0) ? 10 : 3;
				Line line = new Line(center.GetPointAt(angle, radius - length), center.GetPointAt(angle, radius - 1));
				bitmap.DrawLine(line, Color.White, width);
			}

			// primary compass pointer
			{
				PointDList polygon = new PointDList()
				{
					center.GetPointAt(Bearing, radius - 1),
					center.GetPointAt(Bearing.SubtractDegrees(5), radius - 10),
					center.GetPointAt(Bearing.AddDegrees(5), radius - 10),
				};
				bitmap.DrawFilledPolygon(polygon, Color.Yellow);
			}

			// secondary compass pointer
			{
				PointDList polygon = new PointDList()
				{
					center.GetPointAt(DestinationBearing, radius - 1),
					center.GetPointAt(DestinationBearing.SubtractDegrees(5), radius - 8),
					center.GetPointAt(DestinationBearing.AddDegrees(5), radius - 8),
				};
				bitmap.DrawFilledPolygon(polygon, Color.Orange);
			}
			return bitmap;
		}

	}
}
