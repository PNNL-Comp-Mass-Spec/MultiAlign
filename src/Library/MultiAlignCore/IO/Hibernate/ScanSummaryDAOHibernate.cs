using System.Collections.Generic;
using System.Data.SQLite;
using FeatureAlignment.Data;
using MultiAlignCore.Data;
using NHibernate.Criterion;

namespace MultiAlignCore.IO.Hibernate
{
    public class ScanSummaryDAOHibernate : GenericDAOHibernate<ScanSummary>
    {
        /// <summary>
        /// Searches for and returns a List of MS feature Objects in the Database that have the exact Dataset Id given.
        /// </summary>
        /// <returns>List of Umc Objects</returns>
        public List<ScanSummary> FindByDatasetId(int datasetId)
        {
            ICriterion criterion = Restrictions.Eq("GroupId", datasetId);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        public void DeleteByDatasetId(int datasetId)
        {
            DeleteByCriteria("T_ScanSummaries", "Dataset_ID", datasetId);
        }

        public void ClearAll()
        {
            var path = NHibernateUtil.Connection;
            using (var connection = new SQLiteConnection("Data Source = " + path + " ;", true))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM T_ScanSummaries";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
