using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Devices.Optics.Classifiers
{
	public class FaceFinder : ThingFinder
	{
		public FaceFinder(String cascadeFile)
			: base(cascadeFile)
		{
		}

	}
}
