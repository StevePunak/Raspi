using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Serialization;

namespace RaspiCommon.Spatial.Imaging
{
	[IsSerializable]
	public class ProcessingMetrics
	{
		public Double CannyThreshold { get; set; }
		public Double CannyThresholdLinking { get; set; }

		public Double RhoRes { get; set; }
		public Double ThetaResPiDivisor { get; set; }
		public int HoughThreshold { get; set; }

		public Double MinimumLandmarkSegment { get; set; }       // line segments must be at least this many meters in order to be considered
		public Double MinimumConnectLength { get; set; }         // right angle rays must be within this many meters to connect
		public Double MaximumConnectLength { get; set; }         // will not connect right angle this far apart
		public Double RightAngleSlackDegrees { get; set; }       // slack from 90° which may still constitute right angle
		public Double MinimumLandmarkSeparation { get; set; }    // landmarks must be this many meters apart
		public Double BearingSlack { get; set; }                 // degrees slack when computing similar angles
		public Double LinePathWidth { get; set; }                // lines must be within this many meters to be consolidated into a single line

		public ProcessingMetrics()
		{
			CannyThreshold = 120;
			CannyThresholdLinking = 120;
			RhoRes = 2;
			ThetaResPiDivisor = 45;
			HoughThreshold = 20;
			MinimumLandmarkSegment = .05;        // line segments must be at least this many meters in order to be considered
			MinimumConnectLength = .025;         // right angle rays must be within this many meters to connect
			MaximumConnectLength = .2;           // will not connect right angle this far apart
			RightAngleSlackDegrees = 1;          // slack from 90° which may still constitute right angle
			MinimumLandmarkSeparation = .1;      // landmarks must be this many meters apart
			BearingSlack = 2;                    // degrees slack when computing similar angles
			LinePathWidth = .1;                  // lines must be within this many meters to be consolidated into a single line
		}
	}
}
