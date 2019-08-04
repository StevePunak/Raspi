using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Devices.Spatial;

namespace Testing
{
	class LidarTest
	{
		public LidarTest()
		{
			NullCompass compass = new NullCompass();
			RPLidar lidar = new RPLidar("/dev/ttyS0", 360.0/4, compass);
			lidar.Sample += OnLidarSample;
			lidar.Start();
			lidar.GetDeviceInfo();
			lidar.StartScan();

			Thread.Sleep(30000);
			lidar.StopScan();
			lidar.Stop();

			Console.WriteLine("Received {0} bytes int {1} gos", lidar.Port.TotalBytesReceived, lidar.Port.TotalDeliveries);
		}

		private void OnLidarSample(RaspiCommon.Lidar.LidarSample sample)
		{
		}
	}
}
