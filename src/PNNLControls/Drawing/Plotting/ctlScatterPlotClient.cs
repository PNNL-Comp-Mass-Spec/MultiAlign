using Derek;
using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;


using PNNLControls.Diagnostics;

namespace PNNLControls
{
	public enum LinearParameters{SLOPE = 0, INTERCEPT = 1, RSQUARED = 2};
	public enum PlotSelection{NOT_SELECTED, SELECTED}; 

	/// <summary>
	/// Summary description for ctlScatterPlotClient.
	/// </summary>
	public class ctlScatterPlotClient : UserControl
	{			
		#region Event Declarations
		public delegate void PercentageCompleteDelegate(int percent);
		public delegate void DrawingCompleteDelegate();
		public delegate void CalculationCompleteDelegate();
		public delegate void MouseHoverDelegate(int x, int y);
		public delegate void ClickedPlotDelegate(int startRow, int endRow, int startColumn, int endColumn, Point p, bool selected);		
		public event ClickedPlotDelegate OnPlotClicked			= null;
		public event PercentageCompleteDelegate OnPercentDrawn	= null;	
		public event PercentageCompleteDelegate OnPercentCalc	= null;		
		public event DrawingCompleteDelegate OnDrawingComplete	= null;								
		public event CalculationCompleteDelegate OnCalculationComplete = null;		
		public event MouseEventHandler  ScatterPlotMouseWheel = null;

		#endregion

		#region Members
		private const int MAX_PLOTS				= 25;
		private const int ALPHA_HIGHLIGHT		= 140;
		private const int AXIS_SIZE				= 1;	// Axis Line Width/Height from Edge of subplot
		private const int PLOT_SIZE				= 10;	// Plot Point Size	
		private const int THREADING_TIMEOUT		= 1000;	// Timeout for any thread to wait before it can acquire the lock to render.
		private const int NUM_LINEAR_PARAMETERS = 3;					
		private System.ComponentModel.Container components		= null;		
		private Font m_rsquaredFont =  new Font(FontFamily.GenericSerif, 8);		
		private PictureBox displayArea;							// Display area to draw the bitmap to.											
		private ArrayList m_commonPoints		= null;
		private Thread m_renderThread			= null;			// Rendering background thread
		private Thread m_calcThread				= null;			// Calculation background thread
		private Mutex m_lock			= new Mutex(); 
		private Color m_dataColor				= Color.Black;	// Color choice to make subplots
		private Color m_dataColorAlt			= Color.White;	// Alternative color to plot
		private clsPlotRange m_range			= new clsPlotRange();													// Manages bookkeeping for data ranges
		private ctlHeatMapLegend mLegend		= null;				 // For Color					
		protected BitmapTools m_bitmap_tools	= new BitmapTools(); // Unsafe tools for fast drawing of bitmaps for several points.		
		private Bitmap m_bitmap					= null;				 // The drawing area of the chart				
		private Graphics m_graphics				= null;				 // Graphics object used to draw on bitmap during painting phase 						
		private Font m_textFont					= new Font(FontFamily.GenericSansSerif, 8.25f, FontStyle.Regular);
		private int  m_axisPadding				= 3;		
		private int  m_numClusters				= 0;	// Number of Clusters
		private int  m_numDataSets				= 0;	// Number of Datasets
		private int[] m_rowAlignment		= null;	// For Label-Plot Alignment
		private int[] m_columnAlignment		= null;	// For Label-Plot Alignment
		private bool m_autoScaleColor			= true;
		private bool m_recreateBitmaps 			= true;		
		private bool m_autoUsePlotColor			= true;
		private bool m_showScatterData 			= true;
		private bool m_showRSquaredValue		= true;	
		private bool m_showLogData				= false;			
		private bool m_showEquation				= false;		
		private float [,] m_normalData			= null;
		private float [,] m_logData				= null;
		private float [,] m_scatterData			= null;		
		private float [,] m_linearFitNorm		= null;		
		private float [,] m_linearFitLog		= null;
		private float [,] m_linearFit			= null;
		private PlotSelection  [,] m_selectedPlots		= null; 
		private PlotDisplayBounds  m_displayBounds		= null;
		private clsShape m_shape				= new PointShape(PLOT_SIZE, false);
		private bool m_calculationDone = false;

		#endregion

		#region Constructor
		/// <summary>
		/// ScatterPlotClient constructor.  This sets up the event handlers for data and other member initilizations.
		/// </summary>
		public ctlScatterPlotClient()
		{			
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);		
			InitializeComponent();
			
			this.MouseWheel += new MouseEventHandler(ctlScatterPlotClient_MouseWheel);

			/*////////////////////////////////////////////////
			 *	Create new classes for required members.
			/*////////////////////////////////////////////////		
			this.displayArea.BackColor = Color.White;
			Resize				  += new EventHandler(ctlScatterPlotClient_Resize);
			displayArea.Paint	  += new PaintEventHandler(displayArea_Paint);				
			displayArea.MouseDown += new MouseEventHandler(ctlScatterPlotClient_MouseDown); 			
			m_displayBounds = new PlotDisplayBounds(0,0);

