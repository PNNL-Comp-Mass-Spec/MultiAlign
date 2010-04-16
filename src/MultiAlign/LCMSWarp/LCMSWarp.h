#pragma once
#include "FeatureMatch.h" 
#include "MassTimeFeature.h" 
#include "AlignmentMatch.h" 
#include "CombinedRegression.h"
#include "Utilities.h" 
#include "Interpolation.h"
#include <vector>
#include <map>
#include <iostream>
#include <fstream>
#include <hash_map> 

using namespace std;

namespace MultiAlignEngine
{
	namespace Alignment
	{
		enum enmLCMSWarpCalibrationType
		{
			MZ_REGRESSION=0,
			SCAN_REGRESSION,
			BOTH
		}; 
		


		class LCMSWarp
		{
	#pragma warning(disable:4251)
			vector<double> mvect_temp_feature_best_delta;
			vector<int> mvect_temp_feature_best_index;

			vector<int> mvect_section_unique_feature_indices;
			vector<int> mvect_num_features_in_sections; 
			vector<int> mvect_num_features_in_baseline_sections; 

	#pragma warning(default:4251)
			Utilities::Interpolation mobj_interpolation; 
			double *marr_alignment_score; 
			int *marr_best_previous_index; 

			double mdouble_net_std; 
			
			const double mdblMinMassNETLikelihood; 
			double mdouble_min_baseline_net; 
			double mdouble_max_baseline_net; 
			bool mbln_use_mass; 
			// decides whether or not promiscous matches are kept in the scoring function
			// for alignment to a mass tag database this can be used safely because 
			// the mass tag db will not have split umcs. But for MS to MS alignments
			// it is best to keep this false or all split umcs will match to first instance.
			bool mbln_keep_promiscous_matches; 


			double mdouble_max_net; 
			double mdouble_min_net; 

			// this is used to control the granularity of the msms section size when 
			// comparing against ms sections. The number of sections in the msms will be
			// # of sections in ms * mint_max_section_distortion.
			// Thus each section of the ms can be compared to msms section which are 
			// 1/mint_max_section_distortion to mint_max_section_distortion times
			// the ms section size of the chromatographic run.


			void ComputeSectionMatch(int ms_section, vector<FeatureMatch> &section_matching_features, 
				double min_net, double max_net); 

			Regression::CombinedRegression mobj_mz_recalibration; 
			Regression::CombinedRegression mobj_net_recalibration; 


			void PerformMZMassErrorRegression(); 
			void PerformScanMassErrorRegression(); 

			int mint_mass_cal_num_delta_bins; 
			int mint_mass_cal_num_slices; 
			int mint_mass_cal_num_jump; 

			double mdblMinScore; 

			enmLCMSWarpCalibrationType menmCalibrationType; 

			// this is the mass window around which the mass tolerance is applied. 
			double mdouble_mass_window; 


			int mint_max_promiscous_umc_matches; 

			int mint_num_matches_per_section; 

			int mint_num_sections; 
			int mint_num_baseline_sections; 
			int mint_max_jump; 

			int mint_percent_done;
			double mdouble_mass_tolerance;
			double mdouble_net_tolerance; 

			double mdouble_mass_std; 
			int mint_max_section_distortion; 
			int mint_num_matches_per_baseline_start; 

			double mdblNormalProb; 
			double mdblU; 
			double mdblMuMass; 
			double mdblMuNET; 
			#pragma warning(disable:4251)
			vector<AlignmentMatch> mvect_alignment_func; 
			vector<MassTimeFeature> mvect_features; 
			vector<MassTimeFeature> mvect_baseline_features; 
			vector<FeatureMatch> mvect_feature_matches; 
			vector<double> mvect_subsection_match_scores; 
			#pragma warning(default:4251)
			double mdblNETSlope; 
			double mdblNETIntercept; 
			double mdblNETLinearRSq; 
			// this slope and intercept is calculated using the likelihood score
			// in mvect_subsection_match_scores. 
			// The range of scans used is the range which covers the start scan 
			// of the Q2 and the end scan of the Q3 of the nets of the matched features.
			void CalculateNETSlopeAndIntercept(); 
			double GetCurrentlyStoredSectionMatchScore(double max_miss_zscore, 
				int &num_unique_features, int &num_unmatched_features_in_section); 

