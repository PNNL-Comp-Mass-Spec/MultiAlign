using System;
using System.Collections.Generic;
using System.IO;
using MultiAlignCore.Data;
using MultiAlignCore.IO.SequenceData;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmicsIO.IO;

namespace MultiAlignCore.IO.Features
{
    public class UMCLoadingEventArgs: EventArgs 
    {
        public UMCLoadingEventArgs(string message)
        {
            Message = message;
        }
        public string Message
        {
            get;
            private set;
        }
    }
    /// <summary>
    /// Loads UMC's from the given sources.
    /// </summary>
    public static class UMCLoaderFactory
    {
        /// <summary>
        /// Status event fired when umc's are loaded.
        /// </summary>
        public static event EventHandler<UMCLoadingEventArgs> Status;
        /// <summary>
        /// Helper function to tell listeners about data.
        /// </summary>
        /// <param name="message"></param>
        private static void UpdateStatus(string message)
        {
            if (Status != null)
            {
                Status(null, new UMCLoadingEventArgs(message));
            }
        }

        #region Old Code To Be Deleted
        ///// <summary>
        ///// Loads data files into memory.
        ///// </summary>
        ///// <param name="dataset"></param>
        ///// <param name="featureCache"></param>
        ///// <param name="msFeatureCache"></param>
        ///// <param name="options"></param>
        ///// <returns></returns>
        //public static List<clsUMC> LoadData(DatasetInformation      dataset,
        //                                    IUmcDAO                 featureCache,
        //                                    IMSFeatureDAO           msFeatureCache,
        //                                    IGenericDAO<MSFeatureToLCMSFeatureMap> msFeatureMapCache,
        //                                    LCMSFeatureFindingOptions options)
        //{
        //    clsUMC[]        loadedFeatures  = null;
        //    IFeatureFinder  featureFinder   = new UMCFeatureFinder();
        //    int             umcIndex        = 0;
        //    //clsUMCCreator   umcFinder       = new clsUMCCreator();
        //    //umcFinder.UMCFindingOptions     = options; 
        //    string extension                = Path.GetExtension(dataset.Path).ToUpper();
        //    List<UMCLight> newFeatures      = null;

        //    if (extension == ".TXT")
        //    {
        //            // LCMS Features File                         
        //            UmcReader umcReader = new UmcReader(dataset.Path);
        //            loadedFeatures      = umcReader.GetUmcList().ToArray();
        //            int minScan         = int.MaxValue;
        //            int maxScan         = int.MinValue;

        //            // Find scan extrema to calculate a NET value.
        //            foreach (clsUMC umc in loadedFeatures)
        //            {
        //                minScan         = Math.Min(umc.mint_start_scan, minScan);
        //                maxScan         = Math.Max(umc.mint_end_scan, maxScan);
        //            }

        //            // Scale for the NET

        //            foreach (clsUMC umc in loadedFeatures)
        //            {
        //                umc.Net = Convert.ToDouble(umc.mint_scan - minScan) / Convert.ToDouble(maxScan - minScan);
        //                umc.mint_umc_index = umcIndex++;
        //            }                    
        //    }
        //    else
        //    {
        //        bool foundNewFeatures           = false;
        //        List<MSFeatureLight> msFeatures = new List<MSFeatureLight>();

        //        switch (extension)
        //        {
        //            case ".DB3":                    
        //                try
        //                {
        //                    loadedFeatures = featureCache.FindAll();
        //                }
        //                catch (NHibernate.ADOException)
        //                {
        //                    loadedFeatures = new clsUMC[0];
        //                }

        //                // If no UMCs were loaded from the SQLite DB,
        //                // then we need to create UMCs using MSFeature data from the DB
        //                if (loadedFeatures.Length < 1)
        //                {
        //                    msFeatures = msFeatureCache.FindAll();
        //                }
        //                UpdateStatus("Loaded features from the database.");
        //                break;
        //            case ".SQLITE":                        
        //                try
        //                {
        //                    loadedFeatures = featureCache.FindAll();
        //                }
        //                catch (NHibernate.ADOException)
        //                {
        //                    loadedFeatures = new clsUMC[0];
        //                }

        //                // If no UMCs were loaded from the SQLite DB,
        //                // then we need to create UMCs using MSFeature data from the DB
        //                if (loadedFeatures.Length < 1)
        //                {
        //                    msFeatures = msFeatureCache.FindAll();
        //                }
        //                UpdateStatus("Loaded features from the database.");
        //                break;
        //            case ".CSV":                  
        //                MSFeatureLightFileReader reader             = new MSFeatureLightFileReader();
        //                reader.Delimeter                            = ",";
        //                IEnumerable<MSFeatureLight> newMsFeatures   = reader.ReadFile(dataset.Path);                        
        //                msFeatures.AddRange(newMsFeatures);
        //                foundNewFeatures                            = true;
        //                UpdateStatus("Loaded features from the CSV files.");
        //                break;

        //            default:
        //                throw new ArgumentException("Incorrect extension for file. Please use pek, csv, LCMSFeatures.txt, SQLite or DB3 files as inputs.");
        //        }

