using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.Workflow;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Data.SequenceData;
using MultiAlignCore.IO.Features;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmicsIO.IO;


namespace MultiAlignRogue
{
    public class MSFeatureFinding : WorkflowBase
    {
        public FeatureDataAccessProviders Providers { get; set; }


        public IList<UMCLight> LoadDataset(DatasetInformation dataset,
           MsFeatureFilteringOptions msFilteringOptions,
           LcmsFeatureFindingOptions lcmsFindingOptions,
           LcmsFeatureFilteringOptions lcmsFilteringOptions)
        {
            UpdateStatus(string.Format("[{0}] - Loading dataset [{0}] - {1}.", dataset.DatasetId, dataset.DatasetName));
            var datasetId = dataset.DatasetId;
            var features = UmcLoaderFactory.LoadUmcFeatureData(dataset.Features.Path, dataset.DatasetId,
                Providers.FeatureCache);

            UpdateStatus(string.Format("[{0}] Loading MS Feature Data [{0}] - {1}.", dataset.DatasetId,
                dataset.DatasetName));
            var msFeatures = UmcLoaderFactory.LoadMsFeatureData(dataset.Features.Path);
            var scansInfo = UmcLoaderFactory.LoadScanSummaries(dataset.Scans.Path);
            dataset.BuildScanTimes(scansInfo);

            var msnSpectra = new List<MSSpectra>();

            // If we don't have any features, then we have to create some from the MS features
            // provided to us.
            if (features.Count < 1)
            {
                msFeatures = LcmsFeatureFilters.FilterMsFeatures(msFeatures, msFilteringOptions);
                msFeatures = Filter(msFeatures, ref dataset);


                features = CreateLcmsFeatures(dataset,
                    msFeatures,
                    lcmsFindingOptions,
                    lcmsFilteringOptions);

                var maxScan = Convert.ToDouble(features.Max(feature => feature.Scan));
                var minScan = Convert.ToDouble(features.Min(feature => feature.Scan));
                var id = 0;

                foreach (var feature in features)
                {
                    feature.Id = id++;
                    feature.Net = (Convert.ToDouble(feature.Scan) - minScan) / (maxScan - minScan);
                    feature.Net = feature.Net;
                    feature.MassMonoisotopicAligned = feature.MassMonoisotopic;
                    feature.NetAligned = feature.Net;
                    feature.GroupId = datasetId;
                    feature.SpectralCount = feature.MsFeatures.Count;

                    foreach (var msFeature in feature.MsFeatures.Where(msFeature => msFeature != null))
                    {
                        msFeature.UmcId = feature.Id;
                        msFeature.GroupId = datasetId;
                        msFeature.MSnSpectra.ForEach(x => x.GroupId = datasetId);
                        msnSpectra.AddRange(msFeature.MSnSpectra);
                    }
                }
            }
            else
            {
                if (!UmcLoaderFactory.AreExistingFeatures(dataset.Features.Path))
                {
                    var i = 0;
                    foreach (var feature in features)
                    {
                        feature.GroupId = datasetId;
                        feature.Id = i++;
                    }
                }

                // Otherwise, we need to map the MS features to the LCMS Features provided.
                // This would mean that we extracted data from an existing database.
                if (msFeatures.Count > 0)
                {
                    var map = FeatureDataConverters.MapFeature(features);
                    foreach (var feature in
                        from feature in msFeatures
                        let doesFeatureExists = map.ContainsKey(feature.UmcId)
                        where doesFeatureExists
                        select feature)
                    {
                        map[feature.UmcId].AddChildFeature(feature);
                    }
                }
            }


            // Process the MS/MS data with peptides
            UpdateStatus("Reading List of Peptides");
            var sequenceProvider = PeptideReaderFactory.CreateReader(dataset.SequencePath);
            if (sequenceProvider != null)
            {
                UpdateStatus("Reading List of Peptides");
                var peptides = sequenceProvider.Read(dataset.SequencePath);
                var count = 0;
                var peptideList = peptides.ToList();
                peptideList.ForEach(x => x.Id = count++);

                UpdateStatus("Linking MS/MS to any known Peptide/Metabolite Sequences");

                var linker = new PeptideMsMsLinker();
                linker.LinkPeptidesToSpectra(msnSpectra, peptideList);
            }
            return features;
        }


        public List<UMCLight> CreateLcmsFeatures(
                    DatasetInformation information,
                    List<MSFeatureLight> msFeatures,
                    LcmsFeatureFindingOptions options,
                    LcmsFeatureFilteringOptions filterOptions)
        {
            // Make features
            if (msFeatures.Count < 1)
                throw new Exception("No features were found in the feature files provided.");

            UpdateStatus("Finding features.");
            ISpectraProvider provider = null;
            if (information.RawPath != null && !string.IsNullOrWhiteSpace(information.RawPath))
            {
                UpdateStatus("Using raw data to create better features.");
                provider = RawLoaderFactory.CreateFileReader(information.RawPath);
                provider.AddDataFile(information.RawPath, 0);
            }

            ValidateFeatureFinderMaxScanLength(information, options, filterOptions);

            var finder = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.TreeBased);
            finder.Progress += (sender, args) => UpdateStatus(args.Message);
            var features = finder.FindFeatures(msFeatures, options, provider);

