using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAlignCore.Data.Features
{
    public static class LcmsFeatureFilters
    {
        public static List<T> FilterFeatures<T>(List<T> features, LcmsFeatureFilteringOptions options)
            where T: UMCLight
        {
            var minimumSize = options.FeatureLengthRange.Minimum;
            var maximumSize = options.FeatureLengthRange.Maximum;


            // Scan Length
            var newFeatures = features.Where(delegate(T x)
            {
                var size = Math.Abs(x.ScanStart - x.ScanEnd);
                return size >= minimumSize && size <= maximumSize;
            });

            return newFeatures.Where(x => x.Abundance > 0).ToList();
        }

        public static List<T> FilterFeatures<T>(List<T> features, LcmsFeatureFilteringOptions options, Dictionary<int, double> scanTimes)
    where T : UMCLight
        {
            var minimumSize = options.FeatureLengthRange.Minimum;
            var maximumSize = options.FeatureLengthRange.Maximum;


            // Scan Length
            var newFeatures = features.Where(delegate(T x)
            {
                try
                {
                    if (x.ScanStart != 0)
                    {
                        var size = Math.Abs(scanTimes[x.ScanStart] - scanTimes[x.ScanEnd]);
                        return size >= minimumSize && size <= maximumSize;
                    }
                    else //Scan 0 won't show up in scanTimes dictionary, so the feature length is just the time of the last feature scan.
                    {
                        var size = scanTimes[x.ScanEnd];
                        return size >= minimumSize && size <= maximumSize;
                    }
                }
                catch
                {
                    throw (new IndexOutOfRangeException(String.Format("Scan {0} or {1} not found in scan to time map.", x.ScanStart, x.ScanEnd)));
                }
            });

            return newFeatures.Where(x => x.Abundance > 0).ToList();
        }

        /// <summary>
        /// Filters the list of MS Features based on user defined filtering criteria.
        /// </summary>
        /// <param name="features"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static List<MSFeatureLight> FilterMsFeatures(IEnumerable<MSFeatureLight> features, MsFeatureFilteringOptions options)
        {
            var minimumMz = options.MzRange.Minimum;
            var maximumMz = options.MzRange.Maximum;

            var minimumCharge = options.ChargeRange.Minimum;
            var maximumCharge = options.ChargeRange.Maximum;

            var filteredMsFeatures = features;

            if (options.ShouldUseDeisotopingFilter)
            {
                filteredMsFeatures =
                    filteredMsFeatures.Where(msFeature => msFeature.Score <= options.MinimumDeisotopingScore);
            }

            if (options.ShouldUseIntensityFilter)
            {
                filteredMsFeatures =
                    filteredMsFeatures.Where(msFeature => msFeature.Abundance >= options.MinimumIntensity);
            }

            if (options.ShouldUseMzFilter)
            {
                filteredMsFeatures =
                    filteredMsFeatures.Where(msFeature => msFeature.Mz >= minimumMz && msFeature.Mz <= maximumMz);
            }

            if (options.ShouldUseChargeFilter)
            {
                filteredMsFeatures =
                   filteredMsFeatures.Where(msFeature => msFeature.ChargeState >= minimumCharge
                                                                && msFeature.ChargeState <= maximumCharge);
            }

            return filteredMsFeatures.ToList();
        }
    }
}
