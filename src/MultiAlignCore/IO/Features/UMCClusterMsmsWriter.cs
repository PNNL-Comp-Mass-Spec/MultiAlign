﻿using System.IO;
using System.Text;
using System.Collections.Generic;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Algorithms.Features;
using MultiAlignCore.Algorithms.MSLinker;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.Features;


namespace MultiAlignCore.IO.Features
{
    
    /// <summary>
    /// Writes a list of clusters to a cross tab.
    /// </summary>
    public class UMCClusterMsmsWriter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path"></param>
        public UMCClusterMsmsWriter(string path)
        {
            Path = path + "_msms.csv"; ;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clusters"></param>
        public void WriteClusters(FeaturesExtractedEventArgs clusters)
        {
            
            using (TextWriter writer = File.CreateText(Path))
            {
                // Build the header.
                string mainHeader    = "c-id, c-net, c-mass, c-tightness, c-ambiguity, meta-id, meta-net, meta-mass, meta-drifttime, meta-stac, meta-stac-up,";                                                
                mainHeader          += "dataset-id, ms-feature-id, ms-mz, ms-scan, ms-charge, msn-scan, msn-precursor, msn-feature-id"; 
                writer.WriteLine(mainHeader);

                // Parse each cluster - cluster per line.
                foreach (int clusterID in clusters.Clusters.Keys)
                {    
                    clsCluster cluster      = clusters.Clusters[clusterID];                    
                    StringBuilder builder   = new StringBuilder();
                    builder.AppendFormat("{0},{1},{2},{3},{4}", cluster.Id, 
                                                                cluster.Net, 
                                                                cluster.Mass, 
                                                                cluster.MeanScore, 
                                                                cluster.Ambiguity);
                                    
                    bool hasMatches = clusters.MassTagMatches.ContainsKey(clusterID);
                    if (hasMatches)
                    {

                        // Now we map the mass tag data.
                        foreach (ClusterToMassTagMap map in clusters.MassTagMatches[clusterID])
                        {
                            string clusterString = builder.ToString();
                            MassTagLight tag = clusters.MassTags[map.MassTagId];

                            clusterString += string.Format(",{0},{1},{2},{3},{4},{5}", tag.ID,
                                                                                                     tag.NET,
                                                                                                     tag.MassMonoisotopic,
                                                                                                     tag.DriftTime,
                                                                                                     map.StacScore,
                                                                                                     map.StacUP);
                            // Now we look at the mapped features.
                            bool containsCluster = clusters.MappedFeatures.ContainsKey(clusterID);
                            if (containsCluster)
                            {
                                List<FeatureExtractionMap> maps = clusters.MappedFeatures[clusterID];
                                foreach (FeatureExtractionMap xx in maps)
                                {
                                    clusterString += string.Format(",{0},{1},{2},{3},{4},{5},{6},{7}",
                                                                            xx.DatasetID,
                                                                            xx.MSFeatureId,
                                                                            xx.MSMz,
                                                                            xx.MSScan,
                                                                            xx.MSCharge,
                                                                            xx.MSnScan,
                                                                            xx.MSnPrecursorMz,
                                                                            xx.MSnFeatureId);
                                }
                            }
                            writer.WriteLine(clusterString);
                        }
                    }
                    else
                    {
                        builder.Append(",,,,,,");     

                        // Now we look at the mapped features.
                        bool containsCluster = clusters.MappedFeatures.ContainsKey(clusterID);
                        if (containsCluster)
                        {
                            List<FeatureExtractionMap> maps = clusters.MappedFeatures[clusterID];

                            foreach (FeatureExtractionMap map in maps)
                            {
                                builder.AppendFormat(",{0},{1},{2},{3},{4},{5},{6},{7}",
                                                                        map.DatasetID,
                                                                        map.MSFeatureId,
                                                                        map.MSMz,
                                                                        map.MSScan,
                                                                        map.MSCharge,
                                                                        map.MSnScan,
                                                                        map.MSnPrecursorMz,
                                                                        map.MSnFeatureId);
                            }
                        }
                        writer.WriteLine(builder.ToString());
                    }                    
                }
            }
        }
        public override string ToString()
        {
            return "MSMS Writer For Meta Samples";
        }
    }
}
