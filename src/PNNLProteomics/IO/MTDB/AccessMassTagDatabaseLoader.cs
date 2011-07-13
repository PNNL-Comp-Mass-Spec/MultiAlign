using System;
using System.Data;
using System.Data.OleDb;

namespace PNNLProteomics.IO.MTDB
{
    /// <summary>
    /// Access mass tag information from a Microsoft Access formatted MTDB.
    /// </summary>
    public class AccessMassTagDatabaseLoader: MassTagDatabaseLoader
    {
        /// <summary>
        /// Default connection provider as ACE OLEDB 12.0 Access provider.
        /// </summary>
        private const string DEFAULT_PROVIDER = "Microsoft.ACE.OLEDB.12.0";

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public AccessMassTagDatabaseLoader()
        {
            DatabasePath    = null;
            Provider        = DEFAULT_PROVIDER;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Path to database file.</param>
        /// <param name="provider">Type of provider to use for MS level access.</param>
        public AccessMassTagDatabaseLoader(string path, string provider)
        {
            DatabasePath    = path;
            Provider        = provider;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Path to database file.</param>
        public AccessMassTagDatabaseLoader(string path)
        {
            DatabasePath    = path;
            Provider        = DEFAULT_PROVIDER;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the path of the local access database.
        /// </summary>
        public string DatabasePath
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the data provider to use.
        /// </summary>
        public string Provider
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new ADO connection to a SQL server database.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new OleDbConnection(connectionString);
        }
        /// <summary>
        /// Creates a connection string for use.
        /// </summary>
        /// <returns></returns>
        protected override string CreateConnectionString()
        {
            if (DatabasePath == null)
                throw new NullReferenceException("The database path was not set.");

            return string.Format("Data Source={0}; Provider={1}",
                                    DatabasePath,
                                    Provider);
        }
        /// <summary>
        /// Creates a new Sqlite data paramter for use in queries.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="value">Value of parameter.</param>
        /// <returns>New parameter.</returns>
        protected override IDbDataParameter CreateParameter(string name, object value)
        {
            return new OleDbParameter(name, value);
        }
        #endregion
    }    
}
