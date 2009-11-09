using System;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions; 
using System.Reflection;


namespace ECsoft.Windows.Forms
{
	/// <summary>
	/// Extends the existing DataGrid control to provide the following functions: 
	/// 1)	Drag and drop support to reorder the columns
	/// </summary>
	/// 
	public class DataGridEx : DataGrid
	{
		#region Event Handler
		
		public event ColumnStartDragEventHandler ColumnStartDrag;
		public event ColumnSequenceChangingEventHandler ColumnSequenceChanging;
		public event ColumnSequenceChangedEventHandler ColumnSequenceChanged;
		
		#endregion
		
		#region Constants
		private const int ColumnHeaderHeightOffset = 8;
		private const int CaptionHeightOffset		= 6;
		private const int NODROPIMAGE_WIDTH			= 30;
		#endregion
		
		#region Member Variables
		
		#region Drag and Drop to reorder columns
		private bool allowColumnsReorder;				// Enable/Disable Column Reordering
		private int dragColumnIndex;					// Drag column's index
		private DataGridColumnStyle dragColumnStyle;	// Drag column's style
		private bool isColumnDragging;					// Is column dragging
		private Point mouseDelta;						// Mouse Delta from dragged point
		private Graphics displayGraphics;				// Screen Graphics
		private IntPtr pDisplayDC;						// Handle of Screen Graphics
		private Graphics bufferGraphics;				// Bitmap Graphics
		private IntPtr pBufferDC;						// Handle of Bitmap Graphics
		private Bitmap buffer;							// In-memory buffer
		private Point bufferPos;						// Position for image in the buffer
		private Size dragImageSize;						// Actual size of the dragged image
        
		#endregion
		
		#endregion
		
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the DataGridEx.
		/// </summary>
		public DataGridEx() 
		{	
			allowColumnsReorder = false;
			isColumnDragging	= false;
			dragColumnIndex		= -1;
			dragColumnStyle		= null;

			
			DataGridTableStyle myGridStyle = new DataGridTableStyle();
//			myGridStyle.MappingName = TableName;

			//TableStyles.Add(DataGridTableStyle.DefaultTableStyle.);
			TableStyles.Add(myGridStyle);
		}

		#endregion
		
		#region Properties
		
		[Browsable(false)]
		[Category("Layout")]
		[Description("Gets the height of row headers in the DataGridEx.")]
		public int ColumnHeaderHeight
		{
			get
			{
				Graphics g = Graphics.FromHwnd(Handle);
				SizeF size = g.MeasureString(Name, HeaderFont);
					
				return size.ToSize().Height + ColumnHeaderHeightOffset;
			}
		}
		
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Enables or Disables the drag and drop support to reorder the columns.")]
		public bool AllowUserToOrderColumns
		{
			get	{ return allowColumnsReorder; }
			set	{ allowColumnsReorder = value; }
		}

		#endregion
		
		#region Mouse Movement Management
		
		#region OnMouseDown

