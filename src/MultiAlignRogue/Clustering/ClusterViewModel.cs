using FeatureAlignment.Data;
using FeatureAlignment.Data.Features;

namespace MultiAlignRogue.Clustering
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using MultiAlignCore.Extensions;
    using MultiAlignCore.IO.Features;
    using MultiAlignRogue.Utils;
    using MultiAlignRogue.ViewModels;

    using System.IO;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Xml;
    using System.Xml.Serialization;

    using MultiAlignCore.IO.RawData;

    using Xceed.Wpf.AvalonDock.Layout;

    /// <summary>
    /// The view model for the ClusterView.
    /// </summary>
    public class ClusterViewModel : ViewModelBase
    {
        /// <summary>
        /// The name of the default layout file to use.
        /// </summary>
        private const string StandardLayoutFileName = "StandardLayout.xml";

        /// <summary>
        /// The path of the default layout file to use.
        /// </summary>
        private readonly string standardLayoutFilePath;

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
        private readonly ScanSummaryProviderCache rawProvider;

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
        /// The LayoutRoot for layout serialization.
        /// </summary>
        private LayoutRoot layoutRoot;

        /// <summary>
        /// This stores the original layout for comparison to determine if it has changed.
        /// </summary>
        private bool layoutUpdated;

        /// <summary>
        /// This stores the original settings for comparison to determine if they have changed.
        /// </summary>
        private ClusterViewerSettings originalSettings;

        /// <summary>
        /// The selected MS/MS spectrum.
        /// </summary>
        private MSSpectra selectedMsMsSpectra;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterViewModel"/> class.
        /// </summary>
        /// <param name="viewFactory">Factory for creating child windows.</param>
        /// <param name="matches">The clusters.</param>
        /// <param name="providers">Database access provider</param>
        /// <param name="rawProvider">Provider for LCMSRun for access to PBF files.</param>
        public ClusterViewModel(IClusterViewFactory viewFactory, List<ClusterMatch> matches, FeatureDataAccessProviders providers, ScanSummaryProviderCache rawProvider)
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

            this.ClusterPlotViewModel = new ClusterPlotViewModel(clusters);

            this.ShowChargeStateDistributionCommand = new RelayCommand(this.ShowChargeStateDistributionImpl);
            this.ShowDatasetHistogramCommand = new RelayCommand(this.ShowDatasetHistogramImpl);

            this.layoutUpdated = false;
            this.originalSettings = new ClusterViewerSettings();

            // Set up standard layout path
            var assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                this.standardLayoutFilePath = Path.Combine(assemblyPath, StandardLayoutFileName);
            }

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

            // When the selected MSFeature changes, update MS/MS spectra
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

            // Load layout.
            this.layoutFilePath = "layout.xml";
            this.LoadLayoutFile();

            if (this.Matches.Count > 0)
            {
                this.SelectedMatch = this.Matches[0];
            }
        }

        /// <summary>
        /// Gets a command that displays a charge state distribution plot.
        /// </summary>
        public ICommand ShowChargeStateDistributionCommand { get; }

        /// <summary>
        /// Gets a command that displays the dataset cluster histogram plot.
        /// </summary>
        public ICommand ShowDatasetHistogramCommand { get; }

        /// <summary>
        /// Gets a command that shows the settings window.
        /// </summary>
        public ICommand SettingsCommand { get; }

        /// <summary>
        /// Gets the list of clusters.
        /// </summary>
        public ObservableCollection<ClusterMatch> Matches { get; }

        /// <summary>
        /// Gets the average abundance for features in a cluster.
        /// </summary>
        public Dictionary<UMCClusterLight, double> ClusterAbundance { get; private set; }

        /// <summary>
        /// Gets the list of features for the selected cluster.
        /// </summary>
        public ObservableCollection<UMCLightViewModel> Features { get; }

        /// <summary>
        /// Gets the list of MsMs spectra.
        /// </summary>
        public ObservableCollection<MSSpectra> MsMsSpectra { get; }

        /// <summary>
        /// Gets or sets the selected MS/MS spectrum.
        /// </summary>
        public MSSpectra SelectedMsMsSpectra
        {
            get => this.selectedMsMsSpectra;
            set
            {
                if (this.selectedMsMsSpectra != value)
                {
                    this.selectedMsMsSpectra = value;

                    this.RaisePropertyChanged(nameof(SelectedMsMsSpectra), null, value, true);
                }
            }
        }

        /// <summary>
        /// Gets the view model for extracted ion chromatogram plots.
        /// </summary>
        public XicPlotViewModel XicPlotViewModel { get; }

        /// <summary>
        /// Gets the view model for the cluster plot.
        /// </summary>
        public ClusterPlotViewModel ClusterPlotViewModel { get; }

        /// <summary>
        /// Gets the view model for displaying cluster features.
        /// </summary>
        public ClusterFeaturePlotViewModel ClusterFeaturePlotViewModel { get; }

        /// <summary>
        /// Gets the path to the layout file.
        /// </summary>
        public string LayoutFilePath
        {
            get => this.layoutFilePath;
            private set
            {
                if (this.layoutFilePath != value)
                {
                    this.layoutFilePath = value;
                    this.RaisePropertyChanged(nameof(LayoutFilePath), string.Empty, this.layoutFilePath, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the LayoutRoot for layout serialization.
        /// </summary>
        public LayoutRoot LayoutRoot
        {
            get => this.layoutRoot;
            set
            {
                if (this.layoutRoot != value)
                {
                    this.layoutRoot = value;
                    this.RaisePropertyChanged(nameof(LayoutRoot), null, value, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected cluster.
        /// </summary>
        public ClusterMatch SelectedMatch
        {
            get => this.selectedMatch;
            set
            {
                if (this.selectedMatch != value)
                {
                    this.selectedMatch = value;
                    this.RaisePropertyChanged(nameof(SelectedMatch), null, value, true);
                }
            }
        }

        /// <summary>
        /// Saves the layout to the file.
        /// </summary>
        public void SaveLayoutFile()
        {
            if (!this.layoutUpdated &&
                this.originalSettings.Equals(this.ClusterPlotViewModel.ClusterViewerSettings))
            {
                return;
            }

            var viewSettingsSerializer = new XmlSerializer(typeof(ViewSettings));
            var viewSettings = new ViewSettings
            {
                ClusterViewLayoutRoot = this.LayoutRoot,
                ClusterViewerSettings = this.ClusterPlotViewModel.ClusterViewerSettings
            };

            var xmlSettings = new XmlWriterSettings { Indent = true, CloseOutput = true };

            using (var writer = XmlWriter.Create(File.Open(this.layoutFilePath, FileMode.Create), xmlSettings))
            {
                viewSettingsSerializer.Serialize(writer, viewSettings);
            }
        }

        /// <summary>
        /// Loads the layout from file.
        /// </summary>
        public void LoadLayoutFile()
        {
            var filePath = (!string.IsNullOrEmpty(this.layoutFilePath) && File.Exists(this.layoutFilePath))
                               ? this.layoutFilePath
                               : this.standardLayoutFilePath;

            var viewSettingsSerializer = new XmlSerializer(typeof(ViewSettings));

            using (var reader = File.Open(filePath, FileMode.Open))
            {
                try
                {
                    var viewSettings = (ViewSettings)viewSettingsSerializer.Deserialize(reader);
                    this.LayoutRoot = viewSettings.ClusterViewLayoutRoot;
                    this.LayoutRoot.PropertyChanged += (o, e) => this.layoutUpdated = true;
                    this.ClusterPlotViewModel.ClusterViewerSettings = viewSettings.ClusterViewerSettings;
                    this.originalSettings = viewSettings.ClusterViewerSettings;

                }
                catch (InvalidCastException)
                {
                    MessageBox.Show("Could not deserialize layout settings.");
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
