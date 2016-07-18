using MultiAlignCore.IO.TextFiles;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Data loading options.  These options filter DeconTools (or similar) data when it is first loaded into MultiAlign
    /// </summary>
    public class DataLoadingOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DataLoadingOptions()
        {
            UseIsotopicFitFilter = true;
            MaximumIsotopicFit = 0.15;

            UseAbundanceFilter = false;
            MinimumAbundance = 0;
            MaximumAbundance = 1e15;

            UseLCScanFilter = false;
            MinimumLCScan = 0;
            MaximumLCScan = 999999;

            UseMaximumDataCountFilter = false;
            MaximumPointsToLoad = 800000;
        }

        public bool UseIsotopicFitFilter { get; set; }
        public double MaximumIsotopicFit { get; set; }

        public bool UseAbundanceFilter { get; set; }
        public double MinimumAbundance { get; set; }
        public double MaximumAbundance { get; set; }

        public bool UseLCScanFilter { get; set; }
        public int MinimumLCScan { get; set; }
        public int MaximumLCScan { get; set; }

        public bool UseMaximumDataCountFilter { get; set; }
        public int MaximumPointsToLoad { get; set; }

        public DeconToolsIsosFilterOptions GetIsosFilterOptions()
        {
            var isosFilterOptions = new DeconToolsIsosFilterOptions();

            if (UseIsotopicFitFilter)
            {
                isosFilterOptions.MaximumIsotopicFit = MaximumIsotopicFit;
            }

            if (UseAbundanceFilter)
            {
                isosFilterOptions.AbundanceMinimum = MinimumAbundance;
                isosFilterOptions.AbundanceMaximum = MaximumAbundance;
            }

            if (UseLCScanFilter)
            {
                isosFilterOptions.LCScanStart = MinimumLCScan;
                isosFilterOptions.LCScanEnd = MaximumLCScan;
            }

            if (UseMaximumDataCountFilter)
            {
                isosFilterOptions.MaximumDataPoints = MaximumPointsToLoad;
            }

            return isosFilterOptions;
        }
    }
}
