using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon.Extensions;

namespace TrackBot.TTY
{
	[CommandText("r")]
	class Render : CommandBase
	{
		const String SAVE_LOCATION1 = "/var/www/html/render1.png";
		const String SAVE_LOCATION2 = "/var/www/html/render2.png";

		public Render()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Console.WriteLine("Creating grid bitmap");
			Console.WriteLine("Environment is {0}", Widgets.Environment);
			Console.WriteLine("PixelSize: {0}", Widgets.Environment.PixelSize);

			Mat image = Widgets.Environment.PointsToBitmap();
			image.Save(SAVE_LOCATION1);

			Widgets.Environment.ProcessImage(image, Widgets.Environment.Bearing, Widgets.Environment.PixelsPerMeter);

#if zero
			Mat mat = new Mat(Widgets.Environment.PixelSize, DepthType.Cv8U, 3);
			CvInvoke.Rectangle(mat, new Rectangle(100, 100, 100, 100), new Bgr(Color.Green).MCvScalar);

			for(Double bearing = 0;bearing < 360;bearing++)
			{
				Double range = Widgets.Environment.Lidar.GetDistance(bearing) * Widgets.Environment.PixelsPerMeter;
				PointD p1 = Widgets.Environment.PixelCenter.GetPointAt(bearing, range) as PointD;
				Rectangle rect = new Rectangle(p1.ToPoint(), new Size(1, 1));
				CvInvoke.Rectangle(mat, rect, new Bgr(Color.Red).MCvScalar);
			}
#endif
			//Image<Bgr, byte> bitmap = new Image<Bgr, byte>(Widgets.Environment.PixelSize);
			//bitmap.Draw(new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height), new Bgr(Color.Green), 0);
			//bitmap.Save(SAVE_LOCATION);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "r";
			description = "render the grid";
		}
	}
}
