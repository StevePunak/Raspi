using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace RaspiCommon.Devices.Optics
{
	public class FacePredictionList : List<FacePrediction>
	{
		public bool TryGetPrediction(Rectangle rectangle, out FacePrediction prediction)
		{
			prediction = Find(p => p.Rectangle.Equals(rectangle));
			return prediction != null;
		}
		public bool TryGetPrediction(UInt32 id, out FacePrediction prediction)
		{
			prediction = Find(p => p.ID.Equals(id));
			return prediction != null;
		}
	}

	public class FacePrediction
	{
		public Rectangle Rectangle { get; set; }
		public Mat Image { get; set; }
		public String Name { get; set; }
		public UInt32 ID { get; set; }
		public Double Match { get; set; }

		public FacePrediction()
			: this(Rectangle.Empty, null, String.Empty, 0, 0) { }

		public FacePrediction(Rectangle rectangle, Mat image, String name, Int32 id, Double match)
			: this(rectangle, image, name, (UInt32)id, match) { }

		public FacePrediction(Rectangle rectangle, Mat image, String name, UInt32 id, Double match)
		{
			Rectangle = rectangle;
			Image = image;
			Name = name;
			ID = id;
			Match = match;
		}

		public override string ToString()
		{
			return $"{Name} {Match}";
		}
	}
}
