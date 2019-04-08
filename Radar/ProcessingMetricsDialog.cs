using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RaspiCommon.Spatial.Imaging;

namespace Radar
{
	public partial class ProcessingMetricsDialog : Form
	{
		public ProcessingMetrics ProcessingMetrics { get; set; }

		public ProcessingMetricsDialog()
		{
			ProcessingMetrics = new ProcessingMetrics();

			InitializeComponent();
		}

		private void OnFormLoad(object sender, EventArgs args)
		{
			try
			{
				LoadTextFields();
				textCannyThreshold.Select();
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
				Close();
			}

		}

		void LoadTextFields()
		{
			textCannyThreshold.Text = ProcessingMetrics.CannyThreshold.ToString();
			textThresholdLinking.Text = ProcessingMetrics.CannyThresholdLinking.ToString();
			textRhoRes.Text = ProcessingMetrics.RhoRes.ToString();
			textThetaRes.Text = ProcessingMetrics.ThetaResPiDivisor.ToString();
			textHoughThreshold.Text = ProcessingMetrics.HoughThreshold.ToString();
			textMinLandmarkSegment.Text = ProcessingMetrics.MinimumLandmarkSegment.ToString();
			textMinConnectLength.Text = ProcessingMetrics.MinimumConnectLength.ToString();
			textMaxConnectLength.Text = ProcessingMetrics.MaximumConnectLength.ToString();
			textRightAngleSlack.Text = ProcessingMetrics.RightAngleSlackDegrees.ToString();
			textMinLandmarkSeparation.Text = ProcessingMetrics.MinimumLandmarkSeparation.ToString();
			textBearingSlack.Text = ProcessingMetrics.BearingSlack.ToString();
			textLinePathWidth.Text = ProcessingMetrics.LinePathWidth.ToString();
		}

		private void OnInputFieldChanged(object sender, EventArgs e)
		{
			foreach(Control control in Controls)
			{
				if(control is TextBox)
				{
					Double value;
					if(Double.TryParse(control.Text, out value) == false)
					{
						control.ForeColor = Color.Red;
					}
					else
					{
						control.ForeColor = Color.Black;
					}
				}
			}
		}

		private void OnOKClicked(object sender, EventArgs e)
		{
			Double cannyThreshold, thresholdLinking, rhoRes, thetaRes, houghThreshold;
			Double minLandmarkSegment, minConnectLength, maxConnectLength, rightAngleSlack, minLandmarkSeparation, bearingSlack, linePathWidth;
			if( Double.TryParse(textCannyThreshold.Text, out cannyThreshold) &&
				Double.TryParse(textThresholdLinking.Text, out thresholdLinking) &&
				Double.TryParse(textRhoRes.Text, out rhoRes) &&
				Double.TryParse(textThetaRes.Text, out thetaRes) &&
				Double.TryParse(textHoughThreshold.Text, out houghThreshold) &&
				Double.TryParse(textMinLandmarkSegment.Text, out minLandmarkSegment) &&
				Double.TryParse(textMinConnectLength.Text, out minConnectLength) &&
				Double.TryParse(textMaxConnectLength.Text, out maxConnectLength) &&
				Double.TryParse(textRightAngleSlack.Text, out rightAngleSlack) &&
				Double.TryParse(textMinLandmarkSeparation.Text, out minLandmarkSeparation) &&
				Double.TryParse(textBearingSlack.Text, out bearingSlack) &&
				Double.TryParse(textLinePathWidth.Text, out linePathWidth))
			{
				ProcessingMetrics = new ProcessingMetrics()
				{
					CannyThreshold = cannyThreshold,
					CannyThresholdLinking = thresholdLinking,
					RhoRes = rhoRes,
					ThetaResPiDivisor = thetaRes,
					HoughThreshold = (int)houghThreshold,
					MinimumLandmarkSegment = minLandmarkSegment,
					MinimumConnectLength = minConnectLength,
					MaximumConnectLength = maxConnectLength,
					RightAngleSlackDegrees = rightAngleSlack,
					MinimumLandmarkSeparation = minLandmarkSeparation,
					BearingSlack = bearingSlack,
					LinePathWidth = linePathWidth
				};
				DialogResult = DialogResult.OK;
				Close();
			}

		}

		private void OnDefaultsClicked(object sender, EventArgs e)
		{
			ProcessingMetrics = new ProcessingMetrics();
			LoadTextFields();
		}
	}
}
