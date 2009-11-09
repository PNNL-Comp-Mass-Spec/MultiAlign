using System;
using System.Collections;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsDataTableBase.
	/// </summary>
	public class clsDataTableBase: IDisposable
	{
		protected ArrayList m_headers;
		protected Hashtable m_dataHash; 

		public clsDataTableBase()
		{
			m_headers = new ArrayList();
			m_dataHash = new Hashtable();	
		}
		
		public virtual void Clear()
		{
			m_headers.Clear();
			m_dataHash.Clear();
		}
	
		public ArrayList Headers
		{
			get
			{
				return m_headers;
			}
			set
			{
				m_headers = value;
			}
		}

		public Hashtable Data
		{
			get
			{
				return m_dataHash;
			}
			set
			{
				m_dataHash = value;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			m_headers.Clear();
			m_dataHash.Clear();
		}

		#endregion
	}
}
