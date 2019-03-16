using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System;

namespace KanoopCommon.CommonObjects
{
	public class ColorPair
	{
		Color	m_ForeColor;
		public Color ForeColor
		{
			get { return m_ForeColor; }
			set { m_ForeColor = value; }
		}
		Color	m_BackColor;
		public Color BackColor
		{
			get { return m_BackColor; }
			set { m_BackColor = value; }
		}

		public ColorPair(Color fore, Color back)
		{
			m_ForeColor = fore;
			m_BackColor = back;
		}

		public ColorPair(String str)
		{
			String[] parts = str.Split(',');
			m_ForeColor = Color.FromArgb(Convert.ToInt32(parts[0]));
			m_BackColor = Color.FromArgb(Convert.ToInt32(parts[1]));
		}

		public bool Equals(Color foreColor, Color backColor)
		{
			return m_ForeColor.Equals(foreColor) && m_BackColor.Equals(backColor);
		}

		public bool Equals(ColorPair other)
		{
			return m_ForeColor.Equals(other.ForeColor) && m_BackColor.Equals(other.BackColor);
		}

		public override string ToString()
		{
			return String.Format("{0},{1}", m_ForeColor.ToArgb(), m_BackColor.ToArgb());
		}

		public ColorPair Clone()
		{
			return new ColorPair(m_ForeColor, m_BackColor);
		}
	}
}
