using System.Collections.Generic;
using MultiAlignCore.IO.Parameters;
using MultiAlignEngine;
using MultiAlignEngine.Alignment;
using MultiAlignCore.Data;


namespace MultiAlignCore.Algorithms.Alignment
{
    public class AlignmentOptions
    {
        public AlignmentOptions()
        {
            NumTimeSections				= 100; 
			ContractionFactor			= 3; 
			MaxTimeJump			        = 10; 
			MaxPromiscuity			    = 3; 
			UsePromiscuousPoints		= false; 
			MassCalibrationUseLSQ   	= false; 
			MassCalibrationWindow		= 6.0; 
			MassCalibrationNumXSlices   = 12; 
			MassCalibrationNumMassDeltaBins    = 50; 
			MassCalibrationMaxJump  	= 20; 
			MassCalibrationMaxZScore	= 3; 
			MassCalibrationLSQZScore	= 2.5; 
			MassCalibrationLSQNumKnots  = 12; 
			MassTolerance				= 6.0; 
			NETTolerance				= 0.03; 
			AlignmentType				= enmAlignmentType.NET_MASS_WARP; 
			RecalibrationType       	= enmCalibrationType.HYBRID_CALIB;
			AlignToMassTagDatabase		= false; 
			AlignmentBaselineName		= ""; 
			MassBinSize				    = .2;
			NETBinSize				    = .001;
			DriftTimeBinSize		    = .03;

            SplitAlignmentInMZ          = false;
			MZBoundaries				= new List<classAlignmentMZBoundary>();

			/// Construct the m/z boundary object.			
			MZBoundaries.Add(new classAlignmentMZBoundary(0.0, 505.7));
			MZBoundaries.Add(new classAlignmentMZBoundary(505.7, 999999999.0));
        }

        public bool AlignToMassTagDatabase
        {
            get;
            set;
        }

        [DataSummaryAttribute("Alignment Baseline Name")]
        public string AlignmentBaselineName { get; set; }
        [ParameterFileAttribute("AlignmentType", "AlignmentOptions")]
        [DataSummaryAttribute("Alignment Type")]
        public enmAlignmentType AlignmentType { get; set; }
        [DataSummaryAttribute("Apply Mass Recalibration")]
        //[ParameterFileAttribute("ApplyMassRecalibration", "AlignmentOptions")]
        //public bool ApplyMassRecalibration { get; set; }
        [DataSummaryAttribute("Contraction Factor")]
        [ParameterFileAttribute("ContractionFactor", "AlignmentOptions")]
        public short ContractionFactor { get; set; }
        [ParameterFileAttribute("HistogramDriftTimeBinSize", "AlignmentOptions")]
        [DataSummaryAttribute("Histogram Drift Time Bin Size")]
        public double DriftTimeBinSize { get; set; }
        [DataSummaryAttribute("Is Aligned to Mass Tag Database")]
        public bool IsAlignmentBaselineAMasstagDB { get; set; }
        [ParameterFileAttribute("HistogramMassBinSize", "AlignmentOptions")]
        [DataSummaryAttribute("Histogram Mass Bin Size")]
        public double MassBinSize { get; set; }
        [ParameterFileAttribute("MassCalibrationLSQNumKnots", "AlignmentOptions")]
        [DataSummaryAttribute("Mass Calibration LSQ Number Knots")]
        public short MassCalibrationLSQNumKnots { get; set; }
        [ParameterFileAttribute("MassCalibrationLSQZScore", "AlignmentOptions")]
        [DataSummaryAttribute("Mass Calibration LSQZScore")]
        public double MassCalibrationLSQZScore { get; set; }
        [DataSummaryAttribute("Mass Calibration Max Jump")]
        [ParameterFileAttribute("MassCalibrationMaxJump", "AlignmentOptions")]
        public short MassCalibrationMaxJump { get; set; }
        [ParameterFileAttribute("MassCalibrationMaxZScore", "AlignmentOptions")]
        [DataSummaryAttribute("Max Z-Score")]
        public double MassCalibrationMaxZScore { get; set; }
        [ParameterFileAttribute("MassCalibrationNumMassDeltaBins", "AlignmentOptions")]
        [DataSummaryAttribute("Mass Calibration Number of Mass Delta Bins")]
        public short MassCalibrationNumMassDeltaBins { get; set; }
        [ParameterFileAttribute("MassCalibrationNumXSlices", "AlignmentOptions")]
        [DataSummaryAttribute("Mass Calibration Number of X Slices")]
        public short MassCalibrationNumXSlices { get; set; }
        [DataSummaryAttribute("Mass Calibration Use LSQ")]
        [ParameterFileAttribute("MassCalibrationUseLSQ", "AlignmentOptions")]
        public bool MassCalibrationUseLSQ { get; set; }
        [DataSummaryAttribute("Mass Calibration Window")]
        [ParameterFileAttribute("MassCalibrationWindow", "AlignmentOptions")]
        public double MassCalibrationWindow { get; set; }
        [ParameterFileAttribute("MassTolerance", "AlignmentOptions")]
        [DataSummaryAttribute("Mass Tolerance")]
        public double MassTolerance { get; set; }
        [ParameterFileAttribute("MaxPromiscuity", "AlignmentOptions")]
        [DataSummaryAttribute("Max Promiscuity")]
        public short MaxPromiscuity { get; set; }
        [ParameterFileAttribute("MaxTimeJump", "AlignmentOptions")]
        [DataSummaryAttribute("Max Time Jump")]
        public short MaxTimeJump { get; set; }
        [DataSummaryAttribute("Split Alignment M/Z Boundary")]
        public List<classAlignmentMZBoundary> MZBoundaries { get; set; }
        [ParameterFileAttribute("HistogramNETBinSize", "AlignmentOptions")]
        [DataSummaryAttribute("Histogram NET Bin Size")]
        public double NETBinSize { get; set; }
        [DataSummaryAttribute("Net Tolerance")]
        [ParameterFileAttribute("NETTolerance", "AlignmentOptions")]
        public double NETTolerance { get; set; }
        [DataSummaryAttribute("Number of Time Sections")]
        [ParameterFileAttribute("NumTimeSections", "AlignmentOptions")]
        public int NumTimeSections { get; set; }
        [ParameterFileAttribute("RecalibrationType", "AlignmentOptions")]
        [DataSummaryAttribute("Recalibration Type")]
        public enmCalibrationType RecalibrationType { get; set; }
        [ParameterFileAttribute("SplitAlignmentMZ", "AlignmentOptions")]
        [DataSummaryAttribute("Split Alignment by M/Z")]
        public bool SplitAlignmentInMZ { get; set; }
        [ParameterFileAttribute("UsePromiscuousPoints", "AlignmentOptions")]
        [DataSummaryAttribute("Use Promiscuous Points")]
        public bool UsePromiscuousPoints { get; set; }


