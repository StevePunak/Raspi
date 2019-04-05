using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;

namespace RaspiCommon.Spatial.Imaging
{
	public class FuzzyPath
	{
		public BearingAndRange Vector { get; set; }
		public BearingAndRangeList Elements { get; set; }

		public FuzzyPath()
			: this(0, null) {}

		public FuzzyPath(Double vectorBearing, BearingAndRangeList elements)
		{
			if(elements.Count > 0)
			{
				Elements = elements;
				Vector = new BearingAndRange(vectorBearing, Elements.Average(e => e.Range));
			}
			else
			{
				Vector = new BearingAndRange(vectorBearing, 0);
			}
			Elements = elements;
		}

		FuzzyPath(BearingAndRange vector, BearingAndRangeList elements)
		{
			Vector = vector;
			Elements = elements;
		}

		public FuzzyPath(byte[] input)
		{
			Elements = new BearingAndRangeList();
			using(BinaryReader br = new BinaryReader(new MemoryStream(input)))
			{
				byte[] vector = br.ReadBytes(BearingAndRange.ByteArraySize);
				while(br.BaseStream.Position < br.BaseStream.Length)
				{
					byte[] bar = br.ReadBytes(BearingAndRange.ByteArraySize);
					Elements.Add(new BearingAndRange(bar));
				}
				Vector = new BearingAndRange(vector);
			}
		}

		public Line GetLineFrom(PointD point)
		{
			return new Line(point, point.GetPointAt(Vector.Bearing, Vector.Range));
		}

		public byte[] Serizalize()
		{
			byte[] output = new byte[BearingAndRange.ByteArraySize * (Elements.Count + 1)];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(output)))
			{
				bw.Write(Vector.Serialize());
				foreach(BearingAndRange rab in Elements)
				{
					bw.Write(rab.Serialize());
				}
			}
			return output;
		}

		public FuzzyPath Clone()
		{
			return new FuzzyPath(Vector.Clone(), Elements.Clone());
		}

		public override string ToString()
		{
			return String.Format("Fuzzy: {0}", Vector);
		}
	}
}
