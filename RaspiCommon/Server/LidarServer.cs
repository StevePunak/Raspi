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
using KanoopCommon.Logging;
using KanoopCommon.TCP;
using KanoopCommon.Threading;
using MQTT;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;

namespace RaspiCommon.Server
{
	public class LidarServer : ThreadBase
	{
		public const String RangeBlobTopic = "trackbot/lidar/rangeblob";
		public const String BarrierTopic = "trackbot/lidar/barriers";
		public const String CurrentPathTopic = "trackbot/lidar/currentpath";

		public const int RangeBlobPacketSize = sizeof(Double);
		static readonly TimeSpan SlowRunInterval = TimeSpan.FromSeconds(1);
		static readonly TimeSpan StandardRunInterval = TimeSpan.FromMilliseconds(250);

		public LidarEnvironment Environment { get; set; }
		
		public MqttClient Client { get; private set; }

		public String MqqtClientID { get; private set; }

		public IPAddress Address { get; private set; }

		private FuzzyPath _fuzzyPath;
		private bool _fuzzyPathChanged;

		public LidarServer(LidarEnvironment environment, String mqqtBrokerAddress, String mqqtClientID)
			: base(typeof(LidarServer).Name)
		{
			IPAddress address;

			if( IPAddress.TryParse(mqqtBrokerAddress, out address) == false &&
				IPAddressExtensions.TryResolve(mqqtBrokerAddress, out address) == false)
			{
				throw new CommonException("Could not resolve '{0}' to a host", mqqtBrokerAddress);
			}
			Address = address;
			MqqtClientID = mqqtClientID;
			Environment = environment;

			Environment.FuzzyPathChanged += OnEnvironment_FuzzyPathChanged;
			_fuzzyPathChanged = false;

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
				}

				if(Client.Connected)
				{
					SendRangeData();

					if(_fuzzyPathChanged)
					{
						SendFuzzyPath();
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

		private void SendFuzzyPath()
		{
			Log.LogText(LogLevel.DEBUG, "Sending fuzzy path");

			byte[] output = _fuzzyPath.Serizalize();
			Client.Publish(CurrentPathTopic, output);
			_fuzzyPathChanged = false;
		}

		void SendRangeData()
		{
			byte[] output = new byte[RangeBlobPacketSize * Environment.Lidar.Vectors.Length];

			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(output)))
			{
				for(int offset = 0;offset < Environment.Lidar.Vectors.Length;offset++)
				{
					LidarVector vector = Environment.Lidar.Vectors[offset];
					bw.Write(vector.Range);
				}
			}

			File.WriteAllBytes(@"/home/pi/tmp/blob.bin", output);
			Client.Publish(RangeBlobTopic, output);
		}

		private void OnEnvironment_FuzzyPathChanged(FuzzyPath path)
		{
			Log.LogText(LogLevel.DEBUG, "FuzzyPath path changed");
			_fuzzyPath = path.Clone();
			_fuzzyPathChanged = true;
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
