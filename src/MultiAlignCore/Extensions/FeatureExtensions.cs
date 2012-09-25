using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLOmics.Data;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Features;

namespace MultiAlignCore.Extensions
{
    public static class FeatureExtensions
    {
        /// <summary>
        /// Creates a charge map for a given ms feature list.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, List<MSFeatureLight>> CreateChargeMap(this UMCLight feature)
        {
            Dictionary<int, List<MSFeatureLight>> chargeMap = new Dictionary<int, List<MSFeatureLight>>();
            foreach (MSFeatureLight msFeature in feature.MSFeatures)
            {
                if (!chargeMap.ContainsKey(msFeature.ChargeState))
                {
                    chargeMap.Add(msFeature.ChargeState, new List<MSFeatureLight>());
                }
                chargeMap[msFeature.ChargeState].Add(msFeature);
            }

            return chargeMap;
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


        public static void ReconstructUMCCluster(this UMCClusterLight cluster, FeatureDataAccessProviders providers)
        {
            List<UMCLight> features = providers.FeatureCache.FindByClusterID(cluster.ID);
            foreach(UMCLight feature in features)
            {
                    cluster.AddChildFeature(feature);
                    feature.ReconstructUMC(providers);
            }
        }
        public static void ReconstructUMC(this UMCLight feature, FeatureDataAccessProviders providers)
        {
            // This is easy to grab all ms features for this feature.
            List<MSFeatureLight> msFeatures = providers.MSFeatureCache.FindByFeatureId(feature.GroupID, feature.ID);

            // We are reconstruction the objects here.  But 
            // I want to reduce the number of transactions to make.
            // So I go back through the database and pull out all msms spectra first
            // then sort it out in memory.

            // Get the map
            List<MSFeatureToMSnFeatureMap> msmsFeatures = 
                providers.MSFeatureToMSnFeatureCache.FindByUMCFeatureId(feature.GroupID,
                                                                        feature.ID);

            // Then grab the spectra id list
            List<int> ids = msmsFeatures.ConvertAll<int> (x => x.MSMSFeatureID);
            
            // Make a map, from the dataset, then the spectra to the ms feature Id.
            Dictionary<int, Dictionary<int, MSFeatureToMSnFeatureMap>> map 
                            = new Dictionary<int, Dictionary<int, MSFeatureToMSnFeatureMap>>();

            // construct that map here.
            foreach (MSFeatureToMSnFeatureMap subFeature in msmsFeatures)
            {
                // first map the dataset id
                if (!map.ContainsKey(subFeature.MSDatasetID))
                {
                    map.Add(subFeature.MSDatasetID, new Dictionary<int,MSFeatureToMSnFeatureMap>());
                }

                // Then map its msms spectra id
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
            
            foreach (MSFeatureLight msFeature in msFeatures)
            {
                feature.AddChildFeature(msFeature);

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
        }     
    }
}
