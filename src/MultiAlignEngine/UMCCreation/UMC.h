#pragma once

#define NUM_CHARGE_STATES_LENGTH 10

namespace MultiAlignEngine
{
	namespace UMCCreation
	{
		class UMC
		{
		public:
			/// Masses
			double	mdouble_min_mono_mass; 
			double	mdouble_max_mono_mass; 
			double	mdouble_average_mono_mass; 
			double	mdouble_median_mono_mass; 

			/// Abundances
			double	mdouble_max_abundance;
			double	mdouble_sum_abundance; 

			/// Scans
			int		min_num_members; 
			int		mint_start_scan; 
			int		mint_stop_scan; 
			int		mint_max_abundance_scan; 

			/// Charge State and M/Z
			double	mdouble_class_rep_mz;
			short	mshort_class_rep_charge; 
			short	mshort_class_highest_charge;
			double	marray_chargeStateAbundances[NUM_CHARGE_STATES_LENGTH];

			/// Indexing
			int		mint_umc_index; 

			UMC(void);
			~UMC(void);
		};
	}
}