			this.displayArea.MouseHover += new EventHandler(displayArea_MouseHover);
		}
		#endregion
		
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.displayArea = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// displayArea
			// 
			this.displayArea.Dock = System.Windows.Forms.DockStyle.Fill;
			this.displayArea.Location = new System.Drawing.Point(0, 0);
			this.displayArea.Name = "displayArea";
			this.displayArea.Size = new System.Drawing.Size(150, 150);
			this.displayArea.TabIndex = 0;
			this.displayArea.TabStop = false;
			// 
			// ctlScatterPlotClient
			// 
			this.Controls.Add(this.displayArea);
			this.Name = "ctlScatterPlotClient";
			this.ResumeLayout(false);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{				
				/*
				 *  Stop the render thread safely.
				 */
				QuitRenderThread();
				
				if (m_bitmap != null)
					m_bitmap.Dispose();
					
				if (m_graphics != null)
					m_graphics.Dispose();
					
				if (mLegend != null)
					mLegend.Dispose();
					
				m_bitmap = null;
				if(components != null)
				{										
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Label / Plot Alignment
		/// <summary>
		/// Horizontal alignment for data sets to be drawn to.  This is to match the drawing to an external label.
		/// </summary>
		public int[] AlignHorizontal
		{
			get 
			{
				return m_columnAlignment;
			}
			set
			{								
				m_columnAlignment = value;
			}
		}

		/// <summary>
		/// Vertical alignment for data sets to be drawn to.  This is to match the drawing to an external label.
		/// </summary>
		public int[] AlignVertical
		{
			get 
			{
				return m_rowAlignment;
			}
			set
			{						
				m_rowAlignment = value;				
			}
		}
	
		#endregion

		#region Plot Properties ... Colors
		/// <summary>
		/// Alternative color to use for data plots when background and foreground colors are equivalent.
		/// </summary>
		public Color DataColorAlternative
		{
			get 
			{
				return m_dataColorAlt;
			}
			set
			{
				m_dataColorAlt = value;
			}
		}

		/// <summary>
		/// Color to be used for plot points.
		/// </summary>
		public Color DataColor
		{
			get 
			{
				return m_dataColor;
			}
			set
			{
				m_dataColor = value;
			}
		}
			

		#endregion

		#region Drawing / Rendering Bitmap
		/// <summary>
		/// 
		/// </summary>
		/// <param name="row"></param>
		/// <param name="selectionValue"></param>
		public void SelectRow(int row, bool selectionValue)
		{
			if (row >= m_displayBounds.StartRow  && row <= m_displayBounds.EndRow)
			{
				for(int i = m_displayBounds.StartColumn; i <= m_displayBounds.EndColumn; i++)
				{
					if (selectionValue && m_selectedPlots[row,i] == PlotSelection.NOT_SELECTED)
					{
						m_selectedPlots[row,i] = PlotSelection.SELECTED;						
						DrawSubPlot(row,i);
					}
					else if(selectionValue == false)
					{
						m_selectedPlots[row,i] = PlotSelection.NOT_SELECTED;						
						DrawSubPlot(row,i);
					}	
				}
				OnDrawingComplete();										
				if (OnPlotClicked != null)
				{									
					OnPlotClicked(row, row, m_displayBounds.StartColumn,m_displayBounds.EndColumn, System.Drawing.Point.Empty, selectionValue);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="column"></param>
		/// <param name="selectionValue"></param>
		public void SelectColumn(int column, bool selectionValue)
		{
			if (column >= m_displayBounds.StartColumn  && column <= m_displayBounds.EndColumn)
			{
				for(int i = m_displayBounds.StartRow; i <= m_displayBounds.EndRow; i++)
				{
					if (selectionValue && m_selectedPlots[i,column] == PlotSelection.NOT_SELECTED)
					{
						m_selectedPlots[i,column] = PlotSelection.SELECTED;						
						DrawSubPlot(i,column);
					}
					else if(selectionValue == false)
					{
						m_selectedPlots[i,column] = PlotSelection.NOT_SELECTED;						
						DrawSubPlot(i,column);
					}
				}
				OnDrawingComplete();										
				if (OnPlotClicked != null)
				{									
					OnPlotClicked(m_displayBounds.StartRow, m_displayBounds.EndRow,column, column, System.Drawing.Point.Empty, selectionValue);
				}
			}
		}
		
		/// <summary>
		/// Draws the control on the given graphics.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="drawFocusRect">If true, the focus rectangle may be drawn, if the control 
		/// is the current input focus and ShowFocus is true.  Otherwise, no focus rectangle 
		/// will be drawn.</param>
		public void OnRefresh() 
		{										
			try 
			{		
				// Make sure that 
				if (QuitRenderThread() == false)
				{					
					Debugger.Log(0,"Log","Threading (Render): Could not stop render thread from rendering.");					
				}
				else
				{																				
					m_renderThread = new Thread(new System.Threading.ThreadStart(WorkerDrawStart));
					m_renderThread.Name = "scatterplotWorkerThread";				
					m_renderThread.Start();						
					while(!m_renderThread.IsAlive);																							
				}																	
			} 
			catch (Exception ex)
			{
				Debugger.Log(10, "Log", ex.ToString());
				//System.console.writeline(ex.Message);
			}
		}
					
		/// <summary>
		/// Returns the internal data rendered as a bitmap.
		/// </summary>
		/// <returns>Full Bitmap of all rendered data including clipped data.</returns>
		public Bitmap ToBitmap() 
		{
			return ToBitmap(false);
		}

		/// <summary>
		/// Returns the internal data rendered as a bitmap.
		/// </summary>
		/// <param name="refresh">Specifies whether to force a render of the current data.</param>
		/// <returns>Full bitmap of all rendered data including clipped data.</returns>
		public Bitmap ToBitmap(bool refresh)
		{
			//System.console.writeline("Retreiving a bitmap by thread: " + Thread.CurrentThread.Name); 
			Lock();			

			Bitmap newImage = null;			
			if (m_bitmap != null)
			{
				newImage = new Bitmap((int) m_bitmap.Width, (int)m_bitmap.Height);	
				m_bitmap_tools.Copy(m_bitmap, newImage);												
			}						
			UnLock();

			return newImage;			
		}

		
		/// <summary>
		/// Required thumbnail abort callback.
		/// </summary>
		/// <returns>false</returns>
		private bool ThumbnailCallback()
		{
			return false;
		}

		/// <summary>
		/// Sets the thumbnail image of the current rendered scatterplot.
		/// </summary>
		/// <param name="targetImage"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Image GetThumbnail(int width, int  height)
		{
			Lock();
			Image targetImage = null;						
			if (m_bitmap != null)
			{
				targetImage =  m_bitmap.GetThumbnailImage(width, height,
					new Image.GetThumbnailImageAbort(ThumbnailCallback),
					IntPtr.Zero);													
			}
			UnLock();				
			return targetImage;
		}

		
		/// <summary>
		/// Creates the bitmaps for display.
		/// </summary>
		/// <param name="size">Size of the bitmaps to create.</param>
		private void CreateBitmaps(Size size) 
		{	
			/* potentially creating a large bitmap...otherwise refrain from calling collection */
			if (m_numDataSets > 10)
				GC.Collect();								

			if (m_bitmap != null) 
				m_bitmap.Dispose();			
			if (m_graphics != null) 			
				m_graphics.Dispose();	
		
			bool canCreateBitmaps = false;
			bool throwException   = false;
			
			int width  = size.Width;
			int height = size.Height;

			if (width <= 0 || height <= 0)
			{
				throw new Exception("Width/Height cannot be negative or zero.");
			}

			/* Attempt to create a large bitmap */
			while (canCreateBitmaps == false)
			{
				try
				{
					m_bitmap	= new Bitmap(width, height, PixelFormat.Format24bppRgb);
					canCreateBitmaps = true;
				}
				catch
				{
					GC.Collect();
					//console.writeline(ex.Message);
					width  /= 2;
					height /= 2;
					throwException = true;

					if (width == 0 || height == 0)
						canCreateBitmaps = true;
				}
			}

			// even if we found some memory to allocate...we need to tell the user to 
			// try a different size.
			if (throwException == true)
			{
				OutOfBitmapMemoryException badMemory = new OutOfBitmapMemoryException();				
				badMemory.Width  = width/2;
				badMemory.Height = height/2;
				m_bitmap.Dispose();
				m_bitmap = null;

				// Clean up all the memory.
				GC.Collect();
				throw badMemory;
			}
			m_recreateBitmaps   = false;			
			m_graphics			= Graphics.FromImage(m_bitmap);				
			m_graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;						
		}

		/// <summary>
		/// Displays the text appropiate for the scatter plot to the graphics context supplied.
		/// </summary>
		/// <param name="g">Graphics object to draw with.</param>
		/// <param name="row">Row to render</param>
		/// <param name="col">Column to render</param>
		/// <param name="width">Width of subplot</param>
		/// <param name="height">Height of subplot</param>
		/// <param name="colour">Color of text</param>
		private void DrawText(Graphics g , int row, int col, int width, int height, Color colour)
		{			
			string displayString = "";
			Rectangle plotBounds = new Rectangle(0, 0, width, height);
			Color color			 = ComputeColorComplement(colour);
			StringFormat sf		 = new StringFormat(StringFormatFlags.NoWrap);
			clsLabelPlotter fontPlotter = new clsLabelPlotter();										
			sf.Alignment				= StringAlignment.Far;											
			fontPlotter.IsVertical	    = false;				
			fontPlotter.LineAlignment	= StringAlignment.Far;
			fontPlotter.MinFontSize = 2.0f;
			fontPlotter.MaxFontSize = 96.0f;

			/* Draw the r-squared value */
			if (m_showRSquaredValue)
			{																		
				displayString = String.Format("{0:0.00}", m_linearFit[row*m_numDataSets + col, (int)LinearParameters.RSQUARED]);
				
				fontPlotter.Label	= displayString;							
				fontPlotter.Bounds	= plotBounds;						
				SizeF sizef			= fontPlotter.GetTextSizeForWidth(g, width);				
			
				float fontSize		= fontPlotter.GetBestFontSize(g);
				if (fontSize > m_textFont.Size)				
					fontSize = m_textFont.Size;
							
				Font font = new Font(m_textFont, m_textFont.Style); 																														
				g.DrawString(displayString, font, new SolidBrush(color), plotBounds, sf);													
				plotBounds.Y += Convert.ToInt32(m_textFont.SizeInPoints) + 2;
			}		

			/* Draw the equation */
			if (m_showEquation)
			{
				displayString = String.Format("y = {0:0.00}x + {1:0.00}",
					m_linearFit[row*m_numDataSets + col, (int)LinearParameters.SLOPE],
					m_linearFit[row*m_numDataSets + col, (int)LinearParameters.INTERCEPT]);				
				fontPlotter.Label	= displayString;							
				fontPlotter.Bounds	= plotBounds;						
				SizeF sizef			= fontPlotter.GetTextSizeForWidth(g, width);				
				float fontSize		= fontPlotter.GetBestFontSize(g);
				if (fontSize > m_textFont.Size)				
					fontSize = m_textFont.Size;
							
				Font font = new Font(m_textFont, m_textFont.Style); 																														
				g.DrawString(displayString, font, new SolidBrush(color), plotBounds, sf);
			}
		}
	
		/// <summary>
		/// Draws the data points to the scatter plot.
		/// </summary>
		/// <param name="bmp"></param>
		/// <param name="scaledWidth"></param>
		/// <param name="scaledHeight"></param>
		/// <param name="xrange_per_pixel"></param>
		/// <param name="yrange_per_pixel"></param>
		/// <param name="colour">Color of the data points</param>
		private void DrawPoints(Bitmap bmp, Graphics g, int row, int col, Color colour_series)
		{			
				if (m_showScatterData == false) return;

				float scaleFactorX = (float)(Math.Abs(m_columnAlignment[0] - m_columnAlignment[1]))/(float)bmp.Width;
				float scaleFactorY = (float)(Math.Abs(m_rowAlignment[0] -  m_rowAlignment[1]))/(float)bmp.Height;		
				int scaledWidth = (int)(bmp.Width*scaleFactorX);
				int scaledHeight = (int)(bmp.Height*scaleFactorY);										
				float xrange = (m_range.mflt_xend - m_range.mflt_xstart);
				float yrange = (m_range.mflt_yend - m_range.mflt_ystart);			
				float xrange_per_pixel = scaledWidth / xrange;
				float yrange_per_pixel = scaledHeight / yrange;
				float xstart = m_range.mflt_xstart;
				float ystart = m_range.mflt_ystart;	
							
				ArrayList points = new ArrayList();	
				float fwidth  = Convert.ToSingle(Width);
				float fdiff   = Convert.ToSingle(Math.Abs(m_columnAlignment[0] - m_columnAlignment[1]));
				float fratio  = 2.0f*fdiff/fwidth;
				float fPoints = (fratio) * Convert.ToSingle(m_numClusters);
				int totalPointsToPlot = Math.Min(m_numClusters, Convert.ToInt32(fPoints));
				for(int i = 0; i < totalPointsToPlot; i++)
				{
					int   k = marr_randomMask[i];
					float x = m_scatterData[k,col];
					float y = m_scatterData[k,row];
					if (float.IsNaN(x) || float.IsNaN(y))
						continue;
									
					/* Find its plot x,y */
					int plotXValue  = (int)((((x - xstart)) * xrange_per_pixel)) + m_axisPadding ;									
					int plotYValue  = (int)((scaledHeight - ((y - ystart)) * yrange_per_pixel)) - m_axisPadding;																													
																		
					/*
					 *  Only draw if we are within the bounds of the bitmap, control to draw into.
					 */							  
					if (plotXValue > m_axisPadding  && plotXValue < scaledWidth && plotYValue > 0 && plotYValue < scaledHeight - m_axisPadding ) 
					{					
						Derek.BitmapTools.PixelData p = new Derek.BitmapTools.PixelData();					
						p.red   = colour_series.R;
						p.green = colour_series.G;
						p.blue  = colour_series.B;							
						points.Add(new ChartDataPlotPoint(plotXValue, plotYValue, p));									
					}	
				}						
				
				m_bitmap_tools.DrawDots(points, m_shape, bmp);																
													
				/*
						*	Paint the lines defining the axis of the data.
						*/														
				Pen pen = new Pen(new SolidBrush(colour_series),AXIS_SIZE);				
				g.DrawLine(pen,
						m_axisPadding,
						m_axisPadding,
						m_axisPadding,
						bmp.Height - m_axisPadding);												
				g.DrawLine(pen,
						m_axisPadding,
						bmp.Height - m_axisPadding,
						bmp.Width  - m_axisPadding,
						bmp.Height - m_axisPadding);
		}

		/// <summary>
		/// Draw a subplot.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		private void DrawSubPlot(int row, int col) 
		{						
				DrawSubPlot(row, col, false);
		}

		/// <summary>
		/// Draw a sub plot.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="drawTranspose"></param>
		private void DrawSubPlot(int row, int col, bool drawTranspose) 
		{						
			int colAligned, rowAligned, colAlignedNext, rowAlignedNext;										
			int colAdjusted = col - m_displayBounds.StartColumn;
			int rowAdjusted = row - m_displayBounds.StartRow;

			if (m_columnAlignment == null || m_rowAlignment == null)
				return;

			colAligned	   = m_columnAlignment[colAdjusted];						
			colAlignedNext = m_columnAlignment[colAdjusted + 1];						
			rowAligned	   = m_rowAlignment[rowAdjusted];												
			rowAlignedNext = m_rowAlignment[rowAdjusted + 1]; 						
										
			Bitmap smallBitmap = new Bitmap(Math.Abs(colAligned     - colAlignedNext - 1),
				Math.Abs(rowAlignedNext - rowAligned)    - 1);						
			Color colour_series;																															
			/*
			* Color the background
			*/
			float rsquaredValue = m_linearFit[row*m_numDataSets + col, (int)LinearParameters.RSQUARED];
			Color colour = Legend.GetColor(rsquaredValue);						
	
			/*
			* Specify a color for the plot data...if color value is out of
			* range then we color it differently.
			*/ 
			colour_series = m_dataColor;														
			if (m_autoUsePlotColor)						
				colour_series = ComputeColorComplement(colour);

			Graphics g = Graphics.FromImage(smallBitmap);
			g.FillRectangle(new SolidBrush(colour),0,0,smallBitmap.Width, smallBitmap.Height);			

			if (NumberOfRows < MAX_PLOTS || NumberOfColumns < MAX_PLOTS)
			{
				DrawPoints(smallBitmap, g, row, col, colour_series);																						
				if (m_showRSquaredValue || m_showEquation)
				{
					DrawText(g, row, col,smallBitmap.Width, smallBitmap.Height, colour);			
				}
			}
			m_graphics.DrawImage(smallBitmap, m_columnAlignment[colAdjusted], m_rowAlignment[rowAdjusted], smallBitmap.Width, smallBitmap.Height);
			if (drawTranspose == true)
			{
				if (m_rowAlignment.Length > colAdjusted && m_columnAlignment.Length > rowAdjusted)
				{
					m_graphics.DrawImage(smallBitmap, m_columnAlignment[rowAdjusted], m_rowAlignment[colAdjusted], smallBitmap.Width, smallBitmap.Height);
				}
			}

			if (m_selectedPlots[row,col] == PlotSelection.SELECTED)				
			{						
				SolidBrush selectedBrush = new SolidBrush(Color.FromArgb(ALPHA_HIGHLIGHT,Color.White));				
				Rectangle  rect = new Rectangle(colAligned, rowAligned, smallBitmap.Width, smallBitmap.Height);
				m_graphics.FillRectangle(selectedBrush, rect);
			}
			smallBitmap.Dispose();																					
		}

		/// <summary>
		/// Ensures the chart area bitmap was created.
		/// </summary>
		private void DrawBitmaps() 
		{										
			/*////////////////////////////////////////////////////////////////////////////////////
			*	Check the boundaries of the alignment arrays.
			/*////////////////////////////////////////////////////////////////////////////////////						
			if (Width <= 0 || Height <= 0)
				return;			
			Lock();
			if (m_recreateBitmaps == true)	
			{
				CreateBitmaps(new Size(Width, Height));				
			}
			int quarter   = Convert.ToInt32(m_numDataSets*m_numDataSets*.25);
			/*/////////////////////////////////////////////////////////////////////
			 *  Paint the data points for each series
			 *		i,j -> grid
			/*/////////////////////////////////////////////////////////////////////	
			HiPerfTimer timer = new HiPerfTimer();
			int numberDrawn   = 0; 
			int numberOfPlots = Math.Abs(m_displayBounds.StartColumn - m_displayBounds.EndColumn)*Math.Abs(m_displayBounds.StartRow - m_displayBounds.EndRow);
			
			timer.Start();
			if ( Math.Abs(m_displayBounds.StartColumn - m_displayBounds.EndColumn) == Math.Abs(m_displayBounds.StartRow - m_displayBounds.EndRow) && m_displayBounds.StartRow == m_displayBounds.StartColumn)
			{
				for(int col = m_displayBounds.StartColumn; col <= m_displayBounds.EndColumn; col++)
				{								
					for(int row = m_displayBounds.StartRow; row <= m_displayBounds.EndRow; row++)
					{		
						DrawSubPlot(row,col, false);																				
						if (row == col)	
						{	
							numberDrawn	+=	1;
						}
						else
						{
							DrawSubPlot(col, row, false);
							numberDrawn	+=	2;
						}
						if (timer.Duration >= .5)
						{
							timer.Stop();
							double percentDrawn = Convert.ToDouble(numberDrawn)/Convert.ToDouble(numberOfPlots);
							percentDrawn		= Math.Max(0,Math.Min(percentDrawn,1.0))*100.0;
							UnLock();
							UpdatePercentDrawn(Convert.ToInt32(percentDrawn));			
							Lock();			
							timer.Start();
						}			
					}																																			 																																											
				}	
			}					
			else
			{																	 
				for(int col = m_displayBounds.StartColumn; col <= m_displayBounds.EndColumn; col++)
				{								
					for(int row = m_displayBounds.StartRow; row <= m_displayBounds.EndRow; row++)
					{										
						DrawSubPlot(row,col, false);						
						numberDrawn++;
						if (timer.Duration >= .5)
						{
							timer.Stop();
							double percentDrawn = Convert.ToDouble(numberDrawn)/Convert.ToDouble(numberOfPlots);
							percentDrawn		= Math.Max(0,Math.Min(percentDrawn,1.0))*100.0;
							UnLock();
							UpdatePercentDrawn(Convert.ToInt32(percentDrawn));			
							Lock();	
							timer.Start();					
						}			
					}																																			 																																											
				}	
			}
			UnLock();						
		}
				
		#endregion

		#region Properties	
	
		/// <summary>
		/// Font of the subplots' text.
		/// </summary>
		public Font TextFont
		{
			get
			{
				return m_textFont;
			}
			set
			{
				m_textFont = value;
			}
		}

		/// <summary>
		/// Displays linear regression equation for each plot on chart.
		/// </summary>
		public bool ShowEquation
		{
			get
			{
				return m_showEquation;
			}
			set
			{
				m_showEquation = value;
			}
		}

		/// <summary>
		/// Displays R-Squared Cross Correlation value on scatterplot for each baseline comparison.
		/// </summary>
		public bool ShowRSquaredValue
		{
			get
			{
				return m_showRSquaredValue;
			}
			set
			{
				m_showRSquaredValue = value;
			}
		}
		
		/// <summary>
		/// Get/Set whether to show scatter point data when rendering.
		/// </summary>
		public bool ShowScatterData
		{
			get
			{
				return m_showScatterData;
			}
			set
			{
				m_showScatterData = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public float[,] LinearFitNormal
		{
			get
			{
				return this.m_linearFitNorm;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public float[,] LinearFitLog
		{
			get
			{
				return this.m_linearFitLog;
			}
		}

		/// <summary>
		/// Gets the number of clusters in a single dataset.
		/// </summary>
		public long NumberOfClusters
		{
			get
			{
				return this.m_numClusters;
			}
		}

		/// <summary>
		/// Gets the number of unique datasets.
		/// </summary>
		public long  NumberOfDatasets
		{
			get
			{
				return m_numDataSets;
			}
		}

		/// <summary>
		/// Legend for the heatmap. 
		/// </summary>
		public ctlHeatMapLegend Legend 
		{
			get{return this.mLegend;}
			set{this.mLegend = value;}
		}
		
		/// <summary>
		/// Gets/Sets the axis padding to use for each plot.
		/// </summary>
		public int AxisPadding
		{
			get
			{
				return m_axisPadding;
			}
			set
			{
				if (value > 0)
					m_axisPadding = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int NumberOfRows
		{
			get
			{
				return m_displayBounds.NumberOfRows;
			}			
		}		
		/// <summary>
		/// 
		/// </summary>
		public int NumberOfColumns
		{
			get
			{
				return m_displayBounds.NumberOfColumns;
			}			
		}
		/// <summary>
		/// 
		/// </summary>
		public Font RSquaredFont 
		{
			get
			{
				return m_rsquaredFont;
			}
			set
			{
				m_rsquaredFont = value;
			}
		}

		
		/// <summary>
		/// Get/Set the color of the data point.
		/// </summary>
		[Category("Plot Appearance")]
		[Description("Gets / Sets the start displayable row.")]
		public int StartRow
		{
			get
			{
				return m_displayBounds.StartRow;
			}
			set
			{
				m_displayBounds.StartRow = value;
			}
		}

		/// <summary>
		/// Get/Set the color of the data point.
		/// </summary>
		[Category("Plot Appearance")]
		[Description("Gets / Sets the end displayable row.")]
		public int EndRow
		{
			get
			{
				return m_displayBounds.EndRow;
			}
			set
			{
				m_displayBounds.EndRow = value;
			}
		}

		
		/// <summary>
		/// Get/Set the color of the data point.
		/// </summary>
		[Category("Plot Appearance")]
		[Description("Gets / Sets the start displayable Column.")]
		public int StartColumn
		{
			get
			{
				return m_displayBounds.StartColumn;
			}
			set
			{
				m_displayBounds.StartColumn= value;
			}
		}/// <summary>
		/// Get/Set the color of the data point.
		/// </summary>
		[Category("Plot Appearance")]
		[Description("Gets / Sets the end displayable Column.")]
		public int EndColumn
		{
			get
			{
				return m_displayBounds.EndColumn;
			}
			set
			{
				m_displayBounds.EndColumn= value;
			}
		}
		/// <summary>
		/// Gets/Sets whether the background colors of each plot should be scaled to the data provided.
		/// </summary>
		public bool AutoScaleColor
		{
			get
			{
				return m_autoScaleColor;
			}
			set 
			{
				m_autoScaleColor = value;
			}
		}
						
		/// <summary>
		/// Gets/Sets Logarithmic R-Squared Matrix
		/// </summary>
		public float [,] LinearFitParameters
		{
			get
			{
				return m_linearFit;
				
			}
			set 
			{				
				m_linearFit = value;
			}
		}	

		/// <summary>
		/// 
		/// </summary>
		public bool AutoComputeDataPointColor
		{
			get
			{
				return m_autoUsePlotColor;
			}
			set
			{	
				m_autoUsePlotColor = value;
			}
		}


		private int [] marr_randomMask = null;

		/// <summary>
		/// Get/Set the scatterplot data to render.
		/// </summary>
		public float[,] ScatterPlotData
		{
			get
			{				
				return m_scatterData;
			}
			set
			{
				if (value != null)				
				{
					m_showLogData = false;
					m_numDataSets = value.GetUpperBound(1) + 1;					
					m_numClusters = value.Length/m_numDataSets;					
					m_logData	  = new float[m_numClusters,m_numDataSets];				
					m_normalData  = value;
					m_scatterData = m_normalData; 									
					/*
					 *	m_commonPoints tracks what data sets need to have common shared points
					 *  m_selectedPlots tracks what data sets need to render with a gray overplot
					 */ 
					m_commonPoints  = new ArrayList(m_numDataSets);											
					m_selectedPlots = new PlotSelection[m_numDataSets, m_numDataSets];

					/// 
					/// Instead of calculating random points on the fly 
					/// here we make the random mask of points to plot.  The control can then use this array from 0 points to the # of clusters
					/// to plot.  
					/// 
					Random random	 = new Random();
					marr_randomMask = new int[m_numClusters];
					for(int i = 0; i < m_numClusters; i++)
					{
						marr_randomMask[i] = random.Next(m_numClusters);
					}
					CalculateData();					
				}
			}
		}

		private void CalculateData() 
		{										
			try 
			{		
				if (QuitCalcThread() == false)
				{					
					System.Diagnostics.Trace.WriteLine("Could not stop calculation thread from running.");
				}
				else
				{																				
					m_calcThread = new Thread(new System.Threading.ThreadStart(WorkerCalcStart));
					m_calcThread.Name = "calcWorkerThread";				
					m_calcThread.Start();						
					while(!m_calcThread.IsAlive);																							
					
					m_calculationDone = true;
				}													
			} 
			catch (Exception ex)
			{
				
				System.Diagnostics.Trace.WriteLine(ex.Message);				
			}
		}

		/// <summary>
		///  Gets/Sets what data sets should be tested for commonality with data points.
		/// </summary>
		public ArrayList CommonSets
		{
			get
			{
				return m_commonPoints;
			}
			set
			{
				m_commonPoints = value;
			}
		}

		#endregion

		#region Threading
		/// <summary>
		/// Locks the Reader Writer Lock.  
		/// </summary>
		private bool Lock()
		{	
			
			bool val = m_lock.WaitOne();
			//AcquireWriterLock(3000);
			//bool val = m_lock.IsWriterLockHeld;
			Console.WriteLine("\t\tScatterPlot Client: Lock: " + val.ToString());
			return val;
		}

		/// <summary>
		/// Unlocks the Reader Writer Lock. 
		/// </summary>
		private void UnLock()
		{	
			System.Diagnostics.Trace.Write("\t\tScatterPlot Client: Unlocking Called:");
			if (m_lock != null)
			{
				System.Diagnostics.Trace.WriteLine(" Unlocked");
				m_lock.ReleaseMutex();
			}
			else
			{
				System.Diagnostics.Trace.WriteLine(" No handle to lock");
			}
		}			

		
		/// <summary>
		/// Stops the render thread if it is already rendering a scatter plot.
		/// </summary>
		/// <returns>true if success; false if failure</returns>
		private bool QuitRenderThread()
		{
			if (m_renderThread == null)
				return true;
			m_renderThread.Abort();
			bool status = m_renderThread.Join(100);					
			return status;
		}

		private bool QuitCalcThread()
		{
			if (m_calcThread == null)
				return true;
			m_calcThread.Abort();
			bool status = m_calcThread.Join(100);					
			return status;
		}

			
		/// <summary>
		/// Entry point for the worker thread that renders the scatter plot.
		/// </summary>
		private void WorkerDrawStart()
		{
			if (m_calculationDone == false)
				return;

			try
			{				
				if (m_calculationsDone == false)
					return;

				DrawBitmaps();									
				if (OnDrawingComplete!= null)
					OnDrawingComplete();								
			}
			catch (System.Threading.ThreadAbortException ex) 
			{
				
				/* Catch the threading exception */						
				try
				{
					UnLock();
				}
				catch
				{
					//System.console.writeline(lockEx.Message);
				}
				System.Console.WriteLine("Threading Abort Exception: " + ex.Message);				
			}
			catch(OutOfBitmapMemoryException ex)
			{					
				UnLock();				
				
				long newHeight;
				long newWidth;						

				newHeight  = ex.Height / (NumberOfDatasets*NumberOfDatasets); 
				newWidth   = ex.Width  / (NumberOfDatasets*NumberOfDatasets); 

				string message;
				message  = "The size of this scatter plot was too large for your system to handle.";				
				message += "  The height and width for the labels has shown to be too large for the number of plots you are attempting to plot.";				
				message += String.Format("  We suggest that you use {0} pixels for the label height {1} pixels for the label width.", newWidth, newHeight);
				MessageBox.Show(message, "Not Enough Memory!", System.Windows.Forms.MessageBoxButtons.OK);
				System.Threading.Thread.CurrentThread.Abort();
			}
			catch (Exception ex)
			{
				Debugger.Log(10, "Log", ex.ToString());
				//System.console.writeline(ex.Message);											
				try
				{
					UnLock();
				}
				catch//(Exception lockEx)
				{
					//System.console.writeline(lockEx.Message);
				}
			}
		}		

		private bool m_calculationsDone = false;
		private int  m_percentCalculated = 0;

		/// <summary>
		/// Entry point for threaded calculations.
		/// </summary>
		private void WorkerCalcStart()
		{
			try
			{				
				/* Computes the log of the scatter data */
				
				m_percentCalculated += 0;
				ComputeLogData();
				m_percentCalculated += 25;

				if (OnPercentCalc != null)
					OnPercentCalc(m_percentCalculated);

				/* Auto ranges all of the plots to fit within the bounds of the drawing subplot */
				AutoRangeData();	
		
				m_percentCalculated += 25;
				if (OnPercentCalc != null)
					OnPercentCalc(m_percentCalculated);

				/* Calculates the linear regression parameters */

				CalculateLinearRegression();				
				if (OnPercentCalc != null)
					OnPercentCalc(m_percentCalculated);

				/* Computes the color scale based off of the linear parameters R-squared value */
				ComputeColorScale();	
				if (OnPercentCalc != null)
					OnPercentCalc(m_percentCalculated);

				if (OnCalculationComplete != null)
					OnCalculationComplete();		
				
				m_calculationsDone = true;
			}
			catch (System.Threading.ThreadAbortException ex) 
			{								
				System.Diagnostics.Trace.WriteLine("ScatterPlot::WorkerCalcStart Threading Abort Exception: " + ex.Message);				
			}			
			catch (Exception ex)
			{				
				System.Diagnostics.Trace.WriteLine("ScatterPlot::WorkerCalcStart.  Caught an exception " + ex.Message);
			}
		}		

		#endregion
			
		#region Events / Event Handlers 
		
		/// <summary>
		/// Raises event that the control is percentdone of complete rendering.  Reader Writer Lock should be locked before making this call.
		/// </summary>
		/// <param name="percentDone">amount the control has rendered</param>
		private void UpdatePercentDrawn(double percentDone)
		{				
			
			if (OnPercentDrawn!= null)
			{									
				OnPercentDrawn(Convert.ToInt32(percentDone));				
			}			
		}
	
		/// <summary>
		/// Event handler for the OnPaint call.  Redraws the bitmap to the clipped area. 
		/// </summary>
		/// <param name="sender">Object who called the event.</param>
		/// <param name="e">Arguments detailing clipped area.</param>
		private void displayArea_Paint(object sender, PaintEventArgs e)
		{							
			System.Diagnostics.Trace.WriteLine("ScatterPlot Client:  PAINTING");	
			Lock();				
			if (m_bitmap != null)
			{
				try
				{
					e.Graphics.DrawImage(m_bitmap, 0, 0);	
				}
				catch(Exception ex)
				{
					System.Diagnostics.Trace.WriteLine("ScatterPlot Client:  PAINTING EXCEPTION: " + ex.StackTrace);
					System.Diagnostics.Trace.WriteLine(ex.Message);
				}
			}
			UnLock();										
		}			

		/// <summary>
		/// If the control resizes then the bitmaps need to be resized.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ctlScatterPlotClient_Resize(object sender, EventArgs e)
		{			
			m_recreateBitmaps = true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ctlScatterPlotClient_MouseDown(object sender, MouseEventArgs e)
		{	
			/*try
			{

				// Make sure they are not right clicking as this control has options 
				// that appear in a context menu with a right click.
				if (e.Button == System.Windows.Forms.MouseButtons.Left)
				{
					int row = -1;
					int col = -1;

					if (AlignHorizontal == null || AlignVertical == null)
						return;

					///
					///	Find the plot row,col that we clicked on.
					///
					for(int i = 0; i < AlignHorizontal.Length - 1; i++)
					{
						if (e.X > AlignHorizontal[i] && e.X < AlignHorizontal[i + 1])					
						{
							col = i + m_displayBounds.StartColumn;
							break;
						}
					}	
					for(int i = 0; i < AlignVertical.Length - 1; i++)
					{
						if (e.Y > AlignVertical[i] && e.Y < AlignVertical[i + 1])					
						{
							row = i + m_displayBounds.StartRow;
							break;
						}
					}
	
					 ///
					 ///	If the plot was found then send an event to the parent control.
					 ///
					if (row != -1 && col != -1)
					{
						///
						///	Since we are using a tristate , NOT,JUST,HIGHLIGHTED we have to return a bool
						///  use a temp variable instead of casting the enum to a boolean.
						///
						bool selectionValue = false;					
						if (m_selectedPlots[row,col] == PlotSelection.NOT_SELECTED)
						{
							m_selectedPlots[row,col] = PlotSelection.SELECTED;
							selectionValue = true;
						}
						else
						{
							m_selectedPlots[row,col] = PlotSelection.NOT_SELECTED;
							selectionValue = false;
						}
					
						DrawSubPlot(row,col);					
						if (OnDrawingComplete != null)
							OnDrawingComplete();										

						if (OnPlotClicked != null)
						{				
							Point p = new Point(e.X, e.Y);
							OnPlotClicked(row, row, col, col,p, selectionValue);
						}
					}
				}
			}
			catch
			{
			}
			*/
		}
		#endregion
		
		#region Range - Autoscaling data and color
		/// <summary>
		/// Gets the actual viewport that will be used, given the user's potential
		/// viewport.  Expands/contracts the viewport according to the settings of
		/// AutoViewPortXAxis and AutoViewPortYAxis.
		/// </summary>
		/// <param name="potentialViewPort"></param>
		/// <returns>The actual viewport to be used.</returns>
		public void AutoRangeData() 
		{									
			float xMin = float.MaxValue;
			float xMax = float.MinValue;
			float yMin = float.MaxValue;
			float yMax = float.MinValue;
			
			// If no data exists just make it look at a normalized 0,1 range for X,Y
			if (m_scatterData == null)
			{
				m_range.mflt_xstart = 0.0F;
				m_range.mflt_xend	= 1.0F;
				m_range.mflt_ystart = 0.0F;
				m_range.mflt_yend	= 1.0F;
				return;
			}
			
			// Find the smallest X,Y and the largest X,Y				
			for(int i = 0; i < m_numDataSets; i++)				
			{
				for(int k = 0; k <  m_numClusters; k++)
				{
					float x = m_scatterData[k,i];
					if (float.IsNaN(x) == false)
					{
						xMin = Math.Min(xMin, x);
						xMax = Math.Max(xMax, x);
						yMin = Math.Min(yMin, x);
						yMax = Math.Max(yMax, x);											
					}
				}
			}

			m_range.mflt_xstart = xMin;
			m_range.mflt_ystart = yMin;
			m_range.mflt_xend = xMax;
			m_range.mflt_yend = yMax;	
		}
		
		/// <summary>
		/// Computes the autoscale for the current variability matrix
		/// </summary>
		private void ComputeColorScale()
		{			
			if (m_autoScaleColor && m_linearFit != null)
			{	
				float min,max;
				min = m_linearFit[0,(int)LinearParameters.RSQUARED]; 
				max = m_linearFit[0,(int)LinearParameters.RSQUARED]; 

				for(int i = 0; i < m_numDataSets*m_numDataSets; i++)
				{
					if(m_linearFit[i,(int)LinearParameters.RSQUARED] > max)						
						max = m_linearFit[i,(int)LinearParameters.RSQUARED];					
					if(m_linearFit[i,(int)LinearParameters.RSQUARED] < min)
						min = m_linearFit[i,(int)LinearParameters.RSQUARED];
				}
				this.Legend.UpperRange = max;
				this.Legend.LowerRange = min;
			}						
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		private Color ComputeColorComplement(Color col)
		{									
			int rc, gc, bc;	
			int maxc = Math.Max(col.G, Math.Max(col.R, col.B));
			int minc = Math.Min(col.G, Math.Min(col.R,col.B));
			
			if (maxc < 128)
			{
				rc = 255;
				gc = 255; 
				bc = 255;
			}
			else 
			{
				rc = 0;
				gc = 0; 
				bc = 0;
			}
			return Color.FromArgb(rc, gc, bc);
		}
				
		/// <summary>
		/// Show the normal data set by setting the reference
		/// </summary>
		public void ShowNormal()
		{
			// Set the reference to normal data.
			m_scatterData = m_normalData;
			m_linearFit   = m_linearFitNorm;
			if(m_showLogData == true)
			{				
				AutoRangeData();
				ComputeColorScale();
				OnRefresh();											
			}
			m_showLogData = false;			
		}

		/// <summary>
		/// Setup the data set reference to point to the log data.
		/// </summary>
		public void ShowLogarithmic()
		{	
			// Set the reference to logarithmic data., auto range if we havent.
			m_scatterData	= m_logData;				
			m_linearFit		= m_linearFitLog;

			if (m_showLogData == false)	
			{				
				AutoRangeData();
				ComputeColorScale();											
				OnRefresh();
			}
			m_showLogData = true;			
		}
	
		/// <summary>
		/// Computes the log of the scatterplot data provided.
		/// </summary>
		private void ComputeLogData()
		{
			int i,j;
			i = 0; 
			j = 0;
			try
			{
				for( i = 0; i < m_numDataSets; i++)					
				{
					for( j = 0; j < m_numClusters; j++)
					{										
							m_logData[j,i] = Convert.ToSingle(Math.Log(m_normalData[j,i]));					
					}
				}
			}
			catch//(Exception e)
			{
				//System.console.writeline(e.Message + " " + i.ToString() + " " + j.ToString());
			}
		}
		
		/// <summary>
		/// Calculates the cross correlation matrices for logarithmic and normal data sets.
		/// </summary>
		/// <param name="stdDev">True calculates the std. dev. while false calculates the normal.</param>
		private void CalculateLinearRegression()
		{			
			int num_selected = m_numDataSets;		 											
			m_linearFitNorm     = new float[num_selected*num_selected, NUM_LINEAR_PARAMETERS];						
			m_linearFitLog      = new float[num_selected*num_selected, NUM_LINEAR_PARAMETERS];
			
			double [] x = new double[0]; 
			double [] y = new double[0]; 

			float totalPercent	= 50.0f;
			int tempPercentDone = m_percentCalculated;
			float pointsPerStep = 1.0f / Convert.ToSingle(num_selected);

			for (int baseline_index = 0 ; baseline_index < num_selected; baseline_index++)
			{
				int baseline_column = baseline_index; 				 										
				for (int index = baseline_index  ; index < num_selected ; index++)
				{														
					double slope     = 0.0;
					double intercept = 0.0;
					double rsq		 = 0.0;

					GetNonZeroCommonPoints(baseline_column, index, m_normalData, ref x, ref y);			
					LinearRegression(x,y,ref slope, ref intercept, ref rsq);																		
					m_linearFitNorm[index * num_selected + baseline_index, (int)LinearParameters.SLOPE]		= Convert.ToSingle(slope); 		
					m_linearFitNorm[index * num_selected + baseline_index, (int)LinearParameters.INTERCEPT]	= Convert.ToSingle(intercept); 			
					m_linearFitNorm[index * num_selected + baseline_index, (int)LinearParameters.RSQUARED]	= Convert.ToSingle(rsq); 

					
					if (slope != 0)
						slope = 1.0/slope;
					if (intercept != 0)
						intercept = 1.0/intercept;	
					m_linearFitNorm[baseline_index * num_selected + index, (int)LinearParameters.SLOPE]		= Convert.ToSingle(slope); 		
					m_linearFitNorm[baseline_index * num_selected + index, (int)LinearParameters.INTERCEPT]	= Convert.ToSingle(intercept); 			
					m_linearFitNorm[baseline_index * num_selected + index, (int)LinearParameters.RSQUARED]	= Convert.ToSingle(rsq); 	
						
					/* Logarithmic */
					GetNonZeroCommonPoints(baseline_column, index, m_logData, ref x, ref y);	
					LinearRegression(x,y,ref slope, ref intercept, ref rsq);


										
					m_linearFitLog[index * num_selected + baseline_index, (int)LinearParameters.SLOPE]		= Convert.ToSingle(slope); 		
					m_linearFitLog[index * num_selected + baseline_index, (int)LinearParameters.INTERCEPT]	= Convert.ToSingle(intercept); 			
					m_linearFitLog[index * num_selected + baseline_index, (int)LinearParameters.RSQUARED]	= Convert.ToSingle(rsq); 					
						
					if (slope != 0)
						slope = 1.0/slope;
					if (intercept != 0)
						intercept = 1.0/intercept;
					m_linearFitLog[baseline_index * num_selected + index, (int)LinearParameters.SLOPE]		= Convert.ToSingle(slope); 		
					m_linearFitLog[baseline_index * num_selected + index, (int)LinearParameters.INTERCEPT]	= Convert.ToSingle(intercept); 			
					m_linearFitLog[baseline_index * num_selected + index, (int)LinearParameters.RSQUARED]	= Convert.ToSingle(rsq); 

				}
				tempPercentDone = m_percentCalculated + Convert.ToInt32(totalPercent * (pointsPerStep*Convert.ToSingle(baseline_index)));
				if (OnPercentCalc != null)
					OnPercentCalc(tempPercentDone);		
			}			
		
			m_percentCalculated += Convert.ToInt32(totalPercent);
			m_linearFit = m_linearFitNorm;			
		}
		
		/// <summary>
		/// Gets non-zero common points
		/// </summary>
		/// <param name="baseline_column"></param>
		/// <param name="dataset_column"></param>
		/// <param name="baseline_intensities"></param>
		/// <param name="dataset_intensities"></param>
		private void GetNonZeroCommonPoints(int baseline_column, 
			int dataset_column,
			float [,] data, /*bool logarithmic,*/
			ref double [] baseline_intensities, 
			ref double [] dataset_intensities)
		{
			try
			{
				// two pass process. In the first round figure out how many points are needed. 
				// in the second copy that over. 
				int num_to_copy = 0; 
				int num_copied = 0; 
				
				for (int cluster_num = 0 ; cluster_num < m_numClusters; cluster_num++)
				{					
					double baseline_intensity = data[cluster_num ,baseline_column]; 
					double dataset_intensity  = data[cluster_num ,dataset_column];  

					if (!double.IsNaN(baseline_intensity) && !double.IsNaN(dataset_intensity) && baseline_intensity != 0 && dataset_intensity != 0)
					{
						num_to_copy++ ; 
					}
				}

				baseline_intensities = new double[num_to_copy] ; 
				dataset_intensities = new double[num_to_copy] ; 
			

				for (int cluster_num = 0 ; cluster_num < m_numClusters; cluster_num++)
				{
					double baseline_intensity = data[cluster_num ,baseline_column]; //marr_intensities[baseline_index] ;
					double dataset_intensity  = data[cluster_num ,dataset_column];  //marr_intensities[dataset_index] ;

					if (!double.IsNaN(baseline_intensity) && !double.IsNaN(dataset_intensity) 
						&& baseline_intensity != 0 && dataset_intensity != 0)
					{						
						baseline_intensities[num_copied] = Convert.ToSingle(baseline_intensity) ; 
						dataset_intensities[num_copied]  = Convert.ToSingle(dataset_intensity) ; 
						num_copied++ ; 
					}
				}
			}
			catch
			{
				
			}
		}


		private static void LinearRegression(double [] X, double [] Y, ref double slope, ref double intercept, ref double rsquare)
		{
			double SumY, SumX, SumXY, SumXX, SumYY ; 
			SumY = 0 ; 
			SumX = 0 ; 
			SumXY = 0 ;
			SumXX = 0 ;
			SumYY = 0 ;
			int num_pts = X.Length ; 
			for (int index = 0 ; index < num_pts ; index++)
			{
				SumX = SumX + X[index]  ; 
				SumY = SumY + Y[index] ; 
				SumXX = SumXX + X[index] * X[index] ; 
				SumXY = SumXY + X[index] * Y[index] ; 
				SumYY = SumYY + Y[index] * Y[index] ; 
			}
			slope = (num_pts * SumXY - SumX * SumY) / (num_pts * SumXX - SumX * SumX) ; 
			intercept = (SumY - slope * SumX) / num_pts ;

			double temp = (num_pts * SumXY - SumX * SumY) / Math.Sqrt((num_pts*SumXX - SumX * SumX)*(num_pts*SumYY - SumY * SumY)) ; 
			rsquare = temp * temp ; 
		}


		#endregion


		private void displayArea_MouseHover(object sender, EventArgs e)
		{
//			int x = Cursor.Position.X;
//			int y = Cursor.Position.Y;
//			if (DatasetHover != null)
//				DatasetHover(x,y);
		}

		private void ctlScatterPlotClient_MouseWheel(object sender, MouseEventArgs e)
		{
			if (this.ScatterPlotMouseWheel != null)
			{
				ScatterPlotMouseWheel(sender, e);
			}
		}
	}
	#region Memory Exception Class
	/// <summary>
	/// Out of Memory Bitmap Exception
	/// </summary>
	public class OutOfBitmapMemoryException: Exception
	{
		private long m_width;
		private long m_height;
		private long m_memory;
		
		public OutOfBitmapMemoryException()
		{			

		}
		public long Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}
		public long Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}
		public long Memory
		{
			get
			{
				return m_memory;
			}
			set
			{
				m_memory = value;
			}
		}
	}	

	#endregion

	#region Display Bounds Class
	public class PlotDisplayBounds
	{
		private int m_startRow		= 0;
		private int m_endRow		= 0;
		private int m_startColumn	= 0;
		private int m_endColumn		= 0;
		private int m_numColumns	= 0;
		private int m_numRows		= 0;

		public PlotDisplayBounds(int numRows, int numCols)
		{
			m_numRows     = numRows;
			m_numColumns  = numCols;
			
			if (numRows > 0 && numCols > 0)
			{
				m_startRow    = 0;
				m_endRow      = numRows - 1;
				m_startColumn = 0;
				m_endColumn   = numCols - 1;
			}
		}
		public int GetColumnIndex(int col)
		{	
			return  col - m_startColumn;				
		}
		public int GetRowIndex(int row)
		{
			return row - m_startRow;
		}
		

		public int NumberOfRows
		{
			get
			{
				return m_numRows;
			}		
			set
			{
				m_numRows = value;												
			}
		}		
		public int NumberOfColumns
		{
			get
			{
				return m_numColumns;
			}		
			set
			{
				m_numColumns =  value;											
			}			
		}

		public int StartRow
		{
			get
			{
				return m_startRow;
			}
			set
			{
				m_startRow = value;
			}
		}

		public int EndRow
		{
			get
			{
				return m_endRow;
			}
			set
			{
				m_endRow = value;
			}
		}
		
		public int StartColumn
		{
			get
			{
				return m_startColumn;
			}
			set
			{
				m_startColumn = value;
			}
		}

		public int EndColumn
		{
			get
			{
				return m_endColumn;
			}
			set
			{
				m_endColumn = value;
			}
		}

		public override string ToString()
		{
			return String.Format("Start Row: {0:000} End Row: {1:000}  Start Column: {2:000} End Column: {3:000}",
				m_startRow, m_endRow, m_startColumn, m_endColumn);

		}	

	}
	#endregion

}