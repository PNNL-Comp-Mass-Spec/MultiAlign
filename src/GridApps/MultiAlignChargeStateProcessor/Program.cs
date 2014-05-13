using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;

namespace MultiAlignChargeStateProcessor
{
    /// <summary>
    ///  This project is for clustering single charge states from an existing database file.
    /// </summary>
    static class Program
    {
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hConsoleHandle"></param>
        /// <param name="dwMode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>        
        static int Main(string [] args)
        {
            var handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            SetConsoleMode(handle, ENABLE_EXTENDED_FLAGS);

            try
            {
                if (args.Length < 2)
                {
                    Console.WriteLine(@"MultiAlignChargeStateProcessor databasePath chargeState crossTabPath [dataset List]");
                    Console.WriteLine(@"\tThe cross-tab file will be placed in the same directory as the database path");
                    return 1;
                }

                // Setup the analysis processing    
                var databasePath = args[0]; 
                var databaseName = Path.GetFileNameWithoutExtension(databasePath);
                var path         = Path.GetDirectoryName(databasePath);
                var crossPath = args[2];
                var chargeState     = Convert.ToInt32(args[1]);

                List<string> datasetList = null;
                if (args.Length == 4)
                {
                    datasetList = File.ReadAllLines(args[3]).ToList(); 
                }


                if (path == null)
                {
                    Console.WriteLine(@"The directory path is invalid");
                    return 1;
                }

                
                NHibernateUtil.ConnectToDatabase(databasePath, false);

                IDatasetDAO datasetCache = new DatasetDAOHibernate();
                var dateSuffix        = AnalysisPathUtils.BuildDateSuffix();
                Logger.LogPath           = Path.Combine(path, string.Format("{0}_charge_{2}_{1}.txt", databaseName, dateSuffix, chargeState));

                Logger.PrintMessage("Find all datasets", true);
                var datasets = datasetCache.FindAll();
                Logger.PrintMessage(string.Format("Found {0} datasets", datasets.Count), true);
                
                // Create the clustering algorithm - average linkage                
                IClusterer<UMCLight, UMCClusterLight> clusterer = new UMCAverageLinkageClusterer<UMCLight, UMCClusterLight>();

                // Create the DAO object to extract the features
                var database      = new UmcAdoDAO {DatabasePath = databasePath};
                IUmcDAO featureDao = database;
                
                
                Logger.PrintMessage(string.Format("Extracting Features"), true);
                var tempFeatures = featureDao.FindByCharge(chargeState);                                
                Logger.PrintMessage(string.Format("Found {0} features", tempFeatures.Count), true);


                var features = new List<UMCLight>();
                if (datasetList != null)
                {
                    var featuremap = datasets.ToDictionary(info => info.DatasetName.ToLower());

                    var focusedDatasetList = new Dictionary<int, DatasetInformation>();
                    foreach (var name in datasetList)
                    {
                        var key = name.ToLower();
                        if (featuremap.ContainsKey(key))
                        {
                            Logger.PrintMessage("Using dataset: " + name);
                            focusedDatasetList.Add(featuremap[key].DatasetId, featuremap[key]);
                        }
                        else
                            throw new Exception("Didn't find the dataset required..." + name);
                    }

                    features.AddRange(from feature in tempFeatures let use = focusedDatasetList.ContainsKey(feature.GroupId) where use select feature);

                    Logger.PrintMessage(string.Format("Found {0} filtered features for dataset list", features.Count), true);
                }
                else
                {
                    features = tempFeatures;
                }

                // Handle logging progress.
                clusterer.Progress      += clusterer_Progress;
                clusterer.Parameters.Tolerances.DriftTime           = .3;
                clusterer.Parameters.Tolerances.Mass                = 16;
                clusterer.Parameters.Tolerances.Net       = .014;
                clusterer.Parameters.OnlyClusterSameChargeStates    = true;
                clusterer.Parameters.CentroidRepresentation         = ClusterCentroidRepresentation.Mean;
                clusterer.Parameters.DistanceFunction               = PNNLOmics.Algorithms.Distance.DistanceFactory<UMCLight>.CreateDistanceFunction(PNNLOmics.Algorithms.Distance.DistanceMetric.WeightedEuclidean);
                
                // Then cluster
                var clusterWriter = new UmcClusterWriter();
                IClusterWriter<UMCClusterLight> writer = clusterWriter; //new UMCClusterDummyWriter(); 
                try
                {                    
                    clusterWriter.Open(crossPath);
                    clusterWriter.WriteHeader(datasets);

                    clusterer.ClusterAndProcess(features, writer);
                    Logger.PrintMessage("", true);
                    Logger.PrintMessage("ANALYSIS SUCCESS", true);
                    return 0;
                }
                catch (Exception ex)
                {
                    Logger.PrintMessage("Unhandled Error: " + ex.Message);
                    var innerEx = ex.InnerException;
                    while (innerEx != null)
                    {
                        Logger.PrintMessage("Inner Exception: " + innerEx.Message);
                        innerEx = innerEx.InnerException;
                    }
                    Logger.PrintMessage("Stack: " + ex.StackTrace);
                    Logger.PrintMessage("");
                    Logger.PrintMessage("ANALYSIS FAILED");
                    return 1;
                }                
                finally
                {
                    clusterWriter.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.PrintMessage("Unhandled Error: " + ex.Message, true);
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.PrintMessage("Inner Exception: " + innerEx.Message);
                    innerEx = innerEx.InnerException;
                }
                Logger.PrintMessage("Stack: " + ex.StackTrace, true);
                Logger.PrintMessage("");
                Logger.PrintMessage("ANALYSIS FAILED");
                return 1;
            }
        }

        static void clusterer_Progress(object sender, PNNLOmics.Algorithms.ProgressNotifierArgs e)
        {
            Logger.PrintMessage(e.Message, true);
        }
    }
}
