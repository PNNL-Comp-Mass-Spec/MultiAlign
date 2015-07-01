#region

using System.Data.SQLite;
using PNNLOmics.Algorithms.FeatureMatcher.Data;

#endregion

namespace MultiAlignCore.IO.Hibernate
{
    public class STACDAOHibernate : GenericDAOHibernate<STACFDR>
    {
        public void ClearAll()
        {
            var path = NHibernateUtil.Connection;
            using (var connection = new SQLiteConnection("Data Source = " + path + " ;", true))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM T_STAC_FDR";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}