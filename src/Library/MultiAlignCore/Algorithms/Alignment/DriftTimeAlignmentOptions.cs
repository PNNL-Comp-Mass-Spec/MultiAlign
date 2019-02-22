#region

using System.ComponentModel;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Parameters;

#endregion

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
            DriftTimeTolerance = 5;
            MassPPMTolerance = 15;
            NETTolerance = .03;

            MaxChargeState = 4;
            MinChargeState = 1;

            ShouldPerformOffset = true;
            ShouldUseAllObservationsForOffsetCalculation = false;
        }

        /// <summary>
        /// Gets or sets whether to perform the offset for drift time calculation.
        /// </summary>
        [ParameterFile("ShouldPerformOffset", "DriftTime")]
        [DataSummary("Perform offset calculation")]
        [Category("Alignment")]
        [Description("Should perform offset correction based on error distributions.")]
        public bool ShouldPerformOffset { get; set; }

        /// <summary>
        /// Gets or sets whether to use all observations for the drift time offset calculation.
        /// </summary>
        [ParameterFile("UseAllObservationsForOffset", "DriftTime")]
        [DataSummary("Uses all observatiosn for the offset calculation")]
        [Category("Alignment")]
        [Description("")]
        public bool ShouldUseAllObservationsForOffsetCalculation { get; set; }

        /// <summary>
        /// Gets or sets whether to use all observations for the drift time offset calculation.
        /// </summary>
        [ParameterFile("ShouldAlignDriftTimes", "DriftTime")]
        [DataSummary("Aligned Drift Times")]
        [Category("Ion Mobility")]
        [Description("Determines if drift time alignment should be computed at all.  True yes = IMS, False no = LC-MS.")
        ]
        public bool ShouldAlignDriftTimes { get; set; }

        /// <summary>
        /// Gets or sets the drift time tolerance to use.
        /// </summary>
        [ParameterFile("DriftTimeTolerance", "DriftTime")]
        [DataSummary("Drift Time Tolerance")]
        [Category("Tolerances")]
        [Description("Drift time tolerance (ms).")]
        public double DriftTimeTolerance { get; set; }

        /// <summary>
        /// Gets or sets the Mass PPM Tolerance (monoisotopic).
        /// </summary>
        [ParameterFile("MassTolerance", "DriftTime")]
        [DataSummary("Mass Tolerance")]
        [Category("Tolerances")]
        [Description("Monoisotopic mass tolerance in parts per million (PPM)")]
        public double MassPPMTolerance { get; set; }

        /// <summary>
        /// Gets or sets teh maximum charge state to allow.
        /// </summary>
        [ParameterFile("MaxChargeState", "DriftTime")]
        [DataSummary("Maximum Charge State")]
        [Category("Tolerances")]
        [Description("Maximum charge state to consider")]
        public int MaxChargeState { get; set; }

        /// <summary>
        /// Gets or sets the minimum charge state to allow.
        /// </summary>
        [ParameterFile("MinChargeState", "DriftTime")]
        [DataSummary("Minimum Charge State")]
        [Category("Tolerances")]
        [Description("Minimum charge state to consider.")]
        public int MinChargeState { get; set; }

        /// <summary>
        /// Gets or sets the NET tolerance.
        /// </summary>
        [ParameterFile("NETTolerance", "DriftTime")]
        [DataSummary("NET Tolerance")]
        [Category("Tolerances")]
        [Description("Normalized elution time (NET) tolerance.")]
        public double NETTolerance { get; set; }
    }
}