using System.Data;
using System.Data.SQLite;
using MultiAlignCore.Data.MassTags;
using PNNLOmics.Data.MassTags;
using System.Collections.Generic;
using PNNLOmics.Data;
using System;

namespace MultiAlignCore.IO.MTDB
{
    /// <summary>
    /// This class is because the database schema of the APE tool dont match that from the MTDBCreator.  So
    /// I made this class because I was tired of manually changing the @#$%#$%^ database schema because it can never be consistent.
    /// </summary>
    public class ApeMassTagDatabaseLoader : MassTagDatabaseLoader
    {
        /// <summary>
        /// Sqlite database loader constructor.
        /// </summary>
        public ApeMassTagDatabaseLoader(string databasePath)
        {
            DatabasePath = databasePath; 
        }

        /// <summary>
        /// Gets or sets the database path.
        /// </summary>
        public string DatabasePath
        {
            get;
            set;
        }

        #region Abstract Method Implementations.
        /// <summary>
        /// Sets up the command for execution.
        /// </summary>
        /// <param name="command"></param>
        protected override void SetupProteinMassTagCommand(IDbCommand command)
        {
            command.CommandText     = "";
            command.CommandTimeout  = 180;
            command.CommandType     = CommandType.Text;
            string commandString    = "SELECT * FROM T_Mass_Tag_To_Protein_Map ";
            commandString += " INNER JOIN T_Proteins ON T_Mass_Tag_to_Protein_Map.Ref_ID = T_Proteins.Ref_ID";
            command.CommandText     = commandString;
        }
        /// <summary>
        /// Sets up the mass tag command.
        /// </summary>
        /// <param name="command"></param>
        protected override void SetupMassTagCommand(IDbCommand command)
        {
            command.CommandText     = "";
            command.CommandTimeout  = 180;
            command.CommandType     = CommandType.Text;
            string commandString    = "SELECT * FROM T_Mass_Tags ";
            commandString           += string.Format(" WHERE (High_Normalized_Score >= {0})", Options.MinimumXCorr);
            commandString           += string.Format(" AND (High_Discriminant_Score >= {0})", Options.MinimumDiscriminant);
            commandString           += string.Format(" AND (High_Peptide_Prophet_Probability >= {0})", Options.PeptideProphetVal);
            command.CommandText     = commandString;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }
        /// <summary>
        /// Creates a new connection string.
        /// </summary>
        /// <returns>String to use for connecting to local databases.</returns>
        protected override string CreateConnectionString()
        {
            string newPath = DatabasePath;
            if (DatabasePath.Contains(" "))
            {
                newPath = "\"" + DatabasePath.Trim() + "\"";
            }
            return string.Format("Data Source={0}", newPath);
        }
        /// <summary>
        /// Creates a new Sqlite data paramter for use in queries.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="value">Value of parameter.</param>
        /// <returns>New parameter.</returns>
        protected override IDbDataParameter CreateParameter(string name, object value)
        {
            return new SQLiteParameter(name, value);
        }
        #endregion

        #region  This is a "Good thing I can design software" region where I saved my ass because the database formats were different.
        //private Dictionary<string, Protein> CreateProteinInfo(IDbConnection connection)
        //{
        //    Dictionary<string, Protein> map = new Dictionary<string, Protein>();

        //    using (IDbCommand command = connection.CreateCommand())
        //    {
        //            command.CommandText = "SELECT * FROM T_Proteins";
        //            using(IDataReader reader = command.ExecuteReader())
        //            {
        //                while(reader.Read())
        //                {					        
        //                    if (reader["Mass_Tag_ID"] != System.DBNull.Value) 
        //                    {
        //                        int     id          = Convert.ToInt32(reader["Mass_Tag_ID"]); 
        //                        string  refName     = "";                                
        //                        int     proteinId   = -1; 				
        //                        int     refID       = -1;

        //                        if (reader["Ref_ID"] != System.DBNull.Value)        refID       = Convert.ToInt32(reader["Ref_ID"]);
        //                        if (reader["Description"] != System.DBNull.Value) refName       = reader["Description"].ToString();
        //                        //if (reader["Protein"] != System.DBNull.Value) proteinId = Convert.ToInt32(reader["Protein"]);
                                
        //                        Protein protein     = new Protein();
        //                        protein.Sequence    = refName;
        //                        protein.RefID       = refID;
        //                        protein.ProteinID   = proteinId;
                                
        //                        // Link to the Mass tags.
        //                        bool hasMassTagID   = massTagToProtein.ContainsKey(id);
        //                        if (!hasMassTagID)
        //                        {
        //                            massTagToProtein.Add(id, new List<Protein>());
        //                        }
        //                        massTagToProtein[id].Add(protein); ;
        //                    }                                                        
        //                }
        //                reader.Close();
        //            }                    
        //        }
        //    }

        //    return map;
        //}
        
