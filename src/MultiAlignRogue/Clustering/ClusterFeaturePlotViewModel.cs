using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MultiAlignRogue.ViewModels;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using PNNLOmics.Data.Features;

namespace MultiAlignRogue.Clustering
{
    public class ClusterFeaturePlotViewModel : ViewModelBase
    {
        private readonly LinearAxis clusterFeaturePlotXaxis;

        private readonly LinearAxis clusterFeatureplotYaxis;

        /// <summary>
        /// The features to display.
        /// </summary>
        private IEnumerable<UMCLightViewModel> features;

        public ClusterFeaturePlotViewModel()
        {
            this.ClusterFeaturePlotModel = new PlotModel();
            this.clusterFeaturePlotXaxis = new LinearAxis
            {
                Title = "NET",
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 1.0
            };
            this.ClusterFeaturePlotModel.Axes.Add(this.clusterFeaturePlotXaxis);

            this.clusterFeatureplotYaxis = new LinearAxis
            {
                Title = "Mass",
                Position = AxisPosition.Left,
                AbsoluteMinimum = 0
            };
            this.ClusterFeaturePlotModel.Axes.Add(this.clusterFeatureplotYaxis);

            this.ClusterFeaturePlotModel.MouseDown += this.FeatureClusterPlotMouseDown;

            Messenger.Default.Register<PropertyChangedMessage<bool>>(
                this,
                args =>
                {
                    if (args.Sender is UMCLightViewModel)
                    {
                        this.BuildClusterFeaturePlot(false);
                        this.ClusterFeaturePlotModel.InvalidatePlot(true);
                    }
                });
        }

        /// <summary>
        /// Gets the plot model for the selected cluster's feature plot.
        /// </summary>
        public PlotModel ClusterFeaturePlotModel { get; private set; }

        /// <summary>
        /// Gets or sets the features to display chromatograms for.
        /// </summary>
        public IEnumerable<UMCLightViewModel> Features
        {
            get { return this.features; }
            set
            {
                if (this.features != value)
                {
                    this.features = value;
                    this.BuildClusterFeaturePlot(true);
                    this.RaisePropertyChanged();
                }
            }
        }

        private void BuildClusterFeaturePlot(bool setAnnotation = true)
        {
            this.ClusterFeaturePlotModel.Series.Clear();

            var trackerFormatString = "{1}: {2:0.###}" + Environment.NewLine +
                                      "{3}: {4:0.###}";

            var unselectedFeatures = this.Features.Where(feat => !feat.Selected).ToArray();
            var unselectedSeries = new ScatterSeries
            {
                ItemsSource = unselectedFeatures,
                MarkerFill = OxyColors.Red,
                TrackerFormatString = trackerFormatString,
            };
            if (unselectedFeatures.Length > 0)
            {
                this.ClusterFeaturePlotModel.Series.Add(unselectedSeries);
            }

            var selectedFeatures = this.Features.Where(feat => feat.Selected).ToArray();
            var selectedSeries = new ScatterSeries
            {
                ItemsSource = selectedFeatures,
                MarkerFill = OxyColors.Gold,
                TrackerFormatString = trackerFormatString
            };
            if (selectedFeatures.Length > 0)
            {
                this.ClusterFeaturePlotModel.Series.Add(selectedSeries);
            }

            if (setAnnotation)
            {
                this.SetAnnotations();
            }

            this.ClusterFeaturePlotModel.InvalidatePlot(true);
        }

        private void SetAnnotations()
        {
            this.ClusterFeaturePlotModel.Annotations.Clear();
            double minNet = double.PositiveInfinity;
            double maxNet = 0;
            double minMass = double.PositiveInfinity;
            double maxMass = 0;
            foreach (var feature in this.Features)
            {
                minNet = Math.Min(feature.UMCLight.NetAligned, minNet);
                maxNet = Math.Max(feature.UMCLight.NetAligned, maxNet);
                minMass = Math.Min(feature.UMCLight.MassMonoisotopicAligned, minMass);
                maxMass = Math.Max(feature.UMCLight.MassMonoisotopicAligned, maxMass);
            }

            var netRange = maxNet - minNet;
            var massRange = maxMass - minMass;

            var verticalAnnotation = new LineAnnotation
            {
                X = maxNet + (0.01 * maxNet),
                MinimumY = Math.Max(minMass - (0.01 * minMass), 0),
                MaximumY = maxMass + (0.01 * maxMass),
                TextColor = OxyColors.Gray,
                Text = massRange.ToString("0.###"),
                TextOrientation = AnnotationTextOrientation.Vertical,
                LineStyle = LineStyle.Dash,
                Type = LineAnnotationType.Vertical,
            };

            var horizontalAnnotation = new LineAnnotation
            {
                Y = minMass - (0.01 * minMass),
                MinimumX = Math.Max(minNet - (0.01 * minNet), 0),
                MaximumX = maxNet + (0.01 * maxNet),
                TextColor = OxyColors.Gray,
                Text = netRange.ToString("0.###"),
                TextOrientation = AnnotationTextOrientation.Horizontal,
                LineStyle = LineStyle.Dash,
                Type = LineAnnotationType.Horizontal,
            };

            if (this.Features.Count() > 1)
            {
                this.ClusterFeaturePlotModel.Annotations.Add(verticalAnnotation);
                this.ClusterFeaturePlotModel.Annotations.Add(horizontalAnnotation);
            }

            minNet = Math.Max(minNet - (0.025 * minNet), 0);
            maxNet = maxNet + (0.025 * maxNet);
            minMass = Math.Max(minMass - (0.025 * minMass), 0);
            maxMass = maxMass + (0.025 * maxMass);

            this.clusterFeaturePlotXaxis.Minimum = minNet;
            this.clusterFeaturePlotXaxis.Maximum = maxNet;
            this.clusterFeatureplotYaxis.Minimum = minMass;
            this.clusterFeatureplotYaxis.Maximum = maxMass;
        }

        /// <summary>
        /// Event handler for OxyPlot left mouse click.
        /// </summary>
        /// <param name="sender">The sending PlotView.</param>
        /// <param name="args">The event arguments.</param>
        private void FeatureClusterPlotMouseDown(object sender, OxyMouseEventArgs args)
        {
            var series = this.ClusterFeaturePlotModel.GetSeriesFromPoint(args.Position, 10);
            if (series != null)
            {
                var result = series.GetNearestPoint(args.Position, false);
                if (result != null)
                {
                    var featurePoint = result.Item as UMCLightViewModel;
                    if (featurePoint != null)
                    {
                        featurePoint.Selected = !featurePoint.Selected;
                        this.BuildClusterFeaturePlot(false);
                    }
                }
            }
        }
    }
}
