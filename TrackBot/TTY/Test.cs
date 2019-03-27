using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			for(Double d = 0;d < 360;d += Widgets.Lidar.VectorSize)
			{
				Console.WriteLine("@ {0}° range is {1:0.000}", d, Widgets.Lidar.GetDistance(d));
			}

			Console.WriteLine("Done...");

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "stop";
			description = "stop all activities";
		}
	}
}
