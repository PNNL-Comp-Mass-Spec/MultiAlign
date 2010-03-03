using System;
using System.Collections.Generic;

using MultiAlignEngine.Features;

namespace PNNLProteomics.Filters
{
    /// <summary>
    /// Filter for minimum number of members.
    /// </summary>
    public class DriftTimeFilter : IFilter<clsUMC>
    {
        private double m_minimumDriftTime;
        private double m_maximumDriftTime;

        public DriftTimeFilter(double maximumDriftTime, double minimumDriftTime)
        {
            m_maximumDriftTime = maximumDriftTime;
            m_minimumDriftTime = minimumDriftTime;
        }

        /// <summary>
        /// Gets or sets the minimum number of data members requierd in a UMC cluster.
        /// </summary>
        public double MaximumDriftTime
        {
            get { return m_maximumDriftTime; }
            set { m_maximumDriftTime = value; }
        }
        /// <summary>
        /// Gets or sets the minimum number of data members requierd in a UMC cluster.
        /// </summary>
        public double MinimumDriftTime
        {
            get { return m_minimumDriftTime; }
            set { m_minimumDriftTime = value; }
        }

        /// <summary>
        /// Checks to see if the UMC Cluster has at least the minimum number of members present.
        /// </summary>
        /// <param name="umcCluster"></param>
        /// <returns></returns>
        public bool DoesPassFilter(clsUMC umc)
        {            
            return (umc.DriftTime <= m_maximumDriftTime && umc.DriftTime >= m_minimumDriftTime);
        }
    }
}
