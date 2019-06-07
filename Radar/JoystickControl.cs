using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.CvEnum;
using RaspiCommon.Extensions;
using KanoopCommon.Geometry;

namespace Radar
{
	public partial class JoystickControl : PictureBox
	{
		public delegate void SpeedChangedHandler(int left, int right);

		public event SpeedChangedHandler SpeedChanged;

		Size ControlSize;
		Point ControlLocation;
		Double ControlRadius;

		bool _joystickDragged;

		Point Center;
		Rectangle _square;
		Circle _circle;

		bool Spinning { get; set; }

		public int RightSpeed { get; private set; }
		public int LeftSpeed { get; private set; }

		public JoystickControl()
		{
			InitializeComponent();

			Dock = DockStyle.Fill;

			SpeedChanged += delegate { };

			Center = new Point(Size.Width / 2, Size.Height / 2);
			ControlSize = new Size(30, 30);
			ControlRadius = ControlSize.Width / 2;

			CalculateBounds();

			MoveJoystick(Center);

			RightSpeed = LeftSpeed = 0;
		}

		private void OnJoystickMouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left && _circle.Contains(e.Location))
			{
				_joystickDragged = true;
				ControlLocation = e.Location;
				Spinning = ModifierKeys.HasFlag(Keys.Control);
			}
			else
			{
				_joystickDragged = false;
				MoveJoystick(Center);
			}
		}

		private void OnJoystickMouseMove(object sender, MouseEventArgs e)
		{
			if(_joystickDragged)
			{
				if(_circle.Contains(e.Location))
				{
					MoveJoystick(e.Location);
				}
				else
				{
					PointD location = _circle.ClosestPointOnEdge(e.Location);
					MoveJoystick(location.ToPoint());
				}

				Invalidate();
			}
		}

		void MoveJoystick(Point where)
		{
			ControlLocation = where;
			CalculateSpeed();
		}

		private void OnJoystickMouseUp(object sender, MouseEventArgs e)
		{
			_joystickDragged = false;
			MoveJoystick(Center);
			Invalidate();
		}

		private void OnResize(object sender, EventArgs e)
		{
			Center = new Point(Size.Width / 2, Size.Height / 2);
			MoveJoystick(Center);
		}

		private void CalculateSpeed()
		{
			if(!Spinning)
			{
				Double forward = Center.Y - ControlLocation.Y;
				forward = (forward / _circle.Radius) * 100;

				Double lateral = Center.X - ControlLocation.X;
				lateral = (lateral / _circle.Radius);
				lateral /= 2;
				double rightSpeed = forward + (forward * lateral);
				double leftSpeed = forward - (forward * lateral);
				RightSpeed = (int)rightSpeed;
				LeftSpeed = (int)leftSpeed;
			}
			else
			{
				Double lateral = Center.X - ControlLocation.X;
				lateral = (lateral / _circle.Radius);
				LeftSpeed = (int)-(lateral * 100);
				RightSpeed = (int)(lateral * 100);
			}
			SpeedChanged(LeftSpeed, RightSpeed);
		}

		void CalculateBounds()
		{
			int min = Math.Min(Height, Width);
			Center = new Point(Width / 2, Height / 2);
			_square = new Rectangle(new Point(Center.X - min / 2, Center.Y - min / 2), new Size(min, min));
			_circle = new Circle(Center, min / 2);
		}

		private void OnPaint(object sender, PaintEventArgs e)
		{
			Mat image = new Mat(Size, DepthType.Cv8U, 3);

			CalculateBounds();
			image.DrawCircle(_circle, Color.Green);

			Circle control = new Circle(ControlLocation, ControlRadius);
			image.DrawCircle(control, Color.Blue, 0);

			PointD textLocation = new PointD(_square.X + 1, Center.Y);
			image.DrawText(String.Format("L {0} R {1}", LeftSpeed, RightSpeed), FontFace.HersheyPlain, Color.Red, textLocation, 1);

			e.Graphics.DrawImage(image.Bitmap.Clone() as Image, Point.Empty);
		}
	}
}
