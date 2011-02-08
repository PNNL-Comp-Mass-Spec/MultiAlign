using System;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace PNNLControls
{	
    /// <summary>
    /// Class that handles the axis bounds and plotting.
    /// </summary>
	public class clsPlotterAxis
	{
		private Rectangle m_chartBounds;
		private Int32 mint_XAxisMargin = 0;
		private Int32 mint_YAxisMargin = 0;
		// width from the left taken up by the ylabel.
		private Int32 mint_ylabel_percent = 40 ; 
		// width from the axis taken up by the y units
		private Int32 mint_yunit_percent = 60 ; 
		// height from the bottom tak up by the x label.
		private Int32 mint_xlabel_percent = 40 ; 
		// height from the axis taken up by the x units.
		private Int32 mint_xunit_percent = 60 ; 
        private Font m_font = new Font("Microsoft Sans Serif", 10);
		private int m_minFontSize = 8;
		private int m_maxFontSize = 15;

		private bool m_autoSizeFonts = true;

		private clsUnitPlotter mobj_yUnitPlotter = null; 
		private clsLabelPlotter mobj_yLabelPlotter = null ; 
		private clsUnitPlotter mobj_xUnitPlotter = null; 
		private clsLabelPlotter mobj_xLabelPlotter = null ;

		private Rectangle m_bounds;

		#region "Public Properties"

		public int MinFontSize 
		{
			get 
			{
				return this.m_minFontSize;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentException("Font size must be >= 0", "MinFontSize");
				}
				this.m_minFontSize = value;
			}
		}

		public int MaxFontSize 
		{
			get 
			{
				return this.m_maxFontSize;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentException("Font size must be >= 0", "MaxFontSize");
				}
				this.m_maxFontSize = value;
			}
		}
		
		public float FontSize
		{
			get
			{
				return this.m_font.Size ; 
			}
			set
			{
				if (value != this.m_font.Size)
				{
					this.m_font = new Font(m_font.FontFamily, value) ; 
				}
			}
		}
		public float XMinValue
		{
			get
			{
				return this.mobj_xUnitPlotter.MinRange;
			}
		}

		public float XMaxValue
		{
			get
			{
				return this.mobj_xUnitPlotter.MaxRange;
			}
		}

		public  float YMinValue
		{
			get
			{
				return this.mobj_yUnitPlotter.MinRange;
			}
		}

		public  float YMaxValue
		{
			get
			{
				return this.mobj_yUnitPlotter.MaxRange;
			}
		}

		public Font LabelAndUnitFont 
		{
			get
			{
				return this.m_font;
			}
			set 
			{
				this.m_font = value;
			}
		}

		public bool AutoSizeFonts 
		{
			get 
			{
				return m_autoSizeFonts;
			}
			set 
			{
				this.m_autoSizeFonts = value;
			}
		}


		public string XAxisLabel
		{			
			get
			{
				return this.mobj_xLabelPlotter.Label ; 
			}
			set
			{
				this.mobj_xLabelPlotter.Label = value;
			}
		}

		public string YAxisLabel
		{
			get
			{
				return this.mobj_yLabelPlotter.Label ;
			}
			set
			{
				this.mobj_yLabelPlotter.Label = value;
			}
		}

		public Rectangle ChartAreaBounds 
		{
			get 
			{
				return this.m_chartBounds;
			}
		}

		public  Int32 XPixelMargin
		{
			get
			{
				return mint_XAxisMargin;
			}
		}

		public  Int32 YPixelMargin
		{
			get
			{
				return mint_YAxisMargin;
			}
		}

		public Rectangle Bounds 
		{
			get 
			{
				return this.m_bounds;
			}
			set 
			{
				this.m_bounds = value;
			}
		}


		public Rectangle XLabelBounds
		{
			get
			{
				return this.mobj_xLabelPlotter.Bounds ; 
			}
			set
			{
				this.mobj_xLabelPlotter.Bounds = value ; 
			}
		}
		public Rectangle XUnitBounds
		{
			get
			{
				return this.mobj_xUnitPlotter.Bounds ; 
			}
			set
			{
				this.mobj_xUnitPlotter.Bounds = value ; 
			}
		}

		public Rectangle YLabelBounds
		{
			get
			{
				return this.mobj_yLabelPlotter.Bounds ; 
			}
			set
			{
				this.mobj_yLabelPlotter.Bounds = value ; 
			}
		}
		public Rectangle YUnitBounds
		{
			get
			{
				return this.mobj_yUnitPlotter.Bounds ; 
			}
			set
			{
				this.mobj_yUnitPlotter.Bounds = value ; 
			}
		}

		public IList XTickMarks
		{
			get 
			{
				return this.mobj_xUnitPlotter.TickMarks;
			}
		}

		public IList YTickMarks 
		{
			get 
			{
				return this.mobj_yUnitPlotter.TickMarks;
			}
		}

		#endregion

		#region Public Methods

		public void SetRanges(float xMinData, float xMaxData, float yMinData, float yMaxData)
		{      
			// By default first set the xminunit values and ymintunit values to these people.
			this.mobj_xUnitPlotter.SetRange(xMinData, xMaxData) ; 
			this.mobj_yUnitPlotter.SetRange(yMinData, yMaxData) ;
			//Console.WriteLine("Setting ranges {0} {1} {2} {3}", xMinData, xMaxData, yMinData, yMaxData);
		}

