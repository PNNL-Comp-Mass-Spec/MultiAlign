using System;
using System.IO;
using System.Text;  
using System.Collections.Generic;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// 
    /// </summary>
    public class UMCClusterScanWriter: IFeatureClusterWriter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path"></param>
        public UMCClusterScanWriter(string path)
        {
            Path = path;
        }

        public string Path
        {
            get;
            set;
        }

        #region IFeatureClusterWriter Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clusters"></param>
        public void WriteClusters(List<UMCClusterLight> clusters, List<DatasetInformation> datasets)
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
        #endregion
    }
}
