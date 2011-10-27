using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignEngine.Features;

namespace MultiAlignCore.Data.Features
{
    public static class LCMSFeatureFilters
    {        
        public static List<clsUMC> FilterFeatures(List<clsUMC> features, FeatureFilterOptions options)
        {
            // Scan Length
            features = features.FindAll(delegate(clsUMC x)
            {
                return Math.Abs(x.ScanStart - x.ScanEnd) >= options.MinimumScanLength;                
            });


            features = features.FindAll(delegate(clsUMC x)
            {
                return  x.AbundanceMax >= options.MinimumAbundance;
            });


            features = features.FindAll(delegate(clsUMC x)
            {
                return  (x.ChargeRepresentative >= options.MinimumChargeState && x.ChargeRepresentative <= options.MaximumChargeState);
            });


            
            features = features.FindAll(delegate(clsUMC x)
            {
                return x.AverageDeconFitScore <= options.IsotopicFit;
            });

            return features;
        }
    }
}
