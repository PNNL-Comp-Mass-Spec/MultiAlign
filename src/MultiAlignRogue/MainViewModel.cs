﻿using System;
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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs;
using Ookii;
using MultiAlign;
using MultiAlign.Commands;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlign.ViewModels.Datasets;
using MultiAlign.ViewModels.Wizard;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Hibernate;
using MultiAlignCore.IO.InputFiles;
using PNNLOmics.Algorithms.Alignment.LcmsWarp;
using PNNLOmics.Algorithms.Distance;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;

namespace MultiAlignRogue
{
    using MultiAlignRogue.DMS;

    using MessageBox = System.Windows.MessageBox;

    public class MainViewModel : ViewModelBase
    {
        #region Properties
        public AnalysisDatasetSelectionViewModel DataSelectionViewModel;
        public MultiAlignAnalysis m_analysis { get; set;}
        VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog();
        public AnalysisConfig m_config { get; set;}
        public AlgorithmProvider m_algorithms { get; set; }
        public MultiAlignAnalysisOptions m_options { get; set;}
        public AnalysisOptionsViewModel m_AnalysisOptions { get; set;}
        private DatasetInformation m_baseline { get; set; }
        private AlgorithmBuilder builder { get; set; }
        private AlgorithmProvider algorithms { get; set; }
        private LCMSFeatureAligner aligner { get; set; }

        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set;}
        public FeatureLoader FeatureCache;
        public List<DatasetInformation> selectedFiles;
        public List<AlignmentType> CalibrationOptions { get; set; }
        public List<DistanceMetric> DistanceMetrics { get; set; }
        public List<ClusterCentroidRepresentation> CentroidRepresentations { get; set; } 
        private IFeatureWindowFactory msFeatureWindowFactory;
        private IAlignmentWindowFactory alignmentWindowFactory;

        public RelayCommand SelectFilesCommand { get; private set; }
        public RelayCommand SelectDirectoryCommand { get; private set; }
        public RelayCommand FindMSFeaturesCommand { get; private set; }
        public RelayCommand PlotMSFeaturesCommand { get; private set; }
        public RelayCommand SearchDmsCommand { get; private set; }
        public RelayCommand AlignToBaselineCommand { get; private set; }
        public RelayCommand DisplayAlignmentCommand { get; private set; }
        public RelayCommand AddFolderCommand { get; private set; }
        public RelayCommand ClusterFeaturesCommand { get; private set; }
        public RelayCommand DisplayClustersCommand { get; private set; }

        public DataTable datasetInfo { get; set; }
        private FeatureDataAccessProviders Providers;
        private Dictionary<DatasetInformation, IList<UMCLight>> Features { get; set; }
        private List<classAlignmentData> AlignmentInformation { get; set; } 

