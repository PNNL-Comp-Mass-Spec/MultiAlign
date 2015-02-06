#region

using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data;
using MultiAlignCore.Data.SequenceData;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Extensions;

#endregion

namespace MultiAlignCore.Extensions
{
    public static class FeatureExtensions
    {
        public static Dictionary<int, int> CreateChargeMap<T>(this List<T> features) where T : FeatureLight
        {
            var map = new Dictionary<int, int>();
            foreach (var feature in features)
            {
                if (!map.ContainsKey(feature.ChargeState))
                {
                    map.Add(feature.ChargeState, 0);
                }
                map[feature.ChargeState]++;
            }
            return map;
        }

        public static Dictionary<int, List<T>> MapCharges<T>(this List<T> features) where T : FeatureLight
        {
            var map = new Dictionary<int, List<T>>();
            foreach (var feature in features)
            {
                if (!map.ContainsKey(feature.ChargeState))
                {
                    map.Add(feature.ChargeState, new List<T>());
                }
                map[feature.ChargeState].Add(feature);
            }
            return map;
        }

        /// <summary>
        ///     Creates SIC's mapped by charge state for the MS Features in the feature.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="provider">Object that can read data from a raw file or data source.</param>
        /// <returns></returns>
        public static Dictionary<int, List<XYZData>> CreateChargeSIC(this UMCLight feature, ISpectraProvider provider)
        {
            var chargeMap = feature.CreateChargeMap();
            var sicMap = new Dictionary<int, List<XYZData>>();

            foreach (var charge in chargeMap.Keys)
            {
                chargeMap[charge].Sort(delegate(MSFeatureLight x, MSFeatureLight y) { return x.Scan.CompareTo(y.Scan); }
                    );
                var data = chargeMap[charge].ConvertAll(x => new XYZData(x.Scan, x.Abundance, x.Mz));
                sicMap.Add(charge, data);
            }

            if (provider != null)
            {
                // Creates an SIC map for a given charge state of the feature.
                foreach (var charge in sicMap.Keys)
                {
                    var data = sicMap[charge];

                    // The data is alread sorted.
                    var minScan = int.MaxValue;
                    var maxScan = int.MinValue;
                    var mzValues = new List<double>();
                    foreach (var x in data)
                    {
                        mzValues.Add(x.Z);
                        minScan = Math.Min(minScan, Convert.ToInt32(x.X));
                        maxScan = Math.Max(maxScan, Convert.ToInt32(x.X));
                    }
                    mzValues.Sort();
                    double mz = 0;
                    var mid = Convert.ToInt32(mzValues.Count/2);
                    mz = mzValues[mid];
                    minScan -= 20;
                    maxScan += 20;

                    // Build the SIC
                    var intensities = new List<XYZData>();
                    for (var scan = minScan; scan < maxScan; scan++)
                    {
                        var summary = new ScanSummary();
                        var spectrum = provider.GetRawSpectra(scan, feature.GroupId, 1, out summary);
                        double intensity = 0;
                        var minDistance = double.MaxValue;
                        var index = -1;
                        for (var i = 0; i < spectrum.Count; i++)
                        {
                            var distance = spectrum[i].X - mz;
                            if (distance < minDistance)
                            {
                                index = i;
                                minDistance = distance;
                            }
                        }

                        if (index >= 0)
                        {
                            intensity = spectrum[index].Y;
                        }
                        var newPoint = new XYZData(scan, intensity, mz);
                        intensities.Add(newPoint);
                    }

                    sicMap[charge] = intensities;
                }
            }

            return sicMap;
        }

        /// <summary>
        ///     Creates SIC's mapped by charge state for the MS Features in the feature.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, List<XYZData>> CreateChargeSIC(this UMCLight feature)
        {
            var chargeMap = feature.CreateChargeMap();
            var sicMap = new Dictionary<int, List<XYZData>>();

            foreach (var charge in chargeMap.Keys)
            {
                chargeMap[charge].Sort(delegate(MSFeatureLight x, MSFeatureLight y) { return x.Scan.CompareTo(y.Scan); }
                    );
                var data = chargeMap[charge].ConvertAll(x => new XYZData(x.Scan, x.Abundance, x.Mz));
                sicMap.Add(charge, data);
            }
            return sicMap;
        }

