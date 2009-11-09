using System;
using System.Drawing;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ChartLayout.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class ChartLayout
	{
		private float titleFraction = .10F;
		private int minTitleHeight = 15;
		private int maxTitleHeight = 50;

		private ChartLegendLocation legendLocation;

		private float legendFraction = .20F;
		private int maxLegendWidth = 250;
		private int minLegendWidth = 75;
		private int maxLegendHeight = 150;
		private int minLegendHeight = 50;

		private ctlChartBase chart;
		private ChartLayoutMode mode = new ChartLayoutModeLegendRight();
		
		public ChartLayout(ctlChartBase chart)
		{
			this.chart = chart;
			this.LegendLocation = ChartLegendLocation.Right;
		}

		#region "Properties"

		internal ctlChartBase Chart 
		{
			get 
			{
				return this.chart;
			}
		}

		public int MinTitleHeight 
		{
			get 
			{
				return this.minTitleHeight;
			}
			set 
			{
				if (value < 0) 
				{
					throw new ArgumentException("Min height must be >= 0", "MinTitleHeight");
				}
				this.minTitleHeight = value;
				chart.PerformLayout();
			}
		}

		public int MaxTitleHeight 
		{
			get 
			{
				return this.maxTitleHeight;
			}
			set 
			{
				if (value < 0) 
				{
					throw new ArgumentException("Max height must be >= 0", "MaxTitleHeight");
				}
				this.maxTitleHeight = value;
				chart.PerformLayout();
			}
		}

		public float TitleFraction 
		{
			get 
			{
				return this.titleFraction;
			}
			set 
			{
				if (value <= 0 || value >= 1) 
				{
					throw new ArgumentException("Value must be between 0 and 1", "TitleFraction");
				}
				this.titleFraction = value;
				chart.PerformLayout();
			}
		}

		public float LegendFraction 
		{
			get 
			{
				return this.legendFraction;
			}
			set 
			{
				if (value <= 0 || value >= 1) 
				{
					throw new ArgumentException("Value must be between 0 and 1", "LegendFraction");
				}
				this.legendFraction = value;
				chart.PerformLayout();
			}
		}

		public int MinLegendHeight 
		{
			get 
			{
				return this.minLegendHeight;
			}
			set 
			{
				if (value < 0) 
				{
					throw new ArgumentException("Min height must be >= 0", "MinLegendHeight");
				}
				this.minLegendHeight = value;
				chart.PerformLayout();
			}
		}

		public int MaxLegendHeight 
		{
			get 
			{
				return this.maxLegendHeight;
			}
			set 
			{
				if (value < 0) 
				{
					throw new ArgumentException("Max height must be >= 0", "MaxLegendHeight");
				}
				this.maxLegendHeight = value;
				chart.PerformLayout();
			}
		}

		public int MinLegendWidth 
		{
			get 
			{
				return this.minLegendWidth;
			}
			set 
			{
				if (value < 0) 
				{
					throw new ArgumentException("Min width must be >= 0", "MinLegendWidth");
				}
				this.minLegendWidth = value;
				chart.PerformLayout();
			}
		}

		public int MaxLegendWidth
		{
			get 
			{
				return this.maxLegendWidth;
			}
			set 
			{
				if (value < 0) 
				{
					throw new ArgumentException("Max width must be >= 0", "MaxLegendWidth");
				}
				this.maxLegendWidth = value;
				chart.PerformLayout();
			}
		}

		#endregion

		public Rectangle TitleBounds() 
		{
			return this.mode.TitleBounds(this);
		}

		public Rectangle LegendBounds() 
		{
			return this.mode.LegendBounds(this);
		}

		public Rectangle PlotBounds() 
		{
			return this.mode.PlotBounds(this);
		}

		public ChartLegendLocation LegendLocation 
		{
			get 
			{
				return this.legendLocation;
			}
			set 
			{
				this.legendLocation = value;
				switch (this.LegendLocation) 
				{
					case ChartLegendLocation.Bottom :
						this.mode = new ChartLayoutModeLegendBottom();
						break;
					case ChartLegendLocation.Left :
						this.mode = new ChartLayoutModeLegendLeft();
						break;
					case ChartLegendLocation.Right :
						this.mode = new ChartLayoutModeLegendRight();
						break;
					case ChartLegendLocation.Floating :
						this.mode = new ChartLayoutModeLegendFloating();
						break;
					case ChartLegendLocation.UnderTitle :
						this.mode = new ChartLayoutModeLegendUnderTitle();
						break;
				}
				this.chart.LegendLocationChanged();
				this.Chart.PerformLayout();
			}
		}
	}
	internal abstract class ChartLayoutMode 
	{
		public abstract Rectangle TitleBounds(ChartLayout layout);
		public abstract Rectangle LegendBounds(ChartLayout layout);
		public abstract Rectangle PlotBounds(ChartLayout layout);

		public virtual int LegendWidth(ChartLayout layout) 
		{
			if (layout.Chart.HasLegend) 
			{
				int width = (int) (layout.LegendFraction * layout.Chart.Width);
				width = Math.Min(layout.MaxLegendWidth, width);
				width = Math.Max(layout.MinLegendWidth, width);
				return width;
			}
			return 0;
		}

		public virtual int LegendHeight(ChartLayout layout)
		{
            if (layout.Chart.HasLegend)
            {
                int height = (int)(layout.LegendFraction * layout.Chart.Height);
                height = Math.Min(layout.MaxLegendHeight, height);
                height = Math.Max(layout.MinLegendHeight, height);
                return height;
            }
            return 0;
		}
	}

	internal abstract class ChartLayoutModeTitleTop : ChartLayoutMode {
		public override Rectangle TitleBounds(ChartLayout layout)
		{
            if (layout.Chart.TitleVisible == true)
            {
                int width = layout.Chart.Width;
                int left = layout.Chart.Margins.GetDefaultMargin(width);
                int top = layout.Chart.Margins.GetDefaultMargin(layout.Chart.Height);
                int height = (int)(layout.Chart.Height * layout.TitleFraction);

                width -= 2 * layout.Chart.Margins.GetDefaultMargin(width);
                height = layout.Chart.Height;

                height = Math.Min(layout.MaxTitleHeight, height);
                height = Math.Max(layout.MinTitleHeight, height);
                return new Rectangle(left, top, width, height);
            }
            return new Rectangle(0, 0, 0, 0);
		}
	}

	internal class ChartLayoutModeLegendRight : ChartLayoutModeTitleTop
	{
		public override Rectangle LegendBounds(ChartLayout layout)
		{
			if (layout.Chart.HasLegend) 
			{
				int left = layout.Chart.Width - this.LegendWidth(layout);
				int potentialHeight = layout.Chart.Height - this.TitleBounds(layout).Bottom;
				int top = this.TitleBounds(layout).Bottom + layout.Chart.Margins.GetDefaultMargin(potentialHeight);
				int height = layout.Chart.Height - top;
					height = height - layout.Chart.Margins.GetChartBottomMargin(potentialHeight);
				int width = this.LegendWidth(layout);
				width = width - layout.Chart.Margins.GetDefaultMargin(width);
				return new Rectangle(left, top, width, height);
			}
			return new Rectangle(0, 0, 0, 0);
		}

		public override Rectangle PlotBounds(ChartLayout layout)
		{                        
                int left = 0;
                int top = this.TitleBounds(layout).Bottom;
                int width = layout.Chart.Width;
                int height = layout.Chart.Height;
                    
                if (layout.Chart.TitleVisible == true)
                {
                    height -= this.TitleBounds(layout).Bottom;
                }

                if (layout.Chart.HasLegend)
                {
                    width = this.LegendBounds(layout).Left;
                }
                return new Rectangle(left, top, width, height);
		}
	}

	internal class ChartLayoutModeLegendLeft : ChartLayoutModeTitleTop 
	{
		public override Rectangle LegendBounds(ChartLayout layout)
		{
			if (layout.Chart.HasLegend) 
			{
				int potentialHeight = layout.Chart.Height - this.TitleBounds(layout).Bottom;
				int top = this.TitleBounds(layout).Bottom + layout.Chart.Margins.GetDefaultMargin(potentialHeight);
				int height = layout.Chart.Height - top;
				height = height - layout.Chart.Margins.GetChartBottomMargin(potentialHeight);
				int width = this.LegendWidth(layout);
				int left = layout.Chart.Margins.GetDefaultMargin(width);
				width = width - layout.Chart.Margins.GetDefaultMargin(width);
				return new Rectangle(left, top, width, height);
			}
			return new Rectangle(0, 0, 0, 0);
		}

		public override Rectangle PlotBounds(ChartLayout layout)
		{

            if (layout.Chart.TitleVisible == true)
            {
                int left = 0;
                int top = this.TitleBounds(layout).Bottom;
                int width = layout.Chart.Width;
                int height = layout.Chart.Height;

                height -= -this.TitleBounds(layout).Bottom;
                if (layout.Chart.HasLegend)
                {
                    left = this.LegendBounds(layout).Right;
                    width = layout.Chart.Width - this.LegendBounds(layout).Width;
                }
                return new Rectangle(left, top, width, height);
            }
            return new Rectangle(0, 0, 0, 0);
		}
	}

	internal class ChartLayoutModeLegendBottom : ChartLayoutModeTitleTop 
	{
		public override Rectangle LegendBounds(ChartLayout layout)
		{
			if (layout.Chart.HasLegend) 
			{
				int left = layout.Chart.Margins.GetDefaultMargin(layout.Chart.Width);
				int top = layout.Chart.Height - this.LegendHeight(layout);
				top -= layout.Chart.Margins.GetDefaultMargin(this.LegendHeight(layout));
				int height = this.LegendHeight(layout);
				int width = layout.Chart.Width - 2 * layout.Chart.Margins.GetDefaultMargin(layout.Chart.Width);
				return new Rectangle(left, top, width, height);
			}
			return new Rectangle(0, 0, 0, 0);
		}

		public override Rectangle PlotBounds(ChartLayout layout)
		{

			int left = 0;
			int top = this.TitleBounds(layout).Bottom;
            if (layout.Chart.TitleVisible == false)
                top = 0;

			int height = layout.Chart.Height - top;
			int width = layout.Chart.Width;
			if (layout.Chart.HasLegend) 
			{
				height = this.LegendBounds(layout).Top - top;
			}
			return new Rectangle(left, top, width, height);
		}
	}

	internal class ChartLayoutModeLegendFloating : ChartLayoutModeTitleTop
	{
		public override Rectangle LegendBounds(ChartLayout layout)
		{
            if (layout.Chart.HasLegend)
			    return layout.Chart.Legend.Bounds;
            return new Rectangle(0, 0, 0, 0);                
		}

		public override Rectangle PlotBounds(ChartLayout layout)
		{
			int left = 0;
			int top = this.TitleBounds(layout).Bottom;
			int width = layout.Chart.Width;
			int height = layout.Chart.Height - top;
			return new Rectangle(left, top, width, height);
		}
	}

	internal class ChartLayoutModeLegendUnderTitle : ChartLayoutModeTitleTop 
	{
		public override Rectangle LegendBounds(ChartLayout layout)
		{
			if (layout.Chart.HasLegend) 
			{
				int left = layout.Chart.Margins.GetDefaultMargin(layout.Chart.Width);
				int top = this.TitleBounds(layout).Bottom;
                if (layout.Chart.TitleVisible == false)
                    top = 0;

				int height = this.LegendHeight(layout);
				int width = layout.Chart.Width - 2 * layout.Chart.Margins.GetDefaultMargin(layout.Chart.Width);
				return new Rectangle(left, top, width, height);
			}
			return new Rectangle(0, 0, 0, 0);
		}

		public override Rectangle PlotBounds(ChartLayout layout)
		{
                int left = 0;
                int top = this.TitleBounds(layout).Bottom;
                int width = layout.Chart.Width;
                int height = layout.Chart.Height - this.TitleBounds(layout).Bottom;
                if (layout.Chart.HasLegend)
                {
                    top = LegendBounds(layout).Bottom;
                    height = layout.Chart.Height - this.LegendBounds(layout).Bottom;
                }
                return new Rectangle(left, top, width, height);
            
		}
	}
}
