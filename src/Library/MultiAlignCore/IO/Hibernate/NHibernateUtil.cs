#region

using System;
using System.Data.SQLite;
using System.IO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

#endregion

namespace MultiAlignCore.IO.Hibernate
{
    /// <summary>
    ///     Hibernate Session Factory that will create a session if one does not already exist.
    ///     To create a session, Hibernate will need to be configured.
    ///     In the case of this class, we are using a Hibernate Configuration file.
    /// </summary>
    public static class NHibernateUtil
    {
        private static String m_dbLocation;
        //private static readonly Configuration Configuration;
        private static ISessionFactory m_sessionFactory;

        /// <summary>
        ///     Static constructor that initializes the Hibernate Configuration object
        /// </summary>
        static NHibernateUtil()
        {
            //Configuration = new Configuration();
            //// Load configuration from hibernate.cfg.xml
            //Configuration.Configure();
            //Configuration.AddAssembly(typeof (NHibernateUtil).Assembly);
            m_dbLocation = "Analysis.db3";
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

            m_sessionFactory = GetFluentConfiguration(m_dbLocation).BuildSessionFactory();
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
                    m_sessionFactory = GetFluentConfiguration(m_dbLocation).BuildSessionFactory();
                }
                return m_sessionFactory;
            }
        }

        private static FluentConfiguration GetFluentConfiguration(string dbLocation, bool createIfMissing = true)
        {
            if (!File.Exists(dbLocation) && !createIfMissing)
            {
                throw new FileNotFoundException("The file does not exist.");
            }

            var conf = Fluently.Configure()
                .Database(
                    SQLiteConfiguration.Standard
                        //.UsingFile(m_dbLocation) // Won't work because of the need for custom additions to the connection string
                        .AdoNetBatchSize(1000)
                        //.ShowSql()
                        .ConnectionString("Data Source=" + dbLocation + ";Version=3;New=True;PRAGMA journal_mode=OFF;PRAGMA synchronous=OFF;PRAGMA page_size=65536"))
                .Mappings(m =>
                {
                    //Directory.CreateDirectory("Mappings");
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.AlignmentDataMapping>()                        /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.ClusterToMassTagMapMapping>()                  /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.DatabaseSearchSequenceMapping>()               /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.DatasetInformationMapping>()                   /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.DatasetToExperimentalFactorMapMapping>()       /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.ExperimentalFactorMapping>()                   /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.InputFileMapping>()                            /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.LcmsWarpAlignmentMatchMapping>()               /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.MassTagLightMapping>()                         /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.MassTagToProteinMapMapping>()                  /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.MSFeatureLightMapping>()                       /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.MSFeatureToMSnFeatureMapMapping>()             /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.MSMSClusterMapMapping>()                       /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.MSSpectraMapping>()                            /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.OptionPairMapping>()                           /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.ParameterHibernateMappingMapping>()            /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.ProteinMapping>()                              /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.ScanSummaryMapping>()                          /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.SequenceToMsnFeatureMapping>()                 /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.STACFDRMapping>()                              /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.UMCClusterLightMapping>()                      /* .ExportTo("Mappings") */ ;
                    m.FluentMappings.AddFromAssemblyOf<FluentMappings.UMCLightMapping>()                             /* .ExportTo("Mappings") */ ;
                });

            if (!File.Exists(dbLocation))
            {
                conf = conf.ExposeConfiguration(BuildSchemaNew);
            }
            else
            {
                conf = conf.ExposeConfiguration(BuildSchemaOpen);
            }

            return conf;
        }

        /// <summary>
        /// Export the schema to a new database
        /// </summary>
        /// <param name="config"></param>
        private static void BuildSchemaNew(Configuration config)
        {
            new SchemaExport(config).Create(false, true);
        }

        /// <summary>
        /// Validate or update the schema in an existing database
        /// </summary>
        /// <param name="config"></param>
        private static void BuildSchemaOpen(Configuration config)
        {
            try
            {
                // Try to validate the schema. If it is correct, we can use it as is.
                new SchemaValidator(config).Validate();
                //config.SetProperty("adonet.batch_size", "100");
            }
            catch (HibernateException)
            {
                // Validation failed; we need to update the schema.
                // If this fails, we need to report an error.
                // If we are supposed to append, then only 'update' the schema
                // This will also create if it does not exist
                new SchemaUpdate(config).Execute(false, true);
            }
        }

        /// <summary>
        ///     Sets the database file location in the Configuration so Hibernate knows where to look.
        /// </summary>
        /// <param name="dbLocation">The file location of the database file</param>
        private static void SetConfigurationDbLocation(string dbLocation)
        {
            m_dbLocation = dbLocation;
            //Configuration.SetProperty("connection.connection_string",
            //    "Data Source=" + dbLocation + ";Version=3;New=True;PRAGMA journal_mode=OFF;PRAGMA synchronous=OFF;PRAGMA page_size=65536");
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