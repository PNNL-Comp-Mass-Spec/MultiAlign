using FeatureAlignment.Data.Features;

namespace MultiAlignRogue.Clustering
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;

    using InformedProteomics.Backend.Data.Biology;
    using MultiAlignCore.IO;
    using MultiAlignRogue.Utils;
    using MultiAlignRogue.ViewModels;
    using OxyPlot;
    using OxyPlot.Annotations;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    using InformedProteomics.Backend.Data.Spectrometry;

    using MultiAlignCore.IO.RawData;

    public class XicPlotViewModel : PlotViewModelBase
    {
        /// <summary>
        /// For throttling plot updates.
        /// </summary>
        private readonly Throttler plotBuilderThrottler;

        /// <summary>
        /// For throttling Y-Axis scale adjustments.
        /// </summary>
        private readonly Throttler yaxisScaleThrottler;

        /// <summary>
        /// Provider for LCMSRun for access to PBF files.
        /// </summary>
        private readonly ScanSummaryProviderCache rawProvider;

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

        public RelayCommand SavePlotCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XicPlotViewModel"/> class.
        /// </summary>
        public XicPlotViewModel(ScanSummaryProviderCache rawProvider)
        {
            this.rawProvider = rawProvider;
            this.plotBuilderThrottler = new Throttler(TimeSpan.FromMilliseconds(100));
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
                Title = "NET",
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 1.0,
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
        public PlotModel XicPlotModel { get; }

        /// <summary>
        /// Gets all possible charge states that can be selected.
        /// </summary>
        public ObservableCollection<ChargeStateViewModel> ChargeStates { get; }

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
                    this.plotBuilderThrottler.Run(this.BuildPlot);
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected MS feature.
        /// </summary>
        public MSFeatureLight SelectedMsFeature
        {
            get => this.selectedMsFeature;
            set
            {
                if (this.selectedMsFeature != value)
                {
                    this.selectedMsFeature = value;
                    this.RaisePropertyChanged(nameof(SelectedMsFeature), null, value, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Y axis on this plot should
        /// automatically zoom based on the visible range in the x axis.
        /// </summary>
        public bool AutoScaleYAxis
        {
            get => this.autoScaleYAxis;
            set
            {
                if (this.autoScaleYAxis != value)
                {
                    this.autoScaleYAxis = value;
                    this.RaisePropertyChanged(nameof(AutoScaleYAxis), !value, value, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the legend on the XIC plot
        /// should be visible.
        /// </summary>
        public bool IsLegendVisible
        {
            get => this.XicPlotModel.IsLegendVisible;
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

            var minX = double.PositiveInfinity;
            var maxX = 0.0;
            var minY = double.PositiveInfinity;
            var maxY = 0.0;

            var chargeHash = new HashSet<int>();

            MSFeatureLight maxFeature = null;

            var i = 0;
            foreach (var feature in this.Features)
            {
                var xicList = this.GetXic(feature.UMCLight);
                foreach (var xic in xicList)
                {
                    if (xic.Count == 0)
                    {
                        continue;
                    }

                    if (!chargeHash.Contains(xic[0].ChargeState))
                    {
                        chargeHash.Add(xic[0].ChargeState);
                    }


                    // Get dataset info for mapping scan # -> retention time
                    var dsInfo = SingletonDataProviders.GetDatasetInformation(feature.UMCLight.GroupId);

                    foreach (var msFeature in xic)
                    {
                        minX = Math.Min(minX, msFeature.Net);
                        maxX = Math.Max(maxX, msFeature.Net);
                        minY = Math.Min(minY, msFeature.Abundance);
                        maxY = Math.Max(maxY, msFeature.Abundance);
                    }

                    var maxA = xic.Max(msf => msf.Abundance);
                    var maxL = xic.FirstOrDefault(msf => msf.Abundance.Equals(maxA));
                    if (maxFeature == null || (maxL != null && maxL.Abundance >= maxFeature.Abundance))
                    {
                        maxFeature = maxL;
                    }

                    var color = this.Colors[i++ % this.Colors.Count];
                    var series = new LineSeries
                    {
                        ItemsSource = xic,
                        Mapping = dataPoint => new DataPoint(((MSFeatureLight)dataPoint).Net, ((MSFeatureLight)dataPoint).Abundance),
                        Title = string.Format("{0}({1}+) ID({2})", dsInfo.DatasetName, xic[0].ChargeState, feature.UMCLight.Id),
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
                foreach (var point in series.ItemsSource)
                {
                    if (point is MSFeatureLight msFeature && msFeature.Net >= minX && msFeature.Net <= maxX)
                    {
                        maxY = Math.Max(maxY, msFeature.Abundance);
                        minY = Math.Min(minY, msFeature.Abundance);
                    }
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
            if (!(arg.Sender is ChargeStateViewModel chargeStateViewModel))
                return;

            foreach (var series in this.XicPlotModel.Series)
            {
                if (!(series is LineSeries lineSeries) || !lineSeries.ItemsSource.OfType<MSFeatureLight>().Any())
                    continue;

                var msFeature = lineSeries.ItemsSource.OfType<MSFeatureLight>().First();
                if (msFeature != null && msFeature.ChargeState == chargeStateViewModel.ChargeState)
                {
                    lineSeries.IsVisible = arg.NewValue;
                }
            }

            this.XicPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Event handler that is triggered when a feature is toggled.
        /// </summary>
        /// <param name="arg">Event arguments.</param>
        private void FeatureSelectionChanged(PropertyChangedMessage<bool> arg)
        {
            if (!(arg.Sender is UMCLightViewModel umcLightViewModel))
                return;

            foreach (var series in this.XicPlotModel.Series)
            {
                if (!(series is LineSeries lineSeries) || !lineSeries.ItemsSource.OfType<MSFeatureLight>().Any())
                    continue;

                var msFeature = lineSeries.ItemsSource.OfType<MSFeatureLight>().First();
                if (msFeature != null && msFeature.GroupId == umcLightViewModel.UMCLight.GroupId)
                {
                    lineSeries.IsVisible = arg.NewValue;
                }
            }

            this.XicPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Event handler for OxyPlot left mouse click.
        /// </summary>
        /// <param name="sender">The sending PlotView.</param>
        /// <param name="args">The event arguments.</param>
        private void XicPlotMouseDown(object sender, OxyMouseDownEventArgs args)
        {
            if (args.ChangedButton != OxyMouseButton.Left)
            {
                return;
            }

            var series = this.XicPlotModel.GetSeriesFromPoint(args.Position, 10);
            var result = series?.GetNearestPoint(args.Position, false);
            if (result?.Item is MSFeatureLight msFeaturePoint)
            {
                this.SelectedMsFeature = msFeaturePoint;
            }
        }

        /// <summary>
        /// Gets an XIC using the <see cref="InformedProteomicsReader" />.
        /// </summary>
        /// <param name="feature">The feature to get the XIC for.</param>
        /// <returns>The XIC.</returns>
        private IEnumerable<List<MSFeatureLight>> GetXic(UMCLight feature)
        {
            var xicList = new List<List<MSFeatureLight>>();
            if (!(this.rawProvider.GetScanSummaryProvider(feature.GroupId) is InformedProteomicsReader ipr))
                return xicList;

            var lcms = ipr.LcMsRun;
            for (var charge = feature.MinCharge; charge <= feature.MaxCharge; charge++)
            {
                var mz = (feature.MassMonoisotopicAligned + charge * Constants.Proton) / charge;
                var xic = lcms.GetFullPrecursorIonExtractedIonChromatogram(mz, new Tolerance(10, ToleranceUnit.Ppm))
                    .Where(xicP => xicP.ScanNum >= feature.ScanStart && xicP.ScanNum <= feature.ScanEnd)
                    .ToList();
                var msFeatures = xic.Select(
                    xicPoint =>
                        new MSFeatureLight
                        {
                            GroupId = feature.GroupId,
                            Scan = xicPoint.ScanNum,
                            Net = ipr.GetScanSummary(xicPoint.ScanNum).Net,
                            Abundance = xicPoint.Intensity,
                            ChargeState = charge,
                        }).ToList();
                xicList.Add(msFeatures);
            }

            return xicList;
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
            var annotation = new LineAnnotation
            {
                X = this.SelectedMsFeature.Net,
                TextColor = OxyColors.Gray,
                Text = this.SelectedMsFeature.Net.ToString("0.###"),
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
