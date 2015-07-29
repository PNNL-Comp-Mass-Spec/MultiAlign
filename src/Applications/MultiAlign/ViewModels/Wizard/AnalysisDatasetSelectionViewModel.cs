using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;
using MultiAlign.Commands;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlign.ViewModels.Databases;
using MultiAlign.ViewModels.Datasets;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.InputFiles;

namespace MultiAlign.ViewModels.Wizard
{
    public class AnalysisDatasetSelectionViewModel : ViewModelBase
    {
        private readonly string m_featureFileFilter;
        private readonly string m_inputFileFilter;
        private readonly OpenFileDialog m_openFileDialog;

        private MultiAlignAnalysis m_analysis;
        private InputAnalysisInfo m_analysisInput;
        private Dictionary<InputFileType, string> m_filterMap = new Dictionary<InputFileType, string>();
        private string m_dataFolderPath;
        private string m_inputFilePath;
        private DmsDatabaseServerViewModel m_selectedDmsDatabase;
        private SearchOption m_shouldSearchSubdirectories;
        private string m_singleFilePath;

        public AnalysisDatasetSelectionViewModel(MultiAlignAnalysis analysis)
        {
            m_inputFileFilter = "Input Files (*.txt)| *.txt| All Files (*.*)|*.*";
            m_featureFileFilter = DatasetFilterFactory.BuildFileFilters(InputFileType.Features);
            m_openFileDialog = new OpenFileDialog {Filter = m_inputFileFilter};
            m_analysis = analysis;
            ShouldSearchSubDirectories = SearchOption.TopDirectoryOnly;


            // Create The Dataset View Model for Binding
            Datasets = new ObservableCollection<DatasetInformationViewModel>();
            foreach (var information in analysis.MetaData.Datasets)
            {
                var info = new DatasetInformationViewModel(information);
                info.Selected += info_Selected;
                Datasets.Add(info);
            }


            // Route the events here...
            AddFolderCommand = new BaseCommand(AddFolderDelegate, BaseCommand.AlwaysPass);
            AddInputFileCommand = new BaseCommand(AddInputFileDelegate, BaseCommand.AlwaysPass);
            AddSingleFileCommand = new BaseCommand(AddSingleFileDelegate, BaseCommand.AlwaysPass);

            BrowseSingleFileCommand = new BaseCommand(BrowseSingleFile, BaseCommand.AlwaysPass);
            BrowseInputFileCommand = new BaseCommand(BrowseInput, BaseCommand.AlwaysPass);
            BrowseDataFolderCommand = new BrowseFolderCommand(x => { DataFolderPath = x; });

            RemoveSelectedCommand = new BaseCommand(RemoveSelected, BaseCommand.AlwaysPass);
            SelectAllCommand = new BaseCommand(SelectAllDelegate, BaseCommand.AlwaysPass);
            SelectNoneCommand = new BaseCommand(SelectNoneDelegate, BaseCommand.AlwaysPass);

            SelectedDatasets = new ObservableCollection<DatasetInformationViewModel>();
        }

        public int SelectedCount
        {
            get { return Datasets.Count(x => x.IsSelected); }
        }


        public ObservableCollection<DatasetInformationViewModel> SelectedDatasets { get; private set; }

        #region Event Handlers

        /// <summary>
        ///     Selects no items
        /// </summary>
        private void SelectAllDelegate()
        {
            foreach (var dataset in Datasets)
                dataset.IsSelected = true;
        }

        /// <summary>
        ///     Selects all items
        /// </summary>
        private void SelectNoneDelegate()
        {
            foreach (var dataset in Datasets)
                dataset.IsSelected = false;
        }

        /// <summary>
        ///     Removes all selected items.
        /// </summary>
        private void RemoveSelected()
        {
            var datasets = Datasets.Where(dataset => dataset.IsSelected).ToList();


            foreach (var dataset in datasets)
            {
                Datasets.Remove(dataset);
                Analysis.MetaData.Datasets.Remove(dataset.Dataset);
            }

            var id = 0;
            foreach (var info in Datasets)
            {
                info.DatasetId = id++;
            }

            OnPropertyChanged("SelectedCount");
        }

        private void BrowseSingleFile()
        {
            m_openFileDialog.Filter = m_featureFileFilter;
            m_openFileDialog.FileName = SingleFilePath;
            var result = m_openFileDialog.ShowDialog();
            if (result != null && result.Value)
            {
                SingleFilePath = m_openFileDialog.FileName;
            }
        }

        private void BrowseInput()
        {
            m_openFileDialog.Filter = m_inputFileFilter;
            m_openFileDialog.FileName = InputFilePath;
            var result = m_openFileDialog.ShowDialog();
            if (result != null && result.Value)
            {
                InputFilePath = m_openFileDialog.FileName;
            }
        }

