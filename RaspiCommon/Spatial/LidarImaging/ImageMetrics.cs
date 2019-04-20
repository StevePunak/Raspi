using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Spatial.LidarImaging
{
	public class ImageMetrics
	{
		public Double MetersSquare { get; set; }
		public Double PixelsPerMeter { get; set; }
		public Double Scale { get { return 1 / PixelsPerMeter; } }
	}
}
