﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Configuration;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InformedProteomics.Backend.MassFeature;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO;
using MultiAlignRogue.Utils;
using MultiAlignRogue.ViewModels;
using NHibernate.Util;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MultiAlignRogue.Clustering
{
    using MultiAlignCore.Data;
    using MultiAlignCore.IO.RawData;

    public class XicPlotViewModel : PlotViewModelBase
    {
        /// <summary>
        /// For throttling plot updates.
        /// </summary>
        private readonly Throttler plotBuilderthrottler;

        /// <summary>
        /// For throttling Y-Axis scale adjustments.
        /// </summary>
        private readonly Throttler yaxisScaleThrottler;

        /// <summary>
        /// Provider for LCMSRun for access to PBF files.
        /// </summary>
        private ScanSummaryProviderCache rawProvider;

        /// <summary>
        /// The retention time axis.
        /// </summary>
        private readonly LinearAxis xaxis;

        /// <summary>
        /// The abundance axis.
        /// </summary>
        private readonly LinearAxis yaxis;

        /// <summary>
        /// The features to display chromatograms for.
        /// </summary>
        private IEnumerable<UMCLightViewModel> features;

        /// <summary>
        /// The selected charge states.
        /// </summary>
        private IEnumerable<int> selectedChargeStates;

        /// <summary>
        /// The selected MS feature.
        /// </summary>
        private MSFeatureLight selectedMsFeature;

        /// <summary>
        /// A value indicating whether the Y axis on this plot should
        /// automatically zoom based on the visible range in the x axis.
        /// </summary>
        private bool autoScaleYAxis;

        public RelayCommand SavePlotCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XicPlotViewModel"/> class.
        /// </summary>
        public XicPlotViewModel(ScanSummaryProviderCache rawProvider)
        {
            this.rawProvider = rawProvider;
            this.plotBuilderthrottler = new Throttler(TimeSpan.FromMilliseconds(100));
            this.yaxisScaleThrottler = new Throttler(TimeSpan.FromMilliseconds(20));
            this.ChargeStates = new ObservableCollection<ChargeStateViewModel>();
            this.XicPlotModel = new PlotModel
            {
                Title = "Extracted Ion Chromatograms",
                ////RenderingDecorator = rc => new XkcdRenderingDecorator(rc)
            };

            this.AutoScaleYAxis = true;

            this.xaxis = new LinearAxis
            {
                Title = "Retention Time",
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = 0,
                Minimum = 0,
            };

            this.yaxis = new LinearAxis
            {
                Title = "Abundance",
                Position = AxisPosition.Left,
                AbsoluteMinimum = 0,
                Minimum = 0,
                StringFormat = "0.###E0",
                IsPanEnabled = false,
                IsZoomEnabled = false,
            };

            this.xaxis.AxisChanged += (o, e) =>
            {
                if (this.AutoScaleYAxis)
                {
                    this.ScaleYAxis();
                }
            };

            this.SavePlotCommand = new RelayCommand(this.SavePlot);

            this.XicPlotModel.Axes.Add(this.xaxis);
            this.XicPlotModel.Axes.Add(this.yaxis);

            this.XicPlotModel.MouseDown += this.XicPlotMouseDown;

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, this.ChargeStateSelectionChanged);
            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, this.FeatureSelectionChanged);
            Messenger.Default.Register<PropertyChangedMessage<MSFeatureLight>>(this, arg =>
            {
                this.SetMsFeatureAnnotation();
                this.XicPlotModel.InvalidatePlot(true);
            });

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, arg =>
            {
                if (arg.Sender == this && arg.PropertyName == "AutoScaleYAxis")
                {
                    this.yaxis.IsZoomEnabled = !arg.NewValue;
                    this.yaxis.IsPanEnabled = !arg.NewValue;
                    if (arg.NewValue)
                    {
                        this.xaxis.Reset();
                        this.yaxis.Reset();
                        this.ScaleYAxis();
                        this.XicPlotModel.InvalidatePlot(false);
                    }
                }
            });
        }

        /// <summary>
        /// Gets the model for the extracted ion chromatogram plot.
        /// </summary>
        public PlotModel XicPlotModel { get; private set; }

        /// <summary>
        /// Gets all possible charge states that can be selected.
        /// </summary>
        public ObservableCollection<ChargeStateViewModel> ChargeStates { get; private set; }

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
                    this.plotBuilderthrottler.Run(this.BuildPlot);
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected MS feature.
        /// </summary>
        public MSFeatureLight SelectedMsFeature
        {
            get { return this.selectedMsFeature; }
            set
            {
                if (this.selectedMsFeature != value)
                {
                    this.selectedMsFeature = value;
                    this.RaisePropertyChanged("SelectedMsFeature", null, value, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Y axis on this plot should
        /// automatically zoom based on the visible range in the x axis.
        /// </summary>
        public bool AutoScaleYAxis
        {
            get { return this.autoScaleYAxis; }
            set
            {
                if (this.autoScaleYAxis != value)
                {
                    this.autoScaleYAxis = value;
                    this.RaisePropertyChanged("AutoScaleYAxis", !value, value, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the legend on the XIC plot
        /// should be visible.
        /// </summary>
        public bool IsLegendVisible
        {
            get { return this.XicPlotModel.IsLegendVisible; }
            set
            {
                if (this.XicPlotModel.IsLegendVisible != value)
                {
                    this.XicPlotModel.IsLegendVisible = value;
                    this.XicPlotModel.InvalidatePlot(false);
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Builds XIC plot based on selected charge state chromatograms.
        /// </summary>
        private void BuildPlot()
        {
            this.XicPlotModel.Series.Clear();

            double minX = double.PositiveInfinity;
            double maxX = 0;
            double minY = double.PositiveInfinity;
            double maxY = 0;

            var chargeHash = new HashSet<int>();

            MSFeatureLight maxFeature = null;

            int i = 0;
            foreach (var feature in this.Features)
            {
                var chargeMap = feature.UMCLight.CreateChargeMap();
                chargeHash.UnionWith(chargeMap.Keys);

                foreach (var charge in chargeMap.Keys)
                {
                    var scanDataProvider = this.rawProvider.GetScanSummaryProvider(feature.UMCLight.GroupId);
                    var msfeatures = chargeMap[charge];

                    // Get dataset info for mapping scan # -> retention time
                    var dsInfo = SingletonDataProviders.GetDatasetInformation(feature.UMCLight.GroupId);

                    foreach (var msfeature in msfeatures)
                    {
                        var scanSum = scanDataProvider.GetScanSummary(msfeature.Scan);
                        var rt = scanSum.Time;
                        minX = Math.Min(minX, rt);
                        maxX = Math.Max(maxX, rt);
                        minY = Math.Min(minY, msfeature.Abundance);
                        maxY = Math.Max(maxY, msfeature.Abundance);
                    }

                    var maxA = msfeatures.Max(msf => msf.Abundance);
                    var maxL = msfeatures.FirstOrDefault(msf => msf.Abundance.Equals(maxA));
                    if (maxFeature == null || (maxL != null && maxL.Abundance >= maxFeature.Abundance))
                    {
                        maxFeature = maxL;
                    }

                    var color = this.Colors[i++ % this.Colors.Count];
                    var series = new LineSeries
                    {
                        Title = string.Format("{0}({1}+) ID({2})", dsInfo.DatasetName, charge, feature.UMCLight.Id),
                        Color = color,
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 3,
                        MarkerFill = OxyColors.White,
                        MarkerStroke = color,
                        MarkerStrokeThickness = 0.5,
                        TrackerFormatString = "{0}" + Environment.NewLine +
                                   "{1}: {2:0.###} (Scan: {Scan:0})" + Environment.NewLine +
                                   "{3}: {4:0.###E0}" + Environment.NewLine
                    };

                    msfeatures.ForEach(point => series.Points.Add(new DataPoint(
                                                     scanDataProvider.GetScanSummary(point.Scan).Time,
                                                     point.Abundance)));

                    this.XicPlotModel.Series.Add(series);   
                }
            }

            this.SelectedMsFeature = maxFeature;
            this.SetMsFeatureAnnotation();

            this.ChargeStates.Clear();
            foreach (var chargeState in chargeHash)
            {
                this.ChargeStates.Add(new ChargeStateViewModel(chargeState));
            }

            minX = Math.Max(0, minX - (0.01 * minX));
            maxX = maxX + (0.01 * maxX);
            maxY = maxY + (0.05 * maxY);

            this.xaxis.Minimum = minX;
            this.xaxis.Maximum = maxX;
            this.yaxis.Minimum = minY;
            this.yaxis.Maximum = maxY;

            this.xaxis.Zoom(minX, maxX);
            this.yaxis.Zoom(minY, maxY);

            this.XicPlotModel.InvalidatePlot(true);
        }

        private void ScaleYAxis()
        {
            var minX = this.xaxis.ActualMinimum;
            var maxX = this.xaxis.ActualMaximum;
            var maxY = 0.0;
            var minY = Double.NegativeInfinity;
            foreach (var series in this.XicPlotModel.Series.OfType<LineSeries>())
            {
                foreach (var point in series.Points.Where(p => p.X >= minX && p.X <= maxX))
                {
                    maxY = Math.Max(maxY, point.Y);
                    minY = Math.Min(minY, point.Y);
                }
            }

            var paddedMax = maxY + (0.03 * maxY);
            var paddedMin = Math.Max(0, minY - (0.005 * minY));

            this.yaxis.Minimum = paddedMin;
            this.yaxis.Maximum = paddedMax;
            this.yaxis.Zoom(paddedMin, paddedMax);
        }

        /// <summary>
        /// Event handler that is triggered when a charge state is toggled.
        /// </summary>
        /// <param name="arg">The event arguments.</param>
        private void ChargeStateSelectionChanged(PropertyChangedMessage<bool> arg)
        {
            var chargeStateViewModel = arg.Sender as ChargeStateViewModel;
            if (chargeStateViewModel != null)
            {
                foreach (var series in this.XicPlotModel.Series)
                {
                    var lineSeries = series as LineSeries;
                    if (lineSeries != null && lineSeries.ItemsSource.Any())
                    {
                        var msfeature = lineSeries.ItemsSource.First() as MSFeatureLight;
                        if (msfeature != null && msfeature.ChargeState == chargeStateViewModel.ChargeState)
                        {
                            lineSeries.IsVisible = arg.NewValue;
                        }
                    }
                }

                this.XicPlotModel.InvalidatePlot(true);
            }
        }

        /// <summary>
        /// Event handler that is triggered when a feature is toggled.
        /// </summary>
        /// <param name="arg">Event arguments.</param>
        private void FeatureSelectionChanged(PropertyChangedMessage<bool> arg)
        {
            var umcLightViewModel = arg.Sender as UMCLightViewModel;
            if (umcLightViewModel != null)
            {
                foreach (var series in this.XicPlotModel.Series)
                {
                    var lineSeries = series as LineSeries;
                    if (lineSeries != null && lineSeries.ItemsSource.Any())
                    {
                        var msfeature = lineSeries.ItemsSource.First() as MSFeatureLight;
                        if (msfeature != null && umcLightViewModel.UMCLight.MsFeatures.Contains(msfeature))
                        {
                            lineSeries.IsVisible = arg.NewValue;
                        }
                    }
                }

                this.XicPlotModel.InvalidatePlot(true);
            }
        }

        /// <summary>
        /// Event handler for OxyPlot left mouse click.
        /// </summary>
        /// <param name="sender">The sending PlotView.</param>
        /// <param name="args">The event arguments.</param>
        private void XicPlotMouseDown(object sender, OxyMouseEventArgs args)
        {
            var series = this.XicPlotModel.GetSeriesFromPoint(args.Position, 10);
            if (series != null)
            {
                var result = series.GetNearestPoint(args.Position, false);
                if (result != null)
                {
                    var msfeaturePoint = result.Item as MSFeatureLight;
                    if (msfeaturePoint != null)
                    {
                        this.SelectedMsFeature = msfeaturePoint;
                    }
                }
            }
        }

        /// <summary>
        /// Set the annotation for selected MS feature.
        /// </summary>
        private void SetMsFeatureAnnotation()
        {
            if (this.SelectedMsFeature == null)
            {
                return;
            }

            this.XicPlotModel.Annotations.Clear();
            var scanDataProvider = this.rawProvider.GetScanSummaryProvider(this.SelectedMsFeature.GroupId);
            var elutionTime = scanDataProvider.GetScanSummary(this.SelectedMsFeature.Scan).Time;
            var annotation = new LineAnnotation
            {
                X = elutionTime,
                TextColor = OxyColors.Gray,
                Text = elutionTime.ToString("0.###"),
                TextOrientation = AnnotationTextOrientation.Vertical,
                LineStyle = LineStyle.Dash,
                Type = LineAnnotationType.Vertical,
            };

            this.XicPlotModel.Annotations.Add(annotation);
        }

        public void SavePlot()
        {
            PlotSavingViewModel.SavePlot(this.XicPlotModel, 800, 600, "XIC_View");
        }
    }
}
