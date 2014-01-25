using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using System.Windows;

namespace MultiAlign.ViewModels.Wizard
{
    public class AnalysisBaselineSelectionViewModel : ViewModelBase 
    {

        private System.Windows.Forms.OpenFileDialog m_openFileDialog;
        DmsDatabaseServerViewModel m_selectedDmsDatabase;

        MultiAlignAnalysis m_analysis;

        public AnalysisBaselineSelectionViewModel(MultiAlignAnalysis analysis)
        {
            m_openFileDialog = new System.Windows.Forms.OpenFileDialog();
            m_openFileDialog.Filter = "Mass Tag Database (.db3)|*.db3|Direct Infusion IMS Database (.dims)|*.dims|APE Cache Database (.ape)|*.ape|All Files (*.*)|*.*";

            m_analysis = analysis;

        }

        #region Event Handlers

        private void ToggleButton_Checked_1(object sender, RoutedEventArgs e)
        {
            Analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.SQL;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.Sqlite;
        }

        private void buttonFindPNNL_Click(object sender, RoutedEventArgs e)
        {
            //DatabaseSearchToolWindow dmsWindow = new DatabaseSearchToolWindow();
            //DatabasesViewModel databaseView = new DatabasesViewModel();
            //dmsWindow.DataContext = databaseView;
            //dmsWindow.Owner = Window.GetWindow(this);
            //dmsWindow.Icon = dmsWindow.Owner.Icon;
            //dmsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //IDatabaseServerLoader loader = MassTagDatabaseLoaderFactory.Create(MtdbDatabaseServerType.Dms);
            //ICollection<InputDatabase> databases = loader.LoadDatabases();

            //foreach (InputDatabase database in databases)
            //{
            //    databaseView.AddDatabase(database);
            //}

            //bool? result = dmsWindow.ShowDialog();
            //if (result == true)
            //{
            //    if (databaseView.SelectedDatabase != null)
            //    {
            //        SelectedDatabaseServer = databaseView.SelectedDatabase;
            //        InputDatabase database = SelectedDatabaseServer.Database;
            //        database.DatabaseName = database.DatabaseName;
            //        database.DatabaseServer = database.DatabaseServer;
            //        Analysis.MetaData.Database = database;


            //        if (PropertyChanged != null)
            //        {
            //            PropertyChanged(this, new PropertyChangedEventArgs("IsDatabaseDms"));
            //        }
            //    }
            //}
        }

        private void BrowseForFolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult result = m_openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string extension = System.IO.Path.GetExtension(m_openFileDialog.FileName).ToLower();

                switch (extension)
                {
                    case ".dims":
                        Analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.DirectInfusionIms;
                        Analysis.MetaData.Database.LocalPath = m_openFileDialog.FileName;
                        break;
                    case ".ape":
                        Analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.APE;
                        Analysis.MetaData.Database.LocalPath = m_openFileDialog.FileName;
                        break;
                    case ".db3":
                        Analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.Sqlite;
                        Analysis.MetaData.Database.LocalPath = m_openFileDialog.FileName;
                        break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = false;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetDatabaseAsBaseline();
        }
        #endregion

        public MultiAlignAnalysis Analysis
        {
            get
            {
                return m_analysis;
            }
            set
            {
                if (value != m_analysis)
                {
                    m_analysis = value;
                    OnPropertyChanged("Analysis");
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

        private void SetDatabaseAsBaseline()
        {
            Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = true;
            Analysis.MetaData.BaselineDataset = null;
        }

        public bool IsDatabaseDms
        {
            get
            {
                if (Analysis == null)
                    return false;

                return (Analysis.MetaData.Database.DatabaseFormat == MassTagDatabaseFormat.SQL);
            }
            set
            {
                MassTagDatabaseFormat type = Analysis.MetaData.Database.DatabaseFormat;
                bool isDms = type == MassTagDatabaseFormat.SQL;
                if (isDms != value)
                {

                    if (value)
                        Analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.SQL;
                    else
                        Analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.Sqlite;

                    OnPropertyChanged("IsDatabaseDms");                    
                }
            }
        }
    }
}
