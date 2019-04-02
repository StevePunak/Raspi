using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("sd")]
	class StoppingDistance : CommandBase
	{
		public StoppingDistance()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Double parm;
			if(commandParts.Count < 2 || Double.TryParse(commandParts[1], out parm) == false || parm < 0.01 || parm > 2)
			{
				throw new CommandException("Invalid parameter value");
			}

			Widgets.Tracks.StoppingDistance = parm;
			Program.Config.StoppingDistance = parm;
			Program.Config.Save();

			Console.WriteLine("Stopping distance set to {0}", Widgets.Tracks.StoppingDistance);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "sd [stopping distnace in meters]";
			description = "set standard stopping distance for maneuvering";
		}


	}
}
