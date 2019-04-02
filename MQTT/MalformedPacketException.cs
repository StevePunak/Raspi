using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT
{
	public class MalformedPacketException : MqttException
	{
		public MalformedPacketException(String format, params object[] parms)
			: base(format, parms) { }
	}
}
