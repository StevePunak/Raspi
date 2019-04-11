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
using System.IO.Ports;
using KanoopCommon.Extensions;
using RaspiCommon.Lidar;
using KanoopCommon.CommonObjects;
using KanoopCommon.Logging;
using KanoopCommon.Geometry;
using RaspiCommon.Devices.MotorControl;
using RaspiCommon.Devices.Servos;
using RaspiCommon.Devices.Spatial;
using RaspiCommon.Devices.Locomotion;

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

		PWMMotorDriver _motorDriver;

		RPLidar _lidar;

		int _lidarDataCount;

		System.Windows.Forms.Timer _lidarTimer;
		Bitmap _lidarBitmap;
		int _cx, _cy, _u;
		int _tx, _ty;

		const int WIDTH = 300, HEIGHT = 300, HAND = 150;

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

				OnRefreshComportsClicked(null, null);
				OnStopLidarClicked(null, null);

				SetupLidarDisplay();
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(ThreadBase.GetFormattedStackTrace(e));
			}
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			GpioSharp.DeInit();

			if(_lidar != null)
			{
				_lidar.Stop();
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
			textLidarStatus.Text = _lidarDataCount.ToString();
		}

		private void OnStartClicked(object sender, EventArgs e)
		{
			if(_motorDriver == null)
			{
				_motorDriver = new PWMMotorDriver(Program.Config.StepperA1Pin, Program.Config.StepperA2Pin, Program.Config.StepperB1Pin, Program.Config.StepperB2Pin);
				_motorDriver.Start();
			}
		}

		private void OnStopClicked(object sender, EventArgs e)
		{
			if(_motorDriver != null)
			{
				_motorDriver.Stop();
			}
			_motorDriver = null;
		}

		private void OnRadioClicked(object sender, EventArgs e)
		{
			if(_motorDriver != null)
			{
				_motorDriver.Direction = radioStepperForward.Checked ? Direction.Forward : Direction.Backward;
			}
		}

		private void OnMotorSliderScroll(object sender, EventArgs e)
		{
//			_motorDriver.SleepInterval = TimeSpan.FromMilliseconds(sliderMotor.Value);
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
				_rangeFinder = new HCSR04_RangeFinder(inputPin, outputPin, RFDir.Front);
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

		private void OnStartLidarClicked(object sender, EventArgs args)
		{
			try
			{
				comboComPorts.Enabled = false;

				_lidar = new RPLidar(Program.Config.LidarComPort, .25);
				_lidarDataCount = 0;
				//				_lidar.LidarResponseData += OnLidarRata;
				_lidar.Sample += OnLidarSample;
				_lidar.Start();

				btnStartLidar.Enabled = false;
				btnStopLidar.Enabled = true;

				groupLidarCommands.Enabled = true;
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private void OnLidarSample(LidarSample sample)
		{
			// Log.SysLogText(LogLevel.DEBUG, "{0}", sample);
		}

		private void OnLidarRata(LidarResponse response)
		{
			_lidarDataCount++;
			Log.SysLogText(LogLevel.DEBUG, "WIN: Response {0}", response);
		}

		private void OnStopLidarClicked(object sender, EventArgs e)
		{
			if(_lidar != null)
			{
				_lidar.Stop();
				_lidar = null;
			}
			comboComPorts.Enabled = true;
			btnStartLidar.Enabled = true;
			btnStopLidar.Enabled = false;
			groupLidarCommands.Enabled = false;
		}

		private void OnRefreshComportsClicked(object sender, EventArgs e)
		{
			List<String> ports = new List<string>(SerialPort.GetPortNames());
			ports.TerminateAll();

			comboComPorts.Items.Clear();
			comboComPorts.Items.AddRange(ports.ToArray());

			if(String.IsNullOrEmpty(Program.Config.LidarComPort) == false && ports.Contains(Program.Config.LidarComPort))
			{
				comboComPorts.SelectedItem = Program.Config.LidarComPort;
			}
		}

		private void OnSelectedLidarComportChanged(object sender, EventArgs e)
		{
			Program.Config.LidarComPort = comboComPorts.SelectedItem.ToString();
			Program.Config.Save();
		}

		private void OnResetLidarClicked(object sender, EventArgs e)
		{
			_lidar.Reset();
		}

		private void OnStopScanClicked(object sender, EventArgs args)
		{
			try
			{
				if(!_lidar.StopScan())
				{
					throw new CommonException("Stop scan failed");
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private void OnStartScanLicked(object sender, EventArgs args)
		{
			try
			{
				if(!_lidar.StartScan())
				{
					throw new CommonException("Start scan failed");
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
			}

		}

		private void OnTestLidarClicked(object sender, EventArgs args)
		{
			try
			{
				LidarCommand command = new GetDeviceInfoCommand();
				_lidar.SendCommand(command);

				LidarResponse response;
				if(_lidar.TryGetResponse(TimeSpan.FromSeconds(1), out response))
				{
					Program.Log.LogText(LogLevel.DEBUG, "Next Response successful");
				}

				_lidar.StopMotor();
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private void SetupLidarDisplay()
		{
			_lidarBitmap = new Bitmap(WIDTH, HEIGHT);
			picLidar.BackColor = Color.Black;
			picLidar.BackgroundImage = _lidarBitmap;
			_cx = WIDTH / 2;
			_cy = HEIGHT / 2;

			_lidarTimer = new System.Windows.Forms.Timer();
			_lidarTimer.Tick += OnLidarDrawTick;
			_lidarTimer.Interval = 5;

			_lidarTimer.Start();

			_tx = _ty = 20;
		}
		
		private void OnPanelPaint(object sender, PaintEventArgs e)
		{
			if(_lidar == null)
			{
				return;
			}

			DrawRadar(e.Graphics);

			return;

		}

		private void OnLidarDrawTick(object sender, EventArgs e)
		{
			if(_lidar == null)
			{
				return;
			}

			using(Graphics g = panelBitmap.CreateGraphics())
			{
				DrawRadar(g);
			}
				

//			panelBitmap.Invalidate();
		}

		int _angle = 0;
		Double _dist = 60;

		Direction _lag = Direction.Forward;

		static readonly PointD center = new PointD(150, 150);
		static readonly Brush _eraserBrush = new SolidBrush(Color.Black);
		static readonly Pen _eraserPen = new Pen(Color.Black, 2);
		static readonly Pen _greenPen = new Pen(Color.Green, 1);

		private void OnLidarBitmapPressed(object sender, EventArgs e)
		{
			if(_lidar != null)
			{
				Bitmap bm =_lidar.GenerateBitmap();
				bm.Save(@"c:\pub\tmp\tmp.png");
			}
		}

		static readonly Pen _redPen = new Pen(Color.Red);
		Line _eraseLine;

		const int LENGTH = 150;

		private void DrawRadar(Graphics g)
		{

			// get distance at this angle
			_dist = _lidar.GetRangeAtBearing(_angle) * 10;
//			Log.SysLogText(LogLevel.DEBUG, "DISTANCE: {0}° {1:0.00} [{2}]={3:0.00}", _angle, _dist, _u, _lidar.Vectors[_angle]);

			if(_eraseLine != null)
			{
				// erase last line
				g.DrawLine(_eraserPen, center.ToPoint(), _eraseLine.P2.ToPoint());
				if(_dist > 0)
				{
					PointD p = FlatGeo.GetPoint(center, _eraseLine.Bearing, _dist);
					g.DrawRectangle(_redPen, new Rectangle(p.ToPoint(), new Size(1, 1)));
				}

			}

			if(++_angle == 360)
				_angle = 0;

			Line radarLine = new Line(center, FlatGeo.GetPoint(center, _angle, LENGTH));
			g.DrawLine(_greenPen, center.ToPoint(), radarLine.P2.ToPoint());

			_eraseLine = radarLine;

#if zero
			if(_lag == Direction.Forward)
			{
				if(++_dist == 85)
					_lag = Direction.Backward;
			}
			else
			{
				if(--_dist == 35)
					_lag = Direction.Forward;
			}
#endif
		}

		private void OnLidarDrawTick2(object sender, EventArgs e)
		{
			const int lim = 20;

			if(_lidar == null)
			{
				return;
			}

			//pen
			Double distance = _lidar.GetRangeAtBearing(_u);

			using(Pen p = new Pen(Color.Green, 1f))
			using(Pen rp = new Pen(Color.Red, 1f))
			using(Graphics g = Graphics.FromImage(_lidarBitmap))
			{

				//calculate x, y coordinate of HAND
				int tu = (_u - lim) % 360;
				int x, y, rx, ry;
				rx = ry = 0;

				distance = distance > 0 && distance < 15 ? distance * 10 : 0;

				if(_u >= 0 && _u <= 180)
				{
					//right half
					//u in degree is converted into radian.

					x = _cx + (int)(HAND * Math.Sin(Math.PI * _u / 180));
					y = _cy - (int)(HAND * Math.Cos(Math.PI * _u / 180));

					if(distance != 0)
					{
						rx = _cx + (int)(distance * Math.Sin(Math.PI * _u / 180));
						ry = _cy - (int)(distance * Math.Cos(Math.PI * _u / 180));
					}
				}
				else
				{
					x = _cx - (int)(HAND * -Math.Sin(Math.PI * _u / 180));
					y = _cy - (int)(HAND * Math.Cos(Math.PI * _u / 180));
					if(distance != 0)
					{
						rx = _cx - (int)(distance * -Math.Sin(Math.PI * _u / 180));
						ry = _cy - (int)(distance * Math.Cos(Math.PI * _u / 180));
					}
				}

				if(tu >= 0 && tu <= 180)
				{
					//right half
					//tu in degree is converted into radian.

					_tx = _cx + (int)(HAND * Math.Sin(Math.PI * tu / 180));
					_ty = _cy - (int)(HAND * Math.Cos(Math.PI * tu / 180));
				}
				else
				{
					_tx = _cx - (int)(HAND * -Math.Sin(Math.PI * tu / 180));
					_ty = _cy - (int)(HAND * Math.Cos(Math.PI * tu / 180));
				}

				//draw circle
				g.DrawEllipse(p, 0, 0, WIDTH, HEIGHT);  //bigger circle
				g.DrawEllipse(p, 80, 80, WIDTH - 160, HEIGHT - 160);    //smaller circle

				//draw perpendicular line
				g.DrawLine(p, new Point(_cx, 0), new Point(_cx, HEIGHT)); // UP-DOWN
				g.DrawLine(p, new Point(0, _cy), new Point(WIDTH, _cy)); //LEFT-RIGHT

				//draw HAND
				g.DrawLine(new Pen(Color.Black, 1f), new Point(_cx, _cy), new Point(_tx, _ty));
				g.DrawLine(p, new Point(_cx, _cy), new Point(x, y));
				if(distance > 0)
				{
					g.DrawLine(rp, new Point(_cx, _cy), new Point(rx, ry));
				}

				//load bitmap in picturebox1
				picLidar.Image = _lidarBitmap;
			}

			//update
			_u++;
			if(_u >= 360)
			{
				_u = 0;
			}
		}

	}
}
