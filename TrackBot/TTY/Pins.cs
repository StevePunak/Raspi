using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("pins")]
	class Pins : CommandBase
	{
		public Pins()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			foreach(PropertyInfo property in Program.Config.GetType().GetProperties())
			{
				if(property.PropertyType == typeof(GpioPin))
				{
					Console.WriteLine("{0} {1}", property.GetValue(Program.Config), property.Name);
				}
			}
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "b [ms / meters]";
			description = "move backward for the time or distance specified";
		}
	}
}
