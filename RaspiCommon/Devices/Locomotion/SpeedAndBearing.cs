using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;

namespace RaspiCommon.Devices.Locomotion
{
	public class SpeedAndBearing
	{
		public int ByteArraySize { get { return TrackSpeed.ByteArraySize + sizeof(Double); } }

		public TrackSpeed TrackSpeed { get; set; }
		public Double Bearing { get; set; }

		public SpeedAndBearing()
		{
			TrackSpeed = new TrackSpeed();
			Bearing = 0;
		}

		public SpeedAndBearing(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				TrackSpeed = new TrackSpeed(br.ReadBytes(TrackSpeed.ByteArraySize));
				Bearing = br.ReadDouble();
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(TrackSpeed.Serialize());
				bw.Write(Bearing);
			}
			return serialized;
		}

		public override string ToString()
		{
			return String.Format("{0} {1}", TrackSpeed, Bearing.ToAngleString());
		}
	}
}
