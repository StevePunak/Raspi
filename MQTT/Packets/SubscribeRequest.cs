using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	public class SubscribeRequest : ControlPacket
	{
		public TopicFilterList Topics { get; set; }
		public UInt16 MessageID { get; set; }

		public SubscribeRequest(MqttClient client)
			: base(client, ControlPacketType.Subscribe)
		{
			MessageID = 0;
			Topics = new TopicFilterList();
		}

		public override byte[] Serialize()
		{
			RemainingLength = sizeof(UInt16) + Topics.SerializedLength;

			byte[] fixedHeader = base.Serialize();
			byte[] serialized = new byte[fixedHeader.Length + RemainingLength];

			using(MemoryStream ms = new MemoryStream(serialized))
			using(BinaryWriter bw = new BinaryWriter(ms))
			{
				bw.Write(fixedHeader);
				bw.Write((UInt16)(IPAddress.HostToNetworkOrder((Int16)MessageID)));
				bw.Write(Topics.Serialize());
			}

			return serialized;
		}

		protected override byte MakeFlags()
		{
			return (byte)0x02;
		}

		public override string ToString()
		{
			return String.Format("Subsribe: {0}", Topics);
		}
	}
}
