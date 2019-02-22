using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MultiAlign.Commands;
using MultiAlign.ViewModels.Databases;
using MultiAlign.ViewModels.Datasets;
using MultiAlign.ViewModels.IO;
using MultiAlign.Windows.Viewers.Databases;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.MTDB;

namespace MultiAlign.ViewModels.Wizard
{
    public class AnalysisBaselineSelectionViewModel : ViewModelBase
    {
        private readonly MultiAlignAnalysis m_analysis;
        private DatasetInformationViewModel m_selectedDataset;
        private DmsDatabaseServerViewModel m_selectedDmsDatabase;

        public AnalysisBaselineSelectionViewModel(MultiAlignAnalysis analysis)
        {
            var filter =
                "Mass Tag Database (.db3)|*.db3|Direct Infusion IMS Database (.dims)|*.dims|All Files (*.*)|*.*";
            m_analysis = analysis;

            IsDatabaseDms = false;
            IsDatabaseLocal = false;
            IsBaselineDataset = true;

            SetDatabaseToDms = new BaseCommand(SetDatabaseToDmsDelegate, BaseCommand.AlwaysPass);
            SetDatabaseToLocal = new BaseCommand(SetDatabaseToLocalDelegate, BaseCommand.AlwaysPass);
            SetBaselineToDatabase = new BaseCommand(SetBaselineToDatabaseDelegate, BaseCommand.AlwaysPass);
            SetBaselineToDataset = new BaseCommand(SetBaselineToDatasetDelegate, BaseCommand.AlwaysPass);
            FindLocalDatabase = new BrowseOpenFileCommand(x =>
            {
                DatabaseFilePath = x;
                IsDatabaseLocal = true;
                OnPropertyChanged("RequiresDatabaseSelection");
            }, filter);
            FindDmsDatabase = new BaseCommand(FindDmsDatabaseDelegate, BaseCommand.AlwaysPass);
            ClearDatabase = new BaseCommand(ClearDatabaseDelegate, BaseCommand.AlwaysPass);
            Datasets = new ObservableCollection<DatasetInformationViewModel>();
            UpdateDatasets();

            StacOptionsViewModel = new StacOptionsViewModel(analysis.Options.StacOptions);
            MassTagDatabaseOptionsViewModel =
                new MassTagDatabaseOptionsViewModel(analysis.Options.MassTagDatabaseOptions);
        }

        public StacOptionsViewModel StacOptionsViewModel { get; set; }
        public MassTagDatabaseOptionsViewModel MassTagDatabaseOptionsViewModel { get; set; }

        public string DatabaseFilePath
        {
            get { return m_analysis.MetaData.Database.LocalPath; }
            set
            {
                if (m_analysis.MetaData.Database.LocalPath != value)
                {
                    m_analysis.MetaData.Database.DatabaseFormat = InputDatabase.DetermineFormat(value);
                    m_analysis.MetaData.Database.LocalPath = value;
                    OnPropertyChanged("DatabaseFilePath");
                }
            }
        }

        public bool IsBaselineDataset
        {
            get { return IsBaselineDatabase == false; }

            set
            {
                if (m_analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB == value)
                {
                    m_analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = !value;
                    OnPropertyChanged("IsBaselineDatabase");
                    OnPropertyChanged("IsBaselineDataset");
                    OnPropertyChanged("RequiresDatabaseSelection");
                }
            }
        }

        public bool IsBaselineDatabase
        {
            get { return m_analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB; }

            set
            {
                if (m_analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB != value)
                {
                    m_analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = value;
                    if (value)
                    {
                        SelectedDataset = null;
                    }
                    OnPropertyChanged("RequiresDatabaseSelection");

                    OnPropertyChanged("IsBaselineDatabase");
                    OnPropertyChanged("IsBaselineDataset");
                }
            }
        }

        public DmsDatabaseServerViewModel SelectedDatabaseServer
        {
            get { return m_selectedDmsDatabase; }
            set
            {
                if (m_selectedDmsDatabase != value)
                {
                    m_selectedDmsDatabase = value;
                    OnPropertyChanged("SelectedDatabaseServer");
                }
            }
        }

        /// <summary>
        /// Gets the list of datasets.
        /// </summary>
        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set; }

