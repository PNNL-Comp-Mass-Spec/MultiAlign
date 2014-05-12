#region

using System.Collections.Generic;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignCore.Data.Features
{
    /// <summary>
    ///     Holds the link from a cluster matched to a set of mass tags.
    /// </summary>
    public class UMCClusterLightMatched : IFeatureMap
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        public UMCClusterLightMatched()
        {
            ClusterMatches = new List<ClusterToMassTagMap>();
            Cluster = null;
        }

        /// <summary>
        ///     Gets or sets the cluster that was matched.
        /// </summary>
        public UMCClusterLight Cluster { get; set; }

        /// <summary>
        ///     Gets or sets the list of AMT Matches
        /// </summary>
        public List<ClusterToMassTagMap> ClusterMatches { get; set; }


        /// <summary>
        ///     Gets the ID of the underlying feature.
        /// </summary>
        public object Id
        {
            get { return Cluster.Id; }
        }
    }
}