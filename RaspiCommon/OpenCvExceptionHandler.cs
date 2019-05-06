using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Dispatcher;
using KanoopCommon.Logging;

namespace RaspiCommon
{
	public class OpenCvExceptionHandler : ExceptionHandler
	{
		// HandleException method override gives control to 
		// your code.
		public override bool HandleException(Exception e)
		{
			// This method contains logic to decide whether 
			// the exception is serious enough
			// to terminate the process.
			Log.SysLogText(LogLevel.ERROR, "OpenCVException: {0}", e.Message);

			return ShouldTerminateProcess(e);
		}

		public bool ShouldTerminateProcess(Exception ex)
		{
			// Write your logic here.
			return false;
		}
	}
}
