using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using MultiAlign.Drawing;
using MultiAlignEngine.Features;
using PNNLProteomics.Algorithms;
using PNNLProteomics.Algorithms.Clustering;
using PNNLProteomics.Data;
using PNNLProteomics.IO;
using PNNLProteomics.MultiAlign;
using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;


namespace MultiAlignConsole
{
    /// <summary>
    /// Main application.
    /// </summary>
    class Program
    {
        #region Constants 
        /// <summary>
        /// Default path for plots.
        /// </summary>
        private const string THUMBNAIL_PATH = "Plots";        
        private const int PLOT_WIDTH        = 800; 
        private const int PLOT_HEIGHT       = 800;
        private const int PLOT_WIDTH_HTML   = 256; 
        private const int PLOT_HEIGHT_HTML   = 256;            
        #endregion

        #region Members
        /// <summary>
        /// Holds a list of HTML tags for displaying plot results.
        /// </summary>
        private static List<string> m_htmlPage;
        private static ManualResetEvent     m_triggerEvent;
        private static string               m_logPath;
        private static MultiAlignAnalysis   m_analysis;
        /// <summary>
        /// Where the data and plots needs to go.
        /// </summary>
        private static string m_analysisPath;
        /// <summary>
        /// The name of the analysis.
        /// </summary>
        private static string m_analysisName;
        /// <summary>
        /// Path to save the plots.
        /// </summary>
        private static string m_plotSavePath;
        /// <summary>
        /// Height of the thumbnail plots.
        /// </summary>
        private static int m_height;
        /// <summary>
        /// Width of the thumbnail plots.
        /// </summary>
        private static int m_width;
        /// <summary>
        /// Height of the thumbnail plots.
        /// </summary>
        private static int m_heightHTML;
        /// <summary>
        /// Width of the thumbnail plots.
        /// </summary>
        private static int m_widthHTML;
        #endregion
        
        #region Methods
        static void SaveImage(Image image, string name)
        {
            if (image != null)
            {                
                string path = Path.Combine(m_plotSavePath, name);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);                
            }
            else
            {
                Log(string.Format("Could not create {0} plot.", name));
            }
        }
        /// <summary>
        /// Creates a cluster size histogram for unique datasets.
        /// </summary>
        static private void CreateClusterSizeHistogram(UmcDAOHibernate featureCache, int datasets)
        {
            //Dictionary<int, 
            for (int i = 0; i < datasets; i++)
            {
                List<clsUMC> features = featureCache.FindByDatasetId(i);
                foreach (clsUMC feature in features)
                {

                }
            }            
        }
        /// <summary>
        /// Creates the final analysis plots.
        /// </summary>
        static void CreateFinalAnalysisPlots()
        {
            Log("Creating Final Plots");
            PushTextHeader("Feature Plots "); 
            PushStartTable();
            PushStartTableRow();

            // Create the heatmap
            UmcDAOHibernate cache = new UmcDAOHibernate();
            List<clsUMC> umcs = cache.FindAll();
            ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true,
                                                1, 100,
                                                "Charge State Histogram", "Charge State", "Count", m_width, m_height);

            Image image = RenderDatasetInfo.ChargeStateHistogram_Thumbnail(umcs, m_width, m_height, options);
            SaveImage(image, "ChargeStates.png");
            PushImageColumn(Path.Combine("Plots", "ChargeStates.png"));

            PushEndTableRow();
            PushEndTable();

            PushTextHeader("Cluster Plots "); 
            PushStartTable();
            PushStartTableRow();

            UmcClusterDAOHibernate  clusterCache = new UmcClusterDAOHibernate();
            List<clsCluster>        clusters     = clusterCache.FindAll();

            // Then render the cluster size image.
            options.Title   = "Cluster Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";
            image           = RenderDatasetInfo.ClusterSizeHistogram_Thumbnail(clusters, m_width, m_height, options);
            SaveImage(image, "ClusterMemberSizes.png");
            PushImageColumn(Path.Combine("Plots", "ClusterMemberSizes.png"));

            // Then render the cluster size image.
            options.Title = "Cluster Dataset Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";
            image = RenderDatasetInfo.ClusterDatasetMemberSizeHistogram_Thumbnail(clusters, m_width, m_height, options);
            SaveImage(image, "ClusterDatasetMemberSizes.png");
            PushImageColumn(Path.Combine("Plots", "ClusterDatasetMemberSizes.png"));

