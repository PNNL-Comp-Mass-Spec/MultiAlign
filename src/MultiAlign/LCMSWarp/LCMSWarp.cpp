
#include ".\LCMSWarp.h"
#include <math.h>
#include <algorithm>
#include <float.h>
#include "MathUtils.h"
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	namespace Alignment
	{
		bool AlignmentMatchCompareBySection1(AlignmentMatch &a, AlignmentMatch &b) 
		{
			if (a.mint_section_start < b.mint_section_start)
				return true; 
			if (a.mint_section_start > b.mint_section_start)
				return false; 
			return (a.mdouble_net_start_2 < b.mdouble_net_start_2); 
		}

		void LCMSWarp::SetDefaultStdevs()
		{
			mdouble_net_std = 0.007; 
			mdouble_mass_std = 20; 
		}

		LCMSWarp::LCMSWarp(void):mdblMinMassNETLikelihood(1e-4)
		{

			mbln_use_mass					= false; 
			mdouble_mass_window				= 50; 
			mdouble_mass_tolerance			= 20; 
			mdouble_net_tolerance			= 0.02; 
			mint_max_section_distortion		= 2; 
			mdouble_net_std					= 0.007; 
			marr_alignment_score			= NULL; 
			marr_best_previous_index		= NULL; 
			mint_max_jump					= 10; 
			mdouble_mass_std				= 20; 
			mint_percent_done				= 0;
			mint_max_promiscous_umc_matches = 5; 
			mbln_keep_promiscous_matches	= false; 
			marr_alignment_score			= NULL; 
			marr_best_previous_index		= NULL; 

			
			this->menmCalibrationType	 = MZ_REGRESSION; 
			mint_mass_cal_num_delta_bins = 100; 
			mint_mass_cal_num_slices	 = 12; 
			mint_mass_cal_num_jump		 = 50; 

			double ztolerance						= 3; 
			Regression::enmRegressionType reg_type	= Regression::CENTRAL; 

			mobj_mz_recalibration.SetCentralRegressionOptions(mint_mass_cal_num_slices, 
																mint_mass_cal_num_delta_bins,
																mint_mass_cal_num_jump, 
																ztolerance, 
																reg_type); 
			mobj_net_recalibration.SetCentralRegressionOptions(mint_mass_cal_num_slices, 
																mint_mass_cal_num_delta_bins, 
																mint_mass_cal_num_jump, 
																ztolerance, reg_type); 

			double outlier_zscore	= 2.5; 
			int num_knots			= 12; 
			mobj_mz_recalibration.SetLSQOptions(num_knots, outlier_zscore); 
			mobj_net_recalibration.SetLSQOptions(num_knots, outlier_zscore); 

			mdblMinScore = -100000; 
			mdblMuMass = 0;
			mdblMuNET = 0; 
			mdblNormalProb = 0.3; 
		}

		LCMSWarp::~LCMSWarp(void)
		{
			mvect_section_unique_feature_indices.clear();
			mvect_num_features_in_sections.clear(); 
			mvect_num_features_in_baseline_sections.clear(); 
			mvect_alignment_func.clear(); 
			mvect_features.clear(); 
			mvect_baseline_features.clear(); 
			mvect_feature_matches.clear(); 
			mvect_subsection_match_scores.clear(); 
			mvect_temp_feature_best_delta.clear();
			mvect_temp_feature_best_index.clear();

			if (marr_alignment_score != 0)
			{
				delete [] marr_alignment_score; 
				marr_alignment_score = NULL;
			}
			if (marr_best_previous_index != 0)
			{
				delete [] marr_best_previous_index; 
				marr_best_previous_index = NULL;
			}

		}

		void LCMSWarp::ClearAllButData()
		{
			mvect_section_unique_feature_indices.clear();
			mvect_num_features_in_sections.clear(); 
			mvect_num_features_in_baseline_sections.clear(); 
			mvect_alignment_func.clear(); 
			mvect_feature_matches.clear(); 
			mvect_subsection_match_scores.clear(); 
			mvect_temp_feature_best_delta.clear();
			mvect_temp_feature_best_index.clear();
		}

		// clears the input features
		void LCMSWarp::ClearInputData()
		{
			mvect_features.clear(); 
			mvect_baseline_features.clear(); 
		}

		void LCMSWarp::Clear()
		{
			ClearInputData(); 
			ClearAllButData(); 
		}
		
		void LCMSWarp::GetAlignmentFunction(std::vector<double> &vectAligneeNET, std::vector<double> &vectReferenceNET) const 
		{
			vectAligneeNET.clear(); 
			vectReferenceNET.clear(); 
			int numPieces = (int) mvect_alignment_func.size(); 
			for (int pieceNum = 0; pieceNum < numPieces; pieceNum++)
			{
				AlignmentMatch match = mvect_alignment_func[pieceNum]; 
				vectAligneeNET.push_back(match.mdouble_net_start); 
				vectReferenceNET.push_back(match.mdouble_net_start_2); 
			}
		}

		void LCMSWarp::GetFeatureCalibratedMassesAndAlignedNETs(std::vector <int> &umcIndices, std::vector<double> &umcCalibratedMasses, 
			std::vector<double> &umcAlignedNETS,
			std::vector<double> &vectAlignedDriftTimes)
		{
			int numFeatures = (int) mvect_features.size(); 
			for (int featureNum = 0; featureNum < numFeatures; featureNum++)
			{
				MultiAlignEngine::Alignment::MassTimeFeature mtFeature = mvect_features[featureNum]; 
				umcIndices.push_back(mtFeature.mint_id); 
				umcCalibratedMasses.push_back(mtFeature.mdouble_mono_mass_calibrated); 
				umcAlignedNETS.push_back(mtFeature.mdouble_aligned_net); 
				vectAlignedDriftTimes.push_back(mtFeature.mdouble_driftTime);				
			}
		}
		void LCMSWarp::GetFeatureCalibratedMassesAndAlignedNETs(std::vector <int> &umcIndices, 
																std::vector<double> &umcCalibratedMasses, 
																std::vector<double> &umcAlignedNETS,
																std::vector<int> &umcAlignedScans, 
																std::vector<double> &umcDriftTimes,
																int minScan,
																int maxScan)
		{
			int numFeatures = (int) mvect_features.size(); 
			for (int featureNum = 0; featureNum < numFeatures; featureNum++)
			{
				MultiAlignEngine::Alignment::MassTimeFeature mtFeature = mvect_features[featureNum]; 
				umcIndices.push_back(mtFeature.mint_id); 
				umcCalibratedMasses.push_back(mtFeature.mdouble_mono_mass_calibrated); 
				umcAlignedNETS.push_back(mtFeature.mdouble_aligned_net); 
				umcAlignedScans.push_back(minScan + (int)(mtFeature.mdouble_aligned_net * (maxScan - minScan))); 
				umcDriftTimes.push_back(mtFeature.mdouble_driftTime);
			}
		}

		/********************************************************************************
			The function generates candidate matches between the MS and the MSMS
			data loaded into mvect_features and mvect_baseline_features respectively.
			It does so by finding out all pairs of (MassTimeFeature, clsMSMSDBFeature that 
			match within a provided mass tolerance window. 
		*******************************************************************************/
		void LCMSWarp::GenerateCandidateMatches()
		{
			if (mvect_features.size() == 0)
				return; 

			MassTimeFeature feature; 
			feature = mvect_features[0]; 
			sort(mvect_features.begin(), mvect_features.end(), &SortFeaturesByMass); 
			sort(mvect_baseline_features.begin(), mvect_baseline_features.end(), &SortFeaturesByMass); 

			ClearAllButData(); 

			// now go through each MassTimeFeature and see if the next baseline MassTimeFeature matches to it.
			int feature_index = 0; 
			int baseline_feature_index = 0; 
			int num_features = (int) mvect_features.size(); 
			int num_baseline_features = (int) mvect_baseline_features.size(); 

			/// Could not find any candidate matches.

			if (num_baseline_features <= 0)
				return;

			MassTimeFeature baseline_feature; 
			FeatureMatch feature_match; 

			mint_percent_done = 0; 

			while (feature_index < num_features)
			{
				mint_percent_done = (100 * feature_index) / num_features; 
				feature = mvect_features[feature_index];

				double mass_tolerance = feature.mdouble_mono_mass * mdouble_mass_tolerance / 1000000; 

				if (baseline_feature_index == num_baseline_features)
					baseline_feature_index = num_baseline_features-1; 
				baseline_feature = mvect_baseline_features[baseline_feature_index]; 
				while(baseline_feature_index >= 0 && baseline_feature.mdouble_mono_mass > feature.mdouble_mono_mass - mass_tolerance)
				{
					baseline_feature_index--; 
					if (baseline_feature_index >= 0)
						baseline_feature = mvect_baseline_features[baseline_feature_index]; 
				}

				baseline_feature_index++; 

				while(baseline_feature_index < num_baseline_features && 
					mvect_baseline_features[baseline_feature_index].mdouble_mono_mass < feature.mdouble_mono_mass + mass_tolerance )
				{
					if (mvect_baseline_features[baseline_feature_index].mdouble_mono_mass > feature.mdouble_mono_mass - mass_tolerance)
					{
						feature_match.mint_feature_index = feature_index; 
						feature_match.mint_feature_index_2 = baseline_feature_index; 
						feature_match.mdouble_net = feature.mdouble_net; 
						feature_match.mdouble_net_2 = mvect_baseline_features[baseline_feature_index].mdouble_net; 

						mvect_feature_matches.push_back(feature_match); 
					}
					baseline_feature_index++; 
				}
				feature_index++; 
			}

			// now all the matches are created. So, lets go through all the matches and find a mapping of 
			// how many times a baseline feature is matched to. Put matches into a multimap from mass tag id to 
			// mvect_feature_matches index.
			multimap<int, int>	map_mass_tag_2_matches;
			map<int,int>		map_mass_tag_2_count; 
			map<int,int>::iterator tag_2_count_iterator; 
			int num_matches = (int) mvect_feature_matches.size(); 
			for (int match_index = 0; match_index < num_matches; match_index++)
			{
				feature_match = mvect_feature_matches[match_index]; 
				int baseline_index = feature_match.mint_feature_index_2; 
				tag_2_count_iterator = map_mass_tag_2_count.find(baseline_index);
				if (tag_2_count_iterator != map_mass_tag_2_count.end())
				{
					int new_count = (*tag_2_count_iterator).second + 1; 
					map_mass_tag_2_count[baseline_index] = new_count; 
				}
				else
				{
					map_mass_tag_2_count[baseline_index] = 1; 
				}
				map_mass_tag_2_matches.insert(pair<int,int>(baseline_index, match_index)); 
			}

			// now lets go through each of the baseline features that were matched, and for each one
			// keep at the most mint_max_promiscous_umc_matches (or none, 
			// if mbln_keep_promiscous_matches is false).. Keeping, of course, only the first 
			// mint_max_promiscous_umc_matches by scan.

			vector<FeatureMatch> temp_matches; 
			temp_matches.reserve(mvect_feature_matches.size()); 
			multimap<double,int> net_matches_to_index; 
			multimap<double,int>::iterator scan_matches_iterator; 

			for (multimap<int,int>::iterator matches_iterator = map_mass_tag_2_matches.begin(); 
					matches_iterator != map_mass_tag_2_matches.end(); matches_iterator++)
			{
				int baseline_index =(*matches_iterator).first;
				int num_hits = map_mass_tag_2_count[baseline_index]; 
				if (num_hits <= mint_max_promiscous_umc_matches)
				{
					// add all of these to the temp_matches. 
					for (int i = 0; i < num_hits; i++)
					{
						int match_index = (*matches_iterator).second; 
						FeatureMatch match = mvect_feature_matches[match_index];
						temp_matches.push_back(match); 
						matches_iterator++; 
					}
					// in the iterations, we must have gone past the last one, so move back by one.
					matches_iterator--; 
				}
				else if (mbln_keep_promiscous_matches)
				{
					// keep those matches the have the minimum scan numbers. 
					net_matches_to_index.clear(); 
					for (int i = 0; i < num_hits; i++)
					{
						int match_index = (*matches_iterator).second; 
						net_matches_to_index.insert(pair<double,int>(mvect_feature_matches[match_index].mdouble_net, match_index)); 
						matches_iterator++; 
					}
					// now, keep only the first mint_max_promiscous_umc_matches in the temp matrix.
					scan_matches_iterator = net_matches_to_index.begin(); 
					for (int i = 0; i < mint_max_promiscous_umc_matches; i++)
					{
						int match_index = (*scan_matches_iterator).second; 
						FeatureMatch match = mvect_feature_matches[match_index];
						temp_matches.push_back(match); 
						scan_matches_iterator++; 
					}
					// in the iterations, we must have gone past the last one, so move back by one.
					matches_iterator--; 
				}
			}

			mvect_feature_matches = temp_matches; 
		}


		/*******************************************************************
			This function breaks the MS Features into mint_num_sections and the 
			MSMS Features into mint_num_baseline_sections sections. Each section in the 
			ms is compared to all contiguous sections in the msms of size between 
			1 and mint_num_matches_per_baseline_start and a probability score, 
			p(t_i, t_(i-1) matches T_j, T_k) is computed. 
			Then, 
				p(t_i matches T_j) = sum over T_k, t_(i-1) (p(t_(i-1) matches T_k) * p((t_i, t_(i-1)) in MS
						matches (T_k, T_j) in MSMS)

			Then, it computes all the probability measures p(t_i, t_(i-1)
			matches T_j, T_k) using : 
				p(t_i, t_(i-1) matches T_j, T_k) = product over all peptides k in 
					section t_i t_(i-1) 
					[(probability of msms observation of match) * f_m(peptide k scan_num transformed, 
					time of mass tag m)])
					where f_m is the pdf of the random variable of rt.
		********************************************************************/
		void LCMSWarp::GetMatchProbabilities()
		{
			mint_percent_done = 0; 
			int num_features  = (int) mvect_features.size(); 

			mvect_temp_feature_best_delta.clear(); 
			mvect_temp_feature_best_delta.reserve(num_features); 

			mvect_temp_feature_best_index.clear(); 
			mvect_temp_feature_best_index.reserve(num_features); 

			mvect_num_features_in_sections.clear(); 
			mvect_num_features_in_sections.reserve(mint_num_sections); 

			for (int i = 0; i < mint_num_sections; i++)
				mvect_num_features_in_sections.push_back(0); 

			mvect_num_features_in_baseline_sections.clear(); 
			mvect_num_features_in_baseline_sections.reserve(mint_num_baseline_sections); 
			for (int i = 0; i < mint_num_baseline_sections; i++)
				mvect_num_features_in_baseline_sections.push_back(0); 

			mdouble_max_net = INT_MIN; 
			mdouble_min_net = INT_MAX; 
			for (int i = 0; i < num_features; i++)
			{
				if (mvect_features[i].mdouble_net > mdouble_max_net)
					mdouble_max_net = mvect_features[i].mdouble_net;
				if (mvect_features[i].mdouble_net < mdouble_min_net)
					mdouble_min_net = mvect_features[i].mdouble_net;
				mvect_temp_feature_best_delta.push_back(DBL_MAX); 
				mvect_temp_feature_best_index.push_back(-1); 
			}

			for (int i = 0; i < num_features; i++)
			{
				double net = mvect_features[i].mdouble_net;
				int section_num = System::Convert::ToInt32(((net - mdouble_min_net) * mint_num_sections) / (mdouble_max_net - mdouble_min_net));
				if (section_num >= mint_num_sections)
					section_num--; 
				mvect_num_features_in_sections[section_num]++; 
			}

			mdouble_min_baseline_net  = DBL_MAX; 
			mdouble_max_baseline_net  = -1*DBL_MAX; 
			int num_baseline_features = (int) mvect_baseline_features.size(); 
			for (int i = 0; i < num_baseline_features; i++)
			{
				if (mvect_baseline_features[i].mdouble_net < mdouble_min_baseline_net)
					mdouble_min_baseline_net = mvect_baseline_features[i].mdouble_net; 
				if (mvect_baseline_features[i].mdouble_net > mdouble_max_baseline_net)
					mdouble_max_baseline_net = mvect_baseline_features[i].mdouble_net; 
			}

			for (int i = 0; i < num_baseline_features; i++)
			{
				double net = mvect_baseline_features[i].mdouble_net; 
				int msms_section_num = (int) (((net - mdouble_min_baseline_net) * mint_num_baseline_sections) / (mdouble_max_baseline_net - mdouble_min_baseline_net)); 
				if (msms_section_num == mint_num_baseline_sections)
					msms_section_num--; 
				mvect_num_features_in_baseline_sections[msms_section_num]++; 
			}

			// sort the FeatureMatch vector by the MassTimeFeature net value.. 
			sort(mvect_feature_matches.begin(), mvect_feature_matches.end(), &SortFeatureMatchesByNet);

			int num_matches = (int) mvect_feature_matches.size(); 

			vector<FeatureMatch> vect_section_features; 
			int last_section_end_index = 0; 

			// We already calculated, number of matches per section as number of baseline sections times 
			// square of contraction factor.
			int num_section_matches = mint_num_sections * mint_num_matches_per_section; 
			mvect_subsection_match_scores.clear(); 
			mvect_subsection_match_scores.reserve(num_section_matches); 
			// we set default match scores to log(1) = 0; this will be used for 
			// sections that have no features in the MS side that matched the MSMS data.
			for (int i = 0; i < num_section_matches; i++)
				mvect_subsection_match_scores.push_back(mdblMinScore); 

			if (num_matches == 0)
				return; 

			for (int section = 0; section < mint_num_sections; section++)
			{
				// want the status to go to a little less than 100 percent.
				mint_percent_done = (section * 100) / (mint_num_sections+1); 

				int start_match_index = 0; 
				vect_section_features.clear(); 

				double section_start_net = mdouble_min_net + (section*(mdouble_max_net - mdouble_min_net))/mint_num_sections;
				double section_end_net = mdouble_min_net + ((section+1)*(mdouble_max_net - mdouble_min_net))/mint_num_sections;

				double net = mvect_feature_matches[0].mdouble_net;
				while(start_match_index < num_matches && 
					((net = mvect_feature_matches[start_match_index].mdouble_net) < section_start_net))
				{
					start_match_index++; 
				}

				int end_match_index = start_match_index;
				while(end_match_index < num_matches && 
					((net = mvect_feature_matches[end_match_index].mdouble_net) < section_end_net))
				{
					end_match_index++; 
				}
				// now lets compute the match score for the sections.
				if (end_match_index != start_match_index)
				{
					vector<FeatureMatch>::iterator start_iter = mvect_feature_matches.begin() + start_match_index; 
					vector<FeatureMatch>::iterator stop_iter = start_iter + (end_match_index - start_match_index); 
					vect_section_features.insert(vect_section_features.begin(), start_iter, stop_iter); 
				}
				// now we have the appropriate matches for the section stored in vect_section_features. 
				// Lets compute all the matching scores of this ms section against all pairs of baseline sections.
				ComputeSectionMatch(section, vect_section_features, section_start_net, section_end_net); 
			}
		}

		double LCMSWarp::GetCurrentlyStoredSectionMatchScore(double max_miss_zscore,
				int &num_unique_features, int &num_unmatched_features_in_section)
		{
			// now we have to compute match scores for this section. 
			// remember, match score for a section is : 
			// = log (P(match of ms section to msms section) = sum over unique features in the ms
			// ( -1*(delta_match ^2)/(2*std_net*std_net) - # unique features * log(2*pi*std_net*std_net)
			double match_score = 0; 

			const double log_2_pi_std_net_std_net = log(2*3.1415*mdouble_net_std*mdouble_net_std); 
			const double log_2_pi_std_mass_std_mass = log(2*3.1415*mdouble_mass_std*mdouble_mass_std); 
			for (int i = 0; i < num_unique_features; i++)
			{
				int ms_feature_index = mvect_section_unique_feature_indices[i]; 
				int msms_feature_index = mvect_temp_feature_best_index[ms_feature_index]; 
				MassTimeFeature feature = mvect_features[ms_feature_index]; 
				MassTimeFeature baseline_feature = mvect_baseline_features[msms_feature_index]; 

				double delta_net = mvect_temp_feature_best_delta[ms_feature_index]; 

				if (mbln_use_mass)
				{
					double mass_delta = (feature.mdouble_mono_mass - baseline_feature.mdouble_mono_mass) * 1000000 / baseline_feature.mdouble_mono_mass;
					double likelihood = GetMatchLikelihood(mass_delta, delta_net); 
					match_score += log(likelihood); 
				}
				else
				{
					if (abs(delta_net) > mdouble_net_tolerance)
					{
						// Should really have been checking against a SLiC score distance, rather than these 
						// tolerances. Hence it is important to see how these distances measure out.
						delta_net = mdouble_net_tolerance; 
						num_unmatched_features_in_section++; 
						match_score -= 0.5 * (mdouble_net_tolerance/mdouble_net_std) * (mdouble_net_tolerance/mdouble_net_std); 
						match_score -= 0.5 * log_2_pi_std_net_std_net; 
					}
					else
					{
						match_score -= 0.5*(delta_net/mdouble_net_std) * (delta_net/mdouble_net_std); 
						match_score -= 0.5*log_2_pi_std_net_std_net; 
					}
				}
			}
			return match_score; 
		}
		/************************************************************************************
			This function computes the similarity score between the section provided, and all 
			the baseline sections. For this, we match the section against all the 
			baseline sections starting at index i where i ranges from 0 to num_msms_sections and 
			the size of the MSMS section varies from 1 to mint_num_matches_per_baseline_start.
			For each MS and MSMS section matched, the MS coordinates are transformed into the MSMS
			transformation using a straight linear transformation between these subsections. 
			Of course, this piecewise linear allows aribitrary functions in the limit of the whole
			MS and MSMS datasets.

			The clsMS2MSFeatureMatch vector that has elements that match based on mass alone and are used 
			in this process as the short cuts. 

		*************************************************************************************/
		void LCMSWarp::ComputeSectionMatch(int ms_section, vector<FeatureMatch> &section_matching_features, 
			double min_net, double max_net)
		{
			int num_matching_features = (int)section_matching_features.size(); 
			double baseline_section_width = (mdouble_max_baseline_net - mdouble_min_baseline_net)*1.0/ mint_num_baseline_sections;
			double max_miss_zscore = 3; 
			if (max_miss_zscore < mdouble_net_tolerance / mdouble_net_std) 
				max_miss_zscore = mdouble_net_tolerance / mdouble_net_std; 


			// keep track of only the unique indices of ms features, because we want to
			// get best matches only for each one. 
			mvect_section_unique_feature_indices.clear(); 
			mvect_section_unique_feature_indices.reserve(num_matching_features); 
			for (int i = 0; i < num_matching_features; i++)
			{
				bool found = false; 
				FeatureMatch match = section_matching_features[i]; 
				for (int j = 0; j < i; j++)
				{
					if (match.mint_feature_index == 
							section_matching_features[j].mint_feature_index)
					{
						found = true; 
						break; 
					}
				}
				if (!found)
				{
					mvect_section_unique_feature_indices.push_back(match.mint_feature_index); 
				}
			}

			int num_unique_features_START = (int) mvect_section_unique_feature_indices.size(); 
			int num_features_in_section_START = mvect_num_features_in_sections[ms_section]; 


			for (int baseline_section_start = 0; baseline_section_start < mint_num_baseline_sections; baseline_section_start++)
			{

				double baseline_start_net = mdouble_min_baseline_net + baseline_section_start  * baseline_section_width; 
				int end_section = baseline_section_start + mint_num_matches_per_baseline_start;
				if (end_section >= mint_num_baseline_sections)
					end_section = mint_num_baseline_sections;
				int num_baseline_features_in_section = 0; 
				for (int baseline_section_end = baseline_section_start; baseline_section_end < end_section; baseline_section_end++)
				{
					int num_unique_features = num_unique_features_START; 
					int num_features_in_section = num_features_in_section_START; 
					int num_unmatched_features_in_section = num_features_in_section - num_unique_features; 
					num_baseline_features_in_section += mvect_num_features_in_baseline_sections[baseline_section_end]; 
					// which score section does this match correspond to ? 
					// For each ms_section there are mint_num_matches_per_section.
					// And, for each baseline_section_start there are mint_num_matches_per_baseline_start.
					// And then we are (baseline_section_end - baseline_section_start) from the first 
					// section match.
					int section_index = ms_section * mint_num_matches_per_section 
						+ baseline_section_start * mint_num_matches_per_baseline_start 
						+ (baseline_section_end - baseline_section_start); 
					double baseline_end_net = mdouble_min_baseline_net + (baseline_section_end+1) * baseline_section_width; 

					for (int i = 0; i < num_unique_features; i++)
					{
						int ms_feature_index = mvect_section_unique_feature_indices[i]; 
						mvect_temp_feature_best_delta[ms_feature_index] = DBL_MAX; 
						mvect_temp_feature_best_index[ms_feature_index] = -1; 
					}

					// now we have msms section and matching ms section. Lets transform the 
					// scan numbers to nets using a transformation of the two sections, 
					// and use temporary vector mvect_temp_feature_best_delta to 
					// keep only the best match
					FeatureMatch match; 
					for (int i = 0; i < num_matching_features; i++)
					{
						// remember that we are transforming (min_ms_scan, max_ms_scan) to 
						// (baseline_start_net, baseline_end_net), so an ms feature with scan num t 
						// would transform to 
						// (t - min_ms_scan) * (baseline_end_net - baseline_start_net)/(max_ms_scan - min_ms_scan) + baseline_start_net
						match = section_matching_features[i]; 
						int ms_feature_index = match.mint_feature_index; 
						double feature_net = match.mdouble_net; 

						double transform_net = (feature_net - min_net) * (baseline_end_net - baseline_start_net); 
						transform_net = transform_net / (max_net - min_net) + baseline_start_net; 

						double delta_match = transform_net - match.mdouble_net_2; 
						if (abs(delta_match) < abs(mvect_temp_feature_best_delta[ms_feature_index]))
						{
							mvect_temp_feature_best_delta[ms_feature_index] = delta_match; 
							mvect_temp_feature_best_index[ms_feature_index] = match.mint_feature_index_2; 
						}
					}

					mvect_subsection_match_scores[section_index] = GetCurrentlyStoredSectionMatchScore
						(max_miss_zscore, num_unique_features, num_unmatched_features_in_section); 
				}
			}
		}

	/******************************************************************************************************
	*	Function calculates the alignment scores between the ms maps based on the match scores that were calculated 
	*	between the subsections. 
	*	marr_alignment_score(ms_section, ms_section_2, width) = max over p_end { marr_alignment_score(ms_section-1, p_end, ms_section_2) 
				+ mvect_subsection_match_scores [ms_section, ms_section_2, width] } 
	*	For alignment scores with ms_section 0, penalize for missing points. i.e.
		marr_alignment_score(0, j, width) = probability of not observing all the peptides in sections 0 to j-1 
		in map 2.
	****************************************************************************************************/
		void LCMSWarp::CalculateAlignmentMatrix()
		{
			mint_percent_done = 0; 
			int num_matches = (int) mvect_feature_matches.size(); 
			int num_possible_alignments = mint_num_sections*mint_num_baseline_sections*mint_num_matches_per_baseline_start; 

			if (marr_alignment_score != NULL)
				delete [] marr_alignment_score; 
			if (marr_best_previous_index != NULL)
				delete [] marr_best_previous_index; 

			marr_alignment_score = new double [num_possible_alignments]; 
			marr_best_previous_index = new int [num_possible_alignments]; 

			// set all the scores to - inf.
			for (int i = 0; i < num_possible_alignments; i++)
			{
				marr_alignment_score[i] = -1 * DBL_MAX; 
				marr_best_previous_index[i] = -1; 
			}

			// set all alignment scores of section 0 with sections j as equal to the 
			// probability of missing all points in sections 0 - j-1 in second map. 

			const double log_2_pi_std_net_std_net = log(2*3.1415*mdouble_net_std*mdouble_net_std); 
			// assuming that the unmatches score was at 3 standard deviations away.
			double unmatched_score = - 0.5 * log_2_pi_std_net_std_net; 
			if (mdouble_net_tolerance < 3 * mdouble_net_std)
			{
				unmatched_score -= 1 * 0.5 * 9.0;
			}
			else
			{
				unmatched_score -= 1 * 0.5 * (mdouble_net_tolerance * mdouble_net_tolerance) / (mdouble_net_std * mdouble_net_std);
			}
			if (mbln_use_mass == true)
			{
				// assume that for the unmatched, the masses were also off at mass tolerance. So use the same thresholds are for net.
				unmatched_score = 2* unmatched_score; 
			}

			int num_unmatched_msms_features = 0; 
			for (int baseline_section = 0; baseline_section < mint_num_baseline_sections; baseline_section++)
			{
				// lets assume that everything that was missed was past 3 standard deviations in net.
				//double match_score = - 1.0 * num_unmatched_msms_features*0.5 * 9.0; 
				//match_score -= num_unmatched_msms_features * 0.5 * log_2_pi_std_net_std_net; 
				for (int section_width = 0; section_width < mint_num_matches_per_baseline_start; section_width++)
				{
					// no need multiplying with ms_section because its 0
					int alignment_index = baseline_section * mint_num_matches_per_baseline_start + section_width; 
					//marr_alignment_score[alignment_index] = match_score + mvect_subsection_match_scores[alignment_index]; 
					//marr_alignment_score[alignment_index] = mvect_subsection_match_scores[alignment_index]; 
					marr_alignment_score[alignment_index] = 0; 
				}
				num_unmatched_msms_features += mvect_num_features_in_baseline_sections[baseline_section];  
			}

			int num_unmatched_ms_features = 0; 
			for (int section = 0; section < mint_num_sections; section++)
			{
				for (int section_width = 0; section_width < mint_num_matches_per_baseline_start; section_width++)
				{
					// no need multiplying with ms_section_2 because its 0
					int alignment_index = section * mint_num_matches_per_section + section_width; 
					marr_alignment_score[alignment_index] = mvect_subsection_match_scores[alignment_index] + unmatched_score * num_unmatched_ms_features; 
				}
				num_unmatched_ms_features += mvect_num_features_in_sections[section];  
			}

			// since we have already scored the ms_sections with index = 0 above, start at 1.
			for (int section = 1; section < mint_num_sections; section++)
			{
				mint_percent_done = (100 * section) / mint_num_sections; 
				for (int baseline_section = 1; baseline_section < mint_num_baseline_sections; baseline_section++)
				{
					double max_score = -1 * DBL_MAX; 
					double max_score_width = 0; 
					for (int section_width = 0; section_width < mint_num_matches_per_baseline_start; section_width++)
					{
						int alignment_index = section * mint_num_matches_per_section + baseline_section * mint_num_matches_per_baseline_start + section_width; 

						double current_best_score = -1*DBL_MAX; 
						int best_baseline_section = -1; 
						int best_previous_alignment_index; 
						//for (int previous_baseline_section_width = 1; previous_baseline_section_width <= mint_num_matches_per_baseline_start + mint_max_jump;	 previous_baseline_section_width++)
						//{
						//	int previous_baseline_section = (baseline_section - previous_baseline_section_width); 
						//	if (previous_baseline_section < 0)
						//		break; 

						//	int previous_alignment_index = (section-1) * mint_num_matches_per_section + previous_baseline_section * mint_num_matches_per_baseline_start + previous_baseline_section_width-1; 
						//	if (marr_alignment_score[previous_alignment_index] > current_best_score)
						//	{
						//		best_baseline_section = previous_baseline_section; 
						//		current_best_score = marr_alignment_score[previous_alignment_index]; 
						//		best_previous_alignment_index = previous_alignment_index; 
						//	}
						//}
						for (int previous_baseline_section = baseline_section -1; previous_baseline_section >= baseline_section - mint_num_matches_per_baseline_start - mint_max_jump; previous_baseline_section--)
						{
							if (previous_baseline_section < 0)
								break; 
							int max_width = baseline_section - previous_baseline_section; 
							if (max_width > mint_num_matches_per_baseline_start)
								max_width = mint_num_matches_per_baseline_start; 
							int previous_baseline_section_width = max_width; 
							int previous_alignment_index = (section-1) * mint_num_matches_per_section + previous_baseline_section * mint_num_matches_per_baseline_start + previous_baseline_section_width-1; 
							if (marr_alignment_score[previous_alignment_index] > current_best_score)
							{
								best_baseline_section = previous_baseline_section; 
								current_best_score = marr_alignment_score[previous_alignment_index]; 
								best_previous_alignment_index = previous_alignment_index; 
							}
						}
						if (current_best_score != -1*DBL_MAX)
						{
							marr_alignment_score[alignment_index] = current_best_score + mvect_subsection_match_scores[alignment_index]; 
							marr_best_previous_index[alignment_index] = best_previous_alignment_index; 
						}
						else
							marr_alignment_score[alignment_index] = -1 * DBL_MAX; 
					}
				}
			}

		}

		/******************************************************************************************************
		*	Function calculates the best alignment function from the alignment scores. 
		****************************************************************************************************/
		void LCMSWarp::CalculateAlignmentFunction()
		{
			mint_percent_done = 0; 
			// in order to do this, we need the best score from section = mint_num_sections -1 and 
			// to go backwards to find the best match. 
			int section = mint_num_sections -1; 
			int num_matches = (int) mvect_feature_matches.size(); 

			int best_previous_alignment_index = -1; 
			double best_score = -1 * DBL_MAX; 
			int best_aligned_baseline_section = -1;
			int num_features_baseline = (int) mvect_baseline_features.size();
			int num_unmatched_features_baseline_section_start = num_features_baseline; 
			const double log_2_pi_std_net_std_net = log(2*3.1415*mdouble_net_std*mdouble_net_std); 
			int best_alignment_index = -1; 

			for (int baseline_section = 0; baseline_section < mint_num_baseline_sections; baseline_section++)
			{
				double max_score = -1 * DBL_MAX; 
				double max_score_width = 0; 
				//everything past this section would have remained unmatched.
				for (int section_width = 0; section_width < mint_num_matches_per_baseline_start; section_width++)
				{
					int num_unmatched_features_baseline_section_end;
					if (baseline_section+section_width >= mint_num_baseline_sections)
						num_unmatched_features_baseline_section_end = 0; 
					else
						num_unmatched_features_baseline_section_end = num_unmatched_features_baseline_section_start - mvect_num_features_in_baseline_sections[baseline_section+section_width]; 

					if (num_unmatched_features_baseline_section_end < 0)
						cerr<<baseline_section<<" is where it became less than 0 = "<<num_unmatched_features_baseline_section_end<<endl; 

					int alignment_index = section * mint_num_matches_per_section + baseline_section * mint_num_matches_per_baseline_start + section_width; 

					double alignment_score = marr_alignment_score[alignment_index]; 
					//double unmatched_score = - 1.0 * num_unmatched_features_baseline_section_end*0.5 * (mdouble_net_tolerance/mdouble_net_std) * (mdouble_net_tolerance/mdouble_net_std); 
					//unmatched_score -= num_unmatched_features_baseline_section_end * 0.5 * log_2_pi_std_net_std_net;
					//alignment_score += unmatched_score; 
					//marr_alignment_score[alignment_index] = alignment_score; 
					if (alignment_score > best_score)
					{
						best_score = alignment_score; 
						best_previous_alignment_index = marr_best_previous_index[alignment_index]; 
						best_aligned_baseline_section = baseline_section; 
						best_alignment_index = alignment_index; 
					}
				}
				num_unmatched_features_baseline_section_start -= mvect_num_features_in_baseline_sections[baseline_section]; 

			}

			double msms_section_width = (mdouble_max_baseline_net - mdouble_min_baseline_net)*1.0/ mint_num_baseline_sections;

			double net_start				= mdouble_min_net + (section * (mdouble_max_net - mdouble_min_net)) / mint_num_sections; 
			double net_end					= mdouble_min_net + ((section+1) * (mdouble_max_net - mdouble_min_net)) / mint_num_sections; 
			int    baseline_section_start	= best_aligned_baseline_section; 
			int    baseline_section_end		= baseline_section_start + best_alignment_index % mint_num_matches_per_baseline_start + 1;
			double baseline_start_net		= baseline_section_start * msms_section_width + mdouble_min_baseline_net; 
			double baseline_end_net			= baseline_section_end * msms_section_width + mdouble_min_baseline_net; 

			AlignmentMatch alignment_match;
			alignment_match.Set(net_start,
								net_end, 
								section, 
								mint_num_sections, 
								baseline_start_net, 
								baseline_end_net,
								baseline_section_start, 
								baseline_section_end, 
								best_score, 
								mvect_subsection_match_scores[best_alignment_index]); 
			mvect_alignment_func.clear(); 
			mvect_alignment_func.push_back(alignment_match); 

			while(best_previous_alignment_index >= 0)
			{
				int section_start = (best_previous_alignment_index/mint_num_matches_per_section); // should be the current one - 1
				int section_end   = section_start + 1; 
				mint_percent_done = 100 - (100* section_start) / mint_num_sections; 

				double net_start  = mdouble_min_net + (section_start * (mdouble_max_net - mdouble_min_net)) / mint_num_sections; 
				double net_end    = mdouble_min_net + (section_end * (mdouble_max_net - mdouble_min_net)) / mint_num_sections; 

				int baseline_section_start = (best_previous_alignment_index - (section_start * mint_num_matches_per_section))/mint_num_matches_per_baseline_start; 
				int baseline_section_end   = baseline_section_start + best_previous_alignment_index % mint_num_matches_per_baseline_start +1;
				double baseline_start_net  = baseline_section_start * msms_section_width + mdouble_min_baseline_net; 
				double baseline_end_net    = baseline_section_end * msms_section_width + mdouble_min_baseline_net; 

				alignment_match.Set(net_start, 
									net_end,
									section_start,
									section_end, 
									baseline_start_net, 
									baseline_end_net, 
									baseline_section_start, 
									baseline_section_end, 
									marr_alignment_score[best_previous_alignment_index], 
									mvect_subsection_match_scores[best_previous_alignment_index]); 

				best_previous_alignment_index = marr_best_previous_index[best_previous_alignment_index]; 
				mvect_alignment_func.push_back(alignment_match); 
			}
			sort(mvect_alignment_func.begin(), mvect_alignment_func.end(), &AlignmentMatchCompareBySection1); 
		}

		void LCMSWarp::GetSubsectionMatchScores(std::vector<float> &vectSubsectionMatchScores,
												std::vector<float> &vectAligneeVals, 
												std::vector<float> &vectRefVals,
												bool Standardize ) const
		{
			vectSubsectionMatchScores.clear(); 
			int num_matches = (int) mvect_feature_matches.size(); 
			for (int ms_section = 0; ms_section < mint_num_sections; ms_section++)
			{
				vectAligneeVals.push_back((float)mdouble_min_net + (float) (ms_section * (mdouble_max_net - mdouble_min_net)) / mint_num_sections); 
			}
			for (int msms_section = 0; msms_section < mint_num_baseline_sections; msms_section++)
			{
				vectRefVals.push_back((float)mdouble_min_baseline_net + (float)(msms_section * (mdouble_max_baseline_net - mdouble_min_baseline_net)) / mint_num_baseline_sections); 
			}

			for (int ms_section = 0; ms_section < mint_num_sections; ms_section++)
			{
				for (int msms_section = 0; msms_section < mint_num_baseline_sections; msms_section++)
				{
					double max_score = -1 * DBL_MAX; 
					double max_score_width = 0; 
					for (int msms_section_width = 0; msms_section_width < mint_num_matches_per_baseline_start; msms_section_width++)
					{
						if (msms_section+msms_section_width >= mint_num_baseline_sections)
							continue; 
						int index = ms_section * mint_num_matches_per_section + msms_section * mint_num_matches_per_baseline_start +msms_section_width; 
						if (mvect_subsection_match_scores[index] > max_score)
						{
							max_score = mvect_subsection_match_scores[index]; 
							max_score_width = msms_section_width + 1; 
						}
					}
					vectSubsectionMatchScores.push_back((float)max_score); 
				}
			}
			if (Standardize)
			{
				int index = 0; 
				for (int ms_section = 0; ms_section < mint_num_sections; ms_section++)
				{
					double sumX=0, sumXX=0; 
					int startIndex = index; 
					double realMinScore = DBL_MAX; 
					int numPts = 0; 
					for (int msms_section = 0; msms_section < mint_num_baseline_sections; msms_section++)
					{
						double score = vectSubsectionMatchScores[index]; 
						if (score != mdblMinScore)
						{
							if (score < realMinScore)
							{
								realMinScore = score;
							}
							sumX += score;  
							sumXX += score*score;  
							numPts++; 
						}
						index++; 
					}
					double var = 0; 
					if (numPts > 1)
						var = (sumXX - sumX*sumX/numPts)/(numPts-1); 
					double stdev = 1; 
					double avg = 0; 
					if (numPts >= 1)
						avg = sumX/(numPts);

					if (var != 0)
						stdev = sqrt(var); 

					index=startIndex; 
					for (int msms_section = 0; msms_section < mint_num_baseline_sections; msms_section++)
					{
						double score = vectSubsectionMatchScores[index]; 
						if (score == mdblMinScore)
							score = realMinScore; 
						if (numPts > 1)
							vectSubsectionMatchScores[index] = System::Convert::ToSingle(((score-avg)/stdev));  
						else 
							vectSubsectionMatchScores[index] = 0;  
						index++; 
					}
				}
			}
		}

		void LCMSWarp::PerformAlignment(int num_ms_sections, int contraction_factor, int max_jump)
		{
			mint_num_sections = num_ms_sections;
			mint_max_section_distortion = contraction_factor; 
			mint_max_jump = max_jump; 
			// because each ms section can match ms section of size from 1 division to distortion ^ 2
			// divisions.
			mint_num_baseline_sections = mint_num_sections * mint_max_section_distortion; 
			mint_num_matches_per_baseline_start =  mint_max_section_distortion * mint_max_section_distortion; 
			// each ms section can match mint_num_baseline_sections starting points, each with mint_num_matches_per_baseline_start
			// sections.
			mint_num_matches_per_section = mint_num_baseline_sections * mint_num_matches_per_baseline_start; 

			GenerateCandidateMatches();
			GetMatchProbabilities();
			CalculateAlignmentMatrix();
			CalculateAlignmentFunction(); 
		}

		void LCMSWarp::CalculateAlignmentMatches(stdext::hash_map<int,int> &hashUMCId2MassTagId)
		{
			hashUMCId2MassTagId.clear(); 
			sort(mvect_features.begin(), mvect_features.end(), &SortFeaturesByMass); 
			sort(mvect_baseline_features.begin(), mvect_baseline_features.end(), &SortFeaturesByMass); 

			// now go through each MassTimeFeature and see if the next clsMSMSDBFeature matches to it.
			int feature_index			= 0;	
			int baseline_feature_index	= 0; 
			int num_features			= (int) mvect_features.size(); 
			int num_baseline_features	= (int) mvect_baseline_features.size(); 

			MassTimeFeature feature; 
			FeatureMatch feature_match; 

			mvect_feature_matches.clear();

			double min_match_score	 = - 0.5 * (mdouble_mass_tolerance * mdouble_mass_tolerance) / (mdouble_mass_std * mdouble_mass_std); 
			min_match_score			-= 0.5 * (mdouble_net_tolerance * mdouble_net_tolerance) / (mdouble_net_std * mdouble_net_std); 

			while (feature_index < num_features)
			{
				feature = mvect_features[feature_index];

				double mass_tolerance = feature.mdouble_mono_mass * mdouble_mass_tolerance / 1000000; 

				if (baseline_feature_index == num_baseline_features)
					baseline_feature_index = num_baseline_features -1; 

				while(baseline_feature_index >= 0 && mvect_baseline_features[baseline_feature_index].mdouble_mono_mass > feature.mdouble_mono_mass - mass_tolerance)
					baseline_feature_index--; 
				baseline_feature_index++; 

				int best_match			= INT_MAX; 
				double best_match_score = min_match_score; 
				while(baseline_feature_index < num_baseline_features && 
					mvect_baseline_features[baseline_feature_index].mdouble_mono_mass < feature.mdouble_mono_mass + mass_tolerance)
				{
					if (mvect_baseline_features[baseline_feature_index].mdouble_mono_mass > feature.mdouble_mono_mass - mass_tolerance)
					{
						double net_diff			 = abs(mvect_baseline_features[baseline_feature_index].mdouble_net - feature.mdouble_aligned_net); 
						double mass_diff		 = abs(mvect_baseline_features[baseline_feature_index].mdouble_mono_mass - feature.mdouble_mono_mass) * 1000000.0/feature.mdouble_mono_mass; 
						double match_score		 = - (0.5 * net_diff * net_diff) / (mdouble_net_std * mdouble_net_std); 
						match_score				-=  0.5 * (mass_diff * mass_diff) / (mdouble_mass_std * mdouble_mass_std); 

						if (match_score > best_match_score)
						{
							best_match							= baseline_feature_index; 
							best_match_score					= match_score; 
							feature_match.mint_feature_index	= feature_index; 
							feature_match.mint_feature_index_2	= baseline_feature_index; 
							feature_match.mdouble_net			= feature.mdouble_net; 
							feature_match.mdouble_net_2			= mvect_baseline_features[baseline_feature_index].mdouble_net; 
						}
					}
					baseline_feature_index++; 
				}
				if (best_match != INT_MAX)
				{
					mvect_feature_matches.push_back(feature_match); 
					hashUMCId2MassTagId[feature.mint_id] = mvect_baseline_features[best_match].mint_id; 
				}
				feature_index++; 
			}
			CalculateNETSlopeAndIntercept(); 
		}
		void LCMSWarp::GetErrorHistograms(double mass_bin,
												 double net_bin, 
												 vector<double> &mass_error_bin,
												 vector<int> &mass_error_frequency,
												 vector<double> &net_error_bin,
												 vector<int> &net_error_frequency)
		{	
			vector<FeatureMatch>::iterator iter = mvect_feature_matches.begin();

			vector<double>	mass_errors; 
			vector<double>	net_errors; 		

			/// Reserve the memory so we dont constantly allocate during this process.
			int num_matches = mvect_feature_matches.size();			
			mass_errors.reserve(num_matches);
			net_errors.reserve(num_matches);

			while(iter != mvect_feature_matches.end())
			{
				mass_errors.push_back((*iter).mdouble_ppmMassError);
				net_errors.push_back((*iter).mdouble_netError);
				iter++;
			}
					
			Utilities::CreateHistogram<double>(mass_errors, mass_error_bin, mass_error_frequency, mass_bin) ; 
			Utilities::CreateHistogram<double>(net_errors,  net_error_bin, net_error_frequency, net_bin) ; 
			
		}
		void LCMSWarp::GetStatistics(double *massStd, double *netStd, double *massMu, double *netMu)
		{	
			*netMu = mdblMuNET;
			*massMu = mdblMuMass;
			*netStd = mdouble_net_std;
			*massStd = mdouble_mass_std;						
		}
		/// Calculates the alignment matches post alignment to baseline dataset (dataset or msms AMTDB)
		void LCMSWarp::CalculateAlignmentMatches()
		{
			sort(mvect_features.begin(), mvect_features.end(), &SortFeaturesByMass); 
			sort(mvect_baseline_features.begin(), mvect_baseline_features.end(), &SortFeaturesByMass); 

			// now go through each MassTimeFeature and see if the next clsMSMSDBFeature matches to it.
			int feature_index = 0;	
			int baseline_feature_index = 0; 
			int num_features = (int) mvect_features.size(); 
			int num_baseline_features = (int) mvect_baseline_features.size(); 

			/// Feature value declarations.  
			MassTimeFeature feature; 
			FeatureMatch	feature_match; 

			mvect_feature_matches.clear();

			double min_match_score = -0.5 * (mdouble_mass_tolerance * mdouble_mass_tolerance) / (mdouble_mass_std * mdouble_mass_std); 
			min_match_score -= 0.5 * (mdouble_net_tolerance * mdouble_net_tolerance) / (mdouble_net_std * mdouble_net_std); 

			while (feature_index < num_features)
			{
				feature = mvect_features[feature_index];

				double mass_tolerance = feature.mdouble_mono_mass * mdouble_mass_tolerance / 1000000; 

				if (baseline_feature_index == num_baseline_features)
					baseline_feature_index = num_baseline_features -1; 

				while(baseline_feature_index >= 0 && mvect_baseline_features[baseline_feature_index].mdouble_mono_mass > feature.mdouble_mono_mass - mass_tolerance)
					baseline_feature_index--; 
				baseline_feature_index++; 

				int best_match = INT_MAX; 
				double best_match_score = min_match_score; 
				while(baseline_feature_index < num_baseline_features && 
					mvect_baseline_features[baseline_feature_index].mdouble_mono_mass < feature.mdouble_mono_mass + mass_tolerance )
				{
					if (mvect_baseline_features[baseline_feature_index].mdouble_mono_mass > feature.mdouble_mono_mass - mass_tolerance)
					{
						///
						/// Calculate the mass and net errors 
						///
						double net_diff  = (mvect_baseline_features[baseline_feature_index].mdouble_net - feature.mdouble_aligned_net); 
						double mass_diff = (mvect_baseline_features[baseline_feature_index].mdouble_mono_mass - feature.mdouble_mono_mass) * 1000000.0/feature.mdouble_mono_mass; 

						///
						/// Calculte the match score						
						///
						double match_score = - (0.5 * net_diff * net_diff) / (mdouble_net_std * mdouble_net_std); 
						match_score -=  0.5 * (mass_diff * mass_diff) / (mdouble_mass_std * mdouble_mass_std); 

						///
						/// If the match score is greater than the best match score, then update the holding item.						
						///
						if (match_score > best_match_score)
						{
							best_match							= baseline_feature_index; 
							best_match_score					= match_score; 
							feature_match.mint_feature_index	= feature_index; 
							feature_match.mint_feature_index_2	= baseline_feature_index; 
							feature_match.mdouble_net			= feature.mdouble_net; 
							feature_match.mdouble_netError		= net_diff;
							feature_match.mdouble_ppmMassError	= mass_diff;
							feature_match.mdouble_net_2			= mvect_baseline_features[baseline_feature_index].mdouble_net; 
						}
					}
					baseline_feature_index++; 
				}
				///
				/// If we found a match, add it to the vector of matches.
				///
				if (best_match != INT_MAX)
				{
					mvect_feature_matches.push_back(feature_match); 
				}
				feature_index++; 
			}
			CalculateNETSlopeAndIntercept(); 
		}

		double LCMSWarp::GetTransformedNet(double val)
		{

			int alignment_func_len = (int) mvect_alignment_func.size(); 
			if (val < mvect_alignment_func[0].mdouble_net_start)
			{
				double net_start = mvect_alignment_func[0].mdouble_net_start; 
				double net_start_baseline = mvect_alignment_func[0].mdouble_net_start_2; 
				double net_end = mvect_alignment_func[0].mdouble_net_end; 
				double net_end_baseline = mvect_alignment_func[0].mdouble_net_end_2; 

				double net_transformed = ((val - net_start) * (net_end_baseline - net_start_baseline)) /(net_end - net_start) + net_start_baseline;
				return net_transformed;
			}
			else if (val > mvect_alignment_func[alignment_func_len-1].mdouble_net_end)
			{
				double net_start = mvect_alignment_func[alignment_func_len-1].mdouble_net_start; 
				double net_end = mvect_alignment_func[alignment_func_len-1].mdouble_net_end; 
				double net_start_baseline = mvect_alignment_func[alignment_func_len-1].mdouble_net_start_2; 
				double net_end_baseline = mvect_alignment_func[alignment_func_len-1].mdouble_net_end_2; 

				double net_transformed = ((val - net_end) * (net_end_baseline - net_start_baseline)) /(net_end - net_start) + net_end_baseline;
				return net_transformed;
			}

			int ms_section_index = 0;
			AlignmentMatch match; 

			for (; ms_section_index < alignment_func_len; ms_section_index++)
			{
				match = mvect_alignment_func[ms_section_index]; 
				if (val <= match.mdouble_net_end && val >= match.mdouble_net_start)
					break; 
			}

			double net_start = match.mdouble_net_start; 
			double net_end = match.mdouble_net_end; 

			double net_start_baseline = match.mdouble_net_start_2; 
			double net_end_baseline = match.mdouble_net_end_2; 

			int baseline_section_start = match.mint_section_start_2; 
			int baseline_section_end = match.mint_section_end_2; 

			double net_transformed = ((val - net_start) * (net_end_baseline - net_start_baseline)) / (net_end - net_start) + net_start_baseline;
			return net_transformed; 
		}

		void LCMSWarp::GetTransformedNets()
		{
			// now go through each MassTimeFeature and see if the next MassTimeFeature matches to it.
			int feature_index = 0; 
			int num_features = (int) mvect_features.size(); 

			MassTimeFeature feature; 
			int alignment_func_len = (int) mvect_alignment_func.size(); 
			map<int,int> map_section_to_index; 

			for (int i = 0; i < (int)mvect_alignment_func.size(); i++)
			{
				map_section_to_index.insert(pair<int,int>(mvect_alignment_func[i].mint_section_start, i)); 
			}

			for( int feature_index = 0; feature_index < num_features; feature_index++)
			{
				feature = mvect_features[feature_index];
				if (feature.mdouble_net < mvect_alignment_func[0].mdouble_net_start)
				{
					double net_start			= mvect_alignment_func[0].mdouble_net_start; 
					double net_start_baseline	= mvect_alignment_func[0].mdouble_net_start_2; 
					double net_end				= mvect_alignment_func[0].mdouble_net_end; 
					double net_end_baseline		= mvect_alignment_func[0].mdouble_net_end_2; 

					double ms_net_transformed = ((feature.mdouble_net - net_start) * (net_end_baseline - net_start_baseline)) /(net_end - net_start) + net_start_baseline;
					mvect_features[feature_index].mdouble_aligned_net = ms_net_transformed; 
					continue; 
				}
				else if (feature.mdouble_net > mvect_alignment_func[alignment_func_len-1].mdouble_net_end)
				{
					double net_start			= mvect_alignment_func[alignment_func_len-1].mdouble_net_start; 
					double net_end				= mvect_alignment_func[alignment_func_len-1].mdouble_net_end; 
					double net_start_baseline	= mvect_alignment_func[alignment_func_len-1].mdouble_net_start_2; 
					double net_end_baseline		= mvect_alignment_func[alignment_func_len-1].mdouble_net_end_2; 

					double net_transformed = ((feature.mdouble_net - net_end) * (net_end_baseline - net_start_baseline)) /(net_end - net_start) + net_end_baseline;
					mvect_features[feature_index].mdouble_aligned_net = net_transformed; 
					continue; 
				}

				int ms_section_1 = System::Convert::ToInt32(((feature.mdouble_net - mdouble_min_net) * mint_num_sections) /(mdouble_max_net - mdouble_min_net));
				if (ms_section_1 >= mint_num_sections)
					ms_section_1 = mint_num_sections - 1; 

				int ms_section_index = map_section_to_index[ms_section_1]; 

				double net_start			= mvect_alignment_func[ms_section_index].mdouble_net_start; 
				double net_end				= mvect_alignment_func[ms_section_index].mdouble_net_end; 

				double net_start_baseline	= mvect_alignment_func[ms_section_index].mdouble_net_start_2; 
				double net_end_baseline		= mvect_alignment_func[ms_section_index].mdouble_net_end_2; 

				int baseline_section_start	= mvect_alignment_func[ms_section_index].mint_section_start_2; 
				int baseline_section_end	= mvect_alignment_func[ms_section_index].mint_section_end_2; 

				double net_transformed = ((feature.mdouble_net - net_start) * (net_end_baseline - net_start_baseline)) / (net_end - net_start) + net_start_baseline;
				mvect_features[feature_index].mdouble_aligned_net = net_transformed; 
			}
		}

		void LCMSWarp::GetMatchDeltas(vector<double> &ppms)
		{
			mint_percent_done	= 10; 
			GenerateCandidateMatches(); 

			mint_percent_done	= 20; 
			int num_matches		= (int) mvect_feature_matches.size();
			ppms.clear(); 

			for (int match_num = 0; match_num < num_matches; match_num++)
			{
				FeatureMatch match		= mvect_feature_matches[match_num]; 
				int ms_index			= match.mint_feature_index; 
				int baseline_index		= match.mint_feature_index_2; 

				double mass_diff		= mvect_features[ms_index].mdouble_mono_mass - mvect_baseline_features[baseline_index].mdouble_mono_mass; 
				double mass_ppm_diff	= (mass_diff * 1000.0 * 1000.0) / mvect_features[ms_index].mdouble_mono_mass;
				ppms.push_back(mass_ppm_diff); 
			}
			mint_percent_done = 100; 
		}

		enmLCMSWarpCalibrationType LCMSWarp::GetCalibrationType() 
		{
			return menmCalibrationType; 
		}

		void LCMSWarp::SetCalibrationType(enmLCMSWarpCalibrationType calib_type) 
		{
			menmCalibrationType = calib_type; 
		}

		void LCMSWarp::PerformMZMassErrorRegression()
		{
			// first copy all the mzs and the mass errors into a vector of RegressionPts
			vector<Regression::RegressionPts>	vect_calibrations; 
			Regression::RegressionPts			calib_match; 
			int num_matches = mvect_feature_matches.size(); 
			
			for (int match_num = 0; match_num < num_matches; match_num++)
			{
				FeatureMatch feature_match			= mvect_feature_matches[match_num]; 
				MassTimeFeature feature				= mvect_features[feature_match.mint_feature_index]; 
				MassTimeFeature baseline_feature	= mvect_baseline_features[feature_match.mint_feature_index_2]; 
				double ppm							= (feature.mdouble_mono_mass - baseline_feature.mdouble_mono_mass) / baseline_feature.mdouble_mono_mass * 1000000; 
				double mz							= feature.mdouble_mz;
				double net_diff						= baseline_feature.mdouble_net - feature.mdouble_aligned_net; 
				calib_match.mdouble_x				= mz; 
				calib_match.mdouble_net_error		= net_diff; 
				calib_match.mdouble_mass_error		= ppm; 


				vect_calibrations.push_back(calib_match) ;
			}

			mobj_mz_recalibration.CalculateRegressionFunction(vect_calibrations); 

			int num_features		= mvect_features.size(); 
			for(int feature_num = 0; feature_num < num_features; feature_num++)
			{
				double mz			= mvect_features[feature_num].mdouble_mz; 
				double mass			= mvect_features[feature_num].mdouble_mono_mass_original; 
				double ppm_shift	= mobj_mz_recalibration.GetPredictedValue(mz);
				double new_mass		= mass - (mass * ppm_shift)/ 1000000; 
				mvect_features[feature_num].mdouble_mono_mass_calibrated = new_mass;
				mvect_features[feature_num].mdouble_mono_mass = new_mass;
			}

			for (int match_num = 0; match_num < num_matches; match_num++)
			{
				FeatureMatch feature_match		 = mvect_feature_matches[match_num]; 
				MassTimeFeature feature			 = mvect_features[feature_match.mint_feature_index]; 
				MassTimeFeature baseline_feature = mvect_baseline_features[feature_match.mint_feature_index_2]; 

				double ppm		= (feature.mdouble_mono_mass - baseline_feature.mdouble_mono_mass) / baseline_feature.mdouble_mono_mass * 1000000; 
				double mz		= feature.mdouble_mz;
				double net_diff = baseline_feature.mdouble_net - feature.mdouble_aligned_net; 
			}
		}

		void LCMSWarp::PerformScanMassErrorRegression()
		{
			// first copy all the mzs and the mass errors into a vector of RegressionPts
			vector<Regression::RegressionPts> vect_calibrations; 
			int num_matches = mvect_feature_matches.size(); 
			Regression::RegressionPts calib_match; 

			for (int match_num = 0; match_num < num_matches; match_num++)
			{
				FeatureMatch feature_match			= mvect_feature_matches[match_num]; 
				MassTimeFeature feature				= mvect_features[feature_match.mint_feature_index]; 
				MassTimeFeature baseline_feature	= mvect_baseline_features[feature_match.mint_feature_index_2]; 
				double ppm							= (feature.mdouble_mono_mass - baseline_feature.mdouble_mono_mass) / baseline_feature.mdouble_mono_mass * 1000000; 
				double net							= (double) feature.mdouble_net;
				double net_diff						= baseline_feature.mdouble_net - feature.mdouble_aligned_net; 
				calib_match.mdouble_x				= net; 
				calib_match.mdouble_net_error		= net_diff; 
				calib_match.mdouble_mass_error		= ppm; 
				vect_calibrations.push_back(calib_match) ;
			}

			mobj_net_recalibration.CalculateRegressionFunction(vect_calibrations); 

			int num_features = mvect_features.size(); 
			for(int feature_num = 0; feature_num < num_features; feature_num++)
			{
				double mz			= mvect_features[feature_num].mdouble_mz; 
				double net			= mvect_features[feature_num].mdouble_net; 
				double mass			= mvect_features[feature_num].mdouble_mono_mass; 
				double power_val	= 1; 
				double ppm_shift	= mobj_net_recalibration.GetPredictedValue(net);
				double new_mass		= mass - mass * ppm_shift/1000000; 
				mvect_features[feature_num].mdouble_mono_mass_calibrated	= new_mass;
				mvect_features[feature_num].mdouble_mono_mass				= new_mass;
			}

			for (int match_num = 0; match_num < num_matches; match_num++)
			{
				FeatureMatch feature_match = mvect_feature_matches[match_num]; 
				MassTimeFeature feature = mvect_features[feature_match.mint_feature_index]; 
				MassTimeFeature baseline_feature = mvect_baseline_features[feature_match.mint_feature_index_2]; 
				double ppm = (feature.mdouble_mono_mass - baseline_feature.mdouble_mono_mass) / baseline_feature.mdouble_mono_mass * 1000000; 
				double net = feature.mdouble_net;
				double net_diff = baseline_feature.mdouble_net - feature.mdouble_aligned_net; 
			}

		}

		void LCMSWarp::PerformMassCalibration()
		{
			switch (menmCalibrationType)
			{
				case MZ_REGRESSION:
					PerformMZMassErrorRegression(); 
					break; 
				case SCAN_REGRESSION:
					PerformScanMassErrorRegression(); 
					break; 
				case BOTH:
					PerformMZMassErrorRegression(); 
					PerformScanMassErrorRegression(); 
					break; 
			}
		}


		void LCMSWarp::CalculateStandardDeviations()
		{
			// lets calculate the mass and net standard deviation based on the assumption
			// that we will have a mixture of normal and uniform errors for both the 
			// NET and the MASS values. 
			// calculate the mass standard deviation and the net standard deviation here.
			int num_matches = (int) mvect_feature_matches.size(); 
			// if there are only more than 6 matches that we will calculate a relevant mdouble_mass_std.
			if (num_matches > 6)
			{
				FeatureMatch match; 
				vector<double> vect_mass_deltas;
				vector<double> vect_net_deltas; 
				vect_mass_deltas.reserve(num_matches); 
				vect_net_deltas.reserve(num_matches); 
				for (int match_num = 0; match_num < num_matches; match_num++)
				{
					match = mvect_feature_matches[match_num]; 
					MassTimeFeature feature = mvect_features[match.mint_feature_index]; 
					MassTimeFeature baseline_feature = mvect_baseline_features[match.mint_feature_index_2]; 
					double current_mass_delta = ((baseline_feature.mdouble_mono_mass - feature.mdouble_mono_mass)* 1000000)/feature.mdouble_mono_mass; 
					double current_net_delta = baseline_feature.mdouble_net - feature.mdouble_aligned_net; 
					
					vect_mass_deltas.push_back(current_mass_delta); 
					vect_net_deltas.push_back(current_net_delta); 
				}
				mdblNormalProb=0;
				mdblU=0; 
				mdblMuMass=0; 
				mdblMuNET=0; 
				Utilities::MathUtils::TwoDEM(vect_mass_deltas, vect_net_deltas, mdblNormalProb, mdblU, mdblMuMass, mdblMuNET, mdouble_mass_std, mdouble_net_std); 
				//std::cerr<<"Mass Error Mean ="<<mdblMuMass<<" NET Error Mean = "<<mdblMuNET<<" Mass Stdev ="<<mdouble_mass_std<<" NET Stdev = "<<mdouble_net_std<<std::endl; 
				//Regression::NormUnifEM mixture_model_resolver; 
				//mixture_model_resolver.CalculateDistributions(vect_mass_deltas); 
				//mdouble_mass_std = mixture_model_resolver.GetStd(); 

				//mixture_model_resolver.CalculateDistributions(vect_net_deltas); 
				//mdouble_net_std = mixture_model_resolver.GetStd(); 

				// Similarly calculate net standard deviations.
			}

		}

		double LCMSWarp::GetPPMShiftFromMZ(double mz)
		{
			if (menmCalibrationType == MZ_REGRESSION || menmCalibrationType == BOTH)
				return mobj_mz_recalibration.GetPredictedValue(mz); 
			return 0; 
		}

		double LCMSWarp::GetPPMShiftFromNET(double net)
		{
			if (menmCalibrationType == SCAN_REGRESSION || menmCalibrationType == BOTH)
				return mobj_net_recalibration.GetPredictedValue(net); 
			return 0; 
		}

		double LCMSWarp::GetPPMShift(double mz, double net)
		{
			double ppm_shift = 0; 
			switch (menmCalibrationType)
			{
				case MZ_REGRESSION:
					ppm_shift = mobj_mz_recalibration.GetPredictedValue(mz); 
					break; 
				case SCAN_REGRESSION:
					ppm_shift = mobj_net_recalibration.GetPredictedValue(net); 
					break; 
				case BOTH:
					ppm_shift = mobj_mz_recalibration.GetPredictedValue(mz); 
					ppm_shift += mobj_net_recalibration.GetPredictedValue(net); 
					break; 
			}
			return ppm_shift; 
		}

		void LCMSWarp::GetSlopeAndIntercept(double &slope, double &intercept, double &rsquare, vector<Regression::RegressionPts> &regression_pts)
		{
			double SumY, SumX, SumXY, SumXX, SumYY; 
			SumY = 0; 
			SumX = 0; 
			SumXY = 0;
			SumXX = 0;
			SumYY = 0;

			int num_pts = regression_pts.size(); 
			for (int index = 0; index < num_pts; index++)
			{
				Regression::RegressionPts pt = regression_pts[index]; 

				SumX = SumX + pt.mdouble_x; 
				SumY = SumY + pt.mdouble_mass_error; 
				SumXX = SumXX + pt.mdouble_x * pt.mdouble_x; 
				SumXY = SumXY + pt.mdouble_x * pt.mdouble_mass_error; 
				SumYY = SumYY + pt.mdouble_mass_error * pt.mdouble_mass_error; 
			}
			slope = (num_pts * SumXY - SumX * SumY) / (num_pts * SumXX - SumX * SumX); 
			intercept = (SumY - slope * SumX) / num_pts;

			double temp = (num_pts * SumXY - SumX * SumY) / sqrt((num_pts*SumXX - SumX * SumX)*(num_pts*SumYY - SumY * SumY)); 
			rsquare = temp * temp; 
		}
		void LCMSWarp::GetTransformedNets(std::vector<int> &vectIds, std::vector<double> &vectNETs)
		{
			vectIds.clear(); 
			vectNETs.clear();
			int numPoints = (int) mvect_features.size(); 
			for (int pointNum = 0; pointNum < numPoints; pointNum++)
			{
				MassTimeFeature feature = mvect_features[pointNum]; 
				vectIds.push_back(feature.mint_id); 
				vectNETs.push_back(feature.mdouble_aligned_net); 
			}
		}
		void LCMSWarp::GetNETSlopeAndIntercept(double &slope, double &intercept) const
		{
			slope = mdblNETSlope; 
			intercept = mdblNETIntercept; 
		}
		void LCMSWarp::CalculateNETSlopeAndIntercept()
		{
			// copy the start_section of all the alignment matches.
			std::vector<double> vectStartNets; 
			for (std::vector<FeatureMatch>::const_iterator iter = mvect_feature_matches.begin();
				iter != mvect_feature_matches.end(); iter++)
			{
				vectStartNets.push_back((*iter).mdouble_net); 
			}
			sort(vectStartNets.begin(), vectStartNets.end()); 

			int numPoints = (int) vectStartNets.size(); 
			if (numPoints == 0)
			{
				mdblNETSlope = 0; 
				mdblNETIntercept = 0; 
				return; 
			}
			int startSection = System::Convert::ToInt32(((vectStartNets[numPoints/4]-mdouble_min_net)*mint_num_sections)/(mdouble_max_net-mdouble_min_net)); 
			int endSection = System::Convert::ToInt32(((vectStartNets[(3*numPoints)/4]-mdouble_min_net)*mint_num_sections)/(mdouble_max_net-mdouble_min_net)); 
			if (startSection >= mint_num_sections)
				startSection = mint_num_sections-1; 
			if (endSection >= mint_num_sections)
				endSection = mint_num_sections-1; 

			std::cerr<<"Finding linear slopes. Using start section ="<<startSection<<" and end section = "<<endSection<<std::endl; 

			double SumY, SumX, SumXY, SumXX, SumYY; 
			SumY = 0; 
			SumX = 0; 
			SumXY = 0;
			SumXX = 0;
			SumYY = 0;

			int num_pts = 0; 
			for (int section = startSection; section <= endSection; section++)
			{
				double max_score = -1 * DBL_MAX; 
				double y = 0; 
				for (int baseline_section = 0; baseline_section < mint_num_baseline_sections; baseline_section++)
				{
					for (int section_width = 0; section_width < mint_num_matches_per_baseline_start; section_width++)
					{
						int alignment_index = section * mint_num_matches_per_section + baseline_section * mint_num_matches_per_baseline_start + section_width; 
						if (mvect_subsection_match_scores[alignment_index]> max_score)
						{
							max_score = mvect_subsection_match_scores[alignment_index]; 
							y = baseline_section; 
						}
					}
				}

				double net = (section*(mdouble_max_net-mdouble_min_net))/mint_num_sections + mdouble_min_net; 
				double aligned_net = (y*(mdouble_max_baseline_net-mdouble_min_baseline_net))/mint_num_baseline_sections + mdouble_min_baseline_net; 
				SumY += aligned_net; 
				SumX += net; 
				SumXY += (net*aligned_net);
				SumXX += (net * net); 
				SumYY += (aligned_net * aligned_net);
				num_pts++; 
			}
			mdblNETSlope = (num_pts * SumXY - SumX * SumY) / (num_pts * SumXX - SumX * SumX); 
			mdblNETIntercept = (SumY - mdblNETSlope * SumX) / num_pts;

			double temp = (num_pts * SumXY - SumX * SumY) / sqrt((num_pts*SumXX - SumX * SumX)*(num_pts*SumYY - SumY * SumY)); 
			mdblNETLinearRSq = temp * temp; 
		}

		void LCMSWarp::GetAlignmentMatchesScansAndNet(std::vector<double> &vectMatchScans, std::vector<double> &vectMatchNets, 
			std::vector<double> &vectMatchAlignedNet) const
		{
			for (std::vector<FeatureMatch>::const_iterator iter = mvect_feature_matches.begin();
				iter != mvect_feature_matches.end(); iter++)
			{
				vectMatchAlignedNet.push_back(mvect_features[(*iter).mint_feature_index].mdouble_aligned_net); 
				vectMatchScans.push_back((*iter).mdouble_net); 
				vectMatchNets.push_back((*iter).mdouble_net_2); 
			}
		}
		
			/*/////////////////////////////////////////////////////////////////////////////////
				GetResiduals
					Purpose: Returns the residuals calculated between matches of baseline to 
								alignee for linear net, custom net, and linear - custom net 
								against scan number.
					Arguments:
							NET Numbers (x-values)
							Linear Net = Baseline - Predicted Linear
							Custom Net = Baseline - Custom Net
							Linear - Custom = Custom - Predicted Linear

							where:
								Predicted Linear = slope * scan  + intercept (y=mx+b)
			/////////////////////////////////////////////////////////////////////////////////*/
			void LCMSWarp::GetResiduals(std::vector<double> & vectNet,
							  std::vector<double> & vectMZ,
							  std::vector<double> & vectLinearNet,
							  std::vector<double> & vectCustomNet,
							  std::vector<double> & vectLinearCustomNet,
							  std::vector<double> & vectMassError,
							  std::vector<double> & vectMassErrorCorrected)
			{

				/// Allocate space for the vectors
				int count = mvect_feature_matches.size();
				vectNet.reserve(count);
				vectMZ.reserve(count);
				vectLinearNet.reserve(count);
				vectCustomNet.reserve(count);
				vectLinearCustomNet.reserve(count);
				vectMassError.reserve(count);
				vectMassErrorCorrected.reserve(count);

				/// 
				/// Using an iterator to move through the vector, find all the NET residuals for the mass and time features
				/// that matched.
				///
				for (std::vector<FeatureMatch>::const_iterator iter = mvect_feature_matches.begin();
																				iter != mvect_feature_matches.end(); 
																				iter++)
				{	
					MassTimeFeature feature = mvect_features[(*iter).mint_feature_index];
					double mtNET			= feature.mdouble_aligned_net - (*iter).mdouble_netError;					
					double predictedLinear  = mdblNETSlope * (*iter).mdouble_net_2 + mdblNETIntercept;					
					double massError	    = (*iter).mdouble_ppmMassError;
					double scanNumber       = (*iter).mdouble_net;
					
					/// NET 
					vectNet.push_back(scanNumber);
					/// 
					/// Here we take the aligned net and subtract the error?
					/// Then we subtract the predicted linear from this?  It makes no sense.  
					/// What we want to do is say what is the predicted linear, and subtract the previous net value.
					/// to get the residual linear net value.
					/// 
					vectLinearNet.push_back(feature.mdouble_aligned_net - predictedLinear);
					/// 
					/// VIPER uses this: mtNET - feature.mdouble_aligned_net);
					/// To calculate the aligned (custom) net alignment.  But this is just saying -netError.  
					///
					vectCustomNet.push_back((*iter).mdouble_netError);
					vectLinearCustomNet.push_back(feature.mdouble_aligned_net - predictedLinear);					

					/// Gather the PPM shift to calculate the right calibrated residual.
					double ppmShift = 0.0;				
					if (mbln_use_mass) 
					{
						ppmShift = GetPPMShift(feature.mdouble_mz, scanNumber);
					}
					/// Mass Residuals
					vectMZ.push_back(feature.mdouble_mz);					
					vectMassError.push_back(massError);
					
					vectMassErrorCorrected.push_back(massError - ppmShift);
				}
			}
	}
}