        protected void EnableSorting(bool status)
        {
            foreach (DataGridTableStyle style in TableStyles)
            {
                style.AllowSorting = status;
            }
        }

		
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			try
            {

                DataGrid.HitTestInfo hti = HitTest(e.X, e.Y);
				
				/* 
				 * The following conditions must be met in order to start of the drag operation
				 * 1) Column Header is clicked with left mouse button.
				 * 2) AllowUserToOrderColumns property has been set to true
				 * 
				 * */
				if (
					(hti.Type == DataGrid.HitTestType.ColumnHeader)
					&& ((e.Button & MouseButtons.Left) == MouseButtons.Left)
					&& (allowColumnsReorder))
				{                    

					//	Notify application that the drag operation is about to start
					ColumnStartDragEventArgs csde = new ColumnStartDragEventArgs(hti.Column, GetColumnName(hti.Column), false);
					OnColumnStartDrag(csde);

                    if (!csde.Cancel)	//	This column is drag-able
                    {
                        //	Remember the column header clicked
                        dragColumnIndex = hti.Column;
                        isColumnDragging = true;


                        //	Calculate the mouse delta from the click point
                        mouseDelta = new Point
                            (
                            e.X - GetColumnOffset(hti.Column),
                            (CaptionVisible == true ? GetCaptionHeight() : 0)
                            );

                        Point dragColumn = PointToScreen(new Point(e.X, mouseDelta.Y));
                        Point screenPoint = PointToScreen(new Point(e.X, e.Y));
                        screenPoint.X -= mouseDelta.X;
                        screenPoint.Y = dragColumn.Y;

                        //	Retrieve the DataGridColumnStyle for the drag column
                        dragColumnStyle = GetColumnStyle(hti.Column);

                        //	Create the screen graphic device
                        CreateDisplayDC();

                        Graphics tempGraphics = Graphics.FromHwnd(this.Handle);
                        //	Determine the actual size of the dragged image						
                        System.Drawing.Font font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F,
                            System.Drawing.FontStyle.Regular,
                            System.Drawing.GraphicsUnit.Point,
                            ((System.Byte)(0)));
                        SizeF sizeString = tempGraphics.MeasureString(dragColumnStyle.HeaderText, font);
                        int width = Convert.ToInt32(sizeString.Width);


                        //	Determine and create the image buffer.
                        buffer = new Bitmap(Math.Max(width, NODROPIMAGE_WIDTH + 2) + 100, ColumnHeaderHeight + 100);
                        bufferGraphics = Graphics.FromImage(buffer);
                        pBufferDC = bufferGraphics.GetHdc();


                        dragImageSize = new Size(width, ColumnHeaderHeight + 10);//dragColumnStyle.Width, ColumnHeaderHeight + 10);

                        //	Copy the rectangluar image from the screen into buffer.
                        WinGDI.BitBlt(pBufferDC, 0, 0, buffer.Width, buffer.Height, pDisplayDC, screenPoint.X, screenPoint.Y, WinGDI.SRCCOPY);

                        //	Store the image buffer position for tracking
                        bufferPos = new Point(screenPoint.X, screenPoint.Y);
                    }
				}
			}
			catch{ /* Ignore */ }
			finally
			{
				base.OnMouseDown(e);
			}
		}

		#endregion
		
		#region OnMouseMove

		protected override void OnMouseMove(MouseEventArgs e)
		{	
			try
			{	
                
                    
				DataGrid.HitTestInfo hti = HitTest(e.X, e.Y);

				
				/* 
				 * The following conditions must be met before performing the drawing of drag operation
				 * 1) AllowUserToOrderColumns property has been set to true
				 * 2) Column is being dragged with left mouse button
				 * 
				 * */
				if (
					(allowColumnsReorder)
					&& (isColumnDragging)
					&& ((e.Button & MouseButtons.Left) == MouseButtons.Left))
				{	
					if (hti.Column >= 0)
                    {

						//	Re-calculate the mouse location in the screen
						Point screenPoint = PointToScreen(new Point(e.X, e.Y));
						screenPoint.X -= mouseDelta.X;
						screenPoint.Y  = bufferPos.Y;
						
						//	Performs the image swapping
						WinGDI.BitBlt(pDisplayDC, bufferPos.X, bufferPos.Y, buffer.Width, buffer.Height, pBufferDC, 0, 0, WinGDI.SRCCOPY);
						WinGDI.BitBlt(pBufferDC, 0, 0, buffer.Width, buffer.Height, pDisplayDC, screenPoint.X, screenPoint.Y, WinGDI.SRCCOPY);
						bufferPos = new Point(screenPoint.X, screenPoint.Y);

						/*
						 * Notify application that the dragged column is dragging over 
						 * to another column and determines whether the dragged column
						 * can be dropped onto it.
						 * 
						 * */
						ColumnSequenceChangingEventArgs csce = new ColumnSequenceChangingEventArgs(dragColumnIndex, GetColumnName(dragColumnIndex), hti.Column, GetColumnName(hti.Column), false); 
						OnColumnSequenceChanging(csce);
						
						if (csce.Cancel)	//	Drop on this column is not allow
							DrawDragImage(displayGraphics, screenPoint.X, screenPoint.Y, false);
						else
							DrawDragImage(displayGraphics, screenPoint.X, screenPoint.Y, true);
					}
				}
			}
			catch(Exception) { /* Ignore */ }
			finally
			{
				base.OnMouseMove(e);
			}
		}

		#endregion

		#region OnMouseUp

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			try
			{                
                
				DataGrid.HitTestInfo hti = HitTest(e.X, e.Y);
                if (
                    (allowColumnsReorder)
                    && (isColumnDragging)
                    && ((e.Button & MouseButtons.Left) == MouseButtons.Left))
                {
                    //	Clean up the drawn image from the screen
                    WinGDI.BitBlt(pDisplayDC, bufferPos.X, bufferPos.Y, buffer.Width, buffer.Height, pBufferDC, 0, 0, WinGDI.SRCCOPY);

                    /* 
                     * The following conditions must be met before moving the drag column
                     * 1) Drop column is within bound
                     * 2) Source Column <> Destination Column
                     * */
                    if (hti.Column >= 0 && dragColumnIndex != hti.Column)
                    {
                        /*
                         * Notify application that the dragged column is about to drop onto 
                         * another column.
                         * */
                        ColumnSequenceChangingEventArgs columnChanging = new ColumnSequenceChangingEventArgs(dragColumnIndex, GetColumnName(dragColumnIndex), hti.Column, GetColumnName(hti.Column), false);
                        OnColumnSequenceChanging(columnChanging);

                        if (!columnChanging.Cancel)	//	Drop on this column is allowed
                        {
                            //	Move the column
                            MoveColumn(dragColumnIndex, hti.Column);

                            //	Notify application that the dragged column is reordered to new location
                            ColumnSequenceChangedEventArgs columnChanged = new ColumnSequenceChangedEventArgs(columnChanging.SourceColumnIndex, columnChanging.SourceColumnName, columnChanging.DestinationColumnIndex, columnChanging.DestinationColumnName);
                            OnColumnSequenceChanged(columnChanged);
                        }
                    }

                    //	House Keeping
                    isColumnDragging    = false;                    
                    dragColumnIndex     = -1;
                    dragColumnStyle     = null;                 
                    DisposeDisplayDC();
                    DisposeBufferDC();
                }
			}
			catch(Exception) { /* Ignore */ }
			finally
			{                               
				base.OnMouseUp(e);
            }
		}

		#endregion

		#endregion

		#region Event Delegation Management
		
		#region OnColumnSequenceChanged

		protected virtual void OnColumnSequenceChanged(ColumnSequenceChangedEventArgs e)
		{
			if (ColumnSequenceChanged != null)
				ColumnSequenceChanged(this, e);
		}

		#endregion
		
		#region OnColumnSequenceChanging

		protected virtual void OnColumnSequenceChanging(ColumnSequenceChangingEventArgs e)
		{
			if (ColumnSequenceChanging != null)
				ColumnSequenceChanging(this, e);
		}

		#endregion
		
		#region OnColumnStartDrag

		protected void OnColumnStartDrag(ColumnStartDragEventArgs e)
		{
			if (ColumnStartDrag != null)
				ColumnStartDrag(this, e);
		}

		#endregion
		
		#endregion
		
		#region Helper Functions
		
		#region GetColumnName
		
		/// <summary>
		/// Get the column name based on the column index
		/// </summary>
		/// <param name="columnIndex">Column Index</param>
		/// <returns>Column Name</returns>
		private string GetColumnName(int columnIndex)
		{
			DataGridTableStyle ts = TableStyles[0];

			if ((columnIndex < 0) || (columnIndex > ts.GridColumnStyles.Count))
				throw new ArgumentOutOfRangeException("columnIndex");
			return ts.GridColumnStyles[columnIndex].MappingName;
		}

		#endregion
		
		#region GetColumnWidth
		
		/// <summary>
		/// Get the column width based on the column index
		/// </summary>
		/// <param name="columnIndex">Column Index</param>
		/// <returns>Column Width</returns>
		private int GetColumnWidth(int columnIndex)
		{
			DataGridTableStyle ts = TableStyles[0];
			
			if ((columnIndex < 0) || (columnIndex > ts.GridColumnStyles.Count))
				throw new ArgumentOutOfRangeException("columnIndex");
			
			return ts.GridColumnStyles[columnIndex].Width;
		}
		
		/// <summary>
		/// Get the column width based on the column name
		/// </summary>
		/// <param name="columnName">Column Name</param>
		/// <returns>Column Width</returns>
		private int GetColumnWidth(string columnName)
		{
			DataGridTableStyle ts = TableStyles[0];
			
			if (!ts.GridColumnStyles.Contains(columnName))
				throw new ArgumentException(columnName, "Column Name - " + columnName + " does not exists."); 
			return ts.GridColumnStyles[columnName].Width;
		}

		#endregion
		
		#region GetColumnStyle
		
		/// <summary>
		/// Get the DataGridColumnStyle based on the column index
		/// </summary>
		/// <param name="columnIndex">Column Index</param>
		/// <returns>DataGridColumnStyle</returns>
		private DataGridColumnStyle GetColumnStyle(int columnIndex)
		{
			DataGridTableStyle ts = TableStyles[0];
						
			if ((columnIndex < 0) || (columnIndex > ts.GridColumnStyles.Count))
				throw new ArgumentOutOfRangeException("columnIndex");
			
			return ts.GridColumnStyles[columnIndex];
		}
		
		/// <summary>
		/// Get the DataGridColumnStyle based on the column name
		/// </summary>
		/// <param name="columnName">Column Name</param>
		/// <returns>DataGridColumnStyle</returns>
		private DataGridColumnStyle GetColumnStyle(string columnName)
		{
			DataGridTableStyle ts = TableStyles[0];
			
			if (!ts.GridColumnStyles.Contains(columnName))
				throw new ArgumentException(columnName, "Column Name - " + columnName + " does not exists."); 
			return ts.GridColumnStyles[columnName];
		}

		#endregion
		
		#region MoveColumn
		
		/// <summary>
		///	Move the drag column from one location to another
		/// </summary>
		/// <param name="fromColumn">Source Column Index</param>
		/// <param name="toColumn">Destination Column Index</param>
		public void MoveColumn(int fromColumn, int toColumn) 
		{ 
			if(fromColumn == toColumn) return;
			
			DataGridTableStyle oldTS = TableStyles[0];
			DataGridTableStyle newTS = new DataGridTableStyle();
			newTS.MappingName = oldTS.MappingName;
			CopyTableStyle(oldTS, newTS);	// Copy the old TableStyle to new TableStyle
			
			for(int i = 0; i < oldTS.GridColumnStyles.Count; i++) 
			{
				if(i != fromColumn && fromColumn < toColumn)
					newTS.GridColumnStyles.Add(oldTS.GridColumnStyles[i]); 
			
				if(i == toColumn) 
					newTS.GridColumnStyles.Add(oldTS.GridColumnStyles[fromColumn]); 
			
				if(i != fromColumn && fromColumn > toColumn) 
					newTS.GridColumnStyles.Add(oldTS.GridColumnStyles[i]);      
			} 
			TableStyles.Remove(oldTS); 
			TableStyles.Add(newTS); 
		}
		#endregion
		
		#region CopyTableStyle
		
		/// <summary>
		/// Copy the display-related properties of the given DataGridTableStyle
		/// </summary>
		/// <param name="oldTS">From TableStyle</param>
		/// <param name="newTS">To TableStyle</param>
		private void CopyTableStyle(DataGridTableStyle oldTS, DataGridTableStyle newTS)
		{
			newTS.AllowSorting = oldTS.AllowSorting;
			newTS.AlternatingBackColor = oldTS.AlternatingBackColor;
			newTS.BackColor = oldTS.BackColor;
			newTS.ColumnHeadersVisible = oldTS.ColumnHeadersVisible;
			newTS.ForeColor = oldTS.ForeColor;
			newTS.GridLineColor = oldTS.GridLineColor;
			newTS.GridLineStyle = oldTS.GridLineStyle;
			newTS.HeaderBackColor = oldTS.HeaderBackColor;
			newTS.HeaderFont = oldTS.HeaderFont;
			newTS.HeaderForeColor = oldTS.HeaderForeColor;
			newTS.LinkColor = oldTS.LinkColor;
			newTS.PreferredColumnWidth = oldTS.PreferredColumnWidth;
			newTS.PreferredRowHeight = oldTS.PreferredRowHeight;
			newTS.ReadOnly = oldTS.ReadOnly;
			newTS.RowHeadersVisible = oldTS.RowHeadersVisible;
			newTS.RowHeaderWidth = oldTS.RowHeaderWidth;
			newTS.SelectionBackColor = oldTS.SelectionBackColor;
			newTS.SelectionForeColor = oldTS.SelectionForeColor;
		}

		#endregion
		
		#region GetCaptionHeight
		
		/// <summary>
		/// Get the DataGrid control caption's height
		/// </summary>
		/// <returns>Caption Height</returns>
		private int GetCaptionHeight()
		{
			try
			{
				Graphics g = Graphics.FromHwnd(Handle);
				SizeF sizeF = g.MeasureString(CaptionText, CaptionFont);
				return sizeF.ToSize().Height + CaptionHeightOffset;
			}
			catch(SecurityException) 
			{
				return 0;
			}
		}

		#endregion
		
		#region CreateDisplayDC
		
		/// <summary>
		/// Create the screen graphics device
		/// </summary>
		private void CreateDisplayDC()
		{			
			pDisplayDC = WinGDI.CreateDC("DISPLAY", null, null, (System.IntPtr)null);
			displayGraphics = Graphics.FromHdc(pDisplayDC);
		}
		#endregion
		
		#region DisposeDisplayDC

		/// <summary>
		/// Dispose the screen graphics device
		/// </summary>
		private void DisposeDisplayDC()
		{
			displayGraphics.Dispose();
			WinGDI.DeleteDC(pDisplayDC);
		}
		#endregion
		
		#region DisposeBufferDC

		/// <summary>
		/// Dispose the in-memory graphics device
		/// </summary>
		private void DisposeBufferDC()
		{
			bufferGraphics.ReleaseHdc(pBufferDC);
			bufferGraphics.Dispose();

			buffer.Dispose();
			buffer = null;
		}
		#endregion
		
		#region DrawDragImage

		/// <summary>
		/// 
		/// </summary>
		/// <param name="g">Graphics Device</param>
		/// <param name="x">X coordinate in the screen</param>
		/// <param name="y">Y coordinate in the screen</param>
		/// <param name="bEnabled">If false, an X icon will be drawn in the semi-transparent drag column</param>
		private void DrawDragImage(Graphics g, int x, int y, bool bEnabled)
		{
			try
			{	
				g.SmoothingMode = SmoothingMode.AntiAlias;	// Anti-aliased rendering
				
				Rectangle rect = new Rectangle(x, y, dragImageSize.Width, dragImageSize.Height);
				using(Pen p = new Pen(Color.Black))
				{	
					//	Semi-Transparent
					using(Brush backBrush = new SolidBrush(Color.Navy))//.FromArgb(127, Color.White)))
					{
						using(Brush foreBrush = new SolidBrush(Color.White))
						{
							g.FillRectangle(backBrush, rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2);
							g.DrawRectangle(p, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
							
							using(StringFormat sf = new StringFormat(StringFormatFlags.NoWrap))
							{
								sf.LineAlignment = StringAlignment.Center;

								if (dragColumnStyle.Alignment == HorizontalAlignment.Center)
									sf.Alignment = StringAlignment.Center;
								else if (dragColumnStyle.Alignment == HorizontalAlignment.Left)
									sf.Alignment = StringAlignment.Near;
								else if (dragColumnStyle.Alignment == HorizontalAlignment.Right)
									sf.Alignment = StringAlignment.Far;
															
								g.DrawString(dragColumnStyle.HeaderText, new Font(HeaderFont, FontStyle.Bold), foreBrush, rect, sf);
							}
							
							//	Draw an X icon in the semi-transparent drag column
							if (!bEnabled)
							{
								using(Pen redPen = new Pen(new SolidBrush(Color.Red), 3))
								{
									//	Note: Those offset applied here is meant to avoid drawing the X icon outside
									//	of the bound rectangle.
									Point p1; Point p2; // X
									Point p3; Point p4;	// Y
									if (dragImageSize.Width > NODROPIMAGE_WIDTH)
									{
										int left = rect.Left + ((dragImageSize.Width - NODROPIMAGE_WIDTH) / 2);
										p1 = new Point(left, rect.Top + 1);
										p2 = new Point(left + NODROPIMAGE_WIDTH, rect.Top + buffer.Height - 2);
										
										p3 = new Point(left + NODROPIMAGE_WIDTH, rect.Top + 1);
										p4 = new Point(left, rect.Top + buffer.Height - 2);
									}
									else
									{
										p1 = new Point(rect.Left + 1, rect.Top + 1);
										p2 = new Point(rect.Left + NODROPIMAGE_WIDTH - 1, rect.Top + buffer.Height - 2);
										
										p3 = new Point(rect.Left + NODROPIMAGE_WIDTH - 1, rect.Top + 1);
										p4 = new Point(rect.Left + 1, rect.Top + buffer.Height - 2);
									}
									redPen.StartCap = LineCap.Round;
									redPen.EndCap   = LineCap.Round;
									
									g.DrawLine(redPen, p1, p2);
									g.DrawLine(redPen, p3, p4);
								}
							}
						}	
					}
				}
			}
			catch(SecurityException) {}
		}
		#endregion
		
		#region GetColumnOffset

		/// <summary>
		/// Get the X offset location of the specified column
		/// </summary>
		/// <param name="columnIndex"></param>
		/// <returns></returns>
		protected int GetColumnOffset(int columnIndex)
		{
			GridColumnStylesCollection columns = TableStyles[0].GridColumnStyles;
			
			if (columnIndex < 0 || columnIndex > columns.Count)
				throw new ArgumentOutOfRangeException("columnIndex");
			
			int offset = (TableStyles[0].RowHeadersVisible ? TableStyles[0].RowHeaderWidth : 0);
			
			for(int i = 0; i < columnIndex; i++)
			{
				if (columns[i].PropertyDescriptor != null)
					offset += columns[i].Width;		
			}
			offset -= HorizScrollBar.Value;

			return offset;
		}
		#endregion
		
		#endregion
	}	
}
