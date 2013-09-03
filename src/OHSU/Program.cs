using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignTestSuite.OHSU;

namespace ClusterExtractorConsole
{
    public class Program
    {
        static void Main(string[] args)
        {   
            if (args[0].ToLower() == "-extract")
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Invalid number of arguments.");
                    Console.WriteLine("ClusterExtractor -extract databasetPath crosstabPath chargeState minimumClusterSize");
                }
                else
                {
                    Console.WriteLine("Extracting clusters");
                    string databasePath     = args[1];
                    string crossTabPath     = args[2];
                    int chargeState         = Convert.ToInt32(args[3]);
                    int minClusterSize      = Convert.ToInt32(args[4]);

                    ExtractClusters(databasePath, crossTabPath, chargeState, minClusterSize);
                }
            }
            else
            {

            }
        }
        /// <summary>
        /// This extracts clusters from the database provided creating the cross tab provided
        /// </summary>
        public static void ExtractClusters(string databasePath, string crossPath, int charge, int minimumClusterSize)
        {
            ClusterExtractor extractor = new ClusterExtractor();
            extractor.CreateCrossTab(databasePath, crossPath, charge, minimumClusterSize);
        }
    }
}
