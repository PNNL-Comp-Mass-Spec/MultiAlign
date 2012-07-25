using System.Collections.Generic;
using MultiAlignEngine.Features;


namespace MultiAlignCore.Data.Cluster
{
    public static class ClusterStats
    {
        /// <summary>
        /// Creates a cluster size histogram for dataset members.
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static float[] GetClusterMemberSizes(List<clsCluster> clusters)
        {
            //TODO: Move this method to a new class.
            if (clusters.Count < 1)
                return null;

            Dictionary<int, int> clusterMaps = new Dictionary<int, int>();
            // Bin all data.
            foreach (clsCluster cluster in clusters)
            {
                int members = cluster.DatasetMemberCount;
                if (!clusterMaps.ContainsKey(members))
                {
                    clusterMaps.Add(members, 0);
                }
                clusterMaps[members] = clusterMaps[members] + 1;
            }

            // Find the maximum cluster size.
            List<int> sizes = new List<int>();
            foreach (int key in clusterMaps.Keys)
            {
                sizes.Add(key);
            }
            sizes.Sort();
            int maxClusters = sizes[sizes.Count - 1] + 3;

            // Create the histogram.
            float[] bins = new float[maxClusters];
            float[] freqs = new float[maxClusters];

            int i = 0;
            for (i = 0; i < maxClusters; i++)
            {
                if (clusterMaps.ContainsKey(i))
                {
                    freqs[i] = clusterMaps[i];
                }
                else
                {
                    freqs[i] = 0;
                }
            }

            return freqs;
        }

    }
}
