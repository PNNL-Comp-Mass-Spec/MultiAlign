using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Algorithms.Features;
using MultiAlignCore.IO.Clusters;

namespace MultiAlignCore.IO.Features
{
    
    /// <summary>
    /// Writes a list of clusters to a cross tab.
    /// </summary>
    public class UMCClusterAbundanceCrossTabWriter : BaseUmcClusterWriter
    {
        private string m_path;
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path"></param>
        public UMCClusterAbundanceCrossTabWriter()
            : base(false)
        {
            Name        = "Cross Tab Abundances Max and Sum ";
            Description = "Exports a cross tab of clusters containing the abundance sum and max";
            Extension   = "_abundanceMaxSum.csv";
        }
                
        protected override void Write(List<UMCClusterLight> clusters,
                                    List<DatasetInformation> datasets)
        {
            Write(clusters, new Dictionary<int, List<ClusterToMassTagMap>>(), datasets, new Dictionary<string, MassTagLight>());        
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clusters"></param>
        protected override void Write(List<UMCClusterLight> clusters,
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
                    builder.Append(string.Format("{0},{1},{2},{3},{4}", cluster.ID, cluster.UMCList.Count, features.Keys.Count, cluster.Tightness, cluster.AmbiguityScore));


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
    }
}
