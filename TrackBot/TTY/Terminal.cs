using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using RaspiCommon;

namespace TrackBot.TTY
{
	class Terminal
	{

		static Dictionary<String, CommandBase> _commands;

		public static void Run()
		{
			InitCommandProcessors();
			Console.WriteLine("Welcome to trackbot");

			MainLoop();

		}

		private static void InitCommandProcessors()
		{
			_commands = new Dictionary<string, CommandBase>();

			foreach(Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if(type.IsSubclassOf(typeof(CommandBase)))
				{
					CommandTextAttribute attr = type.GetCustomAttribute<CommandTextAttribute>();
					if(attr != null)
					{
						Object command = Activator.CreateInstance(type);
						_commands.Add(attr.Value, command as CommandBase);
					}
				}
			}
		}

		private static void Usage()
		{
			Console.WriteLine("Usage:");
			foreach(CommandBase command in _commands.Values)
			{
				Console.WriteLine("  {0}", command.Usage());
			}
		}

		private static void MainLoop()
		{
			while(true)
			{
				try
				{
					Console.Write("> ");
					String line = Console.ReadLine();

					List<String> commandParts = new List<string>(line.Replace("=", " ").Trim().RemoveMultipleWhitespace().Split(' '));
					CommandBase command;
					if(_commands.TryGetValue(line, out command) == false)
					{
						throw new CommandException("Illegal command");
					}

					if(command.Execute(commandParts) == false)
						break;
				}
				catch(Exception e)
				{
					Console.WriteLine("ERROR: {0}", e.Message);
				}
			}
		}

	}
}