            PushEndTableRow();
            PushEndTable();

            PushTextHeader("Cluster Score Plots ");
            PushTextHeader("Mass");

            PushStartTable();
            PushStartTableRow();
 
            options.DisplayLegend = false;

            // Mass vs. Member Count  
            options.Title       = "Cluster Monoisotopic Mass Vs. Cluster Member Count";
            options.YAxisLabel  = "Cluster Monoisotopic Mass";
            options.XAxisLabel  = "Member Count";
            image = RenderDatasetInfo.ClusterMemberVsMass_Thumbnail(clusters, options);
            SaveImage(image, "MassVsMemberCount.png");
            PushImageColumn(Path.Combine("Plots", "MassVsMemberCount.png"));

            // Mass Vs. Average Cluster Score.
            options.Title = "Mono Mass vs. Average Cluster Score";
            options.YAxisLabel = "Cluster Monoisotopic Mass";
            options.XAxisLabel = "Average Cluster Score";
            image = RenderDatasetInfo.ClusterScoreVsMassLine_Thumbnail(clusters, options);
            SaveImage(image, "MassVsAverageClusterScore.png");
            PushImageColumn(Path.Combine("Plots", "MassVsAverageClusterScore.png"));

            // Mass vs. Cluster score 
            options.Title = "Monoisotopic Mass vs. Cluster Score";
            options.YAxisLabel = "Cluster Monoisotopic Mass";
            options.XAxisLabel = "Cluster Score";
            image = RenderDatasetInfo.ClusterScoreVsMass_Thumbnail(clusters, options);
            SaveImage(image, "ClusterScoreVsMass.png");
            PushImageColumn(Path.Combine("Plots", "ClusterScoreVsMass.png"));
            PushEndTableRow();
            PushEndTable();

            PushTextHeader("NET");
            PushStartTable();
            PushStartTableRow();
            
            // Cluster size vs. NET
            options.Title       = "Cluster Member Count vs. NET";
            options.XAxisLabel  = "Cluster NET";
            options.YAxisLabel  = "Member Count";
            image                    = RenderDatasetInfo.ClusterMemberVsNET_Thumbnail(clusters, options);
            SaveImage(image, "MemberCountVsNET.png");
            PushImageColumn(Path.Combine("Plots", "MemberCountVsNET.png"));

            // average Cluster score vs. NET
            options.Title = "Average Cluster Score vs. NET";
            options.XAxisLabel = "Cluster NET";
            options.YAxisLabel = "Average Cluster Score";
            image = RenderDatasetInfo.ClusterScoreVsNetLine_Thumbnail(clusters, options);
            SaveImage(image, "AverageClusterScoreVsNET.png");
            PushImageColumn(Path.Combine("Plots", "AverageClusterScoreVsNET.png"));
            
            // Cluster score vs. NET
            options.Title = "Cluster Score vs. NET";
            options.XAxisLabel = "Cluster NET";
            options.YAxisLabel = "Cluster Score";
            image = RenderDatasetInfo.ClusterScoreVsNET_Thumbnail(clusters, options);
            SaveImage(image, "ClusterScoreVsNET.png");
            PushImageColumn(Path.Combine("Plots", "ClusterScoreVsNET.png" ));

            PushEndTableRow();
            PushEndTable();

