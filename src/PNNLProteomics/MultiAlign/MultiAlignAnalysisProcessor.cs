using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization.Formatters.Binary;

using System.Xml;
using MultiAlignEngine;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.Features;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.PeakMatching;

using PNNLOmics.Data.Features;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;


using PNNLProteomics.IO;
using PNNLProteomics.IO.UMC;
using PNNLProteomics.IO.MTDB;
using PNNLProteomics.SMART;
using PNNLProteomics.EventModel;
using PNNLProteomics.Data.Factors;
using PNNLProteomics.Data.Alignment;
using PNNLProteomics.MultiAlign;
using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;
using PNNLProteomics.Data;
using PNNLProteomics.Data.Analysis;
using PNNLProteomics.Algorithms.PeakMatching;
using PNNLProteomics.Algorithms.Alignment;
using PNNLProteomics.Algorithms;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;

using Mammoth.Data;

namespace PNNLProteomics.MultiAlign
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
        /// Fired when the analysis is complete.
        /// </summary>
        public event EventHandler AnalysisComplete;
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
        /// 
        /// </summary>
        private FeatureDataAccessProviders m_providers;
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
            m_providers  = null;
            m_analysis   = null;
            m_algorithms = null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the data providers to the underlying data cache.
        /// </summary>
        public FeatureDataAccessProviders DataProviders
        {
            get
            {
                return m_providers;
            }
            set
            {
                m_providers = value;
            }
        }
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
            }
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

        #region Analysis Methods
        /// <summary>
        /// Load the data from the dataset information objects to the cache at the analysis Path
        /// </summary>
        /// <param name="datasets">Datasets to load.</param>
        /// <param name="options">Options to use for UMC finding if required.</param>
        /// <param name="analysisPath">Path to save data to.</param>
        private void LoadDatasetData(List<DatasetInformation> datasets,
                             clsUMCFindingOptions     options,
                             string                   analysisPath)
        {            
            IUmcDAO featureCache = m_providers.FeatureCache;
            foreach (DatasetInformation dataset in datasets)
            {
                List<clsUMC> features = UMCLoaderFactory.LoadData(  dataset,
                                                                    featureCache,
                                                                    options);
                                
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
        /// <summary>
        /// Performs clustering using the mammoth framework.
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="clusterer"></param>
        /// <returns></returns>
        public void PerformClustering(MultiAlignAnalysis analysis,
                                      IClusterer<UMCLight, Mammoth.Data.MammothCluster> clusterer)
        {
            UpdateStatus("Using Cluster Algorithm: " + clusterer.ToString());

            // Tolerances
            FeatureTolerances tolerances            = new FeatureTolerances();
            FeatureClusterParameters parameters     = new FeatureClusterParameters();
            tolerances.DriftTime                    = analysis.ClusterOptions.DriftTimeTolerance;
            tolerances.Mass                         = analysis.ClusterOptions.MassTolerance;
            tolerances.RetentionTime                = analysis.ClusterOptions.NETTolerance;
            parameters.CentroidRepresentation       = PNNLOmics.Data.Features.ClusterCentroidRepresentation.Mean;
            parameters.Tolerances                   = tolerances;
            parameters.OnlyClusterSameChargeStates  = (analysis.ClusterOptions.IgnoreCharge == false);
            if (analysis.ClusterOptions.ClusterRepresentativeType == enmClusterRepresentativeType.MEDIAN)
            {
                parameters.CentroidRepresentation = ClusterCentroidRepresentation.Median;
            }

            // This just tells us whether we are using mammoth memory partitions or not.
            if (analysis.ClusterOptions.RecursionLevels > 0)
            {
                UpdateStatus(string.Format("Using Mammoth clustering framework with {0} recursive memory partitions.", analysis.ClusterOptions.RecursionLevels));
            }
            else
            {
                UpdateStatus("Using Mammoth clustering framework without partitions.  Clustering will run with all data loaded into memory.");
            }

            string databaseName = Path.Combine(m_analysis.PathName, m_analysis.AnalysisName);
            int maxChargeState = 15;
            int minChargeState = 1;

            // Connect to the database.
            using (MammothDatabase database = new MammothDatabase(databaseName))
            {
                database.Connect();
                MammothDatabaseRange range = new Mammoth.Data.MammothDatabaseRange(-1,
                                                                                30000,
                                                                                -1.5,
                                                                                1.5,
                                                                                0,
                                                                                70,
                                                                                false);
                List<int> chargeStatesToCluster = new List<int>();
                if (analysis.ClusterOptions.IgnoreCharge)
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

                Mammoth.Algorithms.MammothClusterer processor = new Mammoth.Algorithms.MammothClusterer();
                foreach (int chargeState in chargeStatesToCluster)
                {

                    if (chargeState < 0)
                    {
                        UpdateStatus(string.Format("Clustering all charge states."));
                    }
                    else
                    {
                        UpdateStatus(string.Format("Clustering Charge State = {0}.", chargeState));
                    }
                    range.SingleChargeState = chargeState;
                    processor.ClusterDatabase(database, analysis.ClusterOptions.RecursionLevels, parameters, range, parameters.Tolerances, clusterer);
                    UpdateStatus("Finished clustering charge state.");
                }
            }
        }        
        /// <summary>
        /// Aligns all of the datasets.
        /// </summary>
        public void AlignDatasets(MultiAlignAnalysis analysis)
        {
            // Connect to database of features.                
            IUmcDAO featureCache = m_providers.FeatureCache;
            
            // Load the baseline data.
            List<clsUMC> baselineFeatures   = null;
            int baselineDatasetID             = -1;
            DatasetInformation baselineInfo = null;

            if (!analysis.DefaultAlignmentOptions.IsAlignmentBaselineAMasstagDB)
            {
                int i = 0;
                foreach(DatasetInformation info in analysis.Datasets)
                {
                    if (info.mstrLocalPath == analysis.BaselineDataset)
                    {
                        baselineDatasetID = i;
                        break;
                    }
                    i++;
                }
                baselineInfo        = analysis.Datasets[baselineDatasetID];
                baselineFeatures    = featureCache.FindByDatasetId(baselineDatasetID);
            }
            else
            {
                if (analysis.MassTagDatabase == null)
                {
                    analysis.MassTagDatabase = MTDBLoaderFactory.LoadMassTagDB(analysis.MassTagDBOptions);
                }
            }
            
            // Align pairwise and cache results intermediately.
            IFeatureAligner aligner = m_algorithms.Aligner;
            
            for (int datasetNum = 0; datasetNum < m_analysis.Datasets.Count; datasetNum++)
            {
                if (datasetNum != baselineDatasetID)
                {                        
                    DatasetInformation  datasetInfo      = m_analysis.Datasets[datasetNum];
                    List<clsUMC>        features         = featureCache.FindByDatasetId(datasetNum);
                    classAlignmentData  alignmentData    = null;

                    //// We dont track the UMC Index...this was used previously by the UMC Data object :(
                    //// Here we are applying a temp fix to iterate through the features and assign its unique ID.                    
                    int featureIDIndex = 0;
                    foreach (clsUMC feature in features)
                    {
                        feature.mint_umc_index = featureIDIndex++;
                    }

                    if (baselineInfo != null)
                    {
                        alignmentData = aligner.AlignFeatures(baselineFeatures,
                                                            features,
                                                            analysis.AlignmentOptions[datasetNum]);
                    }
                    else
                    {
                        alignmentData = aligner.AlignFeatures(analysis.MassTagDatabase,
                                                            features,
                                                            analysis.AlignmentOptions[datasetNum]);
                    }
                    alignmentData.aligneeDataset       = datasetInfo.DatasetName;
                    analysis.AlignmentData.Add(alignmentData);

                    featureCache.UpdateAll(features);                    
                    if (FeaturesAligned != null)
                    {                            
                        FeaturesAlignedEventArgs args = new FeaturesAlignedEventArgs(baselineInfo,
                                                                                     datasetInfo,
                                                                                     alignmentData);
                        FeaturesAligned(this, args);
                    }
                    features.Clear();
                    features = null;
                }
            }
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
                    List<clsCluster> clusters = m_providers.ClusterCache.FindAll();
                    IPeakMatcher peakMatcher  = m_algorithms.PeakMatcher;

                    if (m_analysis.UseSMART)
                    {
                        UpdateStatus("Peak matching with SMART");
                        m_analysis.STACTResults = peakMatcher.PerformSTAC(clusters,
                                                                m_analysis.MassTagDatabase,
                                                                m_analysis.SMARTOptions);

                        m_analysis.PeakMatchingResults = peakMatcher.ConvertSTACResultsToPeakResults(m_analysis.STACTResults,
                                                                                          m_analysis.MassTagDatabase,
                                                                                          clusters);
                    }
                    else
                    {
                        UpdateStatus("Traditional Peak matching.");
                        // No-shift, and 11-dalton shift.
                        m_analysis.PeakMatchingResults = peakMatcher.PerformPeakMatching(clusters, 
                                                                m_analysis.MassTagDatabase, m_analysis.PeakMatchingOptions, 0.0);

                        UpdateStatus("Traditional Peak matching with 11-dalton shift.");
                        m_analysis.PeakMatchingResultsShifted = peakMatcher.PerformPeakMatching(clusters,
                                                                m_analysis.MassTagDatabase, m_analysis.PeakMatchingOptions, 11.0);
                    }
                }
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

            if (m_providers == null)
            {
                throw new NullReferenceException("The data cache providers have not been set for this analysis.");
            }

            if (analysis == null)
            {
                throw new NullReferenceException("The analysis data storage cannot be null.");
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
        /// Starts the main analysis.
        /// </summary>
        private void PerformAnalysis()
        {
            clsMassTagDB database        = null;

            UpdateStatus("Setting up parameters");
            
            // Load the mass tag database if we are aligning, or if we are 
            // peak matching (but aligning to a reference dataset.
            if (m_analysis.UseMassTagDBAsBaseline)
            {
                database = MTDBLoaderFactory.LoadMassTagDB(m_analysis.MassTagDBOptions);
            }
            else
            {
                if (m_analysis.MassTagDBOptions.menm_databaseType != MassTagDatabaseType.None)
                {
                    database = MTDBLoaderFactory.LoadMassTagDB(m_analysis.MassTagDBOptions);
                }
            }
            m_analysis.MassTagDatabase   = database;

            UpdateStatus("Loading data");
            LoadDatasetData(m_analysis.Datasets, 
                            m_analysis.UMCFindingOptions, 
                            Path.Combine(m_analysis.PathName, m_analysis.AnalysisName));

            UpdateStatus("Aligning datasets.");
            AlignDatasets(m_analysis);

            UpdateStatus("Performing clustering.");
            PerformClustering(m_analysis, m_algorithms.Clusterer);


            // Performs peak matching if deemed to.
            PerformPeakMatching();

            UpdateStatus(string.Format("Analysis {0} Completed.", m_analysis.AnalysisName));
            if (AnalysisComplete != null)
                AnalysisComplete(this, null);            
        }
        #endregion     
    }
}
