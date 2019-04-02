using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Threading;
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

						CommandBase existing;
						if(_commands.TryGetValue(attr.Value, out existing))
						{
							throw new CommandException("{0} conflicts with existing command {1}", command.GetType(), existing.GetType());
						}
						_commands.Add(attr.Value, command as CommandBase);
					}
				}
			}
		}

		private static void Usage()
		{
			Console.WriteLine("Usage:");

			List<String> names = new List<String>(_commands.Keys);
			names.Sort();
			foreach(String cmdName in names)
			{
				CommandBase command = _commands[cmdName];
				String syntax, description;
				command.Usage(out syntax, out description);

				String output = String.Format("  {0,-16} - {1}", syntax, description);

				Console.WriteLine(output);
			}
		}

		private static void MainLoop()
		{
			while(true)
			{
				try
				{
					Console.Write("> ");
					String line = Console.ReadLine().Trim();

					if(line.Length == 0)
					{
						_commands["s"].Execute(null);
						continue;
					}

					if(line[0] == '?')
					{
						Usage();
						continue;
					}

					List<String> commandParts = new List<string>(line.Replace("=", " ").Trim().RemoveMultipleWhitespace().Split(' '));
					CommandBase command;
					if(_commands.TryGetValue(commandParts[0], out command) == false)
					{
						throw new CommandException("Illegal command");
					}

					if(command.Execute(commandParts) == false)
						break;
				}
				catch(CommandException e)
				{
					Console.WriteLine("ERROR: {0}", e.Message);
					Usage();
				}
				catch(Exception e)
				{
					Console.WriteLine("OTHER EXCEPTION:\n{0}", ThreadBase.GetFormattedStackTrace(e));
				}
			}
		}

	}
}
