#pragma once
#include "Prophets/PeakMatchingResults.h"
#include "clsMassTagDB.h"
#include "clsUMC.h" 
#include "clsCluster.h" 
#include "clsPeakMatch.h" 
#include "clsDataSummaryAttribute.h"

namespace MultiAlignEngine
{
	namespace PeakMatching
	{
		[System::Serializable]
		public __gc class clsPeakMatchingResults
		{

		public:

			/// Not sure why this is defined here...
			// TODO: Clean up this bit
			[System::Serializable]
			__gc class clsPeakMatchingTriplet : public System::IComparable
			{
			public:
				int mintFeatureIndex;
				int mintMassTagIndex; 
				int mintProteinIndex; 				
				int CompareTo(System::Object* obj)
				{ 
					clsPeakMatchingTriplet *u = dynamic_cast<clsPeakMatchingTriplet *>(obj); 
					if (mintProteinIndex == u->mintProteinIndex) 
					{
						if (mintMassTagIndex == u->mintMassTagIndex)
							return 0;
						if (mintMassTagIndex < u->mintMassTagIndex)
							return -1; 
						return 1; 
					} 
					if (mintProteinIndex < u->mintProteinIndex) 
						return -1; 
					return 1; 
				} 
			}; 

			clsPeakMatchingTriplet* marrPeakMatchingTriplet __gc[]; 
			MassTags::clsProtein* marrProteins __gc[];
			MassTags::clsMassTag* marrMasstags __gc[]; 			
			int mint_numMatches;

			clsPeakMatchingResults(void);
			~clsPeakMatchingResults(void);

			
			/// We cannot serialize this object because its not managed. 
			[System::NonSerialized] 			
			MultiAlignEngine::PeakMatching::PeakMatchingResults __nogc *mobjPeakMatchingResults; 

			inline void AddPeakMatchResult(MassTags::clsMassTag *mtTag, Features::clsUMC *umc)
			{
				MultiAlignEngine::MassTags::MassTag massTag; 
				CopyMassTag(mtTag, massTag); 
				mobjPeakMatchingResults->AddPeakMatchResult(massTag, umc->mshort_class_rep_charge,  umc->mdouble_mono_mass_calibrated, umc->mdouble_net, umc->mint_umc_index); 
			}
			inline void AddPeakMatchResult(MassTags::clsMassTag *mtTag, Features::clsCluster *cluster, int clusterIndex)
			{
				MultiAlignEngine::MassTags::MassTag massTag; 
				CopyMassTag(mtTag, massTag); 
				mobjPeakMatchingResults->AddPeakMatchResult(massTag, cluster->mshort_charge, cluster->mdouble_mass_calibrated, cluster->mdouble_net, clusterIndex); 
			}
			// This function extracts the proteins that correspond to the mass tags that have been 
			// added to the mass tag database in mobjPeakMatchingResults 
			void ExtractProteinInformation(MassTags::clsMassTagDB *masstagDB);

			// Function gets the matches that are stored in it. 
			// arrFeatureIndices are the actual feature indices for a match. The arrMassTagIndices element in the same row, is the index into 
			// the array of mass tags returned in arrMassTags array, while the arrProteinIndices is the index into the array of proteins
			// returned in arrProteins. 
			void GetMatches(clsPeakMatchingTriplet* (&arrPeakMatchingTriplet) __gc[], MassTags::clsProtein* (&arrProteins) __gc[],
				MassTags::clsMassTag* (&arrMasstags) __gc[]); 


			void UpdatePeakMatchArrays() 
			{
				GetMatches(marrPeakMatchingTriplet, marrProteins, marrMasstags); 
			}

			[clsDataSummaryAttribute("Number Mass Tags Matched")]
			__property int get_NumMassTagsMatched()
			{
				if (marrMasstags == 0)
					return 0;
				return marrMasstags->Length; 				
				//if (mobjPeakMatchingResults == NULL)
				//	return 0;
				//return mobjPeakMatchingResults->GetNumMassTagsMatched(); 
			}

			
			[clsDataSummaryAttribute("Number of Matches")]
			__property int get_NumMatches()
			{
				return mint_numMatches;
				//if (mobjPeakMatchingResults == NULL)
				//	return 0; 
				//return mobjPeakMatchingResults->GetNumMatches(); 
			}

			
			[clsDataSummaryAttribute("Number of Proteins Matched")]
			__property int get_NumProteinsMatched()
			{
				//if (mobjPeakMatchingResults == NULL)
				//	return 0; 
				//return mobjPeakMatchingResults->GetNumProteinsMatched();
				if (marrProteins == 0)
					return 0;
				return marrProteins->Length;
			}
		};
	}
}