            PushTextHeader("Cluster Filter Scatter Plots");
            PushStartTable();
            // Cluster score vs. NET
            int nPlots      = 20;
            int nCols       = 5;
            double maxScore = 1.0;
            double dScore   = maxScore / Convert.ToDouble(nPlots);
            double score    = dScore;
            int cols = 0;
            for (int i = 0; i < nPlots; i++ )
            {
                if (cols == 0)
                {
                    PushStartTableRow();
                }                
                options.Title       = "Cluster Score Monoisotopic Mass vs. NET (score = " + score.ToString() + " )";
                options.XAxisLabel  = "Cluster NET";
                options.YAxisLabel  = "Cluster Monoisotopic Mass";
                image               = RenderDatasetInfo.ClustersPassingScoreScatterPlot_Thumbnail(clusters, score, options);
                if (image != null)
                {
                    string scoreID = string.Format("ClusterScoreScatterPlot-{0:0.00}.png", score);
                    SaveImage(image, scoreID);
                    PushImageColumn(Path.Combine("Plots", scoreID));
                }

                cols++;
                if (cols == nCols)
                {
                    PushEndTableRow();
                    cols = 0;
                }
                score += dScore;
            }
            // Make sure the tag is closed.
            if (cols != nCols)
            {
                PushEndTableRow();             
            }
            PushStartTableRow();
            score               = double.PositiveInfinity;
            options.Title       = "Cluster Score Monoisotopic Mass vs. NET (score = " + score.ToString() + " )";
            options.XAxisLabel  = "Cluster NET";
            options.YAxisLabel  = "Cluster Monoisotopic Mass";
            image = RenderDatasetInfo.ClustersPassingScoreScatterPlot_Thumbnail(clusters, score, options);
            if (image != null)
            {
                string scoreID = string.Format("ClusterScoreScatterPlot-{0:0.00}.png", score);
                SaveImage(image, scoreID);
                PushImageColumn(Path.Combine("Plots", scoreID));
            }
            options.Title       = string.Format("Cluster Count vs. Cluster Score (Total Clusters = {0})", clusters.Count); 
            options.XAxisLabel  = "Cluster Score";
            options.YAxisLabel  = "Cluster Count";
            image = RenderDatasetInfo.ClustersCountPassingScoreFilter_Thumbnail(clusters, dScore, dScore, maxScore, options);
            if (image != null)
            {
                string scoreID = "ClusterScoreVsCount.png";
                SaveImage(image, scoreID);
                PushImageColumn(Path.Combine("Plots", scoreID));
            }
            
            PushEndTableRow();
            PushStartTableRow();
            Log("Creating Scan Width Plots");

            List<double> scoresX = new List<double>();
            List<double> rangesX = new List<double>();
            Dictionary<int, List<double>> ranges = CreateScanWidths(clusters, ref rangesX, ref scoresX);


            options.Title = string.Format("Cluster NET Range vs. Cluster Score (Total Clusters = {0})", clusters.Count);
            options.XAxisLabel = "Cluster NET Range";
            options.YAxisLabel = "Cluster Score";
            image = RenderDatasetInfo.ClusterNetRangeVsScore_Thumbnail( rangesX, scoresX, options);
            if (image != null)
            {
                string scoreID = "ClusterNETRangeVsScore.png";
                SaveImage(image, scoreID);
                PushImageColumn(Path.Combine("Plots", scoreID));
            }

            PushEndTableRow();
            PushEndTable();

            PushTextHeader("Cluster NET Range Histograms");
            PushStartTable();

            options.XAxisLabel = "NET Range";
            options.YAxisLabel = "Count";

            List<int> sizes = new List<int>();
            foreach (int size in ranges.Keys)
            {
                sizes.Add(size);
            }
            sizes.Sort();

            cols = 0;            
            foreach (int clusterSize in sizes)
            {
                if (clusterSize < 2)
                    continue;

                if (cols == 0)
                {
                    PushStartTableRow();
                }
                List<double> range = ranges[clusterSize];
                options.Title = "Cluster Dataset Member Size vs. Clusters NET Range Histogram (cluster size = " + clusterSize.ToString() + " total clusters = " + range.Count.ToString() + " )";                
                MultiAlign.Charting.controlHistogram chart = RenderDatasetInfo.ClusterSizeNETRange_Chart(clusterSize, range, options);
                if (chart != null)
                {
                    image = chart.ToBitmap(m_width, m_height);
                    SaveImage(image, string.Format("ClusterDatasetMemberSizeNETRange_{0}.png", clusterSize));
                    PushImageColumn(Path.Combine("Plots", string.Format("ClusterDatasetMemberSizeNETRange_{0}.png", clusterSize)));
                }
                cols++;
                if (cols == nCols)
                {
                    cols = 0;
                    PushEndTableRow();
                }
            }

