namespace MultiAlignCore.IO.DatasetLoaders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Algorithms;
    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.Algorithms.Distance;
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO.Features;
    using MultiAlignCore.IO.RawData;
    using MultiAlignCore.IO.TextFiles;

    /// <summary>
    /// Loads and filters DeconTools datasets.
    /// </summary>
    public class DeconToolsLoader : IDatasetLoader, ISettingsContainer
    {
        /// <summary>
        /// Scan summary provider cache that encapulates loading and persisting raw data
        /// </summary>
        private readonly ScanSummaryProviderCache scanSummaryProviderCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromexFilter" /> class. 
        /// </summary>
        /// <param name="filter">Gets or sets the elution time range to limit features in.</param>
        /// <param name="scanSummaryProviderCache">Scan summary provider cache that encapulates loading and persisting raw data.</param>
        public DeconToolsLoader(ScanSummaryProviderCache scanSummaryProviderCache, DeconToolsFilter filter = null)
        {
            this.scanSummaryProviderCache = scanSummaryProviderCache;
            this.Filter = filter ?? new DeconToolsFilter();
            this.RestoreDefaults();
        }

        /// <summary>
        /// Gets or sets the type of clustering algorithm to use create LCMS features from DeconTools results.
        /// </summary>
        public LcmsClusteringOptions ClusteringOptions { get; set; }

        /// <summary>
        /// Gets or sets the elution time range to limit features in.
        /// </summary>
        public ElutionTimeRange<IElutionTimePoint> ElutionTimeRange { get; set; }

        /// <summary>
        /// Gets or sets the minimum and maximum lengths of features to retain.
        /// </summary>
        public ElutionTimeRange<IElutionTimePoint> ElutionLengthRange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the RAW file should be read.
        /// </summary>
        /// <remarks>
        /// True = raw file will be read (if available). False = Scans file will be read.
        /// </remarks>
        public bool ShouldLoadRawData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether MS feature information should
        /// be persisted to the database.
        /// </summary>
        public bool ShouldPersistMsFeatures { get; set; }

        /// <summary>
        /// Gets the filter used for filtering DeconTools MSFeatures and LCMSFeatures.
        /// </summary>
        public DeconToolsFilter Filter { get; private set; }

        /// <summary>
        /// Loads features from a DeconTools results dataset and persists the features to a data access object.
        /// </summary>
        /// <param name="dataset">The data set to load.</param>
        /// <param name="dataAccessProvider">The data access object to persist features to.</param>
        /// <param name="progress">The progress reporter to report progress and status messages to.</param>
        public void Load(DatasetInformation dataset, IUmcDAO dataAccessProvider, IProgress<ProgressData> progress = null)
        {
            var features = this.Load(dataset, progress);
            dataAccessProvider.SaveFeaturesByDataset(features, dataset.DatasetId, progress);
        }

        /// <summary>
        /// Loads features from a DeconTools results dataset.
        /// </summary>
        /// <param name="dataset">The dataset to load.</param>
        /// <param name="progress"></param>
        /// <returns>The loaded LCMS features.</returns>
        /// <remarks>
        /// Performs multiple steps to load a DeconTools dataset:
        ///     1. Load DeconTools scans file OR raw data
        ///     2. Load DeconTools feature finding results (isos file)
        ///     3. Perform clustering to create LCMS features from the DeconTools results.
        ///     4. Persit LCMS features and/or MS features to database.
        /// </remarks>
        public List<UMCLight> Load(DatasetInformation dataset, IProgress<ProgressData> progress = null)
        {
            // (1) Read raw/scans data
            var scanSummaryProvider = this.scanSummaryProviderCache.GetScanSummaryProvider(dataset.RawPath);

            // Set ranges required by filter.
            this.Filter.ScanRange = new ElutionTimeRange<ScanTimePoint>(
                                        this.ElutionTimeRange.MinValue.ToScan(scanSummaryProvider),
                                        this.ElutionTimeRange.MinValue.ToScan(scanSummaryProvider));
            this.Filter.ElutionLengthRange = new ElutionTimeRange<NetTimePoint>(
                                        this.ElutionTimeRange.MinValue.ToNet(scanSummaryProvider),
                                        this.ElutionTimeRange.MaxValue.ToNet(scanSummaryProvider));

            // (2) Read and filter MSFeatureFile
            var reader = new MsFeatureLightFileReader();
            var msFeatures = reader.ReadFile(dataset.FeaturePath, this.Filter).ToList();

            // (3) Cluster features
            var clusterer = ClusterFactory.CreateMsFeatureClusterer(this.ClusteringOptions.LcmsFeatureClusteringAlgorithm);
            var lcmsFeatures = clusterer.Cluster(msFeatures, progress);

            // Further filtering of LCMS features.
            var filteredUmcs = this.Filter.FilterFeatures(lcmsFeatures);

            // (4) Persist features to database
            // TODO

            return filteredUmcs;
        }

        /// <summary>
        /// Reset all settings to their default settings.
        /// </summary>
        public void RestoreDefaults()
        {
            this.Filter.RestoreDefaults();
            this.ElutionTimeRange = new ElutionTimeRange<IElutionTimePoint>(this.Filter.ScanRange.MinValue, this.Filter.ScanRange.MaxValue);
            this.ElutionLengthRange = new ElutionTimeRange<IElutionTimePoint>(this.Filter.ElutionLengthRange.MinValue, this.Filter.ElutionLengthRange.MaxValue);
            this.ClusteringOptions = new LcmsClusteringOptions
            {
                LcmsFeatureClusteringAlgorithm = ClusteringAlgorithmTypes.SingleLinkage,
                DistanceFunction = DistanceMetric.Euclidean,
                ClusterCentroidRepresentation = ClusterCentroidRepresentation.Median,
                ShouldSeparateCharge = false,
            };

            this.ShouldLoadRawData = true;
            this.ShouldPersistMsFeatures = false;
        }
    }
}
