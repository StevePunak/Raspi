using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.Devices.Servos;

namespace TrackBot.Servos
{
	internal class ServoController : ServoMoveThread
	{
		protected override void SetServoPosition(ServoParameters servo, int value)
		{
			Log.LogText(LogLevel.DEBUG, "Moving {0} to {1}", servo, value);
			Pigs.SetServoPosition(servo.Pin, value);
		}

		protected override void SetIdle(ServoParameters servo)
		{
			Pigs.SetServoPosition(servo.Pin, 0);
		}
	}
}
