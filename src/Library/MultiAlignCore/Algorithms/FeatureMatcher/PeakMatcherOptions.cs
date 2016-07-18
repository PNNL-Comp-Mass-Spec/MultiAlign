namespace MultiAlignCore.Algorithms.FeatureMatcher
{

    /// <summary>
    /// Options class for matching AMT's to features
    /// </summary>
    public class PeakMatcherOptions
    {
        /// <summary>
        /// constructor.
        /// </summary>
        public PeakMatcherOptions()
        {
            Tolerances  = new FeatureTolerances();
            DaltonShift = 0;
        }

        /// <summary>
        /// Gets or sets the feature tolerances for matching AMT's to features.
        /// </summary>
        public FeatureTolerances Tolerances
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount to shift (in daltons) when doing the peak matching.
        /// </summary>
        public double DaltonShift
        {
            get;
            set;
        }
    }
}
