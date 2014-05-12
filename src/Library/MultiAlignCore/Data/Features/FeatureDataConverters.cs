#region

using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

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

        public static clsUMC ConvertToUMC(UMCLight feature)
        {
            var umc = new clsUMC();
            umc.AbundanceMax = feature.Abundance;
            umc.AbundanceSum = feature.AbundanceSum;
            umc.AverageDeconFitScore = -1;
            umc.ChargeRepresentative = Convert.ToInt16(feature.ChargeState);
            umc.DatasetId = feature.GroupId;
            umc.ScanEnd = feature.ScanEnd;
            umc.ScanStart = feature.ScanStart;
            var maxCharge = feature.ChargeState;
            var maxAbundance = long.MinValue;
            var scan = feature.ScanStart;
            var mz = Convert.ToSingle(feature.Mz);
            foreach (var msFeature in feature.MsFeatures)
            {
                maxCharge = Math.Max(msFeature.ChargeState, maxCharge);
                if (msFeature.Abundance > maxAbundance)
                {
                    scan = msFeature.Scan;
                    maxAbundance = msFeature.Abundance;
                    mz = Convert.ToSingle(msFeature.Mz);
                }
            }
            umc.ChargeMax = Convert.ToInt16(maxCharge);
            umc.Scan = scan;
            umc.MZForCharge = mz;
            umc.Net = feature.RetentionTime;
            umc.DriftTime = feature.DriftTime;
            umc.Id = feature.Id;
            umc.mint_umc_id = feature.Id;
            umc.mint_umc_index = feature.Id;
            umc.Mass = feature.MassMonoisotopic;
            umc.MassCalibrated = feature.MassMonoisotopic;
            umc.ChargeRepresentative = Convert.ToInt16(feature.ChargeState);
            umc.ConformationId = 0;

            return umc;
        }

        public static List<clsUMC> ConvertToUMC(IEnumerable<UMCLight> features)
        {
            var oldFeatures = new List<clsUMC>();
            foreach (var feature in features)
            {
                oldFeatures.Add(ConvertToUMC(feature));
            }
            return oldFeatures;
        }

        public static List<clsMassTag> ConvertToMassTag(List<MassTagLight> massTags)
        {
            var tags = new List<clsMassTag>();
            foreach (var tag in massTags)
            {
                // mixed mode tag
                var mmTag = new clsMassTag();
                mmTag.mintMassTagId = tag.Id;
                mmTag.mintConformerID = tag.ConformationId;
                mmTag.mdblAvgGANET = tag.NetAverage;
                mmTag.mdblMonoMass = tag.MassMonoisotopic;
                mmTag.DriftTime = tag.DriftTime;
                tags.Add(mmTag);
            }
            return tags;
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
                var umc = new UMCLight();
                umc.ChargeState = 0;
                umc.Net = tag.Net;
                umc.MassMonoisotopic = tag.MassMonoisotopic;
                umc.MassMonoisotopicAligned = tag.MassMonoisotopic;
                umc.DriftTime = tag.DriftTime;
                umc.Id = tag.Id;
                umc.ChargeState = tag.ChargeState;
                umc.GroupId = -1;
                baselineFeatures.Add(umc);
            }
            return baselineFeatures;
        }
    }
}