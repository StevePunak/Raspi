using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTT.ClientThreads;
using RaspiCommon.Spatial;
using RaspiCommon.Extensions;
using KanoopCommon.Extensions;

namespace RaspiCommon.Devices.Compass
{
	public class MqttCompass : SubscribeThread, ICompass
	{
		public double Bearing { get; private set; }

		public String ServerAddress { get; set; }
		public String Topic { get; set; }

		public event NewBearingHandler NewBearing;

		public MqttCompass(String serverAddress, String topic)
			: base(serverAddress, $"{Environment.MachineName}-CompassClient-{DateTime.UtcNow.ToMillisecondsSinceEpoch()}", new List<string>() { topic })
		{
			Bearing = 0;

			NewBearing += delegate {};

			NewBearing(0);
		}
	}
}
