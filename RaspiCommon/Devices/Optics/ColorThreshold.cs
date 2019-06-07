using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Structure;
using KanoopCommon.Serialization;

namespace RaspiCommon.Devices.Optics
{
	[IsSerializable]
	public class ColorThresholdList : Dictionary<Color, ColorThreshold> { }

	[IsSerializable]
	public class ColorThreshold
	{
		public Color Color { get; set; }
		public int MinimumValue { get; set; }
		public int MaximumOtherValue { get; set; }

		public ColorThreshold()
		{
			Color = Color.White;
		}
		public ColorThreshold(Color color, int minimumValue, int maxOtherValue)
		{
			Color = color;
			MinimumValue = minimumValue;
			MaximumOtherValue = maxOtherValue;
		}

		public void MakeMCvScalarArrays(out MCvScalar lowRange, out MCvScalar topRange)
		{
			MakeMCvScalarArrays(Color, MinimumValue, MaximumOtherValue, out lowRange, out topRange);
		}

		public static void MakeMCvScalarArrays(Color color, int minimumValue, int maximumOtherValue, out MCvScalar lowRange, out MCvScalar topRange)
		{
			if(color == Color.Blue)
			{
				lowRange = new MCvScalar(minimumValue, 0, 0);
				topRange = new MCvScalar(255, maximumOtherValue, maximumOtherValue);
			}
			else if(color == Color.Green)
			{
				lowRange = new MCvScalar(0, minimumValue, 0);
				topRange = new MCvScalar(maximumOtherValue, 255, maximumOtherValue);
			}
			else if(color == Color.Red)
			{
				lowRange = new MCvScalar(0, 0, minimumValue);
				topRange = new MCvScalar(maximumOtherValue, maximumOtherValue, 255);
			}
			else
			{
				lowRange = new MCvScalar(0, 0, 0);
				topRange = new MCvScalar(255, 255, 255);
			}
		}

		public override string ToString()
		{
			return String.Format("{0} Threshold {1}, {2}", Color, MinimumValue, MaximumOtherValue);
		}
	}
}
