using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using KanoopCommon.Geometry;
using RaspiCommon.Extensions;

namespace Radar.MainDisplay
{
	public partial class RadarForm
	{
		void DrawRadarDisplay()
		{
			PointCloud2D cloud = _vectors != null ? _vectors.ToPointCloud2D() : _client.Vectors.ToPointCloud2D();

			// draw all the dots
			Mat bitmap = cloud.ToBitmap(_radarBitmap.Height, Color.Red, PixelsPerMeterImage);
			//bitmap.Save(@"c:\pub\tmp\lidar.bmp");
			PointD origin = bitmap.Center();

			// draw fuzzy path
			if(_client.FuzzyPath != null && _client.ChassisMetrics != null)
			{
				BearingAndRange leftBar = PointD.Empty.BearingAndRangeTo(_client.FuzzyPath.FrontLeft.Origin).Scale(PixelsPerMeterImage);
				BearingAndRange rightBar = PointD.Empty.BearingAndRangeTo(_client.FuzzyPath.FrontRight.Origin).Scale(PixelsPerMeterImage);
				PointD leftFront = origin.GetPointAt(leftBar);
				PointD rightFront = origin.GetPointAt(rightBar);
				//				PointD frontLeft = origin.GetPointAt(_client.FuzzyPath.FrontLeft.Origin.Scale(PixelsPerMeterImage));
				bitmap.DrawVectorLines(_client.FuzzyPath.FrontLeft, leftFront, PixelsPerMeterImage, Color.DarkGreen);
				bitmap.DrawVectorLines(_client.FuzzyPath.FrontRight, rightFront, PixelsPerMeterImage, Color.DarkGray);
			}

			Image<Bgr, Byte> imageCV = new Image<Bgr, byte>(Properties.Resources.tank);
			Mat tank = imageCV.Mat;
			tank = tank.Rotate(_client.Bearing);
			bitmap.DrawCenteredImage(tank, new PointD(250, 250));
			Bitmap bm = new Bitmap(bitmap.Bitmap);
			picLidar.BackgroundImage = bm;

		}
	}
}
