#region

using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.Algorithms
{
    using System;

    public static class ClusterFactory
    {
        public static Clustering.IClusterer<UMCLight, UMCClusterLight> CreateLcmsFeatureClusterer(ClusteringAlgorithmTypes clusterType)
        {
            Clustering.IClusterer<UMCLight, UMCClusterLight> clusterer = null;
            switch (clusterType)
            {
                case ClusteringAlgorithmTypes.AverageLinkage:
                    clusterer = new Clustering.UMCAverageLinkageClusterer<UMCLight, UMCClusterLight>();
                    break;
                case ClusteringAlgorithmTypes.Centroid:
                    clusterer = new Clustering.UMCCentroidClusterer<UMCLight, UMCClusterLight>();
                    break;
                case ClusteringAlgorithmTypes.SingleLinkage:
                    clusterer = new Clustering.UMCSingleLinkageClusterer<UMCLight, UMCClusterLight>();
                    break;
                case ClusteringAlgorithmTypes.Prims:
                    clusterer = new Clustering.UMCPrimsClustering<UMCLight, UMCClusterLight>();
                    break;
                case ClusteringAlgorithmTypes.Promex:
                    clusterer = new Clustering.PromexClusterer();
                    break;
            }

            return clusterer;
        }

        public static Clustering.IClusterer<MSFeatureLight, UMCLight> CreateMsFeatureClusterer(ClusteringAlgorithmTypes clusterType)
        {
            Clustering.IClusterer<MSFeatureLight, UMCLight> clusterer = null;
            switch (clusterType)
            {
                case ClusteringAlgorithmTypes.AverageLinkage:
                    clusterer = new Clustering.UMCAverageLinkageClusterer<MSFeatureLight, UMCLight>();
                    break;
                case ClusteringAlgorithmTypes.SingleLinkage:
                    clusterer = new Clustering.MSFeatureSingleLinkageClustering<MSFeatureLight, UMCLight>();
                    break;
                case ClusteringAlgorithmTypes.Centroid:
                    clusterer = new Clustering.UMCCentroidClusterer<MSFeatureLight, UMCLight>();
                    break;
                case ClusteringAlgorithmTypes.Prims:
                    clusterer = new Clustering.UMCPrimsClustering<MSFeatureLight, UMCLight>();
                    break;
                case ClusteringAlgorithmTypes.BinarySearchTree:
                    clusterer = new Clustering.MsFeatureTreeClusterer<MSFeatureLight, UMCLight>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return clusterer;
        }
    }
}