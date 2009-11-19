
#include "PeakMatchingResults.h"

namespace MultiAlignEngine
{
	namespace PeakMatching
	{
		PeakMatchingResults::PeakMatchingResults()
		{
		}

		PeakMatchingResults::~PeakMatchingResults()
		{
		}
		void PeakMatchingResults::CopyRelevantProteinsFromMassTagDB(const MultiAlignEngine::MassTags::MassTagDB *mtDBSrc)
		{
			mtDBSrc->CopyRelevantProteinsToMassTagDB(mobjMasstagDB); 
		}
		void PeakMatchingResults::GetMatches(std::vector<int> &vectFeatureIndices, std::vector<int> &vectMassTagIndices, 
			std::vector<int> &vectProteinIndices , const std::vector<MultiAlignEngine::MassTags::Protein>* &vectProteins, 
			const std::vector<MultiAlignEngine::MassTags::MassTag>* &vectMasstags) const
		{
			// The mass tag database has stored in it all the proteins and mass tags corresponding to the matches.
			// So thats where we need to get all the data from. 
			int numMatches = (int) mvectPeakMatches.size(); 
			std::set <int> massTagID; 

			stdext::hash_map <int, int > massTagSeen;
			// first get all mass tags in the matches into the set of mass tags. 
			for (int matchNum = 0; matchNum < numMatches; matchNum++)
			{
				PeakMatch match = mvectPeakMatches[matchNum]; 
				massTagSeen.insert(std::pair<int,int>(match.mintMasstagID, matchNum)); 
			}

			// copy hash to mass tag vector
			std::vector<int> vectMassTagIDs; 
			vectMassTagIDs.reserve(massTagSeen.size()); 
			for (stdext::hash_map <int, int >::iterator massTagIter = massTagSeen.begin();
				massTagIter != massTagSeen.end(); massTagIter++)
			{
				vectMassTagIDs.push_back((*massTagIter).first); 
			}


			vectMasstags = mobjMasstagDB.GetMassTagVector(); 
			vectProteins = mobjMasstagDB.GetProteinVector(); 
			const std::multimap<int,int> mapMassTagId2ProteinIndex = mobjMasstagDB.GetMassTagId2ProteinIndexMap(); 
			const stdext::hash_map <int, int > hashMapMassTagId2Index  = mobjMasstagDB.GetMassTagId2IndexHash();

			// now the relevant mass tags are copied, their ids are copied, the protein names are copied. 
			// So lets create the table of matches using the three vectors vectFeatureIndices, vectMassTagIndices and 
			// vectProteinIndices. Basically, vectFeatureIndices will have the index of the ms feature in a peak match.
			// the vectMassTagIndices will have the corresponding massTagID, and vectProteinIndices will have the corresponding
			// parent protein. 
			for (std::multimap<int,int>::const_iterator featureIter = mmapFeatureIndex2PeakMatch.begin(); featureIter != mmapFeatureIndex2PeakMatch.end(); 
				featureIter++)
			{
				int featureIndex = (*featureIter).first; 
				int peakMatchIndex = (*featureIter).second; 
				PeakMatch pkMatch = mvectPeakMatches[peakMatchIndex]; 
				int massTagID = pkMatch.mintMasstagID; 
				stdext::hash_map <int, int>::const_iterator massTagIterHash = hashMapMassTagId2Index.find(massTagID); 
				int massTagIndex = (*massTagIterHash).second; 
				// now go through each of the parent proteins of this massTagID and push the triplet into their 
				// corresponding vectors
				for (std::multimap<int,int>::const_iterator massTagIter = mapMassTagId2ProteinIndex.find(massTagID); 
					massTagIter != mapMassTagId2ProteinIndex.end(); 
					massTagIter++)
				{
					if ((*massTagIter).first != massTagID)
						break; 
					vectFeatureIndices.push_back(featureIndex); 
					vectMassTagIndices.push_back(massTagIndex); 
					vectProteinIndices.push_back((*massTagIter).second); 
				}
			}
		}
	}
}