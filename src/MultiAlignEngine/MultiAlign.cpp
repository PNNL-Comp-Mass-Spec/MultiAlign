// This is the main DLL file.

#include "stdafx.h"

#include "MultiAlign.h"
#include <algorithm> 
#include <fstream>
#include "clsSingleLinkageClustering.h" 
using namespace std ; 

namespace MultiAlign
{
	clsMS2MSAlignWrapper::clsMS2MSAlignWrapper()
	{
		mobj_ms2ms_feature_matcher = __nogc new LCMSWarp() ; 
		mint_num_sections = 100 ;
		mshort_contraction_factor = 2 ; 
		mshort_max_section_jump = 5 ; 
		mdbl_mass_tolerance = 3 ; // in ppm
		mdbl_net_tolerance = 0.02 ; 
		mdbl_std_net = 0.007 ; 
		mdbl_std_mass = 10 ; // in ppm  
		menm_alignment_type = NET_WARP ; 
	}
	clsMS2MSAlignWrapper::~clsMS2MSAlignWrapper() 
	{
		delete mobj_ms2ms_feature_matcher ; 
	}

	void clsMS2MSAlignWrapper::SetAlignmentData(UMCManipulation::clsClusters *(&umc_clusters))
	{
		mobj_clusters = umc_clusters ; 
	}

	void clsMS2MSAlignWrapper::SetUMCNets()
	{

		int num_datasets = mobj_clusters->mobj_umc_data->NumDatasets ; 
		int min_scan, max_scan ; 
		mobj_clusters->mobj_umc_data->GetMinMaxScan(mint_baseline_sample, &min_scan, &max_scan) ; 
		for (int dataset_num = 0 ; dataset_num < num_datasets ; dataset_num++)
		{
			int umc_start_index, umc_stop_index ; 
			mobj_clusters->mobj_umc_data->GetDataIndex(dataset_num, &umc_start_index, &umc_stop_index) ; 
			for (int umc_num = umc_start_index ; umc_num < umc_stop_index ; umc_num++)
			{
				UMCManipulation::clsUMC *umc = mobj_clusters->mobj_umc_data->marr_umcs[umc_num] ; 
				double net = ((umc->mint_scan_aligned - min_scan) *1.0)/ (max_scan - min_scan+1) ; 
				umc->mdbl_net = net ; 
			}
		}
	}
	
	void clsMS2MSAlignWrapper::CopyUMCSToFeatures(vector<clsFeatureData> &vect_features)
	{
		vect_features.clear() ; 
		clsFeatureData feature ; 

		SetUMCNets() ; 

		int num_datasets = mobj_clusters->mobj_umc_data->NumDatasets ; 
		int min_scan, max_scan ; 
		mobj_clusters->mobj_umc_data->GetMinMaxScan(mint_baseline_sample, &min_scan, &max_scan) ; 
		for (int dataset_num = 0 ; dataset_num < num_datasets ; dataset_num++)
		{
			int umc_start_index, umc_stop_index ; 
			mobj_clusters->mobj_umc_data->GetDataIndex(dataset_num, &umc_start_index, &umc_stop_index) ; 
			for (int umc_num = umc_start_index ; umc_num < umc_stop_index ; umc_num++)
			{
				UMCManipulation::clsUMC *umc = mobj_clusters->mobj_umc_data->marr_umcs[umc_num] ; 
				feature.Set(umc_num, dataset_num, umc->mdbl_mono_mass_calibrated, umc->mdbl_net) ; 
				vect_features.push_back(feature) ; 
			}
		}
	}

