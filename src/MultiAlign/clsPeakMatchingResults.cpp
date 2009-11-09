#include "StdAfx.h"
#include "clsUtilities.h" 
#include ".\clsPeakMatchingResults.h"
#using <mscorlib.dll>
namespace MultiAlignEngine
{
	namespace PeakMatching
	{

		clsPeakMatchingResults::clsPeakMatchingResults(void)
		{
			mobjPeakMatchingResults = __nogc new MultiAlignEngine::PeakMatching::PeakMatchingResults(); 
			mint_numMatches			= 0;
		}
		clsPeakMatchingResults::~clsPeakMatchingResults(void)
		{
			if (mobjPeakMatchingResults != 0)
				delete mobjPeakMatchingResults; 
		}
		void clsPeakMatchingResults::ExtractProteinInformation(MassTags::clsMassTagDB *masstagDB)
		{
			mobjPeakMatchingResults->CopyRelevantProteinsFromMassTagDB(masstagDB->GetMassTagDB()); 
		}

		void clsPeakMatchingResults::GetMatches(clsPeakMatchingTriplet* (&arrPeakMatchingTriplet) __gc[], 
			MassTags::clsProtein* (&arrProteins) __gc[], MassTags::clsMassTag* (&arrMasstags) __gc[])
		{
			std::vector<int> vectFeatureIndices; 
			std::vector<int> vectMassTagIndices; 
			std::vector<int> vectProteinIndices; 
			const std::vector<MultiAlignEngine::MassTags::Protein>* vectProteins; 
			const std::vector<MultiAlignEngine::MassTags::MassTag>* vectMassTags; 

			mobjPeakMatchingResults->GetMatches(vectFeatureIndices, vectMassTagIndices, vectProteinIndices, 
				vectProteins, vectMassTags); 

			int numTriplets = (int) vectFeatureIndices.size(); 
			arrPeakMatchingTriplet = new clsPeakMatchingTriplet* __gc [numTriplets]; 
			for (int tripletNum = 0; tripletNum < numTriplets; tripletNum++)
			{
				clsPeakMatchingTriplet *trip = new clsPeakMatchingTriplet(); 
				trip->mintFeatureIndex = vectFeatureIndices[tripletNum]; 
				trip->mintMassTagIndex = vectMassTagIndices[tripletNum]; 
				trip->mintProteinIndex = vectProteinIndices[tripletNum]; 
				arrPeakMatchingTriplet[tripletNum] = trip;
			}

			int numProteins = (int) vectProteins->size(); 
			arrProteins = new MassTags::clsProtein* __gc [numProteins]; 
			for (int proteinNum = 0; proteinNum < numProteins; proteinNum++)
			{
				arrProteins[proteinNum] = MultiAlignEngine::MassTags::CopyProtein((*vectProteins)[proteinNum]); 
			}

			int numMassTags = (int) vectMassTags->size(); 
			arrMasstags = new MassTags::clsMassTag* __gc [numMassTags]; 
			for (int massTagNum = 0; massTagNum < numMassTags; massTagNum++)
			{
				arrMasstags[massTagNum] = MassTags::CopyMassTag((*vectMassTags)[massTagNum]); 
			}

			mint_numMatches = mobjPeakMatchingResults->GetNumMatches();
		}		
	}
}