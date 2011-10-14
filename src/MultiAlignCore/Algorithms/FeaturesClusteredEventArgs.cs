using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Cluster;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    /// Event arguments after clustering UMC's is completed.
    /// </summary>
    public class FeaturesClusteredEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor for the list of clusters.
        /// </summary>
        /// <param name="clusters"></param>
        public FeaturesClusteredEventArgs(List<UMCClusterLight> clusters)
        {
            Clusters = clusters;
        }
        /// <summary>
        /// Gets a list of the clusters found.
        /// </summary>
        public List<UMCClusterLight> Clusters
        {
            get;
            private set;
        }
    }
}
