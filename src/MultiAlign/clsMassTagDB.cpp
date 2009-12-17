
#include ".\clsmasstagdb.h"
#using <mscorlib.dll>
#include <string> 
#include "clsMassTimeTag.h"

namespace MultiAlignEngine
{
	namespace MassTags
	{

		clsMassTagDB::clsMassTagDB(void)
		{
			mobjMasstagDB = new MultiAlignEngine::MassTags::MassTagDB(); 
		}

		clsMassTagDB::~clsMassTagDB(void)
		{
		}
		
		void clsMassTagDB::AddMassTags(clsMassTag *massTags __gc[])
		{
			int numMassTags = massTags->Length; 
			MultiAlignEngine::MassTags::MassTag massTag; 
			for (int massTagNum = 0; massTagNum < numMassTags; massTagNum++)
			{
				clsMassTag *mt = massTags[massTagNum]; 
				CopyMassTag(mt, massTag); 
				mobjMasstagDB->AddMassTag(massTag); 
			}
		}

		void clsMassTagDB::AddProteins(int refIds __gc[], System::String* names __gc[], int mtids __gc[])
		{
			int numEntries = refIds->Count; 
			char buffer[256]; 
			for (int entryNum = 0; entryNum < numEntries; entryNum++)
			{
				int refId = refIds[entryNum]; 
				CopyStr(names[entryNum], buffer); 
				std::string str = buffer; 
				mobjMasstagDB->AddProteinMassTagMatch(refId, str, mtids[entryNum]); 
			}
		}

		System::Collections::ArrayList* clsMassTagDB::GetMassAndTimeTags(double shiftDaltons)
		{
			int numMTTags = mobjMasstagDB->GetNumMassTags(); 
			System::Collections::ArrayList* arrMTTags = new System::Collections::ArrayList(numMTTags); 
			for (int mtTagNum = 0; mtTagNum < numMTTags; mtTagNum++)
			{
				MultiAlignEngine::MassTags::MassTag mt = mobjMasstagDB->GetMassTagFromIndex(mtTagNum); 
				MultiAlignEngine::clsMassTimeTag *mtTag = new MultiAlignEngine::clsMassTimeTag(mt.mdblMonoMass + shiftDaltons, mt.mdblAvgGANET, mt.mintMassTagId, true); 
				arrMTTags->Add(mtTag); 
			}
			return arrMTTags; 
		}
		void clsMassTagDB::CopyRelevantProteinsToMassTagDB(MultiAlignEngine::MassTags::MassTagDB &mtDBDest)
		{
			mobjMasstagDB->CopyRelevantProteinsToMassTagDB(mtDBDest); 
		}
		void clsMassTagDB::GetMassTagsAsMassTimeFeatures(std::vector<MultiAlignEngine::Alignment::MassTimeFeature> 
			&vectFeatures)
		{
			mobjMasstagDB->GetMassTagsAsMassTimeFeatures(vectFeatures); 
		}
	}
}