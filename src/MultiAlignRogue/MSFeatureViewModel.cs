using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MultiAlignCore.Data.MetaData;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PNNLOmics.Data.Features;
using ScatterSeries = OxyPlot.Series.ScatterSeries;

namespace MultiAlignRogue
{
    class MSFeatureViewModel : PlotViewModelBase
    {
        public PlotModel Model { get; private set; }

        private readonly LinearAxis netAxis;
        private readonly LinearAxis massAxis;

        private int numSections;
        private int featuresPerSection;

        private IList<UMCLight> features; 

        public MSFeatureViewModel()
        {

        }

        public MSFeatureViewModel(Dictionary<DatasetInformation, IList<UMCLight>> features)
        {
            this.Model = new PlotModel
            {
                Title = "MS Features",
                IsLegendVisible = true,
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.LeftTop,
                LegendOrientation = LegendOrientation.Vertical
            };

            this.netAxis = new LinearAxis
            {
                Title = "Scan",
                Position = AxisPosition.Bottom,
                IsAxisVisible = true,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 1.0
            };

            this.massAxis = new LinearAxis
            {
                Title = "Monoisotopic Mass",
                Position = AxisPosition.Left,
                IsAxisVisible = true,
                AbsoluteMinimum = 0
            };

            this.Model.Axes.Add(this.netAxis);
            this.Model.Axes.Add(this.massAxis);
            PlotFeatures(features);
            
        }

        public IEnumerable<DataPoint> GetPoints(IList<UMCLight> Features, int maxScan)
        {
            var dataPoints = new List<DataPoint>();
            foreach (var feature in Features)
            {
                dataPoints.Add(new DataPoint(feature.ScanStart / (double)maxScan, feature.MassMonoisotopic));
                dataPoints.Add(new DataPoint(feature.ScanEnd / (double)maxScan, feature.MassMonoisotopic));
                dataPoints.Add(new DataPoint(Double.NaN, feature.MassMonoisotopic));
            }
            return dataPoints;
        }

        private void PlotFeatures(Dictionary<DatasetInformation, IList<UMCLight>> Features)
        {
            try
            {
                int i = 0;
                foreach (var file in Features.Keys)
                {
                    LineSeries currentFeatures = new LineSeries
                    {
                        Color = Colors.ElementAt(i),
                        Title = file.DatasetName,
                        StrokeThickness = 0.8
                    };
                    currentFeatures.Points.AddRange(GetPoints(Features[file], file.ScanTimes.Keys.Max()));
                    this.Model.Series.Add(currentFeatures);
                    i = (i + 1)%Colors.Count; //Cycle through available colors if we run out
                }
            }
            catch
            {
                MessageBox.Show("Make sure that the selected files have detected features.");
            }
        }
    }
}
