using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MultiAlignCore.IO;
using MultiAlignRogue.Utils;
using OxyPlot;
using OxyPlot.Series;
using PNNLOmics.Data.Features;
using Remotion.Linq.Collections;

namespace MultiAlignRogue.Clustering
{
    public class XicPlotViewModel : PlotViewModelBase
    {
        /// <summary>
        /// For throttling plot updates.
        /// </summary>
        private readonly Throttler throttler;

        /// <summary>
        /// The features to display chromatograms for.
        /// </summary>
        private IEnumerable<UMCLight> features;

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
            this.XicPlotModel = new PlotModel();
            this.ChargeStates = new ObservableCollection<int>();
        }

        /// <summary>
        /// Gets the model for the extracted ion chromatogram plot.
        /// </summary>
        public PlotModel XicPlotModel { get; private set; }

        /// <summary>
        /// Gets all possible charge states that can be selected.
        /// </summary>
        public ObservableCollection<int> ChargeStates { get; private set; }

        /// <summary>
        /// Gets or sets the selected charge states.
        /// </summary>
        public IEnumerable<int> SelectedChargeStates
        {
            get { return this.selectedChargeStates; }
            set
            {
                if (this.selectedChargeStates != value)
                {
                    this.selectedChargeStates = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the features to display chromatograms for.
        /// </summary>
        public IEnumerable<UMCLight> Features
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
            int i = 0;
            foreach (var feature in this.Features)
            {
                // Get dataset info for mapping scan # -> retention time
                var dsinfo = SingletonDataProviders.GetDatasetInformation(feature.GroupId);
                var series = new LineSeries
                {
                    ItemsSource = feature.MsFeatures,
                    Mapping = o => new DataPoint(
                                                 dsinfo.ScanTimes[((MSFeatureLight)o).Scan],
                                                 ((MSFeatureLight)o).Abundance),
                    Color = this.Colors[i++ % this.Colors.Count]
                };

                this.XicPlotModel.Series.Add(series);
                this.XicPlotModel.InvalidatePlot(true);
            }
        }
    }
}
