#region

using System;
using System.Collections.Generic;
using System.IO;
using MultiAlignCore.Data;
using MultiAlignCore.Data.SequenceData;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignCore.IO.MsMs
{
    public class MSMSClusterWriter
    {
        private void ExportMSMSClusters(string path, double similarity, List<MsmsCluster> clusters,
            List<MSMSClusterMap> maps, int id, List<double> xValues, List<double> yValues, List<int> groupIDS)
        {
            using (
                TextWriter xwriter = File.CreateText(Path.Combine(path, string.Format("clusters-{0}.csv", similarity))))
            {
                foreach (var cluster in clusters)
                {
                    cluster.Id = id++;
                    var line = string.Format("{0},", cluster.Id);

                    // This is temporary.  So that we only export clusters with two features.
                    if (cluster.Features.Count > 2)
                        continue;

                    // Organize the spectra so they are sorted by dataset.
                    cluster.Features.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                    {
                        // Sort by scan if they are the same feature.
                        if (x.GroupId == y.GroupId)
                        {
                            return x.Scan.CompareTo(y.Scan);
                        }
                        // Otherwise we want to sort by what dataset they came from.
                        return x.GroupId.CompareTo(y.GroupId);
                    });

                    groupIDS[0] = cluster.Features[0].GroupId;
                    groupIDS[1] = cluster.Features[1].GroupId;

                    xValues.Add(Convert.ToDouble(cluster.Features[0].Scan));
                    yValues.Add(Convert.ToDouble(cluster.Features[1].Scan));

                    foreach (var feature in cluster.Features)
                    {
                        var newMap = new MSMSClusterMap();
                        newMap.ClusterID = cluster.Id;
                        newMap.MSMSID = feature.MSnSpectra[0].Id;
                        newMap.GroupID = feature.GroupId;
                        maps.Add(newMap);
                        line += string.Format("{2},{0},{1},", feature.Scan, feature.Mz, feature.GroupId);
                    }
                    xwriter.WriteLine(line);
                }
            }
        }

        private void ExportMatches(string path, double similarity, List<double> tX, List<double> tY, double[] coeffs)
        {
            using (
                TextWriter xwriter = File.CreateText(Path.Combine(path, string.Format("matches-{0}.csv", similarity))))
            {
                var x = new double[1];
                double value = 0;
                for (var i = 0; i < tX.Count; i++)
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

        private void ExportMatches(
            Dictionary<int, Dictionary<int, Dictionary<int, DatabaseSearchSequence>>> sequenceMap, string path,
            double similarity, List<MsmsCluster> clusters, int id)
        {
            using (
                TextWriter xwriter =
                    File.CreateText(Path.Combine(path, string.Format("match-sequences-{0}.csv", similarity))))
            {
                xwriter.WriteLine(
                    "cluster id, cluster mean score, dataset id, scan, mz, charge, mass most abundant, find scan, pep seq, score, mass, charge, tryptic ends");

                foreach (var cluster in clusters)
                {
                    cluster.Id = id++;
                    var line = string.Format("{0},{1},", cluster.Id, cluster.MeanScore);

                    // This is temporary.  So that we only export clusters with two features.
                    if (cluster.Features.Count > 2)
                        continue;

                    // Organize the spectra so they are sorted by dataset.
                    cluster.Features.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                    {
                        // Sort by scan if they are the same feature.
                        if (x.GroupId == y.GroupId)
                        {
                            return x.Scan.CompareTo(y.Scan);
                        }
                        // Otherwise we want to sort by what dataset they came from.
                        return x.GroupId.CompareTo(y.GroupId);
                    });

                    foreach (var feature in cluster.Features)
                    {
                        var findScan = feature.MSnSpectra[0].Scan;

                        DatabaseSearchSequence sequence = null;
                        if (!sequenceMap[feature.GroupId].ContainsKey(findScan))
                        {
                            line += string.Format("{0},{1},,,,,,,,,", feature.GroupId, feature.Scan);
                            continue;
                        }

                        try
                        {
                            sequence = sequenceMap[feature.GroupId][findScan][feature.ChargeState];
                        }
                        catch
                        {
                        }
                        var pepsequence = "";
                        double score = 0;

                        if (sequence != null)
                        {
                            pepsequence = sequence.Sequence;
                            score = sequence.Score;
                        }

                        line += string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                            feature.GroupId,
                            feature.Scan,
                            feature.Mz,
                            feature.ChargeState,
                            feature.MassMonoisotopicMostAbundant,
                            findScan,
                            pepsequence,
                            score);
                    }
                    xwriter.WriteLine(line);
                }
            }
        }
    }
}