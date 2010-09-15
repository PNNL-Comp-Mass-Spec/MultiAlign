using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using PNNLControls;
using System.Drawing.Imaging;

namespace PNNLControls 
{
	/// <summary>
	/// Summary description for ctlChartBase.
	/// </summary>
	public abstract class ctlChartBase : System.Windows.Forms.UserControl, System.ComponentModel.ISupportInitialize
	{	
		// Series collection
		protected clsSeriesCollection mobj_series_collection;
		// etc.
		protected clsMargins mobj_margins = new clsMargins();
		protected clsPlotRange mobj_range = new clsPlotRange();
		protected Derek.BitmapTools mobj_bitmap_tools = null ;

        private SaveFileDialog m_saveFileDialog;

		private bool m_autoViewportOnAddition = false;
		private bool m_autoViewportOnSeriesChange = false;
		private bool m_autoViewportYAxis = false;
		private bool m_autoViewportXAxis = false;
		private float m_autoViewportYBase = 0;
		private float m_autoViewportXBase = 0;
		private bool m_useAutoViewportYBase = false;
		private bool m_useAutoViewportXBase = false;

		/// <summary>
		/// The color used for selected/hilighted series.
		/// </summary>
		private Color mHilightColor = Color.Magenta;

		/// <summary>
		/// The indexes of selected series.
		/// </summary>
		private IntRangeSelector mSelectedSeries = new IntRangeSelector();

		/// <summary>
		/// Tells whether the hilight bitmap mask for selected series needs to 
		/// be recreated, or whether it is current.
		/// </summary>
		private bool mRecreateHilightMask = true;

		/// <summary>
		/// The hilight mask for selected series.
		/// </summary>
		private Bitmap mHilightMaskBitmap;

		/// <summary>
		/// Options for copy/paste
		/// </summary>
		private bool mParametersCopyEnabled = true;
		private bool mSeriesCopyEnabled = true;
		private bool mSeriesPasteEnabled = true;
		private bool mParametersPasteEnabled = true;

        /// <summary>
        /// Determines if the series collection needs to be repainted or not.
        /// </summary>
		protected bool m_repaintSeries = true;
		private bool m_xAxisDrawGridLines = true;
		private bool m_yAxisDrawGridLines = true;
		private bool m_autoSizeFonts = true;
		private PenProvider m_gridLinePenProvider;
		private EventHandler m_gridLinePenProviderChangedHandler;
		private ChartLayer m_gridLinesLayer = ChartLayer.UnderSeries;
		private SolidBrush m_chartBackgroundBrush = new SolidBrush(Color.White);

		/// <summary>
		/// Enables or disables panning of the chart by using the arrow keys.
		/// </summary>
		private bool mPanWithArrowKeys = true;

		/// <summary>
		/// The fraction of the screen that the y axis is expanded above the highest peak.
		/// </summary>
		private float mflt_vertical_scaling = 1.0F ; 
		/// <summary>
		/// The fraction of the screen that is panned on arrow key clicks.
		/// </summary>
		private float mPanFraction = .1F;

		private ChartMarkerLayer mMarkerLayer;

		private ViewPortHistory mViewPortHistory;

		/// <summary>
		/// The three main drawing parts of the control that are handled by the 
		/// chart itself.  The title, the legend, and the main plot area (chart + axes + labels).
		/// </summary>
        protected clsPlotterAxis mobj_axis_plotter;
        /// <summary>
        /// Flag indicating whether to draw the legend.
        /// </summary>
        protected bool mbool_legendVisible;
        /// <summary>
        /// Flag indicating whether to draw the axis or not.
        /// </summary>
        protected bool mbool_axisVisible;
        /// <summary>
        /// Flag indicating whether to draw the title.
        /// </summary>
        protected bool mbool_titleVisible;
        /// <summary>
        /// Object that describes the data series.
        /// </summary>
		protected ChartLegend m_legend;
        /// <summary>
        /// Plotter for plotting lables on the control.
        /// </summary>
		protected clsLabelPlotter m_titlePlotter;
		/// <summary>
		/// And the layout that controls how the three parts above are positioned within
		/// this control.
		/// </summary>
		private ChartLayout m_layout;

		//private ToolTip toolTip;

		private bool m_hasLegend = true;
        

		/// <summary>
		/// Graphics object used to draw on bitmap during painting phase
		/// </summary>
		private Graphics mobj_bitmap_graphics; 
		/// <summary>
		/// The drawing area of the chart with background, series, and gridlines drawn, 
		/// but without the hilighted series drawn.
		/// </summary>
		protected Bitmap mobj_bitmap = null;
		/// <summary>
		/// The drawing area of the chart with background, series, gridlines, and 
		/// hilighted series drawn.
		/// </summary>
		private Bitmap mobj_series_bitmap = null;
        /// <summary>
        /// Temporary rendering bitmap.
        /// </summary>
        private Bitmap mTempBitmap = null;
        float mfloat_autoViewportPaddingX = 0.0F;
        float mfloat_autoViewportPaddingY = 0.0F;

		/// <summary>
		/// The drawing area of the chart with only series drawn on it.
		/// </summary>
		//private Bitmap mobj_bitmap_plain = null;

		private Bitmap mChartAreaInfoBitmap = null;
		
		private ChartZoomHandler m_defaultZoomHandler;

		private bool mShowFocus = true;

		private PNNLControls.SeriesChangedHandler m_seriesChangedHandler;

		/// <summary>
		/// Event that allows for post rendering of the chart area, such as the ChartZoomHandler does.
		/// </summary>
		//public event ChartPostRenderingProcessor PostRender;
		/// <summary>
		/// List of &lt;PostProcessorPriority, ChartPostRenderingProcessor&gt; pairs.
		/// </summary>
		private SortedList mPostRenderingProcessors = new SortedList();

		/// <summary>
		/// Event that is raised when the viewport is changed through the 
		/// ViewPort property
		/// </summary>
		public event ViewPortChangedHandler ViewPortChanged;
		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private static readonly Color[] DEFAULT_SERIES_COLORS = new Color[] 
			{
				Color.Red, Color.Blue, Color.Yellow, Color.Purple, Color.Green, Color.Orange, 
				Color.Pink, Color.Black, Color.SkyBlue, Color.ForestGreen, Color.Lavender, Color.Tan};

		private bool mblnUpdateInProgress = false ; 

		// The plot parameter
        private clsPlotParams m_plotParams;
        private PlotParamsChangedHandler m_plotParamsHandler;

        #region Windows Generated 
        private ContextMenu mContextMenu;
		private MenuItem menuLegendLeft;
		private MenuItem menuLegendRight;
		private MenuItem menuLegendBottom;
		private MenuItem menuLegendTop;
		private MenuItem menuLegendFloating;
		private MenuItem menuLegend;
		private MenuItem menuPropertyGrid;
		private MenuItem menuLegendShow;
		private MenuItem menuLegendDivider;
		private MenuItem menuViewPort;
		private MenuItem menuShowAllData;
		private MenuItem menuEditViewPort;
		private MenuItem menuClearViewPortHistory;
		private MenuItem menuItemCopy;
		private MenuItem menuItemPaste;
		private MenuItem menuItemSelectAllSeries;
		private MenuItem menuItemDelete;
		private MenuItem menuItemDisplayOptions;
		private MenuItem menuItemSeriesVisible;
		private MenuItem menuItemPasteByReference;
		private MenuItem menuItemBringToBack;
		private MenuItem menuItemBringToFront;
        private MenuItem menuItemSeperator1;
		private MenuItem menuItemSelectedSeries;
		private MenuItem menuItemSeriesSpecific;
        private MenuItem menuItemCopyImage;
        private MenuItem menuItemSaveImage;
        private MenuItem menuItem2;
        private MenuItem menuItem3;
        private MenuItem menuItem1;
        #endregion


        /// <summary>
        /// Default construtor for a chart base object.
        /// </summary>
		public ctlChartBase()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SuspendLayout();
			
			InitializeComponent();
			
			this.m_seriesChangedHandler = new SeriesChangedHandler(SeriesChanged);
			this.mMarkerLayer = new ChartMarkerLayer(this);
			this.m_gridLinePenProviderChangedHandler = new EventHandler(GridLinePenProviderChanged);
			mobj_series_collection = new clsSeriesCollection(this) ;
			mobj_bitmap_tools = new Derek.BitmapTools() ; 
			
			this.m_plotParams = new clsPlotParams(new SquareShape(5, false), Color.Black);
			this.m_plotParamsHandler = new PlotParamsChangedHandler(this.PlotParamsChanged);
			this.m_plotParams.PlotParamsChanged += this.m_plotParamsHandler;


            mbool_axisVisible   = true;
            mbool_legendVisible = true;
            mbool_titleVisible  = true;


			this.m_legend = new ChartLegend(this);
			this.mobj_axis_plotter = new clsPlotterAxis();
			this.m_titlePlotter = new clsLabelPlotter();
			this.m_titlePlotter.AutoSize = this.AutoSizeFonts;
			this.m_layout = new ChartLayout(this);
			this.LegendLocationChanged();

			RectangleF initialViewPort = new RectangleF(0, 0, 1, 1);
			this.mViewPortHistory = new ViewPortHistory(this, initialViewPort);
			this.mViewPortHistory.CurrentEntryChanged += new CurrentEntryChangedHandler(this.ViewPortHistoryCurrentEntryChanged);
			this.ViewPort = initialViewPort;

			ViewPortHistoryMenu histMenu = new ViewPortHistoryMenu(this.mViewPortHistory);
			this.menuViewPort.MenuItems.Add(histMenu.BackMenuItem);
			histMenu.BackMenuItem.Index = 2;
			MenuUtils.SetMaximumMenuHeight(histMenu.BackMenuItem, 220);
			this.menuViewPort.MenuItems.Add(histMenu.ForwardMenuItem);
			histMenu.ForwardMenuItem.Index = 3;
			MenuUtils.SetMaximumMenuHeight(histMenu.ForwardMenuItem, 220);

			this.mobj_margins.MarginsChanged += new EventHandler(MarginsChanged);

			this.m_defaultZoomHandler = new ChartZoomHandler(this);
			this.GridLinePen = new PenProvider(new Pen(Color.LightGray, 1));
			this.m_gridLinePenProvider.PenChanged += new EventHandler(this.GridLinePenProviderChanged);
			this.XAxisLabel = "X Axis";
			this.YAxisLabel = "Y Axis";

			this.mobj_axis_plotter.AutoSizeFonts = false;
			this.m_titlePlotter.AutoSize = false;

			// Hookup event handlers for selected series.
			mSelectedSeries.IntAdded += new IntChangedHandler(this.mSeriesSelected_IntAdded);
			mSelectedSeries.IntRemoved += new IntChangedHandler(this.mSeriesSelected_IntRemoved);


            m_saveFileDialog = new SaveFileDialog();            
            m_saveFileDialog.AddExtension = true;
            m_saveFileDialog.CheckPathExists = true;
            m_saveFileDialog.DereferenceLinks = true;
            m_saveFileDialog.ValidateNames = true;
            m_saveFileDialog.Filter = "WMF (*.wmf)|*.wmf|EMF (*.emf)|*.emf|Jpeg (*.jpg)|*.jpg|Tiff (*.tiff)|*.tiff|Gif (*.gif)|*.gif|Bitmap (*.bmp)|*.bmp";
            m_saveFileDialog.OverwritePrompt = true;
            m_saveFileDialog.FilterIndex = 1;

