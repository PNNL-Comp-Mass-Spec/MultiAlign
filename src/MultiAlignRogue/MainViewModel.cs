using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Ookii.Dialogs;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlign.ViewModels.Wizard;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using MultiAlignRogue.ViewModels;
using Ookii.Dialogs.Wpf;

namespace MultiAlignRogue
{
    using System.Windows;

    using DMS;

    using MultiAlignCore.Algorithms.Options;
    using MultiAlignCore.Extensions;

    using MultiAlignRogue.Alignment;
    using MultiAlignRogue.Clustering;
    using MultiAlignRogue.Feature_Finding;

    using MessageBox = System.Windows.MessageBox;

    public class MainViewModel : ViewModelBase
    {
        #region Private Data Members
        public AnalysisDatasetSelectionViewModel DataSelectionViewModel;

        private readonly AnalysisConfig m_config;
        private readonly FeatureLoader featureCache;

        private DataTable datasetInfo;
        private FeatureDataAccessProviders providers;

        private FeatureFindingSettingsViewModel featureFindingSettingsViewModel;
        private AlignmentSettingsViewModel alignmentSettingsViewModel;
        private ClusterSettingsViewModel clusterSettingsViewModel;

        private IClusterViewFactory clusterViewFactory;

        private IReadOnlyCollection<DatasetInformationViewModel> selectedDatasets;

        private string inputFilePath;

        private string projectPath;

        private string windowTitle;

        private string outputDirectory;

        private int progressTracker;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            m_config = new AnalysisConfig();
            Analysis = new MultiAlignAnalysis();
            m_config.AnalysisName = "Analysis.db3";
            m_config.Analysis = Analysis;

            this.WindowTitle = "MultiAlign Rogue";

            DataSelectionViewModel = new AnalysisDatasetSelectionViewModel(Analysis);
            
            SelectFilesCommand = new RelayCommand(SelectFiles, () => !string.IsNullOrWhiteSpace(this.ProjectPath));
            SelectDirectoryCommand = new RelayCommand(SelectDirectory, () => !string.IsNullOrWhiteSpace(this.ProjectPath));
            AddFolderCommand = new RelayCommand(AddFolderDelegate, () => !string.IsNullOrWhiteSpace(this.InputFilePath) && Directory.Exists(this.InputFilePath) && !string.IsNullOrWhiteSpace(this.ProjectPath));
            SearchDmsCommand = new RelayCommand(SearchDms, () => this.ShowOpenFromDms && !string.IsNullOrWhiteSpace(this.ProjectPath));
            CreateNewProjectCommand = new RelayCommand(this.CreateNewProject);
            SaveProjectCommand = new RelayCommand(SaveProject, () => !string.IsNullOrWhiteSpace(this.ProjectPath));
            LoadProjectCommand = new RelayCommand(LoadProject);
            SaveAsProjectCommand = new RelayCommand(this.SaveProjectAs, () => !string.IsNullOrWhiteSpace(this.ProjectPath));
            
            featureCache = new FeatureLoader { Providers = Analysis.DataProviders };
            Datasets = new ObservableCollection<DatasetInformationViewModel>();

            featureCache.Providers = Analysis.DataProviders;
            this.FeatureFindingSettingsViewModel = new FeatureFindingSettingsViewModel(Analysis, featureCache, Datasets);
            this.AlignmentSettingsViewModel = new AlignmentSettingsViewModel(Analysis, featureCache, Datasets);
            this.ClusterSettingsViewModel = new ClusterSettingsViewModel(Analysis, Datasets);
        }
        #endregion

        #region Command
        /// <summary>
        /// Gets a command for selecting files for a new dataset (scans, isos, raw).
        /// </summary>
        public RelayCommand SelectFilesCommand { get; private set; }

        /// <summary>
        /// Gets a command for adding a dataset from DMS.
        /// </summary>
        public RelayCommand SearchDmsCommand { get; private set; }

        /// <summary>
        /// Gets a command for selecting a directory path for a new dataset.
        /// </summary>
        public RelayCommand SelectDirectoryCommand { get; private set; }

