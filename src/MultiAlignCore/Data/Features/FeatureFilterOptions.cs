using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignEngine;
using System.Text;
using MultiAlignCore.IO.Parameters;

namespace MultiAlignCore.Data.Features
{
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
        [ParameterFileAttribute("MinimumAbundance", "LCMSFeatureFilters")]
        [DataSummaryAttribute("Is Data Paired")]
        public double MinimumAbundance
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumIsotopicFit", "LCMSFeatureFilters")]
        [DataSummaryAttribute("Minimum Isotopic Fit")]
        public double IsotopicFit
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumScanLength", "LCMSFeatureFilters")]
        [DataSummaryAttribute("Minimum Scan Length")]
        public int MinimumScanLength
        {
            get;
            set;
        }
        [ParameterFileAttribute("MaximumChargeState", "LCMSFeatureFilters")]
        [DataSummaryAttribute("Maximum Charge State")]
        public int MaximumChargeState
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumChargeState", "LCMSFeatureFilters")]
        [DataSummaryAttribute("Minimum Charge State")]
        public int MinimumChargeState
        {
            get;
            set;
        }

        [ParameterFileAttribute("MaximumMonoisotopicMass", "LCMSFeatureFilters")]
        [DataSummaryAttribute("Maximum Monoisotopic Mass")]
        public double MaximumMonoIsotopicMass
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumMonoisotopicMass", "LCMSFeatureFilters")]
        [DataSummaryAttribute("Minimum Monoisotopic mass")]
        public double MinimumMonoIsotopicMass
        {
            get;
            set;
        }
    }
}
