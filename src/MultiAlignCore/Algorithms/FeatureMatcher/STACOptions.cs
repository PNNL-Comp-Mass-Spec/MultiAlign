using MultiAlignEngine;
using PNNLOmics.Algorithms.FeatureMatcher;
using MultiAlignCore.IO.Parameters;
using MultiAlignCore.Data;
using System.ComponentModel;

namespace MultiAlignCore.Algorithms.FeatureMatcher
{
    /// <summary>
    /// Options for running STAC.
    /// </summary>
    public class STACOptions
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public STACOptions()
        {
            MassTolerancePPM        = 6.0;
            NETTolerance            = 0.03;
            DriftTimeTolerance      = 50.0;
            WriteResultsBackToMTS   = false;            
            ShiftAmount             = 11;
            UsePriors               = true;
            UseEllipsoid            = false;
            UseDriftTime            = false;
            ShouldCalculateSTAC     = true;
        }

        [ParameterFileAttribute("UsePriors", "STAC")]
        [Category("AMT")]
        [Description("Determines if prior probilities of the tags being matched to should be used.")]
        public bool UsePriors
        {
            get;
            set;
        }
        [ParameterFileAttribute("WriteResultsBackToMTS", "STAC")]
        [Category("AMT")]
        [Description("Determines if results from STAC should be written back to the Mass Tag System (PNNL Only).")]
        public bool WriteResultsBackToMTS
        {
            get;
            set;
        }
        [ParameterFileAttribute("HistogramBinWidth", "STAC")]
        [Category("Binning")]
        [Description("Determines the width of the bin")]        
        public double HistogramBinWidth 
        {
            get;
            set;
        }
        [ParameterFileAttribute("HistogramMultiplier", "STAC")]
        [Category("Binning")]
        [Description("")]        
        public double HistogramMultiplier  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShiftAmount", "STAC")]
        [Category("FDR Calculation")]
        [Description("Shift amount for the dalton shift FDR calculation.")]        
        public double  ShiftAmount  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateHistogramFDR", "STAC")]
        [Category("FDR Calculation")]
        [Description("Determines if a FDR histogram should be calculated.")]        
        public bool ShouldCalculateHistogramFDR  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateShiftFDR", "STAC")]
        [Category("FDR Calculation")]
        [Description("Determines if STAC should calculate the dalton shift FDR.")]        
        public bool ShouldCalculateShiftFDR  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateSLiC", "STAC")]
        [Category("FDR Calculation")]
        [Description("Determines if STAC should calculate The SLiC Score previously used by VIPER")]        
        public bool ShouldCalculateSLiC  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateSTAC", "STAC")]
        [Category("FDR Calculation")]
        [Description("Determines if the STAC scores should be calculated")]        
        public bool ShouldCalculateSTAC  
        {
            get;
            set;
        }
        [ParameterFileAttribute("UseDriftTime", "STAC")]
        [Category("Ion Mobility")]
        [Description("Determines if Ion Mobility was used.")]        
        public bool UseDriftTime  
        {
            get;
            set;
        }
        [ParameterFileAttribute("UseEllipsoid", "STAC")]
        [Category("Tolerances")]
        [Description("")]        
        public bool UseEllipsoid  
        {
            get;
            set;
        }
        [ParameterFileAttribute("DriftTimeTolerance", "STAC")]
        [Category("Tolerances")]
        [Description("Drift time tolerance if using Ion Mobility data.")]        
        public double DriftTimeTolerance { get; set; }
        [ParameterFileAttribute("MassTolerancePPM", "STAC")]
        [Category("Tolerances")]
        [Description("Monoisotopic Mass tolerance in parts per million (PPM).")]        
        public double MassTolerancePPM { get; set; }
        [ParameterFileAttribute("NETTolerance", "STAC")]
        [Category("Tolerances")]
        [Description("Normalized elution time (NET) tolerance.")]        
        public double NETTolerance { get; set; }
        [ParameterFileAttribute("Refined", "STAC")]
        [Category("Tolerances")]
        [Description("Determines if tolerances were refined using STAC.")]        
        public bool Refined { get; set; }
    }
}