        /// <summary>
        ///     Creates SIC's mapped by charge state for the MS Features in the feature.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, List<XYZData>> CreateChargeSICForMonoMass(this UMCLight feature)
        {
            var chargeMap = feature.CreateChargeMap();
            var sicMap = new Dictionary<int, List<XYZData>>();

            foreach (var charge in chargeMap.Keys)
            {
                chargeMap[charge].Sort(delegate(MSFeatureLight x, MSFeatureLight y) { return x.Scan.CompareTo(y.Scan); }
                    );
                var data = chargeMap[charge].ConvertAll(x => new XYZData(x.Scan, x.Abundance, x.MassMonoisotopicAligned));
                sicMap.Add(charge, data);
            }
            return sicMap;
        }

        /// <summary>
        ///     Reconstructions a UMC
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="providers"></param>
        public static void ReconstructUMC(this UMCLight feature, FeatureDataAccessProviders providers, bool getMsMs)
        {
            // This is easy to grab all ms features for this feature.
            var msFeatures = providers.MSFeatureCache.FindByFeatureId(feature.GroupId, feature.Id);


            foreach (var msFeature in msFeatures)
            {
                feature.AddChildFeature(msFeature);
            }

            if (getMsMs)
            {
                feature.ReconstructMSFeature(providers, getMsMs);
            }
        }

        /// <summary>
        ///     Reconstructs a MS Feature by loading the MS/MS data
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="providers"></param>
        public static void ReconstructMSFeature(this MSFeatureLight msFeature, FeatureDataAccessProviders providers)
        {
            // We are reconstruction the objects here.  But 
            // I want to reduce the number of transactions to make.
            // So I go back through the database and pull out all msms spectra first
            // then sort it out in memory.

            // Get the map
            var msmsFeatures = new List<MSFeatureToMSnFeatureMap>();

            // Maps the id's of the MS/MS spectra
            var ids = new List<int>();

            // Make a map, from the dataset, then the spectra to the ms feature Id.
            var map
                = new Dictionary<int, Dictionary<int, MSFeatureToMSnFeatureMap>>();


            msmsFeatures = providers.MSFeatureToMSnFeatureCache.FindByUMCFeatureId(msFeature.GroupId,
                msFeature.Id);
            // Then grab the spectra id list
            ids = msmsFeatures.ConvertAll(x => x.MSMSFeatureID);


            // construct that map here.
            foreach (var subFeature in msmsFeatures)
            {
                // first map the dataset id
                if (!map.ContainsKey(subFeature.MSDatasetID))
                {
                    map.Add(subFeature.MSDatasetID, new Dictionary<int, MSFeatureToMSnFeatureMap>());
                }

                //TODO: There may be multiple MSMS spectra
                // Then map its msms spectra id
                if (map[subFeature.MSDatasetID].ContainsKey(subFeature.MSFeatureID))
                    continue;

                map[subFeature.MSDatasetID].Add(subFeature.MSFeatureID, subFeature);
            }

            // Now we get all the spectra, map to the UMC, then the ms/ms spectra. 
            var spectra = providers.MSnFeatureCache.FindBySpectraId(ids);

            var spectraMap = new Dictionary<int, Dictionary<int, MSSpectra>>();
            foreach (var spectrum in spectra)
            {
                if (!spectraMap.ContainsKey(spectrum.GroupId))
                {
                    spectraMap.Add(spectrum.GroupId, new Dictionary<int, MSSpectra>());
                }
                spectraMap[spectrum.GroupId].Add(spectrum.Id, spectrum);
            }

            // Here we check the dataset.
            if (map.ContainsKey(msFeature.GroupId))
            {
                // then check the ms/ms spectra
                if (map[msFeature.GroupId].ContainsKey(msFeature.Id))
                {
                    // ok, we are sure that the spectra is present now!
                    var singleMap = map[msFeature.GroupId][msFeature.Id];
                    var spectrum = spectraMap[singleMap.MSDatasetID][singleMap.MSMSFeatureID];
                    msFeature.MSnSpectra.Add(spectrum);
                }
            }
        }

