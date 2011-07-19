using System;
using System.IO;

namespace PNNLProteomics.IO
{
		
	/// <summary>
	/// Summary description for clsReadProgressStream.
	/// http://msdn.microsoft.com/msdnmag/issues/06/12/NETMatters/
	/// </summary>
	public class clsReadProgressStream: classContainerStream
	{
		private int m_lastProgress = 0;

		public delegate void ProgressChangedEventHandler(object o, int progress);
		public event ProgressChangedEventHandler ProgressChanged;
		
		public clsReadProgressStream(Stream stream): base(stream)
		{
			if (!stream.CanRead || !stream.CanSeek || stream.Length <= 0) 
				throw new ArgumentException("stream");
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int amountRead = base.Read(buffer, offset, count);
			if (ProgressChanged != null)
			{
				int newProgress = (int)(Position * 100.0 / Length);
				if (newProgress > m_lastProgress)
				{
					m_lastProgress = newProgress;
					ProgressChanged(this, m_lastProgress);
				}
			}
			return amountRead;
		}
	}
}
