using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Encoding;
using KanoopCommon.Logging;

namespace RaspiCommon.Spatial.DeadReckoning
{
	public class DRGrid
	{
		public int ByteArraySize { get { return sizeof(Double) + Matrix.ByteArraySize + UTF8.Length(Name); } }

		public String Name { get; set; }
		public DRMatrix Matrix { get; set; }
		public Double Scale { get; set; }

		public GridCell this[double x, double y]
		{
			get
			{
				Double scaledX = x / Scale;
				Double scaledY = y / Scale;
				if(scaledX >= 0 && scaledY >= 0 && scaledX < Matrix.Width && scaledY < Matrix.Height)
				{
					return Matrix.Cells[(int)scaledX, (int)scaledY];
				}
				else
				{
					Log.SysLogText(LogLevel.WARNING, "GridCell overrun trying to get {0},{1}", scaledX, scaledY);
					return new GridCell();
				}
			}
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
			Name = UTF8.ReadEncoded(br);
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
				bw.Write(UTF8.Encode(Name));
			}
			return serialized;
		}
	}
}
