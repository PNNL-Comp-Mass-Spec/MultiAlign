using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Algorithms.Features;

namespace MultiAlignCore.IO.Features
{
    
    /// <summary>
    /// Writes a list of clusters to a cross tab.
    /// </summary>
    public class UMCClusterAbundanceCrossTabWriter : IFeatureClusterWriter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path"></param>
        public UMCClusterAbundanceCrossTabWriter(string path)
        {
            Path = path + "_abundance.csv"; ;
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
        public void WriteClusters(List<UMCClusterLight> clusters,
                                    List<DatasetInformation> datasets)
        {
            WriteClusters(clusters, new Dictionary<int, List<ClusterToMassTagMap>>(), datasets, new Dictionary<string, MassTagLight>());        
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clusters"></param>
        public void WriteClusters(List<UMCClusterLight> clusters,
                                    Dictionary<int, List<ClusterToMassTagMap>> clusterMap,
                                    List<DatasetInformation>             datasets,
                                    Dictionary<string, MassTagLight>     tags)
        {
            using (TextWriter writer = File.CreateText(Path))
            {
                // Build the header.
                string mainHeader = "Cluster ID, Total Members, Dataset Members,  Tightness, Ambiguity";              
                
                // Make blank columns for clusters that dont have enough dta.
                string blankColumns = ",,";

                // Map the dataset ID's to a list of numbers sorted from lowest to highest.
                List<int> datasetIds = new List<int>();
                foreach (DatasetInformation info in datasets)
                {
                    datasetIds.Add(info.DatasetId);                    
                }
                datasetIds.Sort();

                if (clusterMap.Count > 0)
                {
                    mainHeader += ", MassTag ID, Conformation ID, Peptide Sequence, STAC, STAC-UP";
                }

                string header = mainHeader;
                for (int i = 0; i < datasetIds.Count; i++)
                {
                    header += string.Format(", AbundanceMax-{0}, AbundanceSum-{0}", datasetIds[i]);
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
                            umcBuilder.Append(string.Format(",{0},{1}", umc.Abundance, umc.AbundanceSum));
                        }
                        else
                        {
                            umcBuilder.Append(blankColumns);
                        }                        
                    }

                    StringBuilder builder = new StringBuilder();
                    builder.Append(string.Format("{0},{1},{2},{3},{4}", cluster.ID, cluster.UMCList.Count, features.Keys.Count, cluster.Score, cluster.AmbiguityScore));


                    if (clusterMap.Count > 0)
                    {
                        if (clusterMap.ContainsKey(cluster.ID))
                        {

                            foreach (ClusterToMassTagMap map in clusterMap[cluster.ID])
                            {

                                string clusterString = builder.ToString();
                                string key = map.ConformerId + "-" + map.MassTagId;
                                MassTagLight tag = tags[key];
                                clusterString += string.Format(",{0},{1},{2},{3},{4}", tag.ID,
                                                                                     tag.ConformationID,
                                                                                     tag.PeptideSequence,
                                                                                     map.StacScore,
                                                                                     map.StacUP);
                                writer.WriteLine(clusterString + umcBuilder.ToString());
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
            return "Abundance Cross Tab";
        }
    }
}
