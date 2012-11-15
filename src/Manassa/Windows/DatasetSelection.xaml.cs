﻿using System;
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

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for DatasetSelection.xaml
    /// </summary>
    public partial class DatasetSelection : System.Windows.Controls.UserControl
    {
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

            Datasets                     = new ObservableCollection<DatasetInformation>();
            m_inputFileFilter            = "Input Files (*.txt)| *.txt| All Files (*.*)|*.*";
            m_featureFileFilter          = DatasetFilterFactory.BuildFileFilters(MultiAlignCore.IO.InputFiles.InputFileType.Features);
            m_openFileDialog             = new OpenFileDialog();
            m_openFileDialog.Filter      = m_inputFileFilter;
            DataContext                  = this;
            ShouldSearchSubDirectories   = false;
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



        public bool ShouldSearchSubDirectories
        {
            get { return (bool)GetValue(ShouldSearchSubDirectoriesProperty); }
            set { SetValue(ShouldSearchSubDirectoriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShouldSearchSubDirectories.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShouldSearchSubDirectoriesProperty =
            DependencyProperty.Register("ShouldSearchSubDirectories", typeof(bool), typeof(DatasetSelection));




        public ObservableCollection<DatasetInformation> Datasets
        {
            get { return (ObservableCollection<DatasetInformation>)GetValue(InputFilesProperty); }
            set { SetValue(InputFilesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputFiles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputFilesProperty =
            DependencyProperty.Register("Datasets", typeof(ObservableCollection<DatasetInformation>), typeof(DatasetSelection));



        private void AddInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            bool fileExists = System.IO.File.Exists(SingleFilePath);
            if (fileExists)
            {
                // Read input files
                InputAnalysisInfo info              = MultiAlignFileInputReader.ReadInputFile(SingleFilePath); 
                List<DatasetInformation> datasets   = DatasetInformation.CreateDatasetsFromInputFile(info.Files);
                foreach (DatasetInformation dataset in datasets)
                {
                    Datasets.Add(dataset);
                }
            }
        }

        private void BrowseForInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            m_openFileDialog.Filter = m_inputFileFilter;
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
                    return;
                }
                if (type == InputFileType.Scans)
                {
                    return;
                }

                InputFile file  = new InputFile();
                file.Path       = SingleFilePath;
                file.FileType   = type;

                List<InputFile> inputs = new List<InputFile>();
                inputs.Add(file);

                List<DatasetInformation> info = DatasetInformation.CreateDatasetsFromInputFile(inputs);
                foreach (DatasetInformation dataset in info)
                {
                    Datasets.Add(dataset);
                }
            }
        }

        private void BrowseSingleFile_Click(object sender, RoutedEventArgs e)
        {
            m_openFileDialog.Filter = m_featureFileFilter;
            DialogResult result = m_openFileDialog.ShowDialog();
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
                // Find datasets
            }
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {

            List<string> extensions = new List<string>() { "*_isos.csv", "*LCMSFeatures.txt", "*.syn", "*.raw", "*.mzxml" };

            System.IO.SearchOption option = System.IO.SearchOption.TopDirectoryOnly;

            if (ShouldSearchSubDirectories)
            {
                option = System.IO.SearchOption.AllDirectories;
            }

            List<InputFile> files = DatasetSearcher.FindDatasets(m_folderBrowser.SelectedPath,
                                        extensions,
                                        option);

            List<DatasetInformation> datasets = DatasetInformation.CreateDatasetsFromInputFile(files);
            foreach (DatasetInformation dataset in datasets)
            {
                Datasets.Add(dataset);
            }
        }

        private void RemoveSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in MainDatasetGrid.SelectedItems)
            {
                DatasetInformation info = o as DatasetInformation;
                Datasets.Remove(info);
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

        private void LoadFromPreviousButton_Click(object sender, RoutedEventArgs e)
        {

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
}
