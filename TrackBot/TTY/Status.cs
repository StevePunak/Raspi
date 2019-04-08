using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("s")]
	public class Status : CommandBase
	{
		public Status()
			: base(true) {}

		public override bool Execute(List<string> commandParts)
		{
			Console.WriteLine("Heading: {0:0.0}", Widgets.Instance.GyMag.Bearing);
			Console.WriteLine("Magnetic Deviation: {0:0.0}°", Widgets.Instance.GyMag.MagneticDeviation);
			Console.WriteLine("Lidar Offset {0:0.0}°", Widgets.Instance.ImageEnvironment.CompassOffset);
			Console.WriteLine("Track1: {0}  Track2: {1}", Widgets.Instance.Tracks.LeftSpeed, Widgets.Instance.Tracks.RightSpeed);

#if ULTRASONIC
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Obstacle Distances:  ");
			foreach(HCSR04_RangeFinder rangeFinder in Widgets.Instance.RangeFinders.Values)
			{
				sb.AppendFormat("{0} => {1:0.00}m    ", rangeFinder.Direction, rangeFinder.Range);
			}
			Console.WriteLine(sb.ToString());
#endif
			Double forwardBearing = Widgets.Instance.GyMag.Bearing;
			Double backwardBearing = Widgets.Instance.GyMag.Bearing.AddDegrees(180);
			Double forwardRange = Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(forwardBearing, Program.Config.BearingFuzz);
			Double backwardRange = Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(backwardBearing, Program.Config.BearingFuzz);
			Console.WriteLine("Range at [Forward {0:0.00}° {1:0.00}m]   [Backward {2:0.00}° {3:0.00}m]", forwardBearing, forwardRange, backwardBearing, backwardRange);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "s";
			description = "show TrackBot status";
		}
	}
}
