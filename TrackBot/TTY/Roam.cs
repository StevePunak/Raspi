using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackBot.Spatial;

namespace TrackBot.TTY
{
	[CommandText("roam")]
	class Roam : CommandBase
	{
		public Roam()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Activity.StartActivity(ActivityType.RoamAndSeek);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "roam";
			description = "begin roaming activities";
		}
	}
}
