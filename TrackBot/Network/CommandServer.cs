using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Server;

namespace TrackBot.Network
{
	class CommandServer : TelemetryClient
	{
		public CommandServer(String clientID)
			: base(Program.Config.RadarHost, clientID, new List<String>() {
																			MqttTypes.CommandsTopic,
																		  })
		{
		}

	}
}
