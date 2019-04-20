using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;

namespace RaspiCommon.Devices.Optics
{
	public class LEDPosition
	{
		public static int ByteArraySize { get { return sizeof(UInt32) + PointD.ByteArraySize; } }

		public Color Color { get; set; }
		public PointD Location { get; set; }
		public DateTime CreateTime { get; set; }

		public LEDPosition()
		{
			Color = Color.Empty;
			Location = PointD.Empty;
			CreateTime = DateTime.UtcNow;
		}

		public LEDPosition(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				Color = ColorExtensions.DeserializeColor(br.ReadBytes(sizeof(int)));
				Location = new PointD(br.ReadBytes(PointD.ByteArraySize));
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Color.Serialize());
				bw.Write(Location.Serialize());
			}
			return serialized;
		}

		public override string ToString()
		{
			return String.Format("{0} at {1}", Color, Location);
		}
	}

	public class LEDPositionList : List<LEDPosition>
	{
		public int ByteArraySize { get { return LEDPosition.ByteArraySize * Count; } }

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				foreach(LEDPosition position in this)
				{
					bw.Write(position.Serialize());
				}
			}
			return serialized;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach(LEDPosition position in this)
			{
				sb.AppendFormat("{0}\n", position.ToString());
			}
			return sb.ToString();
		}
	}
}