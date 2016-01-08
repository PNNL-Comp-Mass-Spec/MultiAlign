
namespace MultiAlignCore.Data.Features
{
    /// <summary>
    /// MS Feature Filtering Options.  These options are for filtering input Ms features deisotoped by another tool.
    /// </summary>
    public class MsFeatureFilteringOptions
    {
        /// <summary>
        /// Minimum deisotoping score from the DeconTools paper
        /// </summary>
        private const double MINIMUM_DEISOTOPING_SCORE = .15;

        public MsFeatureFilteringOptions()
        {
            MzRange                 = new FilterRange();
            MinimumIntensity        = 0;
            MinimumDeisotopingScore = MINIMUM_DEISOTOPING_SCORE;
            ChargeRange             = new FilterRange(1, 6);

            ShouldUseIntensityFilter    = false;
            ShouldUseMzFilter           = false;
            ShouldUseDeisotopingFilter  = true;
            ShouldUseChargeFilter       = true;
        }
        /// <summary>
        /// Gets or sets the range for a charge states
        /// </summary>
        public FilterRange ChargeRange { get; set; }
        public FilterRange MzRange              { get; set; }
        public double MinimumIntensity          { get; set; }
        public double MinimumDeisotopingScore   { get; set; }
        public bool ShouldUseMzFilter           { get; set; }
        public bool ShouldUseIntensityFilter    { get; set; }
        public bool ShouldUseChargeFilter        { get; set; }
        public bool ShouldUseDeisotopingFilter { get; set; }
    }
}