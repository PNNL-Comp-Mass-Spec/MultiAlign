#region

using System.Data.SQLite;
using MultiAlignCore.Data.Alignment;

#endregion

namespace MultiAlignCore.IO.Features.Hibernate
{
    public class AlignmentDAOHibernate : GenericDAOHibernate<classAlignmentData>, IAlignmentDAO
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