using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using MQTT;

namespace BotClaw
{
	public partial class BotClawForm : Form
	{
		public const String ArmRotationTopic = "trackbot/control/robotarm/rotation";
		public const String ArmElevationTopic = "trackbot/control/robotarm/elevation";
		public const String ArmThrustTopic = "trackbot/control/robotarm/thrust";
		public const String ArmClawTopic = "trackbot/control/robotarm/claw";

		MqttClient _client;
		MJPEGStream _videoStream;
		public BotClawForm()
		{
			InitializeComponent();
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			// create MJPEG video source
			_videoStream = new MJPEGStream("http://raspi:8081/");

			// set event handlers
			_videoStream.NewFrame += OnVideoFrame; 
			// start the video source
			_videoStream.Start();

			_client = new MqttClient("192.168.0.50", "MyBotClaw");
			_client.Connect();

		}

		private void OnVideoFrame(object sender, NewFrameEventArgs eventArgs)
		{
			picVideo.Image = eventArgs.Frame.Clone() as Bitmap;
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			_client.Disconnect();
			_videoStream.Stop();
		}

		private void OnRotationScroll(object sender, EventArgs e)
		{
			int value = trackRotation.Value;
			byte[] bytes = BitConverter.GetBytes(value);
			_client.Publish(ArmRotationTopic, bytes);
		}

		private void OnThrustScroll(object sender, EventArgs e)
		{
			int value = trackThrust.Value;
			byte[] bytes = BitConverter.GetBytes(value);
			_client.Publish(ArmThrustTopic, bytes);
		}

		private void OnClawScroll(object sender, EventArgs e)
		{
			int value = trackClaw.Value;
			byte[] bytes = BitConverter.GetBytes(value);
			_client.Publish(ArmClawTopic, bytes);
		}

		private void OnElevationScroll(object sender, EventArgs e)
		{
			int value = trackElevation.Value;
			byte[] bytes = BitConverter.GetBytes(value);
			_client.Publish(ArmElevationTopic, bytes);
		}
	}
}
