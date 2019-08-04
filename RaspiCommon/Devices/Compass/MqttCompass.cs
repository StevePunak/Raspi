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
		public String BearingTopic { get; set; }
		public String RawDataTopic { get; set; }

		public event NewBearingHandler NewBearing;
		public event CompassRawDataHandler RawData;

		public MqttCompass(String serverAddress, String bearingTopic, String rawDataTopic)
			: base(typeof(MqttCompass).Name,
				  serverAddress, 
				  $"{Environment.MachineName}-CompassClient-{DateTime.UtcNow.ToMillisecondsSinceEpoch()}", 
				  new List<string>()
				  {
					  bearingTopic,
					  rawDataTopic,
				  })
		{
			Bearing = 0;
			BearingTopic = bearingTopic;
			RawDataTopic = rawDataTopic;

			InboundSubscribedMessage += OnInboundSubscribedMessage;
			NewBearing += delegate {};
			RawData += delegate {};
		}

		private void OnInboundSubscribedMessage(MqttClient client, PublishMessage packet)
		{
			if(packet.Topic == BearingTopic)
			{
				Bearing = BitConverter.ToDouble(packet.Message, 0).AddDegrees(MagneticDeviation);
				NewBearing(Bearing);
			}
			else if(packet.Topic == RawDataTopic)
			{
				CompassRawData data = new CompassRawData(packet.Message);
				RawData(data.MX, data.MY, data.MZ);
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
