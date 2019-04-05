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
	public class LandmarkList : List<Landmark> {}

	public class Landmark
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
