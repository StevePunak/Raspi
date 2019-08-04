using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Database;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using RaspiCommon;
using RaspiCommon.Data.DataSource;
using RaspiCommon.Data.Entities.Track;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Devices.Locomotion;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Devices.RobotArms;
using RaspiCommon.Devices.Spatial;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Network;
using RaspiCommon.PiGpio;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.DeadReckoning;
using RaspiCommon.Spatial.LidarImaging;
using TrackBot.ForkLift;
using TrackBot.Network;
using TrackBot.Servos;
using TrackBot.Spatial;
using TrackBot.Tracks;
using TrackBotCommon.Environs;

namespace TrackBot
{
	class Widgets : IWidgetCollection
	{
		public event RangeHandler ForwardPrimaryRange;
		public event RangeHandler BackwardPrimaryRange;
		public event RangeHandler ForwardSecondaryRange;
		public event RangeHandler BackwardSecondaryRange;
		public event NewDestinationBearingHandler NewDestinationBearing;
		public event DistanceToTravelHandler DistanceToTravel;
		public event DistanceLeftHandler DistanceLeft;
		public event NewBearingHandler BearingChanged;
		public event FuzzyPathChangedHandler FuzzyPathChanged;
		public event LandmarksChangedHandler LandmarksChanged;
		public event BarriersChangedHandler BarriersChanged;
		public event DeadReckoningEnvironmentReceivedHandler DeadReckoningEnvironmentReceived;
		public event CameraImagesAnalyzedHandler CameraImagesAnalyzed;

		public BotTracks Tracks { get; private set; }
		public TrackSpeed TrackSpeed { get { return new TrackSpeed() { LeftSpeed = Tracks.LeftSpeed, RightSpeed = Tracks.RightSpeed }; } } 
		public Dictionary<RFDir, HCSR04_RangeFinder> RangeFinders { get; private set; }
		public Lift Lift { get; private set; }

		public IImageEnvironment ImageEnvironment { get; private set; }
		public bool HasImageEnvironment { get { return ImageEnvironment !=null; } }

		public ILandscape Landscape { get; private set; }
		public SpatialPoll SpatialPollThread { get; private set; }

		public SaveImageThread SaveImageThread { get; set; }
		public TelemetryServer Server { get; private set; }
		public ServoController ServoController { get; private set; }

		public DeadReckoningEnvironment DeadReckoningEnvironment { get; private set; }

		public TrackDataSource DataSource { get; private set; }

		public RemoteMqttController CommandServer { get; private set; }

		public ICompass Compass { get; private set; }

		public Chassis Chassis { get; private set; }

		public MeArm RobotArm { get; private set; }

		public Camera Camera { get; private set; }
		public SolidColorAnalysis LEDImageAnalysis { get; private set; }

		public PanTilt PanTilt { get; private set; }

