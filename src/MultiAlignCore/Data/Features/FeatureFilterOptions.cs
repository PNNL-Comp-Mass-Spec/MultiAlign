using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignEngine;
using System.Text;

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
        [clsParameterFile("MinimumAbundance", "FeatureFilters")]
        [clsDataSummaryAttribute("Is Data Paired")]
        public double MinimumAbundance
        {
            get;
            set;
        }
        [clsParameterFile("MinimumIsotopicFit", "FeatureFilters")]
        [clsDataSummaryAttribute("Minimum Isotopic Fit")]
        public double IsotopicFit
        {
            get;
            set;
        }
        [clsParameterFile("MinimumScanLength", "FeatureFilters")]
        [clsDataSummaryAttribute("Minimum Scan Length")]
        public int MinimumScanLength
        {
            get;
            set;
        }
        [clsParameterFile("MaximumChargeState", "FeatureFilters")]
        [clsDataSummaryAttribute("Maximum Charge State")]
        public int MaximumChargeState
        {
            get;
            set;
        }
        [clsParameterFile("MinimumChargeState", "FeatureFilters")]
        [clsDataSummaryAttribute("Minimum Charge State")]
        public int MinimumChargeState
        {
            get;
            set;
        }

        [clsParameterFile("MaximumMonoisotopicMass", "FeatureFilters")]
        [clsDataSummaryAttribute("Maximum Monoisotopic Mass")]
        public double MaximumMonoIsotopicMass
        {
            get;
            set;
        }
        [clsParameterFile("MinimumMonoisotopicMass", "FeatureFilters")]
        [clsDataSummaryAttribute("Minimum Monoisotopic mass")]
        public double MinimumMonoIsotopicMass
        {
            get;
            set;
        }
    }
}
