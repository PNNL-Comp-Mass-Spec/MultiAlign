using System;
using PNNLProteomics.Data.MetaData;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using MultiAlign.Drawing;
using MultiAlignCustomControls.Charting;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;
using PNNLProteomics.Algorithms;
using PNNLProteomics.Algorithms.Clustering;
using PNNLProteomics.Data;
using PNNLProteomics.IO;
using PNNLProteomics.IO.Reports;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hConsoleHandle"></param>
        /// <param name="dwMode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        #region Constants
        private const uint ENABLE_EXTENDED_FLAGS    = 0x0080;
		private const int LC_DATA                   = 0;
		private const int IMS_DATA                  = 1;
        /// <summary>
        /// Default path for plots.
        /// </summary>
        private const string THUMBNAIL_PATH         = "Plots";        
        private const int PLOT_WIDTH                = 800; 
        private const int PLOT_HEIGHT               = 800;
        private const int PLOT_WIDTH_HTML           = 256; 
        private const int PLOT_HEIGHT_HTML          = 256;            
        #endregion

        #region Members
        /// <summary>
        /// Path to the file that specifies input data paths.
        /// </summary>
        private static string m_inputPaths;
        /// <summary>
        /// Path to parameter file.
        /// </summary>
        private static string m_parameterFile;
        /// <summary>
        /// Name of the HTML webpage.
        /// </summary>
        private static string m_htmlPathName;
        /// <summary>
        /// Holds a list of HTML tags for displaying plot results.
        /// </summary>
        private static List<string> m_htmlPage;
        /// <summary>
        /// Event that is triggered when an analysis is completed.
        /// </summary>
        private static ManualResetEvent     m_triggerEvent;
        /// <summary>
        /// Path of log file.
        /// </summary>
        private static string               m_logPath;
        /// <summary>
        /// ID of the job to use.
        /// </summary>
        private static int                  m_jobID;
        /// <summary>
        /// Reference to the analysis that is running.
        /// </summary>
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
        /// Flag to show help or not.
        /// </summary>
        private static bool m_showHelp;
        /// <summary>
        /// Options from command line argument
        /// </summary>
        private static IDictionary<string, List<string>> m_options;
        /// <summary>
        /// Creates plots on an existing database.
        /// </summary>
        private static bool m_createPlots;
        /// <summary>
        /// Path to database to create plots from.
        /// </summary>
        private static string   m_databaseName;
        /// <summary>
        /// Objects that access data from the databases.
        /// </summary>
        private static FeatureDataAccessProviders m_dataProviders;
        /// <summary>
        /// Flag that indicates whether a plot for the baseline features has been made.
        /// </summary>
        static bool m_createdBaselinePlots;
        /// <summary>
        /// Object that can generate an HTML report.
        /// </summary>
        private static AnalysisHTMLReport m_report;
        #endregion
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        static Program()
        {            
            m_width                 = PLOT_WIDTH;
            m_height                = PLOT_HEIGHT;
            m_logPath               = null;
            m_inputPaths            = null;
            m_parameterFile         = null;
            m_htmlPathName          = "index.html";
            m_showHelp              = false;
            m_createdBaselinePlots  = false;
            m_htmlPage              = new List<string>();
            m_analysisPath          = null;
            m_analysisName          = null;
            m_databaseName          = null;
            m_createPlots           = false;
            m_report                = new AnalysisHTMLReport();
            m_report.ImageWidth     = PLOT_WIDTH_HTML;
            m_report.ImageHeight    = PLOT_HEIGHT_HTML;
        }

        #region Plot Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="name"></param>
        static void SaveImage(Image image, string name)
        {
            if (image != null)
            {                
                string path = Path.Combine(m_plotSavePath, name);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);                
            }
            else
            {
                PrintMessage(string.Format("Could not create {0} plot.", name));
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
        /// Creates the final analysis plots
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="clusterCache"></param>
        static void CreateFinalAnalysisPlots()
        {
            UmcDAOHibernate cache               = new UmcDAOHibernate();
            UmcClusterDAOHibernate clusterCache = new UmcClusterDAOHibernate();

            CreateFinalAnalysisPlots(cache, clusterCache);
        }
        /// <summary>
        /// Creates the final analysis plots.
        /// </summary>
        static void CreateFinalAnalysisPlots(IUmcDAO cache, IUmcClusterDAO clusterCache)
        {
            PrintMessage("Creating Final Plots");
            m_report.PushTextHeader("Analysis Info ");
            m_report.PushStartTable();
            m_report.PushStartTableRow();

            // Create the heatmap
            List<clsUMC> umcs           = cache.FindAll();
            ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true,
                                                1, 100,
                                                "Charge State Histogram", "Charge State", "Count", m_width, m_height);
            options.DisplayLegend   = false;
            Image image             = RenderDatasetInfo.ChargeStateHistogram_Thumbnail(umcs, m_width, m_height, options);
            SaveImage(image, "ChargeStates.png");
            m_report.PushImageColumn(Path.Combine("Plots", "ChargeStates.png"));

            m_report.PushEndTableRow();
            m_report.PushEndTable();

            m_report.PushTextHeader("Cluster Data");
            m_report.PushStartTable();
            m_report.PushStartTableRow();

            List<clsCluster> clusters   = clusterCache.FindAll();            
            options.Title               = "Cluster Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";
            options.DisplayLegend       = false;
            image                       = RenderDatasetInfo.ClusterSizeHistogram_Thumbnail(clusters, m_width, m_height, options);
            SaveImage(image, "ClusterMemberSizes.png");
            m_report.PushImageColumn(Path.Combine("Plots", "ClusterMemberSizes.png"));
            
            options.Title               = "Cluster Dataset Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";
            image                       = RenderDatasetInfo.ClusterDatasetMemberSizeHistogram_Thumbnail(clusters, m_width, m_height, options);
            SaveImage(image, "ClusterDatasetMemberSizes.png");
            m_report.PushImageColumn(Path.Combine("Plots", "ClusterDatasetMemberSizes.png"));

            // Mass vs. Cluster score 
            options.Title               = "Clusters";
            options.YAxisLabel          = "Cluster Monoisotopic Mass";
            options.XAxisLabel          = "Cluster NET";
            image                       = RenderDatasetInfo.ClusterScatterPlot_Thumbnail(clusters, options);
            SaveImage(image, "ClusterScatterPlot.png");
            m_report.PushImageColumn(Path.Combine(THUMBNAIL_PATH, "ClusterScatterPlot.png"));
            m_report.PushEndTableRow();
            m_report.PushStartTableRow();
            m_report.PushEndTable();


            m_report.PushStartTable(true);
            m_report.PushStartTableRow();
            m_report.PushStartTableColumn();
            m_report.PushData("Dataset Members");
            m_report.PushEndTableColumn();

            m_report.PushStartTableColumn();
            m_report.PushData("Count");
            m_report.PushEndTableColumn();
            m_report.PushEndTableRow();

            float[] histogram = RenderDatasetInfo.GetClusterMemberSizes(clusters);
            if (histogram != null)
            {
                for (int i = 0; i < histogram.Length; i++)
                {
                    m_report.PushStartTableRow();
                    m_report.PushStartTableColumn();
                    m_report.PushData(i.ToString());
                    m_report.PushEndTableColumn();

                    m_report.PushStartTableColumn();
                    m_report.PushData(histogram[i].ToString());
                    m_report.PushEndTableColumn();
                    m_report.PushEndTableRow();
                }
            }
            m_report.PushEndTable();
        }
        /// <summary>
        /// Creates the plots post analysis.
        /// </summary>
        /// <param name="providers"></param>
        static void CreatePlotsOffline(FeatureDataAccessProviders providers)
        {
            PrintMessage("Connecting to existing database: " + m_databaseName);
            try
            {
                bool exists = File.Exists(m_databaseName);
                if (!exists)
                {
                    PrintMessage("The database you specified does not exist.");
                    return;
                }

                providers = SetupDataProviders(m_databaseName, false);
            }
            catch (System.IO.IOException ex)
            {
                PrintMessage("Could not connect to database: " + ex.Message);
                return;
            }

            PrintMessage("Creating Plot Thumbnail Path");
            // set the plot save path.
            string directoryName = Path.GetDirectoryName(m_databaseName);

            m_htmlPathName = Path.Combine(directoryName, m_htmlPathName);
            m_plotSavePath = Path.Combine(directoryName, THUMBNAIL_PATH);

            // Find out where it's located.
            if (!Directory.Exists(m_plotSavePath))
            {
                Directory.CreateDirectory(m_plotSavePath);
            }

            // We have to create a header for the HTML file.
            m_report.PushHeader();
            CreateFinalAnalysisPlots(providers.FeatureCache, providers.ClusterCache);
            m_report.PushEndHeader();
            CreatePlotReport();
            return;
        }
        /// <summary>
        /// Creates alignment plots.
        /// </summary>
        static void CreateAlignmentPlots(FeaturesAlignedEventArgs e)
        {
            string name = e.AligneeDatasetInformation.DatasetName;
            PrintMessage("Features Aligned - " + name);
                
            // Hack so that the baseline plot is made first.
            if (!m_createdBaselinePlots)
            {                
                m_createdBaselinePlots  = true;

                // This may not be necessary...
                int baselineIndex       = -1;
                int index               = 0;
                foreach (DatasetInformation info in m_analysis.MetaData.Datasets)
                {
                    if (info.DatasetName == m_analysis.BaselineDatasetName)
                    {
                        baselineIndex = index;
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }
                if (baselineIndex >= 0)
                {
                    m_report.PushTextHeader("Baseline Dataset for " + m_analysis.BaselineDatasetName);
                    m_report.PushStartTable();
                    m_report.PushStartTableRow();

                    DatasetInformation baselineInfo     = m_analysis.MetaData.Datasets[baselineIndex];
                    ChartDisplayOptions baselineOptions = new ChartDisplayOptions(false, true, true, true);
                    baselineOptions.MarginMin       = 1;
                    baselineOptions.MarginMax       = 100;
                    baselineOptions.Title           = "Feature Plot " + baselineInfo.DatasetName;
                    baselineOptions.XAxisLabel      = "Scan";
                    baselineOptions.YAxisLabel      = "Monoisotopic Mass";
                    baselineOptions.Width           = m_width;
                    baselineOptions.Height          = m_height;
                    baselineOptions.DisplayLegend   = true;
                    List<clsUMC> baselineUmcs       = m_dataProviders.FeatureCache.FindByDatasetId(Convert.ToInt32(baselineInfo.DatasetId));
                    Image baselineImage             = RenderDatasetInfo.FeaturesScatterPlot_Thumbnail(baselineUmcs, baselineOptions);
                    string baselineLabelName        = Path.GetFileNameWithoutExtension(baselineInfo.DatasetName) + "_featurePlot.png";
                    string baselinePath             = Path.Combine(m_plotSavePath, baselineLabelName);
                    baselineImage.Save(baselinePath, System.Drawing.Imaging.ImageFormat.Png);
                    m_report.PushImageColumn(Path.Combine("Plots", baselineLabelName));
                    m_report.PushEndTableRow();
                    m_report.PushEndTable();
                }
            }

            m_report.PushTextHeader("Alignment Plots for " + e.AligneeDatasetInformation.DatasetName);
            m_report.PushStartTable();
            m_report.PushStartTableRow();
            ChartDisplayOptions options = new ChartDisplayOptions(false, true, true, true);

            options.MarginMin       = 1;
            options.MarginMax       = 100;
            options.Title           = "Feature Plot " + name;
            options.XAxisLabel      = "Scan";
            options.YAxisLabel      = "Monoisotopic Mass";
            options.Width           = m_width;
            options.Height          = m_height;
            options.DisplayLegend   = true;
            
            List<clsUMC> umcs       = m_dataProviders.FeatureCache.FindByDatasetId(Convert.ToInt32(e.AligneeDatasetInformation.DatasetId));
            Image image             = RenderDatasetInfo.FeaturesScatterPlot_Thumbnail(umcs, options);
            string labelName        = Path.GetFileNameWithoutExtension(name) + "_featurePlot.png";
            string path             = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));

            options.MarginMin       = 1;
            options.MarginMax       = 100;
            options.Title           = "Alignment Heatmap " + name;
            options.XAxisLabel      = "Baseline";
            options.YAxisLabel      = "Alignee";
            options.Width           = m_width;
            options.Height          = m_height;
            
            image                   = RenderDatasetInfo.AlignmentHeatmap_Thumbnail(e.AlignmentData, m_width, m_height);
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            labelName               = Path.GetFileNameWithoutExtension(name) + "_heatmap.png";
            path                    = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));

            m_report.PushStartTableRow();
            options.DisplayLegend   = false;
            options.Title           = "NET Error Histogram " + name;
            options.XAxisLabel      = "NET Error (%)";
            options.YAxisLabel      = "Count";
            image                   = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.netErrorHistogram, options);
            labelName               = Path.GetFileNameWithoutExtension(name) + "_netErrorHistogram.png";
            path                    = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayGridLines = true;
            options.DisplayLegend = false;
            options.Title = "Net vs. Scan Residuals" + name;
            image = RenderDatasetInfo.NETResiduals_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_netResiduals.png";
            path = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));
            
            options.Title           = "Mass Error Histogram " + name;
            options.XAxisLabel      = "Mass Error (PPM)";
            image                   = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.massErrorHistogram, options);
            labelName               = Path.GetFileNameWithoutExtension(name) + "_massErrorHistogram.png";
            path                    = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayLegend   = true;
            options.Title           = "Mass vs. Scan Residuals" + name;
            image                   = RenderDatasetInfo.MassVsScanResiduals_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName               = Path.GetFileNameWithoutExtension(name) + "_massScanResiduals.png";
            path                    = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayLegend   = true;
            options.Title           = "Mass vs. m/z Residuals" + name;
            image                   = RenderDatasetInfo.ClusterMassVsMZResidual_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName               = Path.GetFileNameWithoutExtension(name) + "_massMZResiduals.png";
            path                    = Path.Combine(m_plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            m_report.PushImageColumn(Path.Combine("Plots", labelName));
            m_report.PushEndTableRow();

            if (e.DriftTimeAlignmentData != null)
            {
                
                options.DisplayLegend   = false;
                options.Title           = "Drift Time Plot";
                options.XAxisLabel      = "Baseline Drift Times (ms)";
                options.YAxisLabel      = "Alignee Drift Times (ms)";

                List<FeatureMatch<UMC, UMC>> matches = e.DriftTimeAlignmentData.Matches;
                int totalMatches                     = matches.Count;
                float[] x                            = new float[totalMatches];
                float[] yC                           = new float[totalMatches];
                float[] y                            = new float[totalMatches];

                int i = 0;
                foreach (FeatureMatch<UMC, UMC> match in matches)
                {
                    y[i]    = Convert.ToSingle(match.ObservedFeature.DriftTime);
                    yC[i]   = Convert.ToSingle(match.ObservedFeature.DriftTimeAligned);
                    x[i]    = Convert.ToSingle(match.TargetFeature.DriftTime);
                    i++;
                }
                labelName   = Path.GetFileNameWithoutExtension(name) + "_driftTimes.png";
                path        = Path.Combine(m_plotSavePath, labelName);
                image       = GenericPlotFactory.ScatterPlot_Thumbnail(x, y, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                m_report.PushImageColumn(Path.Combine("Plots", labelName));
                
                options.Title   = "Aligned Drift Time Plot";
                labelName       = Path.GetFileNameWithoutExtension(name) + "_driftTimesAligned.png";
                path            = Path.Combine(m_plotSavePath, labelName);
                image           = GenericPlotFactory.ScatterPlot_Thumbnail(x, yC, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                m_report.PushImageColumn(Path.Combine("Plots", labelName));
                
                options.Title   = "Drift Time Error Distributions";
                labelName       = Path.GetFileNameWithoutExtension(name) + "_driftTimesErrorHistogram.png";
                path            = Path.Combine(m_plotSavePath, labelName);
                image           = GenericPlotFactory.ResidualHistogram_Thumbnail(x, y, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                m_report.PushImageColumn(Path.Combine("Plots", labelName));

                options.Title   = "Aligned Drift Time Error Distributions";
                labelName       = Path.GetFileNameWithoutExtension(name) + "_driftTimesErrorHistogramAligned.png";
                path            = Path.Combine(m_plotSavePath, labelName);
                image           = GenericPlotFactory.ResidualHistogram_Thumbnail(x, yC, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                m_report.PushImageColumn(Path.Combine("Plots", labelName));
                m_report.PushEndTableRow();
            }

            m_report.PushEndTable();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Validates the input options to make sure everything is set.
        /// </summary>
        /// <returns></returns>
        private static bool ValidateSetup()
        {
            bool validated = true;
            if (m_inputPaths == null)
            {
                PrintMessage("No input file provided.");
                validated = false;
            }
            if (m_parameterFile == null)
            {
                PrintMessage("No parameter file specified.");
                validated = false;
            }
            if (m_analysisName == null)
            {
                PrintMessage("No analysis name provided.");
                validated = false;
            }
            if (m_analysisPath == null)
            {
                PrintMessage("No analysis path provided.");
                validated = false;
            }
            return validated;
        }
       
        /// <summary>
        /// Processes the command line arguments.
        /// </summary>
        /// <param name="args"></param>
        static void ProcessCommandLineArguments(string[] args)
        {
            int jobID   = -1;
            bool worked = false;
            m_options   = CommandLineParser.ProcessArgs(args, 0);
            foreach (string option in m_options.Keys)
            {                
                try
                {
                    List<string> values = m_options[option];
                    switch (option)
                    {
                        case "-job":
                            worked = int.TryParse(values[0], out jobID);
                            if (worked)
                            {
                                m_jobID = jobID;
                            }
                            else
                            {
                                PrintMessage(string.Format("The Job ID {0} specified could not be understood.",
                                                            values[0]),
                                                            true);
                            }
                            break;
                        case "-path":
                            m_analysisPath  = values[0];
                            break;
                        case "-files":
                            m_inputPaths    = values[0];
                            break;
                        case "-params":
                            m_parameterFile = values[0];
                            break;
                        case "-name":
                            m_analysisName  = values[0];                            
                            break;
                        case "-log":
                            m_logPath       = values[0];
                            break;
                        case "-html":
                            m_htmlPathName  = values[0];
                            break;
                        case "-h":
                            m_showHelp      = true;
                            break;
                        case "-help":
                            m_showHelp      = true;
                            break;
                        case "-plots":
                            m_createPlots   = true;
                            if (values.Count > 0)
                            {
                                m_databaseName = values[0];
                            }
                            else
                            {
                                m_databaseName = null;
                            }
                            break;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    PrintMessage(string.Format("You did not provide enough information for the option {0}", option));
                    return;
                }
                
            }
        }
        #endregion

        #region Processor Events
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_AnalysisError(object sender, AnalysisErrorEventArgs e)
        {
            PrintMessage(string.Format("There was an error while performing the analysis.  {0} : {1}", e.ErrorMessage, e.Exception.Message));
            m_triggerEvent.Set();
        }
        #endregion

        #region Printing
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        /// <param name="message"></param>
        static void PrintMessage(string message)
        {
            PrintMessage(message, true);
        }
        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        /// <param name="message"></param>
        static void PrintMessage(string message, bool useMemory)
        {
            string newMessage = message;
            if (useMemory)
            {
                newMessage = DateTime.Now.ToString() + " - " + GetMemory().ToString() + " MB - " + newMessage;
            }
            if (m_logPath != null)
            {
                File.AppendAllText(m_logPath, newMessage + Environment.NewLine);
            }
            Console.WriteLine(newMessage);
        }
        /// <summary>
        /// Prints the help message.
        /// </summary>
        static void PrintHelp()
        {
            PrintMessage(" ", false);
            PrintMessage("usage: MultiAlignConsole [options]", false);
            PrintMessage(" ", false);
            PrintMessage("[Options]", false);
            PrintMessage(" ", false);
            PrintMessage("   -files  inputFile.txt ", false);
            PrintMessage("          ASCII Text file with input file names.", false);
            PrintMessage("          In list of files use asterik to indicate the baseline choice, e.g. 'dataset *'", false);
            PrintMessage("   -name analysisName  ", false);
            PrintMessage("          Name to give analysis.", false);
            PrintMessage("   -log logPath.txt", false);
            PrintMessage("          Path to provide for log files.", false);
            PrintMessage("   -h", false);
            PrintMessage("          Prints this help message.", false);
            PrintMessage("   -help", false);
            PrintMessage("          Prints this help message.", false);
            PrintMessage("   -html htmlPathName.html", false);
            PrintMessage("          Name to give output HTML plot file.", false);
            PrintMessage("   -params parameterFile.xml  ", false);
            PrintMessage("          XML file defining MultiAlign parameters.", false);
            PrintMessage("   -path  AnalysisPath      ", false);
            PrintMessage("          File directory of where to put MultiAlign output.  Can be relative or absolute.", false);
            PrintMessage("   -centroid      ", false);
            PrintMessage("          To use centroid distance as clustering algorithm.", false);
            PrintMessage("   -plots   [databaseName]  ", false);
            PrintMessage("          Creates plots for final analysis.  If [databaseName] specified when not running analysis, this will create plots post-analysis.", false);
        }
        /// <summary>
        /// Prints the version of MA to the log file.
        /// </summary>
        public static void PrintVersion()
        {
            PrintMessage("[VersionInfo]");
            // get the version object for this assembly
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName name = assembly.GetName();
            Version version = name.Version;
            PrintMessage(string.Format("{0} - version {1}", name, version));

            AppDomain MyDomain = AppDomain.CurrentDomain;
            Assembly[] AssembliesLoaded = MyDomain.GetAssemblies();

            PrintMessage("Loaded Assemblies");
            foreach (Assembly subAssembly in AssembliesLoaded)
            {
                AssemblyName subName = subAssembly.GetName();
                if (!subName.Equals(name))
                {
                    PrintMessage(string.Format("\t{0} - version {1}",
                                                                    subName,
                                                                    subName.Version));
                }
            }
            PrintMessage("");
            PrintMessage("[LogStart]");
        }
        #endregion 

        #region Event Handlers
        /// <summary>
        /// Displays the status message when the analysis completes.
        /// </summary>
        /// <param name="status"></param>
        static void DisplayStatus(int level, string status)
        {
            PrintMessage(status);            
        }
        /// <summary>
        /// Displays the title.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="title"></param>
        static void DisplayTitle(int data, string title)
        {
            PrintMessage(title);
        }
        static void VerifyClusters(double netTolerance)
        {
            PrintMessage("Validating Database.");
            PrintMessage("Loading clusters.");
            UmcClusterDAOHibernate clusterCache = new UmcClusterDAOHibernate();
            PrintMessage("Loading features.");
            UmcDAOHibernate umcCache = new UmcDAOHibernate();

            List<clsUMC> umcs = umcCache.FindAll();
            List<clsCluster> clusters = clusterCache.FindAll();

            Dictionary<int, List<clsUMC>> map = new Dictionary<int, List<clsUMC>>();


            PrintMessage("Mapping features to clusters.");
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


            PrintMessage("Validating all features clustered.");
            if (map.ContainsKey(-1))
            {
                PrintMessage("Invalid cluster.  Cluster ID was -1.");
                NHibernateUtil.Dispose();
                throw new Exception("Invalid cluster.  The ID = -1. ");                
            }

            PrintMessage("Validating cluster IDs and NETs.");
            foreach (clsCluster cluster in clusters)
            {
                bool contains = map.ContainsKey(cluster.Id);
                if (!contains)
                {
                    PrintMessage("Validation Failed.");
                    NHibernateUtil.Dispose();
                    throw new Exception("Invalid cluster ID not matching to some features.");
                }

                List<clsUMC> features = map[cluster.Id];
                if (features.Count != cluster.MemberCount)
                {
                    PrintMessage("Validation Failed.");
                    NHibernateUtil.Dispose();
                    throw new Exception("The ID's match for a cluster, but the member count does not.");
                }
                List<double> nets = new List<double>();
                foreach (clsUMC feature in features)
                {
                    nets.Add(feature.Net);

                    if (feature.ClusterId != cluster.Id)
                    {
                        PrintMessage("Validation Failed.");
                        NHibernateUtil.Dispose();
                        PrintMessage(string.Format("Cluster (ID = {0}) does not match the cluster ID stored in feature id {1}.", cluster.Id, feature.Id));
                    }
                }

                // Find garbage NETS
                nets.Sort();
                for (int i = 0; i < nets.Count - 1; i++)
                {
                    double range = nets[i + 1] - nets[i];
                    if (range > netTolerance)
                    {
                        PrintMessage(string.Format("Cluster's (ID = {0}) NET range is out of tolerance {1} > {2}.", cluster.Id, range, netTolerance));
                    }
                }
            }
            PrintMessage("Cluster Validation Passed.");
        }
        /// <summary>
        /// Terminates the application when the analysis is complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_AnalysisComplete(object sender, AnalysisCompleteEventArgs e)
        {

            PrintMessage("Saving dataset information to database.");
            DatasetDAOHibernate datasetDAOHibernate = new DatasetDAOHibernate();
            List<DatasetInformation> datasetList    = m_analysis.MetaData.Datasets;
            datasetDAOHibernate.AddAll(datasetList);
            m_report.PushEndHeader();
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
            PrintMessage("Features Clustered.");            
        }
        /// <summary>
        /// Logs when features are peak matched.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesPeakMatched(object sender, FeaturesPeakMatchedEventArgs e)
        {
            PrintMessage("Features are peak matched.");
        }
        /// <summary>
        /// Logs when features are loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_FeaturesLoaded(object sender, FeaturesLoadedEventArgs e)
        {
            PrintMessage(string.Format("Loaded {0} features from {1}", e.Features.Count, e.DatasetInformation.DatasetName));
        }
        /// <summary>
        /// Logs status messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void processor_Status(object sender, AnalysisStatusEventArgs e)
        {
            PrintMessage(e.StatusMessage);
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

        /// <summary>
        /// Creates the HTML output file.
        /// </summary>
        static void CreatePlotReport()
        {
            PrintMessage("Creating Report.");            
            string htmlPath = m_htmlPathName;
            if (m_analysisPath != null)
            {
                htmlPath = Path.Combine(m_analysisPath, m_htmlPathName);
            }
            m_report.AnalysisName = m_analysisName;
            m_report.CreateReport(htmlPath);
        }        
        
        #region Data Provider Setup
        /// <summary>
        /// Sets up the NHibernate caches for storing and retrieving data.
        /// </summary>
        /// <param name="analysisPath"></param>
        /// <returns></returns>
        private static FeatureDataAccessProviders SetupDataProviders(string path, bool createNew)
        {
            try
            {
                bool exists = File.Exists(path);
                if (exists && createNew)
                {
                    File.Delete(path);
                }

                NHibernateUtil.ConnectToDatabase(path, createNew);
                IUmcDAO featureCache        = new UmcDAOHibernate();
                IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();

                FeatureDataAccessProviders providers =
                    new FeatureDataAccessProviders(featureCache, clusterCache);

                return providers;
            }
            catch (System.IO.IOException ex)
            {
                PrintMessage("Could not access the database.  Is it opened somewhere else?");
                throw ex;
            }
        }
        /// <summary>
        /// Creates data providers to the database of the analysis name and path provided.
        /// </summary>
        /// <returns></returns>
        private static FeatureDataAccessProviders SetupDataProviders()
        {
            string path = AnalysisPathUtils.BuildAnalysisName(m_analysisPath, m_analysisName);
            return SetupDataProviders(path, true);
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
        /// 
        static void StartMultiAlign()
        {
            // Builds the list of algorithm providers.
            AlgorithmBuilder builder                = new AlgorithmBuilder();
            FeatureDataAccessProviders providers    = null;                   
            
            // See if the user wants help
            if (m_showHelp)
            {
                PrintHelp();
                return;
            }

            // Then validate the input.
            bool validated  = ValidateSetup();
            if (!validated)
            {
                if (m_createPlots && m_databaseName != null)
                {
                    PrintVersion();
                    CreatePlotsOffline(providers);
                }
                else
                {
                    PrintHelp();
                }
                return;
            }

            //Create the analysis directory.
            if (!Directory.Exists(m_analysisPath))
            {
                PrintMessage("Creating analysis path " + m_analysisPath);
                Directory.CreateDirectory(m_analysisPath);
            }
            else
            {
                PrintMessage("Analysis path " + m_analysisPath + " already exists.");
            }
            
            // Create the LOG FILE.
            if (m_logPath == null)  m_logPath = AnalysisPathUtils.BuildLogPath(m_analysisPath, m_analysisName);            
            else                    m_logPath = Path.Combine(m_analysisPath, m_logPath);


            PrintVersion();
            m_plotSavePath  = AnalysisPathUtils.BuildPlotPath(m_analysisPath);
                                                            
            bool containsExtensionDB3 = m_analysisName.EndsWith(".db3");
            if (!containsExtensionDB3)
            {
                m_analysisName += ".db3";
            }

            // Setup algorithm providers.
            if (m_options.ContainsKey("-centroid"))
            {
                PrintMessage("Building centroid clusterer");
                builder.BuildClusterer(ClusteringAlgorithmType.Centroid);
            }
            else if(m_options.ContainsKey("-singlelinkage"))
            {
            
                PrintMessage("Building single linkage clusterer");
                builder.BuildClusterer(ClusteringAlgorithmType.SingleLinkage);
            }
            else
            {
                PrintMessage("Built average linkage clusterer.");                
            }

            // create application and analysis.
            PrintMessage("Starting MultiAlign Console Application.");
            PrintMessage("Creating analysis: "  + m_analysisName);
            PrintMessage("Storing analysis: "   + Path.GetFullPath(m_analysisPath));
            PrintMessage("Using Files:  "       + Path.GetFullPath(m_inputPaths));
            PrintMessage("Using Parameters: "   + Path.GetFullPath(m_parameterFile));            
            // Read the input datasets.
            if (!File.Exists(m_inputPaths))
            {
                PrintMessage(string.Format("The input file {0} does not exist.", m_inputPaths));
                return;
            }
            // Make sure we have parameters!
            if (!File.Exists(m_parameterFile))
            {
                PrintMessage("The parameter file does not exist.");
                return;
            }

            PrintMessage("Parsing Input Filenames and Databases.");
            InputAnalysisInfo analysisSetupInformation = null;

            try
            {
                analysisSetupInformation = MultiAlignFileInputReader.ReadInputFile(m_inputPaths);
            }
            catch(Exception ex)
            {
                PrintMessage("The input file had some bad lines in it.  " + ex.Message);
                return;
            }
            PrintMessage("Found " + analysisSetupInformation.Files.Count.ToString() + " files.");
            
            // Validate the mass tag database settings.            
            bool useMTDB = false;
            try
            {
                useMTDB = analysisSetupInformation.Database.ValidateDatabaseType();
            }
            catch (AnalysisMTDBSetupException ex)
            {
                PrintMessage("There was a problem with the mass tag database specification.  " + ex.Message);
                return;
            }


            PrintMessage("Creating Analysis Objects.");
            MultiAlignAnalysis analysis             = new MultiAlignAnalysis();
            analysis.MetaData.AnalysisPath          = m_analysisPath;
            analysis.MetaData.AnalysisName          = m_analysisName;
            analysis.UseMassTagDBAsBaseline         = true;
            analysis.MetaData.ParameterFile         = m_parameterFile;
            analysis.MetaData.InputFileDefinition   = m_inputPaths;        
            analysis.MetaData.AnalysisSetupInfo     = analysisSetupInformation;
            m_analysis                              = analysis;
            MultiAlignAnalysisProcessor processor   = new MultiAlignAnalysisProcessor();

            PrintMessage("Creating Plot Thumbnail Path");
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
            PrintMessage("Loading parameters.");
            PNNLProteomics.IO.XMLParamterFileReader reader = new PNNLProteomics.IO.XMLParamterFileReader();
            reader.ReadParameterFile(m_parameterFile, ref m_analysis);            
            
            if (useMTDB)
            {
                switch (analysisSetupInformation.Database.DatabaseFormat)
                {
                    case PNNLProteomics.Data.MassTags.MassTagDatabaseFormat.Access:
                        PrintMessage(string.Format("Using local Mass Tag Database at location: {0}",
                                                    analysisSetupInformation.Database.LocalPath));

                        m_analysis.MassTagDBOptions.mstr_databaseFilePath   = analysisSetupInformation.Database.LocalPath;
                        m_analysis.MassTagDBOptions.mstrServer              = analysisSetupInformation.Database.DatabaseServer;
                        m_analysis.MassTagDBOptions.menm_databaseType       = MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS;

                        break;
                    case PNNLProteomics.Data.MassTags.MassTagDatabaseFormat.SQL:
                        PrintMessage(string.Format("Using Mass Tag Database {0} on server: {1} ",
                                                    analysisSetupInformation.Database.DatabaseName,
                                                    analysisSetupInformation.Database.DatabaseServer));
                        m_analysis.MassTagDBOptions.mstrDatabase        = analysisSetupInformation.Database.DatabaseName;
                        m_analysis.MassTagDBOptions.mstrServer          = analysisSetupInformation.Database.DatabaseServer;
                        m_analysis.MassTagDBOptions.menm_databaseType   = MultiAlignEngine.MassTags.MassTagDatabaseType.SQL;

                        break;
                }
                                            
                // Validate the baseline
                if (analysisSetupInformation.BaselineFile == null)
                {
                    m_analysis.UseMassTagDBAsBaseline = true;
                    PrintMessage(string.Format("Using mass tag database {0} as the alignment baseline.", analysisSetupInformation.Database.DatabaseName));
                }
                else
                {
                    m_analysis.UseMassTagDBAsBaseline   = false;                    
                    string baselineDataset              = Path.GetFileName(analysisSetupInformation.BaselineFile.Path);
                    m_analysis.BaselineDatasetName      = baselineDataset;
                    PrintMessage(string.Format("Using dataset {0} as the alignment baseline.", baselineDataset));
                }
            }
            else
            {
                m_analysis.MassTagDBOptions.menm_databaseType = MultiAlignEngine.MassTags.MassTagDatabaseType.None;
                m_analysis.UseMassTagDBAsBaseline = false;
                // Validate the baseline
                if (analysisSetupInformation.BaselineFile == null)
                {
                    PrintMessage("No baseline dataset or database was selected.");
                    return;
                }

                string baselineDataset = Path.GetFileName(analysisSetupInformation.BaselineFile.Path);
                m_analysis.BaselineDatasetName = baselineDataset;
                PrintMessage(string.Format("Using dataset {0} as the alignment baseline.", baselineDataset));
            }

            // Output the settings to INI for viewing.
            string outParamName                 = Path.GetFileNameWithoutExtension(m_parameterFile);
            string outParamPath                 = Path.Combine(m_analysisPath, outParamName);            
            XMLParameterFileWriter xmlWriter    = new XMLParameterFileWriter();                
            xmlWriter.WriteParameterFile(outParamPath + ".xml", m_analysis);     

            // Create dataset information.
            int i = 0;
            PrintMessage("Creating dataset and other input information.");

            foreach (InputFile file in analysisSetupInformation.Files)
            {
                switch(file.FileType)
                {
                    case InputFileType.Features:
                        DatasetInformation datasetInfo  = new DatasetInformation();
                        datasetInfo.Path                = file.Path;
                        datasetInfo.DatasetId           = (i.ToString());
                        i++;
                        datasetInfo.DatasetName         = Path.GetFileName(file.Path);
                        datasetInfo.JobId               = "";
                        datasetInfo.mstrResultsFolder   = Path.GetDirectoryName(file.Path);
                        datasetInfo.ParameterFileName   = "";
                        datasetInfo.Selected            = true;
                        PrintMessage("Created dataset information for " + file.Path);
                        analysis.MetaData.Datasets.Add(datasetInfo);
                        break;
                    case InputFileType.Scans:                        
                        analysis.MetaData.OtherFiles.Add(file);
                        PrintMessage("Created scan level information for " + file.Path);
                        break;
                    case InputFileType.Raw:
                        analysis.MetaData.OtherFiles.Add(file);
                        PrintMessage("Created raw level information for " + file.Path);
                        break;
                }               
            }

            processor.AnalysisError         += new EventHandler<AnalysisErrorEventArgs>(processor_AnalysisError);
            processor.FeaturesAligned       += new EventHandler<FeaturesAlignedEventArgs>(processor_FeaturesAligned);
            processor.FeaturesLoaded        += new EventHandler<FeaturesLoadedEventArgs>(processor_FeaturesLoaded);
            processor.FeaturesClustered     += new EventHandler<FeaturesClusteredEventArgs>(processor_FeaturesClustered);
            processor.FeaturesPeakMatched   += new EventHandler<FeaturesPeakMatchedEventArgs>(processor_FeaturesPeakMatched);
            processor.AnalysisComplete      += new EventHandler<AnalysisCompleteEventArgs>(processor_AnalysisComplete);
            processor.Status                += new EventHandler<AnalysisStatusEventArgs>(processor_Status);

            PrintMessage("Setting up data providers for caching and storage.");            
            try
            {
                providers = SetupDataProviders();
            }
            catch(System.IO.IOException)
            {
                return;
            }

            analysis.DataProviders      = providers;
            m_dataProviders             = providers;
            processor.AlgorithmProvders = builder.GetAlgorithmProvider();

            PrintMessage("Analysis Started.");    
            processor.StartAnalysis(analysis);
            // Wait for the analysis to complete.
            WaitHandle.WaitAll(new WaitHandle[] { m_triggerEvent });

            
            CreateFinalAnalysisPlots(providers.FeatureCache, providers.ClusterCache);            
            CreatePlotReport();

            analysis.Dispose();
            processor.Dispose();
            CleanupDataProviders();
            PrintMessage("Analysis Complete.");                        
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
		static void Main(string[] args)
		{
			IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
			SetConsoleMode(handle, ENABLE_EXTENDED_FLAGS);
            ProcessCommandLineArguments(args);
            StartMultiAlign();
        }
    }
}
