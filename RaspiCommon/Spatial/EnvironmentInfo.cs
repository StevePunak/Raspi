using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Spatial
{
	public class EnvironmentInfo : ICloneable
	{
		public Double ForwardPrimaryRange { get; set; }
		public Double BackwardPrimaryRange { get; set; }
		public Double ForwardSecondaryRange { get; set; }
		public Double BackwardSecondaryRange { get; set; }
		public Double Bearing { get; set; }
		public Double DestinationBearing { get; set; }
		public Double DistanceToTravel { get; set; }
		public Double DistanceLeft { get; set; }

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
