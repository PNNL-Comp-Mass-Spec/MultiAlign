// clsUMCCreator.h
#pragma once
#using <mscorlib.dll>
using namespace System;

#include "clsUMCFindingOptions.h"
#include "UMCCreation/UMCCreator.h"
#include "clsUMCData.h"

namespace MultiAlignEngine
{
	namespace Features
	{
		public __value enum enmState { IDLE = 0, LOADING, LOADED, FILTERING, CLUSTERING, SUMMARIZING, FAILED, COMPLETE }; 
		[Serializable]
		public __gc class clsUMCCreator
		{
			int mint_percent_done; 
			int mint_highestChargeState;
			int mint_numberOfPeaksFound;
			enmAbundanceReportingType menum_abundanceReportingType;

			System::String *mstr_message; 
			System::String *mstr_file_name; 
			enmState menm_state; 
			
			clsUMCFindingOptions *mobjUMCFindingOptions; 
			[System::NonSerialized]MultiAlignEngine::UMCCreation::UMCCreator __nogc *mobj_umc_creator; 
			void LoadFindUMCs(bool is_pek_file); 
			void SetOptions();
		

		public:
			clsUMCCreator(); 
			~clsUMCCreator(); 

			void FindUMCs();
			void LoadUMCs(bool is_pek_file); 
			void LoadFindUMCsPEK(); 
			void LoadFindUMCsCSV(); 
			void ResetStatus(); 

			void SetIsotopePeaks(MultiAlignEngine::Features::clsIsotopePeak* (&isotope_peaks) __gc[]); 
			int  GetUmcMapping(	int (&isotope_peaks_index) __gc[], 
								int (&umc_index) __gc[]); 
			int  GetUmcMappingOriginalIndex(	int (&isotope_peaks_original_index) __gc[],
												int (&umc_index) __gc[]); 
			void SetMinMaxScans(int min, 
								int max); 
			void SetOptions(float wt_mono_mass, 
							float wt_avg_mass, 
							float wt_log_abundance,
							float wt_scan, 
							float wt_fit,
							float wt_net,
							float mono_constraint,
							float avg_constraint,
							double max_dist, 
							bool use_net); 

			MultiAlignEngine::Features::clsUMC* GetUMCs()[]; 
			MultiAlignEngine::Features::clsIsotopePeak* GetIsotopePeaks()[]; 


			__property short get_PercentComplete()
			{
				switch (menm_state)
				{
					case enmState::LOADING:
						if (mobj_umc_creator != NULL)
							return mobj_umc_creator->GetPercentComplete(); 
						return 0; 
						break; 
					case enmState::CLUSTERING:
						if (mobj_umc_creator != NULL)
							return mobj_umc_creator->GetPercentComplete(); 
						return 0; 
						break; 
					case enmState::SUMMARIZING:
						if (mobj_umc_creator != NULL)
							return mobj_umc_creator->GetPercentComplete(); 
						return 0; 
						break; 
					case enmState::COMPLETE:
						return 100; 
						break; 
				}
				return 0; 
			}			
			// Gets the UMC abundance reporting type
			__property enmAbundanceReportingType get_UMCAbundanceReportingType()
			{
				return menum_abundanceReportingType;
			}		
			// Sets the UMC abundance reporting type.
			__property void set_UMCAbundanceReportingType(enmAbundanceReportingType value)
			{
				menum_abundanceReportingType = value;
			}
			__property int get_IsotopicPeaksFound()
			{
				return mint_numberOfPeaksFound;
			}						
			__property int get_HighestChargeState()
			{
				return mint_highestChargeState;
			}
			__property clsUMCFindingOptions* get_UMCFindingOptions()
			{
				return mobjUMCFindingOptions; 
			}
			__property void set_UMCFindingOptions(clsUMCFindingOptions *ptr)
			{
				mobjUMCFindingOptions = ptr;
				if (mobj_umc_creator != 0)
				{
					menum_abundanceReportingType = ptr->get_UMCAbundanceReportingType();
					SetOptions();
				}
			}
			__property System::String* get_FileName()
			{
				return mstr_file_name; 
			}
			__property void set_FileName(System::String* fileName)
			{
				mstr_file_name = fileName; 
			}
			__property System::String* get_StatusMessage()
			{
				return mstr_message;
			}
			__property enmState get_State()
			{
				return menm_state;
			}
		};
	}
}