			this.ResumeLayout();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
				this.m_chartBackgroundBrush.Dispose();
				if (this.mobj_bitmap_graphics != null) 
				{
					this.mobj_bitmap_graphics.Dispose();
				}
				if (this.mobj_bitmap != null) 
				{
					this.mobj_bitmap.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region "Painting"

		public void BeginUpdate()
		{
			mblnUpdateInProgress = true ; 
		}
		public void EndUpdate()
		{
			mblnUpdateInProgress = false ; 
		}
        /// <summary>
        /// Gets or sets whether the axis is visible.
        /// </summary>
        public bool AxisVisible
        {
            get
            {
                return mbool_axisVisible;
            }
            set
            {
                mbool_axisVisible = value;
                this.PerformLayout();
            }
        }
        /// <summary>
        /// Gets or sets whether the title is visible.
        /// </summary>
        public bool TitleVisible
        {
            get
            {
                return mbool_titleVisible;
            }
            set
            {
                mbool_titleVisible = value;
                this.PerformLayout();
            }
        }
        /// <summary>
        /// Gets or sets whether the legend is visible.
        /// </summary>
        public bool LegendVisible
        {
            get
            {
                return mbool_legendVisible;
            }
            set 
            {
                mbool_legendVisible = value;

                if (m_layout != null)
                    m_layout.Chart.HasLegend = value;
            }
        }

		/// <summary>
		/// Draws the control on the given graphics.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="drawFocusRect">If true, the focus rectangle may be drawn, if the control 
		/// is the current input focus and ShowFocus is true.  Otherwise, no focus rectangle 
		/// will be drawn.</param>
		private void DrawToGraphics(Graphics g, bool drawFocusRect, bool drawBackground) 
		{            
			if (drawBackground) 
			{
				g.FillRectangle(new SolidBrush(this.BackColor), 
					0, 0, this.Width, this.Height);
			}
			// Draw a focus rectangle
			if (Focused && ShowFocus && drawFocusRect)
			{
				using (Pen p = new Pen(Color.Black, 1)) 
				{
					p.DashStyle = DashStyle.Dash;
					g.DrawRectangle(p, 1, 1, this.Width - 3, this.Height - 3);
				}
			}
			try 
			{								
				// set axes information to set up plot area and margins
				// set graphics clip and location for drawing of series
				System.Drawing.Drawing2D.GraphicsContainer container = g.BeginContainer();
				g.IntersectClip(this.mobj_axis_plotter.ChartAreaBounds);
                if (mbool_axisVisible == true)
                {
                    g.TranslateTransform(this.mobj_axis_plotter.ChartAreaBounds.Left,
                        this.mobj_axis_plotter.ChartAreaBounds.Top);
                }
				try 
				{
					this.EnsureChartAreaBitmapsCreated();
					// Ensure that the hilight mask is accurate with the currently selected series
					this.EnsureHighlightMaskBitmapCreated();

					// end series painting
					EndPaintSeries(g);

					// Copy the series bitmap from the "permanent copy to the temporary"
					this.mobj_bitmap_tools.Copy(mobj_bitmap, mobj_series_bitmap);

					// Hilight the series selected
					mobj_bitmap_tools.MarkWhere(mobj_series_bitmap, HilightColor.ToArgb(), 
						mHilightMaskBitmap, 1);
					
					// Draw the bitmap to the screen
					Rectangle bounds = this.mobj_axis_plotter.ChartAreaBounds;
					if (bounds.Height > 0 && bounds.Width > 0) 
					{
						g.DrawImageUnscaled(mobj_series_bitmap, 0,0);
					}

					// draw the marker layer, any text labels, etc.
					this.mMarkerLayer.Draw(g);

					// allow registered drawing routines to take a crack at doing special drawing
					// on the chart area
					this.RunPostProcessors(g);

					//unset graphics clip and location
					g.EndContainer(container);

					// paint axes
					DrawAxes(g);
                    DrawTitle(g);
                    DrawLegend(g);
				}
				catch (Exception ex) 
				{
					Console.WriteLine(ex);
				}			
			} 
			catch (Exception ex) 
			{
                Console.WriteLine(ex.Message);
			}
        }
        /// <summary>
        /// Renders the title to the graphics object.
        /// </summary>
        /// <param name="g"></param>
        protected void DrawLegend(Graphics g)
        {
            try
            {
                if (mbool_legendVisible == true)
                    this.m_legend.Draw(g, this.ForeColor);
            }
            catch
            {
            }
        }
        /// <summary>
        /// Renders the title to the graphics object.
        /// </summary>
        /// <param name="g"></param>
        protected void DrawTitle(Graphics g)
        {
            try
            {
                if (mbool_titleVisible == true)
                    this.m_titlePlotter.Draw(g, this.ForeColor);
            }
            catch
            {
            }
        }
		/// <summary>
		/// Do painting.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			DrawToGraphics(e.Graphics, true, false);
		}

		public Bitmap ChartAreaInfoBitmap 
		{
			get 
			{
				this.EnsureChartAreaInfoBitmapCreated();
				return (Bitmap) this.mChartAreaInfoBitmap.Clone();
			}
		}

		private void EnsureChartAreaBitmapsCreated() 
		{
			// begin series painting
			if (!mblnUpdateInProgress && BeginPaintSeries(this.mobj_axis_plotter.ChartAreaBounds)) 
			{
				if (this.GridLineLayer == ChartLayer.UnderSeries) 
				{
					this.DrawGrid();
				}
				// paint each series
				for (int i = 0; i < this.mobj_series_collection.Count; i++)
				{
					//Bitmap oldBitmap = (Bitmap) mobj_bitmap.Clone();
					mobj_bitmap_tools.Copy(mobj_bitmap, mTempBitmap);		
					clsSeries data = this.mobj_series_collection[i];
					if (data.PlotParams.Visible) 
					{
						this.PaintSeries(this.mobj_bitmap_graphics, mobj_bitmap, data);
						this.mobj_bitmap_tools.MarkDifferences(mobj_bitmap, mTempBitmap, 
							this.mChartAreaInfoBitmap, ChartVisibilityBitmapConstants.SeriesStart + i);
					}
				}
				if (this.GridLineLayer == ChartLayer.AboveSeries) 
				{
					this.DrawGrid();
				}
			}
			this.m_repaintSeries = false;
		}

		/// <summary>
		/// Recreates the hilight mask if necessary.
		/// </summary>
		private void EnsureHighlightMaskBitmapCreated() 
		{
			if (this.mRecreateHilightMask) 
			{
				// Fill the mask bitmap with zeros.
				this.mobj_bitmap_tools.MarkRectangle(this.mHilightMaskBitmap, 0, 
					new Rectangle(0, 0, mHilightMaskBitmap.Width, mHilightMaskBitmap.Height));
				// For each selected series, mark the hilight mask where the series is visible
				foreach (int i in this.mSelectedSeries.Values) 
				{
					mobj_bitmap_tools.MarkWhere(mHilightMaskBitmap, 1, mChartAreaInfoBitmap, 
						ChartVisibilityBitmapConstants.SeriesStart + i);
				}
				this.mRecreateHilightMask = false;
			}
		}

		private void EnsureChartAreaInfoBitmapCreated() 
		{
			this.EnsureChartAreaBitmapsCreated();
		}

		/// <summary>
		/// Create a default parameter for the data.
		/// </summary>
		/// <param name="g"></param>
		private clsPlotParams CreatePlotParams(Color c) 
		{
			clsPlotParams _params = (clsPlotParams) this.PlotParams.Clone();
			ColorInterpolater interpolater = new LinearColorInterpolater();
			Color[] colors = new Color[1];
			colors[0] = c;
			interpolater.Colors = colors;
			_params.Coloring = interpolater;
			return _params;
		}
        /// <summary>
        /// Draws the axis data to the graphics object provided.
        /// </summary>
        /// <param name="g">Graphics object to render axis to.</param>
		protected virtual void DrawAxes(Graphics g)
		{	
			try
			{	
                if (mbool_axisVisible == true)
				    mobj_axis_plotter.Draw(g, this.ForeColor) ; 
			}        				
			catch
            {
			}					
		}

		/// <summary>
		/// Called by the OnPaint method before any of the series 
		/// are painted through calling PaintSeries
		/// </summary>
		/// <param name="g"></param>
		/// <returns>True to draw each series, false otherwise</returns>
		protected virtual bool BeginPaintSeries(Rectangle size) 
		{
			bool newBitmaps = false;
			if (size.Height <= 0 || size.Width <= 0) 
			{
				// make sure that bitmaps are created anyway
				CreateBitmaps(new Size(1, 1));
				// but no need to do further drawing
				return false;
			}
			// create a new bitmap if needed
			if (this.mobj_bitmap == null || 
				this.mobj_bitmap.Height != size.Height ||
				this.mobj_bitmap.Width != size.Width) 
			{
				newBitmaps = true;
				CreateBitmaps(new Size(size.Width, size.Height));
			}
			if (this.m_repaintSeries || newBitmaps) 
			{
				this.mobj_bitmap_tools.MarkRectangle(this.mChartAreaInfoBitmap, 
					ChartVisibilityBitmapConstants.BackGround, new Rectangle(0, 0, size.Width, size.Height));
				this.mobj_bitmap_graphics.FillRectangle(this.m_chartBackgroundBrush, 0, 0, 
					this.MaxChartAreaXPixel, this.MaxChartAreaYPixel);
				return true;
			} 
			else 
			{
				return false;
			}
		}

		private void CreateBitmaps(Size size) 
		{
			if (this.mobj_bitmap != null) 
			{
				this.mobj_bitmap.Dispose();
			}
			if (this.mTempBitmap != null) 
			{
				this.mTempBitmap.Dispose();
			}
			if (this.mChartAreaInfoBitmap != null) 
			{
				this.mChartAreaInfoBitmap.Dispose();
			}
			if (this.mHilightMaskBitmap != null) 
			{
				this.mHilightMaskBitmap.Dispose();
			}
			if (this.mobj_series_bitmap != null) 
			{
				this.mobj_series_bitmap.Dispose();
			}
			this.mobj_bitmap = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
			this.mTempBitmap = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
			this.mChartAreaInfoBitmap = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
			this.mHilightMaskBitmap = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
			this.mobj_series_bitmap = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);


			
			if (this.mobj_bitmap_graphics != null) 
			{
				this.mobj_bitmap_graphics.Dispose();
			}
			this.mobj_bitmap_graphics = Graphics.FromImage(this.mobj_bitmap);
		}

		protected virtual void DrawGrid() 
		{
			Pen pen = this.m_gridLinePenProvider.Pen;
			this.mobj_bitmap_tools.Copy(this.mobj_bitmap, this.mTempBitmap);
			
			if (this.XAxisGridLines) 
			{
				foreach (float chartValue in this.mobj_axis_plotter.XTickMarks) 
				{
					int screenValue = (int) this.mobj_axis_plotter.XScreenPixel(chartValue);
					this.mobj_bitmap_graphics.DrawLine(pen, 
						screenValue, 0, screenValue, this.MaxChartAreaYPixel);
				}
			}
			if (this.YAxisGridLines) 
			{
				foreach (float chartValue in this.mobj_axis_plotter.YTickMarks) 
				{
					int screenValue = (int) this.mobj_axis_plotter.YScreenPixel(chartValue);
					this.mobj_bitmap_graphics.DrawLine(pen,  
						0, screenValue, this.MaxChartAreaXPixel, screenValue);
				}
			}
			this.mobj_bitmap_tools.MarkDifferences(mTempBitmap, mobj_bitmap, 
				mChartAreaInfoBitmap, ChartVisibilityBitmapConstants.Gridlines);
			pen.Dispose();
		}

		/// <summary>
		/// Called by the OnPaint method to paint each series of the 
		/// chart.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="data"></param>
		protected abstract void PaintSeries(Graphics g, Bitmap bitmap, clsSeries data);

		internal abstract void PaintLegendSymbol(Graphics g, clsSeries series, Rectangle into);

		internal abstract Size GetPreferredLegendSymbolSize(clsSeries series);


		/// <summary>
		/// Called by the OnPaint method after all of the series in the 
		/// chart have been painted through calls to PaintSeries.
		/// </summary>
		/// <param name="g"></param>
		protected virtual void EndPaintSeries(Graphics g) 
		{
		}

		#region Post Processors

		public void AddPostProcessor(ChartPostRenderingProcessor processor, 
			PostProcessPriority priority) 
		{
			if (processor == null) 
			{
				throw new ArgumentNullException("processor");
			}
			if ((int) priority > (int) PostProcessPriority.Highest || (int) priority < (int) PostProcessPriority.Lowest) 
			{
				throw new ArgumentException("Priority must be between PostProcessPriority.Highest and PostProcessPriority.Lowest", "priority");
			}
			this.mPostRenderingProcessors.Add(priority, processor);
			this.Invalidate();
		}

		public void RemovePostProcessor(ChartPostRenderingProcessor processor) 
		{
			int index = this.mPostRenderingProcessors.IndexOfValue(processor);
			if (index > 0) 
			{
				this.mPostRenderingProcessors.RemoveAt(index);
			}
			this.Invalidate();
		}

		private void RunPostProcessors(Graphics g) 
		{
			PostRenderEventArgs args = new PostRenderEventArgs(g);
			foreach (ChartPostRenderingProcessor processor in mPostRenderingProcessors.Values) 
			{
				processor(this, args);
			}
		}
		#endregion

		#endregion

		#region "Viewport Manipulation"

		[System.ComponentModel.Category("View Port")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Description("Determines whether the visible portion of the chart automatically resizes"
			 + " when a series is added, so that all data points are visible.")]
		public bool AutoViewPortOnAddition 
		{
			get 
			{
				return this.m_autoViewportOnAddition;
			}
			set 
			{
				this.m_autoViewportOnAddition = value;
			}
		}

		public void ResetAutoViewPortOnAddition() 
		{
			this.AutoViewPortOnAddition = false;
		}

