#define TRACE_LIDAR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using KanoopCommon.Addresses;
using KanoopCommon.Conversions;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.TCP.Clients;
using RaspiCommon.Lidar;

namespace RaspiCommon.Network
{
	public class RPLidarClient
	{
		public delegate void ClientDisconnectedHandler(RPLidarClient client);

		static readonly byte[] StartMarker = ASCIIEncoding.UTF8.GetBytes("!!RP!!");
		static readonly int HeaderSize = StartMarker.Length + sizeof(UInt16);

		public event RangeBlobReceivedHandler RangeBlobReceived;

		public IPv4AddressPort ServerAddress { get; set; }
		public TcpClientClient Client { get; private set; }

		byte[] _receiveBuffer;
		int _bytesInBuffer;
		int _receiveOffset;
		Timer _timer;

#if TRACE_LIDAR
		FileStream _traceStream;
#endif

		public RPLidarClient(String serverAddress)
		{
			ServerAddress = new IPv4AddressPort(serverAddress);
			Reset();
			RangeBlobReceived += delegate {};
#if TRACE_LIDAR
			String traceFile = @"c:/pub/tmp/lidar_client.bin";
			if(File.Exists(traceFile))
			{
				File.Delete(traceFile);
			}
			_traceStream = new FileStream(traceFile, FileMode.CreateNew);
#endif
		}

		public void Connect()
		{
			try
			{
				Reset();
				Client = new TcpClientClient();
				Client.DataReceived += OnClientDataReceived;
				Client.SocketDisconnected += OnClientSocketDisconnected;
				Client.Connect(ServerAddress);
			}
			catch(Exception)
			{
				StartConnectTimer();
			}
		}

		public void AddBytes(byte[] buffer)
		{
			OnClientDataReceived(buffer);
		}

		private void StartConnectTimer()
		{
			_timer = new Timer();
			_timer.Elapsed += OnConnectTimerElapsed;
			_timer.Interval = 5000;
			_timer.AutoReset = false;
			_timer.Enabled = true;
		}

		private void OnConnectTimerElapsed(object sender, ElapsedEventArgs e)
		{
			Connect();
		}

		private void OnClientDataReceived(byte[] data)
		{
#if TRACE_LIDAR
			_traceStream.Write(data, 0, data.Length);
#endif
			int bytesToCopy = Math.Min(data.Length, _receiveBuffer.Length - _bytesInBuffer);
			if(bytesToCopy < data.Length)
			{
				Console.WriteLine("Dropping data!");
			}
			Array.Copy(data, 0, _receiveBuffer, _bytesInBuffer, bytesToCopy);
			_bytesInBuffer += bytesToCopy;
			ProcessBuffer();
		}

		private void ProcessBuffer()
		{
			List<LidarVector[]> rangeArrays = new List<LidarVector[]>();

			bool gotNewChunk = false;
			do
			{
				gotNewChunk = false;
				int markerIndex = _receiveBuffer.IndexOf(StartMarker);
				if(markerIndex >= 0 && _bytesInBuffer > markerIndex + HeaderSize)
				{
					int bytesFromStartMarker = _bytesInBuffer - markerIndex;
					int totalBytesProcessed = 0;
					using(MemoryStream ms = new MemoryStream(_receiveBuffer, markerIndex, bytesFromStartMarker))
					using(BinaryReader br = new BinaryReader(ms))
					{
						// dump start marker
						byte[] dump = br.ReadBytes(StartMarker.Length);
						int vectorCount = ByteOrder.NetworkToHost(br.ReadUInt16());
						int sequence = ByteOrder.NetworkToHost(br.ReadUInt16());
						int additionalBytesNeeded = vectorCount * sizeof(Double);
						if(bytesFromStartMarker >= HeaderSize + additionalBytesNeeded + sizeof(UInt16) + sizeof(UInt16))
						{
							List<LidarVector> vectors = new List<LidarVector>();
							Double bearing = 0;
							for(int x = 0;x < vectorCount;x++, bearing += 360.0 / vectorCount)
							{
								Double range = ByteOrder.NetworkToHost(br.ReadDouble());

								LidarVector vector = new LidarVector() { BearingAndRange = new BearingAndRange(bearing, range)  };
								if(range < 0 || range > 30)
								{
									Log.SysLogHex(LogLevel.DEBUG, _receiveBuffer, markerIndex, bytesFromStartMarker);
									Log.SysLogText(LogLevel.DEBUG, "GOTBAD: {0} at index {1}", vector, x);
								}

								vectors.Add(vector);
							}
							rangeArrays.Add(vectors.ToArray());

							totalBytesProcessed = HeaderSize + additionalBytesNeeded;
							gotNewChunk = true;
						}
					}

					if(gotNewChunk)
					{
						RemoveBytes(totalBytesProcessed);
					}
				}
			} while(gotNewChunk);

			foreach(LidarVector[] vectorSet in rangeArrays)
			{
				RangeBlobReceived(vectorSet);
			}
		}

		void RemoveBytes(int count)
		{
			Array.Copy(_receiveBuffer, count, _receiveBuffer, 0, _bytesInBuffer - count);
			_bytesInBuffer -= count;
		}

		private void OnClientSocketDisconnected(TcpClientClient client)
		{
			StartConnectTimer();
		}

		private void Reset()
		{
			_receiveBuffer = new byte[65536];
			_bytesInBuffer = 0;
			_receiveOffset = 0;
		}

	}
}
