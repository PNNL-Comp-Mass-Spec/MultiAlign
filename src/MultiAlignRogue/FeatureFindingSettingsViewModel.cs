using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MultiAlign.Data;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data.Features;

namespace MultiAlignRogue
{
    public class FeatureFindingSettingsViewModel : ViewModelBase
    {
        private readonly MultiAlignAnalysis analysis;

        private readonly FeatureLoader featureCache;

        private readonly IFeatureWindowFactory msFeatureWindowFactory;

        private readonly IProgress<int> progress;

        private Dictionary<DatasetInformation, IList<UMCLight>> features;

        private IReadOnlyCollection<DatasetInformation> selectedDatasets;

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
            this.selectedDatasets = new ReadOnlyCollection<DatasetInformation>(new List<DatasetInformation>());
            this.msFeatureWindowFactory = new MSFeatureViewFactory();

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
            get { return this.analysis.Options.InstrumentTolerances.Mass; }
            set
            {
                this.analysis.Options.AlignmentOptions.MassTolerance = value;
                this.analysis.Options.InstrumentTolerances.Mass = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumFeatureLength
        {
            get { return this.analysis.Options.LcmsFilteringOptions.FeatureLengthRange.Minimum; }
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FeatureLengthRange.Minimum = value;
                this.analysis.Options.LcmsFilteringOptions.FeatureLengthRange.Minimum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MaximumFeatureLength
        {
            get { return this.analysis.Options.LcmsFilteringOptions.FeatureLengthRange.Maximum; }
            set
            {
                this.analysis.Options.LcmsFilteringOptions.FeatureLengthRange.Maximum = value;
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
            await Task.Run(() => LoadFeatures());
        }

        private void LoadFeatures()
        {
            this.featureCache.Providers = this.analysis.DataProviders;
            var selectedFiles = selectedDatasets;
            foreach (var file in selectedFiles.Where(file => !file.DoingWork)) // Do not try to run on files already loading features.
            {
                file.DoingWork = true;
                ThreadSafeDispatcher.Invoke(() => PlotMSFeaturesCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => FindMSFeaturesCommand.RaiseCanExecuteChanged());
                var features = this.featureCache.LoadDataset(file, this.analysis.Options.MsFilteringOptions, this.analysis.Options.LcmsFindingOptions, this.analysis.Options.LcmsFilteringOptions);
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
                features = new Dictionary<DatasetInformation, IList<UMCLight>>();
                foreach (var file in selectedDatasets.Where(file => file.FeaturesFound)) // Select only datasets with features.
                {
                    var feat = await Task.Run(() => UmcLoaderFactory.LoadUmcFeatureData(file.Features.Path, file.DatasetId,
                            featureCache.Providers.FeatureCache));

                    features.Add(file, feat);
                }
                msFeatureWindowFactory.CreateNewWindow(features);
            }
            catch
            {
                MessageBox.Show("Feature cache currently being accessed. Try again in a few moments");
            }
        }
    }
}
