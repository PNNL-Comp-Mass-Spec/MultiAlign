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
            IsDataPaired            = false;
            MassTolerancePPM        = 20;
            NETTolerance            = .035;
            PairedMass              = 0;
            UsePriorProbabilities   = true;
        }

        [clsParameterFile("IsDataPaired", "STACOptions")]
        [clsDataSummaryAttribute("Is Data Paired")]
        public bool IsDataPaired { get; set; }
        [clsParameterFile("MassTolerancePPM", "STACOptions")]
        [clsDataSummaryAttribute("Mass Tolerance PPM")]
        public double MassTolerancePPM { get; set; }
        [clsParameterFile("NETTolerance", "STACOptions")]
        [clsDataSummaryAttribute("NET Tolerance")]
        public double NETTolerance { get; set; }
        [clsParameterFile("PairedMass", "STACOptions")]
        [clsDataSummaryAttribute("Mass of paired species")]
        public double PairedMass { get; set; }
        [clsParameterFile("UsePriorProbabilities", "STACOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public bool UsePriorProbabilities { get; set; }
    }
}
