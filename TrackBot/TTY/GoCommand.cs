using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Spatial.Imaging;
using TrackBot.Spatial;

namespace TrackBot.TTY
{
	[CommandText("go")]
	class GoCommand : CommandBase
	{
		public GoCommand()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			FuzzyPathList paths = Widgets.Instance.ImageEnvironment.FindGoodDestinations(Program.Config.ShortRangeClearance);
			if(paths != null)
			{
				Widgets.Instance.ImageEnvironment.FuzzyPath = paths.Longest;
			}

			Activity.StartActivity(ActivityType.GoToDestination, true);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "go";
			description = "begin activity";
		}
	}
}