			inline double GetMatchLikelihood(double massDelta, double netDelta)
			{
				double massZ = massDelta/mdouble_mass_std; 
				double netZ = netDelta/mdouble_net_std; 
				double norm_prob = exp(-0.5 * (massZ * massZ + netZ * netZ)) /(2 * 3.14159 * mdouble_net_std*mdouble_mass_std); 
				double likelihood = (norm_prob * mdblNormalProb + (1-mdblNormalProb)* mdblU);  
				if (likelihood < mdblMinMassNETLikelihood)
					likelihood = mdblMinMassNETLikelihood; 
				return likelihood; 
			}

		public:


			void SetNETOptions(int num_ms_sections, int contraction_factor, int max_jump, int max_promiscuity)
			{
				mint_num_sections = num_ms_sections;
				mint_max_section_distortion = contraction_factor; 
				mint_max_jump = max_jump; 
				// because each ms section can match ms section of size from 1 division to distortion ^ 2
				// divisions.
				mint_num_baseline_sections = mint_num_sections * mint_max_section_distortion; 
				mint_num_matches_per_baseline_start =  mint_max_section_distortion * mint_max_section_distortion; 
				mint_num_matches_per_section = mint_num_baseline_sections * mint_num_matches_per_baseline_start; 

				mint_max_promiscous_umc_matches = max_promiscuity; 
			}

			void SetMassCalOptions(double mass_window, int num_mass_delta_bin, int num_slices, 
				int mass_jump, double ztolerance, bool use_lsq)
			{
				mdouble_mass_window = mass_window; 

				mint_mass_cal_num_delta_bins = num_mass_delta_bin; 
				mint_mass_cal_num_slices = num_slices; 
				mint_mass_cal_num_jump = mass_jump; 

				Regression::enmRegressionType reg_type = Regression::CENTRAL; 
				if (use_lsq)
					reg_type = Regression::HYBRID; 
				mobj_mz_recalibration.SetCentralRegressionOptions(mint_mass_cal_num_slices, 
					mint_mass_cal_num_delta_bins, mint_mass_cal_num_jump, ztolerance, reg_type); 
				mobj_net_recalibration.SetCentralRegressionOptions(mint_mass_cal_num_slices, 
					mint_mass_cal_num_delta_bins, mint_mass_cal_num_jump, ztolerance, reg_type); 

			}

			void SetTolerances(double mass_tolerance, double net_tolerance)
			{
				mdouble_mass_tolerance = mass_tolerance; 
				mdouble_net_tolerance = net_tolerance; 
			}


			void SetMassCalLSQOptions(int num_knots, double outlier_zscore)
			{
				mobj_mz_recalibration.SetLSQOptions(num_knots, outlier_zscore); 
				mobj_net_recalibration.SetLSQOptions(num_knots, outlier_zscore); 
			}

			double GetNETIntercept()
			{
				return mdblNETIntercept;
			}
			double GetNETSlope()
			{
				return mdblNETSlope;
			}
			double GetNETLinearRSquared()
			{
				return mdblNETLinearRSq;				
			}

			double GetMassCalibrationWindow()
			{
				return mdouble_mass_window; 
			}
			double GetMassTolerance()
			{
				return mdouble_mass_tolerance; 
			}
			void SetMassTolerance(double val)
			{
				mdouble_mass_tolerance = val; 
			}

			void ResetPercentComplete()
			{
				mint_percent_done = 0; 
			}

			int GetPercentComplete()
			{
				return mint_percent_done; 
			}

			enmLCMSWarpCalibrationType GetCalibrationType();
			void SetCalibrationType(enmLCMSWarpCalibrationType calib_type); 
			void GenerateCandidateMatches(); 
			void CalculateAlignmentFunction(); 
			void CalculateAlignmentMatrix(); 
			void GetMatchProbabilities(); 
			void GetStatistics(double *massStd, double *netStd, double *massMu, double *netMu);
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
			void GetResiduals(std::vector<double> & vectNet,
							  std::vector<double> & vectMZ,
							  std::vector<double> & vectLinearNet,
							  std::vector<double> & vectCustomNet,
							  std::vector<double> & vectLinearCustomNet,
							  std::vector<double> & vectMassError,
							  std::vector<double> & vectMassErrorCorrected);
							  
							  
							  

			void GetAlignmentFunction(	std::vector<double> &vectAligneeNET,
										std::vector<double> &vectReferenceNET) const; 

			// clears associated vectors.. leaves the mvectors of ms and msms features alone.
			void ClearAllButData(); 
			// clears the input features
			void ClearInputData(); 
			void Clear(); 

