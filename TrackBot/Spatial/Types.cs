using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.Spatial
{
	public enum ActivityType
	{
		None,
		RoamAndSeekUS,
		TravelLongestPath,
		GoToDestination,
		FindTwoLEDs
	}

	public enum CellContents
	{
		Unknown = 0,
		Empty = 1,
		Barrier = 2,

		Special
	}
}
