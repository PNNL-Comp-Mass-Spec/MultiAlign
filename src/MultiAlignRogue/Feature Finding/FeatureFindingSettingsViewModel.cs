using FeatureAlignment.Data.Features;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignRogue.Feature_Finding
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using InformedProteomics.Backend.Utils;
    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.IO.RawData;
    using MultiAlignRogue.Utils;
    using MultiAlignRogue.ViewModels;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using MultiAlignCore.IO;
    using MultiAlignCore.IO.Features;

    using NHibernate;

    using NHibernateUtil = MultiAlignCore.IO.Hibernate.NHibernateUtil;

    public class FeatureFindingSettingsViewModel : ViewModelBase
    {
        private readonly MultiAlignAnalysis analysis;

        private readonly IFeatureWindowFactory msFeatureWindowFactory;

        private readonly Dictionary<DatasetInformation, IList<UMCLight>> featuresByDataset;

        private double totalProgress;

        private bool shouldShowProgress;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="datasets"></param>
        /// <param name="msFeatureWindowFactory"></param>
        public FeatureFindingSettingsViewModel(
                                               MultiAlignAnalysis analysis,
                                               ObservableCollection<DatasetInformationViewModel> datasets,
                                               IFeatureWindowFactory msFeatureWindowFactory = null)
        {
            this.analysis = analysis;
            this.Datasets = datasets;
            this.msFeatureWindowFactory = msFeatureWindowFactory ?? new MSFeatureViewFactory();
            this.msFeatureWindowFactory = new MSFeatureViewFactory();
            this.featuresByDataset = new Dictionary<DatasetInformation, IList<UMCLight>>();
            this.MsFeatureClusterers = new ObservableCollection<MsFeatureClusteringAlgorithmType>(
                                       Enum.GetValues(typeof(MsFeatureClusteringAlgorithmType)).Cast<MsFeatureClusteringAlgorithmType>());
            this.LcmsFeatureClusterers = new ObservableCollection<GenericClusteringAlgorithmType>(
                           Enum.GetValues(typeof(GenericClusteringAlgorithmType)).Cast<GenericClusteringAlgorithmType>());

            this.CanCreateXics = datasets.Select(dataset => RawLoaderFactory.CreateFileReader(dataset.Dataset.RawFile.Path, dataset.DatasetId))
                                        .Any(reader => reader is ISpectraProvider);

            // When dataset is selected/unselected, update can executes.
            this.MessengerInstance.Register<PropertyChangedMessage<bool>>(this, this.UpdateDatasetSelection);

            // When dataset state changes, update can executes.
            this.MessengerInstance.Register<PropertyChangedMessage<DatasetInformationViewModel.DatasetStates>>(this, args =>
            {
                if (args.Sender is DatasetInformationViewModel && args.PropertyName == "DatasetState")
                {
                    ThreadSafeDispatcher.Invoke(() =>
                    {
                        this.FindMsFeaturesCommand.RaiseCanExecuteChanged();
                        this.PlotMsFeaturesCommand.RaiseCanExecuteChanged();
                        this.PlotAlignedFeaturesCommand.RaiseCanExecuteChanged();
                    });
                }
            });

            this.FindMsFeaturesCommand = new RelayCommand(
                                        async () => await this.LoadMsFeaturesAsync(),
                                        () => this.Datasets.Any(ds => ds.IsSelected && !ds.IsFindingFeatures));

            this.PlotMsFeaturesCommand = new RelayCommand(
                                        async () => await this.PlotMsFeatures(false),
                                        () => this.Datasets.Any(
                                                ds =>
                                                    ds.DatasetState >
                                                    DatasetInformationViewModel.DatasetStates.FindingFeatures &&
                                                    ds.IsSelected));

            this.PlotAlignedFeaturesCommand = new RelayCommand(
                                        async () => await this.PlotMsFeatures(true),
                                        () => this.Datasets.Any(ds => ds.IsAligned));

            this.RestoreDefaultsCommand = new RelayCommand(this.RestoreDefaults);
        }

        public ObservableCollection<DatasetInformationViewModel> Datasets { get; }

        public ObservableCollection<MsFeatureClusteringAlgorithmType> MsFeatureClusterers { get; }

        public ObservableCollection<GenericClusteringAlgorithmType> LcmsFeatureClusterers { get; }

        public RelayCommand FeatureFindingDefaultsCommand { get; private set; }

        public RelayCommand FindMsFeaturesCommand { get; }

        public RelayCommand PlotMsFeaturesCommand { get; }

        public RelayCommand PlotAlignedFeaturesCommand { get; }

        public RelayCommand RestoreDefaultsCommand { get; }

        public double MassResolution
        {
            get => this.analysis.Options.InstrumentTolerances.Mass;
            set
            {
                this.analysis.Options.AlignmentOptions.MassTolerance = value;
                this.analysis.Options.InstrumentTolerances.Mass = value;
                this.RaisePropertyChanged();
            }
        }

        public string[] TimeOptions { get; } = { "Minutes", "Scans" };

        public string TreatAsTimeOrScan
        {
            get => this.FilterOnMinutes ? "Minutes" : "Scans";
            set
            {
                this.FilterOnMinutes = value.Equals("Minutes");
                this.RaisePropertyChanged();
                this.RaisePropertyChanged("MinimumFeatureLength");
                this.RaisePropertyChanged("MaximumFeatureLength");
            }
        }

        public bool FilterOnMinutes
        {
            get => this.analysis.Options.LcmsFilteringOptions.FilterOnMinutes;
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FilterOnMinutes = value;
                this.RaisePropertyChanged();
            }
        }

        // ReSharper disable once IdentifierTypo
        public bool CanCreateXics
        {
            get => this.canCreateXICs;
            set
            {
                if (this.canCreateXICs != value)
                {
                    this.canCreateXICs = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool canCreateXICs;

        public double MinimumFeatureLength
        {
            get
            {
                if (this.FilterOnMinutes)
                {
                    return this.MinimumFeatureLengthMinutes;
                }
                return this.MinimumFeatureLengthScans;
            }
            set
            {
                if (this.FilterOnMinutes)
                {
                    this.MinimumFeatureLengthMinutes = value;
                }
                else
                {
                    this.MinimumFeatureLengthScans = value;
                }
                this.RaisePropertyChanged();
            }
        }

        public double MaximumFeatureLength
        {
            get
            {
                if (this.FilterOnMinutes)
                {
                    return this.MaximumFeatureLengthMinutes;
                }
                return this.MaximumFeatureLengthScans;
            }
            set
            {
                if (this.FilterOnMinutes)
                {
                    this.MaximumFeatureLengthMinutes = value;
                }
                else
                {
                    this.MaximumFeatureLengthScans = value;
                }
                this.RaisePropertyChanged();
            }
        }

        public double MinimumFeatureLengthMinutes
        {
            get => this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeMinutes.Minimum;
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeMinutes.Minimum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MaximumFeatureLengthMinutes
        {
            get => this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeMinutes.Maximum;
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeMinutes.Maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumFeatureLengthScans
        {
            get => this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeScans.Minimum;
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeScans.Minimum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MaximumFeatureLengthScans
        {
            get => this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeScans.Maximum;
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeScans.Maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumFeatureDataPoints
        {
            get => this.analysis.Options.LcmsFilteringOptions.MinimumDataPoints;
            set
            {
                this.analysis.Options.LcmsFilteringOptions.MinimumDataPoints = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumIntensity
        {
            get => this.analysis.Options.MsFilteringOptions.MinimumIntensity;
            set
            {
                this.analysis.Options.MsFilteringOptions.MinimumIntensity = value;
                this.RaisePropertyChanged();
            }
        }

        public double FragmentationTolerance
        {
            get => this.analysis.Options.InstrumentTolerances.FragmentationWindowSize;
            set
            {
                this.analysis.Options.InstrumentTolerances.FragmentationWindowSize = value;
                this.RaisePropertyChanged();
            }
        }

        public double MaximumMz
        {
            get => this.analysis.Options.MsFilteringOptions.MzRange.Maximum;
            set
            {
                this.analysis.Options.MsFilteringOptions.MzRange.Maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumMz
        {
            get => this.analysis.Options.MsFilteringOptions.MzRange.Minimum;
            set
            {
                this.analysis.Options.MsFilteringOptions.MzRange.Minimum = value;
                this.RaisePropertyChanged();
            }
        }


        public double MaximumCharge
        {
            get => this.analysis.Options.MsFilteringOptions.ChargeRange.Maximum;
            set
            {
                this.analysis.Options.MsFilteringOptions.ChargeRange.Maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumCharge
        {
            get => this.analysis.Options.MsFilteringOptions.ChargeRange.Minimum;
            set
            {
                this.analysis.Options.MsFilteringOptions.ChargeRange.Minimum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumDeisotopingScore
        {
            get => this.analysis.Options.MsFilteringOptions.MinimumDeisotopingScore;
            set
            {
                this.analysis.Options.MsFilteringOptions.MinimumDeisotopingScore = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseChargeStateFilter
        {
            get => this.analysis.Options.MsFilteringOptions.ShouldUseChargeFilter;
            set
            {
                this.analysis.Options.MsFilteringOptions.ShouldUseChargeFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseMzFilter
        {
            get => this.analysis.Options.MsFilteringOptions.ShouldUseMzFilter;
            set
            {
                this.analysis.Options.MsFilteringOptions.ShouldUseMzFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseIntensityFilter
        {
            get => this.analysis.Options.MsFilteringOptions.ShouldUseIntensityFilter;
            set
            {
                this.analysis.Options.MsFilteringOptions.ShouldUseIntensityFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseDeisotopingFilter
        {
            get => this.analysis.Options.MsFilteringOptions.ShouldUseDeisotopingFilter;
            set
            {
                this.analysis.Options.MsFilteringOptions.ShouldUseDeisotopingFilter = value;
                this.RaisePropertyChanged();
            }
        }

        // ReSharper disable once IdentifierTypo
        public bool ShouldCreateXics
        {
            get => this.analysis.Options.LcmsFindingOptions.FindXics && this.CanCreateXics;
            set
            {
                if (this.analysis.Options.LcmsFindingOptions.FindXics != value)
                {
                    this.analysis.Options.LcmsFindingOptions.FindXics = value && this.CanCreateXics;
                    this.RaisePropertyChanged();
                }
            }
        }

        // ReSharper disable once IdentifierTypo
        public bool ShouldRefineXics
        {
            get => this.analysis.Options.LcmsFindingOptions.RefineXics && this.CanCreateXics;
            set
            {
                if (this.analysis.Options.LcmsFindingOptions.RefineXics != value)
                {
                    this.analysis.Options.LcmsFindingOptions.RefineXics = value && this.CanCreateXics;
                    this.RaisePropertyChanged();
                }
            }
        }

        public int SmoothingWindowSize
        {
            get => this.analysis.Options.LcmsFindingOptions.SmoothingWindowSize;
            set
            {
                if (this.analysis.Options.LcmsFindingOptions.SmoothingWindowSize != value)
                {
                    this.analysis.Options.LcmsFindingOptions.SmoothingWindowSize = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public int SmoothingPolynomialOrder
        {
            get => this.analysis.Options.LcmsFindingOptions.SmoothingPolynomialOrder;
            set
            {
                if (this.analysis.Options.LcmsFindingOptions.SmoothingPolynomialOrder != value)
                {
                    this.analysis.Options.LcmsFindingOptions.SmoothingPolynomialOrder = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double XicRelativeIntensityThreshold
        {
            get => this.analysis.Options.LcmsFindingOptions.XicRelativeIntensityThreshold;
            set
            {
                if (Math.Abs(this.analysis.Options.LcmsFindingOptions.XicRelativeIntensityThreshold - value) > Single.Epsilon)
                {
                    this.analysis.Options.LcmsFindingOptions.XicRelativeIntensityThreshold = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool ShouldPerformSecondPassClustering
        {
            get => this.analysis.Options.LcmsFindingOptions.SecondPassClustering;
            set
            {
                if (this.analysis.Options.LcmsFindingOptions.SecondPassClustering != value)
                {
                    this.analysis.Options.LcmsFindingOptions.SecondPassClustering = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public MsFeatureClusteringAlgorithmType FirstPassClusterer
        {
            get => this.analysis.Options.LcmsFindingOptions.FirstPassClusterer;
            set
            {
                if (this.analysis.Options.LcmsFindingOptions.FirstPassClusterer != value)
                {
                    this.analysis.Options.LcmsFindingOptions.FirstPassClusterer = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public GenericClusteringAlgorithmType SecondPassClusterer
        {
            get => this.analysis.Options.LcmsFindingOptions.SecondPassClusterer;
            set
            {
                if (this.analysis.Options.LcmsFindingOptions.SecondPassClusterer != value)
                {
                    this.analysis.Options.LcmsFindingOptions.SecondPassClusterer = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double TotalProgress
        {
            get => this.totalProgress;
            set
            {
                if (Math.Abs(this.totalProgress - value) > Single.Epsilon)
                {
                    this.totalProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool ShouldShowProgress
        {
            get => this.shouldShowProgress;
            set
            {
                if (this.shouldShowProgress != value)
                {
                    this.shouldShowProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public async Task LoadMsFeaturesAsync()
        {
            await Task.Run(() => this.LoadFeatures());
        }

        internal void LoadFeatures(List<DatasetInformationViewModel> workFlowDatasets = null, IProgress<PRISM.ProgressData> workflowProgress = null)
        {
            var featureCache = new FeatureLoader { Providers = this.analysis.DataProviders };
            this.ShouldShowProgress = true;
            var selectedFiles = workFlowDatasets ?? this.Datasets.Where(file => !file.DoingWork).Where(ds => ds.IsSelected).ToList();
            foreach (var file in selectedFiles)
            {
                file.DatasetState = DatasetInformationViewModel.DatasetStates.FindingFeatures;
                ThreadSafeDispatcher.Invoke(() => this.PlotMsFeaturesCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => this.FindMsFeaturesCommand.RaiseCanExecuteChanged());
            }

            var taskBarProgress = TaskBarProgress.GetInstance();
            taskBarProgress.ShowProgress(this, true);
            workflowProgress = workflowProgress ?? new Progress<PRISM.ProgressData>();
            IProgress<PRISM.ProgressData> totalProgressRpt = new Progress<PRISM.ProgressData>(pd =>
            {
                this.TotalProgress = pd.Percent;
                taskBarProgress.SetProgress(this, pd.Percent);
                workflowProgress.Report(pd);
            });
            var totalProgressData = new PRISM.ProgressData(totalProgressRpt);

            DatabaseIndexer.IndexClustersDrop(NHibernateUtil.Path);
            DatabaseIndexer.IndexFeaturesDrop(NHibernateUtil.Path);

            var i = 1;
            foreach (var file in selectedFiles)
            {
                // Set range based on file
                totalProgressData.StepRange((i++ * 100.0) / selectedFiles.Count);
                var fileInstance = file;

                var progressData = new PRISM.ProgressData(new Progress<PRISM.ProgressData>(pd =>
                {
                    fileInstance.Progress = pd.Percent;

                    // Report file progress
                    totalProgressData.Report(fileInstance.Progress);
                }));

                var progressRpt = new Progress<PRISM.ProgressData>(pd => progressData.Report(pd.Percent));

                progressData.StepRange(30);

                IList<UMCLight> features;

                // Load features from the database.
                try
                {
                    this.analysis.DataProviders.DatabaseLock.EnterReadLock();
                    features = featureCache.LoadDataset(
                        file.Dataset,
                        this.analysis.Options.MsFilteringOptions,
                        this.analysis.Options.LcmsFindingOptions,
                        this.analysis.Options.LcmsFilteringOptions,
                        this.analysis.Options.DataLoadOptions,
                        this.analysis.DataProviders.ScanSummaryProviderCache,
                        this.analysis.DataProviders.IdentificationProviderCache,
                        progressRpt);
                }
                finally
                {   // Always close read lock, even during failure condition so we don't have a recursive lock error.
                    this.analysis.DataProviders.DatabaseLock.ExitReadLock();
                }

                if (!this.featuresByDataset.ContainsKey(file.Dataset))
                {
                    this.featuresByDataset.Add(file.Dataset, new List<UMCLight>());
                }

                this.featuresByDataset[file.Dataset] = features;

                file.DatasetState = DatasetInformationViewModel.DatasetStates.PersistingFeatures;
                ThreadSafeDispatcher.Invoke(() => this.PlotMsFeaturesCommand.RaiseCanExecuteChanged());

                // TODO: We were using this log file to track speed changes for writing the database. We probably don't need it anymore.
                using (var logger = new StreamWriter("nhibernate_stats.txt", true))
                {
                    logger.WriteLine();
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    var scanSumProvider =
                        this.analysis.DataProviders.ScanSummaryProviderCache.GetScanSummaryProvider(
                            file.Dataset.DatasetId);
                    if (scanSumProvider.IsBackedByFile)
                    {
                        var ssDao = this.analysis.DataProviders.ScanSummaryDao;
                        ssDao.DeleteByDatasetId(file.Dataset.DatasetId);

                        // Add all of the Scan Summaries for this dataset to the database, but first properly set the dataset ID
                        ssDao.AddAllStateless(
                            scanSumProvider.GetScanSummaries().Select(
                                sum =>
                                    {
                                        sum.DatasetId = file.Dataset.DatasetId;
                                        return sum;
                                    }).ToList());
                    }

                    progressData.StepRange(100);

                    // Cache features to database.
                    try
                    {
                        this.analysis.DataProviders.DatabaseLock.EnterWriteLock();
                        featureCache.CacheFeatures(features, progressRpt);
                    }
                    catch (NonUniqueObjectException ex)
                    {
                        MessageBox.Show("Could not completely persist features: " + ex.Message);
                    }
                    catch (Exception ex) // TODO: Figure out which exception should actually be caught here
                    {
                        MessageBox.Show("Could not persist features to database: " + ex.Message);
                        file.DatasetState = DatasetInformationViewModel.DatasetStates.Loaded;
                        continue;
                    }
                    finally
                    {   // Always close write lock, even during failure condition so we don't have a recursive lock error.
                        this.analysis.DataProviders.DatabaseLock.ExitWriteLock();
                    }

                    stopWatch.Stop();
                    logger.WriteLine("Writing: {0}s", stopWatch.Elapsed.TotalSeconds);
                }

                file.DatasetState = DatasetInformationViewModel.DatasetStates.FeaturesFound;
                ThreadSafeDispatcher.Invoke(() => this.FindMsFeaturesCommand.RaiseCanExecuteChanged());
                file.Progress = 0;
            }

            DatabaseIndexer.IndexFeatures(NHibernateUtil.Path);

            taskBarProgress.ShowProgress(this, false);
            this.ShouldShowProgress = false;
        }

        public async Task PlotMsFeatures(bool showAlignedFeatures)
        {
            try
            {
                if (showAlignedFeatures)
                {
                    this.msFeatureWindowFactory.CreateNewWindow(
                        await this.GetFeatures(this.Datasets.Where(ds => ds.IsSelected).Where(file => file.IsAligned)),
                        true);
                }
                else
                {
                    this.msFeatureWindowFactory.CreateNewWindow(
                        await this.GetFeatures(this.Datasets.Where(ds => ds.IsSelected)
                                  .Where(file => file.DatasetState > DatasetInformationViewModel.DatasetStates.FindingFeatures)),
                        false);
                }
            }
            catch
            {
                MessageBox.Show("Feature cache currently being accessed. Try again in a few moments");
            }
        }

        private async Task<Dictionary<DatasetInformation, IList<UMCLight>>> GetFeatures(IEnumerable<DatasetInformationViewModel> datasets)
        {
            var featureCache = new FeatureLoader { Providers = this.analysis.DataProviders };
            var datasetFeatures = new Dictionary<DatasetInformation, IList<UMCLight>>();
            foreach (var file in datasets)
            {   // Select only datasets with features.
                IList<UMCLight> feat;
                if (this.featuresByDataset.ContainsKey(file.Dataset))
                {
                    feat = this.featuresByDataset[file.Dataset];
                }
                else
                {
                    var fileInstance = file;
                    feat = await Task.Run(() => UmcLoaderFactory.LoadUmcFeatureData(
                            fileInstance.Dataset,
                            featureCache.Providers.FeatureCache));
                    this.featuresByDataset.Add(file.Dataset, feat);
                }

                datasetFeatures.Add(file.Dataset, feat);
            }

            return datasetFeatures;
        }

        /// <summary>
        /// Reset the settings the default values
        /// </summary>
        public void RestoreDefaults()
        {
            var defaultOptions = new MultiAlignAnalysisOptions();

            this.MassResolution = defaultOptions.InstrumentTolerances.Mass;
            this.FragmentationTolerance = defaultOptions.InstrumentTolerances.FragmentationWindowSize;

            this.MinimumMz = defaultOptions.MsFilteringOptions.MzRange.Minimum;
            this.MaximumMz = defaultOptions.MsFilteringOptions.MzRange.Maximum;
            this.ShouldUseMzFilter = defaultOptions.MsFilteringOptions.ShouldUseMzFilter;

            this.MinimumCharge = defaultOptions.MsFilteringOptions.ChargeRange.Minimum;
            this.MaximumCharge = defaultOptions.MsFilteringOptions.ChargeRange.Maximum;
            this.ShouldUseChargeStateFilter = defaultOptions.MsFilteringOptions.ShouldUseChargeFilter;

            this.FilterOnMinutes = defaultOptions.LcmsFilteringOptions.FilterOnMinutes;
            this.MinimumFeatureLength = defaultOptions.LcmsFilteringOptions.FeatureLengthRangeMinutes.Minimum;
            this.MaximumFeatureLength = defaultOptions.LcmsFilteringOptions.FeatureLengthRangeMinutes.Maximum;

            this.MinimumFeatureLengthMinutes = this.MinimumFeatureLength;
            this.MaximumFeatureLengthMinutes = this.MaximumFeatureLength;

            this.MinimumFeatureLengthScans = defaultOptions.LcmsFilteringOptions.FeatureLengthRangeScans.Minimum;
            this.MaximumFeatureLengthScans = defaultOptions.LcmsFilteringOptions.FeatureLengthRangeScans.Maximum;

            this.MinimumFeatureDataPoints = defaultOptions.LcmsFilteringOptions.MinimumDataPoints;

            this.MinimumDeisotopingScore = defaultOptions.MsFilteringOptions.MinimumDeisotopingScore;
            this.ShouldUseDeisotopingFilter = defaultOptions.MsFilteringOptions.ShouldUseDeisotopingFilter;

            this.MinimumIntensity = defaultOptions.MsFilteringOptions.MinimumIntensity;
            this.ShouldUseIntensityFilter = defaultOptions.MsFilteringOptions.ShouldUseIntensityFilter;

            this.FirstPassClusterer = defaultOptions.LcmsFindingOptions.FirstPassClusterer;
            this.ShouldCreateXics = defaultOptions.LcmsFindingOptions.FindXics;

            this.XicRelativeIntensityThreshold = defaultOptions.LcmsFindingOptions.XicRelativeIntensityThreshold;
            this.ShouldRefineXics = defaultOptions.LcmsFindingOptions.RefineXics;

            this.ShouldPerformSecondPassClustering = defaultOptions.LcmsFindingOptions.SecondPassClustering;
            this.SecondPassClusterer = defaultOptions.LcmsFindingOptions.SecondPassClusterer;

            this.SmoothingWindowSize = defaultOptions.LcmsFindingOptions.SmoothingWindowSize;
            this.SmoothingPolynomialOrder = defaultOptions.LcmsFindingOptions.SmoothingPolynomialOrder;
        }

        /// <summary>
        /// Event handler for dataset IsSelected property changed.
        /// When dataset is selected/unselected, update can executes.
        /// </summary>
        /// <param name="args">The message changed arguments, containing the new value.</param>
        private void UpdateDatasetSelection(PropertyChangedMessage<bool> args)
        {
            if (args.Sender is DatasetInformationViewModel && args.PropertyName == "IsSelected")
            {   // Make sure that this message is for DatasetInformationViewModel.IsSelected
                this.FindMsFeaturesCommand.RaiseCanExecuteChanged();
                this.PlotMsFeaturesCommand.RaiseCanExecuteChanged();
                this.PlotAlignedFeaturesCommand.RaiseCanExecuteChanged();

                // ReSharper disable once CommentTypo
                // Add an event listener to update CanCreateXics whenever the Datasets collection changes
                this.CanCreateXics = this.Datasets
                        .Where(dataset => dataset.IsSelected)
                        .Select(dataset => RawLoaderFactory.CreateFileReader(dataset.Dataset.RawFile.Path, dataset.DatasetId))
                        .Any(reader => reader != null && reader is ISpectraProvider);
            }
        }
    }
}
