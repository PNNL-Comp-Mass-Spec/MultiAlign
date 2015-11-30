
using System;

namespace MultiAlignCore.Data.Features
{
    /// <summary>
    /// LC-MS Feature Filtering Options.  These options filter LC-MS features
    /// </summary>
    public class LcmsFeatureFilteringOptions
    {
        public const double MIN_FEATURE_LENGTH = 50;
        public const double MAX_FEATURE_LENGTH = 300;

        public LcmsFeatureFilteringOptions()
        {
            FilterOnMinutes = false;
            FeatureLengthRangeScans = new FilterRange(MIN_FEATURE_LENGTH, MAX_FEATURE_LENGTH);
            FeatureLengthRangeMinutes = new FilterRange(double.MinValue, double.MaxValue);
            MinimumDataPoints = 3;
        }

        /// <summary>
        /// If true, then feature lengths are defined based on times rather than scans.
        /// </summary>
        public bool FilterOnMinutes { get; set; }

        /// <summary>
        /// Gets or sets the range (in minutes) to use for feature lengths
        /// </summary>
        public FilterRange FeatureLengthRangeScans { get; set; }

        /// <summary>
        /// Gets or sets the range (in scans) to use for feature lengths
        /// </summary>
        public FilterRange FeatureLengthRangeMinutes { get; set; }

        /// <summary>
        /// Minimum number of data points for a good feature; used in conjunction with FeatureLengthRangeMinutes
        /// </summary>
        public double MinimumDataPoints { get; set; }

        /// <summary>
        /// For NHibernate; set when the class is persisted or read
        /// </summary>
        public int Id { get; set; }
    }
}