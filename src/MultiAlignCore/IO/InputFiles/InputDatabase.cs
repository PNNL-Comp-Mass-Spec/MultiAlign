using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.MTDB;

namespace MultiAlignCore.IO.InputFiles
{
    /// <summary>
    /// Encapsulates the type of database to load.
    /// </summary>
    public class InputDatabase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InputDatabase()
        {
            DatabaseServer      = null;
            DatabaseName        = null;
            LocalPath           = null;
            DatabaseFormat      = MassTagDatabaseType.None;
        }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        public string DatabaseName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        public string DatabaseServer
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the mass tag database path.
        /// </summary>
        public string LocalPath
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the database format.
        /// </summary>
        public MassTagDatabaseType DatabaseFormat
        {
            get;
            set;
        }
        
        #region Methods
        /// <summary>
        /// Determines if the right combination of input settings are correct for specifying a MTDB.
        /// </summary>
        /// <returns>True if a database is to be used, false if not.</returns>
        public bool ValidateDatabaseType()
        {
            // By default we use a MTS enabled MTDB.
            bool useMTDB    = true;


            if (DatabaseFormat == MassTagDatabaseType.SQL)
            {
                if (DatabaseName == null && DatabaseServer != null)
                {
                    throw new AnalysisMTDBSetupException("The server was specified but the database was not.");
                }

                if (DatabaseServer == null && DatabaseName != null)
                {
                    throw new AnalysisMTDBSetupException("The database name was specified but the server was not.");
                }
            }
            else
            {
                if (DatabaseName == null && DatabaseServer == null)
                {
                    if (LocalPath == null)
                    {
                        // No, we do not have a MTDB to use.
                        DatabaseFormat = MassTagDatabaseType.None;
                        useMTDB = false;
                    }
                }                
            }
            if (DatabaseName != null && DatabaseServer != null && LocalPath != null)
            {
                throw new AnalysisMTDBSetupException("A database server and name were provided in addition to a local database path.  You can only specify one database.");
            }                
            return useMTDB;
        }
        #endregion
    }
}
