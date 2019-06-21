using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using KanoopCommon.Database;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.Data.DataSource;
using RaspiCommon.Data.Entities.Track;
using RaspiCommon.Extensions;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.LidarImaging;

namespace TrackBotCommon.Environs
{
	public class TrackBotLandscape : Landscape, ILandscape
	{
		public PointD CurrentLocation { get; set; }
		public PointD PossibleLocation { get; set; }
		public List<Circle> Circles { get; set; }

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

		public void AnalyzeImageLandmarks()
		{
			LandmarkPairList markerPairs = PointMarkers.GetAllPossiblePairs();
			LandmarkPairList landmarkPairs = Landmarks.GetAllPossiblePairs();

			LandmarkPairList matches = new LandmarkPairList();

			foreach(LandmarkPair markerPair in markerPairs)
			{
				LandmarkPair closest = Landmarks.GetClosestPairAtSpan(markerPair.Span, .01);
				if(closest != null)
				{
					matches.Add(closest);
					// now the marker pair is the same pair as the pair in the landscape
				}
			}
		}

		public void PreliminaryAnalysis()
		{
			// debug method to dump info
			foreach(Landmark l1 in Landmarks)
			{
				foreach(Landmark l2 in Landmarks)
				{
					if(l1 == l2)
						continue;

					Log.SysLogText(LogLevel.DEBUG, "{0} is {1:0.000} away from {2}", l1, l1.Location.DistanceTo(l2.Location), l2);
				}
			}
		}

		#region Persistent Storage

		public void LoadLandmarks()
		{
			LandmarkList landmarks;
			if(DataSource.LandmarksGet(this, out landmarks).ResultCode == DBResult.Result.Success)
			{
				Landmarks = landmarks;
			}
			if(DataSource.PointMarkersGet(this, out landmarks).ResultCode == DBResult.Result.Success)
			{
				PointMarkers = landmarks;
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
			foreach(PointMarker pointMarker in PointMarkers)
			{
				pointMarker.Landscape = this;
				if(DataSource.PointMarkerInsert(pointMarker).ResultCode != DBResult.Result.Success)
				{
					throw new RaspiException("Could not insert pointMarker");
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

		#endregion

		#region Image Creation

		public Mat CreateImage(Double pixelsPerMeter, SpatialObjects landscapeObjects)
		{
			Mat image = new Mat((int)(MetersSquare * pixelsPerMeter), (int)(MetersSquare * pixelsPerMeter), DepthType.Cv8U, 3);
			image.SetTo(Drawing.Black);

			if((landscapeObjects & SpatialObjects.Landmarks) != 0)
			{
				DrawLandmarks(image, pixelsPerMeter, (landscapeObjects & SpatialObjects.Labels) != 0);
			}

			if((landscapeObjects & SpatialObjects.CurrentLocation) != 0)
			{
				DrawCurrentLocation(image, pixelsPerMeter);
			}

			if((landscapeObjects & SpatialObjects.PossibleLocation) != 0)
			{
				DrawPossibleLocation(image, pixelsPerMeter);
			}

			if((landscapeObjects & SpatialObjects.Circles) != 0)
			{
				DrawCircles(image, pixelsPerMeter);
			}

			return image;
		}

		private void DrawLandmarks(Mat image, Double pixelsPerMeter, bool drawLabels)
		{
			DrawingParameters drawing = Drawing.ObjectParms[SpatialObjects.Landmarks];
			foreach(Landmark landmark in Landmarks)
			{
				PointD position = landmark.Location.Clone();
				position.Scale(pixelsPerMeter);
				CvInvoke.Circle(image, position.ToPoint(), 4, drawing.Color, drawing.LineWidth);

				if(drawLabels)
				{
					PointD textPosition = position.Clone();
					textPosition.Y += 15;
					CvInvoke.PutText(image, landmark.ToString(), textPosition.ToPoint(), FontFace.HersheyPlain, .5, Drawing.ObjectParms[SpatialObjects.Labels].Color);
				}
			}
		}

		private void DrawCurrentLocation(Mat image, Double pixelsPerMeter)
		{
			DrawingParameters drawing = Drawing.ObjectParms[SpatialObjects.CurrentLocation];

			PointD position = CurrentLocation.Clone();
			position.Scale(pixelsPerMeter);
			image.DrawCross(position, 4, drawing.Color, drawing.LineWidth);
		}

		private void DrawPossibleLocation(Mat image, Double pixelsPerMeter)
		{
			if(PossibleLocation != null)
			{
				DrawingParameters drawing = Drawing.ObjectParms[SpatialObjects.PossibleLocation];

				PointD position = CurrentLocation.Clone();
				position.Scale(pixelsPerMeter);
				image.DrawCross(position, 4, drawing.Color, drawing.LineWidth);
			}
		}

		private void DrawCircles(Mat image, Double pixelsPerMeter)
		{
			if(Circles != null)
			{
				DrawingParameters drawing = Drawing.ObjectParms[SpatialObjects.Circles];
				foreach(Circle circle in Circles)
				{
					PointD center = circle.Center.Clone();
					center.Scale(pixelsPerMeter);

					Double radius = circle.Radius * pixelsPerMeter;
					image.DrawCircle(new Circle(center, radius), drawing.Color, drawing.LineWidth);
				}
			}
		}

		#endregion
	}
}
