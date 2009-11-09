#include "StdAfx.h"
#include <fstream> 
#include <float.h> 
#include <math.h>
#include ".\centralregression.h"
using namespace std; 

namespace MultiAlignEngine
{
	namespace Regression
	{
		CentralRegression::CentralRegression(void)
		{
			mint_num_x_bins = 0; 
			mint_num_y_bins = 0; 
			mint_num_jumps = 0; 
			mdouble_tolerance = 0.8; // 5 standard deviations.
			mdouble_outlier_zscore = mdouble_tolerance; 
			mint_min_section_pts = 5; 
		}

		CentralRegression::~CentralRegression(void)
		{
		}

		void CentralRegression::SetOptions(int num_x_bins, int num_y_bins, int num_jumps, double ztolerance)
		{
			mint_num_x_bins = num_x_bins; 
			mint_num_y_bins = num_y_bins; 
			mint_num_jumps = num_jumps; 
			mdouble_tolerance = ztolerance; 
			mdouble_outlier_zscore = ztolerance; 

			mint_num_section_matches = mint_num_y_bins * (2*num_jumps+1); 

			mvect_match_scores.clear(); 
			mvect_alignment_scores.clear(); 
			mvect_best_previous_index.clear (); 

		}

		void CentralRegression::SetOutlierZScore(double outlier_zscore)
		{
			mdouble_outlier_zscore = outlier_zscore; 
		}

		void CentralRegression::CalculateMinMax()
		{
			int num_pts = mvect_pts.size(); 

			mdouble_min_x = DBL_MAX; 
			mdouble_max_x = -1 * DBL_MAX; 

			mdouble_min_y = DBL_MAX; 
			mdouble_max_y = -1 * DBL_MAX; 

			for (int i = 0; i < num_pts; i++)
			{
				RegressionPts pt = mvect_pts[i]; 
				if (pt.mdouble_x < mdouble_min_x)
					mdouble_min_x = pt.mdouble_x; 
				if (pt.mdouble_x > mdouble_max_x)
					mdouble_max_x = pt.mdouble_x; 
				if (pt.mdouble_mass_error < mdouble_min_y)
					mdouble_min_y = pt.mdouble_mass_error; 
				if (pt.mdouble_mass_error > mdouble_max_y)
					mdouble_max_y = pt.mdouble_mass_error; 
			}
		}


		void CentralRegression::CalculateSectionStdAndCount(int interval_num)
		{
			vector<double> vect_pts;

			int num_pts = mvect_pts.size(); 
			double x_interval = (mdouble_max_x - mdouble_min_x)/ mint_num_x_bins;

			for (int pt_num = 0; pt_num < num_pts; pt_num++)
			{
				RegressionPts pt = mvect_pts[pt_num]; 
				int section_num = System::Convert::ToInt32((pt.mdouble_x - mdouble_min_x) / x_interval); 
				if (section_num == mint_num_x_bins)
					section_num = mint_num_x_bins - 1; 
				if (interval_num == section_num)
				{
					mvect_count[section_num]++; 
					vect_pts.push_back(pt.mdouble_mass_error); 
				}
			}
			if (mvect_count[interval_num] > mint_min_section_pts)
			{
				mobj_norm_unif_em.CalculateDistributions(vect_pts); 
				mvect_std_y[interval_num] = mobj_norm_unif_em.GetStd(); 
				if (mvect_std_y[interval_num] != 0)
				{
					double tolerance = mvect_std_y[interval_num] * mdouble_tolerance; 
					mvect_section_tolerance[interval_num] = tolerance; 
	//				mvect_section_tolerance[interval_num] = 6; 

					// for now the formula below might look over complicated and just 
					// simplify to mdouble_tolerance * mdouble_tolerance
					// but in the future we might be using an absolute tolerance in the previous line.
					double mismatch_score = (tolerance * tolerance) / (mvect_std_y[interval_num]*mvect_std_y[interval_num]); 
	//				double mismatch_score = mdouble_tolerance * mdouble_tolerance; 
					mvect_section_mismatch_score[interval_num] = mismatch_score; 
				}
				else
				{
					mvect_section_mismatch_score[interval_num] = mdouble_tolerance * mdouble_tolerance; 
					mvect_section_tolerance[interval_num] = 0; 
				}

			}
			else
			{
				mvect_std_y[interval_num] = 0.1; 
			}
		}

