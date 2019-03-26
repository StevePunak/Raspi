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
		public GpioPin RangeFinderInputPin { get; set; }
		public GpioPin ServoPin { get; set; }
		public GpioPin StepperB1Pin { get; set; }
		public GpioPin StepperB2Pin { get; set; }
		public GpioPin StepperA1Pin { get; set; }
		public GpioPin StepperA2Pin { get; set; }

		public GpioPin LiftInputPin1 { get; set; }
		public GpioPin LiftInputPin2 { get; set; }
		public GpioPin LiftInputPin3 { get; set; }
		public GpioPin LiftInputPin4 { get; set; }

		public Double CompassXAdjust { get; set; }
		public Double CompassYAdjust { get; set; }
		public Double MagneticDeviation { get; set; }

		public Double StopDistance { get { return .15; } }
		public TimeSpan RampUp { get { return TimeSpan.FromMilliseconds(50); } }
		public TimeSpan RampDown { get { return TimeSpan.FromMilliseconds(50); } }
		public Double MaxRangeDetect { get { return .5; } }

		public Double LidarOffsetDegrees { get; set; }

		Dictionary<RFDir, GpioPin> _rangeFinderOutputPins;
		public Dictionary<RFDir, GpioPin>  RangeFinderEchoPins
		{
			get
			{
				if(_rangeFinderOutputPins == null)
				{
					_rangeFinderOutputPins = new Dictionary<RFDir, GpioPin>();
				}
				return _rangeFinderOutputPins;
			}
			set { _rangeFinderOutputPins = value; }
		}

		Dictionary<int, Double> _metersPerSecondAtPower;
		public Dictionary<int, Double> MetersPerSecondAtPower
		{
			get
			{
				if(_metersPerSecondAtPower == null)
				{
					_metersPerSecondAtPower = new Dictionary<int, double>();
				}
				return _metersPerSecondAtPower;
			}
			set { _metersPerSecondAtPower = value; }
		}

		public String LidarComPort { get; set; }

		public static String GetDefaultConfigFileName()
		{
			return CONFIG_FILE;
		}
	}
}
