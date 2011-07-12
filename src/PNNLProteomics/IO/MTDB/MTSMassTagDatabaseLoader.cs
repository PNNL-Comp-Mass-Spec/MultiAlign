using System.Data.SqlClient;

namespace PNNLProteomics.IO.MTDB
{
    /// <summary>
    /// Access the mass tag system for downloading mass tag database information.
    /// </summary>
    public class MTSMassTagDatabaseLoader: MassTagDatabaseLoader
    {
        /// <summary>
        /// Default password
        /// </summary>
        private const string DEFAULT_PASSWORD = "mt4fun";
        /// <summary>
        /// Default user name.
        /// </summary>
        private const string DEFAULT_USERNAME = "mtuser";

        public MTSMassTagDatabaseLoader()
        {
            UserName = DEFAULT_USERNAME;
            Password = DEFAULT_PASSWORD;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the server name.
        /// </summary>
        public string ServerName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the server password.
        /// </summary>
        public string Password
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Creates an ADO connection to the Mass Tag system.
        /// </summary>
        /// <param name="connectionString">Connection to the database.</param>
        /// <returns></returns>
        protected override System.Data.IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
        /// <summary>
        /// Creates a valid SQL connection string to the MTS system.
        /// </summary>
        /// <returns></returns>
        protected override string CreateConnectionString()
        {
            return string.Format("Server={0};Database={1};Integrated Security=no; User ID={2}; PWD={3}",
                                    ServerName,
                                    DatabaseName,
                                    UserName,
                                    Password);

        }
    }
}
