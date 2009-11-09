#pragma once
#include <vector>
#include <math.h>
using namespace std; 
namespace MultiAlignEngine
{
	namespace Regression
	{
		class NormUnifEM
		{
			double mdouble_mean; 
			double mdouble_var;
			double mdouble_norm_fraction; 
			int mint_num_iterations; 
			vector<double> mvect_unif_prob; 
		public:
			NormUnifEM(void);
			~NormUnifEM(void);
			void Reset(); 
			void CalculateDistributions(vector<double> &vals);
			double GetStd() { return sqrt(mdouble_var); } 
			double GetMean() { return mdouble_mean; } 
			double GetNormProb() { return mdouble_norm_fraction; } 
		};
	}
}