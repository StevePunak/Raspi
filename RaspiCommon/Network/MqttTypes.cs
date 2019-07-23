using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.Locomotion;
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
	public delegate void RangeBlobReceivedHandler(DateTime timestamp, LidarVector[] vectors);
	public delegate void ImageMetricsReceivedHandler(ImageMetrics metrics);
	public delegate void ChassisMetricsReceivedHandler(ChassisMetrics metrics);
	public delegate void SpeedAndBearingReceivedHandler(SpeedAndBearing speedAndBearing);
	public delegate void EnvironmentInfoReceivedHandler(EnvironmentInfo metrics);
	public delegate void PointCloudReceivedHandler(PointCloud2D pointCloud);
	public delegate void FuzzyPathReceivedHandler(FuzzyPath path);
	public delegate void CameraImageReceivedHandler(List<String> imageFiles);
	public delegate void CameraImagesAnalyzedHandler(ImageAnalysis analysis);
	public delegate void LidarOffsetReceivedHandler(Double lidarOffset);

	public delegate void DeadReckoningEnvironmentReceivedHandler(DeadReckoningEnvironment environment);

	public delegate void PercentageCommandReceivedHandler(int percent);
	public delegate void RaspiCameraParametersReceivedHandler(RaspiCameraParameters parameters);

	public delegate void SpinStepReceivedHandler(SpinDirection spinDirection);

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
		public const String LidarMetricsTopic = "trackbot/widgets/lidar/metrics";

		// lidar
		public const String RangeBlobTopic = "trackbot/lidar/rangeblob";
		public const String LandmarksTopic = "trackbot/lidar/landmarks";
		public const String BarriersTopic = "trackbot/lidar/barriers";
		public const String CurrentPathTopic = "trackbot/lidar/currentpath";
		public const String ImageMetricsTopic = "trackbot/lidar/metrics";
		public const String FuzzyPathTopic = "trackbot/lidar/fuzzypath";

		// control
		public const String CommandsTopic = "trackbot/control/commands";
		public const String ArmRotationTopic = "trackbot/control/robotarm/rotation";
		public const String ArmElevationTopic = "trackbot/control/robotarm/elevation";
		public const String ArmThrustTopic = "trackbot/control/robotarm/thrust";
		public const String ArmClawTopic = "trackbot/control/robotarm/claw";
		public const String BotSpeedTopic = "trackbot/control/motion/speed";
		public const String BotSpinStepLeftDegreesTopic = "trackbot/control/motion/spinstep/left/degrees";
		public const String BotSpinStepRightDegreesTopic = "trackbot/control/motion/spinstep/right/degrees";
		public const String BotSpinStepLeftTimeTopic = "trackbot/control/motion/spinstep/left/ms";
		public const String BotSpinStepRightTimeTopic = "trackbot/control/motion/spinstep/right/ms";
		public const String BotMoveTimeTopic = "trackbot/control/motion/go/time";
		public const String BotTiltTopic = "trackbot/control/pantilt/tilt";
		public const String BotPanTopic = "trackbot/control/pantilt/pan";

		// optical camera
		public const String CameraLastImageTopic = "trackbot/camera/lastimage";
		public const String CameraLastAnalysisTopic = "trackbot/camera/analysiscomplete";
		public const String RaspiCameraSetParametersTopic = "trackbot/camera/set/parameters";

		// realtime feedback (telemetry)
		public const String SpeedAndBearingTopic = "trackbot/telemetry/speedandbearing";
		public const String Voltage1Topic = "trackbot/telemetry/voltage/1";

	}
}
