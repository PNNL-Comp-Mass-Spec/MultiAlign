using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace MultiAlignRogue
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using MultiAlign.Data;
    using MultiAlign.ViewModels.Datasets;

    using MultiAlignCore.Algorithms.Options;
    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO.InputFiles;

    using Ookii.Dialogs;

    using MessageBox = System.Windows.MessageBox;

    /// <summary>
    /// View model for specifying the datasets and paths for a <see cref="RogueProject" />.
    /// </summary>
    public class NewProjectViewModel : ViewModelBase
    {
        /// <summary>
        /// The path to the Rogue Project file.
        /// </summary>
        private string projectFilePath;

        /// <summary>
        /// The path for the output (Analysis) files.
        /// </summary>
        private string outputDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectViewModel"/> class.
        /// </summary>
        public NewProjectViewModel()
        {
            this.Datasets = new ObservableCollection<DatasetInformationViewModel>();

            this.BrowseProjectFilesCommand = new RelayCommand(this.BrowseProjectFilesImpl);
            this.BrowseOutputDirectoriesCommand = new RelayCommand(this.BrowseOutputDirectoriesImpl);
            this.AddDatasetCommand = new RelayCommand(this.AddDatasetImpl);
            this.CreateCommand = new RelayCommand(
                                                  this.CreateImpl,
                                                  () => this.Datasets.Count > 0 &&
                                                        !string.IsNullOrWhiteSpace(this.OutputDirectory) &&
                                                        !string.IsNullOrWhiteSpace(this.ProjectFilePath));
        }

        /// <summary>
        /// An event that is triggered when the user has clicked Create.
        /// </summary>
        public event EventHandler Success;

        /// <summary>
        /// Gets the list of datasets to be added to the project.
        /// </summary>
        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set; }

        /// <summary>
        /// Gets a command that opens the file browser to select output file path.
        /// </summary>
        public RelayCommand BrowseProjectFilesCommand { get; private set; }

        /// <summary>
        /// Gets a command that opens the directory browser to select output directory.
        /// </summary>
        public RelayCommand BrowseOutputDirectoriesCommand { get; private set; }

        /// <summary>
        /// Gets a command that adds a dataset to the dataset list.
        /// </summary>
        public RelayCommand AddDatasetCommand { get; private set; }

        /// <summary>
        /// Gets a command that creates the project.
        /// </summary>
        public RelayCommand CreateCommand { get; private set; }

        /// <summary>
        /// Gets a command that cancels the project creation.
        /// </summary>
        public RelayCommand CancelCommand { get; private set; }

        /// <summary>
        /// Gets or sets the path to the Rogue Project file.
        /// </summary>
        public string ProjectFilePath
        {
            get { return this.projectFilePath; }
            set
            {
                if (this.projectFilePath != value)
                {
                    this.projectFilePath = value;
                    if (string.IsNullOrWhiteSpace(this.outputDirectory))
                    {
                        this.OutputDirectory = Path.GetDirectoryName(value);
                    }

                    this.CreateCommand.RaiseCanExecuteChanged();
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the path for the output (Analysis) files.
        /// </summary>
        public string OutputDirectory
        {
            get { return this.outputDirectory; }
            set
            {
                if (this.outputDirectory != value)
                {
                    this.outputDirectory = value;
                    this.CreateCommand.RaiseCanExecuteChanged();
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="RogueProject" /> for the select dataset parameters.
        /// </summary>
        /// <returns>The <see cref="RogueProject" />.</returns>
        public RogueProject GetRogueProject()
        {
            return new RogueProject
            {
                AnalysisPath = string.Format("{0}\\Analysis", this.OutputDirectory),
                Datasets = new List<DatasetInformation>(this.Datasets.Select(x => x.Dataset)),
                LayoutFilePath = string.Format("{0}\\Layout.xml", this.OutputDirectory)
            };
        }

        /// <summary>
        /// Implementation for <see cref="BrowseProjectFilesCommand" />.
        /// Opens the file browser to select output file path.
        /// </summary>
        private void BrowseProjectFilesImpl()
        {
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".xml",
                Filter = @"Supported Files|*.xml"
            };

            var result = saveFileDialog.ShowDialog();
            if (result != null && result.Value)
            {
                this.ProjectFilePath = saveFileDialog.FileName;
            }
        }

        /// <summary>
        /// Implementation for <see cref="BrowseOutputDirectoriesCommand" />.
        /// </summary>
        private void BrowseOutputDirectoriesImpl()
        {
            var folderBrowser = new VistaFolderBrowserDialog();
            var result = folderBrowser.ShowDialog();
            if (result != null && result.Value)
            {
                this.OutputDirectory = folderBrowser.SelectedPath;
            }
        }

        /// <summary>
        /// Implementation for <see cref="AddDatasetCommand" />.
        /// </summary>
        private void AddDatasetImpl()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                DefaultExt = ".raw|.csv",
                Filter = @"Supported Files|*.raw;*.csv;|Raw Files (*.raw)|*.raw|CSV Files (*.csv)|*.csv;"
            };

            var result = openFileDialog.ShowDialog();
            if (result != null && result.Value)
            {
                var filePaths = openFileDialog.FileNames;
                var allFilesSelected = filePaths.Any(file => file.EndsWith(".raw")) &&
                                       filePaths.Any(file => file.EndsWith("_isos.csv")) &&
                                       filePaths.Any(file => file.EndsWith("_scans.csv"));
                if (!allFilesSelected)
                {
                    var statusMessage =
                        "MultiAlign Rogue requires at least a .raw file, an isos file, and a scans file.";
                    ApplicationStatusMediator.SetStatus(statusMessage);
                    MessageBox.Show(statusMessage);
                    return;
                }

                var datasets = DatasetInformation.ConvertInputFilesIntoDatasets(this.GetInputFilesFromPath(filePaths));
                foreach (var dataset in datasets)
                {
                    var datasetInformationViewModel = new DatasetInformationViewModel(dataset);
                    datasetInformationViewModel.RemovalRequested += (s, e) => this.Datasets.Remove(datasetInformationViewModel);
                    this.Datasets.Add(datasetInformationViewModel);
                    this.CreateCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Implementation for <see cref="CreateCommand" />.
        /// Creates the project.
        /// </summary>
        private void CreateImpl()
        {
            if (this.Success != null)
            {
                this.Success(this, EventArgs.Empty);
            }
        }

        private List<InputFile> GetInputFilesFromPath(IEnumerable<string> filePaths)
        {
            var files = new List<InputFile>();
            foreach (var filePath in filePaths)
            {
                var type = DatasetInformation.SupportedFileTypes.FirstOrDefault(sft => filePath.ToLower().Contains(sft.Extension));
                if (type != null)
                {
                    files.Add(new InputFile { Path = filePath, FileType = type.InputType });
                }
            }

            return files;
        }
    }
}
