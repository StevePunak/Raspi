using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using RaspiCommon;
using KanoopCommon.Threading;
using System.Reflection;

namespace RaspiForm
{
	public partial class MainForm : Form
	{
		const int SERVO_SLIDER_DIVISOR = 50;

		L298N_DC_TankTracks _tracks;

		Servo _servo;
		HCSR04_RangeFinder _rangeFinder;
		ESC _esc;

		bool _initialized;

		public MainForm()
		{
			_initialized = false;

			InitializeComponent();
			_servo = null;
		}

		private void OnFormLoad(object sender, EventArgs args)
		{
			try
			{
				SetupGPIOList(Controls);

				System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
				t.Interval = 500;
				t.Tick += OnTimerTick;
				t.Enabled = true;
				t.Start();

				_initialized = true;

				MaybeRunTracks();
				MaybeStartRangeFinder();
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(ThreadBase.GetFormattedStackTrace(e));
			}
		}

		private void SetupGPIOList(Control.ControlCollection controls)
		{
			foreach(Control control in controls)
			{
				if(control.Name.Contains("listGpio"))
				{
					Console.WriteLine(control.Name);
					ComboBox box = control as ComboBox;
					foreach(Object value in Enum.GetValues(typeof(GpioPin)))
					{
						box.Items.Add(value);
					}

					GpioPin pin;
					if(Program.Config.PinSelectionControls.TryGetValue(box.Name, out pin) == true)
					{
						box.SelectedItem = pin;
					}
				}
				if(control.Controls.Count > 0)
				{
					SetupGPIOList(control.Controls);
				}
			}
		}

		private void SaveGpios(Control.ControlCollection controls = null)
		{
			if(controls == null)
			{
				controls = this.Controls;
			}
			foreach(Control control in controls)
			{
				if(control.Name.Contains("listGpio"))
				{
					ComboBox box = control as ComboBox;

					if(Program.Config.PinSelectionControls.ContainsKey(box.Name) == false)
					{
						Program.Config.PinSelectionControls.Add(box.Name, GpioPin.Unset);
					}
					if(box.SelectedItem != null)
					{
						Program.Config.PinSelectionControls[box.Name] = (GpioPin)box.SelectedItem;

						// save to config property
						String propertyName = String.Format("{0}Pin", box.Name.Substring(8));
						PropertyInfo property = typeof(RaspiConfig).GetProperty(propertyName);
						if(property != null)
						{
							property.SetValue(Program.Config, (GpioPin)box.SelectedItem);
						}
					}
				}
				if(control.Controls.Count > 0)
				{
					SaveGpios(control.Controls);
				}
			}
			Program.Config.Save();
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			if(_rangeFinder != null)
			{
				textRange.Text = String.Format("{0:0.000}m", _rangeFinder.Range);
			}
		}

		private void OnStartClicked(object sender, EventArgs e)
		{
			MotorDriver.Start();
			MotorDriver.Running = true;
		}

		private void OnStopClicked(object sender, EventArgs e)
		{
			MotorDriver.Running = false;
			MotorDriver.Stop();
		}

		private void OnRadioClicked(object sender, EventArgs e)
		{
			MotorDriver.Direction = radioStepperForward.Checked ? Direction.Forward : Direction.Backward;
		}

		private void OnMotorSliderScroll(object sender, EventArgs e)
		{
			MotorDriver.SleepInterval = TimeSpan.FromMilliseconds(sliderMotor.Value);
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			GpioSharp.DeInit();
		}

		private void OnServoSliderScroll(object sender, EventArgs e)
		{
			if(_servo == null)
			{
				MaybeStartServo();
			}
			if(_servo != null)
			{
				textServoSpeed.Text = String.Format("{0} Hz", sliderServo.Value * SERVO_SLIDER_DIVISOR);
				_servo.Value = (UInt32)sliderServo.Value * SERVO_SLIDER_DIVISOR;
			}
		}

		private void OnGpioStepperA1_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();
		}

