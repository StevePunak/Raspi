using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Addresses;
using KanoopCommon.Logging;
using KanoopCommon.TCP;
using KanoopCommon.Threading;

namespace RaspiCommon.Server
{
	public class LidarServer : TcpServer
	{
		public const int PacketSize = 13;
		static readonly TimeSpan ReceiveTimeout = TimeSpan.FromSeconds(5);

		public RPLidar Lidar { get; set; }

		public LidarServer(RPLidar lidar) 
			: base(IPAddress.Any, 1234, typeof(LidarServer).Name)
		{
			Lidar = lidar;

			Interval = TimeSpan.FromMilliseconds(250);

			ConnectionDebugLogging = true;
		}

		protected override TcpConnectedClient OnConnectionReceived(TcpClient client, TcpListener listener)
		{
			Console.WriteLine("Received new connection to range server");
			return new RangeServerClient(client, listener, this);
		}

		protected override bool OnRun()
		{
			List<TcpConnectedClient> clients = Clients;

			if(clients.Count > 0)
			{
				foreach(RangeServerClient client in clients)
				{
					StringBuilder sb = new StringBuilder(PacketSize * (int)(Lidar.VectorSize / 360));

					for(int offset = 0;offset < Lidar.Vectors.Length;offset++)
					{
						Double angle = (Double)offset * Lidar.VectorSize;

						sb.AppendFormat("!{0:000.000}{1:0.000}", angle, Lidar.Vectors[offset]);
					}
					String output = sb.ToString();

					if(DateTime.UtcNow > client.LastReceiveTime + ReceiveTimeout)
					{
						//Console.WriteLine("Disconnecting server");
						//client.Close();
					}
					else if(client.IsConnected)
					{
						Log.LogText(LogLevel.DEBUG, "Sending {0} bytes to client", output.Length);
						client.Send(output);
					}
				}
			}

			return base.OnRun();
		}
	}

	class RangeServerClient : TcpConnectedClient
	{
		public DateTime LastReceiveTime { get; private set; }

		public RangeServerClient(TcpClient tcpClient, TcpListener listener, TcpServer parent) 
			: base(tcpClient, listener, parent)
		{
			LastReceiveTime = DateTime.UtcNow;
			Log.SysLogText(LogLevel.DEBUG, "Client connected");
//			((LidarServer)parent).Lidar.Sample += OnLidarSample;
			Log.SysLogText(LogLevel.DEBUG, "registered event");
		}

		private void OnLidarSample(Lidar.LidarSample sample)
		{
			String output = String.Format("!{0:000.000}{1:0.000}", sample.Bearing, sample.Distance);
			Send(output);
		}

		protected override void OnDataReceived(byte[] buffer)
		{
			LastReceiveTime = DateTime.UtcNow;
		}

		protected override void OnDisconnect()
		{
			Log.SysLogText(LogLevel.DEBUG, "Client disconnected");
			((LidarServer)_server).Lidar.Sample -= OnLidarSample;
		}
	}
}
