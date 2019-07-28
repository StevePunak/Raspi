using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using MQTT;
using RaspiCommon.Devices.Locomotion;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Network;

namespace RaspiCommon.Network
{
	public class RaspiControlClient
	{
		#region Properties

		public IPAddress Address { get; set; }

		public String MqqtClientID { get; set; }

		public MqttClient Client { get; set; }

		#endregion

		#region Constructor

		public RaspiControlClient(String brokerAddress, String mqqtClientID)
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

		protected void Stop()
		{
			Client.Stop();
		}

		public void SendCommand(String command)
		{
			Send(MqttTypes.CommandsTopic, command, false);
		}

		public void SendSpeed(int left, int right)
		{
			TrackSpeed speed = new TrackSpeed() { LeftSpeed = left, RightSpeed = right };
			Send(MqttTypes.BotSpeedTopic, speed.Serialize(), false);
		}

		public void SendSpinStepLeftDegrees(Double degrees)
		{
			byte[] payload = BitConverter.GetBytes(degrees);
			Send(MqttTypes.BotSpinStepLeftDegreesTopic, payload);
		}

		public void SendSpinStepRightDegrees(Double degrees)
		{
			byte[] payload = BitConverter.GetBytes(degrees);
			Send(MqttTypes.BotSpinStepRightDegreesTopic, payload);
		}

		public void SendSpinStepLeftTime(TimeSpan time)
		{
			byte[] payload = BitConverter.GetBytes((int)time.TotalMilliseconds);
			Send(MqttTypes.BotSpinStepLeftTimeTopic, payload);
		}

		public void SendSpinStepRightTime(TimeSpan time)
		{
			byte[] payload = BitConverter.GetBytes((int)time.TotalMilliseconds);
			Send(MqttTypes.BotSpinStepRightTimeTopic, payload);
		}

		public void SendMoveStep(Direction direction, TimeSpan time, int speed)
		{
			MoveStep move = new MoveStep(direction, time, speed);
			byte[] payload = move.Serialize();
			Send(MqttTypes.BotMoveTimeTopic, payload);
		}

		public void SendPercentage(String topic, int value)
		{
			Send(topic, BitConverter.GetBytes(value));
		}

		public void SendCameraParameters(RaspiCameraParameters parameters)
		{
			Send(MqttTypes.RaspiCameraSetParametersTopic, parameters.Serialize());
		}

		#endregion

		#region Private Routines

		private void Send(String topic, String what, bool retain = false)
		{
			Send(topic, ASCIIEncoding.UTF8.GetBytes(what), retain);
		}

		private void Send(String topic, byte[] what = null, bool retain = false)
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
