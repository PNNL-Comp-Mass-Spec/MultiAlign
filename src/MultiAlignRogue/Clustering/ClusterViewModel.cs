using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MultiAlign.ViewModels.Charting;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Features;
using MultiAlignRogue.Utils;
using MultiAlignRogue.ViewModels;

namespace MultiAlignRogue.Clustering
{
    using System.Text.RegularExpressions;

    using MultiAlignCore.IO.RawData;

    /// <summary>
    /// The view model for the ClusterView.
    /// </summary>
    public class ClusterViewModel : ViewModelBase
    {
        /// <summary>
        /// Data access providers for reconstructing clusters.
        /// </summary>
        private readonly FeatureDataAccessProviders providers;

        /// <summary>
        /// Lock for database access.
        /// </summary>
        private readonly object dbLock;

        /// <summary>
        /// The throttler.
        /// </summary>
        private readonly Throttler throttler;

        /// <summary>
        /// Factory for creating child windows.
        /// </summary>
        private readonly IClusterViewFactory viewFactory;

        /// <summary>
        /// Provider for LCMSRun for access to PBF files.
        /// </summary>
        private readonly InformedProteomicsReader rawProvider;

        /// <summary>
        /// The selected cluster.
        /// </summary>
        private ClusterMatch selectedMatch;

        /// <summary>
        /// The selected feature.
        /// </summary>
        private IEnumerable<UMCLightViewModel> selectedFeatures;

        /// <summary>
        /// Path to layout file.
        /// </summary>
        private string layoutFilePath;

        /// <summary>
        /// The selected MS/MS spectrum.
        /// </summary>
        private MSSpectra selectedMsMsSpectra;

        /// <summary>
        /// The MsMsSpectraViewModel.
        /// </summary>
        private MsMsSpectraViewModel msMsSpectraViewModel;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterViewModel"/> class.
        /// </summary>
        /// <param name="viewFactory">Factory for creating child windows.</param>
        /// <param name="matches">The clusters.</param>
        /// <param name="layoutFilePath">Path to layout file.</param>
        /// <param name="rawProvider">Provider for LCMSRun for access to PBF files.</param>
        public ClusterViewModel(IClusterViewFactory viewFactory, List<ClusterMatch> matches, FeatureDataAccessProviders providers, string layoutFilePath, InformedProteomicsReader rawProvider)
        {
            this.viewFactory = viewFactory;
            this.providers = providers;
            this.dbLock = new object();
            this.throttler = new Throttler(TimeSpan.FromMilliseconds(500));
            this.XicPlotViewModel = new XicPlotViewModel(rawProvider);
            this.ClusterFeaturePlotViewModel = new ClusterFeaturePlotViewModel();
            this.Matches = new ObservableCollection<ClusterMatch>(matches ?? new List<ClusterMatch>());
            var clusters = this.Matches.Select(match => match.Cluster).ToList();
            this.rawProvider = rawProvider;

            this.SettingsCommand = new RelayCommand(() => this.viewFactory.CreateSettingsWindow(this.ClusterPlotViewModel.ClusterViewerSettings));

            this.Features = new ObservableCollection<UMCLightViewModel>();
            this.MsMsSpectra = new ObservableCollection<MSSpectra>();
            this.LayoutFilePath = layoutFilePath;

            this.MsMsSpectraViewModel = new MsMsSpectraViewModel(new MSSpectra(), "MsMs Spectrum");
            this.ClusterPlotViewModel = new ClusterPlotViewModel(clusters);

            this.ShowChargeStateDistributionCommand = new GalaSoft.MvvmLight.Command.RelayCommand(this.ShowChargeStateDistributionImpl);
            this.ShowDatasetHistogramCommand = new RelayCommand(this.ShowDatasetHistogramImpl);

            // Listen for changes in selected cluster in ClusterViewModel.
            Messenger.Default.Register<PropertyChangedMessage<UMCClusterLight>>(
                this,
                args =>
            {
                if (args.Sender == this.ClusterPlotViewModel && !this.ClusterPlotViewModel.SelectedCluster.Equals(this.SelectedMatch))
                {
                    this.SelectedMatch = this.Matches.FirstOrDefault(match => match.Cluster == this.ClusterPlotViewModel.SelectedCluster);
                } 
            });

            // Listen for changes in selected cluster internally.
            Messenger.Default.Register<PropertyChangedMessage<ClusterMatch>>(
                this,
                arg =>
            {
                if (arg.Sender == this)
                {
                    this.throttler.Run(() => this.MatchSelected(arg));
                }
            });

            Messenger.Default.Register<PropertyChangedMessage<MSFeatureLight>>(
                this,
                arg =>
            {
                if (arg.Sender == this.XicPlotViewModel && arg.NewValue != null)
                {
                    this.MsMsSpectra.Clear();
                    foreach (var msmsSpectrum in arg.NewValue.MSnSpectra)
                    {
                        this.MsMsSpectra.Add(msmsSpectrum);
                    }

                    var first = this.MsMsSpectra.FirstOrDefault();
                    if (first != null)
                    {
                        this.SelectedMsMsSpectra = first;
                    } 
                }
            });

            if (this.Matches.Count > 0)
            {
                this.SelectedMatch = this.Matches[0];
            }
        }

