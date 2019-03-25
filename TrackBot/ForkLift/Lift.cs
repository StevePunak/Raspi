using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon;

namespace TrackBot.ForkLift
{
	public class Lift : MotorDriver
	{
		public Lift()
			: base(Program.Config.LiftInputPin1, Program.Config.LiftInputPin2, Program.Config.LiftInputPin3, Program.Config.LiftInputPin4)
		{

		}
	}
}
