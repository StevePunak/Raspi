using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AForge.Video;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using KanoopCommon.Database;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using KanoopControls.SimpleTextSelection;
using MQTT;
using Ookii.Dialogs;
using RaspiCommon;
using RaspiCommon.Data.DataSource;
using RaspiCommon.Data.Entities.Facial;
using RaspiCommon.Data.Entities.Track;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.GamePads;
using RaspiCommon.Devices.Locomotion;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Extensions;
using RaspiCommon.GraphicalHelp;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Network;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.DeadReckoning;
using RaspiCommon.Spatial.LidarImaging;
using TrackBotCommon.Environs;
using TrackBotCommon.InputDevices;
using TrackBotCommon.InputDevices.GamePads;

namespace Radar
{
	public partial class RadarForm : Form
	{
		const String COL_LABEL = "Label";
		const String COL_POSITION = "Position";
		const String COL_RANGE = "Range";
		const String COL_BEARING = "Bearing";

		const String TAB_FULL_IMAGE = "Full Image";
		const String TAB_BLUE = "Blue";
		const String TAB_GREEN = "Green";
		const String TAB_RED = "Red";

		const String DR_NAME = "ManCave";

		const Double METERS_SQUARE = 10;

		const Double SPIN_STEP_DEGREES = 3;

		Double PixelsPerMeterImage
		{
			get
			{
				Double value = _client != null && _client.ImageMetrics != null
					? _client.ImageMetrics.PixelsPerMeter : 50;
				return value;
			}
		}

		Double PixelsPerMeterLandscape
		{
			get { return 50; }
		}

		Double PixelsPerMeterDR
		{
			get { return 50; }
		}

		TelemetryClient _client;
		RaspiControlClient _mqqtController;
		String Host { get; set; }

		Chassis Chassis { get; set; }

		System.Windows.Forms.Timer _drawTimer;

		Bitmap _radarBitmap;

		Image _tank;

		int _tempImageCount;

		bool _layoutComplete;

		TrackDataSource _ds;
		public TrackBotLandscape Landscape { get; private set; }

		public LandscapeMetrics LandscapeMetrics { get; set; }
		public ImageMetrics ImageMetrics { get; set; }
		public EnvironmentInfo EnvironmentInfo { get; set; }

		public int AnalysisPixelsPerMeter { get { return 100; } }

		bool _redrawDR;
		public DeadReckoningEnvironment DREnvironment { get; set; }

		public String FullImage { get; set; }
		public String BlueImage { get; set; }
		public String GreenImage { get; set; }
		public String RedImage { get; set; }
		public CascadeClassifier FaceClassifier { get; private set; }

		MJPEGStream _videoStream;

		BotArmImage _robotArm;
		VideoCapture _capture;
		Mat _captureFrame;

		GamePadBase _gamepad;
		ClawControl _clawControl;
		PanTiltControl _panTilt;
		SpeedAndBearing _speedAndBearing;
		bool _grabbingImages;
		bool _selectingClassifyImage;
		Bitmap _classifierCleanImage;
		Mat _classifierImage;
		Point _classifySelectAnchor;
		Rectangle _classifySelectedRectangle;
		List<Rectangle> _classifyRectangles;
		FacialDataSource _fds;
		FaceNameList _faceNames;

		FaceRecognizer _faceRecognizer;
		bool _faceRecognizerReady;

		public RadarForm()
		{
			_capture = null;
			_captureFrame = null;
			_client = null;
			_faceRecognizerReady = false;

			Host = String.IsNullOrEmpty(Program.Config.MqttPublicHost) ? "raspi" : Program.Config.MqttPublicHost;
			InitializeComponent();

			Image < Bgr, Byte > imageCV = new Image<Bgr, byte>(Properties.Resources.tank);
			botDash.TankBitmap = imageCV.Mat;

		}

		private void OnFormLoad(object sender, EventArgs args)
		{
			try
			{
				SetupBitmap();
				GetConnected();
				Log.SysLogText(LogLevel.DEBUG, "Got connected");

				_drawTimer = new System.Windows.Forms.Timer();
				_drawTimer.Interval = 500;
				_drawTimer.Enabled = true;
				_drawTimer.Tick += OnDrawTimer;
				_drawTimer.Start();

				_tank = Radar.Properties.Resources.tank.Resize(6, 10);
				Log.SysLogText(LogLevel.DEBUG, "Started draw timer");

				if(Program.Config.LastRadarWindowSize.Height > 200)
				{
					Size = Program.Config.LastRadarWindowSize;
					splitTopToBottom.SplitterDistance = Program.Config.SplitTopToBottomPosition;
					splitButtons.SplitterDistance = Program.Config.SplitLeftToRightPosition;
					splitRadar.SplitterDistance = Program.Config.SplitRadarPosition;
					Location = Program.Config.LastRadarWindowLocation;
				}
//				LoadLandscape();

				flowBitmap.Height = 40;

				textCommand.Select();
				tabAnalysis.SelectedIndex = Program.Config.LastAnalyticsTabIndex;

				textAnalysisCoordinates.Text = String.Empty;

				SetupImageTabs();

				LoadOptions();

				_fds = DataSourceFactory.Create<FacialDataSource>(Program.Config.FacialDBCredentials);
				_fds.GetAllNames(out _faceNames);

				FaceClassifier = new CascadeClassifier(Program.Config.FaceCascadeFile);
				BatonFinder = new BatonFinder(Program.Config.BatonCascadeFile)
				{
					MinimumInterval = TimeSpan.FromSeconds(1),
				};

				new Thread(delegate ()
				{
					LoadEigen();
				});

				StartVideoFeed();

				trackBarRotation.Value = 50;
				trackBarElevation.Value = 50;
				trackBarClaw.Value = 50;
				trackBarThrust.Value = 50;
				_mqqtController.SendPercentage(MqttTypes.ArmRotationTopic, 50);
				_mqqtController.SendPercentage(MqttTypes.ArmElevationTopic, 50);
				_mqqtController.SendPercentage(MqttTypes.ArmThrustTopic, 50);
				_mqqtController.SendPercentage(MqttTypes.ArmClawTopic, 50);

				_gamepad = GamePadBase.Create(Program.Config.GamePadType);
				_gamepad.LeftIndex.Continuous = true;
				_gamepad.RightIndex.Continuous = true;
				_gamepad.GamepadEvent += OnGamepadEvent;
				_gamepad.HatChanged += OnGampadHatChanged;
				_gamepad.XButtonChanged += OnGamepadXButtonChanged;
				_gamepad.BButtonChanged += OnGamepadBButtonChanged;
				_gamepad.AButtonChanged += OnGampadAButtonChanged;
				_gamepad.YButtonChanged += OnGamepadYButtonChanged;
				_gamepad.LeftStickPressedChanged += OnGamepadLeftStickPressedChanged;
				_gamepad.RightStickPressedChanged += OnGamepadRightStickPressedChanged;
				_gamepad.LeftIndexButtonChanged += OnGamepadLeftIndexButtonChanged;
				_gamepad.RightIndexButtonChanged += OnGamepadRightIndexButtonChanged;
				_gamepad.LeftTriggerChanged += OnGamepadLeftTriggerChanged;
				_gamepad.RightTriggerChanged += OnGampadRightTriggerChanged;
				_gamepad.Start();

				btnStartStop.Text = "Start";

				_layoutComplete = true;
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
				Close();
			}
		}

