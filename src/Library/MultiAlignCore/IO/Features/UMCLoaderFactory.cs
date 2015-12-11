#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.RawData;
using MultiAlignCore.IO.TextFiles;

#endregion

namespace MultiAlignCore.IO.Features
{
    using InformedProteomics.Backend.MassSpecData;

    using MultiAlignCore.Data.MassTags;

    /// <summary>
    ///     Loads UMC's from the given sources.
    /// </summary>
    public static class UmcLoaderFactory
    {
        /// <summary>
        ///     Status event fired when umc's are loaded.
        /// </summary>
        public static event EventHandler<UmcLoadingEventArgs> Status;

        /// <summary>
        ///     Helper function to tell listeners about data.
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
        ///     Loads feature data from the files provided.
        /// </summary>
        /// <returns></returns>
        public static IList<UMCLight> LoadUmcFeatureData(DatasetInformation dataset, IUmcDAO featureCache, IScanSummaryProvider provider = null)
        {
            var features = new List<UMCLight>();
            var extension = Path.GetExtension(dataset.Features.Path);
            if (extension == null) return features;

            extension = extension.ToUpper();
            switch (extension)
            {
                case ".TXT":
                    if (dataset.Features.Path.EndsWith("_LCMSFeatures.txt"))
                    {
                        var reader = new LcImsFeatureFileReader(provider, dataset.DatasetId);
                        features = reader.ReadFile(dataset.Features.Path).ToList();
                    }
                    else
                    {
                        var umcReader = new LCMSFeatureFileReader(dataset.Features.Path);
                        features = umcReader.GetUmcList();
                    }
                    break;
                case ".DB3":
                    features = featureCache.FindByDatasetId(dataset.DatasetId);
                    break;
                case ".MS1FT":
                    if (provider != null && provider is InformedProteomicsReader)
                    {
                        var promexReader = new PromexFileReader(provider as InformedProteomicsReader, dataset.DatasetId);
                        features = promexReader.ReadFile(dataset.Features.Path).ToList();   
                    }
                    break;
                default: //Was reconstructing features from scratch even when they were already cached because the file extention was ".csv" not ".db3"
                        features = featureCache.FindByDatasetId(dataset.DatasetId);
                    break;
            }

            if (features != null && provider is ISpectraProvider)
            {
                var spectraProvider = provider as ISpectraProvider;
                LoadMsMs(features, spectraProvider);
            }
            return features;
        }

        /// <summary>
        ///     Determines if the features came from the database or a feature file.
        /// </summary>
        /// <returns></returns>
        public static bool AreExistingFeatures(string path)
        {
            var areExisting = false;
            var extension = Path.GetExtension(path);
            if (extension == null) return false;

            extension = extension.ToUpper();
            switch (extension)
            {
                case ".DB3":
                    areExisting = true;
                    break;
            }
            return areExisting;
        }

        /// <summary>
        ///     Loads MS Features from a CSV file or existing database.
        /// </summary>
        /// <returns></returns>
        public static List<MSFeatureLight> LoadMsFeatureData(string path, DeconToolsIsosFilterOptions isosFilterOptions)
        {
            var msFeatures = new List<MSFeatureLight>();
            var extension = Path.GetExtension(path);
            if (extension == null) return msFeatures;

            extension = extension.ToUpper();
            switch (extension)
            {
                case ".PEK":
                    throw new NotImplementedException("Support for .PEK files is not available at this time");
                    // var pekReader = new PEKFileReader();
                    // var pekMsFeatures = pekReader.ReadFile(path);
                    // msFeatures.AddRange(pekMsFeatures);
                    // UpdateStatus("Loaded features from the PEK file.");
                    break;

                default:
                    var reader = new MsFeatureLightFileReader {Delimiter = ','};
                    reader.IsosFilteroptions = isosFilterOptions;
                    var newMsFeatures = reader.ReadFile(path);
                    msFeatures.AddRange(newMsFeatures);
                    UpdateStatus("Loaded features from the CSV files.");
                    break;
            }

            return msFeatures;
        }

        public static List<ScanSummary> LoadScanSummaries(string path)
        {
            var scanSummaries = new List<ScanSummary>();
            var reader = new ScansFileReader {Delimiter = ','};
            var scans = reader.ReadFile(path);
            scanSummaries.AddRange(scans);
            UpdateStatus("Loaded scan summaries from CSV files.");
            return scanSummaries;
        }

        public static void LoadMsMs(List<UMCLight> features, ISpectraProvider spectraProvider)
        {
            foreach (var feature in features)
            {
                foreach (var msFeature in feature.Features)
                {
                    var fragmentationSpectra = spectraProvider.GetMSMSSpectra(
                        msFeature.Scan,
                        msFeature.Mz,
                        false);
                    msFeature.MSnSpectra.AddRange(fragmentationSpectra);
                }
            }
        }
    }
}