        public int ProgressTracker { get; private set; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            m_config = new AnalysisConfig();
            m_algorithms = new AlgorithmProvider();
            m_analysis = new MultiAlignAnalysis();
            m_options = m_analysis.Options;
            m_config.AnalysisName = "Analysis";
            m_config.Analysis = m_analysis;
            m_AnalysisOptions = new AnalysisOptionsViewModel(m_options);
            builder = new AlgorithmBuilder();
            aligner = new LCMSFeatureAligner();
            
            SelectFilesCommand = new RelayCommand(SelectFiles);
            SelectDirectoryCommand = new RelayCommand(SelectDirectory);
            FindMSFeaturesCommand = new RelayCommand(LoadMSFeatures, () => this.selectedFiles != null && this.selectedFiles.Count > 0 && this.selectedFiles.Any(file => !file.DoingWork));
            PlotMSFeaturesCommand = new RelayCommand(async () => await PlotMSFeatures(), () => this.selectedFiles.Any(file => file.FeaturesFound));
            AddFolderCommand = new RelayCommand(AddFolderDelegate, () => !string.IsNullOrWhiteSpace(this.InputFilePath) && Directory.Exists(this.InputFilePath));
            DataSelectionViewModel = new AnalysisDatasetSelectionViewModel(m_config.Analysis);
            SearchDmsCommand = new RelayCommand(() => this.SearchDms(), () => this.ShowOpenFromDms);
            AlignToBaselineCommand = new RelayCommand(AsyncAlignToBaseline);
            DisplayAlignmentCommand = new RelayCommand(DisplayAlignment);
            ClusterFeaturesCommand = new RelayCommand(AsyncClusterFeatures);
            DisplayClustersCommand = new RelayCommand(DisplayFeatures);
            
            FeatureCache = new FeatureLoader { Providers = m_analysis.DataProviders };
            Features = new Dictionary<DatasetInformation, IList<UMCLight>>();
            this.selectedFiles = new List<DatasetInformation>();
            msFeatureWindowFactory = new MSFeatureViewFactory();
            alignmentWindowFactory = new AlignmentViewFactory();
            Datasets = new ObservableCollection<DatasetInformationViewModel>();
            AlignmentInformation = new List<classAlignmentData>();

            CalibrationOptions = new List<AlignmentType>();
            Enum.GetValues(typeof(AlignmentType)).Cast<AlignmentType>().ToList().ForEach(x => CalibrationOptions.Add(x));
            DistanceMetrics = new List<DistanceMetric>();
            Enum.GetValues(typeof(DistanceMetric)).Cast<DistanceMetric>().ToList().ForEach(x => DistanceMetrics.Add(x));
            CentroidRepresentations = new List<ClusterCentroidRepresentation>();
            Enum.GetValues(typeof(ClusterCentroidRepresentation)).Cast<ClusterCentroidRepresentation>().ToList().ForEach(x => CentroidRepresentations.Add(x));
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
            ////DataSelectionViewModel.AddDatasets(files);
            ////m_config.AnalysisPath = inputFilePath;
            ////Providers = SetupDataProviders(true);
            ////m_analysis.DataProviders = Providers;
            ////UpdateDatasets();
            ////OnPropertyChanged("m_config");
            ////OnPropertyChanged("Datasets");
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

        #region Feature Loading
        public async void LoadMSFeatures()
        {
            await Task.Run(() => LoadFeatures());
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

        private void LoadFeatures()
        {
            List<DatasetInformation> selectedFilesCopy = new List<DatasetInformation>();
            FeatureCache.Providers = m_analysis.DataProviders;
            foreach (var dataset in selectedFiles)
            {
                selectedFilesCopy.Add(dataset); //Prevents crashes from changing selected files while this thread is running
            }

            foreach (var file in selectedFilesCopy.Where(file => !file.DoingWork)) // Do not try to run on files already loading features.
            {
                file.DoingWork = true;
                ThreadSafeDispatcher.Invoke(() => PlotMSFeaturesCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => FindMSFeaturesCommand.RaiseCanExecuteChanged());
                IProgress<int> progress = new Progress<int>(ReportProgess);
                var features = FeatureCache.LoadDataset(file, m_options.MsFilteringOptions, m_options.LcmsFindingOptions,
                    m_options.LcmsFilteringOptions);
                FeatureCache.CacheFeatures(features);
                
                file.FeaturesFound = true;
                this.RaisePropertyChanged("m_analysis");
                progress.Report(0);

                file.DoingWork = false;
                ThreadSafeDispatcher.Invoke(() => PlotMSFeaturesCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => FindMSFeaturesCommand.RaiseCanExecuteChanged());
            }
        }
        #endregion

        #region Plot Features
        public async Task PlotMSFeatures()
        {
            try
            {
                Features = new Dictionary<DatasetInformation, IList<UMCLight>>();
                foreach (var file in selectedFiles.Where(file => file.FeaturesFound)) // Select only datasets with features.
                {
                    var features = await Task.Run(() => UmcLoaderFactory.LoadUmcFeatureData(file.Features.Path, file.DatasetId,
                            Providers.FeatureCache));

                    Features.Add(file, features);
                }
                msFeatureWindowFactory.CreateNewWindow(Features);
            }
            catch
            {
                MessageBox.Show("Feature cache currently being accessed. Try again in a few moments");
            }
        }
        #endregion

        #region Align Features
        public async void AsyncAlignToBaseline()
        {
            await Task.Run(() => AlignToBaseline());
        }

        private void AlignToBaseline()
        {
            if (SelectedBaseline != null && SelectedBaseline.FeaturesFound)
            {
                //Update algorithms and providers
                FeatureCache.Providers = m_analysis.DataProviders;
                algorithms = builder.GetAlgorithmProvider(m_options);
                aligner.m_algorithms = algorithms;

                var baselineFeatures = FeatureCache.LoadDataset(m_baseline, m_options.MsFilteringOptions,
                    m_options.LcmsFindingOptions, m_options.LcmsFilteringOptions);
                var alignmentData = new AlignmentDAOHibernate();
                alignmentData.ClearAll();

                List<DatasetInformation> selectedFilesCopy = new List<DatasetInformation>();
                foreach (var dataset in selectedFiles)
                {
                    selectedFilesCopy.Add(dataset); //Prevents crashes from changing selected files while this thread is running
                }

                foreach (var file in selectedFilesCopy)
                {
                    if (file.IsBaseline || !file.FeaturesFound) continue;
                    var features = FeatureCache.LoadDataset(file, m_options.MsFilteringOptions,
                        m_options.LcmsFindingOptions, m_options.LcmsFilteringOptions);
                    var alignment = aligner.AlignToDataset(ref features, baselineFeatures, file, m_baseline);
                    //Check if there is information from a previous alignment for this dataset. If so, replace it. If not, just add the new one.
                    var priorAlignment = from x in AlignmentInformation where x.DatasetID == alignment.DatasetID select x;
                    if (priorAlignment.Any())
                    {
                        AlignmentInformation.Remove(priorAlignment.Single());
                        AlignmentInformation.Add(alignment);
                    }
                    else
                    {
                        AlignmentInformation.Add(alignment);
                    }

                    FeatureCache.CacheFeatures(features);
                    file.IsAligned = true;
                    this.RaisePropertyChanged("m_analysis");
                }
            }
            else
            {
                MessageBox.Show("Please select a baseline with detected features.");
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

        #region MS Feature Settings
        public double MassResolution
        {
            get { return m_options.InstrumentTolerances.Mass; }
            set
            {
                m_options.AlignmentOptions.MassTolerance = value;
                m_options.InstrumentTolerances.Mass = value;
                this.RaisePropertyChanged("MassResolution");
            }
        }

        public double MinimumFeatureLength
        {
            get { return m_options.LcmsFilteringOptions.FeatureLengthRange.Minimum; }
            set
            {
                m_options.LcmsFilteringOptions.FeatureLengthRange.Minimum = value;
                m_options.LcmsFilteringOptions.FeatureLengthRange.Minimum = value;
                this.RaisePropertyChanged("MinimumFeatureLength");
            }
        }

        public double MaximumFeatureLength
        {
            get { return m_options.LcmsFilteringOptions.FeatureLengthRange.Maximum; }
            set
            {
                m_options.LcmsFilteringOptions.FeatureLengthRange.Maximum = value;
                m_options.LcmsFilteringOptions.FeatureLengthRange.Maximum = value;
                this.RaisePropertyChanged("MaximumFeatureLength");
            }
        }

        public double MinimumIntensity
        {
            get { return m_options.MsFilteringOptions.MinimumIntensity; }
            set
            {
                m_options.MsFilteringOptions.MinimumIntensity = value;
                this.RaisePropertyChanged("MinimumIntensity");
            }
        }

        public double FragmentationTolerance
        {
            get { return m_options.InstrumentTolerances.FragmentationWindowSize; }
            set
            {
                m_options.InstrumentTolerances.FragmentationWindowSize = value;
                this.RaisePropertyChanged("FragmentationTolerance");
            }
        }

        public double MaximumMz
        {
            get { return m_options.MsFilteringOptions.MzRange.Maximum; }
            set
            {
                m_options.MsFilteringOptions.MzRange.Maximum = value;
                this.RaisePropertyChanged("MaximumMz");
            }
        }

        public double MinimumMz
        {
            get { return m_options.MsFilteringOptions.MzRange.Minimum; }
            set
            {
                m_options.MsFilteringOptions.MzRange.Minimum = value;
                this.RaisePropertyChanged("MinimumMz");
            }
        }


        public double MaximumCharge
        {
            get { return m_options.MsFilteringOptions.ChargeRange.Maximum; }
            set
            {
                m_options.MsFilteringOptions.ChargeRange.Maximum = value;
                this.RaisePropertyChanged("MaximumCharge");
            }
        }

        public double MinimumCharge
        {
            get { return m_options.MsFilteringOptions.ChargeRange.Minimum; }
            set
            {
                m_options.MsFilteringOptions.ChargeRange.Minimum = value;
                this.RaisePropertyChanged("MinimumCharge");
            }
        }

        public double MinimumDeisotopingScore
        {
            get { return m_options.MsFilteringOptions.MinimumDeisotopingScore; }
            set
            {
                m_options.MsFilteringOptions.MinimumDeisotopingScore = value;
                this.RaisePropertyChanged("MinimumDeisotopingScore");
            }
        }

        public bool ShouldUseChargeStateFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseChargeFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseChargeFilter = value;
                this.RaisePropertyChanged("ShouldUseChargeStateFilter");
            }
        }

        public bool ShouldUseMzFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseMzFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseMzFilter = value;
                this.RaisePropertyChanged("ShouldUseMzFilter");
            }
        }

