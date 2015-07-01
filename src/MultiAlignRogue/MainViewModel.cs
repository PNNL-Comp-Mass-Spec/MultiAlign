using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs;
using Ookii;
using MultiAlign;
using MultiAlign.Commands;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlign.ViewModels.Datasets;
using MultiAlign.ViewModels.Wizard;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using PNNLOmics.Algorithms.Alignment.LcmsWarp;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;

namespace MultiAlignRogue
{
    using MultiAlignRogue.DMS;

    using MessageBox = System.Windows.MessageBox;

    public class MainViewModel : INotifyPropertyChanged
    {


        public AnalysisDatasetSelectionViewModel DataSelectionViewModel;
        public MultiAlignAnalysis m_analysis { get; set;}
        VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog();
        public AnalysisConfig m_config { get; set;}
        public MultiAlignAnalysisOptions m_options { get; set;}
        public AnalysisOptionsViewModel m_AnalysisOptions { get; set;}
        private DatasetInformation m_baseline { get; set; }

        //private string[] files; 
        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set;}
        public FeatureLoader FeatureCache;
        public List<DatasetInformation> selectedFiles;
        public List<AlignmentType> CalibrationOptions { get; set; }
        private IWindowFactory msFeatureWindowFactory;

        public RelayCommand SelectFilesCommand { get; private set; }
        public RelayCommand FindMSFeaturesCommand { get; private set; }
        public RelayCommand PlotMSFeaturesCommand { get; private set; }
        public RelayCommand SearchDmsCommand { get; private set; }
        public RelayCommand AlignToBaselineCommand { get; private set; }
        public RelayCommand ClusterListCommand { get; private set; }
        public ICommand AddFolderCommand { get; private set; }

        public string inputFilePath { get; set; }
        public DataTable datasetInfo { get; set; }
        private FeatureDataAccessProviders Providers;
        private Dictionary<DatasetInformation, IList<UMCLight>> Features { get; set; }

        public int ProgressTracker { get; private set; }

 

        public MainViewModel()
        {
            m_config = new AnalysisConfig();
            m_analysis = new MultiAlignAnalysis();
            m_options = m_analysis.Options;
            m_config.AnalysisName = "Analysis";
            m_config.Analysis = m_analysis;
            m_AnalysisOptions = new AnalysisOptionsViewModel(m_options);
            
            SelectFilesCommand = new RelayCommand(SelectFiles);
            FindMSFeaturesCommand = new RelayCommand(LoadMSFeatures);
            PlotMSFeaturesCommand = new RelayCommand(PlotMSFeatures);
            AddFolderCommand = new BaseCommand(AddFolderDelegate, BaseCommand.AlwaysPass);
            DataSelectionViewModel = new AnalysisDatasetSelectionViewModel(m_config.Analysis);
            SearchDmsCommand = new RelayCommand(() => this.SearchDms());
            AlignToBaselineCommand = new RelayCommand(AlignToBaseline);
            ClusterListCommand = new RelayCommand(ShowClusterList);
            FeatureCache = new FeatureLoader { Providers = m_analysis.DataProviders };

            Features = new Dictionary<DatasetInformation, IList<UMCLight>>();
            this.selectedFiles = new List<DatasetInformation>();
            msFeatureWindowFactory = new MSFeatureViewFactory();
            Datasets = new ObservableCollection<DatasetInformationViewModel>();

            CalibrationOptions = new List<AlignmentType>();
            Enum.GetValues(typeof(AlignmentType)).Cast<AlignmentType>().ToList().ForEach(x => CalibrationOptions.Add(x));
        }


