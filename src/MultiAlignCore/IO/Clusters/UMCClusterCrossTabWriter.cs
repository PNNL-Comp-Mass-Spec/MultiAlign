using System.Collections.Generic;
using System.IO;
using System.Text;
using MultiAlignCore.Algorithms.Features;
using MultiAlignCore.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.IO.Clusters;

namespace MultiAlignCore.IO.Features
{
    
    /// <summary>
    /// Writes a list of clusters to a cross tab.
    /// </summary>
    public class UMCClusterCrossTabWriter : BaseUmcClusterWriter
    {
        private string m_path;
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path"></param>
        public UMCClusterCrossTabWriter()
            : base(false)
        {
            Extension   = "_crosstab.csv";
            Name        = "Cluster Cross Tab";
            Description = "Writes cluster data with all data (mass, NET, etc.) for each feature in the columns."; 
        }

        protected override void Write(List<UMCClusterLight> clusters, List<DatasetInformation> datasets)
        {
            Write(clusters, new Dictionary<int, List<ClusterToMassTagMap>>(), datasets, new Dictionary<string, MassTagLight>());
        }
        protected override void Write(  List<UMCClusterLight>                                       clusters,
                                    Dictionary<int, List<ClusterToMassTagMap>>                  clusterMap,
                                    List<DatasetInformation>                                    datasets, 
                                    Dictionary<string, PNNLOmics.Data.MassTags.MassTagLight>    tags)
        {

            using (TextWriter writer = File.CreateText(Path))
            {
                // Build the header.
                string mainHeader = "Cluster ID, Mono Mass, Net, Drift Time, Charge,  Total Members, Dataset Members,  Tightness, Ambiguity";


                // Make blank columns for clusters that dont have enough dta.
                string blankColumns = ",,,,,,,,,,,";

                // Map the dataset ID's to a list of numbers sorted from lowest to highest.
                List<int> datasetIds = new List<int>();
                foreach (DatasetInformation info in datasets)
                {
                    datasetIds.Add(info.DatasetId);
                }
                datasetIds.Sort();

                if (clusterMap.Count > 0)
                {
                    mainHeader  += ", MassTag ID, Conformation ID, Peptide Sequence, Mass, Net, Drift Time, STAC, STAC-UP";                    
                }

                string header = mainHeader;              

                for (int i = 0; i < datasetIds.Count; i++)
                {
                    header += string.Format(", ID.{0}, DatasetID.{0}, MonoMass.{0}, Net.{0}, DriftTime.{0}, Charge.{0},  AbundanceMax.{0}, AbundanceSum.{0}, Scan.{0}, ScanStart.{0}, ScanEnd.{0}", datasets[i].DatasetName);
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
                            umcBuilder.Append(string.Format(",{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", umc.ID, umc.GroupID,
                                                                                                                umc.MassMonoisotopic, umc.NET, umc.DriftTime, 
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
                    StringBuilder builder = new StringBuilder();
                    builder.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", cluster.ID, cluster.MassMonoisotopic, cluster.RetentionTime, cluster.DriftTime, cluster.ChargeState, cluster.UMCList.Count, features.Keys.Count, cluster.Tightness, cluster.AmbiguityScore));
                    if (clusterMap.Count > 0)
                    {
                        if (clusterMap.ContainsKey(cluster.ID))
                        {

                            foreach (ClusterToMassTagMap map in clusterMap[cluster.ID])
                            {

                                string clusterString = builder.ToString();
                                string key           = map.ConformerId + "-" + map.MassTagId;
                                MassTagLight tag     = tags[key];                                
                                clusterString       += string.Format(",{0},{1},{2},{3},{4},{5},{6},{7}", tag.ID,
                                                                                                         tag.ConformationID,
                                                                                                         tag.PeptideSequence,
                                                                                                         tag.MassMonoisotopic,
                                                                                                         tag.NETAverage,                                                                                     
                                                                                                         tag.DriftTime,
                                                                                                         map.StacScore,
                                                                                                         map.StacUP);
                                writer.WriteLine(clusterString  + umcBuilder.ToString());
                            }
                        }
                        else
                        {
                            writer.WriteLine(builder.Append(",,,,,,,," + umcBuilder.ToString()));
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
