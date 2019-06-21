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
	public class PanTiltControl
	{
		const int MINIMUM = 0;
		const int MAXIMUM = 100;

		int _pan;
		public int Pan
		{
			get { return _pan; }
			set
			{
				if(value.EnsureBetween(MINIMUM, MAXIMUM) != _pan)
				{
					_pan = value.EnsureBetween(MINIMUM, MAXIMUM);
					Log.SysLogText(LogLevel.DEBUG, "Set pan {0}", _pan);
					SendPan(_pan);
				}
			}
		}

		int _tilt;
		public int Tilt
		{
			get { return _tilt; }
			set
			{
				if(value.EnsureBetween(MINIMUM, MAXIMUM) != _tilt)
				{
					_tilt = value.EnsureBetween(MINIMUM, MAXIMUM);
					Log.SysLogText(LogLevel.DEBUG, "Set tilt {0}", _tilt);
					SendTilt(_tilt);
				}
			}
		}

		public PCSideRadarController RadarController { get; private set; }

		public PanTiltControl(PCSideRadarController controller)
		{
			RadarController = controller;
			_pan = _tilt = 50;
		}

		public void PanUp(int howMuch)
		{
			Pan += howMuch;
		}

		public void PanDown(int howMuch)
		{
			Pan -= howMuch;
		}

		public void SetPan(int value)
		{
			Pan = value;
		}

		public void TiltUp(int howMuch)
		{
			Tilt += howMuch;
		}

		public void TiltDown(int howMuch)
		{
			Tilt -= howMuch;
		}

		public void SetTilt(int value)
		{
			Tilt = value;
		}

		void SendPan(int value)
		{
			RadarController.SendPercentage(MqttTypes.BotPanTopic, value);
		}

		void SendTilt(int value)
		{
			RadarController.SendPercentage(MqttTypes.BotTiltTopic, value);
		}

	}
}
