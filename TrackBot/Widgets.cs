using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Database;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using RaspiCommon;
using RaspiCommon.Data.DataSource;
using RaspiCommon.Data.Entities;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Devices.Spatial;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Network;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.DeadReckoning;
using RaspiCommon.Spatial.Imaging;
using TrackBot.ForkLift;
using TrackBot.Network;
using TrackBot.Spatial;
using TrackBot.Tracks;
using TrackBotCommon.Environs;

namespace TrackBot
{
	class Widgets : IWidgetCollection
	{
		public event ForwardPrimaryRangeHandler ForwardPrimaryRange;
		public event BackwardPrimaryRangeHandler BackwardPrimaryRange;
		public event ForwardSecondaryRangeHandler ForwardSecondaryRange;
		public event BackwardSecondaryRangeHandler BackwardSecondaryRange;
		public event NewDestinationBearingHandler NewDestinationBearing;
		public event DistanceToTravelHandler DistanceToTravel;
		public event DistanceLeftHandler DistanceLeft;
		public event NewBearingHandler BearingChanged;
		public event FuzzyPathChangedHandler FuzzyPathChanged;
		public event LandmarksChangedHandler LandmarksChanged;
		public event BarriersChangedHandler BarriersChanged;
		public event DeadReckoningEnvironmentReceivedHandler DeadReckoningEnvironmentReceived;

		public BotTracks Tracks { get; private set; }
		public Dictionary<RFDir, HCSR04_RangeFinder> RangeFinders { get; private set; }
		public Lift Lift { get; private set; }

		public LSM9D51CompassAccelerometer GyMag { get; private set; }
		public IImageEnvironment ImageEnvironment { get; private set; }
		public ILandscape Landscape { get; private set; }
		public SpatialPoll SpatialPollThread { get; private set; }

		public SaveImageThread SaveImageThread { get; set; }
		public TelemetryServer Server { get; private set; }

		public DeadReckoningEnvironment DeadReckoningEnvironment { get; private set; }

		public TrackDataSource DataSource { get; private set; }

		public CommandServer CommandServer { get; private set; }

		public ICompass Compass { get { return GyMag; } }

		public Chassis Chassis { get; private set; }

		public Camera Camera { get; private set; }

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
		}

		public void StartWidgets()
		{
			StartCamera();
			StartChassis();
			StartDatabase();
			StartRangeFinders();
			StartTracks();
			StartSpatial();
			StartActivities();
			StartLift();
			StartSaveImageThread();
			StartSpatialPolling();
			StartCommandServer();
			StartDeadReckoningEnvironment();
		}

		public void StopWidgets()
		{
			StopDeadReckoningEnvironment();
			StopCamera();
			StopCommandServer();
			StopSpatialPolling();
			StopLift();
			StopActivities();
			Tracks.Stop();
			StopRangeFinders();
			StopSpatial();
			GpioSharp.DeInit();
			StopSaveImageThread();
			StopDatabase();
			StopChassis();

			foreach(ThreadBase thread in ThreadBase.GetRunningThreads())
			{
				Log.SysLogText(LogLevel.DEBUG, "Remaining: {0}", thread);
			}
		}

		private void StartDeadReckoningEnvironment()
		{
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
			Camera = new Camera();
			Camera.Start();
		}

		private void StopCamera()
		{
			Camera.Stop();
			Camera = null;
		}

		private void StartChassis()
		{
			Chassis = new XiaorTankTracks();
			Chassis.Points.Add(ChassisParts.Lidar, new PointD(Chassis.Points[ChassisParts.RearLeft].X + .115, Chassis.Points[ChassisParts.FrontRight].Y + .120));
			Chassis.Points.Add(ChassisParts.FrontRangeFinder, new PointD(Chassis.Width / 2, 0));
			Chassis.Points.Add(ChassisParts.RearRangeFinder, new PointD(Chassis.Width / 2, Chassis.Length));
		}

		private void StopChassis()
		{

		}

		private void StartCommandServer()
		{
			Console.WriteLine("Starting Mqqt Command Client");
			CommandServer = new CommandServer(String.Format("raspi.{0}", Environment.MachineName));
			CommandServer.Start();
		}

		private void StopCommandServer()
		{
			CommandServer.Stop();
		}

		private void StartDatabase()
		{
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
			Server = new TelemetryServer(this, Program.Config.RadarHost, "trackbot-lidar");
			Server.Start();
		}

		private void StopTelemetryServer()
		{
			Server.Stop();
			Server = null;
		}

		private void StartSpatialPolling()
		{
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
			Lift = new Lift();
			Lift.Start();
		}

		private void StopLift()
		{
			Lift.Stop();
		}

		private void StartSpatial()
		{
			GyMag = new LSM9D51CompassAccelerometer();
			GyMag.MagneticDeviation = Program.Config.MagneticDeviation;
			GyMag.XAdjust = Program.Config.CompassXAdjust;
			GyMag.YAdjust = Program.Config.CompassYAdjust;
			GyMag.NewBearing += OnNewBearing;

			List<String> ports = new List<String>(SerialPort.GetPortNames());
			ports.TerminateAll();
			if(ports.Contains(Program.Config.LidarComPort) == true)
			{
				if(Program.Config.RadarHost == null)
				{
					Program.Config.RadarHost = "192.168.0.50";
					Program.Config.Save();
				}
				ImageEnvironment = new TrackLidar(Program.Config.LidarMetersSquare, Program.Config.LidarPixelsPerMeter);
				ImageEnvironment.BarriersChanged += OnImageEnvironment_BarriersChanged;
				ImageEnvironment.LandmarksChanged += OnImageEnvironment_LandmarksChanged;
				ImageEnvironment.FuzzyPathChanged += OnImageEnvironment_FuzzyPathChanged;
				StartTelemetryServer();
			}
			else
			{
				Console.WriteLine("Lidar com port {0} does not exist... initializing virtual environment", Program.Config.LidarComPort);
				ImageEnvironment = new VirtualEnvironment();
			}



			ImageEnvironment.Start();
		}

		private void StopSpatial()
		{
			if(ImageEnvironment is TrackLidar)
			{
				StopTelemetryServer();
			}
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
			Tracks.DistanceToTravel += OnDistanceToTravel;
			Tracks.DistanceLeft += OnDistanceLeft;

			Tracks.LeftSpeed = 0;
			Tracks.RightSpeed = 0;
		}

		public void SetForwardSecondaryRange(Double range)
		{
			ForwardSecondaryRange(range);
		}

		public void SetBackwardSecondaryRange(Double range)
		{
			BackwardSecondaryRange(range);
		}

		private void StopTracks()
		{
			Tracks.LeftSpeed = 0;
			Tracks.RightSpeed = 0;
		}

		private void OnForwardPrimaryRange(double range)
		{
			ForwardPrimaryRange(range);
		}

		private void OnBackwardPrimaryRange(double range)
		{
			BackwardPrimaryRange(range);
		}

		private void OnForwardSecondaryRange(double range)
		{
			ForwardSecondaryRange(range);
		}

		private void OnBackwardSecondaryRange(double range)
		{
			BackwardSecondaryRange(range);
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

		public Double GetRangeAtDirection(Direction direction)
		{
			Double bearing = Widgets.Instance.GyMag.Bearing;
			if(direction == Direction.Backward)
			{
				bearing = bearing.AddDegrees(180);
			}

			Double range = Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.Chassis, bearing, Widgets.Instance.ImageEnvironment.RangeFuzz);

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



	}
}
