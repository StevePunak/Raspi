#undef DEBUG_SERIAL

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Queueing;
using KanoopCommon.Serial;
using KanoopCommon.Threading;
using RaspiCommon;
using RaspiCommon.Lidar;

namespace RaspiCommon
{

	public partial class RPLidar
	{
		#region Public Properties

		public event LidarResponseHandler LidarResponseData;
		public event LidarSampleHandler Sample;

		public String PortName { get; private set; }

		public MonoSerialPort Port { get; private set; }

		public bool Active { get { return _lastGoodSampleTime > DateTime.UtcNow - TimeSpan.FromSeconds(1); } }

		public Double Offset { get; set; }

		public Double RenderPixelsPerMeter { get { return 50; } } 

		#endregion

		#region Privte Member Variables

		public Double[] Vectors { get; private set; }

		byte[] _receiveBuffer;
		int _bytesInBuffer;
		int _recvOffset;
		State _state;

		DateTime _lastGoodSampleTime;

		Int32 _chunkLength;
		ResponseMode _responseMode;
		LidarTypes.ResponseType _responseType;

		byte[] _responseData;

		MemoryQueue<LidarResponse> _responseQueue;

		int _responseWaiters;

		#endregion

		enum State
		{
			Sync,
			StartFlag,
			LengthModeAndType,
			SingleResponse,
			MultiResponse
		}

		enum ResponseMode
		{
			SingleRequestSingleResponse = 0x0,
			SingleRequestMultipleResponse = 0x01,
			Reserved1 = 0x02,
			Reserved2 = 0x03,
		}

		public RPLidar(String portName)
		{
			Vectors = new double[360];
			Array.Clear(Vectors, 0, 360);

			List<String> ports = new List<String>(SerialPort.GetPortNames());
			ports.TerminateAll();
			if(ports.Contains(portName) == false)
			{
				throw new RaspiException("Invalid port name '{0}'", portName);
			}
			PortName = portName;
			_receiveBuffer = new byte[1000000];
			_responseQueue = new MemoryQueue<LidarResponse>();
			_responseWaiters = 0;

			LidarResponseData += delegate { };
			Sample += delegate { };
		}

		public void Start()
		{
			_recvOffset = _bytesInBuffer = 0;
			_state = State.Sync;
			Port = new MonoSerialPort(PortName);
			Port.DataReceived += OnSerialDataReceived;
			Port.Open();

			Port.Port.DtrEnable = false;
		}

		public void Stop()
		{
			if(Port != null && Port.IsOpen)
			{
				Log.SysLogText(LogLevel.DEBUG, "Closing Port");
				Port.Port.DtrEnable = true;
				Port.DataReceived -= OnSerialDataReceived;
				Port.Close();
				Port = null;
				Log.SysLogText(LogLevel.DEBUG, "Done");
			}
		}

		public bool TryGetResponse(TimeSpan waitTime, out LidarResponse response)
		{
			_responseWaiters++;
			return _tryGetResponse(waitTime, out response);
		}

		public Bitmap GenerateBitmap()
		{
			Bitmap bitmap = new Bitmap(1000, 1000);

			PointD center = new PointD(bitmap.Width / 2, bitmap.Height / 2);

			using(Graphics g = Graphics.FromImage(bitmap))
			{
				g.FillRectangle(new SolidBrush(Color.Black), new RectangleF(new PointF(0, 0), bitmap.Size));

				Pen redPen = new Pen(Color.Red);
				for(int degrees = 0;degrees < 360;degrees++)
				{
					Double distance = Vectors[degrees];
					if(distance != 0)
					{
						Double lineLength = distance * RenderPixelsPerMeter;

						PointD point = FlatGeo.GetPoint(center, degrees, lineLength);
						g.FillRectangle(new SolidBrush(Color.Red), new RectangleF(point.ToPoint(), new Size(1, 1)));
					}

				}
			}

			return bitmap;
		}

		public bool StartExpressScan()
		{
			LidarCommand command = new StartExpressScanCommand();
			SendCommand(command);

			LidarResponse response;
			return _tryGetResponse(TimeSpan.FromMilliseconds(500), out response);
		}

		public bool StopExpressScan()
		{
			_responseQueue.Clear();
			LidarCommand command = new StopCommand();
			SendCommand(command);

			LidarResponse response;
			_tryGetResponse(TimeSpan.FromMilliseconds(250), out response);
			return true;
		}

		public bool GetDeviceInfo()
		{
			LidarCommand command = new GetDeviceInfoCommand();
			SendCommand(command);

			LidarResponse response;
			bool result = TryGetResponse(TimeSpan.FromMilliseconds(500), out response);
			return result;
		}

		public bool StartScan()
		{
			LidarCommand command = new StartScanCommand();
			SendCommand(command);
			return true;
			LidarResponse response;
			return _tryGetResponse(TimeSpan.FromMilliseconds(5000), out response);
		}

