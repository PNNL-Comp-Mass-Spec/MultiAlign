using System;
using System.IO;
using System.Text;  
using System.Collections.Generic;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Data;
using MultiAlignCore.Algorithms.Features;
using MultiAlignCore.IO.Clusters;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// 
    /// </summary>
    public class UMCClusterScanWriter : BaseUmcClusterWriter
    {
        private string m_path;
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path"></param>
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

                foreach (UMCClusterLight cluster in clusters)
                {
                    double minNet   = double.MaxValue;
                    double maxNet   = double.MinValue;

                    StringBuilder umcBuilder = new StringBuilder();
                    foreach(UMCLight umc in cluster.UMCList)
                    {
                        minNet  = Math.Min(umc.RetentionTime, minNet);
                        maxNet  = Math.Max(umc.RetentionTime, maxNet);

                        umcBuilder.Append(string.Format(",{0},{1},{2},{3},{4},{5},{6},{7}", umc.ID, umc.GroupID, umc.MassMonoisotopic, umc.NET, umc.Abundance, umc.Scan, umc.ScanStart, umc.ScanEnd));
                    }

                    StringBuilder builder = new StringBuilder();                    
                    builder.Append(string.Format("{0},{1},{2},{3},{4}", cluster.ID, cluster.MassMonoisotopic, cluster.RetentionTime, minNet, maxNet));

                    writer.WriteLine(builder.Append(umcBuilder.ToString()));
                }
            }
        }
    }
}
