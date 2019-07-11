using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon.Extensions;

namespace RaspiCommon.Lidar
{
	public class LidarVector : IVector
	{
		public BearingAndRange BearingAndRange { get; set; }
		public Double Bearing { get { return BearingAndRange.Bearing; } set { BearingAndRange.Bearing = value; } }
		public Double Range { get { return BearingAndRange.Range; } set { BearingAndRange.Range = value; } }
		public DateTime RefreshTime { get; set; }

		public LidarVector()
		{
			BearingAndRange = new BearingAndRange();
			RefreshTime = DateTime.MinValue;
		}

		/// <summary>
		/// Load the input array from the given blob
		/// </summary>
		/// <param name="vectors"></param>
		/// <param name="blob"></param>
		public static void LoadFromRangeBlob(LidarVector[] vectors, byte[] blob)
		{
			if(blob.Length == 0)
				return;
			Double vectorSize = (Double)360 / (Double)vectors.Length;
			using(BinaryReader br = new BinaryReader(new MemoryStream(blob)))
			{
				for(int offset = 0;offset < vectors.Length;offset++)
				{
					Double range = br.ReadDouble();
					//if(range != 0)
					//	count++;
					vectors[offset] = new LidarVector()
					{
						Bearing = (Double)offset * vectorSize,
						Range = range
					};
					vectors[offset].RefreshTime = DateTime.UtcNow;
				}
			}
		}

		public static Mat MakeBitmap(LidarVector[] vectors, Size size, Double pixelsPerMeter, Color dotColor)
		{
			Double vectorSize = (Double)360 / (Double)vectors.Length;
			Mat mat = new Mat(size, DepthType.Cv8U, 3);
			PointD center = mat.Center();
			MCvScalar color = new Bgr(dotColor).MCvScalar;

			for(Double offset = 0;offset < vectors.Length;offset++)
			{
				Double bearing = offset * vectorSize;
				Double rangeMeters = vectors[(int)offset].Range;
				Double range = rangeMeters * pixelsPerMeter;

				if(range != 0)
				{
					PointD point = center.GetPointAt(bearing, range) as PointD;
					CvInvoke.Line(mat, point.ToPoint(), point.ToPoint(), color);
				}
			}

			Rectangle rect = new Rectangle(center.ToPoint(), new Size(1, 1));
			CvInvoke.Rectangle(mat, rect, new Bgr(Color.LightSalmon).MCvScalar);

			return mat;
		}

		public override string ToString()
		{
			return String.Format("{0:0.000}m at {1:0.00}°", Range, Bearing);
		}

	}
}
