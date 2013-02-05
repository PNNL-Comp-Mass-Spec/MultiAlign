using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using System.IO;
using NUnit;
using NUnit.Framework;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Distance;
using System.Diagnostics;
using MultiAlignCore.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;

namespace MultiAlignTestSuite.Algorithms
{
    public class UMCClustering
    {
        public List<UMCClusterLight> ReadClusters(string path)
        {
            List<UMCClusterLight> clusters = new List<UMCClusterLight>();
            List<string> data = File.ReadLines(path).ToList();

            bool isClusters = true;

            int i = 1;

            Dictionary<int, UMCClusterLight> clusterMap = new Dictionary<int, UMCClusterLight>();

            while (i < data.Count && isClusters)
            {
                isClusters = !(data[i].ToLower().Contains("dataset"));
                if (isClusters)
                {
                    UMCClusterLight cluster = new UMCClusterLight();
                    string[] lineData = data[i].Split(',');
                    cluster.ID = Convert.ToInt32(lineData[0]);
                    cluster.MassMonoisotopic = Convert.ToDouble(lineData[1]);
                    cluster.RetentionTime = Convert.ToDouble(lineData[2]);
                    cluster.NET = Convert.ToDouble(lineData[2]);
                    cluster.DriftTime = Convert.ToDouble(lineData[3]);
                    cluster.ChargeState = Convert.ToInt32(lineData[4]);

                    if (!clusterMap.ContainsKey(cluster.ID))
                    {
                        clusterMap.Add(cluster.ID, cluster);
                    }

                    clusters.Add(cluster);
                }
                i = i + 1;
            }

            i = i + 1;
            while (i < data.Count)
            {

                string line = data[i];
                string[] lineData = line.Split(',');
                if (line.Length > 6)
                {
                    int clusterID = Convert.ToInt32(lineData[0]);
                    UMCLight feature = new UMCLight();
                    feature.GroupID = Convert.ToInt32(lineData[1]);
                    feature.ID = Convert.ToInt32(lineData[2]);
                    feature.MassMonoisotopic = Convert.ToDouble(lineData[3]);
                    feature.RetentionTime = Convert.ToDouble(lineData[4]);
                    feature.NETAligned = Convert.ToDouble(lineData[4]);
                    feature.DriftTime = Convert.ToDouble(lineData[5]);
                    feature.ChargeState = Convert.ToInt32(lineData[6]);

                    if (clusterMap.ContainsKey(clusterID))
                    {
                        clusterMap[clusterID].AddChildFeature(feature);
                    }
                }
                i = i + 1;
            }

            return clusters;
        }
        public List<UMCLight> ReadFeatures(string path)
        {
            List<UMCLight> features = new List<UMCLight>();
            List<string> data       = File.ReadLines(path).ToList();
            bool isClusters         = true;
            int i                   = 1;

            while (i < data.Count && isClusters)
            {
                isClusters = !(data[i].ToLower().Contains("dataset"));                
                i = i + 1;
            }
            i = i + 1;
            while (i < data.Count)
            {
                string line = data[i];
                string[] lineData = line.Split(',');
                if (line.Length > 6)
                {
                    int clusterID = Convert.ToInt32(lineData[0]);
                    UMCLight feature = new UMCLight();
                    feature.GroupID = Convert.ToInt32(lineData[1]);
                    feature.ID = Convert.ToInt32(lineData[2]);
                    feature.MassMonoisotopicAligned    = Convert.ToDouble(lineData[3]);
                    feature.RetentionTime       = Convert.ToDouble(lineData[4]);
                    feature.MassMonoisotopic = feature.MassMonoisotopicAligned;
                    feature.NETAligned  = Convert.ToDouble(lineData[4]);
                    feature.NET         = feature.NETAligned;
                    feature.DriftTime   = Convert.ToDouble(lineData[5]);
                    feature.ChargeState = Convert.ToInt32(lineData[6]);
                    features.Add(feature);                    
                }
                i = i + 1;
            }
            return features;
        }

