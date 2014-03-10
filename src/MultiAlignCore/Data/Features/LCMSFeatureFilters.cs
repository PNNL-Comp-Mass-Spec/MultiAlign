using System.Linq;
using MultiAlignCore.Algorithms.Options;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;

namespace MultiAlignCore.Data.Features
{
    public static class LcmsFeatureFilters
    {
        public static List<UMCLight> FilterFeatures(List<UMCLight> features, LcmsFeatureFilteringOptions options)
        {
            var minimumSize = options.FeatureLengthRange.Minimum;
            var maximumSize = options.FeatureLengthRange.Maximum;
            

            // Scan Length
            var newFeatures = features.FindAll(delegate(UMCLight x)
            {
                var size = Math.Abs(x.ScanStart - x.ScanEnd);
                return size >= minimumSize && size <= maximumSize;
            });

            return newFeatures.Where(x => x.Abundance > 0).ToList();            
        }
        /// <summary>
        /// Filters the list of MS Features based on user defined filtering criteria.
        /// </summary>
        /// <param name="features"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static List<MSFeatureLight> FilterMsFeatures(List<MSFeatureLight> features, MsFeatureFilteringOptions options)
        {
            var minimumMz = options.MzRange.Minimum;
            var maximumMz = options.MzRange.Maximum;

            var minimumCharge = options.ChargeRange.Minimum;
            var maximumCharge = options.ChargeRange.Maximum;

            var filteredMsFeatures = new List<MSFeatureLight>();
            filteredMsFeatures.AddRange(features);

            if (options.ShouldUseDeisotopingFilter)
            {
                filteredMsFeatures = 
                    filteredMsFeatures.FindAll(msFeature => msFeature.Score <= options.MinimumDeisotopingScore);                
            }

            if (options.ShouldUseIntensityFilter)
            {
                filteredMsFeatures =
                    filteredMsFeatures.FindAll(msFeature => msFeature.Abundance >= options.MinimumIntensity);
            }

            if (options.ShouldUseMzFilter)
            {
                filteredMsFeatures =
                    filteredMsFeatures.FindAll(msFeature => msFeature.Mz >= minimumMz && msFeature.Mz <= maximumMz);
            }

            if (options.ShouldUseChargeFilter)
            {
                filteredMsFeatures =
                   filteredMsFeatures.FindAll(msFeature => msFeature.ChargeState >= minimumCharge 
                                                                && msFeature.ChargeState <= maximumCharge);
            }
            
            return filteredMsFeatures;
        }
    }
}
