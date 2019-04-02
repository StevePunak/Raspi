using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon.Lidar.Environs;

namespace TrackBot.TTY
{
	[CommandText("fp")]
	class FindPath : CommandBase
	{
		public FindPath()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			FuzzyPath path = Widgets.Environment.FindGoodDestination();
			if(path != null)
			{
				Widgets.Environment.FuzzyPath = path;
			}

			return true;
		}
		
		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "cg";
			description = "Clear Grid";
		}

	}
}
