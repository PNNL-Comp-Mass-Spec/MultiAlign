#pragma once
#include "IsotopePeak.h" 
#include <vector> 
#include <map> 
#include <math.h> 
#include <float.h> 
#include "UMC.h" 

namespace MultiAlignEngine
{
	namespace UMCCreation
	{
		class UMCCreator
		{
			float	mflt_wt_mono_mass; 
			float	mflt_wt_average_mass; 
			float	mflt_wt_log_abundance; 
			float	mflt_wt_scan; 
			float	mflt_wt_fit; 
			float	mflt_wt_net; 
			float	mflt_constraint_mono_mass; 
			float	mflt_constraint_average_mass; 

			double	mdouble_max_distance; 
			short	mshort_percent_complete; 
			bool	mbln_use_net; 

			double	mdouble_fitFilter;
			double	mdouble_intensityFilter;
			bool	mbool_invertFitFilter;
			bool	mbool_useFitFilter;
			bool	mbool_useIntensityFilter;
	

			/// Determines whether peak passes filtering criteria 
			bool UMCCreator::FilterIsotopicPeak(IsotopePeak *peak);

		public:
			int mint_min_scan; 
			int mint_max_scan;
			int mint_numberOfPeaks;
			int mint_featuresFound;

			std::multimap<int, int> mmultimap_umc_2_peak_index; 
			std::vector<IsotopePeak> mvect_isotope_peaks; 
			std::vector<int> mvect_umc_num_members; 
			std::vector<UMC> mvect_umcs; 

			UMCCreator(void);
			~UMCCreator(void);

			short GetPercentComplete()
			{ 
				return mshort_percent_complete; 
			}

			inline double PeakDistance(IsotopePeak &a, IsotopePeak &b) 
			{
				if ((a.mdouble_mono_mass - b.mdouble_mono_mass) * mflt_wt_mono_mass > mflt_constraint_mono_mass
					|| (a.mdouble_average_mass - b.mdouble_average_mass) * mflt_wt_average_mass > mflt_constraint_average_mass)
				{
					return DBL_MAX; 
				}

				double a_log_abundance	= log10(a.mdouble_abundance); 
				double b_log_abundance	= log10(b.mdouble_abundance); 
				double ppm				= (a.mdouble_mono_mass - b.mdouble_mono_mass) / a.mdouble_mono_mass * 1000000; 
				double ppmAvg			= (a.mdouble_average_mass - b.mdouble_average_mass) / a.mdouble_average_mass * 1000000; 
				if (!mbln_use_net)
				{
					double sqrDist = ppm * ppm * mflt_wt_mono_mass * mflt_wt_mono_mass ; 
					sqrDist += ppmAvg * ppmAvg * mflt_wt_average_mass * mflt_wt_average_mass; 
					sqrDist += (a_log_abundance - b_log_abundance) * (a_log_abundance - b_log_abundance) * mflt_wt_log_abundance * mflt_wt_log_abundance; 
					sqrDist += (a.mint_scan - b.mint_scan) * (a.mint_scan - b.mint_scan) * mflt_wt_scan * mflt_wt_scan; 
					sqrDist += (a.mflt_fit - b.mflt_fit) * (a.mflt_fit - b.mflt_fit) * mflt_wt_fit * mflt_wt_fit; 
					return sqrt(sqrDist); 
				}
				else
				{
					double sqrDist = ppm * ppm * mflt_wt_mono_mass * mflt_wt_mono_mass ; 
					//sqrDist += ppmAvg * ppmAvg * mflt_wt_average_mass * mflt_wt_average_mass; 
					sqrDist += (a_log_abundance - b_log_abundance) * (a_log_abundance - b_log_abundance) * mflt_wt_log_abundance * mflt_wt_log_abundance; 
					// Convert scan difference to Generic NET
					double net_distance = (a.mint_scan - b.mint_scan) * 1.0 / (mint_max_scan - mint_min_scan); 
					sqrDist += net_distance * net_distance * mflt_wt_net * mflt_wt_net; 
					sqrDist += (a.mflt_fit - b.mflt_fit) * (a.mflt_fit - b.mflt_fit) * mflt_wt_fit * mflt_wt_fit; 
					return sqrt(sqrDist); 
				}
			}
			int GetNumUmcs()
			{
				return mvect_umcs.size(); 
			}; 
			
			
			void ReadCSVFile(char *fileName); 
			void ReadPekFileMemoryMapped(char *fileName); 
			void ReadPekFile(char *fileName); 
			void CreateUMCsSinglyLinkedWithAll();
			void RemoveShortUMCs(int min_length); 
			void CalculateUMCs(); 
			void PrintPeaks(); 
			void PrintUMCs(bool print_members); 
			void Reset(); 
			void SetUseNet(bool use) { mbln_use_net = use; }; 
			void SetOptions(float wt_mono_mass, 
							float wt_avg_mass, 
							float wt_log_abundance,
							float wt_scan, 
							float wt_fit,
							float wt_net,
							float mono_constraint, 
							float avg_constraint, 
							double max_dist,
							bool use_net,
							double fitFilter,
							double intensityFilter,
							bool useFitFilter,
							bool invertFitFilter,
							bool useIntensityFilter)
			{
				mflt_wt_mono_mass		= wt_mono_mass; 
				mflt_wt_average_mass	= wt_avg_mass; 
				mflt_wt_log_abundance	= wt_log_abundance; 
				mflt_wt_scan			= wt_scan; 
				mflt_wt_fit				= wt_fit;
				mflt_wt_net				= wt_net; 

				mflt_constraint_mono_mass	 = mono_constraint; 
				mflt_constraint_average_mass = avg_constraint; 
				mdouble_max_distance		 = max_dist; 
				mbln_use_net				 = use_net; 

				mbool_invertFitFilter		 = invertFitFilter;
				mbool_useFitFilter			 = useFitFilter;
				mbool_useIntensityFilter	 = useIntensityFilter;
				mdouble_fitFilter			 = fitFilter;
				mdouble_intensityFilter		 = intensityFilter;
			}
			void SetMinMaxScan(int minScan, int maxScan) 
			{ 
				mint_min_scan = minScan; 
				mint_max_scan = maxScan; 
			}; 
			void SetPeks(std::vector<IsotopePeak> &vectPks); 

		};
	}
}