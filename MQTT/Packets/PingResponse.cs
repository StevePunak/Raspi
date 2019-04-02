using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	public class PingResponse : ControlPacket
	{
		public PingResponse(MqttClient client)
			: base(client, ControlPacketType.PingResponse)
		{
		}

		public override byte[] Serialize()
		{
			RemainingLength = 0;

			byte[] fixedHeader = base.Serialize();
			byte[] serialized = new byte[fixedHeader.Length + RemainingLength];

			using(MemoryStream ms = new MemoryStream(serialized))
			using(BinaryWriter bw = new BinaryWriter(ms))
			{
				bw.Write(fixedHeader);
			}

			return serialized;
		}

		public static bool TryParse(MqttClient client, Header header, byte[] buffer, int index, int length, out ControlPacket packet, out int bytesParsed)
		{
			packet = null;
			bytesParsed = 0;

			if(length - index >= 0)
			{
				PingResponse p = new PingResponse(client)
				{
					PacketHeader = header,
				};
				bytesParsed = index;
				packet = p;
			}

			return packet != null;
		}

		protected override byte MakeFlags()
		{
			return 0;
		}

		public override string ToString()
		{
			return String.Format("PingRsp");
		}
	}
}
