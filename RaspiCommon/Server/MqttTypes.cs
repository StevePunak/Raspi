using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.Imaging;

namespace RaspiCommon.Server
{
	public delegate void LandscapeLandmarksReceivedHandler(ImageVectorList vectors);
	public delegate void LandscapeMetricsReceivedHandler(LandscapeMetrics metrics);

	public delegate void ImageLandmarksReceivedHandler(ImageVectorList vectors);
	public delegate void RangeBlobReceivedHandler(LidarVector[] vectors);
	public delegate void ImageMetricsReceivedHandler(ImageMetrics metrics);
	public delegate void BearingReceivedHandler(Double bearing);
	public delegate void EnvironmentInfoReceivedHandler(EnvironmentInfo metrics);

	public class MqttTypes
	{
		public const String LandscapeMetricsTopic = "trackbot/landscape/metrics";
		public const String BearingTopic = "trackbot/compass/bearing";
		public const String RangeBlobTopic = "trackbot/lidar/rangeblob";
		public const String LandmarksTopic = "trackbot/lidar/landmarks";
		public const String BarriersTopic = "trackbot/lidar/barriers";
		public const String CurrentPathTopic = "trackbot/lidar/currentpath";
		public const String ImageMetricsTopic = "trackbot/lidar/metrics";
		public const String DistanceAndBearingTopic = "trackbot/environment/distances";

	}
}
