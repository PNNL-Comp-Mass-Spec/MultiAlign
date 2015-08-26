using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Algorithms.FeatureMatcher.Data;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.MTDB;

namespace MultiAlignSTACRunner
{
    /// <summary>
        ///  This project is for running STAC with previously analyzed MultiAlign results.
        /// </summary>
        static class Program
        {
            private const uint ENABLE_EXTENDED_FLAGS        = 0x0080;
            
            private const int CONST_COLUMN_ID               = 0;
            private const int CONST_COLUMN_TOTAL_MEMBERS    = 1; 
            private const int CONST_COLUMN_DATASET_MEMBERS  = 2;            
            private const int CONST_COLUMN_TIGHTNESS        = 3;
            private const int CONST_COLUMN_AMBIGUITY        = 4;
            private const int CONST_COLUMN_MASS             = 5;
            private const int CONST_COLUMN_NET              = 6;
            private const int CONST_COLUMN_DRIFT            = 7;
    
            /// <summary>
            /// Reads the clusters and filters cluster by cluster.  
            /// </summary>
            private static List<UMCClusterLight> ReadClusters(string path, FilteringOptions options)
            {
                var clusters = new List<UMCClusterLight>();
                var totalClusters = 0;                
                using (TextReader reader = File.OpenText(path))
                {                     
                    reader.ReadLine();
                    string line;
                    
                        while ((line = reader.ReadLine()) != null)
                        {
                            var data           = line.Split(',');

                            var mass            = Convert.ToDouble(data[CONST_COLUMN_MASS]);
                            var retentionTime   = Convert.ToDouble(data[CONST_COLUMN_NET]);

                            var cluster         = new UMCClusterLight
                            {
                                Id                      = Convert.ToInt32(data[CONST_COLUMN_ID]),
                                MassMonoisotopic        = mass,
                                MassMonoisotopicAligned = mass,                                                                
                                NetAligned              = retentionTime,            
                                Net                     = retentionTime,     
                                DriftTime               = Convert.ToDouble(data[CONST_COLUMN_DRIFT]),                                                                
                                Tightness               = Convert.ToDouble(data[CONST_COLUMN_TIGHTNESS]),
                                AmbiguityScore          = Convert.ToDouble(data[CONST_COLUMN_AMBIGUITY]),
                                DatasetMemberCount      = Convert.ToInt32(data[CONST_COLUMN_DATASET_MEMBERS]),
                                MemberCount             = Convert.ToInt32(data[CONST_COLUMN_TOTAL_MEMBERS])
                            };

                            var shouldFilter = ShouldFilterCluster(cluster, options);
                            if (!shouldFilter)
                                clusters.Add(cluster);
                            totalClusters++;
                        }
                   
                }

                Logger.PrintMessage(string.Format("Found {0} filtered clusters from {1} total clusters", clusters.Count, totalClusters));

                return clusters;
            }

            private static bool ShouldFilterCluster(UMCClusterLight cluster, FilteringOptions options)
            {
                // 
                //  Total dataset member count      ~
                //  Ratio of members dataset : total                
                //      1 but not > 1.5-2
                // 
                var datasetMemberCount = cluster.DatasetMemberCount;
                if (datasetMemberCount < options.TotalDatasetMembers)
                    return true;
                var totalMembers = cluster.MemberCount;
                var ratio        = Convert.ToDouble(totalMembers)/Convert.ToDouble(datasetMemberCount);
                if (ratio <= options.MaxRatio && ratio >= options.MinRatio)
                    return false;
                return true;
            }

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
            static int Main(string[] args)
            {
                var handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                SetConsoleMode(handle, ENABLE_EXTENDED_FLAGS);

                var clusterIdMap = new Dictionary<int, UMCClusterLight>();

                try
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("MultiAlignSTACRunner crosstabDirectory databasePath outputName");
                        Console.WriteLine("\tThe cross-tab file will be named similar to the database path");
                        return 1;
                    }

