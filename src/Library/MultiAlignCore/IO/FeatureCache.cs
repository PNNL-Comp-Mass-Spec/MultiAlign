#region

using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Algorithms;
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

#endregion

namespace MultiAlignCore.IO
{
    /// <summary>
    ///     This class is in charge of caching the appropriate LCMS Feature data to the underlying database.
    /// </summary>
    public class FeatureLoader : WorkflowBase
    {
        public FeatureDataAccessProviders Providers { get; set; }

        public void CacheFeatures(IList<UMCLight> features)
        {
            // SpectraTracker - Makes sure that we only record a MS spectra once, before we cache
            // this keeps us from trying to put duplicate entries into the MS/MS data 
            // table/container.
            var spectraTracker = new Dictionary<int, MSSpectra>();
            var msmsFeatures = new List<MSSpectra>();
            var mappedPeptides = new List<DatabaseSearchSequence>();
            var sequenceMaps = new List<SequenceToMsnFeature>();

            // This dictionary makes sure that the peptide was not seen already, since a peptide can be mapped multiple times...?                
            var matches = new List<MSFeatureToMSnFeatureMap>();
            var msFeatures = new List<MSFeatureLight>();
            var peptideId = 0;

            // Next we may want to map our MSn features to our parents.  This would allow us to do traceback...
            foreach (var feature in features)
            {
                var totalMsMs = 0;
                var totalIdentified = 0;
                var datasetId = feature.GroupId;
                msFeatures.AddRange(feature.MsFeatures);

                // For Each MS Feature
                foreach (var msFeature in feature.MsFeatures)
                {
                    totalMsMs += msFeature.MSnSpectra.Count;
                    // For each MS / MS 
                    foreach (var spectrum in msFeature.MSnSpectra)
                    {
                        var match = new MSFeatureToMSnFeatureMap
                        {
                            RawDatasetID = datasetId,
                            MSDatasetID = datasetId,
                            MSFeatureID = msFeature.Id,
                            MSMSFeatureID = spectrum.Id,
                            LCMSFeatureID = feature.Id
                        };
                        spectrum.GroupId = datasetId;
                        matches.Add(match);

                        if (spectraTracker.ContainsKey(spectrum.Id))
                            continue;

                        msmsFeatures.Add(spectrum);
                        spectraTracker.Add(spectrum.Id, spectrum);

                        // We are prepping the sequences that we found from peptides that were 
                        // matched only, not all of the results. 
                        // These maps here are made to help establish database search results to msms 
                        // spectra
                        foreach (var peptide in spectrum.Peptides)
                        {
                            peptide.GroupId = datasetId;
                            var newPeptide = new DatabaseSearchSequence(peptide, feature.Id)
                            {
                                GroupId = datasetId,
                                Id = peptideId++
                            };
                            mappedPeptides.Add(newPeptide);

                            var sequenceMap = new SequenceToMsnFeature
                            {
                                UmcFeatureId = feature.Id,
                                DatasetId = msFeature.GroupId,
                                MsnFeatureId = spectrum.Id,
                                SequenceId = peptide.Id
                            };
                            sequenceMaps.Add(sequenceMap);
                        }

                        totalIdentified += spectrum.Peptides.Count;
                    }
                }

                feature.MsMsCount = totalMsMs;
                feature.IdentifiedSpectraCount = totalIdentified;
            }

            var count = 0;
            //TODO: Fix!!! make sure sequence maps are unique
            sequenceMaps.ForEach(x => x.Id = count++);

            if (msmsFeatures.Count > 0)
                Providers.MSnFeatureCache.AddAll(msmsFeatures);

            if (matches.Count > 0)
                Providers.MSFeatureToMSnFeatureCache.AddAll(matches);

            if (sequenceMaps.Count > 0)
                Providers.SequenceMsnMapCache.AddAll(sequenceMaps);

            if (mappedPeptides.Count > 0)
                Providers.DatabaseSequenceCache.AddAll(mappedPeptides);

            if (msFeatures.Count > 0)
                Providers.MSFeatureCache.AddAll(msFeatures);
            Providers.FeatureCache.AddAll(features);
        }

        /// <summary>
        ///     Creates LCMS Features
        /// </summary>
        private List<UMCLight> CreateLcmsFeatures(
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

        /// <summary>
        ///     Load a single dataset from the provider.
        /// </summary>
        /// <returns></returns>
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
                    feature.Net = (Convert.ToDouble(feature.Scan) - minScan)/(maxScan - minScan);
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

        /// <summary>
        ///     Filters the list of MS Features that may be from MS/MS deisotoped data.
        /// </summary>
        private List<MSFeatureLight> Filter(List<MSFeatureLight> msFeatures, ref DatasetInformation dataset)
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
            using (var provider = RawLoaderFactory.CreateFileReader(rawPath))
            {
                if (provider != null)
                {
                    provider.AddDataFile(rawPath, 0);
                    foreach (var scan in scanMap.Keys)
                    {
                        ScanSummary summary;
                        provider.GetRawSpectra(scan, 0, out summary);
                        if (summary.MsLevel == 1)
                            fullScans.Add(scan, true);
                        dataset.ScanTimes.Add(scan, summary.Time);
                    }
                }
            }

            return msFeatures.Where(x => fullScans.ContainsKey(x.Scan)).ToList();
        }
    }
}