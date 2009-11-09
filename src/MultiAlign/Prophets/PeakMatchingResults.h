#pragma once
#include <vector>
#include <map>
#include "../MassTags/MassTagDB.h" 
#include "../UMCCreation/UMC.h" 
#include "PeakMatch.h" 

namespace MultiAlignEngine
{
	namespace PeakMatching
	{
		class PeakMatchingResults
		{
			MassTags::MassTagDB mobjMasstagDB; 
			std::vector<PeakMatch> mvectPeakMatches; 
			std::multimap<int,int> mmapMasstagId2PeakMatchIndex; 
			std::multimap<int,int> mmapFeatureIndex2PeakMatch; 
		public:
			PeakMatchingResults();
			~PeakMatchingResults();
			inline void AddPeakMatchResult(MassTags::MassTag &mt, short repCharge, double umcMass, double umcNET, int featureIndex)
			{
				PeakMatch match(mt.mintMassTagId, featureIndex, mt.mfltAvgFCS1, 
					mt.mfltAvgFCS2, mt.mfltAvgFCS3, 0, repCharge, (mt.mdblMonoMass - umcMass)/umcMass*1000000.0, 
					mt.mdblAvgGANET - umcNET); 
				mvectPeakMatches.push_back(match); 
				int numPeakMatches = mvectPeakMatches.size(); 
				mmapMasstagId2PeakMatchIndex.insert(std::pair<int, int>(mt.mintMassTagId, numPeakMatches-1)); 
				mmapFeatureIndex2PeakMatch.insert(std::pair<int, int>(featureIndex, numPeakMatches-1)); 
				mobjMasstagDB.AddMassTag(mt); 
			}
			int GetNumMatches() { return (int) mvectPeakMatches.size(); }
			int GetNumMassTagsMatched() { return mobjMasstagDB.GetNumMassTags(); } 
			int GetNumProteinsMatched() { return mobjMasstagDB.GetNumProteins(); } 
			void CopyRelevantProteinsFromMassTagDB(const MultiAlignEngine::MassTags::MassTagDB *mtDBSrc); 
			void GetMatches(std::vector<int> &vectFeatureIndices, std::vector<int> &vectMassTagIndices , 
				std::vector<int> &vectProteinIndices, const std::vector<MultiAlignEngine::MassTags::Protein>* &vectProteins, 
				const std::vector<MultiAlignEngine::MassTags::MassTag>* &vectMasstags) const; 
		}; 
	}
}