#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using NHibernate.Criterion;
using PNNLOmics.Data;

#endregion

namespace MultiAlignCore.IO.Features.Hibernate
{
    public class MSnFeatureDAOHibernate : GenericDAOHibernate<MSSpectra>, IMSnFeatureDAO
    {
        /// <summary>
        ///     Searches for and returns a List of MS Spectra Objects in the Database that have the exact Dataset Id given.
        /// </summary>
        /// <returns>List of Umc Objects</returns>
        public List<MSSpectra> FindByDatasetId(int datasetId)
        {
            ICriterion criterion = Restrictions.Eq("GroupId", datasetId);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        public List<MSSpectra> FindBySpectraId(List<int> spectraId)
        {
            ICriterion criterion = Restrictions.In("Id", spectraId);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        /// <summary>
        ///     Gets the number of MS/MS Spectra that were linked in the experiment.
        /// </summary>
        /// <returns></returns>
        public int GetMsMsCount()
        {
            var count = 0;
            var path = NHibernateUtil.Connection;
            using (var connection = new SQLiteConnection("Data Source = " + path + " ;", true))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // This is a hack, but the provider was incorrectly 
                    // saying that "SELECT COUNT(*) FROM..."
                    // always had one entry, whether one row existed, or five, or none.
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM T_Msn_Features";
                    using (var reader = command.ExecuteReader())
                    {
                        var values = new object[10];
                        while (reader.Read())
                        {
                            var test = Convert.ToInt32(reader["SPECTRA_ID"]);
                            count ++;
                        }
                    }
                }
            }
            return count;
        }
    }
}