		void CentralRegression::CalculateSectionsStd()
		{

			mvect_count.reserve(mint_num_x_bins); 
			mvect_std_y.reserve(mint_num_x_bins); 
			mvect_std_y.clear(); 

			for (int interval = 0; interval < mint_num_x_bins; interval++)
			{
				mvect_std_y.push_back(0); 
				mvect_count.push_back(0); 
				mvect_section_mismatch_score.push_back(0); 
				mvect_section_tolerance.push_back(0); 
				CalculateSectionStdAndCount(interval); 
			}
		}

		void CentralRegression::Clear()
		{
			mvect_match_scores.clear(); 
			mvect_alignment_scores.clear(); 
			mvect_best_previous_index.clear(); 
			mvect_count.clear();
			mvect_pts.clear(); 
			mvect_std_y.clear(); 
			mmap_alignment_function.clear(); 
			mvect_section_mismatch_score.clear(); 
			mvect_section_tolerance.clear(); 
		}

		void CentralRegression::SetUnmatchedScoreMatrix()
		{
			mvect_match_scores.clear(); 
			// assign each sections scores to minimum for that section.
			// For each possible matching sections the minimum score would correspond to the 
			// situation that all points in the section lie outside the tolerance.

			// for now assume that the tolerance is in z-score units. 
			for (int x_section = 0; x_section < mint_num_x_bins; x_section++)
			{
				double section_mismatch_score = mvect_section_mismatch_score[x_section] * mvect_count[x_section]; 

				for (int section_match_num = 0; section_match_num < mint_num_section_matches; 
						section_match_num++)
				{
					mvect_match_scores.push_back(section_mismatch_score); 
				}
			}
		}

		void CentralRegression::CalculateScoreMatrix()
		{
			mvect_match_scores.reserve(mint_num_x_bins * mint_num_section_matches); 

			// Now we need to calculate the score matrix for all possible 
			// score blocks. The way this is done is that for every x section
			// all possible y segments between y_interest - mint_num_jumps to 
			// y_interest + mint_num_jumps are scored. 

			// first set the unmatched score.
			SetUnmatchedScoreMatrix(); 

			double y_interval_size = (mdouble_max_y - mdouble_min_y)/ mint_num_y_bins;
			double x_interval_size = (mdouble_max_x - mdouble_min_x)/ mint_num_x_bins;
			// now for each point that is seen, add the supporting score
			// to appropriate section match. 
			int num_pts = mvect_pts.size(); 
			for (int pt_num = 0; pt_num < num_pts; pt_num++)
			{
				RegressionPts pt = mvect_pts[pt_num]; 
				int x_section = System::Convert::ToInt32((pt.mdouble_x - mdouble_min_x) / x_interval_size); 
				if (x_section == mint_num_x_bins)
					x_section = mint_num_x_bins - 1; 

				// if the point belongs to a section where the minimum # of points is not 
				// met, we ignore if.
				if (mvect_count[x_section] < mint_min_section_pts || mvect_std_y[x_section] == 0)
				{
					continue; 
				}


				double y_tolerance = mvect_section_tolerance[x_section]; 

				// numeric issues...
				int y_interval = System::Convert::ToInt32((0.0001 + (pt.mdouble_mass_error - mdouble_min_y) / y_interval_size)); 

				if (y_interval == mint_num_y_bins)
					y_interval = mint_num_y_bins - 1; 

				// now go through each section match that this point would contribute to.
				int min_y_start = System::Convert::ToInt32(y_interval - y_tolerance / y_interval_size  ); 
				int max_y_start = System::Convert::ToInt32(y_interval + y_tolerance / y_interval_size  ); 

				double section_mismatch_score = mvect_section_mismatch_score[x_section]; 

				double x_fraction = (pt.mdouble_x - mdouble_min_x) / x_interval_size - x_section; 
				double y_estimated = 0;
				for (int y_from = min_y_start; y_from <= max_y_start; y_from++)
				{
					if (y_from < 0)
						continue; 
					if (y_from >= mint_num_y_bins)
						break; 
					for (int y_to = y_from - mint_num_jumps; y_to <= y_from + mint_num_jumps; y_to++)
					{
						if (y_to < 0)
							continue; 
						if (y_to >= mint_num_y_bins)
							break; 

						// now assuming linear piecewise transformation, calculate
						// the estimated y value.
						y_estimated = (y_from + (y_to - y_from) * x_fraction) * y_interval_size + mdouble_min_y; 
						double y_delta = pt.mdouble_mass_error - y_estimated; 

						// make sure that this point is indeed in the linear range to effect the score
						if (abs(y_delta) > y_tolerance)
							continue; 

						double match_score = (y_delta * y_delta) / (mvect_std_y[x_section] * mvect_std_y[x_section] ); 
						int jump = y_to - y_from + mint_num_jumps; 
						int section_index = x_section * mint_num_section_matches + y_from * (2*mint_num_jumps+1) + jump; 
						double current_match_score = mvect_match_scores[section_index];
						mvect_match_scores[section_index] = current_match_score - section_mismatch_score + match_score; 
					}
				}

			}
		}

