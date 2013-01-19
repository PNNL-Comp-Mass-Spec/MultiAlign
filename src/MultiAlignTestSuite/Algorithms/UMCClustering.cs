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

namespace MultiAlignTestSuite.Algorithms
{
    public class UMCClustering
    {
        public List<UMCClusterLight> ReadClusters(string path)
        {
            List<UMCClusterLight> clusters  = new List<UMCClusterLight>();
            List<string> data               = File.ReadLines(path).ToList();

            bool isClusters = true;

            int i = 1;

            Dictionary<int, UMCClusterLight> clusterMap = new Dictionary<int, UMCClusterLight>();

            while (i < data.Count && isClusters)
            {
                isClusters = !(data[i].ToLower().Contains("dataset"));
                if (isClusters)
                {
                    UMCClusterLight cluster = new UMCClusterLight();
                    string [] lineData       = data[i].Split(',');
                    cluster.ID               = Convert.ToInt32(lineData[0]);
                    cluster.MassMonoisotopic = Convert.ToDouble(lineData[1]);
                    cluster.RetentionTime    = Convert.ToDouble(lineData[2]);
                    cluster.NET              = Convert.ToDouble(lineData[2]);
                    cluster.DriftTime        = Convert.ToDouble(lineData[3]);
                    cluster.ChargeState      = Convert.ToInt32(lineData[4]);

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
                
                string line         = data[i];
                string[] lineData   = line.Split(',');
                if (line.Length > 6)
                {
                    int clusterID            = Convert.ToInt32(lineData[0]);
                    UMCLight feature         = new UMCLight();
                    feature.GroupID          = Convert.ToInt32(lineData[1]);                    
                    feature.ID               = Convert.ToInt32(lineData[2]);
                    feature.MassMonoisotopic = Convert.ToDouble(lineData[3]);
                    feature.RetentionTime    = Convert.ToDouble(lineData[4]);
                    feature.NETAligned       = Convert.ToDouble(lineData[4]);
                    feature.DriftTime        = Convert.ToDouble(lineData[5]);
                    feature.ChargeState      = Convert.ToInt32(lineData[6]);

                    if (clusterMap.ContainsKey(clusterID))
                    {
                        clusterMap[clusterID].AddChildFeature(feature);
                    }
                }
                i = i + 1;
            }
            
            return clusters;
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
