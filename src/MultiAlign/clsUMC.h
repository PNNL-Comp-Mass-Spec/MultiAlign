#pragma once
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	namespace Features
	{
		public __gc class clsIsotopePeak
		{
			public:
				int		mint_original_index; 
				int		mint_umc_index; 
				int		mint_scan; 
				short	mshort_charge; 
				double	mdouble_abundance; 
				double	mdouble_mz; 
				float	mflt_fit; 
				double	mdouble_average_mass; 
				double	mdouble_mono_mass; 
				double	mdouble_max_abundance_mass;
				double	mdouble_i2_abundance; 
				clsIsotopePeak()
				{
				};
		}; 

		[System::Serializable]
		public __gc class clsUMC
		{
			public:
				int		mint_dataset_index; 
				int		mint_cluster_index; 
				int		mint_umc_index; 
				double	mdouble_mono_mass; 
				double	mdouble_class_rep_mz; 
				short	mshort_class_rep_charge; 
				short	mshort_class_highest_charge;
				int		mint_scan;
				int		mint_start_scan; 
				int		mint_end_scan; 
				double	mdouble_net; 
				double	mdouble_mono_mass_calibrated; 
				int		mint_scan_aligned; 
				double	mdouble_abundance; 
				double  mdouble_max_abundance; 
				double  mdouble_sum_abundance;
				double	marray_chargeStatesAbundances __gc[];

				clsUMC(void);
				~clsUMC(void);
		};
	}
}