		void CentralRegression::CalculateAlignmentMatrix()
		{
			mvect_best_previous_index.clear();
			mvect_alignment_scores.clear() ; 

			mvect_alignment_scores.reserve((mint_num_x_bins+1) * mint_num_y_bins); 
			mvect_best_previous_index.reserve ((mint_num_x_bins+1) * mint_num_y_bins); 

			for (int y_section = 0; y_section < mint_num_y_bins; y_section++)
			{
				int index = y_section; 
				mvect_best_previous_index.push_back(-2); 
				mvect_alignment_scores.push_back(0); 
			}

			for (int x_section = 1; x_section <= mint_num_x_bins; x_section++)
			{
				for (int y_section = 0; y_section < mint_num_y_bins; y_section++)
				{
					int index = x_section * mint_num_y_bins + y_section; 
					mvect_best_previous_index.push_back(-1); 
					mvect_alignment_scores.push_back(DBL_MAX); 
				}
			}

			for (int x_section = 1; x_section <= mint_num_x_bins; x_section++)
			{
				//System::Console::WriteLine(S"x-section:{0}", x_section.ToString());

				for (int y_section = 0; y_section < mint_num_y_bins; y_section++)
				{
					//System::Console::WriteLine(y_section);
					int index = x_section * mint_num_y_bins + y_section; 
					double best_alignment_score = DBL_MAX; 

					// The following two loops are the same. The only reason this is arranged in this 
					// manner is so that if all jumps score equally, the jump kept is the one that 
					// corresponds to no mass jump.
					for (int jump = mint_num_jumps; jump < 2*mint_num_jumps + 1; jump++)
					{
						int y_section_from = y_section - jump + mint_num_jumps; 
						if (y_section_from < 0)
							break; 
						int previous_alignment_index = (x_section-1) * mint_num_y_bins + y_section_from; 
						int previous_match_index = (x_section-1) * mint_num_section_matches 
											+ y_section_from * (2*mint_num_jumps+1) + jump; 
						double previous_alignment_score = mvect_alignment_scores[previous_alignment_index]; 
						double previous_match_score = mvect_match_scores[previous_match_index]; 
						if (previous_alignment_score + previous_match_score < best_alignment_score)
						{
							best_alignment_score = previous_alignment_score + previous_match_score; 
							mvect_best_previous_index[index] = previous_alignment_index; 
							mvect_alignment_scores[index] = best_alignment_score; 
						}
					}
					for (int jump = 0; jump < mint_num_jumps; jump++)
					{
						//System::Console::WriteLine(S"jump = {0}", jump.ToString());
						int y_section_from = y_section - jump + mint_num_jumps; 
						if (y_section_from < 0)
							break; 
						int previous_alignment_index = (x_section-1) * mint_num_y_bins + y_section_from; 
						int previous_match_index = (x_section-1) * mint_num_section_matches 
											+ y_section_from * (2*mint_num_jumps+1) + jump;
						if ((previous_alignment_index > mvect_alignment_scores.size() - 1) || 
							(previous_match_index > mvect_match_scores.size() - 1))
							break;
						double previous_alignment_score = mvect_alignment_scores[previous_alignment_index]; 
						double previous_match_score = mvect_match_scores[previous_match_index]; 
						if (previous_alignment_score + previous_match_score < best_alignment_score)
						{
							best_alignment_score = previous_alignment_score + previous_match_score; 
							mvect_best_previous_index[index] = previous_alignment_index; 
							mvect_alignment_scores[index] = best_alignment_score; 
						}
					}
				}
			}
		}

