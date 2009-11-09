#include "StdAfx.h"
#include ".\combinedregression.h"
#include <fstream> 
using namespace std; 

namespace MultiAlignEngine
{
	namespace Regression
	{
		CombinedRegression::CombinedRegression(void)
		{
			menmRegressionType = HYBRID; 
			mbln_lsq_failed = false; 
		}

		CombinedRegression::~CombinedRegression(void)
		{
		}

		void CombinedRegression::SetLSQOptions(int num_knots, double outlier_zscore)
		{
			mobj_nat_cubic_regression.SetOptions(num_knots); 
			mobj_lsq_regression.SetOptions(num_knots); 
			mobj_central_regression.SetOutlierZScore(outlier_zscore); 
		}

		void CombinedRegression::SetCentralRegressionOptions(int num_x_bins, int num_y_bins, int num_jumps, double reg_ztolerance, enmRegressionType reg_type) 
		{
			mobj_central_regression.SetOptions(num_x_bins, num_y_bins, num_jumps, reg_ztolerance); 
			menmRegressionType = reg_type; 
		}


		void CombinedRegression::CalculateRegressionFunction(vector<RegressionPts> &calib_matches)
		{
			//ofstream fout("c:\\matches.csv"); 
			//int num_pts = calib_matches.size(); 
			//for (int i = 0; i < num_pts; i++)
			//{
			//	fout<<calib_matches[i].mdouble_x<<","<<calib_matches[i].mdouble_mass_error<<"\n"; 
			//}
			//fout.close(); 

			switch (menmRegressionType)
			{
					case CENTRAL : 
						mobj_central_regression.CalculateRegressionFunction(calib_matches); 
						break; 
					default:
						mobj_central_regression.CalculateRegressionFunction(calib_matches); 
						mobj_central_regression.RemoveRegressionOutliers(); 
	//					mobj_lsq_regression.CalculateLSQRegressionCoefficients(mobj_central_regression.mint_num_x_bins-1, order, mobj_central_regression.mvect_pts); 
						mbln_lsq_failed = !mobj_nat_cubic_regression.CalculateLSQRegressionCoefficients(mobj_central_regression.mvect_pts); 
						break; 
			}
		}

		double CombinedRegression::GetPredictedValue(double x)
		{
			switch (menmRegressionType)
			{
					case CENTRAL : 
						return mobj_central_regression.GetPredictedValue(x); 
						break; 
					default:
						//return mobj_lsq_regression.GetPredictedValue(x); 
						if (!mbln_lsq_failed)
							return mobj_nat_cubic_regression.GetPredictedValue(x); 
						else
							return mobj_central_regression.GetPredictedValue(x); 
						break; 
			}
			return 0; 
		}

		void CombinedRegression::PrintRegressionFunction(char *file_name)
		{
			switch (menmRegressionType)
			{
					case CENTRAL : 
						mobj_central_regression.PrintRegressionFunction(file_name); 
						break; 
					default:
							mobj_lsq_regression.PrintRegressionFunction(file_name); 
						break; 
			}
		}
	}
}