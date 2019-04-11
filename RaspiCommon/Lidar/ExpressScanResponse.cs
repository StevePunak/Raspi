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
	public class ExpressScanResponse : LidarResponse
	{
		public byte Sync { get; private set; }
		public byte Checksum { get; private set; }
		public Double StartAngle { get; private set; }
		public bool NewScan { get; private set; }

		public byte[] Bytes { get; private set; }

		public List<LidarTypes.Cabin> Cabins { get; private set; }

		public ExpressScanResponse(byte[] responseBytes)
		{
			Cabins = new List<LidarTypes.Cabin>();
			using(MemoryStream ms = new MemoryStream(responseBytes))
			using(BinaryReader br = new BinaryReader(ms))
			{
				Bytes = br.ReadBytes(4);

				byte b0 = Bytes[0];
				byte b1 = Bytes[1];
				byte b2 = Bytes[2];
				byte b3 = Bytes[3];

				byte s0 = (byte)(b0 >> 4);
				byte s1 = (byte)(b1 >> 4);
				Sync = (byte)((s0 << 4) | s1);

				byte c0 = (byte)(b0 & 0x0f);
				byte c1 = (byte)(b1 & 0x0f);
				Checksum = (byte)((c0 << 4) | c1);

				Double q6Angle = ((b3 << 8) | b2) & 0x7fff;
				StartAngle = q6Angle / 64;



				//int q6Int = q6Angle >> 6;
				//int q6Frac = q6Angle & 0x3f;

				//String val = String.Format("{0}.{1}", q6Int, q6Frac);
				//StartAngle = Double.Parse(val);

				NewScan = (b3 & 0x80) != 0 ? true : false;

				for(int x = 0;x < 16;x++)
				{
					LidarTypes.Cabin cabin = new LidarTypes.Cabin(this, br.ReadBytes(5));
					Cabins.Add(cabin);
				}

			}
		}

		public void DumpToLog()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}\n", ToString());
			foreach(LidarTypes.Cabin cabin in Cabins)
			{
				sb.AppendFormat("{0}\n", cabin.ToString());
			}
			Log.SysLogText(LogLevel.DEBUG, "{0}", sb);
		}

		public override string ToString()
		{
			return String.Format("MEASUREMENT:  {0}  StartAngle {1:0.00} Sync: {2:X2}  Cksum: {3:X2} NewScan: {4}",
				Bytes.ToHexString(), StartAngle, Sync, Checksum, NewScan);
		}
	}

}
