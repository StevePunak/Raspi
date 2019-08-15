using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Serialization;

namespace RaspiCommon.Devices.Servos
{
	[IsSerializable]
	public class ServoParameters
	{
		public String Name { get; set; }
		public GpioPin Pin { get; set; }
		public int Minimum { get; set; }
		public int Maximum { get; set; }
		public int CurrentSetting { get; set; }
		public int DestinationSetting { get; set; }
		public int Quantum { get; set; }

		public bool IsIdle { get { return CurrentSetting == DestinationSetting; } }

		public ServoParameters()
		{
			CurrentSetting = 1500;
			DestinationSetting = 1500;
			Quantum = 50;
		}

		public void SetDestinationPercentage(int percent)
		{
			Double where = MakePercentage(percent);
			DestinationSetting = (int)where;
		}

		public void Center()
		{
			SetDestinationPercentage(50);
			CurrentSetting = DestinationSetting;
		}

		int MakePercentage(int percent)
		{
			return (int)(((Double)Maximum - (Double)Minimum) * ((Double)percent / 100) + Minimum);
		}

		public override string ToString()
		{
			String movement = IsIdle ? "Idle" : $"at {CurrentSetting} Moving to {DestinationSetting}";
			return $"{Name} on pin {Pin} {movement}";
		}
	}
}
