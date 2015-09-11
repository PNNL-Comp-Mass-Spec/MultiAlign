#region

using System;
using System.Collections.Generic;
using System.Data;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.IO.MTDB
{
    /// <summary>
    ///     Mass tag database loader class.
    /// </summary>
    public abstract class MassTagDatabaseLoader : IMtdbLoader
    {
        #region Constants

        /// <summary>
        ///     Stored Procedure for getting mass tags and pep prophet stats.
        /// </summary>
        private const string MASS_TAGS_PLUS_PEP = "GetMassTagsPlusConformers";

        /// <summary>
        ///     Stored Procedure for getting mass tags and pep prophet stats.
        /// </summary>
        private const string PROTEINS = "GetMassTagToProteinNameMap";

        /// <summary>
        ///     Default timeout for a command.
        /// </summary>
        private const int COMMAND_TIMEOUT = 180;

        #endregion

        #region  Members

        /// <summary>
        ///     Stored procedure for getting mass tags
        /// </summary>
        protected string m_massTagsPlusPeptideProphetStats_sp;

        /// <summary>
        ///     Name of the protein stored procedure for downloading data from the database.
        /// </summary>
        protected string m_protein2MassTags_sp;

        #endregion

        #region Constructor

        public MassTagDatabaseLoader()
        {
            m_massTagsPlusPeptideProphetStats_sp = MASS_TAGS_PLUS_PEP;
            m_protein2MassTags_sp = PROTEINS;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the default mass tag database options.
        /// </summary>
        public MassTagDatabaseOptions Options { get; protected set; }

        #endregion

        #region Abstract Methods

        /// <summary>
        ///     Creates a database connection.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected abstract IDbConnection CreateConnection(string connectionString);

        /// <summary>
        ///     Creates the connection string for the mass tag database loader.
        /// </summary>
        /// <returns></returns>
        protected abstract string CreateConnectionString();

        /// <summary>
        ///     Creates a new parameter for use in the stored procedure.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract IDbDataParameter CreateParameter(string name, object value);

        #endregion

        #region Loading Methods

        /// <summary>
        ///     Sets up the mass tag command.
        /// </summary>
        /// <param name="command"></param>
        protected virtual void SetupMassTagCommand(IDbCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 180;
            command.CommandText = m_massTagsPlusPeptideProphetStats_sp;

            command.Parameters.Add(CreateParameter("@MinimumHighNormalizedScore", Options.MinimumXCorr));
            command.Parameters.Add(CreateParameter("@MinimumPMTQualityScore", Options.MinimumPmtScore));
            command.Parameters.Add(CreateParameter("@MinimumHighDiscriminantScore", Options.MinimumDiscriminant));
            command.Parameters.Add(CreateParameter("@MinimumPeptideProphetProbability", Options.MinimumPeptideProphetScore));
            //command.Parameters.Add(CreateParameter("@MinimumNet", Options.MinimumNet));
            //command.Parameters.Add(CreateParameter("@MaximumNet", Options.MaximumNet));
            //command.Parameters.Add(CreateParameter("@MinimumMass", Options.MinimumMass));
            //command.Parameters.Add(CreateParameter("@MaximumMass", Options.MaximumMass));

            // Optional filters that are not accessible from the GUI
            if (!string.IsNullOrWhiteSpace(Options.ExperimentFilter))
                command.Parameters.Add(CreateParameter("@ExperimentFilter", Options.ExperimentFilter));

            if (!string.IsNullOrWhiteSpace(Options.ExperimentExclusionFilter))
                command.Parameters.Add(CreateParameter("@ExperimentExclusionFilter", Options.ExperimentExclusionFilter));

        }

        /// <summary>
        ///     Sets up the protein mass tag command for execution.
        /// </summary>
        protected virtual void SetupProteinMassTagCommand(IDbCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 180;
            command.CommandText = m_protein2MassTags_sp;

            command.Parameters.Add(CreateParameter("@MinimumHighNormalizedScore", Options.MinimumXCorr));
            command.Parameters.Add(CreateParameter("@MinimumPMTQualityScore", Options.MinimumPmtScore));
            command.Parameters.Add(CreateParameter("@MinimumHighDiscriminantScore", Options.MinimumDiscriminant));
            command.Parameters.Add(CreateParameter("@MinimumPeptideProphetProbability", Options.MinimumPeptideProphetScore));
        }

        /// <summary>
        ///     Downloads the mass tags
        /// </summary>
        /// <returns></returns>
        protected virtual List<MassTagLight> LoadMassTags()
        {
            var massTags = new List<MassTagLight>();
            using (var connection = CreateConnection(CreateConnectionString()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    SetupMassTagCommand(command);

                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var massTag = new MassTagLight();
                                if (reader["Mass_Tag_ID"] != DBNull.Value)
                                {
                                    var id = Convert.ToInt32(reader["Mass_Tag_ID"]);
                                    var peptide = string.Empty;
                                    float ganet = -1;
                                    var netDefined = false;
                                    float xcorr_max = 0;
                                    float stdNet = 0;
                                    var monoMass = 0.0;
                                    float highDiscriminant = 0;
                                    var numObservations = 0;
                                    var modCount = 0;
                                    var modDescription = string.Empty;
                                    short cleaveageState = 2;
                                    float driftTime = 0;
                                    var charge = 0;
                                    var conformerID = 0;
                                    float highPeptideProphetProbability = 0;
                                    double msgf = 0;


                                    if (reader["Peptide"] != DBNull.Value) 
                                        peptide = reader["Peptide"].ToString();
                                    
                                    if (reader["Net_Value_to_Use"] != DBNull.Value)
                                    {
                                        ganet = Convert.ToSingle(reader["Net_Value_to_Use"]);
                                        netDefined = true;
                                    }

                                    if (reader["High_Normalized_Score"] != DBNull.Value)
                                        xcorr_max = Convert.ToSingle(reader["High_Normalized_Score"]);

                                    if (netDefined && reader["StD_GANET"] != DBNull.Value)
                                        stdNet = Convert.ToSingle(reader["StD_GANET"]);

                                    if (reader["Monoisotopic_Mass"] != DBNull.Value)
                                        monoMass = Convert.ToDouble(reader["Monoisotopic_Mass"]);
                                    
                                    if (reader["Min_MSGF_SpecProb"] != DBNull.Value)
                                        msgf = Convert.ToDouble(reader["Min_MSGF_SpecProb"]);
                                    if (reader["Peptide_Obs_Count_Passing_Filter"] != DBNull.Value)
                                        numObservations = Convert.ToInt32(reader["Peptide_Obs_Count_Passing_Filter"]);

                                    if (reader["Mod_Count"] != DBNull.Value)
                                        modCount = Convert.ToInt32(reader["Mod_Count"]);
                                    if (reader["Mod_Description"] != DBNull.Value)
                                        modDescription = reader["Mod_Description"].ToString();

                                    if (reader["High_Peptide_Prophet_Probability"] != DBNull.Value)
                                        highPeptideProphetProbability = Convert.ToSingle(reader["High_Peptide_Prophet_Probability"]);

                                    if (reader["Cleavage_State"] != DBNull.Value)

                                        cleaveageState = Convert.ToInt16(reader["Cleavage_State"]);
                                    if (reader["Drift_Time_Avg"] != DBNull.Value)
                                        driftTime = Convert.ToSingle(reader["Drift_Time_Avg"]);
                                    if (reader["Conformer_Charge"] != DBNull.Value)
                                        charge = Convert.ToInt32(reader["Conformer_Charge"]);
                                    if (reader["Conformer_ID"] != DBNull.Value)
                                        conformerID = Convert.ToInt32(reader["Conformer_ID"]);

                                    // Make sure the mass tag has been seen enough times
                                    if (numObservations >= Options.MinimumObservationCountFilter)
                                    {
                                        var molecule = new Molecule
                                        {
                                            Name = peptide
                                        };

                                        massTag.Id = id;
                                        massTag.Molecule = molecule;
                                        massTag.Net = ganet;
                                        massTag.NetAverage = ganet;
                                        massTag.NetStandardDeviation = stdNet;
                                        massTag.XCorr = xcorr_max;
                                        massTag.DiscriminantMax = highDiscriminant;
                                        massTag.MassMonoisotopic = monoMass;
                                        massTag.ConformationId = conformerID;
                                        
                                        massTag.ObservationCount = numObservations;
                                        massTag.DriftTime = driftTime;
                                        massTag.PriorProbability = highPeptideProphetProbability;
                                        massTag.CleavageState = cleaveageState;
                                        massTag.ModificationCount = modCount;
                                        massTag.Modifications = modDescription;
                                        massTag.MsgfSpecProbMax = msgf;
                                        massTag.PeptideSequence = peptide;
                                        massTag.ChargeState = charge;

                                        if (!netDefined)
                                        {
                                            continue;
                                        }

                                        var shouldAdd = false;

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

        /// <summary>
        ///     Loads the protein information.
        /// </summary>
        protected virtual Dictionary<int, List<Protein>> LoadProteins()
        {
            var massTagToProtein = new Dictionary<int, List<Protein>>();
            using (var connection = CreateConnection(CreateConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    SetupProteinMassTagCommand(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["Mass_Tag_ID"] != DBNull.Value)
                            {
                                var id = Convert.ToInt32(reader["Mass_Tag_ID"]);
                                var refName = "";
                                var proteinId = -1;
                                var refID = -1;

                                if (reader["Ref_ID"] != DBNull.Value) refID = Convert.ToInt32(reader["Ref_ID"]);
                                if (reader["Reference"] != DBNull.Value) refName = reader["Reference"].ToString();
                                if (reader["Protein_ID"] != DBNull.Value)
                                    proteinId = Convert.ToInt32(reader["Protein_ID"]);

                                var protein = new Protein();
                                protein.Sequence = refName;
                                protein.RefId = refID;
                                protein.ProteinId = proteinId;

                                // Link to the Mass tags.
                                var hasMassTagID = massTagToProtein.ContainsKey(id);
                                if (!hasMassTagID)
                                {
                                    massTagToProtein.Add(id, new List<Protein>());
                                }
                                massTagToProtein[id].Add(protein);
                                ;
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
        ///     Loads the mass tag database from the underlying database system.
        /// </summary>
        /// <returns></returns>
        public virtual MassTagDatabase LoadDatabase()
        {
            var database = new MassTagDatabase();
            var tags = LoadMassTags();
            var proteinMap = LoadProteins();

            database.AddMassTagsAndProteins(tags, proteinMap);

            return database;
        }

        #endregion
    }
}