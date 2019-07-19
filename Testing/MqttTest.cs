using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTT.ClientThreads;
using RaspiCommon.Network;

namespace Testing
{
	class MqttTest
	{
		class MqttTestThread : SubscribeThread
		{
			public MqttTestThread()
				: base("thufir", "test-client-101", new List<String>() { MqttTypes.Voltage1Topic })
			{
			}

			protected override void DoWork()
			{
			}
		}

		public MqttTest()
		{
			MqttTestThread thread = new MqttTestThread();
			thread.InboundSubscribedMessage += OnInboundSubscribedMessage;
			thread.Start();

			Thread.Sleep(10000000);
		}

		private void OnInboundSubscribedMessage(MQTT.MqttClient client, MQTT.Packets.PublishMessage packet)
		{
		}
	}
}
