using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLOmics.Data.Features;
using PNNLOmics.Data;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Data.MassTags;
using PNNLOmics;

namespace MultiAlignCore.Data.Features
{
    public static class FeatureDataConverters
    {
        /// <summary>
        /// Maps a feature list to a dictionary where the key is the feature ID.  This is useful for converting 
        /// between feature types for older algorithms.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="features"></param>
        /// <returns></returns>
        public static Dictionary<int, T> MapFeature<T>(IEnumerable<T> features) where T : FeatureLight
        {
            Dictionary<int, T> map = new Dictionary<int, T>();

            foreach (T feature in features)
            {
                map.Add(feature.ID, feature);
            }
            return map;
        }

        public static clsUMC ConvertToUMC(UMCLight feature)
        {
            clsUMC umc               = new clsUMC();
            umc.AbundanceMax         = feature.Abundance;
            umc.AbundanceSum         = feature.AbundanceSum;
            umc.AverageDeconFitScore = -1;
            umc.ChargeRepresentative = Convert.ToInt16(feature.ChargeState);
            umc.DatasetId            = feature.GroupID;
            umc.ScanEnd              = feature.ScanEnd;
            umc.ScanStart            = feature.ScanStart;
            int maxCharge            = feature.ChargeState;
            long maxAbundance        = long.MinValue;
            int scan                 = feature.ScanStart;
            float mz                 = Convert.ToSingle(feature.Mz);
            foreach (MSFeatureLight msFeature in feature.MSFeatures)
            {
                maxCharge = Math.Max(msFeature.ChargeState, maxCharge);
                if (msFeature.Abundance > maxAbundance)
                {
                    scan            = msFeature.Scan;
                    maxAbundance    = msFeature.Abundance;
                    mz              = Convert.ToSingle(msFeature.Mz);
                }                    
            }
            umc.ChargeMax               = Convert.ToInt16(maxCharge);
            umc.Scan                    = scan;
            umc.MZForCharge             = mz;
            umc.Net                     = feature.RetentionTime;
            umc.DriftTime               = feature.DriftTime;
            umc.Id                      = feature.ID;
            umc.mint_umc_id             = feature.ID;
            umc.mint_umc_index          = feature.ID;
            umc.Mass                    = feature.MassMonoisotopic;
            umc.MassCalibrated          = feature.MassMonoisotopic;
            umc.ChargeRepresentative    = Convert.ToInt16(feature.ChargeState);
            umc.ConformationId          = 0;

            return umc;
        }

        public static List<clsUMC> ConvertToUMC(IEnumerable<UMCLight> features)
        {
            List<clsUMC> oldFeatures = new List<clsUMC>();
            foreach (UMCLight feature in features)
            {
                oldFeatures.Add(ConvertToUMC(feature));
            }
            return oldFeatures;
        }

        public static List<clsMassTag> ConvertToMassTag(List<MassTagLight> massTags)
        {
            List<clsMassTag> tags = new List<clsMassTag>();
            foreach (MassTagLight tag in massTags)
            {
                // mixed mode tag
                clsMassTag mmTag        = new clsMassTag();
                mmTag.mintMassTagId     = tag.ID;
                mmTag.mintConformerID   = tag.ConformationID;
                mmTag.mdblAvgGANET      = tag.NETAverage;
                mmTag.mdblMonoMass      = tag.MassMonoisotopic;
                mmTag.DriftTime         = tag.DriftTime;
                tags.Add(mmTag);
            }
            return tags;
        }
        /// <summary>
        /// Converts mass tags to UMC's.
        /// </summary>
        /// <param name="massTags"></param>
        /// <returns></returns>
        public static List<UMCLight> ConvertToUMC(List<MassTagLight> massTags)
        {
            List<UMCLight> baselineFeatures = new List<UMCLight>();                    
            // Convert the mass tags to features.                
            foreach (MassTagLight tag in massTags)
            {
                UMCLight umc                = new UMCLight();
                umc.ChargeState             = 0;
                umc.NET                     = tag.NET;
                umc.MassMonoisotopicAligned = tag.MassMonoisotopic;
                umc.DriftTime               = tag.DriftTime;
                umc.ID                      = tag.ID;
                umc.ChargeState             = tag.ChargeState;
                umc.GroupID                 = -1;
                baselineFeatures.Add(umc);
            }
            return baselineFeatures;
        }
    }
}
