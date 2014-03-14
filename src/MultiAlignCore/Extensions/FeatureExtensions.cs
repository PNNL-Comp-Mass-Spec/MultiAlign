using System.Linq;
using MultiAlignCore.Data;
using MultiAlignCore.Data.SequenceData;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Extensions;
using System;
using System.Collections.Generic;

namespace MultiAlignCore.Extensions
{
    public static class FeatureExtensions
    {
        public static Dictionary<int, int> CreateChargeMap<T>(this List<T> features) where T : FeatureLight
        {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (T feature in features)
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
            Dictionary<int, List<T>> map = new Dictionary<int, List<T>>();
            foreach (T feature in features)
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
        /// Creates SIC's mapped by charge state for the MS Features in the feature.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="provider">Object that can read data from a raw file or data source.</param>
        /// <returns></returns>
        public static Dictionary<int, List<XYZData>> CreateChargeSIC(this UMCLight feature, ISpectraProvider provider)
        {            
            Dictionary<int, List<MSFeatureLight>> chargeMap = feature.CreateChargeMap();
            Dictionary<int, List<XYZData>> sicMap = new Dictionary<int, List<XYZData>>();

            foreach (int charge in chargeMap.Keys)
            {
                chargeMap[charge].Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                {
                    return x.Scan.CompareTo(y.Scan);
                }
                );
                List<XYZData> data = chargeMap[charge].ConvertAll<XYZData>(x => new XYZData(x.Scan, x.Abundance, x.Mz));
                sicMap.Add(charge, data);
            }

            if (provider != null)
            {
                // Creates an SIC map for a given charge state of the feature.
                foreach (int charge in sicMap.Keys)
                {
                    List<XYZData> data = sicMap[charge];

                    // The data is alread sorted.
                    int minScan = int.MaxValue;
                    int maxScan = int.MinValue;
                    List<double> mzValues = new List<double>();
                    foreach (XYZData x in data)
                    {
                        mzValues.Add(x.Z);
                        minScan = Math.Min(minScan, Convert.ToInt32(x.X));
                        maxScan = Math.Max(maxScan, Convert.ToInt32(x.X));
                    }
                    mzValues.Sort();
                    double mz = 0;
                    int mid = Convert.ToInt32(mzValues.Count / 2);
                    mz = mzValues[mid];
                    minScan -= 20;
                    maxScan += 20;

                    // Build the SIC
                    List<XYZData> intensities = new List<XYZData>();
                    for (int scan = minScan; scan < maxScan; scan++)
                    {
                        var summary = new ScanSummary();
                        List<XYData> spectrum = provider.GetRawSpectra(scan, feature.GroupID, 1, out summary);
                        double intensity = 0;
                        double minDistance = double.MaxValue;
                        int index = -1;
                        for (int i = 0; i < spectrum.Count; i++)
                        {
                            double distance = spectrum[i].X - mz;
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
                        XYZData newPoint = new XYZData(scan, intensity, mz);
                        intensities.Add(newPoint);
                    }

                    sicMap[charge] = intensities;
                }
            }

            return sicMap;
        }
        /// <summary>
        /// Creates SIC's mapped by charge state for the MS Features in the feature.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, List<XYZData>> CreateChargeSIC(this UMCLight feature)
        {
            Dictionary<int, List<MSFeatureLight>> chargeMap = feature.CreateChargeMap();
            Dictionary<int, List<XYZData>> sicMap = new Dictionary<int, List<XYZData>>();

            foreach (int charge in chargeMap.Keys)
            {
                chargeMap[charge].Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                {
                    return x.Scan.CompareTo(y.Scan);
                }
                );
                List<XYZData> data = chargeMap[charge].ConvertAll<XYZData>(x => new XYZData(x.Scan, x.Abundance, x.Mz));
                sicMap.Add(charge, data);
            }
            return sicMap;
        }
        /// <summary>
        /// Creates SIC's mapped by charge state for the MS Features in the feature.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, List<XYZData>> CreateChargeSICForMonoMass(this UMCLight feature)
        {
            Dictionary<int, List<MSFeatureLight>> chargeMap = feature.CreateChargeMap();
            Dictionary<int, List<XYZData>> sicMap = new Dictionary<int, List<XYZData>>();

            foreach (int charge in chargeMap.Keys)
            {
                chargeMap[charge].Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                {
                    return x.Scan.CompareTo(y.Scan);
                }
                );
                List<XYZData> data = chargeMap[charge].ConvertAll<XYZData>(x => new XYZData(x.Scan, x.Abundance, x.MassMonoisotopicAligned));
                sicMap.Add(charge, data);
            }
            return sicMap;
        }        
        /// <summary>
        /// Reconstructions a UMC
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="providers"></param>
        public static void ReconstructUMC(this UMCLight feature, FeatureDataAccessProviders providers, bool getMsMs)
        {
            // This is easy to grab all ms features for this feature.
            List<MSFeatureLight> msFeatures = providers.MSFeatureCache.FindByFeatureId(feature.GroupID, feature.ID);


            foreach (MSFeatureLight msFeature in msFeatures)
            {                
                feature.AddChildFeature(msFeature);
            }

            if (getMsMs)
            {
                feature.ReconstructMSFeature(providers, getMsMs);
            }
        }
        /// <summary>
        /// Reconstructs a MS Feature by loading the MS/MS data
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
            List<MSFeatureToMSnFeatureMap> msmsFeatures = new List<MSFeatureToMSnFeatureMap>();