            UpdateStatus("Filtering features.");
            List<UMCLight> filteredFeatures;
            if (filterOptions.TreatAsTimeNotScan) //Feature length determined based on time (mins)
            {
                filteredFeatures = LcmsFeatureFilters.FilterFeatures(features,
                    filterOptions, information.ScanTimes);
            }
            else //Feature length determined based on scans
            {
                filteredFeatures = LcmsFeatureFilters.FilterFeatures(features, filterOptions);
            }

            UpdateStatus(string.Format("Filtered features from: {0} to {1}.", features.Count, filteredFeatures.Count));
            return filteredFeatures;
        }


        public List<MSFeatureLight> Filter(List<MSFeatureLight> msFeatures, ref DatasetInformation dataset)
        {
            string rawPath = dataset.RawPath;
            if (rawPath == null || string.IsNullOrWhiteSpace(rawPath))
                return msFeatures;

            // First find all unique scans
            var scanMap = new Dictionary<int, bool>();
            foreach (var feature in msFeatures)
            {
                if (!scanMap.ContainsKey(feature.Scan))
                {
                    // Assume all scans are parents
                    scanMap.Add(feature.Scan, true);
                }
            }
            // Then parse each to figure out if this is true.
            var fullScans = new Dictionary<int, bool>();
            var scanTimes = new Dictionary<int, double>();
            using (var provider = RawLoaderFactory.CreateFileReader(rawPath))
            {
                if (provider == null)
                {
                    UpdateStatus(string.Format("Warning: Raw file not found ({0}); scan times are not available!", System.IO.Path.GetFileName(rawPath)));
                }
                else
                {
                    UpdateStatus(string.Format("Reading scan info from {0}", System.IO.Path.GetFileName(rawPath)));

                    provider.AddDataFile(rawPath, 0);
                    foreach (var scan in scanMap.Keys)
                    {
                        ScanSummary summary = provider.GetScanSummary(scan, 0);

                        if (summary == null)
                        {
                            continue;
                        }

                        if (summary.MsLevel == 1)
                            fullScans.Add(scan, true);
                        scanTimes.Add(scan, summary.Time);
                    }
                    dataset.ScanTimes = scanTimes;
                }
            }

            return msFeatures.Where(x => fullScans.ContainsKey(x.Scan)).ToList();
        }


        private static void ValidateFeatureFinderMaxScanLength(
                        DatasetInformation information,
                        LcmsFeatureFindingOptions options,
                        LcmsFeatureFilteringOptions filterOptions)
        {
            if (!filterOptions.TreatAsTimeNotScan)
            {
                if (options.MaximumScanRange < filterOptions.FeatureLengthRange.Maximum)
                {
                    // Bump up the scan range used by the LCMS Feature Finder to allow for longer featuers
                    options.MaximumScanRange = (int)filterOptions.FeatureLengthRange.Maximum;
                }
                return;
            }

            int maxScanLength;

            if (information.ScanTimes.Count == 0)
            {
                // FeatureLengthRange.Maximum is in minutes
                // Assume 3 scans/second (ballpark estimate)
                maxScanLength = (int)filterOptions.FeatureLengthRange.Maximum * 60 * 3;
            }
            else
            {
                // Find the average number of scans that spans FeatureLengthRange.Maximum minutes

                // Step through the dictionary to find the average number of scans per minute
                var minuteThreshold = 1;
                var scanCountCurrent = 0;
                var scanCountsPerMinute = new List<int>();

                foreach (var entry in information.ScanTimes)
                {
                    if (entry.Value < minuteThreshold)
                    {
                        scanCountCurrent++;
                    }
                    else
                    {
                        if (scanCountCurrent > 0)
                        {
                            scanCountsPerMinute.Add(scanCountCurrent);
                        }
                        scanCountCurrent = 0;
                        minuteThreshold++;
                    }
                }

                int averageScansPerMinute;
                if (scanCountsPerMinute.Count > 0)
                {
                    averageScansPerMinute = (int)scanCountsPerMinute.Average();
                }
                else
                {
                    averageScansPerMinute = 180;
                }

                maxScanLength = (int)(filterOptions.FeatureLengthRange.Maximum * averageScansPerMinute * 1.25);
            }

            if (options.MaximumScanRange < maxScanLength)
            {
                // Bump up the scan range used by the LCMS Feature Finder to allow for longer featuers
                options.MaximumScanRange = maxScanLength;
            }
        }

    }
}
