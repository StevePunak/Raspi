using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Devices.Spatial
{
	public class LidarMetrics
	{
		public Double Offset { get; set; }

		public int ByteArraySize { get { return sizeof(Double); } }

		public LidarMetrics()
		{
		}

		public LidarMetrics(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				Offset = br.ReadDouble();
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Offset);
			}
			return serialized;
		}
	}
}