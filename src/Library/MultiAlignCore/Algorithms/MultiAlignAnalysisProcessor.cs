#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Algorithms.Workflow;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Analysis;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Hibernate;
using MultiAlignCore.IO.MTDB;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

#endregion

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    ///     Transition class to remove the excess from the data storage object.
    ///     Remove garbage code and mixed-mode BS.
    /// </summary>
    public class MultiAlignAnalysisProcessor : WorkflowBase, IDisposable
    {
        #region Events

        /// <summary>
        ///     Fired when the baseline features
        /// </summary>
        public event EventHandler<BaselineFeaturesLoadedEventArgs> BaselineFeaturesLoaded;

        /// <summary>
        ///     Fired when features are loaded.
        /// </summary>
        public event EventHandler<FeaturesLoadedEventArgs> FeaturesLoaded;

        /// <summary>
        ///     Fired when mass tags are loaded.
        /// </summary>
        public event EventHandler<MassTagsLoadedEventArgs> MassTagsLoaded;

        /// <summary>
        ///     Fired when features are aligned.
        /// </summary>
        public event EventHandler<FeaturesAlignedEventArgs> FeaturesAligned;

        /// <summary>
        ///     Fired when features are clustered.
        /// </summary>
        public event EventHandler<FeaturesClusteredEventArgs> FeaturesClustered;

        /// <summary>
        ///     Fired when features are peak matched.
        /// </summary>
        public event EventHandler<FeaturesPeakMatchedEventArgs> FeaturesPeakMatched;

        /// <summary>
        ///     Fired when a catastrophic error occurs.
        /// </summary>
        public event EventHandler<AnalysisErrorEventArgs> AnalysisError;

        /// <summary>
        ///     Fired when the analysis is complete.
        /// </summary>
        public event EventHandler<AnalysisCompleteEventArgs> AnalysisComplete;

        /// <summary>
        ///     Fired when the analysis is ready to start.
        /// </summary>
        public event EventHandler<AnalysisGraphEventArgs> AnalysisStarted;

        #endregion

        #region Members

        /// <summary>
        ///     Algorithms used in processing.
        /// </summary>
        private AlgorithmProvider m_algorithms;

        /// <summary>
        ///     Holds all information about how to perform the analysis and how to store results.
        /// </summary>
        private AnalysisConfig m_config;

        /// <summary>
        ///     Thread in charge of performing the analysis.
        /// </summary>
        private Thread m_analysisThread;

        private Dictionary<AnalysisStep, DelegateAnalysisMethod> m_methodMap;

        #endregion

        #region Constructor

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public MultiAlignAnalysisProcessor()
        {
            m_algorithms = null;
            CreateAnalysisMethodMap();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets whether to load data.
        /// </summary>
        public bool ShouldLoadData { get; set; }

        #endregion

        #region Algorithm Providers and Event Handlers

        /// <summary>
        ///     Gets or sets the object that provides the algorithms for clustering, aligning, etc.
        /// </summary>
        public AlgorithmProvider AlgorithmProviders
        {
            get { return m_algorithms; }
            set
            {
                m_algorithms = value;
                if (value != null)
                {
                    m_algorithms.Progress += m_algorithms_Progress;
                }
            }
        }

        /// <summary>
        ///     Status Messages from the algorithms.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_algorithms_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);
        }

        #endregion

        #region Cleanup

        /// <summary>
        ///     Dispose method that will kill the analysis thread.
        /// </summary>
        public void Dispose()
        {
            StopAnalysis();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Aborts the analysis thread.
        /// </summary>
        private void AbortAnalysisThread(Thread threadToAbort)
        {
            try
            {
                if (threadToAbort != null && threadToAbort.IsAlive)
                    threadToAbort.Abort();
            }
            catch (ThreadAbortException)
            {
            }
        }

        #endregion

        #region Delegate Handlers / Marshallers

        #endregion

        #region Analysis Graph Construction

        private void CreateAnalysisMethodMap()
        {
            m_methodMap = new Dictionary<AnalysisStep, DelegateAnalysisMethod>
            {
                {AnalysisStep.FindFeatures, PerformDataLoadAndAlignment},
                {AnalysisStep.Clustering, PerformLcmsFeatureClustering},
                {AnalysisStep.PeakMatching, PerformPeakMatching}
            };
        }

        private AnalysisGraphNode CreateNode(AnalysisStep step, string name, string description)
        {
            var node = new AnalysisGraphNode
            {
                Name = name,
                Description = description,
                CurrentStep = step,
                Method = m_methodMap[step]
            };
            return node;
        }

        public void BuildAnalysisGraph(AnalysisConfig config)
        {
            var graph = new AnalysisGraph();

            // Create a feature database
            if (config.ShouldCreateFeatureDatabaseOnly)
            {
                graph.AddNode(CreateNode(AnalysisStep.FindFeatures, "Feature Creation",
                    "Creates or loads features from deisotoped data."));
            }
            else
            {
                if (config.ShouldLoadMTDB)
                {
                    var node = new AnalysisGraphNode
                    {
                        Name = "Load MTDB",
                        Description = "Loads an AMT Tag Database for alignment or peptide identification",
                        CurrentStep = AnalysisStep.LoadMtdb,
                        Method =
                            config.InitialStep == AnalysisStep.FindFeatures
                                ? CreateMtdb
                                : new DelegateAnalysisMethod(LoadMtdb)
                    };

                    graph.AddNode(node);
                }

                switch (config.InitialStep)
                {
                    case AnalysisStep.LoadMtdb:
                    case AnalysisStep.FindFeatures:
                        graph.AddNode(CreateNode(AnalysisStep.FindFeatures, "Feature Creation And Alignment",
                            "Creates or loads features from deisotoped data and aligns them to a baseline."));
                        graph.AddNode(CreateNode(AnalysisStep.Clustering, "Cluster Features",
                            "Clusters features across datasets."));
                        graph.AddNode(CreateNode(AnalysisStep.PeakMatching, "Feature Identification",
                            "Matches features to an AMT Tag database."));
                        break;
                    case AnalysisStep.Alignment:
                        graph.AddNode(CreateNode(AnalysisStep.Alignment, "Feature Alignment",
                            "Aligns features to a reference to correct for systematic errors."));
                        graph.AddNode(CreateNode(AnalysisStep.Clustering, "Cluster Features",
                            "Clusters features across datasets."));
                        graph.AddNode(CreateNode(AnalysisStep.PeakMatching, "Feature Identification",
                            "Matches features to an AMT Tag database."));
                        break;
                    case AnalysisStep.Clustering:
                        graph.AddNode(CreateNode(AnalysisStep.Clustering, "Cluster Features",
                            "Clusters features across datasets."));
                        graph.AddNode(CreateNode(AnalysisStep.PeakMatching, "Feature Identification",
                            "Matches features to an AMT Tag database."));
                        break;
                    case AnalysisStep.PeakMatching:
                        graph.AddNode(CreateNode(AnalysisStep.PeakMatching, "Feature Identification",
                            "Matches features to an AMT Tag database."));
                        break;
                }
            }
            config.AnalysisGraph = graph;
        }

        #endregion

        #region Loading Data

        /// <summary>
        ///     Load the data from the dataset information objects to the cache at the analysis Path
        /// </summary>
        private void PerformDataLoadAndAlignment(AnalysisConfig config)
        {
            UmcLoaderFactory.Status += UMCLoaderFactory_Status;

            UpdateStatus("Loading data.");
            var analysisOptions = config.Analysis.Options;
            var datasets = config.Analysis.MetaData.Datasets.ToList();
            var lcmsFilterOptions = analysisOptions.LcmsFilteringOptions;
            var msFilterOptions = analysisOptions.MsFilteringOptions;
            var baselineDataset = config.Analysis.MetaData.BaselineDataset;
            var baselineFeatures = LoadBaselineData(baselineDataset,
                msFilterOptions,
                analysisOptions.LcmsFindingOptions,
                lcmsFilterOptions,
                config.Analysis.DataProviders,
                config.Analysis.MassTagDatabase,
                config.Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB);

            var alignmentData = new AlignmentDAOHibernate();
            alignmentData.ClearAll();


            var providers = config.Analysis.DataProviders;
            var featureCache = new FeatureLoader
            {
                Providers = providers
            };

            RegisterProgressNotifier(featureCache);

            MassTagDatabase database = null;
            if (config.Analysis.MassTagDatabase != null)
                database = new MassTagDatabase(config.Analysis.MassTagDatabase,
                    config.Analysis.Options.AlignmentOptions.MassTagObservationCount);

            SingletonDataProviders.Providers = config.Analysis.DataProviders;

            foreach (var dataset in datasets)
            {
                if (dataset.IsBaseline)
                    continue;

                var features = featureCache.LoadDataset(dataset,
                    analysisOptions.MsFilteringOptions,
                    analysisOptions.LcmsFindingOptions,
                    analysisOptions.LcmsFilteringOptions);
                features = AlignDataset(features,
                    baselineFeatures,
                    database,
                    dataset,
                    baselineDataset);
                featureCache.CacheFeatures(features);
            }
            UmcLoaderFactory.Status -= UMCLoaderFactory_Status;
        }

        /// <summary>
        ///     Loads baseline data for alignment.
        /// </summary>
        private IList<UMCLight> LoadBaselineData(DatasetInformation baselineInfo,
            MsFeatureFilteringOptions msFilterOptions,
            LcmsFeatureFindingOptions lcmsFindingOptions,
            LcmsFeatureFilteringOptions lcmsFilterOptions,
            FeatureDataAccessProviders dataProviders,
            MassTagDatabase database,
            bool shouldUseMassTagDbAsBaseline)
        {
            IList<UMCLight> baselineFeatures = null;

            UpdateStatus("Loading baseline features.");
            if (!shouldUseMassTagDbAsBaseline)
            {
                if (baselineInfo == null)
                {
                    throw new Exception("The baseline dataset was never set.");
                }

                var cache = new FeatureLoader
                {
                    Providers = dataProviders
                };

                RegisterProgressNotifier(cache);

                UpdateStatus("Loading baseline features from " + baselineInfo.DatasetName + " for alignment.");

                baselineFeatures = cache.LoadDataset(baselineInfo,
                    msFilterOptions,
                    lcmsFindingOptions,
                    lcmsFilterOptions);

                cache.CacheFeatures(baselineFeatures);
                if (BaselineFeaturesLoaded != null)
                {
                    BaselineFeaturesLoaded(this,
                        new BaselineFeaturesLoadedEventArgs(baselineInfo, baselineFeatures.ToList()));
                }

                DeRegisterProgressNotifier(cache);
            }
            else
            {
                if (database == null)
                    throw new NullReferenceException(
                        "The mass tag database has to have data in it if it's being used for drift time alignment.");

                UpdateStatus("Setting baseline features for post drift time alignment from mass tag database.");
                var tags = FeatureDataConverters.ConvertToUMC(database.MassTags);

                if (BaselineFeaturesLoaded == null)
                    return tags;

                if (tags != null)
                    BaselineFeaturesLoaded(this, new BaselineFeaturesLoadedEventArgs(null, tags.ToList(), database));
            }
            return baselineFeatures;
        }

        /// <summary>
        ///     Updates listeners
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UMCLoaderFactory_Status(object sender, UmcLoadingEventArgs e)
        {
            UpdateStatus(e.Message);
        }

        #endregion

        #region Alignment

        /// <summary>
        ///     Aligns all of the datasets.
        /// </summary>
        public void PerformAlignment(AnalysisConfig config)
        {
            UpdateStatus("Performing Alignment");

            // Connect to database of features.                
            var featureCache = config.Analysis.DataProviders.FeatureCache;

            var options = config.Analysis.Options;

            // Load the baseline data.
            var baselineInfo = config.Analysis.MetaData.BaselineDataset;
            var baselineFeatures = LoadBaselineData(baselineInfo,
                options.MsFilteringOptions,
                options.LcmsFindingOptions,
                options.LcmsFilteringOptions,
                config.Analysis.DataProviders,
                config.Analysis.MassTagDatabase,
                config.Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB);

            // Create the alignment cache and clear it.
            var alignmentCache = new AlignmentDAOHibernate();
            alignmentCache.ClearAll();


            //config.Analysis.Options.AlignmentOptions

            foreach (var datasetInfo in config.Analysis.MetaData.Datasets)
            {
                if (!datasetInfo.IsBaseline)
                {
                    UpdateStatus("Retrieving data from " + datasetInfo.DatasetName + " for alignment.");
                    var features = featureCache.FindByDatasetId(datasetInfo.DatasetId) as IList<UMCLight>;
                    features = AlignDataset(features,
                        baselineFeatures,
                        config.Analysis.MassTagDatabase,
                        datasetInfo,
                        baselineInfo);
                    featureCache.UpdateAll(features);

                    // This dataset is done!                               
                    if (FeaturesLoaded != null)
                        FeaturesLoaded(this, new FeaturesLoadedEventArgs(datasetInfo, features));
                }
                else
                {
                    config.Analysis.AlignmentData.Add(null);
                }
            }
        }

        private IList<UMCLight> AlignDataset(
            IList<UMCLight> features,
            IEnumerable<UMCLight> baselineFeatures,
            MassTagDatabase database,
            DatasetInformation datasetInfo,
            DatasetInformation baselineInfo)
        {
            AlignmentData alignmentData;

            if (baselineInfo == null && database == null)
            {
                throw new NullReferenceException("No reference was set for LC-MS alignment.");
            }

            // align the data.
            if (baselineFeatures != null && baselineInfo != null && baselineInfo.IsBaseline)
            {
                // Align pairwise and cache results intermediately.
                var aligner = m_algorithms.DatasetAligner;
                RegisterProgressNotifier(aligner);

                UpdateStatus("Aligning " + datasetInfo.DatasetName + " to baseline.");
                alignmentData = aligner.Align(baselineFeatures, features);

                DeRegisterProgressNotifier(aligner);
            }
            else
            {
                // Align pairwise and cache results intermediately.
                var aligner = m_algorithms.DatabaseAligner;
                RegisterProgressNotifier(aligner);

                UpdateStatus("Aligning " + datasetInfo.DatasetName + " to mass tag database.");
                alignmentData = aligner.Align(database, features);

                DeRegisterProgressNotifier(aligner);
            }

            if (alignmentData != null)
            {
                alignmentData.aligneeDataset = datasetInfo.DatasetName;
                alignmentData.DatasetID = datasetInfo.DatasetId;
            }

            var args = new FeaturesAlignedEventArgs(datasetInfo,
                baselineFeatures,
                features,
                alignmentData);

            if (FeaturesAligned != null)
                FeaturesAligned(this, args);

            UpdateStatus("Updating cache with aligned features.");
            return features;
        }

        #endregion

        #region Clustering

        /// <summary>
        ///     Performs clustering of LCMS Features
        /// </summary>
        public void PerformLcmsFeatureClustering(AnalysisConfig config)
        {
            var analysis = config.Analysis;
            var clusterer = m_algorithms.Clusterer;

            RegisterProgressNotifier(clusterer);
            UpdateStatus("Using Cluster Algorithm: " + clusterer);

            clusterer.Parameters = LcmsClusteringOptions.ConvertToOmics(analysis.Options.LcmsClusteringOptions);

            // This just tells us whether we are using mammoth memory partitions or not.          
            var featureCache = config.Analysis.DataProviders.FeatureCache;
            var clusterCount = 0;

            var providers = config.Analysis.DataProviders;

            // Here we see if we need to separate the charge...
            // IMS is said to require charge separation 
            if (!analysis.Options.LcmsClusteringOptions.ShouldSeparateCharge)
            {
                UpdateStatus("Clustering features from all charge states.");
                UpdateStatus("Retrieving features for clustering from cache.");
                var features = featureCache.FindAll();
                UpdateStatus(string.Format("Clustering {0} features. ", features.Count));
                var clusters = new List<UMCClusterLight>();
                clusters = clusterer.Cluster(features, clusters);
                foreach (var cluster in clusters)
                {
                    cluster.Id = clusterCount++;
                    cluster.UmcList.ForEach(x => x.ClusterId = cluster.Id);

                    // Updates the cluster with statistics
                    foreach (var feature in cluster.UmcList)
                    {
                        cluster.MsMsCount += feature.MsMsCount;
                        cluster.IdentifiedSpectraCount += feature.IdentifiedSpectraCount;
                    }
                }
                providers.ClusterCache.AddAll(clusters);
                providers.FeatureCache.UpdateAll(features);
                config.Analysis.Clusters = clusters;

                UpdateStatus(string.Format("Found {0} clusters.", clusters.Count));

                if (FeaturesClustered != null)
                {
                    FeaturesClustered(this, new FeaturesClusteredEventArgs(clusters));
                }
            }
            else
            {
                var maxChargeState = featureCache.FindMaxCharge();

                /*
                 * Here we cluster all charge states separately.  Probably IMS Data.
                 */
                UpdateStatus("Clustering charge states individually.");
                for (var chargeState = 1; chargeState <= maxChargeState; chargeState++)
                {
                    var features = featureCache.FindByCharge(chargeState);
                    if (features.Count < 1)
                    {
                        UpdateStatus(string.Format("No features found for charge state {0}.  Stopping clustering",
                            chargeState));
                        break;
                    }
                    UpdateStatus(
                        string.Format("Retrieved and is clustering {0} features from charge state {1}.",
                            features.Count, chargeState));

                    var clusters = clusterer.Cluster(features);
                    foreach (var cluster in clusters)
                    {
                        cluster.Id = clusterCount++;
                        cluster.UmcList.ForEach(x => x.ClusterId = cluster.Id);

                        // Updates the cluster with statistics
                        foreach (var feature in cluster.Features)
                        {
                            cluster.MsMsCount += feature.MsMsCount;
                            cluster.IdentifiedSpectraCount += feature.IdentifiedSpectraCount;
                        }
                    }

                    config.Analysis.DataProviders.ClusterCache.AddAll(clusters);
                    config.Analysis.DataProviders.FeatureCache.UpdateAll(features);
                    UpdateStatus(string.Format("Found {0} clusters.", clusters.Count));

                    if (FeaturesClustered != null)
                    {
                        FeaturesClustered(this, new FeaturesClusteredEventArgs(clusters, chargeState));
                    }
                }

                config.Analysis.Clusters = config.Analysis.DataProviders.ClusterCache.FindAll();
            }
            DeRegisterProgressNotifier(clusterer);
            UpdateStatus(string.Format("Finished clustering.  Found {0} total clusters.", clusterCount));
        }

        #endregion

        #region Identification

        /// <summary>
        ///     Performs peak matching with loaded clusters.
        /// </summary>
        private void PerformPeakMatching(AnalysisConfig config)
        {
            if (!config.ShouldPeakMatch)
                return;

            if (m_config.Analysis.MassTagDatabase == null)
            {
                UpdateStatus("Could not peak match.  The database was not set.");
            }
            else
            {
                var clusters = m_config.Analysis.DataProviders.ClusterCache.FindAll();
                var peakMatcher = m_algorithms.PeakMatcher;

                UpdateStatus("Performing Peak Matching");
                var adapter = peakMatcher as STACAdapter<UMCClusterLight>;
                if (adapter != null)
                {
                    UpdateStatus(adapter.Options.UseDriftTime ? "Using drift time." : "Ignoring drift time.");
                }

                var matchResults = new PeakMatchingResults<UMCClusterLight, MassTagLight>();
                clusters.ForEach(x => x.Net = x.Net);
                matchResults.Matches = peakMatcher.PerformPeakMatching(clusters, m_config.Analysis.MassTagDatabase);

                if (adapter != null)
                {
                    matchResults.FdrTable = adapter.Matcher.StacFdrTable;
                }
                m_config.Analysis.MatchResults = matchResults;

                if (FeaturesPeakMatched != null)
                {
                    FeaturesPeakMatched(this, new FeaturesPeakMatchedEventArgs(clusters, matchResults.Matches));
                }

                UpdateStatus("Updating database with peak matched results.");
                var writer = new PeakMatchResultsWriter();
                int matchedMassTags;
                int matchedProteins;
                writer.WritePeakMatchResults(matchResults,
                    m_config.Analysis.MassTagDatabase,
                    out matchedMassTags,
                    out matchedProteins);

                UpdateStatus(string.Format("Found {0} mass tag matches. Matching to {1} potential proteins.",
                    matchedMassTags,
                    matchedProteins));
            }
        }

        #endregion

        #region Analysis Start/Stop

        /// <summary>
        ///     Starts a multi-Align analysis job.
        /// </summary>
        public void StartAnalysis(AnalysisConfig config)
        {
            m_config = config;

            if (m_algorithms == null)
            {
                throw new NullReferenceException("The algorithms have not been set for this analysis.");
            }

            if (config.Analysis == null)
            {
                throw new NullReferenceException("The analysis data storage cannot be null.");
            }

            if (config.Analysis.DataProviders == null)
            {
                throw new NullReferenceException("The data cache providers have not been set for this analysis.");
            }
            Logger.LogPath = AnalysisPathUtils.BuildLogPath(config.AnalysisPath, config.AnalysisName);

            // Make sure we start with a fresh analysis.
            AbortAnalysisThread(m_analysisThread);

            var threadStart = new ThreadStart(PerformAnalysis);
            m_analysisThread = new Thread(threadStart);
            m_analysisThread.Start();
        }

        /// <summary>
        ///     Aborts the analysis thread.
        /// </summary>
        public void StopAnalysis()
        {
            AbortAnalysisThread(m_analysisThread);
        }

        private void LoadMtdb(AnalysisConfig config)
        {
            UpdateStatus("Loading the database from the SQLite result database.");

            var provider = new MassTagDatabaseLoaderCache {Provider = m_config.Analysis.DataProviders.MassTags};
            RegisterProgressNotifier(provider);

            var database = provider.LoadDatabase();
            config.Analysis.MassTagDatabase = database;

            if (MassTagsLoaded != null)
            {
                MassTagsLoaded(this, new MassTagsLoadedEventArgs(database.MassTags, database));
            }
            DeRegisterProgressNotifier(provider);
        }

        /// <summary>
        ///     Creates an entry in the DB if a new database should be created.
        /// </summary>
        /// <param name="config"></param>
        private void CreateMtdb(AnalysisConfig config)
        {
            MassTagDatabase database;

            // Load the mass tag database if we are aligning, or if we are 
            // peak matching (but aligning to a reference dataset.
            if (m_config.Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB)
            {
                UpdateStatus("Loading Mass Tag database from database:  " +
                             m_config.Analysis.MetaData.Database.DatabaseName);
                database = MtdbLoaderFactory.LoadMassTagDatabase(m_config.Analysis.MetaData.Database,
                    m_config.Analysis.Options.MassTagDatabaseOptions);
            }
            else if (m_config.Analysis.MetaData.Database != null &&
                     m_config.Analysis.MetaData.Database.DatabaseFormat != MassTagDatabaseFormat.None)
            {
                UpdateStatus("Loading Mass Tag database from database:  " +
                             m_config.Analysis.MetaData.Database.DatabaseName);
                database = MtdbLoaderFactory.LoadMassTagDatabase(m_config.Analysis.MetaData.Database,
                    m_config.Analysis.Options.MassTagDatabaseOptions);
            }
            else
            {
                config.Analysis.MassTagDatabase = null;
                config.ShouldPeakMatch = false;
                return;
            }


            if (database != null)
            {
                config.ShouldPeakMatch = true;
                var totalMassTags = database.MassTags.Count;
                UpdateStatus("Loaded " + totalMassTags + " mass tags.");
            }

            config.Analysis.MassTagDatabase = database;


            if (database == null)
                return;

            config.Analysis.DataProviders.MassTags.AddAll(database.MassTags);

            var proteinCache = new ProteinDAO();
            proteinCache.AddAll(database.AllProteins);

            var map = (from massTagId in database.Proteins.Keys
                from p in database.Proteins[massTagId]
                select new MassTagToProteinMap
                {
                    ProteinId = p.ProteinId,
                    MassTagId = massTagId,
                    RefId = p.RefId
                }).ToList();

            var tempCache = new GenericDAOHibernate<MassTagToProteinMap>();
            tempCache.AddAll(map);

            if (MassTagsLoaded != null)
                MassTagsLoaded(this, new MassTagsLoadedEventArgs(database.MassTags, database));
        }

        /// <summary>
        ///     Starts the main analysis.
        /// </summary>
        private void PerformAnalysis()
        {
            try
            {
                BuildAnalysisGraph(m_config);
                var graph = m_config.AnalysisGraph;

                if (AnalysisStarted != null)
                    AnalysisStarted(this, new AnalysisGraphEventArgs(graph));

                foreach (var node in graph.Nodes)
                {
                    node.IsCurrent = true;
                    node.Method(m_config);
                    node.IsCurrent = false;
                }
            }
            catch (OutOfMemoryException ex)
            {
                UmcLoaderFactory.Status -= UMCLoaderFactory_Status;
                if (AnalysisError != null)
                    AnalysisError(this, new AnalysisErrorEventArgs("Out of memory.", ex));

                return;
            }
            catch (Exception ex)
            {
                UmcLoaderFactory.Status -= UMCLoaderFactory_Status;
                if (AnalysisError != null)
                    AnalysisError(this, new AnalysisErrorEventArgs("Handled Error. ", ex));

                return;
            }

            if (AnalysisComplete != null)
                AnalysisComplete(this, new AnalysisCompleteEventArgs(m_config.Analysis));
        }

        #endregion
    }
}