using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Spatial.DeadReckoning
{
	public class DRGrid
	{
		public int ByteArraySize { get { return sizeof(Double) + Matrix.ByteArraySize + Name.Length; } }

		public String Name { get; set; }
		public DRMatrix Matrix { get; set; }
		public Double Scale { get; set; }

		public GridCell this[double x, double y]
		{
			get { return Matrix.Cells[(int)(x / Scale), (int)(y / Scale)]; }
		}

		public DRGrid(String name, Double width, Double height, Double scale)
		{
			Name = name;
			Scale = scale;
			Matrix = new DRMatrix(this, ToScale(width), ToScale(height));
		}

		public DRGrid(BinaryReader br)
		{
			Scale = br.ReadDouble();
			Matrix = new DRMatrix(br);
			Name = br.ReadString();
		}

		public int ToScale(Double meters)
		{
			return (int)(meters / Scale);
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Scale);
				bw.Write(Matrix.Serialize());
				bw.Write(Name);
			}
			return serialized;
		}
	}
}