        public static clsAlignmentOptions ConvertNewToOld(AlignmentOptions newOptions)
        {
            clsAlignmentOptions options             = new clsAlignmentOptions();
            options.AlignmentBaselineName           = newOptions.AlignmentBaselineName;
            options.AlignmentType                   = newOptions.AlignmentType;
           
            options.ContractionFactor               = newOptions.ContractionFactor;
            options.DriftTimeBinSize                = newOptions.DriftTimeBinSize;
            options.IsAlignmentBaselineAMasstagDB   = newOptions.IsAlignmentBaselineAMasstagDB;
            options.MassBinSize                     = newOptions.MassBinSize;
            options.MassCalibrationLSQNumKnots      = newOptions.MassCalibrationLSQNumKnots;
            options.MassCalibrationLSQZScore        = newOptions.MassCalibrationLSQZScore;
            options.MassCalibrationMaxJump          = newOptions.MassCalibrationMaxJump;
            options.MassCalibrationMaxZScore        = newOptions.MassCalibrationMaxZScore;
            options.MassCalibrationNumMassDeltaBins = newOptions.MassCalibrationNumMassDeltaBins;
            options.MassCalibrationNumXSlices       = newOptions.MassCalibrationNumXSlices;
            options.MassCalibrationUseLSQ           = newOptions.MassCalibrationUseLSQ;
            options.MassCalibrationWindow           = newOptions.MassCalibrationWindow;
            options.MassTolerance                   = newOptions.MassTolerance;
            options.MaxPromiscuity                  = newOptions.MaxPromiscuity; 
            options.MaxTimeJump                     = newOptions.MaxTimeJump;
            options.MZBoundaries                    = newOptions.MZBoundaries;
            options.NETBinSize                      = newOptions.NETBinSize;
            options.NETTolerance                    = newOptions.NETTolerance;
            options.NumTimeSections                 = newOptions.NumTimeSections; 
            options.RecalibrationType               = newOptions.RecalibrationType; 
            options.SplitAlignmentInMZ              = newOptions.SplitAlignmentInMZ; 
            options.UsePromiscuousPoints            = newOptions.UsePromiscuousPoints;

            return options;
        }
    }
}
