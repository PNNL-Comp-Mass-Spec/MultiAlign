using System;
using System.Collections.Generic;

using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;

using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;

namespace FOX.Data
{
    /// <summary>
    /// Maps data structures from MultiAlign old to OMICS new and vice versa.
    /// </summary>
    public static class OMICSMapper
    {
        /// <summary>
        /// Maps a list of clusters to cluster lights.
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static List<UMCClusterLight> ClusterToOmicsCluster(List<clsCluster> clusters)
        {
            List<UMCClusterLight> clustersOmics = new List<UMCClusterLight>();
            foreach (clsCluster cluster in clusters)
            {
                UMCClusterLight clusterLight    = new UMCClusterLight();
                clusterLight.ChargeState        = cluster.Charge;
                clusterLight.DriftTime          = cluster.DriftTime;
                clusterLight.ID                 = cluster.Id;
                clusterLight.MassMonoisotopic   = cluster.Mass;
                clusterLight.NET                = cluster.Net;
                clustersOmics.Add(clusterLight);
            }

            return clustersOmics;
        }
    }
}
