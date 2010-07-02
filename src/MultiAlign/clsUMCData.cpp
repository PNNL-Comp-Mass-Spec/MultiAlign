
#include ".\clsumcdata.h"
#include "clsMassTimeTag.h"

namespace MultiAlignEngine
{
	namespace Features
	{
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/		
		clsUMCData::clsUMCData(void): mint_default_umc_allocation(100000), mblnClusterIndicesSet(false)
		{
			Allocate(); 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCData::Clear(void)
		{
			mblnClusterIndicesSet	= false; 
			mobjClusterData			= 0; 			
			mint_highestChargeState = 0;

			this->marr_file_names->Clear();
			marr_umcs->Clear();
			
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		clsUMCData::~clsUMCData(void)
		{
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCData::Allocate() 
		{
			mhash_umc_2_dataset_num = new Hashtable();
			marr_file_names = new ArrayList(); 

			mint_num_datasets		 = 0; 
			mint_num_umcs			 = 0; 
			marr_umcs				 = new clsUMC* __gc[mint_default_umc_allocation]; 			
			//marr_umcs				 = new PNNLControls::Data::clsSQLiteCacheArray<clsUMC*>("umcs.db", PNNLControls::Data::enmSQLiteDataCacheMode::Create, 10000, mint_default_umc_allocation);

			marr_dataset_start_index = new ArrayList(); 
			marr_dataset_stop_index  = new ArrayList(); 
			marr_dataset_min_scan    = new ArrayList(); 
			marr_dataset_max_scan    = new ArrayList(); 
		}
		/*************************************************************************************************************
		*
		*	Finds the umc at the idnex
		*
		*************************************************************************************************************/
		 MultiAlignEngine::Features::clsUMC* clsUMCData::GetUMC(int dataset_num, bool aligned, int scan, double mass)
		 {
			/// first map the dataset to the large UMC array.
			int start_index = *dynamic_cast<__box Int32*>(marr_dataset_start_index->Item[dataset_num]); 
			int stop_index	= *dynamic_cast<__box Int32*>(marr_dataset_stop_index->Item[dataset_num]); 
			
			/// Then do an O(n) search through the UMC to see if we can find the UMC that we are searching for.
			int index; 			
			double minDist  = 999999; 
			int minIndex    = -1;
			for (index = start_index; index < stop_index; index++)
			{
				double massOfInterest;
				int    scanOfInterest;

				/// Find out what mass and scan we want to use, if they are aligned.
				if (!aligned)
				{
					scanOfInterest = marr_umcs[index]->mint_scan;
					massOfInterest = marr_umcs[index]->mdouble_mono_mass;
				}
				else
				{
					scanOfInterest = marr_umcs[index]->mint_scan_aligned;
					massOfInterest = marr_umcs[index]->mdouble_mono_mass_calibrated;
				}
				
				double 	distScan = Math::Pow(Convert::ToDouble(scan - scanOfInterest), 2.0);
				double  distMass = Math::Pow(mass - massOfInterest, 2.0);
				
				double dist = Math::Sqrt(distScan + distMass);

				if (dist < minDist)
				{
					minDist  = dist;
					minIndex = index;
				}
			}

			if (minIndex >= 0)
				return marr_umcs[minIndex];

			return NULL;
		 }
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		inline MultiAlignEngine::Features::clsUMC* clsUMCData::GetUMC(int index)
		{
			return marr_umcs[index]; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/		
		void clsUMCData::SetUMCCalibratedMassedAndNETS(	std::vector <int> &umcIndices, 
														std::vector<double> &umcCalibratedMasses, 
														std::vector<double> &umcAlignedNETS, 
														std::vector<double> &umcDriftTimes)
		{
			int numUMCs = (int) umcIndices.size(); 
			for (int umcNum = 0; umcNum < numUMCs; umcNum++)
			{
				int umcIndex										= umcIndices[umcNum]; 
				double calibratedMass								= umcCalibratedMasses[umcNum]; 
				double alignedNET									= umcAlignedNETS[umcNum]; 
				double driftTime									= umcDriftTimes[umcNum]; 
				marr_umcs[umcIndex]->mdouble_mono_mass_calibrated	= calibratedMass; 
				marr_umcs[umcIndex]->mdouble_net					= alignedNET;
				marr_umcs[umcIndex]->DriftTime						= driftTime;
			}
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCData::SetUMCCalibratedMassedAndNETS( std::vector <int> &umcIndices, 
														std::vector<double> &umcCalibratedMasses, 
														std::vector<double> &umcAlignedNETS, 
														std::vector<int> &umcAlignedScans, 
														std::vector<double> &umcDriftTimes)
		{
			int numUMCs = (int) umcIndices.size(); 
			for (int umcNum = 0; umcNum < numUMCs; umcNum++)
			{
				int umcIndex			= umcIndices[umcNum]; 
				double calibratedMass	= umcCalibratedMasses[umcNum]; 
				double alignedNET		= umcAlignedNETS[umcNum]; 
				int alignedScan			= umcAlignedScans[umcNum]; 
				double driftTime		= umcDriftTimes[umcNum];
				marr_umcs[umcIndex]->mdouble_mono_mass_calibrated	= calibratedMass; 
				marr_umcs[umcIndex]->mdouble_net					= alignedNET;
				marr_umcs[umcIndex]->mint_scan_aligned				= alignedScan;
				marr_umcs[umcIndex]->mfloat_drift_time				= Convert::ToSingle(driftTime);

			}
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		MultiAlignEngine::Features::clsUMC* clsUMCData::GetUMCS(int dataset_num)__gc[] 
		{
			int start_index = 0 , stop_index = 0; 
			GetDataIndex(dataset_num, start_index, stop_index); 
			int num_pts = stop_index - start_index;
			clsUMC* umc_arr __gc[] = new clsUMC* __gc[num_pts]; 
			for (int index = start_index; index < stop_index; index++)
			{
				umc_arr[index-start_index] = marr_umcs[index]; 
			}
			return umc_arr; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		int clsUMCData::SetUMCS(System::String *file_path, clsUMC* (&umcs) __gc[], int min_scan, int max_scan)
		{
			mblnClusterIndicesSet = false; 
			int index = file_path->LastIndexOf("\\");
			System::String *file = file_path->Substring(index+1); 

			marr_file_names->Add(file); 
			mhash_umc_2_dataset_num->Add(file, __box(mint_num_datasets)); 
			marr_dataset_start_index->Add(__box(mint_num_umcs)); 

			if (marr_umcs->Length < mint_num_umcs + umcs->Length)
			{
				int newSize = marr_umcs->Length; 
				while (newSize < mint_num_umcs + umcs->Length)
					newSize += 100000; 

				clsUMC* temp __gc[] = new clsUMC* __gc[newSize]; 
				Array::Copy(marr_umcs, temp, mint_num_umcs); 
				marr_umcs = temp; 
			}
			int num_to_add = umcs->Length; 

			for (int i = 0; i < num_to_add; i++)
			{
				marr_umcs[i+mint_num_umcs] = umcs[i]; 
				marr_umcs[i+mint_num_umcs]->mint_dataset_index = mint_num_datasets; 
				marr_umcs[i+mint_num_umcs]->mint_umc_index = i+mint_num_umcs; 
			}
			mint_num_umcs += num_to_add; 
			marr_dataset_stop_index->Add(__box(mint_num_umcs)); 
			marr_dataset_min_scan->Add(__box(min_scan)); 
			marr_dataset_max_scan->Add(__box(max_scan)); 

			mint_num_datasets++; 
			return mint_num_datasets - 1; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		int clsUMCData::SetUMCS(System::String *file_path, clsUMC* (&umcs) __gc[])
		{
			mblnClusterIndicesSet = false; 
			int index = file_path->LastIndexOf("\\");
			System::String *file = file_path->Substring(index+1); 

			marr_file_names->Add(file); 

			mhash_umc_2_dataset_num->Add(file, __box(mint_num_datasets)); 
			marr_dataset_start_index->Add(__box(mint_num_umcs)); 

			if (marr_umcs->Length < mint_num_umcs + umcs->Length)
			{
				int newSize = marr_umcs->Length; 
				while (newSize < mint_num_umcs + umcs->Length)
					newSize += 100000; 

				clsUMC *temp __gc[]  = new clsUMC* __gc[newSize]; 
				Array::Copy(marr_umcs, temp, mint_num_umcs); 
				marr_umcs = temp; 
			}
			int num_to_add = umcs->Length; 
			int min_scan = System::Int32::MaxValue; 
			int max_scan = System::Int32::MinValue; 

			for (int i = 0; i < num_to_add; i++)
			{
				marr_umcs[i+mint_num_umcs] = umcs[i]; 
				marr_umcs[i+mint_num_umcs]->mint_dataset_index = mint_num_datasets; 
				marr_umcs[i+mint_num_umcs]->mint_umc_index = i+mint_num_umcs; 
				if (umcs[i]->mint_scan < min_scan)
					min_scan = umcs[i]->mint_scan; 
				if (umcs[i]->mint_scan > max_scan)
					max_scan = umcs[i]->mint_scan; 
			}
			mint_num_umcs += num_to_add; 
			marr_dataset_stop_index->Add(__box(mint_num_umcs)); 
			marr_dataset_min_scan->Add(__box(min_scan)); 
			marr_dataset_max_scan->Add(__box(max_scan)); 

			mint_num_datasets++; 
			return mint_num_datasets - 1; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		System::Collections::ArrayList* clsUMCData::GetMassAndTimeTags(int datasetIndex)
		{
			int start_index = *dynamic_cast<__box Int32*>(marr_dataset_start_index->Item[datasetIndex]); 
			int stop_index = *dynamic_cast<__box Int32*>(marr_dataset_stop_index->Item[datasetIndex]); 
			int numUMCs = stop_index-start_index; 
			System::Collections::ArrayList *arrMSFeatures = new System::Collections::ArrayList(numUMCs);
			for (int umcIndex = start_index; umcIndex < stop_index; umcIndex++)
			{
				Features::clsUMC *umc = dynamic_cast<Features::clsUMC *>(marr_umcs->Item[umcIndex]); 
				clsMassTimeTag *mtTag = new clsMassTimeTag(umc->mdouble_mono_mass, umc->mdouble_net, umc->DriftTime, umcIndex);
				arrMSFeatures->Add(mtTag); 
			}
			return arrMSFeatures; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		System::Collections::ArrayList* clsUMCData::GetMassAndTimeTagsFromClusters()
		{
			return mobjClusterData->GetMassAndTimeTags();
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCData::GetMassesAndNETs(int dataset_num, bool calibrated_masses, float (&masses) __gc[], float (&nets) __gc[], double (&driftTimes) __gc[])
		{
			int start_index = *dynamic_cast<__box Int32*>(marr_dataset_start_index->Item[dataset_num]); 
			int stop_index	= *dynamic_cast<__box Int32*>(marr_dataset_stop_index->Item[dataset_num]); 
			int num_pts		= stop_index - start_index;
			int index; 

			masses			= new float  __gc[num_pts]; 
			nets			= new float  __gc[num_pts]; 
			driftTimes		= new double __gc[num_pts]; 
			for (index = start_index; index < stop_index; index++)
			{
				if (calibrated_masses)
					masses[index-start_index]	= Convert::ToSingle(marr_umcs[index]->mdouble_mono_mass_calibrated); 
				else
					masses[index-start_index]	= Convert::ToSingle(marr_umcs[index]->mdouble_mono_mass); 
				nets[index-start_index]			= Convert::ToSingle(marr_umcs[index]->mdouble_net); 
				driftTimes[index-start_index]	= Convert::ToDouble(marr_umcs[index]->DriftTime);
			}
		}
		void clsUMCData::GetMZAndNETs(int dataset_num, float (&mz) __gc[], float (&nets) __gc[], double (&driftTimes) __gc[])
		{
			int start_index = *dynamic_cast<__box Int32*>(marr_dataset_start_index->Item[dataset_num]); 
			int stop_index	= *dynamic_cast<__box Int32*>(marr_dataset_stop_index->Item[dataset_num]); 
			int num_pts		= stop_index - start_index;
			int index; 

			mz				= new float  __gc[num_pts]; 
			nets			= new float  __gc[num_pts]; 
			driftTimes		= new double __gc[num_pts]; 
			for (index = start_index; index < stop_index; index++)
			{				
				mz[index-start_index]	    = Convert::ToSingle(marr_umcs[index]->mdouble_class_rep_mz); 
				nets[index-start_index]			= Convert::ToSingle(marr_umcs[index]->mdouble_net); 
				driftTimes[index-start_index]	= Convert::ToDouble(marr_umcs[index]->DriftTime);
			}
		}
		void clsUMCData::GetMZAndScans(int dataset_num, bool aligned,  float (&mz) __gc[], float (&scans) __gc[], double (&driftTimes) __gc[])
		{
			int start_index = *dynamic_cast<__box Int32*>(marr_dataset_start_index->Item[dataset_num]); 
			int stop_index	= *dynamic_cast<__box Int32*>(marr_dataset_stop_index->Item[dataset_num]); 
			int num_pts		= stop_index - start_index;
			int index; 

			mz				= new float  __gc[num_pts]; 
			scans			= new float  __gc[num_pts]; 
			driftTimes		= new double __gc[num_pts]; 
			for (index = start_index; index < stop_index; index++)
			{				
				mz[index-start_index]			= Convert::ToSingle(marr_umcs[index]->mdouble_class_rep_mz); 
				if (!aligned)
				{
					scans[index-start_index]			= Convert::ToSingle(marr_umcs[index]->mint_scan); 
				}
				else
				{
					scans[index-start_index]			=  Convert::ToSingle(marr_umcs[index]->mint_scan_aligned); 
				}
				driftTimes[index-start_index]	= Convert::ToDouble(marr_umcs[index]->DriftTime);
			}
		}
		void clsUMCData::GetMZAndScans(int dataset_num, bool aligned,  float (&mz) __gc[], float (&scans) __gc[], 			
			int chargeState)
		{
			int start_index = *dynamic_cast<__box Int32*>(marr_dataset_start_index->Item[dataset_num]); 
			int stop_index	= *dynamic_cast<__box Int32*>(marr_dataset_stop_index->Item[dataset_num]); 
			int num_pts		= stop_index - start_index;
			int index; 

			mz				= new float  __gc[num_pts]; 
			scans			= new float  __gc[num_pts]; 
			for (index = start_index; index < stop_index; index++)
			{				
				if (marr_umcs[index]->mshort_class_rep_charge == chargeState)
				{
					mz[index-start_index]			= Convert::ToSingle(marr_umcs[index]->mdouble_class_rep_mz); 
					if (!aligned)
					{
						scans[index-start_index]			= Convert::ToSingle(marr_umcs[index]->mint_scan); 
					}
					else
					{
						scans[index-start_index]			=  Convert::ToSingle(marr_umcs[index]->mint_scan_aligned); 
					}
				}
			}
		}
		
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		int clsUMCData::GetMassesAndScans(int dataset_num, 
											bool aligned, 
											float (&masses) __gc[],
											float (&scans) __gc[],
											int chargeState)
		{
			int start_index = *dynamic_cast<__box Int32*>(marr_dataset_start_index->Item[dataset_num]); 
			int stop_index	= *dynamic_cast<__box Int32*>(marr_dataset_stop_index->Item[dataset_num]); 
			int num_pts		= stop_index - start_index;
			int index; 
			
			masses  = new float __gc[num_pts]; 
			scans   = new float __gc[num_pts]; 

			int j				= 0;

			for (index = start_index; index < stop_index; index++)
			{
				if (marr_umcs[index]->mshort_class_rep_charge == chargeState)
				{
					if (!aligned)
					{
						masses[j] = Convert::ToSingle(marr_umcs[index]->mdouble_mono_mass); 
						scans[j] =  Convert::ToSingle(marr_umcs[index]->mint_scan); 
					}
					else
					{
						masses[j] = Convert::ToSingle(marr_umcs[index]->mdouble_mono_mass_calibrated); 
						scans[j] =  Convert::ToSingle(marr_umcs[index]->mint_scan_aligned); 
					}
					j++;
				}
			}

			/// Report how many features were found.  The rest of the umc array is padding.
			return j;
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCData::GetMassesAndScans(int dataset_num, 
											bool aligned, 
											float (&masses) __gc[],
											float (&scans) __gc[])
		{
			int start_index = *dynamic_cast<__box Int32*>(marr_dataset_start_index->Item[dataset_num]); 
			int stop_index = *dynamic_cast<__box Int32*>(marr_dataset_stop_index->Item[dataset_num]); 
			int num_pts = stop_index - start_index;
			int index; 
			
			masses  = new float __gc[num_pts]; 
			scans   = new float __gc[num_pts]; 

			for (index = start_index; index < stop_index; index++)
			{
				if (!aligned)
				{
					masses[index-start_index] = Convert::ToSingle(marr_umcs[index]->mdouble_mono_mass); 
					scans[index-start_index] =  Convert::ToSingle(marr_umcs[index]->mint_scan); 
				}
				else
				{
					masses[index-start_index] = Convert::ToSingle(marr_umcs[index]->mdouble_mono_mass_calibrated); 
					scans[index-start_index] =  Convert::ToSingle(marr_umcs[index]->mint_scan_aligned); 
				}
			}
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		int clsUMCData::GetMassesAndScans(int dataset_num, double (&masses) __gc[], int (&scans) __gc[])
		{
			int start_index = *dynamic_cast<__box Int32*>(marr_dataset_start_index->Item[dataset_num]); 
			int stop_index	= *dynamic_cast<__box Int32*>(marr_dataset_stop_index->Item[dataset_num]); 
			int num_pts		= stop_index - start_index;
			int index; 

			masses = new double __gc[num_pts]; 
			scans = new int __gc[num_pts]; 
			for (index = start_index; index < stop_index; index++)
			{
				masses[index-start_index] = marr_umcs[index]->mdouble_mono_mass; 
				scans[index-start_index]  = marr_umcs[index]->mint_scan; 
			}
			return start_index; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCData::GetDataIndex(int dataset_num, int &start_index, int &stop_index)
		{
			start_index = *dynamic_cast<__box Int32*>(marr_dataset_start_index->Item[dataset_num]); 
			stop_index = *dynamic_cast<__box Int32*>(marr_dataset_stop_index->Item[dataset_num]); 
			return; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCData::GetMinMaxScan(int dataset_num, int __gc & min_scan, int __gc & max_scan)
		{
			min_scan = *dynamic_cast<__box Int32*>(marr_dataset_min_scan->Item[dataset_num]); 
			max_scan = *dynamic_cast<__box Int32*>(marr_dataset_max_scan->Item[dataset_num]); 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCData::GetAllUMCsAsPoints(std::vector<MultiAlignEngine::Clustering::Point> &vectPoints)
		{
			RemoveUMCArrayPadding(); 
			vectPoints.clear(); 
			vectPoints.resize(mint_num_umcs); 
			MultiAlignEngine::Clustering::Point pt; 
			for (int umcNum = 0; umcNum < mint_num_umcs; umcNum++)
			{
				pt.mint_index      = umcNum; 
				pt.mdouble_mass    = marr_umcs[umcNum]->mdouble_mono_mass_calibrated; 
				pt.mdouble_net     = marr_umcs[umcNum]->mdouble_net; 
				pt.mint_datasetID  = marr_umcs[umcNum]->DatasetId;
				pt.mint_charge     = marr_umcs[umcNum]->ChargeRepresentative;
				pt.mdouble_driftTime = marr_umcs[umcNum]->DriftTime;
				vectPoints[umcNum] = pt; 
			}
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCData::SetUMCClusterNums(std::vector<int> &vectClusterIndices)
		{
			RemoveUMCArrayPadding(); 
			if ((int) vectClusterIndices.size() !=  mint_num_umcs)
			{
				throw new System::ApplicationException(S"Incorrect number of elements in cluster index vector"); 
			}
			for (int umcNum = 0; umcNum < mint_num_umcs; umcNum++)
			{
				marr_umcs[umcNum]->mint_cluster_index = vectClusterIndices[umcNum]; 
			}
			mblnClusterIndicesSet = true; 
		}
		/*************************************************************************************************************
		*
		*
		*
		*************************************************************************************************************/
		void clsUMCData::CalculateClusters()
		{
			if (!mblnClusterIndicesSet)
			{
				throw new System::ApplicationException(S"Cluster indices were not set before CalculateClusters was called"); 
			}
			RemoveUMCArrayPadding(); 
			mobjClusterData = new clsClusterData(marr_umcs); 
		}
	}
}