                    // Setup the analysis processing    
                    Logger.PrintMessage("Find all datasets", true);
                    var directoryPath    = args[0].Replace("\r","").Replace("\n","");                    
                    var databasePath     = args[1].Replace("\r","").Replace("\n","");                    
                    var name             = args[2].Replace("\r","").Replace("\n","");                                                 
                    var files          = Directory.GetFiles(directoryPath, "*.csv");

                    Logger.PrintMessage("Creating Log File");
                    var loggerPath       = AnalysisPathUtils.BuildLogPath(Path.GetDirectoryName(name), Path.GetFileNameWithoutExtension(name));
                    Logger.LogPath = loggerPath;
                    Logger.PrintMessage("Saving Log Data to: " + loggerPath);
                    

                    Logger.PrintMessage(string.Format("Creating STAC"), true);

                    // Hardcode bad, but per discussions with OHSU
                    var stac = new STACAdapter<UMCClusterLight>
                    {                        
                        Options = new FeatureMatcherParameters
                        {                            
                            UserTolerances =
                            {
                                MassTolerancePPM = 25,
                                NETTolerance = .035,
                                DriftTimeTolerance = 3
                            },
                            UseDriftTime = true
                        }
                    };


                    var clusterFilteringOptions = new FilteringOptions();

                    WriteOptionsToLogFile(clusterFilteringOptions);
                    WriteOptionsToLogFile(stac.Options);


                    // Read the cluster data
                    var allClusters = new List<UMCClusterLight>();
                    var clusterId   = 0;
                    foreach (var file in files)
                    {
                        var filename = Path.GetFileName(file);
                        Logger.PrintMessage(string.Format("Reading {0}", filename));
                        const string chargeString = "charge";

                        if (filename == null)
                            continue;

                        if (!filename.Contains(chargeString))
                            continue;

                        // Read each file.
                        var start = DateTime.Now;

                        var xname               = filename.Replace("_", "");
                        // ReSharper disable once StringIndexOfIsCultureSpecific.1
                        var indexOfChargeString = xname.IndexOf(chargeString);
                        var charge              = Convert.ToInt32(xname.Substring(indexOfChargeString + chargeString.Length, 1));

                        var clusters = ReadClusters(file, clusterFilteringOptions);
                        var end      = DateTime.Now;
                        Logger.PrintMessage(string.Format("\tReading Took {0:.00} seconds", end.Subtract(start).TotalSeconds));                        

                        foreach (var cluster in clusters)
                        {
                            clusterIdMap.Add(clusterId, cluster);
                            cluster.Id = clusterId++;
                            cluster.ChargeState = charge;
                        }

                        allClusters.AddRange(clusters);
                    }


                    // Load the database
                    Logger.PrintMessage(string.Format("Loading Mass Tag Database: {0}", Path.GetFileNameWithoutExtension(databasePath)));
                    var options             = new MassTagDatabaseOptions();
                    var databaseDefinition  = new InputDatabase
                    {
                        DatabaseFormat  = MassTagDatabaseFormat.Sqlite,
                        LocalPath       = databasePath
                    };
                    var database            = MtdbLoaderFactory.LoadMassTagDatabase(databaseDefinition, options);                    

