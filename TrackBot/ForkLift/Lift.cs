using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.Devices.MotorControl;

namespace TrackBot.ForkLift
{
	public class Lift : PWMMotorDriver
	{
		public Lift()
			: base(Program.Config.LiftInputPin1, Program.Config.LiftInputPin2, Program.Config.LiftInputPin3, Program.Config.LiftInputPin4)
		{
			Log.SysLogText(LogLevel.DEBUG, "Creating LIFT at A1: {0} A2: {1} B1: {2} B2: {3}", A1, A2, B1, B2); 
		}
	}
}
