using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InformedProteomics.Backend.Utils;
using Microsoft.Win32;

using MultiAlign.Data;
using MultiAlign.ViewModels.Wizard;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Options;
using MultiAlignRogue.Utils;
using MultiAlignRogue.ViewModels;
using Ookii.Dialogs.Wpf;

namespace MultiAlignRogue
{
    using System.Windows;

    using DMS;

    using MultiAlignCore.Algorithms.Options;
    using MultiAlignCore.Extensions;

    using MultiAlignRogue.Alignment;
    using MultiAlignRogue.AMTMatching;
    using MultiAlignRogue.Clustering;
    using MultiAlignRogue.Feature_Finding;

    using MessageBox = System.Windows.MessageBox;

    public class MainViewModel : ViewModelBase
    {
        #region Private Data Members
        private MultiAlignAnalysis analysis;

        public AnalysisDatasetSelectionViewModel DataSelectionViewModel;

        private readonly Throttler serializerThrottler;

        private readonly AnalysisConfig m_config;
        private readonly FeatureLoader featureCache;

        private DataTable datasetInfo;
        private FeatureDataAccessProviders providers;

        private DataLoadingSettingsViewModel dataLoadingSettingsViewModel;
        private FeatureFindingSettingsViewModel featureFindingSettingsViewModel;
        private AlignmentSettingsViewModel alignmentSettingsViewModel;
        private ClusterSettingsViewModel clusterSettingsViewModel;
        private StacSettingsViewModel stacSettingsViewModel;

        private TaskBarProgressSingleton taskBarProgressSingleton;

        private IClusterViewFactory clusterViewFactory;

        private IReadOnlyCollection<DatasetInformationViewModel> selectedDatasets;

        private string inputFilePath;

        private string projectPath;

        private string windowTitle;

        private string outputDirectory;

        private string lastInputDirectory;

        private string lastProjectDirectory;

        private string lastOutputDirectory;

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
            m_config = new AnalysisConfig();
            Analysis = new MultiAlignAnalysis();
            m_config.AnalysisName = "Analysis.db3";
            m_config.Analysis = Analysis;

            this.WindowTitle = "MultiAlign Rogue";

            this.serializerThrottler = new Throttler(TimeSpan.FromSeconds(1));

            DataSelectionViewModel = new AnalysisDatasetSelectionViewModel(Analysis);

            SelectFilesCommand = new RelayCommand(SelectFiles, () => !string.IsNullOrWhiteSpace(this.ProjectPath));
            SelectDirectoryCommand = new RelayCommand(SelectDirectory, () => !string.IsNullOrWhiteSpace(this.ProjectPath));
            AddFolderCommand = new RelayCommand(AddFolderDelegate, () => !string.IsNullOrWhiteSpace(this.InputFilePath) && Directory.Exists(this.InputFilePath) && !string.IsNullOrWhiteSpace(this.ProjectPath));
            SearchDmsCommand = new RelayCommand(SearchDms, () => this.ShowOpenFromDms && !string.IsNullOrWhiteSpace(this.ProjectPath));
            CreateNewProjectCommand = new RelayCommand(async () => await this.CreateNewProject());
            SaveProjectCommand = new RelayCommand(SaveProject, () => !string.IsNullOrWhiteSpace(this.ProjectPath));
            LoadProjectCommand = new RelayCommand(async () => await LoadProject());
            SaveAsProjectCommand = new RelayCommand(this.SaveProjectAs, () => !string.IsNullOrWhiteSpace(this.ProjectPath));

            RestoreDefaultSettingsCommand = new RelayCommand(this.RestoreDefaultSettings);

            RunFullWorkflowCommand = new RelayCommand(this.AsyncWorkflow);

            featureCache = new FeatureLoader { Providers = Analysis.DataProviders };
            Datasets = new ObservableCollection<DatasetInformationViewModel>();

            taskBarProgressSingleton = new TaskBarProgressSingleton();

            featureCache.Providers = Analysis.DataProviders;
            this.DataLoadingSettingsViewModel = new DataLoadingSettingsViewModel(Analysis);
            this.FeatureFindingSettingsViewModel = new FeatureFindingSettingsViewModel(Analysis, featureCache, Datasets);
            this.AlignmentSettingsViewModel = new AlignmentSettingsViewModel(Analysis, featureCache, Datasets);
            this.ClusterSettingsViewModel = new ClusterSettingsViewModel(Analysis, Datasets);
            ShouldShowProgress = false;

