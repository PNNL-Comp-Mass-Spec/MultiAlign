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

    using PNNLOmics.Algorithms.Distance;
    using PNNLOmics.Algorithms.FeatureClustering;
    using PNNLOmics.Data.Features;

    using Remotion.Linq.Collections;

    using MessageBox = System.Windows.MessageBox;

    public class ClusterSettingsViewModel : ViewModelBase
    {
        private readonly MultiAlignAnalysis analysis;

        private readonly MultiAlignAnalysisOptions options;

        private IUmcDAO featureCache;

        //private readonly IClusterWindowFactory clusterWindowFactory;

        private readonly IProgress<int> progress;

        private readonly AlgorithmBuilder builder;
        private AlgorithmProvider algorithms;

        public RelayCommand ClusterFeaturesCommand { get; private set; }
        public RelayCommand DisplayClustersCommand { get; private set; }


        public ObservableCollection<DistanceMetric> DistanceMetrics { get; private set; }
        public ObservableCollection<ClusterCentroidRepresentation> CentroidRepresentations { get; private set; }
        public ObservableCollection<LcmsFeatureClusteringAlgorithmType> ClusteringMethods { get; private set; }

        public ClusterSettingsViewModel(MultiAlignAnalysis analysis, IProgress<int> progressReporter = null)
        {
            this.analysis = analysis;
            this.options = analysis.Options;
            this.builder = new AlgorithmBuilder();

            this.ClusterFeaturesCommand = new RelayCommand(this.AsyncClusterFeatures);
            this.DisplayClustersCommand = new RelayCommand(this.DisplayFeatures);

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

        public async void AsyncClusterFeatures()
        {
            await Task.Run(() => this.ClusterFeatures());
        }

        public void ClusterFeatures()
        {
            
            this.algorithms = this.builder.GetAlgorithmProvider(this.options);
            var clusterer = this.algorithms.Clusterer;
            clusterer.Parameters = LcmsClusteringOptions.ConvertToOmics(this.options.LcmsClusteringOptions);
            this.featureCache = this.analysis.DataProviders.FeatureCache;

            // This just tells us whether we are using mammoth memory partitions or not.          
            var clusterCount = 0;
            var providers = this.analysis.DataProviders;

            // Here we see if we need to separate the charge...
            // IMS is said to require charge separation 
            if (!this.analysis.Options.LcmsClusteringOptions.ShouldSeparateCharge)
            {
                var features = this.featureCache.FindAll();
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
                this.analysis.Clusters = clusters;
            }
            else
            {
                var maxChargeState = this.featureCache.FindMaxCharge();

                /*
                 * Here we cluster all charge states separately.  Probably IMS Data.
                 */
                for (var chargeState = 1; chargeState <= maxChargeState; chargeState++)
                {
                    var features = this.featureCache.FindByCharge(chargeState);
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

                    this.analysis.DataProviders.ClusterCache.AddAll(clusters);
                    this.analysis.DataProviders.FeatureCache.UpdateAll(features);
                }
                this.analysis.Clusters = this.analysis.DataProviders.ClusterCache.FindAll();
            }

            MessageBox.Show("Working Command");
        }

        public void DisplayFeatures()
        {
            MessageBox.Show("Working command");
            //TODO: Implement
        }
    }
}
