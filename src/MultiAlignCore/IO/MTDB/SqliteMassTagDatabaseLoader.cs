using System.Data;
using System.Data.SQLite;

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
        public SQLiteMassTagDatabaseLoader()
        {
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
            return string.Format("DataSource={0}", DatabasePath);
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
