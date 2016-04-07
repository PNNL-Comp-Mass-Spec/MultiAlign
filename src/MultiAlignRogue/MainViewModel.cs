namespace MultiAlignRogue
{
    using System.Windows;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using InformedProteomics.Backend.Utils;
    using Microsoft.Win32;

    using Ookii.Dialogs.Wpf;

    using DMS;

    using GalaSoft.MvvmLight.Messaging;

    using MultiAlignCore.Algorithms.Options;
    using MultiAlignCore.Extensions;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO;
    using MultiAlignCore.IO.Features;
    using MultiAlignCore.IO.Options;
    using MultiAlignRogue.Utils;
    using MultiAlignRogue.ViewModels;
    using MultiAlignRogue.Alignment;
    using MultiAlignRogue.AMTMatching;
    using MultiAlignRogue.Clustering;
    using MultiAlignRogue.DataLoading;
    using MultiAlignRogue.Feature_Finding;

    using MessageBox = System.Windows.MessageBox;

    public class MainViewModel : ViewModelBase
    {
        #region Private Data Members
        private MultiAlignAnalysis analysis;

        //private readonly AnalysisConfig analysisConfig;

        private FeatureFindingSettingsViewModel featureFindingSettingsViewModel;
        private AlignmentSettingsViewModel alignmentSettingsViewModel;
        private ClusterSettingsViewModel clusterSettingsViewModel;
        private StacSettingsViewModel stacSettingsViewModel;

        private IClusterViewFactory clusterViewFactory;

        private readonly List<DatasetInformation> deletedDatasets = new List<DatasetInformation>();

        private string inputFilePath;

        private string projectPath;

        private string projectDirectory;

        private string windowTitle;

        private string outputDirectory;

        private string lastInputDirectory;

        private string lastProjectDirectory;

        private string lastOutputDirectory;

        private bool useProjectDirectoryAuto;

        private int progressTracker;

        private bool shouldShowProgress;

        private bool showSplash = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <remarks>Constructor</remarks>
        public MainViewModel()
        {
            this.Analysis = new MultiAlignAnalysis();

            this.WindowTitle = "MultiAlign Rogue";

            this.SelectFilesCommand = new RelayCommand(this.SelectFiles, () => !string.IsNullOrWhiteSpace(this.ProjectPath));
            this.CloseWindowCommand = new RelayCommand(this.PersistProject);
            this.SelectDirectoryCommand = new RelayCommand(this.SelectDirectory, () => !string.IsNullOrWhiteSpace(this.ProjectPath));
            this.AddFolderCommand = new RelayCommand(this.AddFolderDelegate, () => !string.IsNullOrWhiteSpace(this.InputFilePath) && Directory.Exists(this.InputFilePath) && !string.IsNullOrWhiteSpace(this.ProjectPath));
            this.SearchDmsCommand = new RelayCommand(this.SearchDms, () => this.ShowOpenFromDms && !string.IsNullOrWhiteSpace(this.ProjectPath));
            this.CreateNewProjectCommand = new RelayCommand(async () => await this.CreateNewProject());
            this.SaveProjectCommand = new RelayCommand(this.SaveProject, () => !string.IsNullOrWhiteSpace(this.ProjectPath));
            this.LoadProjectCommand = new RelayCommand(async () => await this.LoadProject());
            this.SaveAsProjectCommand = new RelayCommand(this.SaveProjectAs, () => !string.IsNullOrWhiteSpace(this.ProjectPath));

            this.RestoreDefaultSettingsCommand = new RelayCommand(this.RestoreDefaultSettings);

            this.RunFullWorkflowCommand = new RelayCommand(this.AsyncWorkflow);

            this.Datasets = new ObservableCollection<DatasetInformationViewModel>();

            this.DatasetLoaderSelectionViewModel = new DatasetLoaderSelectionViewModel();
            this.FeatureFindingSettingsViewModel = new FeatureFindingSettingsViewModel(this.Analysis, this.Datasets);
            this.AlignmentSettingsViewModel = new AlignmentSettingsViewModel(this.Analysis, this.Datasets);
            this.ClusterSettingsViewModel = new ClusterSettingsViewModel(this.Analysis, this.Datasets);
            this.ShouldShowProgress = false;

            // When dataset is selected, update selected dataset loader
            this.MessengerInstance.Register<PropertyChangedMessage<bool>>(this,
                msg =>
                    {
                        var datasetInformationViewModel = msg.Sender as DatasetInformationViewModel;
                        if (datasetInformationViewModel != null && msg.PropertyName == "IsSelected")
                        {
                            this.DatasetLoaderSelectionViewModel.SelectedDatasetLoaderType =
                                datasetInformationViewModel.Dataset.SupportedDatasetType;
                        }
                    });
        }

        #endregion

        #region Command
        /// <summary>
        /// Gets a command for selecting files for a new dataset (scans, isos, raw).
        /// </summary>
        public RelayCommand SelectFilesCommand { get; private set; }

        /// <summary>
        /// Gets a command for selecting files for a new dataset (scans, isos, raw).
        /// </summary>
        public RelayCommand CloseWindowCommand { get; private set; }

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
        /// Gets a command for creating a new project />.
        /// </summary>
        public RelayCommand CreateNewProjectCommand { get; private set; }

        /// <summary>
        /// Gets a command for loading an existing project />.
        /// </summary>
        public RelayCommand LoadProjectCommand { get; private set; }

        /// <summary>
        /// Gets a command for saving the current project />.
        /// </summary>
        public RelayCommand SaveProjectCommand { get; private set; }

        /// <summary>
        /// Gets a command for saving the current project />
        /// in a new project file.
        /// </summary>
        public RelayCommand SaveAsProjectCommand { get; private set; }

        /// <summary>
        /// Gets a command that restores all settings to their defaults.
        /// </summary>
        public RelayCommand RestoreDefaultSettingsCommand { get; private set; }

        /// <summary>
        /// Gets a command for running feature finding, alignment, and clustering in succession.
        /// </summary>
        public RelayCommand RunFullWorkflowCommand { get; private set; }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the splash screen should be displayed.
        /// </summary>
        public bool ShowSplash
        {
            get { return this.showSplash; }
            private set
            {
                if (this.showSplash != value)
                {
                    this.showSplash = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the current analysis.
        /// </summary>
        public MultiAlignAnalysis Analysis
        {
            get { return this.analysis; }
            private set
            {
                if (this.analysis != value)
                {
                    this.analysis = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the view models for the current datasets.
        /// </summary>
        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the full workflow progress bar should be displayed.
        /// </summary>
        public bool ShouldShowProgress
        {
            get { return this.shouldShowProgress; }
            private set
            {
                if (this.shouldShowProgress != value)
                {
                    this.shouldShowProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the full workflow progress.
        /// </summary>
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

        /// <summary>
        /// Gets the path to the project file.
        /// </summary>
        public string ProjectPath
        {
            get { return this.projectPath; }
            private set
            {
                if (this.projectPath != value)
                {
                    this.projectPath = value;
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

        /// <summary>
        /// Gets or sets the window title to be displayed.
        /// </summary>
        public string WindowTitle
        {
            get { return this.windowTitle; }
            private set
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

        /// <summary>
        /// Gets the path to the project file.
        /// </summary>
        public string InputFilePath
        {
            get { return this.inputFilePath; }
            private set
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

        /// <summary>
        /// Gets the view model for selecting a the dataset settings for editing.
        /// </summary>
        public DatasetLoaderSelectionViewModel DatasetLoaderSelectionViewModel { get; private set; }

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

        public StacSettingsViewModel StacSettingsViewModel
        {
            get { return this.stacSettingsViewModel; }
            set
            {
                this.stacSettingsViewModel = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Import Files

        /// <summary>
        /// Select an input dataset file
        /// </summary>
        public void SelectFiles()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                DefaultExt = ".csv",
                Filter = DatasetLoader.SupportedFileFilter
            };

            if (!string.IsNullOrWhiteSpace(this.lastInputDirectory))
            {
                openFileDialog.InitialDirectory = this.lastInputDirectory;
            }

            var result = openFileDialog.ShowDialog();
            if (result != true)
            {
                return;
            }

            var filePaths = openFileDialog.FileNames;
            if (filePaths.Length == 0)
                return;

            this.AddDatasets(filePaths);

            this.lastInputDirectory = Path.GetDirectoryName(filePaths.First());
        }

        /// <summary>
        /// Select an input dataset folder
        /// </summary>
        public void SelectDirectory()
        {
            var folderBrowser = new VistaFolderBrowserDialog();

            if (!string.IsNullOrWhiteSpace(this.lastInputDirectory))
            {
                folderBrowser.SelectedPath = this.lastInputDirectory;
            }

            var result = folderBrowser.ShowDialog();
            if (result != true)
            {
                return;
            }

            this.InputFilePath = folderBrowser.SelectedPath;
            this.lastInputDirectory = Path.GetDirectoryName(this.InputFilePath);

            this.AddFolderCommand.Execute(null);
        }

        public void AddFolderDelegate()
        {
            if (string.IsNullOrEmpty(this.InputFilePath))
            {
                MessageBox.Show("Select a folder path first. File -> Select Files");
                return;
            }

            if (!Directory.Exists(this.InputFilePath))
            {
                MessageBox.Show("The directory specified does not exist.");
                return;
            }

            this.AddDatasets(this.InputFilePath);
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
                        this.AddDatasets(filePaths);
                    }
                }
            }
        }

        public void UpdateDatasets()
        {
            this.Datasets.Clear();
            var i = 0;
            foreach (var info in this.Analysis.MetaData.Datasets)
            {
                info.DatasetId = i++;
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
                            this.deletedDatasets.Add(vm.Dataset);
                            this.DatasetLoaderSelectionViewModel.SetDatasetLoaders(this.Analysis.GetDatasetLoaders(this.Datasets.Select(ds => ds.Dataset).ToList()));
                        }
                    }
                };

                viewmodel.StateChanged += (s, e) => this.PersistProject();
                this.Datasets.Add(viewmodel);
            }

            this.DatasetLoaderSelectionViewModel.SetDatasetLoaders(this.Analysis.GetDatasetLoaders(this.Datasets.Select(ds => ds.Dataset).ToList()));
        }

        private void AddDatasets(string folderPath)
        {
            var supportedTypes = DatasetLoader.SupportedFileTypes;
            var extensions = new List<string>();

            supportedTypes.ForEach(x => extensions.Add("*" + x.Extension));

            var datasetLoader = new DatasetLoader();
            var datasets = datasetLoader.GetValidDatasets(folderPath, extensions, SearchOption.TopDirectoryOnly);
            if (!string.IsNullOrEmpty(datasetLoader.ErrorMessage))
            {
                MessageBox.Show(datasetLoader.ErrorMessage);
            }

            if (!this.CheckDatasets(datasets))
            {
                MessageBox.Show("Datasets are incompatible.");
                return;
            }

            // Add valid datasets.
            this.Analysis.MetaData.Datasets.AddRange(datasets);
            this.UpdateDatasets();
        }

        private void AddDatasets(IEnumerable<string> files)
        {
            var datasetLoader = new DatasetLoader();
            var datasets = datasetLoader.GetValidDatasets(files);

            if (!string.IsNullOrEmpty(datasetLoader.ErrorMessage))
            {
                MessageBox.Show(datasetLoader.ErrorMessage);
            }

            if (!this.CheckDatasets(datasets))
            {
                MessageBox.Show("Datasets are incompatible.");
                return;
            }

            // Add valid datasets.
            this.Analysis.MetaData.Datasets.AddRange(datasets);
            this.UpdateDatasets();
        }

        private bool CheckDatasets(IEnumerable<DatasetInformation> datasets)
        {
            if (this.Analysis.MetaData.Datasets.Count > 0)
            {
                return DatasetLoader.IsValidDatasetCombo(this.Analysis.MetaData.Datasets[0].DatasetType, datasets.Select(ds => ds.DatasetType));
            }

            return true;
        }

        #endregion

        #region Data Providers
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
        private async Task CreateNewProject()
        {
            var success = false;
            var newProjectViewModel = new NewProjectViewModel();
            var dialog = new NewProjectWindow { DataContext = newProjectViewModel };

#if (!DEBUG)
            try
            {
#endif
                newProjectViewModel.LastInputDirectory = this.lastInputDirectory;
                newProjectViewModel.LastProjectDirectory = this.lastProjectDirectory;
                newProjectViewModel.LastOutputDirectory = this.lastOutputDirectory;
                newProjectViewModel.UseProjectDirectory = this.useProjectDirectoryAuto;

                newProjectViewModel.Success += (s, e) =>
                {
                    success = true;
                    dialog.Close();
                };
                dialog.ShowDialog();

                if (success)
                {
                    this.ShowSplash = false;
                    this.Analysis.AnalysisName = newProjectViewModel.ProjectFilePath;
                    this.ProjectPath = newProjectViewModel.ProjectFilePath;
                    this.projectDirectory = Path.GetDirectoryName(this.projectPath) + Path.DirectorySeparatorChar;
                    this.outputDirectory = newProjectViewModel.OutputDirectory;
                    await this.LoadRogueProject(true, newProjectViewModel.Datasets.Select(x => x.Dataset).ToList());
                    
                    this.PersistProject();

                    this.lastInputDirectory = newProjectViewModel.LastInputDirectory;
                    this.lastProjectDirectory = newProjectViewModel.LastProjectDirectory;
                    this.lastOutputDirectory = newProjectViewModel.LastOutputDirectory;
                    this.useProjectDirectoryAuto = newProjectViewModel.UseProjectDirectory;
                }
#if (!DEBUG)
            }
            catch (Exception ex)
            {
                Logger.PrintMessage("Exception creating a new project: " + ex.Message);
                MessageBox.Show("Exception creating the new project: " + ex.Message);
            }
#endif

        }

        /// <summary>
        /// Load a project
        /// </summary>
        /// <param name="isNewProject">If the project is a new one</param>
        /// <param name="datasets">Datasets to add to the project, if it is a new project.</param>
        /// <returns></returns>
        private async Task LoadRogueProject(bool isNewProject, List<DatasetInformation> datasets = null)
        {
            Directory.SetCurrentDirectory(this.outputDirectory);
            this.Analysis = new MultiAlignAnalysis
            {
                DataProviders = this.SetupDataProviders(this.ProjectPath, isNewProject),
            };

            var fileName = Path.GetFileNameWithoutExtension(this.projectPath);
            this.Analysis.AnalysisName = fileName;
            this.WindowTitle = string.Format("MultiAlign Rogue ({0})", fileName);

            this.deletedDatasets.Clear();
            var dbOptions = this.Analysis.DataProviders.OptionsDao.FindAll();
            this.Analysis.Options = OptionsTransformer.ListToProperties(dbOptions);

            if (datasets != null)
            {
                this.analysis.MetaData.Datasets.AddRange(datasets);
            }

            // Prevent updates of ViewModels (that we will recreate anyway) while loading datasets
            this.clusterViewFactory = null;
            this.FeatureFindingSettingsViewModel = null;
            this.AlignmentSettingsViewModel = null;
            this.StacSettingsViewModel = null;

            var dbDatasets = this.Analysis.DataProviders.DatasetCache.FindAll();
            // Resolve the relative paths
            if (!string.IsNullOrWhiteSpace(this.projectDirectory))
            {
                foreach (var dataset in dbDatasets)
                {
                    foreach (var file in dataset.InputFiles)
                    {
                        if (!string.IsNullOrWhiteSpace(file.RelativePath))
                        {
                            var combined = Path.Combine(this.projectDirectory, file.RelativePath);
                            var cleaned = Path.GetFullPath(combined);
                            if (File.Exists(cleaned))
                            {
                                file.Path = cleaned;
                            }
                        }
                        // TODO: show warning if file cannot be found.
                        // TODO: OR disable redo of step that needs specified file, if file is only needed for e.g. feature finding
                    }
                }
            }
            this.Analysis.MetaData.Datasets.AddRange(dbDatasets);

            this.Analysis.MetaData.BaselineDataset = this.Analysis.MetaData.Datasets.FirstOrDefault(ds => ds.IsBaseline);

            this.Analysis.AnalysisPath = this.ProjectPath;
            this.UpdateDatasets();

            this.clusterViewFactory = new ClusterViewFactory(this.Analysis.DataProviders);

            this.FeatureFindingSettingsViewModel = new FeatureFindingSettingsViewModel(this.Analysis, this.Datasets);
            this.AlignmentSettingsViewModel = new AlignmentSettingsViewModel(this.Analysis, this.Datasets);
            this.StacSettingsViewModel = new StacSettingsViewModel(this.Analysis, this.Datasets);
            DatabaseSelectionViewModel.Instance.Analysis = this.Analysis;
            if (this.Analysis.Options.AlignmentOptions.InputDatabase != null)
            {
                await DatabaseSelectionViewModel.Instance.LoadMassTagDatabase(this.Analysis.Options.AlignmentOptions.InputDatabase);
            }

            this.ClusterSettingsViewModel = new ClusterSettingsViewModel(this.Analysis, this.Datasets, this.clusterViewFactory);

        }

        private void SaveProjectAs()
        {
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".db3",
                Filter = @"Supported Files|*.db3"
            };

            if (!string.IsNullOrWhiteSpace(this.lastProjectDirectory))
            {
                saveFileDialog.InitialDirectory = this.lastProjectDirectory;
            }

            var result = saveFileDialog.ShowDialog();
            if (result != true)
            {
                return;
            }

            this.PersistProject();

            this.lastProjectDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
        }

        private void SaveProject()
        {
            if (!string.IsNullOrWhiteSpace(this.ProjectPath))
            {
                this.PersistProject();
            }
        }

        private async Task LoadProject()
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".db3",
                Filter = @"Supported Files|*.db3"
            };

            if (!string.IsNullOrWhiteSpace(this.outputDirectory))
            {
                openFileDialog.InitialDirectory = this.outputDirectory;
            }

            var result = openFileDialog.ShowDialog();
            if (result != true)
            {
                return;
            }

			this.ShowSplash = false;
            this.outputDirectory = Path.GetDirectoryName(openFileDialog.FileName);
            this.ProjectPath = openFileDialog.FileName;
            this.projectDirectory = Path.GetDirectoryName(openFileDialog.FileName) + Path.DirectorySeparatorChar;

            await this.LoadRogueProject(false);
        }

        private void PersistProject()
        {
            if (this.Analysis.DataProviders == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(this.projectDirectory))
            {
                this.projectDirectory = Path.GetDirectoryName(this.ProjectPath) + Path.DirectorySeparatorChar;
            }
            // Get the relative paths set up.
            foreach (var dataset in this.Datasets.Select(d => d.Dataset))
            {
                foreach (var file in dataset.InputFiles)
                {
                    file.RelativePath = PathUtils.MakeRelativePath(this.projectDirectory, file.Path);
                }
            }

            // Persist
            try
            {
                this.Analysis.DataProviders.DatabaseLock.EnterWriteLock();
                this.Analysis.DataProviders.DatasetCache.AddAll(this.Datasets.Select(d => d.Dataset).ToList());
                this.Analysis.DataProviders.DatasetCache.DeleteAll(this.deletedDatasets);
                // TODO: remove features and such from the database as well.
                this.deletedDatasets.Clear();
                this.Analysis.DataProviders.OptionsDao.AddAll(
                    OptionsTransformer.PropertiesToList(this.Analysis.Options));
            }
            finally
            {
                this.Analysis.DataProviders.DatabaseLock.ExitWriteLock();
            }
        }
        #endregion

        private void RestoreDefaultSettings()
        {
            if (MessageBox.Show("Are you sure you would like to reset all settings to their default values?",
                                "Restore Defaults", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            this.DatasetLoaderSelectionViewModel.RestoreDefaults();

            // Todo: (maybe) use .RestoreDefaults
            this.Analysis.Options = new MultiAlignAnalysisOptions();

            this.FeatureFindingSettingsViewModel.RestoreDefaults();
            this.AlignmentSettingsViewModel.RestoreDefaults();    

            // ToDo: use .RestoreDefaults
            this.ClusterSettingsViewModel = new ClusterSettingsViewModel(this.Analysis, this.Datasets, this.clusterViewFactory);

        }  

        private async void AsyncWorkflow()
        {
            await Task.Run(() => this.RunFullWorkflow());
        }

        private void RunFullWorkflow()
        {
            this.ShouldShowProgress = true;
            var filesSelected = this.FeatureFindingSettingsViewModel.Datasets.Where(ds => ds.IsSelected).ToList().Count != 0;
            var alignmentChosen =
                (this.AlignmentSettingsViewModel.ShouldAlignToBaseline && this.AlignmentSettingsViewModel.SelectedBaseline != null) ||
                (this.AlignmentSettingsViewModel.ShouldAlignToAMT && this.Analysis.MassTagDatabase != null);

            if (filesSelected && alignmentChosen)
            {
                var taskBarProgress = TaskBarProgress.GetInstance();
                taskBarProgress.TakeControl(this);
                taskBarProgress.ShowProgress(this, true);

                var progData = new ProgressData();
                IProgress<ProgressData> totalProgress = new Progress<ProgressData>(pd =>
                {
                    var prog = progData.UpdatePercent(pd.Percent).Percent;
                    this.ProgressTracker = (int)prog;
                    taskBarProgress.SetProgress(this, prog);
                });

                progData.StepRange(50);
                
                // Make copy of selected datasets at time of function call so all work is done on the same set of files
                // even if the user changes the selection while the workflow is running.
                var selectedDatasetsCopy = this.FeatureFindingSettingsViewModel.Datasets.Where(ds => ds.IsSelected).ToList();
                this.FeatureFindingSettingsViewModel.LoadFeatures(selectedDatasetsCopy, totalProgress);
                progData.StepRange(80);
                this.AlignmentSettingsViewModel.AlignToBaseline(selectedDatasetsCopy, totalProgress);
                progData.StepRange(100);
                this.ClusterSettingsViewModel.ClusterFeatures(totalProgress);
                this.ShouldShowProgress = false;

                taskBarProgress.ShowProgress(this, false);
                taskBarProgress.ReleaseControl(this);
            }
            else if (!filesSelected)
            {
                MessageBox.Show("No datasets selected.");
            }
            else
            {
                MessageBox.Show("Alignment settings not set.");
            }

        }
    }
}
