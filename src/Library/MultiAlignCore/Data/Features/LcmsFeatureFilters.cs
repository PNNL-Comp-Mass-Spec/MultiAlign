using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAlignCore.Data.Features
{
    public static class LcmsFeatureFilters
    {
        public static List<T> FilterFeatures<T>(List<T> features, LcmsFeatureFilteringOptions options, Dictionary<int, double> scanTimes = null)
            where T: UMCLight
        {
            IEnumerable<T> newFeatures;
            if (scanTimes == null || !options.FilterOnMinutes)
            {
                var minimumSize = options.FeatureLengthRangeScans.Minimum;
                var maximumSize = options.FeatureLengthRangeScans.Maximum;


                // Scan Length
                newFeatures = features.Where(x =>
                {
                    var size = Math.Abs(x.ScanStart - x.ScanEnd);
                    return size >= minimumSize && size <= maximumSize;
                });
            }
            else
            {
                var minimumSize = options.FeatureLengthRangeMinutes.Minimum;
                var maximumSize = options.FeatureLengthRangeMinutes.Maximum;
                var minimumPoints = options.MinimumDataPoints;

                // Scan Length
                newFeatures = features.Where(x =>
                {
                    try
                    {
                        double size = 0;
                        if (x.ScanStart != 0)
                        {
                            size = Math.Abs(scanTimes[x.ScanEnd] - scanTimes[x.ScanStart]);
                        }
                        else //Scan 0 won't show up in scanTimes dictionary, so the feature length is just the time of the last feature scan.
                        {
                            size = scanTimes[x.ScanEnd];
                        }
                        return size >= minimumSize && size <= maximumSize && x.Features.Count >= minimumPoints;
                    }
                    catch
                    {
                        throw (new IndexOutOfRangeException(String.Format("Scan {0} or {1} not found in scan to time map.", x.ScanStart, x.ScanEnd)));
                    }
                });
            }

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
