#pragma once
#include "clsUMCData.h"
#include "clsClusterOptions.h" 
#include "SLClustering\MassTimeSLClustering.h"

using namespace System::Collections::Generic;

namespace MultiAlignEngine
{
	namespace Clustering
	{

		[System::Serializable]
	public __gc class clsClusterProcessor  : public System::Runtime::Serialization::IDeserializationCallback
		{
			clsClusterOptions *mobjClusterOptions; 
			[NonSerialized]MultiAlignEngine::Clustering::MassTimeSLClustering __nogc *mobjMassTimeSLClustering; 
		public:
			__value enum enmState
			{
				IDLE=0, SETTING_VARIABLES, CLUSTERING_INITIALIZED, CLUSTERING, CALCULATING_CLUSTER_STATS,
				DONE, ERROR
			};
		private:
			enmState menmState; 
		public:
			clsClusterProcessor(void);
			~clsClusterProcessor(void);

			void OnDeserialization(System::Object* sender) 
			{
				// After being deserialized, set this unmanaged object
				mobjMassTimeSLClustering = __nogc new MultiAlignEngine::Clustering::MassTimeSLClustering(); 
			}

			void PerformClustering(Features::clsUMCData *umcData); 			
			clsClusterData* clsClusterProcessor::PerformClustering(List<Features::clsUMC*> *features);

			__property enmState get_State()
			{
				return menmState; 
			}
			__property int get_PercentComplete()
			{
				return mobjMassTimeSLClustering->GetPercentDone(); 
			}
			__property System::String* get_StatusMessage()
			{
				switch(menmState)
				{
					case clsClusterProcessor::SETTING_VARIABLES:
						return new System::String(S"Setting Variables for clustering"); 
						break; 
					case clsClusterProcessor::CLUSTERING:
						return new System::String(S"Clustering UMCS"); 
						break; 
					case clsClusterProcessor::CALCULATING_CLUSTER_STATS:
						return new System::String(S"Calculating cluster properties"); 
						break;
					default:
						return new System::String(S""); 
						break; 
				}
				return 0; 
			}

			__property clsClusterOptions* get_ClusterOptions()
			{
				return mobjClusterOptions; 
			}
			__property void set_ClusterOptions(clsClusterOptions* value)
			{
				mobjClusterOptions = value;
			}

		};
	}
}