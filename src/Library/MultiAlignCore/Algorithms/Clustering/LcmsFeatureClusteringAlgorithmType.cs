namespace MultiAlignCore.Algorithms.Clustering
{    
    public enum ClusteringAlgorithmTypes
    {
        AverageLinkage,
        SingleLinkage,
        Centroid,
        Prims,
        Promex,
        BinarySearchTree
    }

    public class ClusteringAlgorithms
    {
        public static readonly ClusteringAlgorithmTypes[] LcmsFeatureClusteringAlgorithms =
        {
            ClusteringAlgorithmTypes.AverageLinkage,
            ClusteringAlgorithmTypes.SingleLinkage, 
            ClusteringAlgorithmTypes.Centroid,
            ClusteringAlgorithmTypes.Prims,
            ClusteringAlgorithmTypes.Promex
        };

        public static readonly ClusteringAlgorithmTypes[] MsFeatureClusteringAlgorithms =
        {
            ClusteringAlgorithmTypes.AverageLinkage,
            ClusteringAlgorithmTypes.SingleLinkage,
            ClusteringAlgorithmTypes.Centroid,
            ClusteringAlgorithmTypes.Prims,
            ClusteringAlgorithmTypes.BinarySearchTree
        };

        public static readonly ClusteringAlgorithmTypes[] GenericClusteringAlgorithms =
        {
            ClusteringAlgorithmTypes.AverageLinkage,
            ClusteringAlgorithmTypes.SingleLinkage,
            ClusteringAlgorithmTypes.Centroid,
            ClusteringAlgorithmTypes.Prims,
            ClusteringAlgorithmTypes.BinarySearchTree
        };
    }
}
