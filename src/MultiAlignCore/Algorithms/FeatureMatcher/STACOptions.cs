using MultiAlignEngine;

namespace MultiAlignCore.Algorithms.PeakMatching
{
    public class STACOptions
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public STACOptions()
        {

        }
        
        [clsParameterFile("UsePriorProbabilities", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public double HistogramBinWidth 
        {
            get;
            set;
        }
        [clsParameterFile("HistogramMultiplier", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public double HistogramMultiplier  
        {
            get;
            set;
        }
        [clsParameterFile("ShiftAmount", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public double  ShiftAmount  
        {
            get;
            set;
        }
        [clsParameterFile("ShouldCalculateHistogramFDR", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateHistogramFDR  
        {
            get;
            set;
        }
        [clsParameterFile("ShouldCalculateShiftFDR", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateShiftFDR  
        {
            get;
            set;
        }
        [clsParameterFile("ShouldCalculateSLiC", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateSLiC  
        {
            get;
            set;
        }
        [clsParameterFile("ShouldCalculateSTAC", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public bool ShouldCalculateSTAC  
        {
            get;
            set;
        }
        [clsParameterFile("UseDriftTime", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public bool UseDriftTime  
        {
            get;
            set;
        }
        [clsParameterFile("UseEllipsoid", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public bool UseEllipsoid  
        {
            get;
            set;
        }
        [clsParameterFile("UsePriors", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public bool UsePriors  
        {
            get;
            set;
        }
        [clsParameterFile("UserTolerances", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public bool UserTolerances  
        {
            get;
            set;
        }
    }
}
