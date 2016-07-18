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
    public class UMCAverageLinkageTest
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
        [TestCase(@"ClusterData\clusterData-single-smallSpread.txt")]
        [TestCase(@"ClusterData\clusterData-ideal.txt")]
        [TestCase(@"ClusterData\clusterData-merged.txt")]
        [TestCase(@"ClusterData\clusterData-merged-nodelin.txt")]
        [TestCase(@"ClusterData\clusterData-smallMerged.txt")]
        [TestCase(@"ClusterData\clusterData-merged-small.txt")]
        //[TestCase(@"ClusterData\clusterData-single-1500.txt")]
        public void TestAverageLinkage(string path)
        {
            Console.WriteLine("Average Linkage Test: " + path);
            var features = GetClusterData(Path.Combine(TestPathSingleton.TestDirectory, path));

            Assert.IsNotEmpty(features);

            var cluster = new UMCClusterLight();
            cluster.Id = features[0].Id;
            features.ForEach(x => cluster.AddChildFeature(x));

            var maps = new Dictionary<int, UMCClusterLight>();

            var average = new UMCAverageLinkageClusterer<UMCLight, UMCClusterLight>();
            average.Parameters = new FeatureClusterParameters<UMCLight>();
            average.Parameters.CentroidRepresentation = ClusterCentroidRepresentation.Median;
            average.Parameters.Tolerances = new FeatureTolerances();
            average.Parameters.Tolerances.Net = .02;
            average.Parameters.Tolerances.Mass          = 6;
            average.Parameters.Tolerances.DriftTime     = .3;

            var distance = new WeightedEuclideanDistance<UMCLight>();
            average.Parameters.DistanceFunction = distance.EuclideanDistance;
            var euclid = new EuclideanDistanceMetric<UMCLight>();
            average.Parameters.DistanceFunction = euclid.EuclideanDistance;
            var clusters      = average.Cluster(features);

            Console.WriteLine("Clusters = {0}", clusters.Count);
            var id = 1;
            foreach (var testCluster in clusters)
            {
                testCluster.CalculateStatistics(ClusterCentroidRepresentation.Mean);
                var distances = new List<double>();

                // Show a sampling of 5 results
                var threshold = (int)(testCluster.Features.Count / (double)5);
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
        }

        private double WeightedDistance(UMCLight x, UMCLight y)
        {
            double test = 0;

            return test;
        }

        //[Test(Description = "Tests clusters that should have been split.")]
        //[TestCase(@"ClusterData\clusterData-single-smallSpread.txt", .09)]
        //[TestCase(@"ClusterData\clusterData-ideal.txt", .09)]
        //[TestCase(@"ClusterData\clusterData-merged.txt", .09)]
        //[TestCase(@"ClusterData\clusterData-merged.txt", .01)]
        [TestCase(@"ClusterData\clusterData-merged.txt")]
        //[TestCase(@"ClusterData\clusterData-merged-nodelin.txt")]
        public void TestWeightedAverageLinkage(string path)
        {
            Console.WriteLine("Test: " + path);
            var features = GetClusterData(Path.Combine(TestPathSingleton.TestDirectory, path));

            Assert.IsNotEmpty(features);

            var cluster = new UMCClusterLight();
            cluster.Id = features[0].Id;
            features.ForEach(x => cluster.AddChildFeature(x));

            var maps = new Dictionary<int, UMCClusterLight>();

            var average = new UMCAverageLinkageClusterer<UMCLight, UMCClusterLight>();
            average.Parameters                          = new FeatureClusterParameters<UMCLight>();
            average.Parameters.CentroidRepresentation   = ClusterCentroidRepresentation.Mean;
            average.Parameters.Tolerances               = new FeatureTolerances();

            var distance = new WeightedEuclideanDistance<UMCLight>();
            average.Parameters.DistanceFunction = distance.EuclideanDistance;
            var clusters      = average.Cluster(features);

            Console.WriteLine("dataset\tfeature\tmass\tnet\tdrift");
            foreach (var newCluster in clusters)
            {
                foreach (var feature in newCluster.Features)
                {
                    Console.WriteLine("{0},{1},{2},{3},{4}",feature.GroupId,
                                                            feature.Id,
                                                            feature.Net,
                                                            feature.MassMonoisotopicAligned,
                                                            feature.DriftTime);

                }
            }
        }
    }
}
