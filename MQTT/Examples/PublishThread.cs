using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using MQTT;

namespace MQTT.Examples
{
	public abstract class PublishThread : MqttExampleThread
	{
		protected abstract byte[] GetMessage();

		public PublishThread(String brokerAddress, String clientID, List<String> topics)
			: base(typeof(PublishThread).Name, brokerAddress, clientID, topics)
		{
			Interval = TimeSpan.FromSeconds(1);
		}

		protected override void DoWork()
		{
			//byte[] message = GetMessage();
			//Client.Publish(Topics, message);
		}
	}
}
