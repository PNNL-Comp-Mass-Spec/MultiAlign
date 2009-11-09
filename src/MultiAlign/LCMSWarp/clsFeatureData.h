#pragma once

class clsFeatureData
{
	public:
		int		mint_index; 
		int		mint_dataset_index; 
		double	mdouble_mass; 
		double	mdouble_net; 

		clsFeatureData(void):
			mint_index(-1), mint_dataset_index(-1), mdouble_mass(0), mdouble_net(-1)
		{

		};
		~clsFeatureData(void)
		{

		};
		clsFeatureData(int index, int dataset_index, double mass, double net):
					mint_index(index), mint_dataset_index(dataset_index),  mdouble_mass(mass), mdouble_net(net)
		{

		};
		void Set(int index, int dataset_index, double mass, double net)
		{
			mint_index			= index;
			mint_dataset_index	= dataset_index; 
			mdouble_mass		= mass;
			mdouble_net			= net;
		}
};

bool SortFeatureDataByMass(clsFeatureData &a, clsFeatureData &b);