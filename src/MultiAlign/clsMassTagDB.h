#pragma once
#include "clsMassTag.h" 
#include "clsProtein.h" 
#include "MassTags/MassTagDB.h"
namespace MultiAlignEngine
{
	namespace MassTags
	{
		// it is assumed that the destination has enough memory allocated..(god forbid!)
		inline void CopyStr(const char *src, System::String *dest)
		{
			if (src == 0 || strlen(src) == 0)
			{
				dest = new System::String(""); 
				return; 
			}

			int len = strlen(src); 
			dest = new System::String(src); 
		}
		// it is assumed that the destination has enough memory allocated..(god forbid!)
		inline void CopyStr(System::String *src, char *dest)
		{
			if (src == 0 || src == S"" || src->get_Length() == 0)
			{
				dest[0] = '\0'; 
				return; 
			}

			int len = src->get_Length(); 
			for (int i = 0; i < len; i++)
			{
				dest[i] = (char) src->Chars[i]; 
			}
			dest[len] = '\0'; 
		}

		inline void CopyMassTag(const clsMassTag* mtSrc, MultiAlignEngine::MassTags::MassTag &massTagDest)
		{
			massTagDest.mintMassTagId = mtSrc->mintMassTagId; 
			massTagDest.mdblMonoMass = mtSrc->mdblMonoMass;  
			massTagDest.mdblAvgGANET = mtSrc->mdblAvgGANET;  
			massTagDest.mdblPNET = mtSrc->mdblPNET;  
			massTagDest.mdblHighXCorr = mtSrc->mdblHighXCorr;  
			massTagDest.mdblStdGANET = mtSrc->mdblStdGANET;  
			massTagDest.mdblMaxDiscriminant = mtSrc->mdblMaxDiscriminant;  
			massTagDest.mintNumObsPassingFilter = mtSrc->mintNumObsPassingFilter;  
			massTagDest.mshortModCount = mtSrc->mshortModCount;  
			massTagDest.mfltAvgFCS1 = mtSrc->mfltAvgFCS1;  
			massTagDest.mfltAvgFCS2 = mtSrc->mfltAvgFCS2;  
			massTagDest.mfltAvgFCS3 = mtSrc->mfltAvgFCS3;  
			massTagDest.mshortCleavageState = mtSrc->mshortCleavageState; 
			massTagDest.HighPeptideProphetProbability = mtSrc->mfltHighPeptideProphetProbability;

			char buffer[256]; 
			CopyStr(mtSrc->mstrPeptide, buffer); 
			massTagDest.mstrPeptide = buffer; 
			CopyStr(mtSrc->mstrModification, buffer); 
			massTagDest.mstrModification = buffer; 
		}

		inline clsMassTag* CopyMassTag(const MultiAlignEngine::MassTags::MassTag &mtSrc)
		{
			clsMassTag *massTagDest = new clsMassTag(); 
			massTagDest->mintMassTagId = mtSrc.mintMassTagId; 
			massTagDest->mdblMonoMass = mtSrc.mdblMonoMass;  
			massTagDest->mdblAvgGANET = mtSrc.mdblAvgGANET;  
			massTagDest->mdblPNET = mtSrc.mdblPNET;  
			massTagDest->mdblHighXCorr = mtSrc.mdblHighXCorr;  
			massTagDest->mdblStdGANET = mtSrc.mdblStdGANET;  
			massTagDest->mdblMaxDiscriminant = mtSrc.mdblMaxDiscriminant;  
			massTagDest->mintNumObsPassingFilter = mtSrc.mintNumObsPassingFilter;  
			massTagDest->mshortModCount = mtSrc.mshortModCount;  
			massTagDest->mfltAvgFCS1 = mtSrc.mfltAvgFCS1;  
			massTagDest->mfltAvgFCS2 = mtSrc.mfltAvgFCS2;  
			massTagDest->mfltAvgFCS3 = mtSrc.mfltAvgFCS3;  
			massTagDest->mshortCleavageState = mtSrc.mshortCleavageState; 
			massTagDest->HighPeptideProphetProbability = mtSrc.HighPeptideProphetProbability;

			massTagDest->mstrPeptide = new System::String (mtSrc.mstrPeptide.c_str()); 
			massTagDest->mstrModification = new System::String (mtSrc.mstrModification.c_str()); 
			return massTagDest; 
		}

		inline clsProtein* CopyProtein(const MultiAlignEngine::MassTags::Protein &protSrc)
		{
			clsProtein *proteinDest = new clsProtein(); 
			proteinDest->mintRefID = protSrc.mintRefID; 
			proteinDest->mstrProteinName = new System::String(protSrc.mstrName.c_str()); 
			return proteinDest; 
		}

		inline void Copy(const std::vector<MultiAlignEngine::MassTags::MassTag>* src, MassTags::clsMassTag* (&dest) __gc[])
		{
			int numPts = (int) src->size(); 
			dest = new MassTags::clsMassTag* __gc [numPts]; 
			for (int ptNum = 0; ptNum < numPts; ptNum++)
			{
				dest[ptNum] = CopyMassTag((*src)[ptNum]); 
			}
		}

		inline void Copy(const std::vector<MultiAlignEngine::MassTags::Protein>* src, MassTags::clsProtein* (&dest) __gc[])
		{
		}
		[System::Serializable]
		public __gc class clsMassTagDB
		{
			[System::NonSerialized]MultiAlignEngine::MassTags::MassTagDB __nogc *mobjMasstagDB; 
		public:
			const MultiAlignEngine::MassTags::MassTagDB* GetMassTagDB() { return mobjMasstagDB;} ; 
			clsMassTagDB(void);
			~clsMassTagDB(void);
			void AddMassTags(clsMassTag *massTags __gc[]); 
			void AddProteins(int refIds __gc[], System::String* names __gc[], int mtids __gc[]); 
			System::Collections::ArrayList *GetMassAndTimeTags(double daltonshift); 
			inline clsMassTag* GetMassTag(int masstagID)
			{
				MultiAlignEngine::MassTags::MassTag mt =  mobjMasstagDB->GetMassTagFromID(masstagID); 
				return CopyMassTag(mt); 
			}
			inline clsMassTag* GetMassTagFromIndex(int index)
			{
				MultiAlignEngine::MassTags::MassTag mt =  mobjMasstagDB->GetMassTagFromIndex(index); 
				return CopyMassTag(mt); 
			}
			/// Gets the total number of mass tags available.
			inline int GetMassTagCount()
			{
				return mobjMasstagDB->GetMassTagCount();
			}
			// copies the protein to peptides mapping in mobjMasstagDB to destination database
			void CopyRelevantProteinsToMassTagDB(MultiAlignEngine::MassTags::MassTagDB &mtDBDest); 
			void GetMassTagsAsMassTimeFeatures(std::vector<MultiAlignEngine::Alignment::MassTimeFeature> &vectFeatures); 
		};
	}
}