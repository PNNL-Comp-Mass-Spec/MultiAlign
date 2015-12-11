using Microsoft.Win32;
using MultiAlignRogue.ViewModels;
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

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.MetaData;

    using MessageBox = System.Windows.MessageBox;

    /// <summary>
    /// View model for specifying the datasets and paths for a <see cref="RogueProject" />.
    /// </summary>
    public class NewProjectViewModel : ViewModelBase
    {
        private string userDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        /// <summary>
        /// The path to the Rogue Project file.
        /// </summary>
        private string projectFilePath;

        /// <summary>
        /// The path for the output (Analysis) files.
        /// </summary>
        private string outputDirectory;

        /// <summary>
        /// If the user has manually changed the output directory
        /// </summary>
        private bool outputDirectoryModified = false;

        /// <summary>
        /// The last stored output path
        /// </summary>
        private string lastOutputDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectViewModel"/> class.
        /// </summary>
        /// <remarks>Constructor</remarks>
        public NewProjectViewModel()
        {
            this.Datasets = new ObservableCollection<DatasetInformationViewModel>();

            this.BrowseProjectFilesCommand = new RelayCommand(this.BrowseProjectFilesImpl);
            this.BrowseOutputDirectoriesCommand = new RelayCommand(this.BrowseOutputDirectoriesImpl);
            this.AddDatasetCommand = new RelayCommand(this.AddDatasetImpl);
            this.CreateCommand = new RelayCommand(
                                                  this.CreateImpl,
                                                  () => !string.IsNullOrWhiteSpace(this.OutputDirectory) &&
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
        /// Last directory selected for the source data files
        /// </summary>
        /// <remarks>This path is used by the the file selection dialog</remarks>
        public string LastInputDirectory { get; set; }

        /// <summary>
        /// Last directory selected for the project file
        /// </summary>
        /// <remarks>This path is used by the the file selection dialog</remarks>
        public string LastProjectDirectory { get; set; }

        /// <summary>
        /// Last directory selected for the output files
        /// </summary>
        /// <remarks>This path is used by the the file selection dialog</remarks>
        public string LastOutputDirectory {
            get
            {
                return this.lastOutputDirectory;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && this.lastOutputDirectory != value)
                {
                    this.lastOutputDirectory = value;
                    if (string.IsNullOrWhiteSpace(this.outputDirectory))
                    {
                        this.OutputDirectory = value;
                    }
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the path to the Rogue Project file.
        /// </summary>
        public string ProjectFilePath
        {
            get { return this.projectFilePath; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || string.Equals(this.projectFilePath, value))
                {
                    return;
                }

                this.projectFilePath = value;
                var projectFileDirectory = Path.GetDirectoryName(value);

                if (string.IsNullOrWhiteSpace(this.outputDirectory) || (!this.outputDirectoryModified && this.outputDirectory == this.userDocuments))
                {
                    this.OutputDirectory = projectFileDirectory;
                }

                if (string.IsNullOrWhiteSpace(this.LastProjectDirectory))
                {
                    LastProjectDirectory = projectFileDirectory;
                }

                this.CreateCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged();
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
                if (string.IsNullOrWhiteSpace(value) || string.Equals(this.outputDirectory, value))
                {
                    return;
                }

                this.outputDirectory = value;

                if (string.IsNullOrWhiteSpace(this.LastProjectDirectory))
                {
                    LastProjectDirectory = value;
                }

                if (string.IsNullOrWhiteSpace(this.LastOutputDirectory))
                {
                    LastOutputDirectory = value;
                }          

                this.CreateCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged();
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
                AnalysisPath = string.Format("{0}\\Analysis.db3", this.OutputDirectory),
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

            if (!string.IsNullOrWhiteSpace(this.LastProjectDirectory))
                saveFileDialog.InitialDirectory = this.LastProjectDirectory;
            else if (!string.IsNullOrWhiteSpace(this.outputDirectory))
                saveFileDialog.InitialDirectory = this.outputDirectory;

            var result = saveFileDialog.ShowDialog();
            if (result != true)
            {
                return;
            }

            this.ProjectFilePath = saveFileDialog.FileName;
            this.LastProjectDirectory = Path.GetDirectoryName(saveFileDialog.FileName);

            if (string.IsNullOrWhiteSpace(this.outputDirectory))
                this.OutputDirectory = this.LastProjectDirectory;
        }

        /// <summary>
        /// Implementation for <see cref="BrowseOutputDirectoriesCommand" />.
        /// </summary>
        private void BrowseOutputDirectoriesImpl()
        {
            var folderBrowser = new VistaFolderBrowserDialog();

            if (!string.IsNullOrWhiteSpace(this.outputDirectory))
                folderBrowser.SelectedPath = this.outputDirectory;
            else if (!string.IsNullOrWhiteSpace(this.LastProjectDirectory))
                folderBrowser.SelectedPath = this.LastProjectDirectory;

            var result = folderBrowser.ShowDialog();
            if (result == true)
            {
                this.OutputDirectory = folderBrowser.SelectedPath;
                this.outputDirectoryModified = true;
                this.LastOutputDirectory = folderBrowser.SelectedPath;
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
                DefaultExt = ".raw|.pbf|.csv|.ms1ft",
                Filter = DatasetLoader.SupportedFileFilter
            };

            if (!string.IsNullOrWhiteSpace(this.LastInputDirectory))
                openFileDialog.InitialDirectory = this.LastInputDirectory;
            else
                openFileDialog.InitialDirectory = this.userDocuments;

            var result = openFileDialog.ShowDialog();
            if (result != true)
            {
                return;
            }

            var filePaths = openFileDialog.FileNames;
            var datasets = this.GetAndValidateDatasets(filePaths);
            if (datasets.Count == 0)
            {
                return;
            }

            this.LastInputDirectory = Path.GetDirectoryName(filePaths.First());

            foreach (var dataset in datasets)
            {
                var datasetInformationViewModel = new DatasetInformationViewModel(dataset);
                datasetInformationViewModel.RemovalRequested += (s, e) => this.Datasets.Remove(datasetInformationViewModel);
                this.Datasets.Add(datasetInformationViewModel);
                this.CreateCommand.RaiseCanExecuteChanged();                
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
            this.LastOutputDirectory = OutputDirectory;
        }

        private List<DatasetInformation> GetAndValidateDatasets(IEnumerable<string> filePaths)
        {
            var datasetLoader = new DatasetLoader();
            var datasets = datasetLoader.GetValidDatasets(filePaths);

            if (!string.IsNullOrEmpty(datasetLoader.ErrorMessage))
            {
                MessageBox.Show(datasetLoader.ErrorMessage);
            }

            return datasets;
        } 
    }
}
