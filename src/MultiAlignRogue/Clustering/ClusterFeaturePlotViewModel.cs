using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MultiAlignRogue.Utils;
using MultiAlignRogue.ViewModels;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MultiAlignRogue.Clustering
{
    public class ClusterFeaturePlotViewModel : PlotViewModelBase
    {
        private readonly LinearAxis clusterFeaturePlotXaxis;

        private readonly LinearAxis clusterFeaturePlotYaxis;

        public RelayCommand SavePlotCommand { get; }

        /// <summary>
        /// The features to display.
        /// </summary>
        private IEnumerable<UMCLightViewModel> features;

        public ClusterFeaturePlotViewModel()
        {
            this.SavePlotCommand = new RelayCommand(this.SavePlot);

            this.ClusterFeaturePlotModel = new PlotModel();
            this.clusterFeaturePlotXaxis = new LinearAxis
            {
                Title = "NET",
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 1.0
            };
            this.ClusterFeaturePlotModel.Axes.Add(this.clusterFeaturePlotXaxis);

            this.clusterFeaturePlotYaxis = new LinearAxis
            {
                Title = "Mass",
                Position = AxisPosition.Left,
                AbsoluteMinimum = 0
            };
            this.ClusterFeaturePlotModel.Axes.Add(this.clusterFeaturePlotYaxis);

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
        public PlotModel ClusterFeaturePlotModel { get; }

        /// <summary>
        /// Gets or sets the features to display chromatograms for.
        /// </summary>
        public IEnumerable<UMCLightViewModel> Features
        {
            get => this.features;
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

            foreach (var datasetFeatures in this.Features.GroupBy(feat => feat.UMCLight.GroupId))
            {
                var unselectedFeatures = datasetFeatures.Where(feat => !feat.Selected).ToArray();
                var color = this.Colors[datasetFeatures.Key % this.Colors.Count];
                var unselectedColor = color.ChangeSaturation(0.25);
                if (unselectedFeatures.Any())
                {
                    var unselectedSeries = new ScatterSeries
                    {
                        ItemsSource = unselectedFeatures,
                        MarkerFill = unselectedColor,
                        TrackerFormatString = trackerFormatString,
                    };
                    this.ClusterFeaturePlotModel.Series.Add(unselectedSeries);
                }

                var selectedFeatures = datasetFeatures.Where(feat => feat.Selected).ToArray();
                if (selectedFeatures.Any())
                {
                    var selectedSeries = new ScatterSeries
                    {
                        ItemsSource = selectedFeatures,
                        MarkerFill = color,
                        TrackerFormatString = trackerFormatString
                    };
                    this.ClusterFeaturePlotModel.Series.Add(selectedSeries);
                }
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

            var ppm = massRange / maxMass * 1e6;

            var verticalAnnotation = new LineAnnotation
            {
                X = maxNet + (0.05 * netRange),
                MinimumY = Math.Max(minMass - (0.05 * massRange), 0),
                MaximumY = maxMass + (0.05 * massRange),
                TextColor = OxyColors.Gray,
                Text = string.Format("{0} ({1} ppm)", massRange.ToString("0.###"), ppm.ToString("0.###")),
                TextOrientation = AnnotationTextOrientation.Vertical,
                LineStyle = LineStyle.Dash,
                Type = LineAnnotationType.Vertical,
            };

            var horizontalAnnotation = new LineAnnotation
            {
                Y = minMass - (0.05 * massRange),
                MinimumX = Math.Max(minNet - (0.05 * netRange), 0),
                MaximumX = maxNet + (0.05 * netRange),
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

            netRange = netRange.Equals(0) ? 0.1 : netRange;
            massRange = massRange.Equals(0) ? 1.0 : massRange;

            minNet = Math.Max(minNet - (0.095 * netRange), 0);
            maxNet = maxNet + (0.095 * netRange);
            minMass = Math.Max(minMass - (0.095 * massRange), 0);
            maxMass = maxMass + (0.095 * massRange);

            this.clusterFeaturePlotXaxis.Minimum = minNet;
            this.clusterFeaturePlotXaxis.Maximum = maxNet;
            this.clusterFeaturePlotYaxis.Minimum = minMass;
            this.clusterFeaturePlotYaxis.Maximum = maxMass;
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

        public void SavePlot()
        {
            PlotSavingViewModel.SavePlot(this.ClusterFeaturePlotModel, 800, 600, "ClusterFeaturePlot");
        }
    }
}
