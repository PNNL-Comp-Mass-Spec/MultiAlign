#pragma once
namespace MultiAlignEngine
{
	namespace Features
	{
		[System::Serializable]
		public __gc class clsCluster
		{
		public:
			double mdouble_mass; 
			double mdouble_mass_calibrated; 
			int mint_scan; 
			double mdouble_net; 
			double mdouble_aligned_net; 
			short mshort_charge; 
			short mshort_num_dataset_members; 
			clsCluster(void);
			~clsCluster(void);
		};
	}
}