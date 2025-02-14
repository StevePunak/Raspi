using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("f")]
	public class Forward : CommandBase
	{
		public Forward()
			: base(true) {}

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
				Widgets.Instance.Tracks.ForwardTime(time, Widgets.Instance.Tracks.StandardSpeed);
			}
			else
			{
				Widgets.Instance.Tracks.ForwardMeters(parm, Widgets.Instance.Tracks.StandardSpeed);
			}

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "f [ms / meters]";
			description = "move forward for the time or distance specified";
		}
	}
}
