using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using MQTT;
using MQTT.Packets;
using RaspiCommon.Devices.Locomotion;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Network;

namespace TrackBot.Network
{
	class RemoteMqttController : TelemetryClient
	{
		public event PercentageCommandReceivedHandler ArmRotation;
		public event PercentageCommandReceivedHandler ArmElevation;
		public event PercentageCommandReceivedHandler ArmThrust;
		public event PercentageCommandReceivedHandler ArmClaw;
		public event RaspiCameraParametersReceivedHandler RaspiCameraParameters;
		public event SpinStepReceivedHandler SpinLeft;
		public event SpinStepReceivedHandler SpinRight;

		public RemoteMqttController(String clientID, List<String> topics)
			: base(Program.Config.MqttPublicHost, clientID, topics)
		{
			ArmRotation += delegate { };
			ArmElevation += delegate { };
			ArmThrust += delegate { };
			ArmClaw += delegate { };
			RaspiCameraParameters += delegate { };
			SpinLeft += delegate { };
			SpinRight += delegate { };
		}

		protected override void OnLidarClientInboundSubscribedMessage(MqttClient client, PublishMessage packet)
		{
			Log.SysLogText(LogLevel.DEBUG, $"MQTT IN: {packet.Topic}");
			if(packet.Topic == MqttTypes.ArmRotationTopic)
			{
				int percent = BitConverter.ToInt32(packet.Message, 0);
				ArmRotation(percent);
			}
			else if(packet.Topic == MqttTypes.ArmElevationTopic)
			{
				Log.LogText(LogLevel.DEBUG, "Elevation");
				int percent = BitConverter.ToInt32(packet.Message, 0);
				ArmElevation(percent);
			}
			else if(packet.Topic == MqttTypes.ArmThrustTopic)
			{
				Log.LogText(LogLevel.DEBUG, "Thrust");
				int percent = BitConverter.ToInt32(packet.Message, 0);
				ArmThrust(percent);
			}
			else if(packet.Topic == MqttTypes.ArmClawTopic)
			{
				int percent = BitConverter.ToInt32(packet.Message, 0);
				ArmClaw(percent);
			}
			else if(packet.Topic == MqttTypes.BotSpeedTopic)
			{
				TrackSpeed speed = new TrackSpeed(packet.Message);
				Widgets.Instance.Tracks.TravelThread.SetSpeed(speed.LeftSpeed, speed.RightSpeed);
			}
			else if(packet.Topic == MqttTypes.RaspiCameraSetParametersTopic)
			{
				RaspiCameraParameters parameters = new RaspiCameraParameters(packet.Message);
				RaspiCameraParameters(parameters);
			}
			else if(packet.Topic == MqttTypes.BotSpinStepLeftDegreesTopic)
			{
				Double degrees = BitConverter.ToDouble(packet.Message, 0);
				Widgets.Instance.Tracks.TravelThread.SpinStepDegrees(SpinDirection.CounterClockwise, degrees);

				SpinLeft(SpinDirection.CounterClockwise);
			}
			else if(packet.Topic == MqttTypes.BotSpinStepRightDegreesTopic)
			{
				Double degrees = BitConverter.ToDouble(packet.Message, 0);
				Widgets.Instance.Tracks.TravelThread.SpinStepDegrees(SpinDirection.Clockwise, degrees);

				SpinRight(SpinDirection.Clockwise);
			}
			else if(packet.Topic == MqttTypes.BotSpinStepLeftTimeTopic)
			{
				Log.LogText(LogLevel.DEBUG, "SpinStepLeft");
				TimeSpan time = TimeSpan.FromMilliseconds(BitConverter.ToInt32(packet.Message, 0));
				Widgets.Instance.Tracks.TravelThread.SpinStepTime(SpinDirection.CounterClockwise, time);

				SpinLeft(SpinDirection.CounterClockwise);
			}
			else if(packet.Topic == MqttTypes.BotSpinStepRightTimeTopic)
			{
				Log.LogText(LogLevel.DEBUG, "SpinStepRight");
				TimeSpan time = TimeSpan.FromMilliseconds(BitConverter.ToInt32(packet.Message, 0));
				Widgets.Instance.Tracks.TravelThread.SpinStepTime(SpinDirection.Clockwise, time);

				SpinRight(SpinDirection.Clockwise);
			}
			else if(packet.Topic == MqttTypes.BotMoveTimeTopic)
			{
				Log.LogText(LogLevel.DEBUG, "Move");
				MoveStep move = new MoveStep(packet.Message);
				Widgets.Instance.Tracks.TravelThread.MoveStepTime(move);
			}
			else if(packet.Topic == MqttTypes.BotPanTopic)
			{
				int percent = BitConverter.ToInt32(packet.Message, 0);
				Log.LogText(LogLevel.DEBUG, $"Pan {percent}");
				Widgets.Instance.PanTilt.Pan = percent;
			}
			else if(packet.Topic == MqttTypes.BotTiltTopic)
			{
				int percent = BitConverter.ToInt32(packet.Message, 0);
				Log.LogText(LogLevel.DEBUG, $"Tilt {percent}");
				Widgets.Instance.PanTilt.Tilt = percent;
			}
			else
			{
				base.OnLidarClientInboundSubscribedMessage(client, packet);
			}
		}

	}
}
