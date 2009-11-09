#include "StdAfx.h"
#include ".\clsumc.h"
#include "UMCCreation\UMC.h"

namespace MultiAlignEngine
{
	namespace Features
	{
		clsUMC::clsUMC(void)
		{
			mint_cluster_index				= -1; 
			mint_umc_index					= -1; 			
			mdouble_mono_mass					=  0; 
			mdouble_mono_mass_calibrated		=  0; 
			mdouble_abundance					=  0; 			
			mdouble_class_rep_mz				=  0; 
			mshort_class_rep_charge			= -1; 
			mshort_class_highest_charge		=  0;
			marray_chargeStatesAbundances	= new double __gc[NUM_CHARGE_STATES_LENGTH];
			mint_scan						= -1; 
			mint_start_scan					= -1; 
			mint_end_scan					= -1; 
			mdouble_net						= 0; 
			mint_scan_aligned				= -1; 						
		}

		clsUMC::~clsUMC(void)
		{
		}
	}
}