		static Widgets _instance;
		public static Widgets Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new Widgets();
				}
				return _instance;
			}
		}

		Widgets()
		{
			ForwardPrimaryRange += delegate {};
			BackwardPrimaryRange += delegate {};
			ForwardSecondaryRange += delegate {};
			BackwardSecondaryRange += delegate {};
			NewDestinationBearing += delegate {};
			DistanceToTravel += delegate {};
			DistanceLeft += delegate {};
			BearingChanged += delegate {};
			FuzzyPathChanged += delegate {};
			LandmarksChanged += delegate {};
			BarriersChanged += delegate {};
			DeadReckoningEnvironmentReceived += delegate {};
			CameraImagesAnalyzed += delegate {};

			Pigs.InterfaceType = Pigs.PigsType.Direct;
		}

		public void StartWidgets()
		{
			if(Program.Config.ServoControllerEnabled) 					StartServoController();
			if(Program.Config.PanTiltEnabled) 							StartPanTilt();
			if(Program.Config.ChassisEnabled) 							StartChassis();
			if(Program.Config.DatabaseEnabled) 							StartDatabase();
			if(Program.Config.CommandServerEnabled) 					StartCommandServer();
			if(Program.Config.RangeFindersEnabled) 						StartRangeFinders();
			if(Program.Config.TracksEnabled) 							StartTracks();
			if(Program.Config.PhysicalCompassEnabled) 					StartPhysicalCompass();
			if(Program.Config.MqttCompassEnabled)						StartMqttCompass();
			if(Program.Config.LidarEnabled) 							StartLidar();
			if(Program.Config.ActivitiesEnabled) 						StartActivities();
			if(Program.Config.LiftEnabled) 								StartLift();
			if(Program.Config.CameraEnabled) 							StartCamera();
			if(Program.Config.SaveImageThreadEnabled) 					StartSaveImageThread();
			if(Program.Config.SpatialPollingEnabled) 					StartSpatialPolling();
			if(Program.Config.DeadReckoningEnvironmentEnabled) 			StartDeadReckoningEnvironment();
			if(Program.Config.RobotArmEnabled) 							StartRobotArm();
			if(Program.Config.TelemetryServerEnabled)					StartTelemetryServer();
		}

		public void StopWidgets()
		{
			Log.SysLogText(LogLevel.INFO, "Stopping Widgets");
			if(Program.Config.TelemetryServerEnabled)					StopTelemetryServer();
			if(Program.Config.RobotArmEnabled) 							StopRobotArm();
			if(Program.Config.DeadReckoningEnvironmentEnabled) 			StopDeadReckoningEnvironment();
			if(Program.Config.CameraEnabled) 							StopCamera();
			if(Program.Config.CommandServerEnabled) 					StopCommandServer();
			if(Program.Config.SpatialPollingEnabled) 					StopSpatialPolling();
			if(Program.Config.LiftEnabled) 								StopLift();
			if(Program.Config.ActivitiesEnabled) 						StopActivities();
			if(Program.Config.TracksEnabled)							StopTracks();
			if(Program.Config.RangeFindersEnabled) 						StopRangeFinders();
			if(Program.Config.PhysicalCompassEnabled) 					StopPhysicalCompass();
			if(Program.Config.MqttCompassEnabled)						StopMqttCompass();
			if(Program.Config.LidarEnabled) 							StopLidar();
			if(Program.Config.SaveImageThreadEnabled) 					StopSaveImageThread();
			if(Program.Config.DatabaseEnabled) 							StopDatabase();
			if(Program.Config.ChassisEnabled) 							StopChassis();
			if(Program.Config.PanTiltEnabled) 							StopPanTilt();
			if(Program.Config.ServoControllerEnabled) 					StopServoController();

			Log.SysLogText(LogLevel.INFO, "Stopping Pigs");
			Pigs.Stop();

			Log.SysLogText(LogLevel.INFO, "Stopping remaining threads");
			foreach(ThreadBase thread in ThreadBase.GetRunningThreads())
			{
				Log.SysLogText(LogLevel.DEBUG, "Remaining: {0} of type {1}", thread, thread.GetType());
			}
			Log.SysLogText(LogLevel.INFO, "Widgets Stopped");
		}

		private void StartPanTilt()
		{
			PanTilt = new PanTilt(Program.Config.PanPin, Program.Config.TiltPin);
			Log.SysLogText(LogLevel.INFO, "Started {0}", PanTilt);
		}

		private void StopPanTilt()
		{
		}

		private void StartServoController()
		{
			Log.SysLogText(LogLevel.INFO, $"Starting Servo Controller");
			ServoController = new ServoController();
			ServoController.Start();
		}

		private void StopServoController()
		{
			ServoController.Stop();
		}

		private void StartDeadReckoningEnvironment()
		{
			Log.SysLogText(LogLevel.INFO, $"Starting DR Environs");
			TrackDataSource ds = DataSourceFactory.Create<TrackDataSource>(Program.Config.DBCredentials);
			DeadReckoningEnvironment environment;
			if(ds.GetDREnvironment(Program.Config.DeadReckoningEnvironmentName, out environment).ResultCode == DBResult.Result.Success)
			{
				Log.SysLogText(LogLevel.DEBUG, "Got DR Environment {0}", DeadReckoningEnvironment);
				DeadReckoningEnvironment = environment;
				DeadReckoningEnvironmentReceived(DeadReckoningEnvironment);

				DeadReckoningEnvironment.EnvironmentChanged += OnDeadReckoningEnvironmentChanged;
			}
			else
			{
				Log.SysLogText(LogLevel.DEBUG, "Could not get DR Environment");
			}
		}

		private void StopDeadReckoningEnvironment()
		{
			DeadReckoningEnvironment = null;
		}

		private void StartCamera()
		{
			Log.SysLogText(LogLevel.DEBUG, "INIT WIDGETS CAMERA PARAMETERS: {0}", Program.Config.CameraParameters);
			Camera = new MJpegStreamerCamera()
			{
				ConvertTo = ImageType.Bitmap,
				Parameters = Program.Config.CameraParameters,
				SnapshotUrl = Program.Config.SnapshotUrl,
			};
			LEDImageAnalysis = new SolidColorAnalysis(Camera);
			SolidColorAnalysis.SetThreshold(Program.Config.BlueThresholds);
			SolidColorAnalysis.SetThreshold(Program.Config.GreenThresholds);
			SolidColorAnalysis.SetThreshold(Program.Config.RedThresholds);
			SolidColorAnalysis.BearingOffset = Program.Config.CameraBearingOffset;
			SolidColorAnalysis.Compass = Compass;
			LEDImageAnalysis.CameraImagesAnalyzed += OnCameraImagesAnalyzed;
			Log.SysLogText(LogLevel.DEBUG, "Setting dir to '{0}'", Program.Config.RemoteImageDirectory);
			SolidColorAnalysis.ImageAnalysisDirectory = Program.Config.RemoteImageDirectory;
			Camera.Start();
			Camera.AutoSnap = true;
			CommandServer.RaspiCameraParameters += OnRaspiCameraParametersReceived;
		}

		private void StopCamera()
		{
			Camera.Stop();
			Camera = null;
		}

		private void StartChassis()
		{
			Log.SysLogText(LogLevel.INFO, $"Starting Chassis");
			Chassis = new XiaorTankTracks();
			Chassis.Points.Add(ChassisParts.Lidar, new PointD(Chassis.Points[ChassisParts.RearLeft].X + .115, Chassis.Points[ChassisParts.FrontRight].Y + .120));
			Chassis.Points.Add(ChassisParts.FrontRangeFinder, new PointD(Chassis.Width / 2, 0));
			Chassis.Points.Add(ChassisParts.RearRangeFinder, new PointD(Chassis.Width / 2, Chassis.Length));
		}

		private void StopChassis()
		{

		}

		private void StartRobotArm()
		{
			Log.SysLogText(LogLevel.DEBUG, "Starting robot arm");
			RobotArm = new MeArm(ServoController, Program.Config.ClawRotationPin, Program.Config.ClawLeftPin, Program.Config.ClawRightPin, Program.Config.ClawPin)
			{
				ClawPinMin = Program.Config.ClawPinMin,
				ClawPinMax = Program.Config.ClawPinMax,
				LeftPinMin = Program.Config.ClawLeftPinMin,
				LeftPinMax = Program.Config.ClawLeftPinMax,
				RotationPinMin = Program.Config.ClawRotationPinMin,
				RotationPinMax = Program.Config.ClawRotationPinMax,
				RightPinMin = Program.Config.ClawRightPinMin,
				RightPinMax = Program.Config.ClawRightPinMax,
			};
			CommandServer.ArmRotation += OnArmRotationCommand;
			CommandServer.ArmElevation += OnArmElevationCommand;
			CommandServer.ArmThrust += OnArmThrustCommand;
			CommandServer.ArmClaw += OnArmClawCommand;

			RobotArm.Home();
		}

		private void StopRobotArm()
		{
			Log.SysLogText(LogLevel.DEBUG, "Stopping robot arm");
			RobotArm.Stop();
		}

		private void StartCommandServer()
		{
			Console.WriteLine("Starting Mqqt Command Client");
			List <String> topics = new List<String>()
			{
				MqttTypes.CommandsTopic,
			};
			if(Program.Config.RobotArmEnabled)
			{
				topics.Add(MqttTypes.ArmClawTopic);
				topics.Add(MqttTypes.ArmElevationTopic);
				topics.Add(MqttTypes.ArmRotationTopic);
				topics.Add(MqttTypes.ArmThrustTopic);
			}
			if(Program.Config.CameraEnabled)
			{
				topics.Add(MqttTypes.RaspiCameraSetParametersTopic);
			}
			if(Program.Config.TracksEnabled)
			{
				topics.Add(MqttTypes.BotSpeedTopic);
				topics.Add(MqttTypes.BotSpinStepLeftDegreesTopic);
				topics.Add(MqttTypes.BotSpinStepRightDegreesTopic);
				topics.Add(MqttTypes.BotSpinStepLeftTimeTopic);
				topics.Add(MqttTypes.BotSpinStepRightTimeTopic);
				topics.Add(MqttTypes.BotMoveTimeTopic);
				topics.Add(MqttTypes.BotPanTopic);
				topics.Add(MqttTypes.BotTiltTopic);
			}
			CommandServer = new RemoteMqttController(String.Format("raspi.{0}", Environment.MachineName), topics);
			CommandServer.Start();
		}

		private void StopCommandServer()
		{
			CommandServer.Stop();
		}

		private void StartDatabase()
		{
			Log.SysLogText(LogLevel.INFO, $"Starting database connection");
			try
			{
				TrackBotLandscape landscape;
				DataSource = DataSourceFactory.Create<TrackDataSource>(Program.Config.DBCredentials);
				if(DataSource.LandscapeGet<TrackBotLandscape>("Man Cave", out landscape).ResultCode != DBResult.Result.Success)
				{
					throw new TrackBotException("Failed to get landscape");
				}
				Landscape = landscape;
			}
			catch(Exception e)
			{
				Console.WriteLine("Widgets Start Exception: {0}", e.Message);
			}
		}

		private void StopDatabase()
		{
		}

		private void StartTelemetryServer()
		{
			Log.SysLogText(LogLevel.INFO, $"Starting telemetry server");
			Server = new TelemetryServer(this, Program.Config.MqttPublicHost, "trackbot-lidar")
			{
				ImageDirectory = "/home/pi/images"
			};
			Server.Start();
		}

		private void StopTelemetryServer()
		{
			Server.Stop();
			Server = null;
		}

		private void StartSpatialPolling()
		{
			Log.SysLogText(LogLevel.INFO, $"Starting spatial polling");
			SpatialPollThread = new SpatialPoll();
			SpatialPollThread.Start();
		}

		private void StopSpatialPolling()
		{
			SpatialPollThread.Stop();
		}

		private void StartSaveImageThread()
		{
			Log.SysLogText(LogLevel.DEBUG, "Starting save image thread");
			SaveImageThread = new SaveImageThread();
			SaveImageThread.Start();
		}

		private void StopSaveImageThread()
		{
			SaveImageThread.Stop();
		}

		private void StartLift()
		{
			Log.SysLogText(LogLevel.INFO, $"Starting lift");
			Lift = new Lift();
			Lift.Start();
		}

		private void StopLift()
		{
			Lift.Stop();
		}

		private void StartMqttCompass()
		{
			Log.SysLogText(LogLevel.INFO, $"Starting MQTT compass");
			Compass = new MqttCompass(Program.Config.MqttClusterHost, MqttTypes.BearingTopic, MqttTypes.RawCompassDataTopic);
			Compass.Start();
			Compass.MagneticDeviation = Program.Config.MagneticDeviation;
		}

		private void StopMqttCompass()
		{
			Compass.Stop();
		}

		private void StartPhysicalCompass()
		{
			Log.SysLogText(LogLevel.INFO, "Sarting physical compass");
			LSM9DS1CompassAccelerometer gymag = new LSM9DS1CompassAccelerometer();
			gymag.MagneticDeviation = Program.Config.MagneticDeviation;
			gymag.XAdjust = Program.Config.CompassXAdjust;
			gymag.YAdjust = Program.Config.CompassYAdjust;
			gymag.NewBearing += OnNewBearing;
			Compass = gymag;
		}

		private void StopPhysicalCompass()
		{
		}

		private void StartLidar()
		{
			ImageEnvironment = new TrackLidar(Program.Config.LidarMetersSquare, Program.Config.LidarPixelsPerMeter, Program.Config.LidarSpinEnablePin, Compass);
			ImageEnvironment.BarriersChanged += OnImageEnvironment_BarriersChanged;
			ImageEnvironment.LandmarksChanged += OnImageEnvironment_LandmarksChanged;
			ImageEnvironment.FuzzyPathChanged += OnImageEnvironment_FuzzyPathChanged;

			ImageEnvironment.Start();
		}

		private void StopLidar()
		{
			ImageEnvironment.Stop();
		}

		private void OnNewBearing(double bearing)
		{
			BearingChanged(bearing);
			if(ImageEnvironment != null)
			{
				ImageEnvironment.Bearing = bearing;
			}
		}

		private void StartActivities()
		{
		}

		private void StopActivities()
		{
			Activity.StopActivity();
		}

		private void StartRangeFinders()
		{
			RangeFinders = new Dictionary<RFDir, HCSR04_RangeFinder>();
			Log.SysLogText(LogLevel.DEBUG, "There are {0} rangefinders", Program.Config.RangeFinderEchoPins.Count);
			foreach(KeyValuePair<RFDir, GpioPin> kvp in Program.Config.RangeFinderEchoPins)
			{
				Log.SysLogText(LogLevel.DEBUG, "Starting rangefinder {0} on echo pin {1}", kvp.Key, kvp.Value);
				HCSR04_RangeFinder rangeFinder = new HCSR04_RangeFinder(kvp.Value, Program.Config.RangeFinderInputPin, kvp.Key);
				rangeFinder.Start();

				RangeFinders.Add(kvp.Key, rangeFinder);
			}
		}

		private void StopRangeFinders()
		{
			foreach(HCSR04_RangeFinder rangeFinder in RangeFinders.Values)
			{
				rangeFinder.Stop();
			}
			RangeFinders.Clear();
		}

		private void StartTracks()
		{
			Tracks = new BotTracks();
			Tracks.HardwarePWM = Program.Config.TracksHardwarePWM;
			Tracks.ForwardPrimaryRange += OnForwardPrimaryRange;
			Tracks.BackwardPrimaryRange += OnBackwardPrimaryRange;
			Tracks.ForwardSecondaryRange += OnForwardSecondaryRange;
			Tracks.BackwardSecondaryRange += OnBackwardSecondaryRange;
			Tracks.NewDestinationBearing += OnNewDestinationBearing;
			Tracks.LeftAdjust = Program.Config.LeftTrackAdjust;
			Tracks.RightAdjust = Program.Config.RightTrackAdjust;
			Tracks.DistanceToTravel += OnDistanceToTravel;
			Tracks.DistanceLeft += OnDistanceLeft;

			Tracks.LeftSpeed = 0;
			Tracks.RightSpeed = 0;

			Tracks.StartTracks();
		}

		public void SetForwardSecondaryRange(Double range, bool valid)
		{
			ForwardSecondaryRange(range, valid);
		}

		public void SetBackwardSecondaryRange(Double range, bool valid)
		{
			BackwardSecondaryRange(range, valid);
		}

		private void StopTracks()
		{
			Log.SysLogText(LogLevel.INFO, "Stopping tracks");
			Tracks.LeftSpeed = 0;
			Tracks.RightSpeed = 0;

			Tracks.StopTracks();
		}

		private void OnForwardPrimaryRange(double range, bool valid)
		{
			ForwardPrimaryRange(range, valid);
		}

		private void OnBackwardPrimaryRange(double range, bool valid)
		{
			BackwardPrimaryRange(range, valid);
		}

		private void OnForwardSecondaryRange(double range, bool valid)
		{
			ForwardSecondaryRange(range, valid);
		}

		private void OnBackwardSecondaryRange(double range, bool valid)
		{
			BackwardSecondaryRange(range, valid);
		}

		private void OnNewDestinationBearing(double bearing)
		{
			NewDestinationBearing(bearing);
		}

		private void OnDistanceToTravel(double range)
		{
			DistanceToTravel(range);
		}

		private void OnDistanceLeft(double range)
		{
			DistanceLeft(range);
		}

		private void OnImageEnvironment_FuzzyPathChanged(FuzzyPath path)
		{
			FuzzyPathChanged(path);
		}

		private void OnImageEnvironment_LandmarksChanged(ImageVectorList landmarks)
		{
			LandmarksChanged(landmarks);
		}

		private void OnImageEnvironment_BarriersChanged(BarrierList barriers)
		{
			BarriersChanged(barriers);
		}

		private void OnDeadReckoningEnvironmentChanged(DeadReckoningEnvironment environment)
		{
			DeadReckoningEnvironmentReceived(environment);
		}

		public Double GetRangeAtDirection(Direction direction, bool fuzzy = true)
		{
			Double bearing = Widgets.Instance.Compass.Bearing;
			if(direction == Direction.Backward)
			{
				bearing = bearing.AddDegrees(180);
			}

			Double range = 0;
			if(fuzzy)
			{
				range = Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.Chassis, bearing, Widgets.Instance.ImageEnvironment.RangeFuzz);
			}
			else
			{
				Log.SysLogText(LogLevel.DEBUG, "getting non-fuzzy range");
				if((range = Widgets.Instance.ImageEnvironment.GetRangeAtBearing(bearing)) == 0)
				{
					range = Widgets.Instance.ImageEnvironment.ShortestRangeAtBearing(bearing, 4);
					Log.SysLogText(LogLevel.DEBUG, "was zero and is now {0}", range);
				}
			}

			HCSR04_RangeFinder rangeFinder;
			if(direction == Direction.Forward && Widgets.Instance.RangeFinders.TryGetValue(RFDir.Front, out rangeFinder) && rangeFinder.Range != 0)
			{
				range = Math.Min(range, rangeFinder.Range);
			}
			else if(direction == Direction.Backward && Widgets.Instance.RangeFinders.TryGetValue(RFDir.Rear, out rangeFinder) && rangeFinder.Range != 0)
			{
				range = Math.Min(range, rangeFinder.Range);
			}

			return range;
		}

		private void OnCameraImagesAnalyzed(ImageAnalysis analysis)
		{
			CameraImagesAnalyzed(analysis);
		}

		private void OnArmClawCommand(int percent)
		{
			RobotArm.Claw = percent;
		}

		private void OnArmThrustCommand(int percent)
		{
			RobotArm.Right = percent;
		}

		private void OnArmElevationCommand(int percent)
		{
			RobotArm.Left = percent;
		}

		private void OnArmRotationCommand(int percent)
		{
			RobotArm.Rotation = percent;
		}

		private void OnRaspiCameraParametersReceived(RaspiCameraParameters parameters)
		{
			Log.SysLogText(LogLevel.DEBUG, "Received new camera parameters {0}", parameters);
			Program.Config.CameraParameters = Camera.Parameters = parameters;
			Program.Config.Save();
		}
	}
}
