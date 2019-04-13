using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using MQTT;
using RaspiCommon.Network;

namespace RaspiCommon.Network
{
	public class RadarController
	{
		#region Properties

		public IPAddress Address { get; set; }

		public String MqqtClientID { get; set; }

		MqttClient Client { get; set; }

		#endregion

		#region Constructor

		public RadarController(String brokerAddress, String mqqtClientID)
		{
			IPAddress address;

			if(IPAddress.TryParse(brokerAddress, out address) == false &&
				IPAddressExtensions.TryResolve(brokerAddress, out address) == false)
			{
				throw new CommonException("Could not resolve '{0}' to a host", brokerAddress);
			}
			Address = address;
			MqqtClientID = mqqtClientID;
		}

		#endregion

		#region Public Methods

		public void SendCommand(String command)
		{
			Send(MqttTypes.CommandsTopic, command, false);

		}

		private void Send(object mqqtTypes)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Private Routines

		private void Send(String topic, String what, bool retain = false)
		{
			Send(topic, ASCIIEncoding.UTF8.GetBytes(what), retain);
		}

		private void Send(String topic, byte[] what, bool retain = false)
		{
			if(Client == null || Client.Connected == false)
			{
				GetConnected();
			}

			if(Client != null && Client.Connected == true)
			{
				Client.Publish(topic, what, retain);
			}
		}

		private void GetConnected()
		{
			try
			{
				Client = new MqttClient(Address, MqqtClientID)
				{
					QOS = QOSTypes.Qos0
				};
				Client.Connect();
			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.DEBUG, "GetConnected EXCEPTION: {0}", e.Message);
			}
		}

		#endregion
	}
}
