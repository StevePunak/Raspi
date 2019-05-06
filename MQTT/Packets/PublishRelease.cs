using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	public class PublishRelease : ControlMessage
	{
		public UInt16 MessageID { get; set; }

		public PublishRelease(MqttClient client)
			: base(client, ControlMessageType.PublishRelease)
		{
			RemainingLength = 2;
		}

		public static bool TryParse(MqttClient client, Header header, byte[] buffer, int index, int length, out ControlMessage packet, out int bytesParsed)
		{
			packet = null;
			bytesParsed = 0;

			if(length - index >= 2)
			{
				PublishRelease pkt = new PublishRelease(client)
				{
					PacketHeader = header,
					MessageID = (UInt16)IPAddress.NetworkToHostOrder((Int16)BitConverter.ToUInt16(buffer, 2))
				};
				bytesParsed = index + 2;
				packet = pkt;
			}

			return packet != null;
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

		public override string ToString()
		{
			return String.Format("PublishRelease MsgID: {0}", MessageID);
		}
	}
}