        //        // We have UMC's to find!
        //        if (msFeatures.Count > 0)
        //        {

        //            UpdateStatus("Filtering MS Features.");
        //            newFeatures = new List<UMCLight>();

        //            // Needs a refactor.
        //            List<MSFeatureLight> filteredMSFeatures = new List<MSFeatureLight>();
        //            bool exclude = false;
        //            foreach (MSFeatureLight msFeature in msFeatures)
        //            {
        //                exclude = false;
        //                if (options.UseIsotopicFitFilter == true)
        //                {
        //                    if (!options.IsIsotopicFitFilterInverted && msFeature.Score > options.IsotopicFitFilter)
        //                    {
        //                        exclude = true;
        //                    }
        //                    else if (options.IsIsotopicFitFilterInverted && msFeature.Score < options.IsotopicFitFilter)
        //                    {
        //                        exclude = true;
        //                    }
        //                }

        //                if (options.UseIsotopicIntensityFilter)
        //                {
        //                    if (msFeature.Abundance < options.IsotopicIntensityFilter)
        //                    {
        //                        exclude = true;
        //                    }
        //                }

        //                msFeature.GroupID = Convert.ToInt32(dataset.DatasetId);
        //                if (!exclude)
        //                {
        //                    filteredMSFeatures.Add(msFeature);
        //                }
        //            }

        //            UpdateStatus("Creating LC-MS features.");
        //            newFeatures = featureFinder.FindFeatures(filteredMSFeatures, options);

        //            // Make sure we only add back to the database what features u
        //            List<MSFeatureLight> msFeaturesUsed = new List<MSFeatureLight>();
        //            foreach (UMCLight feature in newFeatures)
        //            {
        //                foreach (MSFeatureLight featureLight in feature.MSFeatures)
        //                {
        //                    msFeaturesUsed.Add(featureLight);
        //                }
        //            }

        //            if (foundNewFeatures && options.StoreMSFeatureResults)
        //            {                        
        //                UpdateStatus(string.Format("Adding {0} MS features to the database.", msFeaturesUsed.Count));                    
        //                msFeatureCache.AddAll(msFeaturesUsed);
        //            }
        //        }                            
        //    }

        //    List<clsUMC> features = new List<clsUMC>();
        //    if (newFeatures != null)
        //    {                
        //        // Map between MS features and LCMS Features.
        //        List<MSFeatureToLCMSFeatureMap> msFeatureMapList = new List<MSFeatureToLCMSFeatureMap>();

        //        // Copy the data from the MS features to the old
        //        // data structures.
        //        // This should get deprecated on the next part of the refactor.
        //        foreach (UMCLight feature in newFeatures)
        //        {


        //            clsUMC umc                  = new clsUMC();
        //            umc.AbundanceMax            = feature.Abundance;
        //            umc.AbundanceSum            = feature.AbundanceSum;
        //            umc.AverageDeconFitScore    = -1;
        //            umc.ChargeRepresentative    = Convert.ToInt16(feature.ChargeState);                    
        //            umc.DatasetId               = dataset.DatasetId;                    
        //            umc.ScanEnd                 = feature.ScanEnd;
        //            umc.ScanStart               = feature.ScanStart;
        //            int maxCharge               = feature.ChargeState;
        //            long maxAbundance           = long.MinValue;
        //            int scan                    = feature.ScanStart;
        //            float mz                    = 0;
        //            foreach(MSFeatureLight  msFeature in feature.MSFeatures)
        //            {
        //                maxCharge       = Math.Max(msFeature.ChargeState, maxCharge);
        //                if (msFeature.Abundance > maxAbundance)
        //                {
        //                    scan         = msFeature.Scan;
        //                    maxAbundance = msFeature.Abundance;
        //                    mz           = Convert.ToSingle(msFeature.Mz);                            
        //                }                                                
        //                MSFeatureToLCMSFeatureMap map   = new MSFeatureToLCMSFeatureMap();
        //                map.DatasetID                   = umc.DatasetId;
        //                map.LCMSFeatureID               = feature.ID;
        //                map.MSFeatureID                 = msFeature.ID;
        //                msFeatureMapList.Add(map);
        //            }
        //            umc.ChargeMax               = Convert.ToInt16(maxCharge);
        //            umc.Scan                    = scan;
        //            umc.MZForCharge             = mz;
        //            umc.Net                     = feature.RetentionTime;
        //            umc.DriftTime               = feature.DriftTime;
        //            umc.Id                      = feature.ID;
        //            umc.Mass                    = feature.MassMonoisotopic;
        //            umc.MassCalibrated          = feature.MassMonoisotopic;
        //            umc.ChargeRepresentative    = Convert.ToInt16(feature.ChargeState);                    
        //            umc.ConformationId          = 0;

        //            features.Add(umc);                    
        //        }

