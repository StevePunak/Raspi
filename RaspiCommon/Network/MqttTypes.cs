using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.DeadReckoning;
using RaspiCommon.Spatial.LidarImaging;

namespace RaspiCommon.Network
{
	public delegate void LandscapeLandmarksReceivedHandler(ImageVectorList vectors);
	public delegate void LandscapeMetricsReceivedHandler(LandscapeMetrics metrics);

	public delegate void ImageLandmarksReceivedHandler(ImageVectorList vectors);
	public delegate void RangeBlobReceivedHandler(LidarVector[] vectors);
	public delegate void ImageMetricsReceivedHandler(ImageMetrics metrics);
	public delegate void ChassisMetricsReceivedHandler(ChassisMetrics metrics);
	public delegate void BearingReceivedHandler(Double bearing);
	public delegate void EnvironmentInfoReceivedHandler(EnvironmentInfo metrics);
	public delegate void PointCloudReceivedHandler(PointCloud2D pointCloud);
	public delegate void FuzzyPathReceivedHandler(FuzzyPath path);
	public delegate void CameraImageReceivedHandler(List<String> imageFiles);
	public delegate void CameraImagesAnalyzedHandler(ImageAnalysis analysis);

	public delegate void DeadReckoningEnvironmentReceivedHandler(DeadReckoningEnvironment environment);

	public delegate void CommandReceivedHandler(String command);

	public class MqttTypes
	{
		// physical landscape
		public const String LandscapeMetricsTopic = "trackbot/landscape/metrics";
		public const String DistanceAndBearingTopic = "trackbot/landscape/distances";

		// dead reckoning landscape
		public const String DeadReckoningCompleteLandscapeTopic = "trackbot/deadreckoning/fulllandscape";

		// widgets
		public const String BearingTopic = "trackbot/widgets/compass/bearing";
		public const String ChassisMetricsTopic = "trackbot/widgets/chassis/metrics";

		// lidar
		public const String RangeBlobTopic = "trackbot/lidar/rangeblob";
		public const String LandmarksTopic = "trackbot/lidar/landmarks";
		public const String BarriersTopic = "trackbot/lidar/barriers";
		public const String CurrentPathTopic = "trackbot/lidar/currentpath";
		public const String ImageMetricsTopic = "trackbot/lidar/metrics";
		public const String FuzzyPathTopic = "trackbot/lidar/fuzzypath";

		// control
		public const String CommandsTopic = "trackbot/control/commands";

		// optical camera
		public const String CameraLastImageTopic = "trackbot/camera/lastimage";
		public const String CameraLastAnalysisTopic = "trackbot/camera/analysiscomplete";
	}
}