		public bool StopScan()
		{
			_responseQueue.Clear();
			LidarCommand command = new StopCommand();
			SendCommand(command);

			LidarResponse response;
			_tryGetResponse(TimeSpan.FromMilliseconds(250), out response);
			return true;
		}

		public bool StopMotor()
		{
			Port.Port.DtrEnable = true;

			_responseQueue.Clear();
			LidarCommand command = new SetMotorPwm(0);
			SendCommand(command);

			LidarResponse response;
			_tryGetResponse(TimeSpan.FromMilliseconds(250), out response);
			return true;
		}

		public bool Reset()
		{
			LidarCommand command = new ResetCommand();
			SendCommand(command);

			LidarResponse response;
			return _tryGetResponse(TimeSpan.FromMilliseconds(500), out response);
		}

		public bool _tryGetResponse(TimeSpan waitTime, out LidarResponse response)
		{
			response = _responseQueue.BlockDequeue(waitTime);
//			Log.SysLogText(LogLevel.DEBUG, "Got {0}", response == null ? "null" : response.ToString());
			return response != null;
		}

		private void OnSerialDataReceived(byte[] buffer, int length)
		{
			try
			{
#if DEBUG_SERIAL
				Log.SysLogText(LogLevel.DEBUG, "INBOUND");
				Log.SysLogHex(LogLevel.DEBUG, buffer, 0, length);
#endif
				if(length + _bytesInBuffer > _receiveBuffer.Length)
				{
					Log.SysLogText(LogLevel.WARNING, "Buffer would overflow - Flushing buffer and resyncing");
					StartSync();
				}
				Array.Copy(buffer, 0, _receiveBuffer, _bytesInBuffer, length);
				_bytesInBuffer += length;
#if DEBUG_SERIAL
				Log.SysLogText(LogLevel.DEBUG, "ALL");
				Log.SysLogHex(LogLevel.DEBUG, _receiveBuffer, 0, _bytesInBuffer);
#endif
				bool completedState = true;
				do
				{
					switch(_state)
					{
						case State.Sync:
							if((completedState = Sync()))
							{
								_state = State.StartFlag;
							}
							break;
						case State.StartFlag:
							if((completedState = StartFlag()))
							{
								_state = State.LengthModeAndType;
							}
							break;
						case State.LengthModeAndType:
							if((completedState = LengthModeAndType()))
							{
#if DEBUG_SERIAL
								Log.SysLogText(LogLevel.DEBUG, "Response mode ===>>> {0}", _responseMode);
#endif
								switch(_responseMode)
								{
									case ResponseMode.SingleRequestSingleResponse:
										_state = State.SingleResponse;
										break;
									case ResponseMode.SingleRequestMultipleResponse:
										_state = State.MultiResponse;
										break;
									case ResponseMode.Reserved1:
									case ResponseMode.Reserved2:
									default:
										StartSync();
										break;
								}
							}
							break;
						case State.SingleResponse:
							if((completedState = Response()))
							{
#if DEBUG_SERIAL
								Log.SysLogText(LogLevel.DEBUG, "RECEVED COMPLETE SINGLE RESPONSE");
#endif
								LidarResponse response = LidarResponse.Create(_responseType, _responseData);
								if(response != null)
								{
									LidarResponseData(response);
									if(_responseWaiters > 0)
									{
										_responseQueue.Enqueue(response);
									}
								}
								Log.SysLogHex(LogLevel.DEBUG, _responseData);
								_state = State.Sync;
							}
							break;
						case State.MultiResponse:
#if DEBUG_SERIAL
							Log.SysLogText(LogLevel.DEBUG, "State ===>>> {0} have {1} bytes  offset {2}", _state, _bytesInBuffer, _recvOffset);
#endif
							if((completedState = Response()))
							{
#if false
								Log.SysLogText(LogLevel.DEBUG, "RECEVED COMPLETE MULTI RESPONSE now have {0} bytes  offset {1}", _bytesInBuffer, _recvOffset);
								Log.SysLogHex(LogLevel.DEBUG, _responseData);
#endif
								LidarResponse response = LidarResponse.Create(_responseType, _responseData);
								if(response != null)
								{
									if(response is ExpressScanResponse)
									{
										ProcessExpressScanResponse(response as ExpressScanResponse);
									}
									else if(response is ScanResponse)
									{
										ProcessScanResponse(response as ScanResponse);
									}
									LidarResponseData(response);
									if(_responseWaiters > 0)
									{
//										_responseQueue.Enqueue(response);
									}
								}
								else
								{
									StartSync();
								}
							}
							break;
						default:
							break;
					}

#if DEBUG_SERIAL
				Log.SysLogText(LogLevel.DEBUG, "Completed: {0}  offset: {1}   bytes: {2}  state: {3}", completedState, _recvOffset, _bytesInBuffer, _state);
#endif
				} while(completedState && _recvOffset < _bytesInBuffer);

#if DEBUG_SERIAL
				Log.SysLogText(LogLevel.DEBUG, "Now in State {0}, offset {1} there are {2} bytes left", _state, _recvOffset, _bytesInBuffer);
#endif
				int shift = _recvOffset;
				if(shift > 0)
				{
					Array.Copy(_receiveBuffer, _recvOffset, _receiveBuffer, 0, shift);
				}
				_bytesInBuffer -= shift;
				_recvOffset = 0;
#if DEBUG_SERIAL
				Log.SysLogText(LogLevel.DEBUG, "Shifting {0} bytes there are {1} left offset {2}", shift, _bytesInBuffer, _recvOffset);
#endif
			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.ERROR, "Exception: {0}", e.Message);
			}
		}

