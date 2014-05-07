using System.Collections.Generic;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Maintains a list of all clusters in memory.
    /// </summary>
    public static class UMCClusterLightCacheManager
    {
        /// <summary>
        /// Maintains a list of references to clusters based on id.
        /// </summary>
        private static Dictionary<int, UMCClusterLightMatched> m_clusterMap;

        /// <summary>
        /// Sets the list of clusters to be used later on.
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static void SetClusters(List<UMCClusterLightMatched> clusters)
        {
            if (m_clusterMap == null)
            {
                m_clusterMap = new Dictionary<int,UMCClusterLightMatched>();
            }

            m_clusterMap.Clear();
            clusters.ForEach(x => m_clusterMap.Add(x.Cluster.Id, x));
        }
        /// <summary>
        /// Finds the cluster based on it's id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static UMCClusterLightMatched FindById(int id)
        {
            if (m_clusterMap.ContainsKey(id))
            {
                return m_clusterMap[id];
            }
            return null;
        }
    }
}
