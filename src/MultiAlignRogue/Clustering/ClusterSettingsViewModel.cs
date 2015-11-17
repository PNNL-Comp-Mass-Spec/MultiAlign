using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using InformedProteomics.Backend.Utils;
using MultiAlign.Data;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Distance;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Hibernate;
using MultiAlignRogue.Utils;
using MultiAlignRogue.ViewModels;
using NHibernate.Util;

namespace MultiAlignRogue.Clustering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using MultiAlignCore.Algorithms;
    using MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing;
    using MultiAlignCore.Algorithms.Options;
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.MassTags;
    using MultiAlignCore.Extensions;
    using MultiAlignCore.IO.Features;

    public class ClusterSettingsViewModel : ViewModelBase
    {
        private readonly MultiAlignAnalysis analysis;

        private readonly MultiAlignAnalysisOptions options;

        private IUmcDAO featureCache;

        private readonly IClusterViewFactory clusterWindowFactory;

        private IClusterViewFactory clusterViewFactory;

        private readonly IProgress<int> progress;

        private readonly AlgorithmBuilder builder;
        private AlgorithmProvider algorithms;

        public RelayCommand ClusterFeaturesCommand { get; private set; }
        public RelayCommand DisplayClustersCommand { get; private set; }

        public ObservableCollection<DistanceMetric> DistanceMetrics { get; private set; }
        public ObservableCollection<ClusterCentroidRepresentation> CentroidRepresentations { get; private set; }
        public ObservableCollection<LcmsFeatureClusteringAlgorithmType> ClusteringMethods { get; private set; }

        public ClusterSettingsViewModel(
                                        MultiAlignAnalysis analysis,
                                        ObservableCollection<DatasetInformationViewModel> datasets,
                                        IClusterViewFactory clusterViewFactory = null,
                                        IProgress<int> progressReporter = null)
        {
            this.progress = progressReporter ?? new Progress<int>();
            this.analysis = analysis;
            this.Datasets = datasets;
            this.options = analysis.Options;
            this.builder = new AlgorithmBuilder();
            this.clusterViewFactory = clusterViewFactory ?? new ClusterViewFactory(analysis.DataProviders, new ClusterViewerSettings());

            // When dataset state changes, update can executes.
            this.MessengerInstance.Register<PropertyChangedMessage<DatasetInformationViewModel.DatasetStates>>(this, args =>
            {
                if (args.Sender is DatasetInformationViewModel && args.PropertyName == "DatasetState")
                {
                    ThreadSafeDispatcher.Invoke(() => this.ClusterFeaturesCommand.RaiseCanExecuteChanged());
                    ThreadSafeDispatcher.Invoke(() => this.DisplayClustersCommand.RaiseCanExecuteChanged());
                }
            });

            this.ClusterFeaturesCommand = new RelayCommand(this.AsyncClusterFeatures, () => this.Datasets.Any(ds => ds.FeaturesFound));
            this.DisplayClustersCommand = new RelayCommand(
                                                           async () => await this.DisplayFeatures(),
                                                           () => this.Datasets.Any(ds => ds.IsClustered));

            this.DistanceMetrics = new ObservableCollection<DistanceMetric>();
            Enum.GetValues(typeof(DistanceMetric)).Cast<DistanceMetric>().ToList().ForEach(x => this.DistanceMetrics.Add(x));
            this.CentroidRepresentations = new ObservableCollection<ClusterCentroidRepresentation>();
            Enum.GetValues(typeof(ClusterCentroidRepresentation)).Cast<ClusterCentroidRepresentation>().ToList().ForEach(x => this.CentroidRepresentations.Add(x));
            this.ClusteringMethods = new ObservableCollection<LcmsFeatureClusteringAlgorithmType>();
            Enum.GetValues(typeof(LcmsFeatureClusteringAlgorithmType)).Cast<LcmsFeatureClusteringAlgorithmType>().ToList().ForEach(x => this.ClusteringMethods.Add(x));

            this.PostProcessingComparisonType = new ObservableCollection<ClusterPostProcessingOptions.ClusterComparisonType>(
                Enum.GetValues(typeof(ClusterPostProcessingOptions.ClusterComparisonType)).Cast<ClusterPostProcessingOptions.ClusterComparisonType>());
        }

        public bool ShouldSeparateByCharge
        {
            get { return this.options.LcmsClusteringOptions.ShouldSeparateCharge; }
            set
            {
                this.options.LcmsClusteringOptions.ShouldSeparateCharge = value;
                this.RaisePropertyChanged("ShouldSeparateByCharge");
            }
        }

        public LcmsFeatureClusteringAlgorithmType SelectedLcmsFeatureClusteringAlgorithm
        {
            get { return this.options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm; }
            set
            {
                if (this.options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm != value)
                {
                    this.options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm = value;
                    this.RaisePropertyChanged("SelectedLcmsFeatureClusteringAlgorithm");
                    this.RaisePropertyChanged("ShouldEnableClustererSettings");
                }
            }
        }

        public ClusterCentroidRepresentation SelectedCentroidMethod
        {
            get { return this.options.LcmsClusteringOptions.ClusterCentroidRepresentation; }
            set
            {
                if (this.options.LcmsClusteringOptions.ClusterCentroidRepresentation != value)
                {
                    this.options.LcmsClusteringOptions.ClusterCentroidRepresentation = value;
                    this.RaisePropertyChanged("SelectedCentroidMethod");
                }
            }
        }

        public DistanceMetric SelectedDistanceFunction
        {
            get { return this.options.LcmsClusteringOptions.DistanceFunction; }
            set
            {
                if (this.options.LcmsClusteringOptions.DistanceFunction != value)
                {
                    this.options.LcmsClusteringOptions.DistanceFunction = value;
                    this.RaisePropertyChanged("SelectedDistanceFunction");
                }
            }
        }

        public bool ShouldRefineWithMsMs
        {
            get { return this.options.ClusterPostProcessingoptions.ShouldPerformClusterRefinement; }
            set
            {
                if (this.options.ClusterPostProcessingoptions.ShouldPerformClusterRefinement != value)
                {
                    this.options.ClusterPostProcessingoptions.ShouldPerformClusterRefinement = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<ClusterPostProcessingOptions.ClusterComparisonType> PostProcessingComparisonType { get; private set; } 

        public ClusterPostProcessingOptions.ClusterComparisonType ClusterComparisonType
        {
            get { return this.options.ClusterPostProcessingoptions.ComparisonType; }
            set
            {
                if (this.options.ClusterPostProcessingoptions.ComparisonType != value)
                {
                    this.options.ClusterPostProcessingoptions.ComparisonType = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double ClusterPostProcessingTolerance
        {
            get { return this.options.ClusterPostProcessingoptions.MsMsComparisonTolerance; }
            set
            {
                if (this.options.ClusterPostProcessingoptions.MsMsComparisonTolerance != value)
                {
                    this.options.ClusterPostProcessingoptions.MsMsComparisonTolerance = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool shouldShowProgress;

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

        private double progressPercent;

        public double ProgressPercent
        {
            get { return this.progressPercent; }
            set
            {
                if (this.progressPercent != value)
                {
                    this.progressPercent = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool ShouldEnableClustererSettings
        {
            get
            {
                return this.SelectedLcmsFeatureClusteringAlgorithm != LcmsFeatureClusteringAlgorithmType.Promex;
            }
        }

        public async void AsyncClusterFeatures()
        {
            await Task.Run(() => this.ClusterFeatures());
        }

        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set; } 

        internal void ClusterFeatures(IProgress<ProgressData> workflowProgress = null)
        {
            TaskBarProgressSingleton.ShowTaskBarProgress(this, true);
            workflowProgress = workflowProgress ?? new Progress<ProgressData>();
            IProgress<ProgressData> internalProgress = new Progress<ProgressData>(pd =>
            {
                this.progress.Report((int)pd.Percent);
                this.ProgressPercent = pd.Percent;
                TaskBarProgressSingleton.SetTaskBarProgress(this, pd.Percent);
                workflowProgress.Report(pd);
            });

            this.algorithms = this.builder.GetAlgorithmProvider(this.options);
            var clusterer = this.algorithms.Clusterer;
            clusterer.Parameters = LcmsClusteringOptions.ConvertToOmics(this.options.LcmsClusteringOptions);
            this.featureCache = this.analysis.DataProviders.FeatureCache;
            if (clusterer is PromexClusterer)
            {
                var promexClusterer = clusterer as PromexClusterer;
                promexClusterer.Readers = this.analysis.DataProviders.ScanSummaryProviderCache;
            }

            // This just tells us whether we are using mammoth memory partitions or not.          
            var clusterCount = 0;
            var providers = this.analysis.DataProviders;

            foreach (var dataset in this.Datasets)
            {
                if (dataset.FeaturesFound)
                {
                    dataset.DatasetState = DatasetInformationViewModel.DatasetStates.Clustering;
                }
            }

            ThreadSafeDispatcher.Invoke(this.ClusterFeaturesCommand.RaiseCanExecuteChanged);
            ThreadSafeDispatcher.Invoke(this.DisplayClustersCommand.RaiseCanExecuteChanged);

            this.ShouldShowProgress = true;

            // Here we see if we need to separate the charge...
            // IMS is said to require charge separation 
            if (!this.analysis.Options.LcmsClusteringOptions.ShouldSeparateCharge)
            {
                var progData = new ProgressData(internalProgress);
                IProgress<ProgressData> clusterProgress =
                    new Progress<ProgressData>(pd => progData.Report(pd.Percent));

                progData.StepRange(45);
                var features = new List<UMCLight>();
                int i = 0;
                var datasets = this.Datasets.Where(ds => ds.FeaturesFound).ToList();
                foreach (var dataset in datasets)
                {
                    features.AddRange(this.featureCache.FindByDatasetId(dataset.DatasetId));
                    var rawFilePath = dataset.Dataset.RawFile.Path;
                    this.analysis.DataProviders.ScanSummaryProviderCache.GetScanSummaryProvider(
                        rawFilePath,
                        dataset.DatasetId);

                    progData.Report(++i, datasets.Count);
                }

                progData.StepRange(50);
                var clusters = clusterer.Cluster(features, clusterProgress);
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


                if (this.ShouldRefineWithMsMs)
                {
                    progData.StepRange(75);
                    var clusterRefiner =
                        ClusterPostProcessorBuilder.GetClusterPostProcessor<UMCClusterLight, UMCLight>(
                            this.analysis.Options.ClusterPostProcessingoptions,
                            this.analysis.DataProviders);
                    clusters = clusterRefiner.Cluster(clusters, clusterProgress);
                }

                this.analysis.Clusters = clusters;
                clusters.ForEach(c => c.Abundance = c.UmcList.Sum(umc => umc.AbundanceSum));

                foreach (var dataset in this.Datasets)
                {
                    if (dataset.DatasetState == DatasetInformationViewModel.DatasetStates.Clustering)
                    {
                        dataset.DatasetState = DatasetInformationViewModel.DatasetStates.PersistingClusters;
                    }
                }

                ThreadSafeDispatcher.Invoke(this.ClusterFeaturesCommand.RaiseCanExecuteChanged);
                ThreadSafeDispatcher.Invoke(this.DisplayClustersCommand.RaiseCanExecuteChanged);

                progData.StepRange(85);
                providers.ClusterCache.ClearAllClusters();
                providers.ClusterCache.AddAllStateless(clusters, clusterProgress);

                progData.StepRange(100);
                providers.FeatureCache.UpdateAll(features, clusterProgress);

                foreach (var dataset in this.Datasets)
                {
                    if (dataset.DatasetState == DatasetInformationViewModel.DatasetStates.PersistingClusters)
                    {
                        dataset.DatasetState = DatasetInformationViewModel.DatasetStates.Clustered;
                    }
                }

                ThreadSafeDispatcher.Invoke(this.ClusterFeaturesCommand.RaiseCanExecuteChanged);
                ThreadSafeDispatcher.Invoke(this.DisplayClustersCommand.RaiseCanExecuteChanged);
            }
            else
            {
                var maxChargeState = this.featureCache.FindMaxCharge();
                var progData = new ProgressData(internalProgress);
                var clusterProgress = new Progress<ProgressData>(pd => progData.Report(pd.Percent));

                DatabaseIndexer.IndexClustersDrop(NHibernateUtil.Path);

                /*
                 * Here we cluster all charge states separately.  Probably IMS Data.
                 */
                for (var chargeState = 1; chargeState <= maxChargeState; chargeState++)
                {
                    var maxPercent = ((100.0 * chargeState) / maxChargeState);
                    var maxFirstStep = (maxPercent - progData.MaxPercentage) / 2;
                    progData.StepRange(maxFirstStep);
                    var features = this.featureCache.FindByCharge(chargeState);
                    if (features.Count < 1)
                    {
                        break;
                    }

                    var clusters = clusterer.Cluster(features, clusterProgress);
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

                    this.analysis.Clusters = clusters;
                    clusters.ForEach(c => c.Abundance = c.UmcList.Sum(umc => umc.AbundanceSum));

                    foreach (var dataset in this.Datasets)
                    {
                        if (dataset.DatasetState == DatasetInformationViewModel.DatasetStates.Clustering)
                        {
                            dataset.DatasetState = DatasetInformationViewModel.DatasetStates.PersistingClusters;
                        }
                    }

                    progData.StepRange((maxPercent - maxFirstStep) / 3);
                    this.analysis.DataProviders.ClusterCache.AddAll(this.analysis.Clusters, clusterProgress);

                    progData.StepRange(maxPercent);
                    this.analysis.DataProviders.FeatureCache.UpdateAll(features, clusterProgress);

                    ThreadSafeDispatcher.Invoke(this.ClusterFeaturesCommand.RaiseCanExecuteChanged);
                    ThreadSafeDispatcher.Invoke(this.DisplayClustersCommand.RaiseCanExecuteChanged);

                }

                DatabaseIndexer.IndexClusters(NHibernateUtil.Path);

                this.analysis.Clusters = this.analysis.DataProviders.ClusterCache.FindAll();

                foreach (var dataset in this.Datasets)
                {
                    if (dataset.DatasetState == DatasetInformationViewModel.DatasetStates.PersistingClusters)
                    {
                        dataset.DatasetState = DatasetInformationViewModel.DatasetStates.Clustered;
                    }
                }

                ThreadSafeDispatcher.Invoke(this.ClusterFeaturesCommand.RaiseCanExecuteChanged);
                ThreadSafeDispatcher.Invoke(this.DisplayClustersCommand.RaiseCanExecuteChanged);
            }

            TaskBarProgressSingleton.ShowTaskBarProgress(this, false);
            this.ShouldShowProgress = false;
        }

        public async Task DisplayFeatures()
        {
            // get clusters
            var clusters = this.analysis.Clusters ??
                           await Task.Run(() => this.analysis.DataProviders.ClusterCache.FindAll());

            // reconstruct matches
            var matches = await Task.Run(() => this.ReconstructClusterMatches(clusters));

            this.clusterViewFactory.CreateNewWindow(matches, this.analysis.DataProviders.ScanSummaryProviderCache);
        }

        private List<ClusterMatch> ReconstructClusterMatches(IEnumerable<UMCClusterLight> clusters)
        {
            var matches = new List<ClusterMatch>();
            var clusterMap = clusters.ToDictionary(cluster => cluster.Id);
            var massTagMap = this.analysis.DataProviders.MassTags.FindAll().ToDictionary(massTag => massTag.Id);
            var massTagMatches = this.analysis.DataProviders.MassTagMatches.FindAll();
            
            // Add cluster-mass tag matches
            foreach (var match in massTagMatches)
            {
                var massTag = massTagMap[match.MassTagId];
                var cluster = clusterMap[match.ClusterId];
                matches.Add(new ClusterMatch
                                {
                                    Cluster = cluster,
                                    MassTag = massTag,
                                });
            }

            // Add clusters without mass tag matches
            var usedClusters = new HashSet<int>();
            usedClusters.UnionWith(matches.Select(match => match.Cluster.Id));
            foreach (var cluster in clusters)
            {
                if (!usedClusters.Contains(cluster.Id))
                {
                    usedClusters.Add(cluster.Id);
                    matches.Add(new ClusterMatch
                                    {
                                        Cluster = cluster,
                                        MassTag = new MassTagLight()
                                    });
                }
            }

            return matches;
        }
    }
}
