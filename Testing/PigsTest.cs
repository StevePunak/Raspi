using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon;

namespace Testing
{
	class PigsTest
	{
		public PigsTest()
		{
			Pigs.SetOutputPin(GpioPin.Pin24, true);
			Pigs.SetOutputPin(GpioPin.Pin24, false);
		}
	}
}
