using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Lidar;
using RaspiCommon.Network;

namespace RaspiCommon.Devices.Spatial
{
	public abstract class LidarBase
	{
		public event RangeBlobReceivedHandler RangeBlobReceived;
		public LidarVector[] Vectors { get; protected set; }
		public Double VectorSize { get; set; }
		public Double Offset { get; set; }
		public Double DebugAngle { get; set; }
		public Double RenderPixelsPerMeter { get; set; }
		public GpioPin SpinPin { get; set; }
		public Double Bearing { get; set; }

		protected LidarBase(Double vectorSize)
		{
			VectorSize = vectorSize;
		}

		public Double GetRangeAtBearing(Double bearing)
		{
			Double offset = bearing / VectorSize;
			return Vectors[(int)offset].Range;
		}

		public DateTime GetLastSampleTimeAtBearing(Double bearing)
		{
			Double offset = bearing / VectorSize;
			return Vectors[(int)offset].RefreshTime;
		}

		public virtual void Start()
		{

		}

		public virtual void Stop()
		{

		}

		public void ClearDistanceVectors()
		{
			Array.Clear(Vectors, 0, Vectors.Length);
		}

		protected void EmitRangeBlobReceived()
		{
			RangeBlobReceived(Vectors);
		}
	}
}
