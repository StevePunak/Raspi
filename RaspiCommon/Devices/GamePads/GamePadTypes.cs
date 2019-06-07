using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;
using KanoopCommon.Extensions;

namespace RaspiCommon.Devices.GamePads
{
	public enum GamePadType
	{
		[InfoKey("ZD")]
		ZD,
		[InfoKey("Steel")]
		SteelSeries,
		[InfoKey("ESM")]
		ESM,
		[InfoKey("USB")]
		USB,
		[InfoKey("XINPUT")]
		XBoxOneController,
	}

	public class ControlBase
	{
		public String Name { get; set; }
		public bool Changed { get; set; }
	}

	public class Stick : ControlBase
	{
		public int X { get; set; }
		public int Y { get; set; }
		public bool Press { get; set; }

		public int DeadZoneX { get; set; }
		public int DeadZoneY { get; set; }

		public bool ContinuousOn { get; set; }

		public bool Reverse { get; set; }
		public bool Neutral
		{
			get
			{
				bool neutral = X.AbsoluteDifference(GamePadTypes.NEUTRAL) < DeadZoneX && Y.AbsoluteDifference(GamePadTypes.NEUTRAL) < DeadZoneY;
				return neutral;
			}
		}

		public Stick()
		{
			Name = String.Empty;
		}

		public override string ToString()
		{
			return $"{Name} {X},{Y}  Press {Press}";
		}
	}

	public class Hat : ControlBase
	{
		public Button Left { get; set; }
		public Button Right { get; set; }
		public Button Up { get; set; }
		public Button Down { get; set; }

		public bool Neutral { get { return Left.State == false && Right.State == false && Up.State == false && Down.State == false; } }

		public Hat()
		{
			Name = String.Empty;
			Left = new Button() { Name = "Left" };
			Right = new Button() { Name = "Right" };
			Up = new Button() { Name = "Up" };
			Down = new Button() { Name = "Down" };
		}

		public override string ToString()
		{
			return $"{Name} Left {Left} Right {Right} Up {Up} Down {Down}";
		}
	}

	public class Button : ControlBase
	{
		public bool State { get; set; }
		public bool Continuous { get; set; }

		public Button()
		{
			Name = String.Empty;
			Continuous = false;
		}

		public override string ToString()
		{
			return $"{Name} {State}";
		}
	}

	public class AnalogControl : ControlBase
	{
		public int Value { get; set; }
		public int DeadZone { get; set; }
	}

	public class InfoKeyAttribute : StringAttribute
	{
		public InfoKeyAttribute(string value) 
			: base(value)
		{
		}
	}

	public class GamePadTypes
	{
		public const int NEUTRAL = 32767;
		public const int MAX_STICK = 65535;
		public const int MAX_ANALOG = 65535;
	}
}