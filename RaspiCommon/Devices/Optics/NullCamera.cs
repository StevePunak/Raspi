using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace RaspiCommon.Devices.Optics
{
	public class NullCamera : Camera
	{
		public NullCamera() 
			: base(typeof(NullCamera).Name)
		{
		}

		public override bool TryTakeSnapshot(out Mat image)
		{
			throw new NotImplementedException();
		}

		protected override bool OnRun()
		{
			Interval = TimeSpan.FromSeconds(1);
			return true;
		}
	}
}
