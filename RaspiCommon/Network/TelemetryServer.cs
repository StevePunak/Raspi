using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Addresses;
using KanoopCommon.CommonObjects;
using KanoopCommon.Encoding;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Serialization;
using KanoopCommon.TCP;
using KanoopCommon.Threading;
using MQTT;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Devices.Locomotion;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Devices.Spatial;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.DeadReckoning;
using RaspiCommon.Spatial.LidarImaging;

namespace RaspiCommon.Network
{
	public class TelemetryServer : ThreadBase
	{
		public const int RangeBlobPacketSize = sizeof(Double);
		static readonly TimeSpan SlowRunInterval = TimeSpan.FromSeconds(1);
		static readonly TimeSpan StandardRunInterval = TimeSpan.FromMilliseconds(250);

		public IWidgetCollection Widgets { get; private set; }
		
		public MqttClient Client { get; private set; }

		public String MqqtClientID { get; private set; }

		public IPAddress Address { get; private set; }

		public String ImageDirectory { get; set; }

		private FuzzyPath _fuzzyPath;
		private bool _sendFuzzyPath;
		private bool _sendInitialData;

		private ImageVectorList _landmarks;
		private EnvironmentInfo _environmentInfo;
		private DeadReckoningEnvironment _deadReckoningEnvironment;

		private bool _sendLandmarks;
		private BarrierList _barriers;
		private bool _sendBarriers;
		private bool _distancesChanged;
		private bool _sendDeadReckoningEnvironment;

		public TelemetryServer(IWidgetCollection widgets, String mqqtBrokerAddress, String mqqtClientID)
			: base(typeof(TelemetryServer).Name)
		{
			IPAddress address;

			if( IPAddress.TryParse(mqqtBrokerAddress, out address) == false &&
				IPAddressExtensions.TryResolve(mqqtBrokerAddress, out address) == false)
			{
				throw new CommonException("Could not resolve '{0}' to a host", mqqtBrokerAddress);
			}
			Address = address;
			MqqtClientID = mqqtClientID;
			Widgets = widgets;

			Widgets.FuzzyPathChanged += OnEnvironment_FuzzyPathChanged;
			Widgets.BarriersChanged += OnEnvironment_BarriersChanged;
			Widgets.LandmarksChanged += OnEnvironment_LandmarksChanged;
			Widgets.BearingChanged += OnWidgets_BearingChanged;
			Widgets.ForwardPrimaryRange += OnWidgets_ForwardPrimaryRange;
			Widgets.BackwardPrimaryRange += OnWidgets_BackwardPrimaryRange;
			Widgets.ForwardSecondaryRange += OnWidgets_ForwardSecondaryRange;
			Widgets.BackwardSecondaryRange += OnWidgets_BackwardSecondaryRange;
			Widgets.NewDestinationBearing += OnWidgets_NewDestinationBearing;
			Widgets.DistanceToTravel += OnWidgets_DistanceToTravel;
			Widgets.DistanceLeft += OnWidgets_DistanceLeft;
			Widgets.DeadReckoningEnvironmentReceived += OnDeadReckoningLandscapeReceived;
			Widgets.CameraImagesAnalyzed += OnCameraImagesAnalyzed;
			if(RaspiConfig.Instance.SendSpeedAndBearingTelemetry)
			{
				Widgets.ImageEnvironment.CompassOffsetChanged += OnEnvironment_CompassOffsetChanged;
			}

			ImageDirectory = Camera.ImageDirectory;
			
			_sendFuzzyPath = false;
			_sendBarriers = false;
			_sendLandmarks = false;
			_sendDeadReckoningEnvironment = false;

			_environmentInfo = new EnvironmentInfo();

			Interval = TimeSpan.FromMilliseconds(250);
		}

		protected override bool OnStart()
		{
			return base.OnStart();
		}

		protected override bool OnRun()
		{
			try
			{
				if(Client == null || Client.Connected == false)
				{
					GetConnected();
					SendInitialData();
				}

				if(Client.Connected)
				{
					SendLidarData();

					SendSpeedAndBearing();

					if(_sendFuzzyPath)
					{
						SendFuzzyPath();
					}
					if(_sendBarriers)
					{
						SendBarriers();
					}
					if(_sendLandmarks)
					{
						SendLandmarks();
					}
					if(_distancesChanged)
					{
						SendBearingAndRange();
					}
					if(_sendDeadReckoningEnvironment)
					{
						SendDeadReckoningEnvironment();
					}
					if(_sendInitialData)
					{
						SendInitialData();
					}
				}

			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.ERROR, "{0} EXCEPTION: {1}\n{2}", Name, e.Message, GetFormattedStackTrace(e));
				Interval = TimeSpan.FromSeconds(1);
			}
			return true;
		}