        #region Import Files
        public void SelectFiles()
        {
            var result = folderBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                inputFilePath = folderBrowser.SelectedPath;
                OnPropertyChanged("inputFilePath");
            }

        }

        public void AddFolderDelegate()
        {
            var supportedTypes = DatasetInformation.SupportedFileTypes;
            var extensions = new List<string>();

            supportedTypes.ForEach(x => extensions.Add("*" + x.Extension));

            if (inputFilePath == null)
            {
                ApplicationStatusMediator.SetStatus("Select a folder path first. File -> Select Files");
                System.Windows.MessageBox.Show("Select a folder path first. File -> Select Files");
                return;
            }

            if (!Directory.Exists(inputFilePath))
            {
                ApplicationStatusMediator.SetStatus("The directory specified does not exist.");
                System.Windows.MessageBox.Show("The directory specified does not exist.");
                return;
            }

            var files = DatasetSearcher.FindDatasets(inputFilePath,
                extensions,
                SearchOption.TopDirectoryOnly);
            DataSelectionViewModel.AddDatasets(files);
            m_config.AnalysisPath = inputFilePath;
            Providers = SetupDataProviders(true);
            m_analysis.DataProviders = Providers;
            UpdateDatasets();
            OnPropertyChanged("m_config");
            OnPropertyChanged("Datasets");
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
        #endregion


        public async void LoadMSFeatures()
        {
            await Task.Run(() => LoadFeatures());
        }

        private void LoadFeatures()
        {
            List<DatasetInformation> selectedFilesCopy = new List<DatasetInformation>();
            FeatureCache.Providers = m_analysis.DataProviders;
            foreach (var dataset in selectedFiles)
            {
                selectedFilesCopy.Add(dataset); //Prevents crashes from changing selected files while this thread is running
            }

            foreach (var file in selectedFilesCopy)
            {
                IProgress<int> progress = new Progress<int>(ReportProgess);
                var features = FeatureCache.LoadDataset(file, m_options.MsFilteringOptions, m_options.LcmsFindingOptions,
                    m_options.LcmsFilteringOptions);
                FeatureCache.CacheFeatures(features);
                
                file.FeaturesFound = true;
                OnPropertyChanged("m_analysis");
                progress.Report(0);
            }
        }


        public void PlotMSFeatures()
        {
            Features = new Dictionary<DatasetInformation, IList<UMCLight>>();
            foreach (var file in selectedFiles)
            {
                Features.Add(file, UmcLoaderFactory.LoadUmcFeatureData(file.Features.Path, file.DatasetId,
                Providers.FeatureCache));
            }
            msFeatureWindowFactory.CreateNewWindow(Features);  
        }

        public async void AsyncAlignToBaseline()
        {
            await Task.Run(() => AlignToBaseline());
        }

        private void AlignToBaseline()
        {
            System.Windows.MessageBox.Show("Working Command");
        }

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


        #region MS Feature Settings
        public double MassResolution
        {
            get { return m_options.InstrumentTolerances.Mass; }
            set
            {
                m_options.AlignmentOptions.MassTolerance = value;
                m_options.InstrumentTolerances.Mass = value;
                OnPropertyChanged("MassResolution");
            }
        }

        public double MinimumFeatureLength
        {
            get { return m_options.LcmsFilteringOptions.FeatureLengthRange.Minimum; }
            set
            {
                m_options.LcmsFilteringOptions.FeatureLengthRange.Minimum = value;
                m_options.LcmsFilteringOptions.FeatureLengthRange.Minimum = value;
                OnPropertyChanged("MinimumFeatureLength");
            }
        }

        public double MaximumFeatureLength
        {
            get { return m_options.LcmsFilteringOptions.FeatureLengthRange.Maximum; }
            set
            {
                m_options.LcmsFilteringOptions.FeatureLengthRange.Maximum = value;
                m_options.LcmsFilteringOptions.FeatureLengthRange.Maximum = value;
                OnPropertyChanged("MaximumFeatureLength");
            }
        }

        public double MinimumIntensity
        {
            get { return m_options.MsFilteringOptions.MinimumIntensity; }
            set
            {
                m_options.MsFilteringOptions.MinimumIntensity = value;
                OnPropertyChanged("MinimumIntensity");
            }
        }

        public double FragmentationTolerance
        {
            get { return m_options.InstrumentTolerances.FragmentationWindowSize; }
            set
            {
                m_options.InstrumentTolerances.FragmentationWindowSize = value;
                OnPropertyChanged("FragmentationTolerance");
            }
        }

        public double MaximumMz
        {
            get { return m_options.MsFilteringOptions.MzRange.Maximum; }
            set
            {
                m_options.MsFilteringOptions.MzRange.Maximum = value;
                OnPropertyChanged("MaximumMz");
            }
        }

        public double MinimumMz
        {
            get { return m_options.MsFilteringOptions.MzRange.Minimum; }
            set
            {
                m_options.MsFilteringOptions.MzRange.Minimum = value;
                OnPropertyChanged("MinimumMz");
            }
        }


        public double MaximumCharge
        {
            get { return m_options.MsFilteringOptions.ChargeRange.Maximum; }
            set
            {
                m_options.MsFilteringOptions.ChargeRange.Maximum = value;
                OnPropertyChanged("MaximumCharge");
            }
        }

        public double MinimumCharge
        {
            get { return m_options.MsFilteringOptions.ChargeRange.Minimum; }
            set
            {
                m_options.MsFilteringOptions.ChargeRange.Minimum = value;
                OnPropertyChanged("MinimumCharge");
            }
        }

        public double MinimumDeisotopingScore
        {
            get { return m_options.MsFilteringOptions.MinimumDeisotopingScore; }
            set
            {
                m_options.MsFilteringOptions.MinimumDeisotopingScore = value;
                OnPropertyChanged("MinimumDeisotopingScore");
            }
        }

        public bool ShouldUseChargeStateFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseChargeFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseChargeFilter = value;
                OnPropertyChanged("ShouldUseChargeStateFilter");
            }
        }

        public bool ShouldUseMzFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseMzFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseMzFilter = value;
                OnPropertyChanged("ShouldUseMzFilter");
            }
        }

        public bool ShouldUseIntensityFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseIntensityFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseIntensityFilter = value;
                OnPropertyChanged("ShouldUseIntensityFilter");
            }
        }

        public bool ShouldUseDeisotopingFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseDeisotopingFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseDeisotopingFilter = value;
                OnPropertyChanged("ShouldUseDeisotopingFilter");
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
        #endregion

        #region LC-MS Cluster Settings

        public DatasetInformation SelectedBaseline
        {
            get { return m_baseline; }
            set
            {
                if (m_baseline != value)
                {
                    if (value == null)
                    {
                        m_analysis.MetaData.BaselineDataset = null;
                    }
                    else
                    {
                        m_baseline = value;
                        m_analysis.MetaData.BaselineDataset = value;
                    }
                    OnPropertyChanged("SelectedBaseline");
                }
            }
        }

        public void ShowClusterList()
        {
            System.Windows.MessageBox.Show("Working Command");
        }

        public double MassTolerance
        {
            get { return m_options.LcmsClusteringOptions.InstrumentTolerances.Mass; }
            set
            {
                m_options.LcmsClusteringOptions.InstrumentTolerances.Mass = value;
                OnPropertyChanged("MassTolerance");
            }
        }

        public double NetTolerance
        {
            get { return m_options.LcmsClusteringOptions.InstrumentTolerances.Net; }
            set
            {
                m_options.LcmsClusteringOptions.InstrumentTolerances.Net = value;
                OnPropertyChanged("NetTolerance");
            }
        }

        public FeatureAlignmentType SelectedAlignmentAlgorithm 
        {
            get { return m_options.AlignmentOptions.AlignmentAlgorithm; }
            set
            {
                if (m_options.AlignmentOptions.AlignmentAlgorithm != value)
                {
                    m_options.AlignmentOptions.AlignmentAlgorithm = value;
                    OnPropertyChanged("SelectedAlignmentAlgorithm");
                }
            }
        }
        public LcmsFeatureClusteringAlgorithmType SelectedLcmsFeatureClusteringAlgorithm
        {
            get { return m_options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm; }
            set
            {
                if (m_options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm != value)
                {
                    m_options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm = value;
                    OnPropertyChanged("SelectedClusteringAlgorithm");
                }
            }
        }
        public AlignmentType SelectedCalibrationType
        {
            get { return m_options.AlignmentOptions.AlignmentType; }
            set
            {
                if (m_options.AlignmentOptions.AlignmentType != value)
                {
                    m_options.AlignmentOptions.AlignmentType = value;
                    OnPropertyChanged("SelectedCalibrationType");
                }
            }
        }
          
        
        #endregion 

        private void ReportProgess(int value)
        {
            this.ProgressTracker = value;
            OnPropertyChanged("ProgressTracker");
        }

        private async Task SearchDms()
        {
            var dmsLookupViewModel = new DmsLookupViewModel();
            var dialog = new DmsLookupView { DataContext = dmsLookupViewModel };
            dmsLookupViewModel.DatasetSelected += (o, e) => dialog.Close();
            dialog.ShowDialog();
            if (dmsLookupViewModel.Status)
            {
                this.inputFilePath = dmsLookupViewModel.OutputDirectory;
                this.AddFolderCommand.Execute(null);
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
