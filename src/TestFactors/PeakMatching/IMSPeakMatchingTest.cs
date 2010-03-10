using System;
using System.Collections.Generic;


using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.PeakMatching;


namespace Test.PeakMatching
{    

    public class PeakMatchingTest
    {
        private void Print(string message)
        {
            Console.WriteLine(message);
        }
        private void PrintCluster(clsCluster cluster)
        {
            Console.WriteLine("\tCluster ID {0}\tMass {1}\tNET {2}\tDrift Time{3}",
                            cluster.Id,
                            cluster.MassCalibrated,
                            cluster.NetAligned,
                            cluster.mdouble_driftTime);
        }
        private void PrintMassTag(clsMassTag tag)
        {
            Console.WriteLine("\tTag ID {0}\tMass {1}\tNET {2}\tDrift Time{3}\tPeptide {4}",
                            tag.Id,
                            tag.Mass,
                            tag.NetAverage,
                            tag.DriftTime,
                            tag.mstrPeptide);
        }

        /// <summary>
        /// Test case one, match four UMC clusters to a mass tag
        /// with no drift time.
        /// 
        /// </summary>
        private void TestCase1()
        {
            /// 
            /// Create database 
            /// 
            clsMassTagDB database = new clsMassTagDB();          
            clsMassTag[] tags   = new clsMassTag[2];

            clsMassTag tag      = new clsMassTag();
            tag.DriftTime       = 0;
            tag.Mass            = .5 / 1000000.0; // ppm
            tag.mdblAvgGANET    = .5;
            tag.mstrPeptide     = "A";
            tag.Id              = 0;            
            

            clsMassTag tag2      = new clsMassTag();
            tag2.DriftTime       = 0;
            tag2.Mass            = 1000.5 / 1000000.0; // ppm
            tag2.mdblAvgGANET    = .5;
            tag2.mstrPeptide     = "Bad";
            tag2.Id              = 0;

            tags[1] = tag2;
            tags[0] = tag;            

            database.AddMassTags(tags);
            database.AddProteins(new int[] { 0, 1 }, new string[] { "prot-a" , "prot-b"}, new int[] { 1 , 0});

            Print("Tags");
            foreach (clsMassTag tagX in tags)
            {
                PrintMassTag(tagX);
            }

            /// 
            /// Construct peak matching objects and options 
            /// 
            clsPeakMatchingOptions options      = new clsPeakMatchingOptions();
            options.MassTolerance               = 5; // ppm
            options.NETTolerance                = .7;
            options.DriftTimeTolerance          = 150;            

            clsPeakMatchingProcessor processor  = new clsPeakMatchingProcessor();

            /// 
            /// Construct UMC clusters 
            /// 
            clsClusterData clusterData = new clsClusterData();            
            double maxNET   = 1;
            double minNET   = 0;
            double dNET     = 1;
            double maxMass  = 1.0 / (1000000.0); // ppm
            double minMass  = 0;
            double dMass    = 1.0 / (1000000.0); // ppm

            Print("Clusters");
            int id = 0;
            for(double net = minNET;  net <= maxNET + .01; net += dNET)
            {
                for(double mass = minMass;  mass <= maxMass; mass += dMass)
                {
                    clsCluster cluster          = new clsCluster();
                    cluster.MassCalibrated      = mass;
                    cluster.NetAligned          = net;
                    cluster.mdouble_driftTime   = 0;
                    cluster.Id                  = id++;

                    PrintCluster(cluster);

                    clusterData.AddCluster(cluster);                    
                }
            }

            /// 
            /// Process!!
            /// 
            processor.DriftTimeTolerance    = options.DriftTimeTolerance;
            processor.MassTolerance         = options.MassTolerance;
            processor.NETTolerance          = options.NETTolerance;

            clsPeakMatchingResults results = processor.PerformPeakMatching(clusterData, database);
            

            
            Print("Peak Match Results");
            Console.WriteLine("\tTotal Cluster Matches: {0}", results.NumMatches);
            Console.WriteLine("\tTotal Mass Tag Matches: {0}", results.NumMassTagsMatched);
            Console.WriteLine("\tMatches");
            foreach (clsPeakMatchingResults.clsPeakMatchingTriplet triplet in results.marrPeakMatchingTriplet)
            {
                clsMassTag mtTag = database.GetMassTag(triplet.mintMassTagIndex);                
                Console.WriteLine("\t\tTag {0}\tFeature {1}\tProtein {2}\tPeptide {3}", triplet.mintMassTagIndex, triplet.mintFeatureIndex, triplet.mintProteinIndex,mtTag.mstrPeptide);
            }            
        }
        public void RunTests()
        {
            TestCase1();                      
        }
    }
}
