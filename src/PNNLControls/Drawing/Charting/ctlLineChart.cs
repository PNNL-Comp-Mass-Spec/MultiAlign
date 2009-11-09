using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlLineChart.
	/// </summary>
	public class ctlLineChart : PNNLControls.ctlChartBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private int m_autoDrawPointLimit = 256;

		private bool m_autoDrawPoints = false;
		private bool m_drawPoints = false;
		private bool mbln_draw_sticks = false ; 
		private bool mbln_label_peaks = true ; 
		private bool mblnDrawBox = true ; 
		protected clsPeakLabeler mobj_peak_labeler ; 
		private PNNLControls.ChartPostRenderingProcessor mobjPeakLabeler ;

		public ctlLineChart()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			this.Title = "Line Chart";
			mobj_peak_labeler = new clsPeakLabeler() ; 
			mobjPeakLabeler = new ChartPostRenderingProcessor(this.LabelPeaks) ; 
			base.AddPostProcessor(mobjPeakLabeler, PNNLControls.PostProcessPriority.MidLow) ; 
		}

		private void LabelPeaks(ctlChartBase sender, PostRenderEventArgs args) 
		{
			if (mbln_label_peaks && mobj_peak_labeler != null)
			{
				mobj_peak_labeler.Draw(args.Graphics) ; 
			}
		}

		void DrawLine(Graphics g, Bitmap bitmap, clsSeries series)
		{
			mobj_peak_labeler.DrawingArea =  new Rectangle(new Point(0,0), this.mobj_axis_plotter.ChartAreaBounds.Size) ; 
			ChartDataPoint[] dataPoints = series.PlotData;
			int num_elements = dataPoints.Length ;
			Pen pen = series.PlotParams.LinePen.Pen;
			//g.DrawLine(pen, 0, 0F, int.MaxValue / 256, int.MaxValue / 256);
			//g.DrawLine(pen, 17.93949F, 6.085601E-7F, -0.1272318F, 181.2265F);
			float yMinimum = 0;
			float yMaximum = 0;
			int prevXPixel = 0;
			float prevX = 0;
			float prevY = 0;
			float prevXVal = 0 ; 
			float prevprevXVal = 0 ; 
			int prevYPixel = 0;
			float prevYVal = 0 ; 

			int prevprevYPixel = 0 ; 
			float prevprevYVal = 0 ; 

			int linesDrawn = 0;
			int maxChartX = this.MaxChartAreaXPixel;
			int maxChartY = this.MaxChartAreaYPixel;
			ArrayList points = new ArrayList(num_elements);
			bool drawPoints = this.DrawPoints || this.AutoDrawPoints;
			// The rectangle into which line are clipped before handing them to 
			// Graphics (which will clip them to the onscreen/on-bitmap portion.
			// This doesn't have to be a tight box, just enough to cut the values
			// down to a reasonable size.
			RectangleF rect = //new RectangleF(0,0, this.MaxChartAreaXPixel, this.MaxChartAreaYPixel);
				new RectangleF(-this.MaxChartAreaXPixel * 10, -this.MaxChartAreaYPixel * 10, 
				3 * 10 * this.MaxChartAreaXPixel, 3 * this.MaxChartAreaYPixel * 10);
			GeomRectangleF drawRect = new GeomRectangleF(rect);

			// Get the initial point information
			if (num_elements > 0) 
			{
				prevY = GetScreenPixelY(dataPoints[0].y);
				prevX = GetScreenPixelX(dataPoints[0].x);
				prevXPixel = (int) prevX;
				prevYPixel = (int) prevY;
				prevprevYPixel = prevYPixel ; 
				prevYVal = dataPoints[0].y ; 
				prevprevYVal = prevYVal ; 
				yMinimum = prevY;
				yMaximum = prevY;
				prevXPixel = (int) prevX;
				if ( drawPoints && prevXPixel >= 0 && prevXPixel <= maxChartX
					&& this.mobj_axis_plotter.YOnScreen(dataPoints[0].y)) 
				{
					points.Add(new ChartDataPlotPoint(prevXPixel, prevYPixel, dataPoints[0].color));
				}
			}

			for (int i = 1 ; i < num_elements ; i++)
			{
				float xData = dataPoints[i].x;
				float yData = dataPoints[i].y;
				float currentY = this.mobj_axis_plotter.YScreenPixel(yData);
				int currentYPixel = Convert.ToInt32(currentY) ;
				float currentX = this.mobj_axis_plotter.XScreenPixel(xData);
				int currentXPixel = Convert.ToInt32(currentX) ;
				bool currentXOnScreen = currentXPixel >= 0 && currentXPixel <= maxChartX;
				if (currentXPixel != prevXPixel) 
				{
					// we're on a new screen pixel
					if (prevXPixel >= 0 && prevXPixel <= maxChartX) 
					{
						// first draw max/min line for last pixel
						if (!mbln_draw_sticks)
						{
							DrawLine(g, pen, drawRect, prevXPixel, yMinimum, prevXPixel, yMaximum);
							linesDrawn++;
						}
						else
						{
							DrawLine(g, pen, drawRect, prevXPixel, yMinimum, prevXPixel, maxChartY);
							linesDrawn++;
						}
					}
					
					if ((prevXPixel >= 0 && prevXPixel <= maxChartX) ||
						(currentXPixel >= 0 && currentXPixel <= maxChartX) ||
						(prevXPixel >= maxChartX && currentXPixel <= 0) || 
						(prevXPixel <= 0 && currentXPixel >= maxChartX))
					{
						// now draw line between last pixel and this one
						if (!mbln_draw_sticks)
						{
							DrawLine(g, pen, drawRect, prevX, prevY, currentX, currentY);
							linesDrawn++;
							if (prevYVal > yData && prevYVal > prevprevYVal && prevXPixel >= 0 )
							{
								mobj_peak_labeler.AddPeak(i, new Point(prevXPixel, prevYPixel), prevXVal, mblnDrawBox) ; 
							}
						}
						else
						{
							mobj_peak_labeler.AddPeak(i, new Point(prevXPixel, prevYPixel), prevXVal, mblnDrawBox) ; 
						}
					}
					// update pixel measurements
					prevXPixel = currentXPixel;
					prevX = currentX;
					prevY = currentY;
					yMinimum = currentY;
					yMaximum = currentY;
					prevprevYPixel = prevYPixel ; 
					prevYPixel = currentYPixel ; 
					prevXVal = xData ; 
					prevprevXVal = prevXVal  ; 

					prevprevYVal = prevYVal ; 
					prevYVal = yData ; 

				}
				else 
				{
					// update maximum and minimum for current onscreen x pixel value
					yMinimum = yMinimum < currentY ? yMinimum : currentY;
					yMaximum = yMaximum > currentY ? yMaximum : currentY;

					if (prevYVal > yData && prevYVal > prevprevYVal && prevXPixel >= 0 && prevXPixel <= maxChartX)
					{
						mobj_peak_labeler.AddPeak(i, new Point(prevXPixel, prevYPixel), prevXVal, mblnDrawBox) ; 
					}

					prevX = currentX;
					prevY = currentY;
					prevprevYPixel = prevYPixel ; 
					prevYPixel = currentYPixel ; 
					prevXVal = xData ; 
					prevprevXVal = prevXVal  ; 

					prevprevYVal = prevYVal ; 
					prevYVal = yData ; 
				}

				// Add the point to the list to be drawn scatter style, if necessary
				if (drawPoints && currentXOnScreen && currentYPixel >= 0 && currentYPixel <= maxChartY) 
				{
					points.Add(new ChartDataPlotPoint(currentXPixel, (int) currentY, dataPoints[i].color));
				}
			}
			if (prevXPixel >= 0 && prevXPixel <= maxChartX) 
			{
				// done, just need to make sure that final max/min line is drawn in
				DrawLine(g, pen, drawRect, prevXPixel, yMinimum, prevXPixel, yMaximum);
				linesDrawn++;
			}
			// Draw scatter-style points
			if ((this.m_drawPoints) || (this.m_autoDrawPoints && points.Count <= m_autoDrawPointLimit)) 
			{
				this.mobj_bitmap_tools.DrawDots(points, series.PlotParams.Shape, bitmap);
			}
			

			pen.Dispose();
			//Console.WriteLine("Dots: " + points.Count);
			//Console.WriteLine("Lines Drawn: {0}", linesDrawn);
		}


		private void DrawLine(Graphics g, Pen p, GeomRectangleF rect, float x1, float y1, float x2, float y2) 
		{
			try 
			{
				SegmentF line = new SegmentF(x1, y1, x2, y2);
				if (line.Intersects(rect)) 
				{
					// cut down to small enough that Graphics will definitely draw it
					SegmentF toDraw = line.Intersection(rect);
					//Console.WriteLine("Input {0}, {1} to {2}, {3}", x1, y1, x2, y2);
					//Console.WriteLine("Output {0}", toDraw);
					// then draw it
					g.DrawLine(p, toDraw.StartX, toDraw.StartY, toDraw.EndX, toDraw.EndY);
				}
			}
			catch (Exception e) 
			{
				Console.WriteLine(e);
			}
		}

		protected override void PaintSeries(Graphics g, Bitmap bitmap, clsSeries data)
		{
			// Set smoothing mode to antialias - looks good for smoothing line series, 
			// but not for grid lines, so set it back afterwards
			//System.Drawing.Drawing2D.SmoothingMode smoothing = g.SmoothingMode;
			//g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			DrawLine(g, bitmap, data) ; 
			//g.SmoothingMode = smoothing;
		}

		internal override Size GetPreferredLegendSymbolSize(clsSeries series)
		{
			Pen pen = series.PlotParams.LinePen.Pen;
			int penWidth = (int) Math.Ceiling(pen.Width);
			pen.Dispose();
			int height = penWidth * 2;
			int width = Math.Max(10, penWidth * 7);
			if (this.AutoDrawPoints || this.DrawPoints) 
			{
				height += series.PlotParams.Shape.Size * 2;
				width += series.PlotParams.Shape.Size * 2;
			}
			return new Size(width, height);
		}

		internal override void PaintLegendSymbol(Graphics g, clsSeries series, Rectangle bounds)
		{
			System.Drawing.Drawing2D.GraphicsContainer container = g.BeginContainer();
			// center the image on the bounds
			int centerX = (bounds.Left + bounds.Right) / 2;
			int centerY = (bounds.Top + bounds.Bottom) / 2;
			int y = centerY - series.PlotParams.Shape.Size;
			int x = centerX - series.PlotParams.Shape.Size;
			g.IntersectClip(bounds);
			g.DrawLine(series.PlotParams.LinePen.Pen, bounds.Left, centerY, bounds.Right, centerY);
			if (this.AutoDrawPoints || this.DrawPoints) 
			{
				g.TranslateTransform(centerX, centerY);
				series.PlotParams.DrawShape(g);
			}
			g.EndContainer(container);
		}

		#region "Properties"

		[System.ComponentModel.Description("Controls whether we draw in a Stick mode")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Category("Line Chart")]
		public bool DrawSticks
		{
			get
			{
				return mbln_draw_sticks ; 
			}
			set
			{
				mbln_draw_sticks = value ; 
				this.FullInvalidate() ; 
			}
		}

		[System.ComponentModel.Description("Controls the hieght above the peak top at which a label is drawn")]
		[System.ComponentModel.DefaultValue(8)]
		[System.ComponentModel.Category("Line Chart")]
		public float LabelOffset
		{
			get
			{
				return mobj_peak_labeler.LabelOffset ; 
			}
			set
			{
				mobj_peak_labeler.LabelOffset = value ; 
				this.FullInvalidate() ; 
			}
		}

		[System.ComponentModel.Description("Controls whether or not we label peaks automatically")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Category("Line Chart")]
		public bool DrawPeakLabels
		{
			get
			{
				return mbln_label_peaks ; 
			}
			set
			{
				mbln_label_peaks = value ; 
				this.FullInvalidate() ; 
			}
		}

		[System.ComponentModel.Description("Controls whether labels are drawn vertically")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Category("Line Chart")]
		public bool VerticalLabels
		{
			get
			{
				return mobj_peak_labeler.VerticalLabels ; 
			}
			set
			{
				mobj_peak_labeler.VerticalLabels = value ; 
				this.FullInvalidate();
			}
		}

		[System.ComponentModel.Description("# of bins for peak labeling")]
		[System.ComponentModel.DefaultValue(10)]
		[System.ComponentModel.Category("Line Chart")]
		public int NumXBins
		{
			get
			{
				return mobj_peak_labeler.NumBins ; 
			}
			set
			{
				mobj_peak_labeler.NumBins = value ; 
				this.FullInvalidate();
			}
		}
		
		[System.ComponentModel.Description("Precision of the peak values")]
		[System.ComponentModel.DefaultValue(2)]
		[System.ComponentModel.Category("Line Chart")]
		public int Precision
		{
			get
			{
				return mobj_peak_labeler.Precision ; 
			}
			set
			{
				mobj_peak_labeler.Precision = value ; 
				this.FullInvalidate() ; 
			}
		}


		[System.ComponentModel.Description("# of peaks per bin for peak labeling")]
		[System.ComponentModel.DefaultValue(5)]
		[System.ComponentModel.Category("Line Chart")]
		public int NumPeaksPerBin
		{
			get
			{
				return mobj_peak_labeler.NumPeaksPerBins ; 
			}
			set
			{
				mobj_peak_labeler.NumPeaksPerBins = value ; 
				this.FullInvalidate();
			}
		}

		[System.ComponentModel.Description("Controls whether scatter-style points are drawn in addition to lines")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Category("Line Chart")]
		public bool DrawPoints
		{
			get
			{
				return this.m_drawPoints;
			} 
			set 
			{
				if (this.DrawPoints != value) 
				{
					this.FullInvalidate();
				}
				this.m_drawPoints = value;
			}
		}

		[System.ComponentModel.Description("Controls the number of points per series needed for" 
			 + " auto point drawing to take effect.")]
		[System.ComponentModel.DefaultValue(256)]
		[System.ComponentModel.Category("Line Chart")]
		public int AutoDrawPointsLimit 
		{
			get 
			{
				return this.m_autoDrawPointLimit;
			} 
			set 
			{
				this.m_autoDrawPointLimit = value;
				this.FullInvalidate();
			}
		}


		[System.ComponentModel.Description("Controls whether scatter-style points are drawn if the "
			 + "number of visible points is less than AutoDrawPointsLimit.")]
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Category("Line Chart")]
		public bool AutoDrawPoints
		{
			get 
			{
				return this.m_autoDrawPoints;
			}
			set 
			{
				if (this.AutoDrawPoints != value) 
				{
					this.FullInvalidate();
				}
				this.m_autoDrawPoints = value;
			}
		}

		[System.ComponentModel.Description("Controls whether boxes are drawn around peak labels")]
		[System.ComponentModel.DefaultValue(true)]
		[System.ComponentModel.Category("Line Chart")]
		public bool DrawPeakBoxes
		{
			get 
			{
				return mblnDrawBox;
			}
			set 
			{
				this.mblnDrawBox = value;
				this.FullInvalidate() ; 
			}
		}

		#endregion
		
		public void ResetAutoDrawPoints() 
		{
			this.AutoDrawPoints = false;
		}

		public void ResetDrawPoints() 
		{
			this.DrawPoints = false;
		}

		public void ResetAutoDrawPointsLimit() 
		{
			this.AutoDrawPointsLimit = 256;
		}

		#region "Utils"

		#endregion

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
}