		private void ProcessScanResponse(ScanResponse response)
		{
			if(response.Quality > 10 && response.CheckBit == 1 && response.StartFlag == 1 && response.Angle < 360 && response.Angle >= 0)
			{
				Double angle = response.Angle.AddDegrees(Offset);
				Vectors[(int)angle] = Math.Max(response.Distance, .001);
				_lastGoodSampleTime = DateTime.UtcNow;
			}
			//int intDeg = (int)response.Angle;
			//for(int u = (int)_lastScanHeading.AddDegrees(1);intDeg != response.Angle;intDeg++)
			//{

			//}
		}

		private void ProcessExpressScanResponse(ExpressScanResponse response)
		{
			foreach(LidarTypes.Cabin cabin in response.Cabins)
			{
				LidarSample sample1 = new LidarSample(cabin.ActualAngle1, cabin.Distance1, DateTime.UtcNow);
				Vectors[(int)cabin.ActualAngle1] = cabin.Distance1;
				Sample(sample1);
				LidarSample sample2 = new LidarSample(cabin.ActualAngle2, cabin.Distance2, DateTime.UtcNow);
				Vectors[(int)cabin.ActualAngle2] = cabin.Distance2;
				Sample(sample2);

				Log.SysLogText(LogLevel.DEBUG, "Put {0:0.00} into {1} and {2:0.00} into {3}", cabin.Distance1, (int)cabin.ActualAngle1, cabin.Distance2, (int)cabin.ActualAngle2);
			}
		}

		bool Sync()
		{
			return SeekToByte(LidarProtocol.SYNC);
		}

		bool StartFlag()
		{
			return SeekToByte(LidarProtocol.START_FLAG);
		}

		bool SeekToByte(byte b)
		{
			bool result = false;

			int x = 0;
			for(x = _recvOffset;x < _bytesInBuffer && _receiveBuffer[x] != b;x++) ;
#if DEBUG_SERIAL
			Log.SysLogText(LogLevel.DEBUG, "Seeking to 0x{0:X2}  x is {1}  bytes: {2} ", b, x, _bytesInBuffer);
#endif
			if(x < _bytesInBuffer)
			{
				_recvOffset = x + 1;
				result = true;
			}
			else
			{
				StartSync();
			}

			return result;
		}

		bool LengthModeAndType()
		{
			bool result = false;
			if(_bytesInBuffer - _recvOffset >= 5)
			{
				byte[] thebytes = new byte[4];
				Array.Copy(_receiveBuffer, _recvOffset, thebytes, 0, 4);
				UInt32 value = BitConverter.ToUInt32(thebytes, 0);
				_chunkLength = (int)(value & 0x3FFF);
				_responseMode = (ResponseMode)(value >> 30);
				_responseType = (LidarTypes.ResponseType)_receiveBuffer[_recvOffset + 4];

#if DEBUG_SERIAL
				Log.SysLogText(LogLevel.DEBUG, "len: 0x{0:X4}  mode: {1} value: 0x{2:X4} type: {3}", _chunkLength, _responseMode, value, _responseType);
#endif
				_recvOffset += 5;
				result = true;
			}
			return result;
		}

		bool Response()
		{
			bool result = false;
			if(_bytesInBuffer - _recvOffset >= _chunkLength)
			{
				_responseData = new byte[_chunkLength];
				Array.Copy(_receiveBuffer, _recvOffset, _responseData, 0, _chunkLength);
				_recvOffset += _chunkLength;
				result = true;
			}
			return result;
		}

		void StartSync()
		{
//			Log.SysLogText(LogLevel.WARNING, "Start sync with {0} bytes", _bytesInBuffer);
			_bytesInBuffer = _recvOffset = 0;
			_state = State.Sync;
		}

		public void SendCommand(LidarCommand command)
		{
			byte[] data = command.Serialize();
			//Log.SysLogText(LogLevel.DEBUG, "OUTPUT!!!!!");
			//Log.SysLogHex(LogLevel.DEBUG, data);
			Port.Write(data, 0, data.Length);
		}

	}
}
