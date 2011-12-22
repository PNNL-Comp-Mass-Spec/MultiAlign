using System.Collections.Generic;
using System.IO;
using System.Text;
using MultiAlignCore.Algorithms.Features;
using MultiAlignCore.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

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
            Path = path + "_crosstab.csv";
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
            WriteClusters(clusters, new Dictionary<int, List<ClusterToMassTagMap>>(), datasets, new Dictionary<string, MassTagLight>());
        }
        #endregion

        #region IFeatureClusterWriter Members
        public void WriteClusters(  List<UMCClusterLight>                                       clusters,
                                    Dictionary<int, List<ClusterToMassTagMap>>                  clusterMap,
                                    List<DatasetInformation>                                    datasets, 
                                    Dictionary<string, PNNLOmics.Data.MassTags.MassTagLight>    tags)
        {

            using (TextWriter writer = File.CreateText(Path))
            {
                // Build the header.
                string mainHeader = "Cluster ID, Mono Mass, Net, Total Members, Dataset Members,  Tightness, Ambiguity";


                // Make blank columns for clusters that dont have enough dta.
                string blankColumns = ",,,,,,,,,";

                // Map the dataset ID's to a list of numbers sorted from lowest to highest.
                List<int> datasetIds = new List<int>();
                foreach (DatasetInformation info in datasets)
                {
                    datasetIds.Add(info.DatasetId);
                }
                datasetIds.Sort();

                if (clusterMap.Count > 0)
                {
                    mainHeader  += ", MassTag ID, Conformation ID, Peptide Sequence, STAC, STAC-UP";                    
                }

                string header = mainHeader;

               

                for (int i = 0; i < datasetIds.Count; i++)
                {
                    header += string.Format(", ID.{0}, DatasetID.{0}, MonoMass.{0}, Net.{0}, AbundanceMax.{0}, AbundanceSum.{0}, Scan.{0}, ScanStart.{0}, ScanEnd.{0}", datasets[i].DatasetName);
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
                            umcBuilder.Append(string.Format(",{0},{1},{2},{3},{4},{5},{6},{7},{8}", umc.ID, umc.GroupID, umc.MassMonoisotopic, umc.NET, umc.Abundance, umc.AbundanceSum, umc.Scan, umc.ScanStart, umc.ScanEnd));                            
                        }
                        else
                        {
                            umcBuilder.Append(blankColumns);
                        }
                    }

                    // We may have multiple matches to a single cluster.
                    StringBuilder builder = new StringBuilder();
                    builder.Append(string.Format("{0},{1},{2},{3},{4},{5},{6}", cluster.ID, cluster.MassMonoisotopic, cluster.RetentionTime, cluster.UMCList.Count, features.Keys.Count, cluster.Score, cluster.AmbiguityScore));
                    if (clusterMap.Count > 0)
                    {
                        if (clusterMap.ContainsKey(cluster.ID))
                        {

                            foreach (ClusterToMassTagMap map in clusterMap[cluster.ID])
                            {

                                string clusterString = builder.ToString();
                                string key           = map.ConformerId + "-" + map.MassTagId;
                                MassTagLight tag     = tags[key];                                
                                clusterString       += string.Format(",{0},{1},{2},{3},{4}", tag.ID,
                                                                                     tag.ConformationID,
                                                                                     tag.PeptideSequence,
                                                                                     map.StacScore,
                                                                                     map.StacUP);
                                writer.WriteLine(clusterString  + umcBuilder.ToString());
                            }
                        }
                        else
                        {                            
                            writer.WriteLine(builder.Append(",,,,," + umcBuilder.ToString()));
                        }
                    }
                    else
                    {
                        writer.WriteLine(builder.Append(umcBuilder.ToString()));
                    }
                }
            }
        }
        #endregion

        public override string ToString()
        {
            return "Cluster Cross Tab";
        }
    }
}