            // Maps the id's of the MS/MS spectra
            List<int> ids = new List<int>();

            // Make a map, from the dataset, then the spectra to the ms feature Id.
            Dictionary<int, Dictionary<int, MSFeatureToMSnFeatureMap>> map
                            = new Dictionary<int, Dictionary<int, MSFeatureToMSnFeatureMap>>();

            
            msmsFeatures =  providers.MSFeatureToMSnFeatureCache.FindByUMCFeatureId(msFeature.GroupID,
                                                                                        msFeature.ID);
            // Then grab the spectra id list
            ids = msmsFeatures.ConvertAll<int>(x => x.MSMSFeatureID);
            
                        
            // construct that map here.
            foreach (MSFeatureToMSnFeatureMap subFeature in msmsFeatures)
            {
                // first map the dataset id
                if (!map.ContainsKey(subFeature.MSDatasetID))
                {
                    map.Add(subFeature.MSDatasetID, new Dictionary<int,MSFeatureToMSnFeatureMap>());
                }

                //TODO: There may be multiple MSMS spectra
                // Then map its msms spectra id
                if (map[subFeature.MSDatasetID].ContainsKey(subFeature.MSFeatureID))
                    continue;

                map[subFeature.MSDatasetID].Add(subFeature.MSFeatureID, subFeature);
            }
            
            // Now we get all the spectra, map to the UMC, then the ms/ms spectra. 
            List<MSSpectra> spectra               = providers.MSnFeatureCache.FindBySpectraId(ids);                  
            
            Dictionary<int, Dictionary<int, MSSpectra>> spectraMap = new Dictionary<int,Dictionary<int,MSSpectra>>();
            foreach(MSSpectra spectrum in spectra)
            {
                if (!spectraMap.ContainsKey(spectrum.GroupID))
                {
                    spectraMap.Add(spectrum.GroupID, new Dictionary<int,MSSpectra>());
                }
                spectraMap[spectrum.GroupID].Add(spectrum.ID, spectrum);
            }
                                        
