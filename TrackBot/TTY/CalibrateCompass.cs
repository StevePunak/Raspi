using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon;
using RaspiCommon.Devices.Compass;

namespace TrackBot.TTY
{
	[CommandText("cc")]
	class CalibrateCompass : CommandBase
	{
		public CalibrateCompass()
			: base(true) {}

		public override bool Execute(List<String> commandParts)
		{
			if(Widgets.Instance.Compass is LSM9DS1CompassAccelerometer == false)
			{
				throw new CommandException($"Compass is {Widgets.Instance.Compass} which is not calibratable");
			}
			LSM9DS1CompassAccelerometer compass = Widgets.Instance.Compass as LSM9DS1CompassAccelerometer;

			Widgets.Instance.SpatialPollThread.Paused = true;

			Widgets.Instance.Tracks.Spin(SpinDirection.Clockwise, Widgets.Instance.Tracks.Medium);

			compass.Calibrate();

			Widgets.Instance.Tracks.Stop();

			Console.WriteLine("Adjust X = {0:0.000}  Y = {1:0.000}", compass.XAdjust, compass.YAdjust);
			Program.Config.CompassXAdjust = compass.XAdjust;
			Program.Config.CompassYAdjust = compass.YAdjust;

			Widgets.Instance.SpatialPollThread.Paused = false;

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "cc";
			description = "Calibrate compass by spinning bot";
		}
	}
}
