using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Database;
using KanoopCommon.Geometry;

namespace RaspiCommon.Data.Entities
{
	public class DRGridLocation
	{
		[ColumnName("location_id")]
		public UInt32 LocationID { get; set; }
		[ColumnName("grid_id")]
		public UInt32 GridID { get; set; }
		[ColumnName("point")]
		public PointD Location { get; set; }
		[ColumnName("location_type")]
		public LocationType Type { get; set; }
		public DRGridLocation()
		{

		}

		public override string ToString()
		{
			return String.Format("{0} at {1}", Type, Location);
		}
	}
}
