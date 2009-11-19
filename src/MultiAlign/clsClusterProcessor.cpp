
#include ".\clsClusterProcessor.h"
#using <mscorlib.dll>
#include "SLClustering/MassTimeSLClustering.h" 

namespace MultiAlignEngine
{
	namespace Clustering
	{
		clsClusterProcessor::clsClusterProcessor(void)
		{
			mobjClusterOptions = __gc new clsClusterOptions(); 
			menmState = enmState::IDLE; 
			mobjMassTimeSLClustering = __nogc new MultiAlignEngine::Clustering::MassTimeSLClustering(); 
		}

		clsClusterProcessor::~clsClusterProcessor(void)
		{
			delete mobjMassTimeSLClustering; 
		}
		
		void clsClusterProcessor::PerformClustering(Features::clsUMCData *umcData)
		{
			try
			{
				mobjMassTimeSLClustering->SetOptions(mobjClusterOptions->get_MassTolerance(),
					mobjClusterOptions->get_NETTolerance()); 
				menmState = clsClusterProcessor::SETTING_VARIABLES; 
				std::vector<MultiAlignEngine::Clustering::Point> vectPoints; 
				umcData->GetAllUMCsAsPoints(vectPoints); 

				std::vector<int> vectClusterIndices; 
				menmState = clsClusterProcessor::CLUSTERING; 
				mobjMassTimeSLClustering->Cluster(vectPoints, vectClusterIndices); 
				umcData->SetUMCClusterNums(vectClusterIndices); 
				menmState = clsClusterProcessor::CALCULATING_CLUSTER_STATS; 
				umcData->CalculateClusters(); 
				menmState = clsClusterProcessor::DONE; 
			}
			catch (System::Exception *ex)
			{
				Console::WriteLine(ex->get_Message()); 
				Console::WriteLine(ex->get_StackTrace()); 
			}
		}
	}
}