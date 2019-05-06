using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	public class PingRequest : ControlMessage
	{
		public PingRequest(MqttClient client)
			: base(client, ControlMessageType.PingRequest)
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

		protected override byte MakeFlags()
		{
			return 0;
		}

		public override string ToString()
		{
			return String.Format("PingReq");
		}
	}
}
