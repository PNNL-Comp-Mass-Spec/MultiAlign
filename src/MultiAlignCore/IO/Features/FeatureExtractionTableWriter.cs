using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MultiAlignCore.IO.Features;
using MultiAlignEngine.Features;
using MultiAlignCore.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Algorithms.MSLinker;

namespace MultiAlignCore.IO.Features
{
    public class FeatureExtractionTableWriter
    {
        public void WriteData(string path, FeaturesExtractedEventArgs data)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                string header = "MSn,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},";
                header += "{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}";

                foreach (int clusterIndex in data.MappedFeatures.Keys)
                {
                    bool hasData = false;
                    UMCClusterLight cluster = data.Clusters[clusterIndex];
                    foreach (FeatureExtractionMap fex in data.MappedFeatures[clusterIndex])
                    {
                        hasData       = true;
                        string values = string.Format(header,
                                                    clusterIndex,
                                                    cluster.MassMonoisotopic,
                                                    cluster.NET,
                                                    cluster.DatasetMemberCount,
                                                    cluster.MemberCount,

                                                    fex.DatasetID,
                            //6  ^

                                                    fex.LCMSFeatureID,
                                                    fex.LCMSMass,
                                                    fex.LCMSNet,
                                                    fex.LCMSScan,
                                                    fex.LCMSAbundance,

                                                    fex.MSFeatureId,
                                                    fex.MSMz,
                                                    fex.MSScan,
                                                    fex.MSIntensity,
                                                    fex.MSCharge,
                            // 15 ^

                                                    fex.MSnDatasetID,
                                                    fex.MSnFeatureId,
                                                    fex.MSnPrecursorMz,
                                                    fex.MSnScan,
                                                    fex.MSnRetentionTime);


                        if (data.MassTagMatches.ContainsKey(cluster.ID))
                        {

                            List<ClusterToMassTagMap> matches = data.MassTagMatches[cluster.ID];
                            foreach (ClusterToMassTagMap match in matches)
                            {
                                MassTagLight massTag    = data.MassTags[match.MassTagId];
                                string newData          = string.Format("matched,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",  
                                                                        values,
                                                                        match.MassTagId, 
                                                                        match.StacScore,
                                                                        match.StacUP,
                                                                        massTag.MassMonoisotopic,
                                                                        massTag.ModificationCount,
                                                                        massTag.NETAverage,
                                                                        massTag.PeptideSequence,
                                                                        massTag.PriorProbability,
                                                                        massTag.XCorr,
                                                                        massTag.MSGFSpecProbMax);
                                writer.WriteLine(newData);
                            }
                        }
                        else
                        {                            
                            writer.WriteLine("not-matched," + values);
                        }
                    }

                    if (!hasData)
                    {
                        string values = string.Format("No-MSn,{0},{1},{2},{3},{4}", 
                                                        cluster.ID, 
                                                        cluster.MassMonoisotopic, 
                                                        cluster.NET, 
                                                        cluster.DatasetMemberCount, 
                                                        cluster.MemberCount);

                        if (data.MassTagMatches.ContainsKey(cluster.ID))
                        {
                            List<ClusterToMassTagMap> matches = data.MassTagMatches[cluster.ID];
                            foreach (ClusterToMassTagMap match in matches)
                            {
                                MassTagLight massTag = data.MassTags[match.MassTagId];
                                string newData = string.Format("matched,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                                                        values,
                                                                        match.MassTagId,
                                                                        match.StacScore,
                                                                        match.StacUP,
                                                                        massTag.MassMonoisotopic,
                                                                        massTag.ModificationCount,
                                                                        massTag.NETAverage,
                                                                        massTag.PeptideSequence,
                                                                        massTag.PriorProbability,
                                                                        massTag.XCorr,
                                                                        massTag.MSGFSpecProbMax);
                                writer.WriteLine(newData);
                            }
                        }
                        else
                        {
                            writer.WriteLine("not-matched," +  values);
                        }                        
                    }
                }
            }    
        }
    }
}
