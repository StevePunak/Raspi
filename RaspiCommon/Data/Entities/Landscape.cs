using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Database;
using KanoopCommon.Geometry;

namespace RaspiCommon.Data.Entities
{
	public class Landscape
	{
		[ColumnName("landscape_id")]
		public UInt32 LandscapeID { get; set; }
		[ColumnName("name")]
		public String Name { get; set; }
		[ColumnName("meters_square")]
		public Double MetersSquare { get; set; }
		[ColumnName("create_date")]
		public DateTime CreateDate { get; set; }

		public PointD Center { get { return new PointD(MetersSquare / 2, MetersSquare / 2); } }
		public LandmarkList Landmarks { get; set; }
		public LandmarkList PointMarkers { get; set; }

		public Landscape()
		{
			Landmarks = new LandmarkList();
			PointMarkers = new LandmarkList();
		}

		public override string ToString()
		{
			return String.Format("LANDSCAPE: {0}  {1:00}m²", Name, MetersSquare);
		}
	}
}
