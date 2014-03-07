#pragma once
#include "clsUMCData.h"
#include "clsAlignmentFunction.h" 
#include "classAlignmentResidualData.h"
#include "clsMassTagDb.h" 
#include "LCMSWarp/LCMSWarp.h"

namespace MultiAlignEngine
{
	namespace Alignment
	{
		public __value enum enmState
		{
			IDLE=0,SETTING_VARIABLES, MS_ALIGNMENT_INITIALIZED, MASSTAG_ALIGNMENT_INITIALIZED, ALIGNING_TO_MS, ALIGNING_TO_MASSTAG, DONE, ERROR
		};
		// When Alignee dataset is cluster data then input time is net of cluster
		// When Alignee dataset is umc data then input time is scan
		// When Baseline dataset is umc data, then input baseline time is net
		// When baseline dataset is mass tag database then input baseline time is avgGANET
		[System::Serializable]
		public __gc class clsAlignmentProcessor: 
			public System::Runtime::Serialization::IDeserializationCallback,
			public System::IDisposable
		{
			bool mblnAligningToMassTagDB; 
			bool mblnMassTagDBLoaded; 
			bool mblnClusterAlignee; 
			// in case alignment was to ms features this keeps track of the minimum scan in the 
			// reference features. These are needed because LCMSWarp uses net values for reference
			// values and those are scaled from 0, 1. To scale back, we need these guys.
			int mintMinReferenceDatasetScan; 
			// in case alignment was to ms features this keeps track of the maximum scan in the 
			// reference features. These are needed because LCMSWarp uses net values for reference
			// values and those are scaled from 0, 1. To scale back, we need these guys.
			int mintMaxReferenceDatasetScan; 
			
			int mintMinAligneeDatasetScan; 
			int mintMaxAligneeDatasetScan; 
			double mdblMinAligneeDatasetMZ; 
			double mdblMaxAligneeDatasetMZ; 

			// Flag indicating how much is currently split.
			int mintPercentDone; 

			/*////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				Function: Set Alignee Features 
				Note:
					For a given dataset of UMC's.  Set the alignee (who we are warping to the baseline) features.
					Above boundary will determine if we should allow features above the m/z range or below if the 
					splitMZBoundary flag is set in the alignment options to true				
				Arguments:
				Returns: bool - true if the mz falls within the boundary, false if contrary.
			////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
			bool ValidateAlignmentBoundary( bool split, 
											double mz,
											classAlignmentMZBoundary* boundary);																

			enmState			menmState; 
			System::String *	mstrMessage; 
			MultiAlignEngine::Alignment::clsAlignmentOptions *mobjAlignmentOptions; 
			[System::NonSerialized] MultiAlignEngine::Alignment::LCMSWarp __nogc *mobjLCMSWarp; 
			void ApplyAlignmentOptions(); 
			void PerformNetMassWarp(); 
			void PerformNetWarp(); 

		public:
			void Dispose();
			void OnDeserialization(System::Object* sender) 
			{
				// After being deserialized, set this unmanaged object
				mobjLCMSWarp = __nogc new MultiAlignEngine::Alignment::LCMSWarp(); 
				ApplyAlignmentOptions(); 
			}
			clsAlignmentProcessor(void);
			~clsAlignmentProcessor(void);
			double GetMassStandardDeviation();
			double GetNETStandardDeviation();
			double GetMassMean();
			double GetNETMean();

			void ApplyNETMassFunctionToAligneeDatasetFeatures(MultiAlignEngine::Features::clsUMCData* &umcData); 			
			List<clsUMC*>* ApplyNETMassFunctionToAligneeDatasetFeatures(List<clsUMC*>* &features);

			void ApplyNETMassFunctionToAligneeDatasetFeatures(MultiAlignEngine::Features::clsClusterData* &clusterData); 
			void SetAligneeDatasetFeatures(	MultiAlignEngine::Features::clsUMCData *umcData, 
											int datasetIndex,																
											classAlignmentMZBoundary* boundary);
			
			void SetAligneeDatasetFeatures(	List<clsCluster*>*			clusterData,																
											classAlignmentMZBoundary*	boundary);
			
			void ApplyNETMassFunctionToAligneeDatasetFeatures(List<clsCluster*>* &clusterData);

			//TODO: Remove!
			void SetAligneeDatasetFeatures(	MultiAlignEngine::Features::clsClusterData *clusterData,																
											classAlignmentMZBoundary* boundary); 

			void SetAligneeDatasetFeatures(  List<clsUMC*> *features, 																														
																classAlignmentMZBoundary* boundary);

			void SetReferenceDatasetFeatures(	MultiAlignEngine::Features::clsUMCData *umcData, 
												int referenceDatasetIndex); 
			void SetReferenceDatasetFeatures(MultiAlignEngine::MassTags::clsMassTagDB *massTagDB);
			void SetReferenceDatasetFeatures(List<MultiAlignEngine::MassTags::clsMassTag*>* masstags, bool isDatabase);
			void SetReferenceDatasetFeatures(List<clsUMC*>* features);


			/*////////////////////////////////////////////////////////////////////////////////
				Property:	Net Intercept
				Note:		Returns the NET intercept value for a linear alignment. 				
			////////////////////////////////////////////////////////////////////////////////*/
			__property double get_NETIntercept()
			{
				return this->mobjLCMSWarp->GetNETIntercept();
			}			
			/*////////////////////////////////////////////////////////////////////////////////
				Property:	Net Slope
				Note:		Returns the NET slope value for a linear alignment. 				
			////////////////////////////////////////////////////////////////////////////////*/
			__property double get_NETSlope()
			{
				return this->mobjLCMSWarp->GetNETSlope();
			}			
			/*////////////////////////////////////////////////////////////////////////////////
				Property:	Net Intercept
				Note:		Returns the NET R-Squared value value for a linear alignment. 				
			////////////////////////////////////////////////////////////////////////////////*/
			__property double get_NETLinearRSquared()
			{
				return this->mobjLCMSWarp->GetNETLinearRSquared();				
			}
			__property enmState get_Status()
			{
				return menmState; 
			}
			__property System::String* get_StatusMessage()
			{
				return mstrMessage; 
			}
			__property int get_PercentComplete()
			{
				if (menmState == ALIGNING_TO_MS || menmState == ALIGNING_TO_MASSTAG)
					return mobjLCMSWarp->GetPercentComplete(); 
				return mintPercentDone; 
			}			
			__property clsAlignmentOptions* get_AlignmentOptions()
			{
				return mobjAlignmentOptions; 
			}
			__property void set_AlignmentOptions(clsAlignmentOptions* value)
			{
				mobjAlignmentOptions = value;
				ApplyAlignmentOptions(); 
			}
			__property bool get_MassTagDBLoaded()
			{
				return mblnMassTagDBLoaded; 
			}

			void SetDataForAlignmentToMSFeatures(MultiAlignEngine::Features::clsUMCData *umcData, 
				System::String *aligneeDataset, System::String *referenceDataset); 
			void SetDataForAlignmentToMSFeatures(MultiAlignEngine::Features::clsUMCData *umcData, 
				int aligneeDatasetIndex, int referenceDatasetIndex); 
			void SetDataForAlignmentToMassTagDatabase(MultiAlignEngine::Features::clsUMCData *umcData, 
				System::String* aligneeDataset, MultiAlignEngine::MassTags::clsMassTagDB *massTagDB); 
			void SetDataForAlignmentToMassTagDatabase(MultiAlignEngine::Features::clsUMCData *umcData, 
				int aligneeDatasetIndex, MultiAlignEngine::MassTags::clsMassTagDB *massTagDB); 
			void PerformAlignmentToMSFeatures(); 
			void PerformAlignmentToMassTagDatabase(); 
			clsAlignmentFunction* GetAlignmentFunction(); 									
			/*////////////////////////////////////////////////////////////////////////////////
				Function:	GetErrorHistograms
				Note:		Calculates the mass and net error histograms.
				Arguments:  N/A
				Returns:	void
			////////////////////////////////////////////////////////////////////////////////*/
			void GetErrorHistograms(double  massBin,
									double  netBin,
									double  driftBin,
									double (&massErrorHistogram) __gc[,],
									double (&netErrorHistogram)  __gc[,],
									double (&driftErrorHistogram)  __gc[,]);
			/*////////////////////////////////////////////////////////////////////////////////
				Function:	GetAlignmentHeatMap
				Note:		Calculates the alignment heat scores.
				Arguments:  N/A
				Returns:	void
			////////////////////////////////////////////////////////////////////////////////*/
			void GetAlignmentHeatMap(float (&alignmentScores) __gc[,], float (&xIntervals) __gc[], float (&yNETs ) __gc[]); 
			void GetReferenceNETRange(float __gc & minRefNET, float __gc & maxRefNET); 		
			
			/*////////////////////////////////////////////////////////////////////////////////
				Function:	GetResiduals
				Note:		Calculates the all of the residual data for the alignment.
				Arguments:  void
				Returns:	Object holding all the residual data stored in respective arrays.
			////////////////////////////////////////////////////////////////////////////////*/
			classAlignmentResidualData* GetResidualData();
		};
		
	}
}