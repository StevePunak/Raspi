using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;
using KanoopCommon.Database;
using KanoopCommon.PersistentConfiguration;
using RaspiCommon.Devices.GamePads;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial.LidarImaging;

namespace RaspiCommon
{
	public class RaspiConfig : ProgramConfiguration
	{
		const String CONFIG_FILE = "/var/robo/robo.config";

		public static RaspiConfig Instance { get; private set; }

		// widgets selection for this node
		public bool ServoControllerEnabled { get; set; }
		public bool PanTiltEnabled { get; set; }
		public bool ChassisEnabled { get; set; }
		public bool DatabaseEnabled { get; set; }
		public bool CommandServerEnabled { get; set; }
		public bool RangeFindersEnabled { get; set; }
		public bool TracksEnabled { get; set; }
		public bool LidarEnabled { get; set; }
		public bool PhysicalCompassEnabled { get; set; }
		public bool MqttCompassEnabled { get; set; }
		public bool ActivitiesEnabled { get; set; }
		public bool LiftEnabled { get; set; }
		public bool CameraEnabled { get; set; }
		public bool SaveImageThreadEnabled { get; set; }
		public bool SpatialPollingEnabled { get; set; }
		public bool DeadReckoningEnvironmentEnabled { get; set; }
		public bool RobotArmEnabled { get; set; }
		public bool TelemetryServerEnabled { get; set; }

		public bool SendRangeImagingTelemetry { get; set; }
		public bool SendSpeedAndBearingTelemetry { get; set; }
		public bool SendChassisTelemetry { get; set; }
		public bool SendCompassBearingTelemetry { get; set; }

		public String LogFileName
		{
			get { return "/var/log/robo/robo.log";  }
		}

		String _lastAuthToken;
		public String LastAuthToken
		{
			get
			{
				if(_lastAuthToken == null)
				{
					_lastAuthToken = String.Empty;
				}
				return _lastAuthToken;
			}
			set { _lastAuthToken = value; }
		}

		Dictionary<String, GpioPin> _pinSelectionControls;
		public Dictionary<String, GpioPin> PinSelectionControls
		{
			get
			{
				if(_pinSelectionControls == null)
				{
					_pinSelectionControls = new Dictionary<String, GpioPin>();
				}
				return _pinSelectionControls;
			}
			set { _pinSelectionControls = value; }
		}

		public int TracksSpeed { get; set; }

		public GpioPin TracksRightA1Pin { get; set; }
		public GpioPin TracksRightA2Pin { get; set; }
		public GpioPin TracksRightB1Pin { get; set; }
		public GpioPin TracksRightB2Pin { get; set; }
		public GpioPin TracksRightEnaPin { get; set; }
		public GpioPin TracksRightDirectionPin { get; set; }
		public GpioPin TracksRightStepPin { get; set; }
		public GpioPin TracksLeftA1Pin { get; set; }
		public GpioPin TracksLeftA2Pin { get; set; }
		public GpioPin TracksLeftB1Pin { get; set; }
		public GpioPin TracksLeftB2Pin { get; set; }
		public GpioPin TracksLeftEnaPin { get; set; }
		public GpioPin TracksLeftDirectionPin { get; set; }
		public GpioPin TracksLeftStepPin { get; set; }
		public bool TracksHardwarePWM { get; set; }

		public GpioPin EscControlPinPin { get; set; }
		public GpioPin RangeFinderInputPin { get; set; }
		public GpioPin ServoPin { get; set; }
		public GpioPin StepperB1Pin { get; set; }
		public GpioPin StepperB2Pin { get; set; }
		public GpioPin StepperA1Pin { get; set; }
		public GpioPin StepperA2Pin { get; set; }

		public GpioPin LiftInputPin1 { get; set; }
		public GpioPin LiftInputPin2 { get; set; }
		public GpioPin LiftInputPin3 { get; set; }
		public GpioPin LiftInputPin4 { get; set; }

		public Double CompassXAdjust { get; set; }
		public Double CompassYAdjust { get; set; }
		public Double MagneticDeviation { get; set; }

