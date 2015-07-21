using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Data;
using MultiAlignCore.IO;
using PNNLOmics.Algorithms.Distance;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.IO.Features;
using Remotion.Linq.Collections;

namespace MultiAlignRogue
{
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

            ClusterFeaturesCommand = new RelayCommand(AsyncClusterFeatures);
            DisplayClustersCommand = new RelayCommand(DisplayFeatures);

            DistanceMetrics = new ObservableCollection<DistanceMetric>();
            Enum.GetValues(typeof(DistanceMetric)).Cast<DistanceMetric>().ToList().ForEach(x => DistanceMetrics.Add(x));
            CentroidRepresentations = new ObservableCollection<ClusterCentroidRepresentation>();
            Enum.GetValues(typeof(ClusterCentroidRepresentation)).Cast<ClusterCentroidRepresentation>().ToList().ForEach(x => CentroidRepresentations.Add(x));
            ClusteringMethods = new ObservableCollection<LcmsFeatureClusteringAlgorithmType>();
            Enum.GetValues(typeof(LcmsFeatureClusteringAlgorithmType)).Cast<LcmsFeatureClusteringAlgorithmType>().ToList().ForEach(x => ClusteringMethods.Add(x));

        }

        public bool ShouldSeparateByCharge
        {
            get { return options.LcmsClusteringOptions.ShouldSeparateCharge; }
            set
            {
                options.LcmsClusteringOptions.ShouldSeparateCharge = value;
                this.RaisePropertyChanged("ShouldSeparateByCharge");
            }
        }

        public LcmsFeatureClusteringAlgorithmType SelectedLcmsFeatureClusteringAlgorithm
        {
            get { return options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm; }
            set
            {
                if (options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm != value)
                {
                    options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm = value;
                    this.RaisePropertyChanged("SelectedLcmsFeatureClusteringAlgorithm");
                }
            }
        }

        public ClusterCentroidRepresentation SelectedCentroidMethod
        {
            get { return options.LcmsClusteringOptions.ClusterCentroidRepresentation; }
            set
            {
                if (options.LcmsClusteringOptions.ClusterCentroidRepresentation != value)
                {
                    options.LcmsClusteringOptions.ClusterCentroidRepresentation = value;
                    this.RaisePropertyChanged("SelectedCentroidMethod");
                }
            }
        }

        public DistanceMetric SelectedDistanceFunction
        {
            get { return options.LcmsClusteringOptions.DistanceFunction; }
            set
            {
                if (options.LcmsClusteringOptions.DistanceFunction != value)
                {
                    options.LcmsClusteringOptions.DistanceFunction = value;
                    this.RaisePropertyChanged("SelectedDistanceFunction");
                }
            }
        }

        public async void AsyncClusterFeatures()
        {
            await Task.Run(() => ClusterFeatures());
        }

        public void ClusterFeatures()
        {
            
            algorithms = builder.GetAlgorithmProvider(options);
            var clusterer = algorithms.Clusterer;
            clusterer.Parameters = LcmsClusteringOptions.ConvertToOmics(options.LcmsClusteringOptions);
            featureCache = analysis.DataProviders.FeatureCache;

            // This just tells us whether we are using mammoth memory partitions or not.          
            var clusterCount = 0;
            var providers = analysis.DataProviders;

            // Here we see if we need to separate the charge...
            // IMS is said to require charge separation 
            if (!analysis.Options.LcmsClusteringOptions.ShouldSeparateCharge)
            {
                var features = featureCache.FindAll();
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
                analysis.Clusters = clusters;
            }
            else
            {
                var maxChargeState = featureCache.FindMaxCharge();

                /*
                 * Here we cluster all charge states separately.  Probably IMS Data.
                 */
                for (var chargeState = 1; chargeState <= maxChargeState; chargeState++)
                {
                    var features = featureCache.FindByCharge(chargeState);
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

                    analysis.DataProviders.ClusterCache.AddAll(clusters);
                    analysis.DataProviders.FeatureCache.UpdateAll(features);
                }
                analysis.Clusters = analysis.DataProviders.ClusterCache.FindAll();
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
