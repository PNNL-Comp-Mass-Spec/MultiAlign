#region

using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.Algorithms
{
    public static class ClusterFactory
    {
        public static Clustering.IClusterer<UMCLight, UMCClusterLight> Create(LcmsFeatureClusteringAlgorithmType clusterType)
        {
            Clustering.IClusterer<UMCLight, UMCClusterLight> clusterer = null;
            switch (clusterType)
            {
                case LcmsFeatureClusteringAlgorithmType.AverageLinkage:
                    clusterer = new Clustering.UMCAverageLinkageClusterer<UMCLight, UMCClusterLight>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.Centroid:
                    clusterer = new Clustering.UMCCentroidClusterer<UMCLight, UMCClusterLight>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.SingleLinkage:
                    clusterer = new Clustering.UMCSingleLinkageClusterer<UMCLight, UMCClusterLight>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.FastSingleLinkage:
                    clusterer = new Clustering.MSFeatureSingleLinkageClustering<UMCLight, UMCClusterLight>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.Prims:
                    clusterer = new Clustering.UMCPrimsClustering<UMCLight, UMCClusterLight>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.BinarySearchTree:
                    clusterer = new MsFeatureTreeClusterer<UMCLight, UMCClusterLight>();
                    break;
            }

            return clusterer;
        }
    }

    public class GenericClusterFactory<T, U>
        where T : FeatureLight, IChildFeature<U>, new()
        where U : FeatureLight, IFeatureCluster<T>, new()
    {
        public IClusterer<T, U> Create(LcmsFeatureClusteringAlgorithmType clusterType)
        {
            IClusterer<T, U> clusterer = null;
            switch (clusterType)
            {
                case LcmsFeatureClusteringAlgorithmType.AverageLinkage:
                    clusterer = new UMCAverageLinkageClusterer<T, U>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.Centroid:
                    clusterer = new UMCCentroidClusterer<T, U>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.SingleLinkage:
                    clusterer = new UMCSingleLinkageClusterer<T, U>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.FastSingleLinkage:
                    clusterer = new MSFeatureSingleLinkageClustering<T, U>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.Prims:
                    clusterer = new UMCPrimsClustering<T, U>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.BinarySearchTree:
                    clusterer = new MsFeatureTreeClusterer<T, U>();
                    break;
            }

            return clusterer;
        }
    }
}