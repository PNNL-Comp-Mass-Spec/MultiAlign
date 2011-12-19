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
        [ParameterFileAttribute("MinimumAbundance", "FeatureFilters")]
        [DataSummaryAttribute("Is Data Paired")]
        public double MinimumAbundance
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumIsotopicFit", "FeatureFilters")]
        [DataSummaryAttribute("Minimum Isotopic Fit")]
        public double IsotopicFit
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumScanLength", "FeatureFilters")]
        [DataSummaryAttribute("Minimum Scan Length")]
        public int MinimumScanLength
        {
            get;
            set;
        }
        [ParameterFileAttribute("MaximumChargeState", "FeatureFilters")]
        [DataSummaryAttribute("Maximum Charge State")]
        public int MaximumChargeState
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumChargeState", "FeatureFilters")]
        [DataSummaryAttribute("Minimum Charge State")]
        public int MinimumChargeState
        {
            get;
            set;
        }

        [ParameterFileAttribute("MaximumMonoisotopicMass", "FeatureFilters")]
        [DataSummaryAttribute("Maximum Monoisotopic Mass")]
        public double MaximumMonoIsotopicMass
        {
            get;
            set;
        }
        [ParameterFileAttribute("MinimumMonoisotopicMass", "FeatureFilters")]
        [DataSummaryAttribute("Minimum Monoisotopic mass")]
        public double MinimumMonoIsotopicMass
        {
            get;
            set;
        }
    }
}
