using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Addresses;
using KanoopCommon.CommonObjects;
using KanoopCommon.Logging;
using KanoopCommon.Serialization;
using KanoopCommon.TCP.Clients;
using KanoopCommon.Threading;
using MQTT;
using MQTT.Examples;
using MQTT.Packets;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.Imaging;

namespace RaspiCommon.Server
{
	public class TelemetryClient : SubscribeThread
	{
		public const Double VectorSize = .25;

		public event ImageLandmarksReceivedHandler ImageLandmarksReceived;
		public event RangeBlobReceivedHandler RangeBlobReceived;

		public LidarVector[] Vectors;

		public FuzzyPath FuzzyPath { get; private set; }
		public BarrierList ImageBarriers { get; private set; }
		public ImageVectorList ImageVectors { get; private set; }
		public ImageVectorList LandscapeVectors { get; private set; }
		public Double Bearing { get; private set; }

		public ImageMetrics ImageMetrics { get; private set; }
		public LandscapeMetrics LandscapeMetrics { get; private set; }

		public TelemetryClient(String host, String clientID, List<String> topics)
			: base(host, clientID, topics)
		{
			Vectors = new LidarVector[(int)(360 / VectorSize)];
			for(int offset = 0;offset < Vectors.Length;offset++)
			{
				Vectors[offset] = new LidarVector()
				{
					Bearing = (Double)offset * VectorSize,
					Range = 0,
					RefreshTime = DateTime.UtcNow
				};
			}
			InboundSubscribedMessage += OnLidarClientInboundSubscribedMessage;

			ImageMetrics = new ImageMetrics();
			LandscapeMetrics = new LandscapeMetrics();

			// stub our own events
			ImageLandmarksReceived += delegate {};
			RangeBlobReceived += delegate {};
		}

		private void OnLidarClientInboundSubscribedMessage(MqttClient client, PublishMessage packet)
		{
			if(packet.Topic == MqttTypes.RangeBlobTopic)
			{
				using(BinaryReader br = new BinaryReader(new MemoryStream(packet.Message)))
				{
					for(int offset = 0;offset < Vectors.Length;offset++)
					{
						Double range = br.ReadDouble();
						Vectors[offset].Range = range;
						Vectors[offset].RefreshTime = DateTime.UtcNow;
					}

				}
				RangeBlobReceived(Vectors);
			}
			else
			{
				if(packet.Topic == MqttTypes.CurrentPathTopic)
				{
					FuzzyPath path = new FuzzyPath(packet.Message);
					FuzzyPath = path;
				}
				else if(packet.Topic == MqttTypes.BarriersTopic)
				{
					BarrierList barriers = new BarrierList(packet.Message);
					ImageBarriers = barriers;
				}
				else if(packet.Topic == MqttTypes.LandmarksTopic)
				{
					ImageVectorList landmarks = new ImageVectorList(packet.Message);
					ImageVectors = landmarks;
					ImageLandmarksReceived(ImageVectors);
				}
				else if(packet.Topic == MqttTypes.BearingTopic)
				{
					Bearing = BitConverter.ToDouble(packet.Message, 0);
				}
				else if(packet.Topic == MqttTypes.ImageMetricsTopic)
				{
					ImageMetrics = KVPSerializer.Deserialize<ImageMetrics>(ASCIIEncoding.UTF8.GetString(packet.Message));
					ScaleLandmarks();
				}
				else if(packet.Topic == MqttTypes.LandscapeMetricsTopic)
				{
					LandscapeMetrics = KVPSerializer.Deserialize<LandscapeMetrics>(ASCIIEncoding.UTF8.GetString(packet.Message));
					ScaleLandmarks();
				}
			}
		}

		void ScaleLandmarks()
		{
			if(ImageMetrics.PixelsPerMeter > 0)
			{
				ImageVectorList scaled = ImageVectors.Clone();
				scaled.Scale(1 / ImageMetrics.PixelsPerMeter);
				LandscapeVectors = scaled;
			}
		}

	}
}
