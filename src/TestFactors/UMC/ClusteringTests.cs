using System;
using System.Collections.Generic;

using MultiAlignEngine;
using MultiAlignEngine.Features;
using MultiAlignEngine.Clustering;

namespace Test.UMC
{
    public class ClusteringTests
    {
        public clsUMCData Cluster(clsUMCData umcData, clsClusterOptions clusterOptions)
        {
            int     i        = 0;
            string  basename = "base";
           
            /// 
            /// Create a cluster processor object
            /// 
            clsClusterProcessor processor = new clsClusterProcessor();
            processor.ClusterOptions      = clusterOptions;
            processor.PerformClustering(umcData);

            return umcData;            
        }

        public void PrintClusterData(clsUMCData data)
        {
            int maxSize = 0;
            int minSize = int.MaxValue;

            /// 
            /// Find the size of each cluster...
            /// 
            clsClusterData clusters = data.mobjClusterData;
            int N = clusters.NumClusters;

            for (int i = 0; i < N; i++)
            {
                clsCluster cluster = clusters.GetCluster(i);

                maxSize = Math.Max(cluster.mshort_num_dataset_members, maxSize);
                minSize = Math.Min(cluster.mshort_num_dataset_members, minSize);
            }
            Console.WriteLine("\tMin Cluster Size {0} \n\tMax Cluster Size {1}", minSize, maxSize);
        }


        /// <summary>
        /// Runs cluster tests.
        /// </summary>
        public void TestClusters()
        {
            int     shift       = 0;
            string pathTest = @"C:\Documents and Settings\d3m276\My Documents\Rifle_21_ORBIa_5Oct07_Draco_07-09-15_isos.csv";

            if (true)
            {
                /// 
                /// Find the UMC's
                /// 
                clsUMCCreator creator       = new clsUMCCreator();
                creator.UMCFindingOptions   = new clsUMCFindingOptions();
                creator.FileName            = pathTest;
                creator.LoadUMCs(false);
                creator.FindUMCs();
                clsUMC [] set1 = creator.GetUMCs();

                /// 
                /// Find the next set 
                /// 
                clsUMCCreator creator2      = new clsUMCCreator();
                creator2.UMCFindingOptions  = new clsUMCFindingOptions();
                creator2.FileName           = pathTest;
                creator2.LoadUMCs(false);
                creator2.FindUMCs();
                clsUMC [] set2 = creator2.GetUMCs();
                
                /// 
                /// Set cluster options
                /// 
                clsClusterOptions options = new clsClusterOptions();
                options.MassTolerance = 0.0; // 1.0 / (1000000.0);
                options.NETTolerance  = 0.0; // 1.0 / (1000000.0);
                
                /// 
                /// Run cluster test with one dataset
                /// 
                clsUMCData umcDataTest1 = new clsUMCData();
                set1[0].MassCalibrated = set1[2].MassCalibrated;
                set1[0].Net = set1[2].Net;

                set1[1].MassCalibrated = set1[2].MassCalibrated;
                set1[1].Net = set1[2].Net;

                umcDataTest1.SetUMCS("one", ref set1);
                            

                clsUMCData data1 =  Cluster(umcDataTest1, options);
                Console.WriteLine("Test 1 Single dataset {0}:\n\tTotal UMC's {1}\n\tTotal Clusters {2}", 
                                    pathTest,
                                    set1.Length, 
                                    data1.mobjClusterData.NumClusters);
                PrintClusterData(data1);

                /// //////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Run cluster test with two datasets (the same)
                /// //////////////////////////////////////////////////////////////////////////////////////////////////////           
                clsUMCData umcDataTest2 = new clsUMCData();
                umcDataTest2.SetUMCS("one", ref set1);
                umcDataTest2.SetUMCS("two", ref set2);

                options = new clsClusterOptions();
                clsUMCData data2 =  Cluster(umcDataTest2, options);
                Console.WriteLine("Test 2 Two datasets - Same {0}:\n\tTotal UMC's {1}\n\tTotal Clusters {2}",
                                    pathTest,
                                    set2.Length,
                                    data2.mobjClusterData.NumClusters);
                PrintClusterData(data2);           
            }

            /// //////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Run cluster test with same points 
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////           
            clsUMCData umcDataTest3 = new clsUMCData();
            clsUMC[] set3 = new clsUMC[30];

            clsClusterOptions options3 = new clsClusterOptions();
            options3.MassTolerance = 0;
            options3.NETTolerance  = 0;

            
            for (int i = 0; i < set3.Length; i++)
            {
                double j                = Convert.ToDouble(i) / 1000000;
               
                set3[i]                 = new clsUMC();
                set3[i].MassCalibrated  = 500 + j;
                set3[i].Mass            = 500 + j;
                set3[i].Scan            = 500 + i;
                set3[i].ScanStart       = 500 + i;
                set3[i].ScanEnd         = 500 + i;
                set3[i].ScanAligned     = 500 + i;
                set3[i].Net             = 500 + .001 * i;
                set3[i].Id              = i;
            }

            umcDataTest3.SetUMCS("three", ref set3);

            
            clsUMCData data3 = Cluster(umcDataTest3, options3);
            Console.WriteLine("Test 3 Two datasets - Same {0}:\n\tTotal UMC's {1}\n\tTotal Clusters {2}",
                                pathTest,
                                set3.Length,
                                data3.mobjClusterData.NumClusters);
            PrintClusterData(data3);
            Console.ReadKey();


        }
    }
}
