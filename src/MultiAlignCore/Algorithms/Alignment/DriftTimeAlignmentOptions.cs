using System;
using MultiAlignEngine;
using MultiAlignCore.IO.Parameters;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.Alignment
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
        [ParameterFileAttribute("DriftTimeTolerance", "DriftTime")]
        [DataSummaryAttribute("Drift Time Tolerance")]
        public double DriftTimeTolerance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Mass PPM Tolerance (monoisotopic).
        /// </summary>
        [ParameterFileAttribute("MassTolerance", "DriftTime")]
        [DataSummaryAttribute("Mass Tolerance")]
        public double MassPPMTolerance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the NET tolerance.
        /// </summary>
        [ParameterFileAttribute("NETTolerance", "DriftTime")]
        [DataSummaryAttribute("NET Tolerance")]
        public double NETTolerance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets teh maximum charge state to allow.
        /// </summary>
        [ParameterFileAttribute("MaxChargeState", "DriftTime")]
        [DataSummaryAttribute("Maximum Charge State")]
        public int MaxChargeState
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the minimum charge state to allow.
        /// </summary>
        [ParameterFileAttribute("MinChargeState", "DriftTime")]
        [DataSummaryAttribute("Minimum Charge State")]
        public int MinChargeState
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether to perform the offset for drift time calculation.
        /// </summary>
        [ParameterFileAttribute("ShouldPerformOffset", "DriftTime")]
        [DataSummaryAttribute("Perform offset calculation")]
        public bool ShouldPerformOffset
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether to use all observations for the drift time offset calculation.
        /// </summary>
        [ParameterFileAttribute("UseAllObservationsForOffset", "DriftTime")]
        [DataSummaryAttribute("Uses all observatiosn for the offset calculation")]
        public bool ShouldUseAllObservationsForOffsetCalculation
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether to use all observations for the drift time offset calculation.
        /// </summary>
        [ParameterFileAttribute("ShouldAlignDriftTimes", "DriftTime")]
        [DataSummaryAttribute("Aligned Drift Times")]
        public bool ShouldAlignDriftTimes
        {
            get;
            set;
        }
    }
}
