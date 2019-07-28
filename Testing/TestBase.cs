using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Threading;

namespace Testing
{
	internal abstract class TestBase : ThreadBase
	{
		protected bool Quit { get; set; }

		protected TestBase()
			: base(typeof(TestBase).Name)
		{
			Start();
		}

		protected abstract void Run();

		protected override bool OnRun()
		{
			Run();
			return false;
		}

		public override bool Stop()
		{
			Quit = true;
			return base.Stop();
		}

	}
}
