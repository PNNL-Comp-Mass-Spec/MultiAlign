#pragma once
#using <mscorlib.dll>
#include "clsDataSummaryAttribute.h"
#include "clsFactorInfo.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections;


namespace MultiAlignEngine
{
	public __value enum LabelingType  {Leu_C13N15=0, SPICAT, ICAT, SulfoNHSLCBiotin, 
		PheC13, N14N15, none, PhIST, CleavableICAT, O18, PhIAT, PEOBiotin, Unknown } ;

	public __value enum DeisotopingTool  { ICR2LS=0, Decon2LS, NA, LCMSFeatureFinder } ;

	[System::Serializable]
	public __gc class clsDatasetInfo : public System::IComparable
	{

	private:
		 List<String*>*		     mlist_assignedValues;
		 List<clsFactorInfo *>*  mlist_factorInformation;

	public:
		 [clsDataSummaryAttribute("Dataset ID")]
		 System::String* mstrDatasetId; 
		 [clsDataSummaryAttribute("Volume")]
		 System::String* mstrVolume; 
		 [clsDataSummaryAttribute("Instrument Folder")]
		 System::String* mstrInstrumentFolder; 
		 [clsDataSummaryAttribute("Dataset Path")]
		 System::String* mstrDatasetPath; 
		 [clsDataSummaryAttribute("Results Folder")]
		 System::String* mstrResultsFolder;
		 [clsDataSummaryAttribute("Dataset Name")]
		 System::String* mstrDatasetName;
		 [clsDataSummaryAttribute("Analysis Job ID")]
		 System::String* mstrAnalysisJobId;
		 [clsDataSummaryAttribute("Local Path")]
		 System::String* mstrLocalPath; 
		 [clsDataSummaryAttribute("Alias")]
		 System::String* mstrAlias;

		 [clsDataSummaryAttribute("Column ID")]
		 int mintColumnID; 
		 [clsDataSummaryAttribute("Acquisition Start")]
		 System::DateTime mdateAcquisitionStart; 
		 [clsDataSummaryAttribute("Instrument")]
		 System::String* mstrInstrment; 
		 [clsDataSummaryAttribute("Labeling Type")]
		 LabelingType menmLabelingType; 

		 [clsDataSummaryAttribute("Deisotoping Tool")]
		 DeisotopingTool menmDeisotopingTool; 

		 [clsDataSummaryAttribute("Comment")]
		 System::String* mstrComment; 
		 [clsDataSummaryAttribute("Operator")]
		 System::String* mstrOperator; 

		// Example of block information:
		/*
		 * mintBlockId	mstrReplicateName	minExperimentID	mintRunOrder	mstrBlockingFactor	mintBatchID	
			1	caulo_049_1	32675	1	Global_replicate_block_1	155	
			1	caulo_055_1	32681	2	Global_replicate_block_1	155	
			1	caulo_050_1	32676	3	Global_replicate_block_1	155	
			1	caulo_048_1	32674	4	Global_replicate_block_1	155	
			1	caulo_054_1	32680	5	Global_replicate_block_1	155	
			1	caulo_057_1	32683	6	Global_replicate_block_1	155
		*/
		/// <summary>
		/// batch in which things were run.
		/// </summary>
		 [clsDataSummaryAttribute("Batch ID")]
		 int mintBatchID;
		/// <summary>
		/// block inside the batch in which 
		/// </summary>
		 [clsDataSummaryAttribute("Block ID")]
		 int mintBlockID; 
		/// <summary>
		/// Experiment from which this sample is generated. 
		/// </summary>
		 [clsDataSummaryAttribute("Experiment ID")]
		 int mintExperimentID; 
		/// <summary>
		/// Alternative name for the experiment concatenated with a _[replicate num].
		/// </summary>
		 [clsDataSummaryAttribute("Replicate Name")]
		 System::String* mstrReplicateName;
		/// <summary>
		/// order in which things were run in a block.
		/// </summary>
		 [clsDataSummaryAttribute("Run Order")]
		 int mintRunOrder; 
		/// <summary>
		/// Name for blocking factor. Its a one to one mapping with mintBlockID for datasets in a batch
		/// </summary>
		 [clsDataSummaryAttribute("Blocking Factor")]
		 System::String* mstrBlockingFactor; 

		 bool selected;
		 bool factorsDefined;
		 static int mintNumFactorsSpecified = 0; 
		 static const int MAX_LEVELS = 100; 
		 
		
		int CompareTo(System::Object* obj)
		{
				clsDatasetInfo* tmp = dynamic_cast<clsDatasetInfo*>(obj);
				return(mstrAnalysisJobId->CompareTo(tmp->mstrAnalysisJobId));
		} 

		/*
			List of the factors associated with this dataset.
		*/
		__property List<clsFactorInfo *>* get_Factors()
		{
				return mlist_factorInformation;
		}
			
		__property void set_Factors(List<clsFactorInfo *>* value)
		{
			mlist_factorInformation = value;
		}

	
		/*
			List of the factors values assigned to this dataset.
		*/
		__property List<String *>* get_AssignedFactorValues()
		{			
			return mlist_assignedValues;
		}			
		__property void set_AssignedFactorValues(List<String *>* value)
		{
				mlist_assignedValues = value;		
		}

    	static clsDatasetInfo()
		{			
		}

		/*
			Default Constructor for a Dataset information class.
		*/
		clsDatasetInfo()
		{
			mintBatchID			= 0;
			mintRunOrder		= 0;
			factorsDefined		= false;

			mlist_assignedValues	= new List<String *>(); 
			mlist_factorInformation = new List<clsFactorInfo *>(); 
		}
		 __property bool get_Blocked()
		{
				return mintBatchID > 0; 
		}
	}; 
}