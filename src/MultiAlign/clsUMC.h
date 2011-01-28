#pragma once
#using <mscorlib.dll>
#include "clscluster.h"

namespace MultiAlignEngine
{
	namespace Features
	{
		public __gc class clsIsotopePeak
		{
			public:
				int		mint_original_index; 
				int		mint_umc_index; 
				int		mint_scan; 
				short	mshort_charge; 
				double	mdouble_abundance; 
				double	mdouble_mz; 
				float	mflt_fit; 
				double	mdouble_average_mass; 
				double	mdouble_mono_mass; 
				double	mdouble_max_abundance_mass;
				double	mdouble_i2_abundance; 
				clsIsotopePeak()
				{
				};

				__property int get_Id()
				{
					return mint_original_index;
				}

				__property void set_Id(int value){
					mint_original_index = value;
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

				__property double get_Abundance()
				{
					return mdouble_abundance;
				}

				__property void set_Abundance(double value){
					mdouble_abundance = value;
				}

				__property double get_MZ()
				{
					return mdouble_mz;
				}

				__property void set_MZ(double value){
					mdouble_mz = value;
				}

				__property float get_Fit()
				{
					return mflt_fit;
				}

				__property void set_Fit(float value){
					mflt_fit = value;
				}

				__property double get_MassAverage()
				{
					return mdouble_average_mass;
				}

				__property void set_MassAverage(double value){
					mdouble_average_mass = value;
				}

				__property double get_MassMonoisotopic()
				{
					return mdouble_mono_mass;
				}

				__property void set_MassMonoisotopic(double value){
					mdouble_mono_mass = value;
				}

				__property double get_MassMostAbundant()
				{
					return mdouble_max_abundance_mass;
				}

				__property void set_MassMostAbundant(double value){
					mdouble_max_abundance_mass = value;
				}

				__property double get_AbundancePlus2()
				{
					return mdouble_i2_abundance;
				}

				__property void set_AbundancePlus2(double value){
					mdouble_i2_abundance = value;
				}
		}; 

		[System::Serializable]
		public __gc class clsUMC
		{
			public:
				int		mint_dataset_index; 
				int		mint_dataset_id;
				int		mint_cluster_index; 
				int		mint_umc_index;
				int		mint_umc_id;
				int		mint_conformation_index;
				double	mdouble_mono_mass; 
				double	mdouble_class_rep_mz; 
				short	mshort_class_rep_charge; 
				short	mshort_class_highest_charge;
				int		mint_scan;
				int		mint_start_scan; 
				int		mint_end_scan; 
				double	mdouble_net; 
				double	mdouble_mono_mass_calibrated; 
				int		mint_scan_aligned; 
				double	mdouble_abundance; 
				double  mdouble_max_abundance; 
				__int64	mlong_sum_abundance;
				int		mint_spectral_count;
				float	mfloat_drift_time;
				double	mdouble_averageInterferenceScore;
				double	mdouble_conformationFitScore;
				double	mdouble_isotopicFitScore;
				double	mdouble_membersPercentageScore;
				double	mdouble_combinedScore;
				//clsCluster *umcCluster;

				bool Equals(System::Object *obj)
				{
					clsUMC *umc = dynamic_cast<clsUMC *>(obj); 
					if (mint_dataset_id != umc->mint_dataset_id)
					{
						return false; 
					}
					if (mint_umc_index != umc->mint_umc_index)
					{
						return false; 
					}
					return true; 
				}

				int GetHashCode()
				{
					int hash = 17;

					hash = hash * 23 + mint_dataset_id.GetHashCode();
					hash = hash * 23 + mint_umc_index.GetHashCode();

					return hash;
				}

				clsUMC(void);
				~clsUMC(void);

				__property int get_Id()
				{
					if(mint_umc_id >= 0)
					{
						return mint_umc_id;
					}
					else
					{
						return mint_umc_index;
					}
				}

				__property void set_Id(int value){
					mint_umc_id = value;
				}

				__property int get_DatasetId()
				{
					return mint_dataset_id;
				}

				__property void set_DatasetId(int value){
					mint_dataset_id = value;
				}

				__property int get_ClusterId()
				{
					return mint_cluster_index;
				}

				__property void set_ClusterId(int value){
					mint_cluster_index = value;
				}

				__property int get_ConformationId()
				{
					return mint_conformation_index;
				}

				__property void set_ConformationId(int value){
					mint_conformation_index = value;
				}

				/*
				__property clsCluster* get_UmcCluster()
				{
					return umcCluster;
				}

				__property void set_UmcCluster(clsCluster *value)
				{
					umcCluster = value;
				}
				*/

				__property double get_Mass()
				{
					return mdouble_mono_mass;
				}

				__property void set_Mass(double value){
					mdouble_mono_mass = value;
				}

				__property double get_MassCalibrated()
				{
					return mdouble_mono_mass_calibrated;
				}

				__property void set_MassCalibrated(double value){
					mdouble_mono_mass_calibrated = value;
				}

				__property double get_Net()
				{
					return mdouble_net;
				}

				__property void set_Net(double value){
					mdouble_net = value;
				}

				__property int get_Scan()
				{
					return mint_scan;
				}

				__property void set_Scan(int value){
					mint_scan = value;
				}

				__property int get_ScanStart()
				{
					return mint_start_scan;
				}

				__property void set_ScanStart(int value){
					mint_start_scan = value;
				}

				__property int get_ScanEnd()
				{
					return mint_end_scan;
				}

				__property void set_ScanEnd(int value){
					mint_end_scan = value;
				}

				__property int get_ScanAligned()
				{
					return mint_scan_aligned;
				}

				__property int get_SpectralCount()
				{
					return mint_spectral_count;
				}

				__property void set_SpectralCount(int value){
					mint_spectral_count = value;
				}

				__property void set_ScanAligned(int value){
					mint_scan_aligned = value;
				}

				__property short get_ChargeRepresentative()
				{
					return mshort_class_rep_charge;
				}

				__property void set_ChargeRepresentative(short value){
					mshort_class_rep_charge = value;
				}

				__property int get_ChargeMax()
				{
					return mshort_class_highest_charge;
				}

				__property void set_ChargeMax(int value){
					mshort_class_highest_charge = value;
				}

				__property double get_AbundanceMax()
				{
					return mdouble_max_abundance;
				}

				__property void set_AbundanceMax(double value){
					mdouble_max_abundance = value;
				}

				__property __int64 get_AbundanceSum()
				{
					return mlong_sum_abundance;
				}

				__property void set_AbundanceSum(__int64 value){
					mlong_sum_abundance = value;
				}

				__property double get_MZForCharge()
				{
					return mdouble_class_rep_mz;
				}

				__property void set_MZForCharge(double value){
					mdouble_class_rep_mz = value;
				}

				__property double get_DriftTime()
				{
					return mfloat_drift_time;
				}

				__property void set_DriftTime(double value){
					mfloat_drift_time = value;
				}

				__property double get_AverageInterferenceScore()
				{
					return mdouble_averageInterferenceScore;
				}

				__property void set_AverageInterferenceScore(double value){
					mdouble_averageInterferenceScore = value;
				}

				__property double get_ConformationFitScore()
				{
					return mdouble_conformationFitScore;
				}

				__property void set_ConformationFitScore(double value){
					mdouble_conformationFitScore = value;
				}

				__property double get_IsotopicFitScore()
				{
					return mdouble_isotopicFitScore;
				}

				__property void set_IsotopicFitScore(double value){
					mdouble_isotopicFitScore = value;
				}

				__property double get_MembersPercentageScore()
				{
					return mdouble_membersPercentageScore;
				}

				__property void set_MembersPercentageScore(double value){
					mdouble_membersPercentageScore = value;
				}

				__property double get_CombinedScore()
				{
					return mdouble_combinedScore;
				}

				__property void set_CombinedScore(double value){
					mdouble_combinedScore = value;
				}
		};
	}
}