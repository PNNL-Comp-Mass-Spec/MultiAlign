using System.Diagnostics;
using System.IO;
using System.Windows;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
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
    using MultiAlign.ViewModels.Datasets;

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

        private Dictionary<DatasetInformation, IList<UMCLight>> features;

        private IReadOnlyCollection<DatasetInformationViewModel> selectedDatasets;

        public FeatureFindingSettingsViewModel(
                                               MultiAlignAnalysis analysis,
                                               FeatureLoader featureCache,
                                               IFeatureWindowFactory msFeatureWindowFactory = null,
                                               IProgress<int> progressReporter = null)
        {
            this.analysis = analysis;
            this.featureCache = featureCache;
            this.msFeatureWindowFactory = msFeatureWindowFactory ?? new MSFeatureViewFactory();
            this.progress = progressReporter ?? new Progress<int>();
            this.selectedDatasets = new ReadOnlyCollection<DatasetInformationViewModel>(new List<DatasetInformationViewModel>());
            this.msFeatureWindowFactory = new MSFeatureViewFactory();
            this.features = new Dictionary<DatasetInformation, IList<UMCLight>>();

            this.MessengerInstance.Register<PropertyChangedMessage<IReadOnlyCollection<DatasetInformationViewModel>>>(this, sds =>
            {
                this.selectedDatasets = sds.NewValue;
                this.FindMSFeaturesCommand.RaiseCanExecuteChanged();
                this.PlotMSFeaturesCommand.RaiseCanExecuteChanged();
                this.PlotAlignedFeaturesCommand.RaiseCanExecuteChanged();
            });

            this.FindMSFeaturesCommand = new RelayCommand(
                                        async () => await this.LoadMSFeaturesAsync(),
                                        () => this.selectedDatasets != null && 
                                              this.selectedDatasets.Count > 0 && 
                                              this.selectedDatasets.Any(file => !file.IsFindingFeatures));
            this.PlotMSFeaturesCommand = new RelayCommand(
                                        async () => await this.PlotMSFeatures(false), 
                                        () => this.selectedDatasets.Any(file => file.DatasetState > DatasetInformationViewModel.DatasetStates.FindingFeatures));

            this.PlotAlignedFeaturesCommand = new RelayCommand(
                                        async () => await this.PlotMSFeatures(true),
                                        () => this.selectedDatasets.Any(file => file.IsAligned));
        }

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

        private string[] _timeOptions = {"Minutes", "Scans"};

        public string TreatAsTimeOrScan
        {
            get
            {
                if (FilterOnMinutes)
                {
                    return "Minutes";
                }
                else
                {
                    return "Scans";
                }
            }
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

        public async Task LoadMSFeaturesAsync()
        {
            await Task.Run(() => this.LoadFeatures());
        }

        private void LoadFeatures()
        {
            this.featureCache.Providers = this.analysis.DataProviders;
            var selectedFiles = this.selectedDatasets.Where(file => !file.DoingWork).ToList();
            foreach (var file in selectedFiles)
            {
                file.DatasetState = DatasetInformationViewModel.DatasetStates.FindingFeatures;
                ThreadSafeDispatcher.Invoke(() => this.PlotMSFeaturesCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => this.FindMSFeaturesCommand.RaiseCanExecuteChanged());
            }

            foreach (var file in selectedFiles)
            {
                var features = this.featureCache.LoadDataset(file.Dataset, this.analysis.Options.MsFilteringOptions, this.analysis.Options.LcmsFindingOptions, this.analysis.Options.LcmsFilteringOptions);
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

                    this.featureCache.CacheFeatures(features);
                    stopWatch.Stop();
                    logger.WriteLine("Writing: {0}s", stopWatch.Elapsed.TotalSeconds);
                }

                file.DatasetState = DatasetInformationViewModel.DatasetStates.FeaturesFound;
                ThreadSafeDispatcher.Invoke(() => this.FindMSFeaturesCommand.RaiseCanExecuteChanged());
                this.progress.Report(0);
            }
        }

        public async Task PlotMSFeatures(bool showAlignedFeatures)
        {
            try
            {
                if (showAlignedFeatures)
                {
                    this.msFeatureWindowFactory.CreateNewWindow(
                        await this.GetFeatures(this.selectedDatasets.Where(file => file.IsAligned)),
                        true);
                }
                else
                {
                    this.msFeatureWindowFactory.CreateNewWindow(
                        await this.GetFeatures(this.selectedDatasets.Where(file => file.DatasetState > DatasetInformationViewModel.DatasetStates.FindingFeatures)),
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
                            file1.Dataset.Features.Path,
                            file1.DatasetId,
                            this.featureCache.Providers.FeatureCache));
                    this.features.Add(file.Dataset, feat);
                }

                datasetFeatures.Add(file.Dataset, feat);
            }

            return datasetFeatures;
        }
    }
}