			LCMSWarp(void);
			~LCMSWarp(void);
			void PrintSubsectionMatchScores(ostream &out); 
			void PrintAlignmentScores(ostream &out); 
			void PrintCandidateMatches(char *file_name); 
			void PrintAlignmentFunction(ostream &out); 
			void PerformAlignment(int num_ms_sections, int contraction_factor, int max_jump); 

			int GetNumSections1() { return mint_num_sections; } 
			int GetNumSections2() { return mint_num_baseline_sections; } 
			int GetNumMatchesPerSection1() { return mint_num_matches_per_section; } 
			int GetNumMatchesPerMSStart() { return mint_num_matches_per_baseline_start; } 

			int GetNumCandidateMatches() { return (int) mvect_feature_matches.size(); }; 
				
			void GetTransformedNets();
			double GetTransformedNet(double val); 

			void CalculateAlignmentMatches();
			/// Returns an array of mass and net error histogram values.
			void GetErrorHistograms(double mass_bin,
												 double net_bin, 
												 vector<double> &mass_error_bin,
												 vector<int> &mass_error_frequency,
												 vector<double> &net_error_bin,
												 vector<int> &net_error_frequency);

			void SetFeatures(std::vector<MassTimeFeature> &vectFeatures)
			{
				mvect_features.clear(); 
				mvect_features.insert(mvect_features.begin(), vectFeatures.begin(), vectFeatures.end()); 
			}

			void GetFeatureCalibratedMassesAndAlignedNETs(std::vector <int> &umcIndices, std::vector<double> &umcCalibratedMasses, 
				std::vector<double> &umcAlignedNETS); 
			void GetFeatureCalibratedMassesAndAlignedNETs(std::vector <int> &umcIndices, std::vector<double> &umcCalibratedMasses, 
				std::vector<double> &umcAlignedNETS, std::vector<int> &umcAlignedScans, int minScan, int maxScan); 

			void SetReferenceFeatures(std::vector<MassTimeFeature> &vectFeatures)
			{
				mvect_baseline_features.clear(); 
				mvect_baseline_features.insert(mvect_baseline_features.begin(), vectFeatures.begin(), vectFeatures.end()); 
			}

			inline void AddFeature(MassTimeFeature &feature) 
			{
				mvect_features.push_back(feature);
			}
			inline void AddReferenceFeature(MassTimeFeature &feature)
			{
				mvect_baseline_features.push_back(feature);
			}

			void GetMatchDeltas(vector<double> &ppms); 

			void GetBounds(double *min_baseline_net, double *max_baseline_net, double *min_net, double *max_net)
			{
				*min_baseline_net = mdouble_min_baseline_net; 
				*max_baseline_net = mdouble_max_baseline_net; 
				*min_net = mdouble_min_net; 
				*max_net = mdouble_max_net; 
			}

			void PerformMassCalibration(); 

			void SetFeatureMassesToOriginal()
			{
				int num_features = (int) mvect_features.size(); 
				for (int feature_num = 0; feature_num < num_features; feature_num++)
				{
					mvect_features[feature_num].mdouble_mono_mass = mvect_features[feature_num].mdouble_mono_mass_original; 
				}
			}

			void CalculateStandardDeviations(); 
			void SetDefaultStdevs(); 
			void UseMassAndNetScore(bool use)
			{
				mbln_use_mass = use; 
			}
			double GetPPMShift(double mz, double net); 
			double GetPPMShiftFromMZ(double mz); 
			double GetPPMShiftFromNET(double net); 
			void GetSlopeAndIntercept(double &slope, double &intercept, double & rsquare, vector<Regression::RegressionPts> &regression_pts); 
			// Gets the best subsection match scores for each subsection.
			void GetSubsectionMatchScores(	std::vector<float> &vectSubsectionMatchScores,
											std::vector<float> &vectAligneeVals, 
											std::vector<float> &vectRefVals,
											bool Standardize=true ) const; 
			void SetMassAndNETStdev(double massStdev, double netStdev)
			{
				mdouble_mass_std = massStdev; 
				mdouble_net_std = netStdev; 
			}
			void CalculateAlignmentMatches(stdext::hash_map<int,int> &hashUMCId2MassTagId); 		
			void GetTransformedNets(std::vector<int> &vectIds, std::vector<double> &vectNETs);
			void GetNETSlopeAndIntercept(double &slope, double &intercept) const;
			void GetAlignmentMatchesScansAndNet(std::vector<double> &vectMatchScans,
												std::vector<double> &vectMatchNets,
												std::vector<double> &vectMatchAlignedNet) const;
		};
	}
}