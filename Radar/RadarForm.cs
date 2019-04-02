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
using RaspiCommon.Lidar;
using RaspiCommon.Server;

namespace Radar
{
	public partial class RadarForm : Form
	{
		const Double METERS_SQUARE = 10;
		const Double PIXELS_PER_METER = 50;

		LidarClient _client;
		String Host { get; set; }

		Timer _drawTimer;

		Bitmap _bitmap;

		Double _lastAngle;

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

			_client = new LidarClient(Program.Config.RadarHost, "radarform",
				new List<string>()
				{
					LidarServer.CurrentPathTopic,
					LidarServer.RangeBlobTopic
				});
			_client.Start();

			_lastAngle = 0;
		}

		private void OnDrawTimer(object sender, EventArgs e)
		{
			if(_client == null || _client.Connected == false)
				return;

			Image bitmap = new Bitmap(_bitmap);
			using(Graphics g = Graphics.FromImage(bitmap))
			{
				Pen redPen = new Pen(Color.Red);
				PointD center = bitmap.Center();
				for(Double angle = _lastAngle.AddDegrees(LidarClient.VectorSize);angle != _lastAngle;angle = angle.AddDegrees(LidarClient.VectorSize))
				{
					int offset = (int)(angle / LidarClient.VectorSize);
					LidarVector vector = _client.Vectors[offset];
					if(vector != null && vector.Range != 0)
					{
						PointD where = center.GetPointAt(vector.Bearing, vector.Range * PIXELS_PER_METER) as PointD;
						Rectangle rect = new Rectangle(where.ToPoint(), new Size(1, 1));
						g.DrawRectangle(redPen, rect);
						//Log.SysLogText(LogLevel.DEBUG, "Drawing {0} at {1:0.00}°  {2}", vector, angle, where);
					}
				}
			}

			//_bitmap.Save(@"c:\tmp\output.png");
			picBox.BackgroundImage = bitmap;


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
