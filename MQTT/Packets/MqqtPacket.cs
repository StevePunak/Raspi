using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	public abstract class MqqtPacket
	{
		public PacketType PacketType { get; private set; }

		protected MqqtPacket(PacketType type)
		{
			PacketType = type;
		}
	}
}
