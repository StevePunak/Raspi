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
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Serialization;
using KanoopCommon.TCP;
using KanoopCommon.Threading;
using MQTT;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.Imaging;

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

		private FuzzyPath _fuzzyPath;
		private bool _fuzzyPathChanged;

		private ImageVectorList _landmarks;
		private EnvironmentInfo _environmentInfo;
		private bool _landmarksChanged;
		private BarrierList _barriers;
		private bool _barriersChanged;
		private bool _distancesChanged;

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
			
			_fuzzyPathChanged = false;
			_barriersChanged = false;
			_landmarksChanged = false;

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
					SendRangeData();

					SendBearing();
					if(_fuzzyPathChanged)
					{
						SendFuzzyPath();
					}
					if(_barriersChanged)
					{
						SendBarriers();
					}
					if(_landmarksChanged)
					{
						SendLandmarks();
					}
					if(_distancesChanged)
					{
						SendBearingAndRange();
					}
				}

			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.ERROR, "{0} EXCEPTION: {1}", Name, e.Message);
				Interval = TimeSpan.FromSeconds(1);
			}
			return true;
		}

		private void SendBearing()
		{
			Double forwardBearing = Widgets.Compass.Bearing;
			Double backwardBearing = forwardBearing.AddDegrees(180);
			Double forwardPrimaryRange = Widgets.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Chassis, forwardBearing, 0);
			Double backwardPrimaryRange = Widgets.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Chassis, backwardBearing, 0);

			if( forwardBearing != _environmentInfo.Bearing ||
				forwardPrimaryRange != _environmentInfo.ForwardPrimaryRange ||
				backwardPrimaryRange != _environmentInfo.BackwardPrimaryRange)
			{
				_environmentInfo.Bearing = forwardBearing;
				_environmentInfo.ForwardPrimaryRange = forwardPrimaryRange;
				_environmentInfo.BackwardPrimaryRange = backwardPrimaryRange;
				_distancesChanged = true;
			}

			byte[] output = BitConverter.GetBytes(forwardBearing);
			Client.Publish(MqttTypes.BearingTopic, output, true);
		}

		private void SendFuzzyPath()
		{
			Log.LogText(LogLevel.DEBUG, "Sending Fuzzy Path");
			byte[] output = _fuzzyPath.Serizalize();
			Client.Publish(MqttTypes.CurrentPathTopic, output, true);
			_fuzzyPathChanged = false;
		}

		private void SendBearingAndRange()
		{
//			Log.LogText(LogLevel.DEBUG, "Sending Bearing and Range3");
			byte[] output = BinarySerializer.Serialize(_environmentInfo);
			Client.Publish(MqttTypes.DistanceAndBearingTopic, output, true);
			_distancesChanged = false;
		}

		private void SendLandmarks()
		{
//			Log.LogText(LogLevel.DEBUG, "Sending Landmarks");
			byte[] output = _landmarks.Serialize();
			Client.Publish(MqttTypes.LandmarksTopic, output, true);
			_landmarksChanged = false;
		}

		private void SendBarriers()
		{
//			Log.LogText(LogLevel.DEBUG, "Sending Barriers");
			byte[] output = _barriers.Serialize();
			Client.Publish(MqttTypes.BarriersTopic, output, true);
			_barriersChanged = false;
		}

		private void SendInitialData()
		{
			Log.LogText(LogLevel.DEBUG, "Sending initial data");

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

			ChassisMetrics chassisMetrics = new ChassisMetrics()
			{
				Length = Widgets.Chassis.Length,
				Width = Widgets.Chassis.Width,
				LidarPosition = Widgets.Chassis.LidarPosition
			};
			Client.Publish(MqttTypes.ChassisMetricsTopic, chassisMetrics.Serialize(), true);
		}

		void SendRangeData()
		{
//			Log.LogText(LogLevel.DEBUG, "Sending Range Data");
			byte[] output = Widgets.ImageEnvironment.MakeRangeBlob();
			Client.Publish(MqttTypes.RangeBlobTopic, output);
		}

		private void OnEnvironment_FuzzyPathChanged(FuzzyPath path)
		{
			_fuzzyPath = path.Clone();
			_fuzzyPathChanged = true;
		}

		private void OnEnvironment_LandmarksChanged(ImageVectorList landmarks)
		{
			_landmarks = landmarks.Clone();
			_landmarksChanged = true;
		}

		private void OnEnvironment_BarriersChanged(BarrierList barriers)
		{
			_barriers = barriers.Clone();
			_barriersChanged = true;
		}

		private void OnWidgets_BearingChanged(double bearing)
		{
			_environmentInfo.Bearing = bearing;
			_distancesChanged = true;
		}

		private void OnWidgets_ForwardPrimaryRange(double range)
		{
			_environmentInfo.ForwardPrimaryRange = range;
			_distancesChanged = true;
		}

		private void OnWidgets_BackwardPrimaryRange(double range)
		{
			_environmentInfo.BackwardPrimaryRange = range;
			_distancesChanged = true;
		}

		private void OnWidgets_ForwardSecondaryRange(double range)
		{
			_environmentInfo.ForwardSecondaryRange = range;
			_distancesChanged = true;
		}

		private void OnWidgets_BackwardSecondaryRange(double range)
		{
			_environmentInfo.BackwardSecondaryRange = range;
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
