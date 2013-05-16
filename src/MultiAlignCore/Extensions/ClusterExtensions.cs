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
using PNNLOmicsIO.IO;

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
            cluster.Features.Clear();

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
            
            // Maps the mass tags to clusters, the second dictionary is for the conformations.
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

                // Keeps track of all the proteins that we have mapped so far.
                Dictionary<int, ProteinToMassTags> proteinList = new Dictionary<int,Data.ProteinToMassTags>();

                // Link up the protein data
                foreach (int massTagId in massTagMap.Keys)
                {
                    foreach (int conformationID in massTagMap[massTagId].Keys)
                    {
                        MassTagToCluster clusterTag = massTagMap[massTagId][conformationID];

                        // Here we make sure we link up the protein data too
                        if (database.Proteins.ContainsKey(massTagId))
                        {
                            // Get a list of the proteins this tag mapped to.
                            List<Protein> proteins = database.Proteins[massTagId];

                            // Then for each protein, wrap it with a proteintomasstag map, then
                            //    mapping the tag to the protein
                            //    and mapping the protein to the tags.
                            foreach (Protein p in proteins)
                            {
                                if (!proteinList.ContainsKey(p.ProteinID))
                                {
                                    ProteinToMassTags tempProtein   = new Data.ProteinToMassTags();
                                    tempProtein.Protein             = p;
                                    proteinList.Add(p.ProteinID, tempProtein);                                    
                                }

                                ProteinToMassTags protein = proteinList[p.ProteinID];

                                // Double link the data so we can go back and forth
                                protein.MassTags.Add(clusterTag);
                                clusterTag.MatchingProteins.Add(protein);
                            }                            
                        }
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

        public static void ExportMsMs(this UMCClusterLight cluster, string path, List<DatasetInformation> datasets, IMsMsSpectraWriter writer)
        {
            // Let's map the datasets first.
            Dictionary<int, ISpectraProvider> readers = new Dictionary<int,ISpectraProvider>();    
            Dictionary<int , DatasetInformation> information = new Dictionary<int,Data.DatasetInformation>();

            datasets.ForEach(x => information.Add(x.DatasetId, x));

            // We are only loading what datasets we have to here!
            // The point is, each cluster or feature may have come from a different raw data source...
            // since we dont store all of the data in memory, we have to fetch it from the appropriate source.
            // This means that we have to go into the raw data and get the scans for an MSMS spectra.
            foreach (UMCLight feature in cluster.Features)
            {
                if (!readers.ContainsKey(feature.GroupID))
                {
                    if (information.ContainsKey(feature.GroupID))
                    {
                        DatasetInformation singleInfo = information[feature.GroupID];

                        if (singleInfo.Raw != null && singleInfo.RawPath != null)
                        {
                            // Make sure that we have a file.
                            if (!System.IO.File.Exists(singleInfo.RawPath))
                                continue;

                            // Here we create a data file reader for the file we want to access.
                            ISpectraProvider provider = RawLoaderFactory.CreateFileReader(singleInfo.RawPath);
                            // Then we make sure we key it to the provider.  
                            provider.AddDataFile(singleInfo.RawPath, feature.GroupID);
                            // Then make sure we map it for a dataset, so when we sort through a cluster
                            // we make sure that we can access in O(1) time.
                            readers.Add(feature.GroupID, provider);
                        }
                    }
                }
            }

            // We flag the first write, so that if the file exists, we overwrite.  They should have done 
            // checking to make sure that the file was already created...we dont care.
            bool firstWrite = true;
            foreach (UMCLight feature in cluster.Features)
            {
                if (readers.ContainsKey(feature.GroupID))
                {
                    ISpectraProvider provider = readers[feature.GroupID];
                    foreach (MSFeatureLight msFeature in feature.MSFeatures)
                    {
                        foreach(MSSpectra spectrum in msFeature.MSnSpectra)
                        {
                            List<XYData> data = provider.GetRawSpectra(spectrum.Scan, spectrum.GroupID);
                            spectrum.Peaks    = data;
                        }
                        if (firstWrite)
                        {
                            writer.Write(path, msFeature.MSnSpectra);
                        }
                        else 
                        {
                            writer.Append(path, msFeature.MSnSpectra);
                        }
                    }
                }
            }
        }
    }
}
