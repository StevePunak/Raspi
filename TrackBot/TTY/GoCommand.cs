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
			FuzzyPath path = Widgets.Instance.ImageEnvironment.FindGoodDestination(Program.Config.ShortRangeClearance);
			if(path != null)
			{
				Widgets.Instance.ImageEnvironment.FuzzyPath = path;
			}

			Activity.StartActivity(ActivityType.GoToDestination);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "go";
			description = "begin activity";
		}
	}
}