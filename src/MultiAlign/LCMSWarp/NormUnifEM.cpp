
#include "normunifem.h"
#include <float.h> 

namespace MultiAlignEngine
{
	namespace Regression
	{
		const double MIN_VAR = 0.0001; 

		NormUnifEM::NormUnifEM(void): mint_num_iterations(16), mdouble_mean(0), mdouble_var(10), mdouble_norm_fraction(0.5) 
		{
		}

		NormUnifEM::~NormUnifEM(void)
		{
		}

		void NormUnifEM::Reset()
		{
			mint_num_iterations = 16;
			mdouble_mean = 0;
			mdouble_var = 10;
			mdouble_norm_fraction = 0.5; 
		}
		void NormUnifEM::CalculateDistributions(vector<double> &vect_vals)
		{
			Reset(); 
			if (vect_vals.size() == 0)
			{
				mdouble_mean = 0; 
				mdouble_var = 0.1; 
				mdouble_norm_fraction = 0; 
				return; 
			}

			double min_val = DBL_MAX; 
			double max_val = -1 * DBL_MAX; 
			for (int i = 0; i < (int)vect_vals.size(); i++)
			{
				if (vect_vals[i] < min_val)
					min_val = vect_vals[i]; 
				if (vect_vals[i] > max_val)
					max_val = vect_vals[i]; 
			}
			if (min_val == max_val)
			{
				mdouble_var = 0.1; 
				mdouble_mean = max_val; 
				mdouble_norm_fraction = 0; 
				return; 
			}
			double u = 1 / (max_val - min_val); 

			int num_pts = vect_vals.size(); 

			mvect_unif_prob.clear(); 
			mvect_unif_prob.reserve(num_pts); 


			for (int iter_num = 0; iter_num < mint_num_iterations; iter_num++)
			{
				double mean_next = 0; 
				double var_next = 0; 
				double norm_fraction_next = 0; 
				for (int pt_num = 0; pt_num < num_pts; pt_num++)
				{
					double val = vect_vals[pt_num]; 
					double diff = (val - mdouble_mean); 
					double norm_prob = exp(-(0.5 * diff * diff) /mdouble_var) /(sqrt(2 * 3.14159) * sqrt(mdouble_var)); 
					double posterior_norm_prob = (norm_prob * mdouble_norm_fraction) / (norm_prob * mdouble_norm_fraction + (1-mdouble_norm_fraction)* u);  
					mvect_unif_prob.push_back(posterior_norm_prob); 

					norm_fraction_next += posterior_norm_prob; 
					mean_next += posterior_norm_prob * val; 
					var_next += posterior_norm_prob * (val - mdouble_mean)* (val - mdouble_mean); 
				}
				mdouble_norm_fraction = norm_fraction_next / num_pts; 
				mdouble_mean = mean_next / norm_fraction_next; 
				mdouble_var = var_next / norm_fraction_next; 
				if (mdouble_var < MIN_VAR)
					break; 
			}
		}
	}
}