#region

using System.Data.SQLite;
using MultiAlignCore.Data;

#endregion

namespace MultiAlignCore.IO.Hibernate
{
    public class ClusterToMassTagMapDAOHibernate : GenericDAOHibernate<ClusterToMassTagMap>
    {
        public void ClearAll()
        {
            var path = NHibernateUtil.Connection;
            using (var connection = new SQLiteConnection("Data Source = " + path + " ;", true))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM T_Cluster_To_Mass_Tag_Map";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}