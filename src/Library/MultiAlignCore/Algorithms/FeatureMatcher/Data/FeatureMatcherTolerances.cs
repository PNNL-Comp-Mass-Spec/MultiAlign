using MathNet.Numerics.LinearAlgebra.Double;

namespace MultiAlignCore.Algorithms.FeatureMatcher.Data
{
    public class FeatureMatcherTolerances
    {
        #region Members
        private bool m_refined;

        private double m_massTolerancePPM;
        private double m_netTolerance;

        private float m_driftTimeTolerance;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether the tolerances have been refined.
        /// </summary>
        public bool Refined
        {
            get { return m_refined; }
            set { m_refined = value; }
        }

        /// <summary>
        /// Gets or sets the mass tolerance in ppm.
        /// </summary>
        public double MassTolerancePPM
        {
            get { return m_massTolerancePPM; }
            set { m_refined = false; m_massTolerancePPM = value; }
        }
        /// <summary>
        /// Gets or sets the NET tolerance.
        /// </summary>
        public double NETTolerance
        {
            get { return m_netTolerance; }
            set { m_refined = false; m_netTolerance = value; }
        }

        /// <summary>
        /// Gets or sets the drift time tolerance.
        /// </summary>
        public float DriftTimeTolerance
        {
            get { return m_driftTimeTolerance; }
            set { m_refined = false; m_driftTimeTolerance = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Parameterless constructor for FeatureMatcherTolerances class.
        /// </summary>
        public FeatureMatcherTolerances()
        {
            Clear();
        }
        /// <summary>
        /// Constructor for FeatureMatcherTolerances class taking in all necessary variables.
        /// </summary>
        /// <param name="massTolerancePPM">Mass tolerance in ppm.</param>
        /// <param name="netTolerance">NET tolerance.</param>
        /// <param name="driftTimeTolerance">Drift time tolerance.</param>
        public FeatureMatcherTolerances(double massTolerancePPM, double netTolerance, float driftTimeTolerance)
        {
            Clear();
            m_massTolerancePPM = massTolerancePPM;
            m_netTolerance = netTolerance;
            m_driftTimeTolerance = driftTimeTolerance;
        }
        #endregion

        #region Public functions
        /// <summary>
        /// Resets all tolerances to default values.
        /// </summary>
        public void Clear()
        {
            m_refined = false;
            m_massTolerancePPM = 6.0;
            m_netTolerance = 0.025;
            m_driftTimeTolerance = 1.0f;
        }

        /// <summary>
        /// Gets tolerances as a vector.
        /// </summary>
        /// <param name="driftTime">Whether to include the drift time tolerance.</param>
        /// <returns>A matrix of dimention 2x1 if driftTime=false (3x1 if driftTime=true), containing the tolerances.</returns>
        public DenseMatrix AsVector(bool driftTime)
        {
            DenseMatrix tolerances;
            if (driftTime)
            {
                tolerances = new DenseMatrix(3, 1);
                tolerances[0, 0] = m_massTolerancePPM;
                tolerances[1, 0] = m_netTolerance;
                tolerances[2, 0] = m_driftTimeTolerance;
            }
            else
            {
                tolerances = new DenseMatrix(2, 1);
                tolerances[0, 0] = m_massTolerancePPM;
                tolerances[1, 0] = m_netTolerance;
            }
            return (tolerances);
        }
        #endregion
    }
}
