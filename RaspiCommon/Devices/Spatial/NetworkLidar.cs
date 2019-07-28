using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon.Lidar;
using RaspiCommon.Network;
using KanoopCommon.Extensions;

namespace RaspiCommon.Devices.Spatial
{
	public class NetworkLidar : LidarBase
	{
		public static readonly TimeSpan VectorExpiry = TimeSpan.FromSeconds(1);

		public String HostAddress { get; set; }
		public RPLidarClient Client { get; private set; }

		public NetworkLidar(String hostAddress, Double vectorSize)
			: base(vectorSize)
		{
			HostAddress = hostAddress;
			Client = new RPLidarClient(HostAddress);
			Client.RangeBlobReceived += OnClientRangeBlobReceived;
			ResizeVectors(vectorSize);        // default to 1/4 degree

			RangeBlobReceived += delegate { };
		}

		public override void Start()
		{
			Client.Connect();
		}

		public override void Stop()
		{
			Client.Client.Client.Close();
			base.Stop();
		}

		private void OnClientRangeBlobReceived(DateTime timestamp, LidarVector[] rawInputVectors)
		{
			LidarVector[] inputVectors = LidarVector.AdjustForBearing(rawInputVectors, Offset);
			DateTime now = DateTime.UtcNow;
			for(int x = 0;x < inputVectors.Length;x++)
			{
				// old one is expired and new one is zero
				if(inputVectors[x].Range == 0 && now > Vectors[x].RefreshTime + VectorExpiry)
				{
					Vectors[x].Range = 0;
				}
				else
				{
					Vectors[x].Range = inputVectors[x].Range;
					Vectors[x].RefreshTime = now;
				}
			}
			EmitRangeBlobReceived(timestamp);
		}

		private void ResizeVectors(Double vectorSize)
		{
			List<LidarVector> vectors = new List<LidarVector>();
			for(Double bearing = 0;bearing < 360;bearing += vectorSize)
			{
				vectors.Add(new LidarVector()
				{
					BearingAndRange = new BearingAndRange() { Bearing = bearing, Range = 0 },
				});
			}
			Vectors = vectors.ToArray();
			VectorSize = vectorSize;
		}
	}
}
