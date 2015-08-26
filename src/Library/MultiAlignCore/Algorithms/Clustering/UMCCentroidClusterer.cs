/// <author>Kevin Crowell</author>
/// <datecreated>01-07-2011</datecreated>
/// <summary>Perform clustering of UMC features across datasets into UMC Clusters using average linkage.</summary>

using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Links objects using a centroid distance linkage algorithm.
    /// </summary>
    /// <typeparam name="T">Object to link</typeparam>
    /// <typeparam name="U">Cluster to produce.</typeparam>
    public class UMCCentroidClusterer<T, U> : LinkageClustererBase<T, U>
        where T : FeatureLight, Data.Features.IChildFeature<U>, new()
        where U : FeatureLight, Data.Features.IFeatureCluster<T>, new()
	{

		/// <summary>
        /// Default Constructor.  This sets the parameters and tolerances to their default values.
        /// </summary>
        public UMCCentroidClusterer()
		{
            Parameters      = new FeatureClusterParameters<T>();
        }
        private double GetAverageClusterDistance(U clusterI, U clusterJ, Distance.DistanceFunction<T> function)
        {
            double sum = 0;
            foreach (var featureI in clusterI.Features)
            {
                foreach (var featureJ in clusterJ.Features)
                {
                    sum = function(featureI, featureJ);
                }
            }
            sum = sum / Convert.ToDouble(clusterI.Features.Count * clusterJ.Features.Count);

            return sum;
        }
        /// <summary>
        /// Calculates pairwise distances between features in the list of 
        /// potential features to cluster.        
        /// </summary>
        /// <param name="start">Start UMC index.</param>
        /// <param name="stop">Stop UMC index.</param>
        /// <param name="data">List of data to compute distances over.</param>
        /// <returns>List of UMC distances to consider during clustering.</returns>
        protected List<Data.PairwiseDistance<U>> CalculateDistances(Dictionary<int, U> clusters)
        {
            var massTolerance             = Parameters.Tolerances.Mass;
            var netTolerance              = Parameters.Tolerances.Net;
            var driftTolerance            = Parameters.Tolerances.DriftTime;
            var onlyClusterSameChargeStates = Parameters.OnlyClusterSameChargeStates;

            var distances = new List<Data.PairwiseDistance<U>>();
            foreach(var clusterI in clusters.Values) 
            {                
                var driftTimeX   = clusterI.DriftTime;
                var netAlignedX  = clusterI.Net;
                var massAlignedX = clusterI.MassMonoisotopicAligned;
                var chargeStateX    = clusterI.ChargeState;
                
                foreach(var clusterJ in clusters.Values)
                {
                    // Don't calculate distance to other features within same group
                    if (clusterI == clusterJ)
                    {
                        continue;
                    }

                    // Calculate the distances here (using a cube).  We dont care if we are going to re-compute
                    // these again later, because here we want to fall within the cube, the distance function used
                    // later is more related to determining a scalar value instead.
                    var massDiff = Math.Abs(FeatureLight.ComputeMassPPMDifference(massAlignedX,
                                                                        clusterJ.MassMonoisotopicAligned));
                    var netDiff = Math.Abs(netAlignedX - clusterJ.Net);
                    var driftDiff = Math.Abs(driftTimeX - clusterJ.DriftTime);

                    // Make sure we fall within the distance range before computing...
                    if (massDiff <= massTolerance && netDiff <= netTolerance && driftDiff <= driftTolerance)
                    {
                        // If IMS or equivalent only cluster similar charge states                        
                        if (onlyClusterSameChargeStates)
                        {
                            // Make sure it's the same charge state
                            if (chargeStateX == clusterJ.ChargeState)
                            {
                                // Calculate the pairwise distance
                                var pairwiseDistance = new Data.PairwiseDistance<U>();
                                pairwiseDistance.FeatureX = clusterI;
                                pairwiseDistance.FeatureY = clusterJ;
                                pairwiseDistance.Distance = GetAverageClusterDistance(clusterI, clusterJ, Parameters.DistanceFunction);
                                distances.Add(pairwiseDistance);
                            }
                        }
                        else
                        {                            
                            // Calculate the pairwise distance
                            var pairwiseDistance = new Data.PairwiseDistance<U>();
                            pairwiseDistance.FeatureX = clusterI;
                            pairwiseDistance.FeatureY = clusterJ;
                            pairwiseDistance.Distance = GetAverageClusterDistance(clusterI, clusterJ, Parameters.DistanceFunction);
                            distances.Add(pairwiseDistance);
                        }
                    }
                }
            }
            return distances;
        }		
        /// <summary>
        /// Performs average linkage clustering over the data and returns a list of UMC clusters.
        /// </summary>
        /// <param name="clusters">Singleton clusters</param>		
        /// <returns>List of T clusters.</returns>
        public override List<U> LinkFeatures(List<Data.PairwiseDistance<T>> distances, Dictionary<int, U> clusters)
        {
            var isClustering = true;            
            while (isClustering)
            {
                isClustering = false;

                // Compute pairwise distances between cluster centroids.
                var distancesClusters = CalculateDistances(clusters);
                
                // Find the minimal distance 
                var newDistances = from element in distancesClusters
                                   orderby element.Distance
                                   select element;

                // Link, we dont just take the smallest distance because
                // the two clusters may not be in tolerance.
                foreach (var distance in newDistances)
                {
                    var clusterX = distance.FeatureX;
                    var clusterY = distance.FeatureY;

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

                        // Remove the old cluster so we don't process it again.
                        clusters.Remove(clusterX.Id);
                        clusterX.CalculateStatistics(Parameters.CentroidRepresentation);
                        isClustering = true;
                        break;
                    }
                }
            }

            var array = new U[clusters.Values.Count];
            clusters.Values.CopyTo(array, 0);
            var newClusters = new List<U>();
            newClusters.AddRange(array);

            return newClusters;
        }

		/// <summary>
		/// Determines if two clusters are within mass, NET, and drift time tolerances
		/// </summary>
		/// <param name="clusterX">One of the two clusters to test</param>
		/// <param name="clusterY">One of the two clusters to test</param>
		/// <returns>True if clusters are within tolerance, false otherwise</returns>
		protected override bool AreClustersWithinTolerance(U clusterX, U clusterY)
		{
			// Grab the tolerances
			var massTolerance    = Parameters.Tolerances.Mass;
			var netTolerance     = Parameters.Tolerances.Net;
			var driftTolerance   = Parameters.Tolerances.DriftTime;

			// Calculate differences
            var massDiff = Math.Abs(FeatureLight.ComputeMassPPMDifference(clusterX.MassMonoisotopicAligned, clusterY.MassMonoisotopicAligned));
			var netDiff      = Math.Abs(clusterX.Net - clusterY.Net);
			var driftDiff    = Math.Abs(clusterX.DriftTime - clusterY.DriftTime);

			// Return true only if all differences are within tolerance
			if (massDiff <= massTolerance && netDiff <= netTolerance && driftDiff <= driftTolerance)
			{
				return true;
			}
		    return false;
		}

        public override string ToString()
        {
            return "centroid distance linkage clustering";
        }
	}
}
