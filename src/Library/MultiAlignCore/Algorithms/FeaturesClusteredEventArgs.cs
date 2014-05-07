using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Features;
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
            Clusters    = clusters;
            ChargeState = 0;
            WasChargeStateClusteredSeparately = false;
        }
        /// <summary>
        /// Constructor for the list of clusters.
        /// </summary>
        /// <param name="clusters"></param>
        public FeaturesClusteredEventArgs(List<UMCClusterLight> clusters, int chargeState)
        {
            Clusters = clusters;
            ChargeState = chargeState;
            WasChargeStateClusteredSeparately = true;
        }
        /// <summary>
        /// Gets a list of the clusters found.
        /// </summary>
        public List<UMCClusterLight> Clusters
        {
            get;
            private set;
        }

        public int ChargeState
        {
            get;
            private set;
        }
        public bool WasChargeStateClusteredSeparately
        {
            get;
            private set;
        }
    }
}
