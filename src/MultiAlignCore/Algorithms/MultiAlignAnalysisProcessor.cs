using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Algorithms.MSLinker;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.Mammoth;
using MultiAlignCore.IO.MTDB;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms.FeatureMatcher.MSnLinker;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using PNNLOmics.IO.FileReaders;

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
        public event EventHandler<FeaturesExtractedEventArgs> FeaturesExtracted;
        /// <summary>
        /// Fired when a catastrophic error occurs.
        /// </summary>
        public event EventHandler<AnalysisErrorEventArgs> AnalysisError;
        /// <summary>
        /// Fired when the analysis is complete.
        /// </summary>
        public event EventHandler<AnalysisCompleteEventArgs> AnalysisComplete;
        /// <summary>
        /// Fired when a status update is ready.
        /// </summary>
        public event EventHandler<AnalysisStatusEventArgs> Status;        
        #endregion

        #region Members
        /// <summary>
        /// Algorithms used in processing.
        /// </summary>
        private AlgorithmProvider m_algorithms;
        /// <summary>
        /// Holds all information about how to perform the analysis and how to store results.
        /// </summary>
        private MultiAlignAnalysis m_analysis;
        /// <summary>
        /// Thread in charge of performing the analysis.
        /// </summary>
        private Thread m_analysisThread;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MultiAlignAnalysisProcessor()
        {
            m_analysis   = null;
            m_algorithms = null;
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
                    m_algorithms.Status += new PNNLOmics.Utilities.MessageEventHandler(m_algorithms_Status);
                }
            }
        }
        /// <summary>
        /// Status Messages from the algorithms.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_algorithms_Status(object sender, PNNLOmics.Utilities.MessageEventArgs e)
        {
            
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
        #endregion

        #region Data Loading Methods
        /// <summary>
        /// Loads raw file data.
        /// </summary>
        /// <param name="otherFiles"></param>
        /// <param name="analysisPath"></param>
        /// <param name="dataProviders"></param>
        private void LoadOtherData(List<InputFile> otherFiles,
                                    string analysisPath,
                                    FeatureDataAccessProviders dataProviders)
        {

            Dictionary<string, int> datasetMap = new Dictionary<string, int>();
            Dictionary<int, int> datasetIDMap = new Dictionary<int, int>();
            IRawDataFileReader rawReader = null;
            ScansFileReader scansReader = new ScansFileReader();
            IMSnFeatureDAO msnCache = dataProviders.MSnFeatureCache;

            // Map the dataset ids
            foreach (DatasetInformation information in m_analysis.MetaData.Datasets)
            {
                string datasetName = DatasetInformation.CleanNameDatasetNameOfExtensions(information.DatasetName);
                datasetMap.Add(datasetName, Convert.ToInt32(information.DatasetId));
                datasetIDMap.Add(Convert.ToInt32(information.DatasetId), 0);
            }

            int datasetID = 0;
            foreach (InputFile file in otherFiles)
            {
                string path = file.Path;
                if (path == null)
                    continue;

                string datasetName = DatasetInformation.CleanNameDatasetNameOfExtensions(path);
                datasetName = Path.GetFileNameWithoutExtension(datasetName);
                bool containsKey = datasetMap.ContainsKey(datasetName);
                if (containsKey)
                {
                    datasetID = datasetMap[datasetName];
                }
                else
                {
                    bool hasID = true;
                    datasetID = 0;
                    while (hasID)
                    {
                        datasetID++;
                        hasID = datasetIDMap.ContainsKey(datasetID);
                    }
                    datasetMap.Add(datasetName, datasetID);
                    datasetIDMap.Add(datasetID, 0);
                }

                UpdateStatus("Loading auxillary file " + Path.GetFileName(path) + ".");
                switch (file.FileType)
                {
                    case InputFileType.Scans:
                        List<ScanSummary> scans = scansReader.ReadFile(path).ToList();
                        break;
                    case InputFileType.Raw:
                        rawReader = RawLoaderFactory.CreateFileReader(path);
                        List<MSSpectra> msnSpectra = rawReader.ReadMSMSSpectra(path);
                        int id = 0;
                        foreach (MSSpectra spectra in msnSpectra)
                        {
                            spectra.ID = id++;
                            spectra.GroupID = datasetID;
                        }
                        try
                        {
                            msnCache.AddAll(msnSpectra);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        break;

                }
            }
        }
        /// <summary>
        /// Load the data from the dataset information objects to the cache at the analysis Path
        /// </summary>
        /// <param name="datasets">Datasets to load.</param>
        /// <param name="options">Options to use for UMC finding if required.</param>
        /// <param name="analysisPath">Path to save data to.</param>
        private void LoadDatasetData(List<DatasetInformation> datasets,
                                     UMCFeatureFinderOptions options,
                                     string analysisPath)
        {
            IUmcDAO featureCache = m_analysis.DataProviders.FeatureCache;
            IMSFeatureDAO msFeatureCache = m_analysis.DataProviders.MSFeatureCache;
            IGenericDAO<MSFeatureToLCMSFeatureMap> map = m_analysis.DataProviders.MSFeatureToLCMSFeatureCache;

            foreach (DatasetInformation dataset in datasets)
            {
                UpdateStatus("Loading dataset " + dataset.DatasetName + ".");
                List<clsUMC> features = UMCLoaderFactory.LoadData(dataset,
                                                                    featureCache,
                                                                    msFeatureCache,
                                                                    map,
                                                                    options);

                UpdateStatus(string.Format("Filtering {0} features found.", features.Count));
                int preFiltered = features.Count;
                // refactor to inside of loader factory.
                features = MultiAlignCore.Data.Features.LCMSFeatureFilters.FilterFeatures(features, m_analysis.Options.FeatureFilterOptions);
                UpdateStatus(string.Format("Filtered features from: {0} to {1}.", preFiltered, features.Count));
                UpdateStatus("Adding features to cache database.");
                featureCache.AddAll(features);
                if (FeaturesLoaded != null)
                {
                    FeaturesLoadedEventArgs args = new FeaturesLoadedEventArgs(dataset, features);
                    FeaturesLoaded(this, args);
                }
                features.Clear();
                features = null;
            }
        }          
        #endregion

        #region Analysis Methods
        /// <summary>
        /// Links MSMS data to MS Features.
        /// </summary>
        /// <param name="providers"></param>
        private void LinkMSFeaturesToMSMS(FeatureDataAccessProviders providers, List<DatasetInformation> information)
        {
            foreach (DatasetInformation info in information)
            {                                    
                int id                          = Convert.ToInt32(info.DatasetId);

                List<MSSpectra> spectra = providers.MSnFeatureCache.FindByDatasetId(id);
                if (spectra.Count <= 0)
                {
                    UpdateStatus("No MS/MS data exists.");
                    continue;
                }

                List<MSFeatureLight> features   = null;
                
                UpdateStatus(string.Format("Loading dataset MS feature list id: {0}", id));
                features = providers.MSFeatureCache.FindByDatasetId(id);
                                    
                UpdateStatus(string.Format("Linking {0} MS features to {1} MSMS features for dataset {2}",
                                        features.Count,                        
                                        spectra.Count,
                                        info.DatasetName
                                        ));
                if (features.Count <= 0)
                {
                    UpdateStatus(string.Format("Not enough features exists to link to your raw data to MS/MS data.")); 
                    continue;
                }

                IMSnLinker linker                   = MSnLinkerFactory.CreateLinker(MSnLinkerType.BoxMethod);
                linker.Tolerances                   = new FeatureTolerances();
                linker.Tolerances.Mass              = m_analysis.Options.MSLinkerOptions.MzTolerance;
                Dictionary<int, int> spectraMaps    = linker.LinkMSFeaturesToMSn(features, spectra);

                List<MSFeatureToMSnFeatureMap> matches = new List<MSFeatureToMSnFeatureMap>();
                foreach (MSFeatureLight feature in features)
                {                    
                    if (feature.MSnSpectra.Count > 0)
                    {
                        foreach(MSSpectra spectrum in feature.MSnSpectra)
                        {
                            MSFeatureToMSnFeatureMap match  = new MSFeatureToMSnFeatureMap();
                            match.RawDatasetID  = id;
                            match.MSDatasetID   = id;
                            match.MSFeatureID   = feature.ID;
                            match.MSMSFeatureID = spectrum.ID;
                            matches.Add(match);
                        }
                    }
                }
                
                UpdateStatus(string.Format("Found {0} matching MS-MSn features.  {1} total spectra were matched.", matches.Count, spectraMaps.Keys.Count));
                
                    providers.MSFeatureToMSnFeatureCache.AddAll(matches);
                
            }
        }        
        /// <summary>
        /// Performs clustering using the mammoth framework.
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="clusterer"></param>
        /// <returns></returns>
        public void PerformClustering(MultiAlignAnalysis                    analysis,
                                      IClusterer<UMCLight, UMCClusterLight> clusterer)
        {
            UpdateStatus("Using Cluster Algorithm: " + clusterer.ToString());

            // Tolerances
            FeatureTolerances tolerances                        = new FeatureTolerances();
            FeatureClusterParameters<UMCLight> parameters       = new FeatureClusterParameters<UMCLight>();
            tolerances.DriftTime                                = analysis.Options.ClusterOptions.DriftTimeTolerance;
            tolerances.Mass                                     = analysis.Options.ClusterOptions.MassTolerance;
            tolerances.RetentionTime                            = analysis.Options.ClusterOptions.NETTolerance;
            parameters.CentroidRepresentation                   = PNNLOmics.Data.Features.ClusterCentroidRepresentation.Mean;
            parameters.Tolerances                               = tolerances;            
            parameters.OnlyClusterSameChargeStates              = (analysis.Options.ClusterOptions.IgnoreCharge == false);
            if (analysis.Options.ClusterOptions.ClusterRepresentativeType == enmClusterRepresentativeType.MEDIAN)
            {
                parameters.CentroidRepresentation = ClusterCentroidRepresentation.Median;
            }

            clusterer.Parameters = parameters;                        

            // This just tells us whether we are using mammoth memory partitions or not.          
            string databaseName = Path.Combine(m_analysis.MetaData.AnalysisPath, m_analysis.MetaData.AnalysisName);
            int maxChargeState = 15;
            int minChargeState = 1;

            

            // Connect to the database.
            using (MammothDatabase database = new MammothDatabase(databaseName))
            {
                database.Connect();
                MammothDatabaseRange range = new MammothDatabaseRange(  double.MinValue,
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

                    range.SingleChargeState         = chargeState;
                    List<UMCLight> features         = database.GetNonClusteredFeatures(range);

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

                    List<UMCClusterLight> clusters  = new List<UMCClusterLight>();
                    clusters                        = clusterer.Cluster(features, clusters);

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
        /// <summary>
        /// Aligns all of the datasets.
        /// </summary>
        public void PerformAlignment(MultiAlignAnalysis analysis)
        {
            // Connect to database of features.                
            IUmcDAO featureCache = m_analysis.DataProviders.FeatureCache;
            
            // Load the baseline data.
            List<clsUMC> baselineFeatures   = null;
            int baselineDatasetID           = -1;
            DatasetInformation baselineInfo = analysis.MetaData.BaselineDataset;

            if (!analysis.Options.UseMassTagDBAsBaseline)
            {
                if (baselineInfo == null)
                {
                    throw new Exception("The baseline dataset was never set.");
                }
                UpdateStatus("Loading baseline features from " + baselineInfo.DatasetName + " for alignment.");
                baselineFeatures    = featureCache.FindByDatasetId(baselineInfo.DatasetId);
                baselineDatasetID   = baselineInfo.DatasetId;
            }

            // This says that the user 
            if (baselineInfo == null && m_analysis.Options.DriftTimeAlignmentOptions.ShouldAlignDriftTimes)
            {
                UpdateStatus("Setting baseline features for post drift time alignment.");
                baselineFeatures = new List<clsUMC>();
                // Convert the mass tags to features.                
                foreach (MassTagLight tag in analysis.MassTagDatabase.MassTags)
                {                    
                    clsUMC umc               = new clsUMC();
                    umc.ChargeRepresentative = 0;
                    umc.Net                  = tag.NET;
                    umc.MassCalibrated       = tag.MassMonoisotopic;
                    umc.DriftTime            = tag.DriftTime;
                    umc.Id                   = tag.ID;
                    umc.ChargeRepresentative = Convert.ToInt16(tag.ChargeState);
                    baselineFeatures.Add(umc);
                }
            }
            
            // Align pairwise and cache results intermediately.
            IFeatureAligner aligner = m_algorithms.Aligner;

            for (int datasetNum = 0; datasetNum < m_analysis.MetaData.Datasets.Count; datasetNum++)
            {
                if (datasetNum != baselineDatasetID)
                {
                    DatasetInformation datasetInfo      = m_analysis.MetaData.Datasets[datasetNum];
                    UpdateStatus("Retrieving data from " + datasetInfo.DatasetName + " for alignment.");
                    List<clsUMC> features               = featureCache.FindByDatasetId(datasetNum);
                    classAlignmentData alignmentData    = null;

                    //// We dont track the UMC Index...this was used previously by the UMC Data object :(
                    //// Here we are applying a temp fix to iterate through the features and assign its unique ID.                    
                    int featureIDIndex = 0;
                    foreach (clsUMC feature in features)
                    {
                        feature.mint_umc_index = featureIDIndex++;
                    }

                    // LCMSWarp alignment.
                    KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>> pair
                        = new KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>>();
                    
                    if (baselineInfo != null)
                    {
                        UpdateStatus("Aligning " + datasetInfo.DatasetName + " to baseline.");
                        alignmentData = aligner.AlignFeatures(baselineFeatures,
                                                            features,
                                                            analysis.Options.DefaultAlignmentOptions);


                    }
                    else
                    {
                        UpdateStatus("Aligning " + datasetInfo.DatasetName + " to mass tag database.");
                        alignmentData = aligner.AlignFeatures(analysis.MassTagDatabase,
                                                            features,
                                                            analysis.Options.DefaultAlignmentOptions,
                                                            false);
                    }

                    // Drift time alignment.
                    if (analysis.Options.DriftTimeAlignmentOptions.ShouldAlignDriftTimes)
                    {
                        pair = AlignDriftTimes(features, baselineFeatures, analysis.Options.DriftTimeAlignmentOptions);
                    }

                    alignmentData.aligneeDataset = datasetInfo.DatasetName;                    
                    analysis.AlignmentData.Add(alignmentData);


                    UpdateStatus("Updating cache with aligned features.");
                    featureCache.UpdateAll(features);

                    if (FeaturesAligned != null)
                    {

                        DriftTimeAlignmentResults<UMC, UMC> alignedResults = null;
                        DriftTimeAlignmentResults<UMC, UMC> offsetResults  = null;
                        if (analysis.Options.DriftTimeAlignmentOptions.ShouldAlignDriftTimes)
                        {
                            alignedResults = pair.Key;                            
                            offsetResults  = pair.Value;
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
                    m_analysis.AlignmentData.Add(null);
                }
            }
        }
        /// <summary>
        /// Correct for the drift times.
        /// </summary>
        /// <param name="features"></param>
        /// <param name="baselineFeatures"></param>
        /// <param name="options"></param>
        private KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>> AlignDriftTimes(List<clsUMC> features, List<clsUMC> baselineFeatures, DriftTimeAlignmentOptions options)
        {
            UpdateStatus("Correcting drift times.");
            List<UMC> baselineUMCs  = new List<UMC>();
            List<UMC> aligneeUMCs   = new List<UMC>();

            UpdateStatus("Mapping data structures.");
            Dictionary<int, clsUMC> featureIDMap = new Dictionary<int, clsUMC>();
            // TODO: Convert the clsUmc's to umcs...
            foreach (clsUMC feature in features)
            {
                UMC umc                     = new UMC();
                umc.MassMonoisotopicAligned = feature.MassCalibrated;
                umc.NETAligned              = feature.Net;
                umc.DriftTime               = Convert.ToSingle(feature.DriftTime);                
                umc.ID                      = feature.Id;
                umc.ChargeState             = feature.ChargeRepresentative;
                aligneeUMCs.Add(umc);
                featureIDMap.Add(feature.Id, feature);
            }
            
            foreach (clsUMC feature in baselineFeatures)
            {
                UMC umc                     = new UMC();
                umc.MassMonoisotopicAligned = feature.MassCalibrated;
                umc.NETAligned              = feature.Net;         
                umc.DriftTime               = Convert.ToSingle(feature.DriftTime);
                umc.ID                      = feature.Id;
                umc.ChargeState             = feature.ChargeRepresentative;       
                baselineUMCs.Add(umc);
            }

            // filter based on charge state.                  
            int chargeMax = options.MaxChargeState;
            int chargeMin = options.MinChargeState;

            UpdateStatus(string.Format("Filtering Features Min Charge: {0} <= charge <= Max Charge {1}",chargeMin, chargeMax));
            var filteredQuery       =       from   feature in aligneeUMCs
                                            where  feature.ChargeState <= chargeMax && feature.ChargeState >= chargeMin
                                            select feature;
            List<UMC> filteredUMCs  = filteredQuery.ToList();

            UpdateStatus("Finding Aligned Matches and correcting drift times.");
            DriftTimeAlignmentResults<UMC, UMC> alignedResults = 
                            DriftTimeAlignment<UMC, UMC>.AlignObservedEnumerable(aligneeUMCs, 
                                                                                filteredUMCs, 
                                                                                baselineUMCs, 
                                                                                options.MassPPMTolerance,
                                                                                options.NETTolerance);


            DriftTimeAlignmentResults<UMC, UMC> offsetResults = null;
            if (options.ShouldPerformOffset)
            {
                UpdateStatus("Adjusting drift time offsets.");
                List<UMC> aligneeData = aligneeUMCs;
                if (!options.ShouldUseAllObservationsForOffsetCalculation)
                {
                    UpdateStatus("Using only filtered matches for offset correction.");
                    aligneeData = filteredUMCs;
                }
                else
                {
                    UpdateStatus("Using all feature matches for offset correction.");
                }
                offsetResults = DriftTimeAlignment<UMC, UMC>.CorrectForOffset(aligneeData, baselineUMCs, options.MassPPMTolerance, options.NETTolerance, options.DriftTimeTolerance);
            }


            UpdateStatus("Remapping data structures for persistence to database.");
            //TODO: Then repopulate the drift times post...
            foreach(UMC umc in aligneeUMCs)
            {
                featureIDMap[umc.ID].DriftTime = umc.DriftTimeAligned;
            }

            KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>> pair =
                            new KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>>(alignedResults,
                                                                                                                       offsetResults);
            return pair;
        }        
        /// <summary>
        /// Performs peak matching with loaded clusters. 
        /// </summary>
        public void PerformPeakMatching()
        {
            bool shouldPeakMatch = true;
            if (shouldPeakMatch)
            {
                if (m_analysis.MassTagDatabase != null)
                {
                    List<clsCluster>                oldClusters     = m_analysis.DataProviders.ClusterCache.FindAll();
                    IPeakMatcher<UMCClusterLight>   peakMatcher     = m_algorithms.PeakMatcher;


                    if (!m_analysis.Options.ClusterOptions.AlignClusters)
                    {
                        UpdateStatus("Adjusting masses and NETs for clusters for peak matching.");

                        // If the clusters are not aligned, then the 
                        // peak matching jobs may fail as they require the data structure to have 
                        // calibrated data.  Talk about coupling!
                        foreach (clsCluster cluster in oldClusters)
                        {
                            cluster.MassCalibrated  = cluster.Mass;
                            cluster.NetAligned      = cluster.Net;
                        }
                    }
                    else
                    {
                        //TODO: Align clusters.
                        UpdateStatus("Adjusting masses and NETs for clusters for peak matching.");
                        // If the clusters are not aligned, then the 
                        // peak matching jobs may fail as they require the data structure to have 
                        // calibrated data.  Talk about coupling!
                        foreach (clsCluster cluster in oldClusters)
                        {
                            cluster.MassCalibrated  = cluster.Mass;
                            cluster.NetAligned      = cluster.Net;
                        }
                        m_algorithms.Aligner.AlignFeatures(m_analysis.MassTagDatabase, oldClusters, m_analysis.Options.DefaultAlignmentOptions);
                        m_analysis.DataProviders.ClusterCache.UpdateAll(oldClusters);
                    }
                    
                    
                    // Temp hack while we wait for hibernate to be fixed to allow for OMICs structures to be used.
                    List<UMCClusterLight> clusters = new List<UMCClusterLight>();
                    foreach (clsCluster oldCluster in oldClusters)
                    {
                        UMCClusterLight cluster     = new UMCClusterLight();
                        cluster.ID                  = oldCluster.Id;
                        cluster.MassMonoisotopic    = oldCluster.MassCalibrated;
                        cluster.NET                 = oldCluster.NetAligned;
                        cluster.DriftTime           = oldCluster.DriftTime;
                        clusters.Add(cluster);
                    }
                   
                    UpdateStatus("Performing Peak Matching");
                    PeakMatchingResults<UMCClusterLight, MassTagLight> matchResults = new PeakMatchingResults<UMCClusterLight, MassTagLight>();
                    matchResults.Matches = peakMatcher.PerformPeakMatching(clusters, m_analysis.MassTagDatabase);

                    // Was STAC performed?
                    STACAdapter<UMCClusterLight> adapter = peakMatcher as STACAdapter<UMCClusterLight>;                    
                    if (adapter != null)
                    {
                        
                        matchResults.FdrTable = adapter.Matcher.STACFDRTable;
                    }
                    m_analysis.MatchResults = matchResults;


                    UpdateStatus("Updating database with peak matched results.");
                    PeakMatchResultsWriter writer   = new PeakMatchResultsWriter();
                    int matchedMassTags             = 0;
                    int matchedProteins             = 0;
                    writer.WritePeakMatchResults(matchResults,
                                                 m_analysis.MassTagDatabase,
                                                 out matchedMassTags, 
                                                 out matchedProteins);

                    UpdateStatus(string.Format("Found {0} mass tag matches. Matching to {1} potential proteins.",
                                                                                                matchedMassTags,
                                                                                                matchedProteins));

                    if (m_analysis.Options.PeakMatchingOptions.WriteResultsBackToMTS && m_analysis.MetaData.JobID != -1)
                    {
                        //string databasePath                 = "";
                        MTSPeakMatchResultsWriter mtsWriter = new MTSSqlServerPeakMatchResultWriter();
                        //mtsWriter.WriteClusters(m_analysis, 
                        //                        clusters,
                        //                        databasePath);
                    }
                    else if (m_analysis.Options.PeakMatchingOptions.WriteResultsBackToMTS)
                    {
                        UpdateStatus("Cannot write mass tag results back to database.  The Job ID was not specified for this analysis.");
                    }
                }
            }
        }
        #endregion

        #region MSMS Alignment
        /// <summary>
        /// Extracts the features from the database that have MS/MS
        /// </summary>
        public Dictionary<int, List<UMCLight>> ExtractUMCWithMSMS(MultiAlignAnalysis analysis)
        {
            UpdateStatus("Producing feature traceback data structures.");

            UpdateStatus("Extracting data from each dataset.");
            FeatureDataAccessProviders providers = analysis.DataProviders;
            Dictionary<int, List<UMCLight>> features = new Dictionary<int, List<UMCLight>>();

            foreach (DatasetInformation dataset in analysis.MetaData.Datasets)
            {
                features.Add(dataset.DatasetId, new List<UMCLight>());

                int datasetID = dataset.DatasetId;
                UpdateStatus(string.Format("Mapping data from dataset {0} with id {1}", dataset.DatasetName, dataset.DatasetId));

                UpdateStatus("Extracting LC-MS to MS Feature Map");
                Dictionary<int, int> msFeatureIDToLCMSFeatureID = new Dictionary<int, int>();
                List<MSFeatureToLCMSFeatureMap> msToLcmsFeatureMaps = providers.MSFeatureToLCMSFeatureCache.FindByDatasetId(datasetID);
                foreach (MSFeatureToLCMSFeatureMap map in msToLcmsFeatureMaps)
                {
                    msFeatureIDToLCMSFeatureID.Add(map.MSFeatureID, map.LCMSFeatureID);
                }

                UpdateStatus("Extracting MS to MSn Feature Map");
                List<MSFeatureToMSnFeatureMap> msnToMSFeatureMaps = providers.MSFeatureToMSnFeatureCache.FindByDatasetId(datasetID);
                Dictionary<int, int> msFeatureIDToMsMsFeatureID = new Dictionary<int, int>();
                foreach (MSFeatureToMSnFeatureMap map in msnToMSFeatureMaps)
                {
                    msFeatureIDToMsMsFeatureID.Add(map.MSFeatureID, map.MSMSFeatureID);
                }

                UpdateStatus("Extracting MSn Features");
                Dictionary<int, MSSpectra> msMsFeatureIDToSpectrum = new Dictionary<int, MSSpectra>();
                List<MSSpectra> msnSpectra = providers.MSnFeatureCache.FindByDatasetId(datasetID);
                foreach (MSSpectra spectrum in msnSpectra)
                {
                    msMsFeatureIDToSpectrum.Add(spectrum.ID, spectrum);
                }

                UpdateStatus("Extracting LC-MS Features");
                List<clsUMC> lcmsFeatures = providers.FeatureCache.FindByDatasetId(datasetID);
                Dictionary<int, UMCLight> lcmsFeatureIDToFeature = new Dictionary<int, UMCLight>();
                foreach (clsUMC umc in lcmsFeatures)
                {
                    UMCLight umcFeature = new UMCLight();
                    umcFeature.Abundance = Convert.ToInt64(umc.AbundanceMax);
                    umcFeature.ID = umc.Id;
                    umcFeature.MassMonoisotopic = umc.MassCalibrated;
                    umcFeature.RetentionTime = umc.Net;
                    umcFeature.Scan = umc.Scan;
                    umcFeature.GroupID = datasetID;
                    lcmsFeatureIDToFeature.Add(umc.Id, umcFeature);
                }

                UpdateStatus("Extracting MS Features");
                List<MSFeatureLight> msFeatures = providers.MSFeatureCache.FindByDatasetId(datasetID);

                // Tracks that the UMC was found already.
                Dictionary<int, UMCLight> foundUMC = new Dictionary<int, UMCLight>();

                UpdateStatus("Mapping data.");
                foreach (MSFeatureLight feature in msFeatures)
                {
                    if (!msFeatureIDToMsMsFeatureID.ContainsKey(feature.ID))
                        continue;

                    if (!msFeatureIDToLCMSFeatureID.ContainsKey(feature.ID))
                        continue;

                    // Map the MS/MS feature.
                    int msnID           = msFeatureIDToMsMsFeatureID[feature.ID];
                    MSSpectra spectrum  = msMsFeatureIDToSpectrum[msnID];
                    spectrum.GroupID    = datasetID;
                    int featureID       = msFeatureIDToLCMSFeatureID[feature.ID];

                    // This would mean the feature was filtered out.
                    if (!lcmsFeatureIDToFeature.ContainsKey(featureID))
                    {
                        continue;
                    }

                    feature.MSnSpectra.Add(spectrum);

                    // Map to the UMC 
                    UMCLight umc = null;
                    umc = lcmsFeatureIDToFeature[featureID];

                    feature.GroupID = datasetID;
                    umc.AddChildFeature(feature);

                    // Only add if we found the feature once before.
                    if (!foundUMC.ContainsKey(umc.ID))
                    {
                        features[datasetID].Add(umc);
                        foundUMC.Add(umc.ID, umc);
                    }
                }
            }            
            return features;
        }
        /// <summary>
        /// Extracts umc's with MSMS and determines whether to include all MS features or just the ones
        /// with related MSMS.
        /// </summary>
        /// <param name="full"></param>
        private Dictionary<int, List<UMCLight>> MapRemainingMSFeaturesToUMCs(Dictionary<int, List<UMCLight>>    features,
                                                                             ThermoRawDataFileReader            reader,
                                                                             IMSFeatureToLCMSFeatureDAO         msToLCMSFeatureCache,
                                                                             IMSFeatureDAO                      msFeatureCache)
        {            
            // for each dataset
            foreach (int key in features.Keys)
            {
                List<MSFeatureToLCMSFeatureMap> map = msToLCMSFeatureCache.FindByDatasetId(key);
                List<MSFeatureLight> allFeatures    = msFeatureCache.FindByDatasetId(key);

                Dictionary<int, MSFeatureLight> featureMap = new Dictionary<int, MSFeatureLight>();
                foreach(MSFeatureLight msFeature in allFeatures)
                {
                    featureMap.Add(msFeature.ID, msFeature);
                }
                Dictionary<int, List<MSFeatureToLCMSFeatureMap>> msFeatureMap = new Dictionary<int, List<MSFeatureToLCMSFeatureMap>>();
                foreach (MSFeatureToLCMSFeatureMap keyMap in map)
                {
                    if (!msFeatureMap.ContainsKey(keyMap.LCMSFeatureID))
                    {
                        msFeatureMap.Add(keyMap.LCMSFeatureID, new List<MSFeatureToLCMSFeatureMap>());
                    }
                    msFeatureMap[keyMap.LCMSFeatureID].Add(keyMap);
                }

                // Look at each feature.                   
                foreach (UMCLight feature in features[key])
                {
                    foreach (MSFeatureToLCMSFeatureMap lcMap in msFeatureMap[feature.ID])
                    {
                        MSFeatureLight xMap = featureMap[lcMap.MSFeatureID];

                        int index = feature.MSFeatures.FindIndex(delegate(MSFeatureLight x)
                                                            {
                                                                return x.ID == xMap.ID;
                                                            });
                        if (index < 0)
                        {
                            feature.MSFeatures.Add(xMap);
                        }
                    }
                        
                    feature.MSFeatures.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                    {
                        return x.Scan.CompareTo(y.Scan);
                    }
                    );                        
                }
            }            
            return features;
        }
        /// <summary>
        /// Extracts UMC SIC's for all of the features.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="analysis"></param>
        public void ExtractUMCSICs(string path, MultiAlignAnalysis analysis)
        {
            UpdateStatus("Extracting MSMS data");
            Dictionary<int, List<UMCLight>> features = ExtractUMCWithMSMS(analysis);

            // Create the object that knows how to read the RAW files we are analyzing.
            ThermoRawDataFileReader reader = null;

            if (analysis.MetaData.OtherFiles.Count <= 0)
            {
                UpdateStatus("Raw files specified.");
                reader = new ThermoRawDataFileReader();
                for (int i = 0; i < analysis.MetaData.Datasets.Count; i++)
                {
                    DatasetInformation datasetInformation = analysis.MetaData.Datasets[i];
                    InputFile rawInformation = analysis.MetaData.OtherFiles[i];

                    reader.AddDataFile(rawInformation.Path, datasetInformation.DatasetId);
                }
            }

            UpdateStatus("Completing mapping to rest of the features");
            features = MapRemainingMSFeaturesToUMCs(features, 
                                         reader, 
                                         analysis.DataProviders.MSFeatureToLCMSFeatureCache, 
                                         analysis.DataProviders.MSFeatureCache);

            string sicTitle     = "SIC";
            string sicXLabel    = "Scan";
            string sicYLabel    = "Intensity";
            
            // Look at each dataset.
            foreach (int datasetID in features.Keys)
            {
                // Look at each feature
                foreach (UMCLight feature in features[datasetID])
                {
                    int totalFeatures   = feature.MSFeatures.Count;
                    double[] scans      = new double[totalFeatures];
                    double[] intensity  = new double[totalFeatures];
                    string baseName     = string.Format("-{0}-{1}", datasetID, feature.ID);
                    int i               = 0;

                    double maxI = 0;
                    // Separate the MS features into charge state maps so we only look at one m/z
                    foreach (MSFeatureLight msFeature in feature.MSFeatures)
                    {
                        
                        // Build the SIC arrays for plotting
                        scans[i]     = msFeature.Scan;
                        intensity[i] = Math.Log(msFeature.Abundance, 2);
                        maxI         = Math.Max(intensity[i], maxI);
                        i++;
                    }
                    
                    System.Drawing.RectangleF rect  = new System.Drawing.RectangleF(0, 0, 400, 400);
                    ZedGraph.GraphPane pane         = new ZedGraph.GraphPane(rect, sicTitle + string.Format(" {0:0.000}" , feature.MassMonoisotopic), sicXLabel, sicYLabel);
                    pane.LineType                   = ZedGraph.LineType.Normal;                  
                    //pane.AddCurve("", scans, intensity, System.Drawing.Color.LightGray, ZedGraph.SymbolType.Default);

                    Dictionary<int, List<MSFeatureLight>> chargeMap = MultiAlignCore.Data.Features.LCMSFeatureChargeMapBuilder.BuildChargeMap(feature);

                    System.Drawing.Color[] colors = new System.Drawing.Color[] {System.Drawing.Color.Green,
                                                                                 System.Drawing.Color.Yellow,
                                                                                 System.Drawing.Color.Blue,
                                                                                 System.Drawing.Color.Orange,
                                                                                 System.Drawing.Color.Purple};
                    foreach (int charge in chargeMap.Keys)
                    {
                        
                        double [] fScans        = new double[chargeMap[charge].Count];
                        double [] fIntensities  = new double[chargeMap[charge].Count];
                        
                        i = 0;
                        foreach (MSFeatureLight msFeature in chargeMap[charge])
                        {
                            fScans[i]       = msFeature.Scan;
                            fIntensities[i] = Math.Log(msFeature.Abundance, 2);
                            i++;
                        }
                        
                        pane.AddCurve("",
                                        fScans,
                                        fIntensities,
                                        colors[(charge - 1) % colors.Length],
                                        ZedGraph.SymbolType.Square);                        
                    }

                    using (System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
                    {
                        pane.Fill.Brush = brush;
                        foreach (MSFeatureLight msFeature in feature.MSFeatures)
                        {
                            foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                            {
                                pane.AddStick("",
                                                new double[] { msFeature.Scan, msFeature.Scan },
                                                new double[] { Math.Log(msFeature.Abundance, 2), maxI },
                                                System.Drawing.Color.Red);
                            }
                        }
                    }                    
                    pane.AxisChange();
                    string fullPath = Path.Combine(path, "SIC" + baseName + ".png");
                    pane.GetImage().Save(fullPath);
                                        
                    /*Dictionary<int, List<MSFeatureLight>> chargeMap = MultiAlignCore.Data.Features.LCMSFeatureChargeMapBuilder.BuildChargeMap(feature);
                    foreach (int charge in chargeMap.Keys)
                    {
                        string baseName     = string.Format("{0}-{1}", feature.ID, charge);
                        int totalFeatures   = chargeMap[charge].Count;
                        double[] scans      = new double[totalFeatures];
                        double[] intensity  = new double[totalFeatures];

                        // Build the SIC arrays for plotting
                        int i = 0;
                        foreach (MSFeatureLight msFeature in chargeMap[charge])
                        {
                            scans[i]        = msFeature.Scan;
                            intensity[i]    = msFeature.Abundance;
                            i++;
                        }
                        System.Drawing.RectangleF rect  = new System.Drawing.RectangleF(0, 0, 400, 400);
                        ZedGraph.GraphPane pane         = new ZedGraph.GraphPane(rect, sicTitle, sicXLabel, sicYLabel);

                        pane.AddCurve("", scans, intensity, System.Drawing.Color.Black, ZedGraph.SymbolType.Circle);

                        foreach (MSFeatureLight msFeature in chargeMap[charge])
                        {
                            foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                            {
                                pane.AddCurve("",
                                                new double[] { spectrum.Scan },
                                                new double[] { spectrum.TotalIonCurrent },
                                                System.Drawing.Color.Red,
                                                ZedGraph.SymbolType.Square);
                            }
                            i++;
                        }

                        pane.AxisChange();

                        string fullPath = Path.Combine(path, "SIC" + baseName + ".png");
                        pane.GetImage().Save(fullPath);
                    }*/

                }
            }
        }
        /// <summary>
        /// Performs MS/MS alignment.
        /// </summary>
        /// <param name="analysis"></param>
        public void PerformMSMSAlignment(MultiAlignAnalysis analysis)
        {
            if (analysis.MetaData.OtherFiles.Count <= 0)
            {
                throw new Exception("No RAW files were specified.  Cannot continue with the analysis.");
            }

            Dictionary<int, List<UMCLight>> features  = ExtractUMCWithMSMS(analysis);

            // Create the object that knows how to read the RAW files we are analyzing.
            ThermoRawDataFileReader reader      = new ThermoRawDataFileReader();
            for(int i = 0; i < analysis.MetaData.Datasets.Count; i++)
            {
                DatasetInformation datasetInformation   = analysis.MetaData.Datasets[i];
                InputFile rawInformation                = analysis.MetaData.OtherFiles[i];

                reader.AddDataFile(rawInformation.Path, datasetInformation.DatasetId);
            }

            UpdateStatus("Starting MS/MS clustering.");
            // Create the object o align.
            SpectralNormalizedDotProductComparer comparer = new SpectralNormalizedDotProductComparer();
            MSMSClusterer msCluster         = new MSMSClusterer();
            msCluster.SimilarityTolerance   = .5;
            msCluster.ScanRange             = 2000;
            msCluster.SpectralComparer      = comparer;
            msCluster.Progress              += new EventHandler<ProgressNotifierArgs>(msCluster_Progress);
            List<MSMSCluster> clusters      = msCluster.Cluster(features, reader);            
            List<MSMSClusterMap> maps       = new List<MSMSClusterMap>();
            int id                          = 0;

            using (TextWriter writer = File.CreateText(@"C:\development\clusters.csv"))
            {
                foreach (MSMSCluster cluster in clusters)
                {
                    cluster.ID  = id++;
                    string line = string.Format("{0},", cluster.ID);

                    // Organize the spectra so they are sorted by dataset.
                    cluster.Features.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                    {
                        // Sort by scan if they are the same feature.
                        if (x.GroupID == y.GroupID)
                        {
                            return x.Scan.CompareTo(y.Scan);
                        }
                        // Otherwise we want to sort by what dataset they came from.
                        return x.GroupID.CompareTo(y.GroupID);
                    });

                    foreach (MSFeatureLight feature in cluster.Features)
                    {
                        MSMSClusterMap newMap = new MSMSClusterMap();
                        newMap.ClusterID      = cluster.ID;
                        newMap.MSMSID         = feature.MSnSpectra[0].ID;
                        newMap.GroupID        = feature.GroupID;
                        line +=  string.Format("{0},{1},{2},{3},{4},{5},{6},{7},",                                
                                                                                    feature.GroupID,
                                                                                    feature.MSnSpectra[0].ID,
                                                                                    feature.Abundance,
                                                                                    feature.Scan,
                                                                                    feature.Mz,
                                                                                    feature.MassMonoisotopicMostAbundant,
                                                                                    feature.MSnSpectra[0].Scan,
                                                                                    feature.MSnSpectra[0].PrecursorMZ);
                        maps.Add(newMap);
                    }
                    writer.WriteLine(line);
                }
            }
            analysis.DataProviders.MSMSClusterCache.AddAll(maps);
            reader.Dispose();
        }

        void msCluster_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);
        }
        #endregion

        #region Feature Extraction
        /// <summary>
        /// Extracts the clusters with MS/MS spectra and reports them.
        /// </summary
        /// <param name="providers"></param>
        /// <param name="datasets"></param>
        public void ExtractMSMS(FeatureDataAccessProviders providers,
                                List<DatasetInformation> datasets)
        {
            UpdateStatus("Producing feature traceback data structures.");
            UpdateStatus("Extracting Clusters from database.");            
            List<clsCluster> clusters               = providers.ClusterCache.FindAll();
            Dictionary<int, clsCluster> clusterMap  = new Dictionary<int, clsCluster>();
            Dictionary<int, List<FeatureExtractionMap>> clusterFeatureMap = new Dictionary<int,List<FeatureExtractionMap>>();
            foreach (clsCluster cluster in clusters)
            {                
                clusterMap.Add(cluster.Id, cluster);
                clusterFeatureMap.Add(cluster.Id, new List<FeatureExtractionMap>());
            }

            UpdateStatus("Mapping any matched mass tags to clusters.");
            Dictionary<int, List<ClusterToMassTagMap>> clusterMatches    = new Dictionary<int, List<ClusterToMassTagMap>>();
            Dictionary<int, MassTagLight> massTagsMap                    = new Dictionary<int, MassTagLight>();

            List<ClusterToMassTagMap> matches = providers.MassTagMatches.FindAll();
            foreach (ClusterToMassTagMap map in matches)
            {
                if (!clusterMatches.ContainsKey(map.ClusterId))
                {
                    clusterMatches.Add(map.ClusterId, new List<ClusterToMassTagMap>());
                }
                clusterMatches[map.ClusterId].Add(map);                
            }

            UpdateStatus("Finding all mass tags used in analysis.");
            List<MassTagLight> tags = providers.MassTags.FindAll();
            foreach (MassTagLight tag in tags)
            {
                massTagsMap.Add(tag.ID, tag);
            }

            UpdateStatus("Extracting data from each dataset.");
            foreach (DatasetInformation dataset in datasets)
            {
                int datasetID = dataset.DatasetId;

                UpdateStatus(string.Format("Mapping data from dataset {0} with id {1}", dataset.DatasetName, dataset.DatasetId));

                UpdateStatus("Exctracting LC-MS to MS Feature Map");
                Dictionary<int, int> msFeatureIDToLCMSFeatureID = new Dictionary<int, int>();

                List<MSFeatureToLCMSFeatureMap> msToLcmsFeatureMaps = providers.MSFeatureToLCMSFeatureCache.FindByDatasetId(datasetID);
                foreach (MSFeatureToLCMSFeatureMap map in msToLcmsFeatureMaps)
                {
                    msFeatureIDToLCMSFeatureID.Add(map.MSFeatureID, map.LCMSFeatureID);
                }

                UpdateStatus("Extracting MS to MSn Feature Map");
                List<MSFeatureToMSnFeatureMap> msnToMSFeatureMaps = providers.MSFeatureToMSnFeatureCache.FindByDatasetId(datasetID);
                Dictionary<int, int> msFeatureIDToMsMsFeatureID = new Dictionary<int, int>();
                foreach (MSFeatureToMSnFeatureMap map in msnToMSFeatureMaps)
                {
                    msFeatureIDToMsMsFeatureID.Add(map.MSFeatureID, map.MSMSFeatureID);
                }

                UpdateStatus("Extracting MSn Features");
                Dictionary<int, MSSpectra> msMsFeatureIDToSpectrum = new Dictionary<int, MSSpectra>();
                List<MSSpectra> msnSpectra               = providers.MSnFeatureCache.FindByDatasetId(datasetID);
                foreach (MSSpectra spectrum in msnSpectra)
                {
                    msMsFeatureIDToSpectrum.Add(spectrum.ID, spectrum);
                }

                UpdateStatus("Extracting LC-MS Features");
                List<clsUMC> lcmsFeatures = providers.FeatureCache.FindByDatasetId(datasetID);
                Dictionary<int, clsUMC> lcmsFeatureIDToFeature = new Dictionary<int, clsUMC>();
                foreach (clsUMC umc in lcmsFeatures)
                {
                    lcmsFeatureIDToFeature.Add(umc.Id, umc);
                }

                UpdateStatus("Extracting MS Features");
                List<MSFeatureLight> msFeatures = providers.MSFeatureCache.FindByDatasetId(datasetID);
                foreach (MSFeatureLight feature in msFeatures)
                {
                    /// only map things that have clusters.
                    if (!msFeatureIDToMsMsFeatureID.ContainsKey(feature.ID))
                        continue;

                    if (!msFeatureIDToLCMSFeatureID.ContainsKey(feature.ID))
                        continue;
                        
                    int msnID           = msFeatureIDToMsMsFeatureID[feature.ID];
                    MSSpectra spectrum  = msMsFeatureIDToSpectrum[msnID];
                    int featureID       = msFeatureIDToLCMSFeatureID[feature.ID];

                    // This would mean the feature was filtered out.
                    if (!lcmsFeatureIDToFeature.ContainsKey(featureID))
                    {
                        continue;
                    }

                    clsUMC umc = null;
                    try
                    {
                        umc = lcmsFeatureIDToFeature[featureID];
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    int clusterID       = umc.ClusterId;

                    FeatureExtractionMap fex = new FeatureExtractionMap();

                    fex.DatasetID           = datasetID;

                    fex.MSnScan             = spectrum.Scan;
                    fex.MSnRetentionTime    = spectrum.RetentionTime;
                    fex.MSnPrecursorMz      = spectrum.PrecursorMZ;
                    fex.MSnFeatureId        = spectrum.ID;
                    fex.MSnDatasetID        = spectrum.GroupID;

                    fex.MSCharge            = feature.ChargeState;
                    fex.MSIntensity         = feature.Abundance;
                    fex.MSFeatureId         = feature.ID;
                    fex.MSMz                = feature.Mz;
                    fex.MSScan              = feature.Scan;

                    fex.LCMSAbundance = umc.AbundanceMax;
                    fex.LCMSFeatureID = umc.Id;
                    fex.LCMSMass = umc.MassCalibrated;
                    fex.LCMSNet = umc.Net;
                    fex.LCMSScan = umc.Scan;
                                        
                    clusterFeatureMap[clusterID].Add(fex);                    
                }
            }

            if (FeaturesExtracted != null)
            {
                FeaturesExtractedEventArgs args = new FeaturesExtractedEventArgs(   clusterFeatureMap,
                                                                                    clusterMap,
                                                                                    massTagsMap,
                                                                                    clusterMatches);

                FeaturesExtracted(this, args);
            }           
        }
        #endregion

        #region Analysis Start/Stop
        /// <summary>
        /// Starts a multi-Align analysis job.
        /// </summary>
        public void StartAnalysis(MultiAlignAnalysis analysis)
        {
            if (m_algorithms == null)
            {
                throw new NullReferenceException("The algorithms have not been set for this analysis.");
            }


            if (analysis == null)
            {
                throw new NullReferenceException("The analysis data storage cannot be null.");
            }

            if (analysis.DataProviders == null)
            {
                throw new NullReferenceException("The data cache providers have not been set for this analysis.");
            }

            // Make sure we start with a fresh analysis.
            AbortAnalysisThread(m_analysisThread);

            m_analysis = analysis;

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
        /// <summary>
        /// Gets or sets whether to load data.
        /// </summary>
        public bool LoadData
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
        /// <summary>
        /// Starts the main analysis.
        /// </summary>
        private void PerformAnalysis()
        {
            try
            {
                MassTagDatabase database = null;

                if (LoadData)
                {
                    UpdateStatus("Setting up parameters");

                    // Load the mass tag database if we are aligning, or if we are 
                    // peak matching (but aligning to a reference dataset.
                    if (m_analysis.Options.UseMassTagDBAsBaseline)
                    {
                        UpdateStatus("Loading Mass Tag database from database:  " + m_analysis.Options.MassTagDatabaseOptions.mstrDatabase);

                        database = MTDBLoaderFactory.LoadMassTagDB(m_analysis.Options.MassTagDatabaseOptions, m_analysis.MetaData.AnalysisSetupInfo.Database.DatabaseFormat);
                    }
                    else
                    {
                        if (m_analysis.Options.MassTagDatabaseOptions.menm_databaseType != MassTagDatabaseType.None)
                        {
                            UpdateStatus("Loading Mass Tag database from database:  " + m_analysis.Options.MassTagDatabaseOptions.mstrDatabase);
                            database = MTDBLoaderFactory.LoadMassTagDB(m_analysis.Options.MassTagDatabaseOptions, m_analysis.MetaData.AnalysisSetupInfo.Database.DatabaseFormat);
                        }
                    }

                    if (database != null)
                    {
                        int totalMassTags = database.MassTags.Count;
                        UpdateStatus("Loaded " + totalMassTags.ToString() + " mass tags.");
                    }

                    m_analysis.MassTagDatabase = database;

                    UpdateStatus("Loading auxillary data files.");
                    LoadOtherData(m_analysis.MetaData.OtherFiles,
                                    Path.Combine(m_analysis.MetaData.AnalysisPath, m_analysis.MetaData.AnalysisName),
                                    m_analysis.DataProviders);

                    UpdateStatus("Loading dataset data files.");
                    LoadDatasetData(m_analysis.MetaData.Datasets,
                                    m_analysis.Options.UMCFindingOptions,
                                    Path.Combine(m_analysis.MetaData.AnalysisPath, m_analysis.MetaData.AnalysisName));

                    UpdateStatus("Linking MS Features to MSn Features.");
                    LinkMSFeaturesToMSMS(m_analysis.DataProviders, m_analysis.MetaData.Datasets);
                }

                switch (m_analysis.AnalysisType)
                {
                    case AnalysisType.ExtractSICs:
                        UpdateStatus("Building SIC's");
                        this.ExtractUMCSICs(this.AnalaysisPath, m_analysis);
                        UpdateStatus(string.Format("Analysis {0} Completed.", m_analysis.MetaData.AnalysisName));
                        break;
                    case AnalysisType.MSMSAlignment:
                        UpdateStatus("Creating Anchor Points And Clustering for Ms-Ms alignment.");
                        PerformMSMSAlignment(m_analysis);

                        
                        UpdateStatus(string.Format("Analysis {0} Completed.", m_analysis.MetaData.AnalysisName));
                        break;
                    case AnalysisType.Full:
                        

                        UpdateStatus("Aligning datasets.");
                        PerformAlignment(m_analysis);

                        UpdateStatus("Performing clustering.");
                        PerformClustering(m_analysis, m_algorithms.Clusterer);

                        UpdateStatus("Performing Peak Matching.");
                        PerformPeakMatching();

                        UpdateStatus(string.Format("Analysis {0} Completed.", m_analysis.MetaData.AnalysisName));
                        break;
                }
            }
            catch (OutOfMemoryException ex)
            {
                if (AnalysisError != null)
                {
                    AnalysisError(this, new AnalysisErrorEventArgs("Out of memory.", ex));
                }
                return;
            }
            catch (Exception ex)
            {
                if (AnalysisError != null)
                {
                    AnalysisError(this, new AnalysisErrorEventArgs("Handled Error. ", ex));
                }
                return;
            }
           

            if (AnalysisComplete != null)
            {
                AnalysisComplete(this, new AnalysisCompleteEventArgs(m_analysis));
            }            
        }
        #endregion     
    }

    /// <summary>
    /// Analysis Steps
    /// </summary>
    public enum AnalysisType
    {
        ExtractSICs,
        FactorImporting,
        MSMSAlignment,
        Full,
        Alignment,
        Clustering,
        PeakMatching,
        ExportDataOnly,
        InvalidParameters
    }
}
