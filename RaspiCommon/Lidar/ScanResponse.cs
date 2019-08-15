using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;

namespace RaspiCommon.Lidar
{
	public class ScanResponse : LidarResponse
	{
		public int Quality { get; private set; }
		public int StartFlag { get; private set; }
		public int InverseStartFlag { get; private set; }
		public int CheckBit { get; private set; }
		public int AngleQ6 { get; private set; }
		public Double Angle { get; private set; }
		public int DistanceQ2 { get; private set; }
		public Double Range { get; private set; }
		public bool Valid { get; private set; }

		public byte[] Bytes { get; private set; }

		public ScanResponse(byte[] responseBytes)
		{
			using(MemoryStream ms = new MemoryStream(responseBytes))
			using(BinaryReader br = new BinaryReader(ms))
			{
				Bytes = br.ReadBytes(5);

				byte b0 = Bytes[0];
				byte b1 = Bytes[1];
				byte b2 = Bytes[2];
				byte b3 = Bytes[3];

				Quality = Bytes[0] >> 2;
				StartFlag = Bytes[0] & 0x01;
				InverseStartFlag = (Bytes[0] >> 1) & 0x01;
				CheckBit = Bytes[1] & 0x01;
				AngleQ6 = (Bytes[2] << 7) | (Bytes[1] >> 1);
				Angle = (Double)AngleQ6 / 64.0;
				Range = (((Bytes[4] << 8) | Bytes[3]) / 4.0) / 1000;

				Valid = CheckBit == 1 && StartFlag != InverseStartFlag && Angle < 360 && Range >= 0 && Range < 100;
			}
		}

		public void DumpToLog()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}\n", ToString());
			Log.SysLogText(LogLevel.DEBUG, "{0}", sb);
		}

		public override string ToString()
		{
			return String.Format("Q: {0}  Angle: {1}  Distance: {2:0.00}    sf: {3}  cb: {4}",
				Quality, Angle, Range, StartFlag, CheckBit);
		}
	}

}
