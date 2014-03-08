using MultiAlign.IO;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Algorithms.Workflow;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignCore.IO.MTDB;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Distance;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MultiAlignCore.Algorithms
{        
    /// <summary>
    /// Transition class to remove the excess from the data storage object.
    ///     Remove garbage code and mixed-mode BS.
    /// </summary>
    public class MultiAlignAnalysisProcessor: WorkflowBase, IDisposable
    {   
        #region Events
        /// <summary>
        /// Fired when the baseline features 
        /// </summary>
        public event EventHandler<BaselineFeaturesLoadedEventArgs> BaselineFeaturesLoaded;
        /// <summary>
        /// Fired when features are loaded.
        /// </summary>
        public event EventHandler<FeaturesLoadedEventArgs>      FeaturesLoaded;
        /// <summary>
        /// Fired when mass tags are loaded.
        /// </summary>
        public event EventHandler<MassTagsLoadedEventArgs>      MassTagsLoaded;
        /// <summary>
        /// Fired when features are aligned.
        /// </summary>
        public event EventHandler<FeaturesAlignedEventArgs>     FeaturesAligned;
        /// <summary>
        /// Fired when features are clustered.
        /// </summary>
        public event EventHandler<FeaturesClusteredEventArgs>   FeaturesClustered;
        /// <summary>
        /// Fired when features are peak matched.
        /// </summary>
        public event EventHandler<FeaturesPeakMatchedEventArgs> FeaturesPeakMatched;
        /// <summary>
        /// Fired when a catastrophic error occurs.
        /// </summary>
        public event EventHandler<AnalysisErrorEventArgs>       AnalysisError;
        /// <summary>
        /// Fired when the analysis is complete.
        /// </summary>
        public event EventHandler<AnalysisCompleteEventArgs>    AnalysisComplete;   
        #endregion

        #region Members
        /// <summary>
        /// Algorithms used in processing.
        /// </summary>
        private AlgorithmProvider m_algorithms;
        /// <summary>
        /// Holds all information about how to perform the analysis and how to store results.
        /// </summary>
        private AnalysisConfig m_config;
        /// <summary>
        /// Thread in charge of performing the analysis.
        /// </summary>
        private Thread m_analysisThread; 
        Dictionary<AnalysisStep, DelegateAnalysisMethod> m_methodMap;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MultiAlignAnalysisProcessor()
        {
            m_algorithms      = null;
            AnalysisStartStep = AnalysisStep.Alignment;

            XicWriter = new XicWriter();

            CreateAnalysisMethodMap();            
        }
        #endregion

        #region Properties
        /// <summary>
        /// Writer for exporting Xic's
        /// </summary>
        public IXicWriter XicWriter
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether to load data.
        /// </summary>
        public bool ShouldLoadData
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the path to store the output data
        /// </summary>
        public string AnalysisPath
        {
            get;
            set;
        }
        public AnalysisStep AnalysisStartStep
        {
            get;
            set;
        }
        #endregion

        #region Algorithm Providers and Event Handlers
        /// <summary>
        /// Gets or sets the object that provides the algorithms for clustering, aligning, etc.
        /// </summary>
        public AlgorithmProvider AlgorithmProviders
        {
            get
            {
                return m_algorithms;
            }
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
        /// Status Messages from the algorithms.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_algorithms_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);   
        }       
        #endregion

        #region Cleanup
        /// <summary>
        /// Dispose method that will kill the analysis thread.
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
        /// Aborts the analysis thread.
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
            m_methodMap                                     = new Dictionary<AnalysisStep, DelegateAnalysisMethod>
            {
                {AnalysisStep.FindFeatures, PerformDataLoadAndAlignment},
                {AnalysisStep.Alignment,    PerformAlignment},
                {AnalysisStep.Clustering,   PerformLcmsFeatureClustering},
                {AnalysisStep.PeakMatching, PerformPeakMatching}
            };
        }
        
        private AnalysisGraphNode CreateNode(AnalysisStep step)
        {
            var node        = new AnalysisGraphNode 
            {
                CurrentStep = step, 
                Method = m_methodMap[step]
            };
            return node;
        }
            
        public void BuildAnalysisGraph(AnalysisConfig config)
        {            
            var graph  = new AnalysisGraph();
            
            // Create a feature database
            if (config.ShouldCreateFeatureDatabaseOnly)
            {                
                graph.AddNode(CreateNode(AnalysisStep.FindFeatures));                
            }
            else
            {
                if (config.ShouldLoadMTDB)
                {
                    var node  = new AnalysisGraphNode();
                    node.CurrentStep        = AnalysisStep.LoadMTDB;

                    node.Method = config.InitialStep == AnalysisStep.FindFeatures ? CreateMtdb : new DelegateAnalysisMethod(LoadMtdb);
                    
                    graph.AddNode(node);                    
                }
                
                switch (config.InitialStep)
                {
                    case AnalysisStep.LoadMTDB:
                    case AnalysisStep.FindFeatures:
                        graph.AddNode(CreateNode(AnalysisStep.FindFeatures));
                        graph.AddNode(CreateNode(AnalysisStep.Clustering));
                        graph.AddNode(CreateNode(AnalysisStep.PeakMatching));
                        break;
                    case AnalysisStep.Alignment:
                        graph.AddNode(CreateNode(AnalysisStep.Alignment));
                        graph.AddNode(CreateNode(AnalysisStep.Clustering));
                        graph.AddNode(CreateNode(AnalysisStep.PeakMatching));
                        break;
                    case AnalysisStep.Clustering:
                        graph.AddNode(CreateNode(AnalysisStep.Clustering));
                        graph.AddNode(CreateNode(AnalysisStep.PeakMatching));
                        break;
                    case AnalysisStep.ClusterQC:                        
                        graph.AddNode(CreateNode(AnalysisStep.ClusterQC));
                        break;
                    case AnalysisStep.PeakMatching:
                        graph.AddNode(CreateNode(AnalysisStep.PeakMatching));
                        break;
                }                
            }
            config.AnalysisGraph = graph;
        }
        #endregion

        #region Loading Data
        /// <summary>
        /// Load the data from the dataset information objects to the cache at the analysis Path
        /// </summary>
        private void PerformDataLoadAndAlignment(AnalysisConfig config)
        {
            UMCLoaderFactory.Status += UMCLoaderFactory_Status;

            UpdateStatus("Loading data.");
            var datasets       = config.Analysis.MetaData.Datasets.ToList(); 
            var options        = config.Analysis.Options.FeatureFindingOptions;            
            var filterOptions  = config.Analysis.Options.FeatureFilterOptions;

            var baselineDataset     = config.Analysis.MetaData.BaselineDataset;
            var baselineFeatures    = LoadBaselineData(baselineDataset,
                                                        options,
                                                        filterOptions,
                                                        config.Analysis.DataProviders,
                                                        config.Analysis.MassTagDatabase,
                                                        config.Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB,
                                                        config.Analysis.Options.DriftTimeAlignmentOptions.ShouldAlignDriftTimes);            
            var aligner       = m_algorithms.Aligner;
            var alignmentData = new AlignmentDAOHibernate();
            alignmentData.ClearAll();


            var providers = config.Analysis.DataProviders;
            var featureCache = new FeatureLoader
            {
                Providers = providers
            };

            RegisterProgressNotifier(aligner);
            RegisterProgressNotifier(featureCache);

            MassTagDatabase database = null;
            if (config.Analysis.MassTagDatabase != null)            
                database = new MassTagDatabase(config.Analysis.MassTagDatabase, config.Analysis.Options.AlignmentOptions.MassTagObservationCount);


            if (true)
            {
                SingletonDataProviders.Providers = config.Analysis.DataProviders;
            }

            foreach (var dataset in datasets)
            {
                if (dataset.IsBaseline) continue;

                var features = featureCache.LoadDataset( dataset,
                                                        options,
                                                        filterOptions);

                features = AlignDataset(features,
                                        baselineFeatures,
                                        database,
                                        config.Analysis.Options.AlignmentOptions,
                                        alignmentData,
                                        aligner,
                                        dataset,
                                        baselineDataset);

                featureCache.CacheFeatures(features);
            }
            DeRegisterProgressNotifier(aligner);
            UMCLoaderFactory.Status -= UMCLoaderFactory_Status;
        }

        /// <summary>
        /// Loads baseline data for alignment.
        /// </summary>
        private IList<UMCLight> LoadBaselineData(DatasetInformation         baselineInfo,
                                                LCMSFeatureFindingOptions   options,
                                                FeatureFilterOptions        filterOptions,
                                                FeatureDataAccessProviders  dataProviders,
                                                MassTagDatabase             database,
                                                bool                        shouldUseMassTagDbAsBaseline,
                                                bool                        shouldAlignDriftTimes)
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
                baselineFeatures =  cache.LoadDataset(  baselineInfo,
                                                        options,
                                                        filterOptions);
                cache.CacheFeatures(baselineFeatures);
                if (BaselineFeaturesLoaded != null)
                {
                    BaselineFeaturesLoaded(this, new BaselineFeaturesLoadedEventArgs(baselineInfo, baselineFeatures.ToList()));
                }

                DeRegisterProgressNotifier(cache);
            }
            else
            {                
                if (baselineInfo == null && shouldAlignDriftTimes)
                {
                    if (database == null)                    
                        throw new NullReferenceException("The mass tag database has to have data in it if it's being used for drift time alignment.");
                    
                    UpdateStatus( "Setting baseline features for post drift time alignment from mass tag database.");
                    baselineFeatures = FeatureDataConverters.ConvertToUMC(database.MassTags);
                }
                
                if (BaselineFeaturesLoaded != null)
                    if (baselineFeatures != null)
                        BaselineFeaturesLoaded(this, new BaselineFeaturesLoadedEventArgs(null, baselineFeatures.ToList(), database));
            }
            return baselineFeatures;
        }
        /// <summary>
        /// Updates listeners 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UMCLoaderFactory_Status(object sender, UMCLoadingEventArgs e)
        {
            UpdateStatus( e.Message);
        }
        #endregion
                 
        #region Alignment
        /// <summary>
        /// Aligns all of the datasets.
        /// </summary>
        public void PerformAlignment(AnalysisConfig config)
        {
            UpdateStatus("Performing Alignment");

            // Connect to database of features.                
            var featureCache = config.Analysis.DataProviders.FeatureCache;

            // Load the baseline data.
            var baselineInfo = config.Analysis.MetaData.BaselineDataset;
            var baselineFeatures = LoadBaselineData(baselineInfo,
                                                    config.Analysis.Options.FeatureFindingOptions,
                                                    config.Analysis.Options.FeatureFilterOptions,                                                                   
                                                    config.Analysis.DataProviders,
                                                    config.Analysis.MassTagDatabase,
                                                    config.Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB,
                                                    config.Analysis.Options.DriftTimeAlignmentOptions.ShouldAlignDriftTimes);

            // Create the alignment cache and clear it.
            var alignmentCache = new AlignmentDAOHibernate();
            alignmentCache.ClearAll();

            // Align pairwise and cache results intermediately.
            var aligner = m_algorithms.Aligner;
            RegisterProgressNotifier(aligner);

            foreach(var datasetInfo in config.Analysis.MetaData.Datasets)
            {
                if (!datasetInfo.IsBaseline)
                {
                    UpdateStatus("Retrieving data from " + datasetInfo.DatasetName + " for alignment.");
                    var features             = featureCache.FindByDatasetId(datasetInfo.DatasetId) as IList<UMCLight>;
                    features                 = AlignDataset(features, 
                                                            baselineFeatures, 
                                                            config.Analysis.MassTagDatabase,
                                                            config.Analysis.Options.AlignmentOptions,       
                                                            alignmentCache,
                                                            aligner, 
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
            DeRegisterProgressNotifier(aligner);
        }

        private IList<UMCLight> AlignDataset(
                                            IList<UMCLight>          features,
                                            IList<UMCLight>          baselineFeatures,
                                            MassTagDatabase         database,
                                            AlignmentOptions        options,
                                            IAlignmentDAO           alignmentCache,
                                            IFeatureAligner         aligner, 
                                            DatasetInformation      datasetInfo,
                                            DatasetInformation      baselineInfo)
        {
            classAlignmentData alignmentData;

            // align the data.
            if (baselineFeatures != null)
            {
                UpdateStatus( "Aligning " + datasetInfo.DatasetName + " to baseline.");
                alignmentData = AlignFeatures(features, baselineFeatures, aligner, options);
            }
            else
            {
                UpdateStatus( "Aligning " + datasetInfo.DatasetName + " to mass tag database.");
                alignmentData = AlignFeatures(features, database, aligner, options);
            }

            if (alignmentData != null)
            {
                alignmentData.aligneeDataset = datasetInfo.DatasetName;
                alignmentData.DatasetID = datasetInfo.DatasetId;

            }

            var args = new FeaturesAlignedEventArgs(baselineInfo,
                                                    datasetInfo,
                                                    alignmentData);

            args.AlignedFeatures = features.ToList();
                        
            if (FeaturesAligned != null)            
                FeaturesAligned(this, args);
            
            if (options.ShouldStoreAlignmentFunction && alignmentData != null)                        
                alignmentCache.Add(alignmentData);
                        
            UpdateStatus( "Updating cache with aligned features.");
            return features;
        }
        private classAlignmentData AlignFeatures(IList<UMCLight>   features,
                                                 IList<UMCLight>    baselineFeatures,
                                                 IFeatureAligner   aligner,
                                                 AlignmentOptions  options)
        {
            classAlignmentData alignmentData;
            
            alignmentData = aligner.AlignFeatures(baselineFeatures.ToList(),
                                                    features.ToList(),
                                                    options);
            
            return alignmentData;
        }
        private classAlignmentData AlignFeatures(   IList<UMCLight>  features,
                                                    MassTagDatabase  database,
                                                    IFeatureAligner  aligner,
                                                    AlignmentOptions options)
        {
            var alignmentData = aligner.AlignFeatures(database,
                                                    features.ToList(),
                                                    options,
                                                    false);

            return alignmentData;
        }

        #endregion

        #region Clustering
        /// <summary>
        /// Performs clustering of LCMS Features
        /// </summary>
        public void PerformLcmsFeatureClustering(AnalysisConfig config)
        {
            var analysis = config.Analysis;
            var clusterer = m_algorithms.Clusterer;

            RegisterProgressNotifier(clusterer);
            UpdateStatus( "Using Cluster Algorithm: " + clusterer);
            
            clusterer.Parameters                    = Clustering.LCMSFeatureClusteringOptions.ConvertToOmics(analysis.Options.ClusterOptions);
            clusterer.Parameters.DistanceFunction   = DistanceFactory<UMCLight>.CreateDistanceFunction(analysis.Options.ClusterOptions.DistanceFunction);

            // This just tells us whether we are using mammoth memory partitions or not.          
            var featureCache            = config.Analysis.DataProviders.FeatureCache;            
            var clusterCount                = 0;
            
            var providers = config.Analysis.DataProviders;

            if (analysis.Options.ClusterOptions.IgnoreCharge)
            {
                /*
                 * Here we cluster all charge states together.  Probably Non IMS data.
                 */
                UpdateStatus("Clustering features from all charge states.");
                UpdateStatus( "Retrieving features for clustering from cache.");                
                var features         = featureCache.FindAll();
                UpdateStatus( string.Format("Clustering {0} features. ", features.Count));
                var clusters = new List<UMCClusterLight>();
                clusters = clusterer.Cluster(features, clusters);                
                foreach (UMCClusterLight cluster in clusters)
                {
                    cluster.ID = clusterCount++;
                    cluster.UMCList.ForEach(x => x.ClusterID = cluster.ID);

                    // Updates the cluster with statistics
                    foreach (UMCLight feature in cluster.UMCList)
                    {
                        cluster.MsMsCount += feature.MsMsCount;
                        cluster.IdentifiedSpectraCount += feature.IdentifiedSpectraCount;
                    }
                }
                providers.ClusterCache.AddAll(clusters);
                providers.FeatureCache.UpdateAll(features);
                config.Analysis.Clusters = clusters;
                    
                UpdateStatus( string.Format("Found {0} clusters.", clusters.Count));

                if (FeaturesClustered != null)
                {
                    FeaturesClustered(this, new FeaturesClusteredEventArgs(clusters));
                }
            }
            else
            {
                int maxChargeState = featureCache.FindMaxCharge();
                int minChargeState = 1;
            
                /*
                 * Here we cluster all charge states separately.  Probably IMS Data.
                 */
                UpdateStatus("Clustering charge states individually.");
                for(var chargeState = minChargeState; chargeState <= maxChargeState; chargeState++)
                {
                    List<UMCLight> features         = featureCache.FindByCharge(chargeState);
                    if (features.Count < 1)
                    {
                        UpdateStatus(string.Format("No features found for charge state {0}.  Stopping clustering", chargeState));
                        break;
                    }
                    UpdateStatus(
                        string.Format("Retrieved and is clustering {0} features from charge state {1}.", 
                                features.Count, chargeState));              
                                        
                    var clusters  = clusterer.Cluster(features);
                    foreach (var cluster in clusters)
                    {
                        cluster.ID = clusterCount++;
                        cluster.UMCList.ForEach(x => x.ClusterID = cluster.ID);

                        // Updates the cluster with statistics
                        foreach (UMCLight feature in cluster.Features)
                        {
                            cluster.MsMsCount += feature.MsMsCount;
                            cluster.IdentifiedSpectraCount += feature.IdentifiedSpectraCount;
                        }                    
                    }                    
                    
                    config.Analysis.DataProviders.ClusterCache.AddAll(clusters);
                    config.Analysis.DataProviders.FeatureCache.UpdateAll(features);
                    UpdateStatus( string.Format("Found {0} clusters.", clusters.Count));

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
        /// Performs peak matching with loaded clusters. 
        /// </summary>
        public void PerformPeakMatching(AnalysisConfig config)
        {            
            if (config.ShouldPeakMatch)
            {
                if (m_config.Analysis.MassTagDatabase == null)
                {
                    UpdateStatus("Could not peak match.  The database was not set.");
                }
                else
                {                    
                    List<UMCClusterLight> clusters              = m_config.Analysis.DataProviders.ClusterCache.FindAll();
                    IPeakMatcher<UMCClusterLight> peakMatcher   = m_algorithms.PeakMatcher;

                    UpdateStatus("Performing Peak Matching");                          
                    STACAdapter<UMCClusterLight> adapter = peakMatcher as STACAdapter<UMCClusterLight>;
                    if (adapter != null)
                    {
                        if (adapter.Options.UseDriftTime)
                        {
                            UpdateStatus( "Using drift time.");
                        }
                        else
                        {
                            UpdateStatus( "Ignoring drift time.");
                        }
                    }
                    else
                    {
                        TraditionalPeakMatcher<UMCClusterLight> traditional = peakMatcher as TraditionalPeakMatcher<UMCClusterLight>;
                        if (traditional != null && !m_config.Analysis.MassTagDatabase.DoesContainDriftTime)
                        {
                            if (!m_config.Analysis.MassTagDatabase.DoesContainDriftTime)
                            {
                                UpdateStatus( "The database does not contain drift time.  Ensuring drift time tolerances are disabled.");
                                traditional.Options.DriftTimeTolerance = 1000;
                            }
                        }
                    }

                    PeakMatchingResults<UMCClusterLight, MassTagLight> matchResults = new PeakMatchingResults<UMCClusterLight, MassTagLight>();
                    clusters.ForEach(x => x.NET = x.RetentionTime);
                    matchResults.Matches = peakMatcher.PerformPeakMatching(clusters, m_config.Analysis.MassTagDatabase);

                    if (adapter != null)
                    {
                        matchResults.FdrTable = adapter.Matcher.STACFDRTable;
                    }
                    m_config.Analysis.MatchResults = matchResults;

                    if (FeaturesPeakMatched != null)
                    {
                        FeaturesPeakMatched(this, new FeaturesPeakMatchedEventArgs(clusters, matchResults.Matches));
                    }

                    
                    
                    
                    UpdateStatus( "Updating database with peak matched results.");
                    var writer = new PeakMatchResultsWriter();
                    int matchedMassTags;
                    int matchedProteins;
                    writer.WritePeakMatchResults(matchResults,
                                                 m_config.Analysis.MassTagDatabase,
                                                 out matchedMassTags,
                                                 out matchedProteins);

                    UpdateStatus( string.Format("Found {0} mass tag matches. Matching to {1} potential proteins.",
                                                                                                matchedMassTags,
                                                                                                matchedProteins));

                    if (m_config.Analysis.Options.STACOptions.WriteResultsBackToMTS && m_config.Analysis.MetaData.JobID != -1)
                    {
                        const string databasePath = "";
                        MTSPeakMatchResultsWriter mtsWriter = new MTSSqlServerPeakMatchResultWriter();
                        mtsWriter.WriteClusters(m_config.Analysis,
                                                clusters,
                                                databasePath);
                    }
                    else if (m_config.Analysis.Options.STACOptions.WriteResultsBackToMTS)
                    {
                        UpdateStatus( "Cannot write mass tag results back to database.  The Job ID was not specified for this analysis.");
                    }
                }
            }
        }
        #endregion

                
        #region Analysis Start/Stop
        /// <summary>
        /// Starts a multi-Align analysis job.
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

            // Make sure we start with a fresh analysis.
            AbortAnalysisThread(m_analysisThread);

            var threadStart  = new ThreadStart(PerformAnalysis);
            m_analysisThread = new Thread(threadStart);
            m_analysisThread.Start();
        }
        /// <summary>
        /// Aborts the analysis thread.
        /// </summary>
        public void StopAnalysis()
        {
            AbortAnalysisThread(m_analysisThread);
        }
        private void LoadMtdb(AnalysisConfig config)
        {
            UpdateStatus("Loading the database from the SQLite result database.");

            var provider  = new MassTagDatabaseLoaderCache {Provider = m_config.Analysis.DataProviders.MassTags};
            RegisterProgressNotifier(provider);

            var database                    = provider.LoadDatabase();                       
            config.Analysis.MassTagDatabase = database;
            
            if (MassTagsLoaded != null)
            {
                MassTagsLoaded(this, new MassTagsLoadedEventArgs(database.MassTags, database));
            }
            DeRegisterProgressNotifier(provider);
        }
        /// <summary>
        /// Creates an entry in the DB if a new database should be created.
        /// </summary>
        /// <param name="config"></param>
        private void CreateMtdb(AnalysisConfig config)
        {
            MassTagDatabase database;

            // Load the mass tag database if we are aligning, or if we are 
            // peak matching (but aligning to a reference dataset.
            if (m_config.Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB)
            {
                UpdateStatus("Loading Mass Tag database from database:  " + m_config.Analysis.MetaData.Database.DatabaseName);
                database = MTDBLoaderFactory.LoadMassTagDB(m_config.Analysis.MetaData.Database, m_config.Analysis.Options.MassTagDatabaseOptions);
            }
            else if (m_config.Analysis.MetaData.Database != null && m_config.Analysis.MetaData.Database.DatabaseFormat != MassTagDatabaseFormat.None)
            {
                UpdateStatus("Loading Mass Tag database from database:  " + m_config.Analysis.MetaData.Database.DatabaseName);
                database = MTDBLoaderFactory.LoadMassTagDB(m_config.Analysis.MetaData.Database, m_config.Analysis.Options.MassTagDatabaseOptions);
            }
            else
            {
                config.Analysis.MassTagDatabase = null;
                config.ShouldPeakMatch          = false;
                return;
            }
            

            if (database != null)
            {
                config.ShouldPeakMatch  = true;
                var totalMassTags       = database.MassTags.Count;
                UpdateStatus("Loaded " + totalMassTags + " mass tags.");
            }

            config.Analysis.MassTagDatabase = database;


            if (database != null)
            {
                config.Analysis.DataProviders.MassTags.AddAll(database.MassTags);

                var proteinCache = new ProteinDAO();          
                proteinCache.AddAll(database.AllProteins);

                var map = new List<MassTagToProteinMap>();
                foreach (var massTagId in database.Proteins.Keys)
                {
                    foreach(var p in database.Proteins[massTagId])
                    {                    
                        var tempMap = new MassTagToProteinMap
                        {
                            ProteinId = p.ProteinID,
                            MassTagId = massTagId,
                            RefId = p.RefID
                        };
                        map.Add(tempMap);
                    }
                }

                var tempCache = new GenericDAOHibernate<MassTagToProteinMap>();
                tempCache.AddAll(map);

                if (MassTagsLoaded != null)            
                    MassTagsLoaded(this, new MassTagsLoadedEventArgs(database.MassTags, database));
            }
        }
        
        /// <summary>
        /// Starts the main analysis.
        /// </summary>
        private void PerformAnalysis()
        {
            try
            {
                BuildAnalysisGraph(m_config);

                var graph = m_config.AnalysisGraph;
                foreach (var node in graph.Nodes)
                {
                    node.Method(m_config);
                }                
            }
            catch (OutOfMemoryException ex)
            {
                UMCLoaderFactory.Status -= UMCLoaderFactory_Status;
                if (AnalysisError != null)
                {
                    AnalysisError(this, new AnalysisErrorEventArgs("Out of memory.", ex));
                }
                return;
            }
            catch (Exception ex)
            {
                UMCLoaderFactory.Status -= UMCLoaderFactory_Status;
                if (AnalysisError != null)
                {
                    AnalysisError(this, new AnalysisErrorEventArgs("Handled Error. ", ex));
                }
                return;
            }
           

            if (AnalysisComplete != null)
            {
                AnalysisComplete(this, new AnalysisCompleteEventArgs(m_config.Analysis));
            }            
        }
        #endregion    

        

    }
}
