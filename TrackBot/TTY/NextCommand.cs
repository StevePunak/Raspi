using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using TrackBot.Spatial;

namespace TrackBot.TTY
{
	[CommandText("n")]
	class NextCommand : CommandBase
	{
		public NextCommand()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			if(commandParts.Count > 1 && commandParts[1].Contains("q"))
			{
				Activity.RunningActivity.QuitRequest = true;
			}

			Activity.ChangeState();

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "da [bearing]";
			description = "Set debug angle to bearing";
		}
	}
}
