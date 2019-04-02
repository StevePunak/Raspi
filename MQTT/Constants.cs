using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT
{
	public class Constants
	{
		public const int DefaultBrokerPort = 1883;
		public static readonly TimeSpan DefaultKeepAliveInterval = TimeSpan.FromSeconds(60);
	}
}
