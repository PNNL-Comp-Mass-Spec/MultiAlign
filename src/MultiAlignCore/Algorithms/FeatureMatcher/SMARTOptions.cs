using MultiAlignEngine;

namespace MultiAlignCore.Algorithms.PeakMatching
{
    public class SMARTOptions
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SMARTOptions()
        {
            IsDataPaired            = false;
            MassTolerancePPM        = 20;
            NETTolerance            = .035;
            PairedMass              = 0;
            UsePriorProbabilities   = true;
        }
        [clsParameterFile("IsDataPaired", "SMARTOptions")]
        [clsDataSummaryAttribute("Is Data Paired")]
        public bool IsDataPaired { get; set; }
        [clsParameterFile("MassTolerancePPM", "SMARTOptions")]
        [clsDataSummaryAttribute("Mass Tolerance PPM")]
        public double MassTolerancePPM { get; set; }
        [clsParameterFile("NETTolerance", "SMARTOptions")]
        [clsDataSummaryAttribute("NET Tolerance")]
        public double NETTolerance { get; set; }
        [clsParameterFile("PairedMass", "SMARTOptions")]
        [clsDataSummaryAttribute("Mass of paired species")]
        public double PairedMass { get; set; }
        [clsParameterFile("UsePriorProbabilities", "SMARTOptions")]
        [clsDataSummaryAttribute("Uses the prior probabilities")]
        public bool UsePriorProbabilities { get; set; }
    }
}
