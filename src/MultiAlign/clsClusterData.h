#pragma once
#include "clsUMC.h" 
#include "clsCluster.h" 
#include "clsClusterOptions.h"
#include "clsDataSummaryAttribute.h"
#include <vector> 

namespace MultiAlignEngine
{
	namespace Features
	{
		[System::Serializable]
		public __gc class clsClusterData
		{
			int mintNumClusters; 
			System::Collections::ArrayList *marrClusters; 
			Clustering::enmClusterRepresentativeType menmClusterRepresentativeType; 
			Clustering::enmClusterIntensityType menmClusterIntensityType; 

			/// <summary>
			/// Class used for sorting pairs of key, value pairs based on key.
			/// </summary>
			[System::Serializable]
			__gc class clsPair : public System::IComparable
			{
			public:
				int mintClusterNum; 
				int mintUMCIndex;
				clsPair():mintClusterNum(-1), mintUMCIndex(-1){}
				clsPair(int cluster_num, int umc_index):mintClusterNum(cluster_num), mintUMCIndex(umc_index) {} 
				void Set(int cluster_num, int umc_index) { mintClusterNum = cluster_num; mintUMCIndex = umc_index; }
				int CompareTo(System::Object *obj)
				{
					clsPair *pt = dynamic_cast<clsPair *>(obj); 
					// TODO:  Add clsPair.CompareTo implementation
					if (mintClusterNum < pt->mintClusterNum)
						return -1; 
					if (mintClusterNum > pt->mintClusterNum)
						return 1; 
					return 0; 
				}
			};
			void CreateClustersFromClusterUMCIndexPair(MultiAlignEngine::Features::clsUMC *arrUMCs __gc[]); 
	public:
			double mdblMinNET; 
			double mdblMaxNET; 
			int mintNumDatasets; 
			void AddCluster(clsCluster * cluster);
			// array of intensities for all cluster in each dataset
			double marrClusterIntensity __gc[]; 
			// array of normalized intensities for all cluster in each dataset
			double marrClusterIntensityNormalized __gc[]; 
			int marrClusterMainMemberIndex __gc[]; 
			System::Collections::ArrayList *marrClusterUMCIndexPair; 
			inline clsCluster* GetCluster(int index)
			{
				return dynamic_cast<clsCluster *>(marrClusters->Item[index]); 
			}
			inline int GetClusterMainMember(int clusterIndex, int datasetNum)
			{
				return marrClusterMainMemberIndex[clusterIndex*mintNumDatasets + datasetNum]; 
			}
			System::Collections::ArrayList* GetMassAndTimeTags(); 
			System::Collections::ArrayList* GetMassAndTimeTags(double shift); 

			clsClusterData(MultiAlignEngine::Features::clsUMC *arrUmcs __gc[]);
			clsClusterData();
			~clsClusterData(void);

			void GetMinMaxNET(float __gc &minNET, float __gc &maxNET) 
			{ 
				minNET = (float) mdblMinNET; 
				maxNET = (float) mdblMaxNET; 
			}; 

			void SetClusterCalibratedMassedAndNETS(std::vector<int> &clusterIndices, std::vector<double> &clusterCalibratedMasses, 
				std::vector<double> &clusterAlignedNETS); 
			[clsDataSummaryAttribute("Number of Clusters")]
			__property int get_NumClusters() { return mintNumClusters; }; 
			[clsDataSummaryAttribute("Intensities Normalized")]
			__property bool get_IsDataNormalized() { return marrClusterIntensityNormalized != 0; }; 
		};
	}
}