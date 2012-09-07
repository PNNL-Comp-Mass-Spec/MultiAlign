using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiAlignCore.IO.SequenceData;
using System.Threading;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Algorithms.MSLinker;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.Mammoth;
using MultiAlignCore.IO.MTDB;
using MultiAlignEngine.Features;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms.FeatureMatcher.MSnLinker;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using PNNLOmicsIO.IO;

namespace MultiAlignCore.Algorithms
{        
    /// <summary>
    /// Transition class to remove the excess from the data storage object.
    ///     Remove garbage code and mixed-mode BS.
    /// </summary>
    public class MultiAlignAnalysisProcessor: IDisposable
    {   
        #region Events
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
        /// Fired when features are extracted from datasets.
        /// </summary>
        public event EventHandler<FeaturesExtractedEventArgs>   FeaturesExtracted;
        /// <summary>
        /// Fired when a catastrophic error occurs.
        /// </summary>
        public event EventHandler<AnalysisErrorEventArgs>       AnalysisError;
        /// <summary>
        /// Fired when the analysis is complete.
        /// </summary>
        public event EventHandler<AnalysisCompleteEventArgs>    AnalysisComplete;
        /// <summary>
        /// Fired when a status update is ready.
        /// </summary>
        public event EventHandler<AnalysisStatusEventArgs>      Status;        
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
            AnalysisStartStep = Algorithms.AnalysisStep.Alignment;

            CreateAnalysisMethodMap();            
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether to load data.
        /// </summary>
        public bool ShouldLoadData
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the path to store the SIC's
        /// </summary>
        public string AnalaysisPath
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
        public AlgorithmProvider AlgorithmProvders
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
                    m_algorithms.Progress += new EventHandler<ProgressNotifierArgs>(m_algorithms_Progress);
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
                                    
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
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
            finally
            {                
                threadToAbort = null;
            }
        }
        #endregion

