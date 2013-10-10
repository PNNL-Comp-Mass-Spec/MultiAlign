#pragma once
#include <string> 
#include <vector> 
#include <set> 
#include <hash_map> 
#include <map> 
#include "Protein.h"
#include "MassTag.h"
#include "../LCMSWarp/MassTimeFeature.h"



namespace MultiAlignEngine
{
	namespace MassTags
	{
		class MassTagDB
		{
			struct ProteinMassTagPair
			{
				int mintProtienId;
				int mintMassTagId; 
			};
			struct cmpProteinMassTagPair
			{
				bool operator()(const ProteinMassTagPair &s1, const ProteinMassTagPair &s2) const
				{
					if (s1.mintMassTagId != s2.mintMassTagId) 
						return (s1.mintMassTagId < s2.mintMassTagId); 
					return s1.mintProtienId < s2.mintProtienId; 
				}
			};
			std::set<ProteinMassTagPair, cmpProteinMassTagPair> msetProteinMassTag;
			// vector of mass tags for the current database
			std::vector<MassTag> mvectMassTags;
			// vector of proteins for the current database
			std::vector<Protein> mvectProteins;
			// hash of protein reference id to index in vector of proteins
			stdext::hash_map<int,int> mhashProteinId2Index; 
			// hash of protein reference id to index in vector of proteins
			stdext::hash_map<int, int> mhashMasstagId2Index; 
			// multimap from masstag id to index of protein in vector mvectProteins
			std::multimap<int,int> mmapMassTagId2ProteinIndex;  
			// multimap from proteinId to index of mass tag in vector mvectMassTags
			std::multimap<int,int> mmapProteinId2MassTagIndex; 
		public:
			const std::vector<MassTag>* GetMassTagVector() const { return &mvectMassTags;} ;
			const std::vector<Protein>* GetProteinVector() const { return &mvectProteins;} ;
			const stdext::hash_map<int, int> & GetMassTagId2IndexHash() const { return mhashMasstagId2Index; }
			const std::multimap<int,int> & GetMassTagId2ProteinIndexMap() const { return mmapMassTagId2ProteinIndex; }

			MassTagDB(); 
			~MassTagDB(); 
			// this function adds a mass tag to the vector of mass tags
			inline void AddMassTag(MassTag &massTag)
			{
				if (mhashMasstagId2Index.find(massTag.mintMassTagId) == mhashMasstagId2Index.end())
				{
					mhashMasstagId2Index[massTag.mintMassTagId] = (int) mvectMassTags.size();  
					mvectMassTags.push_back(massTag); 
				}
			}
			inline MassTag GetMassTagFromIndex(int index)
			{
				return mvectMassTags[index]; 
			}
			inline int GetMassTagCount()
			{
				return (int) mvectMassTags.size();
			}

			inline MassTag GetMassTagFromID(int massTagId)
			{
				if (mhashMasstagId2Index.find(massTagId) == mhashMasstagId2Index.end())
				{
					throw "Mass tag not found. Incorrect mass tag ID requested";
				}
				// lets get index of mass tag id.
				int massTagIndex = mhashMasstagId2Index[massTagId];  
				return mvectMassTags[massTagIndex]; 
			}
		// this function adds mappings of mass tags to the MassTagDB. Those mass tags which are 
		// not already loaded in the hash of mass tags are ignored. 
			inline void AddProteinMassTagMatch(int refid, std::string &proteinName, int massTagId)
			{
				if (mhashMasstagId2Index.find(massTagId) == mhashMasstagId2Index.end())
				{
					return;
				}
				// lets get index of mass tag id.
				int massTagIndex = mhashMasstagId2Index[massTagId];  

				//let get index of protein 

				int proteinIndex = -1; 
				if(mhashProteinId2Index.find(refid) == mhashProteinId2Index.end())
				{
					Protein prot; 
					proteinIndex = (int) mvectProteins.size(); 
					prot.mintRefID = refid; 
					prot.mstrName = proteinName; 
					mvectProteins.push_back(prot); 
					// set index of protein into hash with key as refid.
					mhashProteinId2Index[refid] = proteinIndex; 
				}
				else
				{
					proteinIndex = mhashProteinId2Index[refid]; 
				}
				ProteinMassTagPair protMTPair; 
				protMTPair.mintMassTagId = massTagId;
				protMTPair.mintProtienId = refid;
				if (msetProteinMassTag.find(protMTPair) == msetProteinMassTag.end())
				{
					mmapMassTagId2ProteinIndex.insert(std::pair<int,int>(massTagId, proteinIndex)); 
					mmapProteinId2MassTagIndex.insert(std::pair<int,int>(refid, massTagIndex)); 
					msetProteinMassTag.insert(protMTPair); 
				}
			}
			// for a given database with masstags already inserted into it, this function extracts 
			// the relevant proteins and places them in the destination mass tag database. 
			void CopyRelevantProteinsToMassTagDB(MultiAlignEngine::MassTags::MassTagDB &mtDBDest) const; 
			void GetMassTagsAsMassTimeFeatures(std::vector<MultiAlignEngine::Alignment::MassTimeFeature> 
				&vectFeatures); 
			int GetNumMassTags() { return (int) mvectMassTags.size(); }
			int GetNumProteins() { return (int) mvectProteins.size(); }
			void GetInformationForMassTags(std::vector<int> &vectMassTagIds, std::vector<MassTag> &vectMassTags,
				std::vector<Protein> &vectProteins, std::multimap<int,int> &mapMassTagIndex2ProteinIndex,
				stdext::hash_map <int, int> &hashMapMassTagId2Index) const;  
		};
	}
}