	void clsMS2MSAlignWrapper::PrintFeatures()
	{
		std::ofstream fout("c:\\msms_features.csv") ; 
		MassTimeFeature ms_feature ; 

		int min_scan, max_scan ; 
		int umc_start_index, umc_stop_index ; 

		mobj_clusters->mobj_umc_data->GetMinMaxScan(0, &min_scan, &max_scan) ; 
		mobj_clusters->mobj_umc_data->GetDataIndex(0, &umc_start_index, &umc_stop_index) ; 
		fout<<"mono_mass,calibrated_mono_mass,original_mass,net,mz\n" ; 
		for (int umc_num = umc_start_index ; umc_num < umc_stop_index ; umc_num++)
		{
			UMCManipulation::clsUMC *umc = mobj_clusters->mobj_umc_data->marr_umcs[umc_num] ; 

			ms_feature.mdbl_mono_mass = umc->mdbl_mono_mass  ; 
			ms_feature.mdbl_mono_mass_calibrated = umc->mdbl_mono_mass_calibrated ; 
			ms_feature.mdbl_mono_mass_original = umc->mdbl_mono_mass   ; 
			if (mbln_use_nets)
			{
				ms_feature.mdbl_net = umc->mdbl_net ; 
			}
			else
			{
				double net = ((umc->mint_scan - min_scan) *1.0)/ (max_scan - min_scan+1) ; 
				ms_feature.mdbl_net = net ; 
			}

			ms_feature.mdbl_mz = umc->mdbl_class_rep_mz ; 
			ms_feature.mdbl_abundance = umc->mdbl_abundance ;

			fout<<ms_feature.mdbl_mono_mass<<","<<ms_feature.mdbl_mono_mass_calibrated<<","<<ms_feature.mdbl_mono_mass_original<<"," ; 
			fout<<ms_feature.mdbl_net<<","<<ms_feature.mdbl_mz<<"\n" ; 
		}
		fout.close() ; 

		fout.open("c:\\ms_features.csv") ; 
		mobj_clusters->mobj_umc_data->GetMinMaxScan(1, &min_scan, &max_scan) ; 
		mobj_clusters->mobj_umc_data->GetDataIndex(1, &umc_start_index, &umc_stop_index) ; 
		fout<<"mono_mass,calibrated_mono_mass,original_mass,mz,aligned_net,net\n" ; 
		for (int umc_num = umc_start_index ; umc_num < umc_stop_index ; umc_num++)
		{
			UMCManipulation::clsUMC *umc = mobj_clusters->mobj_umc_data->marr_umcs[umc_num] ; 

			ms_feature.mdbl_mono_mass = umc->mdbl_mono_mass  ; 
			ms_feature.mdbl_mono_mass_calibrated = umc->mdbl_mono_mass_calibrated ; 
			ms_feature.mdbl_mono_mass_original = umc->mdbl_mono_mass   ; 
			if (mbln_use_nets)
			{
				ms_feature.mdbl_net = umc->mdbl_net ; 
			}
			else
			{
				double net = ((umc->mint_scan - min_scan) *1.0)/ (max_scan - min_scan+1) ; 
				ms_feature.mdbl_net = net ; 
			}

			ms_feature.mdbl_mz = umc->mdbl_class_rep_mz ; 
			ms_feature.mdbl_abundance = umc->mdbl_abundance ;

			fout<<ms_feature.mdbl_mono_mass<<","<<ms_feature.mdbl_mono_mass_calibrated<<","<<ms_feature.mdbl_mono_mass_original<<"," ; 
			fout<<ms_feature.mdbl_net<<","<<ms_feature.mdbl_mz<<"\n" ; 
		}
		fout.close() ; 

	}

	void clsMS2MSAlignWrapper::CalculatePairwiseDistances(vector<clsFeatureData> &vect_features, 
		int start_feature_num, int stop_feature_num, vector<clsDistance> &vect_distances) 
	{
		vect_distances.clear() ; 
		clsDistance distance ; 
		// now remember that the vect_features are sorted in a mass order and no other order. 
		// so we'll just work with index 0 beginning at start_umc_num and index ending at umc_num.
		for (int feature_index1 = start_feature_num ; feature_index1 <= stop_feature_num ; feature_index1++)
		{
			// we want to keep the feature numbers 0 based. 
			int feature_num_1 = feature_index1 - start_feature_num ; 
			for (int feature_index2 = feature_index1+1 ; feature_index2 <= stop_feature_num ; feature_index2++)
			{
				// we want to keep the feature numbers 0 based. 
				int feature_num_2 = feature_index2 - start_feature_num ; 
				double mass_diff = ((vect_features[feature_index1].mdbl_mass - vect_features[feature_index2].mdbl_mass) * 1000000)/vect_features[feature_index1].mdbl_mass ; 
				double net_diff = vect_features[feature_index1].mdbl_net - vect_features[feature_index2].mdbl_net ; 
				if (abs(mass_diff) < mdbl_mass_tolerance && abs(net_diff) < mdbl_net_tolerance)
				{
					double dist = (mass_diff * mass_diff)/ (2* mdbl_std_mass * mdbl_std_mass) + (net_diff*net_diff) / (2*mdbl_std_net * mdbl_std_net) ; 
					distance.Set(feature_num_1, feature_num_2, dist) ; 
					vect_distances.push_back(distance) ; 
				}
			}

		}
	}

