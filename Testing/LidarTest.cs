using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Addresses;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Devices.Spatial;
using RaspiCommon.Lidar;

namespace Testing
{
	class LidarTest : TestBase
	{
		protected override void Run()
		{
			NullCompass compass = new NullCompass();
//			RPLidarBase lidar = new RPLidarNetwork(new IPv4AddressPort("raspi1:5960"), 360.0/4, compass);
			RPLidarBase lidar = new RPFileLidar(@"c:/pub/tmp/lidar.bin.input", 360.0/4, compass);
			lidar.Sample += OnLidarSample;
			lidar.Start();

			while(!Quit)
			{
				Thread.Sleep(1000);
			}
			lidar.StopScan();
			lidar.Stop();
		}

		private void OnLidarSample(LidarSample sample)
		{
			Console.WriteLine($"{sample}");
		}
	}
}
