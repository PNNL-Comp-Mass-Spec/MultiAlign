#region

using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.Extensions
{
    public static class MSFeatureExtensions
    {
        public static UMCLight GetParentUmc(this MSFeatureLight feature)
        {
            return feature?.GetParentFeature();
        }

        /// <summary>
        ///     Maps all of the MS Features to an indexed scan map.
        /// </summary>
        /// <param name="msFeatures"></param>
        /// <returns></returns>
        public static Dictionary<int, List<MSFeatureLight>> CreateScanMaps(this List<MSFeatureLight> msFeatures)
        {
            var msFeatureMap = new Dictionary<int, List<MSFeatureLight>>();

            foreach (var feature in msFeatures)
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
        ///     Maps all of the MS/MS spectra to an indexed scan map.
        /// </summary>
        /// <param name="msFeatures"></param>
        /// <returns></returns>
        public static Dictionary<int, List<MSSpectra>> CreateScanMapsForMsMs(this List<MSSpectra> spectra)
        {
            var msFeatureMap = new Dictionary<int, List<MSSpectra>>();

            foreach (var spectrum in spectra)
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