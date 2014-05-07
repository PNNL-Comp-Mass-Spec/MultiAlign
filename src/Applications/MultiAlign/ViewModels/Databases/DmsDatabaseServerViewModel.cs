using MultiAlignCore.IO.InputFiles;

namespace MultiAlign.ViewModels.Databases
{
    public class DmsDatabaseServerViewModel: ViewModelBase
    {
        private InputDatabase m_server;

        public DmsDatabaseServerViewModel(InputDatabase server)
        {
            m_server = server;            
        }

        /// <summary>
        /// Gets the selected database server
        /// </summary>
        public InputDatabase Database { get {return m_server;} }

        /// <summary>
        /// Gets or sets the database server
        /// </summary>
        public string DatabaseServer
        {
            get
            {
                return m_server.DatabaseServer;
            }
            set
            {
                if (m_server.DatabaseServer != value)
                {
                    m_server.DatabaseServer = value;
                    OnPropertyChanged("DatabaseServer");
                }                
            }
        }
        /// <summary>
        /// Gets or sets the database name
        /// </summary>
        public string DatabaseName
        {
            get
            {
                return m_server.DatabaseName;
            }
            set
            {
                if (m_server.DatabaseName != value)
                {
                    m_server.DatabaseName = value;
                    OnPropertyChanged("DatabaseName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the database name
        /// </summary>
        public string Description
        {
            get
            {
                return m_server.Description;
            }
            set
            {
                if (m_server.Description != value)
                {
                    m_server.Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Gets or sets the database name
        /// </summary>
        public string Organism
        {
            get
            {
                return m_server.Organism;
            }
            set
            {
                if (m_server.Organism != value)
                {
                    m_server.Organism = value;
                    OnPropertyChanged("Organism");
                }
            }
        }

    }
}
