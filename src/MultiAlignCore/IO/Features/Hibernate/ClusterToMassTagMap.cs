using MultiAlignCore.Data.Alignment;
using System.Data.SQLite;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Data;
using PNNLOmics.Algorithms.FeatureMatcher.Data;

namespace MultiAlignCore.IO.Features.Hibernate
{

    public class ClusterToMassTagMapDAOHibernate : GenericDAOHibernate<ClusterToMassTagMap>
    {
        public void ClearAll()
        {
            string path = NHibernateUtil.Connection;
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + path + " ;"))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM T_Cluster_To_Mass_Tag_Map";
                    command.ExecuteNonQuery();
                }
            }
        }
    }

}
