
#include "clsMassTimeTag.h" 
#include ".\clsPeakMatchingProcessor.h"
#using <mscorlib.dll>

using namespace MultiAlignEngine::Features;
using namespace MultiAlignEngine::MassTags;


using namespace System::Collections::Generic;


namespace MultiAlignEngine
{
	namespace PeakMatching
	{
		clsPeakMatchingProcessor::clsPeakMatchingProcessor(void)
		{
			mdblMassTolerance		= 30; 
			mdblNETTolerance		= 0.05;
			mdblDriftTimeTolerance	= CONST_DRIFT_TIME_TOLERANCE; 
			mintPercentDone			= 0; 
		}

		clsPeakMatchingProcessor::~clsPeakMatchingProcessor(void)
		{
		}


		clsPeakMatchingResults* clsPeakMatchingProcessor::PerformPeakMatching(	Features::clsUMCData	*umcData, 
																				int						datasetIndex,
																				List<clsMassTag*>*		arrMassTags)
		{
			/*////////////////////////////////////////////////////////////////////////////////////////////////
				Here we get a list of all the clusters which are groups of UMC's.  We are 
				going to match the to the database.
			////////////////////////////////////////////////////////////////////////////////////////////////*/
			System::Collections::ArrayList *arrMSFeatures = umcData->GetMassAndTimeTags(datasetIndex); 	

			/*////////////////////////////////////////////////////////////////////////////////////////////////
				We merge the range of mass tag features found in the AMT-DB into the list of UMC Features.			
			////////////////////////////////////////////////////////////////////////////////////////////////*/
			arrMSFeatures->AddRange(arrMassTags); 

			/*////////////////////////////////////////////////////////////////////////////////////////////////
				And then we sort by mass, this way we don't have to search against the entire database O(NM)
				or O(N^2).  This is on best case only a few items during peak matching we have to look at.
				Algorithmically, this is the same or worse because of the sorting step, pratically, we are 
				saving a lot of time.
			////////////////////////////////////////////////////////////////////////////////////////////////*/ 
			arrMSFeatures->Sort(); 

			/*////////////////////////////////////////////////////////////////////////////////////////////////
				Now go through the feature list. For each MS feature, scan to find matches and add them to 
				the peak matching results. 
			////////////////////////////////////////////////////////////////////////////////////////////////*/	
			clsPeakMatchingResults *peakMatchingResults = new clsPeakMatchingResults(); 

			int numElements = arrMSFeatures->get_Count(); 
			int elemNum = 0;
			/*
				Here we iterate through all of the elements in the list.  NOTE SOMETHING VERY IMPORTANT: The 
				mtTag->mblnMSMS is a flag that indicates if the the mass tag is from the database or if it's
				from the acquired sample run.  Notice it's use.  It was part of a trick to so that we could
				do the below loops to practically minimize peak matching running time.  
			*/
			while ( elemNum < numElements)
			{
				mintPercentDone = (int)((elemNum*100)/numElements); 
				clsMassTimeTag *mtTag = dynamic_cast<clsMassTimeTag *>(arrMSFeatures->Item[elemNum]); 
				if (!mtTag->mblnMSMS)
				{
					double lowerNET				= mtTag->mdblNET - mdblNETTolerance; 
					double higherNET			= mtTag->mdblNET + mdblNETTolerance; 

					double lowerDritfTime		= mtTag->mdblDriftTime - mdblDriftTimeTolerance;
					double higherDriftTime		= mtTag->mdblDriftTime + mdblDriftTimeTolerance;

					double currentMassTolerance = mtTag->mdblMass * mdblMassTolerance / 1000000.0;
					double lowerMass			= mtTag->mdblMass - currentMassTolerance;  
					double higherMass			= mtTag->mdblMass + currentMassTolerance;  

					int matchIndex = elemNum -1;
					/*
						Since the mass time values are sorted by mass, we look backwards until we find a tag that 
						is less than the lower mass.  We add this value to the list of potential peak matching results.
						Here we are looking first 
					*/
					while (matchIndex >= 0)
					{
						clsMassTimeTag *mtTagToMatch = dynamic_cast<clsMassTimeTag *>(arrMSFeatures->Item[matchIndex]);
						if (mtTagToMatch->mdblMass < lowerMass)
						{
							break; 
						}
						if (mtTagToMatch->mblnMSMS && mtTagToMatch->mdblNET >= lowerNET && mtTagToMatch->mdblNET <= higherNET)
						{							
							// it is an MSMS mass and time tag, and it is withing mass and time tolerances.
							// add it.
							if (mtTagToMatch->mdblDriftTime >= lowerDritfTime && mtTagToMatch->mdblDriftTime <= higherDriftTime)
							{
								MultiAlignEngine::Features::clsUMC *umc = umcData->GetUMC(mtTag->mintID); 
								MultiAlignEngine::MassTags::clsMassTag *massTag = masstagDB->GetMassTag(mtTagToMatch->mintID); 
								peakMatchingResults->AddPeakMatchResult(massTag, umc); 
							}
						}
						matchIndex--; 
					}

					matchIndex = elemNum + 1; 
					/*
						Do the same as above, but look until we find the upper mass tolerance.
					*/
					while (matchIndex < numElements)
					{
						MultiAlignEngine::clsMassTimeTag *mtTagToMatch = dynamic_cast<clsMassTimeTag *>(arrMSFeatures->Item[matchIndex]);
						if (mtTagToMatch->mdblMass > higherMass)
						{
							break; 
						}
						if (mtTagToMatch->mblnMSMS && mtTagToMatch->mdblNET >= lowerNET && mtTagToMatch->mdblNET <= higherNET)
						{
							// it is an MSMS mass and time tag, and it is withing mass and time tolerances.
							// add it.
							if (mtTagToMatch->mdblDriftTime >= lowerDritfTime && mtTagToMatch->mdblDriftTime <= higherDriftTime)
							{
								clsUMC		*umc	 = umcData->GetUMC(mtTag->mintID); 
								clsMassTag	*massTag = masstagDB->GetMassTag(mtTagToMatch->mintID); 
								peakMatchingResults->AddPeakMatchResult(massTag, umc); 
							}
						}
						matchIndex++; 
					}
				
				}
				elemNum++; 
			}
			/*//////////////////////////////////////////////////////////////////////////////////////////////////////
				Done with peak matching.  Its time to add back the protein results, from the masstagDB to
				the  ones in the clsPeakMatchingResults. 
		
				--- Grab protein information from the database if it exists.
			//////////////////////////////////////////////////////////////////////////////////////////////////////*/
			peakMatchingResults->ExtractProteinInformation(masstagDB); 			
			return peakMatchingResults; 
		}

