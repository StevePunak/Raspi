using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
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
			TravelToDest,
			Stuck
		}

		public RSState ActivityState { get; private set; }

		public Line Destination { get; private set; }

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
			SwitchState(RSState.TravelToDest);
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
			Widgets.Tracks.TurnToBearing(Destination.Bearing);

			Console.WriteLine("Found a good destination at {0} bearing {1:0.0}°", Destination, Destination.Bearing);


			/*** DEBUG ONLY */
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
					Widgets.Environment.RelativeLocation = location;

					if(location.DistanceTo(Destination.P2 as PointD) < .15)
					{
						Console.WriteLine("Arrived at location");
						result = true;
						break;
					}

					if(Widgets.Environment.FuzzyRange() < .2)
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
				SwitchState(RSState.TravelToDest);
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
