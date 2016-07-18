using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAlignCore.Data.Features
{
    public static class LcmsFeatureFilters
    {
        public static List<T> FilterFeatures<T>(List<T> features, LcmsFeatureFilteringOptions options, IScanSummaryProvider scanSummaryProvider = null)
            where T: UMCLight
        {
            IEnumerable<T> newFeatures;
            if (scanSummaryProvider == null || !options.FilterOnMinutes)
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

                //var knownScanNumbers = scanTimes.Keys.ToList();
                //knownScanNumbers.Sort();

                // Scan Length
                newFeatures = features.Where(x =>
                {
                    try
                    {
                        double size = 0;
                        if (x.ScanStart == 0)
                        {
                            //Scan 0 won't show up in scanTimes dictionary, so the feature length is just the time of the last feature scan.
                            size = scanSummaryProvider.GetScanSummary(x.ScanEnd).Time;
                        }
                        else
                        {
                            size = Math.Abs(scanSummaryProvider.GetScanSummary(x.ScanEnd).Time - scanSummaryProvider.GetScanSummary(x.ScanStart).Time);
                        }
                        return size >= minimumSize && size <= maximumSize && x.Features.Count >= minimumPoints;
                    }
                    catch (Exception ex)
                    {
                        throw (new IndexOutOfRangeException(String.Format("Exception determining the elution time for scans {0} and {1}: {2}", x.ScanStart, x.ScanEnd, ex.Message)));
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

        [Obsolete("I put this functionality into ScanSummaryProvider")]
        private static double GetScanTime(IReadOnlyDictionary<int, double> scanTimes, List<int> knownScanNumbers, int scanNumber)
        {
            double scanTime;
            if (scanTimes.TryGetValue(scanNumber, out scanTime))
                return scanTime;

            // Exact match not found; find the elution time of the nearest scan
            // ToDo: Interpolate between the two nearest scans

            var indexNearest = ~(knownScanNumbers.BinarySearch(scanNumber));

            if (indexNearest <= 0)
                return scanTimes[knownScanNumbers[0]];

            if (indexNearest >= knownScanNumbers.Count)
                return scanTimes[knownScanNumbers[knownScanNumbers.Count - 1]];

            return scanTimes[knownScanNumbers[indexNearest]];
        }
    }
}
