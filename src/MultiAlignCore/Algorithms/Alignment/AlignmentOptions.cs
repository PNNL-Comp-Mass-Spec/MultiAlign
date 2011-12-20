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
        [ParameterFileAttribute("AlignmentType", "Alignment")]
        [DataSummaryAttribute("Alignment Type")]
        public enmAlignmentType AlignmentType { get; set; }
        [DataSummaryAttribute("Apply Mass Recalibration")]
        //[ParameterFileAttribute("ApplyMassRecalibration", "Alignment")]
        //public bool ApplyMassRecalibration { get; set; }
        [DataSummaryAttribute("Contraction Factor")]
        [ParameterFileAttribute("ContractionFactor", "Alignment")]
        public int ContractionFactor { get; set; }
        [ParameterFileAttribute("HistogramDriftTimeBinSize", "Alignment")]
        [DataSummaryAttribute("Histogram Drift Time Bin Size")]
        public double DriftTimeBinSize { get; set; }
        [DataSummaryAttribute("Is Aligned to Mass Tag Database")]
        public bool IsAlignmentBaselineAMasstagDB { get; set; }
        [ParameterFileAttribute("HistogramMassBinSize", "Alignment")]
        [DataSummaryAttribute("Histogram Mass Bin Size")]
        public double MassBinSize { get; set; }
        [ParameterFileAttribute("MassCalibrationLSQNumKnots", "Alignment")]
        [DataSummaryAttribute("Mass Calibration LSQ Number Knots")]
        public int MassCalibrationLSQNumKnots { get; set; }
        [ParameterFileAttribute("MassCalibrationLSQZScore", "Alignment")]
        [DataSummaryAttribute("Mass Calibration LSQZScore")]
        public double MassCalibrationLSQZScore { get; set; }
        [DataSummaryAttribute("Mass Calibration Max Jump")]
        [ParameterFileAttribute("MassCalibrationMaxJump", "Alignment")]
        public int MassCalibrationMaxJump { get; set; }
        [ParameterFileAttribute("MassCalibrationMaxZScore", "Alignment")]
        [DataSummaryAttribute("Max Z-Score")]
        public double MassCalibrationMaxZScore { get; set; }
        [ParameterFileAttribute("MassCalibrationNumMassDeltaBins", "Alignment")]
        [DataSummaryAttribute("Mass Calibration Number of Mass Delta Bins")]
        public int MassCalibrationNumMassDeltaBins { get; set; }
        [ParameterFileAttribute("MassCalibrationNumXSlices", "Alignment")]
        [DataSummaryAttribute("Mass Calibration Number of X Slices")]
        public int MassCalibrationNumXSlices { get; set; }
        [DataSummaryAttribute("Mass Calibration Use LSQ")]
        [ParameterFileAttribute("MassCalibrationUseLSQ", "Alignment")]
        public bool MassCalibrationUseLSQ { get; set; }
        [DataSummaryAttribute("Mass Calibration Window")]
        [ParameterFileAttribute("MassCalibrationWindow", "Alignment")]
        public double MassCalibrationWindow { get; set; }
        [ParameterFileAttribute("MassTolerance", "Alignment")]
        [DataSummaryAttribute("Mass Tolerance")]
        public double MassTolerance { get; set; }
        [ParameterFileAttribute("MaxPromiscuity", "Alignment")]
        [DataSummaryAttribute("Max Promiscuity")]
        public int MaxPromiscuity { get; set; }
        [ParameterFileAttribute("MaxTimeJump", "Alignment")]
        [DataSummaryAttribute("Max Time Jump")]
        public int MaxTimeJump { get; set; }
        [DataSummaryAttribute("Split Alignment M/Z Boundary")]
        public List<classAlignmentMZBoundary> MZBoundaries { get; set; }
        [ParameterFileAttribute("HistogramNETBinSize", "Alignment")]
        [DataSummaryAttribute("Histogram NET Bin Size")]
        public double NETBinSize { get; set; }
        [DataSummaryAttribute("Net Tolerance")]
        [ParameterFileAttribute("NETTolerance", "Alignment")]
        public double NETTolerance { get; set; }
        [DataSummaryAttribute("Number of Time Sections")]
        [ParameterFileAttribute("NumTimeSections", "Alignment")]
        public int NumTimeSections { get; set; }
        [ParameterFileAttribute("RecalibrationType", "Alignment")]
        [DataSummaryAttribute("Recalibration Type")]
        public enmCalibrationType RecalibrationType { get; set; }
        [ParameterFileAttribute("SplitAlignmentMZ", "Alignment")]
        [DataSummaryAttribute("Split Alignment by M/Z")]
        public bool SplitAlignmentInMZ { get; set; }
        [ParameterFileAttribute("UsePromiscuousPoints", "Alignment")]
        [DataSummaryAttribute("Use Promiscuous Points")]
        public bool UsePromiscuousPoints { get; set; }


        public static clsAlignmentOptions ConvertToEngine(AlignmentOptions newOptions)
        {
            clsAlignmentOptions options             = new clsAlignmentOptions();
            options.AlignmentBaselineName           = newOptions.AlignmentBaselineName;
            options.AlignmentType                   = newOptions.AlignmentType;
           
            options.ContractionFactor               = System.Convert.ToInt16(newOptions.ContractionFactor);
            options.DriftTimeBinSize                = newOptions.DriftTimeBinSize;
            options.IsAlignmentBaselineAMasstagDB   = newOptions.IsAlignmentBaselineAMasstagDB;
            options.MassBinSize                     = newOptions.MassBinSize;
            options.MassCalibrationLSQNumKnots      = System.Convert.ToInt16(newOptions.MassCalibrationLSQNumKnots);
            options.MassCalibrationLSQZScore        = newOptions.MassCalibrationLSQZScore;
            options.MassCalibrationMaxJump          = System.Convert.ToInt16(newOptions.MassCalibrationMaxJump);
            options.MassCalibrationMaxZScore        = newOptions.MassCalibrationMaxZScore;
            options.MassCalibrationNumMassDeltaBins = System.Convert.ToInt16(newOptions.MassCalibrationNumMassDeltaBins);
            options.MassCalibrationNumXSlices       = System.Convert.ToInt16(newOptions.MassCalibrationNumXSlices);
            options.MassCalibrationUseLSQ           = newOptions.MassCalibrationUseLSQ;
            options.MassCalibrationWindow           = newOptions.MassCalibrationWindow;
            options.MassTolerance                   = newOptions.MassTolerance;
            options.MaxPromiscuity                  = System.Convert.ToInt16(newOptions.MaxPromiscuity); 
            options.MaxTimeJump                     = System.Convert.ToInt16(newOptions.MaxTimeJump);
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
