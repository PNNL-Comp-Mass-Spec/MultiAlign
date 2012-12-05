using System;
using PNNLOmics.Data.MassTags;
using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data;
using MultiAlignCore.Data;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Extensions
{
    public static class ClusterExtensions
    {        
        /// <summary>
        /// Creates a charge map for a given ms feature list.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, int> CreateClusterSizeHistogram(this List<UMCClusterLight> clusters)
        {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (UMCClusterLight cluster in clusters)
            {
                if (!map.ContainsKey(cluster.MemberCount))
                {
                    map.Add(cluster.MemberCount, 0);
                }
                map[cluster.MemberCount]++;
            }

            return map;
        }
        /// <summary>
        /// Creates a charge map for a given ms feature list.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, int> CreateClusterDatasetMemeberSizeHistogram(this List<UMCClusterLight> clusters)
        {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (UMCClusterLight cluster in clusters)
            {
                if (!map.ContainsKey(cluster.DatasetMemberCount))
                {
                    map.Add(cluster.DatasetMemberCount, 0);
                }
                map[cluster.DatasetMemberCount]++;
            }

            return map;
        }
       
        /// <summary>
        /// Gets a cluster and it's subsequent data structures.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="providers"></param>
        public static void ReconstructUMCCluster(this UMCClusterLight cluster, FeatureDataAccessProviders providers)
        {
            cluster.ReconstructUMCCluster(providers, true, true);            
        }
        /// <summary>
        /// Determines if MS/MS should also be discovered.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="providers"></param>
        /// <param name="getMsMS"></param>
        public static void ReconstructUMCCluster(this UMCClusterLight cluster, FeatureDataAccessProviders providers, bool getMsFeature, bool getMsMs)
        {
            List<UMCLight> features = providers.FeatureCache.FindByClusterID(cluster.ID);
            foreach (UMCLight feature in features)
            {
                cluster.AddChildFeature(feature);

                if (getMsFeature)
                {
                    feature.ReconstructUMC(providers, getMsMs);
                }
            }
        }        
        /// <summary>
        /// Reconstructs the mass tags and clusters joining tabled data.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="matches"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public static Tuple<List<UMCClusterLightMatched>, List<MassTagToCluster>> MapMassTagsToClusters(this List<UMCClusterLight>  clusters, 
                                                                    List<ClusterToMassTagMap>   matches,
                                                                    MassTagDatabase             database)
        {

            List<UMCClusterLightMatched> matchedClusters = new List<UMCClusterLightMatched>();
            List<MassTagToCluster> matchedTags           = new List<Data.MassTagToCluster>();

            // Maps a cluster ID to a cluster that was matched (or not in which case it will have zero matches).
            Dictionary<int, UMCClusterLightMatched> clusterMap               = new Dictionary<int, UMCClusterLightMatched>();
            
            // Maps the mass tags to clusters 
            Dictionary<int, Dictionary<int, MassTagToCluster>> massTagMap = 
                new Dictionary<int, Dictionary<int, Data.MassTagToCluster>>();

            // Index the clusters.
            foreach (UMCClusterLight cluster in clusters)
            {
                if (!clusterMap.ContainsKey(cluster.ID))
                {
                    UMCClusterLightMatched matchedCluster = new UMCClusterLightMatched();
                    matchedCluster.Cluster           = cluster;
                    clusterMap.Add(cluster.ID, matchedCluster);
                }
            }

            if (database != null)
            {
                // Index the mass tags.
                foreach (MassTagLight tag in database.MassTags)
                {
                    if (!massTagMap.ContainsKey(tag.ID))
                    {
                        massTagMap.Add(tag.ID, new Dictionary<int, MassTagToCluster>());
                    }
                    if (!massTagMap[tag.ID].ContainsKey(tag.ConformationID))
                    {
                        MassTagToCluster matchedTag = new Data.MassTagToCluster();
                        matchedTag.MassTag = tag;

                        massTagMap[tag.ID].Add(tag.ConformationID, matchedTag);
                    }
                }
            }
            
            // Index and align matches
            foreach (ClusterToMassTagMap match in matches)
            {
                // Find the cluster map
                if (clusterMap.ContainsKey(match.ClusterId))
                {
                    UMCClusterLightMatched cluster           = clusterMap[match.ClusterId];
                    cluster.ClusterMatches.Add(match);

                    MassTagToCluster tag = null;
                    if (massTagMap.ContainsKey(match.MassTagId))
                    {
                        tag = massTagMap[match.MassTagId][match.ConformerId];
                        tag.Matches.Add(cluster);
                        match.MassTag = tag;                                                
                    }
                }
            }

            foreach (int clusterId in clusterMap.Keys)
            {
                matchedClusters.Add(clusterMap[clusterId]);
            }

            foreach (int tagId in massTagMap.Keys)
            {
                foreach (int conformerId in massTagMap[tagId].Keys)
                {
                    matchedTags.Add(massTagMap[tagId][conformerId]);
                }
            }

            Tuple<List<UMCClusterLightMatched>, List<MassTagToCluster>> tuple =
                new Tuple<List<UMCClusterLightMatched>, List<Data.MassTagToCluster>>(matchedClusters, matchedTags);

            return tuple;
        }

        public static bool HasMsMs(this UMCClusterLight cluster)
        {
            foreach (UMCLight feature in cluster.Features)
            {
                bool hasMsMs = feature.HasMsMs();
                if (hasMsMs)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