		public Double BearingFuzz { get; set; }

		public Double StopDistance { get { return .15; } }
		public TimeSpan RampUp { get { return TimeSpan.FromMilliseconds(50); } }
		public TimeSpan RampDown { get { return TimeSpan.FromMilliseconds(50); } }
		public Double MaxRangeDetect { get { return .5; } }
		public Double ShortRangeClearance { get { return .3; } }

		public Double LidarOffsetDegrees { get; set; }
		public int StandardSpeed { get; set; }
		public Double StoppingDistance { get; set; }
		public int PathsToGet { get; set; }

		Dictionary<RFDir, GpioPin> _rangeFinderOutputPins;
		public Dictionary<RFDir, GpioPin>  RangeFinderEchoPins
		{
			get
			{
				if(_rangeFinderOutputPins == null)
				{
					_rangeFinderOutputPins = new Dictionary<RFDir, GpioPin>();
				}
				return _rangeFinderOutputPins;
			}
			set { _rangeFinderOutputPins = value; }
		}

		Dictionary<int, Double> _metersPerSecondAtPower;
		public Dictionary<int, Double> MetersPerSecondAtPower
		{
			get
			{
				if(_metersPerSecondAtPower == null)
				{
					_metersPerSecondAtPower = new Dictionary<int, double>();
				}
				return _metersPerSecondAtPower;
			}
			set { _metersPerSecondAtPower = value; }
		}

		public String LidarComPort { get; set; }
		public String LidarServer { get; set; }

		public Double LidarMetersSquare { get { return 10; } }
		public Double LidarPixelsPerMeter { get { return 50; } }
		public Double RangeFuzz { get; set; }
		public GpioPin LidarSpinEnablePin { get; set; }

		ImageVectorList _landmarks;
		public ImageVectorList Landmarks
		{
			get
			{
				if(_landmarks == null)
				{
					_landmarks = new ImageVectorList();
				}
				return _landmarks;
			}
			set { _landmarks = value; }
		}

		String _saveImageLocation;
		public String SaveImageLocation
		{
			get
			{
				if(_saveImageLocation == null)
				{
					_saveImageLocation = "/var/www/html/grid.png";
				}
				return _saveImageLocation;
			}
			set { _saveImageLocation = value; }
		}

		String _botImage;
		public String BotImage
		{
			get
			{
				if(_botImage == null)
				{
					_botImage = "tank.png";
				}
				return _botImage;
			}
			set { _botImage = value; }
		}

		public String MqttPublicHost { get; set; }
		public String MqttClusterHost { get; set; }

		SqlDBCredentials _DBCredentials;
		public SqlDBCredentials DBCredentials
		{
			get
			{
				if(_DBCredentials == null)
				{
					_DBCredentials = new SqlDBCredentials(SqlDataSource.MYSQL_NATIVE_DRIVER, "raspi", "trackbot", "raspi", "pi");
				}
				return _DBCredentials;
			}
			set { _DBCredentials = value; }
		}

		SqlDBCredentials _facialDBCredentials;
		public SqlDBCredentials FacialDBCredentials
		{
			get
			{
				if(_facialDBCredentials == null)
				{
					_facialDBCredentials = new SqlDBCredentials(SqlDataSource.MYSQL_NATIVE_DRIVER, "thufir", "facial", "trackbot", "trackbot");
				}
				return _facialDBCredentials;
			}
			set { _facialDBCredentials = value; }
		}

		String _landscapeName;
		public String LandscapeName
		{
			get
			{
				if(_landscapeName == null)
				{
					_landscapeName = "Man Cave";
				}
				return _landscapeName;
			}
			set { _landscapeName = value; }
		}

		String _snapshotUrl;
		public String SnapshotUrl { get { if(String.IsNullOrEmpty(_snapshotUrl)) _snapshotUrl = "http://raspi:8085/?action=snapshot"; return _snapshotUrl;  } set { _snapshotUrl = value; } }

