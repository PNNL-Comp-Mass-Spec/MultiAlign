#include "StdAfx.h"

#include <algorithm>
#include ".\NaturalCubicSplineRegression.h"
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
		void NaturalCubicSplineRegression::Clear()
		{
			mvect_pts.clear(); 
			mvect_interval_start.clear(); 
		}

		void NaturalCubicSplineRegression::PreprocessCopyData(vector<RegressionPts> vect_pts)
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

			for (int i = 0; i <= mint_num_knots; i++)
			{
				double val = (i * (mdouble_max_x - mdouble_min_x)) / (mint_num_knots+1) + mdouble_min_x;
				mvect_interval_start.push_back(val); 
			}
		}

		// input points are [x, y].
		// order specifies order of regression line. order = 0 is constant, order = 1 is linear, order = 2 is quadratic,
		// and so on.. maximum order supported is MAX_ORDER.
		bool NaturalCubicSplineRegression::CalculateLSQRegressionCoefficients(vector<RegressionPts> &vect_pts)
		{
			Clear(); 
			if (mint_num_knots < 2)
				// need at least two knots for a natural cubic spline.
				return false; 

			if (vect_pts.size() == 0)
				return false; 

			PreprocessCopyData(vect_pts); 

			MATRIX *A, *b, *c, *b_interp; 
			MATRIX *Atranspose; 
			MATRIX *Aintermediate1, *Aintermediate2, *Aintermediate3; 


			int num_pts = mvect_pts.size(); 

			A = matrix_allocate(num_pts, mint_num_knots, sizeof(double)); 
			b = matrix_allocate(num_pts, 1, sizeof(double)); 

			double **ptr_matrix_A = (double **) A->ptr;
			double **ptr_matrix_b = (double **) b->ptr;


			double interval_width = (mdouble_max_x - mdouble_min_x) /(mint_num_knots+1);

			for (int pt_num = 0; pt_num < num_pts; pt_num++)
			{
				RegressionPts pt = mvect_pts[pt_num]; 
				double coeff = 1; 
				ptr_matrix_A[pt_num][0] = coeff; 
				ptr_matrix_A[pt_num][1] = pt.mdouble_x; 

				int interval_num = System::Convert::ToInt32(((pt.mdouble_x - mdouble_min_x) * (mint_num_knots+1)) / (mdouble_max_x - mdouble_min_x)); 
				if (interval_num > mint_num_knots)
					interval_num = mint_num_knots; 

				double d_Kminus1 = 0; 
				if (pt.mdouble_x > mvect_interval_start[mint_num_knots-1])
				{
					d_Kminus1 = pow(pt.mdouble_x - mvect_interval_start[mint_num_knots-1], 3); 
					if (pt.mdouble_x > mvect_interval_start[mint_num_knots])
						d_Kminus1 -= pow(pt.mdouble_x - mvect_interval_start[mint_num_knots], 3); 
					d_Kminus1 /= interval_width; 
				}

				for (int k = 1; k <= mint_num_knots-2; k++)
				{
					double d_kminus1 = 0; 
					if (pt.mdouble_x > mvect_interval_start[k])
					{
						d_kminus1 = pow(pt.mdouble_x - mvect_interval_start[k], 3); 
						if (pt.mdouble_x > mvect_interval_start[mint_num_knots])
							d_kminus1 -= pow(pt.mdouble_x - mvect_interval_start[mint_num_knots], 3); 
						d_kminus1 /= interval_width; 
					}

					ptr_matrix_A[pt_num][k+1] = d_kminus1 - d_Kminus1; 
				}

				ptr_matrix_b[pt_num][0] = pt.mdouble_mass_error; 
			}

				
			//PrintMatrix(A, "c:\\A.csv"); 
			Atranspose = matrix_transpose(A); 
			Aintermediate1 = matrix_mult(Atranspose, A);
			//PrintMatrix(Aintermediate1, "c:\\intermediate1.csv"); 
			Aintermediate2 = matrix_invert(Aintermediate1); 
			if (Aintermediate2 == NULL)
			{
				return false; 
			}
			Aintermediate3 = matrix_mult(Aintermediate2, Atranspose); 

			c = matrix_mult(Aintermediate3, b); 
			b_interp = matrix_mult(A, c); 

			for (int col_num = 0; col_num < mint_num_knots; col_num++)
			{
				marr_coeffs[col_num] = *((double *)&c->ptr[col_num][0]); 
			}


	//		PrintPoints("c:\\pts_nat.csv"); 
			matrix_free(b); 
			matrix_free(c); 
			matrix_free(Aintermediate1); 
			matrix_free(Aintermediate2); 
			matrix_free(Aintermediate3); 
			matrix_free(Atranspose); 
			matrix_free(A); 
			return true; 
		}

		void NaturalCubicSplineRegression::PrintRegressionFunction(char *file_name)
		{
		}

		double NaturalCubicSplineRegression::GetPredictedValue(double x) 
		{
			if (mvect_pts.size() == 0)
				return 0; 

			if (x <= mdouble_min_x)
			{
				return marr_coeffs[0] + marr_coeffs[1] * mdouble_min_x; 
			}
			if (x >= mdouble_max_x)
			{
				x = mdouble_max_x; 
			}

			double val = marr_coeffs[0]; 
			double interval_width = (mdouble_max_x - mdouble_min_x) /(mint_num_knots+1);

			val = marr_coeffs[0] + marr_coeffs[1] * x;  

			double d_Kminus1 = 0; 
			if (x > mvect_interval_start[mint_num_knots-1])
			{
				d_Kminus1 = pow(x - mvect_interval_start[mint_num_knots-1], 3); 
				if (x > mvect_interval_start[mint_num_knots])
					d_Kminus1 -= pow(x - mvect_interval_start[mint_num_knots], 3); 
				d_Kminus1 /= interval_width; 
			}

			for (int k = 1; k <= mint_num_knots-2; k++)
			{
				double d_kminus1 = 0; 
				if (x > mvect_interval_start[k])
				{
					d_kminus1 = pow(x - mvect_interval_start[k], 3); 
					if (x > mvect_interval_start[mint_num_knots])
						d_kminus1 -= pow(x - mvect_interval_start[mint_num_knots], 3); 
					d_kminus1 /= interval_width; 
				}

				val += (d_kminus1 - d_Kminus1) * marr_coeffs[k+1]; 
			}

			return val; 
		}

		void NaturalCubicSplineRegression::PrintPoints(char *file_name)
		{
			ofstream fout(file_name);
			int num_pts = mvect_pts.size(); 
			for (int pt_num = 0; pt_num < num_pts; pt_num++)
			{
				RegressionPts pt = mvect_pts[pt_num]; 
				fout<<pt.mdouble_x<<","<<pt.mdouble_mass_error<<","<<pt.mdouble_mass_error - GetPredictedValue(pt.mdouble_x)<<"\n"; 
			}
			fout.close(); 
		}
	}
}