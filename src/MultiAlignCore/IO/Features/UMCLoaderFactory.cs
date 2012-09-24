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
                case ".DB3":
                    features = featureCache.FindByDatasetId(dataset.DatasetId);
                    break;
            }
            return features;
        }
        public static bool AreExistingFeatures(DatasetInformation dataset)
        {
            bool areExisting = false;
            string extension = Path.GetExtension(dataset.Features.Path).ToUpper();
            switch (extension)
            {
                case ".TXT":
                    areExisting = false;
                    break;
                case ".DB3":
                    areExisting = true;
                    break;
            }
            return areExisting;
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
                case ".TXT":
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
        public static List<MSSpectra> LoadRawData(DatasetInformation dataset)
        {
            return LoadRawData(dataset, new Dictionary<int, int>());
        }
        /// <summary>
        /// Loads MS Spectra from a raw data file.
        /// </summary>
        /// <param name="rawReader"></param>
        /// <param name="msnCache"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static List<MSSpectra> LoadRawData(DatasetInformation dataset, Dictionary<int, int> excludeMap)
        {
            int datasetID                       = dataset.DatasetId;
            string rawPath                      = dataset.Raw.Path;
            List<MSSpectra> msnSpectra          = new List<MSSpectra>();
            using (ISpectraProvider rawReader   = RawLoaderFactory.CreateFileReader(rawPath))
            {
                rawReader.AddDataFile(rawPath, 0);
                msnSpectra = rawReader.GetMSMSSpectra(0, excludeMap);
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