        public List<UMCClusterLight> CreateSingletonClustersFromClusteredFeatures(List<UMCClusterLight> clusters)
        {
            List<UMCClusterLight> newClusters = new List<UMCClusterLight>();
           
            int i = 0;
            foreach (UMCClusterLight cluster in clusters)
            {
                foreach (UMCLight feature in cluster.Features)
                {
                    UMCClusterLight x   = new UMCClusterLight();                    
                    x.MassMonoisotopic  = feature.MassMonoisotopic;
                    x.RetentionTime     = feature.RetentionTime;
                    x.NET               = feature.NETAligned;                                                                     
                    x.DriftTime         = feature.DriftTime;
                    x.ChargeState       = feature.ChargeState;
                    x.ID                = i++;
                    x.AddChildFeature(feature);                    
                    newClusters.Add(x);
                }
            }
            return newClusters;
        }
        private void WriteClusters(List<UMCClusterLight> clusters)
        {
            Console.WriteLine("[Clusters]");
            Console.WriteLine("NET, Mass, DT");

            for (int i = 0; i < clusters.Count; i++)
            {
                                
                    Console.WriteLine("{0},{1},{2},{3}",
                                                        clusters[i].ID,
                                                        clusters[i].RetentionTime,
                                                        clusters[i].MassMonoisotopic,
                                                        clusters[i].DriftTime);
                
            }
            Console.WriteLine("[Features]");
            Console.WriteLine("Cluster ID, Dataset ID, Feature ID, charge, NET, Mass, DT");
            foreach (UMCClusterLight clusterLight in clusters)
            {
                foreach (UMCLight feature in clusterLight.Features)
                {
                    Console.WriteLine("{0},{1},{2},{3},{4},{5},{6}",
                                        clusterLight.ID,
                                        feature.GroupID,
                                        feature.ID,
                                        feature.ChargeState,
                                        feature.RetentionTime,
                                        feature.MassMonoisotopic,
                                        feature.DriftTime);
                }
            }
        }

        [Test(Description = "Tests how clusters form when the are within tolerance boolean is not set")]
        [TestCase(@"M:\data\proteomics\Clusters\twoClusters-split.csv", DistanceMetric.PowerEuclidean, false)]
        [TestCase(@"M:\data\proteomics\Clusters\twoClusters-split.csv", DistanceMetric.PowerEuclidean, true)]
        [TestCase(@"M:\data\proteomics\Clusters\twoClusters-split.csv", DistanceMetric.Euclidean, false)]
        [TestCase(@"M:\data\proteomics\Clusters\twoClusters-split.csv", DistanceMetric.Euclidean, true)]
        public void TestRestrictiveBoxMethod(string path, DistanceMetric dist, bool useBoxMethod)
        {
            List<UMCLight> features = ReadFeatures(path);
            UMCAverageLinkageClusterer<UMCLight, UMCClusterLight> clusterer = new UMCAverageLinkageClusterer<UMCLight, UMCClusterLight>();

            clusterer.ShouldTestClustersWithinTolerance = useBoxMethod;
            clusterer.Parameters.CentroidRepresentation= ClusterCentroidRepresentation.Mean;
            clusterer.Parameters.DistanceFunction = DistanceFactory<UMCLight>.CreateDistanceFunction(dist);
            clusterer.Parameters.OnlyClusterSameChargeStates = true;
            clusterer.Parameters.Tolerances.Mass = 10;
            clusterer.Parameters.Tolerances.DriftTime = .3;
            clusterer.Parameters.Tolerances.RetentionTime = .03;
            List<UMCClusterLight> clusters = clusterer.Cluster(features);
            int i = 0;
            clusters.ForEach(x => x.ID = i++);
            WriteClusters(clusters);
        }

