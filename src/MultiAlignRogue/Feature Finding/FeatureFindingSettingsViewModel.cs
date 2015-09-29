using System.Diagnostics;
using System.IO;
using System.Windows;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Hibernate;
using MultiAlignRogue.Utils;
using MultiAlignRogue.ViewModels;
using NHibernate.Util;

namespace MultiAlignRogue.Feature_Finding
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;

    using MultiAlign.Data;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO;
    using MultiAlignCore.IO.Features;

    
    public class FeatureFindingSettingsViewModel : ViewModelBase
    {
        private readonly MultiAlignAnalysis analysis;

        private readonly FeatureLoader featureCache;

        private readonly IFeatureWindowFactory msFeatureWindowFactory;

        private readonly IProgress<int> progress;

        private readonly string[] _timeOptions = { "Minutes", "Scans" };

        private readonly Dictionary<DatasetInformation, IList<UMCLight>> features;

        private double totalProgress;

        private bool shouldShowProgress;

        public FeatureFindingSettingsViewModel(
                                               MultiAlignAnalysis analysis,
                                               FeatureLoader featureCache,
                                               ObservableCollection<DatasetInformationViewModel> datasets,
                                               IFeatureWindowFactory msFeatureWindowFactory = null,
                                               IProgress<int> progressReporter = null)
        {
            this.analysis = analysis;
            this.featureCache = featureCache;
            this.Datasets = datasets;
            this.msFeatureWindowFactory = msFeatureWindowFactory ?? new MSFeatureViewFactory();
            this.progress = progressReporter ?? new Progress<int>();
            this.msFeatureWindowFactory = new MSFeatureViewFactory();
            this.features = new Dictionary<DatasetInformation, IList<UMCLight>>();
            this.MsFeatureClusterers = new ObservableCollection<MsFeatureClusteringAlgorithmType>(
                                       Enum.GetValues(typeof(MsFeatureClusteringAlgorithmType)).Cast<MsFeatureClusteringAlgorithmType>());
            this.LcmsFeatureClusterers = new ObservableCollection<GenericClusteringAlgorithmType>(
                           Enum.GetValues(typeof(GenericClusteringAlgorithmType)).Cast<GenericClusteringAlgorithmType>());

            // When dataset is selected/unselected, update can executes.
            this.MessengerInstance.Register<PropertyChangedMessage<bool>>(this, args =>
            {
                if (args.Sender is DatasetInformationViewModel && args.PropertyName == "IsSelected")
                {
                    this.FindMSFeaturesCommand.RaiseCanExecuteChanged();
                    this.PlotMSFeaturesCommand.RaiseCanExecuteChanged();
                    this.PlotAlignedFeaturesCommand.RaiseCanExecuteChanged();   
                }
            });

            // When dataset state changes, update can executes.
            this.MessengerInstance.Register<PropertyChangedMessage<DatasetInformationViewModel.DatasetStates>>(this, args =>
            {
                if (args.Sender is DatasetInformationViewModel && args.PropertyName == "DatasetState")
                {
                    ThreadSafeDispatcher.Invoke(() =>
                    {
                        this.FindMSFeaturesCommand.RaiseCanExecuteChanged();
                        this.PlotMSFeaturesCommand.RaiseCanExecuteChanged();
                        this.PlotAlignedFeaturesCommand.RaiseCanExecuteChanged(); 
                    });
                }
            });

            this.FindMSFeaturesCommand = new RelayCommand(
                                        async () => await this.LoadMSFeaturesAsync(),
                                        () => this.Datasets.Any(ds => ds.IsSelected && !ds.IsFindingFeatures));
            this.PlotMSFeaturesCommand = new RelayCommand(
                                        async () => await this.PlotMSFeatures(false),
                                        () => this.Datasets.Any(
                                                ds =>
                                                    ds.DatasetState >
                                                    DatasetInformationViewModel.DatasetStates.FindingFeatures &&
                                                    ds.IsSelected));

            this.PlotAlignedFeaturesCommand = new RelayCommand(
                                        async () => await this.PlotMSFeatures(true),
                                        () => this.Datasets.Any(ds => ds.IsAligned));
        }

        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set; } 

        public ObservableCollection<MsFeatureClusteringAlgorithmType> MsFeatureClusterers { get; private set; }

        public ObservableCollection<GenericClusteringAlgorithmType> LcmsFeatureClusterers { get; private set; } 

        public RelayCommand FindMSFeaturesCommand { get; private set; }

        public RelayCommand PlotMSFeaturesCommand { get; private set; }

        public RelayCommand PlotAlignedFeaturesCommand { get; private set; }

        public double MassResolution
        {
            get { return this.analysis.Options.InstrumentTolerances.Mass; }
            set
            {
                this.analysis.Options.AlignmentOptions.MassTolerance = value;
                this.analysis.Options.InstrumentTolerances.Mass = value;
                this.RaisePropertyChanged();
            }
        }

        public string[] TimeOptions
        {
            get { return _timeOptions; }
        }

        public string TreatAsTimeOrScan
        {
            get { return FilterOnMinutes ? "Minutes" : "Scans"; }
            set
            {
                FilterOnMinutes = value.Equals("Minutes");
                this.RaisePropertyChanged();
                this.RaisePropertyChanged("MinimumFeatureLength");
                this.RaisePropertyChanged("MaximumFeatureLength");
            }
        }

        public bool FilterOnMinutes
        {
            get { return this.analysis.Options.LcmsFilteringOptions.FilterOnMinutes; }
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FilterOnMinutes = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumFeatureLength
        {
            get
            {
                if (FilterOnMinutes)
                {
                    return MinimumFeatureLengthMinutes;
                }
                return MinimumFeatureLengthScans;
            }
            set
            {
                if (FilterOnMinutes)
                {
                    MinimumFeatureLengthMinutes = value;
                }
                else
                {
                    MinimumFeatureLengthScans = value;
                }
                this.RaisePropertyChanged();
            }
        }

        public double MaximumFeatureLength
        {
            get
            {
                if (FilterOnMinutes)
                {
                    return MaximumFeatureLengthMinutes;
                }
                return MaximumFeatureLengthScans;
            }
            set
            {
                if (FilterOnMinutes)
                {
                    MaximumFeatureLengthMinutes = value;
                }
                else
                {
                    MaximumFeatureLengthScans = value;
                }
                this.RaisePropertyChanged();
            }
        }

        public double MinimumFeatureLengthMinutes
        {
            get { return this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeMinutes.Minimum; }
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeMinutes.Minimum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MaximumFeatureLengthMinutes
        {
            get { return this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeMinutes.Maximum; }
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeMinutes.Maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumFeatureLengthScans
        {
            get { return this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeScans.Minimum; }
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeScans.Minimum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MaximumFeatureLengthScans
        {
            get { return this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeScans.Maximum; }
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FeatureLengthRangeScans.Maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumFeatureDataPoints
        {
            get { return this.analysis.Options.LcmsFilteringOptions.MinimumDataPoints; }
            set
            {
                this.analysis.Options.LcmsFilteringOptions.MinimumDataPoints = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumIntensity
        {
            get { return this.analysis.Options.MsFilteringOptions.MinimumIntensity; }
            set
            {
                this.analysis.Options.MsFilteringOptions.MinimumIntensity = value;
                this.RaisePropertyChanged();
            }
        }

        public double FragmentationTolerance
        {
            get { return this.analysis.Options.InstrumentTolerances.FragmentationWindowSize; }
            set
            {
                this.analysis.Options.InstrumentTolerances.FragmentationWindowSize = value;
                this.RaisePropertyChanged();
            }
        }

        public double MaximumMz
        {
            get { return this.analysis.Options.MsFilteringOptions.MzRange.Maximum; }
            set
            {
                this.analysis.Options.MsFilteringOptions.MzRange.Maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumMz
        {
            get { return this.analysis.Options.MsFilteringOptions.MzRange.Minimum; }
            set
            {
                this.analysis.Options.MsFilteringOptions.MzRange.Minimum = value;
                this.RaisePropertyChanged();
            }
        }


        public double MaximumCharge
        {
            get { return this.analysis.Options.MsFilteringOptions.ChargeRange.Maximum; }
            set
            {
                this.analysis.Options.MsFilteringOptions.ChargeRange.Maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumCharge
        {
            get { return this.analysis.Options.MsFilteringOptions.ChargeRange.Minimum; }
            set
            {
                this.analysis.Options.MsFilteringOptions.ChargeRange.Minimum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumDeisotopingScore
        {
            get { return this.analysis.Options.MsFilteringOptions.MinimumDeisotopingScore; }
            set
            {
                this.analysis.Options.MsFilteringOptions.MinimumDeisotopingScore = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseChargeStateFilter
        {
            get { return this.analysis.Options.MsFilteringOptions.ShouldUseChargeFilter; }
            set
            {
                this.analysis.Options.MsFilteringOptions.ShouldUseChargeFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseMzFilter
        {
            get { return this.analysis.Options.MsFilteringOptions.ShouldUseMzFilter; }
            set
            {
                this.analysis.Options.MsFilteringOptions.ShouldUseMzFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseIntensityFilter
        {
            get { return this.analysis.Options.MsFilteringOptions.ShouldUseIntensityFilter; }
            set
            {
                this.analysis.Options.MsFilteringOptions.ShouldUseIntensityFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseDeisotopingFilter
        {
            get { return this.analysis.Options.MsFilteringOptions.ShouldUseDeisotopingFilter; }
            set
            {
                this.analysis.Options.MsFilteringOptions.ShouldUseDeisotopingFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldCreateXics
        {
            get { return this.analysis.Options.LcmsFindingOptions.FindXics; }
            set
            {
                if (this.analysis.Options.LcmsFindingOptions.FindXics != value)
                {
                    this.analysis.Options.LcmsFindingOptions.FindXics = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool ShouldRefineXics
        {
            get { return this.analysis.Options.LcmsFindingOptions.RefineXics; }
            set
            {
                if (this.analysis.Options.LcmsFindingOptions.RefineXics != value)
                {
                    this.analysis.Options.LcmsFindingOptions.RefineXics = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public int SmoothingWindowSize
        {
            get { return this.analysis.Options.LcmsFindingOptions.SmoothingWindowSize; }
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
            get { return this.analysis.Options.LcmsFindingOptions.SmoothingPolynomialOrder; }
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
            get { return this.analysis.Options.LcmsFindingOptions.XicRelativeIntensityThreshold; }
            set
            {
                if (this.analysis.Options.LcmsFindingOptions.XicRelativeIntensityThreshold != value)
                {
                    this.analysis.Options.LcmsFindingOptions.XicRelativeIntensityThreshold = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool ShouldPerformSecondPassClustering
        {
            get { return this.analysis.Options.LcmsFindingOptions.SecondPassClustering; }
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
            get { return this.analysis.Options.LcmsFindingOptions.FirstPassClusterer; }
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
            get { return this.analysis.Options.LcmsFindingOptions.SecondPassClusterer; }
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
            get { return this.totalProgress; }
            set
            {
                if (this.totalProgress != value)
                {
                    this.totalProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool ShouldShowProgress
        {
            get { return this.shouldShowProgress; }
            set
            {
                if (this.shouldShowProgress != value)
                {
                    this.shouldShowProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public async Task LoadMSFeaturesAsync()
        {
            await Task.Run(() => this.LoadFeatures());
        }

        internal void LoadFeatures(List<DatasetInformationViewModel> workFlowDatasets = null, IProgress<ProgressData> workflowProgress = null)
        {
            this.ShouldShowProgress = true;
            this.featureCache.Providers = this.analysis.DataProviders;
            var selectedFiles = workFlowDatasets ?? this.Datasets.Where(file => !file.DoingWork).Where(ds => ds.IsSelected).ToList();
            foreach (var file in selectedFiles)
            {
                file.DatasetState = DatasetInformationViewModel.DatasetStates.FindingFeatures;
                ThreadSafeDispatcher.Invoke(() => this.PlotMSFeaturesCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => this.FindMSFeaturesCommand.RaiseCanExecuteChanged());
            }

            TaskBarProgressSingleton.ShowTaskBarProgress(this, true);
            workflowProgress = workflowProgress ?? new Progress<ProgressData>();
            IProgress<ProgressData> totalProgressRpt = new Progress<ProgressData>(pd =>
            {
                this.TotalProgress = pd.Percent;
                TaskBarProgressSingleton.SetTaskBarProgress(this, pd.Percent);
                workflowProgress.Report(pd);
            });
            var totalProgressData = new ProgressData(totalProgressRpt);

            DatabaseIndexer.IndexClustersDrop(NHibernateUtil.Path);
            DatabaseIndexer.IndexFeaturesDrop(NHibernateUtil.Path);

            int i = 1;
            foreach (var file in selectedFiles)
            {
                // Set range based on file
                totalProgressData.StepRange((i++ * 100.0) / selectedFiles.Count);
                var progData = new ProgressData();
                var progressRpt = new Progress<ProgressData>(pd =>
                {
                    file.Progress = progData.UpdatePercent(pd.Percent).Percent;
                    // Report file progress
                    totalProgressData.Report(file.Progress);
                });

                progData.StepRange(30);

                var features = this.featureCache.LoadDataset(
                                                    file.Dataset,
                                                    this.analysis.Options.MsFilteringOptions,
                                                    this.analysis.Options.LcmsFindingOptions,
                                                    this.analysis.Options.LcmsFilteringOptions,
                                                    progressRpt);

                if (!this.features.ContainsKey(file.Dataset))
                {
                    this.features.Add(file.Dataset, new List<UMCLight>());
                }

                this.features[file.Dataset] = features;

                file.DatasetState = DatasetInformationViewModel.DatasetStates.PersistingFeatures;
                ThreadSafeDispatcher.Invoke(() => this.PlotMSFeaturesCommand.RaiseCanExecuteChanged());

                using (var logger = new StreamWriter("nhibernate_stats.txt", true))
                {
                    logger.WriteLine();
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    progData.StepRange(100);
                    this.featureCache.CacheFeatures(features, progressRpt);
                    stopWatch.Stop();
                    logger.WriteLine("Writing: {0}s", stopWatch.Elapsed.TotalSeconds);
                }

                file.DatasetState = DatasetInformationViewModel.DatasetStates.FeaturesFound;
                ThreadSafeDispatcher.Invoke(() => this.FindMSFeaturesCommand.RaiseCanExecuteChanged());
                file.Progress = 0;
            }

            DatabaseIndexer.IndexFeatures(NHibernateUtil.Path);

            TaskBarProgressSingleton.ShowTaskBarProgress(this, false);
            this.ShouldShowProgress = false;
        }

        public async Task PlotMSFeatures(bool showAlignedFeatures)
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

        private async Task<Dictionary<DatasetInformation, IList<UMCLight>>>  GetFeatures(IEnumerable<DatasetInformationViewModel> datasets)
        {
            var datasetFeatures = new Dictionary<DatasetInformation, IList<UMCLight>>();
            foreach (var file in datasets)
            {   // Select only datasets with features.
                IList<UMCLight> feat;
                if (this.features.ContainsKey(file.Dataset))
                {
                    feat = this.features[file.Dataset];
                }
                else
                {
                    DatasetInformationViewModel file1 = file;
                    feat = await Task.Run(() => UmcLoaderFactory.LoadUmcFeatureData(
                            file1.Dataset,
                            this.featureCache.Providers.FeatureCache));
                    this.features.Add(file.Dataset, feat);
                }

                datasetFeatures.Add(file.Dataset, feat);
            }

            return datasetFeatures;
        }
    }
}
