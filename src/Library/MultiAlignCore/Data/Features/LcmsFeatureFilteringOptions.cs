
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
            FeatureLengthRange = new FilterRange(MIN_FEATURE_LENGTH, MAX_FEATURE_LENGTH);
            TreatAsTimeNotScan = false;
        }
        /// <summary>
        /// Gets or sets the range to use for feature lengths
        /// </summary>
        /// <remarks>Scan numbers if TreatAsTimeNotScan=False, or time (in minutes) if TreatAsTimeNotScan=True</remarks>
        public FilterRange FeatureLengthRange { get; set; }

        /// <summary>
        /// If true, then feature lengths are defined based on times rather than scans.
        /// </summary>
        public bool TreatAsTimeNotScan { get; set; }
    }
}