                    // Run stac
                    try
                    {
                        Logger.PrintMessage("Matching clusters to peptides in mass tag database.");
                        var matches = stac.PerformPeakMatching(allClusters, database);

                        Logger.PrintMessage(string.Format("Writing Results To {0}", name));
                        var duplicateMatches = new Dictionary<int, Dictionary<int, FeatureMatchLight<UMCClusterLight, MassTagLight>>>();

                        foreach (var match in matches)
                        {
                            if (!duplicateMatches.ContainsKey(match.Observed.Id))                            
                                duplicateMatches.Add(match.Observed.Id, new Dictionary<int, FeatureMatchLight<UMCClusterLight, MassTagLight>>());
                            
                            if (!duplicateMatches[match.Observed.Id].ContainsKey(match.Target.Id))                            
                                duplicateMatches[match.Observed.Id].Add(match.Target.Id, match);
                            
                        }
                        WriteClusterData(name, duplicateMatches, clusterIdMap);
                        

                        Logger.PrintMessage("ANALYSIS SUCCESS");
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

            private static void WriteOptionsToLogFile(FeatureMatcherParameters featureMatcherParameters)
            {
                Logger.PrintMessage("STAC Options");
                Logger.PrintMessage(string.Format("\tUse Drift Time:{0}", featureMatcherParameters.UseDriftTime));
                Logger.PrintMessage(string.Format("\tHistgoram Width:{0}", featureMatcherParameters.HistogramBinWidth));
                Logger.PrintMessage(string.Format("\tHistogram Multiplier:{0}", featureMatcherParameters.HistogramMultiplier));
                Logger.PrintMessage(string.Format("\tShift Amount:{0}", featureMatcherParameters.ShiftAmount));
                Logger.PrintMessage(string.Format("\tShouldCalculateHistogramFDR:{0}", featureMatcherParameters.ShouldCalculateHistogramFDR));
                Logger.PrintMessage(string.Format("\tShouldCalculateSLiC:{0}", featureMatcherParameters.ShouldCalculateSLiC));
                Logger.PrintMessage(string.Format("\tShouldCalculateSTAC:{0}", featureMatcherParameters.ShouldCalculateSTAC));
                Logger.PrintMessage(string.Format("\tShouldCalculateShiftFDR:{0}", featureMatcherParameters.ShouldCalculateShiftFDR));
                Logger.PrintMessage(string.Format("\tUseEllipsoid:{0}", featureMatcherParameters.UseEllipsoid));
                Logger.PrintMessage(string.Format("\tUsePriors:{0}", featureMatcherParameters.UsePriors));
                Logger.PrintMessage(string.Format("\tUserTolerances.MassTolerancePPM:{0}", featureMatcherParameters.UserTolerances.MassTolerancePPM));
                Logger.PrintMessage(string.Format("\tUserTolerances.NETTolerance:{0}", featureMatcherParameters.UserTolerances.NETTolerance));
                Logger.PrintMessage(string.Format("\tUserTolerances.DriftTimeTolerance:{0}", featureMatcherParameters.UserTolerances.DriftTimeTolerance));
            }

            private static void WriteOptionsToLogFile(FilteringOptions clusterFilteringOptions)
            {
                Logger.PrintMessage("Cluster Filtering Options");
                Logger.PrintMessage(string.Format("\tMax Cluster Total to Dataset Ratio:{0}", clusterFilteringOptions.MaxRatio));
                Logger.PrintMessage(string.Format("\tMin Cluster Total to Dataset Ratio:{0}", clusterFilteringOptions.MinRatio));
                Logger.PrintMessage(string.Format("\tRequired Total Members:{0}", clusterFilteringOptions.TotalDatasetMembers));
            }

            private static void WriteClusterData(string path, Dictionary<int, Dictionary<int, FeatureMatchLight<UMCClusterLight, MassTagLight>>> duplicateMatches, Dictionary<int, UMCClusterLight> clusterIdMap)
            {
                using (var writer = File.CreateText(path))
                {
                    writer.WriteLine(
                        "Cluster Id, Dataset Member Count, Total Member Count, Cluster Mass, Cluster NET, Cluster Drift Time, Cluster Charge,Mass Tag Id,Peptide Sequence,Mass Tag Mono Mass,Mass Tag NET,Mass Tag Drift Time, STAC, STAC-UP");
                    foreach (var map in duplicateMatches.Values)
                    {
                        foreach (var match in map.Values)
                        {
                            var cluster = clusterIdMap[match.Observed.Id];


                            writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}",
                                cluster.Id,
                                cluster.DatasetMemberCount,
                                cluster.MemberCount,
                                cluster.MassMonoisotopic,
                                cluster.Net,
                                cluster.DriftTime,                                
                                match.Observed.ChargeState,
                                match.Target.Id,
                                match.Target.PeptideSequence,
                                match.Target.MassMonoisotopic,
                                match.Target.Net,
                                match.Target.DriftTime,
                                match.Confidence,
                                match.Uniqueness);
                        }
                    }
                }
            }
        }
    
}
