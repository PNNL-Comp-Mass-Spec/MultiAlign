/// <author>Kevin Crowell</author>
/// <datecreated>01-07-2011</datecreated>
/// <summary>Perform clustering of UMC features across datasets into UMC Clusters using average linkage.</summary>

using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{
    public class UMCAverageLinkageClusterer<T, U> : LinkageClustererBase<T, U>
        where T : FeatureLight, Data.Features.IChildFeature<U>, new()
        where U : FeatureLight, Data.Features.IFeatureCluster<T>, new()
    {

        /// <summary>
        /// Default Constructor.  This sets the parameters and tolerances to their default values.
        /// </summary>
        public UMCAverageLinkageClusterer()
        {
            ShouldTestClustersWithinTolerance = true;
            Parameters      = new FeatureClusterParameters<T>();
        }


        /// <summary>
        /// Performs average linkage clustering over the data and returns a list of UMC clusters.
        /// </summary>
        /// <param name="data">Data to cluster on.</param>
        /// <param name="distances">pairwise distance between UMC's.</param>
        /// <returns>List of UMC clusters.</returns>
        public override List<U> LinkFeatures(List<Data.PairwiseDistance<T>> distances, Dictionary<int, U> clusters)
        {
            /*
             * We assume that the features have already been put into singleton clusters or have a cluster
             * already associated with them. Otherwise nothing will cluster.
             */

            // Sort links based on distance
            var newDistances = from element in distances
                               orderby element.Distance
                               select element;

            // Then do the linking
            foreach (var distance in newDistances)
            {
                var featureX = distance.FeatureX;
                var featureY = distance.FeatureY;

                var clusterX = featureX.GetParentFeature();
                var clusterY = featureY.GetParentFeature();

                // Determine if they are already clustered into the same cluster
                if (clusterX == clusterY && clusterX != null)
                {
                    continue;
                }

                var areClustersWithinDistance = AreClustersWithinTolerance(clusterX, clusterY);

                // Only cluster if the distance between the clusters is acceptable
                if (areClustersWithinDistance)
                {
                    // Remove the references for all the clusters in the group and merge them into the other cluster.
                    foreach (var umcX in clusterX.Features)
                    {
                        umcX.SetParentFeature(clusterY);
                        clusterY.AddChildFeature(umcX);
                    }
                    clusterY.CalculateStatistics(Parameters.CentroidRepresentation);
                    // Remove the old cluster so we don't process it again.
                    clusters.Remove(clusterX.Id);
                }
            }

            var array           = new U[clusters.Values.Count];
            clusters.Values.CopyTo(array, 0);

            var newClusters = new List<U>();
            newClusters.AddRange(array);

            return newClusters;
        }

        public override string ToString()
        {
            return "average linkage clustering";
        }

        protected override bool AreClustersWithinTolerance(U clusterX, U clusterY)
        {
            // We dont need to compute stats everytime we compare, only if we modify the cluster. BLL
            //clusterX.CalculateStatistics(Parameters.CentroidRepresentation);
            //clusterY.CalculateStatistics(Parameters.CentroidRepresentation);

            return base.AreClustersWithinTolerance(clusterX, clusterY);
        }

        protected override void CalculateStatistics(U cluster)
        {
            // Ignore...the clusters statistics are already clustered.
        }
    }
}
