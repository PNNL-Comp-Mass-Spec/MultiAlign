namespace MultiAlignRogue.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using InformedProteomics.Backend.Utils;

    using MultiAlign.ViewModels.Databases;
    using MultiAlign.ViewModels.IO;
    using MultiAlign.Windows.Viewers.Databases;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.MassTags;
    using MultiAlignCore.IO.InputFiles;
    using MultiAlignCore.IO.MTDB;

    /// <summary>
    /// Singleton view model for selecting AMT tag database and loading it
    /// into the analysis database.
    /// It is a singleton because only one AMT tag database can be selected by the application
    /// at a time.
    /// </summary>
    public class DatabaseSelectionViewModel : ViewModelBase
    {
        /// <summary>
        /// Singleton instance of this class.
        /// </summary>
        private static DatabaseSelectionViewModel instance;

        /// <summary>
        /// The analysis to store selection in.
        /// </summary>
        private MultiAlignAnalysis analysis;

        /// <summary>
        /// The selected database server.
        /// </summary>
        private DmsDatabaseServerViewModel selectedDatabaseServer;

        /// <summary>
        /// A value that indicates whether the mass tag progress bar
        /// should be shown.
        /// </summary>
        private bool showMassTagProgress;

        /// <summary>
        /// The progress for loading the AMT tag
        /// database into the analysis database.
        /// </summary>
        private double massTagLoadProgress;

        /// <summary>
        /// Prevents a default instance of the <see cref="DatabaseSelectionViewModel"/> class from being created.
        /// </summary>
        private DatabaseSelectionViewModel()
        {
            this.ShowMassTagProgress = false;
            this.Analysis = new MultiAlignAnalysis();
            this.SelectAMTCommand = new RelayCommand(async () => await this.SelectAMTAsync());
        }

        /// <summary>
        /// Gets a command that opens a window for selecting an AMT database
        /// and then adds it to the analysis database asynchronously.
        /// </summary>
        public ICommand SelectAMTCommand { get; private set; }

        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        public static DatabaseSelectionViewModel Instance
        {
            get
            {
                return instance ?? (instance = new DatabaseSelectionViewModel());
            }
        }

        /// <summary>
        /// Gets or sets the analysis to store selection in.
        /// </summary>
        public MultiAlignAnalysis Analysis
        {
            get { return this.analysis; }
            set
            {
                if (this.analysis != value)
                {
                    this.analysis = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the selected database server.
        /// </summary>
        public DmsDatabaseServerViewModel SelectedDatabaseServer
        {
            get { return this.selectedDatabaseServer; }
            private set
            {
                if (this.selectedDatabaseServer != value)
                {
                    this.selectedDatabaseServer = value;
                    this.RaisePropertyChanged("SelectedDatabaseServer", null, value, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mass tolerance for the mass tag alignment/matching.
        /// </summary>
        public double MassTolerance
        {
            get { return this.analysis.Options.LcmsClusteringOptions.InstrumentTolerances.Mass; }
            set
            {
                this.analysis.Options.LcmsClusteringOptions.InstrumentTolerances.Mass = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the NET tolerance for the mass tag alignment/matching.
        /// </summary>
        public double NetTolerance
        {
            get { return this.analysis.Options.LcmsClusteringOptions.InstrumentTolerances.Net; }
            set
            {
                this.analysis.Options.LcmsClusteringOptions.InstrumentTolerances.Net = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the mass tag progress bar
        /// should be shown.
        /// </summary>
        public bool ShowMassTagProgress
        {
            get { return this.showMassTagProgress; }
            set
            {
                if (this.showMassTagProgress != value)
                {
                    this.showMassTagProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the progress for loading the AMT tag
        /// database into the analysis database.
        /// </summary>
        public double MassTagLoadProgress
        {
            get { return this.massTagLoadProgress; }
            private set
            {
                if (Math.Abs(this.massTagLoadProgress - value) > float.Epsilon)
                {
                    this.massTagLoadProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Load an AMT tag database into the analysis database.
        /// </summary>
        /// <param name="inputDatabase">The AMT tag database to load.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task LoadMassTagDatabase(InputDatabase inputDatabase)
        {
            this.SelectedDatabaseServer = new DmsDatabaseServerViewModel(inputDatabase);
            await this.AddMassTagsAsync();
        }

        /// <summary>
        /// Open a window for selecting an AMT database and then adding it to the analysis database
        /// asynchronously.
        /// </summary>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task SelectAMTAsync()
        {
            this.SelectAMT();
            if (this.SelectedDatabaseServer != null)
            {
                this.analysis.Options.AlignmentOptions.InputDatabase = this.SelectedDatabaseServer.Database;
                await this.AddMassTagsAsync();
            }
        }

        /// <summary>
        /// Open a window for selecting an AMT database and settings.
        /// </summary>
        private void SelectAMT()
        {
            var dmsWindow = new DatabaseSearchToolWindow();
            var optionsViewModel = new MassTagDatabaseOptionsViewModel(this.analysis.Options.MassTagDatabaseOptions);
            var databaseView = new DatabasesViewModel { MassTagOptions = optionsViewModel };
            dmsWindow.DataContext = databaseView;
            dmsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var loader = MassTagDatabaseLoaderFactory.Create(MtdbDatabaseServerType.Dms);
            var databases = loader.LoadDatabases();

            foreach (var database in databases)
            {
                databaseView.AddDatabase(database);
            }

            if (this.SelectedDatabaseServer != null)
            {
                databaseView.SelectedDatabase = this.SelectedDatabaseServer;
            }

            var result = dmsWindow.ShowDialog();
            if (result == true)
            {
                if (databaseView.SelectedDatabase != null)
                {
                    this.SelectedDatabaseServer = databaseView.SelectedDatabase;
                }
            }
        }

        /// <summary>
        /// Asynchronously loads mass tags from AMT database into analysis database.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task AddMassTagsAsync()
        {
            await Task.Run(() => this.AddMassTags());
        }

        /// <summary>
        /// Loads mass tags from AMT database into analysis database.
        /// </summary>
        private void AddMassTags()
        {
            var loadProgress = new Progress<ProgressData>(pd => this.MassTagLoadProgress = pd.Percent);
            var database = this.SelectedDatabaseServer.Database;
            this.Analysis.MetaData.Database = database;
            this.Analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.MassTagSystemSql;
            this.Analysis.MassTagDatabase = MtdbLoaderFactory.LoadMassTagDatabase(
                                        this.analysis.MetaData.Database,
                                        this.analysis.Options.MassTagDatabaseOptions);
            this.Analysis.DataProviders.MassTags.DeleteAll();
            this.ShowMassTagProgress = true;
            this.Analysis.DataProviders.MassTags.AddAllStateless(this.Analysis.MassTagDatabase.MassTags, loadProgress);
            this.ShowMassTagProgress = false;
        }
    }
}