            PushEndTable();
        }
        /// <summary>
        /// Creates images for detecting chaining phenomena with clusters.
        /// </summary>
        /// <param name="clusters"></param>
        static Dictionary<int, List<double>> CreateScanWidths(List<clsCluster> clusters, ref List<double> ranges, ref List<double> scores)
        {
            int count  = clusters.Count;
            int buffer = 100;
            int i = 0;
            UmcDAOHibernate featureCache = new UmcDAOHibernate();
            //TODO: Fix to make this a dictionary cluster id to object.
            Dictionary<int, List<double>> scanRanges = new Dictionary<int, List<double>>();
            Dictionary<int, clsCluster> scoreRanges = new Dictionary<int, clsCluster>();
            
            //TODO: THIS IS SOME GARBAGE CODE HERE!

            //TODO: Fix this to return an object about cluster statistics.
            while(i < count)
            {
                Dictionary<int, int> clusterToMemberCount = new Dictionary<int,int>();

                // Get a list of clusters 
                List<int> ids = new List<int>();
                for (int j = i; j < i + buffer && j < count; j++)
                {
                    clsCluster cluster = clusters[j];
                    if (cluster.MemberCount < 1)
                        continue;

                    scoreRanges.Add(cluster.Id, cluster);
                    clusterToMemberCount.Add(cluster.Id, cluster.DatasetMemberCount);
                    ids.Add(cluster.Id);
                }

                // Get their scan ranges
                List<clsUMC> umcs = featureCache.FindByClusterID(ids);
                Dictionary<int, List<double>> scans = new Dictionary<int, List<double>>();
                foreach (clsUMC umc in umcs)
                {
                    if (!scans.ContainsKey(umc.ClusterId))
                    {
                        scans.Add(umc.ClusterId, new List<double>());
                    }
                    scans[umc.ClusterId].Add(umc.Net);
                }
                foreach (int clusterID in scans.Keys)
                {
                    List<double> clusterScans = scans[clusterID];
                    clusterScans.Sort();

                    
                    double max   = clusterScans[clusterScans.Count - 1];
                    double min   = clusterScans[0];
                    double range = max - min;

                    ranges.Add(range);
                    scores.Add(scoreRanges[clusterID].MeanScore);


                    int memberCount = clusterToMemberCount[clusterID];
                    if (!scanRanges.ContainsKey(memberCount))
                    {
                        scanRanges.Add(memberCount, new List<double>());
                    }
                    scanRanges[memberCount].Add(range);
                }
                i += buffer;
            }

            return scanRanges;
        }
        /// <summary>
        /// Creates alignment plots.
        /// </summary>
        static void CreateAlignmentPlots(FeaturesAlignedEventArgs e)
        {
            string name = e.AligneeDatasetInformation.DatasetName;
            Log("Features Aligned - " + name);

            PushTextHeader("Alignment Plots for " + e.AligneeDatasetInformation.DatasetName);
            PushStartTable();

            ChartDisplayOptions options = new ChartDisplayOptions(false, true, true, true);

            options.MarginMin = 1;
            options.MarginMax = 100;
            options.Title = "NET Error Histogram " + name;
            options.XAxisLabel = "NET Error (%)";
            options.YAxisLabel = "Count";
            options.Width = m_width;
            options.Height = m_height;

            // Create the heatmap
            Image image = RenderDatasetInfo.AlignmentHeatmap_Thumbnail(e.AlignmentData, m_width, m_height);
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            string labelName = Path.GetFileNameWithoutExtension(name) + "_heatmap.png";
            string path = Path.Combine(m_plotSavePath, labelName);
            PushStartTableRow();
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            PushImageColumn(Path.Combine("Plots", labelName));

            image = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.netErrorHistogram, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_netErrorHistogram.png";
            path = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            PushImageColumn(Path.Combine("Plots", labelName));

            options.Title = "Mass Error Histogram " + name;
            options.XAxisLabel = "Mass Error (PPM)";
            image = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.massErrorHistogram, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_massErrorHistogram.png";
            path = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            PushImageColumn(Path.Combine("Plots", labelName));
            PushEndTableRow();
            PushEndTable();
        }
        /// <summary>
        /// Logs the data to the stored text file path.
        /// </summary>
        /// <param name="message"></param>
        static void Log(string message)
        {
            string newMessage = DateTime.Now.ToString() + " - " + GetMemory().ToString() + " MB - " + message;
            File.AppendAllText(m_logPath, newMessage + Environment.NewLine);
            Console.WriteLine(newMessage);
        }
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
        /// Prints the help message.
        /// </summary>
        static void PrintHelp()
        {
            PrintMessage(" ");
            PrintMessage("usage: MultiAlignConsole fileInputList paramterFile.xml analysisPath analysisName [AverageLinkage]");
            PrintMessage("   [For baseline datasets]");
            PrintMessage("   - fileInputList      = ASCII Text file with input file names.");
            PrintMessage("      In list of files use asterik to indicate the baseline choice, e.g. 'dataset *'");
            PrintMessage("   - parameterFile.xml  = XML file defining MultiAlign parameters.");
            PrintMessage("   - analysisPath       = file directory of where to put MultiAlign output.");
            PrintMessage("   - averagelinkage     = to use the average linkage algorithm or not.");
            PrintMessage(" ");
            PrintMessage("usage: MultiAlignConsole paramterFile.xml analysisPath");
            PrintMessage("   [For Mass Tag Databases]");
            PrintMessage("   - parameterFile.xml = XML file defining MultiAlign parameters.");
            PrintMessage("   - analysisPath      = file directory of where to put MultiAlign output.");
            PrintMessage(" ");
            PrintMessage("usage: MultiAlignConsole -verify databasePath netTolerance");
            PrintMessage("   [For Mass Tag Databases]");
            PrintMessage("   - databasePath      = Path of database created from a previous MA analysis run..");
            PrintMessage("   - -verify           = Command line directive to verify a mass tag database.");
            PrintMessage("   - netTolerance      = A double indicating the NET tolerance used in analysis.");
            PrintMessage(" ");
            PrintMessage("usage: MultiAlignConsole -plots databasePath");
            PrintMessage("   [To create cluster diagnostic plots on existing MA database]");
            PrintMessage("   - databasePath      = Path of database created from a previous MA analysis run..");
            PrintMessage("   - -plots            = Command line directive indicating plot creation.");            
            
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Displays the status message when the analysis completes.
        /// </summary>
        /// <param name="status"></param>
        static void DisplayStatus(int level, string status)
        {
            Log(status);            
        }
        /// <summary>
        /// Displays the title.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="title"></param>
        static void DisplayTitle(int data, string title)
        {
            Log(title);
        }
        static void VerifyClusters(double netTolerance)
        {
            Log("Validating Database.");
            Log("Loading clusters.");
            UmcClusterDAOHibernate clusterCache = new UmcClusterDAOHibernate();
            Log("Loading features.");
            UmcDAOHibernate umcCache = new UmcDAOHibernate();

            List<clsUMC> umcs = umcCache.FindAll();
            List<clsCluster> clusters = clusterCache.FindAll();

            Dictionary<int, List<clsUMC>> map = new Dictionary<int, List<clsUMC>>();


            Log("Mapping features to clusters.");
            foreach (clsUMC umc in umcs)
            {
                int key = umc.ClusterId;
                bool contains = map.ContainsKey(key);
                if (!contains)
                {
                    map.Add(key, new List<clsUMC>());
                }
                map[key].Add(umc);
            }


            Log("Validating all features clustered.");
            if (map.ContainsKey(-1))
            {
                Log("Invalid cluster.  Cluster ID was -1.");
                NHibernateUtil.Dispose();
                throw new Exception("Invalid cluster.  The ID = -1. ");                
            }

            Log("Validating cluster IDs and NETs.");
            foreach (clsCluster cluster in clusters)
            {
                bool contains = map.ContainsKey(cluster.Id);
                if (!contains)
                {
                    Log("Validation Failed.");
                    NHibernateUtil.Dispose();
                    throw new Exception("Invalid cluster ID not matching to some features.");
                }

                List<clsUMC> features = map[cluster.Id];
                if (features.Count != cluster.MemberCount)
                {
                    Log("Validation Failed.");
                    NHibernateUtil.Dispose();
                    throw new Exception("The ID's match for a cluster, but the member count does not.");
                }
                List<double> nets = new List<double>();
                foreach (clsUMC feature in features)
                {
                    nets.Add(feature.Net);

                    if (feature.ClusterId != cluster.Id)
                    {
                        Log("Validation Failed.");
                        NHibernateUtil.Dispose();
                        Log(string.Format("Cluster (ID = {0}) does not match the cluster ID stored in feature id {1}.", cluster.Id, feature.Id));
                    }
                }

                // Find garbage NETS
                nets.Sort();
                for (int i = 0; i < nets.Count - 1; i++)
                {
                    double range = nets[i + 1] - nets[i];
                    if (range > netTolerance)
                    {
                        Log(string.Format("Cluster's (ID = {0}) NET range is out of tolerance {1} > {2}.", cluster.Id, range, netTolerance));
                    }
                }
            }
            Log("Cluster Validation Passed.");
        }
        /// <summary>
        /// Terminates the application when the analysis is complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_AnalysisComplete(object sender, AnalysisCompleteEventArgs e)
        {

            Log("Saving dataset information to database.");
            DatasetDAOHibernate datasetDAOHibernate = new DatasetDAOHibernate();
            List<DatasetInformation> datasetList    = m_analysis.Datasets;
            datasetDAOHibernate.AddAll(datasetList);

            //CreateFinalAnalysisPlots();
            PushEndHeader();
            m_triggerEvent.Set();
        }
        /// <summary>
        /// Logs when features are aligned.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesAligned(object sender, FeaturesAlignedEventArgs e)
        {
            CreateAlignmentPlots(e);
        }
        /// <summary>
        /// Logs when features are clustered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesClustered(object sender, FeaturesClusteredEventArgs e)
        {
            Log("Features Clustered.");            
        }
        /// <summary>
        /// Logs when features are peak matched.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesPeakMatched(object sender, FeaturesPeakMatchedEventArgs e)
        {
            Log("Features are peak matched.");
        }
        /// <summary>
        /// Logs when features are loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesLoaded(object sender, FeaturesLoadedEventArgs e)
        {
            Log(string.Format("Loaded {0} features from {1}", e.Features.Count, e.DatasetInformation.DatasetName));
        }
        /// <summary>
        /// Logs status messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_Status(object sender, AnalysisStatusEventArgs e)
        {
            Log(e.StatusMessage);
        }
        /// <summary>
        /// Calculates the current usage of current processes memory.
        /// </summary>
        /// <returns>Memory usage of current process.</returns>
        static long GetMemory()
        {
            Process process = Process.GetCurrentProcess();
            long memory = process.WorkingSet64;
            memory /= 1024;
            memory /= 1024;
            process.Dispose();

            return memory;            
        }
        #endregion

        #region HTML 
        static void PushImageColumn(string data)
        {
            PushStartTableColumn();
            PushData(string.Format("<a href={0}><img src={0} width={1} height={2} alt={0}/></a>", data, m_widthHTML, m_heightHTML));
            PushEndTableColumn();
        }
        static void PushTextHeader(string data)
        {
            m_htmlPage.Add("<H2>" + data + "</H2>");
        }
        static void PushData(string tag)
        {
            m_htmlPage.Add(tag);
        }
        static void PushStartTable()
        {
            m_htmlPage.Add("<table>");
        }
        static void PushEndTable()
        {
            m_htmlPage.Add("</table>");
        }
        static void PushStartTableRow()
        {
            m_htmlPage.Add("<tr>");
        }
        static void PushEndTableRow()
        {
            m_htmlPage.Add("</tr>");
        }
        static void PushStartTableColumn()
        {
            m_htmlPage.Add("<td>");
        }
        static void PushEndTableColumn()
        {
            m_htmlPage.Add("</td>");
        }
        static void PushHeader()
        {
            m_htmlPage.Add("<html>");
            m_htmlPage.Add("<title>Analysis Name: " + m_analysisName + "</title>");
            m_htmlPage.Add("<h1>Analysis Name: " + m_analysisName + "</h1>"); 
        }
        static void PushEndHeader()
        {
            m_htmlPage.Add("</html>");
        }
        static void CreatePlotReport()
        {
            Log("Creating Report.");
            using (TextWriter htmlWriter = File.CreateText(Path.Combine(m_analysisPath, m_analysisName + "_plots.html")))
            {
                foreach (string tag in m_htmlPage)
                {
                    htmlWriter.WriteLine(tag);
                }
            }
        }
        #endregion

        #region Data Provider Setup
        /// <summary>
        /// Sets up the NHibernate caches for storing and retrieving data.
        /// </summary>
        /// <param name="analysisPath"></param>
        /// <returns></returns>
        private static FeatureDataAccessProviders SetupDataProviders()
        {
            string path = AnalysisPathUtils.BuildAnalysisName(m_analysisPath, m_analysisName);

            NHibernateUtil.SetDbLocationForWrite(path, true);
            NHibernateUtil.SetDbLocationForRead(path);

            IUmcDAO featureCache        = new UmcDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();

            FeatureDataAccessProviders providers =
                new FeatureDataAccessProviders(featureCache, clusterCache);

            return providers;
        }
        private static void CleanupDataProviders()
        {            
            NHibernateUtil.Dispose();
        }
        #endregion

        /// <summary>
        /// Processes the MA analysis data.
        /// </summary>
        /// <param name="args"></param>
        static void StartMultiAlign(string [] args)
        {
            // Builds the list of algorithm providers.
            AlgorithmBuilder builder = new AlgorithmBuilder();                       
            
            m_width         = PLOT_WIDTH;
            m_height        = PLOT_HEIGHT;
            m_widthHTML     = PLOT_WIDTH_HTML;
            m_heightHTML    = PLOT_HEIGHT_HTML;
            m_logPath       = null;

            if (args.Length < 1 || args[0] == "-h" || args[0] == "-help")
            {
                PrintHelp();
                return;
            }

            m_htmlPage = new List<string>();

            // Determine options.
            string fileInputList = null;
            string parameterFile = null;
            if (args.Length == 2)
            {
                if (args[0].ToLower() == "-plots")
                {

                    NHibernateUtil.SetDbLocationForRead(args[1]);

                    m_analysisName = Path.GetFileNameWithoutExtension(args[1]);
                    m_analysisPath = Path.GetDirectoryName(args[1]);


                    // Set the log path.
                    m_logPath      = AnalysisPathUtils.BuildLogPath(m_analysisPath, m_analysisName);                    
                    m_plotSavePath = AnalysisPathUtils.BuildPlotPath(Path.GetDirectoryName(args[1]));

                    PushHeader();
                    CreateFinalAnalysisPlots();
                    PushEndHeader();

                    CreatePlotReport();

                    return;
                }
            }
            else if (args.Length == 3)
            {
                if (args[0].ToLower() == "-verify")
                {
                    NHibernateUtil.SetDbLocationForRead(args[1]);
                    m_analysisName = Path.GetFileNameWithoutExtension(args[1]);
                    m_analysisPath = Path.GetDirectoryName(args[1]);


                    m_logPath       = AnalysisPathUtils.BuildLogPath(m_analysisPath, m_analysisName);
                    m_plotSavePath  = AnalysisPathUtils.BuildPlotPath(Path.GetDirectoryName(args[1]));
                    VerifyClusters(Convert.ToDouble(args[2]));
                    return;
                }
            }
            else if (args.Length >= 4)
            {
                fileInputList  = args[0];
                parameterFile  = args[1];
                m_analysisPath = args[2];
                m_analysisName = args[3];

                if (args.Length > 4)
                {
                    string algorithmType = args[4];                    
                    if (algorithmType.ToLower() == "centroid")
                    {
                        builder.BuildClusterer(ClusteringAlgorithmType.Centroid);
                    }
                    else if (algorithmType.ToLower() == "singlelinkage") 
                    {
                        builder.BuildClusterer(ClusteringAlgorithmType.SingleLinkage);
                    }
                }

                m_logPath       = AnalysisPathUtils.BuildLogPath(m_analysisPath, m_analysisName);
                m_plotSavePath  = AnalysisPathUtils.BuildPlotPath(Path.GetDirectoryName(args[1]));       
            }
            else
            {
                PrintHelp();
                return;
            }

            if (!Directory.Exists(m_analysisPath))
            {
                Directory.CreateDirectory(m_analysisPath);
            }

            Log("Starting MultiAlign Console Application.");
            Log("Creating analysis: "   + m_analysisName);
            Log("Storing analysis: "    + Path.GetFullPath(m_analysisPath));
            Log("Using Files:  "        + Path.GetFullPath(fileInputList));
            Log("Using Parameters: "    + Path.GetFullPath(parameterFile));            
            // Read the input datasets.
            if (!File.Exists(fileInputList))
            {
                Log(string.Format("The input file {0} does not exist.", fileInputList));
                return;
            }
            // Make sure we have parameters!
            if (!File.Exists(parameterFile))
            {
                Log("The parameter file does not exist.");
                return;
            }

            Log("Parsing Filenames.");
            string [] files         = File.ReadAllLines(fileInputList);
            List<string> filenames  = new List<string>();
            string baseline         = null;
            foreach(string filename in files)
            {
                string [] baselineCheck = filename.Split('*');

                if (baselineCheck.Length == 2 && baselineCheck[1] == "")
                {
                    baseline = baselineCheck[0];
                    filenames.Add(baseline);
                    Log(string.Format("Baseline dataset selected: {0}", baseline));
                }
                else
                {                    
                    filenames.Add(filename);
                }
                
            }

            Log("Creating Analysis.");
            MultiAlignAnalysis analysis             = new MultiAlignAnalysis();
            analysis.AnalysisPath                   = m_analysisPath;
            analysis.AnalysisName                   = m_analysisName;
            analysis.BaselineDatasetName                = baseline;
            m_analysis                              = analysis;
            MultiAlignAnalysisProcessor processor   = new MultiAlignAnalysisProcessor();

            Log("Creating Plot Thumbnail Path");
            // set the plot save path.
            m_plotSavePath = Path.Combine(m_analysisPath, THUMBNAIL_PATH);

            // Find out where it's located.
            if (!Directory.Exists(m_plotSavePath))
            {
                Directory.CreateDirectory(m_plotSavePath);
            }

            // Use this to signal when the analysis is done.  We are using a asnychronous call here.
            m_triggerEvent = new ManualResetEvent(false);

            // Setup the parameters.
            Log("Loading parameters.");
            analysis.LoadParametersFromFile(parameterFile);

            MultiAlignParameterIniFileWriter writer = new MultiAlignParameterIniFileWriter();            
            string outParamName = Path.GetFileNameWithoutExtension(parameterFile);
            string outParamPath = Path.Combine(Path.GetDirectoryName(parameterFile), outParamName + ".ini");
            if (outParamPath != parameterFile)
            {
                writer.WriteParametersToFile(outParamPath, analysis);
            }            

            // Create dataset information.
            int i = 0;
            Log("Creating dataset information.");

            foreach (string filename in filenames)
            {
                DatasetInformation info = new DatasetInformation();
                info.Path = filename;
                info.DatasetId = (i.ToString());
                i++;

                info.DatasetName = Path.GetFileName(filename);
                info.JobId = "";
                info.mstrResultsFolder = Path.GetDirectoryName(filename);
                info.ParameterFileName = "";
                info.Selected = true;

                Log("\tCreated dataset information for " + filename);
                analysis.Datasets.Add(info);
            }

            // Load alignment data.
            if (baseline != null)
            {
                analysis.BaselineDatasetName = baseline;
            }
            else
            {
                Log("No Baseline selected.  Please select a baseline dataset.");
                analysis.Dispose();
                return;
            }       
            processor.FeaturesAligned       += new EventHandler<FeaturesAlignedEventArgs>(processor_FeaturesAligned);
            processor.FeaturesLoaded        += new EventHandler<FeaturesLoadedEventArgs>(processor_FeaturesLoaded);
            processor.FeaturesClustered     += new EventHandler<FeaturesClusteredEventArgs>(processor_FeaturesClustered);
            processor.FeaturesPeakMatched   += new EventHandler<FeaturesPeakMatchedEventArgs>(processor_FeaturesPeakMatched);
            processor.AnalysisComplete      += new EventHandler<AnalysisCompleteEventArgs>(processor_AnalysisComplete);
            processor.Status                += new EventHandler<AnalysisStatusEventArgs>(processor_Status);

            Log("Setting up data providers for caching and storage.");
            FeatureDataAccessProviders providers = SetupDataProviders();
            analysis.DataProviders      = providers;
            processor.AlgorithmProvders = builder.GetAlgorithmProvider();

            Log("Analysis Started.");    
            processor.StartAnalysis(analysis);

            // Wait for the analysis to complete.
            WaitHandle.WaitAll(new WaitHandle[] { m_triggerEvent });
            CreatePlotReport();

            analysis.Dispose();
            processor.Dispose();
            CleanupDataProviders();
            Log("Analysis Complete.");                        
        }

        [DllImport("kernel32.dll")]
		public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
		private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
		private const int LC_DATA = 0;
		private const int IMS_DATA = 1;

		static void Main(string[] args)
		{
			IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
			SetConsoleMode(handle, ENABLE_EXTENDED_FLAGS);

            StartMultiAlign(args);
        }
    }
}