	void clsMS2MSAlignWrapper::PerformMS2MSClustering(bool calculate_cluster_stats)
	{
		mobj_clusters->marr_umc_2_cluster = new int __gc[mobj_clusters->mobj_umc_data->NumUMCS] ; 
		mobj_clusters->MassTolerance = mdbl_mass_tolerance ; 
		mobj_clusters->NETTolerance = mdbl_net_tolerance ; 

		menm_status = enmStatus::CLUSTERING ;
		mint_percent_done = 0 ;
		mstr_message = new System::String("Performing Clustering.") ; 

		int num_datasets = mobj_clusters->mobj_umc_data->NumDatasets ; 
		clsSingleLinkageClustering obj_clustering ; 
		vector<clsDistance> vect_distances ; 
		vector<clsFeatureData> vect_features ; 

		mstr_message = new System::String("Copying UMCS to local structures.") ; 
		// copy all the UMCS to features array. 
		CopyUMCSToFeatures(vect_features) ;

		mstr_message = new System::String("Sorting UMCS by Mass.") ; 
		// now sort the features array by mass.
		sort(vect_features.begin(), vect_features.end(), SortFeatureDataByMass) ; 
		// now go through the features array and using the supplied mass and net tolerance,
		// look for the situation where the mass difference between consecutive values is 
		// greater than mass_tolerance.

		int start_feature_num = 0 ; 
		int feature_num = start_feature_num ; 
		int num_features = vect_features.size() ; 

		clsFeatureData current_feature, next_feature ; 
		multimap<int,int>::iterator cluster_iterator ; 

		mstr_message = new System::String("Clustering features.") ; 
		mint_num_clusters = 0 ; 
		while(feature_num < num_features-1)
		{
			mint_percent_done = (100 * feature_num) / num_features ; 
			current_feature = vect_features[feature_num] ; 
			next_feature = vect_features[feature_num+1] ; 
			double mass_diff = ((next_feature.mdbl_mass - current_feature.mdbl_mass ) * 1000000)/ current_feature.mdbl_mass ; 
			if (mass_diff > mdbl_mass_tolerance)
			{
				if (start_feature_num == feature_num)
				{
					// only one element in this cluster. 
					int dataset_index = current_feature.mint_dataset_index ; 
					int index = current_feature.mint_index ; 
					int current_cluster_num = mint_num_clusters ; 

					mobj_clusters->marr_umc_2_cluster[index] = current_cluster_num ; 
					mint_num_clusters++ ; 
				}
				else
				{
					// discovered a break point. Time to make clustering happen between 
					// the features starting at start_feature_num and the current one.
					// For that, we need to find all the pairs of distances 
					// that are within the supplied tolerances. 
					CalculatePairwiseDistances(vect_features, start_feature_num, feature_num, vect_distances) ; 
					// next call for Single Linkage clustering to happen with the pairwise distances. 
					obj_clustering.Cluster(vect_distances, feature_num - start_feature_num+1) ; 
					// so now the clustering is done for this mass bin. copy all the clusterings
					// to cluster object. For multiple data points per cluster copy only the most intense value.				
					int num_clusters = 0 ; 
					int last_cluster_num = -1 ; 

					for(cluster_iterator = obj_clustering.mmap_cluster_members.begin() ; cluster_iterator != obj_clustering.mmap_cluster_members.end() ; cluster_iterator++)
					{
						int cluster_num = (*cluster_iterator).first ; 
						// get the feature_index. Remember that 0 feature number from clustering 
						// is the 0th + start_feature_num here.
						int feature_index = (*cluster_iterator).second +start_feature_num ; 
						int index = vect_features[feature_index].mint_index ; 
						int dataset_index = vect_features[feature_index].mint_dataset_index ; 

						UMCManipulation::clsUMC *umca = mobj_clusters->mobj_umc_data->marr_umcs[index] ; 
						if (cluster_num != last_cluster_num )
						{
							last_cluster_num = cluster_num ; 
							num_clusters++ ; 
						}
						int current_cluster_num = mint_num_clusters+num_clusters-1 ; 
						mobj_clusters->marr_umc_2_cluster[index] = current_cluster_num ; 
					}
					mint_num_clusters += num_clusters ; 
				}
				start_feature_num = feature_num + 1 ; 
			}
			feature_num++ ; 
		}

		// the last point is unattended to. 
		current_feature = vect_features[feature_num] ; 
		if (start_feature_num == feature_num)
		{
			// only one element in this cluster. 
			int dataset_index = current_feature.mint_dataset_index ; 
			int index = current_feature.mint_index ; 
			int current_cluster_num = mint_num_clusters ; 

			mobj_clusters->marr_umc_2_cluster[index] = current_cluster_num ; 
			mint_num_clusters++ ; 
		}
		else
		{
			// discovered a break point. Time to make clustering happen between 
			// the features starting at start_feature_num and the current one.
			// For that, we need to find all the pairs of distances 
			// that are within the supplied tolerances. 
			CalculatePairwiseDistances(vect_features, start_feature_num, feature_num, vect_distances) ; 
			// next call for Single Linkage clustering to happen with the pairwise distances. 
			obj_clustering.Cluster(vect_distances, feature_num - start_feature_num+1) ; 
			// so now the clustering is done for this mass bin. copy all the clusterings
			// to cluster object. For multiple data points per cluster copy only the most intense value.				
			int num_clusters = 0 ; 
			int last_cluster_num = -1 ; 

			for(cluster_iterator = obj_clustering.mmap_cluster_members.begin() ; cluster_iterator != obj_clustering.mmap_cluster_members.end() ; cluster_iterator++)
			{
				int cluster_num = (*cluster_iterator).first ; 
				// get the feature_index. Remember that 0 feature number from clustering 
				// is the 0th + start_feature_num here.
				int feature_index = (*cluster_iterator).second +start_feature_num ; 
				int index = vect_features[feature_index].mint_index ; 
				int dataset_index = vect_features[feature_index].mint_dataset_index ; 

				UMCManipulation::clsUMC *umca = mobj_clusters->mobj_umc_data->marr_umcs[index] ; 
				if (cluster_num != last_cluster_num )
				{
					last_cluster_num = cluster_num ; 
					num_clusters++ ; 
				}
				int current_cluster_num = mint_num_clusters+num_clusters-1 ; 
				mobj_clusters->marr_umc_2_cluster[index] = current_cluster_num ; 
			}
			mint_num_clusters += num_clusters ; 
		}

		mobj_clusters->set_NumClusters(mint_num_clusters) ; 
		mstr_message = new System::String("Copying cluster informations to UMCData.") ; 
		// now copy clusters to mobj_umc_data. 
		mobj_clusters->CalculateClusterToUMCMaps() ;
		// call this only for cases where the cluster mzs need to be changed based on umcs.
		if (calculate_cluster_stats)
			mobj_clusters->CalculateStatistics() ; 
		menm_status = enmStatus::COMPLETE ;
	}

