using MultiAlignCore.Data.Alignment;
using System.Data.SQLite;
using MultiAlignCore.IO.Features.Hibernate;

namespace MultiAlignCore.IO.Features.Hibernate
{

	public class AlignmentDAOHibernate : GenericDAOHibernate<classAlignmentData>, IAlignmentDAO
    {

        public void ClearAll()
        {
            string path = NHibernateUtil.Connection;
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + path + " ;"))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM T_AlignmentData";
                    command.ExecuteNonQuery();
                }
            }
        }
    }

}