		protected override bool OnStop()
		{
			Client.Stop();
			return base.OnStop();
		}

		private void OnCameraImagesAnalyzed(ImageAnalysis analysis)
		{
			byte[] bytes = analysis.Serialize();
			Client.Publish(MqttTypes.CameraLastAnalysisTopic, bytes, true);
		}

		public void PublishImage(List<String> filenames)
		{
			Log.LogText(LogLevel.DEBUG, "Going to publish {0} files");
			List<String> rawFilenames = filenames.GetFileNames();
			byte[] bytes = UTF8.Encode(rawFilenames);
			Client.Publish(MqttTypes.CameraLastImageTopic, bytes);
		}

		private void SendDeadReckoningEnvironment()
		{
			Log.LogText(LogLevel.DEBUG, "Sending DR Environment");
			byte[] serialized = _deadReckoningEnvironment.Serialize();
			Client.Publish(MqttTypes.DeadReckoningCompleteLandscapeTopic, serialized, true);
			_sendDeadReckoningEnvironment = false;
		}

		private void SendSpeedAndBearing()
		{
			if(RaspiConfig.Instance.SendRangeImagingTelemetry)
			{
				Double forwardBearing = Widgets.Compass.Bearing;
				Double backwardBearing = forwardBearing.AddDegrees(180);
				Double forwardPrimaryRange = Widgets.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Chassis, forwardBearing, 0);
				Double backwardPrimaryRange = Widgets.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Chassis, backwardBearing, 0);

