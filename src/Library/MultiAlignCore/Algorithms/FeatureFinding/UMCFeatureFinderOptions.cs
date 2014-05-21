﻿using MultiAlignCore.Data;
using MultiAlignCore.IO.Parameters;
using MultiAlignEngine.Features;
using System.ComponentModel;

namespace MultiAlignCore.Algorithms.FeatureFinding
{
    /// <summary>
    /// Encapsulates feature finding options.
    /// </summary>
    /// 
    public class LCMSFeatureFindingOptions
    {
        public LCMSFeatureFindingOptions()
        {
            MonoMassWeight = 0.01F;
            AveMassWeight = 0.01F;
            LogAbundanceWeight = 0.10F;
            ScanWeight = 0.01F;
            FitWeight = 0.10F;
            NETWeight = 0.10F;

            ConstraintMonoMass      = 6.0F; // in ppm 
            ConstraintAveMass       = 6.0F; // in ppm; 

            MaxDistance             = 0.1;
            UseNET                  = true;
            MinUMCLength            = 3;
            IsotopicIntensityFilter = 0;

            StoreMSFeatureResults          = true;
            IsIsotopicFitFilterInverted    = false;
            UseIsotopicIntensityFilter     = false;
            IsotopicFitFilter              = .15;
            IsotopicIntensityFilter        = 0;
            UMCAbundanceReportingType      = AbundanceReportingType.Sum;
            Split                          = false;
            FeatureFinderAlgorithm         = FeatureFinderType.TreeBased;
            ShouldCreateXicFile            = false;
        }

        [DataSummaryAttribute("Algorithm for feature finding")]
        [ParameterFileAttribute("FeatureFinderAlgorithm", "LCMSFeatureFinding")]
        [Category("Algorithms")]
        [Description("Determines the algorithm to use for feature finding.")]
        public FeatureFinderType FeatureFinderAlgorithm { get; set; }
        [DataSummaryAttribute("Smooth Xic Data")]
        [ParameterFileAttribute("ShouldSmoothXicData", "LCMSFeatureFinding")]
        [Category("Persistence")]
        [Description("Smooths intensity profile when creating Xic's")]
        public bool ShouldSmoothXic { get; set; }
        [DataSummaryAttribute("Create XIC Key File")]
        [ParameterFileAttribute("ShouldCreateXicKeyFile", "LCMSFeatureFinding")]
        [Category("Persistence")]
        [Description("Creates an XIC key file.")]
        public bool ShouldCreateXicFile { get; set; }
        [DataSummaryAttribute("Store MS Feature Results")]
        [ParameterFileAttribute("StoreMSFeatureResults", "LCMSFeatureFinding")]
        [Category("Persistence")]
        [Description("Flag indicating whether MS Feature results should be saved to the database.")]
        public bool StoreMSFeatureResults { get; set; }
        [DataSummaryAttribute("Is Isotopic Peak Fit Filter Inverted")]
        [ParameterFileAttribute("IsIsotopicPeakFitFilterInverted", "LCMSFeatureFinding")]
        [Category("Filtering")]
        [Description("Flag indicating if the isotopic fit score is inverted.  False would mean that a score = 0 represents a good fit, score = 1 representing a poor fit.  True would indicate the inverse.")]
        public bool IsIsotopicFitFilterInverted { get; set; }
        [ParameterFileAttribute("IsotopicFitFilter", "LCMSFeatureFinding")]        
        [Category("Filtering")]
        [Description("Fit score filter.")]
        public double IsotopicFitFilter { get; set; }
        [ParameterFileAttribute("IsotopicIntensityFilter", "LCMSFeatureFinding")]
        [Category("Filtering")]
        [Description("Average Mass Constraint (ppm) to look between MS Features for similarity.")]
        public int IsotopicIntensityFilter { get; set; }
        [ParameterFileAttribute("MaxDistance", "LCMSFeatureFinding")]
        [Category("Filtering")]
        [Description("Maximum distance two MS Features could be considered.  Distance is considered based on mass, NET, fit score, intensity.")]
        public double MaxDistance { get; set; }
        [ParameterFileAttribute("MinUMCLength", "LCMSFeatureFinding")]
        [Category("Filtering")]
        [Description("The minimum number of scans a LC-MS feature can be observed across.")]
        public int MinUMCLength { get; set; }
        [ParameterFileAttribute("UseIsotopicPeakFitFilter", "LCMSFeatureFinding")]
        [Category("Filtering")]
        [Description("Determines if MS Features should be filtered based on their isotopic fit values.")]
        public bool UseIsotopicFitFilter { get; set; }
        [ParameterFileAttribute("UseIsotopicPeakIntensityFilter", "LCMSFeatureFinding")]
        [Category("Filtering")]
        [Description("Determines if MS features should be screened by their intensity values.")]
        public bool UseIsotopicIntensityFilter { get; set; }
        [ParameterFileAttribute("UMCAbundanceReportingType", "LCMSFeatureFinding")]
        [Category("Intensity/Abundance")]
        [Description("Determines how to report the abundance of a LC-MS Feature.")]
        public AbundanceReportingType UMCAbundanceReportingType { get; set; }
        [ParameterFileAttribute("SplitFeatures", "LCMSFeatureFinding")]
        [Category("Splitting")]
        [Description("Determines if features should be split after feature finding.  True is recommended.")]
        public bool Split { get; set; }
        [ParameterFileAttribute("AveMassWeight", "LCMSFeatureFinding")]
        [Category("Weights")]
        [Description("Average mass weight.")]
        public float AveMassWeight { get; set; }
        [ParameterFileAttribute("ConstraintAveMass", "LCMSFeatureFinding")]
        [Category("Weights")]
        [Description("Average Mass Constraint (ppm) to look between MS Features for similarity.")]
        public float ConstraintAveMass { get; set; }
        [ParameterFileAttribute("ConstraintMonoMass", "LCMSFeatureFinding")]
        [Category("Weights")]
        [Description("Monoisotopic Mass Constraint (ppm) to look between MS Features for similarity.")]     
        public float ConstraintMonoMass { get; set; }
        [ParameterFileAttribute("FitWeight", "LCMSFeatureFinding")]
        [Category("Weights")]
        [Description("Deisotoping Fit Score Weight.")]
        public float FitWeight { get; set; }        
        [ParameterFileAttribute("LogAbundanceWeight", "LCMSFeatureFinding")]
        [Category("Weights")]
        [Description("Weight for the abundance (log base 2 transformed).")]
        public float LogAbundanceWeight { get; set; }
        [ParameterFileAttribute("MonoMassWeight", "LCMSFeatureFinding")]
        [Category("Weights")]
        [Description("Monoisotopic mass weight.")]
        public float MonoMassWeight { get; set; }
        [ParameterFileAttribute("NetWeight", "LCMSFeatureFinding")]
        [Category("Weights")]
        [Description("Normalized Elution Time (NET) weight.")]
        public float NETWeight { get; set; }
        [ParameterFileAttribute("ScanWeight", "LCMSFeatureFinding")]
        [Category("Weights")]
        [Description("Scan weight.")]
        public float ScanWeight { get; set; }
        [DataSummaryAttribute("Use NET")]
        [Category("Weights")]
        [Description("Determines whether to use the NET weight or Scan Weight.  NET Weight is recommended.")]
        public bool UseNET { get; set; }
    }

}