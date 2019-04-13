using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;

namespace RaspiCommon.Devices.Chassis
{
	public class ChassisMetrics
	{
		public Double Length { get; set; }
		public Double Width { get; set; }
		public PointD LidarPosition { get; set; }

		public int ByteArraySize { get { return sizeof(Double) + sizeof(Double) + PointD.ByteArraySize; } }

		public ChassisMetrics()
		{

		}

		public ChassisMetrics(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				Length = br.ReadDouble();
				Width = br.ReadDouble();
				LidarPosition = new PointD(br.ReadBytes(PointD.ByteArraySize));
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Length);
				bw.Write(Width);
				bw.Write(LidarPosition.Serialize());
			}
			return serialized;
		}
	}
}
