using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Database;

namespace RaspiCommon.Data.Entities
{
	public class PointMarker : Landmark
	{
		[ColumnName("point_marker_id")]
		public UInt32 PointMarkerID { get; set; }

		public override UInt32 PrimaryID { get { return PointMarkerID; } }

		public PointMarker()
		{
		}
	}
}
