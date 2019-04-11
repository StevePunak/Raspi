using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("b")]
	public class Backward : CommandBase
	{
		public Backward()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Double parm;
			if(commandParts.Count < 2 || Double.TryParse(commandParts[1], out parm) == false)
			{
				throw new CommandException("Invalid parameter value");
			}

			if(parm > 10 || parm == 0)
			{
				TimeSpan time = TimeSpan.FromMilliseconds(parm);
				Widgets.Instance.Tracks.BackwardTime(time, Widgets.Instance.Tracks.StandardSpeed);
			}
			else
			{
				Widgets.Instance.Tracks.BackwardMeters(parm, Widgets.Instance.Tracks.StandardSpeed);
			}

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "b [ms / meters]";
			description = "move backward for the time or distance specified";
		}
	}
}
