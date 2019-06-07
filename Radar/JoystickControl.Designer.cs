namespace Radar
{
	partial class JoystickControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// JoystickControl
			// 
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnJoystickMouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnJoystickMouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnJoystickMouseUp);
			this.Resize += new System.EventHandler(this.OnResize);
			this.ResumeLayout(false);

		}

		#endregion
	}
}
