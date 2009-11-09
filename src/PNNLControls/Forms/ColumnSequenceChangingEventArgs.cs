using System;

namespace ECsoft.Windows.Forms
{
	public delegate void ColumnSequenceChangingEventHandler(object sender, ColumnSequenceChangingEventArgs e);
	
	public class ColumnSequenceChangingEventArgs : EventArgs
	{
		private int		_SourceColumnIndex;
		private string	_SourceColumnName;
		private int		_DestinationColumnIndex;
		private string  _DestinationColumnName;
		private bool	_Cancel;
		
		public ColumnSequenceChangingEventArgs(int sourceColumnIndex, string sourceColumnName, int destColumnIndex, string destColumnName, bool cancel)
		{	
			_SourceColumnIndex		= sourceColumnIndex;
			_SourceColumnName		= sourceColumnName;
			_DestinationColumnIndex = destColumnIndex;
			_DestinationColumnName	= destColumnName;
			_Cancel					= cancel;
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

		public bool Cancel
		{
			get { return _Cancel; }
			set { _Cancel = value; }
		}
	}
}
