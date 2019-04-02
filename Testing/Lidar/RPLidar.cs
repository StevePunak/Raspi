#undef DEBUG_LOG

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using KanoopCommon.Queueing;
using KanoopCommon.Serial;
using RaspiCommon;

namespace RaspiCommon.Lidar
{
	public partial class RPLidar
	{
		public String PortName { get; private set; }

		public MonoSerialPort Port { get; private set; }

		byte[] _receiveBuffer;
		int _bytesInBuffer;
		int _recvOffset;
		State _state;

		Int32 _chunkLength;
		ResponseMode _responseMode;
		Lidar.ResponseType _responseType;

		byte[] _responseData;

		MemoryQueue<LidarResponse> _responseQueue;

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
			List<String> ports = new List<String>(SerialPort.GetPortNames());
			ports.TerminateAll();
			if(ports.Contains(portName) == false)
			{
				throw new RaspiException("Invalid port name '{0}'", portName);
			}
			PortName = portName;
			_receiveBuffer = new byte[16384];
			_responseQueue = new MemoryQueue<LidarResponse>();
		}

		public void Start()
		{
			_recvOffset = _bytesInBuffer = 0;
			_state = State.Sync;
			Port = new MonoSerialPort(PortName);
			Port.DataReceived += OnSerialDataReceived;
			Port.Open();
		}

		public bool TryGetResponse(TimeSpan waitTime, out LidarResponse response)
		{
			Log.SysLogText(LogLevel.DEBUG, "Getting from queue with {0}", _responseQueue.Count);
			response = _responseQueue.BlockDequeue(waitTime);
			Log.SysLogText(LogLevel.DEBUG, "Got {0}", response == null ? "null" : response.ToString());
			return response != null;
		}

		private void OnSerialDataReceived(byte[] buffer, int length)
		{
#if DEBUG_LOG
			Log.SysLogText(LogLevel.DEBUG, "INBOUND");
			Log.SysLogHex(LogLevel.DEBUG, buffer, 0, length);
#endif
			Array.Copy(buffer, 0, _receiveBuffer, _bytesInBuffer, length);
			_bytesInBuffer += length;
#if DEBUG_LOG
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
#if DEBUG_LOG
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
#if DEBUG_LOG
							Log.SysLogText(LogLevel.DEBUG, "RECEVED COMPLETE SINGLE RESPONSE");
#endif
							LidarResponse response = LidarResponse.Create(_responseType, _responseData);
							if(response != null)
							{
								Log.SysLogText(LogLevel.DEBUG, "RESP: {0}", response);
								_responseQueue.Enqueue(response);
							}
							Log.SysLogHex(LogLevel.DEBUG, _responseData);
							_state = State.Sync;
						}
						break;
					case State.MultiResponse:
#if DEBUG_LOG
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
								Log.SysLogText(LogLevel.DEBUG, "{0}", response);
								//if(response is MeasurementCapsuledResponse)
								//{
								//	((MeasurementCapsuledResponse)response).DumpToLog();
								//}
								_responseQueue.Enqueue(response);
							}
						}
						break;
					default:
						break;
				}

#if DEBUG_LOG
				Log.SysLogText(LogLevel.DEBUG, "Completed: {0}  offset: {1}   bytes: {2}  state: {3}", completedState, _recvOffset, _bytesInBuffer, _state);
#endif
			} while(completedState && _recvOffset < _bytesInBuffer);

#if DEBUG_LOG
			Log.SysLogText(LogLevel.DEBUG, "Now in State {0}, offset {1} there are {2} bytes left", _state, _recvOffset, _bytesInBuffer);
#endif
			int shift = _recvOffset;
			if(shift > 0)
			{
				Array.Copy(_receiveBuffer, _recvOffset, _receiveBuffer, 0, shift);
			}
			_bytesInBuffer -= shift;
			_recvOffset = 0;
#if DEBUG_LOG
			Log.SysLogText(LogLevel.DEBUG, "Shifting {0} bytes there are {1} left offset {2}", shift, _bytesInBuffer, _recvOffset);
#endif
		}

		bool Sync()
		{
			return SeekToByte(SYNC);
		}

		bool StartFlag()
		{
			return SeekToByte(START_FLAG);
		}

		bool SeekToByte(byte b)
		{
			bool result = false;

			int x = 0;
			for(x = _recvOffset;x < _bytesInBuffer && _receiveBuffer[x] != b;x++) ;
#if DEBUG_LOG
			Log.SysLogText(LogLevel.DEBUG, "Seeking to 0x{0:X2}  x is {1}  bytes: {2} ", b, x, _bytesInBuffer);
#endif
			if(x < _bytesInBuffer)
			{
				_recvOffset = x + 1;
				result = true;
			}
			else
			{
				Console.WriteLine("Resyncing");
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
				_responseType = (Lidar.ResponseType)_receiveBuffer[_recvOffset + 4];

#if DEBUG_LOG
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
			_bytesInBuffer = _recvOffset = 0;
			_state = State.Sync;
		}

		public void Stop()
		{
			if(Port != null && Port.IsOpen)
			{
				Port.DataReceived -= OnSerialDataReceived;
				Port.Close();
				Port = null;
			}
		}

		public void SendCommand(LidarCommand command)
		{
			byte[] data = command.Serialize();
			Log.SysLogText(LogLevel.DEBUG, "OUTPUT!!!!!");
			Log.SysLogHex(LogLevel.DEBUG, data);
			Port.Write(data, 0, data.Length);
		}

	}
}
