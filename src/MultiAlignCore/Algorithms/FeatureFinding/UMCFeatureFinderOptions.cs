using MultiAlignCore.Data;
using MultiAlignCore.IO.Parameters;
using MultiAlignEngine.Features;

namespace MultiAlignCore.Algorithms.FeatureFinding
{
    /// <summary>
    /// Encapsulates feature finding options.
    /// </summary>
    public class LCMSFeatureFindingOptions
    {
        public LCMSFeatureFindingOptions()
        {
            this.MonoMassWeight = 0.01F;
            this.AveMassWeight = 0.01F;
            this.LogAbundanceWeight = 0.10F;
            this.ScanWeight = 0.01F;
            this.FitWeight = 0.10F;
            this.NETWeight = 0.10F;

            this.ConstraintMonoMass = 6.0F; // in ppm 
            this.ConstraintAveMass = 6.0F; // in ppm; 

            this.MaxDistance = 0.1;
            this.UseNET = true;
            this.MinUMCLength = 3;
            this.IsotopicIntensityFilter = 0;

            this.IsIsotopicFitFilterInverted = false;
            this.UseIsotopicIntensityFilter = false;
            this.IsotopicFitFilter = .15;
            this.IsotopicIntensityFilter = 0;
            this.UMCAbundanceReportingType = AbundanceReportingType.Max;
            this.Split = false;
        }

        [DataSummaryAttribute("Splits features based on scan lengths.")]
        [ParameterFileAttribute("SplitFeatures", "LCMSFeatureFinding")]
        public bool Split { get; set; }
        [DataSummaryAttribute("Average Mass Weight")]
        [ParameterFileAttribute("AveMassWeight", "LCMSFeatureFinding")]
        public float AveMassWeight { get; set; }
        [DataSummaryAttribute("Constraint Average Mass (PPM)")]
        [ParameterFileAttribute("ConstraintAveMass", "LCMSFeatureFinding")]
        public float ConstraintAveMass { get; set; }
        [ParameterFileAttribute("ConstraintMonoMass", "LCMSFeatureFinding")]
        [DataSummaryAttribute("Constraint Mono Mass (PPM)")]
        public float ConstraintMonoMass { get; set; }
        [ParameterFileAttribute("FitWeight", "LCMSFeatureFinding")]
        [DataSummaryAttribute("Fit Weight")]
        public float FitWeight { get; set; }
        [DataSummaryAttribute("Is Isotopic Peak Fit Filter Inverted")]
        [ParameterFileAttribute("IsIsotopicPeakFitFilterInverted", "LCMSFeatureFinding")]
        public bool IsIsotopicFitFilterInverted { get; set; }
        [ParameterFileAttribute("IsotopicFitFilter", "LCMSFeatureFinding")]
        [DataSummaryAttribute("Isotopic Peak Intensity Filter")]
        public double IsotopicFitFilter { get; set; }
        [DataSummaryAttribute("Isotopic Intensity Filter")]
        [ParameterFileAttribute("IsotopicIntensityFilter", "LCMSFeatureFinding")]
        public int IsotopicIntensityFilter { get; set; }
        [DataSummaryAttribute("Log Abundance Weight")]
        [ParameterFileAttribute("LogAbundanceWeight", "LCMSFeatureFinding")]
        public float LogAbundanceWeight { get; set; }
        [DataSummaryAttribute("Max Distance")]
        [ParameterFileAttribute("MaxDistance", "LCMSFeatureFinding")]
        public double MaxDistance { get; set; }
        [DataSummaryAttribute("Min UMC Length")]
        [ParameterFileAttribute("MinUMCLength", "LCMSFeatureFinding")]
        public int MinUMCLength { get; set; }
        [DataSummaryAttribute("Mono Mass Weight")]
        [ParameterFileAttribute("MonoMassWeight", "LCMSFeatureFinding")]
        public float MonoMassWeight { get; set; }
        [ParameterFileAttribute("NetWeight", "LCMSFeatureFinding")]
        [DataSummaryAttribute("NET Weight")]
        public float NETWeight { get; set; }
        [ParameterFileAttribute("ScanWeight", "LCMSFeatureFinding")]
        [DataSummaryAttribute("Scan Weight")]
        public float ScanWeight { get; set; }
        [DataSummaryAttribute("UMC Abundance Reporting Type")]
        [ParameterFileAttribute("UMCAbundanceReportingType", "LCMSFeatureFinding")]
        public AbundanceReportingType UMCAbundanceReportingType { get; set; }
        [DataSummaryAttribute("Use Isotopic Peak Fit Filter")]
        [ParameterFileAttribute("UseIsotopicPeakFitFilter", "LCMSFeatureFinding")]
        public bool UseIsotopicFitFilter { get; set; }
        [DataSummaryAttribute("Use Isotopic Peak Intensity Filter")]
        [ParameterFileAttribute("UseIsotopicPeakIntensityFilter", "LCMSFeatureFinding")]
        public bool UseIsotopicIntensityFilter { get; set; }
        [DataSummaryAttribute("Use NET")]
        [ParameterFileAttribute("UseNET", "LCMSFeatureFinding")]
        public bool UseNET { get; set; }
    }


    public enum AbundanceReportingType
    {
        Sum,
        Max
    }
}
