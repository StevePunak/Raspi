using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("lo")]
	class SetLidarOffset : CommandBase
	{
		public SetLidarOffset()
			: base(true) { }

		public override bool Execute(List<String> commandParts)
		{
			Double value;
			if(commandParts.Count < 2)
			{
				throw new CommandException("Must supply value");
			}
			if(Double.TryParse(commandParts[1], out value))
			{
				if(value < -359.9 || value > 359.9)
				{
					throw new CommandException("Value must be between -359 and 359.9");
				}
				Widgets.Environment.CompassOffset = value;
				Program.Config.LidarOffsetDegrees = value;
				Program.Config.Save();

				Widgets.Environment.GenerateBitmap(true, false).Save(Program.Config.SaveImageLocation);
			}
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "lo [value]";
			description = "Value is compass offset between -359.9 and 359.9";
		}
	}
}
