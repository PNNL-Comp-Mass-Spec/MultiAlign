using System.ComponentModel;
using MultiAlignCore.IO.Parameters;

namespace MultiAlignCore.Data.Features
{
    /// <summary>
    /// Class that holds filtering options for LC-MS Features
    /// </summary>
    public class FeatureFilterOptions
    {
        public FeatureFilterOptions()
        {
            MinimumAbundance    = 0;
            IsotopicFit         = .15;
            MinimumScanLength   = 2;
            MinimumChargeState  = 1;
            MaximumChargeState  = 30;
            MinimumMonoIsotopicMass = 0;
            MaximumMonoIsotopicMass = 100000;
        }
        [ParameterFileAttribute("MaximumChargeState", "LCMSFeatureFilters")]
        [Category("Charge States")]
        [Description("Maximum Charge State a feature can have.")]
        public int MaximumChargeState
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumChargeState", "LCMSFeatureFilters")]
        [Category("Charge States")]
        [Description("Minimum Charge State a feature can have.")]
        public int MinimumChargeState
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumScanLength", "LCMSFeatureFilters")]
        [Category("Elution Time")]
        [Description("Minimum Scan Length a feature can span.")]
        public int MinimumScanLength
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumAbundance", "LCMSFeatureFilters")]
        [Category("Intensity/Abundance")]
        [Description("Minimum abundance allowed.")]
        public double MinimumAbundance
        {
            get;
            set;
        }

        [ParameterFileAttribute("MaximumMonoisotopicMass", "LCMSFeatureFilters")]
        [Category("Mass")]
        [Description("Maximum Monoisotopic Mass a feature can be.")]
        public double MaximumMonoIsotopicMass
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumMonoisotopicMass", "LCMSFeatureFilters")]
        [Category("Mass")]
        [Description("Minimum Monoisotopic mass a feature can be.")]
        public double MinimumMonoIsotopicMass
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumIsotopicFit", "LCMSFeatureFilters")]
        [Category("Scores")]
        [Description("Minimum isotopic fit score")]
        public double IsotopicFit
        {
            get;
            set;
        }
    }
}
