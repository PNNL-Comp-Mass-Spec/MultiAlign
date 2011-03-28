#pragma once
#include "RegressionPts.h"
#include "Matrix.h" 
#include <vector>
using namespace std; 

namespace MultiAlignEngine
{
	namespace Regression
	{
		class LSQSplineRegression
		{
			vector<Regression::RegressionPts> mvect_pts; 
			void PreprocessCopyData(vector<Regression::RegressionPts> &vect_pts); 

			void Clear(); 
		public:
			// maximum order of spline regression supported = 16; 
			static const int MAX_ORDER = 16; 
			int mint_order; 
			int mint_num_knots;
			double mdouble_min_x; 
			double mdouble_max_x; 
			double marr_coeffs[512]; 
			LSQSplineRegression(void): mint_num_knots(0), mint_order(2){};
			~LSQSplineRegression(void) {};
			void SetOptions(int num_knots) { mint_num_knots = num_knots; }; 
			bool CalculateLSQRegressionCoefficients(int order, vector<Regression::RegressionPts> &vect_pts);
			void PrintRegressionFunction(char *file_name);
			double GetPredictedValue(double x); 
		};
	}
}