		[System.ComponentModel.Category("View Port")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Description("If true, when zooming the y axis values will automatically "
			 + " be set so that all values in the selected X range are visible.")]
		public bool AutoViewPortYAxis 
		{
			get 
			{
				return this.m_autoViewportYAxis;
			}
			set 
			{
				this.m_autoViewportYAxis = value;
				if (this.m_autoViewportYAxis) 
				{
					this.ViewPort = this.ViewPort;
				}
			}
		}

		public void ResetAutoViewPortYAxis() 
		{
			this.AutoViewPortYAxis = false;
		}

		[System.ComponentModel.Category("View Port")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Description("If true, when automatically zooming the Y axis, the "
			 + "AutoViewPortYBase value will always be included on the chart.")]
		public bool UseAutoViewPortYBase
		{
			get 
			{
				return this.m_useAutoViewportYBase;
			}
			set 
			{
				this.m_useAutoViewportYBase = value;
				if (this.UseAutoViewPortYBase) 
				{
					this.ViewPort = this.ViewPort;
				}
			}
		}

		public void ResetUseAutoViewPortYBase()
		{
			this.UseAutoViewPortYBase = false;
		}

		[System.ComponentModel.Category("View Port")]
		[System.ComponentModel.DefaultValue(0)]
		[System.ComponentModel.Description("Determines the value that is always visible on the Y axis, "
			 + "when the AutoViewPortYAxis and UseAutoViewPortYBase are both true.")]
		public float AutoViewPortYBase
		{

			get 
			{
				return this.m_autoViewportYBase;
			}
			set 
			{
				this.m_autoViewportYBase = value;
				if (this.AutoViewPortYAxis && this.UseAutoViewPortYBase) 
				{
					this.ViewPort = this.ViewPort;
				}
			}
		}

		public void ResetAutoViewPortYBase() 
		{
			this.AutoViewPortYBase = 0;
		}

		[System.ComponentModel.Category("View Port")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Description("If true, when zooming the x axis values will automatically "
			 + " be set so that all values in the selected y range are visible.")]
		public bool AutoViewPortXAxis 
		{
			get 
			{
				return this.m_autoViewportXAxis;
			}
			set 
			{
				this.m_autoViewportXAxis = value;
				if (this.m_autoViewportXAxis) 
				{
					this.ViewPort = this.ViewPort;
				}
			}
		}

		public void ResetAutoViewPortXAxis() 
		{
			this.AutoViewPortXAxis = false;
		}

		[System.ComponentModel.Category("View Port")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Description("If true, when automatically zooming the X axis, the "
			 + "AutoViewPortXBase value will always be included on the chart.")]
		public bool UseAutoViewPortXBase
		{
			get 
			{
				return this.m_useAutoViewportXBase;
			}
			set 
			{
				this.m_useAutoViewportXBase = value;
				if (this.UseAutoViewPortXBase) 
				{
					this.ViewPort = this.ViewPort;
				}
			}
		}

		public void ResetUseAutoViewPortXBase()
		{
			this.UseAutoViewPortXBase = false;
		}

		[System.ComponentModel.Category("View Port")]
		[System.ComponentModel.DefaultValue(0)]
		[System.ComponentModel.Description("Determines the value that is always visible on the X axis, "
			 + "when the AutoViewPortXAxis and UseAutoViewPortXBase are both true.")]
		public float AutoViewPortXBase
		{

			get 
			{
				return this.m_autoViewportXBase;
			}
			set 
			{
				this.m_autoViewportXBase = value;
				if (this.AutoViewPortXAxis && this.UseAutoViewPortXBase) 
				{
					this.ViewPort = this.ViewPort;
				}
			}
		}

		public void ResetAutoViewPortXBase() 
		{
			this.AutoViewPortYBase = 0;
		}

		
		[System.ComponentModel.Category("View Port")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Description("Determines whether the visible portion of the chart "
			 + "automatically resizes when a series changes, so that all data points are visible.")]
		public bool AutoViewPortOnSeriesChange 
		{
			get 
			{
				return this.m_autoViewportOnSeriesChange;
			}
			set 
			{
				this.m_autoViewportOnSeriesChange = value;
			}
		}

		
		[System.ComponentModel.Category("View Port")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Description("The fraction of the screen that the y axis is expanded above the highest peak.")]
		public float VerticalExpansion
		{
			get
			{
				return mflt_vertical_scaling ; 
			}
			set
			{
				mflt_vertical_scaling = value ; 
				this.FullInvalidate() ; 
			}
		}


		public void  ResetAutoViewPortOnSeriesChange()
		{
			this.AutoViewPortOnSeriesChange = false;
		}

