using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using KanoopCommon.Serial;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Lidar;
using RaspiCommon.PiGpio;

namespace RaspiCommon.Devices.Spatial
{
	public class RPLidarSerial : RPLidarBase
	{
		public MonoSerialPort Port { get; private set; }
		public String PortName { get { return Source; } }

		public RPLidarSerial(String port, double vectorSize, ICompass compass)
			: base(port, vectorSize, compass)
		{
			List<String> ports = new List<String>(SerialPort.GetPortNames());
			ports.TerminateAll();
			if(ports.Contains(Source) == false)
			{
				throw new RaspiException("Invalid port name '{0}'", Source);
			}
		}

		public override void Start()
		{
			base.Start();
			Port = new MonoSerialPort(PortName);
			Port.Port.BaudRate = 115200;
			Port.BufferTime = TimeSpan.FromMilliseconds(500);
			Port.DataReceived += HandleDataReceived;
			Port.Open();

			if(GetDeviceInfo())
			{
				Log.SysLogText(LogLevel.DEBUG, "Retrieved LIDAR info");
				Pigs.SetMode(SpinPin, PinMode.Output);
				Pigs.SetOutputPin(SpinPin, PinState.High);
				StartScan();
				Log.SysLogText(LogLevel.DEBUG, "LIDAR scan started");
			}
			else
			{

			}
		}

		public override void Stop()
		{
			base.Stop();
			Pigs.SetOutputPin(SpinPin, PinState.Low);
			StopScan();
			Thread.Sleep(250);
			Reset();
			Thread.Sleep(250);

			if(Port != null && Port.IsOpen)
			{
				Log.SysLogText(LogLevel.DEBUG, "Closing Port");
				Port.Port.DtrEnable = true;
				Port.DataReceived -= HandleDataReceived;
				Port.Close();
				Log.SysLogText(LogLevel.DEBUG, "Done");
			}
		}

		public override bool StartScan()
		{
			LidarCommand command = new StartScanCommand();
			SendCommand(command);
			return true;
		}

		public override bool StopScan()
		{
			_responseQueue.Clear();
			LidarCommand command = new StopCommand();
			SendCommand(command);

			LidarResponse response;
			_tryGetResponse(TimeSpan.FromMilliseconds(250), out response);
			return true;
		}

		public override bool StartExpressScan()
		{
			LidarCommand command = new StartExpressScanCommand();
			SendCommand(command);

			LidarResponse response;
			return _tryGetResponse(TimeSpan.FromMilliseconds(500), out response);
		}

		public override bool StopExpressScan()
		{
			_responseQueue.Clear();
			LidarCommand command = new StopCommand();
			SendCommand(command);

			LidarResponse response;
			_tryGetResponse(TimeSpan.FromMilliseconds(250), out response);
			return true;
		}

		public override bool StopMotor()
		{
			Port.Port.DtrEnable = true;

			_responseQueue.Clear();
			LidarCommand command = new SetMotorPwm(0);
			SendCommand(command);

			LidarResponse response;
			_tryGetResponse(TimeSpan.FromMilliseconds(250), out response);

			return true;
		}

		public override bool Reset()
		{
			LidarCommand command = new ResetCommand();
			SendCommand(command);

			LidarResponse response;
			return _tryGetResponse(TimeSpan.FromMilliseconds(500), out response);
		}

		public override bool GetDeviceInfo()
		{
			LidarCommand command = new GetDeviceInfoCommand();
			SendCommand(command);

			LidarResponse response;
			bool result = TryGetResponse(TimeSpan.FromMilliseconds(5500), out response);
			Log.SysLogText(LogLevel.INFO, "LIDAR: GetDeviceInfo {0}", result);
			return result;
		}

		private bool TryGetResponse(TimeSpan waitTime, out LidarResponse response)
		{
			_responseWaiters++;
			return _tryGetResponse(waitTime, out response);
		}

		private bool _tryGetResponse(TimeSpan waitTime, out LidarResponse response)
		{
			response = _responseQueue.BlockDequeue(waitTime);
			//			Log.SysLogText(LogLevel.DEBUG, "Got {0}", response == null ? "null" : response.ToString());
			return response != null;
		}

		private void SendCommand(LidarCommand command)
		{
			byte[] data = command.Serialize();
			Log.SysLogText(LogLevel.DEBUG, "OUTPUT!!!!!");
			Log.SysLogHex(LogLevel.DEBUG, data);
			Port.Write(data, 0, data.Length);
		}

	}
}
