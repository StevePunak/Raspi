using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackBot.Spatial;

namespace TrackBot.TTY
{
	[CommandText("stop")]
	class StopActivity : CommandBase
	{
		public StopActivity()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Activity.StopActivity();

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "stop";
			description = "stop all activities";
		}
	}
}
