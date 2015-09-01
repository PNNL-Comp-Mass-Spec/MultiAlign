﻿using System.Diagnostics;
using System.IO;
using System.Windows;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Algorithms.Clustering;
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
            this.MsFeatureClusterers = new ObservableCollection<LcmsFeatureClusteringAlgorithmType>(
                                       Enum.GetValues(typeof(LcmsFeatureClusteringAlgorithmType)).Cast<LcmsFeatureClusteringAlgorithmType>());

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

        public ObservableCollection<LcmsFeatureClusteringAlgorithmType> MsFeatureClusterers { get; private set; } 

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

        public LcmsFeatureClusteringAlgorithmType FirstPassClusterer
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

        public LcmsFeatureClusteringAlgorithmType SecondPassClusterer
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
                var progress = new Progress<ProgressData>(progData => file.Progress = progData.Percent);
                var features = this.featureCache.LoadDataset(
                                                    file.Dataset,
                                                    this.analysis.Options.MsFilteringOptions,
                                                    this.analysis.Options.LcmsFindingOptions,
                                                    this.analysis.Options.LcmsFilteringOptions,
                                                    progress);

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