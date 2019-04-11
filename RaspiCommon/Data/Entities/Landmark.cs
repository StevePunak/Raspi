using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Database;
using KanoopCommon.Geometry;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial.Imaging;

namespace RaspiCommon.Data.Entities
{
	public class LandmarkPairList : List<LandmarkPair> { }

	public class LandmarkPair
	{
		public Landmark L1 { get; set; }
		public Landmark L2 { get; set; }

		public Double Span { get { return L1.Location.DistanceTo(L2.Location); } }

		public LandmarkPair(Landmark l1, Landmark l2)
		{
			L1 = l1;
			L2 = l2;
		}

		public override string ToString()
		{
			return String.Format("{0} to {1}  span: {2:0.000}m", L1, L2, Span);
		}
	}

	public class LandmarkList : List<Landmark>
	{
		public Landmark GetLandmarkNear(PointD other, Double maxDistance)
		{
			Landmark landmark = this.Find(l => l.Location.DistanceTo(other) <= maxDistance);
			return landmark;
		}

		public LandmarkPairList GetAllPossiblePairs()
		{
			LandmarkPairList list = new LandmarkPairList();
			Landmark l1, l2;

			for(int x = 0;x < Count;x++)
			{
				l1 = this[x];
				for(int y = x;y < Count;y++)
				{
					l2 = this[y];
					if(l1 == l2)
						continue;

					list.Add(new LandmarkPair(l1, l2));
				}
			}
			return list;
		}

		public LandmarkPair GetClosestPairAtSpan(Double span, Double fuzz)
		{
			LandmarkPair pair = null;
			LandmarkPairList list = GetPairsAtSpan(span, fuzz);
			if(list.Count > 0)
			{
				pair = list.Find(l => l.Span == list.Min(l2 => l2.Span));
			}
			return pair;
		}

		public LandmarkPairList GetPairsAtSpan(Double span, Double fuzz)
		{
			LandmarkPairList list = new LandmarkPairList();
			LandmarkPairList allPossible = GetAllPossiblePairs();
			foreach(LandmarkPair pair in allPossible)
			{
				Double diff = Math.Abs(span - pair.Span);
				if(diff <= fuzz)
				{
					list.Add(pair);
				}
			}
			return list;
		}
	}

	public class Landmark : IHasPrimaryID
	{
		[ColumnName("landmark_id")]
		public UInt32 LandmarkID { get; set; }
		[ColumnName("landscape_id")]
		public UInt32 LandscapeID { get; set; }
		[ColumnName("location")]
		public PointD Location { get; set; }
		[ColumnName("label")]
		public String Label { get; set; }
		[ColumnName("create_date")]
		public DateTime CreateDate { get; set; }

		public virtual UInt32 PrimaryID { get { return LandmarkID; } }

		public Landscape Landscape { get; set; }

		public Landmark()
			: this(0, 0, PointD.Empty, String.Empty) { }

		public Landmark(PointD origin, ImageVector vector)
			: this(0, 0, origin.GetPointAt(vector.Vector.Bearing, vector.Vector.Range), String.Empty) {}

		public Landmark(UInt32 landmarkID, UInt32 landscapeID, PointD location, String label)
		{
			LandmarkID = landmarkID;
			LandscapeID = landscapeID;
			Location = location;
			Label = label;
		}

		public override string ToString()
		{
			return String.Format("LM{0}@ {1}", 
				String.IsNullOrEmpty(Label) 
				? String.Empty : String.Format(" {0} ", Label),
				Location);
		}
	}

}
