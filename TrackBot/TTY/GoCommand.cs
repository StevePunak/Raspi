using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Spatial.LidarImaging;
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
			bool wait = commandParts.Count > 1;
			Activity.StartActivity(ActivityType.FindTwoLEDs, null, wait);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "go";
			description = "begin activity";
		}
	}
}