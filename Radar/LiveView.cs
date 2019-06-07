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
using RaspiCommon.Devices.Locomotion;

namespace Radar
{
	public partial class LiveView : PictureBox
	{
		static readonly Size SpeedRectangleSize = new Size(30, 200);

		public LiveView()
		{
			InitializeComponent();
		}

		public void SetBitmap(Mat videoFeed, SpeedAndBearing speedAndBearing, Double forwardRange, Double backwardRange)
		{
			Mat image = videoFeed.Clone();

			//Rectangle leftSpeedRect = RectangleExtensions.GetCenteredRectangle(new Point(30, image.Height / 2), SpeedRectangleSize);
			//image.DrawRectangle(leftSpeedRect, Color.Green);

			//Rectangle rightSpeedRect = RectangleExtensions.GetCenteredRectangle(new Point(image.Width - 30, image.Height / 2), SpeedRectangleSize);
			//image.DrawRectangle(rightSpeedRect, Color.Green);

			//leftSpeedRect = leftSpeedRect.Shrink(1);
			//rightSpeedRect = rightSpeedRect.Shrink(1);

			//image.DrawRectangle(leftSpeedRect, Color.Yellow, 0);
			//image.DrawRectangle(rightSpeedRect, Color.Yellow, 0);

			this.BackgroundImage = image.Bitmap.Clone() as Bitmap;
		}
	}
}
