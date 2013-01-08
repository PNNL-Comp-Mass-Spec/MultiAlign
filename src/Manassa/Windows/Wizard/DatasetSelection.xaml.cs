using System;
using MultiAlignCore.IO.InputFiles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using Manassa.Data;
using Manassa.IO;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Data;
using System.ComponentModel;
using System.IO;
using Manassa.Windows.Viewers.Datasets;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for DatasetSelection.xaml
    /// </summary>
    public partial class DatasetSelection : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        public event EventHandler<StatusEventArgs> Status;

        private FolderBrowserDialog m_folderBrowser;
        private OpenFileDialog m_openFileDialog;
        private Dictionary<InputFileType, string> m_filterMap = new Dictionary<InputFileType, string>();
        private string m_inputFileFilter;
        private string m_featureFileFilter;

        public DatasetSelection()
        {
            InitializeComponent();

            m_folderBrowser              = new FolderBrowserDialog();
            m_folderBrowser.SelectedPath = Environment.SpecialFolder.Desktop.ToString();
            
            m_inputFileFilter            = "Input Files (*.txt)| *.txt| All Files (*.*)|*.*";
            m_featureFileFilter          = DatasetFilterFactory.BuildFileFilters(MultiAlignCore.IO.InputFiles.InputFileType.Features);
            m_openFileDialog             = new OpenFileDialog();
            m_openFileDialog.Filter      = m_inputFileFilter;
            DataContext                  = this;
            ShouldSearchSubDirectories   = SearchOption.TopDirectoryOnly;;
        }

        public InputAnalysisInfo AnalysisInputInformation
        {
            get { return (InputAnalysisInfo)GetValue(AnalysisInputInformationProperty); }
            set { SetValue(AnalysisInputInformationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnalysisInputInformation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnalysisInputInformationProperty =
            DependencyProperty.Register("AnalysisInputInformation", typeof(InputAnalysisInfo),
            typeof(DatasetSelection));



        public string FolderPath
        {
            get { return (string)GetValue(FolderPathProperty); }
            set { SetValue(FolderPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FolderPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderPathProperty =
            DependencyProperty.Register("FolderPath", typeof(string), typeof(DatasetSelection));



        public string SingleFilePath
        {
            get { return (string)GetValue(SingleFilePathProperty); }
            set { SetValue(SingleFilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SingleFilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SingleFilePathProperty =
            DependencyProperty.Register("SingleFilePath", typeof(string), typeof(DatasetSelection));



        public string InputFilePath
        {
            get { return (string)GetValue(InputFilePathProperty); }
            set { SetValue(InputFilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FolderPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputFilePathProperty  =
            DependencyProperty.Register("InputFilePath", typeof(string), typeof(DatasetSelection));



        public SearchOption ShouldSearchSubDirectories
        {
            get { return (SearchOption)GetValue(ShouldSearchSubDirectoriesProperty); }
            set { SetValue(ShouldSearchSubDirectoriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShouldSearchSubDirectories.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShouldSearchSubDirectoriesProperty =
            DependencyProperty.Register("ShouldSearchSubDirectories", typeof(SearchOption), typeof(DatasetSelection));


        public MultiAlignAnalysis Analysis
        {
            get { return (MultiAlignAnalysis)GetValue(AnalysisProperty); }
            set { SetValue(AnalysisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Analysis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnalysisProperty =
            DependencyProperty.Register("Analysis", typeof(MultiAlignAnalysis), typeof(DatasetSelection));

        
        private void AddInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            bool fileExists = System.IO.File.Exists(InputFilePath);
            if (fileExists)
            {
                // Read input files
                try
                {
                    InputAnalysisInfo info = MultiAlignFileInputReader.ReadInputFile(InputFilePath);                    
                    UpdateDatasets(info.Files);
                }
                catch
                {
                    ApplicationStatusMediator.SetStatus("Could not read the input file.  Check the file format.");
                }
            }
            else
            {
                ApplicationStatusMediator.SetStatus("The input file does not exist.");
            }
        }

        private void BrowseForInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            m_openFileDialog.Filter = m_inputFileFilter;
            m_openFileDialog.FileName = InputFilePath;
            DialogResult result     = m_openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                InputFilePath = m_openFileDialog.FileName;
            }
        }

        private void InputSingleFile_Click(object sender, RoutedEventArgs e)
        {
            bool fileExists = System.IO.File.Exists(SingleFilePath);

            if (fileExists)
            {

                InputFileType type = DatasetFilterFactory.DetermineInputFileType(SingleFilePath);

                if (type == InputFileType.NotRecognized)
                {
                    ApplicationStatusMediator.SetStatus("The input file was not recognized.");
                    return;
                }
                if (type == InputFileType.Scans)
                {
                    ApplicationStatusMediator.SetStatus("The input file was not recognized.");
                    return;
                }

                InputFile file = new InputFile();
                file.Path = SingleFilePath;
                file.FileType = type;

                List<InputFile> inputs = new List<InputFile>();
                inputs.Add(file);

                UpdateDatasets(inputs);
            }
            else
            {

                ApplicationStatusMediator.SetStatus("The input file does not exist.");
            }
        }

        private void UpdateDatasets(List<InputFile> info)
        {
            List<DatasetInformation> added = Analysis.MetaData.AddInputFiles(info);            
            
        }

        private void BrowseSingleFile_Click(object sender, RoutedEventArgs e)
        {
            m_openFileDialog.Filter     = m_featureFileFilter;
            m_openFileDialog.FileName   = SingleFilePath;
            DialogResult result         = m_openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SingleFilePath = m_openFileDialog.FileName;
            }
        }

        private void BrowseForFolderButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult result =  m_folderBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                FolderPath = m_folderBrowser.SelectedPath;                
            }
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {

            List<string> extensions = new List<string>() { "*_isos.csv", "*LCMSFeatures.txt", "*.syn", "*.raw", "*.mzxml", "*peaks.txt" };

            System.IO.SearchOption option = ShouldSearchSubDirectories;

            
            if (FolderPath == null)
            {
                ApplicationStatusMediator.SetStatus("The directory specified does not exist.");                
                return;
            }

            if (!System.IO.Directory.Exists(FolderPath))
            {
                ApplicationStatusMediator.SetStatus("The directory specified does not exist.");
                return;
            }

            List<InputFile> files = DatasetSearcher.FindDatasets(FolderPath,
                                        extensions,
                                        option);            
            UpdateDatasets(files);
        }

        private void RemoveSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            List<DatasetInformation> datasets = new List<DatasetInformation>();
            
            foreach (object o in MainDatasetGrid.SelectedItems)
            {
                DatasetInformation info = o as DatasetInformation;
                datasets.Add(info);                
            }            
            
            datasets.ForEach(x => Analysis.MetaData.Datasets.Remove(x));

            int id = 0;
            foreach (DatasetInformation info in Analysis.MetaData.Datasets)
            {
                info.DatasetId = id++;
            }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {

            
            MainDatasetGrid.SelectAll();
        }

        private void SelectNoneButton_Click(object sender, RoutedEventArgs e)
        {
            MainDatasetGrid.SelectedIndex = -1;
        }

        private void ToggleSelection()
        {
            foreach (DataGridRow row in MainDatasetGrid.Items)
            {
                row.IsSelected = !row.IsSelected;
            }
        }

        private void LoadFromPreviousButton_Click(object sender, RoutedEventArgs e)
        {

        }


        private void OnNotify(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleSelection();
        }

        private void DatasetDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            DatasetInformation dataset = ((System.Windows.Controls.Button)sender).CommandParameter as DatasetInformation;
            if (dataset != null)
            {
                Window newWindow                = new Window();
                newWindow.Width                 = 650;
                newWindow.Height                = 350;
                DatasetInputFileEditor viewer   = new DatasetInputFileEditor();
                viewer.Dataset                  = dataset;
                newWindow.Content               = viewer;
                newWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                newWindow.ShowDialog();
            }
        }
    }

    /// <summary>
    /// Gets the test class stuff
    /// </summary>
    public class TestClass
    {
        public string DatasetName
        {
            get;
            set;
        }
        public int ID
        {
            get;
            set;
        }
    }

    public class StatusEventArgs: EventArgs
    {
        public StatusEventArgs(string message)
        {
            Message = message;
        }

        public string Message
        {
            get;
            private set;
        }
    }
}
