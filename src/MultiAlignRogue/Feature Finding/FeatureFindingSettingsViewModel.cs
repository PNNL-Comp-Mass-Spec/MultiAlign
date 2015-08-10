using System.Windows;
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

    using PNNLOmics.Data.Features;

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
            });

            this.FindMSFeaturesCommand = new RelayCommand(
                                        async () => await this.LoadMSFeaturesAsync(),
                                        () => this.selectedDatasets != null && 
                                              this.selectedDatasets.Count > 0 && 
                                              this.selectedDatasets.Any(file => !file.IsFindingFeatures));
            this.PlotMSFeaturesCommand = new RelayCommand(
                                        async () => await this.PlotMSFeatures(false), 
                                        () => this.selectedDatasets.Any(file => file.DatasetState > DatasetInformation.DatasetStates.FindingFeatures));

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
            await Task.Run(() => this.LoadFeatures());
        }

        private void LoadFeatures()
        {
            this.featureCache.Providers = this.analysis.DataProviders;
            var selectedFiles = this.selectedDatasets.Where(file => !file.DoingWork).ToList();
            foreach (var file in selectedFiles)
            {
                file.DatasetState = DatasetInformation.DatasetStates.FindingFeatures;
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

                file.DatasetState = DatasetInformation.DatasetStates.PersistingFeatures;
                ThreadSafeDispatcher.Invoke(() => this.PlotMSFeaturesCommand.RaiseCanExecuteChanged());

                this.featureCache.CacheFeatures(features);

                file.DatasetState = DatasetInformation.DatasetStates.FeaturesFound;
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
                        await this.GetFeatures(this.selectedDatasets.Where(file => file.DatasetState > DatasetInformation.DatasetStates.FindingFeatures)),
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
