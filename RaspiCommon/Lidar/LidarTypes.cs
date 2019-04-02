using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;

namespace RaspiCommon.Lidar
{
	public delegate void LidarSampleHandler(LidarSample sample);
	public delegate void LidarResponseHandler(LidarResponse response);
	public delegate void FuzzyPathChangedHandler(FuzzyPath path);

	public class LidarTypes
	{
		public enum ResponseType
		{
			DevInfo = 0x4,
			DevHealth = 0x6,
			Measurement = 0x81,
			MeasurementCapsuled = 0x82,
			MeasurementHq = 0x83,
			SampleRate = 0x15,
			MeasurementCapsuledUltra = 0x84,
			GetLidarConf = 0x20,
			SetLidarConf = 0x21,
			AccBoardFlag = 0xFF,
		}

		public class Cabin
		{
			public Double Distance1 { get; private set; }
			public Double Distance2 { get; private set; }
			public Double OffsetAngle1 { get; private set; }
			public Double OffsetAngle2 { get; private set; }
			public Double ActualAngle1 { get; private set; }
			public Double ActualAngle2 { get; private set; }

			public byte[] Bytes { get; private set; }

			public Cabin(ExpressScanResponse parent, byte[] frame)
			{
				Bytes = frame;

				Distance1 = (Double)((UInt16)(((int)frame[0] >> 2) | ((int)frame[1] << 8))) / 1000;
				Distance2 = (Double)((UInt16)(((int)frame[2] >> 2) | ((int)frame[3] << 8))) / 1000;
				OffsetAngle1 = (frame[0] & 0x03) | ((frame[4] & 0x08) << 2);
				OffsetAngle2 = (frame[2] & 0x03) | ((frame[4] & 0x80) >> 2);

				int chunk_0_0 = frame[0] & 0x1;
				int chunk_0_1 = frame[4] & 0xf;
				int chunk_1_0 = frame[2] & 0x1;
				int chunk_1_1 = frame[4] >> 4;
				int q3_1 = (chunk_0_1 << 1) | chunk_0_0;
				int q3_2 = (chunk_1_1 << 1) | chunk_1_0;

				int a0 = q3_1 >> 3;
				int a1 = q3_1 & 0x7;
				int b0 = q3_2 >> 3;
				int b1 = q3_2 & 0x7;

				OffsetAngle1 = (Double)q3_1 / 8.0;
				ActualAngle1 = OffsetAngle1.AddDegrees(parent.StartAngle);
				OffsetAngle2 = (Double)q3_2 / 8.0;
				ActualAngle2 = OffsetAngle2.AddDegrees(parent.StartAngle);
			}

			public override string ToString()
			{
				return String.Format("{0}  Distance 1: {1}  Distance 2: {2}  Angle 1: {3}  Angle 2: {4}",
					Bytes.ToHexString(), Distance1, Distance2, ActualAngle1, ActualAngle2);
			}
		}

	}
}
