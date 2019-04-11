using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("t")]
	class Test : CommandBase
	{
		public Test()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Double bearing;
			if(commandParts.Count < 2)
			{
				bearing = Widgets.Instance.Compass.Bearing;
			}
			else  if(Double.TryParse(commandParts[1], out bearing) == false)
			{
				throw new CommandException("Invalid parameter");
			}

			Console.WriteLine("Getting fuzz range at {0:0.0}°", bearing);
			PointCloud2D left, right;
			Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.Chassis, bearing, 30, out left, out right);

			double shortest = Widgets.Instance.ImageEnvironment.ShortestRangeAtBearing(0, 90);
			Console.WriteLine("Shortest {0:0.000} ", shortest);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "stop";
			description = "stop all activities";
		}
	}
}
