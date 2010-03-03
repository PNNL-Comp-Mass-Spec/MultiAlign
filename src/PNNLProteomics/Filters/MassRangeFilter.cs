using System;
using System.Collections.Generic;
using System.Text;
using PNNLProteomics.MultiAlign.Hibernate.Domain;
using MultiAlignEngine.Features;

namespace PNNLProteomics.Filters
{
	public class MassRangeFilter : IFilter<clsUMC>, IFilter<clsCluster>
	{
		private double m_minMass;
		private double m_maxMass;

		public MassRangeFilter(double lesserValue, double greaterValue)
		{
			m_minMass = Math.Min(lesserValue, greaterValue);
			m_maxMass = Math.Max(lesserValue, greaterValue);
		}

        public double MinimumMass
        {
            get
            {
                return m_minMass;
            }
            set
            {
                m_minMass = value;
            }
        }
        public double MaximumMass
        {
            get
            {
                return m_maxMass;
            }
            set
            {
                m_maxMass = value;
            }
        }

		public bool DoesPassFilter(clsUMC umc)
		{
			return testMassRange(umc.mdouble_mono_mass_calibrated);
		}

		public bool DoesPassFilter(clsCluster umcCluster)
		{
			return testMassRange(umcCluster.mdouble_mass_calibrated);
		}

		private bool testMassRange(double massValue)
		{
			return (massValue >= m_minMass && massValue <= m_maxMass);
		}
	}
}
