using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Database;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;

namespace RaspiCommon.Spatial.DeadReckoning
{
	public class DeadReckoningEnvironment
	{
		public int ByteArraySize { get { return sizeof(Double) + sizeof(Double) + PointD.ByteArraySize + PointD.ByteArraySize + Grid.ByteArraySize; } }

		public DRGrid Grid { get; private set; }

		[ColumnName("angular_offset")]
		public Double AngularOffset { get; private set; }

		public PointD Origin { get; private set; }

		public PointD CurrentLocation { get; private set; }

		[ColumnName("scale")]
		public Double Scale { get; private set; }

		[ColumnName("name")]
		public String Name { get { return Grid.Name; } }

		[ColumnName("origin_x")]
		public Double _originX
		{
			set { Origin = new PointD(value, Origin.Y);  }
		}

		[ColumnName("origin_y")]
		public Double _originY
		{
			set { Origin = new PointD(Origin.X, value); }
		}

		[ColumnName("grid_id")]
		public UInt32 ID { get; set; }

		public Double Width { get { return Scale * (Double)Grid.Matrix.Width; } }
		public Double Height { get { return Scale * (Double)Grid.Matrix.Height; } }

		public DeadReckoningEnvironment()
			: this(String.Empty, 0, 0, 1, 0, PointD.Empty) {}

		public DeadReckoningEnvironment(String name, Double width, Double height, Double scale, Double angularOffset, PointD origin)
		{
			AngularOffset = angularOffset;
			Scale = scale;
			Origin = origin;

			Grid = new DRGrid(name, width, height, scale);
		}

		public DeadReckoningEnvironment(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				AngularOffset = br.ReadDouble();
				Scale = br.ReadDouble();
				Origin = new PointD(br.ReadBytes(PointD.ByteArraySize));
				CurrentLocation = new PointD(br.ReadBytes(PointD.ByteArraySize));
				Grid = new DRGrid(br);
			}
		}

		public void SetCurrentLocation(PointD location)
		{
			Grid[location.X, location.Y].State = CellState.Unoccupied;
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(AngularOffset);
				bw.Write(Scale);
				bw.Write(Origin.Serialize());
				bw.Write(CurrentLocation.Serialize());
				bw.Write(Grid.Serialize());
			}
			return serialized;
		}

		public override string ToString()
		{
			return String.Format("{0} {1} X {2}", Name, Width.ToMetersString(), Height.ToMetersString());
		}
	}
}
