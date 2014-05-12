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
    public class UMCClusterCrossTabWriter : BaseUmcClusterWriter
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        public UMCClusterCrossTabWriter()
            : base(false)
        {
            Extension = "_crosstab.csv";
            Name = "Cluster Cross Tab";
            Description = "Writes cluster data with all data (mass, NET, etc.) for each feature in the columns.";
        }

        protected override void Write(List<UMCClusterLight> clusters, List<DatasetInformation> datasets)
        {
            Write(clusters, new Dictionary<int, List<ClusterToMassTagMap>>(), datasets,
                new Dictionary<string, MassTagLight>());
        }

        protected override void Write(List<UMCClusterLight> clusters,
            Dictionary<int, List<ClusterToMassTagMap>> clusterMap,
            List<DatasetInformation> datasets,
            Dictionary<string, MassTagLight> tags)
        {
            using (TextWriter writer = File.CreateText(Path))
            {
                // Build the header.
                var mainHeader =
                    "Cluster ID, Mono Mass, Net, Drift Time, Charge,  Total Members, Dataset Members,  Tightness, Ambiguity";


                // Make blank columns for clusters that dont have enough dta.
                var blankColumns = ",,,,,,,,,,,";

                // Map the dataset ID's to a list of numbers sorted from lowest to highest.
                var datasetIds = new List<int>();
                foreach (var info in datasets)
                {
                    datasetIds.Add(info.DatasetId);
                }
                datasetIds.Sort();

                if (clusterMap.Count > 0)
                {
                    mainHeader +=
                        ", MassTag ID, Conformation ID, Peptide Sequence, Mass, Net, Drift Time, STAC, STAC-UP";
                }

                var header = mainHeader;

                for (var i = 0; i < datasetIds.Count; i++)
                {
                    header +=
                        string.Format(
                            ", ID.{0}, DatasetID.{0}, MonoMass.{0}, Net.{0}, DriftTime.{0}, Charge.{0},  AbundanceMax.{0}, AbundanceSum.{0}, Scan.{0}, ScanStart.{0}, ScanEnd.{0}",
                            datasets[i].DatasetName);
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
                            umcBuilder.Append(string.Format(",{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", umc.Id,
                                umc.GroupId,
                                umc.MassMonoisotopic, umc.Net, umc.DriftTime,
                                umc.ChargeState,
                                umc.Abundance, umc.AbundanceSum,
                                umc.Scan, umc.ScanStart, umc.ScanEnd));
                        }
                        else
                        {
                            umcBuilder.Append(blankColumns);
                        }
                    }

                    // We may have multiple matches to a single cluster.
                    var builder = new StringBuilder();
                    builder.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", cluster.Id,
                        cluster.MassMonoisotopic, cluster.RetentionTime, cluster.DriftTime, cluster.ChargeState,
                        cluster.UmcList.Count, features.Keys.Count, cluster.Tightness, cluster.AmbiguityScore));
                    if (clusterMap.Count > 0)
                    {
                        if (clusterMap.ContainsKey(cluster.Id))
                        {
                            foreach (var map in clusterMap[cluster.Id])
                            {
                                var clusterString = builder.ToString();
                                var key = map.ConformerId + "-" + map.MassTagId;
                                var tag = tags[key];
                                clusterString += string.Format(",{0},{1},{2},{3},{4},{5},{6},{7}", tag.Id,
                                    tag.ConformationId,
                                    tag.PeptideSequence,
                                    tag.MassMonoisotopic,
                                    tag.NetAverage,
                                    tag.DriftTime,
                                    map.StacScore,
                                    map.StacUP);
                                writer.WriteLine(clusterString + umcBuilder);
                            }
                        }
                        else
                        {
                            writer.WriteLine(builder.Append(",,,,,,,," + umcBuilder));
                        }
                    }
                    else
                    {
                        writer.WriteLine(builder.Append(umcBuilder));
                    }
                }
            }
        }
    }
}