using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Addresses;
using KanoopCommon.Logging;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Devices.Spatial;
using RaspiCommon.Lidar;
using RaspiCommon.Network;

namespace Testing
{
	class LidarClientTest
	{
		public LidarClientTest()
		{
			bool fileRead = false;

			NullCompass compass = new NullCompass();

			NetworkLidar lidar = new NetworkLidar("raspi1:5959", .25, compass)
			{
				Offset = 90
			};
			lidar.RangeBlobReceived += OnClientRangeBlobReceived;
			if(fileRead)
			{
				using(FileStream fs = new FileStream(@"c:/pub/tmp/trace/lidar_out.bin", FileMode.Open))
				{
					int bytesRead;
					byte[] readBuffer = new byte[16384];
					while((bytesRead = fs.Read(readBuffer, 0, readBuffer.Length)) > 0)
					{
						Console.WriteLine($"read {bytesRead}");
						byte[] buffer = new byte[bytesRead];
						Array.Copy(readBuffer, 0, buffer, 0, bytesRead);
						//Log.SysLogHex(LogLevel.DEBUG, readBuffer);
						lidar.Client.AddBytes(buffer);
					}
				}
			}
			else
			{
				lidar.Start();
			}

			Thread.Sleep(TimeSpan.FromDays(1));
		}

		private void OnClientRangeBlobReceived(DateTime timestamp, LidarVector[] vectors)
		{
			foreach(LidarVector vector in vectors)
			{
				if(vector.Range != 0)
				{
//					Console.WriteLine("VEC: {0}", vector);
				}
			}
		}
	}
}
