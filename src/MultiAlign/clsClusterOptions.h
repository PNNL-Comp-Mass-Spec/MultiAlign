#pragma once
#include "clsDataSummaryAttribute.h"
#include "clsParameterFileAttribute.h"
namespace MultiAlignEngine
{
	namespace Clustering
	{
		[System::Serializable]
		public __gc class clsClusterOptions
		{
			/// <summary>
			/// Mass Tolerance used in the clustering and peak matching.
			/// </summary>
			double mdblMassTolerance; 
			/// <summary>
			/// NET tolerance to be used in the clustering and peak matching.
			/// </summary>
			double mdblNETTolerance; 
			/// <summary>
			/// Drift-Time Tolerance for peak matching.
			/// </summary>
			double mdblDriftTimeTolerance;
		public:
			/// <summary>
			/// how the representative values of NET and mass are calculated from
			/// individual members in the group.. 
			/// </summary>
			__value enum enmClusterRepresentativeType { MEAN = 0, MEDIAN }; 

			/// <summary>
			/// how the intensity of a dataset in a cluster is calculated in the case that 
			/// multiple features from the same dataset are grouped together.
			/// </summary>
			__value enum enmClusterIntensityType { MAX_PER_DATASET = 0, SUM_PER_DATASET }; 
		private:
			enmClusterRepresentativeType menmClusterRepresentativeType; 
			enmClusterIntensityType menmClusterIntensityType; 
		public: 
			clsClusterOptions(void);
			~clsClusterOptions(void);

			[clsDataSummaryAttribute("Mass Tolerance")]			
			[clsParameterFileAttribute("MassTolerance", "ClusterOptions")]
			__property double get_MassTolerance()
			{
				return mdblMassTolerance; 
			}
			__property void set_MassTolerance(double value)
			{
				mdblMassTolerance = value; 
			}

			[clsDataSummaryAttribute("NET Tolerance")]			
			[clsParameterFileAttribute("NETTolerance", "ClusterOptions")]
			__property double get_NETTolerance()
			{
				return mdblNETTolerance; 
			}
			__property void set_NETTolerance(double value)
			{
				mdblNETTolerance = value; 
			}

			
			[clsDataSummaryAttribute("Drift Time Tolerance")]			
			[clsParameterFileAttribute("DriftTimeTolerance", "ClusterOptions")]
			__property double get_DriftTimeTolerance()
			{
				return mdblDriftTimeTolerance; 
			}
			__property void set_DriftTimeTolerance(double value)
			{
				mdblDriftTimeTolerance = value; 
			}

			[clsDataSummaryAttribute("Cluster Intensity Type")]
			[clsParameterFileAttribute("ClusterIntensityType", "ClusterOptions")]
			__property enmClusterIntensityType get_ClusterIntensityType()
			{
				return menmClusterIntensityType; 
			}
			__property void set_ClusterIntensityType(enmClusterIntensityType value)
			{
				menmClusterIntensityType = value; 
			}

			[clsDataSummaryAttribute("Cluster Representative Type")]
			[clsParameterFileAttribute("ClusterRepresentativeType", "ClusterOptions")]
			__property enmClusterRepresentativeType get_ClusterRepresentativeType()
			{
				return menmClusterRepresentativeType; 
			}
			__property void set_ClusterRepresentativeType(enmClusterRepresentativeType value)
			{
				menmClusterRepresentativeType = value; 
			}
		};
	}
}