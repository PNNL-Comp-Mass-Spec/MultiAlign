namespace MultiAlignCore.Algorithms
{
    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.Data.Features;
    using System;

    /// <summary>
    /// This class is a factory for creating a clusterer for a provided Clustering algorithm.
    /// </summary>
    public static class ClusterFactory
    {
        /// <summary>
        /// Create LCMSFeature clusterer.
        /// An LCMSFeature clusterer clusters LCMSFeatures into LCMSClusters.
        /// </summary>
        /// <param name="clusterType">The type of clustering algorithm the clusterer should use.</param>
        /// <returns>The clusterer.</returns>
        /// <remarks>This clusterer is most commonly used for clustering LCMSFeatures across datasets.</remarks>
        public static IClusterer<UMCLight, UMCClusterLight> CreateLcmsFeatureClusterer(ClusteringAlgorithmTypes clusterType)
        {
            IClusterer<UMCLight, UMCClusterLight> clusterer = null;
            switch (clusterType)
            {
                case ClusteringAlgorithmTypes.AverageLinkage:
                    clusterer = new UMCAverageLinkageClusterer<UMCLight, UMCClusterLight>();
                    break;
                case ClusteringAlgorithmTypes.Centroid:
                    clusterer = new UMCCentroidClusterer<UMCLight, UMCClusterLight>();
                    break;
                case ClusteringAlgorithmTypes.SingleLinkage:
                    clusterer = new UMCSingleLinkageClusterer<UMCLight, UMCClusterLight>();
                    break;
                case ClusteringAlgorithmTypes.Prims:
                    clusterer = new UMCPrimsClustering<UMCLight, UMCClusterLight>();
                    break;
                case ClusteringAlgorithmTypes.Promex:
                    clusterer = new PromexClusterer();
                    break;
            }

            return clusterer;
        }

        /// <summary>
        /// Create a second pass feature clusterer.
        /// </summary>
        /// <param name="clusterType">The type of clustering algorithm the clusterer should use.</param>
        /// <returns>The clusterer.</returns>
        /// <remarks>
        /// A second pass feature clusterer clusters LCMSFeatures into LCMSFeatures.
        /// This is most commonly used for clustering features within a single dataset.
        /// </remarks>
        public static IClusterer<UMCLight, UMCLight> CreateSecondPassFeatureClusterer(ClusteringAlgorithmTypes clusterType)
        {
            IClusterer<UMCLight, UMCLight> clusterer = null;
            switch (clusterType)
            {
                case ClusteringAlgorithmTypes.AverageLinkage:
                    clusterer = new UMCAverageLinkageClusterer<UMCLight, UMCLight>();
                    break;
                case ClusteringAlgorithmTypes.Centroid:
                    clusterer = new UMCCentroidClusterer<UMCLight, UMCLight>();
                    break;
                case ClusteringAlgorithmTypes.SingleLinkage:
                    clusterer = new UMCSingleLinkageClusterer<UMCLight, UMCLight>();
                    break;
                case ClusteringAlgorithmTypes.Prims:
                    clusterer = new UMCPrimsClustering<UMCLight, UMCLight>();
                    break;
            }

            return clusterer;
        }

        /// <summary>
        /// Create a first pass feature clusterer.
        /// </summary>
        /// <param name="clusterType">The type of clustering algorithm the clusterer should use.</param>
        /// <returns>The clusterer.</returns>
        /// <remarks>
        /// A first pass feature clusterer clusters MSFeatures into LCMSFeatures.
        /// MSFeatures are most commonly DeconTools results.
        /// </remarks>
        public static IClusterer<MSFeatureLight, UMCLight> CreateMsFeatureClusterer(ClusteringAlgorithmTypes clusterType)
        {
            IClusterer<MSFeatureLight, UMCLight> clusterer;
            switch (clusterType)
            {
                case ClusteringAlgorithmTypes.AverageLinkage:
                    clusterer = new UMCAverageLinkageClusterer<MSFeatureLight, UMCLight>();
                    break;
                case ClusteringAlgorithmTypes.SingleLinkage:
                    clusterer = new MSFeatureSingleLinkageClustering<MSFeatureLight, UMCLight>();
                    break;
                case ClusteringAlgorithmTypes.Centroid:
                    clusterer = new UMCCentroidClusterer<MSFeatureLight, UMCLight>();
                    break;
                case ClusteringAlgorithmTypes.Prims:
                    clusterer = new UMCPrimsClustering<MSFeatureLight, UMCLight>();
                    break;
                case ClusteringAlgorithmTypes.BinarySearchTree:
                    clusterer = new MsFeatureTreeClusterer<MSFeatureLight, UMCLight>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return clusterer;
        }
    }
}