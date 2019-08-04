using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Devices.Compass
{
	public class CompassRawData
	{
		public Double MX { get; set; }
		public Double MY { get; set; }
		public Double MZ { get; set; }

		public int ByteArraySize { get { return sizeof(Double) * 3; } }

		public CompassRawData()
		{

		}

		public CompassRawData(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				MX = br.ReadDouble();
				MY = br.ReadDouble();
				MZ = br.ReadDouble();
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(MX);
				bw.Write(MY);
				bw.Write(MZ);
			}
			return serialized;
		}
	}
}
