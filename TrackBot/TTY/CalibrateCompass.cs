using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("cc")]
	class CalibrateCompass : CommandBase
	{
		public CalibrateCompass()
			: base(true) {}

		public override bool Execute(List<String> commandParts)
		{
			Widgets.Instance.SpatialPollThread.Paused = true;

			Widgets.Instance.Tracks.Spin(SpinDirection.Clockwise, Widgets.Instance.Tracks.Medium);

			Widgets.Instance.GyMag.Calibrate();

			Widgets.Instance.Tracks.Stop();

			Console.WriteLine("Adjust X = {0:0.000}  Y = {1:0.000}", Widgets.Instance.GyMag.XAdjust, Widgets.Instance.GyMag.YAdjust);
			Program.Config.CompassXAdjust = Widgets.Instance.GyMag.XAdjust;
			Program.Config.CompassYAdjust = Widgets.Instance.GyMag.YAdjust;

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
