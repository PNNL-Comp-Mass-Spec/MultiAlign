using System;
using MultiAlignEngine;

namespace PNNLProteomics.Algorithms.Alignment
{
    /// <summary>
    /// Tolerances and options for drift time alignment algorithm.
    /// </summary>
    public class DriftTimeAlignmentOptions
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DriftTimeAlignmentOptions()
        {
            DriftTimeTolerance  = 5;
            MassPPMTolerance    = 15;
            NETTolerance        = .03;

            MaxChargeState      = 4;
            MinChargeState      = 1;

            ShouldPerformOffset                          = true;
            ShouldUseAllObservationsForOffsetCalculation = false;
        }

        /// <summary>
        /// Gets or sets the drift time tolerance to use.
        /// </summary>
        [clsParameterFile("DriftTimeTolerance", "DriftTime")]
        [clsDataSummaryAttribute("Drift Time Tolerance")]
        public double DriftTimeTolerance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Mass PPM Tolerance (monoisotopic).
        /// </summary>
        [clsParameterFile("MassTolerance", "DriftTime")]
        [clsDataSummaryAttribute("Mass Tolerance")]
        public double MassPPMTolerance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the NET tolerance.
        /// </summary>
        [clsParameterFile("NETTolerance", "DriftTime")]
        [clsDataSummaryAttribute("NET Tolerance")]
        public double NETTolerance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets teh maximum charge state to allow.
        /// </summary>
        [clsParameterFile("MaxChargeState", "DriftTime")]
        [clsDataSummaryAttribute("Maximum Charge State")]
        public int MaxChargeState
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the minimum charge state to allow.
        /// </summary>
        [clsParameterFile("MinChargeState", "DriftTime")]
        [clsDataSummaryAttribute("Minimum Charge State")]
        public int MinChargeState
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether to perform the offset for drift time calculation.
        /// </summary>
        [clsParameterFile("ShouldPerformOffset", "DriftTime")]
        [clsDataSummaryAttribute("Perform offset calculation")]
        public bool ShouldPerformOffset
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether to use all observations for the drift time offset calculation.
        /// </summary>
        [clsParameterFile("UseAllObservationsForOffset", "DriftTime")]
        [clsDataSummaryAttribute("Uses all observatiosn for the offset calculation")]
        public bool ShouldUseAllObservationsForOffsetCalculation
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether to use all observations for the drift time offset calculation.
        /// </summary>
        [clsParameterFile("ShouldAlignDriftTimes", "DriftTime")]
        [clsDataSummaryAttribute("Aligned Drift Times")]
        public bool ShouldAlignDriftTimes
        {
            get;
            set;
        }
    }
}
