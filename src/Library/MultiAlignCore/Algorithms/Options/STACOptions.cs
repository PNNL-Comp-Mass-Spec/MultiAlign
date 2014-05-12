#region

using System.ComponentModel;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.IO.Parameters;

#endregion

namespace MultiAlignCore.Algorithms.Options
{
    /// <summary>
    ///     Options for running STAC.
    /// </summary>
    public class StacOptions
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        public StacOptions()
        {
            MassTolerancePPM = 6.0;
            NETTolerance = 0.03;
            DriftTimeTolerance = 50.0;
            ShiftAmount = 11;
            UsePriors = true;
            UseEllipsoid = false;
            UseDriftTime = false;
            ShouldCalculateSTAC = true;
            IdentificationAlgorithm = PeakMatchingType.STAC;
        }

        [ParameterFile("IdentificationAlgorithm", "Algorithm")]
        [Category("Algorithm")]
        [Description("Determines what identification algorithm to use")]
        public PeakMatchingType IdentificationAlgorithm { get; set; }

        [ParameterFile("UsePriors", "STAC")]
        [Category("AMT")]
        [Description("Determines if prior probilities of the tags being matched to should be used.")]
        public bool UsePriors { get; set; }

        [ParameterFile("HistogramBinWidth", "STAC")]
        [Category("Binning")]
        [Description("Determines the width of the bin")]
        public double HistogramBinWidth { get; set; }

        [ParameterFile("HistogramMultiplier", "STAC")]
        [Category("Binning")]
        [Description("")]
        public double HistogramMultiplier { get; set; }

        [ParameterFile("ShiftAmount", "STAC")]
        [Category("FDR Calculation")]
        [Description("Shift amount for the dalton shift FDR calculation.")]
        public double ShiftAmount { get; set; }

        [ParameterFile("ShouldCalculateHistogramFDR", "STAC")]
        [Category("FDR Calculation")]
        [Description("Determines if a FDR histogram should be calculated.")]
        public bool ShouldCalculateHistogramFDR { get; set; }

        [ParameterFile("ShouldCalculateShiftFDR", "STAC")]
        [Category("FDR Calculation")]
        [Description("Determines if STAC should calculate the dalton shift FDR.")]
        public bool ShouldCalculateShiftFDR { get; set; }

        [ParameterFile("ShouldCalculateSLiC", "STAC")]
        [Category("FDR Calculation")]
        [Description("Determines if STAC should calculate The SLiC Score previously used by VIPER")]
        public bool ShouldCalculateSLiC { get; set; }

        [ParameterFile("ShouldCalculateSTAC", "STAC")]
        [Category("FDR Calculation")]
        [Description("Determines if the STAC scores should be calculated")]
        public bool ShouldCalculateSTAC { get; set; }

        [ParameterFile("UseDriftTime", "STAC")]
        [Category("Ion Mobility")]
        [Description("Determines if Ion Mobility was used.")]
        public bool UseDriftTime { get; set; }

        [ParameterFile("UseEllipsoid", "STAC")]
        [Category("Tolerances")]
        [Description("")]
        public bool UseEllipsoid { get; set; }

        [ParameterFile("DriftTimeTolerance", "STAC")]
        [Category("Tolerances")]
        [Description("Drift time tolerance if using Ion Mobility data.")]
        public double DriftTimeTolerance { get; set; }

        [ParameterFile("MassTolerancePPM", "STAC")]
        [Category("Tolerances")]
        [Description("Monoisotopic Mass tolerance in parts per million (PPM).")]
        public double MassTolerancePPM { get; set; }

        [ParameterFile("NETTolerance", "STAC")]
        [Category("Tolerances")]
        [Description("Normalized elution time (NET) tolerance.")]
        public double NETTolerance { get; set; }

        [ParameterFile("Refined", "STAC")]
        [Category("Tolerances")]
        [Description("Determines if tolerances were refined using STAC.")]
        public bool Refined { get; set; }
    }
}