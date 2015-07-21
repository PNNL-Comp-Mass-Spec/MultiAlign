using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlign.ViewModels.Datasets;
using MultiAlign.ViewModels.Wizard;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using PNNLOmics.Algorithms.Alignment.LcmsWarp;
using PNNLOmics.Algorithms.Distance;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;

namespace MultiAlignRogue
{
    using DMS;

    using MessageBox = System.Windows.MessageBox;

    public class MainViewModel : ViewModelBase
    {
        #region Properties
        public AnalysisDatasetSelectionViewModel DataSelectionViewModel;
        public MultiAlignAnalysis m_analysis { get; set;}
        VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog();
        public AnalysisConfig m_config { get; set;}
        
        public MultiAlignAnalysisOptions m_options { get; set;}

        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set;}
        public FeatureLoader FeatureCache;


        public RelayCommand SelectFilesCommand { get; private set; }
        public RelayCommand SelectDirectoryCommand { get; private set; }
        public RelayCommand SearchDmsCommand { get; private set; }
        public RelayCommand AddFolderCommand { get; private set; }
        public RelayCommand SaveProjectCommand { get; private set; }
        public RelayCommand LoadProjectCommand { get; private set; }

        public DataTable datasetInfo { get; set; }
        private FeatureDataAccessProviders Providers;

        private FeatureFindingSettingsViewModel featureFindingSettingsViewModel;
        private AlignmentSettingsViewModel alignmentSettingsViewModel;
        private ClusterSettingsViewModel clusterSettingsViewModel;

        private IReadOnlyCollection<DatasetInformation> selectedDatasets;

        private string inputFilePath;

        public int ProgressTracker { get; private set; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            m_config = new AnalysisConfig();
            m_analysis = new MultiAlignAnalysis();
            m_options = m_analysis.Options;
            m_config.AnalysisName = "Analysis";
            m_config.Analysis = m_analysis;
           
            SelectFilesCommand = new RelayCommand(SelectFiles);
            SelectDirectoryCommand = new RelayCommand(SelectDirectory);
            AddFolderCommand = new RelayCommand(AddFolderDelegate, () => !string.IsNullOrWhiteSpace(this.InputFilePath) && Directory.Exists(this.InputFilePath));
            DataSelectionViewModel = new AnalysisDatasetSelectionViewModel(m_config.Analysis);
            SearchDmsCommand = new RelayCommand(SearchDms, () => this.ShowOpenFromDms);          
            SaveProjectCommand = new RelayCommand(SaveProject);
            LoadProjectCommand = new RelayCommand(LoadProject);
            
            FeatureCache = new FeatureLoader { Providers = m_analysis.DataProviders };
            this.SelectedDatasets = new List<DatasetInformation>();
            Datasets = new ObservableCollection<DatasetInformationViewModel>();

            FeatureCache.Providers = m_analysis.DataProviders;
            this.FeatureFindingSettingsViewModel = new FeatureFindingSettingsViewModel(m_analysis, FeatureCache);
            this.AlignmentSettingsViewModel = new AlignmentSettingsViewModel(m_analysis, FeatureCache);
            this.ClusterSettingsViewModel = new ClusterSettingsViewModel(m_analysis);
        }
        #endregion

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

        public IReadOnlyCollection<DatasetInformation> SelectedDatasets
        {
            get { return this.selectedDatasets; }
            set
            {
                this.selectedDatasets = value;
                this.RaisePropertyChanged("SelectedDatasets", null, value, true);
            }
        }

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
            if (result == DialogResult.OK)
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

                this.AddDataset(this.GetInputFilesFromPath(filePaths), Path.GetDirectoryName(filePaths[0]));
            }
        }

        public void SelectDirectory()
        {
            var result = folderBrowser.ShowDialog();

            if (result == DialogResult.OK)
            {
                InputFilePath = folderBrowser.SelectedPath;
            }
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
            this.AddDataset(files, InputFilePath);
        }

        public void UpdateDatasets()
        {
            Datasets.Clear();
            foreach (var info in m_analysis.MetaData.Datasets)
            {
                var viewmodel = new DatasetInformationViewModel(info);
                Datasets.Add(viewmodel);
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


        private void ReportProgess(int value)
        {
            this.ProgressTracker = value;
            this.RaisePropertyChanged("ProgressTracker");
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
            var dmsLookupViewModel = new DmsLookupViewModel();
            var dialog = new DmsLookupView { DataContext = dmsLookupViewModel };
            dmsLookupViewModel.DatasetSelected += (o, e) => dialog.Close();
            dialog.ShowDialog();
            if (!dmsLookupViewModel.Status) return;

            if (dmsLookupViewModel.ShouldCopyFiles)
            {
                this.InputFilePath = dmsLookupViewModel.OutputDirectory;
                this.AddFolderCommand.Execute(null);
            }
            else
            {
                var filePaths = dmsLookupViewModel.SelectedDataset.GetAvailableFiles();
                if (filePaths.Count > 0)
                {
                    this.AddDataset(this.GetInputFilesFromPath(filePaths), dmsLookupViewModel.OutputDirectory);
                }
            }
        }

        private void AddDataset(List<InputFile> files, string analysisPath)
        {
            DataSelectionViewModel.AddDatasets(files);
            m_config.AnalysisPath = analysisPath;
            Providers = SetupDataProviders(true);
            m_analysis.DataProviders = Providers;
            UpdateDatasets();
            this.RaisePropertyChanged("m_config");
            this.RaisePropertyChanged("Datasets");
        }

        private void SaveProject()
        {
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".xml",
                Filter = @"Supported Files|*.xml"
            };

            var result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.Serialize(saveFileDialog.FileName);
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
            if (result == DialogResult.OK)
            {
                this.Deserialize(openFileDialog.FileName);

                Providers = SetupDataProviders(string.IsNullOrEmpty(this.m_config.AnalysisPath));
                this.FeatureCache.Providers = Providers;
                m_analysis.DataProviders = Providers;
                this.UpdateDatasets();
                this.FeatureFindingSettingsViewModel = new FeatureFindingSettingsViewModel(m_analysis, this.FeatureCache);
                this.AlignmentSettingsViewModel = new AlignmentSettingsViewModel(m_analysis, this.FeatureCache);
            }
        }

        private void Serialize(string filePath)
        {
            var rogueProjectSerializer = new DataContractSerializer(typeof (RogueProject));
            var datasetInfoList = this.Datasets.Select(datasetInformation => datasetInformation.Dataset).ToList();
            var rogueProject = new RogueProject
            {
                MultiAlignAnalysisOptions = this.m_analysis.Options,
                Datasets = datasetInfoList,
                AnalysisPath = this.m_config.AnalysisPath
            };
            using (var writer = File.Open(filePath, FileMode.OpenOrCreate))
            {
                rogueProjectSerializer.WriteObject(writer, rogueProject);
            }
        }

        private void Deserialize(string filePath)
        {
            var rogueProjectSerializer = new DataContractSerializer(typeof(RogueProject));

            using (var reader = File.Open(filePath, FileMode.Open))
            {
                try
                {
                    RogueProject rogueProject = (RogueProject) rogueProjectSerializer.ReadObject(reader);
                    this.m_analysis.Options = rogueProject.MultiAlignAnalysisOptions;                   
                    foreach (var dataset in rogueProject.Datasets)
                    {
                        this.m_analysis.MetaData.Datasets.Add(dataset);
                    }

                    this.m_config.AnalysisPath = rogueProject.AnalysisPath;
                }
                catch (InvalidCastException)
                {
                    MessageBox.Show("Could not deserialize analysis options.");
                }
            }
        }
    }
}
