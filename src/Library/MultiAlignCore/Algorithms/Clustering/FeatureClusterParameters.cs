/*////////////////////////////////////////////////////////////////////////////////////////////////////////////
 * 
 * Name:    UMC Single Linkage Cluster Parameters
 * File:    UMCSingleLinkageClustererParameters.cs
 * Author:  Brian LaMarche 
 * Purpose: Parameters for performing single linkage clustering.
 * Date:    5-19-2010
 * Revisions:
 *          5-19-2010 - BLL - Created class for single linkage clustering parameters.
 ////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

using System;
using MultiAlignCore.Algorithms.Distance;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Parameters for the single linkage UMC Clustering Algorithm.
    /// </summary>
    public class FeatureClusterParameters<T>
        where T: FeatureLight, new()
    {        
        #region Constants
        /// <summary>
        /// Default value whether to separate UMC's based on charge state information.
        /// </summary>
        public const bool CONST_DEFAULT_ONLY_CLUSTER_SAME_CHARGE_STATES = false;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FeatureClusterParameters()
        {
            Clear();
        }

        #region Properties
        public ClusterCentroidRepresentation CentroidRepresentation
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the tolerance values for the clustering algorithm.
        /// </summary>
        public FeatureTolerances Tolerances
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether to separate features based on charge state.
        /// </summary>
        public bool OnlyClusterSameChargeStates
        { 
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the distance function to use for calculating the distance between two UMC's.
        /// </summary>
        public Distance.DistanceFunction<T> DistanceFunction
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the function that determines if two features are within range of each other.
        /// </summary>
        public Distance.WithinTolerances<T> RangeFunction
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Resets the parameters to their default values.
        /// </summary>
        public virtual void Clear()
        {
            Tolerances                      = new FeatureTolerances();
            OnlyClusterSameChargeStates     = CONST_DEFAULT_ONLY_CLUSTER_SAME_CHARGE_STATES;            
            DistanceFunction                = Distance.DistanceFactory<T>.CreateDistanceFunction(DistanceMetric.WeightedEuclidean);
            RangeFunction                   = WithinRange;
            CentroidRepresentation          = ClusterCentroidRepresentation.Median;
        }

        #region Distance Functions
        /// <summary>
        /// Computes the mass difference between two features.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool WithinRange(T x, T y)
        {
			// later is more related to determining a scalar value instead.
            var massDiff = Math.Abs(FeatureLight.ComputeMassPPMDifference(x.MassMonoisotopicAligned, y.MassMonoisotopicAligned));
			var netDiff          = Math.Abs(x.Net - y.Net);
			var driftDiff        = Math.Abs(x.DriftTime - y.DriftTime);

			// Make sure we fall within the distance range before computing...
            return (massDiff <= Tolerances.Mass && netDiff <= Tolerances.Net && driftDiff <= Tolerances.DriftTime);            
        }                
        #endregion
    }
}
