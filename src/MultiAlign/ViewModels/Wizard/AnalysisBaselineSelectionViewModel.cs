using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MultiAlign.Commands;
using MultiAlign.Windows.Viewers.Databases;
using MultiAlignCore.IO.MTDB;
using MultiAlignCore.IO.InputFiles;

namespace MultiAlign.ViewModels.Wizard
{
    public class AnalysisBaselineSelectionViewModel : ViewModelBase 
    {        
        DmsDatabaseServerViewModel          m_selectedDmsDatabase;
        MultiAlignAnalysis                  m_analysis;
        private DatasetInformationViewModel m_selectedDataset;

        public AnalysisBaselineSelectionViewModel(MultiAlignAnalysis analysis)
        {            
            string filter            = "Mass Tag Database (.db3)|*.db3|Direct Infusion IMS Database (.dims)|*.dims|APE Cache Database (.ape)|*.ape|All Files (*.*)|*.*";
            m_analysis               = analysis;
            
            IsDatabaseDms           = false;
            IsDatabaseLocal         = false;
            IsBaselineDataset       = true;

            SetDatabaseToDms        = new BaseCommandBridge(new CommandDelegate(SetDatabaseToDmsDelegate));
            SetDatabaseToLocal      = new BaseCommandBridge(new CommandDelegate(SetDatabaseToLocalDelegate));
            SetBaselineToDatabase   = new BaseCommandBridge(new CommandDelegate(SetBaselineToDatabaseDelegate));
            SetBaselineToDataset    = new BaseCommandBridge(new CommandDelegate(SetBaselineToDatasetDelegate));
            FindLocalDatabase = new BrowseFileCommand((string x) =>
            {
                DatabaseFilePath = x;
                IsDatabaseLocal  = true;
                OnPropertyChanged("RequiresDatabaseSelection");

            }, filter);
            FindDmsDatabase         = new BaseCommandBridge(new CommandDelegate(FindDmsDatabaseDelegate));
            ClearDatabase           = new BaseCommandBridge(new CommandDelegate(ClearDatabaseDelegate));
            Datasets                = new ObservableCollection<DatasetInformationViewModel>();
            UpdateDatasets();
        }
        public void UpdateDatasets()
        {
            Datasets.Clear();
            foreach(DatasetInformation info in m_analysis.MetaData.Datasets)
            {
                DatasetInformationViewModel viewmodel = new DatasetInformationViewModel(info);
                Datasets.Add(viewmodel);
            }
        }
        private void ClearDatabaseDelegate(object parameter)
        {
            IsDatabaseDms           = false;
            IsDatabaseLocal         = false;
            DatabaseFilePath        = "";
            SelectedDatabaseServer  = null;
            m_analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.None;
            OnPropertyChanged("RequiresDatabaseSelection");
        }
        private void SetDatabaseToDmsDelegate(object parameter)
        {
            IsDatabaseDms   = true;
        }
        private void SetDatabaseToLocalDelegate(object parameter)
        {
            IsDatabaseLocal = true;
        }
        private void SetBaselineToDatabaseDelegate(object parameter)
        {
            IsBaselineDatabase = true;
        }
        private void SetBaselineToDatasetDelegate(object parameter)
        {
            IsBaselineDataset = true;
        }
        private void FindDmsDatabaseDelegate(object parameter)
        {
            DatabaseSearchToolWindow dmsWindow   = new DatabaseSearchToolWindow();
            DatabasesViewModel databaseView      = new DatabasesViewModel();
            dmsWindow.DataContext                = databaseView;
            dmsWindow.WindowStartupLocation      = WindowStartupLocation.CenterOwner;
            IDatabaseServerLoader loader         = MassTagDatabaseLoaderFactory.Create(MtdbDatabaseServerType.Dms);
            ICollection<InputDatabase> databases = loader.LoadDatabases();

            foreach (InputDatabase database in databases)
            {
                databaseView.AddDatabase(database);
            }

            if (SelectedDatabaseServer != null)
                databaseView.SelectedDatabase = SelectedDatabaseServer;

            bool? result = dmsWindow.ShowDialog();
            if (result == true)
            {
                if (databaseView.SelectedDatabase != null)
                {
                    SelectedDatabaseServer          = databaseView.SelectedDatabase;
                    InputDatabase database          = SelectedDatabaseServer.Database;
                    database.DatabaseName           = database.DatabaseName;
                    database.DatabaseServer         = database.DatabaseServer;
                    m_analysis.MetaData.Database    = database;
                    IsDatabaseDms                   = true;
                    OnPropertyChanged("RequiresDatabaseSelection");
                }
            }
        }

        
        #region Commands 
        public ICommand SetBaselineToDataset  { get; private set; }
        public ICommand SetBaselineToDatabase { get; private set; }
        public ICommand SetDatabaseToDms      { get; private set; }
        public ICommand SetDatabaseToLocal    { get; private set; }
        public ICommand FindLocalDatabase     { get; private set; }
        public ICommand FindDmsDatabase       { get; private set; }
        public ICommand ClearDatabase       { get; private set; }
        #endregion

