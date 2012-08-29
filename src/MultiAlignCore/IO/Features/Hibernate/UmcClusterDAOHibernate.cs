using System.Collections.Generic;
using MultiAlignEngine.Features;
using NHibernate.Criterion;
using System.Data.SQLite;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.IO.Features.Hibernate
{

    public class UmcClusterDAOHibernate : GenericDAOHibernate<UMCClusterLight>, IUmcClusterDAO
    {
		
        /// <summary>
        /// Searches for and returns a List of UmcCluster Objects in the Database that have the exact Mass given.
        /// </summary>
        /// <param name="mass">Mass value to be searched for</param>
        /// <returns>List of UmcCluster Objects</returns>
        public ICollection<UMCClusterLight> FindByMass(double mass)
        {
            ICriterion criterion = Expression.Eq("Mass", mass);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        public void ClearAllClusters()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + NHibernateUtil.Path))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "DELETE FROM T_Clusters";
                    command.ExecuteNonQuery();
                }
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "UPDATE T_LCMS_Features  SET Cluster_ID = -1";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }

}