		private void OnGpioStepperA2_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();

		}

		private void OnGpioStepperB1_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();

		}

		private void OnGpioStepperB2_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();

		}

		void MaybeStartServo()
		{
			if(!_initialized)
				return;

			GpioPin inputPin = GetGpioPin(listGpioServo);
			if(inputPin != GpioPin.Unset)
			{
				_servo = new Servo(inputPin);
				sliderServo.Minimum = (int)_servo.Minimum / SERVO_SLIDER_DIVISOR;
				sliderServo.Maximum = (int)_servo.Maximum / SERVO_SLIDER_DIVISOR;
				sliderServo.Value = (int)_servo.Mid / SERVO_SLIDER_DIVISOR;
			}
		}

		private void OnGpioServo_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();

			MaybeStartServo();
		}

		void MaybeStartRangeFinder()
		{
			if(!_initialized)
				return;

			if(_rangeFinder != null)
			{
				_rangeFinder.Stop();
			}
			GpioPin inputPin = GetGpioPin(listGpioRangeFinderInput);
			GpioPin outputPin = GetGpioPin(listGpioRangeFinderOuput);
			if(inputPin != GpioPin.Unset)
			{
				_rangeFinder = new HCSR04_RangeFinder(inputPin, outputPin);
				_rangeFinder.Start();
			}
		}

		private void OnGpioRangeFinderInput_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();

			MaybeStartRangeFinder();
		}

		private void OnGpioRangeFinderOutput_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();

			MaybeStartRangeFinder();
		}

		GpioPin GetGpioPin(ComboBox box)
		{
			return box.SelectedItem != null ? (GpioPin)box.SelectedItem : GpioPin.Unset;
		}

		void MaybeRunTracks()
		{
			if(!_initialized)
				return;

			GpioPin la1 = listGpioTracksLeftA1.SelectedItem == null ? GpioPin.Unset : (GpioPin)listGpioTracksLeftA1.SelectedItem;
			GpioPin la2 = listGpioTracksLeftA2.SelectedItem == null ? GpioPin.Unset : (GpioPin)listGpioTracksLeftA2.SelectedItem;
			GpioPin ra1 = listGpioTracksRightA1.SelectedItem == null ? GpioPin.Unset : (GpioPin)listGpioTracksRightA1.SelectedItem;
			GpioPin ra2 = listGpioTracksRightA2.SelectedItem == null ? GpioPin.Unset : (GpioPin)listGpioTracksRightA2.SelectedItem;
			GpioPin enal = listGpioTracksLeftEna.SelectedItem == null ? GpioPin.Unset : (GpioPin)listGpioTracksLeftEna.SelectedItem;
			GpioPin enar = listGpioTracksRightEna.SelectedItem == null ? GpioPin.Unset : (GpioPin)listGpioTracksRightEna.SelectedItem;
			if( la1 != GpioPin.Unset &&
				la2 != GpioPin.Unset &&
				ra1 != GpioPin.Unset &&
				ra2 != GpioPin.Unset &&
				enal != GpioPin.Unset &&
				enar != GpioPin.Unset)
			{
				Console.WriteLine("Starting tracks");
				_tracks = new L298N_DC_TankTracks(la1, la2, enal, ra1, ra2, enar);
				_tracks.LeftSpeed = trackTrackLeftSpeed.Value;
				_tracks.RightSpeed = trackTrackRightSpeed.Value;
			}

		}

		private void OnGpioTracksLeftA1_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();
			MaybeRunTracks();
		}

		private void OnGpioTracksLeftA2_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();
			MaybeRunTracks();
		}

		private void OnGpioTracksLeftEna_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();
			MaybeRunTracks();
		}

		private void OnGpioTracksRightA1_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();
			MaybeRunTracks();
		}

		private void OnGpioTracksRightA2_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();
			MaybeRunTracks();
		}

		private void OnGpioTracksRightEna_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();
			MaybeRunTracks();
		}

		private void OnGpioEscControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveGpios();
			MaybeStartEsc();
		}

		private void OnLeftTrackSpeedSliderChanged(object sender, EventArgs e)
		{
			if(_tracks != null)
			{
				_tracks.LeftSpeed = trackTrackLeftSpeed.Value;
			}
		}

		private void OnRightTrackSpeedSliderChanged(object sender, EventArgs e)
		{
			if(_tracks != null)
			{
				_tracks.RightSpeed = trackTrackRightSpeed.Value;
			}
		}

		void MaybeStartEsc()
		{
			if(!_initialized)
				return;

			GpioPin pin = GetGpioPin(listGpioEscControlPin);
			if(_esc == null || _esc.Pin != pin)
			{
				if(pin != GpioPin.Unset)
				{
					_esc = new ESC(pin);
				}
			}
			else
			{
				Console.WriteLine("Leaving ESC along");
			}
		}

		private void OnCalibrateEscClicked(object sender, EventArgs e)
		{
			if(_esc == null)
			{
				MaybeStartEsc();
			}

			if(_esc != null && _esc.Calibrated == false)
			{
				_esc.Calibrate();
			}
		}

		private void OnEscSpeedSliderChanged(object sender, EventArgs e)
		{
			MaybeStartEsc();
			if(_esc != null)
			{
				_esc.Value = (UInt32)trackEscSpeed.Value;
			}
		}
	}
}
