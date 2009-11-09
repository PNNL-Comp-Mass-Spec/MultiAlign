#pragma once
#include "clsClusterData.h" 
#using   <mscorlib.dll>

using namespace System::Collections;
using namespace MultiAlignEngine::Features;

#include "SLClustering\Point.h"
#include <vector> 

namespace MultiAlignEngine
{
	namespace Features
	{
		
		/// Determines how umc abundances are reported.
		public __value enum enmAbundanceReportingType { PeakArea = 0, PeakMax};

		[Serializable]
		public __gc class clsUMCData
		{
			bool mblnClusterIndicesSet; 
			// the umcs are sorted in terms of the datasets and 
			// indices are kept for start and end of each dataset.
			Hashtable *mhash_umc_2_dataset_num;
			ArrayList *marr_file_names;
			ArrayList *marr_dataset_start_index; 
			ArrayList *marr_dataset_stop_index; 
			ArrayList *marr_dataset_min_scan; 
			ArrayList *marr_dataset_max_scan;
						
			int mint_num_datasets;
			int mint_num_umcs;
			int mint_highestChargeState;

			const int mint_default_umc_allocation; 
			
			void GetDataIndex(int dataset_num, int &start_index, int &stop_index); 
			void Allocate();

			// removes padding in the umc array.
			inline void RemoveUMCArrayPadding()
			{
				if (mint_num_umcs != marr_umcs->Length)
				{
					
					clsUMC *temp __gc[]  = new clsUMC* __gc[mint_num_umcs]; 
					Array::Copy(marr_umcs, temp, mint_num_umcs); 
					marr_umcs = temp; 
					
				}
			}
		public:
			clsUMCData(); 
			~clsUMCData(void); 

			int											marr_highestChargeStates __gc[];
			MultiAlignEngine::Features::clsUMC*			marr_umcs				 __gc[]; 			
			//PNNLControls::Data::clsSQLiteCacheArray<clsUMC*>*	marr_umcs;
			MultiAlignEngine::Features::clsClusterData* mobjClusterData;						
			
			/*************************************************************************************************************
			*		Properties
			*************************************************************************************************************/
			[clsDataSummaryAttribute("Highest Charge State")]
			__property int get_HighestChargeState()
			{
				return mint_highestChargeState;
			}
			[clsDataSummaryAttribute("Highest Charge State")]
			__property void set_HighestChargeState(int value)
			{
				mint_highestChargeState = value;
			}
			[clsDataSummaryAttribute("Number of UMC's")]
			__property int get_NumUMCS()
			{
				return mint_num_umcs; 
			}
			[clsDataSummaryAttribute("Number of UMC Datasets")]
			__property int get_NumDatasets()
			{
				return mint_num_datasets; 
			}
			__property System::String* get_DatasetName() __gc[]
			{
				System::String* datasets __gc[] = new System::String* __gc[mint_num_datasets]; 
				marr_file_names->CopyTo(datasets); 
				return datasets; 
			}			
			__property System::String* get_DatasetNameAt(int dataset_num)
			{
				if (dataset_num >= marr_file_names->Count || dataset_num < 0)
				{
					return 0;
				}
				return dynamic_cast<System::String *>(marr_file_names->Item[dataset_num]); 
			}

			/*************************************************************************************************************
			*		Methods
			*************************************************************************************************************/
			int GetDatasetIndex(System::String *datasetPath)
			{
				int index = datasetPath->LastIndexOf("\\");
				System::String *dataset = datasetPath->Substring(index+1); 
				for (int datasetNum = 0; datasetNum < marr_file_names->Count; datasetNum++)
				{
					if (System::String::Equals(dynamic_cast<System::String*>(marr_file_names->Item[datasetNum]), dataset))
					{
						return datasetNum; 
					}
				}
				return -1; 
			}

			clsUMC* GetUMCS(int dataset_num)__gc[]; 			
			void SetUMCCalibratedMassedAndNETS( std::vector <int> &umcIndices,
												std::vector<double> &umcCalibratedMasses, 
												std::vector<double> &umcAlignedNETS); 
			void SetUMCCalibratedMassedAndNETS( std::vector <int> &umcIndices,
												std::vector<double> &umcCalibratedMasses, 
												std::vector<double> &umcAlignedNETS, 
												std::vector<int> &umcAlignedScans); 
			
			int SetUMCS(System::String *file_path, 
						clsUMC* (&umcs) __gc[], 
						int min_scan,
						int max_scan); 
			int SetUMCS(System::String *file_path,
						clsUMC* (&umcs) __gc[]); 


			void GetMassesAndNETs(	int dataset_num,
									bool calibrated_masses, 
									float (&masses) __gc[], 
									float (&nets) __gc[]); 

			/// Finds a UMC given the dataset it belongs to, and the mass and scan number.			
			clsUMC* clsUMCData::GetUMC( int dataset_num,
										bool aligned, 
										int scan,
										double mass);
											
			
			inline clsUMC* GetUMC(int index); 
			void Clear();
			void GetAllUMCsAsPoints(std::vector<MultiAlignEngine::Clustering::Point> &vectPoints);
			void SetUMCClusterNums(std::vector<int> &vectClusterIndices);
			void CalculateClusters(); 

			System::Collections::ArrayList* GetMassAndTimeTags(int datasetIndex); 
			System::Collections::ArrayList* GetMassAndTimeTagsFromClusters(); 

			// returns the masses and scans of the features in the dataset. Also returns the starting 
			// index for the umcs in the array marr_umcs.
			int GetMassesAndScans(	int dataset_num,
									double (&masses) __gc[],
									int (&scans) __gc[]); 	
			// Finds all masses and scans for the given charge state.
			int GetMassesAndScans(	int dataset_num, 
									bool aligned,
									float (&masses) __gc[], 
									float (&scans) __gc[],
									int chargeState); 
			void GetMassesAndScans(	int dataset_num, 
									bool aligned,
									float (&masses) __gc[], 
									float (&scans) __gc[]); 
			void GetMinMaxScan(	int dataset_num,
								int __gc & min_scan, 
								int __gc & max_scan); 
		};
	}
}