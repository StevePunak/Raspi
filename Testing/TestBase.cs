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

		MutexEvent _quitEvent;

		protected TestBase()
			: base(typeof(TestBase).Name)
		{
			_quitEvent = new MutexEvent();
			Console.CancelKeyPress += OnConsoleCancelKeyPress;
			Start();
		}

		private void OnConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Quit = true;
			_quitEvent.Set();
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
			_quitEvent.Set();
			return base.Stop();
		}

		protected void WaitForQuit()
		{
			WaitForQuit(TimeSpan.Zero);
		}

		protected void WaitForQuit(TimeSpan timeout)
		{
			if(timeout == TimeSpan.Zero)
				_quitEvent.Wait();
			else
				_quitEvent.Wait(timeout);
		}

	}
}
