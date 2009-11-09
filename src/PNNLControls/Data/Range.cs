using System;

namespace PNNLControls
{
	/// <summary>
	/// Class to Keep Rectangular Ranges of elements
	/// </summary>
	public class Range
	{
		int mint_start_row ; 
		int mint_end_row ; 
		int mint_start_column ; 
		int mint_end_column ; 
    
		public int StartRow
		{
			get { return mint_start_row ; }
			set { mint_start_row = value ; }
		}
		public int StartColumn
		{
			get { return mint_start_column ; }
			set { mint_start_column = value ; }
		}
		public int EndRow
		{
			get { return mint_end_row ; }
			set { mint_end_row = value ; }
		}
		public int EndColumn
		{
			get { return mint_end_column ; }
			set { mint_end_column = value ; }
		}
		public int NumRows
		{
			get { return mint_end_row - this.mint_start_row + 1  ; }
		}
		public int NumColumns
		{
			get { return mint_end_column - this.mint_start_column + 1  ; }
		}

		public bool IsRowSelection
		{
			get
			{
				return (mint_start_column == mint_end_column && mint_start_column == -1) ; 
			}
		}

		public Range(Range r)
		{
			// 
			// TODO: Add constructor logic here
			//
			mint_start_row = r.StartRow ; 
			mint_start_column = r.StartColumn ; 
			mint_end_row = r.EndRow ; 
			mint_end_column = r.EndColumn ; 

		}

		public Range(int start_row, int start_column)
		{
			// 
			// TODO: Add constructor logic here
			//
			mint_start_row = start_row ; 
			mint_start_column = start_column ; 
			mint_end_row = start_row ; 
			mint_end_column = start_column ; 

		}

		public Range(int start_row, int start_column, int end_row, int end_column)
		{
			// 
			// TODO: Add constructor logic here
			//
			mint_start_row = Math.Min(start_row, end_row)  ;
			mint_end_row = Math.Max(start_row, end_row) ;

			mint_start_column = Math.Min(start_column, end_column)  ;
			mint_end_column = Math.Max(start_column, end_column) ;
		}
 
		public bool IsElementOf(int row, int column)
		{
			if (row >= mint_start_row && row <= mint_end_row && (IsRowSelection
				|| (column >= mint_start_column && column <= mint_end_column)))
				return true ; 
			return false ; 
		}
	}
}
