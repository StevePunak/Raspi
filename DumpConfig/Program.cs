using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.PersistentConfiguration;
using RaspiCommon;

namespace DumpConfig
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				String configFileName = String.Empty;
				if(args.Length < 2)
				{
					configFileName = RaspiConfig.GetDefaultConfigFileName();
				}
				else
				{
					configFileName = args[1];
				}

				if(File.Exists(configFileName) == false)
				{
					throw new Exception($"File not found: {configFileName}");
				}
				Console.WriteLine($"Dumping: {configFileName}");
				ConfigFile  configFile = new ConfigFile(configFileName);
				RaspiConfig config = (RaspiConfig)configFile.GetConfiguration(typeof(RaspiConfig));

				List<PropertyInfo> properties = new List<PropertyInfo>(typeof(RaspiConfig).GetProperties());
				properties.Sort(delegate (PropertyInfo x, PropertyInfo y)
				{
					return x.Name.CompareTo(y.Name);
				});

				foreach(PropertyInfo property in properties)
				{
					Console.WriteLine("{0}{1}", property.Name.PadRight(30), property.GetValue(config));
				}
			}
			catch(Exception e)
			{
				Console.WriteLine($"EXCEPTION: {e.Message}");
			}
		}
	}
}
