#pragma once
#include <vector>
using namespace std; 
namespace Utilities
{
	template <typename T> void  CreateHistogram(vector<T> &input_values, vector<T> &bins, vector<int> &frequency, T val_step) 
	{
		bins.clear(); 
		frequency.clear(); 
		int num_pts = input_values.size(); 
		if (num_pts == 0)
			return; 


		T min_val = input_values[0]; 
		T max_val = input_values[0]; 
		for (int i = 0; i < num_pts; i++)
		{
			if (input_values[i] < min_val)
				min_val = input_values[i]; 
			if (input_values[i] > max_val)
				max_val = input_values[i]; 
		}

		if (min_val == max_val)
		{
			bins.push_back(min_val); 
			frequency.push_back(1); 
			return; 
		}

		int num_bins = (max_val - min_val) / val_step; 

		T bin_val = min_val; 
		for (int i = 0; i < num_bins; i++)
		{
			bins.push_back(bin_val); 
			frequency.push_back(0); 
			bin_val += val_step; 
		}

		for (int i = 0; i < num_pts; i++)
		{
			int bin_index = (input_values[i] - min_val) / val_step; 
			if (bin_index >= num_bins)
				bin_index = num_bins - 1; 
			frequency[bin_index]++; 
		}
	}
};
