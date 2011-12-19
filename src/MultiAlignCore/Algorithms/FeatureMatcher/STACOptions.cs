using MultiAlignEngine;
using PNNLOmics.Algorithms.FeatureMatcher;
using MultiAlignCore.IO.Parameters;
using MultiAlignCore.Data;


namespace MultiAlignCore.Algorithms.PeakMatching
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
        
        [ParameterFileAttribute("UsePriorProbabilities", "STACOptions")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public double HistogramBinWidth 
        {
            get;
            set;
        }
        [ParameterFileAttribute("HistogramMultiplier", "STACOptions")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public double HistogramMultiplier  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShiftAmount", "STACOptions")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public double  ShiftAmount  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateHistogramFDR", "STACOptions")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateHistogramFDR  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateShiftFDR", "STACOptions")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateShiftFDR  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateSLiC", "STACOptions")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateSLiC  
        {
            get;
            set;
        }
        [ParameterFileAttribute("ShouldCalculateSTAC", "STACOptions")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateSTAC  
        {
            get;
            set;
        }
        [ParameterFileAttribute("UseDriftTime", "STACOptions")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool UseDriftTime  
        {
            get;
            set;
        }
        [ParameterFileAttribute("UseEllipsoid", "STACOptions")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool UseEllipsoid  
        {
            get;
            set;
        }
        [ParameterFileAttribute("UsePriors", "STACOptions")]
        [DataSummaryAttribute("Uses the prior probabilities")]
        public bool UsePriors  
        {
            get;
            set;
        }
        [ParameterFileAttribute("DriftTimeTolerance", "STACOptions")]
        public double DriftTimeTolerance { get; set; }
        [ParameterFileAttribute("MassTolerancePPM", "STACOptions")]
        public double MassTolerancePPM { get; set; }
        [ParameterFileAttribute("NETTolerance", "STACOptions")]
        public double NETTolerance { get; set; }
        [ParameterFileAttribute("Refined", "STACOptions")]
        public bool Refined { get; set; }
        [ParameterFileAttribute("WriteResultsBackToMTS", "STACOptions")]
        public bool WriteResultsBackToMTS
        {
            get;
            set;
        }
    }
}
