using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.Data.Features
{
    /// <summary>
    /// Holds the link from a cluster matched to a set of mass tags.
    /// </summary>
    public class UMCClusterLightMatched
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public UMCClusterLightMatched()
        {
            ClusterMatches  = new List<ClusterToMassTagMap>();
            Cluster         = null;
        }
        /// <summary>
        /// Gets or sets the cluster that was matched.
        /// </summary>
        public UMCClusterLight Cluster
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the list of AMT Matches 
        /// </summary>
        public List<ClusterToMassTagMap> ClusterMatches
        {
            get;
            set;
        }
    }
}
