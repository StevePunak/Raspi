using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon;

namespace TrackBot.Tracks
{
	class BotTracks : L298N_DC_TankTracks
	{
		public Double MetersPerSecond = .47;

		public int Speed
		{
			set { LeftSpeed = RightSpeed = value; }
		}

		public BotTracks()
			: base(
				  Program.Config.TracksLeftA1Pin, 
				  Program.Config.TracksLeftA2Pin, 
				  Program.Config.TracksLeftEnaPin, 
				  Program.Config.TracksRightA1Pin, 
				  Program.Config.TracksRightA2Pin, 
				  Program.Config.TracksRightEnaPin)
		{
		}

		/// <summary>
		/// Spin in place at speed 0 - 100
		/// </summary>
		/// <param name="speed"></param>
		public void Spin(SpinDirection direction, int speed)
		{
			if(direction == SpinDirection.Clockwise)
			{
				LeftSpeed = -speed;
				RightSpeed = speed;
			}
			else
			{
				LeftSpeed = speed;
				RightSpeed = -speed;
			}
		}

		public static void Calibrate()
		{

		}
	}
}