        public bool ShouldUseIntensityFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseIntensityFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseIntensityFilter = value;
                this.RaisePropertyChanged("ShouldUseIntensityFilter");
            }
        }

        public bool ShouldUseDeisotopingFilter
        {
            get { return m_options.MsFilteringOptions.ShouldUseDeisotopingFilter; }
            set
            {
                m_options.MsFilteringOptions.ShouldUseDeisotopingFilter = value;
                this.RaisePropertyChanged("ShouldUseDeisotopingFilter");
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

        #region Alignment Settings

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
                    this.RaisePropertyChanged("SelectedBaseline");
                }
            }
        }

        public void DisplayAlignment()
        {
            foreach (var file in (selectedFiles.FindAll(x => x.IsAligned)))
            {
                var alignment = AlignmentInformation.Find(x => x.DatasetID == file.DatasetId);
                alignmentWindowFactory.CreateNewWindow(alignment);
            }
        }

        public double MassTolerance
        {
            get { return m_options.LcmsClusteringOptions.InstrumentTolerances.Mass; }
            set
            {
                m_options.LcmsClusteringOptions.InstrumentTolerances.Mass = value;
                this.RaisePropertyChanged("MassTolerance");
            }
        }

        public double NetTolerance
        {
            get { return m_options.LcmsClusteringOptions.InstrumentTolerances.Net; }
            set
            {
                m_options.LcmsClusteringOptions.InstrumentTolerances.Net = value;
                this.RaisePropertyChanged("NetTolerance");
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
                    this.RaisePropertyChanged("SelectedAlignmentAlgorithm");
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
                    this.RaisePropertyChanged("SelectedCalibrationType");
                }
            }
        }

        private string inputFilePath;
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

        #region Clustering Settings
        public LcmsFeatureClusteringAlgorithmType SelectedLcmsFeatureClusteringAlgorithm
        {
            get { return m_options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm; }
            set
            {
                if (m_options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm != value)
                {
                    m_options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm = value;
                    this.RaisePropertyChanged("SelectedLcmsFeatureClusteringAlgorithm");
                }
            }
        }

        public ClusterCentroidRepresentation SelectedCentroidMethod
        {
            get { return m_options.LcmsClusteringOptions.ClusterCentroidRepresentation; }
            set
            {
                if (m_options.LcmsClusteringOptions.ClusterCentroidRepresentation != value)
                {
                    m_options.LcmsClusteringOptions.ClusterCentroidRepresentation = value;
                    this.RaisePropertyChanged("SelectedCentroidMethod");
                }
            }
        }

        public DistanceMetric SelectedDistanceFunction
        {
            get { return m_options.LcmsClusteringOptions.DistanceFunction; }
            set
            {
                if (m_options.LcmsClusteringOptions.DistanceFunction != value)
                {
                    m_options.LcmsClusteringOptions.DistanceFunction = value;
                    this.RaisePropertyChanged("SelectedDistanceFunction");
                }
            }
        }
        #endregion

        #region Clustering

        public async void AsyncClusterFeatures()
        {
            await Task.Run(() => ClusterFeatures());
        }
        
        //TODO:Finish integrating clustering from MultiAlignAnalysisProcessor into Rogue
        public void ClusterFeatures()
        {
            List<DatasetInformation> selectedFilesCopy = new List<DatasetInformation>();
            foreach (var dataset in selectedFiles)
            {
                selectedFilesCopy.Add(dataset); //Prevents crashes from changing selected files while this thread is running
            }
            var clusterer = m_algorithms.Clusterer;
            clusterer.Parameters = LcmsClusteringOptions.ConvertToOmics(m_options.LcmsClusteringOptions);

            // This just tells us whether we are using mammoth memory partitions or not.          
            var featureCache = m_analysis.DataProviders.FeatureCache;
            var clusterCount = 0;

            var providers = m_config.Analysis.DataProviders;

            // Here we see if we need to separate the charge...
            // IMS is said to require charge separation 
            if (!m_analysis.Options.LcmsClusteringOptions.ShouldSeparateCharge)
            {
                var features = featureCache.FindAll();
                var clusters = new List<UMCClusterLight>();
                clusters = clusterer.Cluster(features, clusters);
                foreach (var cluster in clusters)
                {
                    cluster.Id = clusterCount++;
                    cluster.UmcList.ForEach(x => x.ClusterId = cluster.Id);

                    // Updates the cluster with statistics
                    foreach (var feature in cluster.UmcList)
                    {
                        cluster.MsMsCount += feature.MsMsCount;
                        cluster.IdentifiedSpectraCount += feature.IdentifiedSpectraCount;
                    }
                }
                providers.ClusterCache.AddAll(clusters);
                providers.FeatureCache.UpdateAll(features);
                m_config.Analysis.Clusters = clusters;
            }
            else
            {
                var maxChargeState = featureCache.FindMaxCharge();

                /*
                 * Here we cluster all charge states separately.  Probably IMS Data.
                 */
                for (var chargeState = 1; chargeState <= maxChargeState; chargeState++)
                {
                    var features = featureCache.FindByCharge(chargeState);
                    if (features.Count < 1)
                    {
                        break;
                    }

                    var clusters = clusterer.Cluster(features);
                    foreach (var cluster in clusters)
                    {
                        cluster.Id = clusterCount++;
                        cluster.UmcList.ForEach(x => x.ClusterId = cluster.Id);

                        // Updates the cluster with statistics
                        foreach (var feature in cluster.Features)
                        {
                            cluster.MsMsCount += feature.MsMsCount;
                            cluster.IdentifiedSpectraCount += feature.IdentifiedSpectraCount;
                        }
                    }

                    m_config.Analysis.DataProviders.ClusterCache.AddAll(clusters);
                    m_config.Analysis.DataProviders.FeatureCache.UpdateAll(features);
                }
                m_config.Analysis.Clusters = m_config.Analysis.DataProviders.ClusterCache.FindAll();
            }

            MessageBox.Show("Working Command");
        }

        public void DisplayFeatures()
        {
            MessageBox.Show("Working command");
            //TODO: Implement
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

        private async Task SearchDms()
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
    }
}