            RegistryLoadSettings();
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

        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set; }

        public bool ShouldShowProgress
        {
            get
            { return shouldShowProgress; }
            set
            {
                if (this.shouldShowProgress != value)
                {
                    this.shouldShowProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

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

        public DataLoadingSettingsViewModel DataLoadingSettingsViewModel
        {
            get { return this.dataLoadingSettingsViewModel; }
            private set
            {
                this.dataLoadingSettingsViewModel = value;
                this.RaisePropertyChanged();
            }
        }

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

            if (!string.IsNullOrWhiteSpace(lastInputDirectory))
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

            if (!string.IsNullOrWhiteSpace(lastInputDirectory))
            {
                folderBrowser.SelectedPath = this.lastInputDirectory;
            }

            var result = folderBrowser.ShowDialog();
            if (result != true)
            {
                return;
            }

            InputFilePath = folderBrowser.SelectedPath;
            this.lastInputDirectory = Path.GetDirectoryName(InputFilePath);

            this.AddFolderCommand.Execute(null);
        }

        public void AddFolderDelegate()
        {
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

            this.AddDatasets(InputFilePath);
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

        public async Task UpdateDatasets()
        {
            Datasets.Clear();
            var i = 0;
            foreach (var info in Analysis.MetaData.Datasets)
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
                        }
                    }
                };

                viewmodel.StateChanged += (s, e) => this.serializerThrottler.Run(this.SaveProject);
                Datasets.Add(viewmodel);
            }

            await this.LoadRawData(this.Datasets);

