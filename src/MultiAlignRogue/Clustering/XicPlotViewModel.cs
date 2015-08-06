using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MultiAlignCore.IO;
using MultiAlignRogue.Utils;
using MultiAlignRogue.ViewModels;
using NHibernate.Util;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PNNLOmics.Data.Features;
using PNNLOmics.Extensions;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace MultiAlignRogue.Clustering
{
    public class XicPlotViewModel : PlotViewModelBase
    {
        /// <summary>
        /// For throttling plot updates.
        /// </summary>
        private readonly Throttler throttler;

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
        /// Initializes a new instance of the <see cref="XicPlotViewModel"/> class.
        /// </summary>
        public XicPlotViewModel()
        {
            this.throttler = new Throttler(TimeSpan.FromMilliseconds(100));
            this.ChargeStates = new ObservableCollection<ChargeStateViewModel>();
            this.XicPlotModel = new PlotModel
            {
                Title = "Extracted Ion Chromatograms",
                ////RenderingDecorator = rc => new XkcdRenderingDecorator(rc)
            };

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
            };

            this.XicPlotModel.Axes.Add(this.xaxis);
            this.XicPlotModel.Axes.Add(this.yaxis);

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, this.ChargeStateSelectionChanged);
            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, this.FeatureSelectionChanged);
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
                    this.throttler.Run(this.BuildPlot);
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

            int i = 0;
            foreach (var feature in this.Features)
            {
                var chargeMap = feature.UMCLight.CreateChargeMap();
                chargeHash.UnionWith(chargeMap.Keys);

                foreach (var charge in chargeMap.Keys)
                {
                    var msfeatures = chargeMap[charge];

                    // Get dataset info for mapping scan # -> retention time
                    var dsinfo = SingletonDataProviders.GetDatasetInformation(feature.UMCLight.GroupId);
                    if (dsinfo == null)
                    {
                        continue;
                    }

                    minX = Math.Min(feature.UMCLight.MsFeatures.Select(msfeature => dsinfo.ScanTimes[msfeature.Scan]).Min(), minX);
                    maxX = Math.Max(feature.UMCLight.MsFeatures.Select(msfeature => dsinfo.ScanTimes[msfeature.Scan]).Max(), maxX);
                    minY = Math.Min(feature.UMCLight.MsFeatures.Min(msfeature => msfeature.Abundance), minY);
                    maxY = Math.Max(feature.UMCLight.MsFeatures.Max(msfeature => msfeature.Abundance), maxY);

                    var color = this.Colors[i++ % this.Colors.Count];
                    var series = new LineSeries
                    {
                        Title = string.Format("{0}({1}+) ID({2})", dsinfo.DatasetName, charge, feature.UMCLight.Id),
                        ItemsSource = msfeatures,
                        Mapping = o => new DataPoint(
                                                     dsinfo.ScanTimes[((MSFeatureLight)o).Scan],
                                                     ((MSFeatureLight)o).Abundance),
                        Color = color,
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 3,
                        MarkerFill = OxyColors.White,
                        MarkerStroke = color,
                        MarkerStrokeThickness = 0.5
                    };

                    this.XicPlotModel.Series.Add(series);   
                }
            }

            this.ChargeStates.Clear();
            foreach (var chargeState in chargeHash)
            {
                this.ChargeStates.Add(new ChargeStateViewModel(chargeState));
            }

            maxY = maxY + (0.05 * maxY);

            this.xaxis.Minimum = minX;
            this.xaxis.Maximum = maxX;
            this.yaxis.Minimum = minY;
            this.yaxis.Maximum = maxY;

            this.xaxis.Zoom(minX, maxX);
            this.yaxis.Zoom(minY, maxY);

            this.XicPlotModel.InvalidatePlot(true);
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
    }
}