        //        if (options.StoreMSFeatureResults)
        //        {
        //            UpdateStatus(string.Format("Mapping {0} MS features to LC-MS features for the database.", 
        //                                    msFeatureMapList.Count));
        //            msFeatureMapCache.AddAll(msFeatureMapList);
        //        }
        //    }
        //    else
        //    {
        //        features.AddRange(loadedFeatures);
        //    }
        //    // Set the Dataset ID number for each UMC
        //    foreach (clsUMC umc in features)
        //    {
        //        umc.DatasetId = Convert.ToInt32(dataset.DatasetId);
        //    }

        //    UpdateStatus("Feature finding complete.");
        //    return features;
        //}
        #endregion

        /// <summary>
        /// Loads feature data from the files provided.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="featureCache"></param>
        /// <returns></returns>
        public static List<UMCLight> LoadUmcFeatureData(DatasetInformation dataset,
                                                        IUmcDAO featureCache)
        {
            List<UMCLight> features = new List<UMCLight>();
            string extension = Path.GetExtension(dataset.Features.Path).ToUpper();
            switch (extension)
            {
                case ".TXT":
                    LCMSFeatureFileReader umcReader = new LCMSFeatureFileReader(dataset.Features.Path);
                    features = umcReader.GetUmcList();
                    break;
            }
            return features;
        }
        /// <summary>
        /// Loads MS Features from a CSV file or existing database.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="msFeatureCache"></param>
        /// <returns></returns>
        public static List<MSFeatureLight> LoadMsFeatureData(DatasetInformation dataset,
                                                              IMSFeatureDAO msFeatureCache)
        {
            List<MSFeatureLight> msFeatures = new List<MSFeatureLight>();

            string path = dataset.Features.Path;

            string extension = Path.GetExtension(path).ToUpper();
            switch (extension)
            {
                case ".DB3":
                    try
                    {
                        msFeatures = msFeatureCache.FindAll();
                        UpdateStatus("Loaded MS features from the database.");
                    }
                    catch (NHibernate.ADOException)
                    {
                        UpdateStatus("ADO Error when loading features from the database.");
                    }
                    break;
                case ".SQLITE":
                    try
                    {
                        msFeatures = msFeatureCache.FindAll();
                        UpdateStatus("Loaded MS features from the database.");
                    }
                    catch (NHibernate.ADOException)
                    {
                        UpdateStatus("ADO Error when loading features from the database.");
                    }
                    break;
                case ".CSV":
                    MSFeatureLightFileReader reader = new MSFeatureLightFileReader();
                    reader.Delimeter                = ",";
                    IEnumerable<MSFeatureLight> newMsFeatures = reader.ReadFile(path);
                    msFeatures.AddRange(newMsFeatures);
                    UpdateStatus("Loaded features from the CSV files.");
                    break;
                default:
                    throw new ArgumentException("Incorrect extension for file. Please use pek, csv, LCMSFeatures.txt, SQLite or DB3 files as inputs.");
            }

            return msFeatures;
        }
        /// <summary>
        /// Loads MSn spectra from an existing database if it exists.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="msnFeatureCache"></param>
        /// <returns></returns>
        public static List<MSSpectra> LoadMsnSpectra(DatasetInformation dataset,
                                                        IMSnFeatureDAO msnFeatureCache)
        {
            List<MSSpectra> spectra = msnFeatureCache.FindByDatasetId(dataset.DatasetId);
            return spectra;
        }
        /// <summary>
        /// Loads all of the data from the scans file.
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static List<ScanSummary> LoadScansData(DatasetInformation dataset)
        {
            if (dataset.Scans == null)
                return new List<ScanSummary>();

            ScansFileReader scansReader         = new ScansFileReader();
            IEnumerable<ScanSummary> scanData   = scansReader.ReadFile(dataset.Scans.Path);
            List<ScanSummary> scans             = new List<ScanSummary>();
            scans.AddRange(scanData);

            return scans;
        }
        public static void LoadSequenceData(DatasetInformation dataset,
                                            IDatabaseSearchSequenceDAO cache)
        {
            if (dataset.Sequence != null)
            {
                MAGEDatabaseSearchAdaptor adapter = new IO.SequenceData.MAGEDatabaseSearchAdaptor();
                adapter.LoadSequenceData(   dataset.Sequence.Path,
                                            dataset.DatasetId,
                                            cache);
            }
        }

        /// <summary>
        /// Loads MS Spectra from a raw data file.
        /// </summary>
        /// <param name="rawReader"></param>
        /// <param name="msnCache"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static List<MSSpectra> LoadRawData(DatasetInformation dataset, ISpectraProvider rawReader)
        {
            int datasetID = dataset.DatasetId;
            string rawPath = dataset.Raw.Path;
            List<MSSpectra> msnSpectra = new List<MSSpectra>();
            using (rawReader = RawLoaderFactory.CreateFileReader(rawPath))
            {
                rawReader.AddDataFile(rawPath, 0);
                msnSpectra = rawReader.GetMSMSSpectra(0);
            }

            int id = 0;
            foreach (MSSpectra spectrum in msnSpectra)
            {
                spectrum.ID = id++;
                spectrum.GroupID = datasetID;
            }
            return msnSpectra;
        }     
    }
}
