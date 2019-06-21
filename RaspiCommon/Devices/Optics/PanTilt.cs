using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using RaspiCommon.Devices.Servos;

namespace RaspiCommon.Devices.Optics
{
	public class PanTilt
	{
		int _panPercentage;
		public int Pan
		{
			get { return _panPercentage; }
			set
			{
				_panPercentage = value.EnsureBetween(0, 100);
				_panServo.Value = _panServo.MakePercentage(_panPercentage);
			}
		}

		int _tiltPercentage;
		public int Tilt
		{
			get { return _tiltPercentage; }
			set
			{
				_tiltPercentage = value.EnsureBetween(0, 100);
				_tiltServo.Value = _tiltServo.MakePercentage(_tiltPercentage);
			}
		}

		Servo _panServo { get; set; }
		Servo _tiltServo { get; set; }

		public PanTilt(GpioPin panPin, GpioPin tiltPin)
		{
			_panServo = new Servo(panPin);
			_tiltServo = new Servo(tiltPin);
			Pan = 50;
			Tilt = 50;
		}

		public override string ToString()
		{
			return $"Pan / Tilt on Pan: {_panServo.Pin}  Tilt: {_tiltServo.Pin}";
		}
	}
}
