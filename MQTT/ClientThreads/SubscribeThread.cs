using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MQTT.Packets;

namespace MQTT.ClientThreads
{
	public class SubscribeThread : MqttClientThread
	{
		public event PublishedMessageReceivedHandler InboundSubscribedMessage;

		public QOSTypes QOS { get; set; }

		public SubscribeThread(String brokerAddress, String clientID, List<String> topics)
			: base(typeof(SubscribeThread).Name, brokerAddress, clientID, topics)
		{
			Interval = TimeSpan.FromSeconds(1);

			InboundSubscribedMessage += delegate { };
		}

		public void Subscribe(String topic)
		{
			Client.Subscribe(topic, QOS);
		}

		public void Unsubscribe(String topic)
		{
			Client.Unsubscribe(topic, QOS);
		}

		protected override void DoWork()
		{
		}

		protected override void DoConnectWork()
		{
			Client.InboundSubscribedMessage += OnInboundSubscribedMessage;
			foreach(String topic in Topics)
			{
				Subscribe(topic);
			}
		}

		private void OnInboundSubscribedMessage(MqttClient client, PublishMessage packet)
		{
			InboundSubscribedMessage(client, packet);
		}
	}
}
