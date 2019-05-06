using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("tb")]
	public class TurnToBearing : CommandBase
	{
		public TurnToBearing()
			: base(true) {}

		public override bool Execute(List<string> commandParts)
		{
			Double to;
			if(commandParts.Count < 2 || Double.TryParse(commandParts[1], out to) == false)
			{
				throw new CommandException("Invalid bearing");
			}

			Widgets.Instance.Tracks.TurnToBearing(to, SpinDirection.None, Program.Config.StandardSpeed);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "tb [bearing]";
			description = "Turn to bearing (left or right)";
		}
	}
}