            this.RaisePropertyChanged("m_config");
        }

        private async Task AddDatasets(string folderPath)
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
            await this.UpdateDatasets();
        }

        private async Task AddDatasets(IEnumerable<string> files)
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
            await this.UpdateDatasets();
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
        private FeatureDataAccessProviders SetupDataProviders(bool createNewDatabase)
        {
            FeatureDataAccessProviders dataProviders;
            Logger.PrintMessage("Setting up data providers for caching and storage.");
            try
            {
                var path = AnalysisPathUtils.BuildAnalysisName(m_config.AnalysisPath, m_config.AnalysisName);
                dataProviders = SetupDataProviders(path, createNewDatabase);
            }
            catch (IOException ex)
            {
                Logger.PrintMessage(ex.Message);
                Logger.PrintMessage(ex.StackTrace);
                throw;
            }
            return dataProviders;
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
        private async Task CreateNewProject()
        {
            var success = false;
            var newProjectViewModel = new NewProjectViewModel();
            var dialog = new NewProjectWindow { DataContext = newProjectViewModel };

#if (!DEBUG)
            try
            {
#endif
                newProjectViewModel.LastInputDirectory = lastInputDirectory;
                newProjectViewModel.LastProjectDirectory = lastProjectDirectory;
                newProjectViewModel.LastOutputDirectory = lastOutputDirectory;

                newProjectViewModel.Success += (s, e) =>
                {
                    success = true;
                    dialog.Close();
                };
                dialog.ShowDialog();

                if (success)
                {
                    this.ShowSplash = false;
                    m_config.AnalysisName = newProjectViewModel.ProjectFilePath;
                    ProjectPath = newProjectViewModel.ProjectFilePath;
                    this.outputDirectory = newProjectViewModel.OutputDirectory;
                    await this.LoadRogueProject(true, newProjectViewModel.Datasets.Select(x => x.Dataset).ToList());
                    
                    this.PersistProject();

                    lastInputDirectory = newProjectViewModel.LastInputDirectory;
                    lastProjectDirectory = newProjectViewModel.LastProjectDirectory;
                    this.lastOutputDirectory = newProjectViewModel.LastOutputDirectory;

                    RegistrySaveSettings();
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

        private async Task LoadRogueProject(bool isNewProject, List<DatasetInformation> datasets = null)
        {
            Directory.SetCurrentDirectory(this.outputDirectory);
            this.Analysis = new MultiAlignAnalysis
            {
                DataProviders = this.SetupDataProviders(this.ProjectPath, isNewProject),
            };

            var dbOptions = this.Analysis.DataProviders.OptionsDao.FindAll();
            this.Analysis.Options = OptionsTransformer.ListToProperties(dbOptions);

            if (datasets != null)
            {
                this.analysis.MetaData.Datasets.AddRange(datasets);
            }

            //Prevent updates of ViewModels (that we will recreate anyway) while loading datasets
            this.clusterViewFactory = null;
            this.DataLoadingSettingsViewModel = null;
            this.FeatureFindingSettingsViewModel = null;
            this.AlignmentSettingsViewModel = null;
            this.StacSettingsViewModel = null;

            this.DataSelectionViewModel.Analysis = this.Analysis;
            this.Analysis.MetaData.Datasets.AddRange(this.Analysis.DataProviders.DatasetCache.FindAll());

            this.Analysis.MetaData.BaselineDataset = this.Analysis.MetaData.Datasets.FirstOrDefault(ds => ds.IsBaseline);

            this.featureCache.Providers = this.Analysis.DataProviders;
            this.m_config.AnalysisPath = this.ProjectPath;
            await this.UpdateDatasets();

            this.clusterViewFactory = new ClusterViewFactory(this.Analysis.DataProviders);

            this.DataLoadingSettingsViewModel = new DataLoadingSettingsViewModel(this.Analysis);
            this.FeatureFindingSettingsViewModel = new FeatureFindingSettingsViewModel(this.Analysis, this.featureCache, this.Datasets);
            this.AlignmentSettingsViewModel = new AlignmentSettingsViewModel(this.Analysis, this.featureCache, this.Datasets);
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

            if (!string.IsNullOrWhiteSpace(lastProjectDirectory))
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

            if (!string.IsNullOrWhiteSpace(outputDirectory))
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

            await this.LoadRogueProject(false);
        }

        private void PersistProject()
        {
            this.Analysis.DataProviders.DatabaseLock.EnterWriteLock();
            this.Analysis.DataProviders.DatasetCache.AddAll(this.Datasets.Select(d => d.Dataset).ToList());
            this.Analysis.DataProviders.OptionsDao.AddAll(OptionsTransformer.PropertiesToList(this.Analysis.Options));
            this.Analysis.DataProviders.DatabaseLock.ExitWriteLock();
        }

        private async Task LoadRawData(IEnumerable<DatasetInformationViewModel> datasets)
        {
            foreach (var dataset in datasets)
            {
                if (dataset.Dataset.RawFile != null && File.Exists(dataset.Dataset.RawFile.Path))
                {
                    var finalDatasetState = dataset.DatasetState == DatasetInformationViewModel.DatasetStates.Waiting ?
                        DatasetInformationViewModel.DatasetStates.Loaded : dataset.DatasetState;

                    dataset.DatasetState = DatasetInformationViewModel.DatasetStates.LoadingRawData;
                    var progress = new Progress<ProgressData>(pd => dataset.Progress = pd.Percent);
                    var provider = this.analysis.DataProviders.ScanSummaryProviderCache.GetScanSummaryProvider(
                        dataset.Dataset.RawFile.Path,
                        dataset.DatasetId);
                    await provider.InitializeAsync(progress);
                    dataset.DatasetState = finalDatasetState;
                }
            }
        }
        #endregion

        /// <summary>
        /// Load settings from the registry
        /// </summary>
        private void RegistryLoadSettings()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            if (currentDirectory == Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
            {
                currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            lastInputDirectory = RegistryReadValue("LastInputDirectory", currentDirectory);
            lastProjectDirectory = RegistryReadValue("LastProjectDirectory", currentDirectory);
            lastOutputDirectory = RegistryReadValue("LastOutputDirectory", currentDirectory);
        }

        /// <summary>
        /// Save settings to the registry
        /// </summary>
        private void RegistrySaveSettings()
        {
            var success = RegistrySaveValue("LastInputDirectory", lastInputDirectory);
            if (!success)
                return;

            RegistrySaveValue("LastProjectDirectory", lastProjectDirectory);
            RegistrySaveValue("LastOutputDirectory", lastOutputDirectory);
        }

        /// <summary>
        /// Read a value from the registry
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="valueIfMissing"></param>
        /// <returns></returns>
        private string RegistryReadValue(string keyName, string valueIfMissing)
        {
            var currentTask = "Initializing";

            try
            {
                currentTask = @"OpenSubKey Software\PNNL\MultiAlign";
                var regKey = Registry.CurrentUser.OpenSubKey(@"Software\PNNL\MultiAlign", false);

                if (regKey == null)
                {
                    // Key not found
                    return valueIfMissing;
                }

                var value = regKey.GetValue(keyName);

                if (value == null)
                {
                    // Entry not found
                    return valueIfMissing;
                }

                return value.ToString();
            }
            catch (Exception ex)
            {
                Logger.PrintMessage(string.Format("Error reading from the registry, {0}: {1}", currentTask, ex.Message));
            }

            return valueIfMissing;
        }

        /// <summary>
        /// Write a value to the registry
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool RegistrySaveValue(string keyName, string value)
        {
            var currentTask = "Initializing";

            try
            {
                currentTask = "OpenSubKey Software";
                var regSoftware = Registry.CurrentUser.OpenSubKey("Software", true);

                if (regSoftware == null)
                {
                    Logger.PrintMessage(string.Format("Error opening registry key {0}; access denied?", @"HKEY_CURRENT_USER\Software"));
                    return false;
                }
                
                currentTask = @"Open Software\PNNL";
                var regPNNL = regSoftware.CreateSubKey("PNNL");
                if (regPNNL == null)
                {
                    Logger.PrintMessage(string.Format("Error opening registry key {0}; access denied?", @"HKEY_CURRENT_USER\Software\PNNL"));
                    return false;
                }

                currentTask = @"Software\PNNL\MultiAlign";
                var subKey = regPNNL.CreateSubKey("MultiAlign");

                if (subKey == null)
                {
                    Logger.PrintMessage(string.Format("Error opening registry key {0}; access denied?", @"HKEY_CURRENT_USER\Software\PNNL\MultiAlign"));
                    return false;
                }

                subKey.SetValue(keyName, value);
                return true;
            }
            catch (Exception ex)
            {
                Logger.PrintMessage(string.Format("Error writing to the registry, {0}: {1}", currentTask, ex.Message));
            }

            return false;
        }

        private void RestoreDefaultSettings()
        {
            if (MessageBox.Show("Are you sure you would like to reset all settings to their default values?",
                                "Restore Defaults", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            this.DataLoadingSettingsViewModel.RestoreDefaults();

            // Todo: (maybe) use .RestoreDefaults
            this.Analysis.Options = new MultiAlignAnalysisOptions();

            this.FeatureFindingSettingsViewModel.RestoreDefaults();
            this.AlignmentSettingsViewModel.RestoreDefaults();    

            // ToDo: use .RestoreDefaults
            this.ClusterSettingsViewModel = new ClusterSettingsViewModel(this.Analysis, this.Datasets, this.clusterViewFactory);
            //this.ClusterSettingsViewModel.RestoreDefaults();

        }  

        private async void AsyncWorkflow()
        {
            await Task.Run(() => RunFullWorkflow());
        }

        private void RunFullWorkflow()
        {
            ShouldShowProgress = true;
            var filesSelected = featureFindingSettingsViewModel.Datasets.Where(ds => ds.IsSelected).ToList().Count != 0;
            var alignmentChosen =
                (AlignmentSettingsViewModel.ShouldAlignToBaseline && AlignmentSettingsViewModel.SelectedBaseline != null) ||
                (AlignmentSettingsViewModel.ShouldAlignToAMT && Analysis.MassTagDatabase != null);

            if (filesSelected && alignmentChosen)
            {
                TaskBarProgressSingleton.TakeTaskbarControl(this);
                TaskBarProgressSingleton.ShowTaskBarProgress(this, true);

                var progData = new ProgressData();
                IProgress<ProgressData> totalProgress = new Progress<ProgressData>(pd =>
                {
                    var prog = progData.UpdatePercent(pd.Percent).Percent;
                    this.ProgressTracker = (int)prog;
                    TaskBarProgressSingleton.SetTaskBarProgress(this, prog);
                });

                progData.StepRange(50);
                
                // Make copy of selected datasets at time of function call so all work is done on the same set of files
                // even if the user changes the selection while the workflow is running.
                var selectedDatasetsCopy = featureFindingSettingsViewModel.Datasets.Where(ds => ds.IsSelected).ToList();
                FeatureFindingSettingsViewModel.LoadFeatures(selectedDatasetsCopy, totalProgress);
                progData.StepRange(80);
                AlignmentSettingsViewModel.AlignToBaseline(selectedDatasetsCopy, totalProgress);
                progData.StepRange(100);
                ClusterSettingsViewModel.ClusterFeatures(totalProgress);
                ShouldShowProgress = false;

                TaskBarProgressSingleton.ShowTaskBarProgress(this, false);
                TaskBarProgressSingleton.ReleaseTaskbarControl(this);
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
