using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;

namespace TrackBot.TTY
{
	abstract class CommandBase
	{
		public String Name { get; private set; }

		protected CommandBase(bool forceCallDummy)
		{
			CommandTextAttribute attr = GetType().GetCustomAttribute<CommandTextAttribute>();
			Name = attr.Value;
		}

		public abstract String Usage();
		public abstract bool Execute(List<String> commandParts);
	}

	class CommandTextAttribute : StringAttribute
	{
		public CommandTextAttribute(String name)
			: base(name) { }
	}
}
