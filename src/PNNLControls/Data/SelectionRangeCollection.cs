using System;
using System.Collections ; 
using System.Windows.Forms ; 

namespace PNNLControls
{
	public enum ActionType { INSERT=1, DELETE, CUT, COPY, PASTE, EDIT } ;
	public enum OrientationType { LEFT =1, RIGHT = 2, TOP = 4, BOTTOM = 8} ; 
	/// <summary>
	/// 
	/// </summary>
	public class SelectionRangeCollection
	{
  
		private ArrayList marr_selection_range ; 
		private int mint_hover_row ; 
		private int mint_hover_column ; 
		int mint_last_row ; 
		int mint_last_column ; 
		private int mint_num_total_columns ; // total number of columns in table.

		public SelectionRangeCollection()
		{
			// 
			// TODO: Add constructor logic here
			//
			marr_selection_range = new ArrayList ()  ; 
			mint_last_row = -1 ; 
			mint_last_column = -1 ;
			mint_hover_row = -1 ; 
			mint_hover_column = -1 ; 
			mint_num_total_columns = -1 ; 
		}

		public int NumColumns
		{
			get
			{
				return this.mint_num_total_columns ; 
			}
			set
			{
				this.mint_num_total_columns = value ; 
			}
		}

		public int HoverRow
		{
			get { return this.mint_hover_row ; }
			set 
			{ 
				this.mint_hover_row = value ; 
				this.mint_hover_column = -1 ; 
			}
		}

		public int HoverColumn
		{
			get { return this.mint_hover_column ; }
			set 
			{ 
				this.mint_hover_column = value ;
				this.mint_hover_row = -1 ; 
			}
		}

		public ArrayList Ranges
		{
			get
			{
				return this.marr_selection_range ; 
			}
		}

		public bool IsHoveredRow(int row)
		{
			if (row == mint_hover_row)
				return true ;
			return false ;
		}
		public bool IsHoveredColumn(int column)
		{
			if (column == mint_hover_column)
				return true ;
			return false ;
		}

		public int SelectedCellOrientation(int rowNum, int colNum)
		{
			int index = ParentRangeIndex(rowNum, colNum) ; 
			if (index == -1)
				return (int)OrientationType.LEFT ; 
			Range range = (Range) this.marr_selection_range[index] ;

			int orientation = 0 ; 

			if (range.StartColumn == colNum )
				orientation |= (int)OrientationType.LEFT ; 
			if (range.EndColumn == colNum || (range.IsRowSelection && colNum == NumColumns-1))
				orientation |= (int)OrientationType.RIGHT ;
 
			if (range.StartRow == rowNum)
				orientation |= (int)OrientationType.TOP ; 
			if (range.EndRow == rowNum)
				orientation |= (int)OrientationType.BOTTOM ;

			return orientation ; 

		}
		public bool IsSelected(int row, int column)
		{
			if (ParentRangeIndex(row, column) != -1)
				return true ;
			else 
				return false ; 
		}

		public void UnSelectRow(int row)
		{
			for (int i = 0 ; i < marr_selection_range.Count ; i++)
			{
				Range range = (Range) this.marr_selection_range[i] ;
				if (range.IsRowSelection)
				{
					// if range is single person with selection, remove this selection. 
					if (range.StartRow == range.EndRow)
					{
						this.marr_selection_range.Remove(range) ; 
						return ; 
					}
					else if (range.StartRow == row)
					{
						// if range is the first row, remove it.
						range.StartRow = range.StartRow + 1 ; 
					}
					else if (range.EndRow == row)
					{
						// if range is the first row, remove it.
						range.StartRow = range.EndRow - 1 ; 
					}
					else
					{
						// split this row selection into two.
						Range range1 = new Range(range.StartRow, range.StartColumn, row-1, range.EndColumn) ; 
						Range range2 = new Range(row+1, range.StartColumn, range.EndRow, range.EndColumn) ; 
						this.marr_selection_range.Remove(range) ; 
						this.marr_selection_range.Add(range1) ; 
						this.marr_selection_range.Add(range2) ; 
					}
				}
			}
		}

		// returns index of the first Range that this (row, column) belongs to
		// or -1 if it does not belong to any.
		private int ParentRangeIndex(int row, int column)
		{
			for (int i = 0 ; i < marr_selection_range.Count ; i++)
			{
				Range range = (Range) this.marr_selection_range[i] ;
				if (range.IsElementOf(row, column))
					return i ; 
			}
			return -1 ; 
		}

