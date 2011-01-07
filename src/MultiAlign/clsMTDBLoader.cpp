
#include ".\clsmtdbloader.h"
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	namespace MassTags
	{
		clsMTDBLoader::~clsMTDBLoader(void)
		{
		}

		void clsMTDBLoader::LoadMassTagsAccess(clsMassTagDB *mtDB)
		{
			System::Data::OleDb::OleDbDataReader *rdr  = 0;
			int allocated = 1000; 
			clsMassTag *arrMassTags __gc[] = new clsMassTag* __gc [allocated];

			mint_num_loaded			= 0; 
			mint_num_mtids_total    = 0;
			try
			{
				try
				{
					mOleconnection->Open();
				}
				catch(...)
				{
					mstrOleConnectionStr	=  System::String::Concat(S"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=",mobjMassTagDBOptions->mstr_databaseFilePath);
					mOleconnection			= new System::Data::OleDb::OleDbConnection(mstrOleConnectionStr);
					mOleconnection->Open();
				}
				menmStatus = DBConnectionStatus::Connecting; 

				// 1.  create a command object identifying
				//     the stored procedure
				System::Data::OleDb::OleDbCommand *cmd  = new System::Data::OleDb::OleDbCommand(
					mstrStoredProcedureMassTagsPlusPeptideProphet, mOleconnection);
				cmd->set_CommandTimeout(180); 

				// 2. set the command object so it knows
				//    to execute a stored procedure
				System::Data::CommandType cmdType = System::Data::CommandType::StoredProcedure; 
				cmd->CommandType = cmdType;

				// 3. add parameters to command, which
				//    will be passed to the stored procedure
			
				cmd->Parameters->Add(new System::Data::OleDb::OleDbParameter("@ConfirmedOnly", __box(mobjMassTagDBOptions->mbyteConfirmedTags)));
				cmd->Parameters->Add(new System::Data::OleDb::OleDbParameter("@MinimumHighNormalizedScore", __box(mobjMassTagDBOptions->mfltMinXCorr)));
				cmd->Parameters->Add(new System::Data::OleDb::OleDbParameter("@MinimumPMTQualityScore", __box(mobjMassTagDBOptions->mdecimalMinPMTScore)));
				cmd->Parameters->Add(new System::Data::OleDb::OleDbParameter("@MinimumHighDiscriminantScore", __box(mobjMassTagDBOptions->mdblMinDiscriminant)));
				cmd->Parameters->Add(new System::Data::OleDb::OleDbParameter("@MinimumPeptideProphetProbability", __box(mobjMassTagDBOptions->mdblPeptideProphetVal)));			
				
				// execute the command
				menmStatus = DBConnectionStatus::Running; 
				rdr = cmd->ExecuteReader();

				menmStatus = DBConnectionStatus::Fetching; 
				int currentlyLoaded = 0;

				try
				{
					System::Data::DataTable *dataTable  = new System::Data::DataTable();
					System::Data::OleDb::OleDbDataAdapter *adapt = new System::Data::OleDb::OleDbDataAdapter(cmd);
					
					adapt->Fill(dataTable);
					mint_num_mtids_total = dataTable->Rows->Count;

					adapt->Dispose();
					dataTable->Dispose();
				}
				catch(...)
				{
				}

				while (rdr->Read())
				{
					// Mass_Tag_ID	
					// Peptide	
					// Monoisotopic_Mass	
					// Net_Value_to_Use	
					// PNET	
					// High_Normalized_Score	
					// StD_GANET	
					// High_Discriminant_Score	
					// Peptide_Obs_Count_Passing_Filter	
					// Mod_Count	
					// Mod_Description	
					// High_Peptide_Prophet_Probability	
					// Mass_Tag_ID	
					// ObsCount_CS1	
					// ObsCount_CS2	
					// ObsCount_CS3	
					// PepProphet_FScore_Max_CS1	
					// PepProphet_FScore_Max_CS2	
					// PepProphet_FScore_Max_CS3	
					// PepProphet_Probability_Max_CS1	
					// PepProphet_Probability_Max_CS2	
					// PepProphet_Probability_Max_CS3	
					// PepProphet_Probability_Avg_CS1	
					// PepProphet_Probability_Avg_CS2

					// iterate through results, printing each to console
					clsMassTag *mass_tag = new clsMassTag(); 
					if (rdr->Item[S"Mass_Tag_ID"] != System::DBNull::Value) // which it never should really be
					{
						int id = System::Convert::ToInt32(rdr->Item[S"Mass_Tag_ID"]); 
						System::String *peptide = S"";
						if (rdr->Item[S"Peptide"] != System::DBNull::Value) 
						{
							peptide = dynamic_cast<System::String*>(rdr->Item[S"Peptide"]);
						}
						float ganet = -1;
						if (rdr->Item[S"Net_Value_to_Use"] != System::DBNull::Value) 
						{
							ganet = System::Convert::ToSingle(rdr->Item[S"Net_Value_to_Use"]);
						}
						
						//float pnet = (float) rdr->Item["PNET"];
						float xcorr_max = 0;
						if (rdr->Item[S"High_Normalized_Score"] != System::DBNull::Value)
						{
							xcorr_max = System::Convert::ToSingle(rdr->Item[S"High_Normalized_Score"]);
						}

						float std_net = 0; 
						if (rdr->Item[S"StD_GANET"] != System::DBNull::Value)
						{
							//std_net = *dynamic_cast<float*>(rdr->Item[S"StD_GANET"]); 
							std_net = System::Convert::ToSingle(rdr->Item[S"StD_GANET"]); 
						}

						double mono_mass = 0.0; 
						if (rdr->Item[S"Monoisotopic_Mass"] != System::DBNull::Value)
						{
							mono_mass = System::Convert::ToDouble(rdr->Item[S"Monoisotopic_Mass"]); 
						}
						float high_discriminant = 0;
						/*if (rdr->Item[S"High_Discriminant_Score"] != System::DBNull::Value)
						{
							high_discriminant = System::Convert::ToSingle(rdr->Item[S"High_Discriminant_Score"]);
						}*/

						int num_obs = 0;
						if (rdr->Item[S"Peptide_Obs_Count_Passing_Filter"] != System::DBNull::Value)
						{
							num_obs = System::Convert::ToInt32(rdr->Item[S"Peptide_Obs_Count_Passing_Filter"]);
						}

						int mod_count = 0; 
						if (rdr->Item[S"Mod_Count"] != System::DBNull::Value)
						{
							mod_count = System::Convert::ToInt32(rdr->Item[S"Mod_Count"]);
						}

						System::String* modification_str = S""; 
						if (rdr->Item[S"Mod_Description"] != System::DBNull::Value)
						{
							modification_str = dynamic_cast<System::String*>(rdr->Item[S"Mod_Description"]);
						}
						float highPeptideProphetProbability = 0; 
						if (rdr->Item[S"High_Peptide_Prophet_Probability"] != System::DBNull::Value)
						{
							highPeptideProphetProbability = System::Convert::ToSingle(rdr->Item[S"High_Peptide_Prophet_Probability"]);
						}

						// 	
						// Mass_Tag_ID	
						// ObsCount_CS1	
						// ObsCount_CS2	
						// ObsCount_CS3	
						// PepProphet_FScore_Max_CS1	
						// PepProphet_FScore_Max_CS2	
						// PepProphet_FScore_Max_CS3	
						float fmax_cs1 = -100;
						if (rdr->Item[S"PepProphet_FScore_Avg_CS1"] != System::DBNull::Value) 
						{
							fmax_cs1 = System::Convert::ToSingle(rdr->Item[S"PepProphet_FScore_Avg_CS1"]);
						}
						float fmax_cs2 = -100;
						if (rdr->Item[S"PepProphet_FScore_Avg_CS2"] != System::DBNull::Value) 
						{
							fmax_cs2 = System::Convert::ToSingle(rdr->Item[S"PepProphet_FScore_Avg_CS2"]);
						}
						float fmax_cs3 = -100;
						if (rdr->Item[S"PepProphet_FScore_Avg_CS3"] != System::DBNull::Value) 
						{
							fmax_cs3 = System::Convert::ToSingle(rdr->Item[S"PepProphet_FScore_Avg_CS3"]);
						}

						/// Make sure the mass tag has been seen enough times
						if (num_obs >= mobjMassTagDBOptions->mintMinObservationCountFilter)
						{
							mass_tag->mintMassTagId			= id; 
							mass_tag->mstrPeptide			= peptide;
							mass_tag->mdblAvgGANET			= ganet; 
							mass_tag->mdblHighXCorr			= xcorr_max; 
							mass_tag->mdblMaxDiscriminant	= high_discriminant;
							mass_tag->mdblMonoMass			= mono_mass; 
							mass_tag->mstrModification		= modification_str; 
							mass_tag->mshortModCount		= mod_count; 
							mass_tag->mdblStdGANET				= std_net;
							mass_tag->mintNumObsPassingFilter	= num_obs; 
							mass_tag->mfltAvgFCS1				= fmax_cs1; 
							mass_tag->mfltAvgFCS2				= fmax_cs2; 
							mass_tag->mfltAvgFCS3				= fmax_cs3;
							mass_tag->HighPeptideProphetProbability	= highPeptideProphetProbability;

							if (mass_tag->mdblAvgGANET != -1)
							{
								if (currentlyLoaded == allocated)
								{
									currentlyLoaded = 0;
									mtDB->AddMassTags(arrMassTags); 
								}
								arrMassTags[currentlyLoaded++] = mass_tag; 
								mint_num_loaded++; 
							}
						}
					}
				}
				if (currentlyLoaded > 0)
				{
					clsMassTag* tempArr __gc[] = new clsMassTag* __gc[currentlyLoaded];
					for (int i = 0; i < currentlyLoaded; i++)
					{
						tempArr[i] = arrMassTags[i]; 
					}
					mtDB->AddMassTags(tempArr); 
				}
				mstrMessage = System::String::Concat("Loaded ", System::Convert::ToString(mint_num_loaded), S" mass tags"); 
			}
			catch(System::Exception *ex)
			{
				throw (ex); 
			}

			if (mOleconnection != 0)
			{
				mOleconnection->Close();
			}
			if (rdr != 0)
			{
				rdr->Close();
			}
			menmStatus = DBConnectionStatus::Done; 
		}
		void clsMTDBLoader::LoadMassTagsToProteinMapAccess(clsMassTagDB *mtDB)
		{
			/* Sample output from GetMassTagToProteinNameMap
				Mass_Tag_ID Protein_ID  Reference
				36715995    107         STM0005
				127178008   107         STM0005
				133235815   107         STM0005
				74040034    108         STM0006
				36601727    109         STM0007
				36652078    109         STM0007
				36660826    109         STM0007
				36660867    109         STM0007
				36620881    109         STM0007
			*/
			int allocated = 1000; 
			System::String *proteinNames __gc[] = new System::String* __gc[allocated]; 
			int proteinIds __gc[] = new int __gc[allocated]; 
			int mtIds __gc[] = new int __gc[allocated]; 

			System::Data::OleDb::OleDbDataReader *rdr  = 0;
			mint_num_loaded = 0; 

			try
			{
				try
				{
					mOleconnection->Open();
				}
				catch(...)
				{
					mstrOleConnectionStr	=  System::String::Concat(S"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=",mobjMassTagDBOptions->mstr_databaseFilePath);
					mOleconnection			= new System::Data::OleDb::OleDbConnection(mstrOleConnectionStr);
					mOleconnection->Open();
				}
				menmStatus = DBConnectionStatus::Connecting; 

				// 1.  create a command object identifying
				//     the stored procedure
				System::Data::OleDb::OleDbCommand *cmd  = new System::Data::OleDb::OleDbCommand(
					mstrStoredProcedureProtein2MassTags, mOleconnection);
				cmd->set_CommandTimeout(180); 

				// 2. set the command object so it knows
				//    to execute a stored procedure
				System::Data::CommandType cmdType = System::Data::CommandType::StoredProcedure; 
				cmd->CommandType = cmdType;

				// 3. add parameters to command, which
				//    will be passed to the stored procedure
				cmd->Parameters->Add(new System::Data::OleDb::OleDbParameter("@ConfirmedOnly", __box(mobjMassTagDBOptions->mbyteConfirmedTags)));
				cmd->Parameters->Add(new System::Data::OleDb::OleDbParameter("@MinimumHighNormalizedScore", __box(mobjMassTagDBOptions->mfltMinXCorr)));
				cmd->Parameters->Add(new System::Data::OleDb::OleDbParameter("@MinimumPMTQualityScore", __box(mobjMassTagDBOptions->mdecimalMinPMTScore)));
				cmd->Parameters->Add(new System::Data::OleDb::OleDbParameter("@MinimumHighDiscriminantScore", __box(mobjMassTagDBOptions->mdblMinDiscriminant)));
				cmd->Parameters->Add(new System::Data::OleDb::OleDbParameter("@MinimumPeptideProphetProbability", __box(mobjMassTagDBOptions->mdblPeptideProphetVal)));

				// execute the command
				menmStatus = DBConnectionStatus::Running; 
				rdr = cmd->ExecuteReader();

				menmStatus = DBConnectionStatus::Fetching; 
				int currentlyLoaded = 0;
				while (rdr->Read())
				{
					// Mass_Tag_ID 
					// Protein_ID  
					// Reference

					// iterate through results, printing each to console
					clsMassTag *mass_tag = new clsMassTag(); 
					if (rdr->Item[S"Mass_Tag_ID"] != System::DBNull::Value) // which it never should really be
					{
						int id = System::Convert::ToInt32(rdr->Item[S"Mass_Tag_ID"]); 
						System::String *refName = S"";
						int proteinId = 0; 
						//if (rdr->Item[S"Protein_ID"] != System::DBNull::Value) 
						if (rdr->Item[S"Ref_ID"] != System::DBNull::Value) 
						{
							//proteinId = System::Convert::ToInt32(rdr->Item[S"Protein_ID"]);
							proteinId = System::Convert::ToInt32(rdr->Item[S"Ref_ID"]);
						}
						if (rdr->Item[S"Reference"] != System::DBNull::Value) 
						{
							refName = dynamic_cast<System::String*>(rdr->Item[S"Reference"]);
						}

						if (currentlyLoaded == allocated)
						{
							currentlyLoaded = 0;
							mtDB->AddProteins(proteinIds, proteinNames,  mtIds); 
						}

						mtIds[currentlyLoaded] = id; 
						proteinIds[currentlyLoaded] = proteinId; 
						proteinNames[currentlyLoaded++] = refName; 
						mint_num_loaded++; 
					}
				}
				if (currentlyLoaded > 0)
				{
					System::String *proteinNamesTemp __gc[] = new System::String* __gc[currentlyLoaded]; 
					int proteinIdsTemp __gc[] = new int __gc[currentlyLoaded]; 
					int mtIdsTemp __gc[] = new int __gc[currentlyLoaded]; 

					for (int i = 0; i < currentlyLoaded; i++)
					{
						proteinNamesTemp[i] = proteinNames[i]; 
						proteinIdsTemp[i] = proteinIds[i]; 
						mtIdsTemp[i] = mtIds[i]; 
					}
					mtDB->AddProteins(proteinIdsTemp, proteinNamesTemp,  mtIdsTemp); 
				}
				mstrMessage = System::String::Concat("Loaded ", System::Convert::ToString(mint_num_loaded), S" proteins"); 
			}
			catch(System::Exception *ex)
			{
				System::Console::WriteLine(System::String::Concat(ex->get_Message(), ex->get_StackTrace())); 
			}

			menmStatus = DBConnectionStatus::Done; 
			if (mOleconnection != 0)
			{
				mOleconnection->Close();
			}
			if (rdr != 0)
			{
				rdr->Close();
			}
			menmStatus = DBConnectionStatus::Done; 
		}
		


		void clsMTDBLoader::LoadMassTags(clsMassTagDB *mtDB)
		{
			System::Data::SqlClient::SqlDataReader *rdr  = 0;
			int allocated = 1000; 
			clsMassTag *arrMassTags __gc[] = new clsMassTag* __gc [allocated];

			mint_num_loaded = 0; 
			
			mint_num_mtids_total  = 0; 

			try
			{
				mconnection->Open();
				menmStatus = DBConnectionStatus::Connecting; 

				// 1.  create a command object identifying
				//     the stored procedure
				System::Data::SqlClient::SqlCommand *cmd  = new System::Data::SqlClient::SqlCommand(
					mstrStoredProcedureMassTagsPlusPeptideProphet, mconnection);
				cmd->set_CommandTimeout(180); 

				// 2. set the command object so it knows
				//    to execute a stored procedure
				System::Data::CommandType cmdType = System::Data::CommandType::StoredProcedure; 
				cmd->CommandType = cmdType;

				// 3. add parameters to command, which
				//    will be passed to the stored procedure
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@MassCorrectionIDFilterList", 0));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@ConfirmedOnly", __box(mobjMassTagDBOptions->mbyteConfirmedTags)));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@MinimumHighNormalizedScore", __box(mobjMassTagDBOptions->mfltMinXCorr)));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@MinimumPMTQualityScore", __box(mobjMassTagDBOptions->mdecimalMinPMTScore)));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@NETValueType",__box(mobjMassTagDBOptions->mbyteNETValType)));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@MinimumHighDiscriminantScore", __box(mobjMassTagDBOptions->mdblMinDiscriminant)));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@ExperimentFilter", 0));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@ExperimentExclusionFilter", 0));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@MinimumPeptideProphetProbability", __box(mobjMassTagDBOptions->mdblPeptideProphetVal)));
				

				mint_num_mtids_total = 1000;
				// execute the command
				menmStatus = DBConnectionStatus::Running; 
				rdr = cmd->ExecuteReader();

				menmStatus = DBConnectionStatus::Fetching; 
				int currentlyLoaded = 0;
				while (rdr->Read())
				{
					// Mass_Tag_ID	
					// Peptide	
					// Monoisotopic_Mass	
					// Net_Value_to_Use	
					// PNET	
					// High_Normalized_Score	
					// StD_GANET	
					// High_Discriminant_Score	
					// Peptide_Obs_Count_Passing_Filter	
					// Mod_Count	
					// Mod_Description	
					// High_Peptide_Prophet_Probability	
					// Mass_Tag_ID	
					// ObsCount_CS1	
					// ObsCount_CS2	
					// ObsCount_CS3	
					// PepProphet_FScore_Avg_CS1	
					// PepProphet_FScore_Avg_CS2	
					// PepProphet_FScore_Avg_CS3	
					// PepProphet_Probability_Max_CS1	
					// PepProphet_Probability_Max_CS2	
					// PepProphet_Probability_Max_CS3	
					// PepProphet_Probability_Avg_CS1	
					// PepProphet_Probability_Avg_CS2

					// iterate through results, printing each to console
					clsMassTag *mass_tag = new clsMassTag(); 
					if (rdr->Item[S"Mass_Tag_ID"] != System::DBNull::Value) // which it never should really be
					{
						int id = System::Convert::ToInt32(rdr->Item[S"Mass_Tag_ID"]); 
						System::String *peptide = S"";
						if (rdr->Item[S"Peptide"] != System::DBNull::Value) 
						{
							peptide = dynamic_cast<System::String*>(rdr->Item[S"Peptide"]);
						}
						float ganet = -1;
						if (rdr->Item[S"Net_Value_to_Use"] != System::DBNull::Value) 
						{
							ganet = System::Convert::ToSingle(rdr->Item[S"Net_Value_to_Use"]);
						}
						
						//float pnet = (float) rdr->Item["PNET"];
						float xcorr_max = 0;
						if (rdr->Item[S"High_Normalized_Score"] != System::DBNull::Value)
						{
							xcorr_max = System::Convert::ToSingle(rdr->Item[S"High_Normalized_Score"]);
						}

						float std_net = 0; 
						if (rdr->Item[S"StD_GANET"] != System::DBNull::Value)
						{
							//std_net = *dynamic_cast<float*>(rdr->Item[S"StD_GANET"]); 
							std_net = System::Convert::ToSingle(rdr->Item[S"StD_GANET"]); 
						}

						double mono_mass = 0.0; 
						if (rdr->Item[S"Monoisotopic_Mass"] != System::DBNull::Value)
						{
							mono_mass = System::Convert::ToDouble(rdr->Item[S"Monoisotopic_Mass"]); 
						}
						float high_discriminant = 0;
						if (rdr->Item[S"High_Discriminant_Score"] != System::DBNull::Value)
						{
							high_discriminant = System::Convert::ToSingle(rdr->Item[S"High_Discriminant_Score"]);
						}

						int num_obs = 0;
						if (rdr->Item[S"Peptide_Obs_Count_Passing_Filter"] != System::DBNull::Value)
						{
							num_obs = System::Convert::ToInt32(rdr->Item[S"Peptide_Obs_Count_Passing_Filter"]);
						}

						int mod_count = 0; 
						if (rdr->Item[S"Mod_Count"] != System::DBNull::Value)
						{
							mod_count = System::Convert::ToInt32(rdr->Item[S"Mod_Count"]);
						}

						System::String* modification_str = S""; 
						if (rdr->Item[S"Mod_Description"] != System::DBNull::Value)
						{
							modification_str = dynamic_cast<System::String*>(rdr->Item[S"Mod_Description"]);
						}

						float highPeptideProphetProbability = 0; 
						if (rdr->Item[S"High_Peptide_Prophet_Probability"] != System::DBNull::Value)
						{
							highPeptideProphetProbability = System::Convert::ToSingle(rdr->Item[S"High_Peptide_Prophet_Probability"]);
						}

						// Mass_Tag_ID	
						// ObsCount_CS1	
						// ObsCount_CS2	
						// ObsCount_CS3	
						// PepProphet_FScore_Avg_CS1	
						// PepProphet_FScore_Avg_CS2	
						// PepProphet_FScore_Avg_CS3	
						float favg_cs1 = -100;
						if (rdr->Item[S"PepProphet_FScore_Avg_CS1"] != System::DBNull::Value) 
						{
							favg_cs1 = System::Convert::ToSingle(rdr->Item[S"PepProphet_FScore_Avg_CS1"]);
						}
						float favg_cs2 = -100;
						if (rdr->Item[S"PepProphet_FScore_Avg_CS2"] != System::DBNull::Value) 
						{
							favg_cs2 = System::Convert::ToSingle(rdr->Item[S"PepProphet_FScore_Avg_CS2"]);
						}
						float favg_cs3 = -100;
						if (rdr->Item[S"PepProphet_FScore_Avg_CS3"] != System::DBNull::Value) 
						{
							favg_cs3 = System::Convert::ToSingle(rdr->Item[S"PepProphet_FScore_Avg_CS3"]);
						}
						short cleaveage_state = 2; 
						if (rdr->Item[S"Cleavage_State"] != System::DBNull::Value) 
						{
							cleaveage_state = System::Convert::ToInt16(rdr->Item[S"Cleavage_State"]);
						}

						mass_tag->mintMassTagId = id; 
						mass_tag->mstrPeptide = peptide;
						mass_tag->mdblAvgGANET = ganet; 
						mass_tag->mdblHighXCorr = xcorr_max; 
						mass_tag->mdblMaxDiscriminant = high_discriminant;
						mass_tag->mdblMonoMass = mono_mass; 
						mass_tag->mstrModification = modification_str; 
						mass_tag->mshortModCount = mod_count; 

						mass_tag->mdblStdGANET = std_net;
						mass_tag->mintNumObsPassingFilter = num_obs; 
						mass_tag->mfltAvgFCS1 = favg_cs1; 
						mass_tag->mfltAvgFCS2 = favg_cs2; 
						mass_tag->mfltAvgFCS3 = favg_cs3; 
						mass_tag->mshortCleavageState = cleaveage_state; 
						mass_tag->HighPeptideProphetProbability = highPeptideProphetProbability;

						if (mass_tag->mdblAvgGANET != -1 && num_obs >= mobjMassTagDBOptions->mintMinObservationCountFilter)
						{
							if (currentlyLoaded == allocated)
							{
								currentlyLoaded = 0;
								mtDB->AddMassTags(arrMassTags); 
							}
							arrMassTags[currentlyLoaded++] = mass_tag; 
							mint_num_loaded++; 
						}
					}
				}
				if (currentlyLoaded > 0)
				{
					clsMassTag* tempArr __gc[] = new clsMassTag* __gc[currentlyLoaded];
					for (int i = 0; i < currentlyLoaded; i++)
					{
						tempArr[i] = arrMassTags[i]; 
					}
					mtDB->AddMassTags(tempArr); 
				}
				mstrMessage = System::String::Concat("Loaded ", System::Convert::ToString(mint_num_loaded), S" mass tags"); 
			}
			catch(System::Exception *ex)
			{
				throw (ex); 
			}

			if (mconnection != 0)
			{
				mconnection->Close();
			}
			if (rdr != 0)
			{
				rdr->Close();
			}
			menmStatus = DBConnectionStatus::Done; 
		}
		void clsMTDBLoader::LoadMassTagsToProteinMap(clsMassTagDB *mtDB)
		{
			/* Sample output from GetMassTagToProteinNameMap
				Mass_Tag_ID Protein_ID  Reference
				36715995    107         STM0005
				127178008   107         STM0005
				133235815   107         STM0005
				74040034    108         STM0006
				36601727    109         STM0007
				36652078    109         STM0007
				36660826    109         STM0007
				36660867    109         STM0007
				36620881    109         STM0007
			*/
			int allocated = 1000; 
			System::String *proteinNames __gc[] = new System::String* __gc[allocated]; 
			int proteinIds __gc[] = new int __gc[allocated]; 
			int mtIds __gc[] = new int __gc[allocated]; 

			System::Data::SqlClient::SqlDataReader *rdr  = 0;
			mint_num_loaded = 0; 

			try
			{				
				mconnection->Open();
				menmStatus = DBConnectionStatus::Connecting; 

				// 1.  create a command object identifying
				//     the stored procedure
				System::Data::SqlClient::SqlCommand *cmd  = new System::Data::SqlClient::SqlCommand(
					mstrStoredProcedureProtein2MassTags, mconnection);
				cmd->set_CommandTimeout(180); 

				// 2. set the command object so it knows
				//    to execute a stored procedure
				System::Data::CommandType cmdType = System::Data::CommandType::StoredProcedure; 
				cmd->CommandType = cmdType;

				// 3. add parameters to command, which
				//    will be passed to the stored procedure
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@ConfirmedOnly", __box(mobjMassTagDBOptions->mbyteConfirmedTags)));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@MinimumHighNormalizedScore", __box(mobjMassTagDBOptions->mfltMinXCorr)));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@MinimumPMTQualityScore", __box(mobjMassTagDBOptions->mdecimalMinPMTScore)));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@MinimumHighDiscriminantScore", __box(mobjMassTagDBOptions->mdblMinDiscriminant)));
				cmd->Parameters->Add(new System::Data::SqlClient::SqlParameter("@MinimumPeptideProphetProbability", __box(mobjMassTagDBOptions->mdblPeptideProphetVal)));

				// execute the command
				menmStatus = DBConnectionStatus::Running; 
				rdr = cmd->ExecuteReader();

				menmStatus = DBConnectionStatus::Fetching; 
				int currentlyLoaded = 0;
				while (rdr->Read())
				{
					// Mass_Tag_ID 
					// Ref_ID  
					// Reference

					// iterate through results, printing each to console
					clsMassTag *mass_tag = new clsMassTag(); 
					if (rdr->Item[S"Mass_Tag_ID"] != System::DBNull::Value) // which it never should really be
					{
						int id = System::Convert::ToInt32(rdr->Item[S"Mass_Tag_ID"]); 
						System::String *refName = S"";
						int proteinId = 0; 
						if (rdr->Item[S"Ref_ID"] != System::DBNull::Value) 
						{
							proteinId = System::Convert::ToInt32(rdr->Item[S"Ref_ID"]);
						}

						if (rdr->Item[S"Reference"] != System::DBNull::Value) 
						{
							refName = dynamic_cast<System::String*>(rdr->Item[S"Reference"]);
						}

						if (currentlyLoaded == allocated)
						{
							currentlyLoaded = 0;
							mtDB->AddProteins(proteinIds, proteinNames,  mtIds); 
						}

						mtIds[currentlyLoaded] = id; 
						proteinIds[currentlyLoaded] = proteinId; 
						proteinNames[currentlyLoaded++] = refName; 
						mint_num_loaded++; 
					}
				}
				if (currentlyLoaded > 0)
				{
					System::String *proteinNamesTemp __gc[] = new System::String* __gc[currentlyLoaded]; 
					int proteinIdsTemp __gc[] = new int __gc[currentlyLoaded]; 
					int mtIdsTemp __gc[] = new int __gc[currentlyLoaded]; 

					for (int i = 0; i < currentlyLoaded; i++)
					{
						proteinNamesTemp[i] = proteinNames[i]; 
						proteinIdsTemp[i] = proteinIds[i]; 
						mtIdsTemp[i] = mtIds[i]; 
					}
					mtDB->AddProteins(proteinIdsTemp, proteinNamesTemp,  mtIdsTemp); 
				}
				mstrMessage = System::String::Concat("Loaded ", System::Convert::ToString(mint_num_loaded), S" proteins"); 
			}
			catch(System::Exception *ex)
			{
				System::Console::WriteLine(System::String::Concat(ex->get_Message(), ex->get_StackTrace())); 
			}

			menmStatus = DBConnectionStatus::Done; 
			if (mconnection != 0)
			{
				mconnection->Close();
			}
			if (rdr != 0)
			{
				rdr->Close();
			}
			menmStatus = DBConnectionStatus::Done; 
		}
		
		clsMassTagDB* clsMTDBLoader::LoadMassTagDatabase()
		{
			clsMassTagDB *mtDB = new clsMassTagDB(); 
			
			if (mobjMassTagDBOptions->menm_databaseType == MassTagDatabaseType::SQL)
			{
				mstrMessage = System::String::Concat(S"Loading Mass Tag database: ", mobjMassTagDBOptions->mstrDatabase); 
				LoadMassTags(mtDB); 
				LoadMassTagsToProteinMap(mtDB); 			
			}else
			{
				mstrMessage = System::String::Concat(S"Loading Mass Tag database: ", mobjMassTagDBOptions->mstr_databaseFilePath);
				LoadMassTagsAccess(mtDB); 
				LoadMassTagsToProteinMapAccess(mtDB); 			
			}
			return mtDB; 
		}
	}
}
