using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	public class DisconnectRequest : ControlMessage
	{
		public DisconnectRequest(MqttClient client)
			: base(client, ControlMessageType.DisconnectRequest)
		{
		}

		public override byte[] Serialize()
		{
			RemainingLength = 0;

			byte[] fixedHeader = base.Serialize();
			return fixedHeader;
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
