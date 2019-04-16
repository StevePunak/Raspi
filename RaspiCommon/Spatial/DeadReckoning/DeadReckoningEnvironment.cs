using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Database;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon.Network;

namespace RaspiCommon.Spatial.DeadReckoning
{
	public class DeadReckoningEnvironment
	{
		public event DeadReckoningEnvironmentReceivedHandler EnvironmentChanged;

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

			EnvironmentChanged += delegate {};
		}

		public DeadReckoningEnvironment(byte[] serialized)
			: this()
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

		public void ProcessEnvironment(PointCloud2D cloud)
		{
			foreach(BearingAndRange vector in cloud)
			{
				if(vector.Range > 0)
				{
					for(Double range = Scale;range < vector.Range;range += Scale)
					{
						PointD point = GetGridPoint(CurrentLocation, vector.Bearing, range);
						Log.SysLogText(LogLevel.DEBUG, "Clearing {0}", point);
						if(Grid[point.X, point.Y].State == CellState.Unknown)
						{
							Grid[point.X, point.Y].State = CellState.Unoccupied;
						}
					}
					// lay the wall down
					{
						PointD point = GetGridPoint(CurrentLocation, vector.Bearing, vector.Range);
						Log.SysLogText(LogLevel.DEBUG, "Putting wall at {0}", point);
						Grid[point.X, point.Y].State = CellState.Occupied;
					}
				}
			}

			EnvironmentChanged(this);
		}

		public void Move(Double bearing, Double range)
		{
			SetCurrentLocation(CurrentLocation.GetPointAt(bearing, range));
		}

		public void SetCurrentLocation(PointD location)
		{
			Log.SysLogText(LogLevel.DEBUG, "Setting DR current location to {0}", location);
			Grid[location.X, location.Y].State = CellState.Unoccupied;
			CurrentLocation = location;
		}

		public PointD GetGridPoint(PointD origin, Double bearing, Double range)
		{
			PointD point = origin.GetPointAt(bearing, range);
			if(!(point.X < Width && point.Y < Height && point.X >= 0 && point.Y >= 0))
			{
				point = PointD.Empty;
			}
			return point;
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(AngularOffset);
				bw.Write(Scale);
				bw.Write(Origin != null ? Origin.Serialize() : PointD.Empty.Serialize());
				bw.Write(CurrentLocation != null ? CurrentLocation.Serialize() : PointD.Empty.Serialize());
				bw.Write(Grid.Serialize());
			}
			return serialized;
		}

		public override string ToString()
		{
			return String.Format("{0} At {1} ({2} X {3})", Name, CurrentLocation, Width.ToMetersString(), Height.ToMetersString());
		}
	}
}
