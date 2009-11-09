#pragma once
#include "CentralRegression.h" 
#include "LSQSplineRegression.h" 
#include "NaturalCubicSplineRegression.h" 

namespace MultiAlignEngine
{
	namespace Regression
	{
		enum enmRegressionType { CENTRAL=0, LSQ, HYBRID };
		class CombinedRegression
		{
			enmRegressionType menmRegressionType; 
			bool mbln_lsq_failed; 
		public:
			CentralRegression mobj_central_regression; 
			LSQSplineRegression mobj_lsq_regression; 
			NaturalCubicSplineRegression mobj_nat_cubic_regression; 
			CombinedRegression(void);
			~CombinedRegression(void);
			void SetCentralRegressionOptions(int num_x_bins, int num_y_bins, int num_jumps, double ztolerance, enmRegressionType reg_type); 
			void SetLSQOptions(int num_knots, double outlier_zscore); 
			void CalculateRegressionFunction(vector<RegressionPts> &calib_matches); 
			void PrintRegressionFunction(char *file_name); 
			double GetPredictedValue(double x); 
		};
	}
}