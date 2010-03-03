using System;
using System.Collections.Generic;
using System.Text;
using MultiAlignEngine.Features;

namespace PNNLProteomics.Filters
{
    /// <summary>
    /// Filter for minimum number of members.
    /// </summary>
    public class IntensityFilter: IFilter<clsUMC>
    {
        private double m_minimumIntensity;
        private double m_maximumIntensity;

        public IntensityFilter(double maximumIntensity, double minimumIntensity)
        {
            m_maximumIntensity = maximumIntensity;
            m_minimumIntensity = minimumIntensity;
        }

        /// <summary>
        /// Gets or sets the minimum number of data members requierd in a UMC cluster.
        /// </summary>
        public double MaximumIntensity
        {
            get { return m_maximumIntensity; }
            set { m_maximumIntensity = value; }
        }
        /// <summary>
        /// Gets or sets the minimum number of data members requierd in a UMC cluster.
        /// </summary>
        public double MinimumIntensity
        {
            get { return m_minimumIntensity; }
            set { m_minimumIntensity = value; }
        }

        /// <summary>
        /// Checks to see if the UMC Cluster has at least the minimum number of members present.
        /// </summary>
        /// <param name="umcCluster"></param>
        /// <returns></returns>
        public bool DoesPassFilter(clsUMC umc)
        {            
            return (umc.AbundanceMax <= m_maximumIntensity && umc.AbundanceMax >= m_minimumIntensity);                
        }
    }
}
