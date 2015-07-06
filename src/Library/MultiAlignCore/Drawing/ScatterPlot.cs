#region

using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PNNLOmics.Annotations;

#endregion

namespace MultiAlignCore.Drawing
{
    /// <summary>
    ///     Heatmap display.  Default color legend is "Hot"
    /// </summary>
    [UsedImplicitly]
    public class ScatterPlot : PlotBase
    {
        public ScatterPlot()
            : this("Scatter Plot", "", "")
        {
        }

        /// <summary>
        ///     Creates a new plot with linear axis.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="xTitle"></param>
        /// <param name="yTitle"></param>
        public ScatterPlot(string title, string xTitle, string yTitle)
        {
            var model = new PlotModel
            {
                Title = title,
            };

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = xTitle
            };
            model.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Title = yTitle
            };
            model.Axes.Add(yAxis);
            Model = model;
        }

        /// <summary>
        ///     Adds a series of x,y points to the plot
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="title"></param>
        /// <param name="fill"></param>
        [UsedImplicitly]
        public void AddSeries(IEnumerable<double> x, IEnumerable<double> y, string title, OxyColor fill)
        {
            var series = new ScatterSeries
            {
                MarkerFill = fill,
                MarkerSize = 2,
                MarkerStrokeThickness = 0,
                MarkerType = MarkerType.Diamond,
                Title = title
            };

            double[] xPoints = x as double[] ?? x.ToArray();
            double[] yPoints = y as double[] ?? y.ToArray();

            if (xPoints.Length != yPoints.Length)
                throw new Exception("The two data arrays must be of equal length.");

            for (int i = 0; i < xPoints.Length; i++)
            {
                var point = new ScatterPoint(xPoints[i], yPoints[i]);
                series.Points.Add(point);
            }

            Model.Series.Add(series);
        }
    }
}