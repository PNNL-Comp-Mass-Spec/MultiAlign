using System.Collections.Generic;
using System.Data;
using PNNLOmics.Data;

namespace PNNLProteomics.IO.MTDB
{

    public abstract class MassTagDatabaseLoader : IMtdbLoader
    {
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
        /// Downloads the mass tags
        /// </summary>
        /// <returns></returns>
        public List<MassTag> GetMassTags()
        {
            List<MassTag> massTags = new List<MassTag>();

            using (IDbConnection connection = CreateConnection(CreateConnectionString()))
            {
                using (IDbCommand command = connection.CreateCommand())
                {

                }
            }

            return massTags;
        }
    }
}
