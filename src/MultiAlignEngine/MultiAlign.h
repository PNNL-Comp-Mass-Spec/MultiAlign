// MultiAlign.h
#include "LCMSWarp.h" 
#include "clsSingleLinkageClustering.h" 
#include "clsFeatureData.h"
#include "clsAlignmentFunction.h"
#include <vector>
using namespace std ; 

#pragma once

using namespace System;

namespace MultiAlign
{
	public __value enum enmStatus { IDLE = 0, ALIGNING, CLUSTERING, FAILED, COMPLETE } ; 
	public __value enum enmFailType {INSUFFICIENT_MATCHES = 0 } ; 

	public __gc class clsMS2MSAlignWrapper
	{
		int mint_percent_done ; 
		System::String *mstr_message ; 
		enmStatus menm_status ; 
		enmFailType menm_fail_type ; 
		enmAlignmentType menm_alignment_type ; 

		UMCManipulation::clsClusters *mobj_clusters ; 

		LCMSWarp __nogc  *mobj_ms2ms_feature_matcher ; 
		int mint_num_sections ;
		short mshort_contraction_factor ; 
		short mshort_max_section_jump ; 
		double mdbl_mass_tolerance ; 
		double mdbl_net_tolerance ; 
		double mdbl_std_net ; 
		double mdbl_std_mass ; 
		bool mbln_use_nets ; 

		// if set to true, the intensity of a cluster in a datasets is the sum of 
		// intensities of features that belong to the same dataset. If set to false, 
		// the most intense point is kept. 
		bool mbln_sum_multiple ; 
		int mint_baseline_sample ; 

		int mint_num_clusters ;

		void CopyUMCSToFeatures(vector<clsFeatureData> &vect_features) ; 
		void CalculatePairwiseDistances(vector<clsFeatureData> &vect_features, int start_feature_num, 
			int stop_feature_num, vector<clsDistance> &vect_distances) ; 

		void SetUMCNets() ; 

	public:
		clsMS2MSAlignWrapper() ;
		~clsMS2MSAlignWrapper() ; 

		void SetClusteringOptions(int baseline_index, bool sum_multiple)
		{
			mint_baseline_sample = baseline_index ; 
			mbln_sum_multiple = sum_multiple ; 
		}
		void SetAlignmentOptions(double mass_tolerance, double net_tolerance)
		{
			mdbl_mass_tolerance = mass_tolerance ; 
			mdbl_net_tolerance = net_tolerance ; 
			mobj_ms2ms_feature_matcher->SetTolerances(mass_tolerance, net_tolerance) ; 
		}

		void SetUseNet(bool use)
		{
			mbln_use_nets = use ; 
		}
		void SetAlignmentData(UMCManipulation::clsClusters *(&umc_clusters)) ; 
		void PerformMS2MSAlignment() ;
		void PerformMS2MSClustering(bool calculate_cluster_stats) ; 
		void PerformMS2MSAlignment(int baseline_sample) ; 
		void PerformMS2MSAlignment(int baseline_sample, int sample, bool use_old_baseline) ; 
		void PerformNetWarp() ; 
		void PerformNetMassWarp() ;
		void SetMSFeatures(int sample_num, vector<MassTimeFeature> &vect_features) ; 
		void SetNETOptions(int num_sections, int contraction_factor, int max_section_jump, int max_promiscous) ;
		void SetMassCalOptions(double mass_window, int num_mass_delta_bin, int num_slices, int mass_jump, double ztolerance, bool use_lsq)
		{
			mobj_ms2ms_feature_matcher->SetMassCalOptions(mass_window, num_mass_delta_bin, num_slices, mass_jump, ztolerance, use_lsq) ; 
		}

		void SetMassCalLSQOptions(int num_knots, double outlier_zscore)
		{
			mobj_ms2ms_feature_matcher->SetMassCalLSQOptions(num_knots, outlier_zscore) ; 
		}

		void Reset() ; 

		// Call this function to get the matches from the result of one alignment.
		void GetMatches(int (&feature_indices) __gc[], int (&baseline_feature_indices) __gc[], 
				double (&feature_nets) __gc[], double (&baseline_feature_nets) __gc[]) ; 
		// Call this function to get the indices of matches from the result of one alignment.
		void GetMatchIndices(int (&feature_indices) __gc[], int (&baseline_feature_indices) __gc[]) ; 
		// Call this function to get the nets of matches from the result of one alignment.
		void GetMatchNets(double (&feature_nets) __gc[], double (&baseline_feature_nets) __gc[]) ;
		// Call this function to get the transformed nets of the umcs.
		void GetTransformedNets(double (&nets) __gc[], double (&transformed_nets) __gc[]) ;

		void PrintFeatures() ; 

		__property int get_PercentComplete()
		{
			switch (menm_status)
			{
				case enmStatus::ALIGNING:
					return mobj_ms2ms_feature_matcher->GetPercentComplete() ; 
					break ; 

			}
			return mint_percent_done ; 
		}

		__property enmStatus get_Status()
		{
			return menm_status ; 
		}

		__property System::String* get_Message()
		{
			return mstr_message ; 
		} 

		__property enmAlignmentType get_AlignmentType()
		{
			return menm_alignment_type ; 
		}

		__property void set_AlignmentType(enmAlignmentType val)
		{
			menm_alignment_type = val ; 
		}

		__property MultiAlign::enmCalibrationType get_CalibrationType()
		{
			return (MultiAlign::enmCalibrationType) mobj_ms2ms_feature_matcher->GetCalibrationType() ;
		}

		__property void set_CalibrationType(MultiAlign::enmCalibrationType val)
		{
			return mobj_ms2ms_feature_matcher->SetCalibrationType((enmCalibrationType)val ); 
		}

	};
}
