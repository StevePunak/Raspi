using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			Widgets.SpatialPollThread.Paused = true;

			Widgets.Tracks.Spin(SpinDirection.Clockwise, Widgets.Tracks.StandardSpeed);

			Widgets.GyMag.Calibrate();

			Widgets.Tracks.Stop();

			Console.WriteLine("Adjust X = {0:0.000}  Y = {1:0.000}", Widgets.GyMag.XAdjust, Widgets.GyMag.YAdjust);
			Program.Config.CompassXAdjust = Widgets.GyMag.XAdjust;
			Program.Config.CompassYAdjust = Widgets.GyMag.YAdjust;

			Widgets.SpatialPollThread.Paused = false;

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "cc";
			description = "Calibrate compass by spinning bot";
		}
	}
}