		void StartVideoFeed()
		{
			// create MJPEG video source
			_videoStream = new MJPEGStream("http://thufir:8080/mjpeg");

			// set event handlers
			_videoStream.NewFrame += OnNewFrameReceived;
			// start the video source
			_videoStream.Start();
		}

		void LoadOptions()
		{
			checkFullScaleImages.Checked = Program.Config.FullScaleImages;
			textImageDelay.Text = Program.Config.CameraParameters.SnapshotDelay.TotalMilliseconds.ToString();
			textImageWidth.Text = Program.Config.CameraParameters.Width.ToString();
			textImageHeight.Text = Program.Config.CameraParameters.Height.ToString();
			textImageBrightness.Text = Program.Config.CameraParameters.Brightness.ToString();
			textImageContrast.Text = Program.Config.CameraParameters.Contrast.ToString();
			textImageSaturation.Text = Program.Config.CameraParameters.Saturation.ToString();
			textImageColorEffect.Text = Program.Config.CameraParameters.ColorEffect;
			listImageEffect.Items.AddRange(
				new List<String>()
				{
					"none",
					"negative",
					"solarise",
					"sketch",
					"denoise",
					"emboss",
					"oilpaint",
					"hatch",
					"gpen",
					"pastel",
					"watercolour",
					"film",
					"blur",
					"saturation",
					"colourswap",
					"washedout",
					"posterise",
					"colourpoint",
					"colourbalance",
					"cartoon",
				}.ToArray());
			listImageMetering.Items.AddRange(
				new List<String>()
				{
					"average",
					"spot",
					"backlit",
					"matrix",
				}.ToArray());
			listImageAutoWhite.Items.AddRange(
				new List<String>()
				{
					"off",
					"auto",
					"sun",
					"cloud",
					"shade",
					"tungsten",
					"fluorescent",
					"incandescent",
					"flash",
					"horizon",
				}.ToArray());
			listImageExposure.Items.AddRange(
				new List<String>()
				{
					"off",
					"auto",
					"night",
					"nightpreview",
					"backlight",
					"spotlight",
					"sports",
					"snow",
					"beach",
					"verylong",
					"fixedfps",
					"antishake",
					"fireworks",
				}.ToArray());
			listImageEffect.Text = Program.Config.CameraParameters.ImageEffect;
			listImageMetering.Text = Program.Config.CameraParameters.MeteringMode;
			listImageAutoWhite.Text = Program.Config.CameraParameters.AutoWhiteBalance;
			listImageExposure.Text = Program.Config.CameraParameters.Exposure;
			checkFlipHorizontal.Checked = Program.Config.CameraParameters.FlipHorizontal;
			checkFlipVertical.Checked = Program.Config.CameraParameters.FlipVertical;
		}

		void RepopulateLandmarks()
		{
#if zero
			listLandmarks.Clear();
			listLandmarks.AddColumnHeader(COL_POSITION, 80);
			listLandmarks.AddColumnHeader(COL_LABEL, 80);
			listLandmarks.AddColumnHeader(COL_RANGE, 80);
			listLandmarks.AddColumnHeader(COL_BEARING, -2);

			foreach(Landmark landmark in Landscape.Landmarks)
			{
				ListViewItem item = listLandmarks.AddRow(landmark.Location, landmark.Location, landmark);
				listLandmarks.SetListViewColumn(item, COL_LABEL, landmark.Label);
				//listLandmarks.SetListViewColumn(item, COL_RANGE, landmark.Label);
				//listLandmarks.SetListViewColumn(item, COL_LABEL, landmark.Label);
			}
#endif
		}

