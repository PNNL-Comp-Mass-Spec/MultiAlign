#region

using FeatureAlignment.Data.Features;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.Algorithms
{
    using System;

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
                case LcmsFeatureClusteringAlgorithmType.Prims:
                    clusterer = new Clustering.UMCPrimsClustering<UMCLight, UMCClusterLight>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.Promex:
                    clusterer = new Clustering.PromexClusterer();
                    break;
            }

            return clusterer;
        }

        public static Clustering.IClusterer<MSFeatureLight, UMCLight> Create(
            MsFeatureClusteringAlgorithmType clusterType)
        {
            Clustering.IClusterer<MSFeatureLight, UMCLight> clusterer = null;
            switch (clusterType)
            {
                case MsFeatureClusteringAlgorithmType.AverageLinkage:
                    clusterer = new Clustering.UMCAverageLinkageClusterer<MSFeatureLight, UMCLight>();
                    break;
                case MsFeatureClusteringAlgorithmType.SingleLinkage:
                    clusterer = new Clustering.MSFeatureSingleLinkageClustering<MSFeatureLight, UMCLight>();
                    break;
                case MsFeatureClusteringAlgorithmType.Centroid:
                    clusterer = new Clustering.UMCCentroidClusterer<MSFeatureLight, UMCLight>();
                    break;
                case MsFeatureClusteringAlgorithmType.Prims:
                    clusterer = new Clustering.UMCPrimsClustering<MSFeatureLight, UMCLight>();
                    break;
                case MsFeatureClusteringAlgorithmType.BinarySearchTree:
                    clusterer = new Clustering.MsFeatureTreeClusterer<MSFeatureLight, UMCLight>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("clusterType");
            }

            return clusterer;
        }
    }

    public class GenericClusterFactory<T, U>
        where T : FeatureLight, IChildFeature<U>, new()
        where U : FeatureLight, IFeatureCluster<T>, new()
    {
        public IClusterer<T, U> Create(GenericClusteringAlgorithmType clusterType)
        {
            IClusterer<T, U> clusterer = null;
            switch (clusterType)
            {
                case GenericClusteringAlgorithmType.AverageLinkage:
                    clusterer = new UMCAverageLinkageClusterer<T, U>();
                    break;
                case GenericClusteringAlgorithmType.Centroid:
                    clusterer = new UMCCentroidClusterer<T, U>();
                    break;
                case GenericClusteringAlgorithmType.SingleLinkage:
                    clusterer = new UMCSingleLinkageClusterer<T, U>();
                    break;
                case GenericClusteringAlgorithmType.Prims:
                    clusterer = new UMCPrimsClustering<T, U>();
                    break;
                case GenericClusteringAlgorithmType.BinarySearchTree:
                    clusterer = new Clustering.MsFeatureTreeClusterer<T, U>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Cannot create generic {0} clusterer.", clusterType));
            }

            return clusterer;
        }
    }
}