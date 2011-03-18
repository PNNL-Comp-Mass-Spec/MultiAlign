using System;
using System.IO;
using System.Collections.Generic;


using MultiAlignEngine;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.Features;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.PeakMatching;

using PNNLProteomics.Data;
using PNNLProteomics.IO;
using PNNLProteomics.SMART;
using PNNLProteomics.EventModel;
using PNNLProteomics.Data.Factors;
using PNNLProteomics.Data.Alignment;
using PNNLProteomics.MultiAlign;
using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;

using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;

namespace PNNLProteomics.IO.UMC
{
    /// <summary>
    /// Loads UMC's from the given sources.
    /// </summary>
    public static class UMCLoaderFactory
    {
        public static List<clsUMC> LoadData(DatasetInformation   dataset,
                                            IUmcDAO              featureCache,
                                            clsUMCFindingOptions options)
        {
            clsUMC[] loadedFeatures = null;
            clsUMCCreator umcFinder = new clsUMCCreator();
            umcFinder.UMCFindingOptions = options;
            int umcIndex = 0;

            string extension = Path.GetExtension(dataset.Path).ToUpper();
            switch (extension)
            {
                case ".TXT":
                    // LCMS Features File                         
                    UmcReader umcReader = new UmcReader(dataset.Path);
                    loadedFeatures = umcReader.GetUmcList().ToArray();
                    int minScan = int.MaxValue;
                    int maxScan = int.MinValue;

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
                    try
                    {                        
                        loadedFeatures = featureCache.FindAll().ToArray();
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
                    NHibernateUtil.SetDbLocationForRead(dataset.Path);
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
                    umcFinder.FileName = dataset.Path;
                    umcFinder.LoadUMCs(true);
                    umcFinder.FindUMCs();
                    loadedFeatures = umcFinder.GetUMCs();
                    break;
                case ".CSV":
                    umcFinder.FileName = dataset.Path;                    
                    umcFinder.LoadUMCs(false);
                    umcFinder.FindUMCs();
                    loadedFeatures = umcFinder.GetUMCs();
                    break;
                default:
                    throw new ArgumentException("Incorrect extension for file. Please use pek, csv, LCMSFeatures.txt, SQLite or DB3 files as inputs.");
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
    }
}
