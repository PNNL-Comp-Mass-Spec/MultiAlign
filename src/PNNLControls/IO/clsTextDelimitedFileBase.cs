using System;
using System.Collections;


namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsTextDelimitedFileBase.
	/// </summary>
	public class clsTextDelimitedFileBase: clsDataTableBase
	{
		protected string m_headerString;
		protected string m_text;
		protected char m_delimiter;
		protected int m_linesBeforeHeader;

		public clsTextDelimitedFileBase()
		{
			m_linesBeforeHeader = -1;
		}

		public override void Clear()
		{
			base.Clear();
			m_text = string.Empty;
		}
	
		/// <summary>
		/// Holds the header information of the data columns
		/// </summary>
		public string HeaderString
		{
			get
			{
				return m_headerString;
			}
			set
			{
				m_headerString = value;
			}
		}

		/// <summary>
		/// Gets the text from the file.
		/// </summary>
		public string Text
		{
			get
			{
				return m_text;
			}
		}
		
		/// <summary>
		/// Lines to read/write before the column headers / data are written/read.
		/// </summary>
		public int LinesBeforeHeader
		{
			get
			{
				return m_linesBeforeHeader;
			}
			set
			{
				m_linesBeforeHeader = value;
			}
		}

		/// <summary>
		/// Character for delimiting the text.
		/// </summary>
		public char Delimiter
		{
			get
			{
				return m_delimiter;
			}
			set
			{
				m_delimiter = value;
			}
		}
	}
}
