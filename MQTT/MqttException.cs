using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;

namespace MQTT
{
	public class MqttException : CommonException
	{
		public MqttException(String format, params object[] parms)
			: base(format, parms) {}
	}
}
