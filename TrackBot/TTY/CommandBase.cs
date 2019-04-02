using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;

namespace TrackBot.TTY
{
	public abstract class CommandBase
	{
		protected enum YesNo
		{
			Yes, No, Escape
		}

		public String Name { get; private set; }

		protected CommandBase(bool forceCallDummy)
		{
			CommandTextAttribute attr = GetType().GetCustomAttribute<CommandTextAttribute>();
			Name = attr.Value;
		}

		public abstract void Usage(out String commandSyntax, out String description);
		public abstract bool Execute(List<String> commandParts);

		protected YesNo GetYesNo(String prompt)
		{
			while(true)
			{
				Console.Write(prompt + " ");
				Console.TreatControlCAsInput = true;
				String line = Console.ReadLine();
				if(line.ToLower() == "y")
				{
					return YesNo.Yes;
				}
				else if(line.ToLower() == "n")
				{
					return YesNo.Yes;
				}
			}
		}
	}

	class CommandTextAttribute : StringAttribute
	{
		public CommandTextAttribute(String name)
			: base(name) { }
	}
}
