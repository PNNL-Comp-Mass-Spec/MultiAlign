using PNNLProteomics.MultiAlign.Hibernate.Domain;
using NHibernate;
using NHibernate.Cfg;
using System.Data.SQLite;
using System;
using NHibernate.Tool.hbm2ddl;
using System.IO;
using System.Reflection;

namespace PNNLProteomics.MultiAlign.Hibernate
{

    /// <summary>
    /// Hibernate Session Factory that will create a session if one does not already exist.
    /// To create a session, Hibernate will need to be configured.
    /// In the case of this class, we are using a Hibernate Configuration file.
    /// </summary>
    public static class NHibernateUtil
    {
		private static String m_dbLocation = null;
		private static Configuration configuration; 
        private static ISessionFactory m_sessionFactory;

		/// <summary>
		/// Static constructor that initializes the Hibernate Configuration object
		/// </summary>
		static NHibernateUtil()
		{
			configuration = new Configuration();            
			configuration.Configure();
			configuration.AddAssembly(typeof(NHibernateUtil).Assembly);
		}

		/// <summary>
		/// Returns a session that is created from the SessionFactory. If a session already existed, it will return the existing Session, not create a new one.
		/// </summary>
		/// <returns>ISession object for the current hibernate session</returns>
		public static ISession OpenSession()
		{
			return SessionFactory.OpenSession();
		}

        /// <summary>
        /// Closes a session.
        /// </summary>
        public static void CloseSession()
        {
            SessionFactory.Close();
        }

		/// <summary>
		/// Creates a SQLite database based on the hibernate config file.
		/// </summary>
		/// <param name="dbLocation">The file location of the database file to be created. The file should not exist before the database is created.</param>
		public static void CreateDatabase(String dbLocation)
		{
			SetConfigurationDbLocation(dbLocation);
			m_dbLocation = dbLocation;
			m_sessionFactory = null;

			using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + dbLocation + ";Version=3;New=True"))
			{
				conn.Open();
				SchemaExport schemaExport = new SchemaExport(configuration);
				schemaExport.Execute(false, true, false, false, conn, null);
			}
		}

		/// <summary>
		/// Sets the location of the database file that the Hibernate Session should be attached to. If a Session already existed,
		/// it will be killed so that a new Session can be created that points to the new database location.
		/// The given database should already exist. If it does not, an exception is thrown.
		/// </summary>
		/// <param name="dbLocation">The file location of the database file</param>
		public static void SetDbLocationForRead(string dbLocation)
		{
			if (m_dbLocation == null || (m_dbLocation != null && !m_dbLocation.Equals(dbLocation)))
			{
				m_dbLocation = dbLocation;

				if (m_sessionFactory != null)
				{
					m_sessionFactory.Close();
					m_sessionFactory = null;
				}

				if (!File.Exists(dbLocation))
				{
					throw new Exception("Database cannot be read because it does not exist.");
				}
			}
		}

		/// <summary>
		/// Sets the location of the database file that the Hibernate Session should be attached to. If a Session already existed,
		/// it will be killed so that a new Session can be created that points to the new database location.
		/// The given database should not already exist and it will be created for you. Hurray!
		/// </summary>
		/// <param name="dbLocation">The file location of the database file</param>
		public static void SetDbLocationForWrite(string dbLocation)
		{
			m_dbLocation = dbLocation;

			if (m_sessionFactory != null)
			{
				m_sessionFactory.Close();
				m_sessionFactory = null;
			}

			if (File.Exists(dbLocation))
			{
				File.Delete(dbLocation);
			}

			CreateDatabase(dbLocation);
		}

		/// <summary>
		/// SessionFactory object containing a get method that creates and returns a Hibernate SessionFactory if one does not already exist
		/// If a SessionFactory already exists, that object will be returned.
		/// </summary>
		private static ISessionFactory SessionFactory
		{
			get
			{
				if (m_sessionFactory == null)
				{
					if (m_dbLocation != null)
					{
						SetConfigurationDbLocation(m_dbLocation);
					}
					else
					{
						string connectionString = configuration.GetProperty("connection.connection_string");
						string fileLocation = connectionString.Split('=', ';')[1];
						if (!File.Exists(fileLocation))
						{
							CreateDatabase(fileLocation);
						}
					}
					m_sessionFactory = configuration.BuildSessionFactory();
				}
				return m_sessionFactory;
			}
		}

		/// <summary>
		/// Sets the database file location in the Configuration so Hibernate knows where to look.
		/// </summary>
		/// <param name="dbLocation">The file location of the database file</param>
		private static void SetConfigurationDbLocation(string dbLocation)
		{
			configuration.SetProperty("connection.connection_string", "Data Source=" + dbLocation + ";Version=3;New=True");
		}


        #region IDisposable Members

        public static void Dispose()
        {            
            NHibernateUtil.SessionFactory.Dispose();
        }

        #endregion
    }

}