	void clsMS2MSAlignWrapper::PerformMS2MSAlignment() 
	{
		mobj_ms2ms_feature_matcher->ResetPercentComplete() ; 
		menm_status = enmStatus::ALIGNING ;

		mstr_message = new System::String("Aligning all against first dataset") ; 
		PerformMS2MSAlignment(mint_baseline_sample) ; 

		mstr_message = new System::String("Done Performing Alignment") ; 
		menm_status = enmStatus::COMPLETE ;
	}

	void clsMS2MSAlignWrapper::PerformMS2MSAlignment(int baseline_sample)
	{
		// Set the options. 
//		mobj_ms2ms_feature_matcher->SetOptions(mdbl_mass_tolerance, mdbl_net_tolerance) ; 

		mint_baseline_sample = baseline_sample ; 

		mobj_ms2ms_feature_matcher->ResetPercentComplete() ; 

		mstr_message = new System::String("Setting MS Features for reference sample.") ; 
		SetMSFeatures(baseline_sample, mobj_ms2ms_feature_matcher->mvect_baseline_features) ; 

		// now align all samples to reference sample.
		int num_samples = mobj_clusters->mobj_umc_data->NumDatasets ; 

		for (int sample_num = 0 ; sample_num < num_samples ; sample_num++)
		{
			if (sample_num == baseline_sample)
				continue ; 
			PerformMS2MSAlignment(baseline_sample, sample_num, true) ; 
		}
	}