        #region Delegate Handlers / Marshallers
        /// <summary>
        /// Updates listeners with status messages.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatus(string message)
        {
            if (Status != null)
            {
                Status(this, new AnalysisStatusEventArgs(message, 0));
            }
        }
        private void RegisterProgressNotifier(IProgressNotifer notifier)
        {
            if (notifier != null)
            {
                notifier.Progress += new EventHandler<ProgressNotifierArgs>(notifier_Progress);
            }
        }
        private void DeRegisterProgressNotifier(IProgressNotifer notifier)
        {
            if (notifier != null)
            {
                notifier.Progress -= notifier_Progress;
            }
        }
        void notifier_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);
        }
        #endregion

        #region Analysis Graph Construction
        private void CreateAnalysisMethodMap()
        {            
            m_methodMap                                     = new Dictionary<AnalysisStep, DelegateAnalysisMethod>();
            m_methodMap.Add(AnalysisStep.FindFeatures,      new DelegateAnalysisMethod(LoadData));            
            m_methodMap.Add(AnalysisStep.Alignment,         new DelegateAnalysisMethod(PerformAlignment));
            m_methodMap.Add(AnalysisStep.Clustering,        new DelegateAnalysisMethod(PerformLCMSFeatureClustering));
            m_methodMap.Add(AnalysisStep.PeakMatching,      new DelegateAnalysisMethod(PerformPeakMatching));            
        }
        
        private AnalysisGraphNode CreateNode(AnalysisStep step)
        {
            AnalysisGraphNode node  = new AnalysisGraphNode();
            node.CurrentStep        = step;
            node.Method             = m_methodMap[step];
            return node;
        }
            
        public void BuildAnalysisGraph(AnalysisConfig config)
        {            
            AnalysisGraph graph  = new AnalysisGraph();
            bool storeMSfeatures = config.Analysis.Options.FeatureFindingOptions.StoreMSFeatureResults;

            /// Create a feature database
            if (config.ShouldCreateFeatureDatabaseOnly)
            {                
                graph.AddNode(CreateNode(AnalysisStep.FindFeatures));
                if (storeMSfeatures)
                {
                    graph.AddNode(CreateNode(AnalysisStep.LoadMSMSScanData));
                    graph.AddNode(CreateNode(AnalysisStep.Traceback));
                    graph.AddNode(CreateNode(AnalysisStep.SpectralClustering));
                }
            }
            else if (config.ShouldTraceback)
            {
                config.Analysis.Options.FeatureFindingOptions.StoreMSFeatureResults = true;
                if (!config.ShouldUseExistingDatabase)
                {
                    graph.AddNode(CreateNode(AnalysisStep.FindFeatures));
                }
                graph.AddNode(CreateNode(AnalysisStep.LoadMSMSScanData));
                graph.AddNode(CreateNode(AnalysisStep.Traceback));
            }
            else if (config.ShouldClusterSpectra)
            {
                if (!config.ShouldUseExistingDatabase)
                {
                    graph.AddNode(CreateNode(AnalysisStep.FindFeatures));
                }
                graph.AddNode(CreateNode(AnalysisStep.LoadMSMSScanData));
                graph.AddNode(CreateNode(AnalysisStep.Traceback));
                graph.AddNode(CreateNode(AnalysisStep.SpectralClustering));
            }
            else
            {
                if (config.ShouldLoadMTDB)
                {
                    AnalysisGraphNode node = new AnalysisGraphNode();
                    node.CurrentStep = AnalysisStep.LoadMTDB;

                    if (config.InitialStep == AnalysisStep.FindFeatures)
                    {
                        node.Method = new DelegateAnalysisMethod(CreateMTDB);
                    }
                    else
                    {
                        node.Method = new DelegateAnalysisMethod(LoadMTDB);
                    }
                    graph.AddNode(node);                    
                }
                

                List<AnalysisStep> steps    = m_methodMap.Keys.ToList();
                steps.Sort();

                foreach (AnalysisStep step in steps)
                {
                    if (step >= config.InitialStep)
                    {
                        graph.AddNode(CreateNode(step));
                    }
                }
                graph.Nodes.Sort(delegate(AnalysisGraphNode x, AnalysisGraphNode y)
                {
                    return x.CurrentStep.CompareTo(y.CurrentStep);
                });
            }

            config.AnalysisGraph = graph;
        }
        #endregion


        #region Loading Data
        /// <summary>
        /// Load the data from the dataset information objects to the cache at the analysis Path
        /// </summary>
        /// <param name="datasets">Datasets to load.</param>
        /// <param name="options">Options to use for UMC finding if required.</param>
        /// <param name="analysisPath">Path to save data to.</param>
        private void LoadData(AnalysisConfig config)
        {

            UMCLoaderFactory.Status += new EventHandler<UMCLoadingEventArgs>(UMCLoaderFactory_Status);

            UpdateStatus("Loading data files.");            
            List<DatasetInformation> datasets           = config.Analysis.MetaData.Datasets; 
            LCMSFeatureFindingOptions options           = config.Analysis.Options.FeatureFindingOptions;            
            IUmcDAO featureCache                        = config.Analysis.DataProviders.FeatureCache;
            IMSFeatureDAO msFeatureCache                = config.Analysis.DataProviders.MSFeatureCache;            
            ISpectraProvider rawReader                  = null;
            IMSnFeatureDAO   msnCache                   = config.Analysis.DataProviders.MSnFeatureCache;
            IMsnFeatureToMSFeatureDAO  msnToMsCache     = config.DataProviders.MSFeatureToMSnFeatureCache;
            FeatureFilterOptions filterOptions          = config.Analysis.Options.FeatureFilterOptions;
            string analysisPath                         = Path.Combine(config.Analysis.MetaData.AnalysisPath,
                                                                            config.Analysis.MetaData.AnalysisName);

            DatasetInformation baselineDataset = config.Analysis.MetaData.BaselineDataset;

            foreach (DatasetInformation dataset in datasets)
            {
                
                LoadDataset(
                            dataset,
                            options, 
                            filterOptions,
                            featureCache, 
                            msFeatureCache, 
                            rawReader, 
                            msnCache,
                            msnToMsCache);    
                                         
            }
            UMCLoaderFactory.Status -= UMCLoaderFactory_Status;
        }
        /// <summary>
        /// Load a single dataset from the provider.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="options"></param>
        /// <param name="filterOptions"></param>
        /// <param name="featureCache"></param>
        /// <param name="msFeatureCache"></param>
        /// <param name="rawReader"></param>
        /// <param name="msnCache"></param>
        /// <param name="msnToMsCache"></param>
        /// <returns></returns>
        private List<UMCLight> LoadDataset(   
                                    DatasetInformation          dataset,
                                    LCMSFeatureFindingOptions   options,
                                    FeatureFilterOptions        filterOptions,
                                    IUmcDAO                     featureCache, 
                                    IMSFeatureDAO               msFeatureCache, 
                                    ISpectraProvider            rawReader,
                                    IMSnFeatureDAO              msnCache,
                                    IMsnFeatureToMSFeatureDAO   msnToMsCache)
        {
            
            UpdateStatus("Loading dataset " + dataset.DatasetName + ".");
            int datasetID           = dataset.DatasetId;
            List<UMCLight> features = UMCLoaderFactory.LoadUmcFeatureData(dataset,
                                                                                    featureCache);

            List<MSFeatureLight> msFeatures = UMCLoaderFactory.LoadMsFeatureData(dataset,
                                                                                 msFeatureCache);

            List<ScanSummary> scans = UMCLoaderFactory.LoadScansData(dataset);

            List<MSSpectra> msnSpectra = UMCLoaderFactory.LoadMsnSpectra(dataset,
                                                                                        msnCache);

            // If we don't have any features, then we have to create some from the MS features
            // provided to us.
            if (features.Count < 1)
            {
                features = CreateLCMSFeatures(msFeatures,
                                                msFeatureCache,
                                                options,
                                                filterOptions);


                double maxScan = Convert.ToDouble(features.Max(delegate(UMCLight feature)
                {
                    return feature.Scan;
                }
                ));
                double minScan = Convert.ToDouble(features.Min(delegate(UMCLight feature)
                {
                    return feature.Scan;
                }
                )); 

                UpdateStatus("Adding features to cache database.");

                foreach (UMCLight feature in features)
                {
                    feature.GroupID         = datasetID;
                    feature.RetentionTime   = (Convert.ToDouble(feature.Scan) - minScan)/(maxScan - minScan);  
                }

                featureCache.AddAll(features);

                // Stores the MS Feature Results
                if (options.StoreMSFeatureResults)
                {
                    // Discard older features.  We only want the ones we are interested in.
                    msFeatures.Clear();

                    // Store any MS Features that we need to 
                    msFeatures = new List<MSFeatureLight>();
                    foreach (UMCLight feature in features)
                        foreach (MSFeatureLight msFeature in feature.MSFeatures)
                            if (msFeature != null)
                            {
                                msFeature.GroupID = datasetID;
                                msFeatures.Add(msFeature);
                            }

                    if (msFeatures.Count > 0)
                    {
                        UpdateStatus(string.Format("Storing {0} MS Features used for feature definition", msFeatures.Count));
                        msFeatureCache.AddAll(msFeatures);
                    }
                }
            }
            else
            {
                // Otherwise, we need to map the MS features to the LCMS Features provided.
                // This would mean that we extracted data from an existing database.
                if (msFeatures.Count > 0)
                {
                    // Here we just map the feature to its parent.
                    Dictionary<int, UMCLight> map = FeatureDataConverters.MapFeature<UMCLight>(features);
                    foreach (MSFeatureLight feature in msFeatures)
                    {
                        bool doesFeatureExists = map.ContainsKey(feature.UMCID);
                        if (doesFeatureExists)
                        {
                            map[feature.UMCID].AddChildFeature(feature);
                        }
                    }
                }
            }

            // Map the MS/MS spectra if we can.
            if (msFeatures.Count > 0)
            {
                List<MSSpectra> spectra = UMCLoaderFactory.LoadMsnSpectra(dataset,
                                                                          msnCache);

                // Load the raw data.
                Dictionary<int, int> mapped = new Dictionary<int, int>();
                if (spectra.Count < 1)
                {
                    // this is to help us only load MSn spectra, because we already know
                    // that the MS features are MS1 scans.  So we dont want to load the scan info.
                    // it's probably a slight increase in resolving power, but warranted.
                    Dictionary<int, int> excludeMap = new Dictionary<int, int>();
                    foreach (MSFeatureLight feature in msFeatures)
                    {
                        bool containsScan = excludeMap.ContainsKey(feature.Scan);
                        if (!containsScan)
                        {
                            excludeMap.Add(feature.Scan, feature.ID);
                        }
                    }

                    UpdateStatus(string.Format("Loading MS/MS Raw data from {0}", dataset.DatasetName));
                    spectra = UMCLoaderFactory.LoadRawData(dataset,
                                                            rawReader,
                                                            excludeMap);

                    if (spectra.Count > 1)
                    {

                        UpdateStatus("Linking MS/MS data to MS Features");
                        mapped = LinkMSFeaturesToMSMS(msFeatures, spectra);
                    }
                }

                if (spectra.Count > 0)
                {
                    List<MSFeatureToMSnFeatureMap> matches = new List<MSFeatureToMSnFeatureMap>();
                    int totalMSFeatures = 0;
                    int totalUMCFeatures = 0;

                    // Next we may want to map our MSn features to our parents.  This would allow us to do traceback...
                    foreach (UMCLight feature in features)
                    {
                        totalUMCFeatures++;
                        foreach (MSFeatureLight msFeature in feature.MSFeatures)
                        {
                            if (msFeature.MSnSpectra.Count > 0)
                            {
                                totalMSFeatures++;
                                foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                                {
                                    MSFeatureToMSnFeatureMap match = new MSFeatureToMSnFeatureMap();
                                    match.RawDatasetID = datasetID;
                                    match.MSDatasetID = datasetID;
                                    match.MSFeatureID = msFeature.ID;
                                    match.MSMSFeatureID = spectrum.ID;
                                    match.LCMSFeatureID = feature.ID;
                                    matches.Add(match);
                                }
                            }
                        }
                    }

                    if (matches.Count > 0)
                    {
                        UpdateStatus(
                            string.Format("Mapped {0} MSn spectra to {1} MS features and {2} LC-MS features",
                                            mapped.Keys.Count,
                                            totalMSFeatures,
                                            totalUMCFeatures
                                            ));
                        msnToMsCache.AddAll(matches);
                    }
                }

            }

            // This dataset is done!                               
            if (FeaturesLoaded != null)
            {
                FeaturesLoadedEventArgs args = new FeaturesLoadedEventArgs(dataset, features);
                FeaturesLoaded(this, args);
            }

            return features;
        }
        /// <summary>
        /// Creates LCMS Features from the MS feature data provided.
        /// </summary>
        /// <param name="msFeatures"></param>
        /// <param name="msFeatureCache"></param>
        /// <param name="options"></param>
        /// <param name="filterOptions"></param>
        /// <returns></returns>
        private List<UMCLight> CreateLCMSFeatures(List<MSFeatureLight> msFeatures, 
                                        IMSFeatureDAO msFeatureCache, 
                                        LCMSFeatureFindingOptions options, 
                                        FeatureFilterOptions filterOptions)
        {
            List<UMCLight> features                 = new List<UMCLight>();
            List<UMCLight> filteredFeatures         = new List<UMCLight>();
            List<MSFeatureLight> filteredMsFeatures = new List<MSFeatureLight>();

            // Make features
            if (msFeatures.Count < 1)
            {
                throw new Exception("No features were found in the feature files provided.");
            }

            // Filter out bad MS Features
            filteredMsFeatures = LCMSFeatureFilters.FilterMSFeatures(msFeatures, options);

            // Find LCMS Features
            IFeatureFinder finder   = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.DeconToolsCSV);
            features                = finder.FindFeatures(filteredMsFeatures, options);

            UpdateStatus("Filtering features.");            
            filteredFeatures = LCMSFeatureFilters.FilterFeatures(features,
                                                                 filterOptions);

            foreach (UMCLight feature in filteredFeatures)
            {
                foreach (MSFeatureLight msFeature in feature.MSFeatures)
                {
                    msFeature.UMCID = feature.ID;
                }
            }

            UpdateStatus(string.Format("Filtered features from: {0} to {1}.", features.Count, filteredFeatures.Count));            
            return filteredFeatures;
        }

            
        void UMCLoaderFactory_Status(object sender, UMCLoadingEventArgs e)
        {
            UpdateStatus(e.Message);
        }
        #endregion

        #region Traceback
        /// <summary>
        /// Extract MS/MS from the ?
        /// </summary>
        /// <param name="providers"></param>
        /// <param name="datasets"></param>
        public void ExtractMSMS(FeatureDataAccessProviders providers,
                                List<DatasetInformation> datasets)
        {
            MsmsExtractor extractor = new MsmsExtractor();
            extractor.Progress += new EventHandler<ProgressNotifierArgs>(extractor_Progress);
            FeaturesExtractedEventArgs args = extractor.ExtractMSMS(providers, datasets);
            
            if (FeaturesExtracted != null)
            {
                FeaturesExtracted(this, args);
            }
        }

        void extractor_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);
        }
        /// <summary>
        /// Links MSMS data to MS Features.
        /// </summary>
        /// <param name="providers"></param>
        private Dictionary<int, int> LinkMSFeaturesToMSMS(List<MSFeatureLight> features, List<MSSpectra> msSpectra)            
        {                            
            if (msSpectra.Count <= 0)
            {
                UpdateStatus("No MS/MS data exists.");
                return new Dictionary<int, int>();
            }
                                
            if (features.Count <= 0)
            {
                UpdateStatus(string.Format("No features exists to link to your raw data to MS/MS data."));
                return new Dictionary<int,int>();
            }

            UpdateStatus(string.Format("Linking {0} MS features to {1} MSMS features.",
                        features.Count,
                        msSpectra.Count));

            IMSnLinker linker       = MSnLinkerFactory.CreateLinker(MSnLinkerType.BoxMethod);
            linker.Tolerances       = new FeatureTolerances();
            linker.Tolerances.Mass  = m_config.Analysis.Options.MSLinkerOptions.MzTolerance;

            Dictionary<int, int> spectraMaps = new Dictionary<int, int>();
            spectraMaps                      = linker.LinkMSFeaturesToMSn(features, msSpectra);

            return spectraMaps;                                         
        }        
        #endregion

   
        #region Spectral Clustering
        private void PerformSpectralClustering(AnalysisConfig config)
        {            
            UpdateStatus("Creating Spectral Cluster Mini-Database.");
            MSMSSpectralClusterer processor = new MSMSSpectralClusterer();
            processor.Progress += new EventHandler<ProgressNotifierArgs>(processor_Progress);
            processor.ClusterMSMSSpectra(config.Analysis);
            processor.Progress -= processor_Progress;
            UpdateStatus("Spectral Cluster Mini-Database completed.");            
        }
        #endregion

        #region Alignment
        /// <summary>
        /// Aligns all of the datasets.
        /// </summary>
        public void PerformAlignment(AnalysisConfig config)
        {
            // Connect to database of features.                
            IUmcDAO featureCache = config.Analysis.DataProviders.FeatureCache;

            // Load the baseline data.
            List<UMCLight> baselineFeatures = null;            
            DatasetInformation baselineInfo;
            baselineFeatures                = LoadBaselineData(config, featureCache, baselineFeatures, out baselineInfo);

            // Create the alignment cache and clear it.
            MultiAlignCore.IO.Features.Hibernate.AlignmentDAOHibernate alignmentCache = new IO.Features.Hibernate.AlignmentDAOHibernate();
            alignmentCache.ClearAll();

            // Align pairwise and cache results intermediately.
            IFeatureAligner aligner = m_algorithms.Aligner;
            RegisterProgressNotifier(aligner as IProgressNotifer);

            foreach(DatasetInformation datasetInfo in config.Analysis.MetaData.Datasets)
            {
                if (!datasetInfo.IsBaseline)
                {
                    UpdateStatus("Retrieving data from " + datasetInfo.DatasetName + " for alignment.");
                    List<UMCLight> features             = featureCache.FindByDatasetId(datasetInfo.DatasetId);
                    classAlignmentData alignmentData    = null;
                    
                    if (baselineInfo != null)
                    {
                        UpdateStatus("Aligning " + datasetInfo.DatasetName + " to baseline.");
                        alignmentData = aligner.AlignFeatures(baselineFeatures,
                                                            features,
                                                            config.Analysis.Options.AlignmentOptions);


                    }
                    else
                    {
                        UpdateStatus("Aligning " + datasetInfo.DatasetName + " to mass tag database.");
                        alignmentData = aligner.AlignFeatures(config.Analysis.MassTagDatabase,
                                                            features,
                                                            config.Analysis.Options.AlignmentOptions,
                                                            false);
                    }

                    KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>> pair
                        = new KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>>();
                    if (config.Analysis.Options.DriftTimeAlignmentOptions.ShouldAlignDriftTimes)
                    {
                        // Drift time alignment.

                        DriftTimeAligner driftTimeAligner = new DriftTimeAligner();
                        RegisterProgressNotifier(driftTimeAligner);
                        pair = CorrectDriftTimes(config, baselineFeatures, features, driftTimeAligner, pair);
                        DeRegisterProgressNotifier(driftTimeAligner);
                    }

                    alignmentData.aligneeDataset    = datasetInfo.DatasetName;                    
                    alignmentData.DatasetID         = datasetInfo.DatasetId;
                    config.Analysis.AlignmentData.Add(alignmentData);
                    alignmentCache.Add(alignmentData);

                    UpdateStatus("Updating cache with aligned features.");
                    featureCache.UpdateAll(features);

                    if (FeaturesAligned != null)
                    {

                        DriftTimeAlignmentResults<UMC, UMC> alignedResults = null;
                        DriftTimeAlignmentResults<UMC, UMC> offsetResults = null;
                        if (config.Analysis.Options.DriftTimeAlignmentOptions.ShouldAlignDriftTimes)
                        {
                            alignedResults = pair.Key;
                            offsetResults = pair.Value;
                        }

                        FeaturesAlignedEventArgs args = new FeaturesAlignedEventArgs(baselineInfo,
                                                                                        datasetInfo,
                                                                                        alignmentData,
                                                                                        alignedResults,
                                                                                        offsetResults);
                        FeaturesAligned(this, args);
                    }
                    features.Clear();
                    features = null;
                }
                else
                {
                    config.Analysis.AlignmentData.Add(null);
                }
            }
            DeRegisterProgressNotifier(aligner as IProgressNotifer);
        }

        private KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>> CorrectDriftTimes(AnalysisConfig config, List<UMCLight> baselineFeatures, List<UMCLight> features, DriftTimeAligner driftTimeAligner, KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>> pair)
        {
            // Make sure that if the database does not have drift time, that we don't
            // perform drift time alignment.
            if (config.Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB)
            {
                // Make sure that we have drift time to align to.
                if (config.Analysis.MassTagDatabase.DoesContainDriftTime)
                {
                    UpdateStatus("Aligning Drift Time to database.");
                    pair = driftTimeAligner.AlignDriftTimes(features,
                                                            baselineFeatures,
                                                            config.Analysis.Options.DriftTimeAlignmentOptions);
                }
                else
                {
                    UpdateStatus("Skipping drift time alignment since the database does not contain drift time.");
                }
            }
            else
            {
                UpdateStatus("Aligning Drift Time to baseline.");
                pair = driftTimeAligner.AlignDriftTimes(features,
                                                        baselineFeatures,
                                                        config.Analysis.Options.DriftTimeAlignmentOptions);
            }
            return pair;
        }
        /// <summary>
        /// Loads baseline data for alignment.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="featureCache"></param>
        /// <param name="baselineFeatures"></param>
        /// <param name="baselineDatasetID"></param>
        /// <param name="baselineInfo"></param>
        /// <returns></returns>
        private List<UMCLight> LoadBaselineData(AnalysisConfig config, IUmcDAO featureCache, List<UMCLight> baselineFeatures, out int baselineDatasetID, out DatasetInformation baselineInfo)
        {
            baselineDatasetID = -1;
            baselineInfo = config.Analysis.MetaData.BaselineDataset;

            if (!config.Analysis.Options.UseMassTagDBAsBaseline)
            {
                if (baselineInfo == null)
                {
                    throw new Exception("The baseline dataset was never set.");
                }
                UpdateStatus("Loading baseline features from " + baselineInfo.DatasetName + " for alignment.");
                baselineFeatures = featureCache.FindByDatasetId(baselineInfo.DatasetId);
                baselineDatasetID = baselineInfo.DatasetId;
            }

            // This says that the user 
            if (baselineInfo == null && config.Analysis.Options.DriftTimeAlignmentOptions.ShouldAlignDriftTimes)
            {
                UpdateStatus("Setting baseline features for post drift time alignment.");
                baselineFeatures = new List<UMCLight>();
                // Convert the mass tags to features.                
                foreach (MassTagLight tag in config.Analysis.MassTagDatabase.MassTags)
                {
                    //UMCLight umc = new UMCLight();
                    //umc.ChargeRepresentative = 0;
                    //umc.Net = tag.NET;
                    //umc.MassCalibrated = tag.MassMonoisotopic;
                    //umc.DriftTime = tag.DriftTime;
                    //umc.Id = tag.ID;
                    //umc.ChargeRepresentative = Convert.ToInt16(tag.ChargeState);
                    //baselineFeatures.Add(umc);

                    UMCLight umc = new UMCLight();
                    umc.ChargeState = 0;
                    umc.NET = tag.NET;
                    umc.MassMonoisotopicAligned = tag.MassMonoisotopic;
                    umc.DriftTime = tag.DriftTime;
                    umc.ID = tag.ID;
                    umc.ChargeState = tag.ChargeState;
                    baselineFeatures.Add(umc);
                }
            }
            return baselineFeatures;
        }
        #endregion

        #region Clustering
        /// <summary>
        /// Performs clustering using the mammoth framework.
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="clusterer"></param>
        /// <returns></returns>
        public void PerformLCMSFeatureClustering(AnalysisConfig config)
        {
            MultiAlignAnalysis analysis                     = config.Analysis;
            IClusterer<UMCLight, UMCClusterLight> clusterer = m_algorithms.Clusterer;

            UpdateStatus("Using Cluster Algorithm: " + clusterer.ToString());

            // Tolerances
            FeatureTolerances tolerances = new FeatureTolerances();
            FeatureClusterParameters<UMCLight> parameters = new FeatureClusterParameters<UMCLight>();
            clusterer.Parameters = Clustering.LCMSFeatureClusteringOptions.ConvertToOmics(analysis.Options.ClusterOptions);

            // This just tells us whether we are using mammoth memory partitions or not.          
            string databaseName = Path.Combine(analysis.MetaData.AnalysisPath, analysis.MetaData.AnalysisName);
            int maxChargeState = 15;
            int minChargeState = 1;

            // Connect to the database.
            using (MammothDatabase database = new MammothDatabase(databaseName))
            {
                database.Connect();
                MammothDatabaseRange range = new MammothDatabaseRange(double.MinValue,
                                                                        double.MaxValue,
                                                                        double.MinValue,
                                                                        double.MaxValue,
                                                                        double.MinValue,
                                                                        double.MaxValue);
                List<int> chargeStatesToCluster = new List<int>();
                if (analysis.Options.ClusterOptions.IgnoreCharge)
                {
                    chargeStatesToCluster.Add(-1);
                }
                else
                {
                    for (int i = minChargeState; i <= maxChargeState; i++)
                    {
                        chargeStatesToCluster.Add(i);
                    }
                }

                int clusterCountID = 0;
                foreach (int chargeState in chargeStatesToCluster)
                {
                    UpdateStatus("Retrieving features for clustering from cache.");

                    range.SingleChargeState = chargeState;
                    List<UMCLight> features = database.GetNonClusteredFeatures(range);

                    // Determine the number of features to cluster.
                    int featureCount = 0;
                    if (features != null)
                    {
                        featureCount = features.Count;
                    }

                    if (chargeState < 0)
                    {
                        UpdateStatus(string.Format("Clustering all charge states.  Total Features = {0}.", featureCount));
                    }
                    else
                    {
                        UpdateStatus(string.Format("Clustering Charge State = {0}.  Total features = {1}",
                                                    chargeState,
                                                    featureCount));
                    }

                    List<UMCClusterLight> clusters = new List<UMCClusterLight>();
                    clusters = clusterer.Cluster(features, clusters);

                    UpdateStatus(string.Format("Found {0} clusters.", clusters.Count));
                    UpdateStatus("Updating cluster id numbers.");
                    foreach (UMCClusterLight cluster in clusters)
                    {
                        cluster.ID = clusterCountID++;
                    }

                    UpdateStatus("Updating feature cache with cluster id's.");
                    database.UpdateFeaturesAndClusters(features);

                    if (FeaturesClustered != null)
                    {
                        FeaturesClustered(this, new FeaturesClusteredEventArgs(clusters));
                    }

                    UpdateStatus("Finished clustering charge state.");
                }
            }
        }        
        #endregion

        #region Identification
        /// <summary>
        /// Performs peak matching with loaded clusters. 
        /// </summary>
        public void PerformPeakMatching(AnalysisConfig config)
        {
            bool shouldPeakMatch = true;
            if (shouldPeakMatch)
            {
                if (m_config.Analysis.MassTagDatabase != null)
                {
                    List<UMCClusterLight> clusters              = m_config.Analysis.DataProviders.ClusterCache.FindAll();
                    IPeakMatcher<UMCClusterLight> peakMatcher   = m_algorithms.PeakMatcher;
                                                           
                    STACAdapter<UMCClusterLight> adapter = peakMatcher as STACAdapter<UMCClusterLight>;
                    if (adapter != null)
                    {
                        if (adapter.Options.UseDriftTime)
                        {
                            UpdateStatus("Using drift time.");
                        }
                        else
                        {
                            UpdateStatus("Ignoring drift time.");
                        }
                    }
                    else
                    {
                        TraditionalPeakMatcher<UMCClusterLight> traditional = peakMatcher as TraditionalPeakMatcher<UMCClusterLight>;
                        if (traditional != null && !m_config.Analysis.MassTagDatabase.DoesContainDriftTime)
                        {
                            if (!m_config.Analysis.MassTagDatabase.DoesContainDriftTime)
                            {
                                UpdateStatus("The database does not contain drift time.  Ensuring drift time tolerances are disabled.");
                                traditional.Options.DriftTimeTolerance = 1000;
                            }
                        }
                    }

                    UpdateStatus("Performing Peak Matching");
                    PeakMatchingResults<UMCClusterLight, MassTagLight> matchResults = new PeakMatchingResults<UMCClusterLight, MassTagLight>();
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

                    UpdateStatus("Updating database with peak matched results.");
                    PeakMatchResultsWriter writer = new PeakMatchResultsWriter();
                    int matchedMassTags = 0;
                    int matchedProteins = 0;
                    writer.WritePeakMatchResults(matchResults,
                                                 m_config.Analysis.MassTagDatabase,
                                                 out matchedMassTags,
                                                 out matchedProteins);

                    UpdateStatus(string.Format("Found {0} mass tag matches. Matching to {1} potential proteins.",
                                                                                                matchedMassTags,
                                                                                                matchedProteins));

                    if (m_config.Analysis.Options.STACOptions.WriteResultsBackToMTS && m_config.Analysis.MetaData.JobID != -1)
                    {
                        string databasePath = "";
                        MTSPeakMatchResultsWriter mtsWriter = new MTSSqlServerPeakMatchResultWriter();
                        mtsWriter.WriteClusters(m_config.Analysis,
                                                clusters,
                                                databasePath);
                    }
                    else if (m_config.Analysis.Options.STACOptions.WriteResultsBackToMTS)
                    {
                        UpdateStatus("Cannot write mass tag results back to database.  The Job ID was not specified for this analysis.");
                    }
                }
            }
        }
        #endregion

        #region MSMS Alignment
        void processor_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);
        }
        #endregion

        private void ExtractSICS(AnalysisConfig config)
        {
            SICExtractor extractor = new SICExtractor();
            UpdateStatus("Building SIC's");
            extractor.Progress += new EventHandler<ProgressNotifierArgs>(extractor_Progress);
            extractor.ExtractUMCSICs(this.AnalaysisPath, m_config.Analysis);
            UpdateStatus(string.Format("Analysis {0} Completed.", m_config.Analysis.MetaData.AnalysisName));
        }
        
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


            ThreadStart threadStart  = new ThreadStart(PerformAnalysis);
            m_analysisThread         = new Thread(threadStart);
            m_analysisThread.Start();
        }
        /// <summary>
        /// Aborts the analysis thread.
        /// </summary>
        public void StopAnalysis()
        {
            AbortAnalysisThread(m_analysisThread);
        }
        private void LoadMTDB(AnalysisConfig config)
        {
            UpdateStatus("Loading the database from the SQLite result database.");

            MassTagDatabase database = new MassTagDatabase();

            UpdateStatus("Loading all of the mass tags.");
            // Get all of the mass tags
            List<MassTagLight> massTags = config.Analysis.DataProviders.MassTags.FindAll();


            UpdateStatus("Loading all of the tag to protein references.");
            // Then get all of the mass tag to protein maps
            IGenericDAO<MassTagToProteinMap> tagToProteinMapCache = new MultiAlignCore.IO.Features.Hibernate.GenericDAOHibernate<MassTagToProteinMap>();
            List<MassTagToProteinMap> maps =  tagToProteinMapCache.FindAll();

            // Then get all of the proteins
            UpdateStatus("Loading all of the protein data.");
            IProteinDAO proteinCache = new MultiAlignCore.IO.Features.Hibernate.ProteinDAO();
            List<Protein> proteins = proteinCache.FindAll();

            UpdateStatus("Indexing the protein data for faster assembly.");
            Dictionary<int, Protein> proteinMap = new Dictionary<int, Protein>();
            foreach (Protein p in proteins)
            {
                if (!proteinMap.ContainsKey(p.ProteinID))
                {
                    proteinMap.Add(p.ProteinID, p);
                }
            }

            UpdateStatus("Re-mapping the proteins to the mass tags.");
            Dictionary<int, List<Protein>> massTagProteinMap = new Dictionary<int, List<Protein>>();

            // Then map them.
            foreach (MassTagLight tag in massTags)
            {
                int id = tag.ID;
                if (!massTagProteinMap.ContainsKey(id))
                {
                    massTagProteinMap.Add(id, new List<Protein>());
                }

                List<MassTagToProteinMap> matchedMaps = maps.FindAll(delegate (MassTagToProteinMap map)
                {
                    return map.MassTagId == id;
                });

                List<Protein> newProteins = new List<Protein>();
                foreach (MassTagToProteinMap mtMap in matchedMaps)
                {
                    newProteins.Add(proteinMap[mtMap.ProteinId]);
                }
                massTagProteinMap[id].AddRange(newProteins);
            }

            UpdateStatus("Building the in memory mass tag database.");
            database.AddMassTagsAndProteins(massTags, massTagProteinMap);

            
            int totalMassTags = database.MassTags.Count;
            UpdateStatus("Loaded " + totalMassTags.ToString() + " mass tags.");            
            config.Analysis.MassTagDatabase = database;
            
            if (MassTagsLoaded != null)
            {
                MassTagsLoaded(this, new MassTagsLoadedEventArgs(massTags));
            }
        }
        /// <summary>
        /// Creates an entry in the DB if a new database should be created.
        /// </summary>
        /// <param name="config"></param>
        private void CreateMTDB(AnalysisConfig config)
        {
            MassTagDatabase database = null;

            // Load the mass tag database if we are aligning, or if we are 
            // peak matching (but aligning to a reference dataset.
            if (m_config.Analysis.Options.UseMassTagDBAsBaseline)
            {
                UpdateStatus("Loading Mass Tag database from database:  " + m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseName);

                database = MTDBLoaderFactory.LoadMassTagDB(m_config.Analysis.Options.MassTagDatabaseOptions,
                                                            m_config.Analysis.MetaData.AnalysisSetupInfo.Database.DatabaseFormat);
            }
            else if (m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseType != MassTagDatabaseType.None)
            {
                UpdateStatus("Loading Mass Tag database from database:  " + m_config.Analysis.Options.MassTagDatabaseOptions.DatabaseName);
                database = MTDBLoaderFactory.LoadMassTagDB(m_config.Analysis.Options.MassTagDatabaseOptions,
                                                            m_config.Analysis.MetaData.AnalysisSetupInfo.Database.DatabaseFormat);
            }
            else
            {
                config.Analysis.MassTagDatabase = null;
                return;
            }
            

            if (database != null)
            {
                int totalMassTags = database.MassTags.Count;
                UpdateStatus("Loaded " + totalMassTags.ToString() + " mass tags.");
            }

            config.Analysis.MassTagDatabase = database;

            //TODO: Re-add the protein maps too.
            config.Analysis.DataProviders.MassTags.AddAll(database.MassTags);

            IProteinDAO proteinCache = new MultiAlignCore.IO.Features.Hibernate.ProteinDAO();          
            proteinCache.AddAll(database.AllProteins);

            List<MassTagToProteinMap> map = new List<MassTagToProteinMap>();
            foreach (int massTagID in database.Proteins.Keys)
            {
                foreach(Protein p in database.Proteins[massTagID])
                {
                    
                    MassTagToProteinMap tempMap = new MassTagToProteinMap();
                    tempMap.ProteinId = p.RefID;
                    tempMap.MassTagId = massTagID;
                    map.Add(tempMap);
                }
            }
            IGenericDAO<MassTagToProteinMap> tempCache = new MultiAlignCore.IO.Features.Hibernate.GenericDAOHibernate<MassTagToProteinMap>();
            tempCache.AddAll(map);

            if (MassTagsLoaded != null)
            {
                MassTagsLoaded(this, new MassTagsLoadedEventArgs(database.MassTags));
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

                AnalysisGraph graph = m_config.AnalysisGraph;
                foreach (AnalysisGraphNode node in graph.Nodes)
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
