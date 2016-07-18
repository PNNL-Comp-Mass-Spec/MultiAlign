using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Distance;
using MultiAlignCore.Data.Features;
using MultiAlignTestSuite.Algorithms.FeatureFinding;
using NUnit.Framework;

namespace MultiAlignTestSuite.Algorithms.FeatureClustering
{

    [TestFixture]
    public class PrimsTest
    {

        private List<UMCLight> GetClusterData(string path)
        {
            var data = File.ReadLines(path).ToList();
            // Remove the header
            data.RemoveAt(0);

            var features = new List<UMCLight>();
            foreach(var line in data)
            {
                var lineData = line.Split(new[] {"\t"}, StringSplitOptions.RemoveEmptyEntries).ToList();

                var feature                 = new UMCLight();
                feature.ClusterId                = Convert.ToInt32(lineData[0]);
                feature.GroupId                  = Convert.ToInt32(lineData[1]);
                feature.Id                       = Convert.ToInt32(lineData[2]);
                feature.MassMonoisotopicAligned  = Convert.ToDouble(lineData[3]);
                feature.Net            = Convert.ToDouble(lineData[4]);
                feature.DriftTime                = Convert.ToDouble(lineData[5]);
                feature.ChargeState              = Convert.ToInt32(lineData[6]);

                features.Add(feature);
            }
            return features;
        }

        [Test(Description = "Tests clusters that should have been split.")]
        //[TestCase(@"ClusterData\clusterData-merged.txt")]
        //[TestCase(@"ClusterData\clusterData-mergedSmall.txt")]
        //[TestCase(@"ClusterData\clusterData-toy.txt")]
        //[TestCase(@"ClusterData\clusterData-ideal.txt")]
        [TestCase(@"ClusterData\clusterData-ideal.txt")]
        //[TestCase(@"ClusterData\clusterData-merged-nodelin.txt")]
        public void TestPrims(string path)
        {
            Console.WriteLine("Test: " + path);
            var features = GetClusterData(Path.Combine(TestPathSingleton.TestDirectory, path));

            Assert.IsNotEmpty(features);

            var cluster = new UMCClusterLight();
            cluster.Id = features[0].Id;
            features.ForEach(x => cluster.AddChildFeature(x));

            var maps = new Dictionary<int, UMCClusterLight>();

            var prims = new UMCPrimsClustering<UMCLight, UMCClusterLight>();
            prims.Parameters                        = new FeatureClusterParameters<UMCLight>();
            prims.Parameters.CentroidRepresentation = ClusterCentroidRepresentation.Mean;
            prims.Parameters.Tolerances             = new FeatureTolerances();

            var clusters = prims.Cluster(features);

            var counts = new Dictionary<int, Dictionary<int, int>>();
            var cid = 0;
            foreach (var clusterx in clusters)
            {
                clusterx.Id = cid++;
                foreach (var feature in clusterx.Features)
                {
                    if (!counts.ContainsKey(feature.GroupId))
                    {
                        counts.Add(feature.GroupId, new Dictionary<int, int>());
                    }
                    if (!counts[feature.GroupId].ContainsKey(feature.Id))
                    {
                        counts[feature.GroupId].Add(feature.Id, 0);
                    }

                    if (feature.Id == 51 || feature.Id == 37)
                    {
                        Console.WriteLine("Found it {0} cluster {1}", feature.Id, clusterx.Id);
                    }

                    counts[feature.GroupId][feature.Id]++;
                    Console.WriteLine("Found {0}", clusterx.Id);
                    if (counts[feature.GroupId][feature.Id] > 1)
                    {
                        Console.WriteLine("Duplicate!!!! cluster {0}  feature {1}", clusterx.Id, feature.Id);
                    }
                }
            }

            Console.WriteLine("Group\tFeature\tCount");
            foreach (var group in counts.Keys)
            {
                foreach (var id in counts[group].Keys)
                {
                    Console.WriteLine("{0}\t{1}\t{2}", group, id, counts[group][id]);
                }
            }

            Console.WriteLine("Clusters = {0}", clusters.Count);
        }
        [Test(Description = "Tests clusters that should have been split.")]
        [TestCase(@"ClusterData\clusterData-multiple-driftTime.txt", 4)]
        [TestCase(@"ClusterData\clusterData-ideal.txt", 4)]
        [TestCase(@"ClusterData\clusterData-merged.txt", 4)]
        [TestCase(@"ClusterData\clusterData-merged-nodelin.txt", 4)]
        [TestCase(@"ClusterData\clusterData-smallMerged.txt", 4)]
        [TestCase(@"ClusterData\clusterData-merged-small.txt", 4)]
        [TestCase(@"ClusterData\clusterData-merged-small.txt", 4)]
        //[TestCase(@"ClusterData\clusterData-single-1500.txt", 4)]
        //[TestCase(@"ClusterData\clusterData-single-1500-two.txt", 4)]
        public void TestPrimsWeighted(string path, double sigma)
        {

            sigma = 1;

            Console.WriteLine();
            Console.WriteLine("Tests: " + path);
            Console.WriteLine("Sigma Cutoff: {0}", sigma);
            var features = GetClusterData(Path.Combine(TestPathSingleton.TestDirectory, path));

            Assert.IsNotEmpty(features);

            var cluster = new UMCClusterLight();
            cluster.Id = features[0].Id;
            features.ForEach(x => cluster.AddChildFeature(x));

            var maps = new Dictionary<int, UMCClusterLight>();


            var prims = new UMCPrimsClustering<UMCLight, UMCClusterLight>(sigma);
            prims.Parameters                        = new FeatureClusterParameters<UMCLight>();
            prims.Parameters.CentroidRepresentation = ClusterCentroidRepresentation.Mean;
            prims.Parameters.Tolerances             = new FeatureTolerances();
            prims.Parameters.OnlyClusterSameChargeStates = false;
            prims.Parameters.Tolerances.DriftTime       = .3;
            prims.Parameters.Tolerances.Mass            = 15;
            prims.Parameters.Tolerances.Net   = .02;
            prims.DumpLinearRelationship                = false;

            var distance = new WeightedEuclideanDistance<UMCLight>();
            prims.Parameters.DistanceFunction   = distance.EuclideanDistance;
            var clusters      = prims.Cluster(features);


            Console.WriteLine();
            Console.WriteLine("Clusters = {0}", clusters.Count);

            var id = 1;
            foreach (var testCluster in clusters)
            {
                testCluster.CalculateStatistics(ClusterCentroidRepresentation.Mean);

                var distances = new List<double>();

                // Show a sampling of 15 results
                var threshold = (int)(testCluster.Features.Count / (double)15);
                if (threshold < 1)
                    threshold = 1;

                testCluster.Id = id++;
                var featureID = 0;

                foreach (var feature in testCluster.Features)
                {
                    featureID++;
                    if (featureID % threshold == 0)
                    {
                        Console.WriteLine("{0},{1},{2},{3}",
                                          feature.Net,
                                          feature.MassMonoisotopicAligned,
                                          feature.DriftTime,
                                          testCluster.Id);
                    }

                    var newDistance = distance.EuclideanDistance(feature, testCluster);
                    distances.Add(newDistance);
                }
                //Console.WriteLine();
                //Console.WriteLine("Distances");
                //distances.ForEach(x => Console.WriteLine(x));
                //Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Test Done:");
            Console.WriteLine();
        }
    }
}
