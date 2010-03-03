using System;
using System.Collections.Generic;
using System.Text;
using MultiAlignEngine.Features;

namespace PNNLProteomics.Filters
{
    /// <summary>
    /// Filter for minimum number of members.
    /// </summary>
    public class SpectralCountFilter : IFilter<clsUMC>
    {
        private double m_minimumSpectralCount;
        private double m_maximumSpectralCount;

        public SpectralCountFilter(double maximumSpectralCount, double minimumSpectralCount)
        {
            m_maximumSpectralCount = maximumSpectralCount;
            m_minimumSpectralCount = minimumSpectralCount;
        }

        /// <summary>
        /// Gets or sets the minimum number of data members requierd in a UMC cluster.
        /// </summary>
        public double MaximumSpectralCount
        {
            get { return m_maximumSpectralCount; }
            set { m_maximumSpectralCount = value; }
        }
        /// <summary>
        /// Gets or sets the minimum number of data members requierd in a UMC cluster.
        /// </summary>
        public double MinimumSpectralCount
        {
            get { return m_minimumSpectralCount; }
            set { m_minimumSpectralCount = value; }
        }

        /// <summary>
        /// Checks to see if the UMC Cluster has at least the minimum number of members present.
        /// </summary>
        /// <param name="umcCluster"></param>
        /// <returns></returns>
        public bool DoesPassFilter(clsUMC umc)
        {
            return (umc.SpectralCount <= m_maximumSpectralCount && umc.SpectralCount >= m_minimumSpectralCount);
        }
    }
}