        public static void ReconstructMSFeature(this UMCLight feature, FeatureDataAccessProviders providers,
            bool getMsMs)
        {
            var msmsFeatureMaps = new List<MSFeatureToMSnFeatureMap>();
            var mappedSequences = new List<SequenceToMsnFeature>();
            var sequences = new List<DatabaseSearchSequence>();

            if (getMsMs)
            {
                msmsFeatureMaps = providers.MSFeatureToMSnFeatureCache.FindByUMCFeatureId(feature.GroupId, feature.Id);
                mappedSequences = providers.SequenceMsnMapCache.FindByDatasetId(feature.GroupId, feature.Id);
                sequences = providers.DatabaseSequenceCache.FindByDatasetId(feature.GroupId, feature.Id);
            }
            var ids = msmsFeatureMaps.ConvertAll(x => x.MSMSFeatureID);
            var spectra = providers.MSnFeatureCache.FindBySpectraId(ids);
            spectra = spectra.Where(x => x.GroupId == feature.GroupId).ToList();

            // Map the sequences into peptide objects...
            // Map the MS features, spectra, and peptide sequences
            var peptideSequenceMaps = MapSequenceMapsToPeptides(sequences);
            var msFeatures = new Dictionary<int, MSFeatureLight>();
            var spectraMap = new Dictionary<int, MSSpectra>();
            var sequenceMap = new Dictionary<int, int>();

            foreach (var map in mappedSequences)
            {
                if (sequenceMap.ContainsKey(map.MsnFeatureId))
                    continue;

                sequenceMap.Add(map.MsnFeatureId, map.SequenceId);
            }

            spectra.ForEach(x => spectraMap.Add(x.Id, x));
            feature.MsFeatures.ForEach(x => msFeatures.Add(x.Id, x));

            foreach (var msmsFeature in msmsFeatureMaps)
            {
                var msFeatureId = msmsFeature.MSFeatureID;
                var spectraId = msmsFeature.MSMSFeatureID;

                if (!msFeatures.ContainsKey(msFeatureId))
                    continue;

                if (!spectraMap.ContainsKey(spectraId))
                    continue;

                var msFeature = msFeatures[msFeatureId];
                var spectrum = spectraMap[spectraId];

                // Annotate the spectra with a peptide sequence
                if (sequenceMap.ContainsKey(spectraId))
                {
                    var peptideId = sequenceMap[spectraId];
                    var hasPeptide = new Dictionary<int, Peptide>();
                    foreach (var peptide in peptideSequenceMaps.Values)
                    {
                        if (peptide.Scan == spectrum.Scan)
                        {
                            if (hasPeptide.ContainsKey(peptide.Scan))
                                continue;

                            hasPeptide.Add(peptide.Scan, peptide);
                            spectrum.Peptides.Add(peptide);
                            peptide.Spectrum = spectrum;
                        }
                    }
                }
                msFeature.MSnSpectra.Add(spectrum);
                spectrum.ParentFeature = msFeature;
            }
        }

        private static Dictionary<int, Peptide> MapSequenceMapsToPeptides(IEnumerable<DatabaseSearchSequence> sequences)
        {
            var peptideSequenceMaps = new Dictionary<int, Peptide>();
            foreach (var sequence in sequences)
            {
                var newPeptide = new Peptide
                {
                    GroupId = sequence.GroupId,
                    Sequence = sequence.Sequence,
                    Score = sequence.Score,
                    Scan = sequence.Scan,
                    Mz = sequence.Mz,
                    MassMonoisotopic = sequence.MassMonoisotopic,
                    Id = sequence.Id
                };

                if (peptideSequenceMaps.ContainsKey(newPeptide.Id))
                    continue;

                peptideSequenceMaps.Add(newPeptide.Id, newPeptide);
            }

            return peptideSequenceMaps;
        }

        /// <summary>
        ///     Finds the ranges of a given feature for all of its dimensions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="features"></param>
        /// <param name="minMass"></param>
        /// <param name="maxMass"></param>
        /// <param name="minNet"></param>
        /// <param name="maxNet"></param>
        /// <param name="minDrift"></param>
        /// <param name="maxDrift"></param>
        public static void FindRanges<T>(this List<T> features,
            out double minMass,
            out double maxMass,
            out double minNet,
            out double maxNet,
            out double minDrift,
            out double maxDrift) where T : FeatureLight
        {
            minMass = double.MaxValue;
            maxMass = double.MinValue;
            minNet = double.MaxValue;
            maxNet = double.MinValue;
            minDrift = double.MaxValue;
            maxDrift = double.MinValue;

            // Find the mins
            foreach (var feature in features)
            {
                minMass = Math.Min(minMass, feature.MassMonoisotopicAligned);
                maxMass = Math.Max(maxMass, feature.MassMonoisotopicAligned);

                minNet = Math.Min(minNet, feature.Net);
                maxNet = Math.Max(maxNet, feature.Net);

                minDrift = Math.Min(minDrift, feature.DriftTime);
                maxDrift = Math.Max(maxDrift, feature.DriftTime);
            }
        }

        public static bool HasMsMs(this UMCLight feature)
        {
            foreach (var msFeature in feature.MsFeatures)
            {
                bool hasMsMs = msFeature.HasMsMs();

                if (hasMsMs)
                    return true;
            }
            return false;
        }

        public static bool HasMsMs(this MSFeatureLight msFeature)
        {
            return msFeature.MSnSpectra.Count > 0;
        }
    }
}