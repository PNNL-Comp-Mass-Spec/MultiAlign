using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

        private int numSectionsPerAxis;
        private int featuresPerSection;

        ////private IList<UMCLight> features; 

        private Dictionary<DatasetInformation, IList<UMCLight>> features;

        public MSFeatureViewModel()
        {

        }

        public MSFeatureViewModel(Dictionary<DatasetInformation, IList<UMCLight>> features)
        {
            this.numSectionsPerAxis = 10;
            this.featuresPerSection = 1;

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

            ////netAxis.AxisChanged += (s, e) => this.PlotFeatures(this.features);

            this.Model.Axes.Add(this.netAxis);
            this.Model.Axes.Add(this.massAxis);
            PlotFeatures(features);
        }

        public IEnumerable<DataPoint> GetPoints(IEnumerable<UMCLight> Features, int maxScan)
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
            this.features = Features;
            double globalMaxMass = 0.0;
            foreach (var dataset in Features)
            {
                var max = dataset.Value.Max(feat => feat.MassMonoisotopic);

                if (max >= globalMaxMass)
                {
                    globalMaxMass = max;
                }
            }

            this.massAxis.AbsoluteMaximum = globalMaxMass;

            this.Model.Series.Clear();

            try
            {
                int i = 0;
                foreach (var file in Features.Keys)
                {
                    LineSeries currentFeatures = new LineSeries
                    {
                        Color = Colors.ElementAt(i),
                        Title = file.DatasetName,
                        StrokeThickness = 0.8,
                    };
                    ////currentFeatures.Points.AddRange(GetPartitionedPoints(Features[file], file.ScanTimes.Keys.Max(), globalMaxMass));
                    currentFeatures.Points.AddRange(GetPoints(Features[file], file.ScanTimes.Keys.Max()));
                    this.Model.Series.Add(currentFeatures);
                    i = (i + 1)%Colors.Count; //Cycle through available colors if we run out
                }
            }
            catch
            {
                MessageBox.Show("Make sure that the selected files have detected features.");
            }

            this.Model.InvalidatePlot(true);
        }

        private IEnumerable<DataPoint> GetPartitionedPoints(IList<UMCLight> Features, int maxScan, double globalMax)
        {
            var netActMaximum = netAxis.ActualMaximum.Equals(0) ? 1.0 : netAxis.ActualMaximum;
            var massActMaximum = massAxis.ActualMaximum.Equals(0) ? globalMax : massAxis.ActualMaximum;

            var netStep = (netActMaximum - netAxis.ActualMinimum) / this.numSectionsPerAxis;
            var massStep = (massActMaximum - massAxis.ActualMinimum) / this.numSectionsPerAxis;

            var featureHash = new HashSet<UMCLight>();

            for (int i = 0; i < this.numSectionsPerAxis; i++)
            {
                var netMin = this.netAxis.ActualMinimum + (i * netStep);
                var netMax = this.netAxis.ActualMinimum + ((i + 1) * netStep);
                var lowScan = netMin * maxScan;
                var hiScan = netMax  *maxScan;
                for (int j = 0; j < this.numSectionsPerAxis; j++)
                {
                    var massMin = this.massAxis.ActualMinimum + (i * massStep);
                    var massMax = this.massAxis.ActualMinimum + ((i + 1) * massStep);

                    var featureRange = Features.Where(feat => !featureHash.Contains(feat) &&
                                                              (feat.MassMonoisotopic >= massMin &&
                                                               feat.MassMonoisotopic <= massMax) &&
                                                              (feat.ScanStart >= lowScan || feat.ScanEnd <= hiScan ||
                                                               (feat.ScanStart <= lowScan && feat.ScanStart >= hiScan)))
                        .OrderByDescending(feat => feat.Abundance).Take(this.featuresPerSection).AsParallel();
                    featureHash.UnionWith(featureRange);
                }
            }

            return this.GetPoints(featureHash, maxScan);
        }
    }
}
