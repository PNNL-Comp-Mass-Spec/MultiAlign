#region

using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignCore.Algorithms
{
    public static class ClusterFactory
    {
        public static IClusterer<UMCLight, UMCClusterLight> Create(LcmsFeatureClusteringAlgorithmType clusterType)
        {
            IClusterer<UMCLight, UMCClusterLight> clusterer = null;
            switch (clusterType)
            {
                case LcmsFeatureClusteringAlgorithmType.AverageLinkage:
                    clusterer = new UMCAverageLinkageClusterer<UMCLight, UMCClusterLight>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.Centroid:
                    clusterer = new UMCCentroidClusterer<UMCLight, UMCClusterLight>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.SingleLinkage:
                    clusterer = new UMCSingleLinkageClusterer<UMCLight, UMCClusterLight>();
                    break;
                case LcmsFeatureClusteringAlgorithmType.Prims:
                    clusterer = new UMCPrimsClustering<UMCLight, UMCClusterLight>();
                    break;
            }

            return clusterer;
        }
    }
}