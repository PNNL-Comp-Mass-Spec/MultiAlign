﻿#region

using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;

#endregion

namespace MultiAlignCore.IO.Clusters
{
    /// <summary>
    /// Writes a list of clusters to a cross tab.
    /// </summary>
    public sealed class UMCClusterMsmsWriter : BaseUmcClusterWriter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public UMCClusterMsmsWriter()
            : base(true)
        {
            Extension = "_msms.csv";
            Name = "Cluster MS/MS meta-data";
            Description = "Writes the a cluster and the associated MS/MS meta-data.";
        }


        protected override void Write(List<UMCClusterLight> clusters, List<DatasetInformation> datasets)
        {
            WriteClusters(clusters,
                new Dictionary<int, List<ClusterToMassTagMap>>(),
                datasets,
                new Dictionary<string, MassTagLight>());
        }

        protected override void Write(List<UMCClusterLight> clusters,
            Dictionary<int, List<ClusterToMassTagMap>> clusterMap, List<DatasetInformation> datasets,
            Dictionary<string, MassTagLight> tags)
        {
            //using (TextWriter writer = File.CreateText(Path))
            //{
            //    // Build the header.
            //    string mainHeader = "c-id, c-net, c-mass, c-tightness, c-ambiguity, meta-id, meta-net, meta-mass, meta-drifttime, meta-stac, meta-stac-up,";
            //    mainHeader += "dataset-id, ms-feature-id, ms-mz, ms-scan, ms-charge, msn-scan, msn-precursor, msn-feature-id";
            //    writer.WriteLine(mainHeader);

            //    // Parse each cluster - cluster per line.
            //    foreach (int clusterID in clusters.Clusters.Keys)
            //    {
            //        UMCClusterLight cluster = clusters.Clusters[clusterID];
            //        StringBuilder builder = new StringBuilder();
            //        builder.AppendFormat("{0},{1},{2},{3},{4}", cluster.ID,
            //                                                    cluster.NET,
            //                                                    cluster.MassMonoisotopic,
            //                                                    cluster.Tightness,
            //                                                    cluster.AmbiguityScore);

            //        bool hasMatches = clusters.MassTagMatches.ContainsKey(clusterID);
            //        if (hasMatches)
            //        {

            //            // Now we map the mass tag data.
            //            foreach (ClusterToMassTagMap map in clusters.MassTagMatches[clusterID])
            //            {
            //                string clusterString = builder.ToString();
            //                MassTagLight tag = clusters.MassTags[map.MassTagId];

            //                clusterString += string.Format(",{0},{1},{2},{3},{4},{5}", tag.ID,
            //                                                                                         tag.NET,
            //                                                                                         tag.MassMonoisotopic,
            //                                                                                         tag.DriftTime,
            //                                                                                         map.StacScore,
            //                                                                                         map.StacUP);
            //                // Now we look at the mapped features.
            //                bool containsCluster = clusters.MappedFeatures.ContainsKey(clusterID);
            //                if (containsCluster)
            //                {
            //                    List<FeatureExtractionMap> maps = clusters.MappedFeatures[clusterID];
            //                    foreach (FeatureExtractionMap xx in maps)
            //                    {
            //                        clusterString += string.Format(",{0},{1},{2},{3},{4},{5},{6},{7}",
            //                                                                xx.DatasetID,
            //                                                                xx.MSFeatureId,
            //                                                                xx.MSMz,
            //                                                                xx.MSScan,
            //                                                                xx.MSCharge,
            //                                                                xx.MSnScan,
            //                                                                xx.MSnPrecursorMz,
            //                                                                xx.MSnFeatureId);
            //                    }
            //                }
            //                writer.WriteLine(clusterString);
            //            }
            //        }
            //        else
            //        {
            //            builder.Append(",,,,,,");

            //            // Now we look at the mapped features.
            //            bool containsCluster = clusters.MappedFeatures.ContainsKey(clusterID);
            //            if (containsCluster)
            //            {
            //                List<FeatureExtractionMap> maps = clusters.MappedFeatures[clusterID];

            //                foreach (FeatureExtractionMap map in maps)
            //                {
            //                    builder.AppendFormat(",{0},{1},{2},{3},{4},{5},{6},{7}",
            //                                                            map.DatasetID,
            //                                                            map.MSFeatureId,
            //                                                            map.MSMz,
            //                                                            map.MSScan,
            //                                                            map.MSCharge,
            //                                                            map.MSnScan,
            //                                                            map.MSnPrecursorMz,
            //                                                            map.MSnFeatureId);
            //                }
            //            }
            //            writer.WriteLine(builder.ToString());
            //        }
            //    }
            //}
        }
    }
}