#region

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PNNLOmics.Annotations;

#endregion

namespace MultiAlignCore.Drawing
{
    /// <summary>
    /// Heatmap display.  Default color legend is "Hot"
    /// </summary>
    [UsedImplicitly]
    public class Heatmap : PlotBase
    {
        public Heatmap(double[,] data)
            : this("Heat map", "", "", data)
        {
        }

        public Heatmap(string title, string xTitle, string yTitle, double[,] data)
            : base(title)
        {
            var model = new PlotModel
            {
                Title = title,
            };

            OxyPalette palette = OxyPalettes.Hot(200);
            var colorAxis = new LinearColorAxis
            {
                InvalidNumberColor = OxyColors.Gray,
                Position = AxisPosition.Right,
                Palette = palette
            };
            model.Axes.Add(colorAxis);

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

            var series = new HeatMapSeries
            {
                X0 = 0,
                X1 = 1,
                Y0 = 0,
                Y1 = 1,
                FontSize = .2
            };
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            series.Data = new double[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    series.Data[i, j] = data[i, j];
                }
            }

            model.Series.Add(series);
            Model = model;
        }
    }
}