		public Size LastRadarWindowSize { get; set; }
		public Point LastRadarWindowLocation { get; set; }
		public int SplitRadarPosition { get; set; }
		public int SplitTopToBottomPosition { get; set; }
		public int SplitLeftToRightPosition { get; set; }

		ProcessingMetrics _processingMetrics;
		public ProcessingMetrics ProcessingMetrics
		{
			get
			{
				if(_processingMetrics == null)
				{
					_processingMetrics = new ProcessingMetrics();
				}
				return _processingMetrics;
			}
			set { _processingMetrics = value; }
		}

		public String DeadReckoningEnvironmentName { get; set; }
		public int LastAnalyticsTabIndex { get; set; }
		public String RemoteImageDirectory { get; set; }

		public ColorThreshold BlueThresholds { get; set; }
		public ColorThreshold GreenThresholds { get; set; }
		public ColorThreshold RedThresholds { get; set; }
		public String EigenRecognizerFile { get; set; }
		public String LBPHRecognizerFile { get; set; }
		public String FisherRecognizerFile { get; set; }
		public Size FaceDetectSize { get { return new Size(100, 100); } }
		public Size FaceRecognizeSize { get { return new Size(300, 300); } }

		RaspiCameraParameters _cameraParameters;
		public RaspiCameraParameters CameraParameters
		{
			get
			{
				if(_cameraParameters == null)
				{
					_cameraParameters = new RaspiCameraParameters();
				}
				return _cameraParameters;
			}
			set { _cameraParameters = value; }
		}

		public Double CameraBearingOffset { get; set; }
		public int IgnoreRows { get; set; }

		public int LeftTrackAdjust { get; set; }
		public int RightTrackAdjust { get; set; }

		public GpioPin ClawRotationPin { get; set; }
		public GpioPin ClawLeftPin { get; set; }
		public GpioPin ClawRightPin { get; set; }
		public GpioPin ClawPin { get; set; }

		public GpioPin PanPin { get; set; }
		public GpioPin TiltPin { get; set; }

		public String FaceCascadeFile { get; set; }
		public String BatonCascadeFile { get; set; }

		public int ClawRotationPinMin { get; set; }
		public int ClawRotationPinMax { get; set; }
		public int ClawLeftPinMin { get; set; }
		public int ClawLeftPinMax { get; set; }
		public int ClawRightPinMin { get; set; }
		public int ClawRightPinMax { get; set; }
		public int ClawPinMin { get; set; }
		public int ClawPinMax { get; set; }

		public int PanHomePosition { get; set; }
		public int TiltHomePosition { get; set; }

		public int ClawLeftHomePosition { get; set; }
		public int ClawRightHomePosition { get; set; }
		public int ClawRotationHomePosition { get; set; }
		public int ClawClawHomePosition { get; set; }

		public GamePadType GamePadType { get; set; }

		public LEDTravelHistoryEntryList LEDTravelHistory { get; set; }

		public bool FullScaleImages { get; set; }

		public static String GetDefaultConfigFileName()
		{
			return Environment.OSVersion.Platform == PlatformID.Unix
				? CONFIG_FILE
				: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "robo.config");
		}

