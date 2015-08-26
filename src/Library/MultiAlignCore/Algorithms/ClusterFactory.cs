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
                case LcmsFeatureClusteringAlgorithmType.Prims:
                    clusterer = new Clustering.UMCPrimsClustering<UMCLight, UMCClusterLight>();
                    break;
            }

            return clusterer;
        }
    }
}