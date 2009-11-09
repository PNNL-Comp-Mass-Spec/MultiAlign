using System;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace PNNLControls
{
	/// <summary>
	/// Tells what part of the charting surface is located at a given point.
	/// </summary>
	[Flags]
	public enum ChartLocation 
	{
		None = 0, 
		ChartArea = 1, 
		Legend = 2, 
		XAxisAndUnits = 4, 
		YAxisAndUnits = 8, 
		XLabel = 16, 
		YLabel = 32, 
		Title = 64	
	}

	/// <summary>
	/// Tells whether gridlines should be drawn above or below the series.
	/// </summary>
	public enum ChartLayer
	{
		UnderSeries = 0,
		AboveSeries = 1,
	}

	#region "Post Render Event"

	/// <summary>
	/// Allows classes to do extra drawing on the charting surface after all series have 
	/// been drawn.
	/// </summary>
	public delegate void ChartPostRenderingProcessor(ctlChartBase chart, PostRenderEventArgs args);

	/// <summary>
	/// Provides access to the Graphics used to draw on the charting surface
	/// </summary>
	public class PostRenderEventArgs : EventArgs 
	{
		private Graphics graphics;
		public PostRenderEventArgs(Graphics g) 
		{
			graphics = g;
		}

		public Graphics Graphics
		{
			get 
			{
				return graphics;
			}
		}
	}
	
	public enum PostProcessPriority 
	{
		Lowest = 0,
		Low = 10,
		MidLow = 30,
		Mid = 50,
		MidHigh = 70,
		High = 90,
		Highest = 100
	}
	#endregion

	#region "ViewPort Changed Event"
		
	public delegate void ViewPortChangedHandler(ctlChartBase chart, ViewPortChangedEventArgs args);

	public class ViewPortChangedEventArgs : EventArgs 
	{
		private RectangleF viewport;

		public ViewPortChangedEventArgs(RectangleF port) 
		{
			viewport = port;
		}

		public RectangleF ViewPort
		{
			get 
			{
				return viewport;
			}
		}
	}
	#endregion
}
