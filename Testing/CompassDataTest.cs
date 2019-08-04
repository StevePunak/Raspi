using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Network;

namespace Testing
{
	class CompassDataTest : TestBase
	{
		Double _minx = 100, _maxx = -100, _miny = 100, _maxy = -100;
		Double _xadjust, _yadjust;
		protected override void Run()
		{
			NewCalibration();

			_minx = 99;
			_maxx = -99;
			_miny = 99;
			_maxy = -99;

			OriginalCalibration();

			_xadjust = -0.2588600181;
			_yadjust = -0.1278899908;

			MqttListen();
		}

		private void MqttListen()
		{
			MqttCompass compass = new MqttCompass("thufir", MqttTypes.BearingTopic, MqttTypes.RawCompassDataTopic);
			compass.RawData += OnCompass_RawData;
			compass.Start();
			compass.WaitForCompletion();

		}

		private void OnCompass_RawData(double mx, double my, double mz)
		{
			Double adjustedX = mx + _xadjust;
			Double adjustedY = my + _yadjust;

			Double angle = Math.Atan2(adjustedY, adjustedX);
			Double degrees = (angle * 180.0) / Math.PI;

			if(degrees < 0)
				degrees += 360;
			else if(degrees >= 360)
				degrees -= 360;
			Console.WriteLine($"{degrees.ToAngleString()}");
		}

		private void NewCalibration()
		{
			List<String> lines = new List<string>(File.ReadAllLines("c:/pub/tmp/cal.txt"));
			foreach(String line in lines)
			{
				String[] parts = line.Split(' ');
				Double mx, my;
				if(parts.Length > 1 && Double.TryParse(parts[0], out mx) && Double.TryParse(parts[1], out my))
				{
					_minx = Math.Min(_minx, mx);
					_maxx = Math.Max(_maxx, mx);
					_miny = Math.Min(_miny, my);
					_maxy = Math.Max(_maxy, my);
				}
			}

			Double xrange = (_maxx - _minx) / 2;
			_xadjust = -(_maxx - xrange);

			Double yrange = (_maxy - _miny) / 2;
			_yadjust = -(_maxy - yrange);
		}

		void OriginalCalibration()
		{
			List<String> lines = new List<string>(File.ReadAllLines("c:/pub/tmp/cal.txt"));
			foreach(String line in lines)
			{
				String[] parts = line.Split(' ');
				Double mx, my;
				if(parts.Length > 1 && Double.TryParse(parts[0], out mx) && Double.TryParse(parts[1], out my))
				{
					_minx = Math.Min(_minx, mx);
					_maxx = Math.Max(_maxx, mx);
					_miny = Math.Min(_miny, my);
					_maxy = Math.Max(_maxy, my);
				}
			}

			Double xrange = (_maxx - _minx) / 2;
			_xadjust = -(_maxx - xrange);
			Double yrange = (_maxy - _miny) / 2;
			Double _yadjust = -(_maxy - yrange);
		}
	}
}
