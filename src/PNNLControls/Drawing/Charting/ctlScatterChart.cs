using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using PNNLControls.Drawing.Charting;

namespace PNNLControls
{

	/// <summary>
	/// Summary description for ctlScatter.
	/// </summary>
	public class ctlScatterChart : ctlChartBase
	{
        /// <summary>
        /// Fired when the filter button is pressed.
        /// </summary>
        public event System.EventHandler Filter;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
        private System.ComponentModel.Container components = null;
        /// <summary>
        /// A filter value determining how to filter the x-data.
        /// </summary>
        private classPointFilter mobj_filterX;
        /// <summary>
        /// A filter value determining how to filter the y-data.
        /// </summary>
        private classPointFilter mobj_filterY;

        /// <summary>
        /// Form that provides users a way to change how the data is filtered.
        /// </summary>
        private formLinearPointFilter mform_filterSettings;

        /// <summary>
        /// Constructor
        /// </summary>
		public ctlScatterChart()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint & ControlStyles.UserPaint &
				ControlStyles.SupportsTransparentBackColor & ControlStyles.DoubleBuffer, true);			
			InitializeComponent();
		    this.Title = "Scatter Chart";

            classLinearPointFilter filterx = new classLinearPointFilter(double.MaxValue, double.MinValue);
            filterx.Invert = false;

            classLinearPointFilter filtery = new classLinearPointFilter(double.MaxValue, double.MinValue);
            filtery.Invert = false;

            mobj_filterX = filterx;
            mobj_filterY = filtery;

            MenuItem item = new MenuItem();
            item.Name = "Filtering";
            item.Text = "Filters";
            item.Click += new EventHandler(item_Click);
            ContextMenu.MenuItems.Add(item);

            mform_filterSettings = new formLinearPointFilter(filterx, filtery, double.MinValue, double.MaxValue, double.MinValue, double.MaxValue);
            mform_filterSettings.UpdatedFilters += new EventHandler(mform_filterSettings_UpdatedFilters);