		void LoadLandscape()
		{
			try
			{
				_ds = DataSourceFactory.Create<TrackDataSource>(Program.Config.DBCredentials);
				TrackBotLandscape landscape;
				if(_ds.LandscapeGet<TrackBotLandscape>(Program.Config.LandscapeName, out landscape).ResultCode == DBResult.Result.Success)
				{
					Landscape = landscape;
					Landscape.DataSource = _ds;
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		void SetupBitmap()
		{
			int size = (int)(METERS_SQUARE * PixelsPerMeterImage);
			_radarBitmap = new Bitmap(size, size);
			using(Graphics g = Graphics.FromImage(_radarBitmap))
			{
				g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, size, size));
				Point center = _radarBitmap.Center().ToPoint();
				for(int radius = 1;radius < METERS_SQUARE / 2;radius++)
				{
					g.DrawEllipse(new Pen(Color.Green), _radarBitmap.RectangleForCircle(center, radius * (int)PixelsPerMeterImage));
				}

				g.DrawLine(new Pen(Color.Green), new Point(center.X - 5, center.Y), new Point(center.X + 5, center.Y));
				g.DrawLine(new Pen(Color.Green), new Point(center.X, center.Y - 5), new Point(center.X, center.Y + 5));
				picLidar.BackgroundImage = _radarBitmap;
			}

			Mat armBase = new Image<Bgr, byte>(Properties.Resources.arm_base).Mat;
			Mat armLower = new Image<Bgr, byte>(Properties.Resources.arm_rear).Mat;
			Mat armUpper = new Image<Bgr, byte>(Properties.Resources.arm_fore).Mat;
			Mat armClaw = new Image<Bgr, byte>(Properties.Resources.arm_claw).Mat;

			_robotArm = new BotArmImage(armBase, armLower, armUpper, armClaw);
		}

		private void OnDrawTimer(object sender, EventArgs e)
		{
			if(_client == null || _client.Connected == false)
				return;

			PointCloud2D cloud = _client.Vectors.ToPointCloud2D();

			// draw all the dots
			Mat bitmap = cloud.ToBitmap(_radarBitmap.Height, Color.Red, PixelsPerMeterImage);
			PointD origin = bitmap.Center();

			// draw fuzzy path
			if(_client.FuzzyPath != null && _client.ChassisMetrics != null)
			{
				BearingAndRange leftBar = PointD.Empty.BearingAndRangeTo(_client.FuzzyPath.FrontLeft.Origin).Scale(PixelsPerMeterImage);
				BearingAndRange rightBar = PointD.Empty.BearingAndRangeTo(_client.FuzzyPath.FrontRight.Origin).Scale(PixelsPerMeterImage);
				PointD leftFront = origin.GetPointAt(leftBar);
				PointD rightFront = origin.GetPointAt(rightBar);
				//				PointD frontLeft = origin.GetPointAt(_client.FuzzyPath.FrontLeft.Origin.Scale(PixelsPerMeterImage));
				bitmap.DrawVectorLines(_client.FuzzyPath.FrontLeft, leftFront, PixelsPerMeterImage, Color.DarkGreen);
				bitmap.DrawVectorLines(_client.FuzzyPath.FrontRight, rightFront, PixelsPerMeterImage, Color.DarkGray);
			}

			Image<Bgr, Byte> imageCV = new Image<Bgr, byte>(Properties.Resources.tank);
			Mat tank = imageCV.Mat;
			tank = tank.Rotate(_client.Bearing);
			bitmap.DrawCenteredImage(tank, new PointD(250, 250));
			Bitmap bm = new Bitmap(bitmap.Bitmap);
			picLidar.BackgroundImage = bm;

			if(_redrawDR)
			{
				DrawDREnvironment();
				_redrawDR = false;
			}

			RefreshMetrics();
		}

		void RefreshMetrics()
		{
			if(EnvironmentInfo != null)
			{
				botDash.Bearing = EnvironmentInfo.Bearing;
				botDash.DestinationBearing = EnvironmentInfo.DestinationBearing;
				botDash.FrontPrimaryRange = EnvironmentInfo.ForwardPrimaryRange;
				botDash.FrontSecondaryRange = EnvironmentInfo.ForwardSecondaryRange;
				botDash.RearPrimaryRange = EnvironmentInfo.BackwardPrimaryRange;
				botDash.RearSecondaryRange = EnvironmentInfo.BackwardSecondaryRange;

				Program.Compass.Bearing = EnvironmentInfo.Bearing;

				botDash.Redraw();
			}
		}

		void GetConnected()
		{
			_client = new TelemetryClient(Program.Config.MqttPublicHost, MqttClient.MakeRandomID(Text),
				new List<string>()
				{
					MqttTypes.CurrentPathTopic,
					MqttTypes.RangeBlobTopic,
					MqttTypes.BarriersTopic,
					MqttTypes.LandmarksTopic,
					MqttTypes.BearingTopic,
					MqttTypes.ImageMetricsTopic,
					MqttTypes.DistanceAndBearingTopic,
					MqttTypes.ChassisMetricsTopic,
					MqttTypes.DeadReckoningCompleteLandscapeTopic,
					MqttTypes.CameraLastImageTopic,
					MqttTypes.CameraLastAnalysisTopic,
					MqttTypes.SpeedAndBearingTopic,
				});
			_client.LandscapeMetricsReceived += OnLandscapeMetricsReceived;
			_client.ImageMetricsReceived += OnImageMetricsReceived;
			_client.EnvironmentInfoReceived += OnEnvironmentInfoReceived;
			_client.ChassisMetricsReceived += OnChassisMetricsReceived;
			_client.DeadReckoningEnvironmentReceived += OnDeadReckoningEnvironmentReceived;
			_client.CameraImageReceived += OnCameraImageReceived;
			_client.CameraImageAnalyzed += OnCameraImageAnalyzed;
			_client.SpeedAndBearing += OnSpeedAndBearingReceived;
			_client.Start();

			_mqqtController = new RaspiControlClient(Program.Config.MqttPublicHost, MqttClient.MakeRandomID(Text));
			_clawControl = new ClawControl(_mqqtController);
			_panTilt = new PanTiltControl(_mqqtController);
		}

		private void OnSpeedAndBearingReceived(SpeedAndBearing speedAndBearing)
		{
			_speedAndBearing = speedAndBearing;
		}

		private void OnCameraImageAnalyzed(ImageAnalysis analysis)
		{
			BeginInvoke(new MethodInvoker(delegate ()
			{
				DrawImages(analysis);

				TabPage tabPage = tabCameraImages.TabPages[TAB_FULL_IMAGE];
				if(tabPage.Tag is Mat && analysis.LEDs.Count > 0)
				{
					Mat image = tabPage.Tag as Mat;
					foreach(ColoredObjectPosition led in analysis.LEDs)
					{
						image.DrawCircleCross(led.Location, 6, led.Color);

					}
				}
				
			}));
		}

#region Drawing Stuff

		void DrawImages(ImageAnalysis analysis)
		{
			try
			{
				String TEMP_IMAGE_DIRECTORY = @"c:\pub\tmp\junk";

				foreach(String filename in analysis.FileNames)
				{
					String sourceFile = Path.Combine(Program.Config.RemoteImageDirectory, filename);
					String destFile = Path.Combine(TEMP_IMAGE_DIRECTORY, String.Format("{0}-{1}", _tempImageCount++, filename));
					if(File.Exists(sourceFile) == false)
						continue;
					File.Copy(sourceFile, destFile, true);
					Log.SysLogText(LogLevel.DEBUG, "Copied {0} to {1}", sourceFile, destFile);

					Mat bitmap = new Mat(destFile);

					String tabPageName = String.Empty;

					if(filename.Contains("blue"))
					{
						tabPageName = TAB_BLUE;
						BlueImage = destFile;
					}
					else if(filename.Contains("red"))
					{
						tabPageName = TAB_RED;
						RedImage = destFile;
					}
					else if(filename.Contains("green"))
					{
						tabPageName = TAB_GREEN;
						GreenImage = destFile;
					}
					else
					{
						tabPageName = TAB_FULL_IMAGE;
						DrawLEDPositions(bitmap, analysis.LEDs, analysis.Candidates);
						FullImage = destFile;
					} 

					SetImage(bitmap, tabPageName);
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		void DrawLEDPositions(Mat image, ColoredObjectPositionList leds, ObjectCandidateList candidates)
		{
			foreach(ColoredObjectPosition led in leds)
			{
				Size size = led.Size != Size.Empty ? led.Size : new Size(20, 20);
				size = size.Grow(10);
				Rectangle rectangle = RectangleExtensions.GetFromPoint(image.Size, led.Location.ToPoint(), size);
				led.Tag = rectangle;
				image.DrawRectangle(rectangle, led.Color, 1);

				ColoredObjectCandidate candidate;
				if(candidates.TryGetCandidateAtPoint(led.Location, out candidate))
				{
					DrawTextForRectangle(image, String.Format("{0:0.000}", candidate.Concentration), candidate.BoundingRectangle, FontFace.HersheyPlain, Color.Yellow, .5);
				}
			}

			foreach(ColoredObjectCandidate candidate in candidates)
			{
				if(candidate.BoundingRectangle.ContainsAny(leds.Points))
				{
					continue;
				}
				image.DrawRectangle(candidate.BoundingRectangle, candidate.Color, 1);
				DrawTextForRectangle(image, String.Format("{0:0.000}", candidate.Concentration), candidate.BoundingRectangle, FontFace.HersheyPlain, Color.White, .5);
			}
		}

		void DrawTextForRectangle(Mat image, String text, Rectangle rectangle, FontFace font, Color color, Double scale)
		{
			if(rectangle.Top > 20)
			{
				image.DrawTextAboveLine(text, FontFace.HersheyPlain, color, rectangle.TopLine(), 4, 1, scale);
			}
			else if(rectangle.Bottom < image.Height - 20)
			{
				image.DrawTextBelowLine(text, FontFace.HersheyPlain, color, rectangle.BottomLine(), 4, 1, scale);
			}
			else
			{
				image.DrawTextAboveLine(text, FontFace.HersheyPlain, color, rectangle.BottomLine(), 4, 1, scale);
			}
		}

		void SetImage(Mat image, String name)
		{
			if(tabCameraImages.TabPages.ContainsKey(name))
			{
				TabPage page = tabCameraImages.TabPages[name];
				page.AutoScroll = true;
				Control[] controls = page.Controls.Find(name, false);
				if(controls.Length > 0)
				{
					Log.SysLogText(LogLevel.DEBUG, "Writing {0} picbox from {1}", image.Size, name);
					image.Save(@"c:\pub\tmp\junk.bmp");

					PictureBox picBox = controls[0] as PictureBox;
					Bitmap bitmap  = new Bitmap(image.Bitmap.Clone() as Image);
					picBox.BackgroundImage = bitmap;
					picBox.Tag = page.Tag = image.Clone();

					if(Program.Config.FullScaleImages)
					{
						picBox.Size = image.Size;
						picBox.Dock = DockStyle.None;
					}
					else
					{
						picBox.Dock = DockStyle.Fill;
						picBox.BackgroundImageLayout = ImageLayout.Zoom;
					}
				}
			}
		}

		void SetupImageTabs()
		{
			tabCameraImages.TabPages.Clear();

			List<String> pageNames = new List<String>() { TAB_FULL_IMAGE, TAB_BLUE, TAB_GREEN, TAB_RED, };
			foreach(String name in pageNames)
			{
				TabPage page = new TabPage()
				{
					AutoScroll = true,
					Text = name,
					Name = name,
				};

				PictureBox pic = new PictureBox()
				{
					BackgroundImageLayout = ImageLayout.None,
					Dock = DockStyle.Fill,
					Location = Point.Empty,
					Name = name,
				};
				pic.MouseMove += OnAnalysisPicMouseMove;
				page.Controls.Add(pic);
				tabCameraImages.TabPages.Add(page);
			}
		}

		private void OnAnalysisPicMouseMove(object sender, MouseEventArgs e)
		{
			textAnalysisCoordinates.Text = String.Format("{0}, {1}", e.Location.X, e.Location.Y);
		}

#endregion

		private void OnCameraImageReceived(List<String> filenames)
		{
			BeginInvoke(new MethodInvoker(delegate()
			{
			}));
		}

		private void OnDeadReckoningEnvironmentReceived(DeadReckoningEnvironment environment)
		{
			DREnvironment = environment;
			_redrawDR = true;
		}

		private void OnChassisMetricsReceived(ChassisMetrics metrics)
		{
			Chassis = new Chassis(metrics.Length, metrics.Width);
			Chassis.Points.Add(ChassisParts.Lidar, metrics.LidarPosition);
			//Chassis.Points.Add(ChassisParts.FrontRangeFinder, new PointD(chassis.Width / 2, 0));
			//Chassis.Points.Add(ChassisParts.RearRangeFinder, new PointD(chassis.Width / 2, chassis.Length));
		}

		private void OnEnvironmentInfoReceived(EnvironmentInfo metrics)
		{
			EnvironmentInfo = metrics.Clone() as EnvironmentInfo;
		}

		private void OnImageMetricsReceived(ImageMetrics metrics)
		{
			ImageMetrics = metrics;
		}

		private void OnLandscapeMetricsReceived(LandscapeMetrics metrics)
		{
			LandscapeMetrics = metrics;
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			if(_videoStream != null)
			{
				_videoStream.Stop();
			}
			if(_client != null)
			{
				_client.Disconnect();
				_client = null;
			}
			if(_gamepad != null)
			{
				_gamepad.Stop();
			}

			ThreadBase.AbortAllRunningThreads();
		}

		private void OnFormResizeEnd(object sender, EventArgs e)
		{
			if(_layoutComplete)
			{
				Program.Config.LastRadarWindowSize = Size;
				Program.Config.Save();
			}

		}

		private void OnSplitterMoved(object sender, SplitterEventArgs e)
		{
			if(_layoutComplete)
			{
				Program.Config.SplitTopToBottomPosition = splitTopToBottom.SplitterDistance;
				Program.Config.SplitLeftToRightPosition = splitButtons.SplitterDistance;
				Program.Config.SplitRadarPosition = splitRadar.SplitterDistance;
				Program.Config.Save();
			}
		}

		private void OnWindowMoved(object sender, EventArgs e)
		{
			if(_layoutComplete)
			{
				Program.Config.LastRadarWindowLocation = Location;
			}
		}

		private void OnGrabInitialLandmarksClicked(object sender, EventArgs e)
		{
			MessageBox.Show("No way");
//			return;
			Landscape.CurrentLocation = Landscape.Center;
			LandmarkList landmarks = Landscape.CreateLandmarksFromImageVectors(Landscape.Center, _client.ImageMetrics.Scale, _client.ImageLandmarks);

			Landscape.Landmarks = landmarks;
			Landscape.ReplaceAllLandmarks();
		}

		private void OnLoadLandmarksClicked(object sender, EventArgs e)
		{
			ReloadLandmarks();
		}

		private void ReloadLandmarks()
		{
			Landscape.LoadLandmarks();

			Landscape.PreliminaryAnalysis();
			Mat bitmap = Landscape.CreateImage(PixelsPerMeterImage, SpatialObjects.Landmarks | SpatialObjects.GridLines | SpatialObjects.Labels);

			Bitmap output = bitmap.Bitmap;
			picFullEnvironment.BackgroundImage = output;
			picFullEnvironment.Size = output.Size;
		}

		private void OnFetchImageClicked(object sender, EventArgs e)
		{
#if zero
			Mat image = _lastFrame.Clone();
			ObjectDetect detect = new ObjectDetect(image, imageCanny, imageTriangles, imageRectangles, imageCircle, imageAllDetected, labelTriangles, labelRectangles, labelCircles);
			detect.Detect();
#endif
		}

		private void OnRunAnalysisClicked(object sender, EventArgs e)
		{
			LandmarkList pointMarkers;
			if(_ds.PointMarkersGet(Landscape, out pointMarkers).ResultCode == DBResult.Result.Success)
			{
				Landscape.PointMarkers = pointMarkers;
				Landscape.AnalyzeImageLandmarks();

				Mat bitmap = Landscape.CreateImage(PixelsPerMeterImage, SpatialObjects.Landmarks | SpatialObjects.GridLines | SpatialObjects.Labels | SpatialObjects.Circles);

				Bitmap output = bitmap.Bitmap;
				picFullEnvironment.BackgroundImage = output;
				picFullEnvironment.Size = output.Size;
			}
		}

		private void OnRadarMouseMove(object sender, MouseEventArgs e)
		{
			PointD cart = new PointD(e.Location);
			BearingAndRange vector = new BearingAndRange(picLidar.BackgroundImage.Center(), cart);
			vector.Range /= PixelsPerMeterImage;
			String text = String.Format("{0}, {1}  [{2:0.000}m {3:0.0}°]", cart.X, cart.Y, vector.Range, vector.Bearing);
			textRadarCateresian.Text = text;
		}

		private void OnLandscapeMouseMove(object sender, MouseEventArgs e)
		{
			PointD point = new PointD(e.Location);
			point.Scale(1 / PixelsPerMeterLandscape);
			textLandscapeCoords.Text = String.Format("{0}", point.ToString(3));
		}

		private void OnLandscapeMouseClick(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right)
			{
				PointD point = new PointD(e.Location);
				point.Scale(1 / PixelsPerMeterLandscape);

				Landmark landmark;
				if((landmark = Landscape.Landmarks.GetLandmarkNear(point, .1)) != null)
				{
					cmenuLandscape.Tag = landmark;
					cmenuLandscape.Show(picFullEnvironment.PointToScreen(e.Location));
				}
			}
		}

		private void OnAssignLandmarkLabelClicked(object sender, EventArgs e)
		{
			SimpleTextSelectionDialog dlg = new SimpleTextSelectionDialog("Enter Label");
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				Landmark landmark = cmenuLandscape.Tag as Landmark;
				landmark.Label = dlg.Answer;
				_ds.LandmarkUpdate(landmark);
			}
		}

		private void OnPullClicked(object sender, EventArgs args)
		{
			try
			{
				if(DREnvironment == null)
				{
					DeadReckoningEnvironment env;
					TrackDataSource tds = DataSourceFactory.Create<TrackDataSource>(Program.Config.DBCredentials);
					if(tds.GetDREnvironment(DR_NAME, out env).ResultCode == DBResult.Result.Success)
					{
						DREnvironment = env;
					}
					DREnvironment.SetCurrentLocation(DREnvironment.Origin);
					tds.SetGridLocation(DR_NAME, LocationType.Current, DREnvironment.Origin);
				}
				if(DREnvironment != null)
				{
					//PointCloud2D slice = _client.Vectors.ToPointCloud2D(360);
					//DREnvironment.ProcessEnvironment(slice);
					DrawDREnvironment();
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private void OnParmsClicked(object sender, EventArgs e)
		{
			ProcessingMetricsDialog dlg = new ProcessingMetricsDialog()
			{
				ProcessingMetrics = Program.Config.ProcessingMetrics
			};
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				Program.Config.ProcessingMetrics = dlg.ProcessingMetrics;
				Program.Config.Save();
			}
		}

		private void DrawDREnvironment()
		{
			int pixelHeight = (int)(DREnvironment.Height * PixelsPerMeterDR);
			int pixelWidth = (int)(DREnvironment.Width * PixelsPerMeterDR);
			Mat bitmap = new Mat(pixelWidth, pixelHeight, DepthType.Cv8U, 3);

			Size frameSize = new Size((int)(pixelWidth / DREnvironment.Width * DREnvironment.Scale), (int)(pixelHeight / DREnvironment.Height * DREnvironment.Scale));
			Size squareSize = frameSize.Shrink(2);
			for(int x = 0;x < DREnvironment.Grid.Matrix.Width;x++)
			{
				for(int y = 0;y < DREnvironment.Grid.Matrix.Height;y++)
				{
					GridCell cell = DREnvironment.Grid.Matrix.Cells[x, y];
					Color cellColor = Color.AliceBlue;
					switch(cell.State)
					{
						case CellState.Occupied:
							cellColor = Color.Red;
							break;

						case CellState.Unoccupied:
							cellColor = Color.Blue;
							break;
					}
					PointD point = new PointD(x  * frameSize.Width, y * frameSize.Height);
					Rectangle rect = new Rectangle(point.ToPoint(), squareSize);
					bitmap.DrawRectangle(rect, cellColor);
				}
			}
			picDeadReckoning.BackgroundImage = bitmap.Bitmap;
			picDeadReckoning.Location = new Point(0, 0);
			picDeadReckoning.Size = bitmap.Size;
		}

		private void DrawDeadReckoningPaths()
		{
			String pic = @"c:\pub\tmp\0208-snap.jpg";
			Mat mat = new Mat(pic);

			String file = @"c:\pub\tmp\blob.bin";
			byte[] bytes = File.ReadAllBytes(file);
			PointCloud2D cloud = new PointCloud2D(bytes);

			FuzzyPathList paths = LidarEnvironment.FindGoodDestinations(cloud, Chassis, 30, 4, .5);

			// draw all the dots
			Mat bitmap = cloud.ToBitmap(_radarBitmap.Height, Color.Red, PixelsPerMeterImage);
			PointD origin = bitmap.Center();

			// draw fuzzy path
			if(paths.Count > 0 && _client.ChassisMetrics != null)
			{
				foreach(FuzzyPath path in paths)
				{
					BearingAndRange leftBar = PointD.Empty.BearingAndRangeTo(path.FrontLeft.Origin).Scale(PixelsPerMeterImage);
					BearingAndRange rightBar = PointD.Empty.BearingAndRangeTo(path.FrontRight.Origin).Scale(PixelsPerMeterImage);
					PointD leftFront = origin.GetPointAt(leftBar);
					PointD rightFront = origin.GetPointAt(rightBar);
					//				PointD frontLeft = origin.GetPointAt(_client.FuzzyPath.FrontLeft.Origin.Scale(PixelsPerMeterImage));
					bitmap.DrawMinMaxLines(path.FrontLeft, leftFront, PixelsPerMeterImage, Color.DarkGreen);
					bitmap.DrawMinMaxLines(path.FrontRight, rightFront, PixelsPerMeterImage, Color.DarkGray);
				}
			}

			//LidarEnvironment env = new LidarEnvironment(ImageMetrics.MetersSquare, AnalysisPixelsPerMeter)
			//{
			//	ProcessingMetrics = Program.Config.ProcessingMetrics
			//};

			//env.Location = picWorkBitmap.BackgroundImage.Center();
			//env.ProcessImage(_sharpened, 0, AnalysisPixelsPerMeter);

			//Mat output = env.CreateImage(SpatialObjects.Everything);
			//picWorkBitmap.Image = output.Bitmap;
		}

		private void OnSavePointCloudClicked(object sender, EventArgs args)
		{
			try
			{
				VistaSaveFileDialog dlg = new VistaSaveFileDialog()
				{
					Title = "Enter File Name",
					Filter = @"Binary files (*.bin)|*.bin|All files (*.*)|*.*",
					DefaultExt = ".bin",
					RestoreDirectory = true,
				};
				if(dlg.ShowDialog() == DialogResult.OK)
				{
					PointCloud2D cloud = _client.Vectors.ToPointCloud2D();
					byte[] serialized = cloud.Serialize();
					File.WriteAllBytes(dlg.FileName, serialized);
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private void OnSaveBitmapClicked(object sender, EventArgs args)
		{
			try
			{
				VistaSaveFileDialog dlg = new VistaSaveFileDialog()
				{
					Title = "Enter File Name",
					Filter = @"Binary files (*.png)|*.png|All files (*.*)|*.*",
					DefaultExt = ".png",
					RestoreDirectory = true,
				};
				if(dlg.ShowDialog() == DialogResult.OK)
				{
					Mat bitmap = _client.Vectors.ToPointCloud2D(0, _client.ImageMetrics.PixelsPerMeter).ToBitmap((int)(_client.ImageMetrics.MetersSquare * _client.ImageMetrics.PixelsPerMeter), Color.Red);
					bitmap.Save(dlg.FileName);
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private void OnCommandButtonClicked(object sender, EventArgs e)
		{
			if(textCommand.Text.Length > 0)
			{
				_mqqtController.SendCommand(textCommand.Text);
				textCommand.Text = String.Empty;
				textCommand.Select();
			}

		}

		private void OnTabAnalysisSelected(object sender, TabControlEventArgs e)
		{
			Program.Config.LastAnalyticsTabIndex = e.TabPageIndex;
			Program.Config.Save();
		}

		private void OnCaptureImageGrabbed(object sender, EventArgs e)
		{
			if(_capture != null && _capture.Ptr != IntPtr.Zero)
			{
				lock(_capture)
				{
					_capture.Retrieve(_captureFrame, 0);
					// picVideo.Image = _captureFrame.Bitmap;
				}
			}
		}

		private void OnSnapClicked(object sender, EventArgs e)
		{
			Mat bitmap = null;
			lock(_capture)
			{
				bitmap = _captureFrame.Clone();
			}
			bitmap.Save(@"c:\pub\tmp\full.png");


			int lowThreshold = 150;
			int highThreshold = 255;
			MCvScalar lowRange = new MCvScalar(lowThreshold, 0, 0);
			MCvScalar topRange = new MCvScalar(highThreshold, 100, 100);
			Mat outputImage = new Mat(bitmap.Size, DepthType.Cv8U, 1);

			CvInvoke.InRange(bitmap, new ScalarArray(lowRange), new ScalarArray(topRange), outputImage);
			int totalcount = CvInvoke.CountNonZero(outputImage);
			outputImage.Save(@"c:\pub\tmp\blue.png");
		}

		private void OnFullScaleImagesChecked(object sender, EventArgs e)
		{
			if(_layoutComplete)
			{
				Program.Config.FullScaleImages = checkFullScaleImages.Checked;
				Program.Config.Save();
			}
		}

		private void OnRotationChange(object sender, EventArgs e)
		{
			_mqqtController.SendPercentage(MqttTypes.ArmRotationTopic, ((TrackBar)sender).Value);
		}

		private void OnElevationChange(object sender, EventArgs e)
		{
			_mqqtController.SendPercentage(MqttTypes.ArmElevationTopic, ((TrackBar)sender).Value);
		}

		private void OnThrustChange(object sender, EventArgs e)
		{
			_mqqtController.SendPercentage(MqttTypes.ArmThrustTopic, ((TrackBar)sender).Value);
		}

		private void OnClawChange(object sender, EventArgs e)
		{
			_mqqtController.SendPercentage(MqttTypes.ArmClawTopic, ((TrackBar)sender).Value);
		}

		private void OnImageParametersCheckChanged(object sender, EventArgs e)
		{
			if(_layoutComplete)
			{
				RaspiCameraParameters parameters = new RaspiCameraParameters()
				{
					SnapshotDelay = TimeSpan.FromMilliseconds(int.Parse(textImageDelay.Text)),
					Width = int.Parse(textImageWidth.Text),
					Height = int.Parse(textImageHeight.Text),
					Brightness = int.Parse(textImageBrightness.Text),
					Contrast = int.Parse(textImageContrast.Text),
					Saturation = int.Parse(textImageSaturation.Text),
					ImageEffect = listImageEffect.Text,
					MeteringMode = listImageMetering.Text,
					AutoWhiteBalance = listImageAutoWhite.Text,
					Exposure = listImageExposure.Text,
					FlipHorizontal = checkFlipHorizontal.Checked,
					FlipVertical = checkFlipVertical.Checked,
				};

				if(parameters.Equals(Program.Config.CameraParameters) == false)
				{
					Program.Config.CameraParameters = parameters;
					Program.Config.Save();
					_mqqtController.SendCameraParameters(Program.Config.CameraParameters);
				}
			}
		}

		private void OnJoystickSpeedChanged(int left, int right)
		{
			Log.SysLogText(LogLevel.DEBUG, "Left {0} Right: {1}", left, right);
			//if(_mqqtController != null && _mqqtController.Client != null && _mqqtController.Client.Connected)
			//{
			//	Log.SysLogText(LogLevel.DEBUG, "Left {0} Right: {1}", left, right);
			//	_mqqtController.SendSpeed(left, right);
			//}
		}

		private void OnGamepadEvent(GamePadBase gamePad)
		{
			const int CLAW_ZONE = 3000;

			if(gamePad.LeftStick.Press == false && gamePad.RightStick.Press == false)
			{
				Double leftSpeed = (int)(((Double)gamePad.LeftStick.Y / 65535 * 200 - 100) * -1);
				Double rightSpeed = (int)(((Double)gamePad.RightStick.Y / 65535 * 200 - 100) * -1);

				if(gamePad.RightStick.X > GamePadTypes.MAX_ANALOG - CLAW_ZONE)
				{
					_clawControl.SetClaw(100);
				}
				else if(gamePad.RightStick.X < CLAW_ZONE)
				{
					_clawControl.SetClaw(0);
				}
				//Log.SysLogText(LogLevel.DEBUG, "Would set L: {0}  R: {1}", leftSpeed, rightSpeed);
				_mqqtController.SendSpeed((int)leftSpeed, (int)rightSpeed);
			}

		}

		private void OnGamepadLeftStickPressedChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button state)
		{
			_clawControl.Rotation = 50;
		}

		private void OnGamepadRightStickPressedChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button state)
		{
		}

		const int CLAW_CONTROL_QUANTUM = 2;
		const int PAN_TILT_CONTROL_QUANTUM = 2;
		private void OnGamepadXButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			if(button.State)
				_mqqtController.SendSpinStepLeftTime(TimeSpan.FromMilliseconds(50));
		}

		private void OnGamepadBButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			if(button.State)
				_mqqtController.SendSpinStepRightTime(TimeSpan.FromMilliseconds(50));
		}

		private void OnGamepadLeftIndexButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			if(button.State)
				_clawControl.ChangeRotation(-CLAW_CONTROL_QUANTUM);
		}

		private void OnGamepadRightIndexButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			if(button.State)
				_clawControl.ChangeRotation(CLAW_CONTROL_QUANTUM);
		}

		private void OnGampadHatChanged(GamePadBase gamePad, Hat hat)
		{
			if(hat.Up.State)
				_panTilt.TiltDown(PAN_TILT_CONTROL_QUANTUM);
			if(hat.Down.State)
				_panTilt.TiltUp(PAN_TILT_CONTROL_QUANTUM);
			if(hat.Right.State)
				_panTilt.PanUp(PAN_TILT_CONTROL_QUANTUM);
			if(hat.Left.State)
				_panTilt.PanDown(PAN_TILT_CONTROL_QUANTUM);
		}

		private void OnGamepadLeftTriggerChanged(GamePadBase gamePad, AnalogControl control)
		{
			int value = AdjustToPercent(control.Value, false);
			Log.SysLogText(LogLevel.DEBUG, "Set Left To {0}", value);
			_clawControl.Elevation = value;
		}

		private void OnGampadRightTriggerChanged(GamePadBase gamePad, AnalogControl control)
		{
			int value = AdjustToPercent(control.Value, true);
			Log.SysLogText(LogLevel.DEBUG, "Set Right To {0}", value);
			_clawControl.Thrust = value;
		}

		int AdjustToPercent(Double value, bool reverse)
		{
			Double v = value / (Double)GamePadTypes.MAX_ANALOG * (Double)100;
			if(reverse)
				v = Math.Abs(100 - v);
			return (int)v;
		}

		/// <summary>
		/// Y - Step Forward
		/// </summary>
		/// <param name="gamePad"></param>
		/// <param name="button"></param>
		private void OnGamepadYButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			Log.SysLogText(LogLevel.DEBUG, "YButton {0}", button);
			if(button.State)
				_mqqtController.SendMoveStep(Direction.Forward, TimeSpan.FromMilliseconds(50), 80);
		}

		/// <summary>
		/// A - Step backward
		/// </summary>
		/// <param name="gamePad"></param>
		/// <param name="button"></param>
		private void OnGampadAButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			Log.SysLogText(LogLevel.DEBUG, "AButton {0}", button);
			if(button.State)
				_mqqtController.SendMoveStep(Direction.Backward, TimeSpan.FromMilliseconds(50), 80);
		}

		private void OnPreferencesClicked(object sender, EventArgs e)
		{
			PreferencesForm dlg = new PreferencesForm();
			dlg.ShowDialog();
		}

		private void OnStartStopClassifyVideoClicked(object sender, EventArgs e)
		{
			_grabbingImages = _grabbingImages ? false : true;
			btnStartStop.Text = _grabbingImages ? "Stop" : "Start";
		}

		Point _classifyClickPoint;
		private void OnClassifyMouseDown(object sender, MouseEventArgs e)
		{
			_classifyClickPoint = e.Location;
			_selectingClassifyImage = true;
			_classifySelectAnchor = e.Location;
			btnAddClassify.Enabled = true;
			_classifierCleanImage = picClassifer.BackgroundImage as Bitmap;
		}

		private void OnClassifyMouseUp(object sender, MouseEventArgs e)
		{
			_selectingClassifyImage = false;
		}

		private void ClearClassifierRectangle()
		{
			if(_classifySelectedRectangle != Rectangle.Empty)
			{
				Rectangle rect = picClassifer.RectangleToScreen(_classifySelectedRectangle);
				ControlPaint.DrawReversibleFrame(rect, Color.Gray, FrameStyle.Dashed);
				_classifySelectedRectangle = Rectangle.Empty;
			}
		}

		private void OnClassifyMouseMove(object sender, MouseEventArgs e)
		{
			if(_selectingClassifyImage)
			{
				ClearClassifierRectangle();
				_classifySelectedRectangle = RectangleExtensions.GetFromTwoPoints(_classifySelectAnchor, e.Location);
				Rectangle rect = picClassifer.RectangleToScreen(_classifySelectedRectangle);
				ControlPaint.DrawReversibleFrame(rect, Color.Gray, FrameStyle.Dashed);
			}
		}

		private void OnAddClicked(object sender, EventArgs e)
		{
			String directory = @"c:\pub\tmp\classify";
			String file = Path.Combine(directory, "classifier_collection.txt");
			String imageFile = DirectoryExtensions.GetNextNumberedFileName(directory, "image", ".jpg", 4);
			_classifierCleanImage.Save(imageFile);
			String output = String.Format("{0}  1  {1} {2} {3} {4}\n", 
				Path.GetFileName(imageFile), _classifySelectedRectangle.X, _classifySelectedRectangle.Y, _classifySelectedRectangle.Width, _classifySelectedRectangle.Height);
			File.AppendAllText(file, output);
			ClearClassifierRectangle();
			picClassifer.BackgroundImage = _classifierCleanImage;
			_selectingClassifyImage = false;
			OnStartStopClassifyVideoClicked(null, null);
		}

		private void OnAnalyzeCascadeClicked(object sender, EventArgs e)
		{
			VistaOpenFileDialog dlg = new VistaOpenFileDialog();
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				FaceNameList names;
				_fds.GetAllNames(out names);

				String imageDirectory = Environment.OSVersion.Platform == PlatformID.Unix
					? @"/home/pi/classify/"
					: @"c:\pub\classify\images";
				String cascadeDirectory = Environment.OSVersion.Platform == PlatformID.Unix
					? @"/home/pi/classify/cascades"
					: @"c:\pub\classify\cascades";
				String inputFile = dlg.FileName;
				String cascadeFile = Path.Combine(cascadeDirectory, "haarcascade_frontalface_default.xml");
				Mat image = new Mat(inputFile);
				_classifierImage = image.Clone();
				_classifyRectangles = image.FindObjects(FaceClassifier, 1.1, 3, Program.Config.FaceDetectSize);
				foreach(Rectangle rectangle in _classifyRectangles)
				{
					image.DrawRectangle(rectangle, Color.Red, 5);
					if(_faceRecognizer != null)
					{
						Mat resized = new Mat(_classifierImage, rectangle).Resize(Program.Config.FaceDetectSize, 0, 0, Inter.Cubic).ToGrayscaleImage();
						resized.Save(@"c:\pub\tmp\images\grey.jpg");
						FaceRecognizer.PredictionResult result = _faceRecognizer.Predict(resized);
						if(result.Label != 0 && result.Distance < 3000)
						{
							FaceName facename = names.Find(n => n.NameID == (UInt32)result.Label);
							if(facename != null)
							{
								String name = facename.Name;
								image.DrawTextBelowRectangle(name, FontFace.HersheyPlain, Color.AliceBlue, rectangle, 50, 3, 2);
							}
						}
					}
				}
				picClassifer.Image = new Bitmap(image.Bitmap);
			}
		}

		private void OnClassifyOpening(object sender, CancelEventArgs e)
		{
			List<String> names;
			_fds.GetAllNames(out names);
			setNameToolStripMenuItem.DropDownItems.Clear();
			foreach(String name in names)
			{
				ToolStripMenuItem item = new ToolStripMenuItem(name);
				item.Click += OnNameClicked;
				setNameToolStripMenuItem.DropDownItems.Add(item);
			}
		}

		private void OnNameClicked(object sender, EventArgs e)
		{
			String name = ((ToolStripMenuItem)sender).Text;
			Rectangle found = new Rectangle();
			foreach(Rectangle rectangle in _classifyRectangles)
			{
				if(rectangle.Contains(_classifyClickPoint))
				{
					if(found.IsEmpty || rectangle.Area() < found.Area())
					{
						found = rectangle;
					}
				}
			}

			if(found.IsEmpty == false)
			{
				Mat part = new Mat(_classifierImage, found).ToGrayscaleImage().Resize(Program.Config.FaceDetectSize, 0, 0, Inter.Cubic);
				_fds.AddImage(name, part);
			}

		}

		private void OnAnalyzeFacesClicked(object sender, EventArgs e)
		{
			VistaOpenFileDialog dlg = new VistaOpenFileDialog();
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				String inputFile = dlg.FileName;
				if(_faceRecognizer != null)
				{
				}

			}
		}

	}
}
