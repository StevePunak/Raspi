using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Network;

namespace Testing
{
	class MqttCompassTest : TestBase
	{
		protected override void Run()
		{
			MqttCompass compass = new MqttCompass("thufir", MqttTypes.BearingTopic);
			compass.NewBearing += Compass_NewBearing;
			compass.Start();
			compass.WaitForCompletion();
		}

		private void Compass_NewBearing(double bearing)
		{
		}
	}
}
