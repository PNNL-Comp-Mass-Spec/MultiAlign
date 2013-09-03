using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using PNNLOmics.Data.Features;
using System.Data.SQLite;
using System.IO;
using System.Runtime.InteropServices;
using PNNLOmics.Algorithms.FeatureClustering;
using MultiAlignCore.IO;

namespace MultiAlignChargeStateProcessor
{
    public class UMCClusterDummyWriter : IClusterWriter<UMCClusterLight>
    {

        #region IClusterWriter<UMCClusterLight> Members

        public void Close()
        {
            
        }

        public void WriteCluster(UMCClusterLight cluster)
        {
            
        }

        #endregion
    }
    /// <summary>
    /// UMC Cluster writer class.
    /// </summary>
    public class UMCClusterWriter : IClusterWriter<UMCClusterLight>
    {
        private TextWriter m_writer;
        private TextWriter m_idMapper;
        private List<DatasetInformation> m_datasets;
        private List<int> m_ids;

        public UMCClusterWriter()
        {
            m_datasets  = new List<DatasetInformation>();
            m_ids       = new List<int>();                        
        }
        public void Open(string path)
        {
            string folder   = Path.GetDirectoryName(path);
            string name     = Path.GetFileNameWithoutExtension(path);
            m_writer        = File.CreateText(path);
            m_idMapper      = File.CreateText(Path.Combine(folder, name + "_map.txt"));
            m_idMapper.WriteLine("Cluster, Dataset, Feature");
        }
        #region IClusterWriter<UMCClusterLight> Members

        public void Close()
        {
            m_writer.Close();
            m_idMapper.Close();
        }

        public void WriteHeader(List<DatasetInformation> datasets)
        {
            string header = "Cluster ID, Total Members, Dataset Members, Tightness, Ambiguity, Mass, NET, DriftTime,";

            StringBuilder builder = new StringBuilder();
            foreach (DatasetInformation information in datasets)
            {
                m_ids.Add(information.DatasetId);
            }
            m_ids.Sort();
            foreach (int id in m_ids)
            {
                builder.AppendFormat("AbundanceSum-{0},", id);
            }

            header += builder.ToString();
            m_writer.WriteLine(header);

        }
        
        public static int m_count = 0;
        public void WriteCluster(UMCClusterLight cluster)
        {
            m_count++;

            StringBuilder idMapper = new StringBuilder();
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0},{1},{2},{3:.000},{4:.000},{5:.0000},{6:.0000},{7:.0000},",
                                    cluster.ID,
                                    cluster.MemberCount,
                                    cluster.DatasetMemberCount,
                                    cluster.Tightness,
                                    cluster.AmbiguityScore,
                                    cluster.MassMonoisotopicAligned,
                                    cluster.RetentionTime,
                                    cluster.DriftTime);

            Dictionary<int, long> clustermap = new Dictionary<int, long>();
            foreach (UMCLight feature in cluster.Features)
            {
                if (clustermap.ContainsKey(feature.GroupID))
                {
                    clustermap[feature.GroupID] += feature.AbundanceSum;
                }
                else
                {
                    clustermap.Add(feature.GroupID, feature.AbundanceSum);
                }
                idMapper.AppendFormat(",{0},{1},{2}\n\r", cluster.ID, feature.GroupID, feature.ID);
            }

            foreach (int did in m_ids)
            {
                // If the cluster does not have an entry for this, then leave it
                if (clustermap.ContainsKey(did))
                {
                    builder.AppendFormat("{0},", clustermap[did]);
                }
                else
                {
                    builder.AppendFormat(",");
                }
            }
            m_writer.WriteLine(builder.ToString());
            m_idMapper.Write(idMapper.ToString());
            builder.Clear();
            idMapper.Clear();

            if (m_count > 1000)
            {
                m_count = 0;
                m_writer.Flush();
                m_idMapper.Flush();
            }
        }

        #endregion
    }

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
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>        
        static int Main(string [] args)
        {
            IntPtr handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            SetConsoleMode(handle, ENABLE_EXTENDED_FLAGS);

            try
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("MultiAlignChargeStateProcessor databasePath chargeState");
                    Console.WriteLine("\tThe cross-tab file will be named similar to the database path");
                    return 1;
                }

                // Setup the analysis processing    
                string databasePath = args[0]; 
                string databaseName = Path.GetFileNameWithoutExtension(databasePath);
                string path         = Path.GetDirectoryName(databasePath);
                string crossPath = args[2];
                string logPath      = Path.Combine(path, databaseName  + "_log.txt");
                int chargeState     = Convert.ToInt32(args[1]);

                
                NHibernateUtil.ConnectToDatabase(databasePath, false);

                IDatasetDAO datasetCache = new DatasetDAOHibernate();
                string dateSuffix        = AnalysisPathUtils.BuildDateSuffix();
                Logger.LogPath           = Path.Combine(path, string.Format("{0}_charge_{2}_{1}.txt", databaseName, dateSuffix, chargeState));

                Logger.PrintMessage("Find all datasets", true);
                List<DatasetInformation> datasets = datasetCache.FindAll();
                Logger.PrintMessage(string.Format("Found {0} datasets", datasets.Count), true);
                
                // Create the clustering algorithm - average linkage                
                IClusterer<UMCLight, UMCClusterLight> clusterer = new UMCAverageLinkageClusterer<UMCLight, UMCClusterLight>();

                // Create the DAO object to extract the features
                IUmcDAO featureDao      = null; 
                UmcAdoDAO database      = new UmcAdoDAO();
                database.DatabasePath   = databasePath; 
                featureDao = database;
                
                
                Logger.PrintMessage(string.Format("Extracting Features"), true);
                List<UMCLight> features = featureDao.FindByCharge(chargeState);                                
                Logger.PrintMessage(string.Format("Found {0} features", features.Count), true);

                // Handle logging progress.
                clusterer.Progress      += new EventHandler<PNNLOmics.Algorithms.ProgressNotifierArgs>(clusterer_Progress);
                clusterer.Parameters.Tolerances.DriftTime           = .3;
                clusterer.Parameters.Tolerances.Mass                = 16;
                clusterer.Parameters.Tolerances.RetentionTime       = .014;
                clusterer.Parameters.OnlyClusterSameChargeStates    = true;
                clusterer.Parameters.CentroidRepresentation         = ClusterCentroidRepresentation.Mean;
                clusterer.Parameters.DistanceFunction               = PNNLOmics.Algorithms.Distance.DistanceFactory<UMCLight>.CreateDistanceFunction(PNNLOmics.Algorithms.Distance.DistanceMetric.WeightedEuclidean);
                
                // Then cluster
                UMCClusterWriter clusterWriter = new UMCClusterWriter();
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
                    Exception innerEx = ex.InnerException;
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
                Exception innerEx = ex.InnerException;
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
