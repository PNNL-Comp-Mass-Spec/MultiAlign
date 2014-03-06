using MultiAlignCore.Algorithms.Workflow;
using MultiAlignCore.Data;
using MultiAlignCore.Data.SequenceData;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using System.Collections.Generic;

using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Data.Features;
using PNNLOmicsIO.IO;
using System;
using System.Linq;

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    /// This class is in charge of caching the appropriate LCMS Feature data to the underlying database.
    /// </summary>
    public class FeatureLoader : WorkflowBase
    {

        public FeatureDataAccessProviders Providers { get; set; }
       
        public void CacheFeatures(IList<UMCLight> features)
        {                                   
                // SpectraTracker - Makes sure that we only record a MS spectra once, before we cache
                // this keeps us from trying to put duplicate entries into the MS/MS data 
                // table/container.
                var spectraTracker  = new Dictionary<int, MSSpectra>();                
                var msmsFeatures    = new List<MSSpectra>();
                var mappedPeptides  = new List<DatabaseSearchSequence>();
                var sequenceMaps    = new List<SequenceToMsnFeature>();
                
                // This dictionary makes sure that the peptide was not seen already, since a peptide can be mapped multiple times...?                
                var matches         = new List<MSFeatureToMSnFeatureMap>();
                var msFeatures      = new List<MSFeatureLight>();                
                // Next we may want to map our MSn features to our parents.  This would allow us to do traceback...
                foreach (var feature in features)
                {
                    var totalMsMs       = 0;
                    var totalIdentified = 0;
                    var datasetId       = feature.GroupID;
                    msFeatures.AddRange(feature.MSFeatures);

                    // For Each MS Feature
                    foreach (var msFeature in feature.MSFeatures)
                    {                        
                        totalMsMs += msFeature.MSnSpectra.Count;
                        // For each MS / MS 
                        foreach (var spectrum in msFeature.MSnSpectra)
                        {
                            var match           = new MSFeatureToMSnFeatureMap
                            {
                                RawDatasetID    = datasetId,
                                MSDatasetID     = datasetId,
                                MSFeatureID     = msFeature.ID,
                                MSMSFeatureID   = spectrum.ID,
                                LCMSFeatureID   = feature.ID
                            };                            
                            spectrum.GroupID    = datasetId;
                            matches.Add(match);
                            
                            if (spectraTracker.ContainsKey(spectrum.ID)) continue;

                            msmsFeatures.Add(spectrum);
                            spectraTracker.Add(spectrum.ID, spectrum);

                            // We are prepping the sequences that we found from peptides that were 
                            // matched only, not all of the results. 
                            // These maps here are made to help establish database search results to msms 
                            // spectra
                            foreach (var peptide in spectrum.Peptides)
                            {
                                peptide.GroupId = datasetId;
                                var newPeptide = new DatabaseSearchSequence(peptide, feature.ID);
                                mappedPeptides.Add(newPeptide);

                                var sequenceMap = new SequenceToMsnFeature
                                {
                                    UmcFeatureId    = feature.ID,
                                    DatasetId       = msFeature.GroupID,
                                    MsnFeatureId    = spectrum.ID,
                                    SequenceId      = peptide.ID
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
        /// Creates LCMS Features 
        /// </summary>
        private List<UMCLight> CreateLcmsFeatures(
                                        DatasetInformation information,
                                        List<MSFeatureLight> msFeatures,
                                        LCMSFeatureFindingOptions options,
                                        FeatureFilterOptions filterOptions)
        {                                    
            // Make features
            if (msFeatures.Count < 1)
                throw new Exception("No features were found in the feature files provided.");

            // Filter out bad MS Features
            var filteredMsFeatures = LCMSFeatureFilters.FilterMSFeatures(msFeatures, options);

            UpdateStatus("Finding features.");
            ISpectraProvider provider = null;
            if (information.RawPath != null)
            {
                provider = RawLoaderFactory.CreateFileReader(information.RawPath);
                provider.AddDataFile(information.RawPath, 0);
            }
            var finder = FeatureFinderFactory.CreateFeatureFinder(options.FeatureFinderAlgorithm);
            var features = finder.FindFeatures(filteredMsFeatures, options, provider);

            UpdateStatus("Filtering features.");
            var filteredFeatures = LCMSFeatureFilters.FilterFeatures(features,
                                                                 filterOptions);
            
            UpdateStatus(string.Format("Filtered features from: {0} to {1}.", features.Count, filteredFeatures.Count));
            return filteredFeatures;
        }

        /// <summary>
        /// Load a single dataset from the provider.
        /// </summary>
        /// <returns></returns>
        public IList<UMCLight> LoadDataset( DatasetInformation dataset,
                                            LCMSFeatureFindingOptions options,
                                            FeatureFilterOptions filterOptions)
        {

            UpdateStatus(string.Format("[{0}] - Loading dataset [{0}] - {1}.", dataset.DatasetId, dataset.DatasetName));
            var datasetId = dataset.DatasetId;
            var features = UMCLoaderFactory.LoadUmcFeatureData(dataset, Providers.FeatureCache);

            UpdateStatus(string.Format("[{0}] Loading MS Feature Data [{0}] - {1}.", dataset.DatasetId,
                dataset.DatasetName));
            var msFeatures = UMCLoaderFactory.LoadMsFeatureData(dataset, Providers.MSFeatureCache);
            var msnSpectra = new List<MSSpectra>();

            // If we don't have any features, then we have to create some from the MS features
            // provided to us.
            if (features.Count < 1)
            {
                var scanMap = dataset.DatasetSummary.ScanMetaData;

                if (scanMap != null && scanMap.Count > 0)
                    msFeatures =
                        msFeatures.Where(x => scanMap.ContainsKey(x.Scan) && scanMap[x.Scan].MsLevel == 1).ToList();

                features = CreateLcmsFeatures(dataset,
                                                msFeatures,                                                
                                                options,
                                                filterOptions);

                var maxScan = Convert.ToDouble(features.Max(feature => feature.Scan));
                var minScan = Convert.ToDouble(features.Min(feature => feature.Scan));
                var id      = 0;
                    
                foreach (var feature in features)
                {
                    feature.ID              = id++;
                    feature.RetentionTime   = (Convert.ToDouble(feature.Scan) - minScan) / (maxScan - minScan);   
                    feature.NET             = feature.RetentionTime;
                    feature.MassMonoisotopicAligned = feature.MassMonoisotopic;
                    feature.NETAligned      = feature.NET;                    
                    feature.GroupID         = datasetId;
                    feature.SpectralCount   = feature.MSFeatures.Count;

                    foreach (var msFeature in feature.MSFeatures.Where(msFeature => msFeature != null))
                    {
                        msFeature.UMCID     = feature.ID;
                        msFeature.GroupID   = datasetId;
                        msFeature.MSnSpectra.ForEach(x => x.GroupID = datasetId);
                        msnSpectra.AddRange(msFeature.MSnSpectra);
                    }
                }
            }
            else
            {
                if (!UMCLoaderFactory.AreExistingFeatures(dataset))
                {
                    var i = 0;
                    foreach (var feature in features)
                    {
                        feature.GroupID = datasetId;
                        feature.ID = i++;
                    }
                }

                // Otherwise, we need to map the MS features to the LCMS Features provided.
                // This would mean that we extracted data from an existing database.
                if (msFeatures.Count > 0)
                {
                    var map = FeatureDataConverters.MapFeature(features);
                    foreach (var feature in
                        from feature in msFeatures
                        let doesFeatureExists = map.ContainsKey(feature.UMCID)
                        where doesFeatureExists
                        select feature)
                    {
                        map[feature.UMCID].AddChildFeature(feature);
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
                peptideList.ForEach(x => x.ID = count++);

                UpdateStatus("Linking MS/MS to any known Peptide/Metabolite Sequences");

                var linker = new PeptideMsMsLinker();
                linker.LinkPeptidesToSpectra(msnSpectra, peptideList);
            }
            return features;
        }


    }
}