            // Here we check the dataset.
            if (map.ContainsKey(msFeature.GroupID))
            {
                // then check the ms/ms spectra
                if (map[msFeature.GroupID].ContainsKey(msFeature.ID))
                {
                    // ok, we are sure that the spectra is present now!
                    MSFeatureToMSnFeatureMap singleMap = map[msFeature.GroupID][msFeature.ID];
                    MSSpectra spectrum = spectraMap[singleMap.MSDatasetID][singleMap.MSMSFeatureID];
                    msFeature.MSnSpectra.Add(spectrum);
                }
            }                              
        }
        public static void ReconstructMSFeature(this UMCLight feature, FeatureDataAccessProviders providers, bool getMsMs)
        {
            var msmsFeatureMaps = new List<MSFeatureToMSnFeatureMap>();         
            var mappedSequences = new List<SequenceToMsnFeature>();
            var sequences       = new List<DatabaseSearchSequence>();

            if (getMsMs)
            {
                msmsFeatureMaps  = providers.MSFeatureToMSnFeatureCache.FindByUMCFeatureId(feature.GroupID, feature.ID);
                mappedSequences  = providers.SequenceMsnMapCache.FindByDatasetId(feature.GroupID, feature.ID);
                sequences        = providers.DatabaseSequenceCache.FindByDatasetId(feature.GroupID, feature.ID);
            }            
            var ids         = msmsFeatureMaps.ConvertAll(x => x.MSMSFeatureID);
            var spectra     = providers.MSnFeatureCache.FindBySpectraId(ids);
            spectra         = spectra.Where(x => x.GroupID == feature.GroupID).ToList(); 

            // Map the sequences into peptide objects...
            // Map the MS features, spectra, and peptide sequences
            var peptideSequenceMaps = MapSequenceMapsToPeptides(sequences);                        
            var msFeatures          = new Dictionary<int, MSFeatureLight>();
            var spectraMap          = new Dictionary<int, MSSpectra>();
            var sequenceMap         = new Dictionary<int, int>();

            foreach (var map in mappedSequences)
            {
                if (sequenceMap.ContainsKey(map.MsnFeatureId)) continue;
                sequenceMap.Add(map.MsnFeatureId, map.SequenceId);
            }

            spectra.ForEach(x => spectraMap.Add(x.ID, x));
            feature.MSFeatures.ForEach(x => msFeatures.Add(x.ID, x));

            foreach (var msmsFeature in msmsFeatureMaps)
            {
                var msFeatureId = msmsFeature.MSFeatureID;
                var spectraId   = msmsFeature.MSMSFeatureID;

                if (!msFeatures.ContainsKey(msFeatureId)) continue;
                if (!spectraMap.ContainsKey(spectraId))   continue;

                var msFeature = msFeatures[msFeatureId];
                var spectrum  = spectraMap[spectraId];

                // Annotate the spectra with a peptide sequence
                if (sequenceMap.ContainsKey(spectraId))
                {
                    var peptideId  = sequenceMap[spectraId];
                    var hasPeptide = new Dictionary<int, Peptide>();
                    foreach (var peptide in peptideSequenceMaps.Values)
                    {
                        if (peptide.Scan == spectrum.Scan)
                        {
                            if (hasPeptide.ContainsKey(peptide.Scan))
                                continue;

                            hasPeptide.Add(peptide.Scan, peptide);
                            spectrum.Peptides.Add(peptide);    
                        }
                    }                                            
                }
                msFeature.MSnSpectra.Add(spectrum);
                spectrum.ParentFeature = msFeature;
            }
        }
                
        private static  Dictionary<int, Peptide> MapSequenceMapsToPeptides(IEnumerable<DatabaseSearchSequence> sequences)
        {
            var peptideSequenceMaps = new Dictionary<int, Peptide>();
            foreach (var sequence in sequences)
            {
                var newPeptide = new Peptide
                {
                    GroupId             = sequence.GroupId,
                    Sequence            = sequence.Sequence,
                    Score               = sequence.Score,
                    Scan                = sequence.Scan,
                    Mz                  = sequence.Mz,
                    MassMonoisotopic    = sequence.MassMonoisotopic,
                    ID                  = sequence.Id
                };

                if (peptideSequenceMaps.ContainsKey(newPeptide.ID)) continue;

                peptideSequenceMaps.Add(newPeptide.ID, newPeptide);
            }

            return peptideSequenceMaps;
        }

        /// <summary>
        /// Finds the ranges of a given feature for all of its dimensions.
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
            out double maxDrift) where T: FeatureLight
        {
            minMass = double.MaxValue;
            maxMass   = double.MinValue;
            minNet   = double.MaxValue;
            maxNet   = double.MinValue;
            minDrift = double.MaxValue;
            maxDrift = double.MinValue;

            // Find the mins
            foreach (T feature in features)
            {
                minMass  = Math.Min(minMass,  feature.MassMonoisotopicAligned);
                maxMass  = Math.Max(maxMass,  feature.MassMonoisotopicAligned);

                minNet   = Math.Min(minNet,   feature.RetentionTime);
                maxNet   = Math.Max(maxNet,   feature.RetentionTime);

                minDrift = Math.Min(minDrift, feature.DriftTime);
                maxDrift = Math.Max(maxDrift, feature.DriftTime);                
            }
        }

        public static bool HasMsMs(this UMCLight feature)
        {
            foreach (MSFeatureLight msFeature in feature.MSFeatures)
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
