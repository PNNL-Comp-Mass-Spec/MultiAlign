using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using InformedProteomics.Backend.Utils;
using MultiAlign.Data;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Distance;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
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
    using MultiAlignCore.Algorithms.Options;
    using MultiAlignCore.Data;
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
            this.DisplayClustersCommand = new RelayCommand(this.DisplayFeatures, () => this.Datasets.Any(ds => ds.IsClustered));

            this.DistanceMetrics = new ObservableCollection<DistanceMetric>();
            Enum.GetValues(typeof(DistanceMetric)).Cast<DistanceMetric>().ToList().ForEach(x => this.DistanceMetrics.Add(x));
            this.CentroidRepresentations = new ObservableCollection<ClusterCentroidRepresentation>();
            Enum.GetValues(typeof(ClusterCentroidRepresentation)).Cast<ClusterCentroidRepresentation>().ToList().ForEach(x => this.CentroidRepresentations.Add(x));
            this.ClusteringMethods = new ObservableCollection<LcmsFeatureClusteringAlgorithmType>();
            Enum.GetValues(typeof(LcmsFeatureClusteringAlgorithmType)).Cast<LcmsFeatureClusteringAlgorithmType>().ToList().ForEach(x => this.ClusteringMethods.Add(x));
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

        public async void AsyncClusterFeatures()
        {
            await Task.Run(() => this.ClusterFeatures());
        }

        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set; } 

        internal void ClusterFeatures()
        {
            IProgress<ProgressData> internalProgress = new Progress<ProgressData>(pd =>
            {
                this.progress.Report((int)pd.Percent);
                this.ProgressPercent = pd.Percent;
            });

            this.algorithms = this.builder.GetAlgorithmProvider(this.options);
            var clusterer = this.algorithms.Clusterer;
            clusterer.Parameters = LcmsClusteringOptions.ConvertToOmics(this.options.LcmsClusteringOptions);
            this.featureCache = this.analysis.DataProviders.FeatureCache;

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
                var progData = new ProgressData { IsPartialRange = true, MaxPercentage = 45 };
                IProgress<ProgressData> clusterProgress =
                    new Progress<ProgressData>(pd => internalProgress.Report(progData.UpdatePercent(pd.Percent)));

                var features = new List<UMCLight>();
                int i = 0;
                var datasets = this.Datasets.Where(ds => ds.FeaturesFound).ToList();
                foreach (var dataset in datasets)
                {
                    features.AddRange(this.featureCache.FindByDatasetId(dataset.DatasetId));
                    clusterProgress.Report(progData.UpdatePercent((100.0 * ++i) / datasets.Count));
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

                progData.StepRange(60);
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
                var clusterProgress = new Progress<ProgressData>();
                var progData = new ProgressData { IsPartialRange = true, MaxPercentage = 0 };

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
                    providers.ClusterCache.ClearAllClusters();
                    this.analysis.DataProviders.ClusterCache.AddAllStateless(this.analysis.Clusters, clusterProgress);

                    progData.StepRange(maxPercent);
                    this.analysis.DataProviders.FeatureCache.UpdateAll(features, clusterProgress);

                    ThreadSafeDispatcher.Invoke(this.ClusterFeaturesCommand.RaiseCanExecuteChanged);
                    ThreadSafeDispatcher.Invoke(this.DisplayClustersCommand.RaiseCanExecuteChanged);

                }

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

            this.ShouldShowProgress = false;
        }

        public void DisplayFeatures()
        {
            this.clusterViewFactory.CreateNewWindow(this.analysis.Clusters ?? this.analysis.DataProviders.ClusterCache.FindAll());
        }
    }
}
