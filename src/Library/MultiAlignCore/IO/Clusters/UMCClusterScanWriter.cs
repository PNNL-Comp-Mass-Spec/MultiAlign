using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Clusters;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// 
    /// </summary>
    public class UMCClusterScanWriter : BaseUmcClusterWriter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public UMCClusterScanWriter()
            : base(false)
        {
            Extension   = "_scans.csv";
            Name        = "Cluster Feature Scans";
            Description = "Writes the cluster feature scan values on each line.";
        }
        
        protected override void Write(List<UMCClusterLight> clusters,
                                    Dictionary<int, List<ClusterToMassTagMap>> clusterMap,
                                    List<DatasetInformation> datasets,
                                    Dictionary<string, MassTagLight> tags)
        {
            WriteClusters(  clusters, 
                            new Dictionary<int,List<ClusterToMassTagMap>>() ,
                            datasets,
                            new Dictionary<string,MassTagLight>());
        }
        protected override void Write(List<UMCClusterLight> clusters, List<DatasetInformation> datasets)
        {
            using (TextWriter writer = File.CreateText(Path))
            {
                writer.WriteLine("Cluster ID, Mono Mass, Net, Min Net, Max Net, Feature ID, Feature Dataset ID, Feature Mono Mass, Feature Net, Feature Abundance, Scan, Scan Start, Scan End");

                foreach (var cluster in clusters)
                {
                    var minNet   = double.MaxValue;
                    var maxNet   = double.MinValue;

                    var umcBuilder = new StringBuilder();
                    foreach(var umc in cluster.UmcList)
                    {
                        minNet  = Math.Min(umc.RetentionTime, minNet);
                        maxNet  = Math.Max(umc.RetentionTime, maxNet);

                        umcBuilder.Append(string.Format(",{0},{1},{2},{3},{4},{5},{6},{7}", umc.Id, umc.GroupId, umc.MassMonoisotopic, umc.Net, umc.Abundance, umc.Scan, umc.ScanStart, umc.ScanEnd));
                    }

                    var builder = new StringBuilder();                    
                    builder.Append(string.Format("{0},{1},{2},{3},{4}", cluster.Id, cluster.MassMonoisotopic, cluster.RetentionTime, minNet, maxNet));

                    writer.WriteLine(builder.Append(umcBuilder));
                }
            }
        }
    }
}
