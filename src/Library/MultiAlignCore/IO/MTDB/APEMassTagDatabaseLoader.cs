using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using PNNLOmics.Data;
using PNNLOmics.Data.MassTags;

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
        public ApeMassTagDatabaseLoader(string databasePath, Algorithms.Options.MassTagDatabaseOptions options)
        {
            DatabasePath    = databasePath;
            Options         = options;
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
            var commandString    = "SELECT * FROM T_Mass_Tag_To_Protein_Map ";
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
            var commandString    = "SELECT * FROM T_Mass_Tags ";
            commandString           += string.Format(" WHERE (High_Normalized_Score >= {0})", Options.MinimumXCorr);
            commandString           += string.Format(" AND (High_Discriminant_Score >= {0})", Options.MinimumDiscriminant);
            commandString           += string.Format(" AND (High_Peptide_Prophet_Probability >= {0})", Options.MinimumPeptideProphetScore);
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
            var newPath = DatabasePath;
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
    }
}