namespace MultiAlignCore.Algorithms.FeatureMatcher.Data
{
    public class SLiCInformation
    {
        #region Members
        private double m_massPPMStDev;
        private double m_netStDev;

        private float m_driftTimeStDev;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the standard deviation of the mass errors in ppm.
        /// </summary>
        public double MassPPMStDev
        {
            get { return m_massPPMStDev; }
            set { m_massPPMStDev = value; }
        }
        /// <summary>
        /// Gets or sets the standard deviation of the NET errors.
        /// </summary>
        public double NETStDev
        {
            get { return m_netStDev; }
            set { m_netStDev = value; }
        }

        /// <summary>
        /// Gets or sets the standard deviation of the drift time errors.
        /// </summary>
        public float DriftTimeStDev
        {
            get { return m_driftTimeStDev; }
            set { m_driftTimeStDev = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor for SLiCInformation class.
        /// </summary>
        public SLiCInformation()
        {
            Clear();
        }
        #endregion

        #region Public functions
        /// <summary>
        /// Reset all SLiC parameters to default values.
        /// </summary>
        public void Clear()
        {
            m_massPPMStDev = 3.0;
            m_netStDev = 0.015;
            m_driftTimeStDev = 0.5f;
        }
        #endregion
    }
}
