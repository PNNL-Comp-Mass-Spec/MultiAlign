using System.Data;
using System.Data.SQLite;

namespace PNNLProteomics.IO.MTDB
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


        #region Methods
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
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string CreateConnectionString()
        {
            return string.Format("DataSource={0}", DatabasePath);
        }
        #endregion
    }
}