	void clsMS2MSAlignWrapper::PerformMS2MSAlignment(int baseline_sample, int sample_num, bool use_old_baseline)
	{
		mint_baseline_sample = baseline_sample ; 

		mobj_ms2ms_feature_matcher->ResetPercentComplete(); 

		if (!use_old_baseline)
		{
			mstr_message = new System::String("Setting MS Features for reference sample.") ; 
			SetMSFeatures(baseline_sample, mobj_ms2ms_feature_matcher->mvect_baseline_features) ; 
		}

		mstr_message = System::String::Concat("Setting MS Features for sample number ", sample_num.ToString()) ;  
		SetMSFeatures(sample_num, mobj_ms2ms_feature_matcher->mvect_features) ; 

		mstr_message = System::String::Concat("Performing alignment of sample ", mobj_clusters->mobj_umc_data->DatasetNameAt(sample_num)) ; 

		switch (menm_alignment_type)
		{
			case NET_WARP:
				PerformNetWarp() ; 
				break ; 
			case NET_MASS_WARP:
				PerformNetMassWarp() ; 
				break ; 
			default:
				break ; 
		}

		// now use the transformed scans of the ms features and set to those in the 
		// array. 
		int start_index, stop_index ;
		// use the same indices for the features as used in the SetMSFeatures function.
		mobj_clusters->mobj_umc_data->GetDataIndex(sample_num, &start_index, &stop_index) ;

		mstr_message = System::String::Concat("Setting aligned scan of ", Convert::ToString(sample_num)) ; 

		int num_ms_features = mobj_ms2ms_feature_matcher->mvect_features.size() ; 
		MassTimeFeature ms_feature ; 
		mint_percent_done = 0 ; 
		int min_scan, max_scan ; 
		mobj_clusters->mobj_umc_data->GetMinMaxScan(mint_baseline_sample, &min_scan, &max_scan) ; 

		for (int feature_num = 0 ; feature_num < num_ms_features ; feature_num++)
		{
			mint_percent_done = (100 * feature_num) / num_ms_features ; 
			ms_feature = mobj_ms2ms_feature_matcher->mvect_features[feature_num] ; 
			UMCManipulation::clsUMC *umc = mobj_clusters->mobj_umc_data->marr_umcs[ms_feature.mint_id] ; 
			umc->mdbl_mono_mass_calibrated = ms_feature.mdbl_mono_mass_calibrated ; 
			// when MS2MS alignment is performed, alignment is on the level of scans. 
			umc->mint_scan_aligned = min_scan + ms_feature.mdbl_aligned_net * (max_scan - min_scan) ; 
		}
	}

