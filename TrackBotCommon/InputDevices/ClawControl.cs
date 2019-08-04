using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using RaspiCommon.Network;

namespace TrackBotCommon.InputDevices
{
	public class ClawControl
	{
		const int MINIMUM = 0;
		const int MAXIMUM = 100;

		static readonly TimeSpan MIN_CHANGE_INTERVAL = TimeSpan.Zero;

		DateTime _lastRotationChange;
		int _rotation;
		public int Rotation
		{
			get { return _rotation; }
			set
			{
				if(DateTime.UtcNow >= _lastRotationChange + MIN_CHANGE_INTERVAL)
				{
					if(value.EnsureBetween(MINIMUM, MAXIMUM) != _rotation)
					{
						_rotation = value.EnsureBetween(MINIMUM, MAXIMUM);
						SendRotation(_rotation);
						_lastRotationChange = DateTime.UtcNow;
					}
				}
			}
		}

		DateTime _lastThrustChange;
		int _thrust;
		public int Thrust
		{
			get { return _thrust; }
			set
			{
				if(DateTime.UtcNow >= _lastThrustChange + MIN_CHANGE_INTERVAL)
				{
					Log.SysLogText(LogLevel.DEBUG, "1 Set thrust {0}", value);
					if(value.EnsureBetween(MINIMUM, MAXIMUM) != _thrust)
					{
						_thrust = value.EnsureBetween(MINIMUM, MAXIMUM);
						Log.SysLogText(LogLevel.DEBUG, "Set thrust {0}", _thrust);
						SendThrust(_thrust);
						_lastThrustChange = DateTime.UtcNow;
					}
				}
			}
		}

		DateTime _lastClawChange;
		int _claw;
		public int Claw
		{
			get { return _claw; }
			set
			{
				if(DateTime.UtcNow >= _lastClawChange + MIN_CHANGE_INTERVAL)
				{
					if(value.EnsureBetween(MINIMUM, MAXIMUM) != _claw)
					{
						_claw = value.EnsureBetween(MINIMUM, MAXIMUM);
						SendClaw(_claw);
						_lastClawChange = DateTime.UtcNow;
					}
				}
			}
		}

		DateTime _lastElevationChange;
		int _elevation;
		public int Elevation
		{
			get { return _elevation; }
			set
			{
				if(DateTime.UtcNow >= _lastElevationChange + MIN_CHANGE_INTERVAL)
				{
					if(value.EnsureBetween(MINIMUM, MAXIMUM) != _elevation)
					{
						_elevation = value.EnsureBetween(MINIMUM, MAXIMUM);
						SendElevation(_elevation);
						_lastElevationChange = DateTime.UtcNow;
					}
				}
			}
		}

		public RaspiControlClient RadarController { get; private set; }

		public ClawControl(RaspiControlClient controller)
		{
			RadarController = controller;
			_rotation = _thrust = _elevation = _claw = 50;
		}

		public void OpenClaw(int howMuch)
		{
			Claw += howMuch;
		}

		public void CloseClaw(int howMuch)
		{
			Claw -= howMuch;
		}

		public void SetClaw(int value)
		{
			Claw = value;
		}

		public void ChangeRotation(int howMuch)
		{
			Rotation += howMuch;
		}

		public void ChangeThrust(int howMuch)
		{
			Thrust += howMuch;
		}

		public void ChangeElevation(int howMuch)
		{
			Elevation += howMuch;
		}

		void SendRotation(int value)
		{
			RadarController.SendPercentage(MqttTypes.ArmRotationTopic, value);
		}

		void SendThrust(int value)
		{
			RadarController.SendPercentage(MqttTypes.ArmThrustTopic, value);
		}

		void SendClaw(int value)
		{
			RadarController.SendPercentage(MqttTypes.ArmClawTopic, value);
		}

		void SendElevation(int value)
		{
			RadarController.SendPercentage(MqttTypes.ArmElevationTopic, value);
		}
	}
}
