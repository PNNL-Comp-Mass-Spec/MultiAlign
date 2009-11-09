#include "StdAfx.h"
#include "clsMassTimeTag.h" 
#include ".\clsPeakMatchingProcessor.h"
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	namespace PeakMatching
	{
		clsPeakMatchingProcessor::clsPeakMatchingProcessor(void)
		{
			mdblMassTolerance = 30; 
			mdblNETTolerance = 0.05;
			mintPercentDone = 0; 
		}

		clsPeakMatchingProcessor::~clsPeakMatchingProcessor(void)
		{
		}

		clsPeakMatchingResults* clsPeakMatchingProcessor::PerformPeakMatching(Features::clsUMCData *umcData, 
			int datasetIndex, MassTags::clsMassTagDB *masstagDB)
		{
			System::Collections::ArrayList *arrMSFeatures = umcData->GetMassAndTimeTags(datasetIndex); 

			// copy mass tag database. 
			System::Collections::ArrayList *arrMassTags = masstagDB->GetMassAndTimeTags(); 

			// now Merge second array into first. 
			arrMSFeatures->AddRange(arrMassTags); 
			// sort all features by mass. 
			arrMSFeatures->Sort(); 
			// now go through the feature list. For each MS feature, scan to find matches and add them to 
			// the peak matching results. 

			clsPeakMatchingResults *peakMatchingResults = new clsPeakMatchingResults(); 

			int numElements = arrMSFeatures->get_Count(); 
			int elemNum = 0;
			while ( elemNum < numElements)
			{
				mintPercentDone = (int)((elemNum*100)/numElements); 
				clsMassTimeTag *mtTag = dynamic_cast<clsMassTimeTag *>(arrMSFeatures->Item[elemNum]); 
				if (!mtTag->mblnMSMS)
				{
					double lowerNET = mtTag->mdblNET - mdblNETTolerance; 
					double higherNET = mtTag->mdblNET + mdblNETTolerance; 

					double currentMassTolerance = mtTag->mdblMass * mdblMassTolerance / 1000000.0;
					double lowerMass = mtTag->mdblMass - currentMassTolerance;  
					double higherMass = mtTag->mdblMass + currentMassTolerance;  

					int matchIndex = elemNum -1;
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
							MultiAlignEngine::Features::clsUMC *umc = umcData->GetUMC(mtTag->mintID); 
							MultiAlignEngine::MassTags::clsMassTag *massTag = masstagDB->GetMassTag(mtTagToMatch->mintID); 
							peakMatchingResults->AddPeakMatchResult(massTag, umc); 
						}
						matchIndex--; 
					}

					matchIndex = elemNum + 1; 
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
							MultiAlignEngine::Features::clsUMC *umc = umcData->GetUMC(mtTag->mintID); 
							MultiAlignEngine::MassTags::clsMassTag *massTag = masstagDB->GetMassTag(mtTagToMatch->mintID); 
							peakMatchingResults->AddPeakMatchResult(massTag, umc); 
						}
						matchIndex++; 
					}
				
				}
				elemNum++; 
			}
			// Done with peak matching.  Its time to add back the protein results, from the masstagDB to the 
			// ones in the clsPeakMatchingResults. 
			peakMatchingResults->ExtractProteinInformation(masstagDB); 
			// Done peak matching!
			return peakMatchingResults; 
		}
		clsPeakMatchingResults* clsPeakMatchingProcessor::PerformPeakMatching(Features::clsClusterData *clusterData, 
			MassTags::clsMassTagDB *masstagDB)
		{
			System::Collections::ArrayList *arrMSFeatures = clusterData->GetMassAndTimeTags(); 

			// copy mass tag database. 
			System::Collections::ArrayList *arrMassTags = masstagDB->GetMassAndTimeTags(); 

			// now Merge second array into first. 
			arrMSFeatures->AddRange(arrMassTags); 
			// sort all features by mass. 
			arrMSFeatures->Sort(); 
			// now go through the feature list. For each MS feature, scan to find matches and add them to 
			// the peak matching results. 

			clsPeakMatchingResults *peakMatchingResults = new clsPeakMatchingResults(); 

			int numElements = arrMSFeatures->get_Count(); 
			int elemNum = 0;
			while ( elemNum < numElements)
			{
				mintPercentDone = (int)((elemNum*100)/numElements); 
				clsMassTimeTag *mtTag = dynamic_cast<clsMassTimeTag *>(arrMSFeatures->Item[elemNum]); 
				if (!mtTag->mblnMSMS)
				{
					double lowerNET = mtTag->mdblNET - mdblNETTolerance; 
					double higherNET = mtTag->mdblNET + mdblNETTolerance; 

					double currentMassTolerance = mtTag->mdblMass * mdblMassTolerance / 1000000.0;
					double lowerMass = mtTag->mdblMass - currentMassTolerance;  
					double higherMass = mtTag->mdblMass + currentMassTolerance;  

					int matchIndex = elemNum -1;
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
							MultiAlignEngine::Features::clsCluster *cluster = clusterData->GetCluster(mtTag->mintID); 
							MultiAlignEngine::MassTags::clsMassTag *massTag = masstagDB->GetMassTag(mtTagToMatch->mintID); 
							peakMatchingResults->AddPeakMatchResult(massTag, cluster, mtTag->mintID); 
						}
						matchIndex--; 
					}

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
							// it is an MSMS mass and time tag, and it is withing mass and time tolerances.
							// add it.
							MultiAlignEngine::Features::clsCluster *cluster = clusterData->GetCluster(mtTag->mintID); 
							MultiAlignEngine::MassTags::clsMassTag *massTag = masstagDB->GetMassTag(mtTagToMatch->mintID); 
							peakMatchingResults->AddPeakMatchResult(massTag, cluster, mtTag->mintID); 
						}
						matchIndex++; 
					}
				
				}
				elemNum++; 
			}
			// Done with peak matching.  Its time to add back the protein results, from the masstagDB to the 
			// ones in the clsPeakMatchingResults. 
			peakMatchingResults->ExtractProteinInformation(masstagDB); 
			peakMatchingResults->UpdatePeakMatchArrays(); 
			// Done peak matching!
			return peakMatchingResults; 
		}
	}
}