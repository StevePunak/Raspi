using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;

namespace RaspiCommon.Devices.Locomotion
{
	public class MoveStep
	{
		public static int ByteArraySize { get { return sizeof(Direction) + TimeSpanExtensions.ByteArraySize + sizeof(int); } }

		public Direction Direction { get; set; }
		public TimeSpan Time { get; set; }
		public int Speed { get; set; }

		public MoveStep(Direction direction, TimeSpan time, int speed)
		{
			Direction = direction;
			Time = time;
			Speed = speed;
		}

		public MoveStep()
			: this(Direction.None, TimeSpan.Zero, 0) { }

		public MoveStep(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				Direction = (Direction) br.ReadInt32();
				Time = TimeSpanExtensions.Deserialize(br);
				Speed = br.ReadInt32();
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write((int)Direction);
				bw.Write(Time.Serialize());
				bw.Write(Speed);
			}
			return serialized;
		}

		public override string ToString()
		{
			return String.Format("Move: {0}  {1}", Direction, Time);
		}
	}
}
