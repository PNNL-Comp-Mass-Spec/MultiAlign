namespace MultiAlignRogue.Clustering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using GalaSoft.MvvmLight.Command;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing;
    using MultiAlignCore.IO.Features;
    using MultiAlignCore.IO.RawData;

    using MultiAlignRogue.Utils;
    using MultiAlignRogue.ViewModels;

    public class ClusterSettingsTabViewModel : SettingsEditorViewModelBase
    {
        /// <summary>
        /// The stored clustering settings.
        /// </summary>
        private readonly CrossDatasetClusteringRunner crossDatasetClusteringRunner;

        /// <summary>
        /// The data access providers for pulling/persisting features from the database.
        /// </summary>
        private readonly IUmcDAO featureDataAccessProviders;

        /// <summary>
        /// The datasets to run clustering on
        /// </summary>
        private readonly List<DatasetInformationViewModel> datasets;

        /// <summary>
        /// The data access providers for pulling/persisting clusters from the database.
        /// </summary>
        private readonly IUmcClusterDAO clusterDataAccessProviders;

        /// <summary>
        /// Scan summary provider cache that encapulates loading and persisting raw data.
        /// </summary>
        private ScanSummaryProviderCache scanSummaryProviderCache;

        /// <summary>
        /// The total progress for the clustering task.
        /// </summary>
        private double progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterSettingsTabViewModel" /> class. 
        /// </summary>
        /// <param name="crossDatasetClusteringRunner">The stored clustering settings.</param>
        /// <param name="datasets">The datasets to run clustering on.</param>
        /// <param name="featureDataAccessProvider">
        /// The data access providers for pulling/persisting features from the database.
        /// </param>
        /// <param name="clusterDataAccessProvider">
        /// The data access providers for pulling/persisting clusters from the database.
        /// </param>
        /// <param name="scanSummaryProviderCache">
        /// Scan summary provider cache that encapulates loading and persisting raw data.
        /// </param>
        public ClusterSettingsTabViewModel(CrossDatasetClusteringRunner crossDatasetClusteringRunner,
                                           List<DatasetInformationViewModel> datasets,
                                           IUmcDAO featureDataAccessProvider,
                                           IUmcClusterDAO clusterDataAccessProvider,
                                           ScanSummaryProviderCache scanSummaryProviderCache)
            : base(crossDatasetClusteringRunner)
        {
            this.crossDatasetClusteringRunner = crossDatasetClusteringRunner;
            this.ClusterAlgorithmSettingsViewModel = new ClusterAlgorithmSettingsViewModel(crossDatasetClusteringRunner.ClusteringOptions);
            this.datasets = datasets;
            this.featureDataAccessProviders = featureDataAccessProvider;
            this.clusterDataAccessProviders = clusterDataAccessProvider;
            this.scanSummaryProviderCache = scanSummaryProviderCache;

            this.RunCommand = new RelayCommand(async () => await this.Run());
        }

        /// <summary>
        /// Gets or sets the total progress for the clustering task.
        /// </summary>
        public double Progress
        {
            get { return this.progress; }
            set
            {
                if (!this.progress.Equals(value))
                {
                    this.progress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the view model for editing the clustering settings.
        /// </summary>
        public ClusterAlgorithmSettingsViewModel ClusterAlgorithmSettingsViewModel { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public async Task Run()
        {
            var progressReporter = new Progress<ProgressData>(pd => this.Progress = pd.Percent);
            var runnableDatasets = this.datasets.Where(dsVm => dsVm.FeaturesFound)
                                                .Select(dsVm => dsVm.Dataset)
                                                .ToList();

            await Task.Run(
                    () => this.crossDatasetClusteringRunner.RunClustering(
                               runnableDatasets,
                               this.featureDataAccessProviders,
                               this.clusterDataAccessProviders,
                               this.scanSummaryProviderCache,
                               progressReporter));
        }

        /// <summary>
        /// Gets a commmand for running the clustering and post processing.
        /// </summary>
        public ICommand RunCommand { get; private set; }
    }
}