		/// <summary>
		/// Get or set the actively viewed region of the chart.
		/// </summary>
		[System.ComponentModel.TypeConverter(typeof(ViewPortConverter))]
		[System.ComponentModel.Category("View Port")]
		[System.ComponentModel.Description("Controls the bounds of the visible portion of the chart.")]
		[System.ComponentModel.Editor(typeof(PNNLControls.ViewPortEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public RectangleF ViewPort 
		{
			// All viewport setting is mediated through the ViewPortHistory
			get 
			{
				float yMin = this.mobj_axis_plotter.YMinValue;
				float height = this.mobj_axis_plotter.YMaxValue - yMin;
				float xMin = this.mobj_axis_plotter.XMinValue;
				float width = this.mobj_axis_plotter.XMaxValue - xMin;
				System.Drawing.RectangleF viewPort = 
					new System.Drawing.RectangleF(xMin, yMin, width, height);
				return viewPort;
			}
			set 
			{
				// Direct it out to viewport history - this will do verification 
				// and then call back to the ViewPortHistoryCurrentEntryChanged method/
				this.mViewPortHistory.CurrentEntry = value;
			}
		}

		/// <summary>
		/// Moves the viewport to the left by the specified fraction of the chart width.
		/// This is the fraction of the charting width, not the full control width.
		/// </summary>
		/// <param name="fractionOfScreenWidth"></param>
		public void MoveViewPortLeft(float fractionOfScreenWidth) 
		{
			int screenMinX = this.mobj_axis_plotter.ChartAreaBounds.Left;
			int screenMaxX = this.mobj_axis_plotter.ChartAreaBounds.Right;
			int movePixels = (int) (this.mobj_axis_plotter.ChartAreaBounds.Width * fractionOfScreenWidth);
			screenMinX = screenMinX - movePixels;
			screenMaxX = screenMaxX - movePixels;

			float xMin = this.GetChartX(this.GetChartAreaX(screenMinX));
			float xMax = this.GetChartX(this.GetChartAreaX(screenMaxX));
			float yMin = this.ViewPort.Top;
			float yMax = this.ViewPort.Bottom;
			this.ViewPort = new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		public void MoveViewPortRight(float fractionOfScreenWidth) 
		{
			MoveViewPortLeft(-fractionOfScreenWidth);
		}

		public void MoveViewPortUp(float fractionOfScreenWidth) 
		{
			int screenMinY = this.mobj_axis_plotter.ChartAreaBounds.Bottom;
			int screenMaxY = this.mobj_axis_plotter.ChartAreaBounds.Top;
			int movePixels = (int) (this.mobj_axis_plotter.ChartAreaBounds.Height * fractionOfScreenWidth);
			screenMinY = screenMinY - movePixels;
			screenMaxY = screenMaxY - movePixels;

			float xMin = this.ViewPort.Left;
			float xMax = this.ViewPort.Right;
			float yMin = this.GetChartY(this.GetChartAreaY(screenMinY));
			float yMax = this.GetChartY(this.GetChartAreaY(screenMaxY));
			this.ViewPort = new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		public void MoveViewPortDown(float fractionOfSceenWidth) 
		{
			MoveViewPortUp(-fractionOfSceenWidth);
		}

		/// <summary>
		/// Responds to the CurrentEntryChanged event of the viewport history.  This is the 
		/// method in which the actual setting of the viewport is done.  Whether the viewport 
		/// is set through the history or through the ViewPort property, the end code path 
		/// is to here. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewPortHistoryCurrentEntryChanged(Object sender, PNNLControls.CurrentEntryChangedEventArgs args) 
		{
			RectangleF rect = (RectangleF) this.mViewPortHistory.CurrentEntry;
			float xMin = rect.Left;
			float xMax = rect.Right;
			float yMin = rect.Top;
			float yMax = rect.Bottom;

			this.mobj_range.mflt_xstart = xMin;
			this.mobj_range.mflt_xend = xMax;
			this.mobj_range.mflt_yend = yMax;
			this.mobj_range.mflt_ystart = yMin;
			this.mobj_axis_plotter.SetRanges(xMin, xMax, yMin, yMax* mflt_vertical_scaling);
			if (this.ViewPortChanged != null) 
			{
				this.ViewPortChanged(this, new ViewPortChangedEventArgs(rect));
			}
			this.PerformLayout();
			this.FullInvalidate();
		}

		/// <summary>
		/// Gets the actual viewport that will be used, given the user's potential
		/// viewport.  Expands/contracts the viewport according to the settings of
		/// AutoViewPortXAxis and AutoViewPortYAxis.
		/// </summary>
		/// <param name="potentialViewPort"></param>
		/// <returns>The actual viewport to be used.</returns>
		internal RectangleF GetActualViewPort(RectangleF potentialViewPort) 
		{
			float yMin = potentialViewPort.Top;
			float yMax = potentialViewPort.Bottom;
			float xMin = potentialViewPort.Left;
			float xMax = potentialViewPort.Right;
			if (yMin >= yMax || xMin >= xMax) 
			{
				throw new ArgumentException("Viewport must have positive width and height");
			}

			if (this.AutoViewPortYAxis) 
			{
				yMin = float.MaxValue;
				yMax = float.MinValue;
				bool hasPoints = false;
				foreach (clsSeries data in this.mobj_series_collection) 
				{
					if (data.PlotParams.Visible) 
					{
						foreach (ChartDataPoint point in data.PlotData) 
						{
							if (point.x >= xMin && point.x <= xMax) 
							{
								hasPoints = true;
								yMin = Math.Min(yMin, point.y);
								yMax = Math.Max(yMax, point.y);
							}
						}
					}
				}
				// Make sure the base value is included, if this is desired
				if (this.UseAutoViewPortYBase) 
				{
					yMin = Math.Min(yMin, this.AutoViewPortYBase);
					yMax = Math.Max(yMax, this.AutoViewPortYBase);
				}
				// In the case that max and min are the same (only one point in view), use 
				// the values passed to this component
				if (yMin == yMax || ! hasPoints) 
				{
					yMin = potentialViewPort.Top;
					yMax = potentialViewPort.Bottom;
				}
			}

			if (this.AutoViewPortXAxis) 
			{
				xMin = float.MaxValue;
				xMax = float.MinValue;
				bool hasPoints = false;
				foreach (clsSeries data in this.mobj_series_collection) 
				{
					if (data.PlotParams.Visible) 
					{
						foreach (ChartDataPoint point in data.PlotData) 
						{
							if (point.y >= yMin && point.y <= yMax) 
							{
								xMin = Math.Min(xMin, point.x);
								xMax = Math.Max(xMax, point.x);
								hasPoints = true;
							}
						}
					}
				}
				// Make sure the base value is included, if this is desired
				if (this.UseAutoViewPortXBase) 
				{
					xMin = Math.Min(xMin, this.AutoViewPortXBase);
					xMax = Math.Max(xMax, this.AutoViewPortXBase);
				}
				// In the case that max and min are the same (only one point in view), use 
				// the values passed to this component
				if (xMin == xMax || ! hasPoints) 
				{
					xMin = potentialViewPort.Left;
					xMax = potentialViewPort.Right;
				}
			}
			return new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		[System.ComponentModel.Browsable(false)]
		public History ViewPortHistory 
		{
			get 
			{
				return this.mViewPortHistory;
			}
		}


        /// <summary>
        /// Gets or sets flag of whether to pad the plot with 
        /// </summary>
        public float PadViewPortX
        {
            get
            {
                return mfloat_autoViewportPaddingX;
            }
            set
            {
                mfloat_autoViewportPaddingX = value;
            }
        }
        public float PadViewPortY
        {
            get
            {
                return mfloat_autoViewportPaddingY;
            }
            set
            {
                mfloat_autoViewportPaddingY = value;
            }
        }

		/// <summary>
		/// Automatically set the viewport so that all data is displayed.
		/// </summary>
		public void AutoViewPort() 
		{
			float xMin = float.MaxValue;
			float xMax = float.MinValue;
			float yMin = float.MaxValue;
			float yMax = float.MinValue;
			
			foreach (clsSeries series in this.mobj_series_collection) 
			{
				if (series.PlotParams.Visible) 
				{
					ChartDataPoint[] dataPoints = series.PlotData;
				
					foreach (ChartDataPoint point in dataPoints) 
					{
						float x = point.x;
						float y = point.y;
						xMin = Math.Min(xMin, x);
						xMax = Math.Max(xMax, x);
						yMin = Math.Min(yMin, y);
						yMax = Math.Max(yMax, y);
					}
				}
			}

            /// 
            /// Give in a little on autoviewport , allow for .1 on either side
            /// 
            float xdif = (xMax - xMin) * mfloat_autoViewportPaddingX;
            float ydif = (yMax - yMin) * mfloat_autoViewportPaddingY;

            xMin = xMin - xdif;
            xMax = xMax + xdif;
            yMin = yMin - ydif;
            yMax = yMax + ydif;

			if (xMin == xMax || yMin == yMax || xMin == float.MaxValue || xMax == float.MinValue 
				|| yMin == float.MaxValue || yMax == float.MinValue) 
			{
				this.ViewPort = this.ViewPort;
			}
			else 
			{
				this.ViewPort = new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin);
			}
			//Console.WriteLine("Auto Viewport {0} {1} {2} {3}", xMin, xMax, yMin, yMax);
		}

		#endregion

		#region "Helpers"
		/// <summary>
		/// Get a pixel offset (relative to the upper-left of the
		/// chart - i.e. point at top of y axis) for the given x value.
		/// </summary>
		/// <param name="xValue"></param>
		/// <returns></returns>
		public float GetScreenPixelX(float xValue) 
		{            
			// Ask axis where it should be
			return this.mobj_axis_plotter.XScreenPixel(xValue);
		}

		public float GetScreenPixelY(float yValue) 
		{
			return this.mobj_axis_plotter.YScreenPixel(yValue);
		}

		public float GetPixelWidth(float width)
		{
			return this.mobj_axis_plotter.PixelWidth(width) ; 
		}

		public float GetPixelHeight(float height)
		{
			return this.mobj_axis_plotter.PixelHeight(height) ; 
		}

		protected Derek.BitmapTools.PixelData PixelDataFromColor(Color c) 
		{
			Derek.BitmapTools.PixelData data = new Derek.BitmapTools.PixelData();
			data.red = c.R;
			data.blue = c.B;
			data.green = c.G;
			return data;
		}

		/// <summary>
		/// Gets the type of chart area that a point is in.  Relative to upper-left of
		/// control.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public ChartLocation GetLocation(Point p) 
		{
			ChartLocation location = ChartLocation.None;
			if (this.Legend.ActualBounds.Contains(p)) 
			{
				location |= ChartLocation.Legend;
			}
			if (this.m_titlePlotter.Bounds.Contains(p)) 
			{
				location |= ChartLocation.Title;
			}
			return location |= this.mobj_axis_plotter.GetChartLocation(p);
		}

		/// <summary>
		/// Gets the chart x-coordinate for the given on-screen x pixel coordinate.
		/// </summary>
		/// <param name="xPixel">The x pixel coordinate, relative to the left edge of the
		/// chart area, not the entire control.</param>
		/// <returns></returns>
		public float GetChartX(int xPixel) 
		{
			return this.mobj_axis_plotter.XChartCoordinate(xPixel);
		}

		public float GetChartY(int yPixel) 
		{
			return this.mobj_axis_plotter.YChartCoordinate(yPixel);
		}

		/// <summary>
		/// Get the x pixel coordinate relative to the upper-left of the 
		/// chart, given a pixel coordinate relative to the upper-left of the 
		/// control.
		/// </summary>
		/// <param name="xPixel"></param>
		/// <returns></returns>
		public int GetChartAreaX(int xPixel) 
		{
			return xPixel - this.mobj_axis_plotter.ChartAreaBounds.X;
		}

		/// <summary>
		/// Get the y pixel coordinate relative to the upper-left of the chart, 
		/// given a pixel coordinate relative to the upper-left of the control.
		/// </summary>
		/// <param name="yPixel"></param>
		/// <returns></returns>
		public int GetChartAreaY(int yPixel) 
		{
			return yPixel - this.mobj_axis_plotter.ChartAreaBounds.Y;
		}

		[System.ComponentModel.Browsable(false)]
		public int MaxChartAreaXPixel
		{
			get 
			{
				return this.mobj_axis_plotter.ChartAreaBounds.Width;
			}
		}

		[System.ComponentModel.Browsable(false)]
		public int  MaxChartAreaYPixel 
		{
			get 
			{
				return this.mobj_axis_plotter.ChartAreaBounds.Height;
			}
		}
		#endregion

		#region "Properties"
		#region "Copy/Paste"
		[Category("Copy/Paste")]
		[Description("Controls whether copying of display parameters from another chart is enabled")]
		[DefaultValue(true)]
		public bool ParametersCopyEnabled 
		{
			get 
			{
				return mParametersCopyEnabled;
			}
			set 
			{
				mParametersCopyEnabled = value;
			}
		}

		[Category("Copy/Paste")]
		[Description("Controls whether copying of series from another chart is enabled")]
		[DefaultValue(true)]
		public bool SeriesCopyEnabled 
		{
			get 
			{
				return mSeriesCopyEnabled;
			}
			set 
			{
				mSeriesCopyEnabled = value;
			}
		}

		[Category("Copy/Paste")]
		[Description("Controls whether pasting of series from another chart is enabled")]
		[DefaultValue(true)]
		public bool SeriesPasteEnabled 
		{
			get 
			{
				return mSeriesPasteEnabled;
			}
			set 
			{
				mSeriesPasteEnabled = value;
			}
		}

		[Category("Copy/Paste")]
		[Description("Controls whether pasting of display parameters from another chart is enabled")]
		[DefaultValue(true)]
		public bool ParametersPasteEnabled 
		{
			get 
			{
				return mParametersPasteEnabled;
			}
			set 
			{
				mParametersPasteEnabled = value;
			}
		}
		#endregion

		[Category("Appearance")]
		[Description("Controls whether focused dashed rectangle is drawn.")]
		[DefaultValue(true)]
		public bool ShowFocus 
		{
			get 
			{
				return mShowFocus;
			}
			set 
			{
				mShowFocus = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
			/// <summary>
			/// The set of plot params used to control
			/// </summary>
		private clsPlotParams PlotParams 
		{
			get 
			{
				return this.m_plotParams;
			}
		}

		[System.ComponentModel.Category("Chart Behavior")]
		[System.ComponentModel.DefaultValue(true)]
		[System.ComponentModel.Description("Controls whether panning of the chart using the " +
			 "arrow keys is enabled or not")]
		public bool PanWithArrowKeys 
		{
			get 
			{
				return this.mPanWithArrowKeys;
			}
			set 
			{
				this.mPanWithArrowKeys = value;
			}
		}

		/// <summary>
		/// The color used for selected/hilighted series.
		/// </summary>
		[Category("Appearance")]
		[Description("The color used for selected series.")]
		public Color HilightColor 
		{
			get 
			{
				return mHilightColor;
			}
			set 
			{
				mHilightColor = value;
				this.Invalidate();
			}
		}


		[System.ComponentModel.Category("Chart Behavior")]
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
		public ChartZoomHandler DefaultZoomHandler 
		{
			get 
			{
				return this.m_defaultZoomHandler;
			}
		}

		[System.ComponentModel.Category("Chart Layout")]
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
		public ChartLayout ChartLayout 
		{
			get 
			{
				return this.m_layout;
			}
		}

		[System.ComponentModel.Category("Chart Layout")]
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
		[System.ComponentModel.Description("The various margins around the areas of the chart")]
		public clsMargins Margins 
		{
			get 
			{
				return this.mobj_margins;
			}
		}

		[System.ComponentModel.Description("The background color of the chart area on which data is drawn.")]
		[System.ComponentModel.Category("Chart Appearance")]
		public Color ChartBackgroundColor
		{
			get 
			{
				return this.m_chartBackgroundBrush.Color;
			}
			set 
			{
				this.m_chartBackgroundBrush.Color = value;
				this.FullInvalidate();
			}
		}

		public void ResetChartBackgroundColor()
		{
			this.ChartBackgroundColor = Color.White;
		}

		[System.ComponentModel.Category("Chart Layout")]
		[System.ComponentModel.Description("Controls whether the chart has a title or not")]
		[System.ComponentModel.DefaultValue(true)]
		public bool HasLegend
		{
			get 
			{
				return this.m_hasLegend;
			} 
			set 
			{
				if (value != HasLegend) 
				{
					this.m_hasLegend = value;
					this.menuLegendFloating.Enabled = value;
					this.menuLegendShow.Checked = value;
					this.menuLegendLeft.Enabled = value;
					this.menuLegendRight.Enabled = value;
					this.menuLegendTop.Enabled = value;
					this.menuLegendBottom.Enabled = value;
					this.PerformLayout();
				}
			}
		}

		public void ResetHasLegend() 
		{
			this.HasLegend = true;
		}

		[System.ComponentModel.Category("Chart Appearance")]
		[System.ComponentModel.DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ChartLegend Legend 
		{
			get 
			{
				return this.m_legend;
			} 
		}

		#region "Headings"

		[System.ComponentModel.Category("Chart Appearance")]
		[System.ComponentModel.Description("Tells whether title and axis fonts should automatically "
			 + " be sized for a best fit.")]
		[System.ComponentModel.DefaultValue(true)]
		public bool AutoSizeFonts 
		{
			get 
			{
				return this.m_autoSizeFonts;
			}
			set 
			{
				// If in init, do not pass values directly to sub-components, just store 
				// value for end of init.
				this.m_autoSizeFonts = value;
				if (!this.mInInit) 
				{
					//Console.WriteLine("Autosizing fonts: {0}", value);
					this.mobj_axis_plotter.AutoSizeFonts = value;
					this.m_titlePlotter.AutoSize = value;
					this.PerformLayout();
				}
			}
		}

		public void ResetAutoSizeFonts() 
		{
			this.AutoSizeFonts = true;
		}

		[System.ComponentModel.Category("Chart Appearance")]
		[System.ComponentModel.Description("The font used to render the title")]
		public Font TitleFont 
		{
			get 
			{
				return this.m_titlePlotter.Font;
			}
			set 
			{
				this.m_titlePlotter.Font = value;
				this.PerformLayout();
			}
		}

		[System.ComponentModel.Category("Chart Appearance")]
		public float TitleMinFontSize 
		{
			get 
			{
				return this.m_titlePlotter.MinFontSize;
			}
			set 
			{
				this.m_titlePlotter.MinFontSize = value;
			}
		}

		[System.ComponentModel.Category("Chart Appearance")]
		public float TitleMaxFontSize 
		{
			get 
			{
				return this.m_titlePlotter.MaxFontSize;
			}
			set 
			{
				this.m_titlePlotter.MaxFontSize = value;
			}
		}

		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
				this.m_titlePlotter.Font = new Font(this.Font.FontFamily, this.m_titlePlotter.Font.Size, 
					this.Font.Style);
				this.m_legend.Font = new Font(this.Font.FontFamily, this.m_legend.Font.Size, this.Font.Style);
				this.mobj_axis_plotter.LabelAndUnitFont =
					new Font(this.Font.FontFamily, this.mobj_axis_plotter.LabelAndUnitFont.Size, this.Font.Style);
				this.PerformLayout();
			}
		}


		[System.ComponentModel.Category("Chart Axes")]
		public int AxisAndLabelMinFontSize 
		{
			get 
			{
				return this.mobj_axis_plotter.MinFontSize;
			}
			set 
			{
				this.mobj_axis_plotter.MinFontSize = value;
				using (Graphics g = this.CreateGraphics()) 
				{
					this.mobj_axis_plotter.Layout(g, this.mobj_margins);
				}
				this.Invalidate();
			}
		}

		[System.ComponentModel.Category("Chart Axes")]
		public int AxisAndLabelMaxFontSize 
		{
			get 
			{
				return this.mobj_axis_plotter.MaxFontSize;
			}
			set 
			{
				if (value > this.mobj_axis_plotter.MaxFontSize)
					this.mobj_axis_plotter.MaxFontSize = value; 
				this.mobj_axis_plotter.FontSize = value ; 

				using (Graphics g = this.CreateGraphics()) 
				{
					this.mobj_axis_plotter.Layout(g, this.mobj_margins);
				}
				this.Invalidate();
			}
		}

		[System.ComponentModel.Category("Chart Axes")]
		[System.ComponentModel.Description("The font used to render the axis and unit labels")]
		public Font AxisAndLabelFont 
		{
			get 
			{
				return this.mobj_axis_plotter.LabelAndUnitFont;
			}
			set 
			{
				this.mobj_axis_plotter.LabelAndUnitFont = value;
				this.PerformLayout();
			}
		}
		
		[System.ComponentModel.Category("Chart Labeling")]
		[System.ComponentModel.Description("The title at the top of the chart")]
		public String Title 
		{
			get 
			{
				return this.m_titlePlotter.Label;
			}
			set 
			{
				this.m_titlePlotter.Label = value;
				this.PerformLayout();
			}
		}

		[System.ComponentModel.Category("Chart Labeling")]
		[System.ComponentModel.Description("The label for the X axis.")]
		[System.ComponentModel.DefaultValue("X Axis")]
		public String XAxisLabel
		{
			get 
			{
				return this.mobj_axis_plotter.XAxisLabel;
			} 
			set 
			{
				this.mobj_axis_plotter.XAxisLabel = value;
				this.PerformLayout();
			}
		}

		[System.ComponentModel.Category("Chart Labeling")]
		[System.ComponentModel.Description("The label for the Y axis.")]
		[System.ComponentModel.DefaultValue("Y Axis")]
		public String YAxisLabel
		{
			get 
			{
				return this.mobj_axis_plotter.YAxisLabel;
			}
			set 
			{
				this.mobj_axis_plotter.YAxisLabel = value;
				this.PerformLayout();
			}
		}

		#endregion

		#region "GridLines"
		[System.ComponentModel.Description("Controls how grid lines are drawn.")]
		[System.ComponentModel.Category("Grid Lines")]
		public PenProvider GridLinePen
		{
			get 
			{
				return this.m_gridLinePenProvider;
			} 
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("GridLinePen");
				}
				if (this.m_gridLinePenProvider != null) 
				{
					this.m_gridLinePenProvider.PenChanged -= this.m_gridLinePenProviderChangedHandler;
				}
				this.m_gridLinePenProvider = value;
				this.m_gridLinePenProvider.PenChanged += this.m_gridLinePenProviderChangedHandler;
			}
		}

		private void GridLinePenProviderChanged(object sender, EventArgs args) 
		{
			//System.Windows.Forms.MessageBox.Show("Pen provider changed " + this.m_gridLinePenProvider.ToString());
			this.FullInvalidate();
		}

		public void ResetGridLinePen() 
		{
			this.m_gridLinePenProvider.Pen = new Pen(Color.LightGray, 1);
		}

		[System.ComponentModel.Description("Controls whether grid lines are drawn or not.")]
		[System.ComponentModel.DefaultValue(true)]
		[System.ComponentModel.Category("Grid Lines")]
		public bool XAxisGridLines 
		{
			get 
			{
				return m_xAxisDrawGridLines;
			}
			set 
			{
				if (this.XAxisGridLines != value) 
				{
					m_xAxisDrawGridLines = value;
					this.FullInvalidate();
				}
			}
		}

		public void ResetXAxisGridLines() 
		{
			this.XAxisGridLines = true;
		}

		[System.ComponentModel.Description("Controls whether grid lines are drawn or not.")]
		[System.ComponentModel.DefaultValue(true)]
		[System.ComponentModel.Category("Grid Lines")]
		public bool YAxisGridLines 
		{
			get 
			{
				return m_yAxisDrawGridLines;
			}
			set 
			{
				if (this.YAxisGridLines != value) 
				{
					m_yAxisDrawGridLines = value;
					this.FullInvalidate();
				}
			}
		}

		public void ResetYAxisGridLines() 
		{
			this.YAxisGridLines = true;
		}

		[System.ComponentModel.Description("Controls whether gridlines are drawn above or below the series data.")]
		[System.ComponentModel.DefaultValue(ChartLayer.UnderSeries)]
		[System.ComponentModel.Category("Grid Lines")]
		public ChartLayer GridLineLayer
		{
			get 
			{
				return m_gridLinesLayer;
			}
			set 
			{
				if (this.GridLineLayer != value) 
				{
					m_gridLinesLayer = value;
					this.FullInvalidate();
				}
			}
		}

		public void ResetGridLineLayer() 
		{
			this.GridLineLayer = ChartLayer.UnderSeries;
		}

		#endregion
		#endregion

		#region "Events"
		//		protected override void OnResize(EventArgs e)
		//		{
		//			base.OnResize(e);
		//			Console.WriteLine("Resized");
		//			//this.PerformLayout();
		//		}

		private void PlotParamsChanged(Object sender, PNNLControls.PlotParamsChangedEventArgs args) 
		{
			// make sure all series have a valid plot params
			for (int i = 0; i < this.mobj_series_collection.Count; i++) 
			{
				clsSeries series = this.mobj_series_collection[i];
				if (series.PlotParams == null) 
				{
					series.PlotParams = this.CreatePlotParams(DEFAULT_SERIES_COLORS[i]);
				}
			}
			PNNLControls.PlotParamField field = args.Field;
			// Depending on the field, update all of the series in the chart to use 
			// the same params - this is only done for some parameters, not the ones
			// like name, main color, etc.
			foreach (clsSeries series in this.mobj_series_collection) 
			{
				if (field == PlotParamField.Shape) 
				{
					series.PlotParams.Shape = this.PlotParams.Shape;
				}
				else if (field == PlotParamField.Visible) 
				{
					series.PlotParams.Visible = this.PlotParams.Visible;
				}
			}
		}

		private void SeriesChanged(Object sender, SeriesChangedEventArgs args) 
		{
			Console.WriteLine("Series Changed {0} {1}",((clsSeries) sender).PlotParams.Name, args.Cause);
			if (this.AutoViewPortOnSeriesChange && args.Cause == SeriesChangedCause.Data) 
			{
				this.AutoViewPort();
			}
			this.SeriesChanged(true);
			this.PerformLayout();
		}

		private void SeriesChanged(bool fullInvalidate) 
		{
			// make sure all series have a valid plot params
			for (int i = 0; i < this.mobj_series_collection.Count; i++) 
			{
				clsSeries series = this.mobj_series_collection[i];
				if (series.PlotParams == null) 
				{
					series.PlotParams = this.CreatePlotParams(DEFAULT_SERIES_COLORS[i]);
					fullInvalidate = true;
				}
			}
			if (fullInvalidate) 
			{
				this.FullInvalidate();
			} 
			else 
			{
				this.Invalidate();
			}
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			DateTime start = DateTime.Now;
			base.OnLayout(levent);
			using (Graphics g = this.CreateGraphics()) 
			{
				Rectangle initialPlotBounds = this.mobj_axis_plotter.ChartAreaBounds;
				SeriesChanged(false);
                
				Rectangle titleBounds = this.m_layout.TitleBounds();
				if (titleBounds != this.m_titlePlotter.Bounds) 
				{
					this.m_titlePlotter.Bounds = titleBounds;
				}

				Rectangle legendBounds = this.m_layout.LegendBounds();
				if (legendBounds != this.m_legend.Bounds) 
				{
					this.m_legend.Bounds = legendBounds;
				}

				Rectangle plotBounds = this.m_layout.PlotBounds();
				if (plotBounds != this.mobj_axis_plotter.Bounds) 
				{
					this.mobj_axis_plotter.Bounds = plotBounds;
				}
				
				m_titlePlotter.Layout(g);
				m_legend.Layout(g);
				mobj_axis_plotter.Layout(g, this.mobj_margins);
				mMarkerLayer.Layout(g);
				
				if ((initialPlotBounds.Width != this.mobj_axis_plotter.ChartAreaBounds.Width) ||
					(initialPlotBounds.Height != this.mobj_axis_plotter.ChartAreaBounds.Height)) 
				{
					this.FullInvalidate();
				} 
				else 
				{
					this.Invalidate();
				}
			}
		}
		
		protected void FullInvalidate() 
		{
			this.mRecreateHilightMask = true;
			this.m_repaintSeries = true;
			this.Invalidate();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus (e);
			if (this.ShowFocus) 
			{
				this.Invalidate();
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus (e);
			if (this.ShowFocus) 
			{
				this.Invalidate();
			}
		}
		#endregion

		#region "Series Manipulation"

		/// <summary>
		/// Called when an int is added to the selected series set.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void mSeriesSelected_IntAdded(object sender, IntChangedEventArgs args) 
		{
			this.mRecreateHilightMask = true;
			this.Invalidate();
		}

		/// <summary>
		/// Called when an int is removed from the selected series set.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void mSeriesSelected_IntRemoved(object sender, IntChangedEventArgs args) 
		{
			this.mRecreateHilightMask = true;
			this.Invalidate();
		}

		/// <summary>
		/// Called by clsSeriesCollection when a series is added to the list.
		/// </summary>
		/// <param name="series"></param>
		internal void SeriesAdded(clsSeries series) 
		{
			series.SeriesChanged += this.m_seriesChangedHandler;
			// Clear the list of indexes of selected series
			this.mSelectedSeries.Clear();
			if (this.AutoViewPortOnAddition) 
			{
				this.AutoViewPort();
			}
			this.PerformLayout();
			this.FullInvalidate();
		}

		/// <summary>
		/// Called by clsSeriesCollection when a series is removed from the list.
		/// </summary>
		/// <param name="series"></param>
		internal void SeriesRemoved(clsSeries series) 
		{
			series.SeriesChanged -= this.m_seriesChangedHandler;
			this.mSelectedSeries.Clear();
			this.PerformLayout();
			this.FullInvalidate();
		}

		public clsSeriesCollection SeriesCollection 
		{
			get 
			{
				return this.mobj_series_collection;
			}
		}

		private delegate void dlgAddSeries(clsSeries series) ; 
		public virtual void AddSeries(PNNLControls.clsSeries series)
		{
			if (this.InvokeRequired)
				this.Invoke(new dlgAddSeries(this.AddSeries), new object[]{series}) ; 
			else
			{
				SeriesCollection.Add(series) ; 
			}
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.mContextMenu = new System.Windows.Forms.ContextMenu();
            this.menuShowAllData = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuLegend = new System.Windows.Forms.MenuItem();
            this.menuLegendShow = new System.Windows.Forms.MenuItem();
            this.menuLegendDivider = new System.Windows.Forms.MenuItem();
            this.menuLegendLeft = new System.Windows.Forms.MenuItem();
            this.menuLegendRight = new System.Windows.Forms.MenuItem();
            this.menuLegendBottom = new System.Windows.Forms.MenuItem();
            this.menuLegendTop = new System.Windows.Forms.MenuItem();
            this.menuLegendFloating = new System.Windows.Forms.MenuItem();
            this.menuPropertyGrid = new System.Windows.Forms.MenuItem();
            this.menuViewPort = new System.Windows.Forms.MenuItem();
            this.menuEditViewPort = new System.Windows.Forms.MenuItem();
            this.menuClearViewPortHistory = new System.Windows.Forms.MenuItem();
            this.menuItemSeperator1 = new System.Windows.Forms.MenuItem();
            this.menuItemCopy = new System.Windows.Forms.MenuItem();
            this.menuItemPaste = new System.Windows.Forms.MenuItem();
            this.menuItemPasteByReference = new System.Windows.Forms.MenuItem();
            this.menuItemCopyImage = new System.Windows.Forms.MenuItem();
            this.menuItemSaveImage = new System.Windows.Forms.MenuItem();
            this.menuItemSelectAllSeries = new System.Windows.Forms.MenuItem();
            this.menuItemSelectedSeries = new System.Windows.Forms.MenuItem();
            this.menuItemDelete = new System.Windows.Forms.MenuItem();
            this.menuItemDisplayOptions = new System.Windows.Forms.MenuItem();
            this.menuItemSeriesVisible = new System.Windows.Forms.MenuItem();
            this.menuItemBringToFront = new System.Windows.Forms.MenuItem();
            this.menuItemBringToBack = new System.Windows.Forms.MenuItem();
            this.menuItemSeriesSpecific = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mContextMenu
            // 
            this.mContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuShowAllData,
            this.menuItem1,
            this.menuLegend,
            this.menuPropertyGrid,
            this.menuViewPort,
            this.menuItemSeperator1,
            this.menuItemCopy,
            this.menuItemPaste,
            this.menuItemPasteByReference,
            this.menuItem2,
            this.menuItemCopyImage,
            this.menuItemSaveImage,
            this.menuItem3,
            this.menuItemSelectAllSeries,
            this.menuItemSelectedSeries,
            this.menuItemSeriesSpecific});
            this.mContextMenu.Popup += new System.EventHandler(this.mContextMenu_Popup);
            // 
            // menuShowAllData
            // 
            this.menuShowAllData.Index = 0;
            this.menuShowAllData.Text = "Show All Data";
            this.menuShowAllData.Click += new System.EventHandler(this.menuShowAllData_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            this.menuItem1.Text = "-";
            // 
            // menuLegend
            // 
            this.menuLegend.Index = 2;
            this.menuLegend.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuLegendShow,
            this.menuLegendDivider,
            this.menuLegendLeft,
            this.menuLegendRight,
            this.menuLegendBottom,
            this.menuLegendTop,
            this.menuLegendFloating});
            this.menuLegend.MergeOrder = 10;
            this.menuLegend.Text = "Legend";
            // 
            // menuLegendShow
            // 
            this.menuLegendShow.Checked = true;
            this.menuLegendShow.Index = 0;
            this.menuLegendShow.Text = "Show";
            this.menuLegendShow.Click += new System.EventHandler(this.menuLegendShow_Click);
            // 
            // menuLegendDivider
            // 
            this.menuLegendDivider.Index = 1;
            this.menuLegendDivider.Text = "-";
            // 
            // menuLegendLeft
            // 
            this.menuLegendLeft.Index = 2;
            this.menuLegendLeft.Text = "Left";
            this.menuLegendLeft.Click += new System.EventHandler(this.menuLegendLeft_Click);
            // 
            // menuLegendRight
            // 
            this.menuLegendRight.Index = 3;
            this.menuLegendRight.Text = "Right";
            this.menuLegendRight.Click += new System.EventHandler(this.menuLegendRight_Click);
            // 
            // menuLegendBottom
            // 
            this.menuLegendBottom.Index = 4;
            this.menuLegendBottom.Text = "Bottom";
            this.menuLegendBottom.Click += new System.EventHandler(this.menuLegendBottom_Click);
            // 
            // menuLegendTop
            // 
            this.menuLegendTop.Index = 5;
            this.menuLegendTop.Text = "Top";
            this.menuLegendTop.Click += new System.EventHandler(this.menuLegendTop_Click);
            // 
            // menuLegendFloating
            // 
            this.menuLegendFloating.Index = 6;
            this.menuLegendFloating.Text = "Floating";
            this.menuLegendFloating.Click += new System.EventHandler(this.menuLegendFloating_Click);
            // 
            // menuPropertyGrid
            // 
            this.menuPropertyGrid.Index = 3;
            this.menuPropertyGrid.MergeOrder = 10;
            this.menuPropertyGrid.Text = "Properties";
            this.menuPropertyGrid.Click += new System.EventHandler(this.menuPropertyGrid_Click);
            // 
            // menuViewPort
            // 
            this.menuViewPort.Index = 4;
            this.menuViewPort.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuEditViewPort,
            this.menuClearViewPortHistory});
            this.menuViewPort.MergeOrder = 10;
            this.menuViewPort.Text = "View Port";
            // 
            // menuEditViewPort
            // 
            this.menuEditViewPort.Index = 0;
            this.menuEditViewPort.Text = "Edit...";
            this.menuEditViewPort.Click += new System.EventHandler(this.menuViewPortEdit_Click);
            // 
            // menuClearViewPortHistory
            // 
            this.menuClearViewPortHistory.Index = 1;
            this.menuClearViewPortHistory.Text = "Clear History";
            this.menuClearViewPortHistory.Click += new System.EventHandler(this.menuClearViewPortHistory_Click);
            // 
            // menuItemSeperator1
            // 
            this.menuItemSeperator1.Index = 5;
            this.menuItemSeperator1.MergeOrder = 10;
            this.menuItemSeperator1.Text = "-";
            // 
            // menuItemCopy
            // 
            this.menuItemCopy.Index = 6;
            this.menuItemCopy.MergeOrder = 10;
            this.menuItemCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.menuItemCopy.Text = "Copy";
            this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
            // 
            // menuItemPaste
            // 
            this.menuItemPaste.Index = 7;
            this.menuItemPaste.MergeOrder = 10;
            this.menuItemPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this.menuItemPaste.Text = "Paste";
            this.menuItemPaste.Click += new System.EventHandler(this.menuItemPaste_Click);
            // 
            // menuItemPasteByReference
            // 
            this.menuItemPasteByReference.Index = 8;
            this.menuItemPasteByReference.MergeOrder = 10;
            this.menuItemPasteByReference.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftV;
            this.menuItemPasteByReference.Text = "Paste by Reference";
            this.menuItemPasteByReference.Click += new System.EventHandler(this.menuItemPasteByReference_Click);
            // 
            // menuItemCopyImage
            // 
            this.menuItemCopyImage.Index = 10;
            this.menuItemCopyImage.MergeOrder = 10;
            this.menuItemCopyImage.Text = "Copy Image";
            this.menuItemCopyImage.Click += new System.EventHandler(this.menuItemCopyImage_Click);
            // 
            // menuItemSaveImage
            // 
            this.menuItemSaveImage.Index = 11;
            this.menuItemSaveImage.MergeOrder = 10;
            this.menuItemSaveImage.Text = "Save Image";
            this.menuItemSaveImage.Click += new System.EventHandler(this.menuItemSaveImage_Click);
            // 
            // menuItemSelectAllSeries
            // 
            this.menuItemSelectAllSeries.Index = 13;
            this.menuItemSelectAllSeries.MergeOrder = 10;
            this.menuItemSelectAllSeries.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.menuItemSelectAllSeries.Text = "Select All Series";
            this.menuItemSelectAllSeries.Click += new System.EventHandler(this.menuItemSelectAllSeries_Click);
            // 
            // menuItemSelectedSeries
            // 
            this.menuItemSelectedSeries.Index = 14;
            this.menuItemSelectedSeries.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDelete,
            this.menuItemDisplayOptions,
            this.menuItemSeriesVisible,
            this.menuItemBringToFront,
            this.menuItemBringToBack});
            this.menuItemSelectedSeries.MergeOrder = 10;
            this.menuItemSelectedSeries.Text = "Selected Series";
            // 
            // menuItemDelete
            // 
            this.menuItemDelete.Index = 0;
            this.menuItemDelete.Shortcut = System.Windows.Forms.Shortcut.Del;
            this.menuItemDelete.Text = "Delete";
            this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
            // 
            // menuItemDisplayOptions
            // 
            this.menuItemDisplayOptions.Index = 1;
            this.menuItemDisplayOptions.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
            this.menuItemDisplayOptions.Text = "Display Options";
            this.menuItemDisplayOptions.Click += new System.EventHandler(this.menuItemDisplayOptions_Click);
            // 
            // menuItemSeriesVisible
            // 
            this.menuItemSeriesVisible.Index = 2;
            this.menuItemSeriesVisible.Text = "Visible";
            this.menuItemSeriesVisible.Click += new System.EventHandler(this.menuItemSeriesVisible_Click);
            // 
            // menuItemBringToFront
            // 
            this.menuItemBringToFront.Index = 3;
            this.menuItemBringToFront.Text = "Bring to Front";
            this.menuItemBringToFront.Click += new System.EventHandler(this.menuItemBringToFront_Click);
            // 
            // menuItemBringToBack
            // 
            this.menuItemBringToBack.Index = 4;
            this.menuItemBringToBack.Text = "Send to Back";
            this.menuItemBringToBack.Click += new System.EventHandler(this.menuItemBringToBack_Click);
            // 
            // menuItemSeriesSpecific
            // 
            this.menuItemSeriesSpecific.Index = 15;
            this.menuItemSeriesSpecific.MergeOrder = 10;
            this.menuItemSeriesSpecific.Text = "Series Specific";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 9;
            this.menuItem2.Text = "-";
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 12;
            this.menuItem3.Text = "-";
            // 
            // ctlChartBase
            // 
            this.ContextMenu = this.mContextMenu;
            this.Name = "ctlChartBase";
            this.Size = new System.Drawing.Size(236, 212);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ctlChartBase_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ctlChartBase_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ctlChartBase_MouseUp);
            this.ResumeLayout(false);

		}
		#endregion

		private Point mMouseBegin;
		private ChartClickState mChartClickState = ChartClickState.Normal;
		private Point mLegendTopLeft;
		private Size mLegendSize;
		private enum ChartClickState
		{
			LegendMove,
			LegendSize,
			Normal,
			//Handled
		}
		
		private void ctlChartBase_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (this.mChartClickState == ChartClickState.LegendMove) 
			{
				this.Legend.Bounds = new Rectangle(this.mLegendTopLeft.X + e.X - this.mMouseBegin.X,
					this.mLegendTopLeft.Y + e.Y - this.mMouseBegin.Y, 
					this.Legend.Bounds.Width,
					this.Legend.Bounds.Height);
				this.PerformLayout();
			}
			else if (this.mChartClickState == ChartClickState.LegendSize) 
			{
				this.Legend.Bounds = new Rectangle(this.mLegendTopLeft.X, this.mLegendTopLeft.Y, 
					this.mLegendSize.Width + e.X - this.mMouseBegin.X,
					this.mLegendSize.Height + e.Y - this.mMouseBegin.Y);
				this.PerformLayout();
			} 
			else if (this.mChartClickState == ChartClickState.Normal) 
			{
				if (this.m_legend.IsLegendSizeSelection(e)) 
				{
					this.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
				}
				else if (this.m_legend.IsLegendMoveSelection(e)) 
				{
					this.Cursor = System.Windows.Forms.Cursors.SizeAll;
				}
				else 
				{
					this.Cursor = System.Windows.Forms.Cursors.Default;
				}
			}													  
		}

		private void ctlChartBase_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Point p = new Point(e.X, e.Y);
			ChartLocation location = this.GetLocation(p);
			if (this.mChartClickState == ChartClickState.LegendMove) 
			{
				this.mChartClickState = ChartClickState.Normal;
				this.Cursor = System.Windows.Forms.Cursors.Default;
				this.Legend.Bounds = new Rectangle(this.mLegendTopLeft.X + e.X - this.mMouseBegin.X,
					this.mLegendTopLeft.Y + e.Y - this.mMouseBegin.Y, 
					this.Legend.Bounds.Width,
					this.Legend.Bounds.Height);
				this.PerformLayout();
			}
