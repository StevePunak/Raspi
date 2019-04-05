using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;

namespace TrackBot.TTY
{
	[CommandText("set")]
	class SetCommand : CommandBase
	{
		public SetCommand()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			if(commandParts.Count < 3)
			{
				throw new CommandException("Invalid parameter value");
			}

			if(commandParts[1].ToLower() == "rangefuzz")
			{
				Double parm;
				if(Double.TryParse(commandParts[2], out parm) == false)
				{
					throw new CommandException("Invalid parameter value 2");
				}
				Program.Config.RangeFuzz = parm;
				Widgets.ImageEnvironment.RangeFuzz = parm;
				Program.Config.Save();
				Console.WriteLine("range fuzz set to {0}", parm);
			}
			else
			{
				Console.WriteLine("Unknown parameter");
			}

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "set [parmname] [vale]";
			description = "Set the given configuration parameter";
		}
	}
}
