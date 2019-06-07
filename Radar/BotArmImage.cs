using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using KanoopCommon.Geometry;
using RaspiCommon.Extensions;

namespace Radar
{
	public class BotArmImage
	{
		public static readonly Point RearArmPivot = new Point(25, 58);
		public static readonly Size MaxSize = new Size(450, 450);

		public Mat Base { get; set; }
		public Mat RearArm { get; set; }
		public Mat ForeArm { get; set; }
		public Mat Claw { get; set; }

		Point BaseCenter { get; set; }
		Point RearArmCenter { get; set; }
		Point ForeArmCenter { get; set; }
		Point ClawCenter { get; set; }

		public BotArmImage(Mat armBase, Mat armRear, Mat armFore, Mat armClaw)
		{
			Base = armBase;
			RearArm = armRear;
			ForeArm = armFore;
			Claw = Claw;
		}

		public Mat GetImage()
		{
			Mat image = new Mat(MaxSize, DepthType.Cv8U, 4);
			int x = (image.Width / 2) - (Base.Width / 2);
			int y = image.Height - Base.Height;
			x = y = 0;
			image.DrawImage(Base, new PointD(x, y));
			return image;
		}



	}
}
