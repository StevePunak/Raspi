using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Devices.Locomotion
{
	public class TrackSpeed
	{
		public static int ByteArraySize { get { return sizeof(int) + sizeof(int); } }

		public int LeftSpeed { get; set; }
		public int RightSpeed { get; set; }

		public TrackSpeed()
		{
			LeftSpeed = RightSpeed = 0;
		}

		public TrackSpeed(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				LeftSpeed = br.ReadInt32();
				RightSpeed = br.ReadInt32();
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(LeftSpeed);
				bw.Write(RightSpeed);
			}
			return serialized;
		}

		public override string ToString()
		{
			return String.Format("L: {0}  R: {1}", LeftSpeed, RightSpeed);
		}

	}
}
