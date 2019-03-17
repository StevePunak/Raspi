using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("r")]
	class Render : CommandBase
	{
		public Render()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Console.WriteLine("Creating grid bitmap");

			Widgets.Environment.SaveBitmap();

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "r";
			description = "render the grid";
		}
	}
}