//			if (this.mChartClickState == ChartClickState.Normal) 
//			{
//
//				ChartLocation loc = this.GetLocation(new Point(e.X, e.Y));
//				if ((loc & ChartLocation.Legend) != 0) 
//				{
//					this.m_legend.ProcessMouseClick(e);
//				}
//				else if ((loc & (ChartLocation.Title | ChartLocation.XLabel | ChartLocation.YLabel)) != 0) 
//				{
//					//					frmChartTitlesEditor editor = new frmChartTitlesEditor();
//					//					editor.Title = this.Title;
//					//					editor.YAxisLabel = this.YAxisLabel;
//					//					editor.XAxisLabel = this.XAxisLabel;
//					//					editor.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
//					//					Point mouseAt = System.Windows.Forms.Control.MousePosition;
//					//					editor.DesktopLocation = new Point(mouseAt.X - 50, mouseAt.Y - 50);
//					//					editor.ShowDialog();
//					//					this.Title = editor.Title;
//					//					this.XAxisLabel = editor.XAxisLabel;
//					//					this.YAxisLabel = editor.YAxisLabel;
//					//					editor.Dispose();
//				}
//			}
			if (this.mChartClickState == ChartClickState.LegendSize) 
			{
				this.mChartClickState = ChartClickState.Normal;
				this.Cursor = System.Windows.Forms.Cursors.Default;
				this.Legend.Bounds = new Rectangle(this.mLegendTopLeft.X, this.mLegendTopLeft.Y, 
					this.mLegendSize.Width + e.X - this.mMouseBegin.X,
					this.mLegendSize.Height + e.Y - this.mMouseBegin.Y);
				this.PerformLayout();
			}
		}

		protected virtual void ctlChartBase_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// If Shift or Control not pressed, then a left click clears the current series 
			// selection.  Shift and control are considered the same for multiple selection 
			// purposes.
			if (!(Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control)
				&& e.Button == MouseButtons.Left)
			{
				this.mSelectedSeries.Clear();
			}

            if (mChartAreaInfoBitmap == null)
                return;

			// First consider selection of series
			if (GetLocation(new Point(e.X, e.Y)) == ChartLocation.ChartArea)
			{
				// Get the type of object shown on the chart at the given point
				int val = mChartAreaInfoBitmap.GetPixel(e.X - mobj_axis_plotter.ChartAreaBounds.X, 
					e.Y - mobj_axis_plotter.ChartAreaBounds.Y).ToArgb();

				// If the chart has a series at the given point, then alternate whether the 
				// series is selected or not.
				if (ChartVisibilityBitmapConstants.IsSeries(val)) 
				{
					if (e.Button == MouseButtons.Left) 
					{
						this.mSelectedSeries.Alternate(
							ChartVisibilityBitmapConstants.SeriesIndexFromInt(val));
					} 
					else if (e.Button == MouseButtons.Right) 
					{
						this.mSelectedSeries.Alternate(
							ChartVisibilityBitmapConstants.SeriesIndexFromInt(val));
					}

                    SeriesSelectedAtPoint(ChartVisibilityBitmapConstants.SeriesIndexFromInt(val), e.X, e.Y);
				}
			} 

			Point p = new Point(e.X, e.Y);
			this.mChartClickState = ChartClickState.Normal;
			ChartLocation location = this.GetLocation(p);
			if ((location & ChartLocation.Legend) != 0 && e.Button == System.Windows.Forms.MouseButtons.Left) 
			{
				if (this.m_legend.IsLegendSizeSelection(e)) 
				{
					this.mChartClickState = ChartClickState.LegendSize;
					this.mMouseBegin = p;
					this.mLegendTopLeft = new Point(this.Legend.ActualBounds.Left, this.Legend.ActualBounds.Top);
					this.mLegendSize = new Size(this.m_legend.ActualBounds.Width, this.m_legend.ActualBounds.Height);
					this.ChartLayout.LegendLocation = ChartLegendLocation.Floating;
					this.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
				} 
				else if (this.m_legend.IsLegendMoveSelection(e)) 
				{
					this.mChartClickState = ChartClickState.LegendMove;
					this.mMouseBegin = p;
					this.mLegendTopLeft = new Point(this.Legend.Bounds.Left, this.Legend.Bounds.Top);
					this.ChartLayout.LegendLocation = ChartLegendLocation.Floating;
					this.Cursor = System.Windows.Forms.Cursors.SizeAll;
				} 
			}
			// Do selection of series through legend
			if (this.m_legend.SeriesAt(e.X, e.Y) != null) 
			{
				this.mChartClickState = ChartClickState.Normal;
				int index = this.SeriesCollection.IndexOf(this.m_legend.SeriesAt(e.X, e.Y));
				if (index >= 0 && index < this.SeriesCollection.Count) 
				{
					if (e.Button == MouseButtons.Left) 
					{
						this.mSelectedSeries.Alternate(index);
					} 
					else if (e.Button == MouseButtons.Right) 
					{
						this.mSelectedSeries.Add(index);
					}
				}
			}
		}


        /// <summary>
        /// Virtual method telling the listening form that the given series at point defined in X,Y was selected.
        /// </summary>
        /// <param name="series">Series index selected</param>
        /// <param name="x">Y coordinate selected</param>
        /// <param name="y">Y coordinate selected</param>
        protected virtual void SeriesSelectedAtPoint(int series, int x, int y)
        {
            /// This method does nothing.
        }

		private void menuLegendFloating_Click(object sender, System.EventArgs e)
		{
			this.ChartLayout.LegendLocation = ChartLegendLocation.Floating;
		}

		private void menuLegendTop_Click(object sender, System.EventArgs e)
		{
			this.ChartLayout.LegendLocation = ChartLegendLocation.UnderTitle;
		}

		private void menuLegendBottom_Click(object sender, System.EventArgs e)
		{
			this.ChartLayout.LegendLocation = ChartLegendLocation.Bottom;
		}

		private void menuLegendRight_Click(object sender, System.EventArgs e)
		{
			this.ChartLayout.LegendLocation = ChartLegendLocation.Right;
		}

		private void menuEditPlotParams_Click(object sender, System.EventArgs e)
		{
			PNNLControls.frmPlotParams editor = new frmPlotParams();
			editor.PlotParams = this.PlotParams;
			editor.CancelEnabled = false;
			System.Windows.Forms.DialogResult result = editor.ShowDialog();
			editor.Dispose();
		}

		private void menuLegendLeft_Click(object sender, System.EventArgs e)
		{
			this.ChartLayout.LegendLocation = ChartLegendLocation.Left;
		}

		private void menuPropertyGrid_Click(object sender, System.EventArgs e)
		{
			frmPropertyGrid fpg = new frmPropertyGrid();
			fpg.SelectedObject = this;
			fpg.ShowDialog();
		}

		internal void LegendLocationChanged() 
		{
			this.menuLegendLeft.Checked = false;
			this.menuLegendRight.Checked = false;
			this.menuLegendTop.Checked = false;
			this.menuLegendBottom.Checked = false;
			this.menuLegendFloating.Checked = false;
			if (this.m_layout != null) 
			{
				switch (this.m_layout.LegendLocation) 
				{
					case ChartLegendLocation.Bottom: 
						this.menuLegendBottom.Checked = true;
						break;
					case ChartLegendLocation.Floating:
						this.menuLegendFloating.Checked = true;
						break;
					case ChartLegendLocation.Left:
						this.menuLegendLeft.Checked = true;
						break;
					case ChartLegendLocation.Right:
						this.menuLegendRight.Checked = true;
						break;
					case ChartLegendLocation.UnderTitle:
						this.menuLegendTop.Checked = true;
						break;
				}
			}
		}

		private void menuLegendShow_Click(object sender, System.EventArgs e)
		{
			this.HasLegend = !this.HasLegend;
		}

		private void menuShowAllData_Click(object sender, System.EventArgs e)
		{
			this.AutoViewPort();
		}

		private void menuViewPortEdit_Click(object sender, System.EventArgs e)
		{
			frmViewPortEditor fvpe = new frmViewPortEditor();
			fvpe.SelectedValue = this.ViewPort;
			fvpe.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			Point mouseAt = System.Windows.Forms.Control.MousePosition;
			fvpe.DesktopLocation = new Point(mouseAt.X - 50, mouseAt.Y - 50);
			fvpe.ShowDialog();
			if (fvpe.DialogResult == DialogResult.OK) 
			{
				this.ViewPort = fvpe.SelectedValue;
			}
			fvpe.Dispose();
		}

		#region ISupportInitialize Members

		private bool mInInit = false;
		public void BeginInit()
		{
			this.mInInit = true;
		}

		public void EndInit()
		{
			this.mInInit = false;
			// Once VS InitializeComponent finishes, the ViewPort will be set.
			// Having the default [0, 1][0, 1] viewport in the history list looks bad, 
			// so clear the history after initialization.
			this.mViewPortHistory.Clear();
			// Set the AutoSizeFonts option to the value set by the user
			this.AutoSizeFonts = this.AutoSizeFonts;
		}

		#endregion

		/// <summary>
		/// Merges the given menu with the currently set context menu (the defualt plus 
		/// whatever has been added to it.)
		/// </summary>
		public override ContextMenu ContextMenu
		{
			get
			{
				return base.ContextMenu;
			}
			set
			{
				if (value == null) 
				{
					return;
				}
				if (this.ContextMenu != null) 
				{
					this.ContextMenu.MergeMenu(value);
				}
				else 
				{
					base.ContextMenu = value;
				}
			}
		}

		/// <summary>
		/// Removes the built-in context menu for chart operations.
		/// </summary>
		public void RemoveDefaultContextMenu() 
		{
			base.ContextMenu = null;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			Keys keysWithoutModifiers = keyData & ~Keys.Modifiers;
			// Detect arrow keys as input, otherwise the form uses arrow keys to switch the
			// active control
			if (keysWithoutModifiers == Keys.Left || keysWithoutModifiers == Keys.Right ||
				keysWithoutModifiers == Keys.Up || keysWithoutModifiers == Keys.Down) 
			{
				return true;
			}
			return base.IsInputKey(keyData);
		}

		[System.ComponentModel.DefaultValue(.1F)]
		[System.ComponentModel.Description("The fraction of the chart area by which the chart is panned "
			 + " when the arrow keys are pressed.")]
		public float PanFraction 
		{
			get 
			{
				return this.mPanFraction;
			}
			set 
			{
				if (value <= 0)
				{ 
					throw new ArgumentOutOfRangeException("PanFraction", value, "Pan fraction must be > 0");
				}
				this.mPanFraction = value;
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (!e.Handled && this.PanWithArrowKeys) 
			{
				float moveMultiplier = 1;
				if(e.Modifiers == Keys.Shift) 
				{
					moveMultiplier = 10;
				} 
				else if (e.Modifiers == Keys.Control) 
				{
					moveMultiplier = .1f;
				}
				switch(e.KeyCode) 
				{
					case Keys.Left:
						e.Handled = true;
						this.MoveViewPortLeft(this.PanFraction * moveMultiplier);
						break;
					case Keys.Right : 
						e.Handled = true;
						this.MoveViewPortRight(this.PanFraction * moveMultiplier);
						break;
					case Keys.Up :
						e.Handled = true;
						this.MoveViewPortUp(this.PanFraction * moveMultiplier);
						break;
					case Keys.Down :
						e.Handled = true;
						this.MoveViewPortDown(this.PanFraction * moveMultiplier);
						break;
				}
			}
			if (!e.Handled && e.KeyCode == Keys.Back)
			{
				if (e.Modifiers == Keys.None) 
				{
					e.Handled = true;
					this.mViewPortHistory.MoveBack(false);
				} 
				else if (e.Modifiers == Keys.Shift) 
				{
					e.Handled = true;
					this.mViewPortHistory.MoveForward(false);
				}
			}
			base.OnKeyDown(e);
		}

		private void menuClearViewPortHistory_Click(object sender, System.EventArgs e)
		{
			this.mViewPortHistory.Clear();
		}


		private void MarginsChanged(object sender, EventArgs e)
		{
			this.PerformLayout();
			this.FullInvalidate();
		}

		/// <summary>
		/// Copies the chart or currently selected series to the application 
		/// clipboard.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItemCopy_Click(object sender, System.EventArgs e)
		{
			// No selected series, copy chart to application clipboard, 
			// allows for copying of settings.
			if (mSelectedSeries.Values.Count == 0) 
			{
				Metafile mf = this.ToMetafile() ; 

				try
				{
					ClipboardMetafileHelper.PutEnhMetafileOnClipboard(this.Handle, mf) ; 
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message + ex.StackTrace) ; 
				}
			}
				// Has selected series, so copy as an array of clsSeries objects
			else 
			{
				int count = mSelectedSeries.Values.Count;
				clsSeries[] seriesToCopy = new clsSeries[count];
				int index = 0;
				foreach (int i in mSelectedSeries.Values) 
				{
					seriesToCopy[index++] = this.mobj_series_collection[i];
				}
				ApplicationClipboard.SetData(seriesToCopy);
			}
		}

		private void menuItemPaste_Click(object sender, System.EventArgs e)
		{
			// First check whether the clipboard has a ctlChartBase from which to 
			// copy settings
			ctlChartBase chart = (ctlChartBase) ApplicationClipboard.GetData(typeof(ctlChartBase));
			if (chart != null) 
			{
				CopySettings(chart);
			}
			// Next see if there are any clsSeries arrays available for copying
			clsSeries[] series = (clsSeries[]) ApplicationClipboard.GetData(typeof(clsSeries[]));
			if (series != null) 
			{
				PasteSeries(series, false);
			}
		}

		protected virtual void PasteSeries(clsSeries[] series, bool byReference) 
		{
			foreach (clsSeries s in series) 
			{
				clsSeries newSeries = s;
				// Get a copy of the series if specified by the argument, or the series
				// doesn't want to be copied by reference.
				if (!byReference || !s.CopyByReferenceAllowed) 
				{
					newSeries = s.CopySeries();
				}
                
                /// 
                /// Modified this to add new series values 
                /// 
                AddSeries(newSeries);
				//this.SeriesCollection.Add(newSeries);				
			}
		}

		/// <summary>
		/// Allows derived classes to do custom copying of settings
		/// </summary>
		/// <param name="other"></param>
		protected virtual void CopySettings(ctlChartBase other) 
		{			
			// only copy viewport related settings for the current time
			this.AutoViewPortOnAddition = other.AutoViewPortOnAddition;
			this.AutoViewPortOnSeriesChange = other.AutoViewPortOnSeriesChange;
			this.UseAutoViewPortXBase = other.UseAutoViewPortXBase;
			this.UseAutoViewPortYBase = other.UseAutoViewPortYBase;
			this.AutoViewPortXAxis = other.AutoViewPortXAxis;
			this.AutoViewPortXBase = other.AutoViewPortXBase;
			this.AutoViewPortYAxis = other.AutoViewPortYAxis;
			this.AutoViewPortYBase = other.AutoViewPortYBase;
			this.ViewPort = other.ViewPort;
		}

		private void menuItemSelectAllSeries_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < SeriesCollection.Count; i++) 
			{
				this.mSelectedSeries.Add(i);
			}
		}

		public clsSeries[] SelectedSeries 
		{
			get 
			{
				int count = this.mSelectedSeries.Values.Count;
				clsSeries[] series = new clsSeries[count];
				int index = 0;
				foreach (int i in this.mSelectedSeries.Values) 
				{
					series[index++] = SeriesCollection[i];
				}
				return series;
			}
		}

		/// <summary>
		/// Enables/Disables certain menu items on popup and merges in any custom 
		/// context menu for the given series.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mContextMenu_Popup(object sender, System.EventArgs e)
		{
			bool hasSelectedSeries = this.mSelectedSeries.Values.Count > 0;
			if (HasSelectedSeries) 
			{
				this.menuItemSelectedSeries.Enabled = true;
				// Set whether the visible menu item should be checked or unchecked
				bool visible = true;
				foreach (clsSeries s in SelectedSeries) 
				{
					visible &= s.PlotParams.Visible;
				}
				this.menuItemSeriesVisible.Checked = visible;
				bool deletable = false;
				foreach (clsSeries s in SelectedSeries) 
				{
					deletable |= s.Deletable(this);
				}
				this.menuItemDelete.Enabled = deletable;
				// Enable copy items or not
				if (this.SeriesCopyEnabled) 
				{
					this.menuItemCopy.Enabled = true;
				}
				else 
				{
					this.menuItemCopy.Enabled = false;
				}
			}
			else 
			{
				// Turn off series specific items
				this.menuItemSelectedSeries.Enabled = false;
				this.menuItemPasteByReference.Enabled = false;
				// Enable/disable copy and paste items
				if (ApplicationClipboard.HasData(typeof(ctlChartBase)) && this.ParametersPasteEnabled) 
				{
					this.menuItemPaste.Enabled = true;
				}
				else 
				{
					this.menuItemPaste.Enabled = false;
				}
			}
			// Enable/Disable paste items
			if (ApplicationClipboard.HasData(typeof(clsSeries[])) && this.SeriesPasteEnabled)
			{
				this.menuItemPaste.Enabled = true;
				this.menuItemPasteByReference.Enabled = true;
			} 
			else if(ApplicationClipboard.HasData(typeof(ctlChartBase)) && this.ParametersPasteEnabled) 
			{
				this.menuItemPaste.Enabled = true;
				this.menuItemPasteByReference.Enabled = false;
			}
			else 
			{
				this.menuItemPaste.Enabled = false;
				this.menuItemPasteByReference.Enabled = false;
			}
			// Put any custom menu items for the series in the SeriesSpecial item
			if (HasOneSeriesSelected && SelectedSeries[0].GetCustomMenuItems(this).Length > 0)
			{
				// Remove any items from previous showing
				this.menuItemSeriesSpecific.MenuItems.Clear();
				// Copy series specific menu items in
				foreach (MenuItem item in SelectedSeries[0].GetCustomMenuItems(this)) 
				{
					this.menuItemSeriesSpecific.MenuItems.Add(item.CloneMenu());
				}
				this.menuItemSeriesSpecific.Enabled = true;
			} 
			else 
			{
				// None or more than 1 series selected, or no custom menu items, disable the 
				// series specific item
				this.menuItemSeriesSpecific.Enabled = false;
			}
		}

		/// <summary>
		/// Tells whether any series are currently selected
		/// </summary>
		/// <returns></returns>
		private bool HasSelectedSeries 
		{
			get 
			{
				return this.mSelectedSeries.Values.Count > 0;
			}
		}

		private bool HasOneSeriesSelected 
		{
			get 
			{
				return this.mSelectedSeries.Values.Count == 1;
			}
		}

		private void menuItemDisplayOptions_Click(object sender, System.EventArgs e)
		{
			if (SelectedSeries.Length > 0) 
			{
				clsPlotParams plotParams = SelectedSeries[0].PlotParams;
				frmPlotParams form = new frmPlotParams();
				form.PlotParams = plotParams;
				if (form.ShowDialog() == DialogResult.OK) 
				{
					plotParams = form.PlotParams;
					foreach (clsSeries series in SelectedSeries) 
					{
						series.PlotParams = plotParams;
					}
				}
				form.Dispose();
			}
		}

		private void menuItemPasteByReference_Click(object sender, System.EventArgs e)
		{
			// See if there are any clsSeries arrays available for copying
			clsSeries[] series = (clsSeries[]) ApplicationClipboard.GetData(typeof(clsSeries[]));
			if (series != null) 
			{
				PasteSeries(series, true);
			}
		}

		private void menuItemDelete_Click(object sender, System.EventArgs e)
		{
			this.SuspendLayout();
			foreach (clsSeries series in this.SelectedSeries) 
			{
				if (series.Deletable(this)) 
				{
					this.SeriesCollection.Remove(series);
				}
			}
			this.ResumeLayout();
		}

		private void menuItemSeriesVisible_Click(object sender, System.EventArgs e)
		{
			foreach (clsSeries series in this.SelectedSeries) 
			{
				clsPlotParams p = series.PlotParams;
				p.Visible = !this.menuItemSeriesVisible.Checked;
				series.PlotParams = p;
			}
		}

		private void menuItemBringToFront_Click(object sender, System.EventArgs e)
		{
			// Turn off autozoom, since z-order change is done by adding and 
			// removing series.  We don't want the viewport to change during 
			// this time.
			bool autoZoom = AutoViewPortOnAddition;
			AutoViewPortOnAddition = false;
			foreach(clsSeries series in this.SelectedSeries) 
			{
				this.SeriesCollection.Remove(series);
				this.SeriesCollection.Add(series);
			}
			AutoViewPortOnAddition = autoZoom;
		}

		private void menuItemBringToBack_Click(object sender, System.EventArgs e)
		{
			// Turn off autozoom, since z-order change is done by adding and 
			// removing series.  We don't want the viewport to change during 
			// this time.
			bool autoZoom = AutoViewPortOnAddition;
			AutoViewPortOnAddition = false;
			foreach(clsSeries series in this.SelectedSeries) 
			{
				this.SeriesCollection.Remove(series);
				this.SeriesCollection.Insert(0, series);
			}
			AutoViewPortOnAddition = autoZoom;
		}

		private void menuItemCopyImage_Click(object sender, System.EventArgs e)
		{
			try 
			{
				DataObject dataObj = new DataObject();
				dataObj.SetData(System.Windows.Forms.DataFormats.Bitmap, ToBitmap());
				ApplicationClipboard.ClearApplicationClipboard();
				Clipboard.SetDataObject(dataObj);
			} 
			catch (Exception ex) 
			{
				MessageBox.Show("Image Copy Failed: " + ex.Message);
			}
		}

		public Bitmap ToBitmap() 
		{
            Bitmap image = new Bitmap(this.Width, this.Height);            
			using (Graphics g = Graphics.FromImage(image)) 
			{
				DrawToGraphics(g, false, true);
			}
			return image;
		}
		public Metafile ToMetafile() 
		{ 

			// create a Metafile object that is compatible with the surface of this 
			// form
			Graphics graphics = this.CreateGraphics() ;
			System.IntPtr hdc = graphics.GetHdc(); 
			Bitmap tempBitmap = new Bitmap(mobj_bitmap) ; 
			Metafile metafile = new Metafile(hdc, new Rectangle( 0, 0,(int) this.Width, (int) this.Height), MetafileFrameUnit.Point, EmfType.EmfPlusDual ); 
			
			// draw to the metafile
			Graphics metafileGraphics           = Graphics.FromImage( metafile ) ;
			metafileGraphics.SmoothingMode      = SmoothingMode.HighQuality; // smooth the 
            metafileGraphics.InterpolationMode  = InterpolationMode.HighQualityBicubic;

			System.Drawing.Drawing2D.GraphicsContainer container = metafileGraphics.BeginContainer();
			metafileGraphics.IntersectClip(this.mobj_axis_plotter.ChartAreaBounds);
			metafileGraphics.TranslateTransform(this.mobj_axis_plotter.ChartAreaBounds.Left, 
				this.mobj_axis_plotter.ChartAreaBounds.Top);

            

			// paint each series
			for (int i = 0; i < this.mobj_series_collection.Count; i++)
			{
				//Bitmap oldBitmap = (Bitmap) mobj_bitmap.Clone();
				clsSeries data = this.mobj_series_collection[i];
				if (data.PlotParams.Visible) 
				{
					this.PaintSeries(metafileGraphics, tempBitmap, data);
					this.mobj_bitmap_tools.MarkDifferences(tempBitmap, mTempBitmap, 
						this.mChartAreaInfoBitmap, ChartVisibilityBitmapConstants.SeriesStart + i);
				}
			}

			// end series painting
			EndPaintSeries(metafileGraphics);

			// draw the marker layer, any text labels, etc.
			this.mMarkerLayer.Draw(metafileGraphics);

			// allow registered drawing routines to take a crack at doing special drawing
			// on the chart area
			this.RunPostProcessors(metafileGraphics);
			metafileGraphics.EndContainer(container);

			// paint axes
			DrawAxes(metafileGraphics);
			this.m_titlePlotter.Draw(metafileGraphics, this.ForeColor);
            if (mbool_legendVisible == true)
			    this.m_legend.Draw(metafileGraphics, this.ForeColor);

			metafileGraphics.DrawImage(tempBitmap, this.mobj_axis_plotter.ChartAreaBounds.Left, 
				this.mobj_axis_plotter.ChartAreaBounds.Top) ; 
			graphics.ReleaseHdc(hdc); 
			graphics.Dispose() ;
			metafileGraphics.Dispose() ; 
			return metafile;
		}

		public Bitmap ToBitmap(int width, int height) 
		{
			int tempWidth   = this.Width ; 
			int tempHeight  = this.Height ; 
			Bitmap image    = new Bitmap(width, height);
			this.Width      = width ; 
			this.Height     = height ; 
			using (Graphics g = Graphics.FromImage(image)) 
			{
				DrawToGraphics(g, false, true);
			}
			this.Width      = tempWidth ; 
			this.Height     = tempHeight ; 
			return image;
		}
        /// <summary>
        /// Displays a save dialog box and allows the user to set the dpi and name for the image.
        /// </summary>
        /// <param name="format">Format to save image in.</param>
        /// <param name="selector">Save dialog filter.</param>
        /// <param name="defaultExtension">Extension of file to save.</param>
		private void Save() 
		{

            frmSaveDPI frmSave = new frmSaveDPI();
            try
            {
                

                if (m_saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string name      = m_saveFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(name);

                Image    image      = null;
                Metafile metaImage  = null;

                switch (extension.ToLower())
                {
                    case ".jpg":
                        image = ToBitmap();                        
                        image.Save(name, ImageFormat.Jpeg);
                        break;
                    case ".tiff":
                        image = ToBitmap();
                        image.Save(name, ImageFormat.Tiff);
                        break;
                    case ".gif":
                        image = ToBitmap();
                        image.Save(name, ImageFormat.Gif);
                        break;
                    case ".bmp":
                        image = ToBitmap();
                        image.Save(name, ImageFormat.Bmp);
                        break;
                    case ".wmf":
                        metaImage = ToMetafile();
                        metaImage.Save(name, ImageFormat.Wmf);
                        break;
                    case ".emf":
                    default:
                        metaImage = ToMetafile();
                        metaImage.Save(name, ImageFormat.Emf);                                                
                        break;
                }
            }
            catch (Exception e)
            {

                MessageBox.Show("Save failed: " + e.Message);
            }
            finally
            {
                frmSave.Dispose();
            }
		}

		#region "Marker Layer"
		public ChartMarkerLayer MarkerLayer 
		{
			get 
			{
				return mMarkerLayer;
			}
		}
		#endregion


        private void menuItemSaveImage_Click(object sender, EventArgs e)
        {
            Save();
        }
	}
	
	/// <summary>
	/// Lists constants used for the 
	/// </summary>
	public sealed class ChartVisibilityBitmapConstants 
	{
		public readonly static int BackGround  = 0x00000000;
		public readonly static int Gridlines   = 0x00000001;
		public readonly static int SeriesStart = 0x00F00000;
		public readonly static int SeriesEnd   = 0x00FFFFFF;
		public readonly static int MarkerStart = 0x00100000;
		public readonly static int MarkerEnd   = 0x001FFFFF;
		private readonly static int mask = 0x00FFFFFF;

		public static int MarkerIndexFromInt(int value) 
		{
			if (value < MarkerStart || value > MarkerEnd) 
			{
				throw new ArgumentException("Value does not represent a marker.");
			}
			return (int) value - MarkerStart;
		}

		public static int SeriesIndexFromInt(int value) 
		{
			if (!IsSeries(value)) 
			{
				throw new ArgumentException("Value does not represent a series.");
			}
			return (int) Masked(value) - SeriesStart;
		}

		public static bool IsSeries(int value) 
		{
			return Masked(value) >= SeriesStart && Masked(value) <= SeriesEnd;
		}

		private static int Masked(int val) 
		{
			return val & mask;
		}
	}
}
