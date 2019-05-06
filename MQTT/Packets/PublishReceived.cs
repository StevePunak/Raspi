using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	public class PublishReceived : ControlMessage
	{
		public UInt16 MessageID { get; set; }

		public PublishReceived(MqttClient client)
			: base(client, ControlMessageType.PublishReceived)
		{
			RemainingLength = 2;
		}

		public override byte[] Serialize()
		{
			RemainingLength = 2;

			byte[] fixedHeader = base.Serialize();
			byte[] serialized = new byte[fixedHeader.Length + RemainingLength];

			serialized[0] = fixedHeader[0];
			serialized[1] = fixedHeader[1];
			serialized[2] = (byte)(MessageID >> 8);
			serialized[3] = (byte)(MessageID & 0xff);

			return serialized;
		}

		protected override byte MakeFlags()
		{
			return 0;
		}

	}
}