	void clsMS2MSAlignWrapper::SetMSFeatures(int sample_num, vector<MassTimeFeature> &vect_features) 
	{
		mint_percent_done = 0 ; 
		vect_features.clear() ; 
		int start_index, stop_index ;

		int min_scan, max_scan ; 
		mobj_clusters->mobj_umc_data->GetMinMaxScan(sample_num, &min_scan, &max_scan) ; 
		mobj_clusters->mobj_umc_data->GetDataIndex(sample_num, &start_index, &stop_index) ;
		int num_pts = stop_index - start_index ; 
		vect_features.reserve(num_pts) ; 

		MassTimeFeature ms_feature ; 
		for (int index = start_index ; index < stop_index ; index++)
		{
			UMCManipulation::clsUMC *umc = mobj_clusters->mobj_umc_data->marr_umcs[index] ; 
			mint_percent_done = (100 * (index - start_index)) / (stop_index - start_index+1) ; 
			ms_feature.mdbl_mono_mass = umc->mdbl_mono_mass  ; 
			ms_feature.mdbl_mono_mass_calibrated = umc->mdbl_mono_mass_calibrated ; 
			ms_feature.mdbl_mono_mass_original = umc->mdbl_mono_mass   ; 
			if (mbln_use_nets)
			{
				ms_feature.mdbl_net = umc->mdbl_net ; 
			}
			else
			{
				double net = ((umc->mint_scan - min_scan) *1.0)/ (max_scan - min_scan+1) ; 
				ms_feature.mdbl_net = net ; 
			}

			ms_feature.mdbl_mz = umc->mdbl_class_rep_mz ; 
			ms_feature.mdbl_abundance = umc->mdbl_abundance ;
			ms_feature.mint_id = index ; 
			vect_features.push_back(ms_feature) ; 
		}
		mint_percent_done = 100 ; 
	}

	void clsMS2MSAlignWrapper::Reset()
	{
		menm_status = enmStatus::IDLE ;
	}

	void clsMS2MSAlignWrapper::SetNETOptions(int num_sections, int contraction_factor, int max_section_jump, int max_promiscous)
	{
		// TODO: Add your implementation code here
		mobj_ms2ms_feature_matcher->SetNETOptions(num_sections, contraction_factor, max_section_jump, max_promiscous) ; 
	}

	void clsMS2MSAlignWrapper::PerformNetWarp()
	{
		mstr_message = new System::String ("Performing Alignment between UMCs and Mass Tag Database");

		mstr_message = new System::String("Generating Candidate Matches");

		mobj_ms2ms_feature_matcher->GenerateCandidateMatches() ;
		if (mobj_ms2ms_feature_matcher->mvect_feature_matches.size() <10)
		{
			mstr_message = new System::String("In sufficient candidate matches");
			menm_status = FAILED ; 
			menm_fail_type = INSUFFICIENT_MATCHES ; 
			return ; 
		}

		mstr_message = new System::String("Get Match Scores Between Sections");
		mobj_ms2ms_feature_matcher->GetMatchProbabilities();

		mstr_message = new System::String("Calculating Alignment Scores Between Sections");
		mobj_ms2ms_feature_matcher->CalculateAlignmentMatrix() ;

		mstr_message = new System::String("Calculating Alignment Function");
		mobj_ms2ms_feature_matcher->CalculateAlignmentFunction() ; 

		mstr_message = new System::String("Getting Transformed Scans");
		mobj_ms2ms_feature_matcher->GetTransformedNets() ; 

		mstr_message = new System::String("Calculating Matches");
		mobj_ms2ms_feature_matcher->CalculateAlignmentMatches() ; 

		//mobj_ms2ms_feature_matcher->PrintCandidateMatches("c:\\matches.csv") ; 
		//ofstream fout("c:\\match_scores.csv") ; 
		//mobj_ms2ms_feature_matcher->PrintSubsectionMatchScores(fout) ; 
		//fout.close() ; 
		//fout.open("c:\\alignment_function.csv") ; 
		//mobj_ms2ms_feature_matcher->PrintAlignmentFunction(fout) ; 
		//fout.close() ; 
		mstr_message = new System::String("Done with Alignment") ;

	}
	void clsMS2MSAlignWrapper::PerformNetMassWarp()
	{
		// first perform a net calibration using a wide mass tolerance which is the same as the 
		// mass window parameter.
		double mass_tolerance = mobj_ms2ms_feature_matcher->GetMassTolerance() ; 
		mobj_ms2ms_feature_matcher->SetMassTolerance(mobj_ms2ms_feature_matcher->GetMassCalibrationWindow()) ; 
		mobj_ms2ms_feature_matcher->UseMassAndNetScore(false) ; 

		PerformNetWarp() ; 

		if (menm_status == FAILED)
			return ; 

		mstr_message = new System::String("Performing Mass Recalibration");
		mobj_ms2ms_feature_matcher->PerformMassCalibration() ; 

		mstr_message = new System::String("Calculate Standard Deviations");
		mobj_ms2ms_feature_matcher->CalculateStandardDeviations() ;
		
		mobj_ms2ms_feature_matcher->SetMassTolerance(mass_tolerance) ; 
		mobj_ms2ms_feature_matcher->UseMassAndNetScore(true) ; 
		PerformNetWarp() ; 

		//mobj_ms2ms_feature_matcher->PrintCandidateMatches("c:\\matches_final.csv") ;
	}


