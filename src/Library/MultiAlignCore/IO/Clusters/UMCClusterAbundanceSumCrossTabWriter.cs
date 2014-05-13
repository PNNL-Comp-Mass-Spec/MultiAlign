#region

using System.Collections.Generic;
using System.IO;
using System.Text;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Clusters;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

#endregion

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    ///     Writes a list of clusters to a cross tab.
    /// </summary>
    public sealed class UmcClusterAbundanceSumCrossTabWriter : BaseUmcClusterWriter
    {
        public UmcClusterAbundanceSumCrossTabWriter()
            : base(false)
        {
            Extension = "_abundance.csv";
            Name = "Cluster Abundances";
            Description = "Writes cluster data with the summed abundance for each feature in the columns.";
        }

        #region IFeatureClusterWriter Members

        protected override void Write(List<UMCClusterLight> clusters,
            List<DatasetInformation> datasets)
        {
            WriteClusters(clusters, new Dictionary<int, List<ClusterToMassTagMap>>(), datasets,
                new Dictionary<string, MassTagLight>());
        }

        /// <summary>
        /// </summary>
        /// <param name="clusters"></param>
        protected override void Write(List<UMCClusterLight> clusters,
            Dictionary<int, List<ClusterToMassTagMap>> clusterMap,
            List<DatasetInformation> datasets,
            Dictionary<string, MassTagLight> tags)
        {
            using (TextWriter writer = File.CreateText(Path))
            {
                // Build the header.
                var mainHeader =
                    "Cluster ID, Total Members, Dataset Members,  Tightness, Ambiguity, Mass, NET, Drift time,";

                // Make blank columns for clusters that dont have enough dta.
                var blankColumns = ",";

                // Map the dataset ID's to a list of numbers sorted from lowest to highest.
                var datasetIds = new List<int>();
                foreach (var info in datasets)
                {
                    datasetIds.Add(info.DatasetId);
                }
                datasetIds.Sort();

                if (clusterMap.Count > 0)
                {
                    mainHeader += ", MassTag ID, Conformation ID, Peptide Sequence, STAC, STAC-UP";
                }

                var header = mainHeader;
                for (var i = 0; i < datasetIds.Count; i++)
                {
                    header += string.Format(", AbundanceSum-{0}", datasetIds[i]);
                }
                writer.WriteLine(header);

                // Parse each cluster - cluster per line.
                foreach (var cluster in clusters)
                {
                    var features = Consolidator.ConsolidateUMCs(cluster.UmcList);

                    // Build the output sets.
                    var umcBuilder = new StringBuilder();
                    foreach (var id in datasetIds)
                    {
                        var containsUMC = features.ContainsKey(id);
                        if (containsUMC)
                        {
                            var umc = features[id];
                            umcBuilder.Append(string.Format(",{0}", umc.AbundanceSum));
                        }
                        else
                        {
                            umcBuilder.Append(blankColumns);
                        }
                    }

                    var builder = new StringBuilder();
                    builder.Append(string.Format("{0},{1},{2},{3},{4}", cluster.Id, cluster.UmcList.Count,
                        features.Keys.Count, cluster.Tightness, cluster.AmbiguityScore));
                    builder.Append(string.Format(",{0},{1},{2},{3}", cluster.MassMonoisotopic, cluster.Net,
                        cluster.DriftTime, cluster.MsMsCount));

                    if (clusterMap.Count > 0)
                    {
                        if (clusterMap.ContainsKey(cluster.Id))
                        {
                            foreach (var map in clusterMap[cluster.Id])
                            {
                                var clusterString = builder.ToString();
                                var key = map.ConformerId + "-" + map.MassTagId;
                                var tag = tags[key];
                                clusterString += string.Format(",{0},{1},{2},{3},{4}", tag.Id,
                                    tag.ConformationId,
                                    tag.PeptideSequence,
                                    map.StacScore,
                                    map.StacUP);
                                writer.WriteLine(clusterString + umcBuilder);
                            }
                        }
                        else
                        {
                            writer.WriteLine(builder.Append(",,,,," + umcBuilder));
                        }
                    }
                    else
                    {
                        writer.WriteLine(builder.Append(umcBuilder));
                    }
                }
            }
        }

        #endregion
    }
}