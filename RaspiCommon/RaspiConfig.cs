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

		public Double LidarMetersSquare { get { return 10; } }
		public Double LidarPixelsPerMeter { get { return 50; } }
		public Double RangeFuzz { get; set; }

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

		public String RadarHost { get; set; }

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

		public int ClawRotationPinMin { get; set; }
		public int ClawRotationPinMax { get; set; }
		public int ClawLeftPinMin { get; set; }
		public int ClawLeftPinMax { get; set; }
		public int ClawRightPinMin { get; set; }
		public int ClawRightPinMax { get; set; }
		public int ClawPinMin { get; set; }
		public int ClawPinMax { get; set; }

		public GamePadType GamePadType { get; set; }

		public LEDTravelHistoryEntryList LEDTravelHistory { get; set; }

		public bool FullScaleImages { get; set; }

		public static String GetDefaultConfigFileName()
		{
			return Environment.OSVersion.Platform == PlatformID.Unix
				? CONFIG_FILE
				: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "robo.config");
		}
	}
}
