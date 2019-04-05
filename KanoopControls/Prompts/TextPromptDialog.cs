using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KanoopControls.Prompts
{
	public partial class TextPromptDialog : Form
	{
		public String ReplyText { get; set; }

		public String Prompt { set { Text = value; } }

		public bool MultiLine { get; set; }

		public TextPromptDialog()
			: this("") {}

		public TextPromptDialog(String text)
		{
			ReplyText = text;

			InitializeComponent();
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			if(MultiLine == false)
			{
				textReply.Multiline = false;
				textReply.Height = new TextBox().Height;
				Height = btnCancel.Location.Y + btnCancel.Height + 50;
			}
			textReply.Text = ReplyText;
			textReply.Select();
		}

		private void OnTextChanged(object sender, EventArgs e)
		{

		}

		private void OnOKClicked(object sender, EventArgs e)
		{
			ReplyText = textReply.Text;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void OnCancelClicked(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}


	}
}
