using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using RaspiCommon;

namespace RaspiCommon.Lidar
{
	public class DeviceInfoResponse : LidarResponse
	{
		public byte Model { get; private set; }
		public SoftwareVersion FirmwareVersion { get; private set; }
		public byte Hardware { get; private set; }
		public String SerialNumber { get; private set; }

		public DeviceInfoResponse(byte[] responseBytes)
		{
			using(MemoryStream ms = new MemoryStream(responseBytes))
			using(BinaryReader br = new BinaryReader(ms))
			{
				Model = br.ReadByte();
				byte minor = br.ReadByte();
				byte major = br.ReadByte();
				Hardware = br.ReadByte();
				byte[] serial = br.ReadBytes(16);
				SerialNumber = serial.ToHexString();
			}
		}

		public override string ToString()
		{
			return String.Format("DevInfo Model {0:X2} Ver: {1}  Serial: {2}", Model, FirmwareVersion, SerialNumber);
		}
	}


	public class GetSampleRateResponse : LidarResponse
	{
		public int Standard { get; private set; }
		public int Express { get; private set; }

		public GetSampleRateResponse(byte[] responseBytes)
		{
			using(MemoryStream ms = new MemoryStream(responseBytes))
			using(BinaryReader br = new BinaryReader(ms))
			{
				Standard = br.ReadInt16();
				Express = br.ReadInt16();
			}
		}

		public override string ToString()
		{
			return String.Format("SampleRate {0} {1}", Standard, Express);
		}
	}


	public abstract class LidarResponse
	{
		public static LidarResponse Create(LidarTypes.ResponseType responseType, byte[] responseBytes)
		{
			LidarResponse response = null;

			switch(responseType)
			{
				case LidarTypes.ResponseType.DevInfo:
					response = new DeviceInfoResponse(responseBytes);
					break;
				case LidarTypes.ResponseType.MeasurementCapsuled:
					response = new ExpressScanResponse(responseBytes);
					break;
				case LidarTypes.ResponseType.SampleRate:
					response = new GetSampleRateResponse(responseBytes);
					break;
				case LidarTypes.ResponseType.Measurement:
					response = new ScanResponse(responseBytes);
					break;
				case LidarTypes.ResponseType.DevHealth:
				case LidarTypes.ResponseType.MeasurementHq:
				case LidarTypes.ResponseType.MeasurementCapsuledUltra:
				case LidarTypes.ResponseType.GetLidarConf:
				case LidarTypes.ResponseType.SetLidarConf:
				case LidarTypes.ResponseType.AccBoardFlag:
				default:
					Log.SysLogText(LogLevel.ERROR, "Unsupported type to factory {0}", responseType);
					break;
			}
			return response;
		}
	}
}
