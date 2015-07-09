using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MultiAlign.Data;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data.Features;

namespace MultiAlignRogue
{
    public class FeatureFindingSettingsViewModel : ViewModelBase
    {
        private readonly MultiAlignAnalysisOptions analysisOptions;

        private readonly FeatureDataAccessProviders providers;

        private readonly FeatureLoader featureCache;

        private readonly IProgress<int> progress;

        private Dictionary<DatasetInformation, IList<UMCLight>> Features { get; set; }

        private IReadOnlyCollection<DatasetInformation> selectedDatasets;

        private IFeatureWindowFactory msFeatureWindowFactory;

        public FeatureFindingSettingsViewModel(MultiAlignAnalysisOptions analysisOptions, FeatureLoader featureCache, FeatureDataAccessProviders providers, IProgress<int> progressReporter = null)
        {
            this.analysisOptions = analysisOptions;
            this.featureCache = featureCache;
            this.providers = providers;
            this.progress = progressReporter ?? new Progress<int>();
            this.selectedDatasets = new ReadOnlyCollection<DatasetInformation>(new List<DatasetInformation>());

            MessengerInstance.Register<PropertyChangedMessage<IReadOnlyCollection<DatasetInformation>>>(this, sds =>
            {
                this.selectedDatasets = sds.NewValue;
                this.FindMSFeaturesCommand.RaiseCanExecuteChanged();
                this.PlotMSFeaturesCommand.RaiseCanExecuteChanged();
            });

            FindMSFeaturesCommand = new RelayCommand(
                                        async () => await LoadMSFeaturesAsync(),
                                        () => this.selectedDatasets != null && 
                                              this.selectedDatasets.Count > 0 && 
                                              this.selectedDatasets.Any(file => !file.DoingWork));
            PlotMSFeaturesCommand = new RelayCommand(
                                        async () => await PlotMSFeatures(), 
                                        () => this.selectedDatasets.Any(file => file.FeaturesFound));
        }

        public RelayCommand FindMSFeaturesCommand { get; private set; }

        public RelayCommand PlotMSFeaturesCommand { get; private set; }

        public double MassResolution
        {
            get { return this.analysisOptions.InstrumentTolerances.Mass; }
            set
            {
                this.analysisOptions.AlignmentOptions.MassTolerance = value;
                this.analysisOptions.InstrumentTolerances.Mass = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumFeatureLength
        {
            get { return this.analysisOptions.LcmsFilteringOptions.FeatureLengthRange.Minimum; }
            set
            {
                this.analysisOptions.LcmsFilteringOptions.FeatureLengthRange.Minimum = value;
                this.analysisOptions.LcmsFilteringOptions.FeatureLengthRange.Minimum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MaximumFeatureLength
        {
            get { return this.analysisOptions.LcmsFilteringOptions.FeatureLengthRange.Maximum; }
            set
            {
                this.analysisOptions.LcmsFilteringOptions.FeatureLengthRange.Maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumIntensity
        {
            get { return this.analysisOptions.MsFilteringOptions.MinimumIntensity; }
            set
            {
                this.analysisOptions.MsFilteringOptions.MinimumIntensity = value;
                this.RaisePropertyChanged();
            }
        }

        public double FragmentationTolerance
        {
            get { return this.analysisOptions.InstrumentTolerances.FragmentationWindowSize; }
            set
            {
                this.analysisOptions.InstrumentTolerances.FragmentationWindowSize = value;
                this.RaisePropertyChanged();
            }
        }

        public double MaximumMz
        {
            get { return this.analysisOptions.MsFilteringOptions.MzRange.Maximum; }
            set
            {
                this.analysisOptions.MsFilteringOptions.MzRange.Maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumMz
        {
            get { return this.analysisOptions.MsFilteringOptions.MzRange.Minimum; }
            set
            {
                this.analysisOptions.MsFilteringOptions.MzRange.Minimum = value;
                this.RaisePropertyChanged();
            }
        }


        public double MaximumCharge
        {
            get { return this.analysisOptions.MsFilteringOptions.ChargeRange.Maximum; }
            set
            {
                this.analysisOptions.MsFilteringOptions.ChargeRange.Maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumCharge
        {
            get { return this.analysisOptions.MsFilteringOptions.ChargeRange.Minimum; }
            set
            {
                this.analysisOptions.MsFilteringOptions.ChargeRange.Minimum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumDeisotopingScore
        {
            get { return this.analysisOptions.MsFilteringOptions.MinimumDeisotopingScore; }
            set
            {
                this.analysisOptions.MsFilteringOptions.MinimumDeisotopingScore = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseChargeStateFilter
        {
            get { return this.analysisOptions.MsFilteringOptions.ShouldUseChargeFilter; }
            set
            {
                this.analysisOptions.MsFilteringOptions.ShouldUseChargeFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseMzFilter
        {
            get { return this.analysisOptions.MsFilteringOptions.ShouldUseMzFilter; }
            set
            {
                this.analysisOptions.MsFilteringOptions.ShouldUseMzFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseIntensityFilter
        {
            get { return this.analysisOptions.MsFilteringOptions.ShouldUseIntensityFilter; }
            set
            {
                this.analysisOptions.MsFilteringOptions.ShouldUseIntensityFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseDeisotopingFilter
        {
            get { return this.analysisOptions.MsFilteringOptions.ShouldUseDeisotopingFilter; }
            set
            {
                this.analysisOptions.MsFilteringOptions.ShouldUseDeisotopingFilter = value;
                this.RaisePropertyChanged();
            }
        } 

        public async Task LoadMSFeaturesAsync()
        {
            await Task.Run(() => LoadFeatures());
        }

        private void LoadFeatures()
        {
            featureCache.Providers = this.providers;
            var selectedFiles = selectedDatasets;
            foreach (var file in selectedFiles.Where(file => !file.DoingWork)) // Do not try to run on files already loading features.
            {
                file.DoingWork = true;
                ThreadSafeDispatcher.Invoke(() => PlotMSFeaturesCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => FindMSFeaturesCommand.RaiseCanExecuteChanged());
                var features = this.featureCache.LoadDataset(file, this.analysisOptions.MsFilteringOptions, this.analysisOptions.LcmsFindingOptions, this.analysisOptions.LcmsFilteringOptions);
                this.featureCache.CacheFeatures(features);

                file.FeaturesFound = true;
                progress.Report(0);

                file.DoingWork = false;
                ThreadSafeDispatcher.Invoke(() => PlotMSFeaturesCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => FindMSFeaturesCommand.RaiseCanExecuteChanged());
            }
        }

        public async Task PlotMSFeatures()
        {
            try
            {
                Features = new Dictionary<DatasetInformation, IList<UMCLight>>();
                foreach (var file in selectedDatasets.Where(file => file.FeaturesFound)) // Select only datasets with features.
                {
                    var features = await Task.Run(() => UmcLoaderFactory.LoadUmcFeatureData(file.Features.Path, file.DatasetId,
                            providers.FeatureCache));

                    Features.Add(file, features);
                }
                msFeatureWindowFactory.CreateNewWindow(Features);
            }
            catch
            {
                MessageBox.Show("Feature cache currently being accessed. Try again in a few moments");
            }
        }
    }
}
