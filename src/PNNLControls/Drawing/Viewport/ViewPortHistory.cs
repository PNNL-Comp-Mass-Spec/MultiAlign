using System;
using System.Drawing;

namespace PNNLControls
{
	/// <summary>
	/// Restricts history to holding only non-null RectangleF instances.
	/// </summary>
	internal class ViewPortHistory : History 
	{
		// The chart that we're managing viewport history for.
		private ctlChartBase mChart;

		internal ViewPortHistory(ctlChartBase chart, RectangleF viewPort) : base(viewPort)
		{
			if (chart == null) 
			{
				throw new ArgumentNullException("chart");
			}
			this.mChart = chart;
		}

		protected override void Validate(Object entry)
		{
			if (entry == null || !(entry is System.Drawing.RectangleF)) 
			{
				throw new System.ArgumentException("Entries to ViewPortHistory must be of RectangleF type");
			}
		}

		protected override Object OnPreChangeCurrentEntry(Object potentialValue)
		{
			RectangleF rect = (RectangleF) base.OnPreChangeCurrentEntry (potentialValue);
			// Get the actual bounds for the current settings of the chart (the rectangle 
			// may change because of autozooming features.
			return this.mChart.GetActualViewPort(rect);
		}
	}

	/// <summary>
	/// Customizes how viewport entries are shown in a menu.
	/// </summary>
	internal class ViewPortHistoryMenu : HistoryMenu 
	{
		public ViewPortHistoryMenu(ViewPortHistory history) : base(history) 
		{
		}

		protected override String GetMenuString(int position, Object historyEntry)
		{
			RectangleF rect = (RectangleF) historyEntry;
			return (position + 1).ToString() + " X: " + rect.Left.ToString("f2") + " to " + rect.Right.ToString("f2")
				+ " Y: " + rect.Top.ToString("f2") + " to " + rect.Bottom.ToString("f2");
		}
	}
}
