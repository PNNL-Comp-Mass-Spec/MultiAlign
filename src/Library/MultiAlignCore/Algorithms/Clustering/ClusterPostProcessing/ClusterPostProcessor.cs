using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing
{
    using MultiAlignCore.Algorithms.SpectralProcessing;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.RawData;

    public class ClusterPostProcessor
    {
        private readonly IFeatureComparisonScorer clusterScorer;

        private readonly SpectraProviderCache spectraProviderCache;

        public ClusterPostProcessor(IFeatureComparisonScorer clusterScorer, SpectraProviderCache spectraProviderCache)
        {
            this.clusterScorer = clusterScorer;
            this.spectraProviderCache = spectraProviderCache;
        }

        /// <summary>
        /// Breaks up clusters into smaller separate clusters that do no match well in MS/MS.
        /// </summary>
        /// <param name="cluster">The cluster to break up.</param>
        /// <returns>The resulting clusters.</returns>
        public List<UMCClusterLight> ProcessClusters(List<UMCClusterLight> clusters)
        {
            var processedClusters = new List<UMCClusterLight>();
            foreach (var cluster in clusters)
            {
                processedClusters.AddRange(this.ProcessCluster(cluster));
            }

            return processedClusters;
        }

        /// <summary>
        /// Breaks up clusters into smaller separate clusters that do no match well in MS/MS.
        /// </summary>
        /// <param name="cluster">The cluster to break up.</param>
        /// <returns>The resulting clusters.</returns>
        public List<UMCClusterLight> ProcessCluster(UMCClusterLight cluster)
        {
            var features = cluster.UmcList;
            var umcToClusterHash = new Dictionary<UMCLight, UMCClusterLight> { { features[0], new UMCClusterLight() } };
            for (int leftFeatureIndex = 1; leftFeatureIndex < features.Count; leftFeatureIndex++)
            {
                var leftFeature = features[leftFeatureIndex];
                for (int rightFeatureIndex = 0; rightFeatureIndex < leftFeatureIndex; rightFeatureIndex++)
                {
                    var rightFeature = features[rightFeatureIndex];
                    UMCClusterLight clusterMatch;

                    // Score features against each other.
                    var score = this.clusterScorer.ScoreComparison(leftFeature, rightFeature);
                    if (score > 0)
                    {   // The features are a good match.
                        // Add the left feature to the right feature's cluster.
                        clusterMatch = umcToClusterHash[rightFeature];
                        clusterMatch.AddChildFeature(leftFeature);
                        umcToClusterHash.Add(rightFeature, clusterMatch);
                    }
                    else
                    {   // Not a match cluster. Cluster the left feature on its own.
                        clusterMatch = new UMCClusterLight();
                        clusterMatch.AddChildFeature(leftFeature);
                        umcToClusterHash.Add(leftFeature, clusterMatch);
                    }

                    clusterMatch.CalculateStatistics(ClusterCentroidRepresentation.Median);
                }
            }

            return umcToClusterHash.Values.Distinct().ToList();
        }
    }
}