        [Test(Description = "Tests how clusters form when the are within tolerance boolean is not set")]
        [TestCase(@"M:\data\proteomics\Clusters\twoClusters-split.csv", DistanceMetric.PowerEuclidean)]
        [TestCase(@"M:\data\proteomics\Clusters\twoClusters-split.csv", DistanceMetric.Euclidean)]
        public void TestDistanceDistributions(string path, DistanceMetric dist)
        {
            List<UMCLight> features = ReadFeatures(path);
            UMCAverageLinkageClusterer<UMCLight, UMCClusterLight> clusterer = new UMCAverageLinkageClusterer<UMCLight, UMCClusterLight>();

            clusterer.ShouldTestClustersWithinTolerance = false;
            clusterer.Parameters.CentroidRepresentation = ClusterCentroidRepresentation.Mean;
            clusterer.Parameters.DistanceFunction = DistanceFactory<UMCLight>.CreateDistanceFunction(dist);
            clusterer.Parameters.OnlyClusterSameChargeStates = true;
            clusterer.Parameters.Tolerances.Mass = 10;
            clusterer.Parameters.Tolerances.DriftTime = .3;
            clusterer.Parameters.Tolerances.RetentionTime = .03;
            List<UMCClusterLight> clusters = clusterer.Cluster(features);

            List<double> distances = new List<double>();
            foreach (UMCClusterLight cluster in clusters)
            {
                UMCLight centroid = new UMCLight();
                centroid.MassMonoisotopicAligned = cluster.MassMonoisotopic;
                centroid.RetentionTime           = cluster.RetentionTime;
                centroid.DriftTime               = cluster.DriftTime;

                DistanceFunction<UMCLight> func = clusterer.Parameters.DistanceFunction;
                foreach (UMCLight feature in cluster.Features)
                {
                    double distance = func(feature, centroid);
                    distances.Add(distance);                    
                }
                distances.Sort();
                int sum = 0;
                foreach (double distance in distances)
                {
                    sum++;
                    Console.WriteLine("{0},{1}", distance, sum);
                }
            }
        }

        [Test]
        [TestCase(@"M:\data\proteomics\Clusters\clusterBaseline-01.csv", DistanceMetric.PowerEuclidean)]
        public void TestDistancesEuclidean(string path, DistanceMetric dist)
        {            
            DistanceFunction<UMCClusterLight> func          = DistanceFactory<UMCClusterLight>.CreateDistanceFunction(DistanceMetric.Euclidean);
            List<UMCClusterLight> oldClusters               = ReadClusters(path);
            List<UMCClusterLight> clusters                  = CreateSingletonClustersFromClusteredFeatures(new List<UMCClusterLight>() {oldClusters[1]});

            Console.WriteLine("Distance, Mass, NET, DT, Mass, Net, DT");

            for (int i = 0; i < clusters.Count; i++)
            {
                for (int j = i + 1; j < clusters.Count; j++)
                {
                    double distance = func(clusters[i], clusters[j]);
                    Console.WriteLine("{0},{1},{2},{3},{4},{5},{6}",
                                                                    distance,
                                                                    clusters[i].MassMonoisotopic,
                                                                    clusters[i].NET,
                                                                    clusters[i].DriftTime,
                                                                    clusters[j].MassMonoisotopic,
                                                                    clusters[j].NET,
                                                                    clusters[j].DriftTime);
                }
            }
        }
        
