#pragma once
namespace MultiAlignEngine
{
	namespace Features
	{
		[System::Serializable]
		public __gc class clsCluster
		{
		public:
			int    mint_cluster_index;
			double mdouble_mass; 
			double mdouble_mass_calibrated; 
			int    mint_scan; 
			double mdouble_net; 
			double mdouble_aligned_net; 
			short  mshort_charge; 
			short  mshort_num_dataset_members; 
			double mdouble_driftTime;

			double mdouble_netError	;
			double mdouble_massError;
			double mdouble_driftError;
			double mdouble_medianScoreValue;
			double mdouble_meanScoreValue;
			
			clsCluster(void);
			~clsCluster(void);

			__property int get_Id()
			{
				return mint_cluster_index;
			}

			__property void set_Id(int value){
				mint_cluster_index = value;
			}
			
			__property double get_MeanScore()
			{
				return mdouble_meanScoreValue;
			}

			__property void set_MeanScore(double value){
				mdouble_meanScoreValue = value;
			}
			__property double get_MedianScore()
			{
				return mdouble_medianScoreValue;
			}

			__property void set_MedianScore(double value){
				mdouble_medianScoreValue = value;
			}

			__property double get_Mass()
			{
				return mdouble_mass;
			}

			__property void set_Mass(double value){
				mdouble_mass = value;
			}

			__property double get_MassCalibrated()
			{
				return mdouble_mass_calibrated;
			}

			__property void set_MassCalibrated(double value){
				mdouble_mass_calibrated = value;
			}

			__property double get_Net()
			{
				return mdouble_net;
			}

			__property void set_Net(double value){
				mdouble_net = value;
			}

			__property double get_NetAligned()
			{
				return mdouble_aligned_net;
			}

			__property void set_NetAligned(double value){
				mdouble_aligned_net = value;
			}

			__property int get_Scan()
			{
				return mint_scan;
			}

			__property void set_Scan(int value){
				mint_scan = value;
			}

			__property short get_Charge()
			{
				return mshort_charge;
			}

			__property void set_Charge(short value){
				mshort_charge = value;
			}

		};
	}
}