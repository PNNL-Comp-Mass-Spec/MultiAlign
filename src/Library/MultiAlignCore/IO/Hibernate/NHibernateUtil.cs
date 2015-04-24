#region

using System;
using System.Data.SQLite;
using System.IO;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

#endregion

namespace MultiAlignCore.IO.Features.Hibernate
{
    /// <summary>
    ///     Hibernate Session Factory that will create a session if one does not already exist.
    ///     To create a session, Hibernate will need to be configured.
    ///     In the case of this class, we are using a Hibernate Configuration file.
    /// </summary>
    public static class NHibernateUtil
    {
        private static String m_dbLocation;
        private static readonly Configuration Configuration;
        private static ISessionFactory m_sessionFactory;

        /// <summary>
        ///     Static constructor that initializes the Hibernate Configuration object
        /// </summary>
        static NHibernateUtil()
        {
            Configuration = new Configuration();
            Configuration.Configure();
            Configuration.AddAssembly(typeof (NHibernateUtil).Assembly);
        }

        /// <summary>
        ///     Gets the path of the database location.
        /// </summary>
        public static string Path
        {
            get { return m_dbLocation; }
        }

        /// <summary>
        ///     Returns a session that is created from the SessionFactory. If a session already existed, it will return the
        ///     existing Session, not create a new one.
        /// </summary>
        /// <returns>ISession object for the current hibernate session</returns>
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        /// <summary>
        ///     Returns a session that is created from the SessionFactory. If a session already existed, it will return the
        ///     existing Session, not create a new one.
        /// </summary>
        /// <returns>ISession object for the current hibernate session</returns>
        public static IStatelessSession OpenStatelessSession()
        {
            return SessionFactory.OpenStatelessSession();
        }


        /// <summary>
        ///     Closes a session.
        /// </summary>
        public static void CloseSession()
        {
            SessionFactory.Close();
        }

        /// <summary>
        ///     Creates a SQLite database based on the hibernate config file.
        /// </summary>
        /// <param name="dbLocation">
        ///     The file location of the database file to be created. The file should not exist before the
        ///     database is created.
        /// </param>
        public static void CreateDatabase(String dbLocation)
        {
            SetConfigurationDbLocation(dbLocation);
            m_dbLocation = dbLocation;
            m_sessionFactory = null;

            using (var conn = new SQLiteConnection("Data Source=" + dbLocation + ";Version=3;New=True", true))
            {
                conn.Open();
                var schemaExport = new SchemaExport(Configuration);
                //schemaExport.Execute(false, true, false, false, conn, null);
                schemaExport.Execute(false, true, false, conn, null);

                var optimizationCommands = new[]
                {
                    "PRAGMA journal_mode = OFF",
                    "PRAGMA synchronous = OFF",
                    "PRAGMA page_size = 65536"
                };
                foreach (var commandText in optimizationCommands)
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        ///     Sets the location of the database file that the Hibernate Session should be attached to. If a Session already
        ///     existed,
        ///     it will be killed so that a new Session can be created that points to the new database location.
        /// </summary>
        /// <param name="databaseLocation">The file location of the database file.</param>
        /// <param name="createIfMissing">Flag indicating whether or not to ignore if the file exists or not.</param>
        public static void ConnectToDatabase(string databaseLocation, bool createIfMissing)
        {
            m_dbLocation = databaseLocation;

            // If the database does not exist, and we are trying to connect to it (not create)
            // then we have a problem.
            var exists = File.Exists(databaseLocation);
            if (!exists && !createIfMissing)
            {
                throw new FileNotFoundException("The file does not exist.");
            }

            // Otherwise, make sure we have a clossed session.
            if (m_sessionFactory != null)
            {
                m_sessionFactory.Close();
                m_sessionFactory = null;
            }

            // If the database is missing, and you want to create a database if it's missing, construct and open open it.
            if (createIfMissing && !exists)
            {
                CreateDatabase(databaseLocation);
            }
        }

        /// <summary>
        ///     SessionFactory object containing a get method that creates and returns a Hibernate SessionFactory if one does not
        ///     already exist
        ///     If a SessionFactory already exists, that object will be returned.
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
                        var connectionString = Configuration.GetProperty("connection.connection_string");
                        var fileLocation = connectionString.Split('=', ';')[1];
                        if (!File.Exists(fileLocation))
                        {
                            CreateDatabase(fileLocation);
                        }
                    }
                    m_sessionFactory = Configuration.BuildSessionFactory();
                }
                return m_sessionFactory;
            }
        }

        /// <summary>
        ///     Sets the database file location in the Configuration so Hibernate knows where to look.
        /// </summary>
        /// <param name="dbLocation">The file location of the database file</param>
        private static void SetConfigurationDbLocation(string dbLocation)
        {
            Configuration.SetProperty("connection.connection_string",
                "Data Source=" + dbLocation + ";Version=3;New=True");
        }

        public static string Connection
        {
            get { return m_dbLocation; }
        }

        #region IDisposable Members

        public static void Dispose()
        {
            try
            {
                SessionFactory.Close();
            }
            catch
            {
            }
            SessionFactory.Dispose();
        }

        #endregion
    }
}