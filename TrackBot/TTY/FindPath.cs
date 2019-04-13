using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial.Imaging;

namespace TrackBot.TTY
{
	[CommandText("fp")]
	class FindPath : CommandBase
	{
		public FindPath()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			FuzzyPathList paths = Widgets.Instance.ImageEnvironment.FindGoodDestinations(Program.Config.ShortRangeClearance);
			if(paths.Count > 0)
			{
				Widgets.Instance.ImageEnvironment.FuzzyPath = paths.Longest;
			}
			else
			{
				Console.WriteLine("No path found");
			}

			return true;
		}
		
		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "fp";
			description = "Find fuzzy path";
		}

	}
}
