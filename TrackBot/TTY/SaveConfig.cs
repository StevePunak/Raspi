using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("save")]
	class SaveConfig : CommandBase
	{
		public SaveConfig()
			: base(true) { }

		public override bool Execute(List<String> commandParts)
		{
			Program.Config.Save();
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "save";
			description = "Save configuration";
		}
	}
}
