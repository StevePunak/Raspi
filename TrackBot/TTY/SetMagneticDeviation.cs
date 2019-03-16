using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("dev")]
	class SetMagneticDeviation : CommandBase
	{
		public SetMagneticDeviation()
			: base(true) {}

		public override bool Execute(List<String> commandParts)
		{
			Double value;
			if(commandParts.Count < 2)
			{
				throw new CommandException("Must supply value");
			}
			if(Double.TryParse(commandParts[1], out value))
			{
				if(value < 0 || value > 359.9)
				{
					throw new CommandException("Value must be between 0 and 359.9");
				}
				Program.Config.MagneticDeviation = value;
			}
			return true;
		}

		public override string Usage()
		{
			return "dev [value]     Value is magnetic deviation between 0 and 359.9";
		}
	}
}
