using MultiAlign.Commands;
using MultiAlign.Commands.Datasets;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.InputFiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace MultiAlign.ViewModels.Wizard
{
    public class AnalysisDatasetSelectionViewModel: ViewModelBase 
    {

        private readonly OpenFileDialog m_openFileDialog;
        DmsDatabaseServerViewModel m_selectedDmsDatabase;

        MultiAlignAnalysis m_analysis;
        private InputAnalysisInfo m_analysisInput;
        private string m_folderPath;
        private Dictionary<InputFileType, string> m_filterMap = new Dictionary<InputFileType, string>();
        private readonly string m_inputFileFilter;
        private readonly string m_featureFileFilter;
        private string m_singleFilePath;
        private string m_inputFilePath;
        private SearchOption m_shouldSearchSubdirectories;

        public AnalysisDatasetSelectionViewModel(MultiAlignAnalysis analysis)
        {
            m_inputFileFilter            = "Input Files (*.txt)| *.txt| All Files (*.*)|*.*";
            m_featureFileFilter          = DatasetFilterFactory.BuildFileFilters(InputFileType.Features);
            m_openFileDialog             = new OpenFileDialog {Filter = m_inputFileFilter};
            m_analysis                   = analysis;
            ShouldSearchSubDirectories   = SearchOption.TopDirectoryOnly;


            // Create The Dataset View Model for Binding
            Datasets = new ObservableCollection<DatasetInformationViewModel>();
            foreach (var information in analysis.MetaData.Datasets)
            {
                var info = new DatasetInformationViewModel(information);
                info.Selected                   += info_Selected;
                Datasets.Add(info);
            }
            

            // Route the events here...
            AddFolderCommand        = new BaseCommandBridge(AddFolderDelegate);
            AddInputFileCommand     = new BaseCommandBridge(AddInputFileDelegate);
            AddSingleFileCommand    = new BaseCommandBridge(AddSingleFileDelegate);
            
            BrowseSingleFileCommand = new BaseCommandBridge(BrowseSingleFile);
            BrowseInputFileCommand  = new BaseCommandBridge(BrowseInput);
            BrowseFolderCommand     = new BrowseFolderCommand(x => { FolderPath = x; });

            RemoveSelectedCommand   = new BaseCommandBridge(RemoveSelected);
            SelectAllCommand        = new BaseCommandBridge(SelectAllDelegate);
            SelectNoneCommand       = new BaseCommandBridge(SelectNoneDelegate);

            SelectedDatasets = new ObservableCollection<DatasetInformationViewModel>();
        }

        public int SelectedCount
        {
            get
            {
                return Datasets.Count(x => x.IsSelected);
            }
        }

        void info_Selected(object sender, EventArgs e)
        {
            OnPropertyChanged("SelectedCount");
        }


        public ObservableCollection<DatasetInformationViewModel> SelectedDatasets
        {
            get;
            private set;
        }

        #region Event Handlers
        /// <summary>
        /// Selects no items
        /// </summary>
        /// <param name="parameter"></param>
        private void SelectAllDelegate(object parameter)
        {
            foreach (var dataset in Datasets)
                dataset.IsSelected = true;
        }
        /// <summary>
        /// Selects all items
        /// </summary>
        /// <param name="parameter"></param>
        private void SelectNoneDelegate(object parameter)
        {
            foreach (var dataset in Datasets)
                dataset.IsSelected = false;
        }
        /// <summary>
        /// Removes all selected items.
        /// </summary>
        /// <param name="parameter"></param>
        private void RemoveSelected(object parameter)
        {
            var datasets = Datasets.Where(dataset => dataset.IsSelected).ToList();


            foreach (var dataset in datasets)
            {
                Datasets.Remove(dataset);
                Analysis.MetaData.Datasets.Remove(dataset.Dataset);
            }

            int id = 0;
            foreach (var info in Datasets)
            {
                info.DatasetId = id++;
            }

            OnPropertyChanged("SelectedCount");
        }
        private void BrowseSingleFile(object parameter)
        {
            
            m_openFileDialog.Filter = m_featureFileFilter;
            m_openFileDialog.FileName = SingleFilePath;
            DialogResult result = m_openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SingleFilePath = m_openFileDialog.FileName;
            }
        }
        private void BrowseInput(object parameter)
        {

            m_openFileDialog.Filter = m_inputFileFilter;
            m_openFileDialog.FileName = InputFilePath;
            DialogResult result = m_openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                InputFilePath = m_openFileDialog.FileName;
            }
        }                    
        /// <summary>
        /// Adds the  folder containing dataset specific elements
        /// </summary>
        /// <param name="parameter"></param>
        private void AddFolderDelegate(object parameter)
        {
            var supportedTypes = DatasetInformation.SupportedFileTypes;
            var extensions = new List<string>();

            supportedTypes.ForEach(x => extensions.Add("*" + x.Extension));

            var option = ShouldSearchSubDirectories;
            if (FolderPath == null)
            {
                ApplicationStatusMediator.SetStatus("The directory specified does not exist.");
                return;
            }

            if (!Directory.Exists(FolderPath))
            {
                ApplicationStatusMediator.SetStatus("The directory specified does not exist.");
                return;
            }

            var files = DatasetSearcher.FindDatasets(FolderPath,
                                        extensions,
                                        option);
            AddDatasets(files);
        }
        /// <summary>
        /// Adds a MultiAlign file
        /// </summary>
        /// <param name="parameter"></param>
        private void AddInputFileDelegate(object parameter)
        {
            bool fileExists = File.Exists(InputFilePath);
            if (fileExists)
            {
                // Read input files
                try
                {
                    var info = MultiAlignFileInputReader.ReadInputFile(InputFilePath);
                    AddDatasets(info.Files);
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
        /// <summary>
        /// Adds datasets from a single file
        /// </summary>
        /// <param name="parameter"></param>
        private void AddSingleFileDelegate(object parameter)
        {
            bool fileExists = File.Exists(SingleFilePath);

            if (fileExists)
            {

                InputFileType type = DatasetInformation.GetInputFileType(SingleFilePath);

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

                var file  = new InputFile {Path = SingleFilePath, FileType = type};

                var inputs = new List<InputFile> {file};

                AddDatasets(inputs);
            }
            else
            {

                ApplicationStatusMediator.SetStatus("The input file does not exist.");
            }
        }
        #endregion
        
        #region View Model Properties
        /// <summary>
        /// Gets or sets the analysis information
        /// </summary>
        public InputAnalysisInfo AnalysisInputInformation
        {
            get
            {
                return m_analysisInput;
            }
            set
            {
                if (m_analysisInput != value)
                {
                    m_analysisInput = value;
                    OnPropertyChanged("AnalysisInputInformation");
                }
            }
        }
        /// <summary>
        /// Gets or sets the input folder path
        /// </summary>
        public string FolderPath
        {
            get
            {
                return m_folderPath;
            }
            set
            {
                if (m_folderPath != value)
                {
                    m_folderPath = value;
                    OnPropertyChanged("FolderPath");
                }
            }
        }
        /// <summary>
        /// Gets or sets the single file path
        /// </summary>
        public string SingleFilePath
        {
            get
            {
                return m_singleFilePath;
            }
            set
            {
                if (m_singleFilePath != value)
                {
                    m_singleFilePath = value;
                    OnPropertyChanged("SingleFilePath");
                }
            }
        }
        /// <summary>
        /// Gets or sets the single file path
        /// </summary>
        public string InputFilePath
        {
            get
            {
                return m_inputFilePath;
            }
            set
            {
                if (m_inputFilePath != value)
                {
                    m_inputFilePath = value;
                    OnPropertyChanged("InputFilePath");
                }
            }
        }
        /// <summary>
        /// Gets or sets the analysis
        /// </summary>
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
        /// <summary>
        /// Gets or sets how the sub directories should be searched.
        /// </summary>
        public SearchOption ShouldSearchSubDirectories
        {
            get
            {
                return m_shouldSearchSubdirectories;
            }
            set
            {
                if (value != m_shouldSearchSubdirectories)
                {
                    m_shouldSearchSubdirectories = value;
                    OnPropertyChanged("ShouldSearchSubDirectories");
                }
            }
        }
        /// <summary>
        /// Gets the set of datasets 
        /// </summary>
        public ObservableCollection<DatasetInformationViewModel> Datasets
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the selected database server.
        /// </summary>
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
        #endregion

        #region Commands
        public ICommand BrowseInputFileCommand { get; set; }
        public ICommand BrowseFolderCommand { get; set; }
        public ICommand BrowseSingleFileCommand { get; set; }
        public ICommand AddFolderCommand { get; set; }
        public ICommand AddInputFileCommand { get; set; }
        public ICommand AddSingleFileCommand { get; set; }
        public ICommand RemoveSelectedCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }
        public ICommand SelectNoneCommand { get; set; }
        public ICommand ModifyDatasetCommand { get; set; }
        #endregion

        /// <summary>
        /// Adds the list of input files into the analysis configuration.
        /// </summary>
        private void AddDatasets(List<InputFile> information)
        {
            List<DatasetInformation> datasets = Analysis.MetaData.AddInputFiles(information);
            foreach (DatasetInformation info in datasets)
            {
                var infoViewModel = new DatasetInformationViewModel(info);
                infoViewModel.Selected += info_Selected;
                Datasets.Add(infoViewModel);
            }
        }
    }
}
