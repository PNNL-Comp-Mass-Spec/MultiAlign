#include "StdAfx.h"
#include ".\MassTimeFeature.h"

namespace MultiAlignEngine
{
	namespace Alignment
	{
		MassTimeFeature::MassTimeFeature(void)
		{
			mdouble_mono_mass = 0.0; 
			mdouble_mono_mass_calibrated = 0.0; 
			mdouble_net = 0; 
			mint_id = -1; 
			mdouble_abundance = 0; 
			mdouble_aligned_net = -1; 
		}

		MassTimeFeature::~MassTimeFeature(void)
		{
		}
		bool SortFeaturesByMass(MassTimeFeature &a, MassTimeFeature &b)
		{
			/*if (a.mdouble_mono_mass < b.mdouble_mono_mass)
				return true; 
			if (a.mdouble_mono_mass > b.mdouble_mono_mass)
				return false; 
			return a.mdouble_net < b.mdouble_net;*/
			return (a.mdouble_mono_mass < b.mdouble_mono_mass);
		}
	}
}