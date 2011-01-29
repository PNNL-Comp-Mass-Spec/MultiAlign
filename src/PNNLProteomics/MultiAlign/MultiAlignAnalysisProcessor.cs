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

using PNNLProteomics.IO;
using PNNLProteomics.SMART;
using PNNLProteomics.EventModel;
using PNNLProteomics.Data.Factors;
using PNNLProteomics.Data.Alignment;
using PNNLProteomics.MultiAlign;
using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;

using PNNLProteomics.Data;
using PNNLProteomics.Data.Analysis;

using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;

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

        #region Constants
        /// <summary>
        /// Maximum number of features to allow in memory when linking clusters to features via Mammoth before persisting objects to the database.
        /// </summary>
        private const int FEATURE_CLUSTER_LINK_CACHE_SIZE = 500;
        #endregion

        #region Members
        /// <summary>
        /// Holds all information about how to perform the analysis and how to store results.
        /// </summary>
        private MultiAlignAnalysis m_analysis;
        /// <summary>
        /// Thread in charge of performing the analysis.
        /// </summary>
        private Thread m_analysisThread;
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MultiAlignAnalysisProcessor()
        {            
        }
        #endregion

        #region Disposing, Abort, and Destruction
        /// <summary>
        /// Dispose method that will kill the analysis thread.
        /// </summary>
        public void Dispose()
        {
            Abort();

            NHibernateUtil.Dispose();
                        
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
        /// <summary>
        /// Aborts the analysis thread.
        /// </summary>
        public void Abort()
        {            
            AbortAnalysisThread(m_analysisThread);
        }
        #endregion

        #region Loading of data (features and mass tag database)
        /// <summary>
        /// Loads the mass tag database.
        /// </summary>
        public clsMassTagDB LoadMassTagDB(clsMassTagDatabaseOptions options)
        {
            clsMassTagDB database = null;
            if (options.menm_databaseType == MassTagDatabaseType.ACCESS && Path.GetExtension(options.mstr_databaseFilePath) == ".txt")
            {
                // XAMT             
                XAMTReader reader   = new XAMTReader();
                database            = reader.ReadXAMTDatabase(options.mstr_databaseFilePath);
            }
            else
            {       
                // MTS or ACCESS 
                clsMTDBLoader loader    = new clsMTDBLoader(options);
                database                = loader.LoadMassTagDatabase();                
            }
            return database;
        }   
        /// <summary>
        /// Load the data from the dataset information objects to the cache at the analysis Path
        /// </summary>
        /// <param name="datasets">Datasets to load.</param>
        /// <param name="options">Options to use for UMC finding if required.</param>
        /// <param name="analysisPath">Path to save data to.</param>
        public void LoadData(List<DatasetInformation> datasets,
                             clsUMCFindingOptions     options,
                             string                   analysisPath)
        {
            NHibernateUtil.SetDbLocationForWrite(analysisPath);

            int index = 0;
            foreach (DatasetInformation dataset in datasets)
            {
                List<clsUMC> features = LoadData(dataset, options);

                foreach (clsUMC umc in features)
                {
                    umc.Id = index++;                    
                }

                UmcDAOHibernate cache = new UmcDAOHibernate();
                cache.AddAll(features);  
                

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
        /// Loads the data provided via the dataset information object.  Uses UMC Finding options if 
        /// reading decon2ls or other tool output.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private List<clsUMC> LoadData(DatasetInformation dataset, clsUMCFindingOptions options)
        {
            clsUMC[] loadedFeatures     = null;            
            clsUMCCreator umcFinder     = new clsUMCCreator();
            umcFinder.UMCFindingOptions = options;
            int umcIndex                = 0;
            
            string extension = Path.GetExtension(dataset.mstrLocalPath).ToUpper();
            switch(extension)
            {
                case ".TXT":
                    // LCMS Features File                         
                    UmcReader umcReader = new UmcReader(dataset.mstrLocalPath);
                    loadedFeatures      = umcReader.GetUmcList().ToArray();                    
                    int minScan         = int.MaxValue;
                    int maxScan         = int.MinValue;

                    // Find scan extrema to calculate a NET value.
                    foreach (clsUMC umc in loadedFeatures)
                    {
                        minScan = Math.Min(umc.mint_start_scan, minScan);
                        maxScan = Math.Max(umc.mint_end_scan, maxScan);
                    }

                    // Scale for the NET
                    
                    foreach (clsUMC umc in loadedFeatures)
                    {                        
                        umc.Net = Convert.ToDouble(umc.mint_scan - minScan) / Convert.ToDouble(maxScan - minScan);
                        umc.mint_umc_index = umcIndex++;
                    }                   
                    break;
                case ".DB3": 
                    NHibernateUtil.SetDbLocationForRead(dataset.mstrLocalPath);
                    try
                    {
                        UmcDAOHibernate umcDAOHibernate = new UmcDAOHibernate();
                        loadedFeatures = umcDAOHibernate.FindAll().ToArray();                        
                    }
                    catch (NHibernate.ADOException adoException)
                    {
                        loadedFeatures = new clsUMC[0];
                    }

                    // If no UMCs were loaded from the SQLite DB, then we need to create UMCs using MSFeature data from the DB
                    if (loadedFeatures.Length < 1)
                    {
                        MSFeatureDAOHibernate msFeatureDAOHibernate = new MSFeatureDAOHibernate();
                        clsIsotopePeak[] msFeatureArray = msFeatureDAOHibernate.FindAll().ToArray();

                        umcFinder.SetIsotopePeaks(ref msFeatureArray);
                        umcFinder.FindUMCs();
                        loadedFeatures = umcFinder.GetUMCs();
                    }
                    break;
                case ".SQLITE":                    
                    NHibernateUtil.SetDbLocationForRead(dataset.mstrLocalPath);
                    try
                    {
                        UmcDAOHibernate umcDAOHibernate = new UmcDAOHibernate();
                        loadedFeatures = umcDAOHibernate.FindAll().ToArray();                        
                    }                        
                    catch (NHibernate.ADOException)
                    {
                        loadedFeatures = new clsUMC[0];
                    }

                    // If no UMCs were loaded from the SQLite DB, then we need to create UMCs using MSFeature data from the DB
                    if (loadedFeatures.Length < 1)
                    {
                        MSFeatureDAOHibernate msFeatureDAOHibernate = new MSFeatureDAOHibernate();
                        clsIsotopePeak[] msFeatureArray = msFeatureDAOHibernate.FindAll().ToArray();

                        umcFinder.SetIsotopePeaks(ref msFeatureArray);
                        umcFinder.FindUMCs();
                        loadedFeatures = umcFinder.GetUMCs();
                    }
                    break;                  
                case ".PEK":
                    // Else we are using a PEK or CSV file
                    umcFinder.FileName = dataset.mstrLocalPath;
                    umcFinder.LoadUMCs(true);
                    umcFinder.FindUMCs();
                    loadedFeatures = umcFinder.GetUMCs();
                    break;
                case ".CSV":
                    umcFinder.FileName = dataset.mstrLocalPath;
                    umcFinder.LoadUMCs(false);
                    umcFinder.FindUMCs();
                    loadedFeatures = umcFinder.GetUMCs();
                    break;
                default:
                    throw new ArgumentException("Incorrect extension for file. Please use pek or csv files as inputs.");                                                                        
            }


            List<clsUMC> features = new List<clsUMC>();
            features.AddRange(loadedFeatures);
            // Set the Dataset ID number for each UMC
            foreach (clsUMC umc in features)
            {
                umc.DatasetId = Convert.ToInt32(dataset.DatasetId);
            }    

            return features;                                     
        }
        #endregion

        #region Individual Analysis Methods
        /// <summary>
        /// Aligns a dataset to a mass tag database.
        /// </summary>
        /// <param name="massTagDatabase"></param>
        /// <param name="features"></param>
        /// <param name="alignmentOptions"></param>
        /// <param name="boundaries"></param>
        /// <returns></returns>
        public classAlignmentData AlignDataset(clsMassTagDB                    massTagDatabase,
                                               List<clsUMC>                    features,
                                               clsAlignmentOptions             alignmentOptions)
        {                        
            clsAlignmentProcessor alignmentProcessor    = new clsAlignmentProcessor();
            alignmentProcessor.AlignmentOptions         = alignmentOptions;
            alignmentProcessor.SetReferenceDatasetFeatures(massTagDatabase); 
            classAlignmentData data =  AlignDataset(alignmentProcessor,
                                features,   
                                alignmentOptions);

            alignmentProcessor.Dispose();
            return data;
        }           
        /// <summary>
        /// Aligns a dataset to a dataset
        /// </summary>
        /// <param name="baselineFeatures"></param>
        /// <param name="features"></param>
        /// <param name="alignmentOptions"></param>
        /// <param name="boundaries"></param>
        /// <returns></returns>
        public classAlignmentData AlignDataset( List<clsUMC>                    baselineFeatures, 
                                                List<clsUMC>                    features,
                                                clsAlignmentOptions             alignmentOptions)
        {
            clsAlignmentProcessor alignmentProcessor    = new clsAlignmentProcessor();
            alignmentProcessor.AlignmentOptions         = alignmentOptions;
            
            alignmentProcessor.SetReferenceDatasetFeatures(baselineFeatures);
            classAlignmentData alignmentData            = AlignDataset( alignmentProcessor,
                                                                        features,
                                                                        alignmentOptions);

            int minScanReference = int.MaxValue;
            int maxScanReference = int.MinValue;
            foreach(clsUMC feature in baselineFeatures)
            {
                minScanReference = Math.Min(minScanReference, feature.Scan);
                maxScanReference = Math.Max(maxScanReference, feature.Scan);
            }

            alignmentData.maxMTDBNET = maxScanReference;
            alignmentData.minMTDBNET = minScanReference;

            alignmentProcessor.Dispose();

            return alignmentData;
        }
        /// <summary>
        /// Aligns the dataset to the data stored in the alignment processor.
        /// </summary>
        private classAlignmentData AlignDataset(clsAlignmentProcessor                 alignmentProcessor,            
                                                List<clsUMC>                          features,
                                                clsAlignmentOptions                   alignmentOptions)
        {    
            
            List<clsAlignmentFunction> alignmentFunctions   = new List<clsAlignmentFunction>();
            List<double[,]> netErrorHistograms              = new List<double[,]>();
            List<double[,]> massErrorHistograms             = new List<double[,]>();
            List<classAlignmentData> alignmentData          = new List<classAlignmentData>();
            List<float[,]> heatScores                       = new List<float[,]>();
            List<float[]> xIntervals                        = new List<float[]>();
            List<float[]> yIntervals                        = new List<float[]>();

            float minMTDBNET = 0.0F;
            float maxMTDBNET = 1.0F;
            alignmentProcessor.GetReferenceNETRange(ref minMTDBNET, ref maxMTDBNET);

            int minScanBaseline = int.MaxValue;
            int maxScanBaseline = int.MinValue;

            // Max the split boundaries (m/z) at 2.            
            int totalBoundaries = 1;
            if (alignmentOptions.SplitAlignmentInMZ == true)
            {
                totalBoundaries = 2;
            }

            for (int i = 0; i < totalBoundaries; i++)
            {                
                // Set features                
                alignmentProcessor.SetAligneeDatasetFeatures(features, alignmentOptions.MZBoundaries[i]);                

                // Find alignment 
                alignmentProcessor.PerformAlignmentToMSFeatures();

                // Extract alignment function
                clsAlignmentFunction alignmentFunction = alignmentProcessor.GetAlignmentFunction();
                alignmentFunctions.Add(alignmentFunction);

                // Correct the features
                alignmentProcessor.ApplyNETMassFunctionToAligneeDatasetFeatures(ref features);
                
                // Find min/max scan for meta-data
                int tempMinScanBaseline = int.MaxValue;
                int tempMaxScanBaseline = int.MinValue;
                foreach (clsUMC feature in features)
                {
                    tempMaxScanBaseline     = Math.Max(tempMaxScanBaseline, feature.Scan);
                    tempMinScanBaseline     = Math.Min(tempMinScanBaseline, feature.Scan);
                }
                
                minScanBaseline = Math.Min(minScanBaseline, tempMinScanBaseline);
                maxScanBaseline = Math.Max(maxScanBaseline, tempMaxScanBaseline);

                // Pull out the heat maps...
                float[,] heatScore = new float[1, 1];
                float[] xInterval = new float[1];
                float[] yInterval = new float[1];
                alignmentProcessor.GetAlignmentHeatMap(ref heatScore, ref xInterval, ref yInterval);

                xIntervals.Add(xInterval);
                yIntervals.Add(yInterval);
                heatScores.Add(heatScore);

                // Mass and net error histograms!  
                double[,] massErrorHistogram = new double[1, 1];
                double[,] netErrorHistogram = new double[1, 1];

                alignmentProcessor.GetErrorHistograms(alignmentOptions.MassBinSize,
                                                            alignmentOptions.NETBinSize,
                                                            ref massErrorHistogram,
                                                            ref netErrorHistogram);
                massErrorHistograms.Add(massErrorHistogram);
                netErrorHistograms.Add(netErrorHistogram);

                // Get the residual data from the warp.
                float[,] linearNet              = new float[1, 1];
                float[,] customNet              = new float[1, 1];
                float[,] linearCustomNet        = new float[1, 1];
                float[,] massError              = new float[1, 1];
                float[,] massErrorCorrected     = new float[1, 1];
                float[,] mzMassError            = new float[1, 1];
                float[,] mzMassErrorCorrected   = new float[1, 1];
                classAlignmentResidualData residualData = alignmentProcessor.GetResidualData();
               
                // Set all of the data now 
                classAlignmentData data     = new classAlignmentData();
                data.massErrorHistogram     = massErrorHistogram;
                data.netErrorHistogram      = netErrorHistogram;                
                data.alignmentFunction      = alignmentFunction;
                data.heatScores             = heatScore;
                data.minScanBaseline        = minScanBaseline;
                data.maxScanBaseline        = maxScanBaseline;
                data.NETIntercept           = alignmentProcessor.NETIntercept;
                data.NETRsquared            = alignmentProcessor.NETLinearRSquared;
                data.NETSlope               = alignmentProcessor.NETSlope;
                data.ResidualData           = residualData;
                data.MassMean               = alignmentProcessor.GetMassMean();
                data.MassStandardDeviation  = alignmentProcessor.GetMassStandardDeviation();
                data.NETMean                = alignmentProcessor.GetNETMean();
                data.NETStandardDeviation   = alignmentProcessor.GetNETStandardDeviation();

                // Find out the max scan or NET value to use for the range depending on what 
                // type of baseline dataset it was (MTDB or dataset).                 
                if (alignmentOptions.IsAlignmentBaselineAMasstagDB == true)
                {
                    data.minMTDBNET = minMTDBNET;
                    data.maxMTDBNET = maxMTDBNET;
                }

                alignmentData.Add(data);                
            }

            classAlignmentData mergedData = new classAlignmentData();
            clsAlignmentFunction mergedAlignmentFunction = new clsAlignmentFunction(enmCalibrationType.HYBRID_CALIB, enmAlignmentType.NET_MASS_WARP);
            float[,] mergedHeatScores = new float[1, 1];

            /// ////////////////////////////////////////////////////////////
            /// Merge the mass error histogram data.
            /// ////////////////////////////////////////////////////////////
            int maxMassHistogramLength = 0;
            int maxNetHistogramLength = 0;
            for (int i = 0; i < alignmentData.Count; i++)
            {
                maxMassHistogramLength = Math.Max(maxMassHistogramLength, alignmentData[0].massErrorHistogram.GetLength(0));
                maxNetHistogramLength = Math.Max(maxNetHistogramLength, alignmentData[0].netErrorHistogram.GetLength(0));
            }

            double[,] massErrorHistogramData = new double[maxMassHistogramLength, 2];
            MergeHistogramData(massErrorHistogramData, alignmentData[0].massErrorHistogram, false);

            /// 
            /// The residual arrays are the same size, here we start the process to count the 
            /// size so we can merge all of the results back into one array.
            /// 
            int countMassResiduals = 0;
            int countNETResiduals = 0;

            for (int i = 0; i < alignmentData.Count; i++)
            {
                if (i > 0)
                    MergeHistogramData(massErrorHistogramData, alignmentData[i].massErrorHistogram, true);

                countMassResiduals += alignmentData[i].ResidualData.mz.Length;
                countNETResiduals += alignmentData[i].ResidualData.scans.Length;
            }

            /// //////////////////////////////////////////////////////////// 
            /// Merge:
            ///     NET error histogram data
            ///     Mass Residual Data            
            /// ////////////////////////////////////////////////////////////
            double[,] netErrorHistogramData = new double[maxNetHistogramLength, 2];
            MergeHistogramData(netErrorHistogramData, alignmentData[0].netErrorHistogram, false);

            mergedData.ResidualData = new classAlignmentResidualData();
            mergedData.ResidualData.customNet = new float[countNETResiduals];
            mergedData.ResidualData.linearCustomNet = new float[countNETResiduals];
            mergedData.ResidualData.linearNet = new float[countNETResiduals];
            mergedData.ResidualData.scans = new float[countNETResiduals];
            mergedData.ResidualData.massError = new float[countMassResiduals];
            mergedData.ResidualData.massErrorCorrected = new float[countMassResiduals];
            mergedData.ResidualData.mz = new float[countMassResiduals];
            mergedData.ResidualData.mzMassError = new float[countMassResiduals];
            mergedData.ResidualData.mzMassErrorCorrected = new float[countMassResiduals];


            int copyNETBlocks = 0;
            int copyMassBlocks = 0;

            for (int i = 0; i < alignmentData.Count; i++)
            {
                /// 
                /// Merge the residual data
                /// 
                alignmentData[i].ResidualData.customNet.CopyTo(mergedData.ResidualData.customNet, copyNETBlocks);
                alignmentData[i].ResidualData.linearCustomNet.CopyTo(mergedData.ResidualData.linearCustomNet, copyNETBlocks);
                alignmentData[i].ResidualData.linearNet.CopyTo(mergedData.ResidualData.linearNet, copyNETBlocks);
                alignmentData[i].ResidualData.scans.CopyTo(mergedData.ResidualData.scans, copyNETBlocks);

                alignmentData[i].ResidualData.massError.CopyTo(mergedData.ResidualData.massError, copyMassBlocks);
                alignmentData[i].ResidualData.massErrorCorrected.CopyTo(mergedData.ResidualData.massErrorCorrected, copyMassBlocks);
                alignmentData[i].ResidualData.mzMassError.CopyTo(mergedData.ResidualData.mzMassError, copyMassBlocks);
                alignmentData[i].ResidualData.mzMassErrorCorrected.CopyTo(mergedData.ResidualData.mzMassErrorCorrected, copyMassBlocks);
                alignmentData[i].ResidualData.mz.CopyTo(mergedData.ResidualData.mz, copyMassBlocks);

                copyNETBlocks += alignmentData[i].ResidualData.scans.Length;
                copyMassBlocks += alignmentData[i].ResidualData.mz.Length;

                mergedData.NETMean = alignmentData[i].NETMean;
                mergedData.MassMean = alignmentData[i].MassMean;
                mergedData.MassStandardDeviation = alignmentData[i].MassStandardDeviation;
                mergedData.NETStandardDeviation = alignmentData[i].NETStandardDeviation;

                if (i > 0)
                    MergeHistogramData(netErrorHistogramData, alignmentData[i].netErrorHistogram, true);
            }

            /// 
            /// Grab the heat scores!
            mergedData.heatScores           = alignmentData[alignmentData.Count - 1].heatScores;
            mergedData.massErrorHistogram   = massErrorHistogramData;
            mergedData.netErrorHistogram    = netErrorHistogramData;

            alignmentProcessor.Dispose();

            return mergedData;

        }
        /// <summary>
        /// Merges the histogram data leaving the result in old.
        /// </summary>
        /// <param name="histogramOld">Data to retain merged data.</param>
        /// <param name="histogramNew">Data to copy.</param>
        /// <param name="checkClosestBin">Flag indicating whether to use the closest bin or to just assume that the x values match between dest and src.</param>
        private void MergeHistogramData(double[,] histogramDest, 
                                        double[,] histogramSource, 
                                        bool checkClosestBin)
        {
            for (int i = 0; i < histogramSource.GetLength(0) && i < histogramDest.GetLength(0); i++)
            {
                int bestIndex = 0;
                double massDiff = double.MaxValue;

                if (checkClosestBin == false)
                {
                    bestIndex = i;
                    histogramDest[i, 0] = histogramSource[bestIndex, 0];
                }
                else
                {
                    int length = Math.Min(histogramDest.GetLength(0), histogramSource.GetLength(0));

                    /// 
                    /// Find the best mass item if the previous mass items are skewed or changed
                    /// 
                    for (int j = 0; j < length; j++)
                    {
                        double diff = Math.Abs(histogramDest[j, 0] - histogramSource[j, 0]);
                        if (diff < massDiff)
                        {
                            bestIndex = j;
                            massDiff = diff;
                        }
                    }
                }
                histogramDest[i, 1] += histogramSource[bestIndex, 1];
            }
        }       
        /// <summary>
        /// Aligns the clusters to the mass tag database
        /// </summary>
        /// <param name="massTagDatabase">Mass tag databaset to align to.</param>
        /// <param name="clusters">Clusters to align.</param>
        /// <param name="options">Alignment options.</param>
        /// <returns>Alignment data for the clusters to mass tag database.</returns>
        public classAlignmentData AlignClustersToMassTagDB( clsMassTagDB          massTagDatabase,
                                                            clsClusterData        clusters,
                                                            clsAlignmentOptions   options)
        {            
            clsAlignmentProcessor alignmentProcessor = new clsAlignmentProcessor();
            alignmentProcessor.SetReferenceDatasetFeatures(massTagDatabase);
            alignmentProcessor.SetAligneeDatasetFeatures(clusters, options.MZBoundaries[0]);

            alignmentProcessor.PerformAlignmentToMassTagDatabase();
            alignmentProcessor.ApplyNETMassFunctionToAligneeDatasetFeatures(ref clusters); 
            clsAlignmentFunction alignmentFunction = alignmentProcessor.GetAlignmentFunction();

            // Heat maps
            float[,] heatScores = new float[1, 1];
            float[] xIntervals  = new float[1];
            float[] yIntervals  = new float[1];
            alignmentProcessor.GetAlignmentHeatMap(ref heatScores, ref xIntervals, ref yIntervals);

            // Ranges
            float minClusterNET = 0.0F, maxClusterNET = 1.0F;
            clusters.GetMinMaxNET(ref minClusterNET, ref maxClusterNET);
            float minMTDBNET    = 0.0F, maxMTDBNET = 1.0F;
            alignmentProcessor.GetReferenceNETRange(ref minMTDBNET, ref maxMTDBNET);


            // Residuals
            float[,] linearNet = new float[1, 1];
            float[,] customNet = new float[1, 1];
            float[,] linearCustomNet = new float[1, 1];
            float[,] massError = new float[1, 1];
            float[,] massErrorCorrected = new float[1, 1];
            float[,] mzMassError = new float[1, 1];
            float[,] mzMassErrorCorrected = new float[1, 1];
            classAlignmentResidualData residualData = alignmentProcessor.GetResidualData();

            // Get error histograms 
            double[,] netErrorHistogram  =  new double[1, 1];
            double[,] massErrorHistogram = new double[1, 1];
            alignmentProcessor.GetErrorHistograms(options.MassBinSize,
                                                  options.NETBinSize,
                                                  ref massErrorHistogram,
                                                  ref netErrorHistogram);

            classAlignmentData  clusterAlignmentData    = new classAlignmentData();
            clusterAlignmentData.alignmentFunction      = alignmentFunction;
            clusterAlignmentData.heatScores             = heatScores;
            clusterAlignmentData.massErrorHistogram     = massErrorHistogram;
            clusterAlignmentData.netErrorHistogram      = netErrorHistogram;
            clusterAlignmentData.NETIntercept           = alignmentProcessor.NETIntercept;
            clusterAlignmentData.NETRsquared            = alignmentProcessor.NETLinearRSquared;
            clusterAlignmentData.NETSlope               = alignmentProcessor.NETSlope;
            clusterAlignmentData.ResidualData           = residualData;
            clusterAlignmentData.MassMean               = alignmentProcessor.GetMassMean();
            clusterAlignmentData.MassStandardDeviation  = alignmentProcessor.GetMassMean();
            clusterAlignmentData.NETMean                = alignmentProcessor.GetNETMean();
            clusterAlignmentData.NETStandardDeviation   = alignmentProcessor.GetNETStandardDeviation();

            return clusterAlignmentData;
        }        
        /// <summary>
        /// Aligns all of the datasets.
        /// </summary>
        public void AlignDatasets(MultiAlignAnalysis analysis)
        {
            // Connect to database of features.                
            UmcDAOHibernate featureCache = new UmcDAOHibernate();
            
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
                    analysis.MassTagDatabase = LoadMassTagDB(analysis.MassTagDBOptions);
                }
            }
            
            // Align pairwise and cache results intermediately.
            
            for (int datasetNum = 0; datasetNum < m_analysis.Datasets.Count; datasetNum++)
            {
                if (datasetNum != baselineDatasetID)
                {                        
                    DatasetInformation  datasetInfo      = m_analysis.Datasets[datasetNum];
                    List<clsUMC>        features         = featureCache.FindByDatasetId(datasetNum);
                    classAlignmentData  alignmentData    = null;

                    // We dont track the UMC Index...this was used previously by the UMC Data object :(
                    // Here we are applying a temp fix to iterate through the features and assign its unique ID.                    
                    int featureIDIndex = 0; 
                    foreach (clsUMC feature in features)
                    {
                        feature.mint_umc_index = featureIDIndex++;
                    }

                    if (baselineInfo != null)
                    {
                        alignmentData = AlignDataset(baselineFeatures,
                                                     features,
                                                     analysis.AlignmentOptions[datasetNum]);
                    }
                    else
                    {
                        alignmentData = AlignDataset(analysis.MassTagDatabase,
                                                     features,
                                                     analysis.AlignmentOptions[datasetNum]);
                    }
                    alignmentData.aligneeDataset       = datasetInfo.DatasetName;
                    analysis.AlignmentData.Add(alignmentData);

                    featureCache.UpdateAll(features);

                    NHibernateUtil.CloseSession();

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
        /// For single datasets this will convert the umc's into a cluster.
        /// </summary>
        /// <param name="umcdata"></param>
        /// <param name="index"></param>
        private clsClusterData ConstructClustersFromDataset(List<clsUMC> umcData, int index)
        {            
            int i = 0;
            clsClusterData clusters                 = new clsClusterData();
            clusters.marrClusterIntensity           = new double[umcData.Count];
            clusters.marrClusterMainMemberIndex     = new int[umcData.Count];
            
            double minNET = double.MaxValue;
            double maxNET = double.MinValue;
            foreach (clsUMC umc in umcData)
            {
                clsCluster cluster              = new clsCluster();
                cluster.mdouble_mass_calibrated = umc.MassCalibrated;
                cluster.mdouble_mass            = umc.Mass;
                cluster.mdouble_net             = umc.Net;
                cluster.mshort_charge           = umc.mshort_class_rep_charge;
                cluster.mint_scan               = umc.ScanAligned;

                minNET = Math.Min(cluster.Net, minNET);
                maxNET = Math.Max(cluster.Net, maxNET);

                cluster.MemberCount        = 1;
                cluster.DatasetMemberCount = 1;
                clusters.marrClusterMainMemberIndex[i]  = i;
                clusters.marrClusterIntensity[i]        = umc.AbundanceSum;
                cluster.mint_cluster_index              = i++;
                cluster.mdouble_driftTime               = umc.DriftTime;
                clusters.AddCluster(cluster);
            }
            clusters.mdblMinNET = minNET;
            clusters.mdblMaxNET = maxNET;

            return clusters;
        }
        /// <summary>
        /// Creates a mammoth database to use in clustering.
        /// </summary>
        private int CreateMammothDatabase(MultiAlignAnalysis analysis,
                                            UmcDAOHibernate featureCache,
                                            string databaseName,
                                            int chargeStateFilter)
        {
            if (File.Exists(databaseName))
            {
                System.IO.File.Delete(databaseName);
            }
            
            // Create the database file.
            System.Data.SQLite.SQLiteConnection.CreateFile(databaseName);
            using (System.Data.SQLite.SQLiteConnection connection =
                new System.Data.SQLite.SQLiteConnection("Data Source=" + databaseName + ";"))
            {
                using (System.Data.SQLite.SQLiteCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandText = "CREATE TABLE T_CLUSTERS( id INTEGER PRIMARY KEY,  mass DOUBLE, net DOUBLE, drift_time DOUBLE, internal INTEGER)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE T_FEATURES(id INTEGER, mass DOUBLE, net DOUBLE, drift_time DOUBLE, dataset_id INTEGER, cluster INTEGER, PRIMARY KEY(id, dataset_id))";
                    command.ExecuteNonQuery();
                }
            }

            int totalFeatures = 0;
            // Add features into database.
            using (Mammoth.Data.MammothDatabase database = new Mammoth.Data.MammothDatabase(databaseName))
            {
                database.Connect();
                foreach (DatasetInformation info in analysis.Datasets)
                {
                    List<clsUMC> datasetFeatures = featureCache.FindByDatasetId(Convert.ToInt32(info.DatasetId));
                    
                    List<PNNLOmics.Data.Features.UMCLight> tempFeatures = new List<PNNLOmics.Data.Features.UMCLight>();
                    foreach (clsUMC umc in datasetFeatures)
                    {
                        totalFeatures++;
                        PNNLOmics.Data.Features.UMCLight feature = new PNNLOmics.Data.Features.UMCLight();
                        feature.Abundance        = umc.AbundanceSum;
                        feature.ChargeState      = umc.ChargeRepresentative;
                        feature.DriftTime        = umc.DriftTime;
                        feature.GroupID          = umc.DatasetId;
                        feature.ID               = umc.Id;
                        feature.MassMonoisotopic = umc.MassCalibrated;
                        feature.NET              = umc.Net;
                        if (chargeStateFilter < 0 || chargeStateFilter == feature.ChargeState)
                        {
                            tempFeatures.Add(feature);
                        }
                    }
                    database.AddFeatures(tempFeatures);
                }
                database.Close();
            }

            return totalFeatures;
        }
        //BLL HACK BECAUSE THE HIBERNATE SHIT SUCKS
        //private Dictionary<int, Dict
        /// <summary>
        /// Updates the feature cache with the appropiate cluster ID's.
        /// </summary>
        private void UpdateFeatureCacheWithClusters(UmcDAOHibernate featureCache,
                                                    List<Mammoth.Data.MammothCluster> clusters)            
        {            
            Dictionary<int, int> allFeatureIDsToClusterID = new Dictionary<int, int>();
            Dictionary<int, int> featureIDToClusterID = new Dictionary<int, int>();
            List<int> ids = new List<int>();

            foreach (Mammoth.Data.MammothCluster cluster in clusters)
            {
                foreach (PNNLOmics.Data.Features.UMCLight feature in cluster.UMCList)
                {
                    int id = feature.ID;
                    //clsUMC umc = featureCache.FindByFeatureID(id);

                    featureIDToClusterID.Add(id, cluster.ID);
                    allFeatureIDsToClusterID.Add(id, cluster.ID);
                    ids.Add(id);                                                            
                    if (ids.Count > FEATURE_CLUSTER_LINK_CACHE_SIZE)
                    {
                        List<clsUMC> umcs = featureCache.FindByFeatureID(ids);                        
                        foreach (clsUMC umc in umcs)
                        {
                            umc.ClusterId = featureIDToClusterID[umc.Id];
                        }
                        featureCache.UpdateAll(umcs);
                        umcs.Clear();
                        ids.Clear();
                        featureIDToClusterID.Clear();
                    }
                }
            }
            if (ids.Count > 0)
            {
                List<clsUMC> umcs = featureCache.FindByFeatureID(ids);
                foreach (clsUMC umc in umcs)
                {
                    umc.ClusterId = featureIDToClusterID[umc.Id];
                }
                featureCache.UpdateAll(umcs);
                umcs.Clear();
                ids.Clear();
                featureIDToClusterID.Clear();             
            }


            //List<clsUMC> storedFeatures = featureCache.FindAll();
            //foreach (clsUMC umc in storedFeatures)
            //{
            //    if (umc.ClusterId > -1 && allFeatureIDsToClusterID.ContainsKey(umc.Id))
            //    {
            //        int id = allFeatureIDsToClusterID[umc.Id];
            //        if (id != umc.ClusterId)
            //        {
            //            throw new Exception("UMC Cluster ID does not match ID");
            //        }
            //    }
            //}
        }
        /// <summary>
        /// Clusters using the original single linkage algorithm.
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>        
        public List<clsCluster> PerformClustering(MultiAlignAnalysis analysis)
        {
            LinkageClustererBase<Mammoth.Data.MammothCluster> clusterer = null;
            switch(m_clusterType)
            {
                case ClusteringAlgorithmType.Centroid:
                    clusterer = new UMCCentroidClusterer<Mammoth.Data.MammothCluster>();
                    break;
                case ClusteringAlgorithmType.AverageLinkage:                    
                    clusterer = new UMCAverageLinkageClusterer<Mammoth.Data.MammothCluster>();    
                    break;
                case ClusteringAlgorithmType.SingleLinkage:
                    clusterer = new UMCSingleLinkageClusterer<Mammoth.Data.MammothCluster>();
                    break;            
            }

            //TODO: Fix, this happens twice.  
            // Tolerances
            PNNLOmics.Algorithms.FeatureTolerances tolerances = new PNNLOmics.Algorithms.FeatureTolerances();
            tolerances.DriftTime     = analysis.ClusterOptions.DriftTimeTolerance;
            tolerances.Mass          = analysis.ClusterOptions.MassTolerance;
            tolerances.RetentionTime = analysis.ClusterOptions.NETTolerance;

            // Parameters 
            PNNLOmics.Algorithms.FeatureClustering.FeatureClusterParameters parameters = new PNNLOmics.Algorithms.FeatureClustering.FeatureClusterParameters();
            parameters.CentroidRepresentation = PNNLOmics.Data.Features.ClusterCentroidRepresentation.Mean;
            if (analysis.ClusterOptions.ClusterRepresentativeType == enmClusterRepresentativeType.MEDIAN)
            {
                parameters.CentroidRepresentation = PNNLOmics.Data.Features.ClusterCentroidRepresentation.Median;
            }
            parameters.Tolerances = tolerances;
            parameters.OnlyClusterSameChargeStates = (analysis.ClusterOptions.IgnoreCharge == false);
            List<clsCluster> clusters = new List<clsCluster>();

            clusterer.Parameters = parameters;
            return PerformClustering(analysis, clusterer);                                          
        }
        void FindWeights(MultiAlignAnalysis analysis, UmcDAOHibernate cache, ref double massWeight, ref double netWeight, ref double driftWeight)
        {
            //foreach (DatasetInformation info in analysis.Datasets)
            //{
            //    List<clsUMC> datasetFeatures = featureCache.FindByDatasetId(Convert.ToInt32(info.DatasetId));

            //    List<PNNLOmics.Data.Features.UMCLight> tempFeatures = new List<PNNLOmics.Data.Features.UMCLight>();
            //    foreach (clsUMC umc in datasetFeatures)
            //    {
            //        PNNLOmics.Data.Features.UMCLight feature = new PNNLOmics.Data.Features.UMCLight();
            //        feature.Abundance = umc.AbundanceSum;
            //        feature.ChargeState = umc.ChargeRepresentative;
            //        feature.DriftTime = umc.DriftTime;
            //        feature.GroupID = umc.DatasetId;
            //        feature.ID = umc.Id;
            //        feature.MassMonoisotopic = umc.MassCalibrated;
            //        feature.NET = umc.Net;
            //        if (chargeStateFilter < 0 || chargeStateFilter == feature.ChargeState)
            //        {
            //            tempFeatures.Add(feature);
            //        }
            //    }
            //    database.AddFeatures(tempFeatures);
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="clusterer"></param>
        /// <returns></returns>
        public List<clsCluster> PerformClustering(  MultiAlignAnalysis analysis,
                                                    IClusterer<UMCLight, Mammoth.Data.MammothCluster> clusterer)
        {                        
            // Cluster Data!      
            UmcDAOHibernate featureCache            = new UmcDAOHibernate();
            UmcClusterDAOHibernate clusterCache     = new UmcClusterDAOHibernate();            
            Mammoth.Data.MammothDatabaseRange range = new Mammoth.Data.MammothDatabaseRange(-1,
                                                                                            30000,
                                                                                            -1.5,
                                                                                            1.5,
                                                                                            0, 
                                                                                            70, 
                                                                                            false);

            UpdateStatus(clusterer.ToString());

            // Tolerances
            PNNLOmics.Algorithms.FeatureTolerances tolerances = new PNNLOmics.Algorithms.FeatureTolerances();
            tolerances.DriftTime            = analysis.ClusterOptions.DriftTimeTolerance;
            tolerances.Mass                 = analysis.ClusterOptions.MassTolerance;
            tolerances.RetentionTime        = analysis.ClusterOptions.NETTolerance;

            double massWeight, netWeight, driftWeight = 0;
            //FindWeights(featureCache, ref massWeight, ref netWeight, ref driftWeight);
            
            // Parameters 
            PNNLOmics.Algorithms.FeatureClustering.FeatureClusterParameters parameters = new PNNLOmics.Algorithms.FeatureClustering.FeatureClusterParameters();
            parameters.CentroidRepresentation = PNNLOmics.Data.Features.ClusterCentroidRepresentation.Mean;
            if (analysis.ClusterOptions.ClusterRepresentativeType == enmClusterRepresentativeType.MEDIAN)
            {
                parameters.CentroidRepresentation = PNNLOmics.Data.Features.ClusterCentroidRepresentation.Median;
            }
            parameters.Tolerances                   = tolerances;
            
            parameters.OnlyClusterSameChargeStates  = (analysis.ClusterOptions.IgnoreCharge == false);
            List<clsCluster> clusters               = new List<clsCluster>();

            int id = 0;
            // Group all charge states            
            string databaseName = "mammothDatabase.db3";
            if (analysis.ClusterOptions.IgnoreCharge)
            {
                UpdateStatus("Clustering all charge states.");

                //TODO: Refactor this into it's own sub-method.
                CreateMammothDatabase(analysis, featureCache, databaseName, -1);                    
                
                // Cluster 
                Mammoth.Algorithms.MammothClusterer processor = new Mammoth.Algorithms.MammothClusterer();
                processor.ClusterDatabase(databaseName, analysis.ClusterOptions.RecursionLevels, parameters, range, parameters.Tolerances, clusterer);
                
                // Retrieve clusters 
                using (Mammoth.Data.MammothDatabase database = new Mammoth.Data.MammothDatabase(databaseName))
                {

                    Mammoth.Data.MammothDatabaseRange retrievalRange = new Mammoth.Data.MammothDatabaseRange(-1,
                                                                                        30000,
                                                                                        -1.5,
                                                                                        1.5,
                                                                                        0,
                                                                                        62,
                                                                                        false);

                    database.Connect();

                    List<Mammoth.Data.MammothCluster> mammothClusters   = new List<Mammoth.Data.MammothCluster>();
                    Mammoth.Data.MammothDatabaseRange newOptions        = new Mammoth.Data.MammothDatabaseRange(
															                            range.MassMinimum,
															                            range.MassMaximum,
															                            range.NETMinimum,
															                            range.NETMaximum,
															                            range.DriftTimeMinimum,
															                            range.DriftTimeMaximum,
															                            true);                    


				    // After the last go around there may be some clusters that were not clustered
				    // or are marked as internal.  Clean up the slack and remove those 
				    // clusters.

				    newOptions.MassMaximum			+= 50000;
				    newOptions.MassMinimum			-= 50000;
				    newOptions.NETMaximum			+= 20;
				    newOptions.NETMinimum			-= 20;
				    newOptions.DriftTimeMaximum		+= 500;
				    newOptions.DriftTimeMinimum		-= 500;
                    
                    mammothClusters                  = database.GetClusters(newOptions);

                    List<clsCluster> tempClusters = new List<clsCluster>();

                    UpdateStatus(string.Format("Found {0} clusters.", tempClusters.Count));

                    foreach(Mammoth.Data.MammothCluster mammothCluster in mammothClusters)
                        {
                            mammothCluster.CalculateStatistics(parameters.CentroidRepresentation);
                            mammothCluster.ID   = id++; 

                            clsCluster cluster  = new clsCluster();
                            cluster.Charge      = mammothCluster.ChargeState;
                            cluster.DriftTime   = mammothCluster.DriftTime;
                            cluster.Id          = mammothCluster.ID;
                            cluster.Mass        = mammothCluster.MassMonoisotopic;
                            cluster.Net         = mammothCluster.NET;
                            cluster.MemberCount = mammothCluster.UMCList.Count;
                            Dictionary<int, int> membercounts = new Dictionary<int, int>();
                            int totalKeys = 0;
                            foreach (UMCLight feature in mammothCluster.UMCList)
                            {
                                int key = feature.GroupID;
                                bool contains = membercounts.ContainsKey(key);
                                if (!contains)
                                {
                                    totalKeys++;
                                    membercounts.Add(key, 0);
                                }
                                membercounts[key]++;
                            }
                            cluster.DatasetMemberCount = totalKeys;
                            cluster.MeanScore   = mammothCluster.Score;
                            cluster.MedianScore = mammothCluster.Score;
                            tempClusters.Add(cluster);
                        }
                    clusterCache.AddAll(tempClusters);
                    UpdateFeatureCacheWithClusters(featureCache, mammothClusters); 
                    database.Close();
                    clusters.AddRange(tempClusters);
                }
            }
            else
            {                
                // Group single charge states together only.
                
                for(int chargeState = 1; chargeState < 15; chargeState++)
                {
                    UpdateStatus(string.Format("Clustering Charge State = {0}.", chargeState));
                    List<clsCluster> tempClusters = new List<clsCluster>();
                    // Create database
                    int total = CreateMammothDatabase(analysis, featureCache, databaseName, chargeState);

                    if (total <= 0)
                    {
                        continue;
                    }

                    // Cluster 
                    Mammoth.Algorithms.MammothClusterer processor = new Mammoth.Algorithms.MammothClusterer();
                    processor.ClusterDatabase(databaseName, analysis.ClusterOptions.RecursionLevels, parameters, range, parameters.Tolerances, clusterer); 
                   
                    // Retrieve clusters 
                    using (Mammoth.Data.MammothDatabase database = new Mammoth.Data.MammothDatabase(databaseName))
                    {
                        
                        Mammoth.Data.MammothDatabaseRange retrievalRange = new Mammoth.Data.MammothDatabaseRange(-1,
                                                                                            30000,
                                                                                            -1.5,
                                                                                            1.5,
                                                                                            0, 
                                                                                            62, 
                                                                                            false);

                        database.Connect();
                        
                        List<Mammoth.Data.MammothCluster> mammothClusters = new List<Mammoth.Data.MammothCluster>();
                        retrievalRange.MassMaximum          += 50000;
                        retrievalRange.MassMinimum          -= 50000;
                        retrievalRange.NETMaximum           += 20;
                        retrievalRange.NETMinimum           -= 20;
                        retrievalRange.DriftTimeMaximum     += 500;
                        retrievalRange.DriftTimeMinimum     -= 500;
                        retrievalRange.InternalCluster       = true;

                        mammothClusters = database.GetClusters(retrievalRange);

                        tempClusters.Clear();
                        foreach(Mammoth.Data.MammothCluster mammothCluster in mammothClusters)
                        {
                            mammothCluster.CalculateStatistics(parameters.CentroidRepresentation);
                            mammothCluster.ID   = id++; 

                            clsCluster cluster  = new clsCluster();
                            cluster.Charge      = mammothCluster.ChargeState;
                            cluster.DriftTime   = mammothCluster.DriftTime;
                            cluster.Id          = mammothCluster.ID;
                            cluster.Mass        = mammothCluster.MassMonoisotopic;
                            cluster.Net         = mammothCluster.NET;
                            cluster.MemberCount = mammothCluster.UMCList.Count;
                            Dictionary<int, int> membercounts = new Dictionary<int, int>();
                            int totalKeys = 0;
                            foreach (UMCLight feature in mammothCluster.UMCList)
                            {
                                int key = feature.GroupID;
                                bool contains = membercounts.ContainsKey(key);
                                if (!contains)
                                {
                                    totalKeys++;
                                    membercounts.Add(key, 0);
                                }
                                membercounts[key]++;
                            }
                            cluster.DatasetMemberCount = totalKeys;
                            cluster.MeanScore   = mammothCluster.Score;
                            cluster.MedianScore = mammothCluster.Score;
                            tempClusters.Add(cluster);
                        }

                        UpdateStatus(string.Format("Found {0} clusters.", tempClusters.Count));

                        clusterCache.AddAll(tempClusters);
                        UpdateFeatureCacheWithClusters(featureCache, mammothClusters);
                        database.Close();

                        clusters.AddRange(tempClusters);
                    }
                }
            }
            return clusters;
        }
        /// <summary>
        /// Performs the peak matching of UMC's to the MTDB and inherent scoring.
        /// </summary>
        public clsPeakMatchingResults PerformPeakMatching(List<clsCluster>        clusters,
                                                          clsMassTagDB            massTagDatabase,
                                                          clsPeakMatchingOptions  options,
                                                          double                  daltonShift)
        {
            clsPeakMatchingProcessor peakMatcher = new clsPeakMatchingProcessor();
            peakMatcher.MassTolerance            = options.MassTolerance;
            peakMatcher.NETTolerance             = options.NETTolerance;
            peakMatcher.DriftTimeTolerance       = options.DriftTimeTolerance;

            clsPeakMatchingResults peakMatchingResults = peakMatcher.PerformPeakMatching(  clusters,
                                                                                           massTagDatabase,
                                                                                           daltonShift);                                    
            return peakMatchingResults;
        }
        /// <summary>
        /// Calculates the SMART Scores if matched to a AMTDB 
        /// for peptide identification.
        /// </summary>
        public classSMARTResults PerformSMART(  List<clsCluster>   clusters,
                                                clsMassTagDB       massTagDatabase,
                                                classSMARTOptions  options)
        {            
            classSMARTProcessor processor = new classSMARTProcessor();

            // Load the mass tags
            List<classSMARTMassTag> massTags = new List<classSMARTMassTag>();
            int totalTags                    = massTagDatabase.GetMassTagCount();
            for (int i = 0; i < totalTags; i++)
            {
                clsMassTag tag = massTagDatabase.GetMassTagFromIndex(i);

                classSMARTMassTag msFeature = new classSMARTMassTag();
                msFeature.mdouble_monoMass  = tag.mdblMonoMass;
                msFeature.mdouble_NET       = tag.NetAverage;
                msFeature.mint_ID           = tag.mintMassTagId;
                msFeature.mint_count        = tag.mintNumObsPassingFilter;

                msFeature.mdouble_probability = tag.HighPeptideProphetProbability;
                massTags.Add(msFeature);
            }

            // Load the clusters
            int totalClusters                 = clusters.Count;
            List<classSMARTUMC> smartFeatures = new List<classSMARTUMC>();
            for (int i = 0; i < totalClusters; i++)
            {
                clsCluster cluster          = clusters[i];
                classSMARTUMC feature       = new classSMARTUMC();
                feature.mdouble_NET         = cluster.NetAligned;
                feature.mdouble_monoMass    = cluster.MassCalibrated;
                feature.mint_id             = i;
                smartFeatures.Add(feature);
            }

            classSMARTResults smartResults = processor.ScoreUMCMatches(massTags,
                                                                       smartFeatures,
                                                                       options);

            return smartResults;
        }
        /// <summary>
        /// Converts the SMART Results into peak matching results.
        /// </summary>
        /// <param name="smart">Results computed using SMART.</param>
        /// <returns>Peak matching results.</returns>
        public clsPeakMatchingResults ConvertSmartResultsToPeakResults(classSMARTResults smart,
                                                                       clsMassTagDB massTagDatabase,
                                                                       List<clsCluster> clusters)
        {
            /// 
            /// Create a new results object for holding peak matching data.
            /// 
            clsPeakMatchingResults results = new clsPeakMatchingResults();

            /// 
            /// Then, look through the UMC cluster keys, 
            /// and pull out the clusters and MTID's from each.
            /// 
            int[] smartKeys = smart.GetUMCMatchIndices();
            foreach (int key in smartKeys)
            {
                /// 
                /// Get the list of all the UMC' data that matched to MTID's
                /// 
                List<classSMARTProbabilityResult> umcMatches =
                    smart.GetResultFromUMCIndex(key);
                /// 
                /// Just in case, make sure we have result data here...
                /// 
                if (umcMatches != null)
                {
                    /// 
                    /// Then since we can have multiple MTID's 
                    /// match to UMC's,
                    /// pull out all possible probabilities and 
                    /// add to the result
                    /// matching data object to construct the 
                    /// protein to UMC - MTID relationships                    
                    /// 
                    /// First - we grab the cluster data, since 
                    /// it should be the same for all probability
                    /// matches
                    /// 
                    /// Second - we enumerate all the matches 
                    /// of umc cluster to mtid adding them
                    /// to the results object.
                    ///                     
                    clsCluster cluster = clusters[key];                        
                    foreach (classSMARTProbabilityResult probability in
                        umcMatches)
                    {
                        clsMassTag tag = massTagDatabase.GetMassTag(probability.MassTagID);
                        results.AddPeakMatchResult(tag, cluster, key);
                    }
                }
            }
            /// 
            /// Finally, now that we have matches, we pull
            /// out all of the protein
            /// information that was retrieved and stored 
            /// in the mass tag database object.            
            /// 
            results.ExtractProteinInformation(massTagDatabase);

            /// 
            /// And last, we finally tie together all of 
            /// the information.
            ///     MTID - Protein - UMC Cluster Index.  
            /// This method will generate the triplet 
            /// structures that are used to key off of each other.
            /// 
            results.UpdatePeakMatchArrays();
            return results;
        }
        #endregion      

        #region Full Analysis
        /// <summary>
        /// Starts a multi-Align analysis job.
        /// </summary>
        public void StartAnalysis(MultiAlignAnalysis analysis)
        {
            // Make sure we start with a fresh analysis.
            AbortAnalysisThread(m_analysisThread);

            m_analysis = analysis;

            ThreadStart threadStart  = new ThreadStart(PerformAnalysis);
            m_analysisThread         = new Thread(threadStart);
            m_analysisThread.Start();
        }
        private void UpdateStatus(string message)
        {
            if (Status != null)
            {
                Status(this, new AnalysisStatusEventArgs(message, 0));
            }
        }
        /// <summary>
        /// Starts the main analysis.
        /// </summary>
        private void PerformAnalysis()
        {
            List<clsCluster> clusters    = null;                        
            clsMassTagDB database        = null;

            UpdateStatus("Setting up parameters");
            
            // Load the mass tag database if we are aligning, or if we are 
            // peak matching (but aligning to a reference dataset.
            if (m_analysis.UseMassTagDBAsBaseline)
            {
                LoadMassTagDB(m_analysis.MassTagDBOptions);
            }
            else
            {
                if (m_analysis.MassTagDBOptions.menm_databaseType != MassTagDatabaseType.None)
                {
                    LoadMassTagDB(m_analysis.MassTagDBOptions);
                }
            }
            m_analysis.MassTagDatabase   = database;

            UpdateStatus("Loading data");
            // Load the features data which stores it to the cache!
            LoadData(m_analysis.Datasets, 
                            m_analysis.UMCFindingOptions, 
                            Path.Combine(m_analysis.PathName, m_analysis.AnalysisName));


            UpdateStatus("Configuring data cache.");
            NHibernateUtil.SetDbLocationForRead(Path.Combine(m_analysis.PathName, m_analysis.AnalysisName));


            UpdateStatus("Aligning datasets.");
            // Align features
            AlignDatasets(m_analysis);

            // Cluster
            UpdateStatus("Performing clustering.");
            clusters = PerformClustering(m_analysis);
            
            if (m_analysis.MassTagDatabase != null)
            {
                if (m_analysis.UseSMART)
                {

                    UpdateStatus("Peak matching with SMART");
                    m_analysis.SMARTResults         = PerformSMART(clusters,
                                                            database,
                                                            m_analysis.SMARTOptions);

                    m_analysis.PeakMatchingResults  = ConvertSmartResultsToPeakResults(m_analysis.SMARTResults,
                                                                                      database,
                                                                                      clusters);                    
                }
                else
                {

                    UpdateStatus("Peak matching");
                    // No-shift, and 11-dalton shift.
                    m_analysis.PeakMatchingResults          = PerformPeakMatching(clusters, database, m_analysis.PeakMatchingOptions, 0.0);
                    m_analysis.PeakMatchingResultsShifted   = PerformPeakMatching(clusters, database, m_analysis.PeakMatchingOptions, 11.0);
                }
            }

            UpdateStatus("Cleaning up resources.");
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            UpdateStatus("Analysis Run Completed.");

            if (AnalysisComplete != null)
                AnalysisComplete(this, null);            
        }
        #endregion   
  
        private ClusteringAlgorithmType m_clusterType = ClusteringAlgorithmType.SingleLinkage;
        //TODO: Fix this when the native code has been refactored.
        public ClusteringAlgorithmType ClusterAlgorithmType
        {
            get
            {
                return m_clusterType;
            }
            set
            {
                m_clusterType = value;
            }
        }
    }

    //TODO: Fix this interface when the native code is all gone.
    public enum ClusteringAlgorithmType
    {
        AverageLinkage,
        SingleLinkage,
        Centroid
    }
}