		public void SetDefaults()
		{
#if zero
			RemoteImageDirectory = "/home/pi/images";
			RadarHost = "192.168.0.50";
			BlueThresholds = new ColorThreshold(Color.Blue, 150, 70);
			GreenThresholds = new ColorThreshold(Color.Green, 150, 100);
			RedThresholds = new ColorThreshold(Color.Red, 150, 70);
			CameraParameters = new RaspiCameraParameters();

			CameraBearingOffset = 4;

#endif
			LidarComPort = "/dev/ttyS0";
			LidarServer = "raspi:5959";

			// 11 13 5    10 9 4
			TracksLeftA1Pin = GpioPin.Pin10;
			TracksLeftA2Pin = GpioPin.Pin09;
			TracksLeftEnaPin = GpioPin.Pin04;

			TracksRightA1Pin = GpioPin.Pin11;
			TracksRightA2Pin = GpioPin.Pin13;
			TracksRightEnaPin = GpioPin.Pin05;

			ClawRotationPin = GpioPin.Pin18;
			ClawLeftPin = GpioPin.Pin22;
			ClawRightPin = GpioPin.Pin27;
			ClawPin = GpioPin.Pin17;

			RangeFinderEchoPins = new Dictionary<RFDir, GpioPin>()
			{
				{ RFDir.Front,      GpioPin.Pin26 },
				{ RFDir.Rear,       GpioPin.Pin16 },
			};

			ClawRotationPinMin = 1000;
			ClawRotationPinMax = 2000;
			ClawLeftPinMin = 800;
			ClawLeftPinMax = 1700;
			ClawRightPinMin = 800;
			ClawRightPinMax = 2200;
			ClawPinMin = 800;
			ClawPinMax = 1800;

			PanPin = GpioPin.Pin06;
			TiltPin = GpioPin.Pin19;

			PanHomePosition = 54;
			TiltHomePosition = 58;

			ClawLeftHomePosition = 50;
			ClawRightHomePosition = 50;
			ClawRotationHomePosition = 58;
			ClawClawHomePosition = 50;

			LidarSpinEnablePin = GpioPin.Pin24;

			SnapshotUrl = "http://127.0.0.1:8085/?action=snapshot";

			EigenRecognizerFile = Path.Combine(RaspiPaths.ClassifyRoot, "faces.eigen");
			LBPHRecognizerFile = Path.Combine(RaspiPaths.ClassifyRoot, "faces.lbph");
			FisherRecognizerFile = Path.Combine(RaspiPaths.ClassifyRoot, "faces.fisher");
			FaceCascadeFile = Path.Combine(RaspiPaths.ClassifyRoot, "haarcascade_frontalface_default.xml");
			MqttClusterHost = "raspi2";
			MqttPublicHost = "thufir";
			MqttPublicHost = "192.168.0.50";


			if(Environment.MachineName.Contains("raspi1"))
			{
				SetConfigDefaultsRaspi1();
			}
			if(Environment.MachineName.Contains("raspi2"))
			{
				SetConfigDefaultsRaspi2();
			}
			Save();
#if zero
			CameraParameters.SnapshotDelay = TimeSpan.FromMilliseconds(1500);
#endif
		}

		void SetConfigDefaultsRaspi1()
		{
			ServoControllerEnabled = true;
			PanTiltEnabled = true;
			ChassisEnabled = true;
			DatabaseEnabled = true;
			CommandServerEnabled = true;
			RangeFindersEnabled = true;
			TracksEnabled = true;
			PhysicalCompassEnabled = false;
			MqttCompassEnabled = true;
			LidarEnabled = true;
			ActivitiesEnabled = true;
			LiftEnabled = false;
			CameraEnabled = false;
			SaveImageThreadEnabled = true;
			SpatialPollingEnabled = true;
			DeadReckoningEnvironmentEnabled = true;
			RobotArmEnabled = true;
			TelemetryServerEnabled = true;
			SendRangeImagingTelemetry = true;
			SendSpeedAndBearingTelemetry = true;
			SendCompassBearingTelemetry = true;
			SendChassisTelemetry = true;
		}

		void SetConfigDefaultsRaspi2()
		{
			ServoControllerEnabled = false;
			PanTiltEnabled = false;
			ChassisEnabled = false;
			DatabaseEnabled = false;
			CommandServerEnabled = true;
			RangeFindersEnabled = false;
			TracksEnabled = false;
			PhysicalCompassEnabled = false;
			MqttCompassEnabled = true;
			LidarEnabled = false;
			ActivitiesEnabled = false;
			LiftEnabled = false;
			CameraEnabled = true;
			SaveImageThreadEnabled = false;
			SpatialPollingEnabled = false;
			DeadReckoningEnvironmentEnabled = false;
			RobotArmEnabled = false;
			TelemetryServerEnabled = true;
			SendRangeImagingTelemetry = false;
			SendSpeedAndBearingTelemetry = false;
			SendChassisTelemetry = false;
			SendCompassBearingTelemetry = false;
		}

		public RaspiConfig()
		{
			Instance = this;
		}
	}
}
