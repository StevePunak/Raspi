using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;

namespace TrackBot.Spatial
{
	class RoamAndSeek : Activity
	{
		const Double MAX_RANGE_FROM_DEST = .15;

		public enum RSState
		{
			Init,
			Idle,
			SpinSeek,
			TravelToDest,
			Stuck
		}

		public RSState ActivityState { get; private set; }

		public PointD Destination { get; private set; }

		private DateTime _enterStateTime;

		// spin-seek
		private Double _startBearing;
		private bool _leftStartBearing;
		private PointD _startLocation;

		public RoamAndSeek()
			: base(ActivityType.RoamAndSeek)
		{
			Interval = TimeSpan.FromMilliseconds(100);
			ActivityState = RSState.Init;
		}

		protected override bool OnRun()
		{
			switch(ActivityState)
			{
				case RSState.Init:
					RunInitState();
					break;
				case RSState.Idle:
					RunIdleState();
					break;
				case RSState.SpinSeek:
					RunSpinSeekState();
					break;
				case RSState.TravelToDest:
					RunTravelToDestState();
					break;
				case RSState.Stuck:
					RunStuckState();
					break;
				default:
					break;
			}
			return true;
		}

		protected override bool OnStop()
		{
			Widgets.Tracks.Stop();
			return base.OnStop();
		}

		void SwitchState(RSState state)
		{
			MethodInfo method;
			bool result;

			Console.WriteLine("{0} Switching state from {1} to {2}", Name, ActivityState, state);

			try
			{
				String methodName;

				// deinit last state
				method = GetStateMethod(StateTransition.Stop, ActivityState, out methodName);
				if(method != null)
				{
					if((result = (bool)method.Invoke(this, null)) == false)
					{
						throw new TrackBotException("Method {0} return false", method);
					}
				}
				else
				{
				}

				ActivityState = state;
				_enterStateTime = DateTime.UtcNow;

				// init new state
				method = GetStateMethod(StateTransition.Init, ActivityState, out methodName);
				if(method != null)
				{
					if((result = (bool)method.Invoke(this, null)) == false)
					{
						throw new TrackBotException("Method {0} return false", method);
					}
				}
				else
				{
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("EXCEPTION {0}: {1}", GetType().Name, e.Message);
				ActivityState = RSState.Idle;
			}
		}

		#region Init State

		void RunInitState()
		{
			Widgets.Tracks.Stop();
			SwitchState(RSState.SpinSeek);
		}

		#endregion

		#region Idle State

		bool InitIdleState()
		{
			Widgets.Tracks.Stop();
			Interval = TimeSpan.FromMilliseconds(500);
			return true;
		}

		bool RunIdleState()
		{
			return true;
		}

		#endregion

		#region SpinSeek State

		bool InitSpinSeekState()
		{
			Interval = TimeSpan.FromMilliseconds(50);

			_startBearing = Widgets.GyMag.Bearing;
			_leftStartBearing = false;
			_startLocation = Widgets.Environment.Location;

			Widgets.Tracks.Spin(SpinDirection.Clockwise, Widgets.Tracks.StandardSpeed);

			Console.WriteLine("Started SpinSeek at {0}  bearing {1:0.00}°", _startLocation, _startBearing);
			return true;
		}

		bool RunSpinSeekState()
		{
			PointD location = Widgets.Environment.Location;
			Double bearing = Widgets.GyMag.Bearing;
			bool markObstacle = true;
			Double range = Widgets.RangeFinder.Range;
			if(range > Program.Config.MaxRangeDetect)
			{
				markObstacle = false;
				Log.LogText(LogLevel.DEBUG, "Range {0:0.00} is greater than max", range);
				range = Program.Config.MaxRangeDetect;
			}

			PointD obstacleLocation = PointD.RelativeTo(location, bearing, range);
			Console.WriteLine("I am at {0}. Obstacle is {1:0.00} away at {2} bearing {3:0.00}°", Widgets.Environment.Location, range, obstacleLocation, bearing);
			Widgets.Environment.ClearPathToObstacle(obstacleLocation, markObstacle);

			Widgets.Tracks.Spin(SpinDirection.Clockwise, Widgets.Tracks.StandardSpeed);
			GpioSharp.Sleep(100);
			Widgets.Tracks.Stop();
			GpioSharp.Sleep(100);

			bearing = Widgets.GyMag.Bearing;
			Double diff = Degrees.AngularDifference(_startBearing, bearing);
//			Console.WriteLine("Now bearing {0:0.0}°  angular diff: {1:0.0}°", bearing, diff);
			if(_leftStartBearing == false && diff < 10)
			{
				_leftStartBearing = true;
			}
			else if(_leftStartBearing)
			{
				if(diff < 5)
				{
					Console.WriteLine("Spin-seek complete... going to roam");

					SwitchState(RSState.TravelToDest);
				}
			}

			if(DateTime.UtcNow > _enterStateTime + TimeSpan.FromSeconds(20))
			{
				Console.WriteLine("Giving up on spin-seek");
				SwitchState(RSState.Idle);
			}

			return true;
		}

		bool StopSpinSeekState()
		{
			Console.WriteLine("Exiting SpinSeek");
			Interval = TimeSpan.FromMilliseconds(50);

			return true;
		}

		#endregion

		#region TravelToDest State

		bool InitTravelToDestState()
		{
			// find a place to travel to
			Destination = Widgets.Environment.FindGoodDestination();
			if(Destination == null)
			{
				Console.WriteLine("Could not find a good destination");
				return false;
			}
			Double bearing = Widgets.Environment.Location.BearingTo(Destination);
			Widgets.Tracks.TurnToBearing(bearing);

			Console.WriteLine("Found a good destination at {0} bearing {1:0.0}°", Destination, bearing);


			/*** DEBUG ONLY */
			Cell dest = Widgets.Environment.Grid.GetCellAtLocation(Destination);
			dest.Contents = CellContents.Special;

			Widgets.Environment.SaveBitmap();

			Widgets.Tracks.SetStart();
			Widgets.Tracks.Speed = Widgets.Tracks.Slow;

			return true;
		}

		bool RunTravelToDestState()
		{
			bool result = false;
			while(DateTime.UtcNow < _enterStateTime + TimeSpan.FromSeconds(10))
			{
				GpioSharp.Sleep(20);

				PointD location;
				if(Widgets.Tracks.TryCalculateCurrentLocation(out location))
				{
					Widgets.Environment.Location = location;

					if(location.DistanceTo(Destination) < .15)
					{
						Console.WriteLine("Arrived at location");
						result = true;
						break;
					}

					if(Widgets.RangeFinder.Range < .2)
					{
						Console.WriteLine("Hit obstacle");
						break;
					}
				}
			}

			if(!result)
			{
				Console.WriteLine("Abandoning attempt");
				SwitchState(RSState.Idle);
			}
			else
			{
				SwitchState(RSState.SpinSeek);
			}


			return result;
		}

#endregion

#region Stuck State

		bool RunStuckState()
		{
			return true;
		}

#endregion
	}
}
