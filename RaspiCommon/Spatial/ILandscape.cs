using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon.Data.Entities;
using RaspiCommon.Spatial.LidarImaging;

namespace RaspiCommon.Spatial
{
	public interface ILandscape
	{
		String Name { get; }
		PointD Center { get; }
		PointD CurrentLocation { get; set; }
		Double MetersSquare { get; }
		LandmarkList Landmarks { get; }

		// nv storage
		void LoadLandmarks();
		void SaveLandmarks();

		// landmark manipulation
		void ReplaceAllLandmarks();

		LandmarkList CreateLandmarksFromImageVectors(PointD landscapeOrigin, Double scale, ImageVectorList imageVectors);
	}
}
