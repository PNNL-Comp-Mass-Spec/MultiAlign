using System;

namespace ECsoft.Windows.Forms
{
	public delegate void ColumnSequenceChangedEventHandler(object sender, ColumnSequenceChangedEventArgs e);
	
	public class ColumnSequenceChangedEventArgs : EventArgs
	{
		private int		_SourceColumnIndex;
		private string	_SourceColumnName;
		private int		_DestinationColumnIndex;
		private string  _DestinationColumnName;
		
		public ColumnSequenceChangedEventArgs(int sourceColumnIndex, string sourceColumnName, int destColumnIndex, string destColumnName)
		{	
			_SourceColumnIndex		= sourceColumnIndex;
			_SourceColumnName		= sourceColumnName;
			_DestinationColumnIndex = destColumnIndex;
			_DestinationColumnName	= destColumnName;
		}

		public int SourceColumnIndex
		{
			get { return _SourceColumnIndex; }
		}

		public string SourceColumnName
		{
			get { return _SourceColumnName; }
		}

		public int DestinationColumnIndex
		{
			get { return _DestinationColumnIndex; }
		}

		public string DestinationColumnName
		{
			get { return _DestinationColumnName; }
		}
	}
}
