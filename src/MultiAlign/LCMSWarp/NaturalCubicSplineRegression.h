#pragma once
#include "RegressionPts.h"
#include "Matrix.h" 
#include <vector>
using namespace std; 

namespace MultiAlignEngine
{
	namespace Regression
	{
		class NaturalCubicSplineRegression
		{

			vector<RegressionPts> mvect_pts; 
			vector<double> mvect_interval_start; 

			void PreprocessCopyData(vector<RegressionPts> vect_pts); 
			void Clear(); 
		public:
			int mint_num_knots;
			double mdouble_min_x; 
			double mdouble_max_x; 
			double marr_coeffs[512]; 
			NaturalCubicSplineRegression(void): mint_num_knots(2){};
			~NaturalCubicSplineRegression(void) {};
			void SetOptions(int num_knots) { mint_num_knots = num_knots; }; 
			bool CalculateLSQRegressionCoefficients(vector<RegressionPts> &vect_pts);
			void PrintRegressionFunction(char *file_name);
			double GetPredictedValue(double x); 
			void PrintPoints(char *file_name); 
		};
	}
}