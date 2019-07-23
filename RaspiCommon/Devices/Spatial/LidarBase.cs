using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspiCommon.Lidar;
using RaspiCommon.Network;
using RaspiCommon.Spatial.LidarImaging;

namespace RaspiCommon.Devices.Spatial
{
	public abstract class LidarBase
	{
		public event RangeBlobReceivedHandler RangeBlobReceived;
		public event CompassOffsetChangedHandler CompassOffsetChanged;

		public LidarVector[] Vectors { get; protected set; }
		public Double VectorSize { get; set; }
		public Double DebugAngle { get; set; }
		public Double RenderPixelsPerMeter { get; set; }
		public GpioPin SpinPin { get; set; }
		public Double Bearing { get; set; }

		Double _offset;
		public Double Offset
		{
			get { return _offset; }
			set
			{
				_offset = value;
				Log.SysLogText(LogLevel.DEBUG, $"NL: set offset to {value}");
				CompassOffsetChanged(value);
				Log.SysLogText(LogLevel.DEBUG, $"NL: set offset complete");
			}
		}

		protected LidarBase(Double vectorSize)
		{
			VectorSize = vectorSize;

			RangeBlobReceived += delegate { };
			CompassOffsetChanged += delegate { };
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

		protected void EmitRangeBlobReceived(DateTime timestamp)
		{
			RangeBlobReceived(timestamp, Vectors);
		}
	}
}
