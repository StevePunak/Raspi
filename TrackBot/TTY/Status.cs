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
			Console.WriteLine("Heading: {0:0.0}  Magnetic Deviation: {1:0.0}°  Lidar Offset {2:0.0}°",
				Widgets.GyMag.Bearing, Widgets.GyMag.MagneticDeviation, Widgets.Lidar.Offset);
			Console.WriteLine("Track1: {0}  Track2: {1}", Widgets.Tracks.LeftSpeed, Widgets.Tracks.RightSpeed);

#if ULTRASONIC
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Obstacle Distances:  ");
			foreach(HCSR04_RangeFinder rangeFinder in Widgets.RangeFinders.Values)
			{
				sb.AppendFormat("{0} => {1:0.00}m    ", rangeFinder.Direction, rangeFinder.Range);
			}
			Console.WriteLine(sb.ToString());
#endif
			Console.WriteLine("Range {0}", Widgets.Environment.FuzzyRange());

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "s";
			description = "show TrackBot status";
		}
	}
}
