using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Lidar
{
	public class SetMotorPwm : LidarCommand
	{
		public SetMotorPwm(UInt16 pwm)
			: base(CommandCode.SetMotorPwm)
		{
			Payload = BitConverter.GetBytes(pwm);
		}
	}

	public class ResetCommand : LidarCommand
	{
		public ResetCommand()
			: base(CommandCode.Reset) { }
	}

	public class StopCommand : LidarCommand
	{
		public StopCommand()
			: base(CommandCode.Stop) { }
	}

	public class StartScanCommand : LidarCommand
	{
		public StartScanCommand()
			: base(CommandCode.Scan) { }
	}

	public class StartExpressScanCommand : LidarCommand
	{
		public StartExpressScanCommand()
			: base(CommandCode.ExpressScan, new byte[5] { 0, 0, 0, 0, 0 }) { }
	}

	public class GetSampleRateCommand : LidarCommand
	{
		public GetSampleRateCommand()
			: base(CommandCode.GetSampleRate) { }
	}

	public class GetAccBoardCommand : LidarCommand
	{
		public GetAccBoardCommand()
			: base(CommandCode.GetAccBoardFlag, new byte[4] { 0, 0, 0, 0 } ) {}
	}

	public class GetDeviceInfoCommand : LidarCommand
	{
		public GetDeviceInfoCommand()
			: base(CommandCode.GetDeviceInfo) {}
	}

	public abstract class LidarCommand
	{
		public const byte Marker = 0xa5;

		const int HEADER_SIZE = 2;
		const byte HAS_PAYLOAD = 0x80;
		const int SIZE_SIZE = 1;
		const int CHECKSUM_SIZE = 1;

		public enum CommandCode
		{
		// Commands without payload and response		
			Stop = 0x25,
			Scan = 0x20,
			ForceScan = 0x21,
			Reset = 0x40,

		// Commands without payload but have response
			GetDeviceInfo = 0x50,
			GetDeviceHealth = 0x52,
			GetSampleRate = 0x59, //added in fw 1.17,
			HqMotorSpeedCtrl = 0xA8,

		// Commands with payload and have response
			ExpressScan = 0x82, //added in fw 1.17,
			HqScan = 0x83, //added in fw 1.24,
			GetLidarConf = 0x84, //added in fw 1.24,
			SetLidarConf = 0x85, //added in fw 1.24,

		//add for A2 to set RPLIDAR motor pwm when using accessory board
			SetMotorPwm = 0xF0,
			GetAccBoardFlag = 0xFF,
		}

		public CommandCode Code { get; private set; }

		public byte[] Payload { get; protected set; }

		public byte Checksum { get; private set; }

		bool HasPayload { get { return Payload != null && Payload.Length > 0; } }

		protected LidarCommand(CommandCode commandCode, byte[] payload = null)
		{
			Code = commandCode;
			Payload = payload;
		}

		public byte[] Serialize()
		{
			int length = HasPayload ? HEADER_SIZE + SIZE_SIZE + Payload.Length + CHECKSUM_SIZE : HEADER_SIZE;
			byte[] data = new byte[length];
			byte cs = 0;

			using(MemoryStream ms = new MemoryStream(data))
			using(BinaryWriter bw = new BinaryWriter(ms))
			{
				bw.Write((byte)Marker);
				cs ^= (byte)Marker;
				bw.Write((byte)Code);
				cs ^= (byte)Code;

				if(length > HEADER_SIZE)
				{
					bw.Write((byte)Payload.Length);
					cs ^= (byte)Payload.Length;

					bw.Write(Payload, 0, Payload.Length);
					foreach(byte b in Payload)
					{
						cs ^= b;
					}
					bw.Write(cs);
				}
			}
			return data;
		}

	}

}
