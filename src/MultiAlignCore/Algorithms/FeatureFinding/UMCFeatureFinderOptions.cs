using System;
using System.Collections.Generic;

using MultiAlignEngine;
using MultiAlignEngine.Features;
using System;

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

        [clsDataSummary("Splits features based on scan lengths.")]
        [clsParameterFile("SplitFeatures", "UMCFindingOptions")]
        public bool Split { get; set; }
        [clsDataSummary("Average Mass Weight")]
        [clsParameterFile("AveMassWeight", "UMCFindingOptions")]
        public float AveMassWeight { get; set; }
        [clsDataSummary("Constraint Average Mass (PPM)")]
        [clsParameterFile("ConstraintAveMass", "UMCFindingOptions")]
        public float ConstraintAveMass { get; set; }
        [clsParameterFile("ConstraintMonoMass", "UMCFindingOptions")]
        [clsDataSummary("Constraint Mono Mass (PPM)")]
        public float ConstraintMonoMass { get; set; }
        [clsParameterFile("FitWeight", "UMCFindingOptions")]
        [clsDataSummary("Fit Weight")]
        public float FitWeight { get; set; }
        [clsDataSummary("Is Isotopic Peak Fit Filter Inverted")]
        [clsParameterFile("IsIsotopicPeakFitFilterInverted", "UMCFindingOptions")]
        public bool IsIsotopicFitFilterInverted { get; set; }
        [clsParameterFile("IsotopicFitFilter", "UMCFindingOptions")]
        [clsDataSummary("Isotopic Peak Intensity Filter")]
        public double IsotopicFitFilter { get; set; }
        [clsDataSummary("Isotopic Intensity Filter")]
        [clsParameterFile("IsotopicIntensityFilter", "UMCFindingOptions")]
        public int IsotopicIntensityFilter { get; set; }
        [clsDataSummary("Log Abundance Weight")]
        [clsParameterFile("LogAbundanceWeight", "UMCFindingOptions")]
        public float LogAbundanceWeight { get; set; }
        [clsDataSummary("Max Distance")]
        [clsParameterFile("MaxDistance", "UMCFindingOptions")]
        public double MaxDistance { get; set; }
        [clsDataSummary("Min UMC Length")]
        [clsParameterFile("MinUMCLength", "UMCFindingOptions")]
        public int MinUMCLength { get; set; }
        [clsDataSummary("Mono Mass Weight")]
        [clsParameterFile("MonoMassWeight", "UMCFindingOptions")]
        public float MonoMassWeight { get; set; }
        [clsParameterFile("NetWeight", "UMCFindingOptions")]
        [clsDataSummary("NET Weight")]
        public float NETWeight { get; set; }
        [clsParameterFile("ScanWeight", "UMCFindingOptions")]
        [clsDataSummary("Scan Weight")]
        public float ScanWeight { get; set; }
        [clsDataSummary("UMC Abundance Reporting Type")]
        [clsParameterFile("UMCAbundanceReportingType", "UMCFindingOptions")]
        public enmAbundanceReportingType UMCAbundanceReportingType { get; set; }
        [clsDataSummary("Use Isotopic Peak Fit Filter")]
        [clsParameterFile("UseIsotopicPeakFitFilter", "UMCFindingOptions")]
        public bool UseIsotopicFitFilter { get; set; }
        [clsDataSummary("Use Isotopic Peak Intensity Filter")]
        [clsParameterFile("UseIsotopicPeakIntensityFilter", "UMCFindingOptions")]
        public bool UseIsotopicIntensityFilter { get; set; }
        [clsDataSummary("Use NET")]
        [clsParameterFile("UseNET", "UMCFindingOptions")]
        public bool UseNET { get; set; }
    }
}
