#region

using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using MultiAlignCore.Data;
using NHibernate.Criterion;

#endregion

namespace MultiAlignCore.IO.Features.Hibernate
{
    public class MassTagMatchDAO : GenericDAOHibernate<ClusterToMassTagMap>, IMassTagMatchDAO
    {
        public void ClearAllMatches()
        {
            using (var connection = new SQLiteConnection("Data Source=" + NHibernateUtil.Path, true))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "DELETE FROM T_Cluster_To_Mass_Tag_Map";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public List<ClusterToMassTagMap> FindByClusterId(int id)
        {
            ICriterion criterion = Restrictions.Eq("ClusterId", id);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);

            return FindByCriteria(criterionList);
        }
    }
}