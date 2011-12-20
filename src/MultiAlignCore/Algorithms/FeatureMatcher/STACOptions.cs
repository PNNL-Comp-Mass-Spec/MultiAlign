using MultiAlignEngine;
using PNNLOmics.Algorithms.FeatureMatcher;
using MultiAlignCore.IO.Parameters;
using MultiAlignCore.Data;


namespace MultiAlignCore.Algorithms.FeatureMatcher
{
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
            UsePriors               = false;
            UseEllipsoid            = false;
            UseDriftTime            = false;            
        }
        
        [ParameterFileAttribute("UsePriorProbabilities", "STAC")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public double HistogramBinWidth 
        {
            get;
            set;
        }
        [ParameterFileAttribute("HistogramMultiplier", "STAC")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public double HistogramMultiplier  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShiftAmount", "STAC")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public double  ShiftAmount  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateHistogramFDR", "STAC")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateHistogramFDR  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateShiftFDR", "STAC")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateShiftFDR  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateSLiC", "STAC")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateSLiC  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateSTAC", "STAC")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateSTAC  
        {
            get;
            set;
        }
        [ParameterFileAttribute("UseDriftTime", "STAC")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool UseDriftTime  
        {
            get;
            set;
        }
        [ParameterFileAttribute("UseEllipsoid", "STAC")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool UseEllipsoid  
        {
            get;
            set;
        }
        [ParameterFileAttribute("UsePriors", "STAC")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool UsePriors  
        {
            get;
            set;
        }
        [ParameterFileAttribute("DriftTimeTolerance", "STAC")]
        public double DriftTimeTolerance { get; set; }
        [ParameterFileAttribute("MassTolerancePPM", "STAC")]
        public double MassTolerancePPM { get; set; }
        [ParameterFileAttribute("NETTolerance", "STAC")]
        public double NETTolerance { get; set; }
        [ParameterFileAttribute("Refined", "STAC")]
        public bool Refined { get; set; }
        [ParameterFileAttribute("WriteResultsBackToMTS", "STAC")]
        public bool WriteResultsBackToMTS
        {
            get;
            set;
        }
    }
}
