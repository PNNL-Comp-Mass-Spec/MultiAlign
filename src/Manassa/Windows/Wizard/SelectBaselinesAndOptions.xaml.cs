using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MultiAlignCore.Data;

using MultiAlignParameterFileEditor;

namespace MultiAlign.Windows.Wizard
{
    /// <summary>
    /// Interaction logic for SelectBaselinesAndOptions.xaml
    /// </summary>
    public partial class SelectBaselinesAndOptions : UserControl, INotifyPropertyChanged
    {
        private System.Windows.Forms.OpenFileDialog m_openFileDialog;        
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
                        Analysis.Options.MassTagDatabaseOptions.DatabaseType     = MultiAlignCore.IO.MTDB.MassTagDatabaseType.APE;
                        Analysis.Options.MassTagDatabaseOptions.DatabaseFilePath = m_openFileDialog.FileName;
                        break;
                    case ".db3":                        
                        Analysis.Options.MassTagDatabaseOptions.DatabaseType     = MultiAlignCore.IO.MTDB.MassTagDatabaseType.SQLite;
                        Analysis.Options.MassTagDatabaseOptions.DatabaseFilePath = m_openFileDialog.FileName;
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

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {                        
            //Analysis.Options.MassTagDatabaseOptions.DatabaseType = MultiAlignCore.IO.MTDB.MassTagDatabaseType.SQLite;
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

    }
}
