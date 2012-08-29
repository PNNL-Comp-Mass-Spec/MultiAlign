using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.FeatureFinding;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Data.Features
{
    public static class LCMSFeatureFilters
    {
        public static List<UMCLight> FilterFeatures(List<UMCLight> features, FeatureFilterOptions options)
        {
            // Scan Length
            features = features.FindAll(delegate(UMCLight x)
            {
                return Math.Abs(x.ScanStart - x.ScanEnd) >= options.MinimumScanLength;                
            });

            features = features.FindAll(delegate(UMCLight x)
            {
                return  x.Abundance >= options.MinimumAbundance;
            });


            features = features.FindAll(delegate(UMCLight x)
            {
                return (x.ChargeState >= options.MinimumChargeState && x.ChargeState <= options.MaximumChargeState);
            });

            features = features.FindAll(delegate(UMCLight x)
            {
                return x.AverageDeconFitScore <= options.IsotopicFit;
            });

            return features;
        }

        public static List<MSFeatureLight> FilterMSFeatures(List<MSFeatureLight> features, LCMSFeatureFindingOptions options)
        {            
            List<MSFeatureLight> filteredMSFeatures = new List<MSFeatureLight>();
            filteredMSFeatures.AddRange(features);

            if (options.UseIsotopicFitFilter == true)
            {
                if (!options.IsIsotopicFitFilterInverted)
                {
                    filteredMSFeatures = filteredMSFeatures.FindAll(delegate(MSFeatureLight msFeature)
                    {
                        return msFeature.Score <= options.IsotopicFitFilter;                        
                    });
                }
                else if (options.IsIsotopicFitFilterInverted)
                {
                    filteredMSFeatures = filteredMSFeatures.FindAll(delegate(MSFeatureLight msFeature)
                    {
                        return msFeature.Score >= options.IsotopicFitFilter;                        
                    });
                }
            }

            if (options.UseIsotopicIntensityFilter)
            {
                filteredMSFeatures = filteredMSFeatures.FindAll(delegate(MSFeatureLight msFeature)
                {
                    return msFeature.Abundance > options.IsotopicIntensityFilter;
                });       
            }
            
            return filteredMSFeatures;
        }
    }
}
