using System;
using System.Collections.Generic;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Data;
using System.Data.SQLite;

namespace MultiAlignCore.IO.Features.Hibernate
{
    public class MassTagMatchDAO : GenericDAOHibernate<ClusterToMassTagMap>,  IMassTagMatchDAO
    {
        public void ClearAllMatches()
        {

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + NHibernateUtil.Path))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "DELETE FROM T_Cluster_To_Mass_Tag_Map";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}
