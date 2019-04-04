using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KanoopCommon.Addresses;
using KanoopCommon.CommonObjects;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.TCP.Clients;
using KanoopCommon.Threading;
using Radar.Properties;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Server;

namespace Radar
{
	public partial class RadarForm : Form
	{
		const Double METERS_SQUARE = 10;
		const Double PIXELS_PER_METER = 50;

		TelemetryClient _client;
		String Host { get; set; }

		Timer _drawTimer;

		Bitmap _bitmap;

		Double _lastAngle;

		Image _tank;

		public RadarForm()
		{
			_client = null;
			Host = String.IsNullOrEmpty(Program.Config.RadarHost) ? "raspi" : Program.Config.RadarHost;
			InitializeComponent();
		}

		private void OnFormLoad(object sender, EventArgs args)
		{
			try
			{
				SetupBitmap();

				GetConnected();
				Log.SysLogText(LogLevel.DEBUG, "Got connectd");

				_drawTimer = new Timer();
				_drawTimer.Interval = 500;
				_drawTimer.Enabled = true;
				_drawTimer.Tick += OnDrawTimer;
				_drawTimer.Start();

				_tank = Resources.tank.Resize(6, 10);
				Log.SysLogText(LogLevel.DEBUG, "Started draw timer");
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
				Close();
			}
		}

		void SetupBitmap()
		{
			int size = (int)(METERS_SQUARE * PIXELS_PER_METER);
			_bitmap = new Bitmap(size, size);
			using(Graphics g = Graphics.FromImage(_bitmap))
			{
				g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, size, size));
				Point center = _bitmap.Center().ToPoint();
				for(int radius = 1;radius < METERS_SQUARE / 2;radius++)
				{
					g.DrawEllipse(new Pen(Color.Green), _bitmap.RectangleForCircle(center, radius * (int)PIXELS_PER_METER));
				}

				g.DrawLine(new Pen(Color.Green), new Point(center.X - 5, center.Y), new Point(center.X + 5, center.Y));
				g.DrawLine(new Pen(Color.Green), new Point(center.X, center.Y - 5), new Point(center.X, center.Y + 5));
				picBox.BackgroundImage = _bitmap;
			}
		}

		private void OnDrawTimer(object sender, EventArgs e)
		{
			if(_client == null || _client.Connected == false)
				return;

			Image bitmap = new Bitmap(_bitmap);
			using(Graphics g = Graphics.FromImage(bitmap))
			{
				Pen redPen = new Pen(Color.Red);
				Pen barrierPen = new Pen(Color.Firebrick);
				Pen landmarkPen = new Pen(Color.Blue);
				PointD center = bitmap.Center();

				// draw all the dots
				for(Double angle = _lastAngle.AddDegrees(TelemetryClient.VectorSize);angle != _lastAngle;angle = angle.AddDegrees(TelemetryClient.VectorSize))
				{
					int offset = (int)(angle / TelemetryClient.VectorSize);
					LidarVector vector = _client.Vectors[offset];
					if(vector != null && vector.Range != 0)
					{
						PointD where = center.GetPointAt(vector.Bearing, vector.Range * PIXELS_PER_METER) as PointD;
						Rectangle rect = new Rectangle(where.ToPoint(), new Size(1, 1));
						g.DrawRectangle(redPen, rect);
						//Log.SysLogText(LogLevel.DEBUG, "Drawing {0} at {1:0.00}°  {2}", vector, angle, where);
					}
				}

				// draw fuzzy path
				if(_client.FuzzyPath != null)
				{
					FuzzyPath path = _client.FuzzyPath.Clone();
					Pen brownPen = new Pen(Color.RosyBrown);
					if(path.Elements.Count > 1)
					{
						PointD p1 = center.GetPointAt(path.Elements.First().Bearing, path.Elements.First().Range * PIXELS_PER_METER);
						g.DrawLine(brownPen, center.ToPoint(), p1.ToPoint());
						PointD p2 = center.GetPointAt(path.Elements.Last().Bearing, path.Elements.Last().Range * PIXELS_PER_METER);
						g.DrawLine(brownPen, center.ToPoint(), p2.ToPoint());
					}
				}

				if(_client.Barriers != null)
				{
					BarrierList barriers = _client.Barriers.Clone();
					Pen orangePen = new Pen(Color.Orange);
					foreach(Barrier barrier in barriers)
					{
						Line line = barrier.GetLine();
						g.DrawLine(orangePen, line.P1.ToPoint(), line.P2.ToPoint());
					}
				}

				if(_client.Landmarks != null)
				{
					LandmarkList landmarks = _client.Landmarks.Clone();
					Pen bluePen = new Pen(Color.Blue, 2);
					foreach(Landmark landmark in landmarks)
					{
						RectangleD rectangle = RectangleD.InflatedFromPoint(landmark.GetPoint(), 6);
						g.DrawEllipse(bluePen, rectangle.ToRectangle());
					}
				}

				Image tank = new Bitmap(Resources.tank).Rotate(_client.Bearing);
//				tank.Save(@"c:\tmp\output.png");

				PointD tankPoint = PointD.UpperLeftFromCenter(center, tank.Size);
				g.DrawImage(tank, tankPoint.ToPoint());
			}

			//_bitmap.Save(@"c:\tmp\output.png");
			picBox.BackgroundImage = bitmap;


		}

		void GetConnected()
		{
			ChooseBotForm dlg = new ChooseBotForm()
			{
				Host = Program.Config.RadarHost
			};
			if(dlg.ShowDialog() != DialogResult.OK)
			{
				throw new CommonException("No host selected");
			}
			Program.Config.RadarHost = dlg.Host;
			Program.Config.Save();

			_client = new TelemetryClient(Program.Config.RadarHost, "radarform",
				new List<string>()
				{
					TelemetryServer.CurrentPathTopic,
					TelemetryServer.RangeBlobTopic,
					TelemetryServer.BarriersTopic,
					TelemetryServer.LandmarksTopic,
					TelemetryServer.BearingTopic
				});
			_client.Start();

			_lastAngle = 0;
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			if(_client != null)
			{
				_client.Disconnect();
				_client = null;
			}

			ThreadBase.AbortAllRunningThreads();
		}
	}
}