				if(forwardBearing != _environmentInfo.Bearing ||
					forwardPrimaryRange != _environmentInfo.ForwardPrimaryRange ||
					backwardPrimaryRange != _environmentInfo.BackwardPrimaryRange)
				{
					_environmentInfo.Bearing = forwardBearing;
					_environmentInfo.ForwardPrimaryRange = forwardPrimaryRange;
					_environmentInfo.BackwardPrimaryRange = backwardPrimaryRange;
					_distancesChanged = true;
				}
			}

			if(RaspiConfig.Instance.SendSpeedAndBearingTelemetry)
			{
				SpeedAndBearing speed = new SpeedAndBearing()
				{
					Bearing = Widgets.Compass.Bearing,
					TrackSpeed = new TrackSpeed()
					{
						LeftSpeed = Widgets.TrackSpeed.LeftSpeed,
						RightSpeed = Widgets.TrackSpeed.RightSpeed,
					},
				};
				byte[] output = speed.Serialize();
				Client.Publish(MqttTypes.SpeedAndBearingTopic, output, false);
			}
		}

		private void SendFuzzyPath()
		{
			Log.LogText(LogLevel.DEBUG, "Sending Fuzzy Path");
			byte[] output = _fuzzyPath.Serizalize();
			Client.Publish(MqttTypes.CurrentPathTopic, output, true);
			_sendFuzzyPath = false;
		}

		private void SendBearingAndRange()
		{
//			Log.LogText(LogLevel.DEBUG, "Sending Bearing and Range3");
			byte[] output = BinarySerializer.Serialize(_environmentInfo);
			Client.Publish(MqttTypes.DistanceAndBearingTopic, output, true);
			_distancesChanged = false;
		}

		private void SendBearing()
		{
						Log.LogText(LogLevel.DEBUG, "Sending Bearing {0}", Widgets.Compass.Bearing);
			byte[] output = BitConverter.GetBytes(Widgets.Compass.Bearing);
			Client.Publish(MqttTypes.BearingTopic, output, true);
			_distancesChanged = false;
		}

		private void SendLandmarks()
		{
//			Log.LogText(LogLevel.DEBUG, "Sending Landmarks");
			byte[] output = _landmarks.Serialize();
			Client.Publish(MqttTypes.LandmarksTopic, output, true);
			_sendLandmarks = false;
		}

		private void SendBarriers()
		{
//			Log.LogText(LogLevel.DEBUG, "Sending Barriers");
			byte[] output = _barriers.Serialize();
			Client.Publish(MqttTypes.BarriersTopic, output, true);
			_sendBarriers = false;
		}

		private void SendInitialData()
		{
			Log.LogText(LogLevel.DEBUG, "Sending initial data");

			if(RaspiConfig.Instance.SendRangeImagingTelemetry)
			{
				ImageMetrics imageMetrics = new ImageMetrics()
				{
					MetersSquare = Widgets.ImageEnvironment.MetersSquare,
					PixelsPerMeter = Widgets.ImageEnvironment.PixelsPerMeter
				};
				String output = KVPSerializer.Serialize(imageMetrics);
				Client.Publish(MqttTypes.ImageMetricsTopic, output, true);

				LandscapeMetrics landscapeMetrics = new LandscapeMetrics()
				{
					MetersSquare = Widgets.Landscape.MetersSquare,
					Name =  Widgets.Landscape.Name
				};
				output = KVPSerializer.Serialize(imageMetrics);
				Client.Publish(MqttTypes.LandscapeMetricsTopic, output, true);

				LidarMetrics lidarMetrics = new LidarMetrics()
				{
					Offset = Widgets.ImageEnvironment.CompassOffset,
				};
				Client.Publish(MqttTypes.LidarMetricsTopic, lidarMetrics.Serialize(), true);
			}

			if(RaspiConfig.Instance.SendChassisTelemetry)
			{
				ChassisMetrics chassisMetrics = new ChassisMetrics()
				{
					Length = Widgets.Chassis.Length,
					Width = Widgets.Chassis.Width,
					LidarPosition = Widgets.Chassis.LidarPosition
				};
				Client.Publish(MqttTypes.ChassisMetricsTopic, chassisMetrics.Serialize(), true);
			}

			_sendInitialData = false;
		}

		void SendLidarData()
		{
			if(RaspiConfig.Instance.SendRangeImagingTelemetry)
			{
				//Log.LogText(LogLevel.DEBUG, "Sending Range Data");
				byte[] output = Widgets.ImageEnvironment.MakeRangeBlob();
				Client.Publish(MqttTypes.RangeBlobTopic, output);
			}
		}

		private void OnDeadReckoningLandscapeReceived(DeadReckoningEnvironment environment)
		{
			_deadReckoningEnvironment = environment;
			_sendDeadReckoningEnvironment = true;
		}

		private void OnEnvironment_FuzzyPathChanged(FuzzyPath path)
		{
			_fuzzyPath = path.Clone();
			_sendFuzzyPath = true;
		}

		private void OnEnvironment_CompassOffsetChanged(double offset)
		{
			_sendInitialData = true;
		}

		private void OnEnvironment_LandmarksChanged(ImageVectorList landmarks)
		{
			_landmarks = landmarks.Clone();
			_sendLandmarks = true;
		}

		private void OnEnvironment_BarriersChanged(BarrierList barriers)
		{
			_barriers = barriers.Clone();
			_sendBarriers = true;
		}

		private void OnWidgets_BearingChanged(double bearing)
		{
			_environmentInfo.Bearing = bearing;
			_distancesChanged = true;
		}

		private void OnWidgets_ForwardPrimaryRange(double range, bool valid)
		{
			_environmentInfo.ForwardPrimaryRange = range;
			_environmentInfo.ForwardPrimaryRangeValid = valid;
			_distancesChanged = true;
		}

		private void OnWidgets_BackwardPrimaryRange(double range, bool valid)
		{
			_environmentInfo.BackwardPrimaryRange = range;
			_environmentInfo.BackwardPrimaryRangeValid = valid;
			_distancesChanged = true;
		}

		private void OnWidgets_ForwardSecondaryRange(double range, bool valid)
		{
			_environmentInfo.ForwardSecondaryRange = range;
			_environmentInfo.ForwardSecondaryRangeValid = valid;
			_distancesChanged = true;
		}

		private void OnWidgets_BackwardSecondaryRange(double range, bool valid)
		{
			_environmentInfo.BackwardSecondaryRange = range;
			_environmentInfo.BackwardSecondaryRangeValid = valid;
			_distancesChanged = true;
		}

		private void OnWidgets_NewDestinationBearing(double bearing)
		{
			_environmentInfo.DestinationBearing = bearing;
			_distancesChanged = true;
		}

		private void OnWidgets_DistanceToTravel(double range)
		{
			_environmentInfo.DistanceToTravel = range;
			_distancesChanged = true;
		}

		private void OnWidgets_DistanceLeft(double range)
		{
			_environmentInfo.DistanceLeft = range;
			_distancesChanged = true;
		}

		void GetConnected()
		{
			Client = new MqttClient(Address, MqqtClientID)
			{
				QOS = QOSTypes.Qos0
			};
			Client.Connect();
		}
	}
}
