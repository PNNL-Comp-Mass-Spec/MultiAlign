using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data;
using MultiAlignCore.Algorithms.Features;

namespace MultiAlignCore.IO.Features
{
    
    /// <summary>
    /// Writes a list of clusters to a cross tab.
    /// </summary>
    public class UMCClusterCrossTabWriter : IFeatureClusterWriter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path"></param>
        public UMCClusterCrossTabWriter(string path)
        {
            Path = path;
        }
        /// <summary>
        /// Gets or sets the path to the file to output.
        /// </summary>
        public string Path
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets 
        /// </summary>
        public LCMSFeatureConsolidator Consolidator
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
                // Build the header.
                string mainHeader   = "Cluster ID, Mono Mass, Net, Members";                
                

                // Make blank columns for clusters that dont have enough dta.
                string blankColumns = ",,,,,,,,";

                // Map the dataset ID's to a list of numbers sorted from lowest to highest.
                List<int> datasetIds = new List<int>();
                foreach (DatasetInformation info in datasets)
                {
                    datasetIds.Add(info.DatasetId);
                }
                datasetIds.Sort();

                string header = mainHeader;
                for (int i = 0; i < datasetIds.Count; i++)
                {
                    header += string.Format(", ID.{0}, DatasetID.{0}, MonoMass.{0}, Net.{0}, Abundance.{0}, Scan.{0}, ScanStart.{0}, ScanEnd.{0}", i);
                }
                writer.WriteLine(header);

                // Parse each cluster - cluster per line.
                foreach (UMCClusterLight cluster in clusters)
                {                    
                    Dictionary<int, UMCLight> features = Consolidator.ConsolidateUMCs(cluster.UMCList);
                    
                    // Build the output sets.
                    StringBuilder umcBuilder = new StringBuilder();
                    foreach (int id in datasetIds)
                    {                        
                        bool containsUMC = features.ContainsKey(id);
                        if (containsUMC)
                        {
                            UMCLight umc = features[id];
                            umcBuilder.Append(string.Format(",{0},{1},{2},{3},{4},{5},{6},{7}", umc.ID, umc.GroupID, umc.MassMonoisotopic, umc.NET, umc.Abundance, umc.Scan, umc.ScanStart, umc.ScanEnd));
                        }
                        else
                        {
                            umcBuilder.Append(blankColumns);
                        }                        
                    }

                    StringBuilder builder = new StringBuilder();
                    builder.Append(string.Format("{0},{1},{2},{3}", cluster.ID, cluster.MassMonoisotopic, cluster.RetentionTime, features.Keys.Count));

                    writer.WriteLine(builder.Append(umcBuilder.ToString()));
                }
            }
        }
        #endregion
    }
}
