using System;
using System.Collections.Generic;
using System.Text;
using MultiAlignEngine.Features;

namespace PNNLProteomics.Filters
{
    /// <summary>
    /// Filter for minimum number of members.
    /// </summary>
    public class NETFilter : IFilter<clsUMC>, IFilter<clsCluster>
    {
        private double m_minimumNET;
        private double m_maximumNET;

        public NETFilter(double maximumNET, double minimumNET)
        {
            m_maximumNET = maximumNET;
            m_minimumNET = minimumNET;
        }

        /// <summary>
        /// Gets or sets the minimum number of data members requierd in a UMC cluster.
        /// </summary>
        public double MaximumNET
        {
            get { return m_maximumNET; }
            set { m_maximumNET = value; }
        }
        /// <summary>
        /// Gets or sets the minimum number of data members requierd in a UMC cluster.
        /// </summary>
        public double MinimumNET
        {
            get { return m_minimumNET; }
            set { m_minimumNET = value; }
        }

        /// <summary>
        /// Checks to see if the UMC Cluster has at least the minimum number of members present.
        /// </summary>
        /// <param name="umcCluster"></param>
        /// <returns></returns>
        public bool DoesPassFilter(clsUMC umc)
        {
            return (umc.Net <= m_maximumNET && umc.Net >= m_minimumNET);
        }

        /// <summary>
        /// Checks to see if the UMC Cluster has at least the minimum number of members present.
        /// </summary>
        /// <param name="umcCluster"></param>
        /// <returns></returns>
        public bool DoesPassFilter(clsCluster umc)
        {
            return (umc.NetAligned <= m_maximumNET && umc.NetAligned >= m_minimumNET);
        }
    }
}
