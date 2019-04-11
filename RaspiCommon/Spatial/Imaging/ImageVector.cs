using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Serialization;

namespace RaspiCommon.Spatial.Imaging
{
	public class ImageVectorList : List<ImageVector>
	{
		public ImageVectorList()
			: base() { }

		public ImageVectorList(byte[] landmarks)
			: base()
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(landmarks)))
			{
				for(int x = 0;x < landmarks.Length / ImageVector.ByteArraySize;x++)
				{
					ImageVector landmark = new ImageVector(br.ReadBytes(ImageVector.ByteArraySize));
					Add(landmark);
				}
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ImageVector.ByteArraySize * Count];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				foreach(ImageVector landmark in this)
				{
					bw.Write(landmark.Serialize());
				}
			}
			return serialized;
		}

		public bool HaveSameOrigin
		{
			get
			{
				bool result = true;
				if(Count > 2)
				{
					PointD origin = null;
					foreach(ImageVector landmark in this)
					{
						if(origin == null)
						{
							origin = landmark.Origin;
						}
						else if(landmark.Origin.Equals(origin) == false)
						{
							result = false;
							break;
						}
					}
				}
				return result;
			}
		}

		public PointDList GetPoints()
		{
			PointDList points = new PointDList();
			foreach(ImageVector landmark in this)
			{
				points.Add(landmark.GetPoint());
			}
			return points;
		}

		public ImageVector GetCentroid(PointD origin)
		{
			if(HaveSameOrigin == false)
			{
				throw new GeometryException("Can't get centroid with disimilar origins");
			}
			PointD centroid = GetPoints().Centroid;
			return new ImageVector(origin, new BearingAndRange(origin, centroid));
		}

		public bool Contains(PointD landmark, Double withinMeters, Double scale)
		{
			Double withinPixels = withinMeters * scale;
			return this.Find(l => l.Origin.DistanceTo(landmark) <= withinPixels) != null;
		}

		public bool Contains(PointD landmark, Double withinRange)
		{
			return this.Find(l => l.GetPoint().DistanceTo(landmark) <= withinRange) != null;
		}

		public void Scale(Double scale)
		{
			foreach(ImageVector vector in this)
			{
				vector.Scale(scale);
			}
		}

		public void DumpToLog()
		{
			Log.SysLogText(LogLevel.DEBUG, "Dumping Landmarks");
			foreach(ImageVector landmark in this)
			{
				Log.SysLogText(LogLevel.DEBUG, "{0}", landmark);
			}
		}

		public ImageVectorList Clone()
		{
			ImageVectorList list = new ImageVectorList();
			foreach(ImageVector landmark in this)
			{
				list.Add(landmark.Clone());
			}
			return list;
		}
	}

	public class ImageVectorGroupList : List<ImageVectorList>
	{
		public ImageVectorList FindGroupWithinRangeOfPoint(PointD point, Double range)
		{
			ImageVectorList groupList = null;
			foreach(ImageVectorList group in this)
			{
				if(group.Contains(point, range))
				{
					groupList = group;
					break;
				}
			}
			return groupList;
		}
	}

	[IsSerializable]
	public class ImageVector
	{
		public static int ByteArraySize = PointD.ByteArraySize + BearingAndRange.ByteArraySize;

		public PointD Origin { get; private set; }
		public BearingAndRange Vector { get; private set; }

		public Object Tag { get; set; }

		public ImageVector()
			: this(new PointD(), new BearingAndRange()) { }

		public ImageVector(PointD origin, BearingAndRange vector)
		{
			Origin = origin;
			Vector = vector;
		}

		public void Scale(Double scale)
		{
			Vector.Range *= scale;
		}

		public ImageVector(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				Origin = new PointD(br.ReadBytes(PointD.ByteArraySize));
				Vector = new BearingAndRange(br.ReadBytes(BearingAndRange.ByteArraySize));
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Origin.Serialize());
				bw.Write(Vector.Serialize());
			}
			return serialized;
		}

		public PointD GetPoint()
		{
			return Origin.GetPointAt(Vector.Bearing, Vector.Range);
		}

		public ImageVector Clone()
		{
			return new ImageVector(Origin.Clone(), Vector.Clone());
		}

		public override string ToString()
		{
			return String.Format("Landmark {0} @ {1}", Vector, GetPoint());
		}
	}
}
