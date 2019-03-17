using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("cg")]
	class ClearGrid : CommandBase
	{
		public ClearGrid()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Widgets.Environment.Reset();
			Widgets.Environment.SaveBitmap();

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "cg";
			description = "Clear Grid";
		}
	}
}
