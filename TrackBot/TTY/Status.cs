using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("s")]
	public class Status : CommandBase
	{
		public Status()
			: base(true) {}

		public override bool Execute(List<string> commandParts)
		{
			Console.WriteLine("Heading: {0:0.0}  Magnetic Deviation: {1:0.0}", Widgets.GyMag.Bearing, Widgets.GyMag.MagneticDeviation);
			Console.WriteLine("Track1: {0}  Track2: {1}", Widgets.Tracks.LeftSpeed, Widgets.Tracks.RightSpeed);
			Console.WriteLine("Obstacle Distance: {0:0.000}", Widgets.RangeFinder.Range);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "s";
			description = "show TrackBot status";
		}
	}
}