		public void MouseHoverRow(int row)
		{
			this.mint_hover_column = -1 ;
			this.mint_hover_row = row ; 
		}

		public void MouseHoverColumn(int column)
		{
			this.mint_hover_row = -1 ; 
			this.mint_hover_column = column ; 
		}

		public void AddActionMouseMove(int row, int column, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (this.IsLastClickedElement(row, column))
					return ; 
				Range range ; 
				if (this.mint_last_row == -1)
				{
					mint_last_row = row ; 
					mint_last_column = column ; 
					range = new Range(mint_last_row, mint_last_column) ; 
					marr_selection_range.Add(range) ; 
				}
				else
				{
					marr_selection_range.RemoveAt(marr_selection_range.Count-1) ; 
					range = new Range(mint_last_row, mint_last_column, row, column) ; 
					marr_selection_range.Add(range) ; 
				}
			}
		}

		public void AddActionClick(int row, int column)
		{

			Range range ; 
			bool shift = ((Control.ModifierKeys & Keys.Shift) != 0) ; 
			bool ctrl = ((Control.ModifierKeys & Keys.Control) != 0) ;


			if(shift && !ctrl)
			{
				// (if this element is already selected, ignore this).
				//        if (IsSelected(row, column))
				//          return ;

				// if the shift is down, then the last range is modified.
				// The range is from  (mint_last_row, mint_last column) to the current (row,column).
				// The last row, last column remain the last row, column.

				// If there was no selection thus far (first mouse down event)
				// set the mint_last_row, mint_last_column
				if (this.mint_last_row == -1)
				{
					mint_last_row = row ; 
					mint_last_column = column ; 
					range = new Range(mint_last_row, mint_last_column) ; 
					marr_selection_range.Add(range) ; 
				}
				else
				{
					marr_selection_range.RemoveAt(marr_selection_range.Count-1) ; 
					range = new Range(mint_last_row, mint_last_column, row, column) ; 
					marr_selection_range.Add(range) ; 
				}
				return ; 
			} 
			else if(ctrl && !shift) 
			{
				// (if this element is already selected, ignore this).
				if (IsSelected(row, column))
					return ;

				// if the control button is down, we create a new range of one element and 
				// add it to the ones
				mint_last_row = row ; 
				mint_last_column = column ; 
				range = new Range(mint_last_row, mint_last_column) ; 
				marr_selection_range.Add(range) ; 
				return ; 
			} 

			// if the control button and the shift buttons are not down, or are both down,
			// clear all selection ranges and create new one. 
			marr_selection_range.Clear() ; 
			range = new Range(row, column) ; 
			marr_selection_range.Add(range) ; 
			this.mint_last_column = column ; 
			this.mint_last_row = row ; 
		}

		public void IsActionValid(int row, int column, ActionType action)
		{
		}

		public int LastClickedRow
		{
			get
			{
				return this.mint_last_row ;
			}
		}
		public int LastClickedColumn
		{
			get
			{
				return this.mint_last_column ;
			}
		}    

		public bool IsLastClickedElement(int row, int column)
		{
			return (row == mint_last_row) && (column == mint_last_column) ; 
		}

		public Range LastRange
		{
			get
			{
				if (this.marr_selection_range.Count == 0)
					return null ; 
				return (Range) this.marr_selection_range[this.marr_selection_range.Count-1] ;
			}
		}
		public Range TotalSelectionRange
		{
			get
			{
				if (this.marr_selection_range.Count == 0)
					return null ; 

				// first lets go through the ranges and see if this is valid.
				// for this each range should have the same number of columns.
				int start_column = ((Range) marr_selection_range[0]).StartColumn ; 
				int end_column = ((Range) marr_selection_range[0]).EndColumn ; 
				int num_rows = ((Range) marr_selection_range[0]).NumRows ; 

				Range range = new Range(0, start_column, num_rows-1 , end_column) ; 

				for (int i = 1 ; i < this.marr_selection_range.Count ; i++)
				{
					Range current_range = (Range)this.marr_selection_range[i] ; 
					Range last_range = (Range) this.marr_selection_range[i-1] ; 
					if (current_range.StartColumn != last_range.StartColumn || current_range.EndColumn != last_range.EndColumn)
						return null; 
					range.EndColumn = current_range.EndColumn ;
					num_rows += current_range.NumRows ; 
					range.EndRow = num_rows -1 ; 
				}
				// if we are here, this selection is valid.
				return range ; 
			}

		}
	}
}
