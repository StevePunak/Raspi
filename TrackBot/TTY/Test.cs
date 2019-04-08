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
