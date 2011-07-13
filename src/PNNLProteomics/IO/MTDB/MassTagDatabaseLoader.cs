using System;
using System.Collections.Generic;
using System.Data;
using PNNLOmics.Data;
using MultiAlignEngine.MassTags;
using PNNLProteomics.Data.MassTags;

namespace PNNLProteomics.IO.MTDB
{
    /// <summary>
    /// Mass tag database loader class.
    /// </summary>
    public abstract class MassTagDatabaseLoader : IMtdbLoader
    {
        #region Constants
        /// <summary>
        /// Stored Procedure for getting mass tags and pep prophet stats.
        /// </summary>
        private const string MASS_TAGS_PLUS_PEP = "GetMassTagsPlusConformers";
        /// <summary>
        /// Stored Procedure for getting mass tags and pep prophet stats.
        /// </summary>
        private const string PROTEINS = "GetMassTagToProteinNameMap";
        /// <summary>
        /// Default timeout for a command.
        /// </summary>
        private const int COMMAND_TIMEOUT       = 180; 
        #endregion

        #region  Members
        /// <summary>
        /// Stored procedure for getting mass tags
        /// </summary>
        protected string m_massTagsPlusPeptideProphetStats_sp;        
        /// <summary>
        /// Name of the protein stored procedure for downloading data from the database.
        /// </summary>
        protected string m_protein2MassTags_sp;
        #endregion

