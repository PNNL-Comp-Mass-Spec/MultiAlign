
#include ".\clsClusterProcessor.h"
#using <mscorlib.dll>
#include "SLClustering/MassTimeSLClustering.h" 

using namespace System::Collections::Generic;

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
				mobjMassTimeSLClustering->SetOptions(	mobjClusterOptions->get_MassTolerance(),
														mobjClusterOptions->get_NETTolerance(),
														mobjClusterOptions->get_DriftTimeTolerance(),
														mobjClusterOptions->IgnoreCharge); 
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
				throw ex;
			}
		}
		
		clsClusterData* clsClusterProcessor::PerformClustering(List<Features::clsUMC*> *features)
		{
			clsClusterData * clusters = 0;
			try
			{
				mobjMassTimeSLClustering->SetOptions(	mobjClusterOptions->get_MassTolerance(),
														mobjClusterOptions->get_NETTolerance(),
														mobjClusterOptions->get_DriftTimeTolerance(),
														mobjClusterOptions->IgnoreCharge); 

				menmState = clsClusterProcessor::SETTING_VARIABLES; 
				std::vector<MultiAlignEngine::Clustering::Point> vectPoints; 
				//umcData->GetAllUMCsAsPoints(vectPoints); 
				vectPoints.resize(features->Count);
				Point p;
				for(int i = 0; i < features->Count; i++)
				{					
					clsUMC* feature		= features->Item[i];
					p.mdouble_driftTime = feature->DriftTime;
					p.mdouble_mass		= feature->MassCalibrated;
					p.mdouble_net		= feature->Net;
					p.mint_charge		= feature->ChargeRepresentative;
					p.mint_datasetID	= feature->DatasetId;
					p.mint_index		= i;
					vectPoints[i]		= p;
				}

				std::vector<int> vectClusterIndices; 
				menmState = clsClusterProcessor::CLUSTERING; 
				mobjMassTimeSLClustering->Cluster(vectPoints, vectClusterIndices); 
				
				for(int i = 0; i < features->Count; i++)
				{					
					clsUMC* feature		= features->Item[i];
					feature->ClusterId	= vectClusterIndices[i];
				}
				
				menmState = clsClusterProcessor::CALCULATING_CLUSTER_STATS; 

				clsUMC* umcs __gc[]			= new clsUMC* __gc[features->Count];
				features->CopyTo(umcs, 0);

				clusters	= new clsClusterData(umcs, mobjClusterOptions->ClusterRepresentativeType);

				menmState = clsClusterProcessor::DONE; 
			}
			catch (System::Exception *ex)
			{
				Console::WriteLine(ex->get_Message()); 
				Console::WriteLine(ex->get_StackTrace());
				throw ex;
			}

			return clusters;
		}
	}
}