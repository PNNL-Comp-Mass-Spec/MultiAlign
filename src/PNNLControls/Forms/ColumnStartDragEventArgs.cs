using System;

namespace ECsoft.Windows.Forms
{
	public delegate void ColumnStartDragEventHandler(object sender, ColumnStartDragEventArgs e);
	
	public class ColumnStartDragEventArgs : EventArgs
	{
		private int		_ColumnIndex;
		private string	_ColumnName;
		private bool	_Cancel;
		
		public ColumnStartDragEventArgs(int columnIndex, string columnName, bool cancel)
		{	
			_ColumnIndex	= columnIndex;
			_ColumnName		= columnName;
			_Cancel			= cancel;
		}

		public int ColumnIndex
		{	
			get { return _ColumnIndex; }
		}
		
		public string ColumnName
		{
			get { return _ColumnName; }
		}
		
		public bool Cancel
		{
			get { return _Cancel; }
			set { _Cancel = value; }
		}
	}
}