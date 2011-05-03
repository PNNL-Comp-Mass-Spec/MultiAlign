using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

using PNNLOmics.Data.Features;
using PNNLProteomics.IO;
using Mammoth.Data;

namespace FunctionalRedundancy
{
    class Program
    {
        /// <summary>
        /// Path to log file.
        /// </summary>
        private static string m_logPath         = null;
        /// <summary>
        /// Path to database.
        /// </summary>
        private static string m_databasePath    = null;

        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        /// <param name="message"></param>
        static void PrintMessage(string message)
        {
            if (m_logPath != null)
            {
                File.AppendAllText(m_logPath, message + Environment.NewLine);
            }
            Console.WriteLine(message);
        }

        /// <summary>
        /// filters the features by consolidating multiple features from the same dataset to one.
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        static List<UMCLight> ConsolidateFeatures(MammothCluster cluster)
        {
            List<UMCLight> features = new List<UMCLight>();
            Dictionary<int, List<UMCLight>> map = new Dictionary<int, List<UMCLight>>();

            foreach (UMCLight feature in cluster.UMCList)
            {
                bool contains = map.ContainsKey(feature.GroupID);
                if (!contains)
                {
                    map.Add(feature.GroupID, new List<UMCLight>());
                }
                map[feature.GroupID].Add(feature);
            }

            foreach (int key in map.Keys)
            {
                UMCLight prominentFeature = map[key][0];
                foreach (UMCLight featureX in map[key])
                {
                    if (featureX.Abundance > prominentFeature.Abundance)
                    {
                        prominentFeature = featureX;                        
                    }
                }

                features.Add(prominentFeature);
            }

            return features;
        }

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        private const int LC_DATA   = 0;
        private const int IMS_DATA  = 1;
        /// <summary>
        /// Main entry point for application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            IDictionary<string, List<string>> options = CommandLineParser.ProcessArgs(args, 0);            
            foreach(string option in options.Keys)
            {
                try
                {
                    List<string> values     = options[option];
                    switch(option)
                    {
                        case "-database":
                            m_databasePath  = values[0];
                            break;
                        case "-log":
                            m_logPath       = values[0];
                            break;                        
                    }   
                }
                catch(ArgumentOutOfRangeException)
                {
                    PrintMessage(string.Format("You did not provide enough information for the option {0}", option));
                    return;
                }
            }

            using (Mammoth.Data.MammothDatabase database = new Mammoth.Data.MammothDatabase(m_databasePath))
            {
                database.Connect();

                Mammoth.Data.MammothDatabaseRange range = new Mammoth.Data.MammothDatabaseRange(-100000, 1000000, -100, 1000, -100000, 10000);
                range.SingleChargeState                 = -1;
                List<MammothCluster> clusters           = database.GetClusters(range);


                foreach (MammothCluster cluster in clusters)
                {
                    cluster.UMCList = ConsolidateFeatures(cluster);
                    cluster.CalculateStatistics(ClusterCentroidRepresentation.Mean);                    

                    
                }
            }
        }

        public class FunctionalCluster: UMCClusterLight
        {

        }
    }
}
