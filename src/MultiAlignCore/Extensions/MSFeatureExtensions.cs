using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Extensions
{
    public static class MSFeatureExtensions
    {
        /// <summary>        
        /// Maps all of the MS Features to an indexed scan map.
        /// </summary>
        /// <param name="msFeatures"></param>
        /// <returns></returns>
        public static Dictionary<int, List<MSFeatureLight>> CreateScanMaps(this List<MSFeatureLight> msFeatures)
        {
            Dictionary<int, List<MSFeatureLight>> msFeatureMap = new Dictionary<int, List<MSFeatureLight>>();

            foreach (MSFeatureLight feature in msFeatures)
            {
                if (!msFeatureMap.ContainsKey(feature.Scan))
                {
                    msFeatureMap.Add(feature.Scan, new List<MSFeatureLight>());
                }
                msFeatureMap[feature.Scan].Add(feature);
            }
            return msFeatureMap;
        }
        /// <summary>
        /// Maps all of the MS/MS spectra to an indexed scan map.
        /// </summary>
        /// <param name="msFeatures"></param>
        /// <returns></returns>
        public static Dictionary<int, List<MSSpectra>> CreateScanMapsForMsMs(this List<MSSpectra> spectra)
        {
            Dictionary<int, List<MSSpectra>> msFeatureMap = new Dictionary<int, List<MSSpectra>>();
            
            foreach (MSSpectra spectrum in spectra)
            {
                if (!msFeatureMap.ContainsKey(spectrum.Scan))
                {
                    msFeatureMap.Add(spectrum.Scan, new List<MSSpectra>());
                }
                msFeatureMap[spectrum.Scan].Add(spectrum);
            }
            

            return msFeatureMap;
        }
    }
}
