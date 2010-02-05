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

		public bool DoesPassFilter(clsUMC umc)
		{
			return testMassRange(umc.mdouble_mono_mass);
		}

		public bool DoesPassFilter(clsCluster umcCluster)
		{
			return testMassRange(umcCluster.mdouble_mass);
		}

		private bool testMassRange(double massValue)
		{
			return (massValue >= m_minMass && massValue <= m_maxMass);
		}
	}
}
