using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InformedProteomics.Backend.Utils;
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

        private int progressTracker;

        private bool shouldShowProgress;

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
            this.FeatureFindingSettingsViewModel = new FeatureFindingSettingsViewModel(Analysis, featureCache, Datasets);
            this.AlignmentSettingsViewModel = new AlignmentSettingsViewModel(Analysis, featureCache, Datasets);
            this.ClusterSettingsViewModel = new ClusterSettingsViewModel(Analysis, Datasets);
            ShouldShowProgress = false;
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
        public void SelectFiles()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                DefaultExt = ".raw|.csv",
                Filter = @"Supported Files|*.raw;*.csv;|Raw Files (*.raw)|*.raw|CSV Files (*.csv)|*.csv|Promex Files (*.ms1ft)|*.ms1ft;"
            };

            var result = openFileDialog.ShowDialog();
            if (result != null && result.Value)
            {
                var filePaths = openFileDialog.FileNames;
                var allFilesSelected = filePaths.Any(file => file.EndsWith(".raw")) &&
                                       filePaths.Any(file => file.EndsWith("_isos.csv") || file.EndsWith(".ms1ft"));
                if (!allFilesSelected)
                {
                    var statusMessage =
                        "MultiAlign Rogue requires at least a .raw file, an feature (isos or ms1ft) file, and a scans file.";
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

                viewmodel.StateChanged += (s, e) => this.serializerThrottler.Run(this.SaveProject);
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
        private async Task CreateNewProject()
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
                await this.LoadRogueProject(rogueProject, true);
                this.Serialize(newProjectViewModel.ProjectFilePath);
                this.ProjectPath = newProjectViewModel.ProjectFilePath;
                Directory.SetCurrentDirectory(Path.GetDirectoryName(ProjectPath));
                this.outputDirectory = newProjectViewModel.OutputDirectory;
            }
        }

        private async Task LoadRogueProject(RogueProject rogueProject, bool isNewProject)
        {
            this.Analysis = new MultiAlignAnalysis
            {
                DataProviders = this.SetupDataProviders(rogueProject.AnalysisPath, isNewProject),
                Options = rogueProject.MultiAlignAnalysisOptions
            };

            this.DataSelectionViewModel.Analysis = this.Analysis;
            this.Analysis.MetaData.Datasets.AddRange(rogueProject.Datasets);

            this.Analysis.MetaData.BaselineDataset = this.Analysis.MetaData.Datasets.FirstOrDefault(ds => ds.IsBaseline);

            this.featureCache.Providers = this.Analysis.DataProviders;
            this.m_config.AnalysisPath = rogueProject.AnalysisPath;
            this.UpdateDatasets();
            this.clusterViewFactory = new ClusterViewFactory(this.Analysis.DataProviders, rogueProject.ClusterViewerSettings, rogueProject.LayoutFilePath);
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

        private async Task LoadProject()
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

                await this.LoadRogueProject(rogueProject, false);
                this.outputDirectory = Path.GetDirectoryName(rogueProject.AnalysisPath);
                this.ProjectPath = openFileDialog.FileName;
            }
        }

        private void Serialize(string filePath)
        {
            var rogueProjectSerializer = new DataContractSerializer(typeof (RogueProject));
            var datasetInfoList = this.Datasets.Select(datasetInformation => datasetInformation.Dataset).ToList();
            ClusterViewerSettings clusterViewerSettings = null;
            if (this.clusterViewFactory is ClusterViewFactory)
            {
                var cvf = clusterViewFactory as ClusterViewFactory;
                if (cvf.ClusterViewModel != null)
                {
                    clusterViewerSettings = cvf.ClusterViewModel.ClusterPlotViewModel.ClusterViewerSettings;
                }
            }

            var rogueProject = new RogueProject
            {
                MultiAlignAnalysisOptions = this.Analysis.Options,
                ClusterViewerSettings = clusterViewerSettings ?? new ClusterViewerSettings(),
                Datasets = datasetInfoList,
                AnalysisPath = this.m_config.AnalysisPath
            };

            var xmlSettings = new XmlWriterSettings() { Indent = true, CloseOutput = true };

            using (var writer = XmlWriter.Create(File.Open(filePath, FileMode.Create), xmlSettings))
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

            Directory.SetCurrentDirectory(Path.GetDirectoryName(filePath));

            return rogueProject;
        }
        #endregion

        private void RestoreDefaultSettings()
        {
            if (
                MessageBox.Show("Are you sure you would like to reset all settings to their default values?",
                    "Restore Defaults", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Analysis.Options = new MultiAlignAnalysisOptions();
                this.FeatureFindingSettingsViewModel = new FeatureFindingSettingsViewModel(this.Analysis, this.featureCache, this.Datasets);
                this.AlignmentSettingsViewModel = new AlignmentSettingsViewModel(this.Analysis, this.featureCache, this.Datasets);
                this.ClusterSettingsViewModel = new ClusterSettingsViewModel(this.Analysis, this.Datasets, this.clusterViewFactory);   
            }
        }

        private async void AsyncWorkflow()
        {
            await Task.Run(() => RunFullWorkflow());
        }

        private void RunFullWorkflow()
        {
            ShouldShowProgress = true;
            bool filesSelected = featureFindingSettingsViewModel.Datasets.Where(ds => ds.IsSelected).ToList().Count != 0;
            bool alignmentChosen = (AlignmentSettingsViewModel.ShouldAlignToBaseline && AlignmentSettingsViewModel.SelectedBaseline != null) || 
                (AlignmentSettingsViewModel.ShouldAlignToAMT && Analysis.MassTagDatabase != null);
            if (filesSelected && alignmentChosen)
            {
                TaskBarProgressSingleton.TakeTaskbarControl(this);
                TaskBarProgressSingleton.ShowTaskBarProgress(this, true);

                var progData = new ProgressData();
                IProgress<ProgressData> totalProgress = new Progress<ProgressData>(pd =>
                {
                    var prog = progData.UpdatePercent(pd.Percent).Percent;
                    this.ProgressTracker = (int) prog;
                    TaskBarProgressSingleton.SetTaskBarProgress(this, prog);
                });

                progData.StepRange(50);
                List<DatasetInformationViewModel> selectedDatasetsCopy =
                    featureFindingSettingsViewModel.Datasets.Where(ds => ds.IsSelected).ToList();  //Make copy of selected datasets at time of function call so all work is done on the same set of files
                FeatureFindingSettingsViewModel.LoadFeatures(selectedDatasetsCopy, totalProgress); //even if the user changes the selection while the workflow is running.
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
