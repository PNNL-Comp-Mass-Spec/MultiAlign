using System;
using System.IO;
using System.Collections;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsTextDelimitedFileWriter.
	/// </summary>
	public class clsTextDelimitedFileWriter : clsTextDelimitedFileBase
	{
		private string m_path;

		public clsTextDelimitedFileWriter()
		{
		}
		
		/// <summary>
		/// Reads the file via the specified format.  Columns = true means headers are the first line in column format.
		/// </summary>
		/// <param name="path">Path to save file.</param>
		/// <param name="columns">Columns = true if header information is stored in column format.</param></param>
		public void Write(string path, bool columns)
		{	
			m_path = path;
			if (columns == true)
				WriteColumns(path, false);
			else
				WriteRows(path, false);
		}


		/// <summary>
		/// Reads the file via the specified format.  Columns = true means headers are the first line in column format.
		/// </summary>
		/// <param name="path">Path to save file</param>
		/// <param name="columns">Columns = true if header information is stored in column format.</param></param>
		/// <param name="append">Append to file or overwrite.</param>
		public void Write(string path, bool columns, bool append)
		{	
			m_path = path;
			if (columns == true)
				WriteColumns(path, append);
			else
				WriteRows(path, append);
		}

		/// <summary>
		/// Reads the file with headers in the rows.
		/// </summary>
		public void Write(string path)
		{
			m_path = path;
			WriteRows(path, false);
		}

		/// <summary>
		/// Reads the data with headers in column format.
		/// </summary>
		private void WriteColumns(string path, bool append)
		{
			StreamWriter fs = new StreamWriter(path, append);
			string line = string.Empty;

			WriteLines(fs, m_linesBeforeHeader);
		
			if (HeaderString != string.Empty)
				fs.WriteLine(HeaderString);

			/// 
			/// Write the header
			/// 
			foreach(string header in m_headers)
			{
				line += header + m_delimiter.ToString();
			}
			line = line.TrimEnd(m_delimiter);
			fs.WriteLine(line);

			/// 
			/// Write the data
			/// 
			
			int currentLineNumber = 0;
			bool dataLeft = true;
			while (dataLeft == true)
			{		
				dataLeft = false;	
				line = string.Empty;
				for(int i = 0; i < m_headers.Count; i++)
				{
					ArrayList datalist = m_dataHash[m_headers[i]] as ArrayList;
					if (datalist.Count > currentLineNumber)
					{
						dataLeft = true;
						line += datalist[currentLineNumber].ToString() + m_delimiter.ToString();
					}
					else
					{
						line += m_delimiter.ToString();
					}
				}
				
				currentLineNumber++;
				if (line != string.Empty && dataLeft == true)
				{
					line = line.Substring(0,line.Length - 1);
					fs.WriteLine(line);
				}
			}
			fs.Close();
		}

		/// <summary>
		/// Reads the data with headers in row format.
		/// </summary>
		private void WriteRows(string path, bool append)
		{
			StreamWriter fs = new StreamWriter(path, append);
			string line = string.Empty;

			WriteLines(fs, m_linesBeforeHeader);

			/// 
			/// Read the file
			/// 
			foreach(string header in m_headers)
			{		
				
				/// 
				/// Create the header to data hash
				/// 
				ArrayList datalist = m_dataHash[header] as ArrayList;
				line = header + m_delimiter.ToString();
				/// 
				/// Now add the data to the header value
				/// 	
				foreach(object data in datalist)
				{
					line += data.ToString() + m_delimiter.ToString();
				}
				line = line.TrimEnd(m_delimiter);
				fs.WriteLine(line);
			}
			/// 
			/// cleanup
			/// 
			fs.Close();			
		}

		/// <summary>
		/// Reads lines number of text from the stream fs.
		/// </summary>
		/// <param name="fs">Stream to read from</param>
		/// <param name="lines">number of lines to read</param>
		/// <returns>lines read as a string</returns>
		private void WriteLines(StreamWriter fs, int lines)
		{
			if (fs == null)
				return;
						
			for(int i = 0; i < lines; i++)
			{
				fs.WriteLine();
			}
		}
	}
}