		void CentralRegression::CalculateRegressionFunction()
		{
			mmap_alignment_function.clear(); 
			// simply start at the last section best score and trace backwards. 
			double best_score = DBL_MAX; 
			int best_previous_index; 
			int best_y_shift = mint_num_y_bins/2; 
			// lets start at the last section where the # of points is at least 
			// mint_min_section_pts)
			int x_section = mint_num_x_bins; 

			while(x_section >= 1)
			{
				if (mvect_count[x_section-1] >= mint_min_section_pts)
				{
					for (int y_section = 0; y_section < mint_num_y_bins; y_section++)
					{
						int index = x_section * mint_num_y_bins + y_section; 
						double ascore = mvect_alignment_scores[index]; 
						if (ascore < best_score)
						{
							best_score = ascore; 
							best_y_shift = y_section; 
							best_previous_index = mvect_best_previous_index[index]; 
						}
					}
					break; 
				}
				else
				{
					x_section--; 
				}
			}

			// if we get here then if x_section >= 1 we found at least one section that helped us
			// with the function. So take that shift and move it to the transformation function till
			// the end. 
			for (int i = x_section; i <= mint_num_x_bins; i++)
			{
				mmap_alignment_function.insert(pair<int, int>(i,best_y_shift));
			}
			while(x_section > 0)
			{
				x_section--; 
				int y_shift = best_previous_index % mint_num_y_bins; 
				mmap_alignment_function.insert(pair<int,int>(x_section, y_shift)); 
				best_previous_index = mvect_best_previous_index[best_previous_index]; 
			}

		}

		void CentralRegression::RemoveOutliersAndCopy(vector<RegressionPts> &calib_matches)
		{
			mvect_pts.insert(mvect_pts.begin(), calib_matches.begin(), calib_matches.end()); 
		}

		void CentralRegression::CalculateRegressionFunction(vector<RegressionPts> 
																&calib_matches)
		{
			Clear(); 
			RemoveOutliersAndCopy(calib_matches); 

			// First step here is to find the boundaries (min, max in x, y domains). 
			CalculateMinMax(); 

			if (mdouble_min_y == mdouble_max_y)
				// constant answers
			{
				for (int x_section = 0; x_section < mint_num_x_bins; x_section++)
				{
					mmap_alignment_function.insert(pair<int,int>(x_section,0)); 
				}
				return; 
			}
			CalculateSectionsStd(); 

			CalculateScoreMatrix();
			//PrintScoreMatrix("c:\\score_matrix.csv"); 
			CalculateAlignmentMatrix(); 
			//PrintAlignmentScoreMatrix("c:\\alignment_matrix.csv"); 
			CalculateRegressionFunction(); 
			//PrintRegressionFunction("c:\\mass_align_func.csv"); 

		}

		void CentralRegression::PrintScoreMatrix(char *file_name)
		{
			double y_interval_size = (mdouble_max_y - mdouble_min_y)/ mint_num_y_bins;
			double x_interval_size = (mdouble_max_x - mdouble_min_x)/ mint_num_x_bins;

			ofstream fout(file_name); 
			double x_start = mdouble_min_x;
			for (int x_section = 0; x_section < mint_num_x_bins; x_section++)
			{
				double x_stop = x_start + x_interval_size; 
				for (int y_section = 0; y_section < mint_num_y_bins; y_section++)
				{
					for (int jump = 0; jump < 2*mint_num_jumps+1; jump++)
					{
						int index = x_section * mint_num_section_matches + y_section * (2*mint_num_jumps+1) + jump; 
						fout<<x_start<<","<<x_stop<<","; 
						fout<<y_section*y_interval_size+mdouble_min_y<<","<<(y_section+jump-mint_num_jumps)*y_interval_size+mdouble_min_y<<","<<mvect_match_scores[index]<<"\n"; 
					}
				}
				x_start = x_stop; 
			}
			fout.close(); 
		}

