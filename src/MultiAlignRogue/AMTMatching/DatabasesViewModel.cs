using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using MultiAlignCore.IO.InputFiles;

namespace MultiAlignRogue.AMTMatching
{
    public class DatabasesViewModel : ViewModelBase
    {
        private const string CONST_ANY_ORGANISM = "<Show All>";
        private string m_databaseNameFilter;
        private string m_organismFilter;
        private DmsDatabaseServerViewModel m_selectedDatabase;

        public DatabasesViewModel()
        {
            Databases = new ObservableCollection<DmsDatabaseServerViewModel>();
            FilteredDatabases = new ObservableCollection<DmsDatabaseServerViewModel>();
            SelectedDatabase = null;
            Organisms = new ObservableCollection<string>();
            m_organismFilter = "";
            m_databaseNameFilter = "";
        }

        /// <summary>
        /// Gets the database collection
        /// </summary>
        public ObservableCollection<DmsDatabaseServerViewModel> Databases { get; private set; }

        public ObservableCollection<DmsDatabaseServerViewModel> FilteredDatabases { get; private set; }

        /// <summary>
        /// Gets or sets the database name filter
        /// </summary>
        public string DatabaseFilter
        {
            get { return m_databaseNameFilter; }
            set
            {
                if (m_databaseNameFilter != value)
                {
                    m_databaseNameFilter = value;

                    if (value != null)
                    {
                        FilterDatabases();
                    }
                    else
                    {
                        m_databaseNameFilter = "";
                        FilterDatabases();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected database.
        /// </summary>
        public DmsDatabaseServerViewModel SelectedDatabase
        {
            get { return m_selectedDatabase; }
            set
            {
                if (m_selectedDatabase != value)
                {
                    m_selectedDatabase = value;
                    RaisePropertyChanged("SelectedDatabase");
                }
            }
        }

        public ObservableCollection<string> Organisms { get; private set; }

        public MassTagDatabaseOptionsViewModel MassTagOptions { get; set; }

        public string SelectedOrganism
        {
            get { return m_organismFilter; }
            set
            {
                if (m_organismFilter != value)
                {
                    if (value == null || value == CONST_ANY_ORGANISM)
                    {
                        m_organismFilter = "";
                    }
                    else
                    {
                        m_organismFilter = value;
                    }
                    FilterDatabases();

                    RaisePropertyChanged("SelectedOrganism");
                }
            }
        }

        private void BuildOrganismsList(ObservableCollection<DmsDatabaseServerViewModel> servers)
        {
            var map = new Dictionary<string, string>();
            var orderedServers = servers.OrderBy(x => x.Organism.ToLower());
            foreach (var server in orderedServers)
            {
                if (!map.ContainsKey(server.Organism))
                {
                    map.Add(server.Organism, server.Organism);
                }
            }

            var tempOrganism = m_organismFilter;

            Organisms.Clear();
            Organisms.Add(CONST_ANY_ORGANISM);
            foreach (var organism in map.Values)
            {
                if (organism != "")
                {
                    Organisms.Add(organism);
                }
            }
            SelectedOrganism = tempOrganism;
        }

        /// <summary>
        /// Adds a database to the collection
        /// </summary>
        /// <param name="server"></param>
        public void AddDatabase(InputDatabase server)
        {
            var viewModel = new DmsDatabaseServerViewModel(server);
            Databases.Add(viewModel);
            FilteredDatabases.Add(viewModel);
            BuildOrganismsList(Databases);
        }

        private void FilterDatabases()
        {
            IEnumerable<DmsDatabaseServerViewModel> databases =
                Databases.Where(
                    x =>
                        x.DatabaseName.ToLower().Contains(DatabaseFilter.ToLower()) &&
                        x.Organism.ToLower().Contains(m_organismFilter.ToLower()))
                    .OrderBy(x => x.DatabaseName.ToLower());
            FilteredDatabases.Clear();
            foreach (var server in databases)
            {
                FilteredDatabases.Add(server);
            }
        }
    }
}