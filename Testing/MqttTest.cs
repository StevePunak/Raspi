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
	class MqttTest : TestBase
	{
		protected override void Run()
		{
			TestSubscribeThread thread = new TestSubscribeThread();
			thread.InboundSubscribedMessage += OnInboundSubscribedMessage;
			thread.Start();

			Double bearing = 0;
			while(!Quit)
			{
				//byte[] output = BitConverter.GetBytes(bearing);
				//thread.Client.Publish(MqttTypes.BearingTopic, output, false);
				//if(++bearing >= 360) bearing = 0;

				Console.WriteLine("Sleeping");
				Sleep(1000);
			}
		}

		class TestSubscribeThread : SubscribeThread
		{
			public TestSubscribeThread()
				: base(typeof(TestSubscribeThread).Name, "thufir", "test-client-101", new List<String>() { MqttTypes.BearingTopic })
			{
			}

			protected override void DoWork()
			{
			}
		}

		private void OnInboundSubscribedMessage(MQTT.MqttClient client, MQTT.Packets.PublishMessage packet)
		{
			if(packet.Topic == MqttTypes.BearingTopic)
			{
				Double bearing = BitConverter.ToDouble(packet.Message, 0);
				Console.WriteLine($"bearing: {bearing}");
			}
		}
	}
}
