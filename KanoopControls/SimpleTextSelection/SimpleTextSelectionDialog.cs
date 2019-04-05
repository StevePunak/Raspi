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
		String m_strPrompt;

		String m_strTitle;
		String m_strDefaultText;
		String m_strAnswer;
		List<String> m_ComboBoxList;

		public String Answer { get { return m_strAnswer; } }

		public SimpleTextSelectionDialog(String prompt)
			: this(prompt, String.Empty, "") {}

		public SimpleTextSelectionDialog(String prompt, String title)
			: this(prompt, title, "") {}

		public SimpleTextSelectionDialog(String prompt, String title, String defaultText)
			: this(prompt, title, defaultText, null) {}

		public SimpleTextSelectionDialog(String strPrompt, String strTitle, String strDefaultText, List<String> comboBoxList)
		{
			m_strTitle = strTitle;
			m_strPrompt = strPrompt;
			m_strDefaultText = strDefaultText;
			m_ComboBoxList = comboBoxList;

			InitializeComponent();
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			Text = m_strTitle;
			lblQuestion.Text = m_strPrompt;
			textAnswer.Text = m_strDefaultText;
			if(m_ComboBoxList != null)
			{
				foreach(String str in m_ComboBoxList)
				{
					textAnswer.Items.Add(str);
				}
			}
			textAnswer.Select();
		}

		private void OnOKClicked(object sender, EventArgs e)
		{
			m_strAnswer = textAnswer.Text;
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
