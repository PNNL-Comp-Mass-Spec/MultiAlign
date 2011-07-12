#pragma once
#include "clsMassTagDB.h" 
#include "clsMassTag.h" 
#include "clsMassTagDatabaseOptions.h"

namespace MultiAlignEngine
{
	namespace MassTags
	{
		public __value enum DBConnectionStatus { InActive=0, Connecting, Running, Fetching, Done}; 
		[System::Serializable]
		public __gc class clsMTDBLoader
		{
			// Use dbo.GetMassTagToProteinNameMap to get the mass tag to protein map for 
			// mass tags with same filters. 

			/* Sample output from GetMassTagsPlusPepProphetStats
			Mass_Tag_ID	Peptide	Monoisotopic_Mass	Net_Value_to_Use	PNET	High_Normalized_Score	StD_GANET	High_Discriminant_Score	Peptide_Obs_Count_Passing_Filter	Mod_Count	Mod_Description	High_Peptide_Prophet_Probability	Mass_Tag_ID	ObsCount_CS1	ObsCount_CS2	ObsCount_CS3	PepProphet_FScore_Avg_CS1	PepProphet_FScore_Avg_CS2	PepProphet_FScore_Avg_CS3	PepProphet_Probability_Max_CS1	PepProphet_Probability_Max_CS2	PepProphet_Probability_Max_CS3	PepProphet_Probability_Avg_CS1	PepProphet_Probability_Avg_CS2
																																																																																																																									-----------	---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	-------------------------	----------------	--------------	---------------------	--------------	-----------------------	--------------------------------	-----------	---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	--------------------------------	-----------	------------	------------	------------	-------------------------	-------------------------	-------------------------	------------------------------	------------------------------	------------------------------	------------------------------	------------------------------
			52885296	SAGAP	401.1910278	0.3140762	8.32E-02	1.5015	0	2.64E-02	0	0	0.327279	52885296	1	0	0	1.56337	-100	-100	0.327279	-100	-100	0.327279	-100	-100
			4760981	IAAGA	401.2274124	-4.83E-03	9.58E-02	1.4394	0	0.472767	0	0	0	4760981	1	0	0	1.77162	-100	-100	0	-100	-100	0	-100	-100
			52948826	GGVGI	401.2274124	<NULL>	0.2015168	1.6411	<NULL>	7.14E-02	0	0	0.210938	52948826	1	0	0	1.27347	-100	-100	0.210938	-100	-100	0.210938	-100	-100
			52830495	GGVGL	401.2274124	<NULL>	0.1813344	1.6411	<NULL>	7.10E-02	0	0	0.210938	52830495	1	0	0	1.27347	-100	-100	0.210938	-100	-100	0.210938	-100	-100
			7941049	QGAGA	402.1862759	1.33E-03	6.73E-02	1.5494	0	5.39E-02	0	0	0	7941049	1	0	0	0.143947	-100	-100	0	-100	-100	0	-100	-100
			14398280	GAGAQ	402.1862759	-6.73E-02	6.39E-02	1.6525	0	1.15E-02	0	0	0	14398280	1	0	0	-0.178665	-100	-100	0	-100	-100	0	-100	-100
			4470013	GAGAGA	402.1862759	-4.09E-02	6.76E-02	1.5623	3.34E-02	4.14E-03	0	0	0	4470013	2	0	0	-0.320956	-100	-100	0	-100	-100	0	-100	-100
			4470095	GAGAAG	402.1862759	6.92E-02	6.89E-02	1.7056	8.57E-02	2.26E-02	0	0	0	4470095	2	0	0	0.410759	-100	-100	0	-100	-100	0	-100	-100
			14398279	GAGAK	402.2226605	-6.73E-02	6.24E-02	1.6525	0	9.91E-02	0	0	0	14398279	1	0	0	0.111803	-100	-100	0	-100	-100	0	-100	-100
			*/

			//static System::String* mstrStoredProcedureMassTagsPlusPeptideProphet = S"GetMassTagsPlusPepProphetStats"; 
			static System::String* mstrStoredProcedureMassTagsPlusPeptideProphet = S"GetMassTagsPlusConformers"; 

			/* Sample output from GetMassTagToProteinNameMap
			 Protein_ID : different proteins with same sequence have the same id. 
			 Use Ref_ID to link protein information. 
				Mass_Tag_ID Protein_ID  Reference Ref_ID                                                                                                                        		
				36715995    107         STM0005   1                                                                                                                         		
				127178008   107         STM0005   2                                                                                                                      		
				133235815   107         STM0005   3                                                                                                                     		
				74040034    108         STM0006                                                                                                                          		
				36601727    109         STM0007                                                                                                                         		
				36652078    109         STM0007                                                                                                                          		
				36660826    109         STM0007                                                                                                                          		
				36660867    109         STM0007                                                                                                                          		
				36620881    109         STM0007                        		
			*/

			static System::String* mstrStoredProcedureProtein2MassTags = S"GetMassTagToProteinNameMap"; 

			//static System::String* mstrStoredProcedureGetProteinsMappedToMassTags = S"GetProteinsMappedToMassTags"; 
			//@ConfirmedOnly tinyint = 0,						-- Set to 1 to only include MTs with Is_Confirmed = 1
			//@MinimumHighNormalizedScore real = 0,			-- The minimum value required for High_Normalized_Score; 0 to allow all
			//@MinimumPMTQualityScore real = 0,				-- The minimum PMT_Quality_Score to allow; 0 to allow all
			//@MinimumHighDiscriminantScore real = 0,			-- The minimum High_Discriminant_Score to allow; 0 to allow all
			//@MinimumPeptideProphetProbability real = 0,		-- The minimum High_Peptide_Prophet_Probability to allow; 0 to allow all
			//@MassTagIDList varchar(max) = '',				-- If defined, then returns the proteins mapped to the given mass tags; the @ConfirmedOnly and @Minimum... parameters are ignored if a list of Mass Tag IDs is provided
			//@IncludeProteinSequence tinyint = 0


			System::String* mstrMessage; 
			System::String* mstrConnectionStr; 
			System::String* mstrOleConnectionStr;

			[System::NonSerialized] System::Data::SqlClient::SqlConnection *mconnection;
			[System::NonSerialized] System::Data::OleDb::OleDbConnection *mOleconnection;
			int mint_num_loaded;
			DBConnectionStatus menmStatus; 
			clsMassTagDatabaseOptions *mobjMassTagDBOptions;
			void LoadMassTags(clsMassTagDB *mtDB); 
			void LoadMassTagsToProteinMap(clsMassTagDB *mtDB); 
			void LoadMassTagsAccess(clsMassTagDB *mtDB); 
			void LoadMassTagsToProteinMapAccess(clsMassTagDB *mtDB); 

			int mint_num_mtids_total;
		public:
			clsMTDBLoader(clsMassTagDatabaseOptions *options)
			{
				mobjMassTagDBOptions = options; 
				
				if (options->menm_databaseType == MassTagDatabaseType::SQL)
				{
					mstrConnectionStr = System::String::Concat(S"Server=", mobjMassTagDBOptions->mstrServer, S";Database=",
					mobjMassTagDBOptions->mstrDatabase, S";Integrated Security=no; User ID=", mobjMassTagDBOptions->mstrUserID, 
					S";PWD=", mobjMassTagDBOptions->mstrPasswd); 				
					mconnection = new System::Data::SqlClient::SqlConnection(mstrConnectionStr);
				}
				else
				{		
					//mstrOleConnectionStr	=  System::String::Concat(S"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=",options->mstr_databaseFilePath);
					mstrOleConnectionStr	=  System::String::Concat(S"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=",options->mstr_databaseFilePath);
					mOleconnection			= new System::Data::OleDb::OleDbConnection(mstrOleConnectionStr);
				}
				menmStatus = DBConnectionStatus::InActive; 
				
			}
			~clsMTDBLoader(void);
			clsMassTagDB* LoadMassTagDatabase(); 			
			__property int get_PercentComplete()
			{
				// we'll just pretend that 10000 mass tags will be loaded.. once we go over,
				// the setting will reset..
				if (mint_num_mtids_total   != 0)
					return  Convert::ToInt32(((Convert::ToDouble(mint_num_loaded) / Convert::ToDouble(mint_num_mtids_total))*100.0));
				return (mint_num_loaded % 1000)/10; 
			}
			__property System::String* get_StatusMessage()
			{
				return mstrMessage; 
			}
		};
	}
}