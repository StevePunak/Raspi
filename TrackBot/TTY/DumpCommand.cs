using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("dump")]
	class DumpCommand : CommandBase
	{
		public DumpCommand()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			RaspiConfig config = Program.Config;
			List<PropertyInfo> properties = new List<PropertyInfo>(typeof(RaspiConfig).GetProperties());
			properties.Sort(delegate (PropertyInfo x, PropertyInfo y)
							{
								return x.Name.CompareTo(y.Name);
							});

			foreach(PropertyInfo property in properties)
			{
				// iterate dictionaries
				List<Type> interfaceTypes = new List<Type>(property.PropertyType.GetInterfaces());
				Type dictionaryType = interfaceTypes.Find(t => t.Name.Contains("IDictionary"));
				if(dictionaryType != null)
				{
					Type keyType = dictionaryType.GetGenericArguments()[0];
					Type valueType = dictionaryType.GetGenericArguments()[1];
					Log.SysLogText(LogLevel.DEBUG, "{0}", property.Name);
					foreach(DictionaryEntry entry in property.GetValue(config) as IDictionary)
					{
						Log.SysLogText(LogLevel.DEBUG, "  {0}{1}", entry.Key.ToString().PadRight(28), entry.Value.ToString());
					}
				}
				else
				{
					Log.SysLogText(LogLevel.DEBUG, "{0}{1}", property.Name.PadRight(30), property.GetValue(config));
				}
			}

			Log.SysLogText(LogLevel.DEBUG, "----------   LED Positions   ----------");
			Program.Config.LEDTravelHistory.DumpToLog();
			Log.SysLogText(LogLevel.DEBUG, "----------   Color Thresholds   ----------");
			Log.SysLogText(LogLevel.DEBUG, "Blue: {0}  Green: {1}  Red: {2}", Program.Config.BlueThresholds, Program.Config.GreenThresholds, Program.Config.RedThresholds);
			Log.SysLogText(LogLevel.DEBUG, "----------   Camera Parameters   ----------");
			Log.SysLogText(LogLevel.DEBUG, "{0}", Program.Config.CameraParameters);
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "dump";
			description = "dump debug info";
		}


	}
}
