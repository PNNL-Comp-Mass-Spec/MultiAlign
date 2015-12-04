using GalaSoft.MvvmLight.Command;
using MultiAlign.Windows.Plots;
using MultiAlignCore.Data.Features;
using NHibernate.Mapping;
using OxyPlot.Annotations;

namespace MultiAlignRogue.Feature_Finding
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows;

    using MultiAlignCore.Data.MetaData;

    using MultiAlignRogue.Utils;

    using NHibernate.Linq.Functions;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    
    using QuadTreeLib;

    class MSFeatureViewModel : PlotViewModelBase
    {
        /// <summary>
        /// NET axis for feature plot (x axis)
        /// </summary>
        private readonly LinearAxis netAxis;

        /// <summary>
        /// Mass axis for feature plot (y axis)
        /// </summary>
        private readonly LinearAxis massAxis;

        /// <summary>
        /// Throttles actions (runs only most recent action in given time period)
        /// </summary>
        private readonly Throttler throttler;

        /// <summary>
        /// Number of sections to filter features within.
        /// </summary>
        private readonly int numSectionsPerAxis;

        /// <summary>
        /// Maximum number of features to be shown per section.
        /// </summary>
        private readonly int featuresPerSection;

        private readonly double globalMaxMass;

        private Dictionary<DatasetInformation, QuadTree<FeaturePoint>> quadTrees;

        private readonly bool aligned;

        /// <summary>
        /// All features on plot.
        /// </summary>
        private readonly Dictionary<DatasetInformation, IList<UMCLight>> allFeatures;

        public RelayCommand SavePlotCommand { get; private set; }

        public MSFeatureViewModel()
        {
            
        }

        public MSFeatureViewModel(Dictionary<DatasetInformation, IList<UMCLight>> features, bool aligned, int numSectionsPerAxis = 10, int featuresPerSection = 50)
        {
            this.SavePlotCommand = new RelayCommand(this.SavePlot);

            this.allFeatures = features;
            this.aligned = aligned;
            this.numSectionsPerAxis = numSectionsPerAxis;
            this.featuresPerSection = featuresPerSection / features.Keys.Count;
            this.throttler = new Throttler(TimeSpan.FromMilliseconds(100));
            this.globalMaxMass = this.GetGlobalMaxMass(features);

            this.quadTrees = new Dictionary<DatasetInformation, QuadTree<FeaturePoint>>();
            foreach (var dataset in features)
            {
                this.quadTrees.Add(
                    dataset.Key,
                    new QuadTree<FeaturePoint>(
                        new RectangleF { X = 0, Y = 0, Width = 1, Height = (float)this.globalMaxMass }));
                foreach (var feature in dataset.Value)
                {
                    var dataset1 = dataset;
                    this.quadTrees[dataset.Key].Insert(new FeaturePoint(feature, scan => (float)this.GetNet(dataset1.Key, scan), aligned));
                }
            }


            this.Model = new PlotModel
            {
                Title = "MS Features",
                IsLegendVisible = true,
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.LeftTop,
                LegendOrientation = LegendOrientation.Vertical,
                ////RenderingDecorator = rc => new XkcdRenderingDecorator(rc)
            };

            // x axis
            this.netAxis = new LinearAxis
            {
                Title = "NET",
                Position = AxisPosition.Bottom,
                IsAxisVisible = true,
                Minimum = 0,
                Maximum = 1.0,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 1.0
            };

            // y axis
            this.massAxis = new LinearAxis
            {
                Title = "Monoisotopic Mass",
                Position = AxisPosition.Left,
                IsAxisVisible = true,
                Minimum = 0,
                Maximum = this.globalMaxMass,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = this.globalMaxMass
            };

            this.Model.Axes.Add(this.netAxis);
            this.Model.Axes.Add(this.massAxis);

            this.netAxis.AxisChanged += (s, e) => this.throttler.Run(() => this.PlotFeatures(this.allFeatures));
            this.massAxis.AxisChanged += (s, e) => this.throttler.Run(() => this.PlotFeatures(this.allFeatures));

            this.throttler.Run(() => this.PlotFeatures(this.allFeatures));
        }

        /// <summary>
        /// Gets the PlotModel for the feature map.
        /// </summary>
        public PlotModel Model { get; private set; }

        /// <summary>
        /// Plot features for each dataset.
        /// </summary>
        /// <param name="features">Dictionary containing list of features for each dataset.</param>
        private void PlotFeatures(Dictionary<DatasetInformation, IList<UMCLight>> features)
        {
            this.Model.Series.Clear();
            this.Model.Annotations.Clear();

            var massPercent = (this.massAxis.ActualMaximum - this.massAxis.ActualMinimum) / this.massAxis.AbsoluteMaximum;
            var netPercent = this.netAxis.ActualMaximum - this.netAxis.ActualMinimum;
            var showMsFeatures = (massPercent <= 0.1) && (netPercent <= 0.1);

            try
            {
                int i = 0;
                foreach (var file in features.Keys)
                {
                    var featurePoints = this.GetPartitionedPoints(
                        file,
                        this.globalMaxMass);

                    if (!this.aligned)
                    {
                        LineSeries currentFeatures = new LineSeries
                        {
                            Color = this.Colors.ElementAt(i),
                            Title = file.DatasetName,
                            StrokeThickness = 0.8,
                        };

                        ScatterSeries currentMsFeatures = new ScatterSeries
                        {
                            MarkerStroke = OxyColors.Black,
                            MarkerType = MarkerType.Circle
                        };

                        foreach (var feature in featurePoints)
                        {
                            if (showMsFeatures && feature.UMCLight.MsFeatures.Count > 0)
                            {
                                var points = this.GetMsFeaturesAndAnnotations(feature, file);
                                currentMsFeatures.Points.AddRange(points.Item2);
                                points.Item1.Stroke = this.Colors.ElementAt(i);
                                this.Model.Annotations.Add(points.Item1);
                            }
                            else
                            {
                                currentFeatures.Points.AddRange(this.GetLcmsScatterPoints(feature));
                            }
                        }

                        if (currentFeatures.Points.Count > 0)
                        {
                            this.Model.Series.Add(currentFeatures);
                        }

                        if (currentMsFeatures.Points.Count > 0)
                        {
                            this.Model.Series.Add(currentMsFeatures);
                        }   
                    }
                    else
                    {
                        var lcmsScatterSeries = new ScatterSeries
                        {
                            MarkerStroke = this.Colors.ElementAt(i),
                            Title = file.DatasetName,
                            MarkerType = MarkerType.Circle
                        };
                        lcmsScatterSeries.Points.AddRange(featurePoints.Select(feature => new ScatterPoint(feature.Rectangle.X, feature.Rectangle.Y, 0.8)));  
                        this.Model.Series.Add(lcmsScatterSeries);
                    }

                    i = (i + 1) % this.Colors.Count; // Cycle through available colors if we run out
                }
            }
            catch
            {
                MessageBox.Show("Make sure that the selected files have detected features.");
            }

            this.Model.InvalidatePlot(true);
        }

        /// <summary>
        /// Partition the current view into (numSectionsPerAxis)^2 sections and select the top 
        /// "featuresPerSection" in each section.
        /// </summary>
        /// <param name="dataset">Dataset to get features points for.</param>
        /// <param name="globalMax">The maximum mass in all datasets.</param>
        /// <returns>
        /// Collection of datapoints for features.
        /// Item 1: LCMS feature datapoints. Item2: MS Feature datapoints.
        /// </returns>
        private IEnumerable<FeaturePoint> GetPartitionedPoints(DatasetInformation dataset, double globalMax)
        {
            var netActMaximum = this.netAxis.ActualMaximum.Equals(0) ? 1.0 : this.netAxis.ActualMaximum;
            var massActMaximum = this.massAxis.ActualMaximum.Equals(0) ? globalMax : this.massAxis.ActualMaximum;

            var netStep = (netActMaximum - this.netAxis.ActualMinimum) / this.numSectionsPerAxis;
            var massStep = (massActMaximum - this.massAxis.ActualMinimum) / this.numSectionsPerAxis;

            var featureHash = new HashSet<FeaturePoint>();

            var featureTree = this.quadTrees[dataset];

            for (int i = 0; i < this.numSectionsPerAxis; i++)
            {
                var netMin = this.netAxis.ActualMinimum + (i * netStep);
                var netMax = this.netAxis.ActualMinimum + ((i + 1) * netStep);
                for (int j = 0; j < this.numSectionsPerAxis; j++)
                {
                    var massMin = this.massAxis.ActualMinimum + (j * massStep);
                    var massMax = this.massAxis.ActualMinimum + ((j + 1) * massStep);
                    var treeFeatures = featureTree.Query(new RectangleF
                                          {
                                              X = (float)netMin,
                                              Y = (float)massMin,
                                              Height = (float)(massMax - massMin),
                                              Width = (float)(netMax - netMin)
                                          });
                    var featureRange = treeFeatures.OrderByDescending(feat => feat.UMCLight.Abundance)
                                                   .Take(this.featuresPerSection);

                    featureHash.UnionWith(featureRange);
                }
            }

            return featureHash;
        }

        /// <summary>
        /// Get a list of data points for a collection of features.
        /// </summary>
        /// <param name="features">List of features for one dataset.</param>
        /// <returns>
        /// Collection of datapoints for features.
        /// Item 1: LCMS feature datapoints. Item2: MS Feature datapoints.
        /// </returns>
        private IEnumerable<DataPoint> GetLcmsScatterPoints(FeaturePoint feature)
        {
            var lcmsDataPoints = new List<DataPoint> { Capacity = 3 };
            lcmsDataPoints.Add(new DataPoint(feature.Rectangle.X, feature.UMCLight.MassMonoisotopic));
            lcmsDataPoints.Add(new DataPoint(feature.Rectangle.X + feature.Rectangle.Width, feature.UMCLight.MassMonoisotopic));

            // Insert NaN point to cause broken line series
            lcmsDataPoints.Add(new DataPoint(double.NaN, feature.UMCLight.MassMonoisotopic));

            return lcmsDataPoints;
        }

        /// <summary>Get scatter points for MS features and a rectangle annotation for the LCMS feature.</summary>
        /// <param name="feature">An LCMS feature.</param>
        /// <param name="dataset">The dataset that the LCMS feature comes from.</param>
        /// <returns>The tuple containing the LCMS feature annotation and the MS feature scatter points..</returns>
        private Tuple<RectangleAnnotation, IEnumerable<ScatterPoint>> GetMsFeaturesAndAnnotations(FeaturePoint feature, DatasetInformation dataset)
        {
            var msdataPoints = new List<ScatterPoint> { Capacity = feature.UMCLight.MsFeatures.Count };

            var minNet = double.PositiveInfinity;
            var maxNet = 0.0;
            var minMass = double.PositiveInfinity;
            var maxMass = 0.0;

            foreach (var msfeature in feature.UMCLight.MsFeatures)
            {
                var net = this.GetNet(dataset, msfeature.Scan);
                minNet = Math.Min(minNet, net);
                maxNet = Math.Max(maxNet, net);
                minMass = Math.Min(minMass, msfeature.MassMonoisotopic);
                maxMass = Math.Max(maxMass, msfeature.MassMonoisotopic);
                msdataPoints.Add(new ScatterPoint(net, msfeature.MassMonoisotopic, 0.8));
            }

            var netRange = maxNet - minNet;
            netRange = netRange.Equals(0.0) ? 0.01 : netRange;
            var massRange = maxMass - minMass;
            massRange = Math.Max(1.0, massRange);

            minNet = minNet - (0.25 * netRange);
            maxNet = maxNet + (0.25 * netRange);
            minMass = Math.Max(minMass - (massRange * 0.5), 0);
            maxMass = maxMass + (massRange * 0.5);

            var annotation = new RectangleAnnotation
            {
                MinimumX = minNet,
                MaximumX = maxNet,
                MinimumY = minMass,
                MaximumY = maxMass,
                Fill = OxyColors.Transparent,
                StrokeThickness = 1.0,
            };

            return new Tuple<RectangleAnnotation, IEnumerable<ScatterPoint>>(annotation, msdataPoints);
        }

        /// <summary>
        /// Get maximum mass for all datasets.
        /// </summary>
        /// <param name="features">Features for all datasets.</param>
        /// <returns>Maximum mass for all datasets.</returns>
        private double GetGlobalMaxMass(Dictionary<DatasetInformation, IList<UMCLight>> features)
        {
            double maxMass = 0.0;
            foreach (var dataset in features)
            {
                var max = dataset.Value.Max(feat => feat.MassMonoisotopic);

                if (max >= maxMass)
                {
                    maxMass = max;
                }
            }

            return maxMass;
        }

        private double GetNet(DatasetInformation dataset, int scan)
        {
            var minScan = dataset.ScanTimes.Keys.Min();
            var minEt = dataset.ScanTimes[minScan];

            var maxScan = dataset.ScanTimes.Keys.Max();
            var maxEt = dataset.ScanTimes[maxScan];

            var et = dataset.ScanTimes[scan];

            return (et - minEt) / (maxEt - minEt);
        }

        public void SavePlot()
        {
            string name = "FeaturePlot";
            if (this.allFeatures.Count == 1)
            {
                name = System.IO.Path.Combine(this.allFeatures.Keys.First().DatasetName, name);
            }
            PlotSavingViewModel.SavePlot(this.Model, 1024, 768, name, true);
        }
    }
}