        /// <summary>
        /// Gets a command for adding the dataset in the selected directory.
        /// </summary>
        public RelayCommand AddFolderCommand { get; private set; }

        /// <summary>
        /// Gets a command for creating a new <see cref="RogueProject" />.
        /// </summary>
        public RelayCommand CreateNewProjectCommand { get; private set; }

        /// <summary>
        /// Gets a command for loading an existing <see cref="RogueProject" />.
        /// </summary>
        public RelayCommand LoadProjectCommand { get; private set; }

        /// <summary>
        /// Gets a command for saving the current <see cref="RogueProject" />.
        /// </summary>
        public RelayCommand SaveProjectCommand { get; private set; }

        /// <summary>
        /// Gets a command for saving the current <see cref="RogueProject" />
        /// in a new project file.
        /// </summary>
        public RelayCommand SaveAsProjectCommand { get; private set; }
        #endregion

        #region Public Properties
        public MultiAlignAnalysis Analysis { get; private set; }

        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set; }

        public int ProgressTracker
        {
            get { return this.progressTracker; }
            private set
            {
                if (this.progressTracker != value)
                {
                    this.progressTracker = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string ProjectPath
        {
            get { return this.projectPath; }
            set
            {
                if (this.projectPath != value)
                {
                    this.projectPath = value;
                    var fileName = Path.GetFileNameWithoutExtension(this.projectPath);
                    this.WindowTitle = string.Format("MultiAlign Rogue ({0})", fileName);
                    this.SaveProjectCommand.RaiseCanExecuteChanged();
                    this.SaveAsProjectCommand.RaiseCanExecuteChanged();
                    this.SelectFilesCommand.RaiseCanExecuteChanged();
                    this.SelectDirectoryCommand.RaiseCanExecuteChanged();
                    this.SearchDmsCommand.RaiseCanExecuteChanged();
                    this.AddFolderCommand.RaiseCanExecuteChanged();
                    this.RaisePropertyChanged();   
                }
            }
        }

        public string WindowTitle
        {
            get { return this.windowTitle; }
            set
            {
                if (this.WindowTitle != value)
                {
                    this.windowTitle = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not "Open From DMS" should be shown on the menu based on whether
        /// or not the user is on the PNNL network or not.
        /// </summary>
        public bool ShowOpenFromDms
        {
            get { return System.Net.Dns.GetHostEntry(string.Empty).HostName.Contains("pnl.gov"); }
        }

        public string InputFilePath
        {
            get { return this.inputFilePath; }
            set
            {
                if (this.inputFilePath != value)
                {
                    this.inputFilePath = value;
                    this.RaisePropertyChanged();
                    this.AddFolderCommand.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Child ViewModels
        public FeatureFindingSettingsViewModel FeatureFindingSettingsViewModel
        {
            get { return this.featureFindingSettingsViewModel; }
            private set
            {
                this.featureFindingSettingsViewModel = value;
                this.RaisePropertyChanged();
            }
        }

        public AlignmentSettingsViewModel AlignmentSettingsViewModel
        {
            get { return this.alignmentSettingsViewModel; }
            set
            {
                this.alignmentSettingsViewModel = value;
                this.RaisePropertyChanged();
            }
        }

        public ClusterSettingsViewModel ClusterSettingsViewModel
        {
            get { return this.clusterSettingsViewModel; }
            set
            {
                this.clusterSettingsViewModel = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Import Files
        public void SelectFiles()
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

                this.AddDatasets(this.GetInputFilesFromPath(filePaths));
            }
        }

        public void SelectDirectory()
        {
            var folderBrowser = new VistaFolderBrowserDialog();
            var result = folderBrowser.ShowDialog();

            if (result != null && result.Value)
            {
                InputFilePath = folderBrowser.SelectedPath;
            }

            this.AddFolderCommand.Execute(null);
        }

        public void AddFolderDelegate()
        {
            var supportedTypes = DatasetInformation.SupportedFileTypes;
            var extensions = new List<string>();

            supportedTypes.ForEach(x => extensions.Add("*" + x.Extension));

            if (string.IsNullOrEmpty(InputFilePath))
            {
                ApplicationStatusMediator.SetStatus("Select a folder path first. File -> Select Files");
                MessageBox.Show("Select a folder path first. File -> Select Files");
                return;
            }

            if (!Directory.Exists(InputFilePath))
            {
                ApplicationStatusMediator.SetStatus("The directory specified does not exist.");
                MessageBox.Show("The directory specified does not exist.");
                return;
            }

            var files = DatasetSearcher.FindDatasets(InputFilePath,
                extensions,
                SearchOption.TopDirectoryOnly);
            this.AddDatasets(files);
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

        private void SearchDms()
        {
            var dmsLookupViewModel = new DmsLookupViewModel { OutputDirectory = this.outputDirectory };
            var dialog = new DmsLookupView { DataContext = dmsLookupViewModel };
            dmsLookupViewModel.DatasetSelected += (o, e) => dialog.Close();
            dialog.ShowDialog();
            if (!dmsLookupViewModel.Status)
            {
                return;
            }

            if (dmsLookupViewModel.ShouldCopyFiles)
            {
                this.InputFilePath = dmsLookupViewModel.OutputDirectory;
                this.AddFolderCommand.Execute(null);
            }
            else
            {
                foreach (var dataset in dmsLookupViewModel.Datasets)
                {
                    var filePaths = dataset.GetAvailableFiles();
                    if (filePaths.Count > 0)
                    {
                        this.AddDatasets(this.GetInputFilesFromPath(filePaths));
                    }
                }
            }
        }

        public void UpdateDatasets()
        {
            Datasets.Clear();
            foreach (var info in Analysis.MetaData.Datasets)
            {
                SingletonDataProviders.AddDataset(info);
                var viewmodel = new DatasetInformationViewModel(info);
                viewmodel.RemovalRequested += (s, e) =>
                {
                    var vm = s as DatasetInformationViewModel;
                    if (vm != null)
                    {
                        var result =
                            MessageBox.Show(
                                string.Format("Are you sure that you'd like to remove {0}", vm.Dataset.DatasetName),
                                "Remove Dataset?",
                                MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            this.Datasets.Remove(vm);
                            this.Analysis.MetaData.Datasets.Remove(vm.Dataset);
                        }
                    }
                };

                viewmodel.StateChanged += (s, e) => this.SaveProject();
                Datasets.Add(viewmodel);
            }
        }

        private void AddDatasets(List<InputFile> files)
        {
            DataSelectionViewModel.AddDatasets(files);
            UpdateDatasets();
            this.RaisePropertyChanged("m_config");
        }
        #endregion

        #region Data Providers
        private FeatureDataAccessProviders SetupDataProviders(bool createNewDatabase)
        {
            FeatureDataAccessProviders providers;
            Logger.PrintMessage("Setting up data providers for caching and storage.");
            try
            {
                var path = AnalysisPathUtils.BuildAnalysisName(m_config.AnalysisPath, m_config.AnalysisName);
                providers = SetupDataProviders(path, createNewDatabase);
            }
            catch (IOException ex)
            {
                Logger.PrintMessage(ex.Message);
                Logger.PrintMessage(ex.StackTrace);
                throw;
            }
            return providers;
        }

        private FeatureDataAccessProviders SetupDataProviders(string path, bool createNew)
        {
            try
            {
                return DataAccessFactory.CreateDataAccessProviders(path, createNew);
            }
            catch (IOException ex)
            {
                Logger.PrintMessage("Could not access the database.  Is it opened somewhere else?" + ex.Message);
                throw;
            }
        }
        #endregion

        #region Project Loading
        private void CreateNewProject()
        {
            var success = false;
            var newProjectViewModel = new NewProjectViewModel();
            var dialog = new NewProjectWindow { DataContext = newProjectViewModel };
            newProjectViewModel.Success += (s, e) =>
                {
                    success = true;
                    dialog.Close();
                };
            dialog.ShowDialog();

            if (success)
            {
                var rogueProject = newProjectViewModel.GetRogueProject();
                rogueProject.MultiAlignAnalysisOptions = new MultiAlignAnalysisOptions();
                this.LoadRogueProject(rogueProject, true);
                this.Serialize(newProjectViewModel.ProjectFilePath);
                this.ProjectPath = newProjectViewModel.ProjectFilePath;
                this.outputDirectory = newProjectViewModel.OutputDirectory;
                this.RaisePropertyChanged("Analysis");
            }
        }

        private void LoadRogueProject(RogueProject rogueProject, bool isNewProject)
        {
            this.Analysis = new MultiAlignAnalysis
            {
                DataProviders = this.SetupDataProviders(rogueProject.AnalysisPath, isNewProject),
                Options = rogueProject.MultiAlignAnalysisOptions
            };

            this.DataSelectionViewModel.Analysis = this.Analysis;
            this.Analysis.MetaData.Datasets.AddRange(rogueProject.Datasets);
            this.featureCache.Providers = this.Analysis.DataProviders;
            this.m_config.AnalysisPath = rogueProject.AnalysisPath;
            this.UpdateDatasets();
            this.clusterViewFactory = new ClusterViewFactory(this.Analysis.DataProviders, rogueProject.LayoutFilePath);
            this.FeatureFindingSettingsViewModel = new FeatureFindingSettingsViewModel(this.Analysis, this.featureCache, this.Datasets);
            this.AlignmentSettingsViewModel = new AlignmentSettingsViewModel(this.Analysis, this.featureCache, this.Datasets);
            this.ClusterSettingsViewModel = new ClusterSettingsViewModel(this.Analysis, this.Datasets, this.clusterViewFactory);
            this.RaisePropertyChanged("Analysis");
        }

        private void SaveProjectAs()
        {
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".xml",
                Filter = @"Supported Files|*.xml"
            };

            var result = saveFileDialog.ShowDialog();
            if (result != null && result.Value)
            {
                this.Serialize(saveFileDialog.FileName);
            }
        }

        private void SaveProject()
        {
            if (!string.IsNullOrWhiteSpace(this.ProjectPath))
            {
                this.Serialize(this.ProjectPath);   
            }
        }

        private void LoadProject()
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = @"Supported Files|*.xml"
            };

            var result = openFileDialog.ShowDialog();
            if (result != null && result.Value)
            {
                var rogueProject = this.Deserialize(openFileDialog.FileName);
                if (string.IsNullOrWhiteSpace(rogueProject.LayoutFilePath))
                {
                    rogueProject.LayoutFilePath = string.Format("{0}\\Layout.xml",
                        Path.GetDirectoryName(rogueProject.AnalysisPath));
                }

                this.LoadRogueProject(rogueProject, false);
                this.outputDirectory = Path.GetDirectoryName(rogueProject.AnalysisPath);
                this.ProjectPath = openFileDialog.FileName;
            }
        }

        private void Serialize(string filePath)
        {
            var rogueProjectSerializer = new DataContractSerializer(typeof (RogueProject));
            var datasetInfoList = this.Datasets.Select(datasetInformation => datasetInformation.Dataset).ToList();
            var rogueProject = new RogueProject
            {
                MultiAlignAnalysisOptions = this.Analysis.Options,
                Datasets = datasetInfoList,
                AnalysisPath = this.m_config.AnalysisPath
            };
            using (var writer = File.Open(filePath, FileMode.Create))
            {
                rogueProjectSerializer.WriteObject(writer, rogueProject);
            }
        }

        private RogueProject Deserialize(string filePath)
        {
            var rogueProjectSerializer = new DataContractSerializer(typeof(RogueProject));
            var rogueProject = new RogueProject();

            using (var reader = File.Open(filePath, FileMode.Open))
            {
                try
                {
                    rogueProject = (RogueProject) rogueProjectSerializer.ReadObject(reader);
                    this.Analysis.Options = rogueProject.MultiAlignAnalysisOptions;                   
                    
                }
                catch (InvalidCastException)
                {
                    MessageBox.Show("Could not deserialize analysis options.");
                }
            }

            return rogueProject;
        }
        #endregion
    }
}
