#region

using System.Data.SQLite;
using FeatureAlignment.Data.Alignment;
using MultiAlignCore.IO.Features;

#endregion

namespace MultiAlignCore.IO.Hibernate
{
    public class AlignmentDAOHibernate : GenericDAOHibernate<AlignmentData>, IAlignmentDAO
    {
        public void ClearAll()
        {
            var path = NHibernateUtil.Connection;
            using (var connection = new SQLiteConnection("Data Source = " + path + " ;", true))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM T_AlignmentData";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}