        /// <summary>
        /// Gets or sets the selected dataset for baseline.
        /// </summary>
        public DatasetInformationViewModel SelectedDataset
        {
            get { return m_selectedDataset; }
            set
            {
                if (m_selectedDataset != value)
                {
                    if (value == null)
                    {
                        m_analysis.MetaData.BaselineDataset = null;
                    }
                    else
                    {
                        m_selectedDataset = value;
                        m_analysis.MetaData.BaselineDataset = value.Dataset;
                    }

                    OnPropertyChanged("SelectedDataset");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether a database is local
        /// </summary>
        public bool IsDatabaseLocal
        {
            get { return (m_analysis.MetaData.Database.DatabaseFormat != MassTagDatabaseFormat.MassTagSystemSql); }
            set
            {
                var format = m_analysis.MetaData.Database.DatabaseFormat;

                var isLocal = (format != MassTagDatabaseFormat.MassTagSystemSql && format != MassTagDatabaseFormat.None);
                if (isLocal != value)
                {
                    // If we are being told not to set it to be local...we change...
                    if (!value)
                        m_analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.MassTagSystemSql;
                    else
                        m_analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.Sqlite;

                    OnPropertyChanged("IsDatabaseLocal");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether a database is dms
        /// </summary>
        public bool IsDatabaseDms
        {
            get { return (m_analysis.MetaData.Database.DatabaseFormat == MassTagDatabaseFormat.MassTagSystemSql); }
            set
            {
                var isDms = (m_analysis.MetaData.Database.DatabaseFormat == MassTagDatabaseFormat.MassTagSystemSql);
                if (isDms != value)
                {
                    // If we are being told to set it to be local...we change...
                    if (value)
                        m_analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.MassTagSystemSql;
                    else
                        m_analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.Sqlite;

                    OnPropertyChanged("IsDatabaseLocal");
                    OnPropertyChanged("IsDatabaseDms");
                }
            }
        }

        /// <summary>
        /// Gets whether a message declaring the database selection is visible or not.
        /// </summary>
        public Visibility RequiresDatabaseSelection
        {
            get
            {
                if (IsBaselineDatabase)
                {
                    if (m_analysis.MetaData.Database.DatabaseFormat != MassTagDatabaseFormat.None)
                        return Visibility.Hidden;

                    return Visibility.Visible;
                }


                return Visibility.Hidden;
            }
        }

        public void UpdateDatasets()
        {
            Datasets.Clear();
            foreach (var info in m_analysis.MetaData.Datasets)
            {
                var viewmodel = new DatasetInformationViewModel(info);
                Datasets.Add(viewmodel);
            }
        }

        private void ClearDatabaseDelegate()
        {
            IsDatabaseDms = false;
            IsDatabaseLocal = false;
            DatabaseFilePath = "";
            SelectedDatabaseServer = null;
            m_analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.None;
            OnPropertyChanged("RequiresDatabaseSelection");
        }

        private void SetDatabaseToDmsDelegate()
        {
            IsDatabaseDms = true;
        }

        private void SetDatabaseToLocalDelegate()
        {
            IsDatabaseLocal = true;
        }

        private void SetBaselineToDatabaseDelegate()
        {
            IsBaselineDatabase = true;
        }

        private void SetBaselineToDatasetDelegate()
        {
            IsBaselineDataset = true;
        }

        private void FindDmsDatabaseDelegate()
        {
            var dmsWindow = new DatabaseSearchToolWindow();
            var databaseView = new DatabasesViewModel();
            dmsWindow.DataContext = databaseView;
            dmsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var loader = MassTagDatabaseLoaderFactory.Create(MtdbDatabaseServerType.Dms);
            var databases = loader.LoadDatabases();

            foreach (var database in databases)
            {
                databaseView.AddDatabase(database);
            }

            if (SelectedDatabaseServer != null)
                databaseView.SelectedDatabase = SelectedDatabaseServer;

            var result = dmsWindow.ShowDialog();
            if (result == true)
            {
                if (databaseView.SelectedDatabase != null)
                {
                    SelectedDatabaseServer = databaseView.SelectedDatabase;
                    var database = SelectedDatabaseServer.Database;
                    database.DatabaseName = database.DatabaseName;
                    database.DatabaseServer = database.DatabaseServer;
                    m_analysis.MetaData.Database = database;
                    IsDatabaseDms = true;
                    OnPropertyChanged("RequiresDatabaseSelection");
                }
            }
        }

        #region Commands

        public ICommand SetBaselineToDataset { get; private set; }
        public ICommand SetBaselineToDatabase { get; private set; }
        public ICommand SetDatabaseToDms { get; private set; }
        public ICommand SetDatabaseToLocal { get; private set; }
        public ICommand FindLocalDatabase { get; private set; }
        public ICommand FindDmsDatabase { get; private set; }
        public ICommand ClearDatabase { get; private set; }

        #endregion
    }
}