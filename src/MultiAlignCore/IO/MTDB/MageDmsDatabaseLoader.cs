using System.Collections.Generic;
using MultiAlignCore.IO.InputFiles;
using Mage;
using System;

namespace MultiAlignCore.IO.MTDB
{
    /// <summary>
    /// Loads database information from the main server
    /// </summary>
    public class MageDmsDatabaseLoader: IDatabaseServerLoader , ISinkModule 
    {
        private const string CONST_DATABASE_LOAD_QUERY  = "SELECT * FROM V_MTS_MT_DBs WHERE State_ID < 15";
        private const string CONST_SERVER               = "gigasax";
        private const string CONST_DATABASE             = "DMS5";
        private const string CONST_SERVERNAME           = "server_name";
        private const string CONST_DATABASENAME         = "mt_db_name";
        private const string CONST_ORGANISM             = "organism";
        private const string CONST_DESCRIPTION          = "description";
        private const string CONST_JOBS                 = "msms_jobs";
        
        private int m_serverNameColumn   = 0;
        private int m_databaseNameColumn = 0;
        private int m_organism           = 0;
        private int m_description        = 0;
        private int m_msmsJobs           = 0;        

        private List<InputDatabase> m_databases;

        public MageDmsDatabaseLoader()
        {
            Server      = CONST_SERVER;
            Database    = CONST_DATABASE;
        }


        #region IDatabaseServerLoader Members
        public ICollection<InputDatabase> LoadDatabases()
        {            
            m_databases                     = new List<InputDatabase>();
            using (MSSQLReader reader       = new MSSQLReader())
            {
                reader.Server               = Server;
                reader.Database             = Database;
                reader.SQLText              = CONST_DATABASE_LOAD_QUERY;
                ProcessingPipeline pipeline = ProcessingPipeline.Assemble("Databases", reader, this);
                pipeline.RunRoot(null);
            }
            return m_databases;
        }
        #endregion

        public string Server { get; set; }
        public string Database { get; set; }

      
        #region ISinkModule Members
        /// <summary>
        /// Handles the definition of columns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            int i = 0;
            foreach (MageColumnDef columnName in args.ColumnDefs)
            {
                switch (columnName.Name.ToLower())
                {
                    case CONST_DATABASENAME:
                        m_databaseNameColumn = i;
                        break;
                    case CONST_SERVERNAME:
                        m_serverNameColumn  = i;
                        break;
                    case CONST_DESCRIPTION:
                        m_description       = i;
                        break;
                    case CONST_ORGANISM:
                        m_organism          = i;
                        break;
                    case CONST_JOBS:
                        m_msmsJobs          = i;
                        break;
                    default:
                        break;
                }
                i++;
            }
        }



        /// <summary>
        /// Handles the definitions of data rows.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (!args.DataAvailable)
            {
                return;
            }

            InputDatabase database  = new InputDatabase();
            database.DatabaseName   = args.Fields[m_databaseNameColumn].ToString();
            database.DatabaseServer = args.Fields[m_serverNameColumn].ToString();
            database.Description    = args.Fields[m_description].ToString();
            database.Organism       = args.Fields[m_organism].ToString();
            database.Jobs           = Convert.ToInt32(args.Fields[m_msmsJobs]);

            m_databases.Add(database);
        }
        #endregion
    }

    public class MageMSGFFinderLoader : ISinkModule
    {
        private const string    CONST_DATABASE_LOAD_QUERY  = "SELECT * FROM V_Analysis_Job_List_Report_2 WHERE [State] LIKE '%Complete%' AND [Tool] LIKE '%MSGFPlus%' AND [Dataset] LIKE '%{0}%' AND [Last_Affected] > DATEADD(Week, -104, GETDATE()) AND [Instrument] LIKE '%LTQ%'";
        private const string    CONST_SERVER               = "gigasax";
        private const string    CONST_DATABASE             = "DMS5";
        private const string    CONST_ARCHIVENAME          = "archive folder path";                
        private int             m_archiveFolderPath        = 0;        
        private List<string>    m_folders;

        public MageMSGFFinderLoader()
        {
            Server      = CONST_SERVER;
            Database    = CONST_DATABASE;
        }

        #region IDatabaseServerLoader Members
        public List<string> LoadFiles(string name)
        {
            m_folders = new List<string>();

            using (MSSQLReader reader       = new MSSQLReader())
            {
                reader.Server               = Server;
                reader.Database             = Database;
                reader.SQLText              = string.Format(CONST_DATABASE_LOAD_QUERY, name);
                ProcessingPipeline pipeline = ProcessingPipeline.Assemble("Results", reader, this);
                pipeline.RunRoot(null);
            }
            return m_folders;
        }
        #endregion

        public string Server { get; set; }
        public string Database { get; set; }
              
        #region ISinkModule Members
        /// <summary>
        /// Handles the definition of columns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            int i = 0;
            foreach (MageColumnDef columnName in args.ColumnDefs)
            {
                switch (columnName.Name.ToLower())
                {
                    case CONST_ARCHIVENAME:
                        m_archiveFolderPath = i;
                        break;
                    default:
                        break;
                }
                i++;
            }
        }

        /// <summary>
        /// Handles the definitions of data rows.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (!args.DataAvailable)
            {
                return;
            }            
            string value = args.Fields[m_archiveFolderPath].ToString();                        
            m_folders.Add(value);
        }
        #endregion

    }
}
