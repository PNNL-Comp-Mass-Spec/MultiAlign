namespace MultiAlignRogue.Clustering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing;
    using MultiAlignCore.IO.Features;
    using MultiAlignCore.IO.RawData;

    using MultiAlignRogue.Utils;

    public class ClusterSettingsTabViewModel : SettingsEditorViewModelBase
    {
        /// <summary>
        /// The data access providers for pulling/persisting features from the database.
        /// </summary>
        private IUmcDAO featureDataAccessProviders;

        /// <summary>
        /// The data access providers for pulling/persisting clusters from the database.
        /// </summary>
        private IUmcClusterDAO clusterDataAccessProviders;

        /// <summary>
        /// Scan summary provider cache that encapulates loading and persisting raw data.
        /// </summary>
        private ScanSummaryProviderCache scanSummaryProviderCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterSettingsTabViewModel" /> class. 
        /// </summary>
        /// <param name="clusterTabSettings"></param>
        /// <param name="featureDataAccessProvider">
        /// The data access providers for pulling/persisting features from the database.
        /// </param>
        /// <param name="clusterDataAccessProvider">
        /// The data access providers for pulling/persisting clusters from the database.
        /// </param>
        /// <param name="scanSummaryProviderCache">
        /// Scan summary provider cache that encapulates loading and persisting raw data.
        /// </param>
        public ClusterSettingsTabViewModel(LcmsClusteringOptions clusterTabSettings,
                                           IUmcDAO featureDataAccessProvider,
                                           IUmcClusterDAO clusterDataAccessProvider,
                                           ScanSummaryProviderCache scanSummaryProviderCache)
            : base(clusterTabSettings)
        {
            this.ClusterAlgorithmSettingsViewModel = new ClusterAlgorithmSettingsViewModel(clusterTabSettings);
            this.featureDataAccessProviders = featureDataAccessProvider;
            this.clusterDataAccessProviders = clusterDataAccessProvider;
            this.scanSummaryProviderCache = scanSummaryProviderCache;
        }

        /// <summary>
        /// Gets the view model for editing the clustering settings.
        /// </summary>
        public ClusterAlgorithmSettingsViewModel ClusterAlgorithmSettingsViewModel { get; private set; }



        /// <summary>
        /// Gets a commmand for running the clustering and post processing.
        /// </summary>
        public ICommand RunCommand { get; private set; }
    }
}
