using System.Collections;
using System.IO;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsTextDelimitedFileReader.
	/// </summary>
	public class clsTextDelimitedFileReader: clsTextDelimitedFileBase
	{
		private string m_path;

		public clsTextDelimitedFileReader()
		{

		}
	
		/// <summary>
		/// Reads the file via the specified format.  Columns = true means headers are the first line in column format.
		/// </summary>
		/// <param name="columns">Columns = true if header information is stored in column format.</param>
		public void Read(string path, bool columns)
		{
			m_path = path;
			if (columns == true)
				ReadColumns(path);
			else
				ReadRows(path);
		}

		
		/// <summary>
		/// Reads the file with headers in the rows.
		/// </summary>
		public void Read(string path)
		{
			m_path = path;
			ReadRows(path);
		}

		/// <summary>
		/// Reads the data with headers in column format.
		/// </summary>
		private void ReadColumns(string path)
		{
			StreamReader fs = new StreamReader(path);
			string line = string.Empty;

			Clear();			
			m_text = ReadLines(fs, m_linesBeforeHeader);
			
			/// 
			/// Read the header
			/// 
			m_headerString = fs.ReadLine();
			string [] headers = null;
			char   [] splitChars   = new char[1];
			splitChars[0] = m_delimiter;
				
			if (m_headerString != null)
			{
				m_text += m_headerString;
				headers = m_headerString.Split(splitChars);
				for(int i = 0; i < headers.Length; i++)
				{
					m_headers.Add(headers[i]);
					if (m_dataHash.ContainsKey(headers[i]) == false)
						m_dataHash.Add(headers[i], new ArrayList());
				}
			}

			if (headers == null)
			{
				fs.Close();
				return;
			}

			/// 
			/// Read the file
			/// 
			while( (line = fs.ReadLine()) != null)
			{		
				m_text += line;
				string []data = line.Split(splitChars);
				/// Make sure that we dont go outside of the headers as well.
				for(int i = 0; i < headers.Length && i < data.Length; i++)
				{
					ArrayList datalist = m_dataHash[headers[i]] as ArrayList;
					if (data[i] != string.Empty)
						datalist.Add(data[i]);
				}
			}

			fs.Close();
		}

		/// <summary>
		/// Reads the data with headers in row format.
		/// </summary>
		private void ReadRows(string path)
		{
			StreamReader fs = new StreamReader(path);
			string line = string.Empty;

			Clear();
			m_text = ReadLines(fs, m_linesBeforeHeader);

			char   [] splitChars   = new char[1];
			splitChars[0]	= m_delimiter;
			m_headerString	= string.Empty;

			/// 
			/// Read the file
			/// 
			while( (line = fs.ReadLine()) != null)
			{		
				m_text += line;
				string []data = line.Split(splitChars);

				/// 
				/// Create the header to data hash
				/// 
				ArrayList datalist = new ArrayList();
				if (data.Length > 0)
				{
					m_headers.Add(data[0]);
					if (m_dataHash.ContainsKey(data[0]) == false)
					{					
						m_dataHash.Add(data[0], datalist);							
					}
				}
				/// 
				/// Now add the data to the header value
				/// 	
				for(int i = 1; i < data.Length; i++)
				{
					datalist.Add(data[i]);
				}
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
		private string ReadLines(StreamReader fs, int lines)
		{
			if (fs == null)
				return string.Empty;

			string line    = string.Empty;
			string lineSum = string.Empty;
			
			/// 
			/// Read off the garbage
			/// 
			for(int i = 0; i < lines; i++)
			{
				if ((line = fs.ReadLine()) != null)
				{
					lineSum += line;
				}
			}
			return lineSum;
		}
	}
}
