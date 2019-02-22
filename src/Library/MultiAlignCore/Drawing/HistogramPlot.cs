#region

using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

#endregion

namespace MultiAlignCore.Drawing
{
    /// <summary>
    /// Creates a histogram from the dictionary provided
    /// </summary>
    public sealed class HistogramPlot : PlotBase
    {
        public HistogramPlot(Dictionary<double, int> histogram, string name, string abscissaAxisLabel) :
            base(name)
        {
            Model.PlotMargins = new OxyThickness(double.NaN, 40, 40, double.NaN);
            Model.Title = name;

            var axis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                GapWidth = 0,
                Angle =  -90,
                IsAxisVisible = false
            };
            Model.Axes.Add(axis);

            // Count axis
            var linearAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = abscissaAxisLabel
            };
            Model.Axes.Add(linearAxis);

            // Count axis
            var linearAxis2 = new LinearAxis
            {
                Position = AxisPosition.Left,
                AbsoluteMinimum = 0,
                MinimumPadding = 1,
                Minimum = 0,
                Title =  "Count"
            };
            Model.Axes.Add(linearAxis2);


            // Add the data to the view model
            var data = new ColumnSeries
            {
                FillColor = OxyColors.Red,
                StrokeColor = OxyColors.Black,
                ValueField = "Value",
                StrokeThickness = 2
            };
            var keys = histogram.Keys.OrderBy(x => x);

            if (!keys.Any())
                return;

            var min = keys.Min();
            var max = keys.Max();

            //axis.Minimum = min;
            //axis.Maximum = max;
            //axis.AbsoluteMaximum = max;
            //axis.AbsoluteMinimum = min;
            //linearAxis.AbsoluteMaximum = max;
            //linearAxis.AbsoluteMinimum = min;
            linearAxis.Maximum = max;
            linearAxis.Minimum = min;

            foreach (var key in keys)
            {
                var keyValue = key.ToString();

                axis.Labels.Add(keyValue);
                //axis.ActualLabels.Add(keyValue);

                var number = histogram[key];
                data.Items.Add(new ColumnItem(number));
            }

            Model.Series.Add(data);
        }
    }
}