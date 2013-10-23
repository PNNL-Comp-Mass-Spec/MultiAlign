using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MultiAlign.ViewModels;
using MultiAlign.Windows.Viewers.Databases;
using MultiAlignCore.Data;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.MTDB;
using MultiAlignCore.Data.MassTags;

//TODO:  Refactor this into a view model!

namespace MultiAlign.Windows.Wizard
{
    /// <summary>
    /// Interaction logic for SelectBaselinesAndOptions.xaml
    /// </summary>
    public partial class SelectBaselinesAndOptions : UserControl, INotifyPropertyChanged
    {
        private System.Windows.Forms.OpenFileDialog m_openFileDialog;

        DmsDatabaseServerViewModel m_selectedDmsDatabase;

        public SelectBaselinesAndOptions()
        {
            InitializeComponent();

            m_openFileDialog        = new System.Windows.Forms.OpenFileDialog();
            m_openFileDialog.Filter = "Mass Tag Database (.db3)|*.db3|APE Cache Database (.ape)|*.ape|All Files (*.*)|*.*";
            DataContext             = this;            
        }

        private void BrowseForFolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult result =  m_openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string extension = System.IO.Path.GetExtension(m_openFileDialog.FileName).ToLower();

                switch(extension)
                {
                    case ".ape":
                        if (Analysis.MetaData.Database == null)
                        {
                            Analysis.MetaData.Database = new InputDatabase(MassTagDatabaseFormat.APE);
                        }
                        Analysis.MetaData.Database.LocalPath = m_openFileDialog.FileName;
                        break;
                    case ".db3":
                        if (Analysis.MetaData.Database == null)
                        {
                            Analysis.MetaData.Database = new InputDatabase(MassTagDatabaseFormat.Sqlite);
                        }
                        Analysis.MetaData.Database.LocalPath = m_openFileDialog.FileName;
                        break;
                }
            }
        }

        public MultiAlignAnalysis Analysis
        {
            get { return (MultiAlignAnalysis)GetValue(AnalysisProperty); }
            set { SetValue(AnalysisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Analysis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnalysisProperty =
            DependencyProperty.Register("Analysis", typeof(MultiAlignAnalysis),
            typeof(SelectBaselinesAndOptions));

        //private MultiAlignAnalysis m_analysis;
        //public MultiAlignAnalysis  Analysis 
        //{
        //    get
        //    {
        //        return m_analysis;
        //    }
        //    set
        //    {
        //        if (m_analysis == value)
        //            return;

        //        m_analysis = value;                
        //        OnNotifyPropertyChanged("Analysis"); 
        //        OnNotifyPropertyChanged("IsDatdabaseDms
        //    }
        //}
                
        private void Button_Click(object sender, RoutedEventArgs e)
        {               
            Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = false;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetDatabaseAsBaseline();
        }
        private void SetDatabaseAsBaseline()
        {
            Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = true;
            Analysis.MetaData.BaselineDataset = null;
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

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
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SelectedDatabaseServer"));
                    }
                }                
            }
        }

        private void buttonFindPNNL_Click(object sender, RoutedEventArgs e)
        {
            DatabaseSearchToolWindow dmsWindow   = new DatabaseSearchToolWindow();
            DatabasesViewModel databaseView      = new DatabasesViewModel();                        
            dmsWindow.DataContext                = databaseView;          
            dmsWindow.Owner                      = Window.GetWindow(this);
            dmsWindow.Icon                       = dmsWindow.Owner.Icon;
            dmsWindow.WindowStartupLocation      = WindowStartupLocation.CenterOwner;            
            IDatabaseServerLoader loader         = MassTagDatabaseLoaderFactory.Create(MtdbDatabaseServerType.Dms);
            ICollection<InputDatabase> databases = loader.LoadDatabases();

            foreach (InputDatabase database in databases)
            {
                databaseView.AddDatabase(database);
            }

            bool? result =  dmsWindow.ShowDialog();
            if (result == true)
            {
                if (databaseView.SelectedDatabase != null)
                {
                    SelectedDatabaseServer      = databaseView.SelectedDatabase;
                    InputDatabase database      = SelectedDatabaseServer.Database;
                    database.DatabaseName       = database.DatabaseName;
                    database.DatabaseServer     = database.DatabaseServer;
                    Analysis.MetaData.Database  = database;


                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsDatabaseDms"));
                    }
                }
            }
        }

        private void ToggleButton_Checked_1(object sender, RoutedEventArgs e)
        {

            Analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.SQL;            
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Analysis.MetaData.Database.DatabaseFormat = MassTagDatabaseFormat.Sqlite;
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

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsDatabaseDms"));
                    }
                }
            }
        }
    }
}
