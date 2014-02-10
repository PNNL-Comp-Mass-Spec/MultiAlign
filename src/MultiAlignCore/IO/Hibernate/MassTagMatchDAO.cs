using System.Collections.Generic;
using System.Data.SQLite;
using MultiAlignCore.Data;
using NHibernate.Criterion;

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

        public List<ClusterToMassTagMap> FindByClusterId(int id)
        {
            ICriterion criterion = Expression.Eq("ClusterId", id);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            
            return FindByCriteria(criterionList);
        
        }
    }
}
