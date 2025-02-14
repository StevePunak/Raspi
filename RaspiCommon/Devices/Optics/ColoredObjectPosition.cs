﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Serialization;
using RaspiCommon.GraphicalHelp;

namespace RaspiCommon.Devices.Optics
{
	public class ColoredObjectPosition
	{
		public static int ByteArraySize { get { return sizeof(UInt32) + PointD.ByteArraySize + sizeof(Double) + SizeExtensions.ByteArraySize + ColoredObjectCandidate.ByteArraySize; } }

		public Color Color { get; set; }
		public PointD Location { get; set; }
		public Double Bearing { get; set; }
		public DateTime CreateTime { get; set; }
		public Size Size { get; set; }
		public ColoredObjectCandidate Candidate { get; set; }

		public Object Tag { get; set; }

		public ColoredObjectPosition()
		{
			Color = Color.Empty;
			Location = PointD.Empty;
			Bearing = 0;
			Size = new Size();

			CreateTime = DateTime.UtcNow;
		}

		public ColoredObjectPosition(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				Color = ColorExtensions.Deserialize(br.ReadBytes(sizeof(int)));
				Location = new PointD(br.ReadBytes(PointD.ByteArraySize));
				Bearing = br.ReadDouble();
				Size = SizeExtensions.Deserialize(br);
				Candidate = new ColoredObjectCandidate(br.ReadBytes(ColoredObjectCandidate.ByteArraySize));
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Color.Serialize());
				bw.Write(Location.Serialize());
				bw.Write(Bearing);
				bw.Write(Size.Serialize());
				bw.Write(Candidate.Serialize());
			}
			return serialized;
		}

		public override string ToString()
		{
			return String.Format("{0} at {1} - bearing {2}", Color.Name, Location, Bearing.ToAngleString());
		}
	}

	public class ColoredObjectPositionList : List<ColoredObjectPosition>
	{
		public int ByteArraySize { get { return ColoredObjectPosition.ByteArraySize * Count; } }

		public List<PointD> Points
		{
			get
			{
				List<PointD> list = new List<PointD>(this.Select(l => l.Location).ToList());
				return list;
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				foreach(ColoredObjectPosition position in this)
				{
					bw.Write(position.Serialize());
				}
			}
			return serialized;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach(ColoredObjectPosition position in this)
			{
				sb.AppendFormat("{0}\n", position.ToString());
			}
			return sb.ToString();
		}
	}

}