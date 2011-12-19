using MultiAlignCore.Data;
using MultiAlignCore.IO.Parameters;
using MultiAlignEngine.Features;

namespace MultiAlignCore.Algorithms.FeatureFinding
{
    /// <summary>
    /// Encapsulates feature finding options.
    /// </summary>
    public class UMCFeatureFinderOptions
    {
        public UMCFeatureFinderOptions()
        {
                this.MonoMassWeight		            = 0.01F; 
				this.AveMassWeight  	            = 0.01F; 
				this.LogAbundanceWeight             = 0.10F; 
				this.ScanWeight 		            = 0.01F; 
				this.FitWeight                      = 0.10F; 
				this.NETWeight			            = 0.10F; 

				this.ConstraintMonoMass	            = 6.0F; // in ppm 
				this.ConstraintAveMass	            = 6.0F; // in ppm; 

				this.MaxDistance    	            = 0.1; 
				this.UseNET				            = true; 
				this.MinUMCLength		            = 3; 
                this.IsotopicIntensityFilter        = 0;
				
				this.IsIsotopicFitFilterInverted    = false;
				this.UseIsotopicIntensityFilter     = false;				
				this.IsotopicFitFilter              = .15;
				this.IsotopicIntensityFilter        = 0;
				this.UMCAbundanceReportingType      = enmAbundanceReportingType.PeakMax;
                this.Split                          = false;
        }

        [DataSummaryAttribute("Splits features based on scan lengths.")]
        [ParameterFileAttribute("SplitFeatures", "UMCFindingOptions")]
        public bool Split { get; set; }
        [DataSummaryAttribute("Average Mass Weight")]
        [ParameterFileAttribute("AveMassWeight", "UMCFindingOptions")]
        public float AveMassWeight { get; set; }
        [DataSummaryAttribute("Constraint Average Mass (PPM)")]
        [ParameterFileAttribute("ConstraintAveMass", "UMCFindingOptions")]
        public float ConstraintAveMass { get; set; }
        [ParameterFileAttribute("ConstraintMonoMass", "UMCFindingOptions")]
        [DataSummaryAttribute("Constraint Mono Mass (PPM)")]
        public float ConstraintMonoMass { get; set; }
        [ParameterFileAttribute("FitWeight", "UMCFindingOptions")]
        [DataSummaryAttribute("Fit Weight")]
        public float FitWeight { get; set; }
        [DataSummaryAttribute("Is Isotopic Peak Fit Filter Inverted")]
        [ParameterFileAttribute("IsIsotopicPeakFitFilterInverted", "UMCFindingOptions")]
        public bool IsIsotopicFitFilterInverted { get; set; }
        [ParameterFileAttribute("IsotopicFitFilter", "UMCFindingOptions")]
        [DataSummaryAttribute("Isotopic Peak Intensity Filter")]
        public double IsotopicFitFilter { get; set; }
        [DataSummaryAttribute("Isotopic Intensity Filter")]
        [ParameterFileAttribute("IsotopicIntensityFilter", "UMCFindingOptions")]
        public int IsotopicIntensityFilter { get; set; }
        [DataSummaryAttribute("Log Abundance Weight")]
        [ParameterFileAttribute("LogAbundanceWeight", "UMCFindingOptions")]
        public float LogAbundanceWeight { get; set; }
        [DataSummaryAttribute("Max Distance")]
        [ParameterFileAttribute("MaxDistance", "UMCFindingOptions")]
        public double MaxDistance { get; set; }
        [DataSummaryAttribute("Min UMC Length")]
        [ParameterFileAttribute("MinUMCLength", "UMCFindingOptions")]
        public int MinUMCLength { get; set; }
        [DataSummaryAttribute("Mono Mass Weight")]
        [ParameterFileAttribute("MonoMassWeight", "UMCFindingOptions")]
        public float MonoMassWeight { get; set; }
        [ParameterFileAttribute("NetWeight", "UMCFindingOptions")]
        [DataSummaryAttribute("NET Weight")]
        public float NETWeight { get; set; }
        [ParameterFileAttribute("ScanWeight", "UMCFindingOptions")]
        [DataSummaryAttribute("Scan Weight")]
        public float ScanWeight { get; set; }
        [DataSummaryAttribute("UMC Abundance Reporting Type")]
        [ParameterFileAttribute("UMCAbundanceReportingType", "UMCFindingOptions")]
        public enmAbundanceReportingType UMCAbundanceReportingType { get; set; }
        [DataSummaryAttribute("Use Isotopic Peak Fit Filter")]
        [ParameterFileAttribute("UseIsotopicPeakFitFilter", "UMCFindingOptions")]
        public bool UseIsotopicFitFilter { get; set; }
        [DataSummaryAttribute("Use Isotopic Peak Intensity Filter")]
        [ParameterFileAttribute("UseIsotopicPeakIntensityFilter", "UMCFindingOptions")]
        public bool UseIsotopicIntensityFilter { get; set; }
        [DataSummaryAttribute("Use NET")]
        [ParameterFileAttribute("UseNET", "UMCFindingOptions")]
        public bool UseNET { get; set; }
    }
}
