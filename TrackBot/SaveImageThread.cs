using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

namespace TrackBot
{
	class SaveImageThread : ThreadBase
	{
		const String STANDARD_SAVE_LOCATION = "/var/www/html/grid.png";
		const String ROTATING_SAVE_LOCATION = "/home/pi/tmp";

		public bool Drawing { get; set; }

		int _imageNumber;

		public SaveImageThread()
			: base(typeof(SaveImageThread).Name)
		{
			Interval = TimeSpan.FromSeconds(5);
			_imageNumber = 0;
			Drawing = false;
		}

		protected override bool OnStart()
		{
			Console.WriteLine("Starting save image thread");

			foreach(String file in Directory.GetFiles(ROTATING_SAVE_LOCATION, "*png"))
			{
				File.Delete(file);
			}

			return base.OnStart();
		}

		protected override bool OnRun()
		{
			if(Drawing)
			{
				Log.SysLogText(LogLevel.DEBUG, "Creating image with bearing {0:0.00}°  Lidar: {1:0.00}°", Widgets.Instance.Compass.Bearing, Widgets.Instance.ImageEnvironment.Bearing);
				Mat mat = Widgets.Instance.ImageEnvironment.PointsToBitmap();

				mat.Save(STANDARD_SAVE_LOCATION);

				_imageNumber++;
			}
			return true;
		}
	}
}
