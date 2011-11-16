using System.Data;
using System.Data.SQLite;
using MultiAlignCore.Data.MassTags;

namespace MultiAlignCore.IO.MTDB
{
    /// <summary>
    /// Stub class.
    /// </summary>
    public class SQLiteMassTagDatabaseLoader : MassTagDatabaseLoader
    {
        /// <summary>
        /// Sqlite database loader constructor.
        /// </summary>
        public SQLiteMassTagDatabaseLoader(string databasePath)
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
            string commandString    = "SELECT * FROM T_Mass_Tag_To_Protein_Name_Map ";
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
            string commandString    = "SELECT * FROM T_Mass_Tags_plus_Conformers ";
            commandString           += string.Format(" WHERE (High_Normalized_Score >= {0})", Options.mfltMinXCorr);
            commandString           += string.Format(" AND (High_Discriminant_Score >= {0})", Options.mdblMinDiscriminant);
            commandString           += string.Format(" AND (High_Peptide_Prophet_Probability >= {0})", Options.mdblPeptideProphetVal);
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
    }
}