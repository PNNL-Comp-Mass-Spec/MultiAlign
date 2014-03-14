using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;

namespace MultiAlign.ViewModels.Charting
{

    public class HistogramViewModel : PlotModelBase
    {
        protected CategoryAxis m_xAxis;
        protected LinearAxis m_yAxis;
        public HistogramViewModel(Dictionary<double, int> histogram, string name) :
            base(name)
        {
            var axis = new CategoryAxis(AxisPosition.Bottom)
            {
                MinorStep = 1,
                LabelField = "Values",
                AbsoluteMinimum = 0,
                GapWidth = 0                
            };

            // Count axis
            var linearAxis = new LinearAxis(AxisPosition.Left, minimum: 0)
            {
                AbsoluteMinimum = 0,
                MinimumPadding = 1,
                Minimum = 0
            };

            Model.Axes.Add(axis);
            Model.Axes.Add(linearAxis);


            // Add the data to the view model
            var data = new ColumnSeries { ValueField = "Value" };
            foreach (var key in histogram.Keys)
            {
                axis.Labels.Add(key.ToString());
                var number = histogram[key];                
                data.Items.Add(new ColumnItem(number));
            }

            m_xAxis = axis;
            m_yAxis = linearAxis;
            Model.Series.Add(data);
        }
    }
}
