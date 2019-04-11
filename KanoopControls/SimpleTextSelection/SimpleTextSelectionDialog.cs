using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KanoopControls.SimpleTextSelection
{
	public partial class SimpleTextSelectionDialog : Form
	{
		String _prompt;

		String _title;
		String _defaultText;
		String _answer;
		List<String> _comboBoxList;

		public String Answer { get { return _answer; } }

		public SimpleTextSelectionDialog(String prompt)
			: this(prompt, String.Empty, "") {}

		public SimpleTextSelectionDialog(String prompt, String title)
			: this(prompt, title, "") {}

		public SimpleTextSelectionDialog(String prompt, String title, String defaultText)
			: this(prompt, title, defaultText, null) {}

		public SimpleTextSelectionDialog(String strPrompt, String strTitle, String strDefaultText, List<String> comboBoxList)
		{
			_title = strTitle;
			_prompt = strPrompt;
			_defaultText = strDefaultText;
			_comboBoxList = comboBoxList;

			InitializeComponent();
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			Text = _title;
			lblQuestion.Text = _prompt;
			textAnswer.Text = _defaultText;
			if(_comboBoxList != null)
			{
				foreach(String str in _comboBoxList)
				{
					textAnswer.Items.Add(str);
				}
			}
			textAnswer.Select();
		}

		private void OnOKClicked(object sender, EventArgs e)
		{
			_answer = textAnswer.Text;
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