//		public void SetLabels(string xAxisLabel, string yAxisLabel, string Title)
//		{
//			this.mobj_xLabelPlotter.Label = xAxisLabel;
//			this.mobj_yLabelPlotter.Label = yAxisLabel;
//			this.mobj_TitlePlotter.Label = Title;
//		}

		/// <summary>
		/// Sets the boundaries of the axes and internal charting area.
		/// </summary>
		/// <param name="xaxis_margin">Pixels of margin to the bottom of the x axis.</param>
		/// <param name="yaxis_margin">Pixels of margin to the left of the y axis.</param>
		/// <param name="width">Width of area into which axes should fit.</param>
		/// <param name="height">Height of area into which axes should fit.</param>
		/// <returns>The rectangle that in which the actual data of the chart 
		/// should be drawn.</returns>
//		private System.Drawing.Rectangle SetPlotArea(int xaxis_margin, int yaxis_margin, int width, int height) 
//		{
//			this.mint_XAxisMargin = xaxis_margin; 
//			this.mint_YAxisMargin = yaxis_margin; 
//
//			this.mint_height = height;
//			this.mint_width = width;
//
//			Rectangle xlabel_bounds = new Rectangle(this.YPixelMargin + this.Bounds.Left, this.mint_height - (this.XPixelMargin * this.mint_xlabel_percent)/100 + this.Bounds.Top,
//				this.mint_width - 2* this.YPixelMargin, (this.XPixelMargin * this.mint_xlabel_percent)/100) ;   
//
//			Rectangle xunit_bounds = new Rectangle(this.YPixelMargin + this.Bounds.Left, this.mint_height - this.XPixelMargin + this.Bounds.Top,
//				this.mint_width - 2* this.YPixelMargin, (this.XPixelMargin * this.mint_xunit_percent)/100) ;   
//        
//			Rectangle ylabel_bounds = new Rectangle(0 + this.Bounds.Left, this.XPixelMargin + this.Bounds.Top, (this.YPixelMargin * this.mint_ylabel_percent)/100, 
//				this.mint_height - 2*this.XPixelMargin) ;   
//
//			Rectangle yunit_bounds = new Rectangle((this.YPixelMargin * (100 - this.mint_yunit_percent))/100  + this.Bounds.Left, this.XPixelMargin + this.Bounds.Top, 
//				(this.mint_yunit_percent * this.YPixelMargin)/100 , this.mint_height - 2* this.XPixelMargin) ;   
//
//			//Rectangle title_bounds = new Rectangle(0 + this.Bounds.Left,0,this.mint_width, this.XPixelMargin) ; 
//
//			this.mobj_xLabelPlotter.Bounds = xlabel_bounds ; 
//			this.mobj_yLabelPlotter.Bounds = ylabel_bounds ; 
////			this.mobj_TitlePlotter.Bounds = title_bounds ;
//
//			this.mobj_xUnitPlotter.Bounds = xunit_bounds ; 
//			this.mobj_yUnitPlotter.Bounds = yunit_bounds ;
//			this.screenAreaPixelHeight = xunit_bounds.Top - yunit_bounds.Top;
//			this.screenAreaPixelWidth = xunit_bounds.Right - yunit_bounds.Right;
//			this.m_chartBounds = new System.Drawing.Rectangle(yunit_bounds.Right, yunit_bounds.Top, 
//				screenAreaPixelWidth, screenAreaPixelHeight);
//			return m_chartBounds;
//		}

