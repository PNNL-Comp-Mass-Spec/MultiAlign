using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.FeatureMatcher.Data
{
    public class FeatureMatcherParameters
    {
        #region Members
        private FeatureMatcherTolerances m_userTolerances;

        private bool m_useEllipsoid;
        private bool m_shouldCalculateShiftFDR;
        private bool m_shouldCalculateSTAC;
        private bool m_shouldCalculateHistogramFDR;
        private bool m_shouldCalculateSLiC;
        private bool m_useDriftTime;
        private bool m_usePriors;

        private List<int> m_chargeStateList;

        private double m_shiftAmount;
        private double m_histogramBinWidth;
        private double m_histogramMultiplier;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor for FeatureMatcherParameters.
        /// </summary>
        public FeatureMatcherParameters()
        {
            Clear();
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the user defined tolerances for initial matching.
        /// </summary>
        public FeatureMatcherTolerances UserTolerances
        {
            get { return m_userTolerances; }
            set { m_userTolerances = value; }
        }
        /// <summary>
        /// Sets tolerances using values.
        /// </summary>
        /// <param name="massTolerance">Mass tolerance in PPM.</param>
        /// <param name="netTolerance">NET tolerance.</param>
        /// <param name="driftTimeTolerance">Drift time tolerance.</param>
        public void SetTolerances(double massTolerance, double netTolerance, float driftTimeTolerance)
        {
            m_userTolerances.Refined = false;
            m_userTolerances.MassTolerancePPM = massTolerance;
            m_userTolerances.NETTolerance = netTolerance;
            m_userTolerances.DriftTimeTolerance = driftTimeTolerance;
        }

        /// <summary>
        /// Gets or sets whether or not an ellipsoidal region is used for shift matches.
        /// </summary>
        public bool UseEllipsoid
        {
            get { return m_useEllipsoid; }
            set { m_useEllipsoid = value; }
        }
        /// <summary>
        /// Gets or sets whether to calculate FDR using a fixed shift.
        /// </summary>
        public bool ShouldCalculateShiftFDR
        {
            get { return m_shouldCalculateShiftFDR; }
            set { m_shouldCalculateShiftFDR = value; }
        }
        /// <summary>
        /// Gets or sets whether to calculate STAC scores and FDR.
        /// </summary>
        public bool ShouldCalculateSTAC
        {
            get { return m_shouldCalculateSTAC; }
            set { m_shouldCalculateSTAC = value; }
        }
        /// <summary>
        /// Gets or sets whether to create a mass error histogram to compute FDR.
        /// </summary>
        public bool ShouldCalculateHistogramFDR
        {
            get { return m_shouldCalculateHistogramFDR; }
            set { m_shouldCalculateHistogramFDR = value; }
        }
        /// <summary>
        /// Gets or sets whether to calculate SLiC scores for each match.
        /// </summary>
        public bool ShouldCalculateSLiC
        {
            get { return m_shouldCalculateSLiC; }
            set { m_shouldCalculateSLiC = value; }
        }
        /// <summary>
        /// Gets or sets whether to use the drift times in calculations.
        /// </summary>
        public bool UseDriftTime
        {
            get { return m_useDriftTime; }
            set { m_useDriftTime = value; }
        }
        /// <summary>
        /// Gets or sets whether to use prior probabilities in the calculation of STAC scores.
        /// </summary>
        public bool UsePriors
        {
            get { return m_usePriors; }
            set { m_usePriors = value; }
        }

        /// <summary>
        /// Gets or sets the list of charge states to perform algorithms over in the case of IMS data.
        /// </summary>
        public List<int> ChargeStateList
        {
            get { return m_chargeStateList; }
            set { m_chargeStateList = value; }
        }

        /// <summary>
        /// Gets or sets the amount of fixed shift to use for calculating shifted FDR.
        /// </summary>
        public double ShiftAmount
        {
            get { return m_shiftAmount; }
            set { m_shiftAmount = value; }
        }
        /// <summary>
        /// Gets or sets the histogram bin width for mass error histogram FDR calculation.
        /// </summary>
        public double HistogramBinWidth
        {
            get { return m_histogramBinWidth; }
            set { m_histogramBinWidth = value; }
        }
        /// <summary>
        /// Gets or sets the multiplier to use when calculating FDR via the mass error histogram.
        /// </summary>
        public double HistogramMultiplier
        {
            get { return m_histogramMultiplier; }
            set { m_histogramMultiplier = value; }
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Resets all parameters to default values.
        /// </summary>
        private void Clear()
        {
            m_userTolerances = new FeatureMatcherTolerances();
            m_useEllipsoid = true;
            m_shouldCalculateShiftFDR = true;
            m_shouldCalculateSTAC = true;
            m_shouldCalculateHistogramFDR = false;
            m_shouldCalculateSLiC = true;
            m_useDriftTime = false;
            m_usePriors = true;
            m_chargeStateList = new List<int>();
            m_shiftAmount = 11.0;
            m_histogramBinWidth = 0.02;
            m_histogramMultiplier = 0.1;
        }
        #endregion
    }
}
