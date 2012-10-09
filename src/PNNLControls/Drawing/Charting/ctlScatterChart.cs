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
		/// Required designer variable.
		/// </summary>
        private System.ComponentModel.Container components = null;
        
        /// <summary>
        /// Constructor
        /// </summary>
		public ctlScatterChart()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint & ControlStyles.UserPaint &
				ControlStyles.SupportsTransparentBackColor & ControlStyles.DoubleBuffer, true);			
			InitializeComponent();
		    this.Title = "Scatter Chart";
                                    
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

        
        public override void AddSeries(clsSeries series)
        {
            //FindExtrema(series);
            base.AddSeries(series);
        }

        


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
