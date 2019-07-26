using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.PiGpio
{
	internal interface IPigsGpio
	{
		void SetMode(GpioPin gpioPin, PinMode mode);
		void SetPullUp(GpioPin gpioPin, PullUp state);
		void SetHardwarePWM(GpioPin gpioPin, UInt32 frequency, UInt32 dutyCyclePercent);
		void SetPWM(GpioPin gpioPin, UInt32 dutyCycle);
		void SetServoPosition(GpioPin gpioPin, int position);
		void SetServoPosition(GpioPin gpioPin, UInt32 position);
		void SetOutputPin(GpioPin gpioPin, PinState value);
		void SetOutputPin(GpioPin gpioPin, bool value);
		void StartInputPin(GpioPin pin, EdgeType edgeType, GpioInputCallback callback);
		void StopInputPin(GpioPin pin);

		void Stop();
		void Start();
	}
}
