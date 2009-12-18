/// <file>NHibernateUtil.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

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
    public class NHibernateUtil
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
		/// Creates a SQLite database based on the hibernate config file.
		/// </summary>
		/// <param name="dbLocation">The file location of the database file to be created. The file should not exist before the database is created.</param>
		public static void createDatabase(String dbLocation)
		{
			setConfigurationDbLocation(dbLocation);
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
		/// The given database should already exist. If it does not, it will be created for you. Hurray!
		/// </summary>
		/// <param name="dbLocation">The file location of the database file</param>
		public static void setDbLocation(string dbLocation)
		{
			if (m_dbLocation == null || (m_dbLocation != null && !m_dbLocation.Equals(dbLocation)))
			{
				m_dbLocation = dbLocation;
				m_sessionFactory = null;

				if (!File.Exists(dbLocation))
				{
					createDatabase(dbLocation);
				}
			}
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
						setConfigurationDbLocation(m_dbLocation);
					}
					else
					{
						string connectionString = configuration.GetProperty("connection.connection_string");
						string fileLocation = connectionString.Split('=', ';')[1];
						if (!File.Exists(fileLocation))
						{
							createDatabase(fileLocation);
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
		private static void setConfigurationDbLocation(string dbLocation)
		{
			configuration.SetProperty("connection.connection_string", "Data Source=" + dbLocation + ";Version=3");
		}

    }

}