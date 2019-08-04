using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Spatial
{
	public delegate void RangeHandler(Double range, bool valid);
	public delegate void NewBearingHandler(Double bearing);
	public delegate void CompassRawDataHandler(Double mx, Double my, Double mz);
	public delegate void NewDestinationBearingHandler(Double bearing);
	public delegate void DistanceToTravelHandler(Double range);
	public delegate void DistanceLeftHandler(Double range);

	[Flags]
	public enum SpatialObjects
	{
		Nothing =			0x00000000,

		Landmarks =			0x00000001,
		CurrentLocation =	0x00000002,
		GridLines =			0x00000004,
		Labels =			0x00000008,
		Distances =			0x00000010,
		Background =		0x00000020,
		PossibleLocation =  0x00000040,
		Circles =	        0x00000080,
		CenterPoint =		0x00000100,
		Barriers =			0x00000200,

		Everything =		0x7fffffff,
	}
}
