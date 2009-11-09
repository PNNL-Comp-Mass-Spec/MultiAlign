using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace PNNLControls
{

	public enum ChartLegendLocation 
	{
		Left = 1,
		Bottom = 2,
		Right = 3,
		Floating = 4,
		/// <summary>
		/// under the title but above the plot
		/// </summary>
		UnderTitle = 5
	}

	internal class LegendEntry 
	{
		public clsLabelPlotter label;
		public Rectangle symbolArea;
		public clsSeries series;
		public Rectangle fullArea;
		//public bool selected;

		
		public LegendEntry(clsLabelPlotter label, Rectangle symbolArea, clsSeries series, 
			Rectangle fullArea) 
		{
			this.label = label;
			this.symbolArea = symbolArea;
			this.series = series;
			this.fullArea = fullArea;
		}
	}

	/// <summary>
	/// Summary description for ChartLegend.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class ChartLegend
	{
		/// <summary>
		/// The bounds of the legend, in which stuff can be drawn
		/// </summary>
		private Rectangle bounds;

		/// <summary>
		/// The actual bounding of the legend, which is entirely inside the bounds.
		/// </summary>
		private Rectangle legendBounds;

		/// <summary>
		/// The chart that we're providing a legend for
		/// </summary>
		private ctlChartBase chart;

		private System.Collections.IList legendEntries = new ArrayList();

		private bool hasBorder = true;
		private Color backColor = Color.Transparent;
		private PenProvider borderPenProvider;
		private EventHandler borderPenProviderChangedHandler;
		private Font font = new Font("Microsoft Sans Serif", 10);
		private int entryMargin = 5;
		private int columnWidth = 125;

		private float minFontSize = 6;
		private float maxFontSize = 12;

		public ChartLegend(ctlChartBase chart)
		{
			this.borderPenProviderChangedHandler = new EventHandler(BorderPenProviderChanged);
			this.chart = chart;
			this.BorderPen = new PenProvider();
		}

		public Font Font 
		{
			get 
			{
				return this.font;
			}
			set 
			{
				this.font = value;
				using (Graphics g = chart.CreateGraphics()) 
				{
					this.Layout(g);
				}
				chart.Invalidate();
			}
		}

		[System.ComponentModel.Description("When autosizing fonts, the maximum font size that "
			+ "legend entries can have")]
		[System.ComponentModel.DefaultValue(12)]
		public float MaxFontSize 
		{
			get 
			{
				return this.maxFontSize;
			}
			set 
			{
				this.maxFontSize = value;
				using (Graphics g = chart.CreateGraphics()) 
				{
					this.Layout(g);
				}
				chart.Invalidate();
			}
		}

		[System.ComponentModel.Description("When autosizing fonts, the minimum font size that "
			 + "legend entries can have")]
		[System.ComponentModel.DefaultValue(6)]
		public float MinFontSize 
		{
			get 
			{
				return this.minFontSize;
			}
			set 
			{
				this.minFontSize = value;
				using (Graphics g = chart.CreateGraphics()) 
				{
					this.Layout(g);
				}
				chart.Invalidate();
			}
		}

		public Rectangle Bounds
		{
			get 
			{
				return bounds;
			}
			set 
			{
				bounds = value;
				chart.PerformLayout();
				chart.Invalidate();
			}
		}

		public Rectangle ActualBounds 
		{
			get 
			{
				return this.legendBounds;
			}
		}

//		/// <summary>
//		/// Gets the preferred width of the legend, given the height that 
//		/// it has to fit in.
//		/// </summary>
//		/// <param name="height"></param>
//		/// <returns></returns>
//		public int GetPreferredWidth(int height, int minWidth) 
//		{
//			return 200;
//		}
//
//		/// <summary>
//		/// Gets the preferred height of the legend, given the width that 
//		/// it has to fit in.
//		/// </summary>
//		/// <param name="width"></param>
//		/// <returns></returns>
//		public int GetPreferredHeight(int width, int minHeight) 
//		{
//			return 75;
//		}

		[System.ComponentModel.Description("The margin between entries in the legend, and between the "
			+ "borders of the legend and entries")]
		[System.ComponentModel.DefaultValue(5)]
		public int EntryMargin 
		{
			get 
			{
				return this.entryMargin;
			}
			set 
			{
				if (value < 0) 
				{
					throw new ArgumentException("Value must be >= 0", "EntryMargin");
				}
				this.entryMargin = value;
				using (Graphics g = chart.CreateGraphics()) 
				{
					this.Layout(g);
				}
				this.chart.Invalidate();
			}
		}

		[System.ComponentModel.Description("The multiple of a number of character sizes that each "
			+ " text column in the legend should be.")]
		[System.ComponentModel.DefaultValue(15)]
		public int ColumnWidth
		{
			get 
			{
				return this.columnWidth;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentException("Value must be > 0", "ColumnMult");
				}
				this.columnWidth = value;
				using (Graphics g = chart.CreateGraphics()) 
				{
					this.Layout(g);
				}
				this.chart.Invalidate();
			}
		}

		/// <summary>
		/// Draw the legend.
		/// </summary>
		/// <param name="g">Graphics object with origin at upper-left of chart.</param>
		public void Draw(Graphics g, Color foreColor) 
		{
			int height = this.legendBounds.Height;
			int width = this.legendBounds.Width;
			GraphicsContainer container = g.BeginContainer();
			g.IntersectClip(this.Bounds);
			g.TranslateTransform(this.legendBounds.X, this.legendBounds.Y);
			//Console.WriteLine("Legend filling {1} {0}", this.Bounds, this.BackColor);
			using (Brush fillBrush = new SolidBrush(this.BackColor)) 
			{
				g.FillRectangle(fillBrush, 0, 0, width, height);
			}
			using (Brush highlightBrush = new SolidBrush(SystemColors.Highlight))
			{
				if (this.HasBorder) 
				{
					Pen pen = this.borderPenProvider.Pen;
					pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
					g.DrawRectangle(pen, 0, 0, width - 1, height - 1);
					pen.Dispose();
				}
				foreach (LegendEntry entry in this.legendEntries) 
				{
					if (Contains(chart.SelectedSeries, entry.series)) 
					{
						g.FillRectangle(highlightBrush, entry.fullArea);
					}
					entry.label.Draw(g, foreColor);
					this.chart.PaintLegendSymbol(g, entry.series, entry.symbolArea);
				
				}
			}
			g.EndContainer(container);
		}

		private bool AttemptLayout(Graphics g, bool force) 
		{
			int num_series = this.chart.SeriesCollection.Count;
			this.legendEntries.Clear();
			int writableWidth = this.Bounds.Width;
			int writableHeight = this.bounds.Height;

			// subtract off width of border, if applicable
			if (this.HasBorder) 
			{
				// subtract twice width of border
				writableWidth -= (int) (this.borderPenProvider.Width * 2);
				writableHeight -= (int) (this.borderPenProvider.Width * 2);
			}

			// Determine the number of columns that will be used, and their width
			int columnWidth = this.ColumnWidth;
			int columns = Math.Max(writableWidth / columnWidth, 1);
			columnWidth = writableWidth / columns;

			// The total width and height occupied by all labels and symbols in the legend.
			int totalWidth = 0;
			int totalHeight = 0;
			
			// The height of the current column
			int currentColumnHeight = 0;

			int preferredSymbolHeight = 0;
			int preferredSymbolWidth = 0;
			int currentColumnMaxWidth = 0;
			int columnLeft = 0;

			// find the preferred size for symbols in the legend
			foreach (clsSeries series in this.chart.SeriesCollection) 
			{
				Size preferredSymbolSize = this.chart.GetPreferredLegendSymbolSize(series);
				preferredSymbolHeight = Math.Max(preferredSymbolHeight, preferredSymbolSize.Height);
				preferredSymbolWidth = Math.Max(preferredSymbolWidth, preferredSymbolSize.Width);
			}

			// correct preferred size so that it takes up no more than 1/4 of column width
			preferredSymbolWidth = Math.Min(preferredSymbolWidth, writableWidth / columns / 4);

			// make sure preferred sizes positive
			preferredSymbolWidth = Math.Max(preferredSymbolWidth, 1);
			// preferred height is no more than the height of one line of text
			preferredSymbolHeight = Math.Max(preferredSymbolHeight, (int) Math.Ceiling(this.Font.Size));

			// compute the width of the text portion of the legend entry
			int labelWidth = columnWidth - preferredSymbolWidth - 3 * EntryMargin;
			labelWidth = Math.Max(labelWidth, 1);

			// make sure that every column has at least one entry, even if it's height exceeds
			// that of the legend
			bool atLeastOneInColumn = false;
			// iterate through the series in the chart
			for (int i = 0; i < this.chart.SeriesCollection.Count; i++)
			{
				clsSeries series = this.chart.SeriesCollection[i];
				// create a new label for the legend entry text
				clsLabelPlotter label = new clsLabelPlotter();
				label.Label = series.PlotParams.Name;
				label.Font = this.Font;
				label.Alignment = System.Drawing.StringAlignment.Near;
				label.LineAlignment = System.Drawing.StringAlignment.Center;

				SizeF labelSize = label.GetTextSizeForWidth(g, labelWidth);
				// compute the actual height used
				int height = Math.Max((int) Math.Ceiling(labelSize.Height), preferredSymbolHeight);
				// set the bounds of the label
				label.Bounds = new Rectangle(columnLeft + 2 * EntryMargin + preferredSymbolWidth, 
					currentColumnHeight + EntryMargin, (int) labelSize.Width + EntryMargin, height);

				// Set the rectangle that the symbol for the series will be drawn into
				Rectangle symbolInto = new Rectangle(columnLeft + EntryMargin, 
					currentColumnHeight + EntryMargin, preferredSymbolWidth, height);

				Rectangle fullArea = new Rectangle(columnLeft + EntryMargin, currentColumnHeight + EntryMargin, 
					(int) labelSize.Width + EntryMargin + preferredSymbolWidth, height);

				// If we've exceeded the column height and already put something in this column, 
				// move onto the next column
				if (currentColumnHeight + height + EntryMargin * 2 > writableHeight && atLeastOneInColumn) 
				{
					atLeastOneInColumn = false;
					totalHeight = Math.Max(totalHeight, currentColumnHeight);
					totalWidth += currentColumnMaxWidth;
					// unless we're being forced to layout with the textsize, if the 
					// width exceeds the legend bounds, return false, meaning a smaller text size
					// is needed
					if (totalWidth > this.Bounds.Width && !force) 
					{
						return false;
					}
					// Reset base locations
					currentColumnHeight = 0;
					columnLeft += currentColumnMaxWidth;
					currentColumnMaxWidth = 0;
					// while not all the space is taken up, try repeating with this series
					// in the next column
					if (!(totalWidth > this.Bounds.Width)) 
					{
						i--;
					}
					continue;
				}

				// compute the actual width of the column, since it might be that all labels are
				// significantly shorter than the full width
				currentColumnMaxWidth = Math.Max(currentColumnMaxWidth, (int) labelSize.Width + 3 * EntryMargin + preferredSymbolWidth);

				atLeastOneInColumn = true;
				currentColumnHeight += height + EntryMargin;

				label.Layout(g);
				// Add the entry to the list to be drawn
				this.legendEntries.Add(new LegendEntry(label, symbolInto, series, fullArea));
			}
			totalHeight = Math.Max(totalHeight, currentColumnHeight);
			totalHeight += EntryMargin;
			totalWidth += currentColumnMaxWidth;
			// if no entries drawn, make width the max possible, same for height
			if (totalHeight == EntryMargin) 
			{
				totalHeight = this.Bounds.Height;
			}
			if (totalWidth == 0) 
			{
				totalWidth = this.Bounds.Width;
			}

			// compute the real bounding box of the legend - i.e. the box which 
			// just goes around the entries instead of taking up the whole space
			int left = (this.Bounds.Left + this.Bounds.Right) / 2 - totalWidth / 2;
			int top = (this.Bounds.Top + this.Bounds.Bottom) / 2 - totalHeight / 2;
			if (left < this.Bounds.Left) 
			{
				left = this.Bounds.Left;
			}
			if (top < this.Bounds.Top) 
			{
				top = this.Bounds.Top;
			}
			int trueWidth = Math.Min(totalWidth, this.Bounds.Width);
			int trueHeight = Math.Min(totalHeight, this.Bounds.Height);
			this.legendBounds = new Rectangle(left, top, trueWidth, trueHeight);
			// Return true if all legend entries could be fit within the bounds of the legend
			return totalHeight <= this.Bounds.Height && totalWidth <= this.Bounds.Width;
		}

		private bool Contains(clsSeries[] series, clsSeries s) 
		{
			for (int i = 0; i < series.Length; i++) 
			{
				if (series[i] == s) 
				{
					return true;
				}
			}
			return false;
		}

		public void Layout(Graphics g) 
		{
			// start with max font size and work down, trying to 
			// fit all legend entries within the legend
			if (this.chart.AutoSizeFonts) 
			{
				float fontSize = this.MaxFontSize;
				do  
				{
					this.font = new Font(this.font.FontFamily, fontSize, this.Font.Style);
					fontSize -= 1;
					if (fontSize < this.minFontSize) 
					{
						break;
					}
				} while (!this.AttemptLayout(g, false));													 
			} 
			// Force a layout attempt with the last size, which is either the minimum size, 
			// a size for which an attempted layout succeeded, or the size of the font if autosizing
			// is turned off.
			this.AttemptLayout(g, true);
		}

		public bool IsLegendSizeSelection(System.Windows.Forms.MouseEventArgs args) 
		{
			Rectangle selection = new Rectangle(this.legendBounds.Right - this.EntryMargin,
				this.legendBounds.Bottom - this.EntryMargin,
				this.EntryMargin, this.EntryMargin);
			return selection.Contains(new Point(args.X, args.Y));
		}

		public bool IsLegendMoveSelection(System.Windows.Forms.MouseEventArgs args) 
		{
			// look to see if the mouse click is in any of the legend entries
//			if (!this.ActualBounds.Contains(new Point(args.X, args.Y)) || this.IsLegendEditSelection(args)) 
//			{
//				return false;
//			}
//			return !IsLegendSizeSelection(args);
//			Console.WriteLine("Legend Move Selection {0} {1}", args.X, args.Y);
			return this.ActualBounds.Contains(new Point(args.X, args.Y)) && !IsLegendEditSelection(args);
		}

		public bool IsLegendEditSelection(System.Windows.Forms.MouseEventArgs args) 
		{
			Point legendAt = new Point(args.X - this.legendBounds.X, args.Y - this.legendBounds.Y);
			foreach (LegendEntry entry in this.legendEntries) 
			{
				if (entry.fullArea.Contains(legendAt)) 
				{
					return true;
				}
			}
			return false;
		}

		public clsSeries SeriesAt(int x, int y) 
		{
			x = x - this.ActualBounds.X;
			y = y - this.ActualBounds.Y;
			foreach (LegendEntry entry in legendEntries) 
			{
				if (entry.fullArea.Contains(x, y)) 
				{
					return entry.series;
				}
			}
			return null;
		}

//		public void ProcessMouseClick(System.Windows.Forms.MouseEventArgs args) 
//		{
//			Point at = new Point(args.X, args.Y);
//			if (this.legendBounds.Contains(at) && args.Button == System.Windows.Forms.MouseButtons.Left
//				&& args.Clicks == 1) {
//				Point legendAt = new Point(at.X - this.legendBounds.X, at.Y - this.legendBounds.Y);
//				//Console.WriteLine("Legend at {0}", legendAt);
//				// look to see if the mouse click is in any of the legend entries
//				foreach (LegendEntry entry in this.legendEntries) 
//				{
//					//Console.WriteLine("Entry Area {0}", entry.fullArea);
//					if (entry.fullArea.Contains(legendAt))
//					{
//						// show the dialog to edit how the series is displayed
//						PNNLControls.frmPlotParams editor = new frmPlotParams();
//						clsPlotParams clone = (clsPlotParams) entry.series.PlotParams.Clone();
//						editor.PlotParams = entry.series.PlotParams;
//						System.Windows.Forms.DialogResult result = editor.ShowDialog();
//						// if cancelled, set plot params back to cloned copy
//						if (result == System.Windows.Forms.DialogResult.OK) 
//						{
//							entry.series.PlotParams = editor.PlotParams;
//						}
//						else 
//						{
//							entry.series.PlotParams = clone;	
//						}
//						// don't continue, since we can't be in two legend entries at once
//						return;
//					}
//				}
//			}
//		}

		[System.ComponentModel.DefaultValue(true)]
		[System.ComponentModel.Description("Controls whether a border is drawn around the legend.")]
		public bool HasBorder
		{
			get 
			{
				return this.hasBorder;
			}
			set 
			{
				this.hasBorder = value;
				using (Graphics g = chart.CreateGraphics()) 
				{
					this.Layout(g);
				}
				chart.Invalidate();
			}
		}

		public void ResetHasBorder() 
		{
			this.HasBorder = true;
		}

		public PenProvider BorderPen 
		{
			get 
			{
				return this.borderPenProvider;
			}
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("BorderPen");
				}
				if (this.borderPenProvider != null) 
				{
					this.borderPenProvider.PenChanged -= this.borderPenProviderChangedHandler;
				}
				this.borderPenProvider = value;
				this.borderPenProvider.PenChanged += this.borderPenProviderChangedHandler;
			}
		}

		private void BorderPenProviderChanged(object sender, EventArgs args) 
		{
			this.chart.Invalidate();
		}
		
		public void ResetBorderPen() 
		{
			this.borderPenProvider.Pen = new Pen(Color.Black, 1);
		}

		[System.ComponentModel.Description("The background color of the legend")]
		public Color BackColor
		{
			get 
			{
				return this.backColor;
			}
			set 
			{
				this.backColor = value;
				chart.Invalidate();
			}
		}

		public void ResetBackColor() 
		{
			this.BackColor = Color.Transparent;
		}
	}
}
