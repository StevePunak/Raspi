using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Structure;

namespace RaspiCommon.Spatial
{
	public class DrawingParameters
	{
		public MCvScalar Color { get; set; }
		public int LineWidth { get; set; }
	}

	public class Drawing
	{
		public static MCvScalar Black = new Bgr(Color.Black).MCvScalar;

		public static Dictionary<SpatialObjects, DrawingParameters> ObjectParms = new Dictionary<SpatialObjects, DrawingParameters>()
		{
			{ SpatialObjects.Landmarks,           new DrawingParameters() { Color = new Bgr(Color.Blue).MCvScalar,        LineWidth = 2,  } },
			{ SpatialObjects.GridLines,           new DrawingParameters() { Color = new Bgr(Color.LightCyan).MCvScalar,   LineWidth = 1,  } },
			{ SpatialObjects.Background,          new DrawingParameters() { Color = new Bgr(Color.Black).MCvScalar,       LineWidth = 1,  } },
			{ SpatialObjects.CurrentLocation,     new DrawingParameters() { Color = new Bgr(Color.Gold).MCvScalar,        LineWidth = 2,  } },
			{ SpatialObjects.Labels,				new DrawingParameters() { Color = new Bgr(Color.White).MCvScalar,       LineWidth = 1,  } },
			{ SpatialObjects.PossibleLocation,    new DrawingParameters() { Color = new Bgr(Color.Yellow).MCvScalar,      LineWidth = 1,  } },
			{ SpatialObjects.Circles,             new DrawingParameters() { Color = new Bgr(Color.Orchid).MCvScalar,      LineWidth = 1,  } },
		};
	}
}