        #region Constructor
        public MassTagDatabaseLoader()
        {
            m_massTagsPlusPeptideProphetStats_sp = MASS_TAGS_PLUS_PEP;
            m_protein2MassTags_sp                = PROTEINS;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the default mass tag database options.
        /// </summary>
        public clsMassTagDatabaseOptions Options
        {
            get;
            set;
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Creates a database connection.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected abstract IDbConnection CreateConnection(string connectionString);
        /// <summary>
        /// Creates the connection string for the mass tag database loader.
        /// </summary>
        /// <returns></returns>
        protected abstract string CreateConnectionString();
        /// <summary>
        /// Creates a new parameter for use in the stored procedure.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract IDbDataParameter CreateParameter(string name, object value);
        #endregion

        #region Loading Methods
        /// <summary>
        /// Downloads the mass tags
        /// </summary>
        /// <returns></returns>
        protected virtual List<MassTag> LoadMassTags()
        {
            List<MassTag> massTags = new List<MassTag>();

            using (IDbConnection connection = CreateConnection(CreateConnectionString()))
            {
                connection.Open();
                
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandType     = CommandType.StoredProcedure;
                    command.CommandTimeout  = 180;
                    command.CommandText     = m_massTagsPlusPeptideProphetStats_sp;    
						    				
				    command.Parameters.Add(CreateParameter("@ConfirmedOnly",                    Options.mbyteConfirmedTags));
				    command.Parameters.Add(CreateParameter("@MinimumHighNormalizedScore",       Options.mfltMinXCorr));
				    command.Parameters.Add(CreateParameter("@MinimumPMTQualityScore",           Options.mdecimalMinPMTScore));
				    command.Parameters.Add(CreateParameter("@MinimumHighDiscriminantScore",     Options.mdblMinDiscriminant));
				    command.Parameters.Add(CreateParameter("@MinimumPeptideProphetProbability", Options.mdblPeptideProphetVal));			
				
                    using(IDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
					        MassTag massTag = new MassTag();
					        if (reader["Mass_Tag_ID"] != System.DBNull.Value)
					        {
						        int     id                  = System.Convert.ToInt32(reader["Mass_Tag_ID"]); 
						        string  peptide             = "";
						        float   ganet               = -1;
						        float   xcorr_max           = 0;
						        float   stdNet              = 0; 
						        double  monoMass            = 0.0; 
						        float   highDiscriminant    = 0;
						        int     numObservations     = 0;                        
						        string  modification        = "";                         
						        int     modCount            = 0;  
						        short   cleaveageState      = 2; 
						        float   driftTime           = 0;
						        int     charge              = 0;
                                int     conformerID         = 0;
						        float   highPeptideProphetProbability = 0;
                                double  msgf                = 0;


						        if (reader["Peptide"] != System.DBNull.Value) 						        peptide                         = reader["Peptide"].ToString();						
						        if (reader["Net_Value_to_Use"] != System.DBNull.Value) 						ganet                           = Convert.ToSingle(reader["Net_Value_to_Use"]);												
						        if (reader["High_Normalized_Score"] != System.DBNull.Value)                 xcorr_max                       = Convert.ToSingle(reader["High_Normalized_Score"]);						
						        if (reader["StD_GANET"] != System.DBNull.Value) 							stdNet                          = Convert.ToSingle(reader["StD_GANET"]); 
						        if (reader["Monoisotopic_Mass"] != System.DBNull.Value)                     monoMass                        = Convert.ToDouble(reader["Monoisotopic_Mass"]);
						        if (reader["Min_MSGF_SpecProb"] != System.DBNull.Value)                     monoMass                        = Convert.ToDouble(reader["Min_MSGF_SpecProb"]); 						
						        if (reader["Peptide_Obs_Count_Passing_Filter"] != System.DBNull.Value)      numObservations                 = Convert.ToInt32(reader["Peptide_Obs_Count_Passing_Filter"]);						
						        if (reader["Mod_Count"] != System.DBNull.Value)         					modCount                        = Convert.ToInt32(reader["Mod_Count"]);						
						        if (reader["Mod_Description"] != System.DBNull.Value)                       modification                    = reader["Mod_Description"].ToString();						
						        if (reader["High_Peptide_Prophet_Probability"] != System.DBNull.Value)      highPeptideProphetProbability   = Convert.ToSingle(reader["High_Peptide_Prophet_Probability"]);
						        if (reader["Cleavage_State"] != System.DBNull.Value)                        cleaveageState                  = Convert.ToInt16(reader["Cleavage_State"]);
						        if (reader["Drift_Time_Avg"] != System.DBNull.Value)                        driftTime                       = Convert.ToSingle(reader["Drift_Time_Avg"]);														
						        if (reader["Conformer_Charge"] != System.DBNull.Value)                      charge                          = Convert.ToInt32(reader["Conformer_Charge"]);														
						        if (reader["Conformer_ID"] != System.DBNull.Value)                          conformerID                     = Convert.ToInt32(reader["Conformer_ID"]);
                                if (reader["Mod_Description"] != System.DBNull.Value)                       modification                    = reader["Mod_Description"].ToString();														
						
						        /// Make sure the mass tag has been seen enough times
						        if (numObservations >= Options.mintMinObservationCountFilter)
                                {
                                    Molecule molecule   = new Molecule();
                                    molecule.Name       = peptide;
                                    molecule.MassTag    = massTag; 
            
							        massTag.ID					    = id;                            
							        massTag.Molecule                = molecule;
							        massTag.NETAverage			    = ganet; 
							        massTag.XCorr				    = xcorr_max; 
							        massTag.DiscriminantMax		    = highDiscriminant;
							        massTag.MassMonoisotopic	    = monoMass; 
							        massTag.ConformationID  		= conformerID; 							
							        massTag.NETStandardDeviation    = stdNet;
							        massTag.ObservationCount    	= Convert.ToUInt16(numObservations); 
							        massTag.DriftTime			    = driftTime;										         
							        massTag.PriorProbability	    = highPeptideProphetProbability;
                                    massTag.CleavageState           = cleaveageState;
                                    massTag.ModificationCount       = modCount;
                                    massTag.MSGFSpecProbMax         = msgf;
                                    massTag.PeptideSequence         = peptide;
                                    
							        if (massTag.NETAverage != -1)
							        {
								        massTags.Add(massTag); 
							        }
						        }
					        }
				        }
                        reader.Close();
                    }                    
                }
                connection.Close();
            }            
            return massTags;                    																		
		}
        /// <summary>
        /// Loads the protein information.
        /// </summary>
        protected virtual Dictionary<int, List<Protein>> LoadProteins()
		{
			Dictionary<int, List<Protein>> massTagToProtein = new Dictionary<int,List<Protein>>();
									
            using (IDbConnection connection = CreateConnection(CreateConnectionString()))
            {
                connection.Open();
                
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandType     = CommandType.StoredProcedure;
                    command.CommandTimeout  = 180;
                    command.CommandText     = m_protein2MassTags_sp;    
						    				
				    command.Parameters.Add(CreateParameter("@ConfirmedOnly",                    Options.mbyteConfirmedTags));
				    command.Parameters.Add(CreateParameter("@MinimumHighNormalizedScore",       Options.mfltMinXCorr));
				    command.Parameters.Add(CreateParameter("@MinimumPMTQualityScore",           Options.mdecimalMinPMTScore));
				    command.Parameters.Add(CreateParameter("@MinimumHighDiscriminantScore",     Options.mdblMinDiscriminant));
				    command.Parameters.Add(CreateParameter("@MinimumPeptideProphetProbability", Options.mdblPeptideProphetVal));	
								
				    using(IDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {					        
					        if (reader["Mass_Tag_ID"] != System.DBNull.Value) 
					        {
						        int     id          = Convert.ToInt32(reader["Mass_Tag_ID"]); 
						        string  refName     = "";                                
						        long    proteinId   = -1; 				
		                        int     refID       = -1;

						        if (reader["Ref_ID"] != System.DBNull.Value)        refID       = Convert.ToInt32(reader["Ref_ID"]);
                                if (reader["Reference"] != System.DBNull.Value)     refName     = reader["Reference"].ToString();                                
                                if (reader["Protein_ID"] != System.DBNull.Value)    proteinId   = Convert.ToInt32(reader["Protein_ID"]);
                                
                                Protein protein     = new Protein();
                                protein.Sequence    = refName;
                                protein.RefID       = refID;
                                
                                // Link to the Mass tags.
                                bool hasMassTagID   = massTagToProtein.ContainsKey(id);
                                if (!hasMassTagID)
                                {
                                    massTagToProtein.Add(id, new List<Protein>());
                                }
                                massTagToProtein[id].Add(protein); ;
					        }                                                        
				        }
                        reader.Close();
				    }                    
                }
                connection.Close();
            }	
			return massTagToProtein;						
		}		
        #endregion

        #region IMtdbLoader Members
        /// <summary>
        /// Loads the mass tag database from the underlying database system.
        /// </summary>
        /// <returns></returns>
        public virtual MassTagDatabase LoadDatabase()
        {
            MassTagDatabase database                  = new MassTagDatabase();
            List<MassTag> tags                        = LoadMassTags();
            Dictionary<int, List<Protein>> proteinMap = LoadProteins();

            database.AddMassTagsAndProteins(tags, proteinMap);            

            return database;
        }
        #endregion
    }
}
