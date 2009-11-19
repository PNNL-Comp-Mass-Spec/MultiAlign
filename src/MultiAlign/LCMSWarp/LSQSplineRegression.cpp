

#include <algorithm>
#include "LSQSplineRegression.h"
#include "Matrix.h" 
#include <float.h>
#include <fstream>
#include <iostream> 
#include <math.h>
using namespace std; 

namespace MultiAlignEngine
{
	namespace Regression
	{
		void LSQSplineRegression::Clear()
		{
			mvect_pts.clear(); 
		}
		void LSQSplineRegression::PreprocessCopyData(vector<RegressionPts> &vect_pts)
		{
			// first find minimum and maximums
			int num_pts = vect_pts.size(); 
			mdouble_min_x = DBL_MAX; 
			mdouble_max_x = -1 * DBL_MAX; 
			for (int pt_num = 0; pt_num < num_pts; pt_num++)
			{
				RegressionPts pt = vect_pts[pt_num]; 
				if (pt.mdouble_x < mdouble_min_x)
					mdouble_min_x = pt.mdouble_x; 
				if (pt.mdouble_x > mdouble_max_x)
					mdouble_max_x = pt.mdouble_x; 
			}
			mvect_pts.insert(mvect_pts.begin(), vect_pts.begin(), vect_pts.end()); 
		}

		// input points are [x, y].
		// order specifies order of regression line. order = 0 is constant, order = 1 is linear, order = 2 is quadratic,
		// and so on.. maximum order supported is MAX_ORDER.
		bool LSQSplineRegression::CalculateLSQRegressionCoefficients(int order, vector<RegressionPts> &vect_pts)
		{
			Clear(); 
			mint_order = order; 


			if (order > MAX_ORDER)
			{
				order = MAX_ORDER; 
				mint_order = MAX_ORDER; 
			}

			PreprocessCopyData(vect_pts); 

			MATRIX *A, *b, *c, *b_interp; 
			MATRIX *Atranspose; 
			MATRIX *Aintermediate1, *Aintermediate2, *Aintermediate3; 


			int num_pts = mvect_pts.size(); 

			A = matrix_allocate(num_pts, mint_order+mint_num_knots+1, sizeof(double)); 
			b = matrix_allocate(num_pts, 1, sizeof(double)); 

			double **ptr_matrix_A = (double **) A->ptr;
			double **ptr_matrix_b = (double **) b->ptr;


			for (int pt_num = 0; pt_num < num_pts; pt_num++)
			{
				RegressionPts calib = mvect_pts[pt_num]; 
				double coeff = 1; 
				ptr_matrix_A[pt_num][0] = coeff; 
				for (int col_num = 1; col_num <= mint_order; col_num++)
				{
					coeff *= calib.mdouble_x; 
					ptr_matrix_A[pt_num][col_num] = coeff; 
				}


				if (mint_num_knots > 0 && mint_order > 0)
				{
					int x_interval =  System::Convert::ToInt32(((mint_num_knots+1) *(calib.mdouble_x - mdouble_min_x)) / (mdouble_max_x - mdouble_min_x) ); 
					if (x_interval >= mint_num_knots + 1)
						x_interval = mint_num_knots;

					// remember that the (order + 1)th basis matrix corresponds to the first knot pts
					// which start when x_interval >= 1.
					for (int col_num = mint_order + 1; col_num <= mint_order + x_interval; col_num++)
					{
						double x_interval_start = mdouble_min_x + ((col_num - mint_order) * (mdouble_max_x - mdouble_min_x))/(mint_num_knots+1); 
						ptr_matrix_A[pt_num][col_num] = pow(calib.mdouble_x - x_interval_start, mint_order); 
					}
					for (int col_num = mint_order + x_interval + 1; col_num <= mint_order + mint_num_knots; col_num++)
					{
						ptr_matrix_A[pt_num][col_num] = 0; 
					}
				}

				ptr_matrix_b[pt_num][0] = calib.mdouble_mass_error; 
			}

				
			//PrintMatrix(A, "c:\\A.csv"); 
			Atranspose = matrix_transpose(A); 
			Aintermediate1 = matrix_mult(Atranspose, A);
			//PrintMatrix(Aintermediate1, "c:\\intermediate1.csv"); 
			Aintermediate2 = matrix_invert(Aintermediate1); 
			if (Aintermediate2 == NULL)
				return false; 
			Aintermediate3 = matrix_mult(Aintermediate2, Atranspose); 

			c = matrix_mult(Aintermediate3, b); 
			b_interp = matrix_mult(A, c); 

			for (int col_num = 0; col_num <= mint_order + mint_num_knots; col_num++)
			{
				marr_coeffs[col_num] = *((double *)&c->ptr[col_num][0]); 
			}


			matrix_free(b); 
			matrix_free(c); 
			matrix_free(Aintermediate1); 
			matrix_free(Aintermediate2); 
			matrix_free(Aintermediate3); 
			matrix_free(Atranspose); 
			matrix_free(A); 
			return true; 
		}

		void LSQSplineRegression::PrintRegressionFunction(char *file_name)
		{
		}

		double LSQSplineRegression::GetPredictedValue(double x) 
		{
			double power_n = 1; 
			double val = marr_coeffs[0]; 
			for (int power = 1; power <= mint_order; power++)
			{
				power_n *= x; 
				val += marr_coeffs[power] * power_n;  
			}
			// now get the x_interval.
			if (mint_num_knots > 0 && mint_order > 0)
			{
				int x_interval =  System::Convert::ToInt32(((mint_num_knots+1) *(x - mdouble_min_x)) / (mdouble_max_x - mdouble_min_x)); 
				if (x_interval >= mint_num_knots + 1)
					x_interval = mint_num_knots;

				for (int col_num = mint_order + 1; col_num <= mint_order + x_interval; col_num++)
				{
					double x_interval_start = mdouble_min_x + ((col_num - mint_order) * (mdouble_max_x - mdouble_min_x))/(mint_num_knots+1); 
					val += pow(x - x_interval_start, mint_order) * marr_coeffs[col_num]; 
				}
			}

			return val; 
		}
	}
}