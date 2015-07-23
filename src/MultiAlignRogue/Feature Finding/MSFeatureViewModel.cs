namespace MultiAlignRogue.Feature_Finding
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows;

    using MultiAlignCore.Data.MetaData;

    using MultiAlignRogue.Utils;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    using PNNLOmics.Data.Features;

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

        /// <summary>
        /// All features on plot.
        /// </summary>
        private readonly Dictionary<DatasetInformation, IList<UMCLight>> allFeatures;

        public MSFeatureViewModel()
        {
            
        }

        public MSFeatureViewModel(Dictionary<DatasetInformation, IList<UMCLight>> features, int numSectionsPerAxis = 10, int featuresPerSection = 50)
        {
            this.allFeatures = features;
            this.numSectionsPerAxis = numSectionsPerAxis;
            this.featuresPerSection = featuresPerSection / features.Keys.Count;
            this.throttler = new Throttler(TimeSpan.FromMilliseconds(100));
            this.globalMaxMass = this.GetGlobalMaxMass(features);

            this.quadTrees = new Dictionary<DatasetInformation, QuadTree<FeaturePoint>>();
            foreach (var dataset in features)
            {
                var maxScan = dataset.Key.ScanTimes.Keys.Max();
                this.quadTrees.Add(
                    dataset.Key,
                    new QuadTree<FeaturePoint>(
                        new RectangleF { X = 0, Y = 0, Width = maxScan, Height = (float)this.globalMaxMass }));
                foreach (var feature in dataset.Value)
                {
                    this.quadTrees[dataset.Key].Insert(new FeaturePoint(feature));
                }
            }


            this.Model = new PlotModel
            {
                Title = "MS Features",
                IsLegendVisible = true,
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.LeftTop,
                LegendOrientation = LegendOrientation.Vertical
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

            var massPercent = (this.massAxis.ActualMaximum - this.massAxis.ActualMinimum) / this.massAxis.AbsoluteMaximum;
            var rtPercent = this.netAxis.ActualMaximum - this.netAxis.ActualMinimum;
            var showMsFeatures = (massPercent <= 0.1) && (rtPercent <= 0.1);

            try
            {
                int i = 0;
                foreach (var file in features.Keys)
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

                    var dataPoints = this.GetPartitionedPoints(
                        this.quadTrees[file],
                        file.ScanTimes.Keys.Max(),
                        this.globalMaxMass,
                        showMsFeatures);

                    currentFeatures.Points.AddRange(dataPoints.Item1);
                    currentMsFeatures.Points.AddRange(dataPoints.Item2);

                    this.Model.Series.Add(currentFeatures);
                    this.Model.Series.Add(currentMsFeatures);
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
        /// <param name="featureTree">QuadTree of features for one dataset.</param>
        /// <param name="maxScan">The maximum LC scan number in the dataset.</param>
        /// <param name="globalMax">The maximum mass in all datasets.</param>
        /// <param name="showMsFeatures">A value indicating whether points with Ms features should be returned.</param>
        /// <returns>
        /// Collection of datapoints for features.
        /// Item 1: LCMS feature datapoints. Item2: MS Feature datapoints.
        /// </returns>
        private Tuple<IEnumerable<DataPoint>, IEnumerable<ScatterPoint>> GetPartitionedPoints(QuadTree<FeaturePoint> featureTree, int maxScan, double globalMax, bool showMsFeatures = false)
        {
            var netActMaximum = this.netAxis.ActualMaximum.Equals(0) ? 1.0 : this.netAxis.ActualMaximum;
            var massActMaximum = this.massAxis.ActualMaximum.Equals(0) ? globalMax : this.massAxis.ActualMaximum;

            var netStep = (netActMaximum - this.netAxis.ActualMinimum) / this.numSectionsPerAxis;
            var massStep = (massActMaximum - this.massAxis.ActualMinimum) / this.numSectionsPerAxis;

            var featureHash = new HashSet<UMCLight>();

            for (int i = 0; i < this.numSectionsPerAxis; i++)
            {
                var netMin = this.netAxis.ActualMinimum + (i * netStep);
                var netMax = this.netAxis.ActualMinimum + ((i + 1) * netStep);
                var lowScan = netMin * maxScan;
                var highScan = netMax  * maxScan;
                for (int j = 0; j < this.numSectionsPerAxis; j++)
                {
                    var massMin = this.massAxis.ActualMinimum + (j * massStep);
                    var massMax = this.massAxis.ActualMinimum + ((j + 1) * massStep);
                    var treeFeatures = featureTree.Query(new RectangleF
                                          {
                                              X = (float)lowScan,
                                              Y = (float)massMin,
                                              Height = (float)(massMax - massMin),
                                              Width = (float)(highScan - lowScan)
                                          });
                    var featureRange =
                        treeFeatures.Select(feat => feat.UMCLight)
                            .OrderByDescending(umc => umc.Abundance)
                            .Take(this.featuresPerSection);

                    featureHash.UnionWith(featureRange);
                }
            }

            return this.GetPoints(featureHash, maxScan, showMsFeatures);
        }

        /// <summary>
        /// Get a list of data points for a collection of features.
        /// </summary>
        /// <param name="features">List of features for one dataset.</param>
        /// <param name="maxScan">The maximum LC scan number in the dataset.</param>
        /// <param name="showMsFeatures">A value indicating whether points with Ms features should be returned.</param>
        /// <returns>
        /// Collection of datapoints for features.
        /// Item 1: LCMS feature datapoints. Item2: MS Feature datapoints.
        /// </returns>
        private Tuple<IEnumerable<DataPoint>, IEnumerable<ScatterPoint>> GetPoints(IEnumerable<UMCLight> features, int maxScan, bool showMsFeatures)
        {
            var lcmsDataPoints = new List<DataPoint>();
            var msDataPoints = new List<ScatterPoint>();
            foreach (var feature in features)
            {
                lcmsDataPoints.Add(new DataPoint(feature.ScanStart / (double)maxScan, feature.MassMonoisotopic));
                lcmsDataPoints.Add(new DataPoint(feature.ScanEnd / (double)maxScan, feature.MassMonoisotopic));

                // Insert NaN point to cause broken line series
                lcmsDataPoints.Add(new DataPoint(double.NaN, feature.MassMonoisotopic));

                if (showMsFeatures)
                {
                    ////foreach (var msfeature in feature.MsFeatures)
                    ////{
                    ////    msDataPoints.Add(new ScatterPoint(msfeature.Net, msfeature.MassMonoisotopic, 0.8));
                    ////}

                    // TODO: Show actual MS features.
                    // This is just a sample to show how I will show MS features.
                    for (int i = feature.ScanStart; i < feature.ScanEnd; i++)
                    {
                        if (i % 20 == 0)
                        {
                            msDataPoints.Add(new ScatterPoint(i / (double)maxScan, feature.MassMonoisotopic, 0.8));   
                        }
                    }
                }
            }

            return new Tuple<IEnumerable<DataPoint>, IEnumerable<ScatterPoint>>(lcmsDataPoints, msDataPoints);
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
    }
}
