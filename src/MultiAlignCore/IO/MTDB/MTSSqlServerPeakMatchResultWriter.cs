using System.Data;
using System.Data.SqlClient;

namespace MultiAlignCore.IO.MTDB
{
    public class MTSSqlServerPeakMatchResultWriter: MTSPeakMatchResultsWriter
    {
        /// <summary>
        /// Creates a new database connection.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override IDbConnection CreateConnection(string path)
        {
            string connectionString = path;
            return new SqlConnection(connectionString);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override IDbDataParameter CreateParameter(string name,
                                                            object value)
        {
            return new SqlParameter(name, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        protected override IDbDataParameter CreateOutputParameter(  string name, 
                                                                    DbType type,
                                                                    int size,
                                                                    byte precision,
                                                                    byte scale)
        {            
            IDbDataParameter param = new SqlParameter(  name,
                                                        (SqlDbType) type,
                                                        size,
                                                        ParameterDirection.Output,
                                                        false,
                                                        precision,
                                                        scale,
                                                        name,
                                                        DataRowVersion.Current,
                                                        null);
            return param;
        }
    }
}
