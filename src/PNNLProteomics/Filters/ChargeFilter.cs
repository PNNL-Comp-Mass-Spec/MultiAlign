using System;
using System.Collections.Generic;
using System.Text;
using MultiAlignEngine.Features;

namespace PNNLProteomics.Filters
{
    /// <summary>
    /// Filter for minimum number of members.
    /// </summary>
    public class ChargeFilter : IFilter<clsUMC>, IFilter<clsCluster>
    {
        private double m_minimumCharge;
        private double m_maximumCharge;

        public ChargeFilter(double maximumCharge, double minimumCharge)
        {
            m_maximumCharge = maximumCharge;
            m_minimumCharge = minimumCharge;
        }

        /// <summary>
        /// Gets or sets the minimum number of data members requierd in a UMC cluster.
        /// </summary>
        public double MaximumCharge
        {
            get { return m_maximumCharge; }
            set { m_maximumCharge = value; }
        }
        /// <summary>
        /// Gets or sets the minimum number of data members requierd in a UMC cluster.
        /// </summary>
        public double MinimumCharge
        {
            get { return m_minimumCharge; }
            set { m_minimumCharge = value; }
        }

        /// <summary>
        /// Checks to see if the UMC Cluster has at least the minimum number of members present.
        /// </summary>
        /// <param name="umcCluster"></param>
        /// <returns></returns>
        public bool DoesPassFilter(clsUMC umc)
        {
            return (umc.ChargeRepresentative <= m_maximumCharge && umc.ChargeRepresentative >= m_minimumCharge);
        }
        /// <summary>
        /// Checks to see if the UMC Cluster has at least the minimum number of members present.
        /// </summary>
        /// <param name="umcCluster"></param>
        /// <returns></returns>
        public bool DoesPassFilter(clsCluster umc)
        {
            return (umc.Charge <= m_maximumCharge && umc.Charge >= m_minimumCharge);
        }
    }
}
