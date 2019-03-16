using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("calcomp")]
	class CalibrateCompass : CommandBase
	{
		public CalibrateCompass()
			: base(true) {}

		public override bool Execute(List<String> commandParts)
		{
			Widgets.Tracks.Spin(SpinDirection.Clockwise, 60);

			Widgets.GyMag.Calibrate();

			Widgets.Tracks.Stop();

			Console.WriteLine("Adjust X = {0:0.000}  Y = {1:0.000}", Widgets.GyMag.XAdjust, Widgets.GyMag.YAdjust);
			Program.Config.CompassXAdjust = Widgets.GyMag.XAdjust;
			Program.Config.CompassYAdjust = Widgets.GyMag.YAdjust;
			Program.Config.Save();

			return true;
		}

		public override String Usage()
		{
			return "calcomp - Calibrate compass by spinning bot";
		}
	}
}
