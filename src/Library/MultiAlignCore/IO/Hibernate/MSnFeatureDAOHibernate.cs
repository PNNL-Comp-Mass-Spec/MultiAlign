using System.Collections.Generic;
using MultiAlignEngine.Features;
using NHibernate.Criterion;
using PNNLOmics.Data;
using System.Data.SQLite;


namespace MultiAlignCore.IO.Features.Hibernate
{
    public class MSnFeatureDAOHibernate : GenericDAOHibernate<MSSpectra>, IMSnFeatureDAO
    {

        /// <summary>
        /// Searches for and returns a List of MS Spectra Objects in the Database that have the exact Dataset Id given.
        /// </summary>
        /// <param name="mass">Dataset value to be searched for</param>
        /// <returns>List of Umc Objects</returns>
        public List<MSSpectra> FindByDatasetId(int datasetId)
        {
            ICriterion criterion = Expression.Eq("GroupID", datasetId);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        public List<MSSpectra> FindBySpectraId(List<int> spectraId)
        {            
            ICriterion criterion = Expression.In("ID", spectraId);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }
        /// <summary>
        /// Gets the number of MS/MS Spectra that were linked in the experiment.
        /// </summary>
        /// <returns></returns>
        public int GetMsMsCount()
        {
            int count   = 0;
            string path = NHibernateUtil.Connection;
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + path + " ;"))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    // This is a hack, but the provider was incorrectly 
                    // saying that "SELECT COUNT(*) FROM..."
                    // always had one entry, whether one row existed, or five, or none.
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "SELECT * FROM T_Msn_Features";
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        
                        object [] values = new object[10];
                        while (reader.Read())
                        {
                            int test = System.Convert.ToInt32(reader["SPECTRA_ID"]);
                            count ++;
                        }                        
                    }
                }
            }
            return count;
        }
    }
}
