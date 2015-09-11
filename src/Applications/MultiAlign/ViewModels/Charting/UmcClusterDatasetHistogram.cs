using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Drawing;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MultiAlign.ViewModels.Charting
{
    public class UmcClusterDatasetHistogram : PlotBase
    {
        protected readonly CategoryAxis m_xAxis;

        public UmcClusterDatasetHistogram(Dictionary<int, int> histogram, string name) :
            base(name)
        {
            var axis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                MinorStep = 1,
                LabelField = "Datasets",
                AbsoluteMinimum = 0,
                GapWidth = 0
            };

            // Count axis
            var linearAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                MaximumPadding = .15,
                AbsoluteMinimum = 1,
                Minimum = 0,
                MinimumPadding = 1
            };

            Model.IsLegendVisible = true;
            Model.Axes.Add(axis);
            Model.Axes.Add(linearAxis);

            // Add the data to the view model
            var data = new ColumnSeries
            {
                ValueField = "Value",
                StrokeThickness = 1,
                LabelFormatString = "{0}",
            };

            var colors = new ColorTypeIterator();
            foreach (var bin in histogram.OrderBy(h => h.Key))
            {
                axis.Labels.Add(bin.Key.ToString());
                int number = histogram[bin.Key];

                var column = new ColumnItem(number)
                {
                    Color = colors.GetColor(bin.Key)
                };
                data.Items.Add(column);
            }

            m_xAxis = axis;
            Model.Series.Add(data);

            Model.Axes[0].MajorGridlineStyle = LineStyle.Solid;
            Model.Axes[1].MajorGridlineStyle = LineStyle.Solid;
        }
    }
}
