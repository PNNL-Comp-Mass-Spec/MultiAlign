#region

using System;
using System.Collections.Generic;
using System.IO;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.RawData;
using MultiAlignCore.IO.TextFiles;

#endregion

namespace MultiAlignCore.Extensions
{
    using System.Linq;

    public static class ClusterExtensions
    {
        /// <summary>
        ///     Gets a cluster and it's subsequent data structures.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="providers"></param>
        public static void ReconstructUMCCluster(this UMCClusterLight cluster, FeatureDataAccessProviders providers)
        {
            cluster.ReconstructUMCCluster(providers, true, true, true, true);
        }

        /// <summary>
        ///     Determines if MS/MS should also be discovered.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="providers"></param>
        /// <param name="getMsMS"></param>
        public static void ReconstructUMCCluster(this UMCClusterLight cluster, FeatureDataAccessProviders providers,
            bool getUmcs, bool getMatches, bool getMsFeature, bool getMsMs)
        {
            if (getUmcs)
            {
                // Reconstruct UMCs
                cluster.Features.Clear();

                var features = providers.FeatureCache.FindByClusterID(cluster.Id);

                var totalSpectra = 0;
                var totalIdentified = 0;
                foreach (var feature in features)
                {
                    cluster.AddChildFeature(feature);

                    if (getMsFeature)
                    {
                        feature.ReconstructUMC(providers, getMsMs);

                        foreach (var msFeature in feature.MsFeatures)
                        {
                            totalSpectra += msFeature.MSnSpectra.Count;
                            foreach (var spectrum in msFeature.MSnSpectra)
                            {
                                if (spectrum.Peptides.Count > 0)
                                    totalIdentified++;
                            }
                        }
                    }
                }

                cluster.IdentifiedSpectraCount = totalIdentified;
                cluster.MsMsCount = totalSpectra;   
            }

            if (getMatches)
            {
                // Reconstruct matches
                cluster.MassTags.Clear();
                var matches = providers.MassTagMatches.FindByClusterId(cluster.Id);
                if (matches != null && matches.Any())
                {
                    var massTags = providers.MassTags.FindMassTags(matches.Select(match => match.MassTagId).ToList());
                    cluster.MassTags.AddRange(massTags);
                }   
            }
        }

        /// <summary>
        ///     Retrieves a list of known peptides attributed to this cluster.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        public static List<Peptide> FindPeptides(this UMCClusterLight cluster)
        {
            var peptides = new List<Peptide>();

            foreach (var feature in cluster.Features)
            {
                foreach (var msFeature in feature.MsFeatures)
                {
                    foreach (var spectrum in msFeature.MSnSpectra)
                    {
                        peptides.AddRange(spectrum.Peptides);
                    }
                }
            }
            return peptides;
        }

        /// <summary>
        ///     Retrieves a list of known peptides attributed to this cluster.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        public static List<MSSpectra> GetLoadedSpectra(this UMCClusterLight cluster)
        {
            var spectra = new List<MSSpectra>();

            foreach (var feature in cluster.Features)
            {
                foreach (var msFeature in feature.MsFeatures)
                {
                    spectra.AddRange(msFeature.MSnSpectra);
                }
            }
            return spectra;
        }

        /// <summary>
        ///     Retrieves a list of known peptides attributed to this cluster.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        public static List<MSSpectra> FindSpectra(this UMCClusterLight cluster, FeatureDataAccessProviders providers)
        {
            var peptides = new List<MSSpectra>();
            return peptides;
        }

        /// <summary>
        ///     Retrieves a list of known peptides attributed to this cluster.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        public static List<MSSpectra> FindSpectra(this UMCClusterLight cluster)
        {
            var peptides = new List<MSSpectra>();
            foreach (var feature in cluster.Features)
            {
                foreach (var msFeature in feature.MsFeatures)
                {
                    peptides.AddRange(msFeature.MSnSpectra);
                }
            }
            return peptides;
        }

