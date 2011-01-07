
#using	 <mscorlib.dll>
#include "clsUMCCreator.h"

namespace MultiAlignEngine
{
	namespace Features
	{
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		clsUMCCreator::clsUMCCreator()
		{
			mobjUMCFindingOptions	 = new clsUMCFindingOptions(); 
			mobj_umc_creator		 = new MultiAlignEngine::UMCCreation::UMCCreator(); 
			mint_highestChargeState	 = 0;
			menum_abundanceReportingType = enmAbundanceReportingType::PeakMax;
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		clsUMCCreator::~clsUMCCreator()
		{
			if (mobj_umc_creator != NULL)
				delete mobj_umc_creator; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCCreator::ResetStatus()
		{
			menm_state = IDLE; 
			mobj_umc_creator->Reset(); 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCCreator::FindUMCs()
		{
			mobj_umc_creator->SetOptions(	mobjUMCFindingOptions->get_MonoMassWeight(), 
											mobjUMCFindingOptions->get_AveMassWeight(),
											mobjUMCFindingOptions->get_LogAbundanceWeight(), 
											mobjUMCFindingOptions->get_ScanWeight(), 
											mobjUMCFindingOptions->get_FitWeight(), 
											mobjUMCFindingOptions->get_NETWeight(),
											mobjUMCFindingOptions->get_ConstraintMonoMass(),
											mobjUMCFindingOptions->get_ConstraintAveMass(),
											mobjUMCFindingOptions->get_MaxDistance(),
											mobjUMCFindingOptions->get_UseNET(),
											mobjUMCFindingOptions->get_IsotopicFitFilter(),
											mobjUMCFindingOptions->get_IsotopicIntensityFilter(),
											mobjUMCFindingOptions->get_UseIsotopicFitFilter(),
											mobjUMCFindingOptions->get_IsIsotopicFitFilterInverted(),
											mobjUMCFindingOptions->get_UseIsotopicIntensityFilter()											
											); 

			menm_state		= MultiAlignEngine::Features::enmState::FILTERING;			
			mstr_message	= new System::String(S"Creating UMC's - Filtering UMC's");

			menm_state		= MultiAlignEngine::Features::enmState::CLUSTERING; 
			mstr_message	= new System::String(S"Creating UMC's - Clustering Isotope Peaks"); 
			mobj_umc_creator->CreateUMCsSinglyLinkedWithAll(); 

			/*
			menm_state		= MultiAlignEngine::Features::enmState::CLUSTERING; 
			mstr_message	= new System::String(S"Creating UMC's - Clustering Isotope Peaks"); 
			mobj_umc_creator->CreateUMCsSinglyLinkedWithAll(); 
			*/
			
			menm_state		= SUMMARIZING; 
			mstr_message	= new System::String(S"Filtering out short isotopic clusters for UMC's"); 
			mobj_umc_creator->RemoveShortUMCs(mobjUMCFindingOptions->get_MinUMCLength());

			mstr_message	= new System::String(S"Calculating UMC statistics"); 
			mobj_umc_creator->CalculateUMCs();

			menm_state		= COMPLETE; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void GetStr(System::String *src, char *dest)
		{
			if (src == 0 || src == "" || src->get_Length() == 0)
			{
				dest[0] = '\0'; 
				return; 
			}

			int len = src->get_Length(); 
			for (int i = 0; i < len; i++)
			{
				dest[i] = (char) src->Chars[i]; 
			}
			dest[len] = '\0'; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCCreator::LoadUMCs(bool is_pek_file)
		{
			Console::WriteLine(S"Loading UMCs"); 
			char file_name[512]; 
			GetStr(mstr_file_name, file_name); 
			menm_state = LOADING;
			
			bool truesx;
			truesx = false;

			if (is_pek_file)
			{
				mstr_message = new System::String(S"Loading PEK file"); 
				mobj_umc_creator->ReadPekFileMemoryMapped(file_name); 
			}
			else
			{
				mstr_message = new System::String("Loading CSV file"); 
				mobj_umc_creator->ReadCSVFile(file_name); 
			}			
			
			mstr_message = String::Format("Loaded {0} Isotopic Peaks.  Filtered to {1} Isotopic Peaks", 
										__box(mobj_umc_creator->mint_numberOfPeaks),
										__box(mobj_umc_creator->mint_featuresFound));

			mint_numberOfPeaksFound = mobj_umc_creator->mint_numberOfPeaks;
			menm_state = LOADED; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCCreator::LoadFindUMCs(bool is_pek_file)
		{
			mobj_umc_creator->SetOptions(	mobjUMCFindingOptions->get_MonoMassWeight(), 
											mobjUMCFindingOptions->get_AveMassWeight(),
											mobjUMCFindingOptions->get_LogAbundanceWeight(), 
											mobjUMCFindingOptions->get_ScanWeight(), 
											mobjUMCFindingOptions->get_FitWeight(), 
											mobjUMCFindingOptions->get_NETWeight(),
											mobjUMCFindingOptions->get_ConstraintMonoMass(),
											mobjUMCFindingOptions->get_ConstraintAveMass(),
											mobjUMCFindingOptions->get_MaxDistance(),
											mobjUMCFindingOptions->get_UseNET(),
											mobjUMCFindingOptions->get_IsotopicFitFilter(),
											mobjUMCFindingOptions->get_IsotopicIntensityFilter(),
											mobjUMCFindingOptions->get_UseIsotopicFitFilter(),
											mobjUMCFindingOptions->get_IsIsotopicFitFilterInverted(),
											mobjUMCFindingOptions->get_UseIsotopicIntensityFilter()											
											); 
	
			char file_name[512]; 
			GetStr(mstr_file_name, file_name); 
			
			///
			/// Load the Decon2ls or ICR2ls output
			///
			menm_state	= LOADING;
			if (is_pek_file)
			{
				mstr_message		= new System::String(S"Loading PEK file"); 
				mobj_umc_creator->ReadPekFileMemoryMapped(file_name); 
			}
			else
			{
				mstr_message		= new System::String("Loading CSV file"); 
				mobj_umc_creator->ReadCSVFile(file_name); 
			}

			mstr_message = String::Format("Loaded {0} Isotopic Peaks.  Filtered to {1} Isotopic Peaks", 
										__box(mobj_umc_creator->mint_numberOfPeaks),
										__box(mobj_umc_creator->mint_featuresFound));

			mint_numberOfPeaksFound = mobj_umc_creator->mint_numberOfPeaks;
			menm_state				= CLUSTERING; 
			mstr_message			= new System::String("Creating UMC's - Clustering Isotope Peaks"); 
			mobj_umc_creator->CreateUMCsSinglyLinkedWithAll(); 

			menm_state				= SUMMARIZING; 
			mstr_message			= new System::String("Filtering out short isotopic clusters for UMC's"); 
			mobj_umc_creator->RemoveShortUMCs(mobjUMCFindingOptions->get_MinUMCLength());
			mstr_message			= new System::String("Calculating UMC statistics"); 
			mobj_umc_creator->CalculateUMCs();
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCCreator::LoadFindUMCsPEK()
		{
			LoadFindUMCs(true); 
			menm_state = COMPLETE; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCCreator::LoadFindUMCsCSV()
		{
			LoadFindUMCs(false); 
			menm_state = COMPLETE; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		int clsUMCCreator::GetUmcMapping(int (&isotope_peaks_index) __gc[], int (&umc_index) __gc[])
		{
			int numMappings = mobj_umc_creator->mmultimap_umc_2_peak_index.size(); 
			isotope_peaks_index = new int __gc [numMappings]; 
			umc_index = new int __gc [numMappings]; 

			int mappingNum = 0; 
			for (std::multimap<int,int>::iterator iter = mobj_umc_creator->mmultimap_umc_2_peak_index.begin(); iter != mobj_umc_creator->mmultimap_umc_2_peak_index.end(); iter++)
			{
				int currentUmcNum				= (*iter).first; 
				int pkIndex						= (*iter).second; 
				isotope_peaks_index[mappingNum] = pkIndex; 
				umc_index[mappingNum]			= currentUmcNum; 

				mappingNum++; 
			}
			return numMappings; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		int clsUMCCreator::GetUmcMappingOriginalIndex(int (&isotope_peaks_original_index) __gc[], int (&umc_index) __gc[])
		{
			int numMappings = mobj_umc_creator->mmultimap_umc_2_peak_index.size(); 
			isotope_peaks_original_index = new int __gc [numMappings]; 
			umc_index = new int __gc [numMappings]; 

			int mappingNum = 0; 
			for (std::multimap<int,int>::iterator iter = mobj_umc_creator->mmultimap_umc_2_peak_index.begin(); iter != mobj_umc_creator->mmultimap_umc_2_peak_index.end(); iter++)
			{
				int currentUmcNum								= (*iter).first; 
				int pkIndex										= (*iter).second; 

				MultiAlignEngine::UMCCreation::IsotopePeak pk	= mobj_umc_creator->mvect_isotope_peaks[pkIndex]; 
				isotope_peaks_original_index[mappingNum]		= pk.mint_original_index; 
				umc_index[mappingNum]							= currentUmcNum; 

				mappingNum++; 
			}
			return numMappings; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		MultiAlignEngine::Features::clsUMC* clsUMCCreator::GetUMCs()[]
		{
			int numUmcs = mobj_umc_creator->GetNumUmcs(); 
			MultiAlignEngine::Features::clsUMC* arr_umcs __gc[]; 
			arr_umcs = new MultiAlignEngine::Features::clsUMC* __gc [numUmcs]; 
			
			mint_highestChargeState = 0;
			for (int umcNum = 0; umcNum < numUmcs; umcNum++)
			{
				MultiAlignEngine::UMCCreation::UMC umc		= mobj_umc_creator->mvect_umcs[umcNum]; 
				MultiAlignEngine::Features::clsUMC *newUmc	= __gc new MultiAlignEngine::Features::clsUMC();

				/// Determines how to set the abundance type
				newUmc->mdouble_abundance					= umc.mdouble_max_abundance; 
				if (menum_abundanceReportingType == enmAbundanceReportingType::PeakArea)
				{
					newUmc->mdouble_abundance				= umc.mdouble_sum_abundance;  
				}
				newUmc->mlong_sum_abundance					= umc.mdouble_sum_abundance;
				newUmc->mdouble_max_abundance			    = umc.mdouble_max_abundance;	
				newUmc->mdouble_class_rep_mz				= System::Convert::ToDouble(umc.mdouble_class_rep_mz); 
				newUmc->mdouble_mono_mass					= System::Convert::ToDouble(umc.mdouble_median_mono_mass); 
				newUmc->mdouble_mono_mass_calibrated		= newUmc->mdouble_mono_mass; 
				newUmc->mint_scan							= umc.mint_max_abundance_scan; 
				newUmc->mint_start_scan						= umc.mint_start_scan; 
				newUmc->mint_end_scan						= umc.mint_stop_scan; 
				newUmc->mdouble_net							= System::Convert::ToDouble(umc.mint_max_abundance_scan); 
				newUmc->mshort_class_highest_charge			= umc.mshort_class_highest_charge;
				newUmc->SpectralCount						= umc.min_num_members;

				/// Find the highest Charge State
				mint_highestChargeState = Math::Max(mint_highestChargeState, Convert::ToInt32(newUmc->mshort_class_highest_charge));

				/// Compute Generic NET value
				if (mobj_umc_creator->mint_max_scan > mobj_umc_creator->mint_min_scan) 
				{					
					newUmc->mdouble_net = (double) (umc.mint_max_abundance_scan - mobj_umc_creator->mint_min_scan) * 1.0 / (mobj_umc_creator->mint_max_scan - mobj_umc_creator->mint_min_scan); 
				}

				newUmc->mint_scan_aligned = umc.mint_max_abundance_scan; 

				newUmc->mint_umc_index				= umc.mint_umc_index; 
				newUmc->mshort_class_rep_charge		= (int) umc.mshort_class_rep_charge; 
				newUmc->mshort_class_highest_charge = (int) umc.mshort_class_highest_charge;
				arr_umcs[umcNum]					= newUmc; 
			}

			return arr_umcs; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		MultiAlignEngine::Features::clsIsotopePeak* clsUMCCreator::GetIsotopePeaks()[]
		{
			int numIsotopePeaks =  this->mobj_umc_creator->mvect_isotope_peaks.size(); 
			MultiAlignEngine::Features::clsIsotopePeak* arr_isotope_peaks __gc[]; 
			arr_isotope_peaks = new MultiAlignEngine::Features::clsIsotopePeak* __gc [numIsotopePeaks]; 
			
			for (int pkNum = 0; pkNum < numIsotopePeaks; pkNum++)
			{
				MultiAlignEngine::UMCCreation::IsotopePeak pk		= mobj_umc_creator->mvect_isotope_peaks[pkNum]; 
				MultiAlignEngine::Features::clsIsotopePeak *newPk	= __gc new MultiAlignEngine::Features::clsIsotopePeak();
				newPk->mdouble_abundance							= pk.mdouble_abundance; 
				newPk->mdouble_average_mass							= pk.mdouble_average_mass; 
				newPk->mdouble_i2_abundance							= pk.mdouble_i2_abundance; 
				newPk->mdouble_max_abundance_mass					= pk.mdouble_max_abundance_mass; 
				newPk->mdouble_mono_mass							= pk.mdouble_mono_mass; 
				newPk->mdouble_mz									= pk.mdouble_mz; 
				newPk->mflt_fit										= pk.mflt_fit; 
				newPk->mint_original_index							= pk.mint_original_index; 
				newPk->mint_scan									= pk.mint_scan; 
				newPk->mint_umc_index								= pk.mint_umc_index; 
				newPk->mshort_charge								= pk.mshort_charge; 
				arr_isotope_peaks[pkNum]							= newPk; 
			}

			return arr_isotope_peaks; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCCreator::SetMinMaxScans(int min, int max)
		{
			mobj_umc_creator->SetMinMaxScan(min, max); 

			if (max <= min)
				throw new Exception("Max scan must be greater than min scan");
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCCreator::SetIsotopePeaks(MultiAlignEngine::Features::clsIsotopePeak* (&isotope_peaks) __gc[])
		{
			std::vector<MultiAlignEngine::UMCCreation::IsotopePeak> vectPeaks;
			int numPeaks = isotope_peaks->Length; 
			vectPeaks.reserve(numPeaks); 

			MultiAlignEngine::Features::clsIsotopePeak *isoPk; 
			for (int pkNum = 0; pkNum < numPeaks; pkNum++)
			{
				MultiAlignEngine::UMCCreation::IsotopePeak pk; 
				isoPk												= isotope_peaks[pkNum]; 
				pk.mint_original_index								= isoPk->mint_original_index; 
				pk.mint_umc_index									= isoPk->mint_umc_index; 
				pk.mint_scan										= isoPk->mint_scan; 
				pk.mshort_charge									= isoPk->mshort_charge; 
				pk.mdouble_abundance								= isoPk->mdouble_abundance; 
				pk.mdouble_mz										= isoPk->mdouble_mz; 
				pk.mflt_fit											= isoPk->mflt_fit; 
				pk.mdouble_average_mass								= isoPk->mdouble_average_mass; 
				pk.mdouble_mono_mass								= isoPk->mdouble_mono_mass; 
				pk.mdouble_max_abundance_mass						= isoPk->mdouble_max_abundance_mass;
				pk.mdouble_i2_abundance								= isoPk->mdouble_i2_abundance; 
				vectPeaks.push_back(pk); 
			}
			mobj_umc_creator->SetPeks(vectPeaks); 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCCreator::SetOptions(	float wt_mono_mass, 
										float wt_avg_mass, 
										float wt_log_abundance,
										float wt_scan, 
										float wt_fit,
										float wt_net,
										float mono_constraint,
										float avg_constraint,
										double max_dist, 
										bool use_net)
		{
			
			
			mobj_umc_creator->SetOptions(	mobjUMCFindingOptions->get_MonoMassWeight(), 
											mobjUMCFindingOptions->get_AveMassWeight(),
											mobjUMCFindingOptions->get_LogAbundanceWeight(), 
											mobjUMCFindingOptions->get_ScanWeight(), 
											mobjUMCFindingOptions->get_FitWeight(), 
											mobjUMCFindingOptions->get_NETWeight(),
											mobjUMCFindingOptions->get_ConstraintMonoMass(),
											mobjUMCFindingOptions->get_ConstraintAveMass(),
											mobjUMCFindingOptions->get_MaxDistance(),
											mobjUMCFindingOptions->get_UseNET(),
											mobjUMCFindingOptions->get_IsotopicFitFilter(),
											mobjUMCFindingOptions->get_IsotopicIntensityFilter(),
											mobjUMCFindingOptions->get_UseIsotopicFitFilter(),
											mobjUMCFindingOptions->get_IsIsotopicFitFilterInverted(),
											mobjUMCFindingOptions->get_UseIsotopicIntensityFilter() 
											); 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCCreator::SetOptions()
		{
			mobj_umc_creator->SetOptions(	mobjUMCFindingOptions->get_MonoMassWeight(), 
											mobjUMCFindingOptions->get_AveMassWeight(),
											mobjUMCFindingOptions->get_LogAbundanceWeight(), 
											mobjUMCFindingOptions->get_ScanWeight(), 
											mobjUMCFindingOptions->get_FitWeight(), 
											mobjUMCFindingOptions->get_NETWeight(),
											mobjUMCFindingOptions->get_ConstraintMonoMass(),
											mobjUMCFindingOptions->get_ConstraintAveMass(),
											mobjUMCFindingOptions->get_MaxDistance(),
											mobjUMCFindingOptions->get_UseNET(),
											mobjUMCFindingOptions->get_IsotopicFitFilter(),
											mobjUMCFindingOptions->get_IsotopicIntensityFilter(),
											mobjUMCFindingOptions->get_UseIsotopicFitFilter(),
											mobjUMCFindingOptions->get_IsIsotopicFitFilterInverted(),
											mobjUMCFindingOptions->get_UseIsotopicIntensityFilter()											
											); 
		}
	}
}
