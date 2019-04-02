using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

namespace MQTT.Examples
{
	public abstract class MqttExampleThread : ThreadBase
	{
		public IPAddress BrokerAddress { get; set; }
		public String ClientID { get; set; }
		public List<String> Topics { get; set; }
		public bool Connected { get; private set; }
		public String UserName { get; set; }
		public String Password { get; set; }
		public TimeSpan KeepAliveInterval { get; set; }
		public MqttClient Client { get; private set; }

		protected abstract void DoWork();

		public MqttExampleThread(String name, String brokerAddress, String clientID, List<String> topics)
			: base(name)
		{
			IPAddress address;
			if(IPAddress.TryParse(brokerAddress, out address) == false &&
								IPAddressExtensions.TryResolve(brokerAddress, out address) == false)
			{
				throw new CommonException("Could not resolve '{0}' to a host", brokerAddress);
			}

			BrokerAddress = address;
			ClientID = clientID;
			Topics = topics;

			Connected = false;
			KeepAliveInterval = Constants.DefaultKeepAliveInterval;

			Interval = TimeSpan.FromSeconds(1);
		}

		public void Disconnect()
		{
			Client.Disconnect();
		}

		protected override bool OnRun()
		{
			if(!Connected)
			{
				GetConnected();
			}

			if(Connected)
			{
				DoWork();
			}
			return true;
		}

		protected override bool OnStop()
		{
			if(Connected)
			{
				Client.Disconnect();
			}
			return base.OnStop();
		}

		void GetConnected()
		{
			try
			{
				Client = new MqttClient(BrokerAddress, ClientID)
				{
					QOS = QOSTypes.Qos1,
					UserName = UserName,
					Password = Password,
					KeepAliveInterval = KeepAliveInterval
				};

				Log.LogText(LogLevel.DEBUG, "Connecting to {0} as {1}", BrokerAddress, ClientID);
				Connected = Client.Connect(TimeSpan.FromSeconds(5));
				Client.ClientDisconnected += OnClientDisconnected;
				Log.LogText(LogLevel.DEBUG, "Connected to {0}", Client);

				DoConnectWork();
			}
			catch(Exception e)
			{
				Log.LogText(LogLevel.ERROR, "{0} ERROR: {1}", Name, e.Message);
			}
		}

		private void OnClientDisconnected(MqttClient client)
		{
			Connected = false;
		}

		protected virtual void DoConnectWork()
		{
			return;
		}
	}
}
