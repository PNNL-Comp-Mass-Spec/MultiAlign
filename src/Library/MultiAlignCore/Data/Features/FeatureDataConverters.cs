#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.Data.Features
{
    public static class FeatureDataConverters
    {
        /// <summary>
        ///     Maps a feature list to a dictionary where the key is the feature ID.  This is useful for converting
        ///     between feature types for older algorithms.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="features"></param>
        /// <returns></returns>
        public static Dictionary<int, T> MapFeature<T>(IEnumerable<T> features) where T : FeatureLight
        {
            var map = new Dictionary<int, T>();

            foreach (var feature in features)
            {
                map.Add(feature.Id, feature);
            }
            return map;
        }     

        /// <summary>
        ///     Converts mass tags to UMC's.
        /// </summary>
        /// <param name="massTags"></param>
        /// <returns></returns>
        public static List<UMCLight> ConvertToUMC(List<MassTagLight> massTags)
        {
            var baselineFeatures = new List<UMCLight>();
            // Convert the mass tags to features.                
            foreach (var tag in massTags)
            {
                var umc = new UMCLight
                {
                    ChargeState = 0,
                    Net = tag.Net,
                    MassMonoisotopic = tag.MassMonoisotopic,
                    MassMonoisotopicAligned = tag.MassMonoisotopicAligned,
                    DriftTime = tag.DriftTime,
                    Id = tag.Id
                };
                umc.ChargeState = tag.ChargeState;
                umc.GroupId = -1;
                baselineFeatures.Add(umc);
            }
            return baselineFeatures;
        }
    }
}