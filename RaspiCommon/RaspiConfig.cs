using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;
using KanoopCommon.PersistentConfiguration;

namespace RaspiCommon
{
	public class RaspiConfig : ProgramConfiguration
	{
		const String CONFIG_FILE = "/var/robo/robo.config";

		public String LogFileName
		{
			get { return "/var/log/robo/robo.log";  }
		}

		String _lastAuthToken;
		public String LastAuthToken
		{
			get
			{
				if(_lastAuthToken == null)
				{
					_lastAuthToken = String.Empty;
				}
				return _lastAuthToken;
			}
			set { _lastAuthToken = value; }
		}

		Dictionary<String, GpioPin> _pinSelectionControls;
		public Dictionary<String, GpioPin> PinSelectionControls
		{
			get
			{
				if(_pinSelectionControls == null)
				{
					_pinSelectionControls = new Dictionary<String, GpioPin>();
				}
				return _pinSelectionControls;
			}
			set { _pinSelectionControls = value; }
		}

		public int TracksSpeed { get; set; }

		public GpioPin TracksRightA1Pin { get; set; }
		public GpioPin TracksLeftA1Pin { get; set; }
		public GpioPin TracksLeftA2Pin { get; set; }
		public GpioPin TracksRightA2Pin { get; set; }
		public GpioPin TracksRightEnaPin { get; set; }
		public GpioPin TracksLeftEnaPin { get; set; }
		public GpioPin EscControlPinPin { get; set; }
		public GpioPin RangeFinderOuputPin { get; set; }
		public GpioPin RangeFinderInputPin { get; set; }
		public GpioPin ServoPin { get; set; }
		public GpioPin StepperB1Pin { get; set; }
		public GpioPin StepperB2Pin { get; set; }
		public GpioPin StepperA1Pin { get; set; }
		public GpioPin StepperA2Pin { get; set; }

		public Double CompassXAdjust { get; set; }
		public Double CompassYAdjust { get; set; }
		public Double MagneticDeviation { get; set; }

		public static String GetDefaultConfigFileName()
		{
			return CONFIG_FILE;
		}
	}
}
