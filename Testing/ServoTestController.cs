using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspiCommon.Devices.Servos;

namespace Testing
{
	internal class ServoTestController : ServoMoveThread
	{
		protected override void SetServoPosition(ServoParameters servo, int value)
		{
			Log.LogText(LogLevel.DEBUG, "Moving {0}", servo);
		}
	}
}
