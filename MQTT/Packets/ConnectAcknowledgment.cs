using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	public class ConnectAcknowledgment : ControlPacket
	{
		public bool SessionPresent { get; private set; }
		public ConnectReturnCode ReturnCode { get; private set; }

		public ConnectAcknowledgment(MqttClient client)
			: base(client, ControlPacketType.ConnectAcknowledgment)
		{
			SessionPresent = false;
		}

		public static bool TryParse(MqttClient client, Header header, byte[] buffer, int index, int length, out ControlPacket packet, out int bytesParsed)
		{
			packet = null;
			bytesParsed = 0;

			if(length - index >= 2)
			{
				ConnectAcknowledgment ack = new ConnectAcknowledgment(client)
				{
					PacketHeader = header,
					SessionPresent = (buffer[index] & 0x01) > 0,
					ReturnCode = (ConnectReturnCode)(buffer[index + 1])
				};
				packet = ack;
				bytesParsed = index + 2;
			}

			return packet != null;
		}

		protected override byte MakeFlags()
		{
			return 0;
		}

		public override string ToString()
		{
			return String.Format("ConnectAck SP: {0} Result: {1}", SessionPresent, ReturnCode);
		}
	}
}
