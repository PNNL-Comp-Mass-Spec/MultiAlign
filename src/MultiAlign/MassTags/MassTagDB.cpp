#include "StdAfx.h"
#include ".\MassTagDB.h"

namespace MultiAlignEngine
{
	namespace MassTags
	{
		MassTagDB::MassTagDB(void)
		{
		}

		MassTagDB::~MassTagDB()
		{
		}
		void MassTagDB::CopyRelevantProteinsToMassTagDB(MultiAlignEngine::MassTags::MassTagDB &mtDBDest) const
		{
			int numMassTagsInDest = (int) mtDBDest.mvectMassTags.size(); 
			stdext::hash_map<int, int> relevantMassTagsHash; 
			for (int mtNum = 0; mtNum < numMassTagsInDest; mtNum++)
			{
				MassTag mt = mtDBDest.mvectMassTags[mtNum];
				relevantMassTagsHash[mt.mintMassTagId] = 1; 
			}

			for (std::multimap<int, int>::const_iterator iter = mmapMassTagId2ProteinIndex.begin(); 
				iter != mmapMassTagId2ProteinIndex.end(); iter++)
			{
				int currentMassTagId = (*iter).first; 
				int currentProteinIndex = (*iter).second; 
				if (relevantMassTagsHash.find(currentMassTagId) != relevantMassTagsHash.end())
				{
					Protein prot = mvectProteins[currentProteinIndex]; 
					mtDBDest.AddProteinMassTagMatch(prot.mintRefID, prot.mstrName, currentMassTagId); 
				}
			}
		}
		void MassTagDB::GetMassTagsAsMassTimeFeatures(std::vector<MultiAlignEngine::Alignment::MassTimeFeature> 
			&vectFeatures) 
		{
			vectFeatures.reserve(mvectMassTags.size()); 
			int numMassTags = (int) mvectMassTags.size(); 
			MultiAlignEngine::Alignment::MassTimeFeature mtFeature; 
			for (int massTagNum = 0; massTagNum < numMassTags; massTagNum++)
			{
				MassTag massTag = mvectMassTags[massTagNum]; 
				mtFeature.mdouble_aligned_net = massTag.mdblAvgGANET; 
				mtFeature.mdouble_mono_mass = massTag.mdblMonoMass; 
				mtFeature.mdouble_mono_mass_calibrated = massTag.mdblMonoMass; 
				mtFeature.mdouble_mono_mass_original = massTag.mdblMonoMass; 
				mtFeature.mdouble_mz = massTag.mdblMonoMass/2 + 1.00782; // assume a charge of 2. Not used anyways.
				mtFeature.mdouble_net = massTag.mdblAvgGANET; 
				mtFeature.mint_id = massTag.mintMassTagId; 
				vectFeatures.push_back(mtFeature); 
			}
		}
		void MassTagDB::GetInformationForMassTags(std::vector<int> &vectMassTagIds, std::vector<MassTag> &vectMassTags,
			std::vector<Protein> &vectProteins, std::multimap<int,int> &mapMassTagIndex2ProteinIndex,
			stdext::hash_map <int, int > &hashMapMassTagId2Index) const
		{
			// go through each masstagid in vectMassTagIds, pick up the corresponding MassTag s into vectMassTags, the 
			// proteins into vectPRoteins and the matching between mass tag and protein indices in mapMassTagIndex2ProteinIndex,
			// and a map between massTagId and index in hashMapMassTagId2Index.
		}
	}
}