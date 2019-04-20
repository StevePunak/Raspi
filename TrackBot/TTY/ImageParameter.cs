using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("ip")]
	class ImageParameter : CommandBase
	{
		public ImageParameter()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			int parm;
			if(commandParts.Count < 3)
			{
				throw new CommandException("Invalid parameter value");
			}

			if(!(commandParts[1] == "br" || commandParts[1] == "ex" || commandParts[1] == "ifx"))
			{
				throw new CommandException("Invalid parameter value");
			}

			if(commandParts[1] == "br" && int.TryParse(commandParts[2], out parm) == true)
			{
				Widgets.Instance.Camera.Brightness = parm;
			}
			else if(commandParts[1] == "ex")
			{
				Widgets.Instance.Camera.Exposure = commandParts[2];
			}
			else if(commandParts[1] == "ifx")
			{
				Widgets.Instance.Camera.Effect = commandParts[2];
			}
			else
			{
				throw new CommandException("Invalid paramters");
			}

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "ip [br/ex/ifx] [value]";
			description = "set camera image brightness";
		}
	}
}
