using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Devices.Optics
{
	public class FoundObjectList : List<FoundObject>
	{

	}

	public class FoundObject
	{
		public Rectangle Rectangle { get; set; }
		public DateTime DetectTime { get; set; }
	}
}
