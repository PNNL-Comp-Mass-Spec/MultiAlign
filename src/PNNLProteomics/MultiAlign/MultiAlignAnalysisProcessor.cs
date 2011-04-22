using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Mammoth.Data;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.PeakMatching;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;
using PNNLProteomics.Algorithms;
using PNNLProteomics.Algorithms.PeakMatching;
using PNNLProteomics.Data;
using PNNLProteomics.Data.Alignment;
using PNNLProteomics.IO.MTDB;
using PNNLProteomics.IO.UMC;
using PNNLProteomics.MultiAlign.Hibernate.Domain;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;

using PNNLProteomics.SMART;

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

        #region Properties
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
            IUmcDAO featureCache = m_analysis.DataProviders.FeatureCache;
            foreach (DatasetInformation dataset in datasets)
            {
                UpdateStatus("Loading dataset " + dataset.DatasetName + ".");
                List<clsUMC> features = UMCLoaderFactory.LoadData(  dataset,
                                                                    featureCache,
                                                                    options);

                UpdateStatus("Loaded dataset " + dataset.DatasetName + ". Adding to cache.");

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
                UpdateStatus("Using Mammoth clustering framework without partitions.  Clustering will run with all features loaded into memory.");
            }

            string databaseName = Path.Combine(m_analysis.AnalysisPath, m_analysis.AnalysisName);
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
                                                                                70);
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
            IUmcDAO featureCache = m_analysis.DataProviders.FeatureCache;
            
            // Load the baseline data.
            List<clsUMC> baselineFeatures   = null;
            int baselineDatasetID           = -1;
            DatasetInformation baselineInfo = null;

            UpdateStatus("Confirming baseline.");
            if (!analysis.UseMassTagDBAsBaseline)
            {
                int i = 0;
                foreach(DatasetInformation info in analysis.Datasets)
                {
                    //TODO: Check the path of the dataset as the dataset name!
                    if (info.DatasetName == analysis.BaselineDatasetName)
                    {
                        baselineDatasetID = i;
                        break;
                    }
                    i++;
                }
                baselineInfo        = analysis.Datasets[baselineDatasetID];

                UpdateStatus("Loading baseline features from " + baselineInfo.DatasetName + " for alignment.");
                baselineFeatures    = featureCache.FindByDatasetId(baselineDatasetID);
            }
            
            // Align pairwise and cache results intermediately.
            IFeatureAligner aligner = m_algorithms.Aligner;

            m_analysis.AlignmentOptions.Clear();

            for (int datasetNum = 0; datasetNum < m_analysis.Datasets.Count; datasetNum++)
            {
                if (datasetNum != baselineDatasetID)
                {

                    DatasetInformation datasetInfo = m_analysis.Datasets[datasetNum];

                    UpdateStatus("Retrieving data from " + datasetInfo.DatasetName + " for alignment.");
                    List<clsUMC> features = featureCache.FindByDatasetId(datasetNum);
                    classAlignmentData alignmentData = null;

                    //// We dont track the UMC Index...this was used previously by the UMC Data object :(
                    //// Here we are applying a temp fix to iterate through the features and assign its unique ID.                    
                    int featureIDIndex = 0;
                    foreach (clsUMC feature in features)
                    {
                        feature.mint_umc_index = featureIDIndex++;
                    }


                    m_analysis.AlignmentOptions.Add(m_analysis.DefaultAlignmentOptions);


                    if (baselineInfo != null)
                    {
                        UpdateStatus("Aligning " + datasetInfo.DatasetName + " to baseline.");
                        alignmentData = aligner.AlignFeatures(baselineFeatures,
                                                            features,
                                                            analysis.AlignmentOptions[datasetNum]);
                    }
                    else
                    {
                        UpdateStatus("Aligning " + datasetInfo.DatasetName + " to mass tag database.");
                        alignmentData = aligner.AlignFeatures(analysis.MassTagDatabase,
                                                            features,
                                                            analysis.AlignmentOptions[datasetNum]);
                    }
                    alignmentData.aligneeDataset = datasetInfo.DatasetName;                    
                    analysis.AlignmentData.Add(alignmentData);


                    UpdateStatus("Updating cache with aligned features.");
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
                else
                {
                    m_analysis.AlignmentData.Add(null);
                    m_analysis.AlignmentOptions.Add(null);
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
                    List<clsCluster> clusters = m_analysis.DataProviders.ClusterCache.FindAll();
                    IPeakMatcher peakMatcher  = m_algorithms.PeakMatcher;


                    if (!m_analysis.ClusterOptions.AlignClusters)
                    {
                        UpdateStatus("Adjusting masses and NETs for clusters for peak matching.");

                        // If the clusters are not aligned, then the 
                        // peak matching jobs may fail as they require the data structure to have 
                        // calibrated data.  Talk about coupling!
                        foreach (clsCluster cluster in clusters)
                        {
                            cluster.MassCalibrated = cluster.Mass;
                            cluster.NetAligned      = cluster.Net;
                        }
                    }
                    
                    if (m_analysis.PeakMatchingOptions.UseSTAC)
                    {
                        UpdateStatus("Peak matching with STAC");
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

                    clsMassTag[] massTagArray = null; // analysis.PeakMatchingResults.marrMasstags;
                    clsProtein[] proteinArray = null; // analysis.PeakMatchingResults.marrProteins;            
                   
                    massTagArray = m_analysis.PeakMatchingResults.marrMasstags;
                    proteinArray = m_analysis.PeakMatchingResults.marrProteins;

                    UpdateStatus("Updating database with peak matched results.");

                    //TODO: Fix this with the providers.
                    IMassTagDAO massTagDAOHibernate = new PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate.MassTagDAOHibernate();
                    IProteinDAO proteinDAOHibernate = new PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate.ProteinDAOHibernate();
                    PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate.GenericDAOHibernate<ClusterToMassTagMap> clusterToMassTagMapDAOHibernate =
                        new PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate.GenericDAOHibernate<ClusterToMassTagMap>();

                    PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate.GenericDAOHibernate<MassTagToProteinMap> massTagToProteinMapDAOHibernate =
                        new Hibernate.Domain.DAOHibernate.GenericDAOHibernate<MassTagToProteinMap>();

                    PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate.GenericDAOHibernate<StacFDR> stacFDRDAOHibernate = 
                        new Hibernate.Domain.DAOHibernate.GenericDAOHibernate<StacFDR>();

                    List<clsMassTag> massTagList                      = new List<clsMassTag>();
                    List<clsProtein> proteinList                      = new List<clsProtein>();
                    List<ClusterToMassTagMap> clusterToMassTagMapList = new List<ClusterToMassTagMap>();
                    List<MassTagToProteinMap> massTagToProteinMapList = new List<MassTagToProteinMap>();
                    List<StacFDR> stacFDRResultsList                  = new List<StacFDR>();

                    foreach (clsPeakMatchingResults.clsPeakMatchingTriplet triplet in m_analysis.PeakMatchingResults.marrPeakMatchingTriplet)
                    {
                        clsMassTag massTag = massTagArray[triplet.mintMassTagIndex];
                        clsProtein protein = proteinArray[triplet.mintProteinIndex];

                        ClusterToMassTagMap clusterToMassTagMap = new ClusterToMassTagMap(triplet.mintFeatureIndex, massTag.Id);
                        MassTagToProteinMap massTagToProteinMap = new MassTagToProteinMap(massTag.Id, protein.Id);

                        if (!clusterToMassTagMapList.Contains(clusterToMassTagMap))
                        {
                            clusterToMassTagMapList.Add(clusterToMassTagMap);
                        }

                        if (!massTagToProteinMapList.Contains(massTagToProteinMap))
                        {
                            massTagToProteinMapList.Add(massTagToProteinMap);
                        }

                        if (!massTagList.Contains(massTag))
                        {
                            massTagList.Add(massTag);
                        }

                        if (!proteinList.Contains(protein))
                        {
                            proteinList.Add(protein);
                        }
                    }

                    foreach (classSMARTFdrResult fdrResult in m_analysis.STACTResults.GetSummaries())
                    {
                        stacFDRResultsList.Add(new StacFDR(fdrResult));
                    }

                    
                    UpdateStatus(string.Format("Found {0} mass tag matches. Matching to {1} potential proteins.", 
                                                                                                massTagList.Count, 
                                                                                                proteinList.Count));

                    massTagDAOHibernate.AddAll(massTagList);
                    proteinDAOHibernate.AddAll(proteinList);
                    clusterToMassTagMapDAOHibernate.AddAll(clusterToMassTagMapList);
                    massTagToProteinMapDAOHibernate.AddAll(massTagToProteinMapList);
                    stacFDRDAOHibernate.AddAll(stacFDRResultsList);
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
        /// Starts the main analysis.
        /// </summary>
        private void PerformAnalysis()
        {
            try
            {
                clsMassTagDB database = null;

                UpdateStatus("Setting up parameters");

                // Load the mass tag database if we are aligning, or if we are 
                // peak matching (but aligning to a reference dataset.
                if (m_analysis.UseMassTagDBAsBaseline)
                {
                    UpdateStatus("Loading Mass Tag database from database:  " + m_analysis.MassTagDBOptions.mstrDatabase);
                    database = MTDBLoaderFactory.LoadMassTagDB(m_analysis.MassTagDBOptions);
                }
                else
                {
                    if (m_analysis.MassTagDBOptions.menm_databaseType != MassTagDatabaseType.None)
                    {
                        UpdateStatus("Loading Mass Tag database from database:  " + m_analysis.MassTagDBOptions.mstrDatabase);
                        database = MTDBLoaderFactory.LoadMassTagDB(m_analysis.MassTagDBOptions);
                    }
                }

                if (database != null)
                {
                    int totalMassTags = database.GetMassTagCount();
                    UpdateStatus("Loaded " + totalMassTags.ToString() + " mass tags.");
                }

                m_analysis.MassTagDatabase = database;

                UpdateStatus("Loading data");
                LoadDatasetData(m_analysis.Datasets,
                                m_analysis.UMCFindingOptions,
                                Path.Combine(m_analysis.AnalysisPath, m_analysis.AnalysisName));

                UpdateStatus("Aligning datasets.");
                AlignDatasets(m_analysis);

                UpdateStatus("Performing clustering.");
                PerformClustering(m_analysis, m_algorithms.Clusterer);

                UpdateStatus("Performing Peak Matching.");                
                PerformPeakMatching();

                UpdateStatus(string.Format("Analysis {0} Completed.", m_analysis.AnalysisName));
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
}