		void CentralRegression::PrintAlignmentScoreMatrix(char *file_name)
		{
			double y_interval_size = (mdouble_max_y - mdouble_min_y)/ mint_num_y_bins;
			double x_interval_size = (mdouble_max_x - mdouble_min_x)/ mint_num_x_bins;

			ofstream fout(file_name); 
			double x_start = mdouble_min_x;
			for (int x_section = 0; x_section < mint_num_x_bins + 1; x_section++)
			{
				double x_stop = x_start + x_interval_size; 
				for (int y_section = 0; y_section < mint_num_y_bins; y_section++)
				{
						int index = x_section * mint_num_y_bins + y_section; 
	//					fout<<x_section<<","<<y_section<<","<<y_section-jump<<mvect_match_scores[index]<<"\n"; 
						fout<<x_start<<","<<x_stop<<","; 
						fout<<y_section*y_interval_size+mdouble_min_y<<","<<mvect_alignment_scores[index]<<"\n"; 
				}
				x_start = x_stop; 
			}
			fout.close(); 
		}

		void CentralRegression::PrintRegressionFunction(char *file_name) 
		{
			double y_interval_size = (mdouble_max_y - mdouble_min_y)/ mint_num_y_bins;
			double x_interval_size = (mdouble_max_x - mdouble_min_x)/ mint_num_x_bins;

			ofstream fout(file_name); 
			double x_start = mdouble_min_x; 
			for (map<int,int>::iterator iter = mmap_alignment_function.begin(); iter != mmap_alignment_function.end();
				iter++)
			{
				double x_stop = x_start + x_interval_size; 
				int x_section = (*iter).first; 
				int y_section = (*iter).second; 
				fout<<x_start<<","<<y_section*y_interval_size+mdouble_min_y<<"\n"; 
				x_start = x_stop; 
			}
			fout.close(); 
		}



		void CentralRegression::PrintPoints(char *file_name)
		{
			ofstream fout(file_name); 
			for (int pt_num = 0; pt_num < (int)mvect_pts.size(); pt_num++)
			{
				RegressionPts pt = mvect_pts[pt_num]; 
				fout<<pt.mdouble_x<<","<<pt.mdouble_mass_error<<"\n"; 
			}
			fout.close(); 
		}

		void CentralRegression::RemoveRegressionOutliers()
		{
			double x_interval_size = (mdouble_max_x - mdouble_min_x)/ mint_num_x_bins;
			vector<RegressionPts> vect_temp_pts; 
			int num_pts = mvect_pts.size(); 
			for (int pt_num = 0; pt_num < num_pts; pt_num++)
			{
				RegressionPts pt = mvect_pts[pt_num]; 
				int interval_num = System::Convert::ToInt32((pt.mdouble_x - mdouble_min_x) / x_interval_size); 
				if (interval_num == mint_num_x_bins)
					interval_num = mint_num_x_bins - 1; 
				double std_y = mvect_std_y[interval_num]; 
				double val = GetPredictedValue(pt.mdouble_x); 
				double delta = (val - pt.mdouble_mass_error) / std_y; 
				if (abs(delta) < mdouble_outlier_zscore) 
				{
					vect_temp_pts.push_back(pt); 
				}
			}
			mvect_pts.clear(); 
			mvect_pts.insert(mvect_pts.begin(), vect_temp_pts.begin(), vect_temp_pts.end()); 
		}

		double CentralRegression::GetPredictedValue(double x)
		{
			double y_interval_size = (mdouble_max_y - mdouble_min_y)/ mint_num_y_bins;
			double x_interval_size = (mdouble_max_x - mdouble_min_x)/ mint_num_x_bins;
			int x_section = System::Convert::ToInt32((x - mdouble_min_x) / x_interval_size); 
			if (x_section >= mint_num_x_bins)
				x_section = mint_num_x_bins - 1; 
			if (x_section < 0)
			{
				x_section = 0; 
				// return the first shift.
				map<int,int>::iterator iter = mmap_alignment_function.begin(); 
				return mdouble_min_y + (*iter).second * y_interval_size; 
			}

			double x_fraction = (x - mdouble_min_x) / x_interval_size - x_section; 

			map<int,int>::iterator iter = mmap_alignment_function.find(x_section); 
			int y_section_from = (*iter).second; 
			int y_section_to = y_section_from; 

			if (x_section < mint_num_x_bins-1)
			{
				iter++; 
				y_section_to = (*iter).second;  
			}

			double y_pred = x_fraction * y_interval_size * (y_section_to - y_section_from)
										+ y_section_from * y_interval_size + mdouble_min_y; 

			return y_pred; 
		}


	}
}