        public string DatabaseFilePath
        {
            get
            {
                return m_analysis.MetaData.Database.LocalPath;
            }
            set
            {
                if (m_analysis.MetaData.Database.LocalPath != value)
                {
                    m_analysis.MetaData.Database.DatabaseFormat = InputDatabase.DetermineFormat(value);
                    m_analysis.MetaData.Database.LocalPath      = value;
                    OnPropertyChanged("DatabaseFilePath");
                }
            }
        }
        public bool IsBaselineDataset
        {

            get
            {                
                return IsBaselineDatabase == false;
            }

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

            get
            {                
                return m_analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB;
            }

            set
            {
                if (m_analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB != value)
                {
                    m_analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = value;
                    OnPropertyChanged("RequiresDatabaseSelection");

                    OnPropertyChanged("IsBaselineDatabase");
                    OnPropertyChanged("IsBaselineDataset");
                }
            }
        }
        public DmsDatabaseServerViewModel SelectedDatabaseServer
        {
            get
            {
                return m_selectedDmsDatabase;
            }
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
        public ObservableCollection<DatasetInformationViewModel> Datasets
        {   
            get; 
            private set; 
        }
        /// <summary>
        /// Gets or sets the selected dataset for baseline.
        /// </summary>
        public DatasetInformationViewModel SelectedDataset 
        {
            get
            {
                return m_selectedDataset;
            }
            set
            {
                if (m_selectedDataset != value)
                {                    
                    m_analysis.MetaData.BaselineDataset = value.Dataset;
                    m_selectedDataset                   = value;
                    OnPropertyChanged("SelectedDataset");
                }
            }
        }
        /// <summary>
        /// Gets or sets whether a database is local
        /// </summary>
        public bool IsDatabaseLocal
        {
            get
            {                
                return (m_analysis.MetaData.Database.DatabaseFormat != MassTagDatabaseFormat.SQL);
            }
            set
            {                
                MassTagDatabaseFormat format = m_analysis.MetaData.Database.DatabaseFormat;

                bool isLocal = (format != MassTagDatabaseFormat.SQL && format != MassTagDatabaseFormat.None);
                if (isLocal != value)
                {
                    // If we are being told not to set it to be local...we change...
                    if (!value)
                        m_analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.SQL;
                    else
                        m_analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.Sqlite;

                    OnPropertyChanged("IsDatabaseLocal");
                    OnPropertyChanged("IsDatabaseLocal");
                }
            }
        }
        /// <summary>
        /// Gets or sets whether a database is dms 
        /// </summary>
        public bool IsDatabaseDms
        {
            get
            {             
                return (m_analysis.MetaData.Database.DatabaseFormat == MassTagDatabaseFormat.SQL);
            }
            set
            {                
                bool isDms                  = (m_analysis.MetaData.Database.DatabaseFormat == MassTagDatabaseFormat.SQL);
                if (isDms != value)
                {
                    // If we are being told to set it to be local...we change...
                    if (value)
                        m_analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.SQL;
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
    }
}
