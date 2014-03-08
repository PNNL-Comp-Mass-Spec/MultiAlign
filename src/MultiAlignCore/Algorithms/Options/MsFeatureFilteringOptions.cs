namespace MultiAlignCore.Algorithms.Options
{
    /// <summary>
    /// MS Feature Filtering Options.  These options are for filtering input Ms features deisotoped by another tool.
    /// </summary>
    public class MsFeatureFilteringOptions
    {
        public const double MINIMUM_DEISOTOPING_SCORE = .15;

        public MsFeatureFilteringOptions()
        {
            MzRange                 = new FilterRange();
            MinimumIntensity        = 0;
            MinimumDeisotopingScore = MINIMUM_DEISOTOPING_SCORE;

            ShouldUseIntensityFilter    = false;
            ShouldUseMzFilter           = false;
            ShouldUseDeisotopingFilter  = true;
        }
        public FilterRange MzRange              { get; set; }
        public double MinimumIntensity          { get; set; }
        public double MinimumDeisotopingScore   { get; set; }

        public bool ShouldUseMzFilter           { get; set; }
        public bool ShouldUseIntensityFilter    { get; set; }  
        public bool ShouldUseDeisotopingFilter    { get; set; }        
    }
}