            DefaultZoomHandler.AddDrawingDelegate(new DrawingZoomRegion(DrawZoomValues));
        }

        protected virtual void DrawZoomValues(ctlChartBase chart, RectangleF rect, RectangleF bounds, Graphics graphics)
        {
            // do nothing.
        }

        void mform_filterSettings_UpdatedFilters(object sender, EventArgs e)
        {
            /// 
            /// FORCE A DAMN UPDATE!
            /// 
            m_repaintSeries = true;
            Invalidate(this.Region, true);

            Refresh();         
        }

        /// <summary>
        /// Handles when the user clicks to setup the filter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void item_Click(object sender, EventArgs e)
        {
            if (Filter != null)
            {
                Filter(this, new classFilterEventArgs(mobj_filterX, mobj_filterY));
            }
            else
            {
                classLinearPointFilter filterx = mobj_filterX as classLinearPointFilter;
                classLinearPointFilter filtery = mobj_filterY as classLinearPointFilter;

                mform_filterSettings.Text = "Filter Settings";
                mform_filterSettings.Icon = ParentForm.Icon;
                mform_filterSettings.ShowDialog();                
            }
        }

        /// <summary>
        /// Given a series, this will find the extrema values for all series in the collection.
        /// </summary>
        /// <param name="series"></param>
        private void FindExtrema(clsSeries series)
        {
            classLinearPointFilter filterx = mobj_filterX as classLinearPointFilter;
            classLinearPointFilter filtery = mobj_filterY as classLinearPointFilter;

            float minx = Convert.ToSingle(filterx.Minimum);
            float maxx = Convert.ToSingle(filterx.Maximum);
            float miny = Convert.ToSingle(filtery.Minimum);
            float maxy = Convert.ToSingle(filtery.Maximum);

            foreach (ChartDataPoint point in series.PlotData)
            {
                maxx = Math.Max(point.x, maxx);
                maxy = Math.Max(point.y, maxy);
                minx = Math.Min(point.x, minx);
                miny = Math.Min(point.y, miny);
            }


            filterx.Minimum = Convert.ToDouble(minx);
            filterx.Maximum = Convert.ToDouble(maxx);
            filtery.Minimum = Convert.ToDouble(miny);
            filtery.Maximum = Convert.ToDouble(maxy);

            mform_filterSettings.MinX = Convert.ToDouble(minx);
            mform_filterSettings.MaxX = Convert.ToDouble(maxx);
            mform_filterSettings.MinY = Convert.ToDouble(miny);
            mform_filterSettings.MaxY = Convert.ToDouble(maxy);
            mform_filterSettings.UpdateValues();
        }

        public override void AddSeries(clsSeries series)
        {
            //FindExtrema(series);
            base.AddSeries(series);
        }

        #region Properties
        /// <summary>
        /// Gets or sets the filtering X object.
        /// </summary>
        public classPointFilter FilterX
        {
            get
            {
                return mobj_filterX;
            }
            set
            {
                mobj_filterX = value;
            }
        }
        /// <summary>
        /// Gets or sets the filtering X object.
        /// </summary>
        public classPointFilter FilterY
        {
            get
            {
                return mobj_filterY;
            }
            set
            {
                mobj_filterY = value;
            }
        }
        #endregion


        # region Painting        
        protected override void PaintSeries(Graphics g, Bitmap bitmap, clsSeries series)
		{
			DateTime start = DateTime.Now;
			// translate graph coordinates into coordinates on bitmap
			ChartDataPoint[] dataPoints = series.PlotData;
			ArrayList points = new ArrayList(dataPoints.Length);
			int maxChartY = this.MaxChartAreaYPixel;
			int maxChartX = this.MaxChartAreaXPixel;
			foreach (ChartDataPoint dataPoint in dataPoints) 
			{
				float dataX = dataPoint.x;
				float dataY = dataPoint.y;
                
                //if (mobj_filterX.Filter(dataX) && mobj_filterY.Filter(dataY))
                {
			        // translate dataX and dataY into bitmap offset x and y from the 
			        // upper left corner
			        int plotXValue = (int) this.mobj_axis_plotter.XScreenPixel(dataX);
			        int plotYValue = (int) this.mobj_axis_plotter.YScreenPixel(dataY);
			        if (plotXValue >= 0 && plotXValue <= maxChartX &&
				        plotYValue >= 0 && plotYValue <= maxChartY) 
			        {
                        /// 
                        /// Perform filtering here
                        /// 
                        points.Add(new ChartDataPlotPoint(plotXValue, plotYValue, dataPoint.color));                        
			        }
                }
			}
			mobj_bitmap_tools.DrawDots(points, series.PlotParams.Shape, bitmap);
		}
		#endregion

        /// <summary>
        /// Returns the preferred lenged size.
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
		internal override Size GetPreferredLegendSymbolSize(clsSeries series)
		{
			int height = series.PlotParams.Shape.Size * 2;
			int width = series.PlotParams.Shape.Size * 2;
			return new Size(width + 4, height + 4);
		}

        /// <summary>
        /// Handles painting the legend symbol.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="series"></param>
        /// <param name="bounds"></param>
		internal override void PaintLegendSymbol(Graphics g, clsSeries series, Rectangle bounds)
		{
			System.Drawing.Drawing2D.GraphicsContainer container = g.BeginContainer();
			// center the image on the bounds
			int centerX = (bounds.Left + bounds.Right) / 2;
			int centerY = (bounds.Top + bounds.Bottom) / 2;
			g.IntersectClip(bounds);
			g.TranslateTransform(centerX, centerY);
			series.PlotParams.DrawShape(g);
			g.EndContainer(container);
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
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}

    public class classFilterEventArgs : EventArgs
    {
        private classPointFilter mobj_filterx;
        private classPointFilter mobj_filtery;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filter"></param>
        public classFilterEventArgs(classPointFilter filterx, classPointFilter filtery)
        {
            mobj_filterx = filterx;
            mobj_filtery = filtery;
        }

        /// <summary>
        /// Gets or sets the current y point filter.
        /// </summary>
        public classPointFilter FilterX
        {
            get
            {
                return mobj_filterx;
            }
            set
            {
                mobj_filterx = value;
            }
        }
        /// <summary>
        /// Gets or sets the current x point filter.
        /// </summary>
        public classPointFilter FilterY
        {
            get
            {
                return mobj_filtery;
            }
            set
            {
                mobj_filtery = value;
            }
        }
    }

}