	void clsMS2MSAlignWrapper::GetMatches(int (&feature_indices) __gc[], int (&baseline_feature_indices) __gc[], 
									double (&feature_nets) __gc[], double (&baseline_feature_nets) __gc[])
	{
		int num_matches = mobj_ms2ms_feature_matcher->mvect_feature_matches.size() ; 
		feature_indices = new int __gc [num_matches] ;
		baseline_feature_indices = new int __gc [num_matches] ;
		feature_nets = new double __gc [num_matches] ; 
		baseline_feature_nets = new double __gc [num_matches] ; 

		for (int index = 0 ; index < num_matches ; index++)
		{
			clsFeatureMatch match =  mobj_ms2ms_feature_matcher->mvect_feature_matches[index] ; 
			MassTimeFeature feature = mobj_ms2ms_feature_matcher->mvect_features[match.mint_feature_index] ; 
			MassTimeFeature baseline_feature = mobj_ms2ms_feature_matcher->mvect_baseline_features[match.mint_feature_index_2] ; 

			feature_indices[index] = feature.mint_id ; 
			baseline_feature_indices[index] = baseline_feature.mint_id ; 
			feature_nets[index] = feature.mdbl_aligned_net ; 
			baseline_feature_nets[index] = feature.mdbl_net ;
		}
	}
	void clsMS2MSAlignWrapper::GetMatchIndices(int (&feature_indices) __gc[], int (&baseline_feature_indices) __gc[])
	{
		int num_matches = mobj_ms2ms_feature_matcher->mvect_feature_matches.size() ; 
		feature_indices = new int __gc [num_matches] ;
		baseline_feature_indices = new int __gc [num_matches] ;
		for (int index = 0 ; index < num_matches ; index++)
		{
			clsFeatureMatch match =  mobj_ms2ms_feature_matcher->mvect_feature_matches[index] ; 
			MassTimeFeature feature = mobj_ms2ms_feature_matcher->mvect_features[match.mint_feature_index] ; 
			MassTimeFeature baseline_feature = mobj_ms2ms_feature_matcher->mvect_baseline_features[match.mint_feature_index_2] ; 

			feature_indices[index] = feature.mint_id ; 
			baseline_feature_indices[index] = baseline_feature.mint_id ; 
		}
	}
	void clsMS2MSAlignWrapper::GetMatchNets(double (&feature_nets) __gc[], double (&baseline_feature_nets) __gc[])
	{
		int num_matches = mobj_ms2ms_feature_matcher->mvect_feature_matches.size() ; 
		feature_nets = new double __gc [num_matches] ; 
		baseline_feature_nets = new double __gc [num_matches] ; 
		for (int index = 0 ; index < num_matches ; index++)
		{
			clsFeatureMatch match =  mobj_ms2ms_feature_matcher->mvect_feature_matches[index] ; 
			MassTimeFeature feature = mobj_ms2ms_feature_matcher->mvect_features[match.mint_feature_index] ; 
			MassTimeFeature baseline_feature = mobj_ms2ms_feature_matcher->mvect_baseline_features[match.mint_feature_index_2] ; 

			feature_nets[index] = feature.mdbl_aligned_net ; 
			baseline_feature_nets[index] = feature.mdbl_net ;
		}
	}
	void clsMS2MSAlignWrapper::GetTransformedNets(double (&feature_nets) __gc[], double (&baseline_feature_nets) __gc[])
	{
		int num_nets = feature_nets->Length ; 
		baseline_feature_nets = new double __gc [num_nets] ; 
		for (int index = 0 ; index < num_nets ; index++)
		{
			double net = mobj_ms2ms_feature_matcher->GetTransformedNet(feature_nets[index]) ; 
			baseline_feature_nets[index] = net ; 
		}
	}
}
