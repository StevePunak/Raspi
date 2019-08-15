using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Addresses;
using KanoopCommon.Logging;
using KanoopCommon.TCP.Clients;
using RaspiCommon.Devices.Compass;

namespace RaspiCommon.Devices.Spatial
{
	public class RPLidarNetwork : RPLidarBase
	{
		public IPv4AddressPort Address { get; private set; }

		TcpClientClient _client;
		FileStream _traceFile;

		public RPLidarNetwork(IPv4AddressPort address, double vectorSize, ICompass compass)
			: base(address.ToString(), vectorSize, compass)
		{
			Address = address;
			_traceFile = null;
		}

		public void OpenTraceFile(String filename)
		{
			_traceFile = File.OpenWrite(filename);
		}

		public override void Start()
		{
			base.Start();

			_client = new TcpClientClient();
			_client.DataReceived += OnDataReceived;
			_client.Connect(Address);

			ForceMultiScanMeasurement = true;
		}

		public override void Stop()
		{
			_client.Disconnect();
		}

		private void OnDataReceived(byte[] data)
		{
			//			Log.SysLogHex(LogLevel.DEBUG, data);
			if(_traceFile != null)
			{
				_traceFile.Write(data, 0, data.Length);
			}
			HandleDataReceived(data, data.Length);
		}

	}
}
