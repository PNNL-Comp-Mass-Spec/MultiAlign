using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;

namespace PNNLControls
{

	public delegate void SelectionMadeHandler(object sender, SelectionEventArgs args);

	public class SelectionEventArgs : EventArgs 
	{
		bool mHandled = false;
		RectangleF mRect;

		public SelectionEventArgs(RectangleF rect) 
		{
			this.mRect = rect;
		}

		public RectangleF Selection 
		{
			get 
			{
				return mRect;
			}
		}

		public bool Handled 
		{
			get 
			{
				return mHandled;
			}
			set 
			{
				mHandled = value;
			}
		}
	}

	/// <summary>
	/// Handles drawing of zoom on chart by listening to MouseDown/MouseUp events.  Now 
	/// raises SelectionEvent
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class ChartZoomHandler
	{
		/// <summary>
		/// Current state of zooming
		/// </summary>
		private ChartZoomHandlerState state;
        
		/// <summary>
		/// Handlers for mouse events and chart rendering
		/// </summary>
		private PNNLControls.ChartPostRenderingProcessor selectionRenderer;
		private MouseEventHandler mouseDownHandler;
		private MouseEventHandler mouseUpHandler;
		private MouseEventHandler mouseMoveHandler;
		private KeyPressEventHandler cancelHandler;
		private ctlChartBase activeChart = null;
		private System.Windows.Forms.Timer paintTimer;

		public event SelectionMadeHandler SelectionMade;

		/// <summary>
		/// Start and end points of current zoom, relative to upper-left of chart control.
		/// </summary>
		private Point start;
		private Point end;

		/// <summary>
		/// Chart that zoom handler is attached to
		/// </summary>
		//private ctlChartBase chart = null;
		private ArrayList charts = new ArrayList();

		/// <summary>
		/// Coloring properties
		/// </summary>
		private Color lineColor = Color.Black;
		private Color fillColor = Color.FromArgb(60, Color.LightSlateGray);
		private Brush internalBrush;
		private Pen basePen;
		private System.Windows.Forms.Keys modifierKeys;

		/// <summary>
		/// Whether this handler is enabled
		/// </summary>
		private bool active = true;
        private List<DrawingZoomRegion> m_zoomDelegates;
		private DateTime lastActionTime = DateTime.Now;

		public ChartZoomHandler()
		{
			this.selectionRenderer = new ChartPostRenderingProcessor(this.DrawSelectionRectangle);
			this.mouseDownHandler = new MouseEventHandler(this.ChartMouseDown);
			this.mouseUpHandler = new MouseEventHandler(this.ChartMouseUp);
			this.mouseMoveHandler = new MouseEventHandler(this.ChartMouseMove);
			this.cancelHandler = new KeyPressEventHandler(this.ChartKeyPress);
			this.state = new ChartZoomHandlerStateNormal();
			this.paintTimer = new Timer();
			this.paintTimer.Enabled = false;
			this.paintTimer.Interval = 100;
			this.paintTimer.Tick += new EventHandler(this.PaintTimerTick);
			this.LineColor = LineColor;
			this.FillColor = FillColor;
			this.ModifierKeys = Keys.None;
            m_zoomDelegates = new List<DrawingZoomRegion>();
		}

		public ChartZoomHandler(ctlChartBase chart)  : this()
		{
            this.Attach(chart);
            m_zoomDelegates = new List<DrawingZoomRegion>();
		}

		#region "Attachment/Detachment"

		/// <summary>
		/// Attach this zoom handler to the given chart.  Must not be attached to a chart
		/// already.
		/// </summary>
		/// <param name="newChart"></param>
		public void Attach(ctlChartBase newChart) 
		{
			charts.Add(newChart);
			newChart.AddPostProcessor(this.selectionRenderer, PostProcessPriority.High);
			newChart.MouseDown += this.mouseDownHandler;
			newChart.MouseUp += this.mouseUpHandler;
			newChart.MouseMove += this.mouseMoveHandler;
			newChart.KeyPress += this.cancelHandler;
			//this.state = new ChartZoomHandlerStateNormal(chart);
		}

		/// <summary>
		/// Detach this zoom handler from the given chart
		/// </summary>
		public void Detach(ctlChartBase oldChart) 
		{
			if (oldChart != null && charts.Contains(oldChart)) 
			{
				oldChart.RemovePostProcessor(this.selectionRenderer);
				oldChart.MouseDown -= this.mouseDownHandler;
				oldChart.MouseUp -= this.mouseUpHandler;
				oldChart.MouseMove -= this.mouseMoveHandler;
				oldChart.KeyPress -= this.cancelHandler;
				charts.Remove(oldChart);
			}
		}
		#endregion

		#region "Helpers"
		internal Rectangle ShrinkRectToDrawInChartArea(ctlChartBase chart, float x1, float x2, float y1, float y2) 
		{
			float minX = Math.Min(x1, x2);
			float maxX = Math.Max(x1, x2);
			float minY = Math.Min(y1, y2);
			float maxY = Math.Max(y1, y2);

			int startX = Math.Max((int) minX, 0);
			int startY = Math.Max((int) minY, 0);
			int endX = Math.Min(chart.MaxChartAreaXPixel - 1, (int)maxX);
			int endY = Math.Min(chart.MaxChartAreaYPixel - 1, (int)maxY);
			return new Rectangle(startX, startY, endX - startX, endY - startY);
		}
		#endregion

		#region "Properties"
        
		[System.ComponentModel.DefaultValue(Keys.None)]
		[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
		public System.Windows.Forms.Keys ModifierKeys 
		{
			get 
			{
				return this.modifierKeys;
			}
			set 
			{
				this.modifierKeys = value;
			}
		}

		public bool Active 
		{
			get 
			{
				return active;
			}
			set 
			{
				// switch to active/inactive state
				if (value) 
				{
					this.state = new ChartZoomHandlerStateNormal();
				}
				else 
				{
					this.state = new ChartZoomHandlerStateDisabled();
				}
				this.active = value;
			}
		}
		
		public Color LineColor
		{
			get 
			{
				return this.lineColor;
			}
			set 
			{
				this.lineColor = value;
				this.basePen = new Pen(LineColor, 1);
			}
		}

		//		public int Alpha 
		//		{
		//			get 
		//			{
		//				return this.internalColorAlpha;
		//			}
		//			set 
		//			{
		//				if (value < 0 || value > 255) 
		//				{
		//					throw new ArgumentException("Must be >=0 and <= 255", "Alpha");
		//				}
		//				this.internalColorAlpha = value;
		//				this.SetColors();
		//			}
		//		}

		public Color FillColor 
		{
			get 
			{
				return fillColor;
			}
			set 
			{
				fillColor = value;
				this.internalBrush = new SolidBrush(FillColor);
			}
		}

		public void ResetFillColor() 
		{
			this.fillColor = Color.FromArgb(60, Color.LightSlateGray);
		}
		#endregion

        public void AddDrawingDelegate(DrawingZoomRegion drawingDelegate)
        {
            m_zoomDelegates.Add(drawingDelegate);
        }

		#region "Event Handlers"

		private void PaintTimerTick(Object sender, EventArgs args) 
		{
			foreach (ctlChartBase chart in charts) 
			{
				chart.Invalidate();
			}
		}

		/// <summary>
		/// Draws the currently selected rectangle on the chart surface
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DrawSelectionRectangle(ctlChartBase sender, PostRenderEventArgs args) 
		{
			//Console.WriteLine("Drawing selection " + sender.Name);
			if (state.DrawSelectionRectangle()) 
			{
				RectangleF chartBounds = state.GetZoomSelection(activeChart, this.start, this.end);
				// translate to onscreen coordinates
				float x1 = sender.GetScreenPixelX(chartBounds.Left);
				float x2 = sender.GetScreenPixelX(chartBounds.Right);
				float y1 = sender.GetScreenPixelY(chartBounds.Top);
				float y2 = sender.GetScreenPixelY(chartBounds.Bottom);
				Rectangle toDraw = ShrinkRectToDrawInChartArea(sender, x1, x2, y1, y2);

                foreach (DrawingZoomRegion deleg in m_zoomDelegates)
                {
                    try
                    {
                        deleg(sender, toDraw, chartBounds, args.Graphics);
                    }
                    catch
                    {
                    }
                }

				args.Graphics.FillRectangle(this.internalBrush, toDraw);
				args.Graphics.DrawRectangle(this.basePen, toDraw);
			}
		}

		private void ChartMouseDown(Object sender, MouseEventArgs args) 
		{
			if (args.Button == MouseButtons.Left)
			{
                if (System.Windows.Forms.ContainerControl.ModifierKeys == this.ModifierKeys)
                {
                    // set initial point and move to new state
                    start = new Point(args.X, args.Y);
                    state = state.GetBeginZoomState((ctlChartBase)sender, start);
                    activeChart = (ctlChartBase)sender;
                    if (state.DrawSelectionRectangle())
                    {
                        this.paintTimer.Enabled = true;
                    }
                }
                else if (ContainerControl.ModifierKeys == Keys.Shift)
                {
                    // set initial point and move to new state
                    start = new Point(args.X, args.Y);
                    state = state.GetBeginZoomState((ctlChartBase)sender, start);
                    activeChart = (ctlChartBase)sender;
                    if (state.DrawSelectionRectangle())
                    {
                        this.paintTimer.Enabled = true;
                    }
                }
			}
		}
		
		/// <summary>
		/// Cancels zoom if escaped key is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ChartKeyPress(Object sender, System.Windows.Forms.KeyPressEventArgs args) 
		{
			if (sender.Equals(activeChart) && args.KeyChar == 27 && !args.Handled) 
			{
				args.Handled = true;
				state = state.GetCancellationState();
				foreach (ctlChartBase chart in charts) 
				{
					chart.Invalidate();
				}
				this.paintTimer.Enabled = false;
			}
		}

		private void ChartMouseMove(Object sender, MouseEventArgs args) 
		{
			//Console.WriteLine("Mouse move time: " + DateTime.Now.Subtract(lastActionTime));
			lastActionTime = DateTime.Now;
			// if drawing selection rectangle, force chart to repaint when 
			// mouse moves
			if (sender.Equals(activeChart) && state.DrawSelectionRectangle()) 
			{
				((ctlChartBase) sender).Invalidate();
			}
			end = new Point(args.X, args.Y);
		}

		private void ChartMouseUp(Object sender, MouseEventArgs args) 
		{
			if (sender.Equals(activeChart) && args.Button == MouseButtons.Left) 
			{
				end = new Point(args.X, args.Y);

				// If state recognizes this as a valid zoom, actually do the zoom
                if (state.ShouldZoom((ctlChartBase)activeChart, start, end) && ContainerControl.ModifierKeys != Keys.Shift)
                {
                    // First raise a selection event
                    RectangleF viewPort = state.GetZoomSelection(activeChart, start, end);
                    SelectionEventArgs selectArgs = new SelectionEventArgs(viewPort);
                    OnSelectionMade(selectArgs);
                    // If not handled, do the zoom.
                    if (!selectArgs.Handled)
                    {
                        foreach (ctlChartBase chart in charts)
                        {
                            try
                            {
                                chart.ViewPort = viewPort;
                                chart.Invalidate();
                            }
                            catch (Exception e)
                            {
                                // ignore - can happen when the user failed to select a rectangle 
                                // with any content, i.e. 0 width X or Y selection.
                            }
                        }
                    }
                }
                else                
                {
                    foreach (ctlChartBase chart in charts)
                    {
                        try
                        {
                            chart.Invalidate();
                        }
                        catch (Exception e)
                        {
                        }
                    }                
                }

				// Get the new zoom state.
				state = state.GetEndZoomState((ctlChartBase) sender, start, end);
				this.paintTimer.Enabled = false;
			}
		}

		protected virtual void OnSelectionMade(SelectionEventArgs args) 
		{
			if (this.SelectionMade != null) 
			{
				this.SelectionMade(this, args);
			}
		}
	}
	#endregion
	
	#region "States"
	/// <summary>
	/// Abstract zoom state interface
	/// </summary>
	internal abstract class ChartZoomHandlerState 
	{
		internal Rectangle ShrinkRectToFitInChartArea(ctlChartBase source, int x1, int x2, int y1, int y2) 
		{
			int minX = Math.Min(x1, x2);
			int maxX = Math.Max(x1, x2);
			int minY = Math.Min(y1, y2);
			int maxY = Math.Max(y1, y2);

			int startX = Math.Max(minX, 0);
			int startY = Math.Max(minY, 0);
			int endX = Math.Min(source.MaxChartAreaXPixel, maxX);
			int endY = Math.Min(source.MaxChartAreaYPixel, maxY);
			return new Rectangle(startX, startY, endX - startX, endY - startY);
		}
			
		internal ChartZoomHandlerState() 
		{
		}

		/// <summary>
		/// Tells whether to draw a selection rectangle onscreen, once the 
		/// user begins a selection and until the selection is ended.
		/// </summary>
		/// <returns></returns>
		internal abstract bool DrawSelectionRectangle();
		/// <summary>
		/// Get the state to go to when the user begins a zoom selection.
		/// </summary>
		/// <param name="begin">Point of user mouse click, relative to the upper-left 
		/// of the chart control.</param>
		/// <returns></returns>
		internal abstract ChartZoomHandlerState GetBeginZoomState(ctlChartBase source, Point begin);
		/// <summary>
		/// Get the state to go to when the user ends a zoom.
		/// </summary>
		/// <param name="begin">Point of user mouse press, relative to the upper-left 
		/// of the chart control.</param>
		/// <param name="end">Point of user mouse release, relative to the upper-left 
		/// of the chart control.</param>
		/// <returns></returns>
		internal abstract ChartZoomHandlerState GetEndZoomState(ctlChartBase source, Point begin, Point end);
		/// <summary>
		/// Tells whether a zoom should be executed when the user releases the 
		/// mouse.
		/// </summary>
		/// <returns></returns>
		internal abstract bool ShouldZoom(ctlChartBase source, Point start, Point release);
		/// <summary>
		/// Gets the chart coordinates to zoom to
		/// </summary>
		/// <param name="start">The onscreen start point of the user selection, relative
		/// to the upper-left of the control.</param>
		/// <param name="release">The onscreen end point of the user selection, relative
		/// to the upper-left of the control.</param>
		/// <param name="chart"></param>
		/// <returns>The chart coordinates of the rectangle to set the viewport to.</returns>
		internal abstract RectangleF GetZoomSelection(ctlChartBase source, Point start, Point release);

		internal abstract ChartZoomHandlerState GetCancellationState();
	}

	/// <summary>
	/// The normal zoom state, before the user has started a click
	/// </summary>
	internal class ChartZoomHandlerStateNormal : ChartZoomHandlerState 
	{

		internal ChartZoomHandlerStateNormal()
		{
		}

		internal override bool DrawSelectionRectangle()
		{
			// never draw selection rectangle in this state
			return false;
		}

		internal override ChartZoomHandlerState GetBeginZoomState(ctlChartBase source, Point begin)
		{
			// get one of the zoom states
			if (source.GetLocation(begin) == ChartLocation.ChartArea) 
			{
				return new ChartZoomHandlerStateXYZoom();
			} 
			else if (source.GetLocation(begin) == ChartLocation.XAxisAndUnits) 
			{
				return new ChartZoomHandlerStateXZoom();
			} 
			else if (source.GetLocation(begin) == ChartLocation.YAxisAndUnits) 
			{
				return new ChartZoomHandlerStateYZoom();
			}
			return this;
		}

		internal override ChartZoomHandlerState GetEndZoomState(ctlChartBase source, Point start, Point end)
		{
			return this;
		}

		internal override RectangleF GetZoomSelection(ctlChartBase source, Point start, Point release)
		{
			// result doesn't matter, this method shouldn't be called
			return new RectangleF(0, 0, 0, 0);
		}
		
		internal override bool ShouldZoom(ctlChartBase source, Point start, Point release)
		{
			// if user ends selection (and we're in this state - for example, just got 
			// attached to a chart while the user had the mouse down) then don't zoom
			return false;
		}

		internal override ChartZoomHandlerState GetCancellationState()
		{
			return this;
		}

	}

	internal class ChartZoomHandlerStateXYZoom : ChartZoomHandlerState
	{
		public ChartZoomHandlerStateXYZoom() : base() 
		{
		}

		internal override bool ShouldZoom(ctlChartBase source, Point start, Point release)
		{
			return true;
		}

		internal override bool DrawSelectionRectangle()
		{
			return true;
		}

		internal override ChartZoomHandlerState GetBeginZoomState(ctlChartBase source, Point begin)
		{
			return this;
		}

		internal override ChartZoomHandlerState GetEndZoomState(ctlChartBase source, Point begin, Point end)
		{
			return new ChartZoomHandlerStateNormal();
		}

		internal override RectangleF GetZoomSelection(ctlChartBase source, Point start, Point release)
		{
			int x1 = source.GetChartAreaX(start.X);
			int y1 = source.GetChartAreaY(start.Y);
			int x2 = source.GetChartAreaX(release.X);
			int y2 = source.GetChartAreaY(release.Y);

			Rectangle screenZoomRect = ShrinkRectToFitInChartArea(source, x1, x2, y1, y2);

			float startX = source.GetChartX(screenZoomRect.Left);
			float endX = source.GetChartX(screenZoomRect.Right);
			float startY = source.GetChartY(screenZoomRect.Bottom);
			float endY = source.GetChartY(screenZoomRect.Top);

			RectangleF zoom = new RectangleF(startX, startY, endX - startX, endY - startY);
			return zoom;
		}

		internal override ChartZoomHandlerState GetCancellationState()
		{
			return new ChartZoomHandlerStateNormal();
		}

	}
	
	internal class ChartZoomHandlerStateXZoom : ChartZoomHandlerState
	{
		public ChartZoomHandlerStateXZoom() : base() 
		{
		}

		internal override bool DrawSelectionRectangle()
		{
			return true;
		}

		internal override ChartZoomHandlerState GetBeginZoomState(ctlChartBase source, Point begin)
		{
			return this;
		}
		internal override ChartZoomHandlerState GetEndZoomState(ctlChartBase source, Point begin, Point end)
		{
			return new ChartZoomHandlerStateNormal();
		}
		internal override ChartZoomHandlerState GetCancellationState()
		{
			return new ChartZoomHandlerStateNormal();
		}
		internal override RectangleF GetZoomSelection(ctlChartBase source, Point start, Point release)
		{
			int x1 = source.GetChartAreaX(start.X);
			int y1 = 0;
			int x2 = source.GetChartAreaX(release.X);
			int y2 = source.MaxChartAreaYPixel;


			Rectangle screenZoomRect = ShrinkRectToFitInChartArea(source, x1, x2, y1, y2);

			float startX = source.GetChartX(screenZoomRect.Left);
			float endX = source.GetChartX(screenZoomRect.Right);

			RectangleF zoom = new RectangleF(startX, source.ViewPort.Y, 
				endX - startX, source.ViewPort.Height);
			return zoom;
		}

		internal override bool ShouldZoom(ctlChartBase source, Point start, Point release)
		{
			return true;
		}
	}

	internal class ChartZoomHandlerStateYZoom : ChartZoomHandlerState
	{
		public ChartZoomHandlerStateYZoom() : base() 
		{
		}
		internal override bool DrawSelectionRectangle()
		{
			return true;
		}
		internal override ChartZoomHandlerState GetBeginZoomState(ctlChartBase source, Point begin)
		{
			return this;
		}
		internal override ChartZoomHandlerState GetCancellationState()
		{
			return new ChartZoomHandlerStateNormal();
		}
		internal override ChartZoomHandlerState GetEndZoomState(ctlChartBase source, Point begin, Point end)
		{
			return new ChartZoomHandlerStateNormal();
		}
		internal override RectangleF GetZoomSelection(ctlChartBase source, Point start, Point release)
		{
			int x1 = 0;
			int y1 = source.GetChartAreaY(start.Y);
			int x2 = source.MaxChartAreaXPixel;
			int y2 = source.GetChartAreaY(release.Y);


			Rectangle screenZoomRect = ShrinkRectToFitInChartArea(source, x1, x2, y1, y2);

			float startY = source.GetChartY(screenZoomRect.Bottom);
			float endY = source.GetChartY(screenZoomRect.Top);

			RectangleF zoom = new RectangleF(source.ViewPort.Left, startY, 
				source.ViewPort.Width, endY - startY);
			return zoom;
		}

		internal override bool ShouldZoom(ctlChartBase source, Point start, Point release)
		{
			return true;
		}
	}
	
	internal class ChartZoomHandlerStateDisabled : ChartZoomHandlerState
	{
		internal override bool DrawSelectionRectangle()
		{
			return false;
		}

		internal override ChartZoomHandlerState GetBeginZoomState(ctlChartBase source, Point begin)
		{
			return this;
		}

		internal override ChartZoomHandlerState GetCancellationState()
		{
			return this;
		}

		internal override ChartZoomHandlerState GetEndZoomState(ctlChartBase source, Point begin, Point end)
		{
			return this;
		}

		internal override RectangleF GetZoomSelection(ctlChartBase source, Point start, Point release)
		{
			return new RectangleF(0,0,0,0);
		}

		internal override bool ShouldZoom(ctlChartBase source, Point start, Point release)
		{
			return false;
		}
	}

	#endregion

    /// <summary>
    /// Drawing arguments when drawing a zoom window.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="region"></param>
    /// <param name="bounds"></param>
    public delegate void DrawingZoomRegion(ctlChartBase sender, RectangleF region, RectangleF bounds, Graphics graphics);
}
