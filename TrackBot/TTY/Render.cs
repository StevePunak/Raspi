using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
		const String SAVE_LOCATION2 = "/home/pi/tmp/render1.png";
		const String BLOB_SAVE_LOCATION = "/home/pi/tmp/rangeblob.bin";

		public Render()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Console.WriteLine("Creating grid bitmap");
			Console.WriteLine("Environment is {0}", Widgets.Instance.ImageEnvironment);
			Console.WriteLine("PixelSize: {0}", Widgets.Instance.ImageEnvironment.PixelSize);

			byte[] blob = Widgets.Instance.ImageEnvironment.MakeRangeBlob();
			Mat image = Widgets.Instance.ImageEnvironment.PointsToBitmap();
			image.Save(SAVE_LOCATION1);
			File.WriteAllBytes(BLOB_SAVE_LOCATION, blob);

			Widgets.Instance.ImageEnvironment.ProcessImage(image, Widgets.Instance.ImageEnvironment.Bearing, Widgets.Instance.ImageEnvironment.PixelsPerMeter);

#if zero
			Mat mat = new Mat(Widgets.Instance.Environment.PixelSize, DepthType.Cv8U, 3);
			CvInvoke.Rectangle(mat, new Rectangle(100, 100, 100, 100), new Bgr(Color.Green).MCvScalar);

			for(Double bearing = 0;bearing < 360;bearing++)
			{
				Double range = Widgets.Instance.Environment.Lidar.GetDistance(bearing) * Widgets.Instance.Environment.PixelsPerMeter;
				PointD p1 = Widgets.Instance.Environment.PixelCenter.GetPointAt(bearing, range) as PointD;
				Rectangle rect = new Rectangle(p1.ToPoint(), new Size(1, 1));
				CvInvoke.Rectangle(mat, rect, new Bgr(Color.Red).MCvScalar);
			}
#endif
			//Image<Bgr, byte> bitmap = new Image<Bgr, byte>(Widgets.Instance.Environment.PixelSize);
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
