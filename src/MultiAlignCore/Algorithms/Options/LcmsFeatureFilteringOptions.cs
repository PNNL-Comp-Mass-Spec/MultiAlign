namespace MultiAlignCore.Algorithms.Options
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
        }
        /// <summary>
        /// Gets or sets the range to use for feature lengths (size in scans minimum number to maximum number).
        /// </summary>
        public FilterRange FeatureLengthRange { get; set; }
    }
}