		clsPeakMatchingResults* clsPeakMatchingProcessor::PerformPeakMatching(	Features::clsClusterData *clusterData, 
																				MassTags::clsMassTagDB *masstagDB)
		{
			return PerformPeakMatching(clusterData,	masstagDB,0.0);
		}
		
		clsPeakMatchingResults* clsPeakMatchingProcessor::PerformPeakMatching(	Features::clsClusterData *clusterData, 
																				MassTags::clsMassTagDB *masstagDB,
																				double shiftDaltons)
		{
			
			/*////////////////////////////////////////////////////////////////////////////////////////////////
				Here we get a list of all the clusters which are groups of UMC's.  We are 
				going to match the to the database.
			////////////////////////////////////////////////////////////////////////////////////////////////*/
			System::Collections::ArrayList *arrMSFeatures = clusterData->GetMassAndTimeTags(0); 
							
			/*////////////////////////////////////////////////////////////////////////////////////////////////
				Here we are copying the database of AMT's.  We apply the shift of 0 to the db here.
				Later we'll apply the dalton shift if applicable.
			////////////////////////////////////////////////////////////////////////////////////////////////*/			
			System::Collections::ArrayList *arrMassTags = masstagDB->GetMassAndTimeTags(shiftDaltons); 

			/*////////////////////////////////////////////////////////////////////////////////////////////////
				We merge the range of mass tag features found in the AMT-DB into the list of UMC Features.			
			////////////////////////////////////////////////////////////////////////////////////////////////*/
			arrMSFeatures->AddRange(arrMassTags); 

			/*////////////////////////////////////////////////////////////////////////////////////////////////
				And then we sort by mass, this way we don't have to search against the entire database O(NM)
				or O(N^2).  This is on best case only a few items during peak matching we have to look at.
				Algorithmically, this is the same or worse because of the sorting step, pratically, we are 
				saving a lot of time.
			////////////////////////////////////////////////////////////////////////////////////////////////*/
			arrMSFeatures->Sort(); 

			/*////////////////////////////////////////////////////////////////////////////////////////////////
				Now go through the feature list. For each MS feature, scan to find matches and add them to 
				the peak matching results. 
			////////////////////////////////////////////////////////////////////////////////////////////////*/			
			clsPeakMatchingResults *peakMatchingResults = new clsPeakMatchingResults(); 

			int numElements = arrMSFeatures->get_Count(); 
			int elemNum		= 0;


			/*
				Here we iterate through all of the elements in the list.  NOTE SOMETHING VERY IMPORTANT: The 
				mtTag->mblnMSMS is a flag that indicates if the the mass tag is from the database or if it's
				from the acquired sample run.  Notice it's use.  It was part of a trick to so that we could
				do the below loops to practically minimize peak matching running time.  
			*/
			while ( elemNum < numElements)
			{
				mintPercentDone = (int)((elemNum*100)/numElements); 

				/// Get the mass tag to match - this is the tag that defines our tolerance bounds.
				clsMassTimeTag *mtTag = dynamic_cast<clsMassTimeTag *>(arrMSFeatures->Item[elemNum]); 
				if (!mtTag->mblnMSMS)
				{
					double lowerNET  = mtTag->mdblNET - mdblNETTolerance; 
					double higherNET = mtTag->mdblNET + mdblNETTolerance; 

					double lowerDritfTime  = mtTag->mdblDriftTime - mdblDriftTimeTolerance;
					double higherDriftTime = mtTag->mdblDriftTime + mdblDriftTimeTolerance;

					double currentMassTolerance = mtTag->mdblMass * mdblMassTolerance / 1000000.0;
					double lowerMass  = mtTag->mdblMass - currentMassTolerance ;  
					double higherMass = mtTag->mdblMass + currentMassTolerance ;  

					int matchIndex = elemNum -1;

					/*
						Since the mass time values are sorted by mass, we look backwards until we find a tag that 
						is less than the lower mass.  We add this value to the list of potential peak matching results.
						Here we are looking first 
					*/
					while (matchIndex >= 0)
					{
						clsMassTimeTag *mtTagToMatch = dynamic_cast<clsMassTimeTag *>(arrMSFeatures->Item[matchIndex]);
						if (mtTagToMatch->mdblMass < lowerMass)
						{
							break; 
						}
						if (mtTagToMatch->mblnMSMS && mtTagToMatch->mdblNET >= lowerNET && mtTagToMatch->mdblNET <= higherNET)
						{
							if (mtTagToMatch->mdblDriftTime >= lowerDritfTime && mtTagToMatch->mdblDriftTime <= higherDriftTime)
							{
								clsCluster *cluster	= clusterData->GetCluster(mtTag->mintID); 
								clsMassTag *massTag	= masstagDB->GetMassTag(mtTagToMatch->mintID); 
								peakMatchingResults->AddPeakMatchResult(massTag, cluster, mtTag->mintID); 
							}
						}
						matchIndex--; 
					}

					/*
						Do the same as above, but look until we find the upper mass tolerance.
					*/
					matchIndex = elemNum + 1; 
					while (matchIndex < numElements)
					{
						clsMassTimeTag *mtTagToMatch = dynamic_cast<clsMassTimeTag *>(arrMSFeatures->Item[matchIndex]);
						if (mtTagToMatch->mdblMass > higherMass)
						{
							break; 
						}
						if (mtTagToMatch->mblnMSMS && mtTagToMatch->mdblNET >= lowerNET && mtTagToMatch->mdblNET <= higherNET)
						{
							if (mtTagToMatch->mdblDriftTime >= lowerDritfTime && mtTagToMatch->mdblDriftTime <= higherDriftTime)
							{
								clsCluster *cluster = clusterData->GetCluster(mtTag->mintID); 
								clsMassTag *massTag = masstagDB->GetMassTag(mtTagToMatch->mintID); 
								peakMatchingResults->AddPeakMatchResult(massTag, cluster, mtTag->mintID); 
							}
						}
						matchIndex++; 
					}									
				}
				elemNum++; 
			}
			/*//////////////////////////////////////////////////////////////////////////////////////////////////////
				Done with peak matching.  Its time to add back the protein results, from the masstagDB to
				the  ones in the clsPeakMatchingResults. 
		
				--- Grab protein information from the database if it exists.
			//////////////////////////////////////////////////////////////////////////////////////////////////////*/
			peakMatchingResults->ExtractProteinInformation(masstagDB); 
			peakMatchingResults->UpdatePeakMatchArrays(); 			
			return peakMatchingResults; 
		}