        protected override Dictionary<int, List<Protein>> LoadProteins()
        {
			Dictionary<int, List<Protein>> massTagToProtein = new Dictionary<int,List<Protein>>();									
            using (IDbConnection connection = CreateConnection(CreateConnectionString()))
            {
                connection.Open();                
                
                using (IDbCommand command = connection.CreateCommand())
                {
                    SetupProteinMassTagCommand(command);

				    using(IDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {					        
					        if (reader["Mass_Tag_ID"] != System.DBNull.Value) 
					        {
						        int     id          = Convert.ToInt32(reader["Mass_Tag_ID"]); 
						        string  refName     = "";                                
						        int     proteinId   = -1; 				
		                        int     refID       = -1;
                                string  description = "";

						        if (reader["Ref_ID"] != System.DBNull.Value)        refID           = Convert.ToInt32(reader["Ref_ID"]);
                                if (reader["Description"] != System.DBNull.Value) refName           = reader["Description"].ToString();
                                if (reader["Protein"] != System.DBNull.Value) description           = reader["Protein"].ToString();
                                if (reader["External_Protein_ID"] != System.DBNull.Value) proteinId = Convert.ToInt32(reader["External_Protein_ID"]);
                                
                                Protein protein     = new Protein();
                                protein.Sequence    = refName;
                                protein.RefID       = refID;
                                protein.ProteinID   = proteinId;
                                
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
        
        protected override List<MassTagLight> LoadMassTags()
        {         
            List<MassTagLight> massTags     = new List<MassTagLight>();
            using (IDbConnection connection = CreateConnection(CreateConnectionString()))
            {
                connection.Open();
                
                using (IDbCommand command = connection.CreateCommand())
                {
                    SetupMassTagCommand(command);

                    try
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MassTagLight massTag = new MassTagLight();
                                if (reader["Mass_Tag_ID"] != System.DBNull.Value)
                                {
                                    int id = System.Convert.ToInt32(reader["Mass_Tag_ID"]);
                                    string peptide = "";
                                    float ganet = -1;
                                    float xcorr_max = 0;
                                    float stdNet = 0;
                                    double monoMass = 0.0;
                                    float highDiscriminant = 0;
                                    int numObservations = 0;
                                    string modification = "";
                                    int modCount = 0;
                                    short cleaveageState = 2;
                                    float driftTime = 0;
                                    int charge = 0;
                                    int conformerID = 0;
                                    float highPeptideProphetProbability = 0;
                                    double msgf = 0;


                                    if (reader["Peptide"] != System.DBNull.Value) peptide = reader["Peptide"].ToString();
                                    if (reader["NET_Avg"] != System.DBNull.Value) ganet = Convert.ToSingle(reader["NET_Avg"]);
                                    if (reader["High_Normalized_Score"] != System.DBNull.Value) xcorr_max = Convert.ToSingle(reader["High_Normalized_Score"]);
                                    if (reader["NET_StDev"] != System.DBNull.Value) stdNet = Convert.ToSingle(reader["NET_StDev"]);
                                    if (reader["Monoisotopic_Mass"] != System.DBNull.Value) monoMass = Convert.ToDouble(reader["Monoisotopic_Mass"]);
                                    if (reader["Min_MSGF_SpecProb"] != System.DBNull.Value) msgf = Convert.ToDouble(reader["Min_MSGF_SpecProb"]);
                                    if (reader["Peptide_Obs_Count_Passing_Filter"] != System.DBNull.Value) numObservations = Convert.ToInt32(reader["Peptide_Obs_Count_Passing_Filter"]);
                                    if (reader["Mod_Count"] != System.DBNull.Value) modCount = Convert.ToInt32(reader["Mod_Count"]);
                                    if (reader["Mod_Description"] != System.DBNull.Value) modification = reader["Mod_Description"].ToString();
                                    if (reader["High_Peptide_Prophet_Probability"] != System.DBNull.Value) highPeptideProphetProbability = Convert.ToSingle(reader["High_Peptide_Prophet_Probability"]);
                                    if (reader["Cleavage_State_Max"] != System.DBNull.Value) cleaveageState = Convert.ToInt16(reader["Cleavage_State_Max"]);

                                    
                                    //TODO: Resupport drift time for this crap
                                    //    if (reader["Drift_Time_Avg"] != System.DBNull.Value) driftTime = Convert.ToSingle(reader["Drift_Time_Avg"]);
                                    //    if (reader["Charge"] != System.DBNull.Value) charge = Convert.ToInt32(reader["Charge"]);
                                    //    if (reader["Conformer_ID"] != System.DBNull.Value) conformerID = Convert.ToInt32(reader["Conformer_ID"]);
                                    

                                    /// Make sure the mass tag has been seen enough times
                                    if (numObservations >= Options.MinimumObservationCountFilter)
                                    {
                                        Molecule molecule           = new Molecule();
                                        molecule.Name               = peptide;
                                        massTag.ID                  = id;
                                        massTag.Molecule            = molecule;
                                        massTag.NET                 = ganet;
                                        massTag.NETAverage          = ganet;
                                        massTag.XCorr               = xcorr_max;
                                        massTag.DiscriminantMax     = highDiscriminant;
                                        massTag.MassMonoisotopic    = monoMass;
                                        massTag.ConformationID       = conformerID;
                                        massTag.NETStandardDeviation = stdNet;
                                        massTag.ObservationCount     = numObservations;
                                        massTag.DriftTime            = driftTime;
                                        massTag.PriorProbability     = highPeptideProphetProbability;
                                        massTag.CleavageState        = cleaveageState;
                                        massTag.ModificationCount    = modCount;
                                        massTag.MSGFSpecProbMax      = msgf;
                                        massTag.PeptideSequence      = peptide;
                                        massTag.ChargeState          = charge;

                                        if (massTag.NETAverage != -1)
                                        {
                                            bool shouldAdd = false;
                                            // If we are using drift time, then we should only 
                                            // use mass tags that have drift time.
                                            if (Options.OnlyLoadTagsWithDriftTime)
                                            {
                                                if (driftTime > 0)
                                                {
                                                    shouldAdd = true;
                                                }
                                            }
                                            else
                                            {
                                                shouldAdd = true;
                                            }

                                            if (shouldAdd)
                                            {
                                                massTags.Add(massTag);
                                            }
                                        }
                                    }
                                }
                            }
                            reader.Close();
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                connection.Close();
            }            
            return massTags;                    																				
        }
        #endregion
    }
}