        /// <summary>
        ///     Reconstructs the mass tags and clusters joining tabled data.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="matches"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public static Tuple<List<UMCClusterLightMatched>, List<MassTagToCluster>> MapMassTagsToClusters(
            this List<UMCClusterLight> clusters,
            List<ClusterToMassTagMap> matches,
            MassTagDatabase database)
        {
            var matchedClusters = new List<UMCClusterLightMatched>();
            var matchedTags = new List<MassTagToCluster>();

            // Maps a cluster ID to a cluster that was matched (or not in which case it will have zero matches).
            var clusterMap = new Dictionary<int, UMCClusterLightMatched>();

            // Maps the mass tags to clusters, the second dictionary is for the conformations.
            var massTagMap =
                new Dictionary<int, Dictionary<int, MassTagToCluster>>();

            // Index the clusters.
            foreach (var cluster in clusters)
            {
                if (!clusterMap.ContainsKey(cluster.Id))
                {
                    var matchedCluster = new UMCClusterLightMatched();
                    matchedCluster.Cluster = cluster;
                    clusterMap.Add(cluster.Id, matchedCluster);
                }
            }

            if (database != null)
            {
                // Index the mass tags.
                foreach (var tag in database.MassTags)
                {
                    if (!massTagMap.ContainsKey(tag.Id))
                    {
                        massTagMap.Add(tag.Id, new Dictionary<int, MassTagToCluster>());
                    }
                    if (!massTagMap[tag.Id].ContainsKey(tag.ConformationId))
                    {
                        var matchedTag = new MassTagToCluster();
                        matchedTag.MassTag = tag;
                        massTagMap[tag.Id].Add(tag.ConformationId, matchedTag);
                    }
                }

                // Keeps track of all the proteins that we have mapped so far.
                var proteinList = new Dictionary<int, ProteinToMassTags>();

                // Link up the protein data
                foreach (var massTagId in massTagMap.Keys)
                {
                    foreach (var conformationID in massTagMap[massTagId].Keys)
                    {
                        var clusterTag = massTagMap[massTagId][conformationID];

                        // Here we make sure we link up the protein data too
                        if (database.Proteins.ContainsKey(massTagId))
                        {
                            // Get a list of the proteins this tag mapped to.
                            var proteins = database.Proteins[massTagId];

                            // Then for each protein, wrap it with a proteintomasstag map, then
                            //    mapping the tag to the protein
                            //    and mapping the protein to the tags.
                            foreach (var p in proteins)
                            {
                                if (!proteinList.ContainsKey(p.ProteinId))
                                {
                                    var tempProtein = new ProteinToMassTags();
                                    tempProtein.Protein = p;
                                    proteinList.Add(p.ProteinId, tempProtein);
                                }

                                var protein = proteinList[p.ProteinId];

                                // Double link the data so we can go back and forth
                                protein.MassTags.Add(clusterTag);
                                clusterTag.MatchingProteins.Add(protein);
                            }
                        }
                    }
                }
            }

            // Index and align matches
            foreach (var match in matches)
            {
                // Find the cluster map
                if (clusterMap.ContainsKey(match.ClusterId))
                {
                    var cluster = clusterMap[match.ClusterId];
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

            foreach (var clusterId in clusterMap.Keys)
            {
                matchedClusters.Add(clusterMap[clusterId]);
            }

            foreach (var tagId in massTagMap.Keys)
            {
                foreach (var conformerId in massTagMap[tagId].Keys)
                {
                    matchedTags.Add(massTagMap[tagId][conformerId]);
                }
            }

            var tuple =
                new Tuple<List<UMCClusterLightMatched>, List<MassTagToCluster>>(matchedClusters, matchedTags);

            return tuple;
        }

        public static bool HasMsMs(this UMCClusterLight cluster)
        {
            foreach (var feature in cluster.Features)
            {
                var hasMsMs = feature.HasMsMs();
                if (hasMsMs)
                {
                    return true;
                }
            }
            return false;
        }

        public static void ExportMsMs(this UMCClusterLight cluster, string path, List<DatasetInformation> datasets,
            IMsMsSpectraWriter writer)
        {
            // Let's map the datasets first.
            var readers = new Dictionary<int, ISpectraProvider>();
            var information = new Dictionary<int, DatasetInformation>();

            datasets.ForEach(x => information.Add(x.DatasetId, x));

            // We are only loading what datasets we have to here!
            // The point is, each cluster or feature may have come from a different raw data source...
            // since we dont store all of the data in memory, we have to fetch it from the appropriate source.
            // This means that we have to go into the raw data and get the scans for an MSMS spectra.
            foreach (var feature in cluster.Features)
            {
                if (!readers.ContainsKey(feature.GroupId))
                {
                    if (information.ContainsKey(feature.GroupId))
                    {
                        var singleInfo = information[feature.GroupId];

                        if (singleInfo.Raw != null && singleInfo.RawPath != null)
                        {
                            // Make sure that we have a file.
                            if (!File.Exists(singleInfo.RawPath))
                                continue;

                            // Here we create a data file reader for the file we want to access.
                            var provider = RawLoaderFactory.CreateFileReader(singleInfo.RawPath);
                            // Then we make sure we key it to the provider.  
                            provider.AddDataFile(singleInfo.RawPath, feature.GroupId);
                            // Then make sure we map it for a dataset, so when we sort through a cluster
                            // we make sure that we can access in O(1) time.
                            readers.Add(feature.GroupId, provider);
                        }
                    }
                }
            }

            // We flag the first write, so that if the file exists, we overwrite.  They should have done 
            // checking to make sure that the file was already created...we dont care.
            var firstWrite = true;
            foreach (var feature in cluster.Features)
            {
                if (readers.ContainsKey(feature.GroupId))
                {
                    var provider = readers[feature.GroupId];
                    foreach (var msFeature in feature.MsFeatures)
                    {
                        foreach (var spectrum in msFeature.MSnSpectra)
                        {
                            var summary = new ScanSummary();
                            var data = provider.GetRawSpectra(spectrum.Scan, spectrum.GroupId, out summary);
                            spectrum.Peaks = data;
                            spectrum.ScanMetaData = summary;
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
        
        /// <summary>
        /// Creates a charge map for a given ms feature list.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, int> CreateClusterSizeHistogram(this IEnumerable<UMCClusterLight> clusters)
        {
            var map = new Dictionary<int, int>();
            foreach (var cluster in clusters)
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
        /// <returns></returns>
        public static Dictionary<int, int> CreateClusterDatasetMemberSizeHistogram(this IEnumerable<UMCClusterLight> clusters)
        {
            var map = new Dictionary<int, int>();
            foreach (var cluster in clusters)
            {
                if (!map.ContainsKey(cluster.DatasetMemberCount))
                {
                    map.Add(cluster.DatasetMemberCount, 0);
                }
                map[cluster.DatasetMemberCount]++;
            }

            return map;
        }


        public static Dictionary<int, int> BuildChargeStateHistogram(this IEnumerable<UMCClusterLight> clusters)
        {
            var chargeHistogram = new Dictionary<int, int>();
            for (var i = 1; i < 10; i++)
            {
                chargeHistogram.Add(i, 0);
            }
            foreach (var cluster in clusters)
            {
                foreach (var feature in cluster.Features)
                {
                    var chargeMap = feature.CreateChargeMap();
                    foreach (var chargeDouble in chargeMap.Keys)
                    {
                        if (!chargeHistogram.ContainsKey(chargeDouble))
                            chargeHistogram.Add(chargeDouble, 0);
                        chargeHistogram[chargeDouble] = chargeHistogram[chargeDouble] + 1;
                    }
                }
            }
            return chargeHistogram;
        }

        public static Dictionary<int, int> BuildChargeStateHistogram(this UMCClusterLight cluster)
        {
            var chargeHistogram = new Dictionary<int, int>();
            for (var i = 1; i < 10; i++)
            {
                chargeHistogram.Add(i, 0);
            }
            foreach (var feature in cluster.Features)
            {
                var chargeMap = feature.CreateChargeMap();
                foreach (var charge in chargeMap.Keys)
                {
                    if (!chargeHistogram.ContainsKey(charge))
                        chargeHistogram.Add(charge, 0);
                    chargeHistogram[charge] = chargeHistogram[charge] + 1;
                }
            }
            return chargeHistogram;
        }
    }
}