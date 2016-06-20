namespace MultiAlignCore.Algorithms.Clustering
{
    using System;
    using System.Collections.Generic;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Algorithms.Distance;
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO.Features;
    using MultiAlignCore.IO.RawData;

    /// <summary>
    /// This class clusters features from several datasets together.
    /// </summary>
    public sealed class CrossDatasetClusteringRunner : ISettingsContainer
    {
        /// <summary>
        /// Initializes a new instance of the CrossDatasetClusteringRunner class.
        /// </summary>
        /// <param name="clusteringOptions">The options to use for clustering.</param>
        public CrossDatasetClusteringRunner(LcmsClusteringOptions clusteringOptions = null)
        {
            this.ClusteringOptions = clusteringOptions ?? new LcmsClusteringOptions();
        }

        /// <summary>
        /// Gets the clustering options.
        /// </summary>
        public LcmsClusteringOptions ClusteringOptions { get; private set; }

        /// <summary>
        /// Cluster features of multiple datasets together.
        /// This overload will extract features from the database for the datasets provided, and then
        /// persist the clusters to the database when finished.
        /// </summary>
        /// <param name="datasets">The datasets to cluster together.</param>
        /// <param name="featureDao">The data access provider to get features for each dataset.</param>
        /// <param name="clusterDao">The data access provider for persisting clusters.</param>
        /// <param name="scanSummaryProviderCache">
        /// Provider for scan summaries for clusters that require raw data.
        /// </param>
        /// <param name="progress">The progress reporter.</param>
        public void RunClustering(
            List<DatasetInformation> datasets,
            IUmcDAO featureDao,
            IUmcClusterDAO clusterDao,
            ScanSummaryProviderCache scanSummaryProviderCache = null,
            IProgress<ProgressData> progress = null)
        {
            // Set up progress reporting
            progress = progress ?? new Progress<ProgressData>();
            var subProgressData = new ProgressData { IsPartialRange = true, MaxPercentage = 33 };
            IProgress<ProgressData> subProgress = new Progress<ProgressData>(pd => progress.Report(subProgressData.UpdatePercent(pd.Percent)));

            // Get features for each dataset from database.
            var featureExtractionProgressData = new ProgressData();
            var features = new List<UMCLight>();
            for (int i = 0; i < datasets.Count; i++)
            {
                var dsFeatures = featureDao.FindByDatasetId(datasets[i].DatasetId);
                features.AddRange(dsFeatures);
                subProgress.Report(featureExtractionProgressData.UpdatePercent(100.0 * i / datasets.Count));
            }

            // Run clustering
            subProgressData.StepRange(66);
            var clusters = this.RunClustering(features, scanSummaryProviderCache, subProgress);

            // Persist features
            subProgressData.StepRange(82.5);
            // featureDao.(features);

            // Persist
            subProgressData.StepRange(100);
            clusterDao.AddAll(clusters, subProgress);
        }

        /// <summary>
        /// Cluster features of multiple datasets together.
        /// </summary>
        /// <param name="features">The features from multiple datasets to cluster together.</param>
        /// <param name="scanSummaryProviderCache">
        /// Provider for scan summaries for clusters that require raw data.
        /// </param>
        /// <param name="progress">The progress reporter.</param>
        /// <returns>List of resulting clusters.</returns>
        public List<UMCClusterLight> RunClustering(
                                                   List<UMCLight> features,
                                                   ScanSummaryProviderCache scanSummaryProviderCache = null,
                                                   IProgress<ProgressData> progress = null)
        {
            // Set up clusterer
            var clusterer =
                ClusterFactory.CreateLcmsFeatureClusterer(this.ClusteringOptions.LcmsFeatureClusteringAlgorithm);
            clusterer.Parameters = new FeatureClusterParameters<UMCLight>
            {
                CentroidRepresentation = ClusterCentroidRepresentation.Apex,
                Tolerances = this.ClusteringOptions.InstrumentTolerances,
                DistanceFunction = DistanceFactory<UMCLight>.CreateDistanceFunction(DistanceMetric.Euclidean)
            };

            // Promex clusterer requires raw data
            var promexClusterer = clusterer as PromexClusterer;
            if (promexClusterer != null && scanSummaryProviderCache != null)
            {
                promexClusterer.Readers = scanSummaryProviderCache;
            }

            return clusterer.Cluster(features, progress);
        }

        /// <summary>
        /// Restore settings back to their default values.
        /// </summary>
        public void RestoreDefaults()
        {
            this.ClusteringOptions.RestoreDefaults();
        }
    }
}