        /// <summary>
        ///     Adds the  folder containing dataset specific elements
        /// </summary>
        public void AddFolderDelegate()
        {
            var supportedTypes = DatasetInformation.SupportedFileTypes;
            var extensions = new List<string>();

            supportedTypes.ForEach(x => extensions.Add("*" + x.Extension));

            var option = ShouldSearchSubDirectories;
            if (DataFolderPath == null)
            {
                ApplicationStatusMediator.SetStatus("The directory specified does not exist.");
                return;
            }

            if (!Directory.Exists(DataFolderPath))
            {
                ApplicationStatusMediator.SetStatus("The directory specified does not exist.");
                return;
            }

            var files = DatasetSearcher.FindDatasets(DataFolderPath,
                extensions,
                option);
            AddDatasets(files);
        }

        /// <summary>
        ///     Adds a MultiAlign file
        /// </summary>
        private void AddInputFileDelegate()
        {
            var fileExists = File.Exists(InputFilePath);
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
        ///     Adds datasets from a single file
        /// </summary>
        private void AddSingleFileDelegate()
        {
            var fileExists = File.Exists(SingleFilePath);

            if (fileExists)
            {
                var type = DatasetInformation.GetInputFileType(SingleFilePath);

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

                var file = new InputFile {Path = SingleFilePath, FileType = type};

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
        ///     Gets or sets the analysis information
        /// </summary>
        public InputAnalysisInfo AnalysisInputInformation
        {
            get { return m_analysisInput; }
            set
            {
                if (m_analysisInput == value)
                    return;

                m_analysisInput = value;
                OnPropertyChanged("AnalysisInputInformation");
            }
        }

        /// <summary>
        ///     Gets or sets the input folder path
        /// </summary>
        public string DataFolderPath
        {
            get { return m_dataFolderPath; }
            set
            {
                if (m_dataFolderPath == value)
                    return;

                m_dataFolderPath = value;
                OnPropertyChanged("DataFolderPath");
            }
        }

        /// <summary>
        ///     Gets or sets the single file path
        /// </summary>
        public string SingleFilePath
        {
            get { return m_singleFilePath; }
            set
            {
                if (m_singleFilePath == value)
                    return;

                m_singleFilePath = value;
                OnPropertyChanged("SingleFilePath");
            }
        }

        /// <summary>
        ///     Gets or sets the single file path
        /// </summary>
        public string InputFilePath
        {
            get { return m_inputFilePath; }
            set
            {
                if (m_inputFilePath == value)
                    return;

                m_inputFilePath = value;
                OnPropertyChanged("InputFilePath");
            }
        }

        /// <summary>
        ///     Gets or sets the analysis
        /// </summary>
        public MultiAlignAnalysis Analysis
        {
            get { return m_analysis; }
            set
            {
                if (value == m_analysis)
                    return;

                m_analysis = value;
                OnPropertyChanged("Analysis");
            }
        }

        /// <summary>
        ///     Gets or sets how the sub directories should be searched.
        /// </summary>
        public SearchOption ShouldSearchSubDirectories
        {
            get { return m_shouldSearchSubdirectories; }
            set
            {
                if (value == m_shouldSearchSubdirectories)
                    return;

                m_shouldSearchSubdirectories = value;
                OnPropertyChanged("ShouldSearchSubDirectories");
            }
        }

        /// <summary>
        ///     Gets the set of datasets
        /// </summary>
        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set; }

        /// <summary>
        ///     Gets or sets the selected database server.
        /// </summary>
        public DmsDatabaseServerViewModel SelectedDatabaseServer
        {
            get { return m_selectedDmsDatabase; }
            set
            {
                if (m_selectedDmsDatabase == value)
                    return;

                m_selectedDmsDatabase = value;
                OnPropertyChanged("SelectedDatabaseServer");
            }
        }

        #endregion

        #region Commands

        public ICommand BrowseInputFileCommand { get; set; }
        public ICommand BrowseDataFolderCommand { get; set; }
        public ICommand BrowseSingleFileCommand { get; set; }
        public ICommand AddFolderCommand { get; set; }
        public ICommand AddInputFileCommand { get; set; }
        public ICommand AddSingleFileCommand { get; set; }
        public ICommand RemoveSelectedCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }
        public ICommand SelectNoneCommand { get; set; }
        public ICommand ModifyDatasetCommand { get; set; }

        #endregion

        private void info_Selected(object sender, EventArgs e)
        {
            OnPropertyChanged("SelectedCount");
        }

        /// <summary>
        ///     Adds the list of input files into the analysis configuration.
        /// </summary>
        public void AddDatasets(List<InputFile> information)
        {
            var datasets = Analysis.MetaData.AddInputFiles(information);
            foreach (
                var infoViewModel in
                    datasets.Select(info => new DatasetInformationViewModel(info)))
            {
                infoViewModel.Selected += info_Selected;
                Datasets.Add(infoViewModel);
            }
        }
    }
}