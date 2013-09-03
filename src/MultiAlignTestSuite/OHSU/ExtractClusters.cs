using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using PNNLOmics.Data.Features;
using System.Data.SQLite;
using System.IO;

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
                10)]
        /// <summary>
        /// Creates a cross tab
        /// </summary>
        /// <param name="charge"></param>
        /// <param name="path"></param>
        public void CreateDatasetMap(string databasePath, string crossPath, int charge, int minimumClusterSize)
        {
            NHibernateUtil.ConnectToDatabase(databasePath, false);

            IDatasetDAO datasetCache    = new DatasetDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();
            IUmcDAO featureCache        = new UmcDAOHibernate();

            Console.WriteLine("Find all datasets");
            List<DatasetInformation> datasets   = datasetCache.FindAll();
            using (TextWriter writer            = File.CreateText(crossPath + ".csv"))
            {
                writer.WriteLine("Dataset, Dataset Id");
                foreach (DatasetInformation info in datasets)
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
                10)]
        public void TestClusterWriting(string databasePath, string crossPath, int charge, int minimumClusterSize)
        {
            NHibernateUtil.ConnectToDatabase(databasePath, false);

            IDatasetDAO     datasetCache                = new DatasetDAOHibernate();
            IUmcClusterDAO  clusterCache                = new UmcClusterDAOHibernate();
            IUmcDAO         featureCache                = new UmcDAOHibernate();
            
            Console.WriteLine("Find all datasets");
            List<DatasetInformation> datasets = datasetCache.FindAll();
            
            Console.WriteLine("Find all clusters"); 
            List<UMCClusterLight> clusters = clusterCache.FindByCharge(charge);

            WriteClusters(datasets, clusters, minimumClusterSize, charge, crossPath, databasePath, 300000);
        }

        [Test]
        [TestCase(
                @"M:\data\proteomics\OHSU\Data\dummy_database-charge.db3", 20, 400)]
        public void TestCreateDummyDatabase(string databasePath, int totalDatasets, int totalClusters)
        {
            File.Delete(databasePath);
            NHibernateUtil.ConnectToDatabase(databasePath, true);

            IDatasetDAO datasetCache = new DatasetDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();
            IUmcDAO featureCache = new UmcDAOHibernate();

            // Creating a dataset
            Console.WriteLine("Creating dummy datasets");
            List<DatasetInformation> datasets = new List<DatasetInformation>();
            int total = totalDatasets;
            for (int i = 0; i < total; i++)
            {
                DatasetInformation dataset = new DatasetInformation();
                dataset.DatasetId = i;
                dataset.DatasetName = "test" + i.ToString();
                datasets.Add(dataset);
            }
            datasetCache.AddAll(datasets);
            datasets.Clear();
            datasets = datasetCache.FindAll();

            // Create features
            Console.WriteLine("Creating features");
            List<UMCLight> features = new List<UMCLight>();
            List<UMCClusterLight> clusters = new List<UMCClusterLight>();
            Random x = new Random();

            int featureId = 0;
            for (int i = 0; i < totalClusters; i++)
            {                
                int N = x.Next(1, total);                
                int charge = x.Next(1, 10);                
                HashSet<int> hash = new HashSet<int>();

                double net      = x.NextDouble();
                double mass     = 400 + (1600 * x.NextDouble());
                double dt       = 60 * x.NextDouble();

                for (int j = 0; j < N; j++)
                {

                    int did = -1;
                    do
                    {
                        did = x.Next(0, total);
                        if (!hash.Contains(did))
                        {
                            hash.Add(did);
                            break;
                        }
                    } while (true);


                    UMCLight feature                = new UMCLight();
                    feature.GroupID                 = did;
                    feature.ID                      = featureId++;
                    feature.ChargeState             = charge;
                    feature.MassMonoisotopic        = Feature.ComputeDaDifferenceFromPPM(mass, 3);
                    feature.MassMonoisotopicAligned = feature.MassMonoisotopic;
                    feature.NET                     = net + .03 * x.NextDouble();
                    feature.NETAligned              = feature.NET;
                    feature.RetentionTime           = feature.NET;
                    feature.DriftTime               = dt;
                    feature.AbundanceSum            = x.Next(100, 200);
                    feature.Abundance               = feature.Abundance;
                    feature.ClusterID               = -1;                    
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
                1)]
        public void TestClusterGeneration(  string databasePath,
                                            string crossPath, 
                                            int charge,
                                            int minimumClusterSize)
        {
            File.Delete(databasePath);
            NHibernateUtil.ConnectToDatabase(databasePath, true);

            IDatasetDAO datasetCache    = new DatasetDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();
            IUmcDAO featureCache        = new UmcDAOHibernate();

            // Creating a dataset
            Console.WriteLine("Creating dummy datasets");
            List<DatasetInformation> datasets = new List<DatasetInformation>();
            int total = 10;
            for (int i = 0; i < total; i++)
            {
                DatasetInformation dataset  = new DatasetInformation();
                dataset.DatasetId           = i;
                dataset.DatasetName         = "test" + i.ToString();
                datasets.Add(dataset);
            }
            datasetCache.AddAll(datasets);
            datasets.Clear();
            datasets = datasetCache.FindAll();

            // Create features
            Console.WriteLine("Creating features");
            List<UMCLight> features = new List<UMCLight>();
            List<UMCClusterLight> clusters = new List<UMCClusterLight>();
            Random x = new Random();
            int featureId = 0;
            for (int i = 0; i < 100; i++)
            {
                UMCClusterLight cluster = new UMCClusterLight();
                cluster.ID              = i;
                cluster.AmbiguityScore  = i;
                cluster.Tightness       = i;
                                
                int N = x.Next(1, total);
                cluster.ID                  = i;
                cluster.ChargeState         = charge;
                HashSet<int> hash = new HashSet<int>();

                for(int j = 0; j < N; j++)
                {
                    
                    int did = -1;
                    do
                    {
                        did = x.Next(0, total);
                        if (!hash.Contains(did))
                        {
                            hash.Add(did);
                            break;
                        }
                    } while (true);
                    
                    
                    UMCLight feature         = new UMCLight();
                    feature.GroupID          = did;
                    feature.ID               = featureId++;
                    feature.ChargeState      = charge;
                    feature.MassMonoisotopic = x.NextDouble();
                    feature.NET              = x.NextDouble();
                    feature.AbundanceSum     = x.Next(100, 200);
                    feature.Abundance        = feature.Abundance;
                    feature.ClusterID        = cluster.ID;
                    
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

            WriteClusters(  datasets,
                            clusters, 
                            minimumClusterSize, 
                            charge,
                            crossPath,
                            databasePath, 
                            300000);
        }

        [Test]
        [TestCase(
                //@"M:\data\proteomics\OHSU\Data\results.db3",
                @"M:\data\proteomics\OHSU\Data\results-boneLossNoMeds\results-boneLossNoMeds.db3",
                @"M:\data\proteomics\OHSU\Data\results-boneLossNoMeds\results-boneLossNoMeds-charge-1", 
                //@"M:\data\proteomics\OHSU\Data\Sarc-3000_charge-1",
                1,
                2)]
        /// <summary>
        /// Creates a cross tab
        /// </summary>
        /// <param name="charge"></param>
        /// <param name="path"></param>
        public void CreateCrossTab(string databasePath, string crossPath, int charge, int minimumClusterSize)
        {
            NHibernateUtil.ConnectToDatabase(databasePath, false);

            IDatasetDAO     datasetCache                = new DatasetDAOHibernate();
            IUmcClusterDAO  clusterCache                = new UmcClusterDAOHibernate();
            IUmcDAO         featureCache                = new UmcDAOHibernate();
            
            Console.WriteLine("Find all datasets");
            List<DatasetInformation> datasets = datasetCache.FindAll();
            
            Console.WriteLine("Find all clusters"); 
            List<UMCClusterLight> clusters = clusterCache.FindByCharge(charge);

            WriteClusters(datasets, clusters, minimumClusterSize, charge, crossPath, databasePath, 50000);
        }

        private void WriteClusters(List<DatasetInformation> datasets, List<UMCClusterLight> clusters, int minimumClusterSize, int charge, string crossPath, string databasePath, int minDatabase)
        {
            Console.WriteLine("Mapping cluster ids");
            // here we map the clusters to a dictionary for quick lookup
            Dictionary<int, ClusterUltraLight> clusterMap = new Dictionary<int, ClusterUltraLight>();

            Dictionary<int, int> clustersCut = new Dictionary<int, int>();
            for (int i = 0; i < minimumClusterSize; i++)
            {
                clustersCut.Add(i, 0);
            }

            // Map the clusters so we can add features.
            foreach (UMCClusterLight cluster in clusters)
            {
                // Only keep clusters of a given size to cut down on files.
                if (cluster.MemberCount < minimumClusterSize)
                {
                    clustersCut[cluster.MemberCount]++;
                    continue;
                }
               
                int id = cluster.ID;
                if (!clusterMap.ContainsKey(id))
                {
                    ClusterUltraLight lightCluster  = new ClusterUltraLight();
                    lightCluster.Ambiguity          = cluster.AmbiguityScore;
                    lightCluster.DatasetCount       = cluster.DatasetMemberCount;             
                    lightCluster.MemberCount        = cluster.MemberCount;
                    lightCluster.Drift              = cluster.DriftTime;
                    lightCluster.Id                 = cluster.ID;
                    lightCluster.Mass               = cluster.MassMonoisotopic;
                    lightCluster.Net                = cluster.NET;
                    lightCluster.Tightness          = cluster.Tightness;
                    lightCluster.abundances = new Dictionary<int, long>();
                    clusterMap.Add(id, lightCluster);
                }
            }
            clusters.Clear();
            clusters = null;

            // Let the user know how many clusters we cut.
            Console.WriteLine("Clusters that were cut");
            foreach (int key in clustersCut.Keys)
            {
                Console.WriteLine("\t{0}\t{1}", key, clustersCut[key]);
            }            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Find all features");
            using (SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source = {0}", databasePath)))
            {
                connection.Open();                                    
                long features = 0;
                
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT Cluster_ID, Dataset_ID, Abundance_Sum, Abundance_Max FROM T_LCMS_Features where Charge = {0}", charge);
                    command.CommandType = System.Data.CommandType.Text;

                    object [] data = new object[4];
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            features++;
                            reader.GetValues(data);

                            int   id = Convert.ToInt32(data[0]);
                            int  did = Convert.ToInt32(data[1]);
                            long sum = Convert.ToInt64(data[2]);
                            long max = Convert.ToInt64(data[3]);
                            
                            if (clusterMap.ContainsKey(id))
                            {
                                try
                                {
                                    Dictionary<int, long> map = clusterMap[id].abundances;
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
                                    int x = 0;
                                    x++;
                                    if (x > 1)
                                    {

                                    }
                                }
                            }

                            if ((features % 1000000) == 0)
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
            string sumPath = crossPath + "-sum.csv";
            using (TextWriter writer = File.CreateText(sumPath))
            {
                string header = "Cluster ID, Total Members, Dataset Members, Tightness, Ambiguity, Mass, NET, DriftTime,";
                
                StringBuilder builder = new StringBuilder();
                List<int> ids = new List<int>();
                foreach(DatasetInformation information in datasets)
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

                long total          = clusterMap.Keys.Count;
                long totalFeatures  = 0;
                int features        = 0;

                
                foreach (int id in clusterMap.Keys)
                {
                    ClusterUltraLight cluster = clusterMap[id];

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

                    foreach (int did in ids)
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

    class ClusterUltraLight
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
