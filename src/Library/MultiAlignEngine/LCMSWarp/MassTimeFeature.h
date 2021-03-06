#pragma once

namespace MultiAlignEngine
{
	namespace Alignment
	{
		class MassTimeFeature
		{
		public:
			double	mdouble_mono_mass; 
			double	mdouble_mono_mass_calibrated; 
			double	mdouble_mono_mass_original; 
			double	mdouble_mz; 
			double	mdouble_net; 
			double	mdouble_abundance; 
			double	mdouble_aligned_net; 
			double  mdouble_driftTime;
			int		mint_id; 
			int		mint_conformerID;

			MassTimeFeature(void);
			MassTimeFeature(const MassTimeFeature &cpy)
			{
				mdouble_mono_mass				= cpy.mdouble_mono_mass; 
				mdouble_net						= cpy.mdouble_net; 
				mint_id							= cpy.mint_id; 
				mdouble_abundance				= cpy.mdouble_abundance; 
				mdouble_aligned_net				= cpy.mdouble_aligned_net; 
				mdouble_mono_mass_calibrated	= cpy.mdouble_mono_mass_calibrated; 
				mdouble_mono_mass_original		= cpy.mdouble_mono_mass_original;
				mdouble_mz						= cpy.mdouble_mz; 
				mdouble_driftTime				= cpy.mdouble_driftTime;
				mint_conformerID				= cpy.mint_conformerID;
			}

			MassTimeFeature& operator=(const MassTimeFeature &cpy)
			{
				mdouble_mono_mass			 = cpy.mdouble_mono_mass; 
				mdouble_net					 = cpy.mdouble_net; 
				mint_id						 = cpy.mint_id; 
				mdouble_abundance			 = cpy.mdouble_abundance; 
				mdouble_aligned_net			 = cpy.mdouble_aligned_net; 
				mdouble_mono_mass_calibrated = cpy.mdouble_mono_mass_calibrated; 
				mdouble_mono_mass_original	 = cpy.mdouble_mono_mass_original;
				mdouble_mz					 = cpy.mdouble_mz;
				mdouble_driftTime			 = cpy.mdouble_driftTime; 
				mint_conformerID		  	 = cpy.mint_conformerID;
				return *this; 
			}
			~MassTimeFeature(void);
		};
		bool SortFeaturesByMass(MassTimeFeature &a, MassTimeFeature &b); 
	}
}