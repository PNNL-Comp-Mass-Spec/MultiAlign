using System;
using System.Collections.Generic;
using System.IO;
using MultiAlignCore.Data;
using MultiAlignCore.Data.SequenceData;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.IO.Features.Exporters
{
    public class MSMSClusterWriter
    {

        private void ExportMSMSClusters(string path, double similarity, List<MSMSCluster> clusters, List<MSMSClusterMap> maps, int id, List<double> xValues, List<double> yValues, List<int> groupIDS)
        {
            using (TextWriter xwriter = File.CreateText(System.IO.Path.Combine(path, string.Format("clusters-{0}.csv", similarity))))
            {
                foreach (MSMSCluster cluster in clusters)
                {
                    cluster.ID = id++;
                    string line = string.Format("{0},", cluster.ID);

                    // This is temporary.  So that we only export clusters with two features.
                    if (cluster.Features.Count > 2) continue;

                    // Organize the spectra so they are sorted by dataset.
                    cluster.Features.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                    {
                        // Sort by scan if they are the same feature.
                        if (x.GroupID == y.GroupID)
                        {
                            return x.Scan.CompareTo(y.Scan);
                        }
                        // Otherwise we want to sort by what dataset they came from.
                        return x.GroupID.CompareTo(y.GroupID);
                    });

                    groupIDS[0] = cluster.Features[0].GroupID;
                    groupIDS[1] = cluster.Features[1].GroupID;

                    xValues.Add(Convert.ToDouble(cluster.Features[0].Scan));
                    yValues.Add(Convert.ToDouble(cluster.Features[1].Scan));

                    foreach (MSFeatureLight feature in cluster.Features)
                    {
                        MSMSClusterMap newMap = new MSMSClusterMap();
                        newMap.ClusterID = cluster.ID;
                        newMap.MSMSID = feature.MSnSpectra[0].ID;
                        newMap.GroupID = feature.GroupID;
                        maps.Add(newMap);
                        line += string.Format("{2},{0},{1},", feature.Scan, feature.Mz, feature.GroupID);
                    }
                    xwriter.WriteLine(line);
                }
            }
        }
        private void ExportMatches(string path, double similarity, List<double> tX, List<double> tY, double[] coeffs)
        {
            using (TextWriter xwriter = File.CreateText(System.IO.Path.Combine(path, string.Format("matches-{0}.csv", similarity))))
            {
                double[] x = new double[1];
                double value = 0;
                for (int i = 0; i < tX.Count; i++)
                {
                    x[0] = tX[i];
                    //ChebyShev(coeffs, x, ref value, null);


                    xwriter.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", tX[i],
                                                                                    tY[i],
                                                                                    value,
                                                                                    tY[i],
                                                                                    tX[i] - tY[i],
                                                                                    tY[i] - value,
                                                                                    (tX[i] - tY[i]) - (tY[i] - value)
                                                                                    );


                }
            }
        }
        private void ExportMatches(Dictionary<int, Dictionary<int, Dictionary<int, DatabaseSearchSequence>>> sequenceMap, string path, double similarity, List<MSMSCluster> clusters, int id)
        {
            using (TextWriter xwriter = File.CreateText(System.IO.Path.Combine(path, string.Format("match-sequences-{0}.csv", similarity))))
            {
                xwriter.WriteLine("cluster id, cluster mean score, dataset id, scan, mz, charge, mass most abundant, find scan, pep seq, score, mass, charge, tryptic ends");

                foreach (MSMSCluster cluster in clusters)
                {
                    cluster.ID = id++;
                    string line = string.Format("{0},{1},", cluster.ID, cluster.MeanScore);

                    // This is temporary.  So that we only export clusters with two features.
                    if (cluster.Features.Count > 2) continue;

                    // Organize the spectra so they are sorted by dataset.
                    cluster.Features.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                    {
                        // Sort by scan if they are the same feature.
                        if (x.GroupID == y.GroupID)
                        {
                            return x.Scan.CompareTo(y.Scan);
                        }
                        // Otherwise we want to sort by what dataset they came from.
                        return x.GroupID.CompareTo(y.GroupID);
                    });

                    foreach (MSFeatureLight feature in cluster.Features)
                    {
                        int findScan = feature.MSnSpectra[0].Scan;

                        DatabaseSearchSequence sequence = null;
                        if (!sequenceMap[feature.GroupID].ContainsKey(findScan))
                        {
                            line += string.Format("{0},{1},,,,,,,,,", feature.GroupID, feature.Scan);
                            continue;
                        }

                        try
                        {
                            sequence = sequenceMap[feature.GroupID][findScan][feature.ChargeState];
                        }
                        catch
                        {
                        }
                        string pepsequence = "";
                        double score = 0;
                        double mass = 0;
                        int charge = 0;
                        int ends = 0;

                        if (sequence != null)
                        {
                            pepsequence = sequence.Sequence;
                            score = sequence.Score;
                            mass = sequence.Mass;
                            charge = sequence.Charge;
                            ends = sequence.TrypticEnds;
                        }

                        line += string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},",
                                                    feature.GroupID,
                                                    feature.Scan,
                                                    feature.Mz,
                                                    feature.ChargeState,
                                                    feature.MassMonoisotopicMostAbundant,
                                                    findScan,
                                                    pepsequence,
                                                    score,
                                                    mass,
                                                    charge,
                                                    ends);
                    }
                    xwriter.WriteLine(line);
                }
            }
        }


    }
}
