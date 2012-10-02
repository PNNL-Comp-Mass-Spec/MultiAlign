﻿using System.Collections.Generic;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;
using MultiAlignCore.Extensions;

namespace MultiAlignCore.Data.Cluster
{
    public static class ClusterStats
    {
        /// <summary>
        /// Creates a cluster size histogram for dataset members.
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static float[] GetClusterMemberSizes(List<UMCClusterLight> clusters)
        {            
            if (clusters.Count < 1)
                return null;

            Dictionary<int, int> clusterMaps = clusters.CreateClusterDatasetMemeberSizeHistogram();
                        
            // Find the maximum cluster size.
            List<int> sizes = new List<int>();
            foreach (int key in clusterMaps.Keys)
            {
                sizes.Add(key);
            }
            sizes.Sort();
            int maxClusters = sizes[sizes.Count - 1] + 3;

            // Create the histogram.
            float[] bins  = new float[maxClusters];
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