        /// <summary>
        /// Gets a command that displays a charge state distribution plot.
        /// </summary>
        public ICommand ShowChargeStateDistributionCommand { get; private set; }

        /// <summary>
        /// Gets a command that displays the dataset cluster histogram plot.
        /// </summary>
        public ICommand ShowDatasetHistogramCommand { get; private set; }

        /// <summary>
        /// Gets a command that shows the settings window.
        /// </summary>
        public ICommand SettingsCommand { get; private set; }

        /// <summary>
        /// Gets the list of clusters.
        /// </summary>
        public ObservableCollection<ClusterMatch> Matches { get; private set; }

        /// <summary>
        /// Gets the average abundance for features in a cluster.
        /// </summary>
        public Dictionary<UMCClusterLight, double> ClusterAbundance { get; private set; }

        /// <summary>
        /// Gets the list of features for the selected cluster.
        /// </summary>
        public ObservableCollection<UMCLightViewModel> Features { get; private set; }

        /// <summary>
        /// Gets the list of MsMs spectra.
        /// </summary>
        public ObservableCollection<MSSpectra> MsMsSpectra { get; private set; }

        /// <summary>
        /// Gets or sets the selected MS/MS spectrum.
        /// </summary>
        public MSSpectra SelectedMsMsSpectra
        {
            get { return this.selectedMsMsSpectra; }
            set
            {
                if (this.selectedMsMsSpectra != value)
                {
                    this.selectedMsMsSpectra = value;
                    if (this.selectedMsMsSpectra != null)
                    {
                        this.MsMsSpectraViewModel = new MsMsSpectraViewModel(this.selectedMsMsSpectra, "MS/MS Spectrum");
                    }

                    this.RaisePropertyChanged("SelectedMsMsSpectra", null, value, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the MsMsSpectraViewModel.
        /// </summary>
        public MsMsSpectraViewModel MsMsSpectraViewModel
        {
            get { return this.msMsSpectraViewModel; }
            private set
            {
                if (this.msMsSpectraViewModel != value)
                {
                    this.msMsSpectraViewModel = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the view model for extracted ion chromatogram plots.
        /// </summary>
        public XicPlotViewModel XicPlotViewModel { get; private set; }

        /// <summary>
        /// Gets the view model for the cluster plot.
        /// </summary>
        public ClusterPlotViewModel ClusterPlotViewModel { get; private set; }

        /// <summary>
        /// Gets the view model for displaying cluster features.
        /// </summary>
        public ClusterFeaturePlotViewModel ClusterFeaturePlotViewModel { get; private set; }

        /// <summary>
        /// Gets the path to the layout file.
        /// </summary>
        public string LayoutFilePath
        {
            get { return this.layoutFilePath; }
            private set
            {
                if (this.layoutFilePath != value)
                {
                    this.layoutFilePath = value;
                    this.RaisePropertyChanged("LayoutFilePath", string.Empty, this.layoutFilePath, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected cluster.
        /// </summary>
        public ClusterMatch SelectedMatch
        {
            get { return this.selectedMatch; }
            set
            {
                if (this.selectedMatch != value)
                {
                    this.selectedMatch = value;
                    this.RaisePropertyChanged("SelectedMatch", null, value, true);
                }
            }
        }

        /// <summary>
        /// Event handler for SelectedCluster changed.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        private async void MatchSelected(PropertyChangedMessage<ClusterMatch> args)
        {
            var match = args.NewValue;
            var cluster = match.Cluster;

            this.Features.Clear();

            // Set feature list for this cluster.
            if (this.selectedMatch != null)
            {
                this.ClusterPlotViewModel.SelectedCluster = cluster;
                await Task.Run(() =>
                {
                    lock (this.dbLock)
                    {
                        cluster.ReconstructUMCCluster(this.providers, true, false, true, true);
                    }
                });
                cluster.UmcList.ForEach(c => this.Features.Add(new UMCLightViewModel(c)));
                this.XicPlotViewModel.Features = new List<UMCLightViewModel>(this.Features);
                this.ClusterFeaturePlotViewModel.Features = new List<UMCLightViewModel>(this.Features);
            }
        }

        /// <summary>
        /// Gets a command that displays a charge state distribution plot.
        /// </summary>
        private void ShowChargeStateDistributionImpl()
        {
            this.viewFactory.CreateChargeStateDistributionWindow(this.Matches.Select(match => match.Cluster), "Charge State Distribution");
        }

        /// <summary>
        /// Gets a command that displays a dataset distribution plot.
        /// </summary>
        private void ShowDatasetHistogramImpl()
        {
            this.viewFactory.CreateDatasetHistogramWindow(this.Matches.Select(match => match.Cluster), "Dataset Distribution");
        }
    }
}
