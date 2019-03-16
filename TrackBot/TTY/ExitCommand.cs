using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("exit")]
	class ExitCommand : CommandBase
	{
		public ExitCommand()
			: base(true) {}

		public override bool Execute(List<string> commandParts)
		{
			return false;
		}

		public override string Usage()
		{
			return "exit   - leave bot shell";
		}
	}
}