//		public void SetPlotArea(ref Bitmap PlotArea, int xaxis_margin, int yaxis_margin)
//		{
//			//mbmpDrawingArea = PlotArea ; 
//			this.SetPlotArea(xaxis_margin, yaxis_margin, PlotArea.Width, PlotArea.Height);	
//		}

		/// <summary>
		/// Gets the x screen coordinate for an x chart coordinate
		/// </summary>
		/// <param name="xValue"></param> 
		/// <returns></returns>
		public float XScreenPixel(float xValue) 
		{
			float range = this.mobj_xUnitPlotter.mflt_max_val - this.mobj_xUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Width;
			return (xValue - this.mobj_xUnitPlotter.mflt_min_val) / range_per_pixel;
		}

		/// <summary>
		/// Gets the y screen coordinate for a y chart coordinate.
		/// </summary>
		/// <param name="yValue"></param>
		/// <returns></returns>
		public float YScreenPixel(float yValue) 
		{
			float range = this.mobj_yUnitPlotter.mflt_max_val - this.mobj_yUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Height;
			return this.m_chartBounds.Height - 
				(yValue - this.mobj_yUnitPlotter.mflt_min_val) / range_per_pixel;
		}

		/// <summary>
		/// Gets the number of pixels used on the screen for drawing a give width. 
		/// </summary>
		/// <param name="width">The width of the drawing area</param>
		/// <returns></returns>
		public float PixelWidth(float width)
		{
			float range = this.mobj_xUnitPlotter.mflt_max_val - this.mobj_xUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Width;
			return width / range_per_pixel;
		}

		/// <summary>
		/// Gets the number of pixels used on the screen for drawing a give height. 
		/// </summary>
		/// <param name="height">The height of the drawing area</param>
		/// <returns></returns>
		public float PixelHeight(float height)
		{
			float range = this.mobj_yUnitPlotter.mflt_max_val - this.mobj_yUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Height;
			return  height / range_per_pixel;
		}

		/// <summary>
		/// Gets the chart coordinate for a screen coordinate relative to the upper-left 
		/// of the chart area.
		/// </summary>
		/// <param name="xPixel"></param>
		/// <returns></returns>
		public float XChartCoordinate(int xPixel) 
		{
			float range = this.mobj_xUnitPlotter.mflt_max_val - this.mobj_xUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Width;
			return xPixel * range_per_pixel + this.mobj_xUnitPlotter.mflt_min_val;
		}

		public float YChartCoordinate(int yPixel) 
		{
			float range = this.mobj_yUnitPlotter.mflt_max_val - this.mobj_yUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Height;
			return -yPixel * range_per_pixel + this.mobj_yUnitPlotter.mflt_max_val;
		}


		public ChartLocation GetChartLocation(Point p) 
		{
			ChartLocation location = ChartLocation.None;
			if (this.mobj_xLabelPlotter.Bounds.Contains(p)) 
			{
				location |= ChartLocation.XLabel;
			} 
			else if (this.mobj_xUnitPlotter.Bounds.Contains(p)) 
			{
				location |= ChartLocation.XAxisAndUnits;
			} 
			else if (this.mobj_yLabelPlotter.Bounds.Contains(p)) 
			{
				location |= ChartLocation.YLabel;
			} 
			else if (this.mobj_yUnitPlotter.Bounds.Contains(p)) 
			{
				location |= ChartLocation.YAxisAndUnits;
			} 
			else if (this.m_chartBounds.Contains(p)) 
			{
				location |= ChartLocation.ChartArea;
			}
			return location;
		}

		/// <summary>
		/// Tells whether the given chart coordinate is in the viewport 
		/// on the screen
		/// </summary>
		/// <param name="xValue"></param>
		/// <returns></returns>
		public bool XOnScreen(float xValue) 
		{
			return xValue >= this.XMinValue && xValue <= this.XMaxValue;
		}

		/// <summary>
		/// Tells whether the given chart coordinate is in the viewport 
		/// on the screen
		/// </summary>
		/// <param name="yValue"></param>
		/// <returns></returns>
		public bool YOnScreen(float yValue) 
		{
			return yValue >= this.YMinValue && yValue <= this.YMaxValue;
		}

		public void Layout(Graphics g, clsMargins margins) 
		{
			int leftMargin = margins.GetChartLeftMargin(Bounds.Width);
			int rightMargin = margins.GetDefaultMargin(Bounds.Width);
			int topMargin = margins.GetDefaultMargin(Bounds.Height);
			int bottomMargin = margins.GetChartBottomMargin(Bounds.Height);
			int left = this.Bounds.Left;
			int right = this.Bounds.Right;
			int top = this.Bounds.Top;
			int bottom = this.Bounds.Bottom;
			int width = this.Bounds.Width;
			int height = this.Bounds.Height;

			// compute bounds of various pieces of charting surface and axes
			Rectangle yLabelBounds = new Rectangle(left, top + topMargin, leftMargin * this.mint_ylabel_percent / 100, height - topMargin - bottomMargin);
			Rectangle yUnitBounds = new Rectangle(left + (leftMargin * (100 - this.mint_yunit_percent)) / 100, 
				top + topMargin, (leftMargin * this.mint_yunit_percent) / 100, height - topMargin - bottomMargin);
			Rectangle xUnitBounds = new Rectangle(left + leftMargin, bottom - bottomMargin, width - leftMargin - rightMargin, 
				(bottomMargin * this.mint_xunit_percent) / 100);
			Rectangle xLabelBounds = new Rectangle(left + leftMargin, bottom - (bottomMargin * (this.mint_xlabel_percent)) / 100, 
				width - leftMargin - rightMargin, (bottomMargin * (this.mint_xlabel_percent)) / 100);
			//Console.WriteLine("XLabelBounds {0}", xLabelBounds);
			this.m_chartBounds = new Rectangle(left + leftMargin, top + topMargin, width - leftMargin - rightMargin, 
				height - topMargin - bottomMargin);
			//Console.WriteLine("m_chartBounds {0}", m_chartBounds);

			// Set the bounds of the various pieces
			this.mobj_xLabelPlotter.Bounds = xLabelBounds;
			this.mobj_yLabelPlotter.Bounds = yLabelBounds;
			this.mobj_xUnitPlotter.Bounds = xUnitBounds;
			this.mobj_yUnitPlotter.Bounds = yUnitBounds;

			// Set the fonts of all pieces to the main font
			this.mobj_xUnitPlotter.Font = this.LabelAndUnitFont;
			this.mobj_xLabelPlotter.Font = this.LabelAndUnitFont;
			this.mobj_yUnitPlotter.Font = this.LabelAndUnitFont;
			this.mobj_yLabelPlotter.Font = this.LabelAndUnitFont;

			// Determine the best font size to use - the largest font that works
			// for all units and labels
			float current_y_font_size = this.mobj_yUnitPlotter.SetTickAndFont(g) ; 
			float current_x_font_size = this.mobj_xUnitPlotter.SetTickAndFont(g) ; 

			// If autosizing fonts, create a new font using the best size
			if (this.AutoSizeFonts) 
			{
				float xlabel_font_size = this.mobj_xLabelPlotter.GetBestFontSize(g); 
				float ylabel_font_size = this.mobj_yLabelPlotter.GetBestFontSize(g); 

				// choose the largest fond that works for all units.
				float font_size = Math.Min(ylabel_font_size, xlabel_font_size) ; 
				font_size = Math.Min(this.MaxFontSize, font_size);
				font_size = Math.Max(this.MinFontSize, font_size);

				this.LabelAndUnitFont = new Font(this.LabelAndUnitFont.FontFamily, font_size, this.LabelAndUnitFont.Style);
			}
			// Set the font of all four label and unit parts to the same value -  
			// either the one that has been explicitly set, or the newly autosized one
			this.mobj_xUnitPlotter.Font = LabelAndUnitFont; 
			this.mobj_yUnitPlotter.Font = LabelAndUnitFont; 
			this.mobj_xLabelPlotter.Font = LabelAndUnitFont; 
			this.mobj_yLabelPlotter.Font = LabelAndUnitFont;

			//Console.WriteLine("Font Size {0}", this.LabelAndUnitFont.Size);
			// finish laying out the components now that the font size is fully known
			this.mobj_xUnitPlotter.Layout(g);
			this.mobj_yUnitPlotter.Layout(g);
			this.mobj_xLabelPlotter.Layout(g);
			this.mobj_yLabelPlotter.Layout(g);
		}

		public void Draw(Graphics g, Color foreColor)
		{  
			this.mobj_yUnitPlotter.Draw(g, foreColor) ; 
			this.mobj_xUnitPlotter.Draw(g, foreColor) ;
			this.mobj_xLabelPlotter.Draw(g, foreColor) ;
			this.mobj_yLabelPlotter.Draw(g, foreColor) ;
			//this.mobj_TitlePlotter.Draw(g) ; 
		}
		#endregion

		#region Private Helper Methods


//		private int GetCoordinate (float val, float min_val, float max_val, int start_coordinate, int width)
//		{
//			float float_width = Convert.ToSingle(width) ; 
//			float float_coordinate = float_width * (val - min_val) / (max_val - min_val) ;
//			Int32 int_coordinate = Convert.ToInt32(float_coordinate) ; 
//			return start_coordinate + int_coordinate ; 
//		}

		#endregion

		#region Constructor/Destructor

		public clsPlotterAxis()
		{
			this.mobj_xLabelPlotter = new clsLabelPlotter() ; 
			this.mobj_yLabelPlotter = new clsLabelPlotter() ; 
			this.mobj_xUnitPlotter = new clsUnitPlotter() ; 
			this.mobj_yUnitPlotter = new clsUnitPlotter() ;
//			this.mobj_TitlePlotter = new clsLabelPlotter() ;

			this.mobj_yLabelPlotter.IsVertical = true ; 
			this.mobj_yUnitPlotter.IsVertical = true ; 
		}
		#endregion
  
	}
}
