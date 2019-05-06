using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	public class SubscribeAcknowledgment : ControlMessage
	{
		public UInt16 MessageID { get; private set; }
		public QOSTypes GrantedQOS { get; set; }
		public bool Success { get; set; }

		public SubscribeAcknowledgment(MqttClient client)
			: base(client, ControlMessageType.SubscribeAcknowledgment)
		{
			MessageID = 0;
			GrantedQOS = QOSTypes.Qos0;
			Success = false;
		}

		public static bool TryParse(MqttClient client, Header header, byte[] buffer, int index, int length, out ControlMessage packet, out int bytesParsed)
		{
			packet = null;
			bytesParsed = 0;

			if(length - index >= 3)
			{
				SubscribeAcknowledgment ack = new SubscribeAcknowledgment(client)
				{
					PacketHeader = header,
					MessageID = (UInt16)((buffer[index] << 8) | buffer[index + 1]),
					GrantedQOS = (QOSTypes)(buffer[index + 2] & 0x03),
					Success = (buffer[index + 2] & 0x80) == 0
				};
				packet = ack;
				bytesParsed = index + 3;
			}

			return packet != null;
		}

		protected override byte MakeFlags()
		{
			return 0;
		}

		public override string ToString()
		{
			return String.Format("SubscribeAck ID: {0}", MessageID);
		}
	}
}
