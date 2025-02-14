using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Encoding;

namespace MQTT.Packets
{
	public class UnsubscribeRequest : ControlMessage
	{
		public List<String> Topics { get; set; }
		public UInt16 MessageID { get; set; }

		public UnsubscribeRequest(MqttClient client)
			: base(client, ControlMessageType.Unsubscribe)
		{
			MessageID = 0;
			Topics = new List<String>();
		}

		public override byte[] Serialize()
		{
			RemainingLength = sizeof(UInt16) + UTF8.Length(Topics);	// topics + message id

			byte[] fixedHeader = base.Serialize();
			byte[] serialized = new byte[fixedHeader.Length + RemainingLength];

			using(MemoryStream ms = new MemoryStream(serialized))
			using(BinaryWriter bw = new BinaryWriter(ms))
			{
				bw.Write(fixedHeader);
				bw.Write((UInt16)(IPAddress.HostToNetworkOrder((Int16)MessageID)));
				bw.Write(UTF8.Encode(Topics));
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
