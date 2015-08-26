#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Datasets;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Hibernate;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.OHSU
{
    [TestFixture]
    public class ClusterExtractor
    {
        [Test]
        [TestCase(
            @"M:\data\proteomics\OHSU\Data\results.db3",
            @"M:\data\proteomics\OHSU\Data\Sarc-3000_charge-1",
            1,
            10, 
            Ignore=true)]
        public void CreateDatasetMap(string databasePath, string crossPath, int charge, int minimumClusterSize)
        {
            NHibernateUtil.ConnectToDatabase(databasePath, false);

            IDatasetDAO datasetCache = new DatasetDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();
            IUmcDAO featureCache = new UmcDAOHibernate();

            Console.WriteLine("Find all datasets");
            var datasets = datasetCache.FindAll();
            using (TextWriter writer = File.CreateText(crossPath + ".csv"))
            {
                writer.WriteLine("Dataset, Dataset Id");
                foreach (var info in datasets)
                {
                    writer.WriteLine("{0},{1}", info.DatasetName, info.DatasetId);
                }
            }
        }

        [Test]
        [TestCase(
            @"M:\data\proteomics\OHSU\Data\results.db3",
            @"M:\data\proteomics\OHSU\Data\Sarc-3000_charge-1",
            1,
            10,
            Ignore = true)]
        public void TestClusterWriting(string databasePath, string crossPath, int charge, int minimumClusterSize)
        {
            NHibernateUtil.ConnectToDatabase(databasePath, false);

            IDatasetDAO datasetCache = new DatasetDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();
            IUmcDAO featureCache = new UmcDAOHibernate();

            Console.WriteLine("Find all datasets");
            var datasets = datasetCache.FindAll();

            Console.WriteLine("Find all clusters");
            var clusters = clusterCache.FindByCharge(charge);

            WriteClusters(datasets, clusters, minimumClusterSize, charge, crossPath, databasePath, 300000);
        }

        [Test]
        [TestCase(
            @"M:\data\proteomics\OHSU\Data\dummy_database-charge.db3", 20, 400,
            Ignore = true)]
        public void TestCreateDummyDatabase(string databasePath, int totalDatasets, int totalClusters)
        {
            File.Delete(databasePath);
            NHibernateUtil.ConnectToDatabase(databasePath, true);

            IDatasetDAO datasetCache = new DatasetDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();
            IUmcDAO featureCache = new UmcDAOHibernate();

            // Creating a dataset
            Console.WriteLine("Creating dummy datasets");
            var datasets = new List<DatasetInformation>();
            var total = totalDatasets;
            for (var i = 0; i < total; i++)
            {
                var dataset = new DatasetInformation();
                dataset.DatasetId = i;
                dataset.DatasetName = "test" + i;
                datasets.Add(dataset);
            }
            datasetCache.AddAll(datasets);
            datasets.Clear();
            datasets = datasetCache.FindAll();

            // Create features
            Console.WriteLine("Creating features");
            var features = new List<UMCLight>();
            var clusters = new List<UMCClusterLight>();
            var x = new Random();

            var featureId = 0;
            for (var i = 0; i < totalClusters; i++)
            {
                var N = x.Next(1, total);
                var charge = x.Next(1, 10);
                var hash = new HashSet<int>();

                var net = x.NextDouble();
                var mass = 400 + (1600*x.NextDouble());
                var dt = 60*x.NextDouble();

                for (var j = 0; j < N; j++)
                {
                    var did = -1;
                    do
                    {
                        did = x.Next(0, total);
                        if (!hash.Contains(did))
                        {
                            hash.Add(did);
                            break;
                        }
                    } while (true);


                    var feature = new UMCLight
                    {
                        GroupId = did,
                        Id = featureId++,
                        ChargeState = charge,
                        MassMonoisotopic = FeatureLight.ComputeDaDifferenceFromPPM(mass, 3)
                    };
                    feature.MassMonoisotopicAligned = feature.MassMonoisotopic;
                    feature.Net = net + 0.03 * x.NextDouble();
                    feature.NetAligned = feature.Net;
                    feature.Net = feature.Net;
                    feature.DriftTime = dt;
                    feature.AbundanceSum = x.Next(100, 200);
                    feature.Abundance = feature.Abundance;
                    feature.ClusterId = -1;
                    features.Add(feature);
                }
            }
            featureCache.AddAll(features);
        }

        [Test]
        [TestCase(
            @"M:\data\proteomics\OHSU\Data\dummy_database.db3",
            @"M:\data\proteomics\OHSU\Data\dummy_charge-1",
            1,
            1,
            Ignore = true)]
        public void TestClusterGeneration(string databasePath,
            string crossPath,
            int charge,
            int minimumClusterSize)
        {
            File.Delete(databasePath);
            NHibernateUtil.ConnectToDatabase(databasePath, true);

            IDatasetDAO datasetCache = new DatasetDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();
            IUmcDAO featureCache = new UmcDAOHibernate();

            // Creating a dataset
            Console.WriteLine("Creating dummy datasets");
            var datasets = new List<DatasetInformation>();
            var total = 10;
            for (var i = 0; i < total; i++)
            {
                var dataset = new DatasetInformation();
                dataset.DatasetId = i;
                dataset.DatasetName = "test" + i;
                datasets.Add(dataset);
            }
            datasetCache.AddAll(datasets);
            datasets.Clear();
            datasets = datasetCache.FindAll();

            // Create features
            Console.WriteLine("Creating features");
            var features = new List<UMCLight>();
            var clusters = new List<UMCClusterLight>();
            var x = new Random();
            var featureId = 0;
            for (var i = 0; i < 100; i++)
            {
                var cluster = new UMCClusterLight();
                cluster.Id = i;
                cluster.AmbiguityScore = i;
                cluster.Tightness = i;

                var N = x.Next(1, total);
                cluster.Id = i;
                cluster.ChargeState = charge;
                var hash = new HashSet<int>();

                for (var j = 0; j < N; j++)
                {
                    var did = -1;
                    do
                    {
                        did = x.Next(0, total);
                        if (!hash.Contains(did))
                        {
                            hash.Add(did);
                            break;
                        }
                    } while (true);


                    var feature = new UMCLight();
                    feature.GroupId = did;
                    feature.Id = featureId++;
                    feature.ChargeState = charge;
                    feature.MassMonoisotopic = x.NextDouble();
                    feature.Net = x.NextDouble();
                    feature.AbundanceSum = x.Next(100, 200);
                    feature.Abundance = feature.Abundance;
                    feature.ClusterId = cluster.Id;

                    cluster.AddChildFeature(feature);
                    features.Add(feature);
                }
                cluster.CalculateStatistics(ClusterCentroidRepresentation.Mean);
                clusters.Add(cluster);
            }
            featureCache.AddAll(features);
            clusterCache.AddAll(clusters);
            clusters = clusterCache.FindAll();

            Console.WriteLine("Find all clusters");
            clusters = clusterCache.FindByCharge(charge);

            WriteClusters(datasets,
                clusters,
                minimumClusterSize,
                charge,
                crossPath,
                databasePath,
                300000);
        }

        /// <summary>
        /// Creates a cross tab
        /// </summary>
        [Test]
        [TestCase(
            //@"M:\data\proteomics\OHSU\Data\results.db3",
            @"M:\data\proteomics\OHSU\Data\results-boneLossNoMeds\results-boneLossNoMeds.db3",
            @"M:\data\proteomics\OHSU\Data\results-boneLossNoMeds\results-boneLossNoMeds-charge-1",
            //@"M:\data\proteomics\OHSU\Data\Sarc-3000_charge-1",
            1,
            2,
            Ignore = true)]
        public void CreateCrossTab(string databasePath, string crossPath, int charge, int minimumClusterSize)
        {
            NHibernateUtil.ConnectToDatabase(databasePath, false);

            IDatasetDAO datasetCache = new DatasetDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();
            IUmcDAO featureCache = new UmcDAOHibernate();

            Console.WriteLine("Find all datasets");
            var datasets = datasetCache.FindAll();

            Console.WriteLine("Find all clusters");
            var clusters = clusterCache.FindByCharge(charge);

            WriteClusters(datasets, clusters, minimumClusterSize, charge, crossPath, databasePath, 50000);
        }

        private void WriteClusters(List<DatasetInformation> datasets, List<UMCClusterLight> clusters,
            int minimumClusterSize, int charge, string crossPath, string databasePath, int minDatabase)
        {
            Console.WriteLine("Mapping cluster ids");
            // here we map the clusters to a dictionary for quick lookup
            var clusterMap = new Dictionary<int, ClusterUltraLight>();

            var clustersCut = new Dictionary<int, int>();
            for (var i = 0; i < minimumClusterSize; i++)
            {
                clustersCut.Add(i, 0);
            }

            // Map the clusters so we can add features.
            foreach (var cluster in clusters)
            {
                // Only keep clusters of a given size to cut down on files.
                if (cluster.MemberCount < minimumClusterSize)
                {
                    clustersCut[cluster.MemberCount]++;
                    continue;
                }

                var id = cluster.Id;
                if (!clusterMap.ContainsKey(id))
                {
                    var lightCluster = new ClusterUltraLight();
                    lightCluster.Ambiguity = cluster.AmbiguityScore;
                    lightCluster.DatasetCount = cluster.DatasetMemberCount;
                    lightCluster.MemberCount = cluster.MemberCount;
                    lightCluster.Drift = cluster.DriftTime;
                    lightCluster.Id = cluster.Id;
                    lightCluster.Mass = cluster.MassMonoisotopic;
                    lightCluster.Net = cluster.Net;
                    lightCluster.Tightness = cluster.Tightness;
                    lightCluster.abundances = new Dictionary<int, long>();
                    clusterMap.Add(id, lightCluster);
                }
            }
            clusters.Clear();
            clusters = null;

            // Let the user know how many clusters we cut.
            Console.WriteLine("Clusters that were cut");
            foreach (var key in clustersCut.Keys)
            {
                Console.WriteLine("\t{0}\t{1}", key, clustersCut[key]);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Find all features");
            using (var connection = new SQLiteConnection(string.Format("Data Source = {0}", databasePath)))
            {
                connection.Open();
                long features = 0;

                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        string.Format(
                            "SELECT Cluster_ID, Dataset_ID, Abundance_Sum, Abundance_Max FROM T_LCMS_Features where Charge = {0}",
                            charge);
                    command.CommandType = CommandType.Text;

                    var data = new object[4];
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            features++;
                            reader.GetValues(data);

                            var id = Convert.ToInt32(data[0]);
                            var did = Convert.ToInt32(data[1]);
                            var sum = Convert.ToInt64(data[2]);
                            var max = Convert.ToInt64(data[3]);

                            if (clusterMap.ContainsKey(id))
                            {
                                try
                                {
                                    var map = clusterMap[id].abundances;
                                    if (map.ContainsKey(did))
                                    {
                                        map[did] += sum;
                                    }
                                    else
                                    {
                                        clusterMap[id].abundances.Add(did, sum);
                                    }
                                }
                                catch
                                {
                                    var x = 0;
                                    x++;
                                    if (x > 1)
                                    {
                                    }
                                }
                            }

                            if ((features%1000000) == 0)
                            {
                                Console.WriteLine("\tPurging Finished Features {0}", features);
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                            }
                        }
                    }
                }
                connection.Close();


                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            clustersCut.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Here we map the features onto the clusters 

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Creating cross tabs");
            var sumPath = crossPath + "-sum.csv";
            using (TextWriter writer = File.CreateText(sumPath))
            {
                var header = "Cluster ID, Total Members, Dataset Members, Tightness, Ambiguity, Mass, NET, DriftTime,";

                var builder = new StringBuilder();
                var ids = new List<int>();
                foreach (var information in datasets)
                {
                    if (information.DatasetId > minDatabase)
                    {
                        continue;
                    }
                    ids.Add(information.DatasetId);
                    builder.AppendFormat("AbundanceSum-{0},", information.DatasetId);
                }
                ids.Sort();

                header += builder.ToString();
                writer.WriteLine(header);

                builder.Clear();

                long total = clusterMap.Keys.Count;
                long totalFeatures = 0;
                var features = 0;


                foreach (var id in clusterMap.Keys)
                {
                    var cluster = clusterMap[id];

                    if (features > 100000)
                    {
                        Console.WriteLine("Written {0} of {1} clusters", totalFeatures, total);
                        features = 0;
                    }
                    totalFeatures++;
                    features ++;

                    builder.AppendFormat("{0},{1},{2},{3:.000},{4:.000},{5:.0000},{6:.0000},{7:.0000},",
                        cluster.Id,
                        cluster.MemberCount,
                        cluster.DatasetCount,
                        cluster.Tightness,
                        cluster.Ambiguity,
                        cluster.Mass,
                        cluster.Net,
                        cluster.Drift);

                    foreach (var did in ids)
                    {
                        if (did > minDatabase)
                        {
                            continue;
                        }
                        // If the cluster does not have an entry for this, then leave it
                        if (cluster.abundances.ContainsKey(did))
                        {
                            builder.AppendFormat("{0},", cluster.abundances[did]);
                        }
                        else
                        {
                            builder.AppendFormat(",");
                        }
                    }
                    writer.WriteLine(builder.ToString());

                    builder.Clear();
                }
            }
        }
    }

    internal class ClusterUltraLight
    {
        public int Id;
        public double Mass;
        public double Net;
        public double Drift;
        public double Ambiguity;
        public double Tightness;
        public int MemberCount;
        public int DatasetCount;
        public Dictionary<int, long> abundances;
    }
}