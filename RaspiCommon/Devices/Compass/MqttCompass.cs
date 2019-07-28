using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTT.ClientThreads;
using RaspiCommon.Spatial;
using RaspiCommon.Extensions;
using KanoopCommon.Extensions;
using MQTT;
using MQTT.Packets;

namespace RaspiCommon.Devices.Compass
{
	public class MqttCompass : SubscribeThread, ICompass
	{
		public Double Bearing { get; private set; }
		public Double MagneticDeviation { get; set; }

		public String ServerAddress { get; set; }
		public String Topic { get; set; }

		public event NewBearingHandler NewBearing;

		public MqttCompass(String serverAddress, String topic)
			: base(typeof(MqttCompass).Name,
				  serverAddress, 
				  $"{Environment.MachineName}-CompassClient-{DateTime.UtcNow.ToMillisecondsSinceEpoch()}", 
				  new List<string>()
				  {
					  topic
				  })
		{
			Bearing = 0;
			Topic = topic;

			InboundSubscribedMessage += OnInboundSubscribedMessage;
			NewBearing += delegate {};
		}

		private void OnInboundSubscribedMessage(MqttClient client, PublishMessage packet)
		{
			if(packet.Topic == Topic)
			{
				Bearing = BitConverter.ToDouble(packet.Message, 0).AddDegrees(MagneticDeviation);
				NewBearing(Bearing);
			}
		}

		void ICompass.Start()
		{
			Start();
		}

		void ICompass.Stop()
		{
			Stop();
		}
	}
}
