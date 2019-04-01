using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Addresses;
using KanoopCommon.CommonObjects;
using KanoopCommon.TCP.Clients;
using KanoopCommon.Threading;
using RaspiCommon.Lidar;

namespace RaspiCommon.Server
{
	public class LidarClient : TcpClientClient
	{
		public const Double VectorSize = .25;

		public IPv4AddressPort Host { get; private set; }

		byte[] _recvBuffer;
		int _bytes;

		MutexLock _lock;

		public LidarVector[] Vectors;

		public LidarClient(String host)
		{
			IPv4AddressPort address;
			if(!IPv4AddressPort.TryParseHostPort(host, out address))
			{
				throw new CommonException("Could not parse address");
			}
			Host = address;

			_recvBuffer = new byte[16364];
			_bytes = 0;

			_lock = new MutexLock();

			Vectors = new LidarVector[(int)(360 / VectorSize)];
		}

		public void Connect()
		{
			DataReceived += OnDataReceived;
			base.Connect(Host);
		}
	
		private void OnDataReceived(byte[] data)
		{
			try
			{
				_lock.Lock();

				if(data.Length + _bytes > _recvBuffer.Length)
				{
					_bytes = 0;
					return;
				}

				Array.Copy(data, 0, _recvBuffer, _bytes, data.Length);
				_bytes += data.Length;
				while(_bytes > 0 && _recvBuffer[0] != '!')
				{
					RemoveBytes(1);
				}

				while(_bytes >= LidarServer.PacketSize)
				{
					TakePacket();
				}
			}
			finally
			{
				_lock.Unlock();
			}
		}

		int parsed = 0;
		void TakePacket()
		{
			try
			{
				String sangle = ASCIIEncoding.UTF8.GetString(_recvBuffer, 1, 7);
				String srange = ASCIIEncoding.UTF8.GetString(_recvBuffer, 8, 4);

				Double angle, range;
				if(Double.TryParse(sangle, out angle) == false || Double.TryParse(srange, out range) == false)
				{
					throw new CommonException("Parse error");
				}
				parsed++;

				LidarVector vector = new LidarVector()
				{
					Bearing = angle,
					Range = range,
					RefreshTime = DateTime.UtcNow
				};

				int offset = (int)(angle / VectorSize);
				if(offset >= Vectors.Length)
				{
					throw new CommonException("Parse error 2");
				}

				Vectors[offset] = vector;

				RemoveBytes(LidarServer.PacketSize);
			}
			catch(Exception)
			{
				_bytes = 0;
			}
		}

		void RemoveBytes(int count)
		{
			Array.Copy(_recvBuffer, count, _recvBuffer, 0, (_bytes - count));
			_bytes -= count;
		}
	}
}
