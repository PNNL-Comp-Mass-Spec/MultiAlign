using System;
using System.Collections.Generic;
using System.IO;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Data;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;
using PNNLOmics.IO.FileReaders;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Loads UMC's from the given sources.
    /// </summary>
    public static class UMCLoaderFactory
    {
        /// <summary>
        /// Loads data files into memory.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="featureCache"></param>
        /// <param name="msFeatureCache"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static List<clsUMC> LoadData(DatasetInformation      dataset,
                                            IUmcDAO                 featureCache,
                                            IMSFeatureDAO           msFeatureCache,
                                            IGenericDAO<MSFeatureToLCMSFeatureMap> msFeatureMapCache,
                                            UMCFeatureFinderOptions options)
        {
            clsUMC[]        loadedFeatures  = null;
            IFeatureFinder  featureFinder   = new UMCFeatureFinder();
            int             umcIndex        = 0;
            //clsUMCCreator   umcFinder       = new clsUMCCreator();
            //umcFinder.UMCFindingOptions     = options; 
            string extension                = Path.GetExtension(dataset.Path).ToUpper();
            List<UMCLight> newFeatures      = null;

            if (extension == ".TXT")
            {
                    // LCMS Features File                         
                    UmcReader umcReader = new UmcReader(dataset.Path);
                    loadedFeatures      = umcReader.GetUmcList().ToArray();
                    int minScan         = int.MaxValue;
                    int maxScan         = int.MinValue;

                    // Find scan extrema to calculate a NET value.
                    foreach (clsUMC umc in loadedFeatures)
                    {
                        minScan         = Math.Min(umc.mint_start_scan, minScan);
                        maxScan         = Math.Max(umc.mint_end_scan, maxScan);
                    }

                    // Scale for the NET

                    foreach (clsUMC umc in loadedFeatures)
                    {
                        umc.Net = Convert.ToDouble(umc.mint_scan - minScan) / Convert.ToDouble(maxScan - minScan);
                        umc.mint_umc_index = umcIndex++;
                    }                    
            }
            else
            {
                bool foundNewFeatures           = false;
                List<MSFeatureLight> msFeatures = new List<MSFeatureLight>();

                switch (extension)
                {
                    case ".DB3":                    
                        try
                        {                        
                            loadedFeatures = featureCache.FindAll().ToArray();
                        }
                        catch (NHibernate.ADOException)
                        {
                            loadedFeatures = new clsUMC[0];
                        }

                        // If no UMCs were loaded from the SQLite DB,
                        // then we need to create UMCs using MSFeature data from the DB
                        if (loadedFeatures.Length < 1)
                        {
                            msFeatures = msFeatureCache.FindAll();
                        }
                        break;
                    case ".SQLITE":                        
                        try
                        {                            
                            loadedFeatures                  = featureCache.FindAll().ToArray();
                        }
                        catch (NHibernate.ADOException)
                        {
                            loadedFeatures = new clsUMC[0];
                        }

                        // If no UMCs were loaded from the SQLite DB,
                        // then we need to create UMCs using MSFeature data from the DB
                        if (loadedFeatures.Length < 1)
                        {
                            msFeatures = msFeatureCache.FindAll();
                        }
                        break;
                    case ".CSV":                  
                        MSFeatureLightFileReader reader             = new MSFeatureLightFileReader();
                        reader.Delimeter                            = ",";
                        IEnumerable<MSFeatureLight> newMsFeatures   = reader.ReadFile(dataset.Path);                        
                        msFeatures.AddRange(newMsFeatures);
                        foundNewFeatures                            = true;
                        break;
                    default:
                        throw new ArgumentException("Incorrect extension for file. Please use pek, csv, LCMSFeatures.txt, SQLite or DB3 files as inputs.");
                }

                // We have UMC's to find!
                if (msFeatures.Count > 0)
                {


                    newFeatures = new List<UMCLight>();

                    // Needs a refactor.
                    List<MSFeatureLight> filteredMSFeatures = new List<MSFeatureLight>();
                    bool exclude = false;
                    foreach (MSFeatureLight msFeature in msFeatures)
                    {
                        exclude = false;
                        if (options.UseIsotopicFitFilter == true)
                        {
                            if (!options.IsIsotopicFitFilterInverted && msFeature.Score > options.IsotopicFitFilter)
                            {
                                exclude = true;
                            }
                            else if (options.IsIsotopicFitFilterInverted && msFeature.Score < options.IsotopicFitFilter)
                            {
                                exclude = true;
                            }
                        }

                        if (options.UseIsotopicIntensityFilter)
                        {
                            if (msFeature.Abundance < options.IsotopicIntensityFilter)
                            {
                                exclude = true;
                            }
                        }

                        msFeature.GroupID = Convert.ToInt32(dataset.DatasetId);
                        if (!exclude)
                        {
                            filteredMSFeatures.Add(msFeature);
                        }
                    }
                    if (foundNewFeatures)
                    {
                        msFeatureCache.AddAll(filteredMSFeatures);
                    }

                    newFeatures = featureFinder.FindFeatures(filteredMSFeatures, options);
                }

                //clsUMCCreator   umcFinder       = new clsUMCCreator();
                //umcFinder.UMCFindingOptions.AveMassWeight = options.AveMassWeight;
                //umcFinder.UMCFindingOptions.ConstraintAveMass = options.ConstraintAveMass;
                //umcFinder.UMCFindingOptions.ConstraintMonoMass = options.ConstraintMonoMass;
                //umcFinder.UMCFindingOptions.FitWeight = options.FitWeight;
                //umcFinder.UMCFindingOptions.IsIsotopicFitFilterInverted = options.IsIsotopicFitFilterInverted;
                //umcFinder.UMCFindingOptions.IsotopicFitFilter = options.IsotopicFitFilter;
                //umcFinder.UMCFindingOptions.IsotopicIntensityFilter = options.IsotopicIntensityFilter;
                //umcFinder.UMCFindingOptions.MaxDistance = options.MaxDistance;
                //umcFinder.UMCFindingOptions.MinUMCLength = options.MinUMCLength;
                //umcFinder.UMCFindingOptions.MonoMassWeight = options.MonoMassWeight;
                //umcFinder.UMCFindingOptions.NETWeight = options.NETWeight;
                //umcFinder.UMCFindingOptions.ScanWeight = options.ScanWeight;
                //umcFinder.UMCFindingOptions.UMCAbundanceReportingType = options.UMCAbundanceReportingType;
                //umcFinder.UMCFindingOptions.UseIsotopicFitFilter = options.UseIsotopicFitFilter;
                //umcFinder.UMCFindingOptions.UseIsotopicIntensityFilter = options.UseIsotopicIntensityFilter;
                //umcFinder.UMCFindingOptions.UseNET = options.UseNET;

                //umcFinder.FileName  = dataset.Path;
                //umcFinder.LoadFindUMCsCSV();                                
                //loadedFeatures      = umcFinder.GetUMCs();                
            }
                                                                       
            List<clsUMC> features = new List<clsUMC>();
            if (newFeatures != null)
            {
                
                // Map between MS features and LCMS Features.
                List<MSFeatureToLCMSFeatureMap> msFeatureMapList = new List<MSFeatureToLCMSFeatureMap>();

                // Copy the data from the MS features to the old
                // data structures.
                // This should get deprecated on the next part of the refactor.
                foreach (UMCLight feature in newFeatures)
                {
                   
                    clsUMC umc                  = new clsUMC();
                    umc.AbundanceMax            = feature.Abundance;
                    umc.AbundanceSum            = feature.AbundanceSum;
                    umc.AverageDeconFitScore    = -1;
                    umc.ChargeRepresentative    = Convert.ToInt16(feature.ChargeState);                    
                    umc.DatasetId               = dataset.DatasetId;                    
                    umc.ScanEnd                 = feature.ScanEnd;
                    umc.ScanStart               = feature.ScanStart;
                    int maxCharge               = feature.ChargeState;
                    long maxAbundance           = long.MinValue;
                    int scan                    = feature.ScanStart;
                    float mz                    = 0;
                    foreach(MSFeatureLight  msFeature in feature.MSFeatures)
                    {
                        maxCharge       = Math.Max(msFeature.ChargeState, maxCharge);
                        if (msFeature.Abundance > maxAbundance)
                        {
                            scan         = msFeature.Scan;
                            maxAbundance = msFeature.Abundance;
                            mz           = Convert.ToSingle(msFeature.Mz);                            
                        }                                                
                        MSFeatureToLCMSFeatureMap map   = new MSFeatureToLCMSFeatureMap();
                        map.DatasetID                   = umc.DatasetId;
                        map.LCMSFeatureID               = feature.ID;
                        map.MSFeatureID                 = msFeature.ID;
                        msFeatureMapList.Add(map);
                    }
                    umc.ChargeMax               = Convert.ToInt16(maxCharge);
                    umc.Scan                    = scan;
                    umc.MZForCharge             = mz;
                    umc.Net                     = feature.RetentionTime;
                    umc.DriftTime               = feature.DriftTime;
                    umc.Id                      = feature.ID;
                    umc.Mass                    = feature.MassMonoisotopic;
                    umc.MassCalibrated          = feature.MassMonoisotopic;
                    umc.ChargeRepresentative    = Convert.ToInt16(feature.ChargeState);                    
                    umc.ConformationId          = 0;

                    features.Add(umc);                    
                }
                msFeatureMapCache.AddAll(msFeatureMapList);                
            }
            else
            {
                features.AddRange(loadedFeatures);
            }
            // Set the Dataset ID number for each UMC
            foreach (clsUMC umc in features)
            {
                umc.DatasetId = Convert.ToInt32(dataset.DatasetId);
            }

            return features;
        }
    }
}
