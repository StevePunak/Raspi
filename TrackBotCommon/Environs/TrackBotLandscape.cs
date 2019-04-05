using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Database;
using KanoopCommon.Geometry;
using RaspiCommon;
using RaspiCommon.Data.DataSource;
using RaspiCommon.Data.Entities;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.Imaging;

namespace TrackBotCommon.Environs
{
	public class TrackBotLandscape : Landscape, ILandscape
	{
		public PointD CurrentLocation { get; set; }

		public SqlDBCredentials Credentials { get; set; }

		public TrackDataSource DataSource { get; set; }
		
		public TrackBotLandscape()
		{
		}

		public TrackBotLandscape(SqlDBCredentials credentials)
			: base()
		{
			Credentials = credentials;
			DataSource = DataSourceFactory.Create<TrackDataSource>(Credentials);
		}

		public LandmarkList CreateLandmarksFromImageVectors(PointD landscapeOrigin, Double scale, ImageVectorList imageVectors)
		{
			LandmarkList output = new LandmarkList();
			foreach(ImageVector imageVector in imageVectors)
			{
				ImageVector scaled = imageVector.Clone();
				scaled.Scale(scale);
				Landmark landmark = new Landmark(landscapeOrigin, scaled)
				{
					Landscape = this
				};

				output.Add(landmark);
			}
			return output;
		}

		public void LoadLandmarks()
		{
			LandmarkList landmarks;
			if(DataSource.LandmarksGet(this, out landmarks).ResultCode == DBResult.Result.Success)
			{
				Landmarks = landmarks;
			}
		}

		public void SaveLandmarks()
		{
			foreach(Landmark landmark in Landmarks)
			{
				if(DataSource.LandmarkInsert(landmark).ResultCode != DBResult.Result.Success)
				{
					throw new RaspiException("Could not insert landmark");
				}
			}
		}

		public void ReplaceAllLandmarks()
		{
			if(DataSource.LandmarksClear(this).ResultCode == DBResult.Result.Success)
			{
				SaveLandmarks();
			}
		}
	}
}