        [Test]
        public void TestDistanceChangeEuclidean()
        {
            UMCClusterLight cluster  = new UMCClusterLight();
            cluster.MassMonoisotopic = 500;
            cluster.NET              = .5;
            cluster.RetentionTime    = .5;
            cluster.DriftTime        = 20;

            
            EuclideanDistanceMetric<UMCClusterLight> euclid = new EuclideanDistanceMetric<UMCClusterLight>();
            DistanceFunction<UMCClusterLight> func          = euclid.EuclideanDistance;

            double deltaNet         = .01;
            double deltaMassPPM     = 1;
            double deltaDriftTime   = 1;

            Console.WriteLine("Mass Diff, Mass Dist, Net, Net Dist, Drift, Drift Dist");

            for (int i = 0; i < 50; i++)
            {
                UMCClusterLight clusterD = new UMCClusterLight();
                UMCClusterLight clusterN = new UMCClusterLight();
                UMCClusterLight clusterM = new UMCClusterLight();

                clusterM.DriftTime          = cluster.DriftTime     + deltaDriftTime;
                clusterM.NET                = cluster.NET           + deltaNet;
                clusterM.RetentionTime      = cluster.RetentionTime + deltaNet;
                clusterM.MassMonoisotopic   = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, deltaMassPPM * i);


                clusterN.DriftTime          = cluster.DriftTime     + deltaDriftTime;
                clusterN.NET                = cluster.NET           + (deltaNet * i);
                clusterN.RetentionTime      = cluster.RetentionTime + (deltaNet * i);
                clusterN.MassMonoisotopic   = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, deltaMassPPM);


                clusterD.DriftTime          = cluster.DriftTime     + (deltaDriftTime * i);
                clusterD.NET                = cluster.NET           + deltaNet;
                clusterD.RetentionTime      = cluster.RetentionTime + deltaNet;
                clusterD.MassMonoisotopic   = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, deltaMassPPM);

                double distM = func(cluster, clusterM);
                double distN = func(cluster, clusterN);
                double distD = func(cluster, clusterD);

                string output = string.Format("{0},{1},{2},{3},{4},{5}", deltaMassPPM*i, distM, deltaNet*i, distN, deltaDriftTime*i, distD);
                Console.WriteLine(output);
                
            }
        }
        [Test]
        public void TestPowerDistanceChangeEuclidean()
        {
            UMCClusterLight cluster = new UMCClusterLight();
            cluster.MassMonoisotopic = 500;
            cluster.NET = .5;
            cluster.RetentionTime = .5;
            cluster.DriftTime = 20;


            PowerEuclideanDistanceMetric<UMCClusterLight> euclid = new PowerEuclideanDistanceMetric<UMCClusterLight>();
            DistanceFunction<UMCClusterLight> func = euclid.EuclideanDistance;

            double deltaNet = .01;
            double deltaMassPPM = 1;
            double deltaDriftTime = 1;

            Console.WriteLine("xxMass Diff, Mass Dist, Net, Net Dist, Drift, Drift Dist");

            for (int i = 0; i < 50; i++)
            {
                UMCClusterLight clusterD = new UMCClusterLight();
                UMCClusterLight clusterN = new UMCClusterLight();
                UMCClusterLight clusterM = new UMCClusterLight();

                clusterM.DriftTime = cluster.DriftTime + deltaDriftTime;
                clusterM.NET = cluster.NET + deltaNet;
                clusterM.RetentionTime = cluster.RetentionTime + deltaNet;
                clusterM.MassMonoisotopic = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, deltaMassPPM * i);


                clusterN.DriftTime = cluster.DriftTime + deltaDriftTime;
                clusterN.NET = cluster.NET + (deltaNet * i);
                clusterN.RetentionTime = cluster.RetentionTime + (deltaNet * i);
                clusterN.MassMonoisotopic = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, deltaMassPPM);


                clusterD.DriftTime = cluster.DriftTime + (deltaDriftTime * i);
                clusterD.NET = cluster.NET + deltaNet;
                clusterD.RetentionTime = cluster.RetentionTime + deltaNet;
                clusterD.MassMonoisotopic = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, deltaMassPPM);

                double distM = func(cluster, clusterM);
                double distN = func(cluster, clusterN);
                double distD = func(cluster, clusterD);

                string output = string.Format("{0},{1},{2},{3},{4},{5}", deltaMassPPM * i, distM, deltaNet * i, distN, deltaDriftTime * i, distD);
                Console.WriteLine(output);

            }
        }
    }
}