		clsPeakMatchingResults* clsPeakMatchingProcessor::PerformPeakMatching(	List<clsCluster*> * clusters,
																				MassTags::clsMassTagDB *masstagDB,
																				double shiftDaltons)
		{
			
			/*////////////////////////////////////////////////////////////////////////////////////////////////
				Here we get a list of all the clusters which are groups of UMC's.  We are 
				going to match the to the database.
			////////////////////////////////////////////////////////////////////////////////////////////////*/
			System::Collections::ArrayList *arrMSFeatures	= new System::Collections::ArrayList();
			for(int i = 0; i < clusters->Count; i++)
			{
				clsCluster * cluster = clusters->Item[i];
				clsMassTimeTag * mtTag = new clsMassTimeTag(cluster->mdouble_mass_calibrated,
															cluster->mdouble_aligned_net,
															cluster->mdouble_driftTime,
															i);
				arrMSFeatures->Add(mtTag);
			}

			System::Collections::ArrayList *arrMassTags		= masstagDB->GetMassAndTimeTags(shiftDaltons); 

			/*////////////////////////////////////////////////////////////////////////////////////////////////
				We merge the range of mass tag features found in the AMT-DB into the list of UMC Features.			
			////////////////////////////////////////////////////////////////////////////////////////////////*/
			arrMSFeatures->AddRange(arrMassTags); 

			/*////////////////////////////////////////////////////////////////////////////////////////////////
				And then we sort by mass, this way we don't have to search against the entire database O(NM)
				or O(N^2).  This is on best case only a few items during peak matching we have to look at.
				Algorithmically, this is the same or worse because of the sorting step, pratically, we are 
				saving a lot of time.
			////////////////////////////////////////////////////////////////////////////////////////////////*/
			arrMSFeatures->Sort(); 

			/*////////////////////////////////////////////////////////////////////////////////////////////////
				Now go through the feature list. For each MS feature, scan to find matches and add them to 
				the peak matching results. 
			////////////////////////////////////////////////////////////////////////////////////////////////*/			
			clsPeakMatchingResults *peakMatchingResults = new clsPeakMatchingResults(); 

			int numElements = arrMSFeatures->get_Count(); 
			int elemNum		= 0;


			/*
				Here we iterate through all of the elements in the list.  NOTE SOMETHING VERY IMPORTANT: The 
				mtTag->mblnMSMS is a flag that indicates if the the mass tag is from the database or if it's
				from the acquired sample run.  Notice it's use.  It was part of a trick to so that we could
				do the below loops to practically minimize peak matching running time.  
			*/
			while ( elemNum < numElements)
			{
				mintPercentDone = (int)((elemNum*100)/numElements); 

				/// Get the mass tag to match - this is the tag that defines our tolerance bounds.
				clsMassTimeTag *mtTag = dynamic_cast<clsMassTimeTag *>(arrMSFeatures->Item[elemNum]); 
				if (!mtTag->mblnMSMS)
				{
					double lowerNET  = mtTag->mdblNET - mdblNETTolerance; 
					double higherNET = mtTag->mdblNET + mdblNETTolerance; 

					double lowerDritfTime  = mtTag->mdblDriftTime - mdblDriftTimeTolerance;
					double higherDriftTime = mtTag->mdblDriftTime + mdblDriftTimeTolerance;

					double currentMassTolerance = mtTag->mdblMass * mdblMassTolerance / 1000000.0;
					double lowerMass  = mtTag->mdblMass - currentMassTolerance ;  
					double higherMass = mtTag->mdblMass + currentMassTolerance ;  

					int matchIndex = elemNum -1;

					/*
						Since the mass time values are sorted by mass, we look backwards until we find a tag that 
						is less than the lower mass.  We add this value to the list of potential peak matching results.
						Here we are looking first 
					*/
					while (matchIndex >= 0)
					{
						clsMassTimeTag *mtTagToMatch = dynamic_cast<clsMassTimeTag *>(arrMSFeatures->Item[matchIndex]);
						if (mtTagToMatch->mdblMass < lowerMass)
						{
							break; 
						}
						if (mtTagToMatch->mblnMSMS && mtTagToMatch->mdblNET >= lowerNET && mtTagToMatch->mdblNET <= higherNET)
						{
							if (mtTagToMatch->mdblDriftTime >= lowerDritfTime && mtTagToMatch->mdblDriftTime <= higherDriftTime)
							{
								clsCluster *cluster	= clusters->Item[mtTag->mintID]; 
								clsMassTag *massTag	= masstagDB->GetMassTag(mtTagToMatch->mintID); 
								peakMatchingResults->AddPeakMatchResult(massTag, cluster, mtTag->mintID); 
							}
						}
						matchIndex--; 
					}

					/*
						Do the same as above, but look until we find the upper mass tolerance.
					*/
					matchIndex = elemNum + 1; 
					while (matchIndex < numElements)
					{
						clsMassTimeTag *mtTagToMatch = dynamic_cast<clsMassTimeTag *>(arrMSFeatures->Item[matchIndex]);
						if (mtTagToMatch->mdblMass > higherMass)
						{
							break; 
						}
						if (mtTagToMatch->mblnMSMS && mtTagToMatch->mdblNET >= lowerNET && mtTagToMatch->mdblNET <= higherNET)
						{
							if (mtTagToMatch->mdblDriftTime >= lowerDritfTime && mtTagToMatch->mdblDriftTime <= higherDriftTime)
							{
								clsCluster *cluster = clusters->Item[mtTag->mintID]; 
								clsMassTag *massTag = masstagDB->GetMassTag(mtTagToMatch->mintID); 
								peakMatchingResults->AddPeakMatchResult(massTag, cluster, mtTag->mintID); 
							}
						}
						matchIndex++; 
					}									
				}
				elemNum++; 
			}
			/*//////////////////////////////////////////////////////////////////////////////////////////////////////
				Done with peak matching.  Its time to add back the protein results, from the masstagDB to
				the  ones in the clsPeakMatchingResults. 
		
				--- Grab protein information from the database if it exists.
			//////////////////////////////////////////////////////////////////////////////////////////////////////*/
			peakMatchingResults->ExtractProteinInformation(masstagDB); 
			peakMatchingResults->UpdatePeakMatchArrays(); 			
			return peakMatchingResults; 
		}
	}
}