using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data.Features;
using PNNLOmicsIO.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Loads UMC's from the given sources.
    /// </summary>
    public static class UmcLoaderFactory
    {
        /// <summary>
        /// Status event fired when umc's are loaded.
        /// </summary>
        public static event EventHandler<UmcLoadingEventArgs> Status;
        /// <summary>
        /// Helper function to tell listeners about data.
        /// </summary>
        /// <param name="message"></param>
        private static void UpdateStatus(string message)
        {
            if (Status != null)
            {
                Status(null, new UmcLoadingEventArgs(message));
            }
        }

        /// <summary>
        /// Loads feature data from the files provided.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="featureCache"></param>
        /// <returns></returns>
        public static IList<UMCLight> LoadUmcFeatureData(DatasetInformation dataset,
                                                        IUmcDAO featureCache)
        {
            var features     = new List<UMCLight>();
            string extension = Path.GetExtension(dataset.Features.Path).ToUpper();            
            switch (extension)
            {
                case ".TXT":
                    var umcReader = new LCMSFeatureFileReader(dataset.Features.Path);
                    features = umcReader.GetUmcList();
                    break;
                case ".DB3":
                    features = featureCache.FindByDatasetId(dataset.DatasetId);
                    break;
            }
            return features;
        }        
        /// <summary>
        /// Determines if the features came from the database or a feature file.
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static bool AreExistingFeatures(DatasetInformation dataset)
        {
            bool areExisting = false;
            string extension = Path.GetExtension(dataset.Features.Path).ToUpper();
            switch (extension)
            {
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
            var msFeatures      = new List<MSFeatureLight>();
            string path         = dataset.Features.Path;
            string extension    = Path.GetExtension(path).ToUpper();
            switch (extension)
            {
                default:
                    var reader = new MSFeatureLightFileReader { Delimeter = "," };
                    IEnumerable<MSFeatureLight> newMsFeatures = reader.ReadFile(path);
                    msFeatures.AddRange(newMsFeatures);                    
                    UpdateStatus("Loaded features from the CSV files.");
                    break;                    
            }

            return msFeatures;
        }
    }
}
